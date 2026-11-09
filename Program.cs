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

        TextBox fahrenheitTextBox = new TextBox()
            .Placeholder("Ex.: 98,6")
            .FontSize(16)
            .MinHeight(34)
            .OnTextChanged(_ => errorLabel.Text = string.Empty);

        void ClearForm()
        {
            fahrenheitTextBox.Text = string.Empty;
            resultLabel.Text = "—";
            resultLabel.Foreground = normalResultColor;
            errorLabel.Text = string.Empty;
            fahrenheitTextBox.Focus();
        }

        void ConvertTemperature()
        {
            if (!TemperatureConverter.TryConvertFahrenheitToCelsius(
                    fahrenheitTextBox.Text,
                    out double celsius,
                    out string errorMessage))
            {
                resultLabel.Text = "—";
                resultLabel.Foreground = normalResultColor;
                errorLabel.Text = errorMessage;
                fahrenheitTextBox.Focus();
                return;
            }

            resultLabel.Text = TemperatureConverter.FormatCelsius(celsius);
            resultLabel.Foreground = celsius < 0
                ? ColdResultColor
                : celsius > 30
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

        Window window = new Window()
            .Title("Conversor de Temperatura")
            .Fixed(480, 360)
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

                        new Label()
                            .Text("°C = (°F − 32) × 5 / 9")
                            .TextWrapping(TextWrapping.Wrap)
                            .WithTheme((theme, label) =>
                                label.Foreground(theme.Palette.DisabledText)),

                        new Label()
                            .Text("Temperatura em °F:"),

                        fahrenheitTextBox,

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
                            .Text("Azul abaixo de 0 °C · Vermelho acima de 30 °C")
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
            .OnLoaded(() => fahrenheitTextBox.Focus());

        Application.Run(window);
    }
}
