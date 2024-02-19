using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Minimal API Application is running!");

app.MapGet("/crypto", async (HttpClient httpClient, string currencyPair = "BTCUSD") =>
{
    
    var apiKey = "Q5KVzUVWTqY0FowOEptda4rt8VIlLVJD";
    var formattedCurrencyPair = UtilityFunctions.GetFormattedCurrencyPair(currencyPair).ToUpper();

    var url = $"https://api.polygon.io/v2/aggs/ticker/X:{formattedCurrencyPair}/range/1/day/2023-01-09/2023-01-09?adjusted=true&sort=asc&limit=120&apiKey={apiKey}";

    var response = await httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var content = await response.Content.ReadAsStringAsync();
    return content;
});


app.Run();

public static class UtilityFunctions
{
    public static string GetFormattedCurrencyPair(string currencyPair)
    {
        // Ensure that the currency pair is properly formatted
        return currencyPair.ToLower();
    }
}