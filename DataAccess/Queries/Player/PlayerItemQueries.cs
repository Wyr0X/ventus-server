namespace VentusServer.DataAccess.Queries
{
    public static class PlayerItemQueries
    {
        public const string CreateTableQuery = @"
        CREATE TABLE IF NOT EXISTS player_items (
            id UUID PRIMARY KEY,
            inventory_id UUID NOT NULL REFERENCES player_inventory(id) ON DELETE CASCADE,
            item_id INT NOT NULL REFERENCES items(id),
            quantity INT DEFAULT 1,
            slot INT,
            custom_data JSONB,
            created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
            updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
        );";

        public const string Insert = @"
        INSERT INTO player_items (id, inventory_id, item_id, quantity, slot, custom_data, created_at, updated_at)
        VALUES (@Id, @InventoryId, @ItemId, @Quantity, @Slot, @CustomData, @CreatedAt, @UpdatedAt);";

        public const string Update = @"
        UPDATE player_items
        SET quantity = @Quantity,
            slot = @Slot,
            custom_data = @CustomData,
            updated_at = @UpdatedAt
        WHERE id = @Id;";

        public const string Delete = @"
        DELETE FROM player_items WHERE id = @Id;";

        public const string DeleteAllByInventoryId = @"
        DELETE FROM player_items WHERE inventory_id = @InventoryId;";

        public const string SelectByInventoryId = @"
        SELECT pi.*, i.name, i.icon, i.item_type
        FROM player_items pi
        JOIN items i ON pi.item_id = i.id
        WHERE pi.inventory_id = @InventoryId;";

        public const string SelectById = @"
        SELECT * FROM player_items WHERE id = @Id;";
    }
}
