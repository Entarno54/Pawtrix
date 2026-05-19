using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Meowtrix.Sdk.Core.Infrastructure.Dto.Room.Create;
using Pawtrix.ViewModels;

namespace Pawtrix.Objects;

public partial class RoomButton : Button
{
    public readonly string RoomId;
    public readonly RoomType RoomType;

    public RoomButton()
    {
        InitializeComponent();
        RoomId = string.Empty;
    }
    
    public RoomButton(string name, Bitmap? icon, string roomId, RoomType roomType)
    {
        InitializeComponent();
        
        if (icon != null) Icon.Source = icon;
        Name = name;
        Text.Text = name;
        RoomId = roomId;
        RoomType = roomType;
    }

    public void OpenRoom(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not Button) return;
        if (Application.Current?.ApplicationLifetime 
            is not IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: MainWindowViewModel mainVm }) 
            return;

        if (RoomType == RoomType.Space)
        {
            mainVm.NavigateToSpace(RoomId);
        } else if (RoomType == RoomType.Room)
        {
            mainVm.NavigateToRoom(RoomId, Name!);
        }
    }
}
