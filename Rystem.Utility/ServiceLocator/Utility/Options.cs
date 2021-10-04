using System.Collections.Generic;

namespace Rystem
{
    public sealed record Options<TManager>(Dictionary<Installation, ProvidedService> Services);
}