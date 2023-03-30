using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OuinexDesktop.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}