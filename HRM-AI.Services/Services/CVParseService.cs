using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Text;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using Microsoft.AspNetCore.Http;

public class CVParseService : ICVParseService
{
    private readonly IOpenAIService _openAI;

    public CVParseService(IOpenAIService openAI) 
    {
        _openAI = openAI;
    }

    public async Task<ResponseModel> ParseCVAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return new ResponseModel
            {
                Code = StatusCodes.Status400BadRequest,
                Message = "File is empty or null"
            };
        }

        var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
        await using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string extractedText = Path.GetExtension(tempFilePath).ToLower() switch
        {
            ".pdf" => ExtractTextFromPdf(tempFilePath),
            ".jpg" or ".jpeg" or ".png" => ExtractTextFromImage(tempFilePath),
            _ => throw new NotSupportedException("Unsupported file type.")
        };

        string prompt = $"""
        Đây là nội dung một CV ứng viên:

        {extractedText}

        Hãy phân tích và xuất ra JSON các thông tin có thể lấy được như: họ tên, email, số điện thoại, học vấn, kinh nghiệm, kỹ năng, chứng chỉ, vị trí ứng tuyển, ngành nghề.

        Lưu ý:
        - Nếu có nhiều kinh nghiệm, học vấn, kỹ năng thì hãy để dưới dạng mảng.
        - JSON phải phù hợp cho mọi ngành nghề (IT, Marketing, Kế toán...).
        - Nếu thiếu thông tin thì để trống hoặc null.
        - Giữ nguyên tiếng Việt.
        """;

        var completionResult = await _openAI.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Model = "gpt-4",
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("Bạn là một hệ thống phân tích CV thông minh."),
                ChatMessage.FromUser(prompt)
            },
            Temperature = (float?)0.3
        });

        if (completionResult.Successful)
        {
            return new ResponseModel
            {
                Code = 200,
                Message = "Phân tích CV thành công",
                Data = completionResult.Choices.First().Message.Content
            };
        }
        else
        {
            return new ResponseModel
            {
                Code = 500,
                Message = completionResult.Error?.Message ?? "GPT không phản hồi"
            };
        }
    }

    private string ExtractTextFromPdf(string path)
    {
        var text = new StringBuilder();
        using var document = UglyToad.PdfPig.PdfDocument.Open(path);
        foreach (var page in document.GetPages())
        {
            text.AppendLine(page.Text);
        }
        return text.ToString();
    }

    private string ExtractTextFromImage(string path)
    {
        using var engine = new Tesseract.TesseractEngine(@"./tessdata", "vie+eng", Tesseract.EngineMode.Default);
        using var img = Tesseract.Pix.LoadFromFile(path);
        using var page = engine.Process(img);
        return page.GetText();
    }
}
