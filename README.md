# TemperaturaConverter

A small desktop temperature converter built with **C#**, **.NET 10**, and
**[MewUI](https://github.com/aprillz/MewUI)**.

## Features

- Converts between Celsius (°C), Fahrenheit (°F), and Kelvin (K)
- Updates the result as you type or change units
- Swap and reset buttons
- Enter key shortcut for conversion
- Rejects invalid numbers and values below absolute zero
- Culture-aware number parsing and formatting
- Light and dark theme support through the MewUI theme

## Requirements

- .NET 10 SDK

## Run

```bash
dotnet run
```

## Publish (optional)

```bash
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

## Project structure

| File | Purpose |
| --- | --- |
| `Program.cs` | MewUI window, controls, and event handling |
| `TemperatureConverter.cs` | Temperature units, parsing, validation, and conversion logic |
| `TemperaturaConverter.csproj` | .NET project and MewUI package reference |

## Conversion formulas

```text
°F = (°C × 9/5) + 32
K  = °C + 273.15
```

Absolute zero limits:

```text
-273.15 °C
-459.67 °F
0 K
```

## Naming conventions

The project uses standard C# naming conventions:

- `PascalCase` for types, methods, properties, and constants
- `camelCase` for local variables and parameters
- Descriptive names such as `sourceUnitComboBox`, `targetValue`, and `errorMessage`
