using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rystem.Text
{
    internal static class StringablePrimitive
    {
        public static bool Check(Type type)
        {
            foreach (Type typeR in NormalTypes)
                if (typeR == type)
                    return true;
            return false;
        }
        public static bool CheckWithNull(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GenericTypeArguments.Length > 0)
                type = type.GenericTypeArguments.FirstOrDefault();
            return Check(type) || type.BaseType == typeof(Enum) || type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }
        private static readonly List<Type> NormalTypes = new List<Type>
        {
            typeof(int),
            typeof(bool),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(long),
            typeof(byte),
            typeof(sbyte),
            typeof(float),
            typeof(uint),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(string),
            typeof(int?),
            typeof(bool?),
            typeof(char?),
            typeof(decimal?),
            typeof(double?),
            typeof(long?),
            typeof(byte?),
            typeof(sbyte?),
            typeof(float?),
            typeof(uint?),
            typeof(ulong?),
            typeof(short?),
            typeof(ushort?),
            typeof(Guid),
            typeof(Guid?)
        };
    }
}