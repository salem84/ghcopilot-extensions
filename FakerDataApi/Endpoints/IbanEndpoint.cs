

using Microsoft.AspNetCore.Http.HttpResults;
using IbanNet;
using IbanNet.Registry;
using IbanNet.Builders;
using Bogus;
using FakerDataApi;

namespace FakerDataApi.Endpoints;

public class IbanEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var ibanApiGroup = endpoints.MapGroup("/api/iban/genera");

        ibanApiGroup.MapPost(string.Empty, GeneraIbanAsync);
    }

    private static async Task<Ok<GeneraIbanResponse>> GeneraIbanAsync(HttpContext context, GeneraIbanRequest request)
    {
        //IIbanRegistry registry = IbanRegistry.Default;
        //var bankIdentifier = faker.Finance.Bic();
        //var branchIdentifier = faker.Finance.;
        //var bankAccountNumber = faker.Finance.Account(length: 12);

        //string iban = new IbanBuilder()
        //    .WithCountry("IT", registry)
        //    .WithBankIdentifier(bankIdentifier)
        //    .WithBranchIdentifier(branchIdentifier)
        //    .WithBankAccountNumber(bankAccountNumber)
        //    .Build();

        var faker = new Faker();

        var iban = faker.Finance.Iban(formatted: false, countryCode: request.CountryCode);

        return TypedResults.Ok(new GeneraIbanResponse(iban));
    }

    public record GeneraIbanRequest(string CountryCode = "IT");
    public record GeneraIbanResponse(string Iban);
}
