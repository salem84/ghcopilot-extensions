using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

var appName = "GHCopilotLocalDev";
var baseUrl = "https://mango-island-0e735f103.4.azurestaticapps.net";
var schemaGraphQLUrl = $"{baseUrl}/schema-todos.graphql";
var dabGraphQLApiUrl = $"{baseUrl}/data-api/graphql";
//var appName = "AIDay2025";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
var app = builder.Build();

app.MapGet("/info", () => "Hello GraphQL Copilot!");
app.MapGet("/callback", () => "You may close this window and return to GitHub where you should refresh the page and start a fresh chat.");

app.MapPost(
    "/", async ([FromHeader(Name = "X-GitHub-Token")] string githubToken,
    [FromBody] Payload payload,
    [FromServices] HttpClient httpClient) =>
    {
        var schemaGraphQL = await httpClient.GetStringAsync(schemaGraphQLUrl);
        var latestMessage = payload.Messages.Last(x => x.Role == "user");
        latestMessage.Content = $"The user query is: \"{latestMessage.Content}\"";

        payload.Messages.Clear();

        // Insert prompt system messages in the message list.
        payload.Messages.Add(new Message
        {
            Role = "system",
            Content = $"You are a GraphQL developer.\r\nYou must write a GraphQL query for a database with the following schema: {schemaGraphQL}"
        });

        // Insert GraphQL Schema prompt
        payload.Messages.Add(new Message
        {
            Role = "system",
            Content = $"Response must be contain only GraphQL response in query format without any other information such as markdown. \r\nIf you cannot generate a GraphQL a \"Bad Request\" error must be returned."
        });

        payload.Messages.Add(latestMessage);

        // Use Copilot's LLM to generate a response to the user's messages.
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken);
        payload.Stream = true;
        payload.Model = "gpt-4o";
        var copilotLLMResponse = await httpClient.PostAsJsonAsync("https://api.githubcopilot.com/chat/completions", payload);

        // Stream the response straight back to the user.
        var responseStream = await copilotLLMResponse.Content.ReadAsStreamAsync();
        return Results.Stream(responseStream, "application/json");
    });

app.Run();

internal class Message
{
    public required string Role { get; set; }
    public required string Content { get; set; }
}

internal class Payload
{
    public bool Stream { get; set; }
    public string Model { get; set; } = string.Empty;
    public List<Message> Messages { get; set; } = [];
}