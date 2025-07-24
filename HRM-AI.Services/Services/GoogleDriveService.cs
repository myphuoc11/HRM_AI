using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using HRM_AI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

public class GoogleDriveService : IGoogleDriveService
{
    private readonly DriveService _service;
    private readonly string _folderId;

    public GoogleDriveService(IConfiguration configuration)
    {
        var credentialsPath = configuration["GoogleDrive:CredentialsPath"];
        _folderId = configuration["GoogleDrive:FolderId"];

        GoogleCredential credential;
        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(new[] {
                    DriveService.Scope.Drive,
                    DriveService.Scope.DriveFile
                });
        }

        _service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "HRM PDF Upload"
        });
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string campaignFolderName, string positionFolderName)
    {
        var campaignFolderId = await EnsureFolderExistsAsync(campaignFolderName, _folderId);

        var positionFolderId = await EnsureFolderExistsAsync(positionFolderName, campaignFolderId);


        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = fileName,
            Parents = new List<string> { positionFolderId }
        };

        var request = _service.Files.Create(fileMetadata, fileStream, contentType);
        request.Fields = "id, webViewLink";
        request.SupportsAllDrives = true;

        var result = await request.UploadAsync();

        if (result.Status != UploadStatus.Completed)
        {
            throw new Exception($"Upload failed: {result.Exception?.Message}");
        }

        var uploadedFile = request.ResponseBody;

        await SetPublicPermission(uploadedFile.Id);

        return uploadedFile.WebViewLink ?? $"https://drive.google.com/file/d/{uploadedFile.Id}/view";
    }

    private async Task SetPublicPermission(string fileId)
    {
        var permission = new Permission
        {
            Type = "anyone",         
            Role = "reader"          
        };

        var request = _service.Permissions.Create(permission, fileId);
        request.Fields = "id";
        request.SupportsAllDrives = true;

        await request.ExecuteAsync();
    }
    public async Task<string> CreateFolderAsync(string folderName, string parentFolderId)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = new List<string> { parentFolderId }
        };

        var request = _service.Files.Create(fileMetadata);
        request.Fields = "id";
        request.SupportsAllDrives = true;

        var folder = await request.ExecuteAsync();
        return folder.Id;
    }
    public async Task<string?> GetFolderIdAsync(string folderName, string parentFolderId)
    {
        var listRequest = _service.Files.List();
        listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and trashed=false and name='{folderName}' and '{parentFolderId}' in parents";
        listRequest.Spaces = "drive";
        listRequest.Fields = "files(id, name)";
        listRequest.SupportsAllDrives = true;
        listRequest.IncludeItemsFromAllDrives = true;

        var result = await listRequest.ExecuteAsync();
        return result.Files.FirstOrDefault()?.Id;
    }


    public async Task<string> EnsureFolderExistsAsync(string folderName, string parentFolderId)
    {
        var folderId = await GetFolderIdAsync(folderName, parentFolderId);
        if (!string.IsNullOrEmpty(folderId))
            return folderId;

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder",
            Parents = new List<string> { parentFolderId }
        };

        var createRequest = _service.Files.Create(fileMetadata);
        createRequest.Fields = "id";
        createRequest.SupportsAllDrives = true;

        var folder = await createRequest.ExecuteAsync();
        return folder.Id;
    }



}

