namespace VentusServer.DataAccess.Queries
{
    public static class PlayerInventoryQueries
    {
        public const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS player_inventory (
                id SERIAL PRIMARY KEY,
                player_id INT NOT NULL UNIQUE REFERENCES players(id) ON DELETE CASCADE,
                gold INT DEFAULT 0,
                items JSONB DEFAULT '[]',
                max_slots INT DEFAULT 20,
                created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );";

        public const string Insert = @"
            INSERT INTO player_inventory (player_id, gold, items, max_slots, created_at, updated_at)
            VALUES (@PlayerId, @Gold, @Items::jsonb, @MaxSlots, @CreatedAt, @UpdatedAt)
            RETURNING id;";

        public const string UpdateGold = @"
            UPDATE player_inventory
            SET gold = @Gold,
                updated_at = @UpdatedAt
            WHERE player_id = @PlayerId;";

        public const string UpdateItems = @"
            UPDATE player_inventory
            SET items = @Items::jsonb,
                updated_at = @UpdatedAt
            WHERE player_id = @PlayerId;";

        public const string UpdateMaxSlots = @"
            UPDATE player_inventory
            SET max_slots = @MaxSlots,
                updated_at = @UpdatedAt
            WHERE player_id = @PlayerId;";

        public const string SelectByPlayerId = @"
            SELECT * FROM player_inventory
            WHERE player_id = @PlayerId
            LIMIT 1;";

        public const string DeleteByPlayerId = @"
            DELETE FROM player_inventory
            WHERE player_id = @PlayerId;";

        public const string ExistsByPlayerId = @"
            SELECT EXISTS (
                SELECT 1 FROM player_inventory WHERE player_id = @PlayerId
            );";
    }
}
