using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rystem.Text
{
    internal class DictionaryConverter : Converter, ICsvInterpreter
    {
        public DictionaryConverter(int index, IDictionary<string, string> abstractionInterfaceMapping, IDictionary<string, string> headerMapping) : base(index, abstractionInterfaceMapping, headerMapping) { }

        public dynamic Deserialize(Type type, string value)
        {
            IDictionary dictionary = (IDictionary)Activator.CreateInstance(type);
            Type[] keyValueType = type.GetGenericArguments();
            foreach (string val in value.Split(this.Enumerable))
            {
                string[] keyValue = val.Split(this.Dictionarable);
                dictionary.Add(
                    this.HelpToDeserialize(keyValueType[0], keyValue[0]),
                    this.HelpToDeserialize(keyValueType[1], keyValue[1])
                    );
            }
            return dictionary;
        }

        public string Serialize(object values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DictionaryEntry entry in values as IDictionary)
            {
                string key = this.HelpToSerialize(entry.Key.GetType(), entry.Key);
                string value = this.HelpToSerialize(entry.Value.GetType(), entry.Value);
                stringBuilder.Append($"{key}{(this.Dictionarable)}{value}{(this.Enumerable)}");
            }
            return stringBuilder.ToString().Trim((this.Enumerable));
        }
    }
}
