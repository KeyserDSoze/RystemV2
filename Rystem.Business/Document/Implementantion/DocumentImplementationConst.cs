using Microsoft.Azure.Cosmos.Table;
using Rystem.Azure.Integration;
using Rystem.Azure.Integration.Storage;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rystem.Business.Document.Implementantion
{
    internal static class DocumentImplementationConst
    {
        public const string PrimaryKey = "PrimaryKey";
        public const string SecondaryKey = "SecondaryKey";
        public const string Timestamp = "Timestamp";
    }
}