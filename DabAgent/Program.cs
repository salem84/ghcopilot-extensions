using DabAgent.Models;
using DabAgent.Services;
using Microsoft.AspNetCore.Mvc;

const string GRAPHQL_SCHEMA_TODOS_RELATIVE_URL = "/schema-todos.graphql";
const string GRAPHQL_SCHEMA_BOOKS_RELATIVE_URL = "/schema-books.graphql";
const string GRAPHQL_SCHEMA_PEOPLE_RELATIVE_URL = "/schema-people.graphql";

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
        var schemaGraphQL = await graphQLService.GetGraphQLSchema(GRAPHQL_SCHEMA_PEOPLE_RELATIVE_URL);

        // Get the latest user message and modify it later to include a custom prompt        
        var lastMessageContent = payload.Messages.Last(x => x.Role == "user").Content;

        payload.Messages.Clear();

        // InsertChat prompt system messages in the message list.
        payload.Messages.Add(new ChatMessage
        {
            Role = "system",
            Content = $@"You are a GraphQL developer.
                You must write a GraphQL query for a database with the following schema: {schemaGraphQL}"
        });

        // Insert GraphQL Schema prompt
        payload.Messages.Add(new ChatMessage
        {
            Role = "system",
            Content = $@"Response must be contain only GraphQL response in query format without any other information such as markdown or other escapes.
                Query must start with a curly brace character.
                If you cannot generate a GraphQL a 'Bad Request' error must be returned."
        });

        // Insert user query message
        payload.Messages.Add(new ChatMessage
        {
            Role = "user",
            Content = $"The user query is: '{lastMessageContent}'"
        });

        logger.LogInformation("Invoking Copilot...");
        var copilotResponse = await copilotApiService.GetStringCompletionsAsync(githubToken, payload);
        logger.LogInformation("Copilot response: {copilotResponse}", copilotResponse);

        logger.LogInformation("Invoking GraphQL Endpoint...");
        var graphQLResponse = await graphQLService.SendGraphQLQuery(copilotResponse);
        logger.LogInformation("Dab GraphQL Endpoint response: {graphQLResponse}", graphQLResponse);

        payload.Messages.Clear();
        payload.Messages.Add(new ChatMessage
        {
            Role = "user",
            Content = $@"You have to inform a user using natural language. The user ask this: '{lastMessageContent}'.
                  Rewrite the GraphQL response '{graphQLResponse}' in a simple way and include also the query."
        });

        //await context.Response.WriteTextStreamEvent(graphQLResponse);
        //await context.Response.WriteEndStreamEvent();
        //return;

        logger.LogInformation("Invoking Copilot...");
        var copilotFinalResponse = await copilotApiService.GetStreamCompletionsAsync(githubToken, payload);
        return Results.Stream(copilotFinalResponse, "application/json");
    });

app.Run();