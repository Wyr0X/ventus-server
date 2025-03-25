public static class PlayerMapper
{
    // Mapea de Entity a modelo de dominio (para la lógica del negocio)
    public static Player ToDomainModel(PlayerEntity entity)
    {
        return new Player
        {
            Id = entity.Id,
            AccountId = entity.AccountId,
            Name = entity.Name,
            Gender = entity.Gender,
            Race = entity.Race,
            Level = entity.Level,
            Class = entity.Class,
            CreatedAt = entity.CreatedAt,
            LastLogin = entity.LastLogin,
            Status = entity.Status,
            // Relacionar otras propiedades necesarias (por ejemplo, Location y WorldRelations)
        };
    }

    // Mapea del modelo de dominio a la entidad de base de datos (para persistencia)
    public static PlayerEntity ToEntity(Player model)
    {
        return new PlayerEntity
        {
            Id = model.Id,
            AccountId = model.AccountId,
            Name = model.Name,
            Gender = model.Gender,
            Race = model.Race,
            Level = model.Level,
            Class = model.Class,
            CreatedAt = model.CreatedAt,
            LastLogin = model.LastLogin,
            Status = model.Status,
            // Relacionar otras propiedades necesarias
        };
    }

    // Mapea del modelo de dominio al modelo de presentación para la vista (PlayerModel)
    public static PlayerModel ToPlayerModel(Player model)
    {
        return new PlayerModel
        {
            Id = model.Id,
            Name = model.Name,
            Gender = model.Gender,
            Race = model.Race,
            Level = model.Level,
            Class = model.Class,
            Status = model.Status,
            LastLogin = model.LastLogin,
            // Aquí puedes agregar más propiedades necesarias para la interfaz
        };
    }

    // Mapea del modelo de presentación al modelo de dominio
    public static Player ToDomainModel(PlayerModel model)
    {
        return new Player
        {
            Id = model.Id,
            Name = model.Name,
            Gender = model.Gender,
            Race = model.Race,
            Level = model.Level,
            Class = model.Class,
            Status = model.Status,
            LastLogin = model.LastLogin,
            // Relacionar otras propiedades necesarias
        };
    }
}
