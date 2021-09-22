using System;

namespace Rystem.Net
{
    public static partial class HttpWebRequestExtensions
    {
        public static RystemHttpRequestBuilder CreateHttpRequest(this Uri uri) 
            => new(uri);
    }
}