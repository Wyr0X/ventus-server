namespace VentusServer.DataAccess.Queries
{
    public static class PlayerLocationQueries
    {
        public const string CreateTableQuery = @"
                    CREATE TABLE IF NOT EXISTS player_locations (
                        player_id INT PRIMARY KEY REFERENCES players(id) ON DELETE CASCADE,
                        world_id INT NOT NULL REFERENCES worlds(id),
                        map_id INT NOT NULL REFERENCES maps(id),
                        pos_x INT NOT NULL,
                        pos_y INT NOT NULL
                    );
                ";
        public const string SelectByPlayerId = @"
            SELECT player_id, world_id, map_id, pos_x, pos_y
            FROM player_locations
            WHERE player_id = @PlayerId
            LIMIT 1;
        ";

        public const string Insert = @"
            INSERT INTO player_locations (player_id, world_id, map_id, pos_x, pos_y)
            VALUES (@PlayerId, @WorldId, @MapId, @PosX, @PosY);
        ";

        public const string InsertOrUpdate = @"
            INSERT INTO player_locations (player_id, world_id, map_id, pos_x, pos_y)
            VALUES (@PlayerId, @WorldId, @MapId, @PosX, @PosY)
            ON CONFLICT (player_id) DO UPDATE SET
                world_id = EXCLUDED.world_id,
                map_id = EXCLUDED.map_id,
                pos_x = EXCLUDED.pos_x,
                pos_y = EXCLUDED.pos_y;
        ";

        public const string Delete = @"
            DELETE FROM player_locations
            WHERE player_id = @PlayerId;
        ";

        public const string SelectPlayersByWorldId = @"
            SELECT *
            FROM player_locations
            WHERE world_id = @WorldId;
        ";
        public const string SelectPlayersByMapId = @"
            SELECT *
            FROM player_locations
            WHERE map_id = @MapId;
        ";
    }
}
