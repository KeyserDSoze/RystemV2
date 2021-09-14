using System.Collections.Generic;

namespace Rystem.Cache
{

    public sealed record HttpResponseCache(int StatusCode, Dictionary<string, string> Headers, byte[] Body);
}