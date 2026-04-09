using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ProjectXProDash.Core;

namespace ProjectXProDash.Services;

public sealed class ThemeManager
{
    private readonly AppSettings _settings;

    public ThemeManager(AppSettings settings)
    {
        _settings = settings;
        ThemeNames = new ObservableCollection<string>
        {
            "Inferno Metal",
            "Studio Ember",
            "Copper Stage"
        };
    }

    public ObservableCollection<string> ThemeNames { get; }

    public string RajdhaniAssetState { get; private set; } = "Missing";

    public string InterAssetState { get; private set; } = "Missing";

    public void Initialize()
    {
        RajdhaniAssetState = DescribeAsset("Assets\\Fonts\\Rajdhani-Bold.ttf");
        InterAssetState = DescribeAsset("Assets\\Fonts\\Inter-Regular.ttf");

        Application.Current.Resources["TelemetryFontFamily"] = LoadFont("Assets\\Fonts\\Rajdhani-Bold.ttf", "Rajdhani", new FontFamily("Bahnschrift"));
        Application.Current.Resources["UiFontFamily"] = LoadFont("Assets\\Fonts\\Inter-Regular.ttf", "Inter", new FontFamily("Segoe UI"));
    }

    public void ApplyTheme(string themeName)
    {
        _settings.SelectedTheme = themeName;
    }

    private static string DescribeAsset(string relativePath)
    {
        var absolutePath = Path.Combine(Environment.CurrentDirectory, relativePath);
        return File.Exists(absolutePath) ? $"Loaded: {Path.GetFileName(relativePath)}" : $"Missing: {Path.GetFileName(relativePath)}";
    }

    private static FontFamily LoadFont(string relativePath, string familyName, FontFamily fallback)
    {
        var absolutePath = Path.Combine(Environment.CurrentDirectory, relativePath);
        if (!File.Exists(absolutePath))
        {
            return fallback;
        }

        try
        {
            var directoryPath = Path.GetDirectoryName(absolutePath);
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                return fallback;
            }

            return new FontFamily(new Uri(directoryPath + Path.DirectorySeparatorChar), $"./#{familyName}");
        }
        catch
        {
            return fallback;
        }
    }
}
