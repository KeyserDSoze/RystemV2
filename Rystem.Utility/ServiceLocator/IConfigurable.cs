namespace Rystem
{
    public interface IConfigurable<TProvider>
    {
        TProvider Configure(string callerName);
        public void Build()
        {
            Configure(GetType().Name);
        }
    }
}