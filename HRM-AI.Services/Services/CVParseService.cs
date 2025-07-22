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
using Newtonsoft.Json;

public class ParsedFieldModel
{
    public string Type { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public int GroupIndex { get; set; }
}


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

        string extractedText;

        var extension = Path.GetExtension(tempFilePath).ToLower();
        if (extension == ".pdf")
        {
            extractedText = ExtractTextFromPdf(tempFilePath);
        }
        else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
        {
            extractedText = await ExtractTextFromImage(tempFilePath);
        }
        else
        {
            return new ResponseModel
            {
                Code = StatusCodes.Status400BadRequest,
                Message = "Unsupported file type."
            };
        }

        string prompt = GeneratePrompt(extractedText);

        var completionResult = await _openAI.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Model = Models.Gpt_4,
            Messages = new List<ChatMessage>
           {
               ChatMessage.FromSystem("You are an intelligent assistant that extracts structured information from CVs."),
               ChatMessage.FromUser(prompt)
           },
            Temperature = 0.3f
        });

        if (completionResult.Successful)
        {
            var content = completionResult.Choices.First().Message.Content;

            try
            {
                var cleanedContent = StripMarkdownJson(content);

                var parsedList = JsonConvert.DeserializeObject<List<ParsedFieldModel>>(cleanedContent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });

                return new ResponseModel
                {
                    Code = StatusCodes.Status200OK,
                    Message = "CV successfully parsed",
                    Data = parsedList
                };
            }
            catch (JsonReaderException ex)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message = $"Failed to parse JSON from GPT: {ex.Message}",
                    Data = content
                };
            }
        }


        return new ResponseModel
        {
            Code = StatusCodes.Status500InternalServerError,
            Message = $"Error calling GPT: {completionResult.Error?.Message}",
            Data = null
        };
    }

    private string GeneratePrompt(string cvContent)
    {
        return $"""
Analyze the entire content of the following CV and extract **all possible structured information** using a flexible JSON format.

For every detected piece of information, return an object with:
- "type" (e.g., "contact", "education", "experience", "skill", "certificate", "language", "project", "personal_info", etc.)
- "key" (e.g., "full_name", "email", "school", "company", "position", "degree", "dob", "gender", "objective", etc.)
- "value" (exact raw value as found in the CV)
- "group_index" (0 for single fields, and 1, 2, ... for grouped items like jobs or education)

Rules:
- Extract **everything you can find** in the CV (including contact info, personal info, languages, interests, references, achievements, activities, publications, etc.)
- **Do not fabricate, guess, or infer** any data. Only return values that are **explicitly stated** in the CV.
- If a section contains multiple entries (like work experience), use group_index = 1, 2,... accordingly.
- **Return ONLY a pure JSON array of objects**, no explanation or commentary.

CV Content:
{cvContent.Trim()}
""";
    }



    private string ExtractTextFromPdf(string path)
    {
        var text = new StringBuilder();
        using var document = PdfDocument.Open(path);
        foreach (var page in document.GetPages())
        {
            text.AppendLine(page.Text);
        }
        return text.ToString();
    }

    private async Task<string> ExtractTextFromImage(string imagePath)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);
                return page.GetText();
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to extract text from image using Tesseract: {ex.Message}");
        }
    }
    private string StripMarkdownJson(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return content;

        // Case 1: GPT trả về trong ```json ... ```
        if (content.StartsWith("```json"))
        {
            int start = content.IndexOf("```json") + 7;
            int end = content.LastIndexOf("```");
            if (end > start)
            {
                return content.Substring(start, end - start).Trim();
            }
        }

        // Case 2: GPT trả về trong ``` ```
        if (content.StartsWith("```"))
        {
            int start = content.IndexOf("```") + 3;
            int end = content.LastIndexOf("```");
            if (end > start)
            {
                return content.Substring(start, end - start).Trim();
            }
        }

        return content.Trim(); // fallback nếu không có markdown
    }
}
