namespace Rystem
{
    public sealed class Service<T>
    {
        private T value;
        public T Value => value ??= ServiceLocator.GetService<T>();
    }
}