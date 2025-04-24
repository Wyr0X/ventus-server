namespace VentusServer.DataAccess.Queries
{
    public static class PlayerSpellsQueries
    {
        public const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS player_spells (
                id SERIAL PRIMARY KEY,
                player_id INT NOT NULL UNIQUE REFERENCES players(id) ON DELETE CASCADE,
                spells JSONB DEFAULT '[]',
                max_slots INT DEFAULT 20,
                created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );";

        public const string Insert = @"
            INSERT INTO player_spells (player_id, spells, max_slots, created_at, updated_at)
            VALUES (@PlayerId, @Spells::jsonb, @MaxSlots, @CreatedAt, @UpdatedAt)
            RETURNING id;";

        public const string UpdateSpells = @"
            UPDATE player_spells
            SET spells = @Spells::jsonb,
                updated_at = @UpdatedAt
            WHERE player_id = @PlayerId;";

        public const string UpdateMaxSlots = @"
            UPDATE player_spells
            SET max_slots = @MaxSlots,
                updated_at = @UpdatedAt
            WHERE player_id = @PlayerId;";

        public const string SelectByPlayerId = @"
            SELECT * FROM player_spells
            WHERE player_id = @PlayerId
            LIMIT 1;";

        public const string DeleteByPlayerId = @"
            DELETE FROM player_spells
            WHERE player_id = @PlayerId;";

        public const string ExistsByPlayerId = @"
            SELECT EXISTS (
                SELECT 1 FROM player_spells WHERE player_id = @PlayerId
            );";
        public const string SelectById = "SELECT * FROM player_spells WHERE Id = @Id";
    }
}
