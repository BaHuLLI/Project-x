using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using ProjectXProDash.Core;
using ProjectXProDash.Models;
using ProjectXProDash.Services;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace ProjectXProDash.Renderers;

public sealed class MainCanvasRenderer : Border, IDisposable
{
    private const float BaseSceneWidth = 1200f;
    private const float BaseSceneHeight = 600f;

    private readonly SKElement _surface;
    private readonly IFrameClock _frameClock;
    private readonly TelemetryProvider _telemetryProvider;
    private readonly TelemetryData _telemetry = new();
    private readonly Random _noiseRandom = new(97);

    private readonly SKPath _deviceOuterPath = new();
    private readonly SKPath _deviceInnerPath = new();
    private readonly SKPath _lensPath = new();
    private readonly SKPath _displayPath = new();
    private readonly SKPath _fasciaPath = new();
    private readonly SKPath _rimLightPath = new();
    private readonly SKPath _baseArcPath = new();
    private readonly SKPath _activeArcPath = new();
    private readonly SKPath _sweepArcPath = new();
    private readonly SKPath _layoutCurvePath = new();
    private readonly SKPath _boltHexPath = new();
    private readonly SKBitmap _grainBitmap = new(192, 108, SKColorType.Bgra8888, SKAlphaType.Premul);
    private readonly byte[] _grainBytes = new byte[192 * 108 * 4];
    private readonly SKRect[] _recessRects = new SKRect[2];
    private readonly SKPoint[] _boltCenters = new SKPoint[4];
    private readonly SKRect[] _ledRects = new SKRect[16];

    private readonly SKPaint _backgroundPaint = NewPaint();
    private readonly SKPaint _atmospherePaint = NewPaint();
    private readonly SKPaint _globalWarmOverlayPaint = NewPaint(new SKColor(255, 138, 31, 12));
    private readonly SKPaint _leftAtmospherePaint = NewPaint(new SKColor(255, 179, 71, 5));
    private readonly SKPaint _rightAtmospherePaint = NewPaint(new SKColor(255, 138, 31, 6));
    private readonly SKPaint _rearShadowPaint = NewPaint(new SKColor(5, 5, 5, 150));
    private readonly SKPaint _contactShadowPaint = NewPaint(new SKColor(8, 7, 6, 196));
    private readonly SKPaint _reflectionPaint = NewPaint(new SKColor(243, 232, 220, 9));
    private readonly SKPaint _floorBloomPaint = NewPaint();
    private readonly SKPaint _deviceBodyPaint = NewPaint();
    private readonly SKPaint _deviceShellPaint = NewPaint();
    private readonly SKPaint _deviceHighlightPaint = NewPaint();
    private readonly SKPaint _fasciaPaint = NewPaint();
    private readonly SKPaint _clearCoatPaint = NewPaint();
    private readonly SKPaint _edgeHighlightPaint = NewStrokePaint(new SKColor(106, 82, 68, 110), 0.9f);
    private readonly SKPaint _accentEdgePaint = NewStrokePaint(new SKColor(255, 179, 71, 64), 1f);
    private readonly SKPaint _rimLightPaint = NewStrokePaint(new SKColor(255, 179, 71, 82), 2.2f);
    private readonly SKPaint _lowerRimGlowPaint = NewStrokePaint(new SKColor(255, 106, 0, 64), 2.8f);
    private readonly SKPaint _lensOverlayPaint = NewPaint();
    private readonly SKPaint _lensEdgePaint = NewStrokePaint(new SKColor(243, 232, 220, 18), 1f);
    private readonly SKPaint _recessPaint = NewPaint(new SKColor(20, 17, 16, 225));
    private readonly SKPaint _recessEdgePaint = NewStrokePaint(new SKColor(106, 82, 68, 150), 1f);
    private readonly SKPaint _boltPaint = NewPaint(new SKColor(42, 34, 29));
    private readonly SKPaint _boltEdgePaint = NewStrokePaint(new SKColor(106, 82, 68, 220), 1f);
    private readonly SKPaint _boltSlotPaint = NewStrokePaint(new SKColor(138, 120, 106, 188), 1.1f);
    private readonly SKPaint _displayDepthPaint = NewPaint();
    private readonly SKPaint _displayVignettePaint = NewPaint();
    private readonly SKPaint _displayGlowPaint = NewPaint();
    private readonly SKPaint _rpmSlotPaint = NewPaint(new SKColor(243, 232, 220, 7));
    private readonly SKPaint _layoutLinePaint = NewStrokePaint(new SKColor(199, 194, 186, 12), 1f);
    private readonly SKPaint _layoutCurvePaint = NewStrokePaint(new SKColor(199, 194, 186, 18), 1.1f);
    private readonly SKPaint _brushedLinePaint = NewStrokePaint(new SKColor(106, 82, 68, 12), 1f);
    private readonly SKPaint _grainPaint = NewPaint(new SKColor(243, 232, 220, 8), antialias: false);
    private readonly SKPaint _aberrationPaint = NewPaint(new SKColor(255, 106, 0, 5));
    private readonly SKPaint _baseArcPaint = NewStrokePaint(new SKColor(205, 186, 167, 76), 22f, SKStrokeCap.Round);
    private readonly SKPaint _tickPaint = NewStrokePaint(new SKColor(205, 186, 167, 120), 1.7f, SKStrokeCap.Round);
    private readonly SKPaint _tickTextPaint = NewPaint(new SKColor(199, 194, 186, 92));
    private readonly SKPaint _activeArcPaint = NewStrokePaint(new SKColor(255, 179, 71), 18f, SKStrokeCap.Round);
    private readonly SKPaint _activeArcGlowPaint = NewStrokePaint(new SKColor(255, 106, 0, 112), 18f, SKStrokeCap.Round);
    private readonly SKPaint _sweepArcPaint = NewStrokePaint(new SKColor(255, 118, 42, 240), 3.4f, SKStrokeCap.Round);
    private readonly SKPaint _ledInactivePaint = NewPaint(new SKColor(42, 34, 29, 120));
    private readonly SKPaint _ledActivePaint = NewPaint();
    private readonly SKPaint _ledGlowPaint = NewPaint(new SKColor(255, 106, 0, 72));
    private readonly SKPaint _ledTintPaint = NewPaint(new SKColor(255, 106, 0, 180));
    private readonly SKPaint _hotspotGlowPaint = NewPaint(new SKColor(255, 106, 0, 90));
    private readonly SKPaint _hotspotCorePaint = NewPaint(new SKColor(255, 179, 71, 80));
    private readonly SKPaint _labelPaint = NewPaint(new SKColor(199, 194, 186, 112));
    private readonly SKPaint _valueOrangePaint = NewPaint(new SKColor(255, 106, 0, 255));
    private readonly SKPaint _speedCorePaint = NewPaint(new SKColor(242, 238, 232, 255));
    private readonly SKPaint _speedGlowNearPaint = NewPaint(new SKColor(255, 106, 0, 46));
    private readonly SKPaint _speedGlowFarPaint = NewPaint(new SKColor(255, 106, 0, 18));
    private readonly SKPaint _unitPaint = NewPaint(new SKColor(255, 106, 0, 255));
    private readonly SKPaint _alertPaint = NewStrokePaint(new SKColor(255, 69, 58, 170), 4f);
    private readonly SKPaint _scanlinePaint = NewStrokePaint(new SKColor(255, 255, 255, 10), 1f);
    private readonly SKPaint _powerOffPaint = NewPaint(new SKColor(0, 0, 0, 220));
    private readonly SKPaint _powerLinePaint = NewPaint(new SKColor(242, 238, 232, 92));
    private readonly SKPaint _logoPaint = NewPaint(new SKColor(242, 238, 232, 210));
    private readonly SKPaint _lensStripPaint = NewPaint(new SKColor(255, 255, 255, 10));

