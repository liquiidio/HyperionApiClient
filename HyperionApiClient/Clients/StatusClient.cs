using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EosRio.HyperionApi
{
    public class StatusClient : ClientExtensions
    {
        private readonly HttpClient _httpClient;

        public StatusClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl { get; set; } = "https://api.wax.liquidstudios.io/";

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>API Service Health Report</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task HealthAsync(CancellationToken cancellationToken = default)
        {
            // TODO return value

            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/health");
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = response.Headers.ToDictionary(h => h.Key, h => h.Value);
                if (response.Content?.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }

                var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
            }
        }
    }
}