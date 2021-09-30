using System.Collections.Generic;

namespace System
{
    public sealed record TryResponse<T>(T Result, List<Exception> Exceptions, bool Executed);
}