    private readonly SKTypeface _rajdhaniTypeface;
    private readonly SKTypeface _interTypeface;

    private SKShader? _backgroundShader;
    private SKShader? _atmosphereShader;
    private SKShader? _floorBloomShader;
    private SKShader? _deviceBodyShader;
    private SKShader? _deviceShellShader;
    private SKShader? _deviceHighlightShader;
    private SKShader? _fasciaShader;
    private SKShader? _clearCoatShader;
    private SKShader? _lensOverlayShader;
    private SKShader? _displayDepthShader;
    private SKShader? _displayVignetteShader;
    private SKShader? _displayGlowShader;
    private SKShader? _activeArcShader;
    private SKShader? _sweepArcShader;
    private SKShader? _ledActiveShader;
    private float _deviceLeft;
    private float _deviceTop;
    private float _deviceWidth;
    private float _deviceHeight;
    private SKRect _displayRect;
    private SKRect _arcRect;
    private int _cachedWidth;
    private int _cachedHeight;
    private bool _disposed;

    public MainCanvasRenderer()
    {
        _frameClock = App.Services.GetRequiredService<IFrameClock>();
        _telemetryProvider = App.Services.GetRequiredService<TelemetryProvider>();
        _rajdhaniTypeface = LoadTypeface("ProjectXProDash.Assets.Fonts.Rajdhani-Bold.ttf", "Rajdhani");
        _interTypeface = LoadTypeface("ProjectXProDash.Assets.Fonts.Inter-Regular.ttf", "Inter");

        _surface = new SKElement
        {
            IgnorePixelScaling = true
        };

        BorderThickness = new Thickness(0);
        Background = Brushes.Transparent;
        Child = _surface;

        _rearShadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 42f);
        _contactShadowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 28f);
        _reflectionPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 18f);
        _floorBloomPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 28f);
        _globalWarmOverlayPaint.BlendMode = SKBlendMode.SoftLight;
        _activeArcGlowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 18f);
        _lowerRimGlowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 14f);
        _ledGlowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 10f);
        _hotspotGlowPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 18f);
        _hotspotCorePaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 8f);
        _speedGlowNearPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 4f);
        _speedGlowFarPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 20f);
        _alertPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 8f);

        _surface.PaintSurface += OnPaintSurface;
        _frameClock.FrameArrived += OnFrameArrived;
        SizeChanged += OnSizeChanged;

        RegenerateNoise();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _surface.PaintSurface -= OnPaintSurface;
        _frameClock.FrameArrived -= OnFrameArrived;
        SizeChanged -= OnSizeChanged;

        DisposeShaders();
        _grainBitmap.Dispose();
        _deviceOuterPath.Dispose();
        _deviceInnerPath.Dispose();
        _lensPath.Dispose();
        _displayPath.Dispose();
        _fasciaPath.Dispose();
        _rimLightPath.Dispose();
        _baseArcPath.Dispose();
        _activeArcPath.Dispose();
        _sweepArcPath.Dispose();
        _layoutCurvePath.Dispose();
        _boltHexPath.Dispose();

        _backgroundPaint.Dispose();
        _atmospherePaint.Dispose();
        _globalWarmOverlayPaint.Dispose();
        _rearShadowPaint.Dispose();
        _contactShadowPaint.Dispose();
        _reflectionPaint.Dispose();
        _floorBloomPaint.Dispose();
        _deviceBodyPaint.Dispose();
        _deviceShellPaint.Dispose();
        _deviceHighlightPaint.Dispose();
        _fasciaPaint.Dispose();
        _clearCoatPaint.Dispose();
        _edgeHighlightPaint.Dispose();
        _accentEdgePaint.Dispose();
        _rimLightPaint.Dispose();
        _lowerRimGlowPaint.Dispose();
        _lensOverlayPaint.Dispose();
        _lensEdgePaint.Dispose();
        _recessPaint.Dispose();
        _recessEdgePaint.Dispose();
        _boltPaint.Dispose();
        _boltEdgePaint.Dispose();
        _boltSlotPaint.Dispose();
        _displayDepthPaint.Dispose();
        _displayVignettePaint.Dispose();
        _displayGlowPaint.Dispose();
        _rpmSlotPaint.Dispose();
        _layoutLinePaint.Dispose();
        _layoutCurvePaint.Dispose();
        _brushedLinePaint.Dispose();
        _grainPaint.Dispose();
        _aberrationPaint.Dispose();
        _baseArcPaint.Dispose();
        _tickPaint.Dispose();
        _tickTextPaint.Dispose();
        _activeArcPaint.Dispose();
        _activeArcGlowPaint.Dispose();
        _sweepArcPaint.Dispose();
        _ledInactivePaint.Dispose();
        _ledActivePaint.Dispose();
        _ledGlowPaint.Dispose();
        _ledTintPaint.Dispose();
        _hotspotGlowPaint.Dispose();
        _hotspotCorePaint.Dispose();
        _labelPaint.Dispose();
        _valueOrangePaint.Dispose();
        _speedCorePaint.Dispose();
        _speedGlowNearPaint.Dispose();
        _speedGlowFarPaint.Dispose();
        _unitPaint.Dispose();
        _alertPaint.Dispose();
        _scanlinePaint.Dispose();
        _powerOffPaint.Dispose();
        _powerLinePaint.Dispose();
        _logoPaint.Dispose();
        _lensStripPaint.Dispose();
        _rajdhaniTypeface.Dispose();
        _interTypeface.Dispose();
        _disposed = true;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e) => _surface.InvalidateVisual();

    private void OnFrameArrived(double deltaSeconds, double framesPerSecond) => _surface.InvalidateVisual();

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var scale = MathF.Min(e.Info.Width / BaseSceneWidth, e.Info.Height / BaseSceneHeight);
        if (scale <= 0f)
        {
            return;
        }

        EnsureGeometry((int)BaseSceneWidth, (int)BaseSceneHeight);
        _telemetry.CopyFrom(_telemetryProvider.Snapshot);

        canvas.Clear(SKColors.Transparent);
        canvas.Save();
        canvas.Scale(scale, scale);
        canvas.Translate(
            (e.Info.Width / scale - BaseSceneWidth) * 0.5f,
            (e.Info.Height / scale - BaseSceneHeight) * 0.5f);

        DrawIndustrialEnvironment(canvas, (int)BaseSceneWidth, (int)BaseSceneHeight);
        DrawFloorSystem(canvas);
        DrawDeviceShadow(canvas);
        DrawDeviceBody(canvas);
        DrawDisplayInterior(canvas);
        DrawLensLayer(canvas);
        DrawHardwareDetails(canvas);
        DrawGlobalWarmOverlay(canvas, (int)BaseSceneWidth, (int)BaseSceneHeight);
        canvas.Restore();
    }

    private void EnsureGeometry(int width, int height)
    {
        if (width == _cachedWidth && height == _cachedHeight)
        {
            return;
        }

        _cachedWidth = width;
        _cachedHeight = height;
        DisposeShaders();

        _deviceWidth = MathF.Min(width * 0.66f, height * 1.18f);
        _deviceHeight = _deviceWidth * 0.56f;
        _deviceLeft = (width - _deviceWidth) * 0.5f;
        _deviceTop = MathF.Max(74f, (height - _deviceHeight) * 0.22f);

        BuildPaths();
        BuildShaders(width, height);
    }

    private void BuildPaths()
    {
        var left = _deviceLeft;
        var top = _deviceTop;
        var right = left + _deviceWidth;
        var bottom = top + _deviceHeight;

        _deviceOuterPath.Reset();
        _deviceOuterPath.MoveTo(left + 74f, bottom - 12f);
        _deviceOuterPath.LineTo(left + 26f, bottom - 86f);
        _deviceOuterPath.LineTo(left + 46f, top + 128f);
        _deviceOuterPath.CubicTo(left + 84f, top + 30f, left + _deviceWidth * 0.30f, top + 2f, left + _deviceWidth * 0.50f, top);
        _deviceOuterPath.CubicTo(left + _deviceWidth * 0.70f, top + 2f, right - 84f, top + 30f, right - 46f, top + 128f);
        _deviceOuterPath.LineTo(right - 26f, bottom - 86f);
        _deviceOuterPath.LineTo(right - 74f, bottom - 12f);
        _deviceOuterPath.QuadTo(right - 104f, bottom + 8f, right - 150f, bottom + 12f);
        _deviceOuterPath.LineTo(left + 150f, bottom + 12f);
        _deviceOuterPath.QuadTo(left + 104f, bottom + 8f, left + 74f, bottom - 12f);
        _deviceOuterPath.Close();

        var innerLeft = left + 70f;
        var innerTop = top + 52f;
        var innerRight = right - 70f;
        var innerBottom = bottom - 52f;

        _deviceInnerPath.Reset();
        _deviceInnerPath.MoveTo(innerLeft + 58f, innerBottom - 14f);
        _deviceInnerPath.LineTo(innerLeft + 30f, innerTop + 64f);
        _deviceInnerPath.CubicTo(innerLeft + 48f, innerTop + 20f, left + _deviceWidth * 0.34f, innerTop, left + _deviceWidth * 0.50f, innerTop - 4f);
        _deviceInnerPath.CubicTo(left + _deviceWidth * 0.66f, innerTop, innerRight - 48f, innerTop + 20f, innerRight - 30f, innerTop + 64f);
        _deviceInnerPath.LineTo(innerRight - 58f, innerBottom - 14f);
        _deviceInnerPath.LineTo(innerRight - 84f, innerBottom + 10f);
        _deviceInnerPath.LineTo(innerLeft + 84f, innerBottom + 10f);
        _deviceInnerPath.Close();

        var lensRect = new SKRect(innerLeft + 36f, innerTop + 28f, innerRight - 36f, innerBottom - 16f);
        _lensPath.Reset();
        _lensPath.AddRoundRect(new SKRoundRect(lensRect, 24f, 24f));

        _displayRect = new SKRect(lensRect.Left + 12f, lensRect.Top + 12f, lensRect.Right - 12f, lensRect.Bottom - 12f);
        _displayPath.Reset();
        _displayPath.AddRoundRect(new SKRoundRect(_displayRect, 18f, 18f));

        _fasciaPath.Reset();
        _fasciaPath.AddRoundRect(new SKRoundRect(new SKRect(left + 170f, bottom - 20f, right - 170f, bottom + 8f), 10f, 10f));

        _rimLightPath.Reset();
        _rimLightPath.MoveTo(innerLeft + 38f, innerTop + 42f);
        _rimLightPath.CubicTo(innerLeft + 74f, innerTop - 2f, left + _deviceWidth * 0.38f, innerTop - 20f, left + _deviceWidth * 0.50f, innerTop - 24f);
        _rimLightPath.CubicTo(left + _deviceWidth * 0.62f, innerTop - 20f, innerRight - 74f, innerTop - 2f, innerRight - 38f, innerTop + 42f);

        _recessRects[0] = new SKRect(left + 102f, top + 168f, left + 126f, top + 254f);
        _recessRects[1] = new SKRect(right - 126f, top + 168f, right - 102f, top + 254f);

        _boltHexPath.Reset();
        const float boltRadius = 10f;
        for (var i = 0; i < 6; i++)
        {
            var angle = MathF.PI / 3f * i - MathF.PI / 6f;
            var point = new SKPoint(MathF.Cos(angle) * boltRadius, MathF.Sin(angle) * boltRadius);
            if (i == 0)
            {
                _boltHexPath.MoveTo(point);
            }
            else
            {
                _boltHexPath.LineTo(point);
            }
        }

        _boltHexPath.Close();
        _boltCenters[0] = new SKPoint(left + 116f, top + 128f);
        _boltCenters[1] = new SKPoint(right - 116f, top + 128f);
        _boltCenters[2] = new SKPoint(left + 144f, bottom - 48f);
        _boltCenters[3] = new SKPoint(right - 144f, bottom - 48f);

        var displayWidth = _displayRect.Width;
        var displayHeight = _displayRect.Height;
        _arcRect = new SKRect(
            _displayRect.Left + displayWidth * 0.14f,
            _displayRect.Top + displayHeight * 0.02f,
            _displayRect.Right - displayWidth * 0.14f,
            _displayRect.Top + displayHeight * 0.48f);
        _baseArcPath.Reset();
        _baseArcPath.AddArc(_arcRect, 205f, 130f);

        _layoutCurvePath.Reset();
        _layoutCurvePath.MoveTo(_displayRect.Left + displayWidth * 0.14f, _displayRect.Top + displayHeight * 0.40f);
        _layoutCurvePath.CubicTo(
            _displayRect.Left + displayWidth * 0.28f,
            _displayRect.Top + displayHeight * 0.16f,
            _displayRect.Left + displayWidth * 0.50f,
            _displayRect.Top + displayHeight * 0.16f,
            _displayRect.Left + displayWidth * 0.70f,
            _displayRect.Top + displayHeight * 0.42f);

        var ledWidth = MathF.Max(10f, displayWidth * 0.026f);
        var ledHeight = MathF.Max(7f, displayHeight * 0.026f);
        var ledGap = MathF.Max(5f, displayWidth * 0.010f);
        var totalLedWidth = _ledRects.Length * ledWidth + (_ledRects.Length - 1) * ledGap;
        var ledLeft = _displayRect.MidX - totalLedWidth * 0.5f;
        var ledTop = _displayRect.Top + displayHeight * 0.16f;
        for (var i = 0; i < _ledRects.Length; i++)
        {
            var x = ledLeft + i * (ledWidth + ledGap);
            _ledRects[i] = new SKRect(x, ledTop, x + ledWidth, ledTop + ledHeight);
        }
    }

    private void BuildShaders(int width, int height)
    {
#pragma warning disable CA1861
        _backgroundShader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, height), new[] { new SKColor(10, 9, 8), new SKColor(16, 13, 11), new SKColor(22, 17, 13) }, new[] { 0f, 0.54f, 1f }, SKShaderTileMode.Clamp);
        _backgroundPaint.Shader = _backgroundShader;
        _atmosphereShader = SKShader.CreateRadialGradient(
            new SKPoint(width * 0.60f, height * 0.12f),
            MathF.Min(MathF.Max(width, height) * 0.70f, 860f),
            new[] { new SKColor(255, 138, 31, 38), new SKColor(255, 106, 0, 20), new SKColor(255, 106, 0, 0) },
            new[] { 0f, 0.46f, 1f },
            SKShaderTileMode.Clamp);
        _atmospherePaint.Shader = _atmosphereShader;
        _floorBloomShader = SKShader.CreateRadialGradient(
            new SKPoint(width * 0.5f, _deviceTop + _deviceHeight + 92f),
            width * 0.34f,
            new[] { new SKColor(255, 106, 0, 56), new SKColor(143, 78, 36, 31), new SKColor(143, 78, 36, 0) },
            new[] { 0f, 0.48f, 1f },
            SKShaderTileMode.Clamp);
        _floorBloomPaint.Shader = _floorBloomShader;
        _deviceBodyShader = SKShader.CreateLinearGradient(
            new SKPoint(_deviceLeft, _deviceTop),
            new SKPoint(_deviceLeft, _deviceTop + _deviceHeight),
            new[] { new SKColor(58, 45, 37), new SKColor(42, 34, 29), new SKColor(26, 22, 20) },
            new[] { 0f, 0.46f, 1f },
            SKShaderTileMode.Clamp);
        _deviceBodyPaint.Shader = _deviceBodyShader;
        _deviceShellShader = SKShader.CreateLinearGradient(
            new SKPoint(_deviceLeft, _deviceTop + 30f),
            new SKPoint(_deviceLeft, _deviceTop + _deviceHeight),
            new[] { new SKColor(106, 82, 68, 88), new SKColor(17, 18, 20, 0) },
            new[] { 0f, 1f },
            SKShaderTileMode.Clamp);
        _deviceShellPaint.Shader = _deviceShellShader;
        _deviceHighlightShader = SKShader.CreateLinearGradient(
            new SKPoint(_deviceLeft + _deviceWidth * 0.56f, _deviceTop + 34f),
            new SKPoint(_deviceLeft + _deviceWidth, _deviceTop + _deviceHeight * 0.86f),
            new[] { new SKColor(255, 138, 31, 0), new SKColor(255, 138, 31, 22), new SKColor(255, 179, 71, 34) },
            new[] { 0f, 0.58f, 1f },
            SKShaderTileMode.Clamp);
        _deviceHighlightPaint.Shader = _deviceHighlightShader;
        _fasciaShader = SKShader.CreateLinearGradient(new SKPoint(_deviceLeft, _deviceTop + _deviceHeight), new SKPoint(_deviceLeft + _deviceWidth, _deviceTop + _deviceHeight), new[] { new SKColor(26, 22, 20), new SKColor(42, 34, 29), new SKColor(26, 22, 20) }, new[] { 0f, 0.5f, 1f }, SKShaderTileMode.Clamp);
        _fasciaPaint.Shader = _fasciaShader;
        _clearCoatShader = SKShader.CreateLinearGradient(new SKPoint(0, _deviceTop + _deviceHeight - 20f), new SKPoint(0, _deviceTop + _deviceHeight + 8f), new[] { new SKColor(243, 232, 220, 16), new SKColor(243, 232, 220, 0) }, new[] { 0f, 1f }, SKShaderTileMode.Clamp);
        _clearCoatPaint.Shader = _clearCoatShader;
        _displayDepthShader = SKShader.CreateRadialGradient(
            new SKPoint(_displayRect.MidX, _displayRect.MidY),
            _displayRect.Width * 0.65f,
            new[] { new SKColor(6, 6, 7), new SKColor(10, 9, 8), new SKColor(16, 13, 11) },
            new[] { 0f, 0.58f, 1f },
            SKShaderTileMode.Clamp);
        _displayDepthPaint.Shader = _displayDepthShader;
        _displayVignetteShader = SKShader.CreateRadialGradient(new SKPoint((_displayRect.Left + _displayRect.Right) * 0.5f, (_displayRect.Top + _displayRect.Bottom) * 0.48f), _displayRect.Width * 0.62f, new[] { new SKColor(255, 255, 255, 0), new SKColor(5, 5, 5, 0), new SKColor(8, 7, 6, 168) }, new[] { 0f, 0.62f, 1f }, SKShaderTileMode.Clamp);
        _displayVignettePaint.Shader = _displayVignetteShader;
        _displayGlowShader = SKShader.CreateLinearGradient(new SKPoint(_displayRect.Left + 24f, _displayRect.Top + 18f), new SKPoint(_displayRect.Left + _displayRect.Width * 0.44f, _displayRect.Top + 86f), new[] { new SKColor(255, 179, 71, 20), new SKColor(255, 179, 71, 0) }, new[] { 0f, 1f }, SKShaderTileMode.Clamp);
        _displayGlowPaint.Shader = _displayGlowShader;
        _lensOverlayShader = SKShader.CreateLinearGradient(new SKPoint(_displayRect.Left, _displayRect.Top), new SKPoint(_displayRect.Left + _displayRect.Width * 0.54f, _displayRect.Top + _displayRect.Height * 0.24f), new[] { new SKColor(243, 232, 220, 24), new SKColor(255, 179, 71, 8), new SKColor(255, 255, 255, 0) }, new[] { 0f, 0.45f, 1f }, SKShaderTileMode.Clamp);
        _lensOverlayPaint.Shader = _lensOverlayShader;
        _activeArcShader = SKShader.CreateLinearGradient(new SKPoint(_arcRect.MidX, _arcRect.Top), new SKPoint(_arcRect.Right, _arcRect.Bottom), new[] { new SKColor(255, 179, 71), new SKColor(255, 106, 0), new SKColor(217, 78, 0) }, new[] { 0f, 0.58f, 1f }, SKShaderTileMode.Clamp);
        _activeArcPaint.Shader = _activeArcShader;
        _sweepArcShader = SKShader.CreateLinearGradient(new SKPoint(_arcRect.Right - 22f, _arcRect.Top + 26f), new SKPoint(_arcRect.Right + 16f, _arcRect.Bottom - 18f), new[] { new SKColor(255, 173, 70), new SKColor(255, 106, 0), new SKColor(217, 78, 0) }, new[] { 0f, 0.5f, 1f }, SKShaderTileMode.Clamp);
        _sweepArcPaint.Shader = _sweepArcShader;
        _ledActiveShader = SKShader.CreateLinearGradient(new SKPoint(_displayRect.Left, _displayRect.Bottom - 42f), new SKPoint(_displayRect.Right, _displayRect.Bottom - 42f), new[] { new SKColor(255, 179, 71), new SKColor(255, 106, 0), new SKColor(217, 78, 0) }, new[] { 0f, 0.5f, 1f }, SKShaderTileMode.Clamp);
        _ledActivePaint.Shader = _ledActiveShader;
