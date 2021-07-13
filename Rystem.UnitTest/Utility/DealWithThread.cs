using Microsoft.Extensions.DependencyInjection;
using Rystem.Background;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class DealWithThread
    {
        [Fact]
        public async Task RunInBackground()
        {
            Action action = async () => await CountAsync(2);
            action.RunInBackground("3", () => 300);
            await Task.Delay(1400);
            action.StopRunningInBackground("3");
            Assert.Equal(8, Counter);
        }
        [Fact]
        public async Task RunInBackground2()
        {
            BackgroundWork.Run(async () => await CountAsync(2), nextRunningTime: () => 300, runImmediately: true);
            await Task.Delay(1100);
            BackgroundWork.Stop();
            Assert.Equal(8, Counter);
        }
        [Fact]
        public async Task RunInIServiceCollection()
        {
            new MyServiceCollection().AddBackgroundWork<MyFirstBackgroundWork>();
            await Task.Delay(2000);
            BackgroundWork.Stop();
            Assert.Equal(6, MyFirstBackgroundWork.Counter);
        }
        private class MyFirstBackgroundWork : IBackgroundWork
        {
            public async Task ActionToDo()
            {
                await Task.Delay(0).NoContext();
                Counter += 2;
            }
            public static int Counter;
            public bool RunImmediately => true;
            public string Cron => "* * * * * *";
        }

        private class MyServiceCollection : IServiceCollection
        {
            public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<ServiceDescriptor> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public int IndexOf(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private int Counter;
        private async Task CountAsync(int v)
        {
            await Task.Delay(0).NoContext();
            Counter += v;
        }
    }
}
