using VentusServer.Domain.Enums;

public interface IPermissionService
{
    Task<bool> HasPermission(AccountModel account, Permission permission);
}
