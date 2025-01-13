using System.Numerics;

namespace StocksBuilder;

public class Stock<T> where T : INumber<T>
{
    public char Symbol { get; set; }
    public string? Name { get; set; }
    public T[]? History { get; set; }
}