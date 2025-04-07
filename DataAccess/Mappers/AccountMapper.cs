using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;

namespace VentusServer.DataAccess.Mappers
{
    public static class AccountMapper
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
                ActivePlayerId = row.active_player_id,
                CreatedAt = row.created_at
            };
        }

        public static AccountModel MapRowToAccount(dynamic row)
        {
            return Map(row);
        }

        public static List<AccountModel> MapRowsToAccounts(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }
    }
}
