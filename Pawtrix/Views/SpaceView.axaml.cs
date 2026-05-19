using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Pawtrix.ViewModels;

namespace Pawtrix.Views;

public partial class SpaceView : UserControl
{
    public SpaceView()
    {
        InitializeComponent();
    }

    public void Exit(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime 
            is not IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: MainWindowViewModel mainVm }) 
            return;
        mainVm.NavigateToRooms();
    }
}