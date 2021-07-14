using Microsoft.Extensions.DependencyInjection;
using Rystem.Azure.Installation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rystem.Business
{
    public sealed class RystemInstaller
    {
        public static AzureBuilder WithAzure(IServiceCollection serviceCollection = default)
            => (serviceCollection).WithAzure();
    }
}