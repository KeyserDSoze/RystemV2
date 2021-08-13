using System.Collections.Generic;

namespace Rystem.Memory
{

    internal record HttpResponseCache(int StatusCode, Dictionary<string, string> Headers, byte[] Body);
}