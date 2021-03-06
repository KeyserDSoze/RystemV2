using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Rystem.Azure;
using Rystem.Cache;
using Rystem.Test.WebApi.Models;

namespace Rystem.Test.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var storage = Configuration.GetSection("Storage");
            services.AddControllers();
            services
                .AddRystem()
                .AddAzureService()
                .AddStorage(new Azure.Integration.Storage.StorageAccount(storage["Name"], storage["Key"]))
                .EndConfiguration();
            services.AddSingleton<FirstSingleton>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rystem.Test.WebApi", Version = "v1" });
            });
            services.AddCache(x =>
            {
                x.AddPath(new StartingStringPathFinder(CachedHttpMethod.Get, "W"));
            })
                .AddAzureCache()
                .WithAzure()
                    .WithBlobStorage(new Azure.Integration.Storage.BlobStorageConfiguration("MyOwnCache"))
                    .Configure();
            //new DailyImport()
            //{
            //    Options = new Background.BackgroundWorkOptions()
            //    {
            //        Cron = "* * * * * *",
            //        Key = "da",
            //        RunImmediately = false
            //    }
            //}.Run();
            //services.AddBackgroundWork<MonthlyReviewImport>(x =>
            //{
            //    x.Cron = "* * * * *";
            //    x.Key = "da";
            //    x.RunImmediately = false;
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRystem();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rystem.Test.WebApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseCache();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
