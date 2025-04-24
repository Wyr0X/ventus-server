using System;
using System.Collections.Generic;

namespace VentusServer.Domain.Models
{
    public class PlayerSpellModel
    {
        public required string SpellId { get; set; }
        public bool IsEquipped { get; set; } // Si el hechizo está equipado/preparado
        public int Slot { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PlayerSpellsModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MaxSlots { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PlayerSpellModel> Spells { get; set; } = new();
        public int Slots = 10;
    }
}
