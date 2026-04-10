using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using ProjectXProDash.Core;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace ProjectXProDash.Renderers;

public sealed class MainCanvasRenderer : Border, IDisposable
{
    private readonly SKElement _surface;
    private readonly IFrameClock _frameClock;

    private readonly SKPaint _backgroundPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        Color = new SKColor(5, 5, 5)
    };

    private readonly SKPaint _panelPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        Color = new SKColor(0, 0, 0)
    };

    private readonly SKPaint _borderPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(52, 52, 52)
    };

    private readonly SKPaint _gridPaint = new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f,
        Color = new SKColor(255, 255, 255, 10)
    };

    private bool _disposed;

    public MainCanvasRenderer()
    {
        _frameClock = App.Services.GetRequiredService<IFrameClock>();

        _surface = new SKElement
        {
            IgnorePixelScaling = true
        };

        BorderThickness = new Thickness(0);
        Background = Brushes.Transparent;
        Child = _surface;

        _surface.PaintSurface += OnPaintSurface;
        _frameClock.FrameArrived += OnFrameArrived;
        SizeChanged += OnSizeChanged;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _surface.PaintSurface -= OnPaintSurface;
        _frameClock.FrameArrived -= OnFrameArrived;
        SizeChanged -= OnSizeChanged;

        _backgroundPaint.Dispose();
        _panelPaint.Dispose();
        _borderPaint.Dispose();
        _gridPaint.Dispose();
    }

    private void OnFrameArrived(double deltaSeconds, double framesPerSecond)
    {
        _ = deltaSeconds;
        _ = framesPerSecond;
        _surface.InvalidateVisual();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _surface.InvalidateVisual();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var width = e.Info.Width;
        var height = e.Info.Height;

        canvas.Clear(SKColors.Transparent);
        canvas.DrawRect(new SKRect(0, 0, width, height), _backgroundPaint);

        var panelRect = new SKRect(12f, 12f, width - 12f, height - 12f);
        canvas.DrawRoundRect(panelRect, 12f, 12f, _panelPaint);
        canvas.DrawRoundRect(panelRect, 12f, 12f, _borderPaint);

        DrawGuideGrid(canvas, panelRect);
    }

    private void DrawGuideGrid(SKCanvas canvas, SKRect rect)
    {
        var horizontalStep = Math.Max(40f, rect.Height / 8f);
        var verticalStep = Math.Max(40f, rect.Width / 8f);

        for (var y = rect.Top + horizontalStep; y < rect.Bottom; y += horizontalStep)
        {
            canvas.DrawLine(rect.Left, y, rect.Right, y, _gridPaint);
        }

        for (var x = rect.Left + verticalStep; x < rect.Right; x += verticalStep)
        {
            canvas.DrawLine(x, rect.Top, x, rect.Bottom, _gridPaint);
        }
    }
}
