using Rystem.Conversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rystem.Conversion
{
    internal static class CsvConvertExtensions
    {
        public static string ToObjectCsv<T>(this T data)
           => new StartConversion().Serialize(data);
        public static T FromObjectCsv<T>(this string value)
           => (T)new StartConversion().Deserialize(typeof(T), value);
    }
}
