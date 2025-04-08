using System;
using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;
using VentusServer.DataAccess.Entities;
using VentusServer.Domain.Enums;

namespace VentusServer.DataAccess.Mappers
{
    public class RoleMapper : BaseMapper
    {
        // 🧭 De entidad (DB) a modelo (dominio)
        public static RoleModel FromRow(dynamic row)
        {
            // Convertimos el array de texto (permissions) a una lista de enums válidos
            List<Permission> permissions = new();

            if (row.permissions is string[] perms)
            {
                foreach (var p in perms)
                {
                    if (Enum.TryParse<Permission>(p, out var perm))
                        permissions.Add(perm);
                }
            }

            return new RoleModel
            {
                RoleId = row.role_id,
                Name = row.name,
                DisplayName = row.display_name,
                IsEditable = row.is_editable,
                Permissions = permissions
            };
        }
        public static RoleModel ToModel(DbRoleEntity entity)
        {
            return new RoleModel
            {
                RoleId = entity.Id,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                IsEditable = entity.IsEditable,
                Permissions = entity.Permissions?
                    .Select(p => Enum.TryParse<Permission>(p, out var perm) ? perm : default)
                    .Where(p => p != default) // Filtra valores inválidos
                    .ToList() ?? new()
            };
        }

        // 🧭 De modelo (dominio) a entidad (DB)
        public static DbRoleEntity ToEntity(RoleModel model)
        {
            return new DbRoleEntity
            {
                Id = model.RoleId,
                Name = model.Name,
                DisplayName = model.DisplayName,
                IsEditable = model.IsEditable,
                Permissions = model.Permissions?
                    .Select(p => p.ToString())
                    .ToList() ?? new()
            };
        }

        // 🧭 Lista de entidades a modelos
        public static List<RoleModel> ToModels(IEnumerable<DbRoleEntity> entities) =>
            entities.Select(ToModel).ToList();

        // 🧭 Lista de modelos a entidades
        public static List<DbRoleEntity> ToEntities(IEnumerable<RoleModel> models) =>
            models.Select(ToEntity).ToList();
        // 🧭 De fila (row) a entidad (DbRoleEntity)
        public static DbRoleEntity ToEntityFromRow(dynamic row)
        {
            List<string> permissions = new();

            if (row.permissions is string[] perms)
            {
                permissions = perms.ToList();
            }

            return new DbRoleEntity
            {
                Id = row.role_id,
                Name = row.name,
                DisplayName = row.display_name,
                IsEditable = row.is_editable,
                Permissions = permissions
            };
        }
        public static List<DbRoleEntity> ToEntitiesFromRows(IEnumerable<dynamic> rows)
{
    return rows.Select(ToEntityFromRow).ToList();
}


    }
}
