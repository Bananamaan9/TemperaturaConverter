using System.Globalization;

namespace TemperaturaConverter;

internal enum ConversionDirection
{
    FahrenheitToCelsius,
    CelsiusToFahrenheit
}

internal static class TemperatureConverter
{
    public const double CelsiusAbsoluteZero = -273.15;
    public const double FahrenheitAbsoluteZero = -459.67;

    public static bool TryConvert(
        string text,
        ConversionDirection direction,
        out double convertedValue,
        out double celsiusValue,
        out string errorMessage)
    {
        convertedValue = 0.0;
        celsiusValue = 0.0;
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(text))
        {
            errorMessage = "Introduza uma temperatura.";
            return false;
        }

        if (!TryParseNumber(text, out double value))
        {
            errorMessage = "Introduza um número válido.";
            return false;
        }

        if (!double.IsFinite(value))
        {
            errorMessage = "Introduza um número finito.";
            return false;
        }

        switch (direction)
        {
            case ConversionDirection.FahrenheitToCelsius:
                if (value < FahrenheitAbsoluteZero)
                {
                    errorMessage =
                        "A temperatura não pode ser inferior ao zero absoluto " +
                        $"({FormatFahrenheit(FahrenheitAbsoluteZero)}).";
                    return false;
                }

                celsiusValue = (value - 32.0) * 5.0 / 9.0;
                convertedValue = celsiusValue;
                break;

            case ConversionDirection.CelsiusToFahrenheit:
                if (value < CelsiusAbsoluteZero)
                {
                    errorMessage =
                        "A temperatura não pode ser inferior ao zero absoluto " +
                        $"({FormatCelsius(CelsiusAbsoluteZero)}).";
                    return false;
                }

                celsiusValue = value;
                convertedValue = value * 9.0 / 5.0 + 32.0;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(direction));
        }

        if (!double.IsFinite(convertedValue))
        {
            errorMessage = "O valor convertido está fora do intervalo suportado.";
            return false;
        }

        return true;
    }

    public static string FormatCelsius(double celsius) =>
        $"{celsius.ToString("F2", CultureInfo.CurrentCulture)} °C";

    public static string FormatFahrenheit(double fahrenheit) =>
        $"{fahrenheit.ToString("F2", CultureInfo.CurrentCulture)} °F";

    private static bool TryParseNumber(string text, out double value)
    {
        const NumberStyles numberStyles = NumberStyles.Float;

        if (double.TryParse(text, numberStyles, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        return double.TryParse(text, numberStyles, CultureInfo.InvariantCulture, out value);
    }
}
