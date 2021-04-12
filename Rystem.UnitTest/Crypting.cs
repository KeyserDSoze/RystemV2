﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class Crypting
    {
        static Crypting()
        {
            CryptingExtensions.Aes.Configure("4a4a4a4a", "4a4a4a4a", "4a4a4a4a4a4a4a4a");
        }
        [Fact]
        public void Aes()
        {
            
            var falseNueve = new FalseNueve()
            {
                Al = "a",
                Ol = "b"
            };
            string crypting1 = falseNueve.Encrypt();
            string crypting2 = falseNueve.Encrypt();
            Assert.NotEqual(crypting1, crypting2);
            Assert.Equal(crypting1.Decrypt<FalseNueve>().Al, crypting2.Decrypt<FalseNueve>().Al);
        }
        [Fact]
        public void Hash()
        {
            var falseNueve = new FalseNueve()
            {
                Al = "a",
                Ol = "b"
            };
            string crypting1 = falseNueve.ToHash();
            string crypting2 = falseNueve.ToHash();
            Assert.Equal(crypting1, crypting2);
        }
        private class FalseNueve
        {
            public string Al { get; set; }
            public string Ol { get; set; }
        }
    }
}