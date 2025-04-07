using System;
using System.Collections.Generic;
using System.Linq;

namespace VentusServer.DataAccess.Mappers
{
    public class BaseMapper
    {
        // 🧭 Mapeo de filas dinámicas a entidades
        public static List<TModel> MapRowsToEntities<TModel>(IEnumerable<dynamic> rows, Func<dynamic, TModel> mapFunc)
        {
            return rows.Select(mapFunc).ToList();
        }

        // 🧭 Imprimir detalles de una fila dinámica
        public static void PrintRow(dynamic row)
        {
            // Convertir el dynamic a un diccionario para acceder a las claves y valores
            var dictionary = (IDictionary<string, object>)row;

            // Imprimir todos los pares clave-valor
            Console.WriteLine("Row Data:");
            foreach (var kvp in dictionary)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }
    }
}
