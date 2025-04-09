namespace VentusServer.DataAccess.Queries
{
    public static class RoleQueries
    {
        public const string CreateTableQuery = @"
        CREATE TABLE IF NOT EXISTS roles (
            role_id VARCHAR(100) PRIMARY KEY,
            display_name VARCHAR(100) NOT NULL,
            is_editable BOOLEAN DEFAULT TRUE,
            permissions TEXT[] DEFAULT ARRAY[]::TEXT[]
        );";

        public const string SelectRoleById = @"
        SELECT * FROM roles
        WHERE role_id = @RoleId
        LIMIT 1;";

        public const string InsertRole = @"
        INSERT INTO roles (role_id, display_name, is_editable, permissions)
        VALUES (@RoleId, @DisplayName, @IsEditable, @Permissions);";

        public const string UpdateRole = @"
        UPDATE roles
        SET display_name = @DisplayName,
            is_editable = @IsEditable,
            permissions = @Permissions
        WHERE role_id = @RoleId;";

        public const string DeleteRole = @"
        DELETE FROM roles
        WHERE role_id = @RoleId;";

        public const string SelectAllRoles = @"
        SELECT * FROM roles;";
        public const string SelectRoleByDisplayName = @"
            SELECT * FROM roles
            WHERE display_name = @DisplayName
            LIMIT 1;";

    }

}
