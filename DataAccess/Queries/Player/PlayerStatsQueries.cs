namespace VentusServer.DataAccess.Queries
{
    public static class PlayerStatsQueries
    {
        public const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS player_stats (
                player_id INT PRIMARY KEY REFERENCES players(id) ON DELETE CASCADE,
                level INT DEFAULT 1,
                xp INT DEFAULT 0,
                gold INT DEFAULT 0,
                bank_gold INT DEFAULT 0,
                free_skill_points  INT DEFAULT 0,
                hp INT DEFAULT 100,
                mp INT DEFAULT 100,
                sp INT DEFAULT 100,
                max_hp INT DEFAULT 100,
                max_mp INT DEFAULT 100,
                max_sp INT DEFAULT 100,
                hunger INT DEFAULT 0,
                thirst INT DEFAULT 0,
                killed_npcs INT DEFAULT 0,
                killed_users INT DEFAULT 0,
                deaths INT DEFAULT 0
            );
        ";

        // Query para insertar las estadísticas de un jugador
        public const string Insert = @"
            INSERT INTO player_stats (
                player_id,
                level,
                xp,
                gold,
                bank_gold,
                free_skill_points,
                hp,
                mp,
                sp,
                max_hp,
                max_mp,
                max_sp,
                hunger,
                thirst,
                killed_npcs,
                killed_users,
                deaths,
                last_updated
            ) 
            VALUES (
                @PlayerId, 
                @Level, 
                @Xp, 
                @Gold, 
                @BankGold, 
                @FreeSkillPoints, 
                @Hp, 
                @Mp, 
                @Sp, 
                @MaxHp, 
                @MaxMp, 
                @MaxSp, 
                @Hunger, 
                @Thirst, 
                @KilledNpcs, 
                @KilledUsers, 
                @Deaths, 
                @LastUpdated
            )";

        // Query para eliminar las estadísticas de un jugador por su ID
        public const string DeleteByPlayerId = @"
            DELETE FROM player_stats 
            WHERE player_id = @PlayerId";

        // Query para verificar si existen estadísticas de un jugador por su ID
        public const string ExistsByPlayerId = @"
            SELECT EXISTS (
                SELECT 1 
                FROM player_stats 
                WHERE player_id = @PlayerId
            )";
        // Query para actualizar o insertar las estadísticas de un jugador
        public const string Upsert = @"
            INSERT INTO player_stats (
                player_id,
                level,
                xp,
                gold,
                bank_gold,
                free_skill_points,
                hp,
                mp,
                sp,
                max_hp,
                max_mp,
                max_sp,
                hunger,
                thirst,
                killed_npcs,
                killed_users,
                deaths,
                last_updated
            ) 
            VALUES (
                @PlayerId, 
                @Level, 
                @Xp, 
                @Gold, 
                @BankGold, 
                @FreeSkillPoints, 
                @Hp, 
                @Mp, 
                @Sp, 
                @MaxHp, 
                @MaxMp, 
                @MaxSp, 
                @Hunger, 
                @Thirst, 
                @KilledNpcs, 
                @KilledUsers, 
                @Deaths, 
                @LastUpdated
            )
            ON CONFLICT (player_id) 
            DO UPDATE SET
                level = EXCLUDED.level,
                xp = EXCLUDED.xp,
                gold = EXCLUDED.gold,
                bank_gold = EXCLUDED.bank_gold,
                free_skill_points = EXCLUDED.free_skill_points,
                hp = EXCLUDED.hp,
                mp = EXCLUDED.mp,
                sp = EXCLUDED.sp,
                max_hp = EXCLUDED.max_hp,
                max_mp = EXCLUDED.max_mp,
                max_sp = EXCLUDED.max_sp,
                hunger = EXCLUDED.hunger,
                thirst = EXCLUDED.thirst,
                killed_npcs = EXCLUDED.killed_npcs,
                killed_users = EXCLUDED.killed_users,
                deaths = EXCLUDED.deaths,
                last_updated = EXCLUDED.last_updated";

        // Query para obtener las estadísticas de un jugador por su ID
        public const string SelectByPlayerId = @"
            SELECT
                player_id,
                level,
                xp,
                gold,
                bank_gold,
                free_skill_points,
                hp,
                mp,
                sp,
                max_hp,
                max_mp,
                max_sp,
                hunger,
                thirst,
                killed_npcs,
                killed_users,
                deaths,
                last_updated
            FROM player_stats
            WHERE player_id = @PlayerId";
    }


}
