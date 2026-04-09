using ProjectXProDash.Models;

namespace ProjectXProDash.UnitTests;

public sealed class TelemetryDataTests
{
    [Fact]
    public void CopyFromCopiesEveryPublicValue()
    {
        var source = new TelemetryData
        {
            SpeedKph = 287.5,
            TorqueNm = 16.8,
            TempC = 47.2,
            BusVoltage = 11.9,
            GForce = 1.18,
            LatencyMs = 2.8,
            ArcNormalized = 0.74,
            PeakLoad = 0.81,
            ActiveLedCount = 14,
            IsConnected = false,
            IsDisplayEnabled = false,
            AnimationTime = 12.4,
            SpeedLabel = "288 KPH",
            LatencyLabel = "2.8 ms",
            DeviceStatusLabel = "Disconnected",
            DeviceModeLabel = "Diagnostics",
            GameStatusLabel = "Idle",
            DeviceInfoLabel = "Bench Rig",
            FrameRateLabel = "58 FPS"
        };

        var target = new TelemetryData();

        target.CopyFrom(source);

        Assert.Equivalent(source, target);
    }
}

