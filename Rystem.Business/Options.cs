using System.Collections.Generic;

namespace Rystem.Business
{
    public sealed record Options<TManager>(Dictionary<Installation, ProvidedService> Services);
}