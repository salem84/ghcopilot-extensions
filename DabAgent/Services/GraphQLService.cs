namespace DabAgent.Services
{
    public class GraphQLService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        private readonly string _swaBaseUrl = configuration["SwaBaseUrl"];

        public async Task<string> GetGraphQLSchema()
        {
            var swaHttpClient = httpClientFactory.CreateClient();
            swaHttpClient.BaseAddress = new Uri(_swaBaseUrl);
            var schemaGraphQL = await swaHttpClient.GetStringAsync("/schema-todos.graphql");
            return schemaGraphQL;
        }

        public async Task<string> SendGraphQLQuery(string query)
        {
            var swaHttpClient = httpClientFactory.CreateClient();
            swaHttpClient.BaseAddress = new Uri(_swaBaseUrl);
            var content = new StringContent(query);
            var response = await swaHttpClient.PostAsync("/data-api/graphql", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
