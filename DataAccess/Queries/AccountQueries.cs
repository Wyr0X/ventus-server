namespace VentusServer.DataAccess.Queries
{
    public static class AccountQueries
    {
        public const string CreateTableQuery = @"
    CREATE TABLE IF NOT EXISTS accounts (
        account_id UUID PRIMARY KEY,
        email VARCHAR(255) NOT NULL,
        account_name VARCHAR(100) NOT NULL,
        password VARCHAR(255) NOT NULL,
        is_deleted BOOLEAN DEFAULT FALSE,
        is_banned BOOLEAN DEFAULT FALSE,
        credits INT DEFAULT 0,
        last_ip VARCHAR(45),
        last_login TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
        session_id UUID,
        created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  
        active_player_id INT,
        UNIQUE (email, account_name)
    );";

        public const string SelectById = "SELECT * FROM accounts WHERE account_id = @AccountId LIMIT 1;";
        public const string SelectByEmail = "SELECT * FROM accounts WHERE email = @Email LIMIT 1;";
        public const string SelectByName = "SELECT * FROM accounts WHERE account_name = @AccountName LIMIT 1;";

        public const string Insert = @"
            INSERT INTO accounts 
                (account_id, email, account_name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at, session_id)
            VALUES 
                (@AccountId, @Email, @AccountName, @PasswordHash, @IsDeleted, @IsBanned, @Credits, @LastIpAddress, @LastLogin, @CreatedAt, @SessionId);";

        public const string UpdatePassword = "UPDATE accounts SET password = @Password WHERE account_id = @AccountId;";
        public const string UpdateAccountName = "UPDATE accounts SET account_name = @AccountName WHERE account_id = @AccountId;";
        public const string UpdateSessionId = "UPDATE accounts SET session_id = @SessionId WHERE account_id = @AccountId;";

        public const string UpdateAccount = @"
            UPDATE accounts SET
                email = @Email,
                account_name = @AccountName,
                password = @PasswordHash,
                is_deleted = @IsDeleted,
                is_banned = @IsBanned,
                credits = @Credits,
                last_ip = @LastIpAddress,
                last_login = @LastLogin,
                session_id = @SessionId
            WHERE account_id = @AccountId;";

        public const string ExistsById = "SELECT 1 FROM accounts WHERE account_id = @AccountId LIMIT 1;";
        public const string ExistsByName = "SELECT 1 FROM accounts WHERE account_name = @AccountName LIMIT 1;";
        public const string IsEmailTaken = "SELECT 1 FROM accounts WHERE LOWER(email) = LOWER(@Email) LIMIT 1;";
        public const string IsNameTaken = "SELECT 1 FROM accounts WHERE LOWER(account_name) = LOWER(@AccountName) LIMIT 1;";
        public const string DeleteByEmail = "DELETE FROM accounts WHERE email = @Email;";
    }
}
