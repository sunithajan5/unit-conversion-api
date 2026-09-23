using System.ComponentModel.DataAnnotations;

namespace UnitConversionApi.Application.Contracts;

/// <summary>
/// Query parameters for a conversion
/// </summary>
public record ConversionRequest(
    [Required] double? Value,
    [Required] string FromUnit,
    [Required] string ToUnit);