namespace Rystem.Cache
{
    public interface ICachePathFinder
    {
        CachedHttpMethod Method { get; }
        bool IsMatching(string uri);
    }
}