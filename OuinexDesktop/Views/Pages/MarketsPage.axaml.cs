using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OuinexDesktop.Views.Pages;

public partial class MarketsPage : UserControl
{
    public MarketsPage()
    {
        InitializeComponent();
        DataContextChanged += ((senderse, args) =>
        {

        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}