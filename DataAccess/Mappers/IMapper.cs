namespace VentusServer.DataAccess.Mappers
{
    public interface IMapper<TModel, TEntity>
    {
        TModel ToModel(TEntity entity);
        TEntity ToEntity(TModel model);
    }
}
