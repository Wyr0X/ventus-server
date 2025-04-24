using VentusServer.Domain.Models;

public interface IPlayerSpellsDAO
{
    Task<PlayerSpellsModel?> GetByIdAsync(int id);
    /// <summary>
    /// Devuelve el inventario de hechizos del jugador o null si no existe.
    /// </summary>
    Task<PlayerSpellsModel?> GetByPlayerId(int playerId);

    /// <summary>
    /// Crea un nuevo registro de inventario de hechizos para el jugador.
    /// </summary>
    Task CreateAsync(PlayerSpellsModel model);

    /// <summary>
    /// Actualiza la lista de hechizos del jugador.
    /// </summary>
    Task UpdateSpells(int playerId, List<PlayerSpellModel> spells);

    /// <summary>
    /// Actualiza la cantidad máxima de espacios para hechizos del jugador.
    /// </summary>
    Task UpdateMaxSlots(int playerId, int maxSlots);

    /// <summary>
    /// Inserta o actualiza el inventario de hechizos del jugador.
    /// </summary>
    Task UpsertAsync(PlayerSpellsModel model);

    /// <summary>
    /// Elimina el inventario de hechizos del jugador.
    /// </summary>
    Task DeleteByPlayerId(int playerId);

    /// <summary>
    /// Comprueba si existe inventario de hechizos para un jugador dado.
    /// </summary>
    Task<bool> ExistsByPlayerId(int playerId);
}