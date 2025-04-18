namespace VentusServer.DataAccess.Queries
{
    public static class ItemQueries
    {
        public const string CreateTableQuery = @"
        CREATE TABLE IF NOT EXISTS items (
            id SERIAL PRIMARY KEY,
            key VARCHAR(50) NOT NULL UNIQUE,
            name JSONB NOT NULL,
            description JSONB NOT NULL,
            hp_min INT,
            hp_max INT,
            mp VARCHAR(10),
            sprite INT[] NOT NULL,
            sound VARCHAR(50),
            created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        );";

        public const string SelectById = "SELECT * FROM items WHERE id = @Id LIMIT 1;";
        public const string SelectByKey = "SELECT * FROM items WHERE key = @Key LIMIT 1;";
        public const string SelectAll = "SELECT * FROM items;";

        public const string Insert = @"
            INSERT INTO items (key, name, description, hp_min, hp_max, mp, sprite, sound)
            VALUES (@Key, @Name::jsonb, @Description::jsonb, @HpMin, @HpMax, @Mp, @Sprite, @Sound);";

        public const string Update = @"
            UPDATE items SET
                name = @Name::jsonb,
                description = @Description::jsonb,
                hp_min = @HpMin,
                hp_max = @HpMax,
                mp = @Mp,
                sprite = @Sprite,
                sound = @Sound
            WHERE key = @Key;";

        public const string Upsert = @"
            INSERT INTO items (key, name, description, hp_min, hp_max, mp, sprite, sound)
            VALUES (@Key, @Name::jsonb, @Description::jsonb, @HpMin, @HpMax, @Mp, @Sprite, @Sound)
            ON CONFLICT (key) DO UPDATE SET
                name = EXCLUDED.name,
                description = EXCLUDED.description,
                hp_min = EXCLUDED.hp_min,
                hp_max = EXCLUDED.hp_max,
                mp = EXCLUDED.mp,
                sprite = EXCLUDED.sprite,
                sound = EXCLUDED.sound;";

        public const string Delete = "DELETE FROM items WHERE id = @Id;";
        public const string ExistsByKey = "SELECT EXISTS(SELECT 1 FROM items WHERE key = @Key);";
    }
}
