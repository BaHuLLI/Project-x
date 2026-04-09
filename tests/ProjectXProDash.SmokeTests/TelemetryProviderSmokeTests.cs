using ProjectXProDash.Models;
using ProjectXProDash.Services;

namespace ProjectXProDash.SmokeTests;

public sealed class TelemetryProviderSmokeTests
{
    [Fact]
    [Trait("Category", "Smoke")]
    public void ProducesSmoothedTelemetryAndRaisesPeriodicUpdates()
    {
        var frameClock = new ManualFrameClock();
        using var mockTelemetryProvider = new MockTelemetryProvider(frameClock);
        using var provider = new TelemetryProvider(frameClock, mockTelemetryProvider);

        TelemetryData? observed = null;
        provider.TelemetryUpdated += telemetry => observed = telemetry;

        for (var index = 0; index < 18; index++)
        {
            frameClock.Step(1.0 / 60.0, 59.7);
        }

        Assert.NotNull(observed);
        Assert.InRange(observed!.SpeedKph, 232.0, 328.0);
        Assert.InRange(observed.ArcNormalized, 0.0, 1.0);
        Assert.InRange(observed.ActiveLedCount, 0, 16);
        Assert.Equal("Connected", observed.DeviceStatusLabel);
        Assert.Equal("60 FPS", observed.FrameRateLabel);
        Assert.EndsWith("KPH", observed.SpeedLabel, StringComparison.Ordinal);
    }
}
