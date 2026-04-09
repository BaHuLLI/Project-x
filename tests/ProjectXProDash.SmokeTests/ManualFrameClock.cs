using ProjectXProDash.Core;

namespace ProjectXProDash.SmokeTests;

internal sealed class ManualFrameClock : IFrameClock
{
    public event Action<double, double>? FrameArrived;

    public double CurrentFramesPerSecond { get; private set; } = 60.0;

    public void Step(double deltaSeconds, double framesPerSecond = 60.0)
    {
        CurrentFramesPerSecond = framesPerSecond;
        FrameArrived?.Invoke(deltaSeconds, framesPerSecond);
    }
}

