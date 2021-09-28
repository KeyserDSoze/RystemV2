using Rystem.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http.Json
{
    public static class HttpClientExtensions
    {
        public static async Task<T> PostAsync<T>(this HttpClient httpClient, Uri requestUri = default, HttpContent httpContent = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsync(requestUri, httpContent, cancellationToken).NoContext()).Content.ReadAsStringAsync().NoContext()).FromJson<T>(options);
        public static async Task<T> PostAsync<T>(this HttpClient httpClient, string requestUri = default, HttpContent httpContent = default, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsync(requestUri, httpContent, cancellationToken).NoContext()).Content.ReadAsStringAsync().NoContext()).FromJson<T>(options);
        public static async Task<TOutput> PostAsJsonAsync<TInput, TOutput>(this HttpClient httpClient, TInput value, Uri requestUri = default, JsonSerializerOptions options = default, JsonSerializerOptions outputOptions = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsJsonAsync(requestUri, value, options, cancellationToken).NoContext()).Content.ReadAsStringAsync(cancellationToken).NoContext()).FromJson<TOutput>(outputOptions);
        public static async Task<TOutput> PostAsJsonAsync<TInput, TOutput>(this HttpClient httpClient, TInput value, string requestUri = default, JsonSerializerOptions options = default, JsonSerializerOptions outputOptions = default, CancellationToken cancellationToken = default)
            => (await (await httpClient.PostAsJsonAsync(requestUri, value, options, cancellationToken).NoContext()).Content.ReadAsStringAsync(cancellationToken).NoContext()).FromJson<TOutput>(outputOptions);
    }
}