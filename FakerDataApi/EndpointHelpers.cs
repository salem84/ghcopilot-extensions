namespace FakerDataApi;

public interface IEndpointRouteHandlerBuilder
{
    static abstract void MapEndpoints(IEndpointRouteBuilder endpoints);
}

public static class IEndpointRouteBuilderExtensions
{
    public static void MapEndpoints<T>(this IEndpointRouteBuilder endpoints)
        where T : IEndpointRouteHandlerBuilder => T.MapEndpoints(endpoints);
}