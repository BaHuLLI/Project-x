using CommunityToolkit.Mvvm.ComponentModel;
using ProjectXProDash.Core;
using ProjectXProDash.Services;

namespace ProjectXProDash.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel(AppSettings appSettings, ThemeManager themeManager)
    {
        WindowBounds = $"Top {appSettings.WindowTop:0} | Left {appSettings.WindowLeft:0} | {appSettings.WindowWidth:0} x {appSettings.WindowHeight:0}";
        RajdhaniAsset = themeManager.RajdhaniAssetState;
        InterAsset = themeManager.InterAssetState;
    }

    [ObservableProperty]
    private string windowBounds;

    [ObservableProperty]
    private string rajdhaniAsset;

    [ObservableProperty]
    private string interAsset;
}
