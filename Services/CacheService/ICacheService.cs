public interface ICacheService
{
    // Obtiene el valor almacenado en la caché, o null si no existe
    T Get<T>(string key) where T : class;

    // Almacena un valor en la caché bajo una clave
    void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    // Elimina un valor de la caché
    void Remove(string key);

    // Verifica si una clave existe en la caché
    bool Exists(string key);
}
