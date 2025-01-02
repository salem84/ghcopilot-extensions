

using Bogus;
using FakerDataApi;
using FakerDataApi.Services;

namespace FakerDataApi.Endpoints;

public class CodiceFiscaleEndpoint : IEndpointRouteHandlerBuilder
{
    private static readonly Dictionary<string, string> CodiciCatastaliCittà = new Dictionary<string, string>
    {
        { "H501", "Roma" },
        { "F205", "Milano" },
        { "F839", "Napoli" },
        { "L219", "Torino" },
        { "G273", "Palermo" },
        // Aggiungi altri codici e città
    };

    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var codiceFiscaleApiGroup = endpoints.MapGroup("/api/codicefiscale/genera");

        codiceFiscaleApiGroup.MapGet(string.Empty, GeneraCodiceFiscaleAsync);
    }

    private static async Task<IResult> GeneraCodiceFiscaleAsync(HttpContext context, ILogger<CodiceFiscaleEndpoint> logger, char? genere = null, int? minAge = null, int? maxAge = null, string cityCode = null)
    {
        logger.LogInformation("Elaborazione di una richiesta per generare dati falsi del codice fiscale.");

        var faker = new Faker("it");

        // Genera un genere casuale se non fornito
        if (genere == null || char.ToUpper(genere.Value) != 'M' && char.ToUpper(genere.Value) != 'F')
        {
            genere = faker.PickRandom(new[] { 'M', 'F' });
        }

        minAge ??= 18;
        maxAge ??= 50;

        if (minAge < 0 || maxAge < minAge)
        {
            return TypedResults.BadRequest("Intervallo di età non valido. Assicurati che 'minAge' sia non negativo e 'maxAge' sia maggiore o uguale a 'minAge'.");
        }

        if (string.IsNullOrEmpty(cityCode))
        {
            // Scegli un codice città casuale se non fornito
            var random = new Random();
            var randomCityCode = new List<string>(CodiciCatastaliCittà.Keys)[random.Next(CodiciCatastaliCittà.Count)];
            cityCode = randomCityCode;
        }

        // Genera la data di nascita entro l'intervallo di età fornito
        var birthDate = faker.Date.Between(DateTime.Today.AddYears(-maxAge.Value), DateTime.Today.AddYears(-minAge.Value)).Date;

        // Genera nomi e cognomi in base al genere
        var firstName = genere == 'M' ? faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male) : faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female);
        var lastName = faker.Name.LastName();

        // Genera il codice fiscale
        var codiceFiscaleGenerator = new CodiceFiscaleGenerator();
        var codiceFiscale = codiceFiscaleGenerator.Generate(firstName, lastName, birthDate, genere.Value, cityCode);

        // Recupera il nome della città in base al codice città
        var cityName = CodiciCatastaliCittà.ContainsKey(cityCode) ? CodiciCatastaliCittà[cityCode] : "Città sconosciuta";

        // Costruisci la risposta
        var response = new
        {
            Nome = firstName,
            Cognome = lastName,
            Sesso = genere,
            DataDiNascita = birthDate.ToString("yyyy-MM-dd"),
            CittaDiNascita = cityName,
            CodiceFiscale = codiceFiscale
        };

        return TypedResults.Ok(response);
    }
}
