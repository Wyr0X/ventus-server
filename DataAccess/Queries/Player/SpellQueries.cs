namespace VentusServer.DataAccess.Queries
{
    public static class SpellQueries
    {
        public const string CreateTableQuery = @"
        CREATE TABLE IF NOT EXISTS spells (
            id TEXT PRIMARY KEY,
            name TEXT NOT NULL,
            mana_cost INT NOT NULL,
            cast_time INT NOT NULL,
            cooldown INT NOT NULL,
            range INT NOT NULL,
            price INT DEFAULT 0,
            is_channeled BOOLEAN DEFAULT FALSE,
            duration INT DEFAULT 0,
            targeting JSONB,
            unit_effects JSONB DEFAULT '[]',
            terrain_effects JSONB DEFAULT '[]',
            summon_effects JSONB DEFAULT '[]',
            target_type TEXT,
            required_class TEXT,
            required_level INT DEFAULT 0,
            requires_line_of_sight BOOLEAN DEFAULT FALSE,
            description TEXT,
            cast_sound TEXT,
            impact_sound TEXT,
            vfx_cast TEXT,
            vfx_impact TEXT,
            cast_mode TEXT,
            created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
            updated_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
        );";

        public const string Insert = @"
            INSERT INTO spells (
                id, name, mana_cost, cast_time, cooldown, range, price,
                is_channeled, duration, targeting,
                unit_effects, terrain_effects, summon_effects,
                target_type, required_class, required_level,
                requires_line_of_sight, description,
                cast_sound, impact_sound, vfx_cast, vfx_impact,
                cast_mode, created_at, updated_at
            ) VALUES (
                @Id, @Name, @ManaCost, @CastTime, @Cooldown, @Range, @Price,
                @IsChanneled, @Duration, @Targeting::jsonb,
                @UnitEffects::jsonb, @TerrainEffects::jsonb, @SummonEffects::jsonb,
                @TargetType, @RequiredClass, @RequiredLevel,
                @RequiresLineOfSight, @Description,
                @CastSound, @ImpactSound, @VfxCast, @VfxImpact,
                @CastMode, @CreatedAt, @UpdatedAt
            )
            RETURNING id;";

        public const string Update = @"
            UPDATE spells SET
                name = @Name,
                mana_cost = @ManaCost,
                cast_time = @CastTime,
                cooldown = @Cooldown,
                range = @Range,
                price = @Price,
                is_channeled = @IsChanneled,
                duration = @Duration,
                targeting = @Targeting::jsonb,
                unit_effects = @UnitEffects::jsonb,
                terrain_effects = @TerrainEffects::jsonb,
                summon_effects = @SummonEffects::jsonb,
                target_type = @TargetType,
                required_class = @RequiredClass,
                required_level = @RequiredLevel,
                requires_line_of_sight = @RequiresLineOfSight,
                description = @Description,
                cast_sound = @CastSound,
                impact_sound = @ImpactSound,
                vfx_cast = @VfxCast,
                vfx_impact = @VfxImpact,
                cast_mode = @CastMode,
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
