using Aprillz.MewUI;
using Aprillz.MewUI.Controls;

namespace TemperaturaConverter;

internal static class Program
{
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
        TextBox inputTextBox = new TextBox()
            .Text("0")
            .Placeholder("Enter a temperature");

        ComboBox sourceUnitComboBox = new ComboBox()
            .Items(TemperatureConverter.UnitDisplayNames)
            .SelectedIndex(0);

        ComboBox targetUnitComboBox = new ComboBox()
            .Items(TemperatureConverter.UnitDisplayNames)
            .SelectedIndex(1);

        Label resultValueLabel = new Label()
            .Text("32 °F")
            .FontSize(34)
            .Bold()
            .WithTheme((theme, label) => label.Foreground(theme.Palette.Accent));

        Label resultDetailsLabel = new Label()
            .Text("0 °C = 32 °F")
            .TextWrapping(TextWrapping.Wrap)
            .WithTheme((theme, label) => label.Foreground(theme.Palette.DisabledText));

        Label errorLabel = new Label()
            .Text(string.Empty)
            .TextWrapping(TextWrapping.Wrap)
            .WithTheme((theme, label) => label.Foreground(theme.Palette.Error));

        inputTextBox.OnTextChanged(_ => UpdateConversion());
        sourceUnitComboBox.OnSelectionChanged(_ => UpdateConversion());
        targetUnitComboBox.OnSelectionChanged(_ => UpdateConversion());

        void ShowError(string message)
        {
            resultValueLabel.Text = "—";
            resultDetailsLabel.Text = string.Empty;
            errorLabel.Text = message;
        }

        void UpdateConversion()
        {
            int sourceUnitIndex = sourceUnitComboBox.SelectedIndex;
            int targetUnitIndex = targetUnitComboBox.SelectedIndex;

            if (sourceUnitIndex < 0 ||
                sourceUnitIndex >= TemperatureConverter.Units.Count ||
                targetUnitIndex < 0 ||
                targetUnitIndex >= TemperatureConverter.Units.Count)
            {
                ShowError("Choose a source unit and a target unit.");
                return;
            }

            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                ShowError("Enter a temperature.");
                return;
            }

            if (!TemperatureConverter.TryParseTemperature(inputTextBox.Text, out double sourceValue))
            {
                ShowError("Enter a valid number.");
                return;
            }

            TemperatureUnit sourceUnit = TemperatureConverter.Units[sourceUnitIndex];
            TemperatureUnit targetUnit = TemperatureConverter.Units[targetUnitIndex];

            if (!TemperatureConverter.TryConvert(
                    sourceValue,
                    sourceUnit,
                    targetUnit,
                    out double targetValue,
                    out string errorMessage))
            {
                ShowError(errorMessage);
                return;
            }

            resultValueLabel.Text =
                $"{TemperatureConverter.Format(targetValue)} {targetUnit.Symbol}";
            resultDetailsLabel.Text =
                $"{TemperatureConverter.Format(sourceValue)} {sourceUnit.Symbol} = " +
                $"{TemperatureConverter.Format(targetValue)} {targetUnit.Symbol}";
            errorLabel.Text = string.Empty;
        }

        void SwapUnits()
        {
            int sourceUnitIndex = sourceUnitComboBox.SelectedIndex;

            sourceUnitComboBox.SelectedIndex = targetUnitComboBox.SelectedIndex;
            targetUnitComboBox.SelectedIndex = sourceUnitIndex;

            UpdateConversion();
        }

        void ResetForm()
        {
            inputTextBox.Text = "0";
            sourceUnitComboBox.SelectedIndex = 0;
            targetUnitComboBox.SelectedIndex = 1;

            UpdateConversion();
        }

        Button primaryButton = new Button()
            .Content("Convert")
            .MinHeight(40)
            .OnClick(UpdateConversion)
            .WithTheme((theme, button) =>
            {
                button.Background(theme.Palette.Accent);
                button.Foreground(theme.Palette.AccentText);
            });

        GroupBox inputGroupBox = new GroupBox()
            .Header("Input")
            .Content(
                new StackPanel()
                    .Spacing(10)
                    .Children(
                        new Label().Text("Temperature"),
                        inputTextBox,
                        new Grid()
                            .Columns("*,*")
                            .Spacing(12)
                            .Children(
                                new StackPanel()
                                    .Spacing(6)
                                    .Children(
                                        new Label().Text("From"),
                                        sourceUnitComboBox),

                                new StackPanel()
                                    .Spacing(6)
                                    .Column(1)
                                    .Children(
                                        new Label().Text("To"),
                                        targetUnitComboBox))));

        GroupBox resultGroupBox = new GroupBox()
            .Header("Result")
            .Content(
                new StackPanel()
                    .Spacing(6)
                    .Children(
                        resultValueLabel,
                        resultDetailsLabel,
                        errorLabel));

        Grid actionGrid = new Grid()
            .Columns("*,Auto,Auto")
            .Spacing(10)
            .Children(
                primaryButton,

                new Button()
                    .Content("Swap")
                    .MinHeight(40)
                    .OnClick(SwapUnits)
                    .Column(1),

                new Button()
                    .Content("Reset")
                    .MinHeight(40)
                    .OnClick(ResetForm)
                    .Column(2));

        Window window = new Window()
            .Title("Temperature Converter")
            .Resizable(640, 620)
            .Padding(24)
            .Content(
                new StackPanel()
                    .Spacing(16)
                    .Children(
                        new Label()
                            .Text("Temperature Converter")
                            .FontSize(24)
                            .Bold(),

                        new Label()
                            .Text("Convert temperatures between Celsius, Fahrenheit, and Kelvin.")
                            .TextWrapping(TextWrapping.Wrap)
                            .WithTheme((theme, label) =>
                                label.Foreground(theme.Palette.DisabledText)),

                        inputGroupBox,
                        resultGroupBox,
                        actionGrid,

                        new Label()
                            .Text("°F = (°C × 9/5) + 32   |   K = °C + 273.15")
                            .FontSize(11)
                            .TextWrapping(TextWrapping.Wrap)
                            .WithTheme((theme, label) =>
                                label.Foreground(theme.Palette.DisabledText))))
            .OnPreviewKeyDown(keyEvent =>
            {
                if (keyEvent.Key == Key.Enter)
                {
                    UpdateConversion();
                    keyEvent.Handled = true;
                }
            });

        UpdateConversion();
        Application.Run(window);
    }
}
