using Rystem.Azure.Integration;
using Rystem.Business;
using Rystem.Business.Document;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.Data
{
    public class Sample : IData
    {
        public string Name { get; init; }
        public RystemDataServiceProvider ConfigureData()
        {
            return RystemDataServiceProvider.WithAzure()
                .WithBlockBlob()
                .AndWithAzure(Installation.Inst00)
                .WithAppendBlob();
        }
        public static async Task Run(Installation installation)
        {
            bool check = installation == Installation.Inst00;
            Sample sample = new() { Name = "XXX" };
            MiniSample mini = new() { Ale3 = "dsadsadsa", X = 4444 };
            await sample.WriteAsync(mini, check, installation: installation).NoContext();
            await sample.WriteAsync(mini, check, installation: installation).NoContext();
            await sample.WriteAsync(mini, check, installation: installation).NoContext();
            if (check)
            {
                var reading = await sample.ReadAsync<MiniSample>(check, installation).NoContext();
                Assert.Equal(3, reading.Count());
            }
            else
            {
                MiniSample x = await sample.ReadAsync<MiniSample>(installation).NoContext();
                Assert.Equal("dsadsadsa", x.Ale3);
            }
            Assert.True(await sample.ExistsAsync(installation).NoContext());
            Assert.True(await sample.DeleteAsync(installation).NoContext());
            Assert.False(await sample.DeleteAsync(installation).NoContext());
            Assert.False(await sample.ExistsAsync(installation).NoContext());
        }
    }
    public class MiniSample
    {
        public string Ale3 { get; set; }
        public double X { get; set; }
    }
}