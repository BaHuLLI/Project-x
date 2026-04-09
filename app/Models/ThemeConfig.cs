using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectXProDash.Models;

public partial class ThemeConfig : ObservableObject
{
    public ThemeConfig(string name, string accentHex, string description)
    {
        Name = name;
        AccentHex = accentHex;
        Description = description;
    }

    public string Name { get; }

    public string AccentHex { get; }

    public string Description { get; }

    [ObservableProperty]
    private bool isSelected;
}
