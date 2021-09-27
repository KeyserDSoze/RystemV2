using Rystem.Text;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient httpClient, Uri requestUri = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await httpClient.GetStringAsync(requestUri, cancellationToken).NoContext()).FromJson<T>(options);
        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string requestUri = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await httpClient.GetStringAsync(requestUri, cancellationToken).NoContext()).FromJson<T>(options);
        public static async Task<T> PostAsync<T>(this HttpClient httpClient, Uri requestUri = default, HttpContent httpContent = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsync(requestUri, httpContent, cancellationToken).NoContext()).Content.ReadAsStringAsync().NoContext()).FromJson<T>(options);
        public static async Task<T> PostAsync<T>(this HttpClient httpClient, string requestUri = default, HttpContent httpContent = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsync(requestUri, httpContent, cancellationToken).NoContext()).Content.ReadAsStringAsync().NoContext()).FromJson<T>(options);
        public static async Task<TOutput> PostAsync<TInput, TOutput>(this HttpClient httpClient, TInput value, Uri requestUri = default, MediaTypeHeaderValue header = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsync(requestUri, JsonContent.Create(value, header, options), cancellationToken).NoContext()).Content.ReadAsStringAsync(cancellationToken).NoContext()).FromJson<TOutput>();
        public static async Task<TOutput> PostAsync<TInput, TOutput>(this HttpClient httpClient, TInput value, string requestUri = default, MediaTypeHeaderValue header = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsync(requestUri, JsonContent.Create(value, header, options), cancellationToken).NoContext()).Content.ReadAsStringAsync(cancellationToken).NoContext()).FromJson<TOutput>();
    }
}