using System;
using System.Collections.ObjectModel;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Pawtrix.ViewModels;

public class RoomViewModel : ViewModelBase
{
    public string RoomId { get; }
    public string RoomName { get; }
    
    public object SelectedItem { get; set; }

    public bool Preloaded;
    
    public string? PrevSpace { get; set; }

    public ScrollViewer Scroller { get; set; }
    
    public ObservableCollection<MessageViewModel> Messages { get; } = [];
    
    public RoomViewModel(string roomId, string roomName)
    {
        RoomId = roomId;
        RoomName = roomName;
    }

    public RoomViewModel()
    {
        
    }

    public void AddMessage(MessageViewModel message)
    {
        Messages.Add(message);

        Console.WriteLine("scroller");
        Console.WriteLine(Scroller);

        Scroller.Offset = new Vector2(0, (float)Scroller.Extent.Height);
        Console.WriteLine("scrolled to end");
    }
}