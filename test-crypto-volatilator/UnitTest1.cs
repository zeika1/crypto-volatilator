namespace test_crypto_volatilator;
using Moq;

public class UtilityFunctionsTests
{
    [Fact]
    public void TestCalculateVolatility()
    {
        // Arrange
        List<decimal> prices = new List<decimal> { 10, 12, 9, 14 }; // Example prices

        // Act
        double volatility = UtilityFunctions.CalculateVolatility(prices);

        double expectedVolatility = 1.92; 
        Assert.Equal(expectedVolatility, volatility, 4); // Using a tolerance of 0.0001 for floating point comparison
    }
}

public class CryptoVolatilityTests
{
    [Fact]
    public async Task TestVolatilityCalculationWithNullCValue()
    {
        // Arrange
        var httpClientMock = new Mock<HttpClient>();
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent(@"{
            ""results"": [
                { ""c"": null },
                { ""c"": 50 },
                { ""c"": 60 }
            ]
        }");
        httpClientMock
            .Setup(client => client.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponseMessage);

        var app = new MinimalApiApp(httpClientMock.Object);

        // Act
        var volatility = await app.GetCryptoVolatilityAsync();

        // Assert
        Assert.Equal(0, volatility); // Since c value is null, volatility should be 0
    }
}