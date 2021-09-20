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
    public class Sample
    {
        public static async Task Run(Installation installation)
        {
            bool check = installation == Installation.Inst00;
            MiniContainer mini = new() { Cads = "XXX", Samples = new List<MiniSample>() { new MiniSample() { Ale3 = "dsadsadsa", X = 4444 } } };
            await mini.WriteAsync(installation: installation).NoContext();
            await mini.WriteAsync(installation: installation).NoContext();
            await mini.WriteAsync(installation: installation).NoContext();
            if (check)
            {
                await foreach (var item in mini.ListAsync(installation: installation))
                {
                    Assert.Equal("dsadsadsa", item.Content.First().Samples.First().Ale3);
                    Assert.Equal(3, item.Content.Count);
                }
            }
            else
            {
                await foreach (var item in mini.ListAsync(10, installation))
                {
                    Assert.Equal("dsadsadsa", item.Content.First().Samples.First().Ale3);
                }
            }
            if (check)
            {
                var counter = 0;
                await foreach (var t in mini.ListAsync(installation: installation))
                {
                    counter++;
                }
                Assert.Equal(3, counter);
            }
            else
            {
                MiniContainer x = await mini.ReadAsync(installation).NoContext();
                Assert.Equal("dsadsadsa", x.Samples.First().Ale3);
            }
            Assert.True(await mini.ExistsAsync(installation).NoContext());
            Assert.True(await mini.DeleteAsync(installation).NoContext());
            Assert.False(await mini.DeleteAsync(installation).NoContext());
            Assert.False(await mini.ExistsAsync(installation).NoContext());
            if (installation != Installation.Inst00)
            {
                MiniContainer miniContainer = new() { Cads = "XXX" };
                for (int i = 0; i < 1_000_000; i++)
                    miniContainer.Samples.Add(new MiniSample() { Ale3 = "ddd", X = 3 });
                await miniContainer.WriteAsync(installation).NoContext();
                var minis = await miniContainer.ReadAsync(installation);
                await miniContainer.DeleteAsync().NoContext();
                Assert.Equal(1_000_000, minis.Samples.Count);
            }
        }
    }
    public class MiniContainer : IData
    {
        public string Cads { get; set; }
        public List<MiniSample> Samples { get; set; } = new List<MiniSample>();
    }
    public class MiniSample
    {
        public string Colos { get; set; }
        public string Ale3 { get; set; }
        public double X { get; set; }
    }
}