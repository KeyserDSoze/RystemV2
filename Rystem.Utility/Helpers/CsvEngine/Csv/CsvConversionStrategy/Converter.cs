using System;
using System.Collections;
using System.Collections.Generic;

namespace Rystem.Text
{
    internal abstract class Converter
    {
        private protected IDictionary<string, string> AbstractionInterfaceMapping;
        private protected IDictionary<string, string> HeaderMapping;
        private protected int Index;
        private protected char IndexAsChar;
        private protected char AbstractionInterface;
        private protected char Header;
        private protected char Enumerable;
        private protected char Dictionarable;
        private protected char ArrayLength;

        public Converter(int index, IDictionary<string, string> abstractionInterfaceMapping, IDictionary<string, string> headerMapping)
        {
            this.Index = index;
            this.IndexAsChar = (char)(ConverterConstant.Start - index);
            this.AbstractionInterface = (char)(ConverterConstant.AbstractionInterface - index);
            this.Header = (char)(ConverterConstant.HeaderInterface - index);
            this.Enumerable = (char)(ConverterConstant.Enumerable - index);
            this.Dictionarable = (char)(ConverterConstant.Dictionarable - index);
            this.ArrayLength = (char)(ConverterConstant.ArrayLength - index);
            this.AbstractionInterfaceMapping = abstractionInterfaceMapping;
            this.HeaderMapping = headerMapping;
        }
        private protected string HelpToSerialize(Type type, object value)
            => this.CreateConverter(type).Serialize(value);

        private protected dynamic HelpToDeserialize(Type type, string value)
            => this.CreateConverter(type).Deserialize(type, value);
        private ICsvInterpreter CreateConverter(Type type)
        {
            if (StringablePrimitive.CheckWithNull(type))
                return new PrimitiveConverter(this.Index + 1, this.AbstractionInterfaceMapping, this.HeaderMapping);
            else if (!(this is AbstractInterfaceConverter) && (type.IsAbstract || type.IsInterface || type.GetInterfaces().Length > 0 || type.BaseType.IsAbstract))
                return new AbstractInterfaceConverter(this.Index, this.AbstractionInterfaceMapping, this.HeaderMapping);
            else if (type.IsArray)
                return new ArrayConverter(this.Index + 1, this.AbstractionInterfaceMapping, this.HeaderMapping);
            else if (typeof(IEnumerable).IsAssignableFrom(type))
                return new EnumerableConverter(this.Index + 1, this.AbstractionInterfaceMapping, this.HeaderMapping);
            else
                return new ObjectConverter(this.Index + 1, this.AbstractionInterfaceMapping, this.HeaderMapping);
        }
    }
}
