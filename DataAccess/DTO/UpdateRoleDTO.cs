public class UpdateRoleDTO
{
    public required string RoleId { get; set; }

    public required string DisplayName { get; set; }
    public List<string> Permissions { get; set; } = new();
}
