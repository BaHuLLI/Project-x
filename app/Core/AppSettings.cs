using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace ProjectXProDash.Core;

public sealed class AppSettings
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public string Language { get; set; } = "EN";

    public string SelectedTheme { get; set; } = "Inferno Metal";

    public double WindowTop { get; set; } = 96.0;

    public double WindowLeft { get; set; } = 128.0;

    public double WindowWidth { get; set; } = 1440.0;

    public double WindowHeight { get; set; } = 860.0;

    [JsonIgnore]
    public string FilePath { get; private set; } = ResolvePath();

    public static AppSettings Load()
    {
        var filePath = ResolvePath();
        if (!File.Exists(filePath))
        {
            return new AppSettings { FilePath = filePath };
        }

        try
        {
            var loaded = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(filePath), SerializerOptions) ?? new AppSettings();
            loaded.FilePath = filePath;
            return loaded;
        }
        catch
        {
            return new AppSettings { FilePath = filePath };
        }
    }

    public void Save()
    {
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, SerializerOptions));
    }

    public void ApplyToWindow(Window window)
    {
        if (WindowWidth >= window.MinWidth)
        {
            window.Width = WindowWidth;
        }

        if (WindowHeight >= window.MinHeight)
        {
            window.Height = WindowHeight;
        }

        window.Left = WindowLeft;
        window.Top = WindowTop;
    }

    public void CaptureFromWindow(Window window)
    {
        if (window.WindowState != WindowState.Normal)
        {
            return;
        }

        WindowTop = window.Top;
        WindowLeft = window.Left;
        WindowWidth = window.Width;
        WindowHeight = window.Height;
    }

    private static string ResolvePath() => Path.Combine(Environment.CurrentDirectory, "appsettings.json");
}
