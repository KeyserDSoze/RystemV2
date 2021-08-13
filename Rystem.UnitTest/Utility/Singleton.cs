using Rystem.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class Singleton
    {
        [Fact]
        public async Task TryToSaveInMemoryAndRetrieveInformation()
        {
            var value = new Key().Instance<ToSingleton>();
            Assert.Null(value);
            new Key().Update(new ToSingleton() { UserId = 3, Id = 4, Title = "dasdsad", Completed = true }, TimeSpan.FromSeconds(2));
            value = new Key().Instance<ToSingleton>();
            Assert.NotNull(value);
            Assert.Equal(3, value.UserId);
            await Task.Delay(2000).NoContext();
            value = new Key().Instance<ToSingleton>();
            Assert.Null(value);
            new Key("aa").Update(new ToSingleton() { UserId = 3, Id = 4, Title = "dasdsad", Completed = true }, TimeSpan.FromSeconds(2));
            value = new Key("aa").Instance<ToSingleton>();
            Assert.NotNull(value);
            Assert.Equal(3, value.UserId);
            value = new Key("aa").Remove<ToSingleton>();
            Assert.NotNull(value);
            Assert.Equal(3, value.UserId);
            value = new Key("aa").Instance<ToSingleton>();
            Assert.Null(value);
        }

        public class ToSingleton
        {
            public int UserId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Completed { get; set; }
        }
    }
}
