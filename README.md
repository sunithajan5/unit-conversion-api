# Unit Conversion API

An ASP.NET Core Web API for converting values between units of measurement. It currently supports length, weight and temperature.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Running the API

```bash
git clone https://github.com/sunithajan5/unit-conversion-api.git
cd unit-conversion-api
dotnet run --project UnitConversionApi.Api
```

The console shows the URL the API is listening on. Swagger UI is available at `/swagger` when running in Development, which is the default for `dotnet run`. If you open the solution in Visual Studio, pressing F5 opens Swagger automatically.

`UnitConversionApi.Api/UnitConversionApi.Api.http` has ready-made requests you can send from Visual Studio or VS Code. Update the port at the top of the file if yours is different.

## Running the tests

```bash
dotnet test
```

## Endpoints

### Convert a value

```
GET /api/conversions?value=100&fromUnit=C&toUnit=F
```

```json
{
  "value": 100,
  "fromUnit": "C",
  "toUnit": "F",
  "result": 212,
  "category": "Temperature"
}
```

### List supported units

```
GET /api/units
GET /api/units?category=Length
```

```json
[
  { "code": "m", "name": "Metre", "category": "Length" },
  { "code": "cm", "name": "Centimetre", "category": "Length" }
]
```

### Errors

All errors are returned in the standard ProblemDetails format (`application/problem+json`).

| Case | Status | Title |
|---|---|---|
| Missing or non-numeric `value`, missing unit | 400 | One or more validation errors occurred. |
| Unit code not recognised | 400 | Unknown unit |
| Units from different categories (e.g. `m` to `kg`) | 400 | Incompatible units |
| `NaN`/`Infinity`, or a temperature below absolute zero | 400 | Invalid value |
| Unknown category in `/api/units` | 400 | One or more validation errors occurred. |
| Anything unexpected | 500 | An unexpected error occurred |

Example:

```json
{
  "title": "Incompatible units",
  "status": 400,
  "detail": "Cannot convert Metre (Length) to Kilogram (Weight). Both units must be in the same category."
}
```

## Supported units

| Category | Base unit | Units |
|---|---|---|
| Length | Metre | `m`, `cm`, `mm`, `km`, `in`, `ft`, `yd`, `mi` |
| Weight | Kilogram | `kg`, `g`, `mg`, `t` (tonne), `lb`, `oz`, `st` (stone) |
| Temperature | Kelvin | `K`, `C`, `F` |

Unit codes are **case sensitive**.

## Project structure

```
UnitConversionApi.Api            HTTP layer: controllers, error handling, startup
UnitConversionApi.Application    Domain model, unit registry, conversion logic
UnitConversionApi.Tests          Unit and integration tests (MSTest, Moq)
```

The Api project depends on Application, never the other way round. Application has no dependency on ASP.NET Core, so the conversion logic can be reused or tested on its own.

## Design decisions

### Converting through a base unit

Each unit is defined once, relative to the base unit of its category:

```
base value  = value * Factor + Offset
result      = (base value - target Offset) / target Factor
```

Any two units in the same category can then be converted in two steps: into the base unit, then out of it. The alternative is to store a conversion factor for every pair of units. That grows quadratically (100 units would need 9,900 factors) and every new unit has to be added against all the existing ones. With a base unit, adding a unit means adding one line.

The `Offset` is there for temperature, which cannot be converted by multiplying alone (°F = °C × 9/5 + 32). For every other unit the offset is 0.

The base units are the SI base units (metre, kilogram, kelvin), so it's clear which unit to use as the base when a new category is added.

### Unit registry

Units are hardcoded in `InMemoryUnitRegistry`, as allowed by the brief. The rest of the application only depends on `IUnitRegistry`, so moving the definitions into a database or config file later means writing one new class.

- Units are held in a dictionary keyed by code, so lookups are O(1) whatever the number of units.
- The dictionary is built once at startup and registered as a singleton. `Unit` is immutable (`init` properties), which makes sharing it across requests safe.
- A duplicate unit code throws when the registry is built, so the mistake shows up immediately instead of causing wrong results.

### Case-sensitive unit codes

Many unit symbols differ only by case once prefixes come in: `mm` (millimetre) and `Mm` (megametre), or `mg` and `Mg`. Matching case-insensitively would work for today's list but would break as more units are added, so codes are matched exactly.

### Error handling

The conversion service throws specific exceptions (`UnknownUnitException`, `IncompatibleUnitsException`, `InvalidValueException`). A single `IExceptionHandler` turns these into 400 responses and anything else into a 500 without internal details. Controllers therefore have no try/catch blocks, and every error has the same shape.

Input validation (missing or non-numeric parameters) is handled by `[ApiController]` and data annotations before the controller runs. `value` is a nullable `double` so that a missing value is rejected rather than treated as 0.

### Numbers and rounding

Values are `double`, which is the usual choice for physical measurements. Floating point arithmetic can produce results like `211.99999999999997`, so results are rounded to 12 significant figures. Significant figures are used rather than decimal places so that very small results (1 mg is 0.000000001 t) are not rounded to zero.

### Other choices

- **Negative values** are allowed for length and weight, since a conversion of a negative quantity (e.g. a change of -5 m) is still meaningful. Temperatures below absolute zero are rejected.
- **GET for conversions**, because a conversion does not change anything on the server. It also means results can be cached.
- **Controllers rather than minimal APIs**, as they give an obvious place for each group of endpoints as the API grows.

## Trade-offs and assumptions

- **Linear conversions only.** Every unit fits `value * Factor + Offset`. Non-linear units (decibels) or units with changing rates (currencies) would need a different kind of conversion. The natural next step would be a strategy per category behind an interface. I didn't add that now, because with three linear categories it would be extra structure with no benefit yet.
- **Rounding** to 12 significant figures is plenty for everyday measurements but would need revisiting for scientific use.
- **No persistence, authentication or caching**, as none were required.

