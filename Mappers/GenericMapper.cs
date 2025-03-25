public static class GenericMapper
{
    // Mapea una entidad a un modelo de dominio usando un delegado para crear el modelo
    public static TDomain ToDomainModel<TEntity, TDomain>(
        TEntity entity,
        Func<TEntity, TDomain> createModelFunc)
        where TEntity : class
        where TDomain : class
    {
        // Usamos el delegado para crear el modelo
        var domainModel = createModelFunc(entity);

        // Realiza el mapeo de propiedades
        foreach (var property in typeof(TEntity).GetProperties())
        {
            var domainProperty = typeof(TDomain).GetProperty(property.Name);
            if (domainProperty != null && domainProperty.CanWrite)
            {
                domainProperty.SetValue(domainModel, property.GetValue(entity));
            }
        }

        return domainModel;
    }

    // Mapea un modelo de dominio a una entidad
    public static TEntity ToEntity<TDomain, TEntity>(
        TDomain model,
        Func<TDomain, TEntity> createEntityFunc)
        where TEntity : class
        where TDomain : class
    {
        // Usamos el delegado para crear la entidad
        var entity = createEntityFunc(model);

        // Realiza el mapeo de propiedades
        foreach (var property in typeof(TDomain).GetProperties())
        {
            var entityProperty = typeof(TEntity).GetProperty(property.Name);
            if (entityProperty != null && entityProperty.CanWrite)
            {
                entityProperty.SetValue(entity, property.GetValue(model));
            }
        }

        return entity;
    }
}
