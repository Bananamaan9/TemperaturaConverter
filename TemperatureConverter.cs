using System.Globalization;

namespace TemperaturaConverter;

internal sealed record TemperatureUnit(
    string Name,
    string Symbol,
    double AbsoluteZero,
    Func<double, double> ToCelsius,
    Func<double, double> FromCelsius)
{
    public string DisplayName => $"{Name} ({Symbol})";
}

internal static class TemperatureConverter
{
    public static IReadOnlyList<TemperatureUnit> Units { get; } =
    [
        new TemperatureUnit(
            Name: "Celsius",
            Symbol: "°C",
            AbsoluteZero: -273.15,
            ToCelsius: value => value,
            FromCelsius: value => value),

        new TemperatureUnit(
            Name: "Fahrenheit",
            Symbol: "°F",
            AbsoluteZero: -459.67,
            ToCelsius: value => (value - 32.0) * 5.0 / 9.0,
            FromCelsius: value => value * 9.0 / 5.0 + 32.0),

        new TemperatureUnit(
            Name: "Kelvin",
            Symbol: "K",
            AbsoluteZero: 0.0,
            ToCelsius: value => value - 273.15,
            FromCelsius: value => value + 273.15)
    ];

    public static string[] UnitDisplayNames { get; } =
        Units.Select(unit => unit.DisplayName).ToArray();

    public static bool TryParseTemperature(string text, out double value)
    {
        const NumberStyles numberStyles = NumberStyles.Float;

        if (double.TryParse(text, numberStyles, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        return double.TryParse(text, numberStyles, CultureInfo.InvariantCulture, out value);
    }

    public static bool TryConvert(
        double sourceValue,
        TemperatureUnit sourceUnit,
        TemperatureUnit targetUnit,
        out double targetValue,
        out string errorMessage)
    {
        targetValue = 0.0;
        errorMessage = string.Empty;

        if (!double.IsFinite(sourceValue))
        {
            errorMessage = "Enter a finite number.";
            return false;
        }

        if (sourceValue < sourceUnit.AbsoluteZero)
        {
            errorMessage =
                $"Temperature cannot be below absolute zero " +
                $"({Format(sourceUnit.AbsoluteZero)} {sourceUnit.Symbol}).";
            return false;
        }

        double celsiusValue = sourceUnit.ToCelsius(sourceValue);
        targetValue = targetUnit.FromCelsius(celsiusValue);

        if (!double.IsFinite(targetValue))
        {
            errorMessage = "The converted value is outside the supported range.";
            return false;
        }

        return true;
    }

    public static string Format(double value)
    {
        double roundedValue = Math.Round(value, 3);

        return Math.Abs(roundedValue) >= 1_000_000
            ? roundedValue.ToString("0.###E+0", CultureInfo.CurrentCulture)
            : roundedValue.ToString("0.###", CultureInfo.CurrentCulture);
    }
}
