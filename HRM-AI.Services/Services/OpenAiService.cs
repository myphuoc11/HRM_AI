using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using HRM_AI.Repositories.Interfaces;
using HRM_AI.Repositories.Models.ParsedFieldModels;
using HRM_AI.Services.Interfaces;
using HRM_AI.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI.GPT3;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Tesseract;
using UglyToad.PdfPig;

namespace HRM_AI.Services.Services
{
    public class OpenAiService : IOpenAiService
    {
        private readonly IConfiguration _configuration;
        private readonly IOpenAIService _openAI;
        private readonly IUnitOfWork _unitOfWork;

        public OpenAiService(IConfiguration configuration, IOpenAIService openAI, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _openAI = openAI;
            _unitOfWork = unitOfWork;
        }

        public async Task<float[]> GetEmbeddingAsync(List<string> texts)
        {
            if (texts == null || texts.Count == 0)
            {
                throw new ArgumentException("Input cannot be empty.", nameof(texts));
            }

            var apiKey = _configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("API key is missing.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "text-embedding-3-small",
                input = texts
            };

            var response = await httpClient.PostAsJsonAsync("https://api.openai.com/v1/embeddings", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve embeddings: {response.StatusCode} - {errorContent}");
            }

            var responseData = await response.Content.ReadFromJsonAsync<OpenAiEmbeddingResponse>();
            return responseData?.Data.FirstOrDefault()?.Embedding ?? throw new Exception("No embedding data returned.");
        }
        public async Task<ResponseModel> ParseCVAsync(IFormFile file, Guid campaignPositionId)
        {
            if (file == null || file.Length == 0)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = "File is empty or null"
                };
            }

            var campaignPosition = await _unitOfWork.CampaignPositionRepository.GetAsync(campaignPositionId);
            if (campaignPosition == null)
            {
                return new ResponseModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = "Campaign position not found."
                };
            }

            var jdParts = new List<string>
   {
       campaignPosition.Description
   };

            jdParts.AddRange(campaignPosition.CampaignPositionDetails
                .Select(d => $"{d.Key}: {d.Value}"));

            string jdText = string.Join("\n", jdParts);
            var campaignEmbeddingVector = await GetEmbeddingAsync(new List<string> { jdText });

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
                Model = OpenAI.GPT3.ObjectModels.Models.Gpt_4,
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

                    var cvEmbeddingVector = await GetEmbeddingAsync(new List<string> { extractedText });

                    // Compare vectors and calculate similarity score  
                    float similarityScore = CalculateCosineSimilarity(campaignEmbeddingVector, cvEmbeddingVector);

                    return new ResponseModel
                    {
                        Code = StatusCodes.Status200OK,
                        Message = $"CV successfully parsed. Similarity Score: {similarityScore}",
                        Data = new
                        {
                            ParsedData = parsedList,
                            SimilarityScore = similarityScore
                        }
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

        private float CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                throw new ArgumentException("Vectors must be of the same length.");

            float dotProduct = 0;
            float magnitudeA = 0;
            float magnitudeB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            magnitudeA = (float)Math.Sqrt(magnitudeA);
            magnitudeB = (float)Math.Sqrt(magnitudeB);

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct / (magnitudeA * magnitudeB);
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
    public class OpenAiEmbeddingResponse
    {
        public List<EmbeddingData> Data { get; set; } = new List<EmbeddingData>();
    }
    public class EmbeddingData
    {
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
