﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class SystemExtensions
    {
        private static readonly JsonSerializerSettings Options = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameHandling = TypeNameHandling.Auto,
        };
        public static T DeepCopy<T>(this T original)
            => original.ToJson(Options).FromJson<T>(Options);
    }
}