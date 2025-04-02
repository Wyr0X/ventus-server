class WorldEntity : Entity
{
    private int _worldId { get; set;}

  public WorldEntity(int id, int worldId) : base(id) // Llamada al constructor base
    {
        _worldId = worldId;
    }


    public int GetWorldId()
    {
        return _worldId;
    }

}