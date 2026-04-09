using ProjectXProDash.Models;
using ProjectXProDash.Services;

namespace ProjectXProDash.SmokeTests;

public sealed class MockTelemetryProviderSmokeTests
{
    [Fact]
    [Trait("Category", "Smoke")]
    public void EmitsConnectedTelemetryWithinExpectedBounds()
    {
        var frameClock = new ManualFrameClock();
        using var provider = new MockTelemetryProvider(frameClock);

        TelemetryData? observed = null;
        provider.TelemetryUpdated += telemetry => observed = telemetry;

        frameClock.Step(1.0 / 18.0, 60.0);

        Assert.NotNull(observed);
        Assert.InRange(observed!.SpeedKph, 232.0, 328.0);
        Assert.InRange(observed.TorqueNm, 15.0, 19.0);
        Assert.InRange(observed.ArcNormalized, 0.08, 0.98);
        Assert.InRange(observed.ActiveLedCount, 0, 16);
        Assert.Equal("Connected", observed.DeviceStatusLabel);
        Assert.True(observed.IsConnected);
        Assert.True(observed.IsDisplayEnabled);
    }
}

