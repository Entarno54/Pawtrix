using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using Pawtrix.Models;
using Pawtrix.Objects;

namespace Pawtrix.ViewModels;

public partial class RoomListViewModel : ViewModelBase
{
    
    
    public ObservableCollection<RoomButton> Chats { get; } = new();
    public ObservableCollection<RoomButton> Spaces { get; } = new();
    
    public string UserName { get; } = Program.Client.UserId;

    public bool Loaded = false;
    
    [RelayCommand]
    private void AddChat()
    {
        
    }
}