using DabAgent.Models;
using System.Net.Http.Headers;

namespace DabAgent.Services;

public class GitHubCopilotApiService(IHttpClientFactory httpClientFactory)
{
    private const string COPILOT_BASE_API_URL = "https://api.githubcopilot.com";

    public async Task<Stream> GetStreamCompletionsAsync(string gitHubToken, AgentPayload payload)
    {
        var ghHttpClient = httpClientFactory.CreateClient();
        ghHttpClient.BaseAddress = new Uri(COPILOT_BASE_API_URL);
        ghHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gitHubToken);
        payload.Stream = true;
        payload.Model = "gpt-4o";
        var copilotLLMResponse = await ghHttpClient.PostAsJsonAsync("/chat/completions", payload);
        return await copilotLLMResponse.Content.ReadAsStreamAsync();
    }

    public async Task<string?> GetStringCompletionsAsync(string gitHubToken, AgentPayload payload)
    {
        var ghHttpClient = httpClientFactory.CreateClient();
        ghHttpClient.BaseAddress = new Uri(COPILOT_BASE_API_URL);
        ghHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gitHubToken);
        payload.Stream = false;
        payload.Model = "gpt-4o";

        var copilotLLMResponse = await ghHttpClient.PostAsJsonAsync("/chat/completions", payload);
        var responseChatCompletion = await copilotLLMResponse.Content.ReadFromJsonAsync<ChatCompletion>();

        //var chatCompletion = await copilotLLMResponse.Content.ReadFromJsonAsync<Microsoft.Extensions.AI.ChatCompletion>();

        return responseChatCompletion?.Choices.FirstOrDefault()?.Message?.Content;
    }
}
