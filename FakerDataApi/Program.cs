using FakerDataApi;
using FakerDataApi.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();

app.MapEndpoints<CodiceFiscaleEndpoint>();
app.MapEndpoints<IbanEndpoint>();

app.Run();

