using System.Windows;
using System.Windows.Threading;

namespace ProjectXProDash.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        Activate();
        Topmost = true;

        Dispatcher.BeginInvoke(() =>
        {
            Topmost = false;
            Activate();
            Focus();
        }, DispatcherPriority.ApplicationIdle);
    }
}
