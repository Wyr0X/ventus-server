using VentusServer.Domain.Enums;

public class RoleModel
{
    public string RoleId { get; set; } = "";
    public string DisplayName { get; set; } = default!; // Nombre visible (ej: "Moderador")
    public List<Permission> Permissions { get; set; } = new(); // Relación M:M

    // ¿Este rol puede ser editado o no?
    public bool IsEditable { get; set; } = true;
}
