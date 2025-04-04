public abstract class BaseCachedService<TModel, TId>
{
    protected readonly Dictionary<TId, TModel> _cache = new();

    // Este método es abstracto, y por eso puede ser override en los hijos
    protected abstract TModel CreateModel(TId id);

    public TModel? GetIfLoaded(TId id)
    {
        return _cache.TryGetValue(id, out var model) ? model : default;
    }

    public TModel GetOrCreate(TId id)
    {
        if (_cache.TryGetValue(id, out var model)) return model;
        model = CreateModel(id);
        _cache[id] = model;
        return model;
    }
}
