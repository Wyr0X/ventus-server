using Game.Models;
using Google.Protobuf.Collections;
using Protos.Game.Session;
using Protos.Game.World;

public class WorldManager
{
    protected EntityManager _entityManager = new EntityManager();

    private readonly Dictionary<int, WorldEntity> _worlds;
    private readonly SyncSystem _syncSystem;
    private readonly Dictionary<int, Entity> _mapsByWorld;


    public WorldManager(EntityManager entityManager, SyncSystem syncSystem)
    {
        _syncSystem = syncSystem;
        _entityManager = entityManager;
        _worlds = new Dictionary<int, WorldEntity>();
        _mapsByWorld = new Dictionary<int, Entity>();
    }
    public Entity GetOrCreateWorld(int worldId)
    {

        if (!_worlds.ContainsKey(worldId))
        {
            // Si el mundo no existe, lo creamos


            WorldComponent worldComponent = new WorldComponent(worldId);
            Component[] components = [
                worldComponent
                ];
            components.Append(worldComponent);
            WorldEntity worldEntity = (WorldEntity)_entityManager.CreateWorldEntity(worldId, components);


            _worlds[worldId] = worldEntity;
            return worldEntity;
        }
        return _worlds[worldId];
    }

    public Entity GetOrCreateMap(int mapId, int worldId)
    {

        if (!_mapsByWorld.ContainsKey(mapId))
        {

            MapComponent mapComponent = new MapComponent(mapId, worldId);

            Component[] components = new Component[]{
                mapComponent
            };

            Entity mapEntity = _entityManager.Create(components);

            _worlds[worldId].Add(mapComponent);
            _mapsByWorld[mapId] = mapEntity;
        }
        return _mapsByWorld[mapId];
    }

    public Entity? GetWorldEntity(int worldId)
    {
        return _worlds[worldId];
    }

    public void RemoveWorldEntity(int worldId)
    {
        if (_worlds.ContainsKey(worldId))
        {
            _worlds.Remove(worldId);
        }
    }
    public void RemoveMapsInWorld(int worldId)
    {
        if (_mapsByWorld.ContainsKey(worldId))
        {
            _mapsByWorld.Remove(worldId);
        }
    }

    public void UpdateWorld()
    {
        foreach (var entity in _worlds.Values)
        {
            WorldEntity world = entity;

            List<Entity> charactersInWorld = GetCharactersInWorld(world.GetWorldId());
            RepeatedField<PlayerState> playersState = [];
            List<Guid> accountsId = [];

            foreach (var entityC in charactersInWorld)
            {
                PlayerEntity playerEntity = (PlayerEntity)entityC;
                Position? position = (Position?)entityC.Get(typeof(Position));
                Character? character = (Character?)entityC.Get(typeof(Character));
                if (character == null || position == null) continue;
                PlayerState playerState = new PlayerState
                {
                    Hp = 100,
                    Id = playerEntity.PlayerId,
                    Name = character.PlayerName,
                    X = position.X,
                    Y = position.Y

                };
                accountsId.Add(character.AccountId);
                playersState.Add(playerState);

                // _syncSystem.UpdatePosition(character.AccountId, character.PlayerId, position.X, position.Y);
            }
            WorldStateUpdate worldStateUpdate =  CreateWorldStateUpdate(playersState);
            _syncSystem.SendWorlState(accountsId, worldStateUpdate);
        }
    }
    public void SpawnPlayer(int worldId, Entity playerSpawnEntity, Character characterSpawn, Position positionOfPlayerSpawn)
    {
        PlayerEntity playerSpawn = (PlayerEntity)playerSpawnEntity;
        if (playerSpawn != null)
        {

            WorldEntity world = _worlds[worldId];

            List<Entity> charactersInWorld = GetCharactersInWorld(worldId);
            Console.WriteLine($"Characters in world: {charactersInWorld.Count}");
            foreach (var entityC in charactersInWorld)
            {
                Character? character = (Character?)entityC.Get(typeof(Character));

                if (character == null) continue;

                _syncSystem.SpawnPlayer(character.AccountId, characterSpawn.PlayerId, positionOfPlayerSpawn.X, positionOfPlayerSpawn.Y);
            }
        }
    }
    public void UnSpawnPlayer(int worldId)
    {

        List<Entity> charactersInWorld = GetCharactersInWorld(worldId);
        if (charactersInWorld.Count == 0)
        {
            RemoveWorldEntity(worldId);
            RemoveMapsInWorld(worldId);
        }

    }
    public List<Entity> GetCharactersInWorld(int worldId)
    {
        List<(IComponent, Entity)> characters = _entityManager.Get(typeof(Character));

        List<Entity> characteresEntity = new List<Entity>();
        foreach (var (component, entity) in characters)
        {
            Entity _entityCharacter = entity;
            Character _componentCharacter = (Character)component;

            if (_componentCharacter.GetCurrentWorldId() == worldId)
            {
                characteresEntity.Add(_entityCharacter);
            }
        }
        return characteresEntity;
    }
    public static WorldStateUpdate CreateWorldStateUpdate(
       RepeatedField<PlayerState> playerStates

    )
    {

        var update = new WorldStateUpdate();
        update.Players.AddRange(playerStates); // Agregar elementos en lugar de asignar directamente
        return update;
    }
}
