using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Polly;

namespace Rystem.UnitTest
{
    public class Instanciation
    {
        static Instanciation()
        {
            ServiceLocator
                .Create()
                .AddSingleton<IFirst, First>()
                .FinalizeWithoutDependencyInjection();
        }
        public interface IFirst
        {
            string A { get; }
        }
        public class First : IFirst
        {
            public string A { get; } = "A";
        }
        public class Second
        {
            public IFirst First { get; }
            public Second(IFirst first)
            {
                First = first;
            }
        }
        public class Third
        {
            public IFirst First { get; }
            public string A { get; }
            public Third(IFirst first, string a)
            {
                First = first;
                A = a;
            }
        }
        private class TestMyInstance
        {
            public string A { get; }
            public string B { get; }
            public TestMyInstance(string a, string b)
            {
                A = a;
                B = b;
            }
            public TestMyInstance(string a, string b, string c)
            {
                A = a;
                B = b;
            }
            public TestMyInstance(string a)
            {
                A = a;
            }
        }
        [Fact]
        public async Task Instance()
        {
            var duringSingletoning = ExecutedInMilliseconds(() => new TestMyInstance("", ""), () => new Generic<TestMyInstance>("a", "b"));
            var x = new Generic<TestMyInstance>("a", "b").Value;
            Assert.Equal("a", x.A);
            Assert.Equal("b", x.B);
            var afterSingletoned = ExecutedInMilliseconds(() => new TestMyInstance("", ""), () => new Generic<TestMyInstance>("a", "b"));
            Assert.True(afterSingletoned < duringSingletoning / 10);
            var t = Generic<Second>.BuildWithDependencyInjection();
            Assert.Equal("A", t.First.A);
            var q = await Generic<Third>.BuildSmartAsync(() => Task.FromResult(new object[1] { "A" })).NoContext();
            Assert.Equal("A", q.A);
        }
        private static double ExecutedInMilliseconds(Action a, Action b)
        {
            DateTime start = DateTime.UtcNow;
            a.Invoke();
            DateTime End = DateTime.UtcNow;
            double fromStartToEndA = End.Subtract(start).TotalMilliseconds;
            start = DateTime.UtcNow;
            b.Invoke();
            End = DateTime.UtcNow;
            double fromStartToEndB = End.Subtract(start).TotalMilliseconds;
            return fromStartToEndB - fromStartToEndA;
        }
    }
}
