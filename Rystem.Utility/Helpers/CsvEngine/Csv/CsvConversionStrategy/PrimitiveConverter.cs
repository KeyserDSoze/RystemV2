using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Rystem.Text
{
    internal class PrimitiveConverter : Converter, ICsvInterpreter
    {
        public PrimitiveConverter(int index, IDictionary<string, string> abstractionInterfaceMapping, IDictionary<string, string> headerMapping) : base(index, abstractionInterfaceMapping, headerMapping) { }

        public dynamic Deserialize(Type type, string value)
        {
            if (value == null)
                return default;
            if (type.BaseType != typeof(Enum))
            {
                return (!string.IsNullOrWhiteSpace(value) ?
                    (!type.IsGenericType ?
                        Convert.ChangeType(value, type, CultureInfo.InvariantCulture) :
                        Convert.ChangeType(value, type.GenericTypeArguments[0], CultureInfo.InvariantCulture)
                    )
                    : default);
            }
            else
            {
                return Enum.Parse(type, value);
            }
        }

        public string Serialize(object value)
        {
            return value.ToString();
        }
    }
}
