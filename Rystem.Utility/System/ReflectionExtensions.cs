﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rystem.Reflection
{
    public static class ReflectionHelper
    {
        public static string NameOfCallingClass(int deep = 1, bool full = false)
        {
            string name;
            Type declaringType;
            int skipFrames = 1 + deep;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == default)
                    return method.Name;
                skipFrames++;
                name = full ? declaringType.FullName : declaringType.Name;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));
            return name;
        }
    }
    public static class ReflectionExtensions
    {
        private readonly static Dictionary<string, PropertyInfo[]> AllProperties = new Dictionary<string, PropertyInfo[]>();
        private readonly static Dictionary<string, ConstructorInfo[]> AllConstructors = new Dictionary<string, ConstructorInfo[]>();
        private readonly static object TrafficCard = new object();
        public static PropertyInfo[] FetchProperties(this Type type, params Type[] attributesToIgnore)
        {
            if (!AllProperties.ContainsKey(type.FullName))
                lock (TrafficCard)
                    if (!AllProperties.ContainsKey(type.FullName))
                        AllProperties.Add(type.FullName, type.GetProperties()
                            .Where(x =>
                            {
                                foreach (Type attributeToIgnore in attributesToIgnore)
                                    if (x.GetCustomAttribute(attributeToIgnore) != default)
                                        return false;
                                return true;
                            }).ToArray());
            return AllProperties[type.FullName];
        }
        public static ConstructorInfo[] FectConstructors(this Type type)
        {
            if (!AllConstructors.ContainsKey(type.FullName))
                lock (TrafficCard)
                    if (!AllConstructors.ContainsKey(type.FullName))
                        AllConstructors.Add(type.FullName, type.GetConstructors().ToArray());
            return AllConstructors[type.FullName];
        }
    }
}
