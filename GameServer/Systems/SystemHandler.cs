using Ventus.Network.Packets;

public class SystemHandler
{
    private readonly Dictionary<CustomGameEvent, List<Action<dynamic>>> _handlers = new();

    public void Subscribe(CustomGameEvent type, Action<dynamic> handler)
    {
        if (!_handlers.ContainsKey(type))
            _handlers[type] = [];

        _handlers[type].Add(handler);
    }

    public void HandlePacket(dynamic message)
    {

        CustomGameEvent type = (CustomGameEvent)message.Type;

        if (_handlers.TryGetValue(type, out var handlerList))
        {

            Console.WriteLine($"Se encontraron {handlerList.Count} handlers para el evento {type}");

            foreach (var handler in handlerList)
            {
                Console.WriteLine($"Ejecutando handler: {handler.Method.Name}");

                try
                {
                    Console.WriteLine($"Ejecutando handler: {handler.Method.Name}");
                    handler(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al ejecutar el handler: {ex.Message}");
                }
            }
        }

    }
}
