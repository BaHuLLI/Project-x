namespace ProjectXProDash.Models;

public sealed class TelemetryData
{
    public double SpeedKph { get; set; } = 300;

    public double TorqueNm { get; set; } = 18;

    public double TempC { get; set; } = 44;

    public double BusVoltage { get; set; } = 11.7;

    public double GForce { get; set; } = 1.09;

    public double LatencyMs { get; set; } = 3.3;

    public double ArcNormalized { get; set; } = 0.82;

    public double PeakLoad { get; set; } = 0.84;

    public int ActiveLedCount { get; set; } = 12;

    public bool IsConnected { get; set; } = true;

    public bool IsDisplayEnabled { get; set; } = true;

    public double AnimationTime { get; set; }

    public string SpeedLabel { get; set; } = "300 KPH";

    public string LatencyLabel { get; set; } = "3.3 ms";

    public string DeviceStatusLabel { get; set; } = "Connected";

    public string DeviceModeLabel { get; set; } = "Low Latency Mode";

    public string GameStatusLabel { get; set; } = "Preview Armed";

    public string DeviceInfoLabel { get; set; } = "PROJECT-X Control Surface";

    public string FrameRateLabel { get; set; } = "60 FPS";

    public void CopyFrom(TelemetryData other)
    {
        SpeedKph = other.SpeedKph;
        TorqueNm = other.TorqueNm;
        TempC = other.TempC;
        BusVoltage = other.BusVoltage;
        GForce = other.GForce;
        LatencyMs = other.LatencyMs;
        ArcNormalized = other.ArcNormalized;
        PeakLoad = other.PeakLoad;
        ActiveLedCount = other.ActiveLedCount;
        IsConnected = other.IsConnected;
        IsDisplayEnabled = other.IsDisplayEnabled;
        AnimationTime = other.AnimationTime;
        SpeedLabel = other.SpeedLabel;
        LatencyLabel = other.LatencyLabel;
        DeviceStatusLabel = other.DeviceStatusLabel;
        DeviceModeLabel = other.DeviceModeLabel;
        GameStatusLabel = other.GameStatusLabel;
        DeviceInfoLabel = other.DeviceInfoLabel;
        FrameRateLabel = other.FrameRateLabel;
    }
}
