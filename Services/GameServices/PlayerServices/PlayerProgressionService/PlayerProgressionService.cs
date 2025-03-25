using Game.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VentusServer.Database;
using VentusServer.Models;
using VentusServer.Services; // Asegúrate de que ICacheService esté en este espacio de nombres

namespace Game.Services
{
    public class PlayerProgressionService : PlayerModuleService<PlayerEconomy, PlayerEconomyEntity>
    {
        // Aquí ya no necesitas definir el constructor, ya que la clase base lo maneja
        public PlayerProgressionService(IRepository<PlayerEconomyEntity> repository, ICacheService cache)
            : base(repository, cache)  // Pasamos las dependencias al constructor de la clase base
        {
            // No es necesario hacer nada más en el constructor aquí
        }

        // Aquí puedes agregar métodos específicos para este servicio si es necesario
    }
}
