namespace DabAgent.Services;

public class GraphQLService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    private readonly string _swaBaseUrl = configuration["SwaBaseUrl"];

    public async Task<string> GetGraphQLSchema(string graphQLschemaRelativeUrl)
    {
        var swaHttpClient = httpClientFactory.CreateClient();
        swaHttpClient.BaseAddress = new Uri(_swaBaseUrl);
        var schemaGraphQL = await swaHttpClient.GetStringAsync(graphQLschemaRelativeUrl);
        return schemaGraphQL;
    }

    public async Task<string> SendGraphQLQuery(string query)
    {
        var swaHttpClient = httpClientFactory.CreateClient();
        swaHttpClient.BaseAddress = new Uri(_swaBaseUrl);

        var queryObject = new
        {
            query = query
        };

        //var content = new StringContent(JsonSeria(queryObject), Encoding.UTF8, "application/json")
        var response = await swaHttpClient.PostAsJsonAsync("/data-api/graphql", queryObject);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
