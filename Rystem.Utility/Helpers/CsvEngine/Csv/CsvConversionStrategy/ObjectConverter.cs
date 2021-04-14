using Rystem.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rystem.Text
{
    internal class ObjectConverter : Converter, ICsvInterpreter
    {
        public ObjectConverter(int index, IDictionary<string, string> abstractionInterfaceMapping, IDictionary<string, string> headerMapping) : base(index, abstractionInterfaceMapping, headerMapping) { }
        private static readonly Type CsvIgnoreAttribute = typeof(CsvIgnore);
        private static readonly Type CsvPropertyAttribute = typeof(CsvProperty);
        public string Serialize(object value)
        {
            if (value == null)
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PropertyInfo property in value.GetType().FetchProperties(CsvIgnoreAttribute))
                stringBuilder.Append($"{SetHeader(property.GetCustomAttribute(CsvPropertyAttribute) is CsvProperty csvProperty ? csvProperty.Name : property.Name)}{this.Header}{this.HelpToSerialize(property.PropertyType, property.GetValue(value))}{this.IndexAsChar}");
            return stringBuilder.ToString().Trim(this.IndexAsChar);

            string SetHeader(string propertyName)
            {
                if (!this.HeaderMapping.ContainsKey(propertyName))
                    this.HeaderMapping.Add(propertyName, this.HeaderMapping.Count.ToString());
                return this.HeaderMapping[propertyName];
            }
        }

        public dynamic Deserialize(Type type, string value)
        {
            if (value == null)
                return default;
            dynamic startValue = Activator.CreateInstance(type);
            string[] values = value.Split(this.IndexAsChar);
            PropertyInfo[] propertyInfo = type.FetchProperties(CsvIgnoreAttribute);
            foreach (string v in values)
            {
                string[] propertyAsString = v.Split(this.Header);
                if (this.HeaderMapping.ContainsKey(propertyAsString[0]))
                {
                    PropertyInfo property = propertyInfo.FirstOrDefault(x => (x.GetCustomAttribute(CsvPropertyAttribute) != null ? (x.GetCustomAttribute(CsvPropertyAttribute) as CsvProperty).Name : x.Name) == this.HeaderMapping[propertyAsString[0]]);
                    if (property != null)
                        property.SetValue(startValue, this.HelpToDeserialize(property.PropertyType, propertyAsString[1]));
                }
            }
            return startValue;
        }
    }
}
