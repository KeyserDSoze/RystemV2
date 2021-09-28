using Rystem;
using Rystem.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public sealed class Generic<T>
    {
        public T Value { get; }
        public Generic(Type type, params object[] values)
        {
            foreach (var constructor in type.FectConstructors())
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length != values.Length)
                    continue;
                else
                {
                    bool somethingDifferent = false;
                    var valueEnumerator = values.GetEnumerator();
                    foreach (var parameter in parameters)
                    {
                        valueEnumerator.MoveNext();
                        var currentType = valueEnumerator.Current.GetType();
                        if (parameter.ParameterType != currentType &&
                            !currentType.GetInterfaces().Any(x => x == parameter.ParameterType) &&
                            currentType.BaseType != parameter.ParameterType)
                        {
                            somethingDifferent = true;
                            break;
                        }
                    }
                    if (!somethingDifferent)
                    {
                        Value = (T)constructor.Invoke(values);
                        break;
                    }
                }
            }
            if (Value == null)
                throw new ArgumentException($"{type.FullName} has no valid constructor or number of arguments passed ({values.Length}) is unvalid");
        }
        public Generic(params object[] values) : this(typeof(T), values)
        {

        }
        private static readonly Type ObjectType = typeof(object);
        private const string BaseModuleName = "System.Private.CoreLib.dll";
        private static readonly object[] EmptyArray = Array.Empty<object>();
        public static async Task<T> BuildAsync(Func<Task<object[]>> creationFunction)
        {
            var response = await creationFunction.Invoke().NoContext();
            return new Generic<T>(response).Value;
        }
        public static T BuildWithDependencyInjection(Type type = default) 
            => BuildSmart(() => EmptyArray, type);
        public static async Task<T> BuildSmartAsync(Func<Task<object[]>> creationFunction, Type type = default)
        {
            var response = await creationFunction.Invoke().NoContext();
            return BuildSmart(() => response, type);
        }
        public static T BuildSmart(Func<object[]> creationFunction, Type type = default)
        {
            var response = creationFunction.Invoke();
            int counter = 0;
            if (type == default)
                type = typeof(T);
            foreach (var constructor in type.FectConstructors())
            {
                var parameters = constructor.GetParameters();
                var possibleParameters = new List<object>();
                foreach (var parameter in parameters)
                {
                    object value = GetFromServiceLocator(parameter.ParameterType);
                    if (value != default)
                        possibleParameters.Add(value);
                    else if (counter < response.Length)
                        possibleParameters.Add(response[counter++]);
                }
                if (possibleParameters.Count == parameters.Length)
                    return new Generic<T>(type, possibleParameters.ToArray()).Value;
            }
            throw new ArgumentException($"{type.FullName} has no valid constructor or number of arguments passed is unvalid");
            
            static object GetFromServiceLocator(Type type)
            {
                if (ServiceLocator.HasService(type))
                    return ServiceLocator.GetService(type);
                else if (type.BaseType != null && type.BaseType != ObjectType && ServiceLocator.HasService(type.BaseType))
                    return ServiceLocator.GetService(type.BaseType);
                else
                    foreach (var @interface in type.GetInterfaces().Where(x => x.Module.Name != BaseModuleName))
                        if (ServiceLocator.HasService(@interface))
                            return ServiceLocator.GetService(@interface);
                return default;
            }
        }
    }
}