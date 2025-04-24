namespace Game.Models
{
    public class MapModel : BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }
        public int WorldId { get; set; } // Relación con el mundo al que pertenece

        // Relación con WorldModel
        public WorldModel? WorldModel { get; set; }

        // Lista de jugadores en el mapa
        public List<PlayerLocationModel> PlayersLocation { get; set; } = new List<PlayerLocationModel>();  // Relación entre el jugador y el mapa
        public List<PlayerLocationModel> spawnedPlayers { get; set; } = new List<PlayerLocationModel>();

        // Lógica de negocio

        // Verifica si un jugador puede acceder a este mapa
        public bool CanAccess(int playerLevel)
        {
            return playerLevel >= MinLevel;
        }

        // Verifica si hay espacio disponible para más jugadores
        public bool HasSpace()
        {
            return PlayersLocation.Count < MaxPlayers;
        }

        // Método para agregar un jugador al mapa si hay espacio y puede acceder
        public bool TryAddPlayer(int playerLevel)
        {
            if (CanAccess(playerLevel) && HasSpace())
            {
                return true; // El jugador puede entrar
            }
            return false; // El jugador no puede entrar
        }

        public void RemovePlayer(int playerId)
        {
            var playerLocation = PlayersLocation.FirstOrDefault(p => p.PlayerId == playerId);
            if (playerLocation != null)
            {
                PlayersLocation.Remove(playerLocation);
            }
            else
            {
                throw new InvalidOperationException("El jugador con el ID proporcionado no existe en este mundo.");
            }
        }

        // Método para imprimir los detalles del mapa
        public void PrintMapDetails()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, "### Map Details ###");
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, $"ID: {Id}");
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, $"Name: {Name}");
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, $"Min Level: {MinLevel}");
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, $"Max Players: {MaxPlayers}");
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, $"World ID: {WorldId}");
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, $"Players Count: {PlayersLocation.Count}");
            LoggerUtil.Log(LoggerUtil.LogTag.MapModel, "### End of Map Details ###");
        }
    }
}
