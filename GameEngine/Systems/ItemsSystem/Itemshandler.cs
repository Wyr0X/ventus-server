using System.Net.WebSockets;
using Protos.Game.Items;

public class ItemHandler
{
    private readonly ItemManager _itemManager;

    public ItemHandler(ItemManager itemManager)
    {
        _itemManager = itemManager;
    }

    // Función que maneja los mensajes de items recibidos desde el cliente
    public void HandleItemMessage(ClientMessageItems itemMessage, WebSocket webSocket)
    {
        switch (itemMessage.MessageTypeCase)
        {
            case ClientMessageItems.MessageTypeOneofCase.UseItemRequest:
                _itemManager.ProcessUseItemRequest(itemMessage.UseItemRequest, webSocket);
                break;
            case ClientMessageItems.MessageTypeOneofCase.EquipItemRequest:
                _itemManager.ProcessEquipItemRequest(itemMessage.EquipItemRequest, webSocket);
                break;
            case ClientMessageItems.MessageTypeOneofCase.DropItemRequest:
                _itemManager.ProcessDropItemRequest(itemMessage.DropItemRequest, webSocket);
                break;
            case ClientMessageItems.MessageTypeOneofCase.PickUpItemRequest:
                _itemManager.ProcessPickUpItemRequest(itemMessage.PickUpItemRequest, webSocket);
                break;
            default:
                Console.WriteLine("❌ Tipo de mensaje de item no reconocido.");
                break;
        }
    }
}
