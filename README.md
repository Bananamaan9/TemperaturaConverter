# TemperaturaConverter

A desktop Fahrenheit → Celsius converter built with **C#**, **.NET 10**, and
**[MewUI](https://github.com/aprillz/MewUI)**.

The app follows the Windows Forms tutorial `Conversor Fahrenheit → Celsius`,
adapted to MewUI's fluent C# markup.

## Features

- Converts Fahrenheit (°F) to Celsius (°C)
- Uses the formula `°C = (°F − 32) × 5 / 9`
- Validates the input with `TryParse`
- Rejects values below absolute zero (`-459.67 °F`)
- `Converter` button and Enter-key shortcut
- `Limpar` button that clears the form and returns focus to the input
- Result is blue below 0 °C and red above 30 °C
- Culture-aware parsing and formatting: both `98.6` and `98,6` work

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
| `Program.cs` | MewUI window, controls, result colors, and event handling |
| `TemperatureConverter.cs` | Parsing, validation, and Fahrenheit → Celsius conversion |
| `TemperaturaConverter.csproj` | .NET project and MewUI package reference |

## Example values

| Fahrenheit | Celsius |
| ---: | ---: |
| 32 | 0.00 °C |
| 212 | 100.00 °C |
| -40 | -40.00 °C |
| 98.6 | 37.00 °C |

## Naming conventions

The project uses standard C# naming conventions:

- `PascalCase` for types, methods, properties, and constants
- `camelCase` for local variables and parameters
- Descriptive names such as `fahrenheitTextBox`, `resultLabel`, and `errorMessage`
