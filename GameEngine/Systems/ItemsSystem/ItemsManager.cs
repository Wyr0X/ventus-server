using System.Net.WebSockets;
using Protos.Game.Items;

public class ItemManager
{
    private readonly ItemLogic _itemLogic;

    public ItemManager(ItemLogic itemLogic)
    {
        _itemLogic = itemLogic;
    }

    // Procesa la solicitud de usar un objeto
    public void ProcessUseItemRequest(UseItemRequest useItemRequest, WebSocket webSocket)
    {
        if (useItemRequest.ItemId <= 0)
        {
            Console.WriteLine("❌ ID del item inválido.");
            return;
        }

        _itemLogic.UseItemLogic(useItemRequest, webSocket);
    }

    // Procesa la solicitud de equipar un objeto
    public void ProcessEquipItemRequest(EquipItemRequest equipItemRequest, WebSocket webSocket)
    {
        if (equipItemRequest.ItemId <= 0)
        {
            Console.WriteLine("❌ ID del item inválido.");
            return;
        }

        _itemLogic.EquipItemLogic(equipItemRequest, webSocket);
    }

    // Procesa la solicitud de dejar un objeto
    public void ProcessDropItemRequest(DropItemRequest dropItemRequest, WebSocket webSocket)
    {
        if (dropItemRequest.ItemId <= 0)
        {
            Console.WriteLine("❌ ID del item inválido.");
            return;
        }

        _itemLogic.DropItemLogic(dropItemRequest, webSocket);
    }

    // Procesa la solicitud de recoger un objeto
    public void ProcessPickUpItemRequest(PickUpItemRequest pickUpItemRequest, WebSocket webSocket)
    {
        if (pickUpItemRequest.ItemId <= 0)
        {
            Console.WriteLine("❌ ID del item inválido.");
            return;
        }

        _itemLogic.PickUpItemLogic(pickUpItemRequest, webSocket);
    }
}