#pragma warning restore CA1861
    }

    private void DrawIndustrialEnvironment(SKCanvas canvas, int width, int height)
    {
        canvas.DrawRect(new SKRect(0, 0, width, height), _backgroundPaint);
        canvas.DrawRect(new SKRect(0, 0, width, height), _atmospherePaint);
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(_deviceLeft - 104f, _deviceTop - 20f, _deviceLeft - 10f, _deviceTop + _deviceHeight + 50f), 26f, 26f), _leftAtmospherePaint);
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(_deviceLeft + _deviceWidth + 14f, _deviceTop - 24f, _deviceLeft + _deviceWidth + 112f, _deviceTop + _deviceHeight + 38f), 24f, 24f), _rightAtmospherePaint);
    }

    private void DrawFloorSystem(SKCanvas canvas)
    {
        var floorCenterX = _deviceLeft + _deviceWidth * 0.5f;
        var floorY = _deviceTop + _deviceHeight + 58f;
        canvas.DrawOval(new SKRect(floorCenterX - _deviceWidth * 0.38f, floorY - 38f, floorCenterX + _deviceWidth * 0.38f, floorY + 34f), _contactShadowPaint);
        canvas.DrawOval(new SKRect(floorCenterX - _deviceWidth * 0.26f, floorY - 12f, floorCenterX + _deviceWidth * 0.26f, floorY + 24f), _reflectionPaint);
        canvas.DrawOval(new SKRect(floorCenterX - _deviceWidth * 0.30f, floorY - 8f, floorCenterX + _deviceWidth * 0.30f, floorY + 54f), _floorBloomPaint);
    }

    private void DrawDeviceShadow(SKCanvas canvas)
    {
        canvas.DrawOval(new SKRect(_deviceLeft + 34f, _deviceTop + 46f, _deviceLeft + _deviceWidth - 34f, _deviceTop + _deviceHeight + 56f), _rearShadowPaint);
    }

    private void DrawDeviceBody(SKCanvas canvas)
    {
        canvas.DrawPath(_deviceOuterPath, _deviceBodyPaint);
        canvas.DrawPath(_deviceOuterPath, _deviceShellPaint);
        canvas.DrawPath(_deviceOuterPath, _deviceHighlightPaint);
        canvas.DrawPath(_deviceOuterPath, _edgeHighlightPaint);
        canvas.DrawPath(_deviceOuterPath, _accentEdgePaint);
        canvas.DrawPath(_deviceInnerPath, _deviceShellPaint);
        canvas.DrawPath(_deviceInnerPath, _deviceHighlightPaint);
        canvas.DrawPath(_deviceInnerPath, _edgeHighlightPaint);
        canvas.DrawPath(_fasciaPath, _fasciaPaint);
        canvas.DrawPath(_fasciaPath, _clearCoatPaint);
        canvas.DrawPath(_rimLightPath, _rimLightPaint);
        canvas.DrawPath(_rimLightPath, _lowerRimGlowPaint);

        var lineLeft = _deviceLeft + 22f;
        var lineRight = _deviceLeft + _deviceWidth - 22f;
        for (var index = 0; index < 24; index++)
        {
            var y = _deviceTop + 26f + index * ((_deviceHeight - 54f) / 24f);
            canvas.DrawLine(lineLeft, y, lineRight, y + 0.4f, _brushedLinePaint);
        }
    }

    private void DrawDisplayInterior(SKCanvas canvas)
    {
        canvas.Save();
        canvas.ClipPath(_displayPath, SKClipOperation.Intersect, true);

        DrawDisplayDepth(canvas);
        DrawBaseTelemetry(canvas);
        DrawActiveTelemetry(canvas);
        DrawTelemetryText(canvas);
        DrawDigitalImperfections(canvas);
        DrawFailureStates(canvas);

        canvas.Restore();
    }

    private void DrawDisplayDepth(SKCanvas canvas)
    {
        var displayWidth = _displayRect.Width;
        var displayHeight = _displayRect.Height;
        canvas.DrawRect(_displayRect, _displayDepthPaint);
        canvas.DrawRect(_displayRect, _displayVignettePaint);

        var innerGlowRect = new SKRect(
            _displayRect.Left + displayWidth * 0.05f,
            _displayRect.Top + displayHeight * 0.05f,
            _displayRect.Left + displayWidth * 0.74f,
            _displayRect.Top + displayHeight * 0.23f);
        canvas.DrawRoundRect(new SKRoundRect(innerGlowRect, 26f, 26f), _displayGlowPaint);

        for (var index = 0; index < 4; index++)
        {
            var y = _displayRect.Top + displayHeight * (0.14f + index * 0.12f);
            canvas.DrawLine(_displayRect.Left + displayWidth * 0.05f, y, _displayRect.Right - displayWidth * 0.04f, y, _layoutLinePaint);
        }

        canvas.DrawPath(_layoutCurvePath, _layoutCurvePaint);
        canvas.DrawLine(
            _displayRect.Left + displayWidth * 0.06f,
            _displayRect.Top + displayHeight * 0.60f,
            _displayRect.Left + displayWidth * 0.58f,
            _displayRect.Top + displayHeight * 0.60f,
            _layoutLinePaint);
        canvas.DrawLine(
            _displayRect.Left + displayWidth * 0.06f,
            _displayRect.Bottom - displayHeight * 0.13f,
            _displayRect.Left + displayWidth * 0.58f,
            _displayRect.Bottom - displayHeight * 0.13f,
            _layoutLinePaint);
    }

    private void DrawBaseTelemetry(SKCanvas canvas)
    {
        _baseArcPaint.StrokeWidth = Clamp(_displayRect.Height * 0.022f, 7f, 12f);
        _tickPaint.StrokeWidth = Clamp(_displayRect.Height * 0.004f, 1f, 1.6f);
        canvas.DrawPath(_baseArcPath, _baseArcPaint);

        for (var index = 0; index <= 8; index++)
        {
            var t = index / 8f;
            var angle = DegreesToRadians(205f + t * 130f);
            var centerX = _arcRect.MidX;
            var centerY = _arcRect.MidY;
            var outerRadius = _arcRect.Width * 0.5f;
            var innerRadius = outerRadius - Clamp(_displayRect.Height * 0.030f, 10f, 16f);
            canvas.DrawLine(
                centerX + MathF.Cos(angle) * innerRadius,
                centerY + MathF.Sin(angle) * innerRadius,
                centerX + MathF.Cos(angle) * outerRadius,
                centerY + MathF.Sin(angle) * outerRadius,
                _tickPaint);
        }

        foreach (var ledRect in _ledRects)
        {
            canvas.DrawRoundRect(new SKRoundRect(ledRect, 2f, 2f), _ledInactivePaint);
        }
    }

    private void DrawActiveTelemetry(SKCanvas canvas)
    {
        var normalized = (float)Math.Clamp(_telemetry.ArcNormalized, 0.02, 0.98);
        var eased = EaseOutQuint(normalized);
        var pulse = (float)(Math.Sin(_telemetry.AnimationTime * 6.4) * 0.5 + 0.5);
        var sweepAngle = 130f * eased;

        _activeArcPaint.StrokeWidth = Clamp(_displayRect.Height * 0.020f, 6f, 11f);
        _activeArcGlowPaint.StrokeWidth = _activeArcPaint.StrokeWidth;
        _sweepArcPaint.StrokeWidth = Clamp(_displayRect.Height * 0.006f, 2f, 4f);

        _activeArcPath.Reset();
        _activeArcPath.AddArc(_arcRect, 205f, sweepAngle);
        canvas.DrawPath(_activeArcPath, _activeArcGlowPaint);
        canvas.DrawPath(_activeArcPath, _activeArcPaint);

        _sweepArcPath.Reset();
        _sweepArcPath.AddArc(_arcRect, 205f + Math.Max(0f, sweepAngle - 16f), 16f);
        canvas.DrawPath(_sweepArcPath, _sweepArcPaint);

        var activeCount = Math.Clamp((int)MathF.Round(eased * _ledRects.Length), 0, _ledRects.Length);
        var hotspotIndex = Math.Clamp(activeCount - 1, 0, _ledRects.Length - 1);

        for (var index = 0; index < _ledRects.Length; index++)
        {
            var threshold = index + 1;
            var intensity = Math.Clamp((_telemetry.ActiveLedCount + eased * _ledRects.Length) - threshold + 1, 0, 1);
            if (intensity <= 0)
            {
                continue;
            }

            var easedIntensity = EaseOutExpo((float)intensity);
            var ledRect = _ledRects[index];
            _ledTintPaint.Color = new SKColor(255, 106, 0, (byte)(120 + easedIntensity * 110));
            canvas.DrawRoundRect(new SKRoundRect(InflateRect(ledRect, 3f, 4f), 3f, 3f), _ledGlowPaint);
            canvas.DrawRoundRect(new SKRoundRect(ledRect, 2f, 2f), _ledActivePaint);
            canvas.DrawRoundRect(new SKRoundRect(ledRect, 2f, 2f), _ledTintPaint);

            if (index != hotspotIndex)
            {
                continue;
            }

            var hotspot = new SKPoint(ledRect.MidX, ledRect.MidY);
            _hotspotGlowPaint.Color = new SKColor(255, 106, 0, (byte)(84 + pulse * 36f));
            _hotspotCorePaint.Color = new SKColor(255, 179, 71, (byte)(70 + pulse * 42f));
            canvas.DrawCircle(hotspot, ledRect.Height * 2.8f, _hotspotGlowPaint);
            canvas.DrawCircle(hotspot, ledRect.Height * 1.4f, _hotspotCorePaint);
        }
    }

    private void DrawTelemetryText(SKCanvas canvas)
    {
        var displayWidth = _displayRect.Width;
        var displayHeight = _displayRect.Height;
        var uiScale = MathF.Min(displayWidth / 640f, displayHeight / 300f);
        var centerX = _displayRect.MidX;
        var speedBaseline = _displayRect.Top + displayHeight * 0.50f;
        var statsTop = _displayRect.Top + displayHeight * 0.77f;
        var statsZoneWidth = displayWidth * 0.68f;
        var statStep = statsZoneWidth / 4f;

        var speed = Math.Round(_telemetry.SpeedKph).ToString("0", CultureInfo.InvariantCulture);
        var speedX = centerX;
        var speedY = speedBaseline;
        var speedSize = Clamp(124f * uiScale, 48f, 124f);

        DrawBloomText(canvas, speed, speedX, speedY, speedSize, _rajdhaniTypeface, _speedGlowFarPaint, _speedGlowNearPaint, _speedCorePaint, SKTextAlign.Center);

        _unitPaint.Typeface = _interTypeface;
        _unitPaint.TextSize = Clamp(16f * uiScale, 10f, 16f);
        _unitPaint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("KPH", centerX, speedY + (18f * uiScale), _unitPaint);

        var gear = GetGear(_telemetry.SpeedKph);
        DrawBloomText(canvas, gear.ToString(CultureInfo.InvariantCulture), centerX, speedY + (56f * uiScale), Clamp(46f * uiScale, 22f, 46f), _rajdhaniTypeface, _speedGlowFarPaint, _speedGlowNearPaint, _valueOrangePaint, SKTextAlign.Center);

        var statsLeft = _displayRect.Left + displayWidth * 0.16f;
        DrawStat(canvas, "TORQUE", $"{_telemetry.TorqueNm:0}", "Nm", statsLeft + statStep * 0f, statsTop, uiScale);
        DrawStat(canvas, "TEMP", $"{_telemetry.TempC:0}", "C", statsLeft + statStep * 1f, statsTop, uiScale);
        DrawStat(canvas, "BUS", $"{_telemetry.BusVoltage:0.0}", "V", statsLeft + statStep * 2f, statsTop, uiScale);
        DrawStat(canvas, "G-FORCE", $"{_telemetry.GForce:0.00}", string.Empty, statsLeft + statStep * 3f, statsTop, uiScale);
    }

    private void DrawDigitalImperfections(SKCanvas canvas)
    {
        canvas.DrawBitmap(_grainBitmap, _displayRect, _grainPaint);
        canvas.Save();
        canvas.ClipRect(new SKRect(_displayRect.Left, _displayRect.Top, _displayRect.Left + 6f, _displayRect.Bottom));
        canvas.Translate(0.5f, 0);
        canvas.DrawRect(_displayRect, _aberrationPaint);
        canvas.Restore();

        canvas.Save();
        canvas.ClipRect(new SKRect(_displayRect.Right - 6f, _displayRect.Top, _displayRect.Right, _displayRect.Bottom));
        canvas.Translate(-0.5f, 0);
        canvas.DrawRect(_displayRect, _aberrationPaint);
        canvas.Restore();
    }

    private void DrawFailureStates(SKCanvas canvas)
    {
        if (!_telemetry.IsConnected)
        {
            var alpha = (byte)(120 + (Math.Sin(_telemetry.AnimationTime * 4.0) * 0.5 + 0.5) * 90);
            _alertPaint.Color = new SKColor(255, 69, 58, alpha);
            var rimRect = InflateRect(_displayRect, 4f, 4f);
            canvas.DrawRoundRect(new SKRoundRect(rimRect, 22f, 22f), _alertPaint);
        }

        if (_telemetry.IsDisplayEnabled)
        {
            return;
        }

        for (var y = _displayRect.Top; y <= _displayRect.Bottom; y += 4f)
        {
            canvas.DrawLine(_displayRect.Left, y, _displayRect.Right, y, _scanlinePaint);
        }

        var collapse = (float)((Math.Sin(_telemetry.AnimationTime * 12.0) * 0.5 + 0.5) * 0.9);
        var lineY = _displayRect.MidY;
        var width = _displayRect.Width * (0.12f + collapse * 0.28f);
        canvas.DrawRect(new SKRect(_displayRect.MidX - width, lineY - 1f, _displayRect.MidX + width, lineY + 1f), _powerLinePaint);
        canvas.DrawRect(_displayRect, _powerOffPaint);
    }

    private void DrawLensLayer(SKCanvas canvas)
    {
        canvas.Save();
        canvas.ClipPath(_lensPath, SKClipOperation.Intersect, true);

        var flareRect = new SKRect(_displayRect.Left + 18f, _displayRect.Top + 12f, _displayRect.Left + _displayRect.Width * 0.44f, _displayRect.Top + 64f);
        canvas.DrawRoundRect(new SKRoundRect(flareRect, 18f, 18f), _lensOverlayPaint);
        canvas.DrawRect(new SKRect(_displayRect.Left + 14f, _displayRect.Top + 8f, _displayRect.Right - 18f, _displayRect.Top + 16f), _lensStripPaint);

        canvas.Restore();
        canvas.DrawPath(_lensPath, _lensEdgePaint);
    }

    private void DrawHardwareDetails(SKCanvas canvas)
    {
        foreach (var recess in _recessRects)
        {
            canvas.DrawRoundRect(new SKRoundRect(recess, 4f, 4f), _recessPaint);
            canvas.DrawRoundRect(new SKRoundRect(recess, 4f, 4f), _recessEdgePaint);
        }

        foreach (var center in _boltCenters)
        {
            canvas.DrawCircle(center, 8f, _boltEdgePaint);
            canvas.DrawCircle(center.X + 1f, center.Y + 1f, 6f, _boltPaint);
            canvas.DrawLine(center.X - 3f, center.Y, center.X + 3f, center.Y, _boltSlotPaint);
        }

        _logoPaint.Typeface = _rajdhaniTypeface;
        _logoPaint.TextSize = 18f;
        _logoPaint.TextAlign = SKTextAlign.Center;
        canvas.DrawText("PROJECT-X", _fasciaPath.Bounds.MidX, _fasciaPath.Bounds.MidY + 6f, _logoPaint);
    }

    private void DrawGlobalWarmOverlay(SKCanvas canvas, int width, int height)
    {
        canvas.DrawRect(new SKRect(0, 0, width, height), _globalWarmOverlayPaint);
    }

    private void DrawStat(SKCanvas canvas, string label, string value, string unit, float x, float y, float uiScale)
    {
        _labelPaint.Typeface = _interTypeface;
        _labelPaint.TextSize = Clamp(10f * uiScale, 7.5f, 10f);
        _labelPaint.TextAlign = SKTextAlign.Left;
        canvas.DrawText(label, x, y, _labelPaint);

        _valueOrangePaint.Typeface = _rajdhaniTypeface;
        _valueOrangePaint.TextSize = Clamp(24f * uiScale, 14f, 24f);
        _valueOrangePaint.TextAlign = SKTextAlign.Left;
        var valueY = y + Clamp(26f * uiScale, 16f, 26f);
        canvas.DrawText(value, x, valueY, _valueOrangePaint);

        if (string.IsNullOrWhiteSpace(unit))
        {
            return;
        }

        _unitPaint.Typeface = _interTypeface;
        _unitPaint.TextSize = Clamp(11f * uiScale, 8f, 11f);
        _unitPaint.TextAlign = SKTextAlign.Left;
        canvas.DrawText(unit, x + _valueOrangePaint.MeasureText(value) + (6f * uiScale), valueY - (2f * uiScale), _unitPaint);
    }

    private static void DrawBloomText(SKCanvas canvas, string text, float x, float y, float size, SKTypeface typeface, SKPaint farGlow, SKPaint nearGlow, SKPaint core, SKTextAlign align)
    {
        farGlow.Typeface = typeface;
        farGlow.TextSize = size;
        farGlow.TextAlign = align;
        nearGlow.Typeface = typeface;
        nearGlow.TextSize = size;
        nearGlow.TextAlign = align;
        core.Typeface = typeface;
        core.TextSize = size;
        core.TextAlign = align;
        canvas.DrawText(text, x, y, farGlow);
        canvas.DrawText(text, x, y, nearGlow);
        canvas.DrawText(text, x, y, core);
    }

    private void RegenerateNoise()
    {
        for (var index = 0; index < _grainBytes.Length; index += 4)
        {
            var value = (byte)_noiseRandom.Next(8, 28);
            _grainBytes[index] = value;
            _grainBytes[index + 1] = value;
            _grainBytes[index + 2] = value;
            _grainBytes[index + 3] = 8;
        }

        System.Runtime.InteropServices.Marshal.Copy(_grainBytes, 0, _grainBitmap.GetPixels(), _grainBytes.Length);
    }

    private void DisposeShaders()
    {
        _backgroundShader?.Dispose();
        _atmosphereShader?.Dispose();
        _floorBloomShader?.Dispose();
        _deviceBodyShader?.Dispose();
        _deviceShellShader?.Dispose();
        _fasciaShader?.Dispose();
        _clearCoatShader?.Dispose();
        _lensOverlayShader?.Dispose();
        _displayDepthShader?.Dispose();
        _displayVignetteShader?.Dispose();
        _displayGlowShader?.Dispose();
        _activeArcShader?.Dispose();
        _sweepArcShader?.Dispose();
        _ledActiveShader?.Dispose();
        _backgroundShader = null;
        _atmosphereShader = null;
        _floorBloomShader = null;
        _deviceBodyShader = null;
        _deviceShellShader = null;
        _fasciaShader = null;
        _clearCoatShader = null;
        _lensOverlayShader = null;
        _displayDepthShader = null;
        _displayVignetteShader = null;
        _displayGlowShader = null;
        _activeArcShader = null;
        _sweepArcShader = null;
        _ledActiveShader = null;
    }

    private static SKPaint NewPaint(SKColor color = default, bool antialias = true)
    {
        var paint = new SKPaint
        {
            IsAntialias = antialias,
            Style = SKPaintStyle.Fill,
            Color = color
        };
        return paint;
    }

    private static SKPaint NewStrokePaint(SKColor color, float width, SKStrokeCap cap = SKStrokeCap.Butt)
    {
        return new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = width,
            StrokeCap = cap,
            Color = color
        };
    }

    private static SKRect InflateRect(SKRect rect, float dx, float dy)
    {
        rect.Inflate(dx, dy);
        return rect;
    }

    private static float DegreesToRadians(float degrees) => degrees * MathF.PI / 180f;

    private static float EaseOutQuint(float value) => 1f - MathF.Pow(1f - value, 5f);

    private static float EaseOutExpo(float value) => value <= 0f ? 0f : 1f - MathF.Pow(2f, -10f * value);

    private static float Clamp(float value, float min, float max) => MathF.Min(MathF.Max(value, min), max);

    private static int GetGear(double speedKph)
    {
        if (speedKph < 60d) return 1;
        if (speedKph < 95d) return 2;
        if (speedKph < 130d) return 3;
        if (speedKph < 170d) return 4;
        if (speedKph < 210d) return 5;
        if (speedKph < 245d) return 6;
        if (speedKph < 285d) return 7;
        return 8;
    }

    private static SKTypeface LoadTypeface(string resourceName, string fallbackFamily)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is not null)
        {
            var typeface = SKTypeface.FromStream(stream);
            if (typeface is not null)
            {
                return typeface;
            }
        }

        return SKTypeface.FromFamilyName(fallbackFamily) ?? SKTypeface.Default;
    }
}
