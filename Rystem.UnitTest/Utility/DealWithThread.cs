﻿using Rystem.BackgroundWork;
using System;
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
            action.RunInBackground("3", 300);
            await Task.Delay(1200);
            action.StopRunningInBackground("3");
            Assert.Equal(8, Counter);
        }
        [Fact]
        public async Task RunInBackground2()
        {
            Ghost.Run(async () => await CountAsync(2), runningTime: 300);
            await Task.Delay(1200);
            Ghost.Stop();
            Assert.Equal(8, Counter);
        }
        private int Counter;
        private async Task CountAsync(int v)
        {
            await Task.Delay(0).NoContext();
            Counter += v;
        }
    }
}