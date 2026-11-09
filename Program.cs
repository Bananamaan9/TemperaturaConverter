using Aprillz.MewUI;
using Aprillz.MewUI.Controls;

namespace TemperaturaConverter;

internal static class Program
{
    private static readonly Color ColdResultColor = Color.FromRgb(0, 102, 204);
    private static readonly Color HotResultColor = Color.FromRgb(214, 48, 48);

    [STAThread]
    private static void Main()
    {
        RegisterPlatform();
        RunApplication();
    }

    private static void RegisterPlatform()
    {
        if (OperatingSystem.IsWindows())
        {
            Win32Platform.Register();
            Direct2DBackend.Register();
        }
        else if (OperatingSystem.IsMacOS())
        {
            MacOSPlatform.Register();
            MewVGMacOSBackend.Register();
        }
        else if (OperatingSystem.IsLinux())
        {
            X11Platform.Register();
            MewVGX11Backend.Register();
        }
        else
        {
            throw new PlatformNotSupportedException(
                "MewUI runs on Windows, Linux, and macOS.");
        }
    }

    private static void RunApplication()
    {
        Color normalResultColor = Color.FromRgb(30, 30, 30);

        Label resultLabel = new Label()
            .Text("—")
            .FontSize(28)
            .Bold()
            .WithTheme((theme, label) =>
            {
                normalResultColor = theme.Palette.WindowText;
                label.Foreground(normalResultColor);
            });

        Label errorLabel = new Label()
            .Text(string.Empty)
            .TextWrapping(TextWrapping.Wrap)
            .WithTheme((theme, label) => label.Foreground(theme.Palette.Error));

        Label inputLabel = new Label()
            .Text("Temperatura em °F:");

        Label formulaLabel = new Label()
            .Text("°C = (°F − 32) × 5 / 9")
            .TextWrapping(TextWrapping.Wrap)
            .WithTheme((theme, label) =>
                label.Foreground(theme.Palette.DisabledText));

        TextBox inputTextBox = new TextBox()
            .Placeholder("Ex.: 98,6")
            .FontSize(16)
            .MinHeight(34)
            .OnTextChanged(_ => errorLabel.Text = string.Empty);

        RadioButton fahrenheitToCelsiusRadioButton = new RadioButton()
            .Content("°F → °C")
            .GroupName("ConversionDirection")
            .IsChecked(true);

        RadioButton celsiusToFahrenheitRadioButton = new RadioButton()
            .Content("°C → °F")
            .GroupName("ConversionDirection");

        ConversionDirection GetDirection() =>
            fahrenheitToCelsiusRadioButton.IsChecked
                ? ConversionDirection.FahrenheitToCelsius
                : ConversionDirection.CelsiusToFahrenheit;

        void ClearForm()
        {
            inputTextBox.Text = string.Empty;
            resultLabel.Text = "—";
            resultLabel.Foreground = normalResultColor;
            errorLabel.Text = string.Empty;
            inputTextBox.Focus();
        }

        void UpdateDirection()
        {
            bool isFahrenheitToCelsius = GetDirection() == ConversionDirection.FahrenheitToCelsius;

            inputLabel.Text = isFahrenheitToCelsius
                ? "Temperatura em °F:"
                : "Temperatura em °C:";

            formulaLabel.Text = isFahrenheitToCelsius
                ? "°C = (°F − 32) × 5 / 9"
                : "°F = °C × 9 / 5 + 32";

            inputTextBox.Placeholder(isFahrenheitToCelsius ? "Ex.: 98,6" : "Ex.: 37");

            resultLabel.Text = "—";
            resultLabel.Foreground = normalResultColor;
            errorLabel.Text = string.Empty;
        }

        void ConvertTemperature()
        {
            ConversionDirection direction = GetDirection();

            if (!TemperatureConverter.TryConvert(
                    inputTextBox.Text,
                    direction,
                    out double convertedValue,
                    out double celsiusValue,
                    out string errorMessage))
            {
                resultLabel.Text = "—";
                resultLabel.Foreground = normalResultColor;
                errorLabel.Text = errorMessage;
                inputTextBox.Focus();
                return;
            }

            resultLabel.Text = direction == ConversionDirection.FahrenheitToCelsius
                ? TemperatureConverter.FormatCelsius(convertedValue)
                : TemperatureConverter.FormatFahrenheit(convertedValue);

            resultLabel.Foreground = celsiusValue < 0
                ? ColdResultColor
                : celsiusValue > 30
                    ? HotResultColor
                    : normalResultColor;

            errorLabel.Text = string.Empty;
        }

        Button convertButton = new Button()
            .Content("Converter")
            .MinHeight(38)
            .OnClick(ConvertTemperature)
            .WithTheme((theme, button) =>
            {
                button.Background(theme.Palette.Accent);
                button.Foreground(theme.Palette.AccentText);
            });

        Button clearButton = new Button()
            .Content("Limpar")
            .Width(90)
            .MinHeight(38)
            .OnClick(ClearForm);

        fahrenheitToCelsiusRadioButton.OnChecked(UpdateDirection);
        celsiusToFahrenheitRadioButton.OnChecked(UpdateDirection);

        Window window = new Window()
            .Title("Conversor de Temperatura")
            .Fixed(480, 420)
            .CanMaximize(false)
            .StartCenterScreen()
            .Padding(20)
            .Content(
                new StackPanel()
                    .Spacing(10)
                    .Children(
                        new Label()
                            .Text("Conversor de Temperatura")
                            .FontSize(22)
                            .Bold(),

                        formulaLabel,

                        new StackPanel()
                            .Horizontal()
                            .Spacing(16)
                            .Children(
                                new Label()
                                    .Text("Conversão:")
                                    .CenterVertical(),

                                fahrenheitToCelsiusRadioButton,
                                celsiusToFahrenheitRadioButton),

                        inputLabel,
                        inputTextBox,

                        new Grid()
                            .Columns("*,Auto")
                            .Spacing(10)
                            .Children(
                                convertButton,

                                clearButton
                                    .Column(1)),

                        resultLabel,
                        errorLabel,

                        new Label()
                            .Text("Cores com base na temperatura em °C: azul abaixo de 0 °C, vermelho acima de 30 °C.")
                            .FontSize(11)
                            .TextWrapping(TextWrapping.Wrap)
                            .WithTheme((theme, label) =>
                                label.Foreground(theme.Palette.DisabledText))))
            .OnPreviewKeyDown(keyEvent =>
            {
                if (keyEvent.Key == Key.Enter)
                {
                    ConvertTemperature();
                    keyEvent.Handled = true;
                }
            })
            .OnLoaded(() => inputTextBox.Focus());

        UpdateDirection();
        Application.Run(window);
    }
}
