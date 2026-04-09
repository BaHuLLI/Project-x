using System.Windows;
using System.Windows.Input;

namespace ProjectXProDash.Core;

public static class WindowDragBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(WindowDragBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject element) =>
        (bool)element.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject element, bool value) =>
        element.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
    {
        if (dependencyObject is not UIElement element)
        {
            return;
        }

        if ((bool)eventArgs.NewValue)
        {
            element.MouseLeftButtonDown += OnMouseLeftButtonDown;
            return;
        }

        element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
    }

    private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs eventArgs)
    {
        if (sender is not DependencyObject dependencyObject)
        {
            return;
        }

        var window = Window.GetWindow(dependencyObject);
        if (window is null)
        {
            return;
        }

        if (eventArgs.ClickCount == 2 && window.ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip)
        {
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
            return;
        }

        try
        {
            window.DragMove();
            eventArgs.Handled = true;
        }
        catch
        {
        }
    }
}
