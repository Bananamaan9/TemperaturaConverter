# TemperaturaConverter

A desktop temperature converter built with **C#**, **.NET 10**, and
**[MewUI](https://github.com/aprillz/MewUI)**.

The app follows the Windows Forms tutorial `Conversor Fahrenheit → Celsius`,
adapted to MewUI's fluent C# markup, including all four suggested challenges.

## Features

- Two conversion directions:
  - Fahrenheit → Celsius: `°C = (°F − 32) × 5 / 9`
  - Celsius → Fahrenheit: `°F = °C × 9 / 5 + 32`
- Two `RadioButton`s select the direction and a single `Converter` button runs it
- Input validation with `TryParse`
- Rejects values below absolute zero
- Enter-key shortcut for conversion
- `Limpar` button clears the input and result and returns focus to the input
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
| `Program.cs` | MewUI window, radio buttons, controls, result colors, and event handling |
| `TemperatureConverter.cs` | Parsing, validation, and both conversion formulas |
| `TemperaturaConverter.csproj` | .NET project and MewUI package reference |

## Example values

| Direction | Input | Result |
| --- | ---: | ---: |
| °F → °C | 32 | 0.00 °C |
| °F → °C | 212 | 100.00 °C |
| °F → °C | -40 | -40.00 °C |
| °F → °C | 98.6 | 37.00 °C |
| °C → °F | 0 | 32.00 °F |
| °C → °F | 100 | 212.00 °F |
| °C → °F | -40 | -40.00 °F |
| °C → °F | 37 | 98.60 °F |

## Naming conventions

The project uses standard C# naming conventions:

- `PascalCase` for types, methods, properties, and constants
- `camelCase` for local variables and parameters
- Descriptive names such as `inputTextBox`, `resultLabel`, and `errorMessage`
