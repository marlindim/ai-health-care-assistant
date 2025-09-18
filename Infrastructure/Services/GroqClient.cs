
using Core.Contract;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class GroqClient : IAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;
        private readonly string baseUrl = "https://api.groq.com/openai/v1/chat/completions";
        public GroqClient(IConfiguration config)
        {
            var apiKey = config["Groq:Api_Key"] ?? throw new Exception("Groq API key not configured");
            _model = _model = config["Groq:Model"] ?? "openai/gpt-oss-20b";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 512)
        {
            var body = new
            {
                model = _model,
                messages = new[]
                {
            new { role = "system", content = "You are a helpful medical assistant AI." },
            new { role = "user", content = prompt }
        },
                max_tokens = maxTokens
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // pretty print for easier reading
            };

            var json = JsonSerializer.Serialize(body, options);
            Console.WriteLine("📤 Request JSON:");
            Console.WriteLine(json);

            var response = await _httpClient.PostAsync(
                baseUrl,
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("📥 Raw Response JSON:");
            Console.WriteLine(result);

            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(result);

            // Defensive parsing with TryGetProperty to avoid crashes
            if (document.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var content = choices[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return content ?? string.Empty;
            }

            return "⚠️ No content found in response.";
        }

        #region Reference
        //public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 512)
        //{
        //    var body = new
        //    {
        //        model = _model,
        //        messages = new[]
        //        {
        //            new { role = "system", content = "You are a helpful medical assistant AI." },
        //            new { role = "user", content = prompt }
        //        },
        //        max_tokens = maxTokens
        //    };

        //    var json = JsonSerializer.Serialize(body);
        //    var response = await _httpClient.PostAsync(
        //        "chat/completions",
        //        new StringContent(json, Encoding.UTF8, "application/json")
        //    );

        //    response.EnsureSuccessStatusCode();
        //    var result = await response.Content.ReadAsStringAsync();

        //    using var document = JsonDocument.Parse(result);
        //    var content = document.RootElement
        //        .GetProperty("choices")[0]
        //        .GetProperty("message")
        //        .GetProperty("content")
        //        .GetString();

        //    return content ?? string.Empty;
        //}
        #endregion
        public async Task<string> SummarizeAsync(string text)
        {
            var prompt = $"Please summarize the following medical record in a concise and clear way:\n\n{text}";
            return await GenerateTextAsync(prompt, 256);
        }
    }
}





#region Old Code
//using Core.Contract;
//using Microsoft.Extensions.Configuration;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;

//namespace Infrastructure.Services
//{
//    public class GroqClient:IAIClient
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _model;
//        string computer = "System";
//        string user = "Human";//Environment.UserName;
//        string assistant = "Hi there! I am your Medical Assistance, How may I be of help";
//        // This is a stub. (https://api.groq.ai/v1/) Replace with real OpenAI/Azure/Open-source client. "https://api.groq.com/openai/v1/"
//        public GroqClient(IConfiguration _Iconfig)
//        {
//            var apiKey = _Iconfig["Groq:ApiKey"] ?? throw new Exception("Model not configured properly");
//            _model = _Iconfig["Groq:Model"] ?? "llama3-8b-8192"; //"groq-3.5-turbo";
//            _httpClient = new HttpClient();
//            _httpClient.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
//            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",apiKey);
//        }
//        public Task<string> GenerateTextAsync(string prompt, int maxTokens = 512)
//        {
//            // Very simple mock response to keep local dev offline-friendly
//            //var mock = $"[AI GENERATED SUMMARY]\nPrompt: {prompt.Substring(0, Math.Min(prompt.Length, 200))}...\n(Replace with real AI client)";
//            //return Task.FromResult(mock);

//            var body = new
//            {
//                model = _model,
//                message = new[]
//                {
//                    new { role = computer, content = assistant },
//                    new { role = user, content = prompt }
//                },
//                max_token = maxTokens
//            };

//            var Json = JsonSerializer.Serialize(body);
//            var response = _httpClient.PostAsync("chat/completions",
//                new StringContent(Json, Encoding.UTF8, "application/json")).Result;
//            //var response = _httpClient.PostAsync("chat/completions", );
//            response.EnsureSuccessStatusCode();
//            var result = response.Content.ReadAsStringAsync().Result;

//            using var document = JsonDocument.Parse(result);
//            var content = document.RootElement
//                .GetProperty("choices")[0]
//                .GetProperty("message")
//                .GetProperty("content")
//                .GetString();
//            return Task.FromResult(content ?? string.Empty);
//        }

//        public Task<string> SummarizeAsync(string text)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
#endregion