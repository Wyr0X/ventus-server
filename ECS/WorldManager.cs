using Game.Models;

public class WorldManager
{
    private readonly Dictionary<int, World> _worlds = new Dictionary<int, World>();
    private int _nextWorldId = 0;

    public int CreateWorld()
    {
        int worldId = _nextWorldId++;
        _worlds[worldId] = new World(worldId);

        return worldId;
    }

    public World? GetWorld(int worldId)
    {
        return _worlds.GetValueOrDefault(worldId);
    }

    public void RemoveWorld(int worldId)
    {
        if (_worlds.ContainsKey(worldId))
        {
            _worlds.Remove(worldId);
        }
    }

    public void Update() {
        foreach (var world in _worlds.Values) {
            world.Update();
        }
    }

    public void SpawnPlayer(PlayerModel playerBasic, PlayerLocation playerLocation) {
        // Buscar World
        // if (GetWorld(playerLocation.World.Id) == null) {
        //     // Instanciar el World
        //     CreateWorld(playerLocation.World.Id);
        // }

        // var world = GetWorld(playerLocation.World.Id);
        // world.SpawnPlayer(playerLocation);
        // ingresamos el pj al world
        // el world valida que haya espacio, permisos, etc.etc
        // se crea la entidad del pj -> con todos los componentes (playerstats, movimiento, networking, etc)

        // ------
        // el world va a tener sistema que recorre las entidades y envia el WorldState
        // va repartiendo las entidades agrupandolas por MAPA y a cada player se le envia el WorldState de ese mapa
        // el WorldState incluye información del World e información del Mapa
    }
}
