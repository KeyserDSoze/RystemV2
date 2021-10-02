using System.Collections.Generic;

namespace System
{
    public sealed record TryResponse<T, TException>(T Result, List<TException> Exceptions, bool Executed, int DistinctExceptions)
        where TException : Exception;
}