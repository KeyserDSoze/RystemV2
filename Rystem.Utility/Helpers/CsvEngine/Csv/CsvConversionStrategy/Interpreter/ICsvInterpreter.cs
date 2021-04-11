using System;
using System.Collections.Generic;
using System.Text;

namespace Rystem.Conversion
{
    internal interface ICsvInterpreter
    {
        string Serialize(object value);
        dynamic Deserialize(Type type, string value);
    }
}
