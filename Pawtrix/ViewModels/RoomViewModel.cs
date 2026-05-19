using System.Collections.ObjectModel;
using Pawtrix.Views;
using Tmds.DBus.Protocol;

namespace Pawtrix.ViewModels;

public class RoomViewModel : ViewModelBase
{
    public string RoomId { get; }
    public string RoomName { get; }
    
    public object SelectedItem { get; set; }

    public bool Preloaded;
    
    public string? PrevSpace { get; set; }

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

        SelectedItem = message;
    }
}