using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VentusServer.Domain.Models;

namespace VentusServer.Services
{
    public class StoreService
    {
        private readonly ItemService _itemService;
        private readonly PlayerInventoryService _inventoryService;
        private readonly PlayerService _playerService;
        private readonly PlayerSpellsService _playerSpellInventoryService;
        private readonly SpellService _spellService;

        public StoreService(
            ItemService itemService,
            PlayerInventoryService inventoryService,
            PlayerService playerService,
            SpellService spellService,
            PlayerSpellsService playerSpellInventoryService)
        {
            _itemService = itemService;
            _inventoryService = inventoryService;
            _playerService = playerService;
            _spellService = spellService;
            _playerSpellInventoryService = playerSpellInventoryService;
        }

        public async Task<BuyItemResult> BuyItemAsync(PlayerModel player, int itemId, int quantity)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Iniciando compra de ítem {itemId} por {quantity} unidades para el jugador {player.Id}.", "BuyItemAsync");

            if (quantity <= 0)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, "Cantidad inválida en compra.", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Cantidad inválida.");
            }

            var item = await _itemService.GetItemByIdAsync(itemId);
            if (item == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Ítem no encontrado. ItemID: {itemId}", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Ítem no encontrado.");
            }

            var inventory = await _inventoryService.GetInventoryByPlayerId(player.Id);
            if (inventory == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Inventario no encontrado para el jugador {player.Id}", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Inventario no encontrado.");
            }

            var totalCost = item.Price * quantity;
            if (inventory.Gold < totalCost)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Oro insuficiente. Necesitado: {totalCost}, Disponible: {inventory.Gold}", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Oro insuficiente.");
            }

            inventory.Gold -= totalCost;
            await _inventoryService.SaveInventoryAsync(inventory);

            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Compra completada. Ítem {itemId} x{quantity} para jugador {player.Id}.", "BuyItemAsync");
            return BuyItemResult.SuccessBuild("Compra realizada correctamente.");
        }

        public async Task<BuyResult> BuyCartAsync(PlayerModel player, List<CartItem> items, List<CartSpell> spells)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Iniciando compra de carrito para jugador {player.Id}.", "BuyCartAsync");

            int totalCost = 0;

            foreach (var cartItem in items)
            {
                var item = await _itemService.GetItemByIdAsync(cartItem.ItemId);
                if (item == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Ítem no encontrado en carrito: {cartItem.ItemId}", "BuyCartAsync", true);
                    return BuyResult.Fail($"Ítem con ID {cartItem.ItemId} no existe.");
                }

                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Ítem validado: {cartItem.ItemId} x{cartItem.Quantity}", "BuyCartAsync");
                totalCost += item.Price * cartItem.Quantity;
            }

            foreach (var cartSpell in spells)
            {
                var spell = await _spellService.GetSpellByIdAsync(cartSpell.SpellId);
                if (spell == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Hechizo no encontrado en carrito: {cartSpell.SpellId}", "BuyCartAsync", true);
                    return BuyResult.Fail($"Hechizo con ID {cartSpell.SpellId} no existe.");
                }

                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Hechizo validado: {cartSpell.SpellId} x{cartSpell.Quantity}", "BuyCartAsync");
                totalCost += spell.Price * cartSpell.Quantity;
            }

            var inventory = await _inventoryService.GetInventoryByPlayerId(player.Id);
            if (inventory == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Inventario no encontrado para jugador {player.Id}", "BuyCartAsync", true);
                return BuyResult.Fail("No se encontro el inventario.");
            }

            if (inventory.Gold < totalCost)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Fondos insuficientes. Total requerido: {totalCost}, disponible: {inventory.Gold}", "BuyCartAsync", true);
                return BuyResult.Fail("No tienes suficiente oro para comprar este carrito.");
            }

            if (inventory.MaxSlots < inventory.Items.Count() + items.Count())
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"No tienes suficiente espacio en el inventario.");
                return BuyResult.Fail("No tienes suficiente espacio en el inventario.");
            }

            inventory.Gold -= totalCost;
            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Oro descontado. Nuevo saldo: {inventory.Gold}", "BuyCartAsync");

            foreach (var cartItem in items)
            {
                var item = await _itemService.GetItemByIdAsync(cartItem.ItemId);
                if (item != null)
                {
                    var existingItem = inventory.Items.FirstOrDefault(i => i.ItemId == cartItem.ItemId);
                    if (existingItem != null)
                    {
                        if (item.MaxStack != null && existingItem.Quantity + cartItem.Quantity <= item.MaxStack)
                        {
                            existingItem.Quantity += cartItem.Quantity;
                        }
                        else if (item.MaxStack == null)
                        {
                            existingItem.Quantity += cartItem.Quantity;
                        }
                        else
                        {
                            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Stack máximo alcanzado para item {cartItem.ItemId}", "BuyCartAsync", true);
                            return BuyResult.Fail($"No se pueden agregar {cartItem.Quantity} del ítem con ID {cartItem.ItemId}. Se alcanzaría o excedería el stack máximo.");
                        }
                    }
                    else
                    {

                        int nextSlot = GetNextAvailableInventorySlot(inventory);
                        var newItem = new PlayerInventoryItemModel
                        {
                            ItemId = cartItem.ItemId,
                            Quantity = cartItem.Quantity,
                            Slot = nextSlot,
                            isEquipped = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            Name = item.Name.Es,
                            Icon = item.IconPath,
                        };
                        inventory.Items.Add(newItem);
                    }
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Item no encontrado al agregar al inventario: {cartItem.ItemId}", "BuyCartAsync", true);
                    return BuyResult.Fail($"Ítem con ID {cartItem.ItemId} no existe.");
                }
            }

            var spellInventory = await _playerSpellInventoryService.GetPlayerSpellsByIdAsync(player.Id);
            if (spellInventory == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Creando nuevo inventario de hechizos para jugador {player.Id}", "BuyCartAsync");

                spellInventory = new PlayerSpellsModel
                {
                    PlayerId = player.Id,
                    Spells = new List<PlayerSpellModel>(),
                    MaxSlots = 10,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            if (spellInventory.MaxSlots < spellInventory.Spells.Count() + spells.Count())
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"No tienes suficiente espacio para los hechizos.");
                return BuyResult.Fail("No tienes suficiente espacio para los hechizos.");
            }
            foreach (var cartSpell in spells)
            {
                var spell = await _spellService.GetSpellByIdAsync(cartSpell.SpellId);
                if (spell != null)
                {
                    var existingSpell = spellInventory.Spells.FirstOrDefault(s => s.SpellId == cartSpell.SpellId);
                    if (existingSpell != null)
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Hechizo ya poseído: {cartSpell.SpellId}", "BuyCartAsync", true);
                        return BuyResult.Fail($"Ya tienes el hechizo con ID {cartSpell.SpellId} en tu inventario.");
                    }
                    else
                    {
                        int nextSlot = GetNextAvailableSpellsSlot(spellInventory);

                        var newSpell = new PlayerSpellModel
                        {
                            SpellId = cartSpell.SpellId,
                            IsEquipped = false,
                            Slot = nextSlot,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        spellInventory.Spells.Add(newSpell);
                    }
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Hechizo no encontrado al agregar: {cartSpell.SpellId}", "BuyCartAsync", true);
                    return BuyResult.Fail($"Hechizo con ID {cartSpell.SpellId} no existe.");
                }
            }

            await _inventoryService.SaveInventoryAsync(inventory);
            await _playerSpellInventoryService.UpsertSpellAsync(spellInventory);

            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Carrito comprado exitosamente para jugador {player.Id}", "BuyCartAsync");
            return BuyResult.CreateSuccess();
        }
        private int GetNextAvailableInventorySlot(PlayerInventoryModel inventory)
        {
            // Creamos un HashSet con todos los slots ocupados
            HashSet<int> occupiedSlots = inventory.Items.Select(item => item.Slot).ToHashSet();

            // Buscamos el primer slot no ocupado
            for (int slot = 0; slot < inventory.MaxSlots; slot++)
            {
                if (!occupiedSlots.Contains(slot))
                {
                    return slot + 1;
                }
            }

            // Si no hay espacio disponible
            throw new InvalidOperationException("No hay slots disponibles en el inventario.");
        }
        private int GetNextAvailableSpellsSlot(PlayerSpellsModel playerSpells)
        {
            // Creamos un HashSet con todos los slots ocupados
            HashSet<int> occupiedSlots = playerSpells.Spells.Select(spell => spell.Slot).ToHashSet();

            // Buscamos el primer slot no ocupado
            for (int slot = 0; slot < playerSpells.MaxSlots; slot++)
            {
                if (!occupiedSlots.Contains(slot))
                {
                    return slot + 1;
                }
            }

            // Si no hay espacio disponible
            throw new InvalidOperationException("No hay slots disponibles en el inventario.");
        }
    }

    public class BuyItemResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static BuyItemResult FailBuild(string message) => new BuyItemResult { Success = false, ErrorMessage = message };
        public static BuyItemResult SuccessBuild(string message) => new BuyItemResult { Success = true, ErrorMessage = null };
    }
}
