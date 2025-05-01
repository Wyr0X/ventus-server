namespace VentusServer.DataAccess.Queries
{
    public static class SpellQueries
    {
        public const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS spells (
                id TEXT PRIMARY KEY,
                name TEXT NOT NULL,
                price INT DEFAULT 0,
                description TEXT,
                mana_cost INT DEFAULT 0,
                cooldown REAL DEFAULT 0,
                cast_time REAL DEFAULT 0,
                range REAL DEFAULT 0,
                school TEXT,
                effects JSONB DEFAULT '[]',
                tags TEXT[],
                required_level INT DEFAULT 1,
                required_class TEXT,
                icon TEXT,
                animation TEXT,
                cast_mode TEXT,  -- 'cast_mode' como TEXT
                target_type TEXT,  -- 'target_type' como TEXT
                area JSONB,
                can_crit BOOLEAN DEFAULT FALSE,
                is_reflectable BOOLEAN DEFAULT FALSE,
                requires_line_of_sight BOOLEAN DEFAULT TRUE,
                interruptible BOOLEAN DEFAULT TRUE,
                created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );";

        public const string Insert = @"
            INSERT INTO spells (
                id, name, price, description, mana_cost, cooldown, cast_time, range,
                school, effects, tags, required_level, required_class,
                icon, animation, cast_mode, target_type, area,
                can_crit, is_reflectable, requires_line_of_sight, interruptible,
                created_at, updated_at
            ) VALUES (
                @Id, @Name, @Price, @Description, @ManaCost, @Cooldown, @CastTime, @Range,
                @School, @Effects::jsonb, @Tags, @RequiredLevel, @RequiredClass,
                @Icon, @Animation, @CastMode, @TargetType, @Area::jsonb,
                @CanCrit, @IsReflectable, @RequiresLineOfSight, @Interruptible,
                @CreatedAt, @UpdatedAt
            )
            RETURNING id;";

        public const string Update = @"
            UPDATE spells SET
                id = @Id,
                name = @Name,
                price = @Price,
                description = @Description,
                mana_cost = @ManaCost,
                cooldown = @Cooldown,
                cast_time = @CastTime,
                range = @Range,
                school = @School,
                effects = @Effects::jsonb,
                tags = @Tags,
                required_level = @RequiredLevel,
                required_class = @RequiredClass,
                icon = @Icon,
                animation = @Animation,
                cast_mode = @CastMode,
                target_type = @TargetType,
                area = @Area::jsonb,
                can_crit = @CanCrit,
                is_reflectable = @IsReflectable,
                requires_line_of_sight = @RequiresLineOfSight,
                interruptible = @Interruptible,
                updated_at = @UpdatedAt
            WHERE id = @Id;";

        public const string SelectById = @"
            SELECT * FROM spells
            WHERE id = @Id
            LIMIT 1;";

        public const string SelectAll = @"
            SELECT * FROM spells;";

        public const string DeleteById = @"
            DELETE FROM spells
            WHERE id = @Id;";

        public const string ExistsById = @"
            SELECT EXISTS (
                SELECT 1 FROM spells WHERE id = @Id
            );";
    }
}
