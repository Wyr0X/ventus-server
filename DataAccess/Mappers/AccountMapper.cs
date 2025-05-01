using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class AccountMapper : BaseMapper
    {
        public static AccountModel Map(dynamic row)
        {
            return new AccountModel
            {
                AccountId = row.account_id,
                Email = row.email,
                AccountName = row.account_name,
                PasswordHash = row.password,
                IsDeleted = row.is_deleted,
                IsBanned = row.is_banned,
                Credits = row.credits,
                LastIpAddress = row.last_ip,
                LastLogin = row.last_login,
                SessionId = row.session_id,
                CreatedAt = row.created_at,
                ActivePlayerId = null,
                RoleId = row.role_id
            };
        }

        public static List<AccountModel> MapRowsToAccounts(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }

        // 👇 Conversión de modelo de juego a entidad de persistencia
        public static DbAccountEntity ToEntity(AccountModel model)
        {
            return new DbAccountEntity
            {
                AccountId = model.AccountId,
                Email = model.Email,
                AccountName = model.AccountName,
                PasswordHash = model.PasswordHash,
                IsDeleted = model.IsDeleted,
                IsBanned = model.IsBanned,
                Credits = model.Credits,
                LastIpAddress = model.LastIpAddress,
                LastLogin = model.LastLogin,
                SessionId = model.SessionId,
                CreatedAt = model.CreatedAt,
                RoleId = model.RoleId

            };
        }

        // 👇 Conversión inversa
        public static AccountModel ToModel(DbAccountEntity entity)
        {
            return new AccountModel
            {
                AccountId = entity.AccountId,
                Email = entity.Email,
                AccountName = entity.AccountName,
                PasswordHash = entity.PasswordHash,
                IsDeleted = entity.IsDeleted,
                IsBanned = entity.IsBanned,
                Credits = entity.Credits,
                LastIpAddress = entity.LastIpAddress ?? "",
                LastLogin = entity.LastLogin,
                SessionId = entity.SessionId,
                CreatedAt = entity.CreatedAt,
                ActivePlayerId = null,
                RoleId = entity.RoleId
            };
        }
    }
}
