namespace VentusServer.DataAccess.Queries
{
    public static class MapQueries
    {
        public const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS maps (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                min_level INT NOT NULL,
                max_players INT NOT NULL,
                world_id INT NOT NULL REFERENCES worlds(id) ON DELETE CASCADE
            );";

        public const string SelectById = @"
            SELECT id, name, min_level, max_players, world_id
            FROM maps
            WHERE id = @Id
            LIMIT 1;";

        public const string SelectAll = @"
            SELECT id, name, min_level, max_players, world_id
            FROM maps;";

        public const string Insert = @"
            INSERT INTO maps (name, min_level, max_players, world_id)
            VALUES (@Name, @MinLevel, @MaxPlayers, @WorldId)
            RETURNING id;";

        public const string Update = @"
            UPDATE maps
            SET name = @Name,
                min_level = @MinLevel,
                max_players = @MaxPlayers,
                world_id = @WorldId
            WHERE id = @Id;";

        public const string Delete = @"
            DELETE FROM maps
            WHERE id = @Id;";

        public const string SelectByWorldId = @"
            SELECT id, name, min_level, max_players, world_id
            FROM maps
            WHERE world_id = @WorldId;";

    }
}
