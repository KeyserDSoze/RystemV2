namespace Rystem.Memory
{
    public interface ICacheCheker
    {
        CachedHttpMethod Method { get; }
        bool IsMatching(string uri);
    }
}