using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjectXProDash.Core;

namespace ProjectXProDash.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly AppSettings _appSettings;

    public MainViewModel(
        AppSettings appSettings,
        DashboardViewModel dashboardViewModel,
        SettingsViewModel settingsViewModel,
        ThemesViewModel themesViewModel)
    {
        _appSettings = appSettings;
        DashboardViewModel = dashboardViewModel;
        SettingsViewModel = settingsViewModel;
        ThemesViewModel = themesViewModel;

        Languages = new ObservableCollection<string> { "EN", "RU", "DE", "CS", "HU" };
        CurrentLanguage = string.IsNullOrWhiteSpace(_appSettings.Language) ? "EN" : _appSettings.Language;
        CurrentContent = DashboardViewModel;

        ShowDashboardCommand = new RelayCommand(_ => SetCurrentContent(DashboardViewModel));
        ShowSettingsCommand = new RelayCommand(_ => SetCurrentContent(SettingsViewModel));
        ShowThemesCommand = new RelayCommand(_ => SetCurrentContent(ThemesViewModel));
        ToggleLanguagePopupCommand = new RelayCommand(_ => IsLanguagePopupOpen = !IsLanguagePopupOpen);
        SelectLanguageCommand = new RelayCommand(SelectLanguage);
        TogglePreviewCommand = new RelayCommand(_ => DashboardViewModel.StartPreviewLabel = "Start Preview");
        MinimizeCommand = new RelayCommand(window => { if (window is Window currentWindow) currentWindow.WindowState = WindowState.Minimized; });
        MaximizeCommand = new RelayCommand(window =>
        {
            if (window is not Window currentWindow)
            {
                return;
            }

            currentWindow.WindowState = currentWindow.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        });
        CloseCommand = new RelayCommand(window => { if (window is Window currentWindow) currentWindow.Close(); });
    }

    public DashboardViewModel DashboardViewModel { get; }

    public SettingsViewModel SettingsViewModel { get; }

    public ThemesViewModel ThemesViewModel { get; }

    public ObservableCollection<string> Languages { get; }

    public RelayCommand ShowDashboardCommand { get; }

    public RelayCommand ShowSettingsCommand { get; }

    public RelayCommand ShowThemesCommand { get; }

    public RelayCommand ToggleLanguagePopupCommand { get; }

    public RelayCommand SelectLanguageCommand { get; }

    public RelayCommand TogglePreviewCommand { get; }

    public RelayCommand MinimizeCommand { get; }

    public RelayCommand MaximizeCommand { get; }

    public RelayCommand CloseCommand { get; }

    [ObservableProperty]
    private object currentContent;

    [ObservableProperty]
    private bool isLanguagePopupOpen;

    [ObservableProperty]
    private string currentLanguage;

    [ObservableProperty]
    private bool isDashboardActive = true;

    [ObservableProperty]
    private bool isSettingsActive;

    [ObservableProperty]
    private bool isThemesActive;

    private void SetCurrentContent(object content)
    {
        CurrentContent = content;
        IsDashboardActive = ReferenceEquals(content, DashboardViewModel);
        IsSettingsActive = ReferenceEquals(content, SettingsViewModel);
        IsThemesActive = ReferenceEquals(content, ThemesViewModel);
        IsLanguagePopupOpen = false;
    }

    private void SelectLanguage(object? parameter)
    {
        if (parameter is not string language || string.IsNullOrWhiteSpace(language))
        {
            return;
        }

        CurrentLanguage = language;
        _appSettings.Language = language;
        IsLanguagePopupOpen = false;
    }
}
