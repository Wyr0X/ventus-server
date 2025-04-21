public class PlayerStatsFactory
{
    public static PlayerStatsModel CreateDefaultPlayerStats(Race race, Gender gender)
    {
        // Crear un modelo base con valores predeterminados
        var stats = new PlayerStatsModel
        {
            Level = 1,
            Xp = 0,
            Gold = 0,
            BankGold = 0,
            FreeSkillPoints = 5, // Asignación inicial de puntos de habilidad
            Hp = 0,
            Mp = 0,
            Sp = 0,
            MaxHp = 0,
            MaxMp = 0,
            MaxSp = 0,
            Hunger = 100,
            Thirst = 100,
            KilledNpcs = 0,
            KilledUsers = 0,
            Deaths = 0,
            LastUpdated = DateTime.UtcNow
        };

        // Asignar valores predeterminados dependiendo de la raza
        stats = AsignarStatsPorRaza(stats, race);

        // Ajustes adicionales si es necesario por el género
        stats = AjustarPorGenero(stats, gender);

        stats.Hp = stats.MaxHp;
        stats.Mp = stats.MaxMp;
        stats.Sp = stats.MaxSp;

        return stats;
    }

    private static PlayerStatsModel AsignarStatsPorRaza(PlayerStatsModel stats, Race race)
    {
        // Asignar los valores específicos de cada raza
        switch (race)
        {
            case Race.Humano:
                stats.MaxHp = 100;
                stats.MaxMp = 50;
                stats.MaxSp = 30;
                break;
            case Race.Elfo:
                stats.MaxHp = 80;
                stats.MaxMp = 70;
                stats.MaxSp = 40;
                break;
            case Race.ElfoOscuro:
                stats.MaxHp = 90;
                stats.MaxMp = 60;
                stats.MaxSp = 50;
                break;
            case Race.Enano:
                stats.MaxHp = 120;
                stats.MaxMp = 40;
                stats.MaxSp = 20;
                break;
            case Race.Orcos:
                stats.MaxHp = 110;
                stats.MaxMp = 50;
                stats.MaxSp = 30;
                break;
            case Race.Gnomo:
                stats.MaxHp = 70;
                stats.MaxMp = 80;
                stats.MaxSp = 60;
                break;
            default:
                throw new ArgumentException("Raza no válida");
        }

        return stats;
    }

    private static PlayerStatsModel AjustarPorGenero(PlayerStatsModel stats, Gender gender)
    {
        // Si decides añadir alguna bonificación o penalización según el género, puedes hacerlo aquí
        if (gender == Gender.Female)
        {
            // Ejemplo de bonificación por género, por si alguna vez decides añadirla
            stats.MaxHp += 5;
        }

        return stats;
    }
}
