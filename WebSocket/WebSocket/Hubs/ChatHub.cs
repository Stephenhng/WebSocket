using Microsoft.AspNetCore.SignalR;

namespace WebSocket.Hubs;

/// <summary>
/// ChatHub manages real-time communication between connected clients.
/// </summary>
public class ChatHub : Hub
{
    #region Fields

    /// <summary>
    /// Stores connected users with their connection IDs as keys and usernames as values.
    /// </summary>
    private static readonly Dictionary<string, string> Users = [];

    #endregion

    #region Methods

    /// <summary>
    /// Invoked when a client connects to the hub.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnConnectedAsync()
    {
        string username = Context.GetHttpContext()!.Request.Query["username"]!;
        Users.Add(Context.ConnectionId, username);
        await AddMessageToChat(string.Empty, $"{username} had connected");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Invoked when a client disconnects from the hub.
    /// </summary>
    /// <param name="exception">The exception that caused the disconnection, if any.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
        await AddMessageToChat(string.Empty, $"{username} had disconnected");
    }

    /// <summary>
    /// Sends a chat message to all connected clients.
    /// </summary>
    /// <param name="user">The username of the sender.</param>
    /// <param name="message">The chat message to be broadcast.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddMessageToChat(string user, string message)
    {
        await Clients.All.SendAsync("ReceivedMessage", user, message);
    }

    #endregion
}
