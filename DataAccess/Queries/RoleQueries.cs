namespace VentusServer.DataAccess.Queries
{
    public static class RoleQueries
    {
        public const string CreateRolesTable = @"
            CREATE TABLE IF NOT EXISTS roles (
                role_id UUID PRIMARY KEY,
                name VARCHAR(50) NOT NULL UNIQUE,
                display_name VARCHAR(100) NOT NULL,
                is_editable BOOLEAN DEFAULT TRUE,
                permissions TEXT[] DEFAULT ARRAY[]::TEXT[]
            );";

        public const string SelectRoleById = @"
            SELECT * FROM roles
            WHERE role_id = @RoleId
            LIMIT 1;";

        public const string SelectRoleByName = @"
            SELECT * FROM roles
            WHERE name = @Name
            LIMIT 1;";

        public const string InsertRole = @"
            INSERT INTO roles (role_id, name, display_name, is_editable, permissions)
            VALUES (@RoleId, @Name, @DisplayName, @IsEditable, @Permissions);";

        public const string UpdateRole = @"
            UPDATE roles
            SET name = @Name,
                display_name = @DisplayName,
                is_editable = @IsEditable,
                permissions = @Permissions
            WHERE role_id = @RoleId;";

        public const string DeleteRole = @"
            DELETE FROM roles
            WHERE role_id = @RoleId;";

        public const string SelectAllRoles = @"
            SELECT * FROM roles;";
    }
}
