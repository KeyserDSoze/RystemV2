using Newtonsoft.Json;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Rystem.Net
{
    public class RystemHttpRequestBuilder
    {
        public Uri Uri { get; }
        public int Timeout { get; private set; } = 30_000;
        public HttpMethod Method { get; private set; } = HttpMethod.Get;
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public bool KeepAlive { get; private set; } = true;
        public byte[] Body { get; private set; }
        public Stream BodyAsStream { get; private set; }
        internal RystemHttpRequestBuilder(Uri uri) => Uri = uri;
        private RystemHttpRequestBuilder ExecuteAction(Action action)
        {
            action.Invoke();
            return this;
        }
        public RystemHttpRequestBuilder AddToHeaders(string key, string value)
        {
            if (!this.Headers.ContainsKey(key))
                this.Headers.Add(key, value);
            else
                this.Headers[key] = value;
            return this;
        }
        public RystemHttpRequestBuilder RemoveFromHeaders(string key)
        {
            if (this.Headers.ContainsKey(key))
                this.Headers.Remove(key);
            return this;
        }
        public RystemHttpRequestBuilder SetTimeout(int timeout)
            => ExecuteAction(() => this.Timeout = timeout);
        public RystemHttpRequestBuilder WithMethod(HttpMethod method)
            => ExecuteAction(() => this.Method = method);
        public RystemHttpRequestBuilder WithKeepAlive()
           => ExecuteAction(() => this.KeepAlive = true);
        public RystemHttpRequestBuilder WithoutKeepAlive()
           => ExecuteAction(() => this.KeepAlive = false);
        public RystemHttpRequestBuilder SetUserAgent(string useragent)
            => AddToHeaders("User-Agent", useragent);
        public RystemHttpRequestBuilder WithContentType(string contentType)
            => AddToHeaders("Content-Type", contentType);
        public RystemHttpRequestBuilder AddBody<T>(T entity, JsonSerializerSettings serializerOptions = null, EncodingType encodingType = EncodingType.UTF8)
            => ExecuteAction(() => { Body = entity.ToJson(serializerOptions).ToByteArray(encodingType); BodyAsStream = null; });
        public RystemHttpRequestBuilder AddBody(string entity, EncodingType encodingType = EncodingType.UTF8)
            => ExecuteAction(() => { Body = entity.ToByteArray(encodingType); BodyAsStream = null; });
        public RystemHttpRequestBuilder AddBody(Stream entity)
            => ExecuteAction(() => { BodyAsStream = entity; Body = null; });
        public RystemHttpRequestBuilder AddBody(byte[] entity)
           => ExecuteAction(() => { Body = entity; BodyAsStream = null; });
        public RystemHttpRequest Build()
            => new(this);
    }
    public class RystemHttpRequest
    {
        private readonly RystemHttpRequestBuilder Builder;
        internal RystemHttpRequest(RystemHttpRequestBuilder rystemHttpRequestBuilder) => Builder = rystemHttpRequestBuilder;
        public async Task<string> InvokeAsync()
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Builder.Uri);
            httpWebRequest.Timeout = Builder.Timeout;
            httpWebRequest.KeepAlive = Builder.KeepAlive;
            if (Builder.Method != null)
                httpWebRequest.Method = Builder.Method.ToString();
            foreach (var header in Builder.Headers)
                httpWebRequest.Headers.Add(header.Key, header.Value);
            Stream body = Builder.Body?.ToStream() ?? Builder.BodyAsStream;
            if (body != null)
            {
                using Stream requestStream = await httpWebRequest.GetRequestStreamAsync().NoContext();
                await body.CopyToAsync(requestStream).NoContext();
            }
            using HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().NoContext();
            using StreamReader reader = new(httpWebResponse.GetResponseStream());
            return await reader.ReadToEndAsync().NoContext();
        }
        public async Task<T> InvokeAsync<T>(JsonSerializerSettings options = null)
            => (await InvokeAsync().NoContext()).FromJson<T>(options);
    }
}