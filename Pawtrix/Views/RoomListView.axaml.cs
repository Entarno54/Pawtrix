using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Meowtrix.Sdk.Core.Domain.MatrixRoom;
using Meowtrix.Sdk.Core.Infrastructure.Dto.Room.Create;
using Pawtrix.ViewModels;
using Pawtrix.Models;
using Pawtrix.Objects;

namespace Pawtrix.Views;

public partial class RoomListView: UserControl
{
    public RoomListView()
    {
        Console.WriteLine("Creatingnewroomlist");
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (DataContext is not RoomListViewModel viewModel) return;
        if (Application.Current?.ApplicationLifetime 
            is not IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: MainWindowViewModel mainVm }) 
            return;
        if (viewModel.Loaded) return;
        viewModel.Loaded = true;
        
        Program.Client.OnMatrixRoomEventsReceived += mainVm.HandleEvent!;
        
        Program.Client.Start();

        Task.Run(async () =>
        {
            while (Program.Client.JoinedRooms.Length == 0)
            {
                await Task.Delay(100);
                Console.WriteLine("Waiting for rooms.");
            }
            
            if (Program.Client.Token != null)
            {
                Program.HttpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", Program.Client.Token);
            }
            foreach (var room in Program.Client.JoinedRooms)
            {
                Console.WriteLine(room);
                Console.WriteLine(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id + "/state");
                string a = await Program.HttpClient.GetStringAsync(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id + "/state");
                JsonElement states = JsonDocument.Parse(a).RootElement;
                Console.WriteLine(states);
                Uri? icon = null;
                //Console.WriteLine(states);
                //Console.WriteLine(parsed.RootElement.GetProperty("name"));
                //Console.WriteLine(a);w
                string? roomName;
                try
                {
                    Console.WriteLine(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id + "/state/m.room.name");
                    string b = await Program.HttpClient.GetStringAsync(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id +
                                                               "/state/m.room.name");
                    Console.WriteLine(b);
                    roomName = JsonDocument.Parse(b).RootElement.GetProperty("name").ToString();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Caught");
                    roomName = "";
                    foreach (JsonElement roomState in states.EnumerateArray())
                    {
                        if (roomState.GetProperty("type").GetString() == "m.room.member")
                        {
                            string? userid = roomState.GetProperty("sender").GetString();
                            if (userid == Program.Client.UserId ||
                                roomState.GetProperty("content").GetProperty("membership").ToString() != "join") continue;
                            roomName += userid + " ";
                        }
                    }
                }
                
                foreach (JsonElement roomState in states.EnumerateArray())
                {
                    if (roomState.GetProperty("type").GetString() == "m.room.avatar")
                    {
                        Uri mxc = new(roomState.GetProperty("content").GetProperty("url").GetString()!);
                        icon = new(Program.Client.BaseAddress + "_matrix/client/v1/media/download/" + mxc.Host + mxc.PathAndQuery);
                        Console.WriteLine("FOUNDICON");
                    } else if (roomState.GetProperty("type").GetString() == "m.room.create")
                    {
                        Console.WriteLine("CreateRoom!!");

                        try
                        {
                            string c =  await Program.HttpClient.GetStringAsync(Program.Client.BaseAddress + "_matrix/client/v3/rooms/" + room.Id + "/state/m.room.create");
                            Console.WriteLine(c);
                            JsonElement createEvent = JsonDocument.Parse(c).RootElement;
                            string type = createEvent.GetProperty("type").GetString() ?? "m.room";
                            
                            if (type == "m.space")
                            {
                                room.RoomType = RoomType.Space;
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    } else if (roomState.GetProperty("type").GetString() == "m.space.child")
                    {
                        // We know it's a space for sure lol.
                        room.RoomType = RoomType.Space;
                        
                        string roomId = roomState.GetProperty("state_key").GetString()!;
                        
                        room.Children.Add(new MatrixRoom(roomId, MatrixRoomStatus.Unknown, new List<string>(), RoomType.Room));
                    }
                }
                
                Console.WriteLine("Eveveveve");
                
                Bitmap? setIcon = null;
                if (icon != null)
                {
                    setIcon = await Functions.DownloadImage(icon);
                }
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    RoomButton newButton = new(roomName, setIcon, room.Id, room.RoomType);

                    if (room.RoomType == RoomType.Space)
                    {
                        viewModel.Spaces.Add(newButton);
                        
                        _ = SetupSpace(room, roomName, mainVm, viewModel);
                    } else if (room.RoomType == RoomType.Room)
                    {
                        bool found = false;
                        foreach (SpaceViewModel view in mainVm.Spaces.Values)
                        {
                            foreach (MatrixRoom hasRoom in view.Space.Children)
                            {
                                if (hasRoom.Id == room.Id)
                                {
                                    view.Rooms.Add(newButton);
                                    Console.WriteLine("added to rooms");
                                    found = true;
                                }
                            }
                        }

                        if (!found)
                        {
                            viewModel.Chats.Add(newButton);
                        }
                    }
                });

                Console.WriteLine("Added1");
            }
        });
    }

    private void Logout(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime 
            is not IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: MainWindowViewModel mainVm }) 
            return;
        Storage.ClearToken();
        mainVm.RestartApp();
    }

    private void Set_Spaces(object? sender, RoutedEventArgs e)
    {
        RoomsList.IsVisible = false;
        SpacesList.IsVisible = true;
    }

    private void Set_Rooms(object? sender, RoutedEventArgs e)
    {
        RoomsList.IsVisible = true;
        SpacesList.IsVisible = false;
    }

    private async Task SetupSpace(MatrixRoom space, string name , MainWindowViewModel mainVm, RoomListViewModel viewModel)
    {
        SpaceViewModel spaceTab;
        
        Console.WriteLine("Hellothereitreal");
        
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            spaceTab = new(space);
            
            mainVm.Spaces.Add(space.Id, spaceTab);
            Console.WriteLine($"Created space {space.Id}");

            List<RoomButton> toChange = new();
            foreach (MatrixRoom room in space.Children)
            {
                foreach (RoomButton roomButton in viewModel.Chats)
                {
                    Console.WriteLine("GettingRoomid");
                    Console.WriteLine(room.Id);
                    Console.WriteLine(roomButton.RoomId);
                    if (room.Id == roomButton.RoomId)
                    {
                        Console.WriteLine("Equals");
                        toChange.Add(roomButton);
                    }
                }
            }

            foreach (RoomButton roomButton in toChange)
            {
                Console.WriteLine("Found");
                viewModel.Chats.Remove(roomButton);

            
                spaceTab.Rooms.Add(roomButton);
                Console.WriteLine("AddedToSpace");
            }
        });
    }
}