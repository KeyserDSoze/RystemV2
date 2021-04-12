using System;
using System.Collections.Generic;

namespace Rystem.Conversion
{
    internal class AbstractInterfaceConverter : Converter, ICsvInterpreter
    {
        public AbstractInterfaceConverter(int index, IDictionary<string, string> abstractionInterfaceMapping, IDictionary<string, string> headerMapping) : base(index, abstractionInterfaceMapping, headerMapping) { }
        public dynamic Deserialize(Type type, string value)
        {
            string[] values = value.Split(this.AbstractionInterface);
            return this.HelpToDeserialize(this.GetAbstractionInterfaceImplementation(values[0]), values[1]);
        }

        public string Serialize(object value)
            => $"{this.SetAbstractionInterface(value.GetType())}{(this.AbstractionInterface)}{this.HelpToSerialize(value.GetType(), value)}";
        private string SetAbstractionInterface(Type type)
        {
            string fullname = type.AssemblyQualifiedName;
            if (!this.AbstractionInterfaceMapping.ContainsKey(fullname))
                this.AbstractionInterfaceMapping.Add(fullname, this.AbstractionInterfaceMapping.Count.ToString());
            return this.AbstractionInterfaceMapping[fullname];
        }
        private Type GetAbstractionInterfaceImplementation(string value)
        {
            Type type = Type.GetType(this.AbstractionInterfaceMapping[value], false, true);
            return type;
        }
    }
}
