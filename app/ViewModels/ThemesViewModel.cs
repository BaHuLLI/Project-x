using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjectXProDash.Core;
using ProjectXProDash.Models;
using ProjectXProDash.Services;

namespace ProjectXProDash.ViewModels;

public partial class ThemesViewModel : ObservableObject
{
    private readonly ThemeManager _themeManager;

    public ThemesViewModel(ThemeManager themeManager)
    {
        _themeManager = themeManager;
        Themes = new ObservableCollection<ThemeConfig>
        {
            new ThemeConfig("Inferno Metal", "#FF6A00", "Warm metal control-stage baseline."),
            new ThemeConfig("Studio Ember", "#FF7D1A", "Softer amber edge for studio-grade presentation."),
            new ThemeConfig("Copper Stage", "#F36A2C", "Copper-leaning premium hardware highlight.")
        };

        Themes[0].IsSelected = true;
        SelectThemeCommand = new RelayCommand(SelectTheme);
    }

    public ObservableCollection<ThemeConfig> Themes { get; }

    public RelayCommand SelectThemeCommand { get; }

    private void SelectTheme(object? parameter)
    {
        if (parameter is not ThemeConfig selectedTheme)
        {
            return;
        }

        foreach (var theme in Themes)
        {
            theme.IsSelected = theme == selectedTheme;
        }

        _themeManager.ApplyTheme(selectedTheme.Name);
    }
}
