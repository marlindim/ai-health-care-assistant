namespace Core.Contract
{
    public interface IAIClient
    {
        Task<string> GenerateTextAsync(string prompt, int maxTokens = 512);
        Task<string> SummarizeAsync(string text);
    }
}
