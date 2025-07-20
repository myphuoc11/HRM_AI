using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace HRM_AI.Services.Services
{
    public class ResumeParserAIService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        public ResumeParserAIService(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _apiKey = cfg["ResumeParserAI:ApiKey"];
        }

        public async Task<JObject> ParseAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            using var form = new MultipartFormDataContent();
            var bytes = ms.ToArray();
            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            form.Add(fileContent, "file", file.FileName);

            var req = new HttpRequestMessage(HttpMethod.Post, "https://resumeparserai.com/api/parse");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            req.Content = form;

            var res = await _http.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var raw = await res.Content.ReadAsStringAsync();
            return JObject.Parse(raw);
        }
    }



}
