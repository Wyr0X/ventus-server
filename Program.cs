using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VentusServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/"); // URL base para WebSocket
            listener.Start();
            Console.WriteLine("Servidor WebSocket iniciado en ws://localhost:5000");

            while (true)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    // Establecer WebSocket
                    WebSocket webSocket = (await context.AcceptWebSocketAsync(null)).WebSocket;
                    Console.WriteLine("Cliente conectado");

                    await HandleWebSocket(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        static async Task HandleWebSocket(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result;
            while (webSocket.State == WebSocketState.Open)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Mensaje recibido: {message}");

                // Responder al cliente
                byte[] response = Encoding.UTF8.GetBytes("Mensaje recibido");
                await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            webSocket.Dispose();
            Console.WriteLine("Cliente desconectado");
        }
    }
}