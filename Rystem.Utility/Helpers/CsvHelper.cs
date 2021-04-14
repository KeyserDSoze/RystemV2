using Rystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Text
{
    public static class CsvHelper
    {
        public static string ToCsv<T>(this IEnumerable<T> entities, char splittingChar = ',')
           where T : new()
           => new CsvDefaultConversion<T>(splittingChar).Write(entities);
        public static IEnumerable<T> FromCsv<T>(this string entity, char splittingChar = ',')
            where T : new()
            => new CsvDefaultConversion<T>(splittingChar).Read(entity);
        public static IEnumerable<T> FromCsv<T>(this IEnumerable<string> entries, char splittingChar = ',')
           where T : new()
           => new CsvDefaultConversion<T>(splittingChar).Read(entries);
        public static IEnumerable<T> FromCsv<T>(this byte[] entity, char splittingChar = ',')
            where T : new()
            => new CsvDefaultConversion<T>(splittingChar).Read(entity.ConvertToString());
        public static async Task<IEnumerable<T>> FromCsvAsync<T>(this Stream entity, char splittingChar = ',')
            where T : new()
            => new CsvDefaultConversion<T>(splittingChar).Read(await entity.ConvertToStringAsync().NoContext());
    }
}
