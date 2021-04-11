using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static partial class HttpWebRequestExtensions
    {
        public static RystemHttpRequestBuilder CreateHttpRequest(this Uri uri) 
            => new(uri);

    }
}