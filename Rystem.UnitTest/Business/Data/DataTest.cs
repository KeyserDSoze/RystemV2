﻿using Rystem.Azure.Integration;
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
    public class DataTest
    {
        static DataTest()
        {
            AzureConst.Load()
                .UseDataOn<MiniContainer>()
                .WithAzure()
                .WithBlockBlob()
                .WithName(x => x.Cads)
                .AndWithAzure(Installation.Inst00)
                .WithAppendBlob()
                .WithName(x => x.Cads)
                .Configure()
                .FinalizeWithoutDependencyInjection();
        }
        [Fact]
        public async Task BlockBlobStorage()
        {
            await Sample.Run(Installation.Default).NoContext();
        }
        [Fact]
        public async Task AppendBlobStorage()
        {
            await Sample.Run(Installation.Inst00).NoContext();
        }
    }
}