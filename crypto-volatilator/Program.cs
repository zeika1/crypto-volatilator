using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
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


//api call deafault set to: BTCUSD
app.MapGet("/crypto", async (HttpClient httpClient, string currencyPair = "BTCUSD", DateTime? startDate = null, DateTime? endDate = null) =>
{
    
    var apiKey = "Q5KVzUVWTqY0FowOEptda4rt8VIlLVJD";

        // If start date is not provided, default to 2023-01-09
    if (!startDate.HasValue)
        startDate = new DateTime(2023, 1, 9);

    // If end date is not provided, default to 2023-01-10
    if (!endDate.HasValue)
        endDate = new DateTime(2023, 1, 10);

    var formattedCurrencyPair = UtilityFunctions.GetFormattedCurrencyPair(currencyPair).ToUpper();

    var startDateString = startDate.Value.ToString("yyyy-MM-dd");
    var endDateString = endDate.Value.ToString("yyyy-MM-dd");

    var url = $"https://api.polygon.io/v2/aggs/ticker/X:{formattedCurrencyPair}/range/1/day/{startDateString}/{endDateString}?adjusted=true&sort=asc&limit=120&apiKey={apiKey}";

    var response = await httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var contentStream = await response.Content.ReadAsStreamAsync();
    var jsonDocument = await JsonDocument.ParseAsync(contentStream);

    var results = jsonDocument.RootElement.GetProperty("results");
    var cArray = new List<decimal>();

    //parsing the reurned json format
    foreach (var result in results.EnumerateArray())
    {
        var cValue = result.GetProperty("c").GetDecimal();
        cArray.Add(cValue);
    }


    var volatility = UtilityFunctions.CalculateVolatility(cArray);
    return volatility;
    


});




app.Run();

public static class UtilityFunctions
{
    //Geting currency from user input
    public static string GetFormattedCurrencyPair(string currencyPair)
    {
        
        return currencyPair.ToLower();
    }

    public static double CalculateVolatility(List<decimal> prices)
    {
        if (prices == null || prices.Count < 2)
        {
            // If there are insufficient data points, return 0
            return 0;
        }

        // Calculate the mean of prices
        decimal mean = prices.Sum() / prices.Count;

        // Calculate the differences from the mean
        List<decimal> differences = new List<decimal>();
        foreach (var price in prices)
        {
            differences.Add(price - mean);
        }

        // Calculate the squared differences
        List<decimal> squaredDifferences = new List<decimal>();
        foreach (var difference in differences)
        {
            squaredDifferences.Add(difference * difference);
        }

        // Calculate the variance
        decimal variance = squaredDifferences.Sum() / prices.Count;

        // Calculate the standard deviation (volatility)
        double volatility = Math.Sqrt((double)variance);

        return Math.Round(volatility, 2);
    }


}





