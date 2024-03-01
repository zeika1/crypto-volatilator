namespace test_crypto_volatilator;

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
        Assert.Equal(expectedVolatility, volatility, 4); 
    }
}
