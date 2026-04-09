using System.Diagnostics;
using System.Windows.Media;

namespace ProjectXProDash.Core;

public interface IFrameClock
{
    event Action<double, double>? FrameArrived;

    double CurrentFramesPerSecond { get; }
}

public sealed class CompositionFrameClock : IFrameClock, IDisposable
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private long _lastTicks;
    private bool _disposed;

    public CompositionFrameClock()
    {
        _lastTicks = _stopwatch.ElapsedTicks;
        CompositionTarget.Rendering += OnRendering;
    }

    public event Action<double, double>? FrameArrived;

    public double CurrentFramesPerSecond { get; private set; } = 60.0;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        CompositionTarget.Rendering -= OnRendering;
        _disposed = true;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        var ticks = _stopwatch.ElapsedTicks;
        var deltaTicks = ticks - _lastTicks;
        _lastTicks = ticks;

        if (deltaTicks <= 0)
        {
            return;
        }

        var deltaSeconds = deltaTicks / (double)Stopwatch.Frequency;
        CurrentFramesPerSecond = deltaSeconds > 0 ? 1.0 / deltaSeconds : 60.0;
        FrameArrived?.Invoke(deltaSeconds, CurrentFramesPerSecond);
    }
}
