using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rystem.Azure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem
{
    public static class RystemBusiness
    {
        public static AzureBuilder WithAzure()
            => new ServiceCollection().AddAzureService();
        public static IHost FinalizeWithoutDependencyInjection(this IServiceCollection services)
            => new RystemHost(services)
                .WithRystem();
        private sealed class RystemHost : IHost
        {
            public IServiceProvider Services { get; }
            public RystemHost(IServiceCollection services)
                => Services = services.BuildServiceProvider();
            public void Dispose()
            {
                throw new NotImplementedException();
            }
            public Task StartAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
            public Task StopAsync(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}