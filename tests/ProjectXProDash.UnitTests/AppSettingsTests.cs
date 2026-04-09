using System.IO;
using ProjectXProDash.Core;

namespace ProjectXProDash.UnitTests;

public sealed class AppSettingsTests
{
    [Fact]
    public void LoadReturnsDefaultsWhenSettingsFileIsMissing()
    {
        using var scope = new CurrentDirectoryScope();

        var settings = AppSettings.Load();

        Assert.Equal("EN", settings.Language);
        Assert.Equal("Inferno Metal", settings.SelectedTheme);
        Assert.Equal(Path.Combine(scope.DirectoryPath, "appsettings.json"), settings.FilePath);
    }

    [Fact]
    public void LoadReturnsDefaultsWhenSettingsFileIsInvalid()
    {
        using var scope = new CurrentDirectoryScope();
        File.WriteAllText(Path.Combine(scope.DirectoryPath, "appsettings.json"), "{ invalid json");

        var settings = AppSettings.Load();

        Assert.Equal("EN", settings.Language);
        Assert.Equal("Inferno Metal", settings.SelectedTheme);
    }

    [Fact]
    public void SaveRoundTripsConfiguredValues()
    {
        using var scope = new CurrentDirectoryScope();
        var settings = AppSettings.Load();

        settings.Language = "CS";
        settings.SelectedTheme = "Studio Ember";
        settings.WindowTop = 44.0;
        settings.WindowLeft = 88.0;
        settings.WindowWidth = 1660.0;
        settings.WindowHeight = 940.0;
        settings.Save();

        var reloaded = AppSettings.Load();

        Assert.Equal("CS", reloaded.Language);
        Assert.Equal("Studio Ember", reloaded.SelectedTheme);
        Assert.Equal(44.0, reloaded.WindowTop);
        Assert.Equal(88.0, reloaded.WindowLeft);
        Assert.Equal(1660.0, reloaded.WindowWidth);
        Assert.Equal(940.0, reloaded.WindowHeight);
    }

    private sealed class CurrentDirectoryScope : IDisposable
    {
        private readonly string _originalDirectory;

        public CurrentDirectoryScope()
        {
            _originalDirectory = Environment.CurrentDirectory;
            DirectoryPath = Path.Combine(Path.GetTempPath(), "ProjectXProDash.Tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(DirectoryPath);
            Environment.CurrentDirectory = DirectoryPath;
        }

        public string DirectoryPath { get; }

        public void Dispose()
        {
            Environment.CurrentDirectory = _originalDirectory;

            if (Directory.Exists(DirectoryPath))
            {
                Directory.Delete(DirectoryPath, recursive: true);
            }
        }
    }
}
