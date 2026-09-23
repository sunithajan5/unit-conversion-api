using System.Text.Json.Serialization;

namespace UnitConversionApi.Application.Domain;

[JsonConverter(typeof(JsonStringEnumConverter<UnitCategory>))]
public enum UnitCategory
{
    Length,
    Weight,
    Temperature
}