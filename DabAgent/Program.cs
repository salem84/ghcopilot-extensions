using Microsoft.AspNetCore.Mvc;
using Octokit;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

var appName = "GHCopilotLocalDev";
var schemaGraphQLUrl = "";
var dabApiUrl = "";
//var appName = "AIDay2025";
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/info", () => "Hello GraphQL Copilot!");
app.MapGet("/callback", () => "You may close this window and return to GitHub where you should refresh the page and start a fresh chat.");

app.MapPost(
    "/", async ([FromHeader(Name = "X-GitHub-Token")] string githubToken,
    [FromBody] Payload payload,
    [FromServices] HttpClient httpClient) =>
    {
        var schemaGraphQL = await httpClient.GetStringAsync(schemaGraphQLUrl);

        // Insert prompt system messages in the message list.
        payload.Messages.Insert(0, new Message
        {
            Role = "system",
            Content = $"Start every response with the user's name, which is "
        });

        // Insert GraphQL Schema prompt
        payload.Messages.Insert(0, new Message
        {
            Role = "system",
            Content = "****"
        });

        // Use Copilot's LLM to generate a response to the user's messages.
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken);
        payload.Stream = true;

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
    public List<Message> Messages { get; set; } = [];
}