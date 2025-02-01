using DabAgent.Services;
using Microsoft.AspNetCore.Mvc;

var appName = "GHCopilotLocalDev";

//var appName = "AIDay2025";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddScoped<GraphQLService>();
builder.Services.AddScoped<GitHubCopilotApiService>();
var app = builder.Build();

app.MapGet("/info", () => "Hello GraphQL Copilot!");
app.MapGet("/callback", () => "You may close this window and return to GitHub where you should refresh the page and start a fresh chat.");

app.MapPost(
    "/", async ([FromHeader(Name = "X-GitHub-Token")] string githubToken,
    [FromBody] AgentPayload payload,
    HttpContext context,
    [FromServices] ILogger<Program> logger,
    [FromServices] GraphQLService graphQLService,
    [FromServices] GitHubCopilotApiService copilotApiService) =>
    {
        var schemaGraphQL = await graphQLService.GetGraphQLSchema();

        // Get the latest user message and modify it later to include a custom prompt        
        var lastMessageContent = payload.Messages.Last(x => x.Role == "user").Content;

        payload.Messages.Clear();

        // InsertChat prompt system messages in the message list.
        payload.Messages.Add(new ChatMessage
        {
            Role = "system",
            Content = $@"You are a GraphQL developer.
                You must write a GraphQL query for a database with the following schema: {{schemaGraphQL}}"
        });

        // Insert GraphQL Schema prompt
        payload.Messages.Add(new ChatMessage
        {
            Role = "system",
            Content = $@"Response must be contain only GraphQL response in query format without any other information such as markdown.
                If you cannot generate a GraphQL a 'Bad Request' error must be returned."
        });

        // Insert user query message
        payload.Messages.Add(new ChatMessage
        {
            Role = "user",
            Content = $"The user query is: '{lastMessageContent}'"
        });

        // Stream the response straight back to the user.
        //var copilotStreamResponse = await copilotApiService.GetStreamCompletionsAsync(githubToken, payload);
        //return Results.Stream(copilotStreamResponse, "application/json");

        var copilotResponse = await copilotApiService.GetStringCompletionsAsync(githubToken, payload);
        logger.LogInformation("Copilot response: {copilotResponse}", copilotResponse);

        var graphQLResponse = await graphQLService.SendGraphQLQuery(copilotResponse);

        //await context.Response.WriteTextStreamEvent(graphQLResponse);
        //await context.Response.WriteEndStreamEvent();
        //return;
    });