
namespace Rystem.Memory
{
    public record StartingStringCacheChecker(CachedHttpMethod Method, string StartingWith) : ICacheCheker
    {
        public bool IsMatching(string uri)
            => uri.TrimStart('/').ToLower().StartsWith(StartingWith.ToLower());
    }
}