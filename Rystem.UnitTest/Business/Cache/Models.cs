using Rystem.Business;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest.Business.Cache
{
    public class SampleKey : ICacheKey<Sample>
    {
        public int Id { get; init; }
        public Task<Sample> FetchAsync()
            => Task.FromResult(Sample.CreateNewSample(Id));
    }

    public class Sample
    {
        public string Ale { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public string Ale3 { get; set; }
        public MiniSample Mini { get; set; }
        public DateTime Timestamp { get; set; }

        public static async Task Run(Installation installation)
        {
            var key = CreateKey(1);
            var value = await key.InstanceAsync(default, installation).NoContext();
            Assert.NotNull(value);
            await Task.Delay(2000);
            value = await key.InstanceAsync(default, installation).NoContext();
            Assert.NotNull(value);
            await key.RestoreAsync(value, installation: installation).NoContext();
            await key.RemoveAsync(installation).NoContext();
            Assert.False(await key.IsPresentAsync(installation).NoContext());
            await key.RestoreAsync().NoContext();
            Assert.True(await key.IsPresentAsync(installation).NoContext());
            await key.RemoveAsync(installation).NoContext();
            Assert.False(await key.IsPresentAsync(installation).NoContext());
        }
        private static SampleKey CreateKey(int x)
            => new SampleKey { Id = x };
        internal static Sample CreateNewSample(int x)
            => new() { Ale = x.ToString(), PrimaryKey = x.ToString(), SecondaryKey = null, Ale3 = x.ToString(), Timestamp = DateTime.UtcNow, Mini = new MiniSample { X = x, Ale3 = x.ToString() } };
    }
    public class MiniSample
    {
        public string Ale3 { get; set; }
        public double X { get; set; }
    }
}