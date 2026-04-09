namespace ProjectXProDash.Services;

public sealed class HardwareBridge
{
    private readonly bool _isConnected = true;
    private readonly string _connectionLabel = "в—Џ Connected, Low Latency Mode";
    private readonly string _deviceName = "PROJECT-X Pro Controller";
    private readonly string _deviceInfo = "USB-C Hardware Bridge";

    public bool IsConnected => _isConnected;

    public string ConnectionLabel => _connectionLabel;

    public string DeviceName => _deviceName;

    public string DeviceInfo => _deviceInfo;
}
