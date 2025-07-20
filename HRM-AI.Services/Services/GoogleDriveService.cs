using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

public class GoogleDriveService
{
    private readonly DriveService _drive;
    private readonly string _sharedDriveId;
    private readonly string _uploadFolderId;

    public GoogleDriveService(IConfiguration config)
    {
        _sharedDriveId = config["GoogleDrive:SharedDriveId"]!;
        _uploadFolderId = config["GoogleDrive:UploadFolderId"]!;

        var credential = GoogleCredential
            .FromFile(config["GoogleDrive:ServiceAccountKeyFile"]!)
            .CreateScoped(DriveService.ScopeConstants.DriveFile);

        _drive = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "HRM_AI"
        });
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string mimeType)
    {
        var fileMeta = new Google.Apis.Drive.v3.Data.File
        {
            Name = fileName,
            Parents = new[] { _uploadFolderId } // folder con trong Shared Drive
        };

        var req = _drive.Files.Create(fileMeta, stream, mimeType);
        // quan trọng: bật chế độ hỗ trợ Shared Drive
        req.SupportsAllDrives = true;
        req.Fields = "id, webViewLink";

        var result = await req.UploadAsync();
        if (result.Status != UploadStatus.Completed)
            throw result.Exception!;

        return req.ResponseBody!.WebViewLink!;

    }
}
