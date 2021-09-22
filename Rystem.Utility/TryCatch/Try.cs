using System;
using System.Threading.Tasks;

namespace Rystem
{
    public static class Try
    {
        public static Catcher Execute(Func<Task> action, int runninAttempts = 1) => new(action, runninAttempts);
        public static Catcher<T> Execute<T>(Func<Task<T>> action, int runninAttempts = 1) => new(action, runninAttempts);
    }
}