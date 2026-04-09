using ProjectXProDash.Core;
using ProjectXProDash.Models;

namespace ProjectXProDash.Services;

public sealed class TelemetryProvider : IDisposable
{
    private readonly IFrameClock _frameClock;
    private readonly MockTelemetryProvider _mockTelemetryProvider;
    private readonly TelemetryData _smoothedTelemetry = new();
    private readonly TelemetryData _targetTelemetry = new();
    private int _eventDivider;
    private bool _disposed;

    public TelemetryProvider(IFrameClock frameClock, MockTelemetryProvider mockTelemetryProvider)
    {
        _frameClock = frameClock;
        _mockTelemetryProvider = mockTelemetryProvider;
        _targetTelemetry.CopyFrom(_mockTelemetryProvider.Snapshot);
        _smoothedTelemetry.CopyFrom(_mockTelemetryProvider.Snapshot);
        _frameClock.FrameArrived += OnFrameArrived;
        _mockTelemetryProvider.TelemetryUpdated += OnTelemetryUpdated;
    }

    public TelemetryData Snapshot => _smoothedTelemetry;

    public event Action<TelemetryData>? TelemetryUpdated;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _frameClock.FrameArrived -= OnFrameArrived;
        _mockTelemetryProvider.TelemetryUpdated -= OnTelemetryUpdated;
        _disposed = true;
    }

    private void OnTelemetryUpdated(TelemetryData telemetryData) => _targetTelemetry.CopyFrom(telemetryData);

    private void OnFrameArrived(double deltaSeconds, double framesPerSecond)
    {
        _smoothedTelemetry.SpeedKph = SmoothExpo(_smoothedTelemetry.SpeedKph, _targetTelemetry.SpeedKph, deltaSeconds, 10.5);
        _smoothedTelemetry.TorqueNm = SmoothExpo(_smoothedTelemetry.TorqueNm, _targetTelemetry.TorqueNm, deltaSeconds, 8.0);
        _smoothedTelemetry.TempC = SmoothExpo(_smoothedTelemetry.TempC, _targetTelemetry.TempC, deltaSeconds, 6.0);
        _smoothedTelemetry.BusVoltage = SmoothExpo(_smoothedTelemetry.BusVoltage, _targetTelemetry.BusVoltage, deltaSeconds, 7.0);
        _smoothedTelemetry.GForce = SmoothExpo(_smoothedTelemetry.GForce, _targetTelemetry.GForce, deltaSeconds, 8.5);
        _smoothedTelemetry.LatencyMs = SmoothExpo(_smoothedTelemetry.LatencyMs, _targetTelemetry.LatencyMs, deltaSeconds, 9.0);
        _smoothedTelemetry.ArcNormalized = SmoothExpo(_smoothedTelemetry.ArcNormalized, _targetTelemetry.ArcNormalized, deltaSeconds, 11.0);
        _smoothedTelemetry.PeakLoad = SmoothExpo(_smoothedTelemetry.PeakLoad, _targetTelemetry.PeakLoad, deltaSeconds, 12.0);
        _smoothedTelemetry.AnimationTime = _targetTelemetry.AnimationTime;
        _smoothedTelemetry.ActiveLedCount = (int)Math.Round(_smoothedTelemetry.ArcNormalized * 16.0);
        _smoothedTelemetry.IsConnected = _targetTelemetry.IsConnected;
        _smoothedTelemetry.IsDisplayEnabled = _targetTelemetry.IsDisplayEnabled;
        _smoothedTelemetry.SpeedLabel = $"{Math.Round(_smoothedTelemetry.SpeedKph):0} KPH";
        _smoothedTelemetry.LatencyLabel = $"{_smoothedTelemetry.LatencyMs:0.0} ms";
        _smoothedTelemetry.DeviceStatusLabel = _targetTelemetry.DeviceStatusLabel;
        _smoothedTelemetry.DeviceModeLabel = _targetTelemetry.DeviceModeLabel;
        _smoothedTelemetry.GameStatusLabel = _targetTelemetry.GameStatusLabel;
        _smoothedTelemetry.DeviceInfoLabel = _targetTelemetry.DeviceInfoLabel;
        _smoothedTelemetry.FrameRateLabel = $"{Math.Clamp((int)Math.Round(framesPerSecond), 57, 60)} FPS";

        _eventDivider++;
        if (_eventDivider < 6)
        {
            return;
        }

        _eventDivider = 0;
        TelemetryUpdated?.Invoke(_smoothedTelemetry);
    }

    private static double SmoothExpo(double current, double target, double deltaSeconds, double rate)
    {
        var blend = 1.0 - Math.Exp(-rate * deltaSeconds);
        return current + (target - current) * blend;
    }
}
