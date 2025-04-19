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
            type VARCHAR(20) NOT NULL,
            rarity VARCHAR(20) NOT NULL DEFAULT 'Common',
            damage INT,
            defense INT,
            mana_bonus INT,
            strength_bonus INT,
            speed_bonus INT,
            hp_min INT,
            hp_max INT,
            mp VARCHAR(10),
            max_stack INT NOT NULL DEFAULT 1,
            icon_path TEXT,
            sprite INT[] NOT NULL,
            sound VARCHAR(50),
            is_tradable BOOLEAN NOT NULL DEFAULT true,
            is_droppable BOOLEAN NOT NULL DEFAULT true,
            is_usable BOOLEAN NOT NULL DEFAULT true,
            created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
            updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        );";

        public const string SelectById = "SELECT * FROM items WHERE id = @Id LIMIT 1;";
        public const string SelectByKey = "SELECT * FROM items WHERE key = @Key LIMIT 1;";
        public const string SelectAll = "SELECT * FROM items;";

        public const string Insert = @"
            INSERT INTO items (
                key, name, description, type, rarity, damage, defense, mana_bonus, strength_bonus,
                speed_bonus, hp_min, hp_max, mp, max_stack, icon_path, sprite, sound,
                is_tradable, is_droppable, is_usable, created_at, updated_at
            ) VALUES (
                @Key, @Name::jsonb, @Description::jsonb, @Type, @Rarity, @Damage, @Defense,
                @ManaBonus, @StrengthBonus, @SpeedBonus, @HpMin, @HpMax, @Mp,
                @MaxStack, @IconPath, @Sprite, @Sound,
                @IsTradable, @IsDroppable, @IsUsable, @CreatedAt, @UpdatedAt
            );";

        public const string Update = @"
            UPDATE items SET
                name = @Name::jsonb,
                description = @Description::jsonb,
                type = @Type,
                rarity = @Rarity,
                damage = @Damage,
                defense = @Defense,
                mana_bonus = @ManaBonus,
                strength_bonus = @StrengthBonus,
                speed_bonus = @SpeedBonus,
                hp_min = @HpMin,
                hp_max = @HpMax,
                mp = @Mp,
                max_stack = @MaxStack,
                icon_path = @IconPath,
                sprite = @Sprite,
                sound = @Sound,
                is_tradable = @IsTradable,
                is_droppable = @IsDroppable,
                is_usable = @IsUsable,
                updated_at = @UpdatedAt
            WHERE key = @Key;";

        public const string Upsert = @"
            INSERT INTO items (
                key, name, description, type, rarity, damage, defense, mana_bonus, strength_bonus,
                speed_bonus, hp_min, hp_max, mp, max_stack, icon_path, sprite, sound,
                is_tradable, is_droppable, is_usable, created_at, updated_at
            ) VALUES (
                @Key, @Name::jsonb, @Description::jsonb, @Type, @Rarity, @Damage, @Defense,
                @ManaBonus, @StrengthBonus, @SpeedBonus, @HpMin, @HpMax, @Mp,
                @MaxStack, @IconPath, @Sprite, @Sound,
                @IsTradable, @IsDroppable, @IsUsable, @CreatedAt, @UpdatedAt
            )
            ON CONFLICT (key) DO UPDATE SET
                name = EXCLUDED.name,
                description = EXCLUDED.description,
                type = EXCLUDED.type,
                rarity = EXCLUDED.rarity,
                damage = EXCLUDED.damage,
                defense = EXCLUDED.defense,
                mana_bonus = EXCLUDED.mana_bonus,
                strength_bonus = EXCLUDED.strength_bonus,
                speed_bonus = EXCLUDED.speed_bonus,
                hp_min = EXCLUDED.hp_min,
                hp_max = EXCLUDED.hp_max,
                mp = EXCLUDED.mp,
                max_stack = EXCLUDED.max_stack,
                icon_path = EXCLUDED.icon_path,
                sprite = EXCLUDED.sprite,
                sound = EXCLUDED.sound,
                is_tradable = EXCLUDED.is_tradable,
                is_droppable = EXCLUDED.is_droppable,
                is_usable = EXCLUDED.is_usable,
                updated_at = EXCLUDED.updated_at;";

        public const string Delete = "DELETE FROM items WHERE id = @Id;";
        public const string ExistsByKey = "SELECT EXISTS(SELECT 1 FROM items WHERE key = @Key);";
    }
}
