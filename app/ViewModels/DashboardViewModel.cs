using CommunityToolkit.Mvvm.ComponentModel;
using ProjectXProDash.Models;
using ProjectXProDash.Services;

namespace ProjectXProDash.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly TelemetryProvider _telemetryProvider;

    public DashboardViewModel(TelemetryProvider telemetryProvider, HardwareBridge hardwareBridge)
    {
        _telemetryProvider = telemetryProvider;
        HeaderStatus = hardwareBridge.ConnectionLabel;
        StartPreviewLabel = "Start Preview";
        GameStatus = telemetryProvider.Snapshot.GameStatusLabel;
        DeviceInfo = hardwareBridge.DeviceInfo;
        DeviceName = hardwareBridge.DeviceName;
        FrameRate = telemetryProvider.Snapshot.FrameRateLabel;
        Speed = telemetryProvider.Snapshot.SpeedLabel;
        Latency = telemetryProvider.Snapshot.LatencyLabel;

        _telemetryProvider.TelemetryUpdated += OnTelemetryUpdated;
    }

    [ObservableProperty]
    private string headerStatus;

    [ObservableProperty]
    private string startPreviewLabel;

    [ObservableProperty]
    private string gameStatus;

    [ObservableProperty]
    private string deviceInfo;

    [ObservableProperty]
    private string deviceName;

    [ObservableProperty]
    private string frameRate;

    [ObservableProperty]
    private string speed;

    [ObservableProperty]
    private string latency;

    private void OnTelemetryUpdated(TelemetryData telemetryData)
    {
        FrameRate = telemetryData.FrameRateLabel;
        Speed = telemetryData.SpeedLabel;
        Latency = telemetryData.LatencyLabel;
    }
}
