using Core.Contract;
using System.Text.Json;
using static Infrastructure.AIqueryResponse.CheckResponse;

namespace Infrastructure.SymptomChecker
{
    public class SymptomCheckerService
    {
        private readonly IAIClient _ai;

        public SymptomCheckerService(IAIClient ai)
        {
            _ai = ai;
        }

        public async Task<SymptomCheckResponse> CheckAsync(SymptomCheckRequest req)
        {
            // Build prompt for AI
            var prompt = $"Patient symptoms: {req.Symptoms}\n" +
                         "Return a JSON object with keys: conditions (array of strings), advice (string), confidence (0-1).";

            var aiResult = await _ai.GenerateTextAsync(prompt);

            try
            {
                // Expected AI response format
                var parsed = JsonSerializer.Deserialize<SymptomCheckResponse>(aiResult);

                if (parsed != null)
                    return parsed;
            }
            catch
            {
                // fallback if AI doesn’t return valid JSON
            }

            // Fallback (safe default)
            return new SymptomCheckResponse(
                new List<string> { "Unknown condition" },
                "Unable to analyze symptoms. Please consult a doctor.",
                0.0
            );
        }
    }
}