using ProjectXProDash.Core;
using ProjectXProDash.Models;

namespace ProjectXProDash.Services;

public sealed class MockTelemetryProvider : IDisposable
{
    private readonly IFrameClock _frameClock;
    private readonly TelemetryData _snapshot = new();
    private readonly Random _random = new(42);
    private double _time;
    private double _sampleAccumulator;
    private bool _disposed;

    public MockTelemetryProvider(IFrameClock frameClock)
    {
        _frameClock = frameClock;
        _frameClock.FrameArrived += OnFrameArrived;
        UpdateSample();
    }

    public TelemetryData Snapshot => _snapshot;

    public event Action<TelemetryData>? TelemetryUpdated;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _frameClock.FrameArrived -= OnFrameArrived;
        _disposed = true;
    }

    private void OnFrameArrived(double deltaSeconds, double framesPerSecond)
    {
        _time += deltaSeconds;
        _sampleAccumulator += deltaSeconds;
        if (_sampleAccumulator < 1.0 / 18.0)
        {
            return;
        }

        _sampleAccumulator = 0.0;
        UpdateSample();
        TelemetryUpdated?.Invoke(_snapshot);
    }

    private void UpdateSample()
    {
        var speed = 280.0
            + 22.0 * Math.Sin(_time * 0.62)
            + 11.0 * Math.Sin(_time * 1.44)
            + (_random.NextDouble() - 0.5) * 5.0;
        speed = Math.Clamp(speed, 232.0, 328.0);

        var torque = 17.2
            + 1.0 * Math.Sin(_time * 0.91)
            + (_random.NextDouble() - 0.5) * 0.35;

        var temp = 43.2
            + 2.2 * Math.Sin(_time * 0.38)
            + (_random.NextDouble() - 0.5) * 0.4;

        var bus = 11.7
            + 0.18 * Math.Sin(_time * 0.54)
            + (_random.NextDouble() - 0.5) * 0.05;

        var gForce = 1.03
            + 0.14 * Math.Sin(_time * 1.12)
            + (_random.NextDouble() - 0.5) * 0.03;

        var latency = 3.15
            + 0.22 * Math.Sin(_time * 0.86)
            + (_random.NextDouble() - 0.5) * 0.08;

        var arc = Math.Clamp((speed - 210.0) / 130.0, 0.08, 0.98);
        var ledCount = Math.Clamp((int)Math.Round(arc * 16.0), 0, 16);
        var peakLoad = Math.Clamp(arc + 0.08 * Math.Sin(_time * 5.6), 0.0, 1.0);

        _snapshot.SpeedKph = speed;
        _snapshot.TorqueNm = torque;
        _snapshot.TempC = temp;
        _snapshot.BusVoltage = bus;
        _snapshot.GForce = gForce;
        _snapshot.LatencyMs = latency;
        _snapshot.ArcNormalized = arc;
        _snapshot.PeakLoad = peakLoad;
        _snapshot.ActiveLedCount = ledCount;
        _snapshot.IsConnected = true;
        _snapshot.IsDisplayEnabled = true;
        _snapshot.AnimationTime = _time;
        _snapshot.SpeedLabel = $"{Math.Round(speed):0} KPH";
        _snapshot.LatencyLabel = $"{latency:0.0} ms";
        _snapshot.DeviceStatusLabel = "Connected";
        _snapshot.DeviceModeLabel = "Low Latency Mode";
        _snapshot.GameStatusLabel = "Preview Armed";
        _snapshot.DeviceInfoLabel = "PROJECT-X Control Surface";
    }
}
