using System;
using System.Collections.Generic;
using System.Text;

namespace Rystem.Text
{
    internal class ArrayConverter : Converter, ICsvInterpreter
    {
        public ArrayConverter(int index, IDictionary<string, string> abstractionInterfaceMapping, IDictionary<string, string> headerMapping) : base(index, abstractionInterfaceMapping, headerMapping) { }

        public dynamic Deserialize(Type type, string value)
        {
            Type elementType = type.GetElementType();
            string[] splitted = value.Split(this.ArrayLength);
            Array array = Array.CreateInstance(elementType, int.Parse(splitted[0]));
            string[] values = splitted[1].Split(this.Enumerable);
            for (int i = 0; i < array.Length; i++)
            {
                if (i >= values.Length)
                    break;
                array.SetValue(this.HelpToDeserialize(elementType, values[i]), i);
            }
            return array;
        }

        public string Serialize(object value)
            => $"{(value as Array).Length}{this.ArrayLength}{new EnumerableConverter(this.Index, this.AbstractionInterfaceMapping, this.HeaderMapping).Serialize(value)}";
    }
}
