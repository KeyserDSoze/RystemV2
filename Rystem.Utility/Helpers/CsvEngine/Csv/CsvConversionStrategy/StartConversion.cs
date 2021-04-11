using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rystem.Conversion
{
    internal class StartConversion : Converter, ICsvInterpreter
    {
        private static readonly Dictionary<string, string> Empty = new Dictionary<string, string>();
        public StartConversion() : base(0, new Dictionary<string, string>(), new Dictionary<string, string>())
        {
        }
        public dynamic Deserialize(Type type, string value)
        {
            string[] values = value.Split(ConverterConstant.CsvPacket);
            this.HeaderMapping = (new DictionaryConverter(this.Index, Empty, Empty).Deserialize(typeof(Dictionary<string, string>), values[0]) as IDictionary<string, string>).ToDictionary(x => x.Value, x => x.Key);
            if (values.Length > 2)
                this.AbstractionInterfaceMapping = (new DictionaryConverter(this.Index, Empty, Empty).Deserialize(typeof(Dictionary<string, string>), values[2]) as IDictionary<string, string>).ToDictionary(x => x.Value, x => x.Key);
            return this.HelpToDeserialize(type, values[1]);
        }
        public string Serialize(object value)
        {
            string returnValue = this.HelpToSerialize(value.GetType(), value);
            return $"{new DictionaryConverter(0, Empty, Empty).Serialize(this.HeaderMapping)}{ConverterConstant.CsvPacket}{returnValue}{(this.AbstractionInterfaceMapping.Count > 0 ? $"{ConverterConstant.CsvPacket}{new DictionaryConverter(0, Empty, Empty).Serialize(this.AbstractionInterfaceMapping)}" : string.Empty)}";
        }
    }
}
