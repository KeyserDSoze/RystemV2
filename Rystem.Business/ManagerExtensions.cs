using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rystem.Business
{
    internal static class ManagerExtensions
    {
        private static readonly Dictionary<string, dynamic> Managers = new();
        private static readonly object Semaphore = new();
        public static dynamic DefaultManager<TEntity>(this TEntity entity, string baseKey, Func<TEntity, dynamic> managerCreator)
        {
            string key = $"{baseKey}{entity.GetType().FullName}";
            if (!Managers.ContainsKey(key))
                lock (Semaphore)
                    if (!Managers.ContainsKey(key))
                        Managers.Add(key, managerCreator.Invoke(entity));
            return Managers[key];
        }
        public static dynamic DefaultManager<TEntity, TEntity2>(this TEntity entity, string baseKey, Func<TEntity, dynamic> managerCreator)
        {
            string key = $"{baseKey}{entity.GetType().FullName}";
            if (!Managers.ContainsKey(key))
                lock (Semaphore)
                    if (!Managers.ContainsKey(key))
                        Managers.Add(key, managerCreator.Invoke(entity));
            return Managers[key];
        }
    }
}