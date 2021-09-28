using System;

namespace Rystem.Text
{
    internal interface ICsvInterpreter
    {
        string Serialize(object value);
        dynamic Deserialize(Type type, string value);
    }
}
