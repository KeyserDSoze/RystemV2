using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Reflection
{
    public static class ReflectionHelper
    {
        public static string NameOfCallingClass(bool full = false)
        {
            string name;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
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
                                    if (x.GetCustomAttribute(attributeToIgnore) != null)
                                        return false;
                                return true;
                            }).ToArray());
            return AllProperties[type.FullName];
        }
    }
}
