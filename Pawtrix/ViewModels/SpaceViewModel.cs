using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using Meowtrix.Sdk.Core.Domain.MatrixRoom;
using Pawtrix.Models;
using Pawtrix.Objects;

namespace Pawtrix.ViewModels;

public partial class SpaceViewModel : ViewModelBase
{
    public string SpaceName { get; set; }
    public MatrixRoom Space { get; set; }
    
    public ObservableCollection<RoomButton> Rooms { get; } = new();

    public SpaceViewModel(MatrixRoom space)
    {
        SpaceName = space.Id;
        Space = space;
    }
    
    public SpaceViewModel()
    {
        SpaceName = string.Empty;
    }
}