using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectXProDash.Core;
using ProjectXProDash.Services;
using ProjectXProDash.ViewModels;
using ProjectXProDash.Views;

namespace ProjectXProDash;

public partial class App : Application
{
    private IHost? _host;

    public static IServiceProvider Services =>
        ((App)Current)._host?.Services
        ?? throw new InvalidOperationException("Services are not initialized.");

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<AppSettings>(_ => AppSettings.Load());
                services.AddSingleton<LoggerService>();
                services.AddSingleton<IFrameClock, CompositionFrameClock>();
                services.AddSingleton<MockTelemetryProvider>();
                services.AddSingleton<TelemetryProvider>();
                services.AddSingleton<HardwareBridge>();
                services.AddSingleton<ThemeManager>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<ThemesViewModel>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        var themeManager = Services.GetRequiredService<ThemeManager>();
        themeManager.Initialize();

        var settings = Services.GetRequiredService<AppSettings>();
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();
        settings.ApplyToWindow(mainWindow);
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            var settings = _host.Services.GetRequiredService<AppSettings>();
            if (MainWindow is Window window)
            {
                settings.CaptureFromWindow(window);
                settings.Save();
            }

            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
