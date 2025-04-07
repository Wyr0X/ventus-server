namespace VentusServer.DataAccess.Queries
{
    public static class WorldQueries
    {
        public const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS worlds (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                description TEXT,
                max_maps INT NOT NULL,
                max_players INT NOT NULL,
                level_requirements INT NOT NULL
            );";

        public const string Insert = @"INSERT INTO worlds (name, description, max_maps, max_players, level_requirements)
            VALUES (@Name, @Description, @MaxMaps, @MaxPlayers, @LevelRequirements)
            RETURNING id, name, description, max_maps, max_players, level_requirements";

        public const string SelectById = @"
            SELECT * FROM worlds
            WHERE id = @WorldId
            LIMIT 1;";

        public const string SelectAll = @"
            SELECT * FROM worlds;";

        public const string Update = @"
            UPDATE worlds
            SET name = @Name,
                description = @Description,
                max_maps = @MaxMaps,
                max_players = @MaxPlayers,
                level_requirements = @LevelRequirements
            WHERE id = @Id;";

        public const string Upsert = @"
            INSERT INTO worlds (id, name, description, max_maps, max_players, level_requirements)
            VALUES (@Id, @Name, @Description, @MaxMaps, @MaxPlayers, @LevelRequirements)
            ON CONFLICT (id) DO UPDATE SET
                name = EXCLUDED.name,
                description = EXCLUDED.description,
                max_maps = EXCLUDED.max_maps,
                max_players = EXCLUDED.max_players,
                level_requirements = EXCLUDED.level_requirements;";

        public const string Delete = @"
            DELETE FROM worlds
            WHERE id = @WorldId;";
    }
}
