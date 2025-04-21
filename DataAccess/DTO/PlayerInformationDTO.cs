namespace Game.DTOs
{
    public class PlayerLocationDTO
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        // Información del mapa donde está el jugador
        public required MapDTO Map { get; set; }

        // Información del mundo donde está el mapa
        public required WorldDTO World { get; set; }
    }

    public class MapDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }
    }

    public class WorldDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class PlayerDTO
    {
        public int Id { get; set; }
        public required Guid AccountId { get; set; }
        public required string Name { get; set; }
        public required int Gender { get; set; }
        public required int Race { get; set; }
        public int Level { get; set; }
        public required int Class { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public required string Status { get; set; }

        // Información de la ubicación del jugador
        public PlayerLocationDTO? Location { get; set; }
        public PlayerStatsDTO? Stats { get; set; }
        public PlayerInventoryDTO? Inventory { get; set; }
    }

    public class GetPlayersResponseDTO
    {
        public required List<PlayerDTO> Players { get; set; }
    }
}
public class PlayerStatsDTO
{
    public int Level { get; set; }
    public int Xp { get; set; }
    public int Gold { get; set; }
    public int BankGold { get; set; }
    public int FreeSkillPoints { get; set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int Sp { get; set; }
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public int MaxSp { get; set; }
    public int Hunger { get; set; }
    public int Thirst { get; set; }
    public int KilledNpcs { get; set; }
    public int KilledUsers { get; set; }
    public int Deaths { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class PlayerInventoryDTO
{
    public List<InventoryItemDTO> Items { get; set; } = new();
}

public class InventoryItemDTO
{
    public int ItemId { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; } = false;
    public int Slot {get; set;}
}
