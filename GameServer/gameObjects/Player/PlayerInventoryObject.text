using System.Collections.Generic;
using System.Linq;

namespace VentusServer.Domain.Objects
{
    public class PlayerInventory
    {
        private List<ItemObject> _items = new List<ItemObject>();

        public IReadOnlyList<ItemObject> Items => _items.AsReadOnly();

        public void AddItem(ItemObject item)
        {
            // Si el ítem ya existe en el inventario, intentar apilar
            var existingItem = _items.FirstOrDefault(i => i.CanStack());
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _items.Add(item);
            }
        }

        public bool RemoveItem(string itemId, int quantity = 1)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null || item.Quantity < quantity)
                return false;

            item.Quantity -= quantity;
            if (item.Quantity <= 0)
                _items.Remove(item);

            return true;
        }

        public ItemObject GetItem(string itemId)
        {
            return _items.FirstOrDefault(i => i.Id == itemId);
        }

        public bool HasItem(string itemId)
        {
            return _items.Any(i => i.Id == itemId);
        }

        public bool EquipItem(string itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId && i.ItemModel.IsEquippable);
            if (item != null)
            {
                item.IsEquipped = true;
                return true;
            }
            return false;
        }

        public bool UnEquipItem(string itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId && i.IsEquipped);
            if (item != null)
            {
                item.IsEquipped = false;
                return true;
            }
            return false;
        }
        public IReadOnlyList<ItemObject> GetEquippedItems()
        {
            return _items.Where(i => i.IsEquipped).ToList().AsReadOnly();
        }
        public ItemObject GetEquippedItem(EquipLocation slot)
        {
            return _items.FirstOrDefault(i => i.IsEquipped && i.ItemModel.EquipLocation == slot) ?? throw new InvalidOperationException("No equipped item found for the specified slot.");
        }
        public bool HasSpaceForItem(ItemObject item)
        {
            // Si el item puede apilar, puedes permitirle ocupar espacio adicional. Si no, solo verifica si hay espacio.
            return _items.Count < 100 || item.CanStack();
        }
        public bool CanReplaceEquipped(EquipLocation slot)
        {
            // Puedes agregar lógica para verificar si la ranura se puede reemplazar (por ejemplo, si es una ranura de equipo única).
            var equippedItem = _items.FirstOrDefault(i => i.IsEquipped && i.ItemModel.EquipLocation == slot);
            return equippedItem == null || equippedItem.ItemModel.CanBeReplaced;
        }
    }
}
