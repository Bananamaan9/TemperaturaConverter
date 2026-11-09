using System.Globalization;

namespace TemperaturaConverter;

internal static class TemperatureConverter
{
    public const double FahrenheitAbsoluteZero = -459.67;

    public static bool TryConvertFahrenheitToCelsius(
        string text,
        out double celsius,
        out string errorMessage)
    {
        celsius = 0.0;
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(text))
        {
            errorMessage = "Introduza uma temperatura.";
            return false;
        }

        if (!TryParseFahrenheit(text, out double fahrenheit))
        {
            errorMessage = "Introduza um número válido.";
            return false;
        }

        if (!double.IsFinite(fahrenheit))
        {
            errorMessage = "Introduza um número finito.";
            return false;
        }

        if (fahrenheit < FahrenheitAbsoluteZero)
        {
            errorMessage =
                "A temperatura não pode ser inferior ao zero absoluto " +
                $"({FormatFahrenheit(FahrenheitAbsoluteZero)}).";
            return false;
        }

        celsius = (fahrenheit - 32.0) * 5.0 / 9.0;
        return true;
    }

    public static string FormatCelsius(double celsius) =>
        $"{celsius.ToString("F2", CultureInfo.CurrentCulture)} °C";

    public static string FormatFahrenheit(double fahrenheit) =>
        $"{fahrenheit.ToString("F2", CultureInfo.CurrentCulture)} °F";

    private static bool TryParseFahrenheit(string text, out double fahrenheit)
    {
        const NumberStyles numberStyles = NumberStyles.Float;

        if (double.TryParse(text, numberStyles, CultureInfo.CurrentCulture, out fahrenheit))
        {
            return true;
        }

        return double.TryParse(text, numberStyles, CultureInfo.InvariantCulture, out fahrenheit);
    }
}
