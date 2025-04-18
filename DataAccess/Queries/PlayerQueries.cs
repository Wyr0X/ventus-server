namespace VentusServer.DataAccess.Queries
{
    public static class PlayerQueries
    {

        public const string CreateTableQuery = @"
    CREATE TABLE IF NOT EXISTS players (
        id SERIAL PRIMARY KEY,
        account_id UUID NOT NULL REFERENCES accounts(account_id) ON DELETE CASCADE,
        name VARCHAR(100) NOT NULL UNIQUE,
        gender INT NOT NULL,
        race INT NOT NULL,
        level INT DEFAULT 1,
        class INT NOT NULL,
        created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
        last_login TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
        status VARCHAR(50) DEFAULT 'active'
    );";

        public const string SelectById = "SELECT * FROM players WHERE id = @PlayerId LIMIT 1;";
        public const string SelectByName = "SELECT * FROM players WHERE name = @Name LIMIT 1;";
        public const string SelectAll = "SELECT * FROM players;";
        public const string SelectByAccountId = "SELECT * FROM players WHERE account_id = @AccountId;";

        public const string Insert = @"
            INSERT INTO players (account_id, name, gender, race, level, class, created_at, last_login, status)
            VALUES (@AccountId, @Name, @Gender, @Race, @Level, @Class, @CreatedAt, @LastLogin, @Status)
            RETURNING id;";

        public const string Upsert = @"
            INSERT INTO players (id, account_id, name, gender, race, level, class, created_at, last_login, status)
            VALUES (@Id, @AccountId, @Name, @Gender, @Race, @Level, @Class, @CreatedAt, @LastLogin, @Status)
            ON CONFLICT (id) DO UPDATE SET
                account_id = EXCLUDED.account_id,
                name = EXCLUDED.name,
                gender = EXCLUDED.gender,
                race = EXCLUDED.race,
                level = EXCLUDED.level,
                class = EXCLUDED.class,
                last_login = EXCLUDED.last_login,
                status = EXCLUDED.status;";

        public const string DeleteById = "DELETE FROM players WHERE id = @PlayerId;";
        public const string ExistsById = "SELECT EXISTS(SELECT 1 FROM players WHERE id = @PlayerId);";
        public const string ExistsByName = "SELECT EXISTS(SELECT 1 FROM players WHERE name = @Name);";
    }
}
