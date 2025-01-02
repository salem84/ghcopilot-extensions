

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

        ibanApiGroup.MapGet(string.Empty, GeneraIbanAsync);
    }

    private static async Task<Ok<string>> GeneraIbanAsync(HttpContext context, string countryCode = "IT")
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

        var faker = new Faker("it");

        var iban = faker.Finance.Iban(formatted: false, countryCode: countryCode);

        return TypedResults.Ok(iban);
    }
}
