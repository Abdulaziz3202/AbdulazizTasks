using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;

namespace KPMGTask.Middlewares
{
    public class WebSocketManager
    {
        //ConcurrentDictionary: in case multiple threads access the socket manager
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

        // Adds a WebSocket connection for a user, if not already added
        public void AddSocket(string userId, WebSocket socket)
        {
            if (!_sockets.ContainsKey(userId))
            {
                _sockets[userId] = socket;
            }
        }

        // Removes a WebSocket connection for a user and disposes of the socket
        public void RemoveSocket(string userId)
        {
            if (_sockets.TryRemove(userId, out var socket))
            {
                if (socket.State == WebSocketState.Open)
                {
                    // Close the WebSocket gracefully before disposing
                    socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None).Wait();
                }
                socket.Dispose();
            }
        }

        // Sends a message to a specific user via WebSocket
        public async Task SendToUser(string userId, string message)
        {
            if (_sockets.ContainsKey(userId))
            {
                var socket = _sockets[userId];

                // Only send message if the WebSocket is in an open state
                if (socket.State == WebSocketState.Open)
                {
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    var buffer = new ArraySegment<byte>(messageBytes);
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    // Optionally log that socket is not open or handle the case
                }
            }
        }
    }
}
