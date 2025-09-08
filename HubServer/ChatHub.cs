using ChatContracts;
using Microsoft.AspNetCore.SignalR;

namespace HubServer;

//public class ChatHub : Hub
public class ChatHub : Hub<IChatClient>
{
    private static readonly object _lockUsers = new object();
    private static List<ConnectedUser> _connectedUsers = new List<ConnectedUser>();
    public override async Task OnConnectedAsync()
    {
        var userId = string.Empty;
        var userName = string.Empty;
        userId = Context.GetHttpContext()?.Request.Query["userId"];
        userName = Context.GetHttpContext()?.Request.Query["username"];
        Console.WriteLine($"User {userName} with id {userId} has connected.");

        Console.WriteLine("Client connected: " + Context.ConnectionId);

        lock (_lockUsers) {
            _connectedUsers.Add(new ConnectedUser
            {
                UserId = userId,
                UserName = userName,
                ConnectionId = Context.ConnectionId
            });

        }
        await Clients.Caller.ReceiveSystemMessage($"Hi{userName}, you have just connected.");
        await Clients.All.UpdateUserList(_connectedUsers);
        //await Clients.Caller.ReceiveSystemMessage($"Hi{userName}, you have just connected.");
        //await Clients.All.SendAsync("UpdateUserList", _connectedUsers);
        //return base.OnConnectedAsync();

        //return Task.CompletedTask;
    }

    public override async Task OnDisconnectedAsync(Exception? exception) 
    {
        ConnectedUser? user;
        lock (_lockUsers) { 
            user = _connectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null) {
                _connectedUsers.Remove(user);
            }
        }

        if (user != null) {
            //await Clients.All.SendAsync("RecieveSystemMessage", $"{user.UserName} has left the chat");
            await Clients.Caller.ReceiveSystemMessage($"{user.UserName} has left the chat");

            //Update the client about the user list change
            //await Clients.All.SendAsync("UpdateUserList", _connectedUsers);
            await Clients.All.UpdateUserList(_connectedUsers);
        }

        await base.OnDisconnectedAsync(exception);

    }

    public async Task ForwardMessage(string fromUserId, string toConnectionId, string message) {
        var toUser = _connectedUsers.FirstOrDefault(u => u.ConnectionId == toConnectionId);
        if (toUser != null) {
            await Clients.Client(toConnectionId).ReceiveMessage(fromUserId, Context.ConnectionId, message);
        }
    }
}
