
namespace Rystem.Cache
{
    public sealed record StartingStringPathFinder(CachedHttpMethod Method, string StartingWith) : ICachePathFinder
    {
        public bool IsMatching(string uri)
            => uri.TrimStart('/').ToLower().StartsWith(StartingWith.ToLower());
    }
}