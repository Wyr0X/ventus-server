namespace VentusServer.DataAccess.Queries
{
    public static class ItemQueries
    {
        public const string CreateTableQuery = @"
CREATE TABLE IF NOT EXISTS items (
    id SERIAL PRIMARY KEY,
    item_key VARCHAR(50) NOT NULL UNIQUE,
    name JSONB NOT NULL,
    description JSONB NOT NULL,
    type VARCHAR(20) NOT NULL,
    rarity VARCHAR(20) NOT NULL DEFAULT 'Common',
    required_level INT,
    quantity INT,
    price INT,
    max_stack INT,
    is_tradable BOOLEAN NOT NULL DEFAULT FALSE,
    is_droppable BOOLEAN NOT NULL DEFAULT FALSE,
    is_usable BOOLEAN NOT NULL DEFAULT FALSE,
    icon_path TEXT,
    sprite INT[],
    sound TEXT,
    data JSONB, -- <== ¡acá está el campo para los datos variables!
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);";


        public const string SelectById = @"
            SELECT * FROM items
            WHERE id = @Id
            LIMIT 1;";

        public const string SelectByKey = @"
            SELECT * FROM items
            WHERE item_key = @Key
            LIMIT 1;";

        public const string SelectAll = "SELECT * FROM items;";

        public const string Insert = @"
            INSERT INTO items (
                item_key, name, description, type, rarity, required_level, price, quantity, max_stack,
                is_tradable, is_droppable, is_usable,
                icon_path, sprite, sound, data, created_at, updated_at
            ) VALUES (
                @Key, @Name::jsonb, @Description::jsonb, @Type, @Rarity, @RequiredLevel, @Price, @Quantity, @MaxStack,
                @IsTradable, @IsDroppable, @IsUsable,
                @IconPath, @Sprite, @Sound, @Data::jsonb, @CreatedAt, @UpdatedAt
            );";


        public const string Update = @"
            UPDATE items SET
                name = @Name::jsonb,
                description = @Description::jsonb,
                type = @Type,
                rarity = @Rarity,
                required_level = @RequiredLevel,
                price = @Price,
                quantity = @Quantity,
                max_stack = @MaxStack,
                is_tradable = @IsTradable,
                is_droppable = @IsDroppable,
                is_usable = @IsUsable,
                icon_path = @IconPath,
                sprite = @Sprite,
                sound = @Sound,
                data = @Data::jsonb,
                updated_at = @UpdatedAt
            WHERE item_key = @Key;";


        public const string Upsert = @"
            INSERT INTO items (
                item_key, name, description, type, rarity, required_level, price, quantity, max_stack,
                is_tradable, is_droppable, is_usable,
                icon_path, sprite, sound, data, created_at, updated_at
            ) VALUES (
                @Key, @Name::jsonb, @Description::jsonb, @Type, @Rarity, @RequiredLevel, @Price, Quantity, @MaxStack,
                @IsTradable, @IsDroppable, @IsUsable,
                @IconPath, @Sprite, @Sound, @Data::jsonb, @CreatedAt, @UpdatedAt
            )
            ON CONFLICT (item_key) DO UPDATE SET
                name = EXCLUDED.name,
                description = EXCLUDED.description,
                type = EXCLUDED.type,
                rarity = EXCLUDED.rarity,
                required_level = EXCLUDED.required_level,
                price = EXCLUDED.Price,
                quantity = EXCLUDED.Quantity,

                max_stack = EXCLUDED.max_stack,
                is_tradable = EXCLUDED.is_tradable,
                is_droppable = EXCLUDED.is_droppable,
                is_usable = EXCLUDED.is_usable,
                icon_path = EXCLUDED.icon_path,
                sprite = EXCLUDED.sprite,
                sound = EXCLUDED.sound,
                data = EXCLUDED.data,
                updated_at = EXCLUDED.updated_at;";


        public const string Delete = "DELETE FROM items WHERE id = @Id;";
        public const string ExistsByKey = "SELECT EXISTS(SELECT 1 FROM items WHERE item_key = @Key);";
    }
}
