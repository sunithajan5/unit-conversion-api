namespace UnitConversionApi.Application.Domain;

/// <summary>
/// A unit of measurement
/// </summary>

public class Unit
{
    public required UnitCategory Category { get; init; }


    public required string Name { get; init; }
    public required string Code { get; init; }
    public required double Factor { get; init; }



   public double Offset { get; init; }
}