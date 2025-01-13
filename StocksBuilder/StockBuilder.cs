using System.Numerics;

namespace StocksBuilder;

public class StockBuilder<T>(int seed) where T : INumber<T>
{
    private readonly Stock<T> _stock = new();
    private readonly Random _rand = new(seed);

    public StockBuilder<T> WithSymbol(char symbol)
    {
        _stock.Symbol = symbol;
        return this;
    }

    public StockBuilder<T> WithName(string name)
    {
        _stock.Name = name;
        return this;
    }

    public StockBuilder<T> WithHistory(params T[] history)
    {
        _stock.History = history;
        return this;
    }

    /// <summary>
    /// Generates a price path for the stock using the Geometric Brownian Motion model.
    /// </summary>
    /// <param name="initialPrice">The initial price of the stock.</param>
    /// <param name="drift">The expected return of the stock.</param>
    /// <param name="volatility">The volatility of the stock's returns.</param>
    /// <param name="amountOfYears">The total time period for the price path in years.</param>
    /// <param name="numberOfSteps">The number of time steps to generate within the total time period.</param>
    /// <returns>The current instance of <see cref="StockBuilder{T}"/> with the generated price path set in the stock's history.</returns>
    public StockBuilder<T> WithGeneratedHistory(T initialPrice, double drift = 0.2, double volatility = 0.4, double amountOfYears = 1.0, int numberOfSteps = 365)
    {
        var pricePath = new T[numberOfSteps + 1];
        pricePath[0] = initialPrice;

        var timeStepSize = amountOfYears / numberOfSteps;

        for (int i = 1; i <= numberOfSteps; i++)
        {
            double standardNormalRandom = NormalSample();
            double priceChangeFactor = (drift - 0.5 * volatility * volatility) * timeStepSize
                                       + volatility * Math.Sqrt(timeStepSize) * standardNormalRandom;
            var nextPrice = pricePath[i - 1] * T.CreateChecked(Math.Exp(priceChangeFactor));
            pricePath[i] = nextPrice;
        }

        _stock.History = pricePath;
        return this;
    }

    private double NormalSample() => Math.Sqrt(-2.0 * Math.Log(_rand.NextDouble())) * Math.Cos(2.0 * Math.PI * _rand.NextDouble());
    
    public Stock<T> Build() => _stock;
}
