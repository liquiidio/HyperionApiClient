using Newtonsoft.Json;
using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Reflection;

namespace EosRio.HyperionApi
{
    public class StatusClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private HttpClient _httpClient;

        public StatusClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>API Service Health Report</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task HealthAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/health");
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }
    
            public T Object { get; }
    
            public string Text { get; }
        }
    
        public bool ReadResponseAsString { get; set; }
        
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = CustomAttributeExtensions.GetCustomAttribute(field, typeof(EnumMemberAttribute)) 
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array) value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public class HistoryClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private HttpClient _httpClient;

        public HistoryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>fetch abi at specific block</summary>
        /// <param name="contract">contract account</param>
        /// <param name="block">target block</param>
        /// <param name="fetch">should fetch the ABI</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_abi_snapshotAsync(string contract, int? block = null, bool? fetch = null, CancellationToken cancellationToken = default)
        {
            if (contract == null)
                throw new ArgumentNullException("contract");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_abi_snapshot?" + Uri.EscapeDataString("contract") + "=").Append(Uri.EscapeDataString(ConvertToString(contract, CultureInfo.InvariantCulture))).Append("&");
            if (block != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("block") + "=").Append(Uri.EscapeDataString(ConvertToString(block, CultureInfo.InvariantCulture))).Append("&");
            }
            if (fetch != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("fetch") + "=").Append(Uri.EscapeDataString(ConvertToString(fetch, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get root actions</summary>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <param name="account">notified account</param>
        /// <param name="track">total results to track (count) [number or true]</param>
        /// <param name="filter">code:name filter</param>
        /// <param name="sort">sort direction</param>
        /// <param name="after">filter after specified date (ISO8601)</param>
        /// <param name="before">filter before specified date (ISO8601)</param>
        /// <param name="simple">simplified output mode</param>
        /// <param name="hot_only">search only the latest hot index</param>
        /// <param name="noBinary">exclude large binary data</param>
        /// <param name="checkLib">perform reversibility check</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response> Get_actionsGetAsync(int? limit = null, int? skip = null, string account = null, string track = null, string filter = null, Sort? sort = null, string after = null, string before = null, bool? simple = null, bool? hotOnly = null, bool? noBinary = null, bool? checkLib = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_actions?");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            if (account != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&");
            }
            if (track != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("track") + "=").Append(Uri.EscapeDataString(ConvertToString(track, CultureInfo.InvariantCulture))).Append("&");
            }
            if (filter != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("filter") + "=").Append(Uri.EscapeDataString(ConvertToString(filter, CultureInfo.InvariantCulture))).Append("&");
            }
            if (sort != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("sort") + "=").Append(Uri.EscapeDataString(ConvertToString(sort, CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("after") + "=").Append(Uri.EscapeDataString(ConvertToString(after, CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("before") + "=").Append(Uri.EscapeDataString(ConvertToString(before, CultureInfo.InvariantCulture))).Append("&");
            }
            if (simple != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("simple") + "=").Append(Uri.EscapeDataString(ConvertToString(simple, CultureInfo.InvariantCulture))).Append("&");
            }
            if (hotOnly != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("hot_only") + "=").Append(Uri.EscapeDataString(ConvertToString(hotOnly, CultureInfo.InvariantCulture))).Append("&");
            }
            if (noBinary != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("noBinary") + "=").Append(Uri.EscapeDataString(ConvertToString(noBinary, CultureInfo.InvariantCulture))).Append("&");
            }
            if (checkLib != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("checkLib") + "=").Append(Uri.EscapeDataString(ConvertToString(checkLib, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get state deltas</summary>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <param name="code">contract account</param>
        /// <param name="scope">table scope</param>
        /// <param name="table">table name</param>
        /// <param name="payer">payer account</param>
        /// <param name="after">filter after specified date (ISO8601)</param>
        /// <param name="before">filter before specified date (ISO8601)</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response2> Get_deltasAsync(int? limit = null, int? skip = null, string code = null, string scope = null, string table = null, string payer = null, string after = null, string before = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_deltas?");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            if (code != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&");
            }
            if (scope != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("scope") + "=").Append(Uri.EscapeDataString(ConvertToString(scope, CultureInfo.InvariantCulture))).Append("&");
            }
            if (table != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("table") + "=").Append(Uri.EscapeDataString(ConvertToString(table, CultureInfo.InvariantCulture))).Append("&");
            }
            if (payer != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("payer") + "=").Append(Uri.EscapeDataString(ConvertToString(payer, CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("after") + "=").Append(Uri.EscapeDataString(ConvertToString(after, CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("before") + "=").Append(Uri.EscapeDataString(ConvertToString(before, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response2>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get producer schedule by version</summary>
        /// <param name="producer">search by producer</param>
        /// <param name="key">search by key</param>
        /// <param name="after">filter after specified date (ISO8601)</param>
        /// <param name="before">filter before specified date (ISO8601)</param>
        /// <param name="version">schedule version</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response3> Get_scheduleAsync(string producer = null, string key = null, string after = null, string before = null, int? version = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_schedule?");
            if (producer != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("producer") + "=").Append(Uri.EscapeDataString(ConvertToString(producer, CultureInfo.InvariantCulture))).Append("&");
            }
            if (key != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("key") + "=").Append(Uri.EscapeDataString(ConvertToString(key, CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("after") + "=").Append(Uri.EscapeDataString(ConvertToString(after, CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("before") + "=").Append(Uri.EscapeDataString(ConvertToString(before, CultureInfo.InvariantCulture))).Append("&");
            }
            if (version != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("version") + "=").Append(Uri.EscapeDataString(ConvertToString(version, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response3>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get transaction by id</summary>
        /// <param name="id">transaction id</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_transactionGetAsync(string id, CancellationToken cancellationToken = default)
        {
            if (id == null)
                throw new ArgumentNullException("id");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_transaction?" + Uri.EscapeDataString("id") + "=").Append(Uri.EscapeDataString(ConvertToString(id, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get actions</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response4> Get_actionsPostAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_actions");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response4>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get transaction by id</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_transactionPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_transaction");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get block traces</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response5> Get_blockAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/trace_api/get_block");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response5>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }
    
            public T Object { get; }
    
            public string Text { get; }
        }
    
        public bool ReadResponseAsString { get; set; }
        
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = CustomAttributeExtensions.GetCustomAttribute(field, typeof(EnumMemberAttribute)) 
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array) value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public class AccountsClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private HttpClient _httpClient;

        public AccountsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get created accounts</summary>
        /// <param name="account">creator account</param>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response6> Get_created_accountsAsync(string account, int? limit = null, int? skip = null, CancellationToken cancellationToken = default)
        {
            if (account == null)
                throw new ArgumentNullException("account");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_created_accounts?" + Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response6>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get account creator</summary>
        /// <param name="account">created account</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response7> Get_creatorAsync(string account, CancellationToken cancellationToken = default)
        {
            if (account == null)
                throw new ArgumentNullException("account");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_creator?" + Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response7>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get account summary</summary>
        /// <param name="account">account name</param>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response8> Get_accountAsync(string account, int? limit = null, int? skip = null, CancellationToken cancellationToken = default)
        {
            if (account == null)
                throw new ArgumentNullException("account");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_account?" + Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response8>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get accounts by public key</summary>
        /// <param name="public_key">public key</param>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <param name="details">include permission details</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response9> Get_key_accountsGetAsync(string publicKey, int? limit = null, int? skip = null, bool? details = null, CancellationToken cancellationToken = default)
        {
            if (publicKey == null)
                throw new ArgumentNullException("publicKey");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_key_accounts?" + Uri.EscapeDataString("public_key") + "=").Append(Uri.EscapeDataString(ConvertToString(publicKey, CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            if (details != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("details") + "=").Append(Uri.EscapeDataString(ConvertToString(details, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response9>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get accounts by public key</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response10> Get_key_accountsPostAsync(Body body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_key_accounts");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response10>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get permission links</summary>
        /// <param name="account">account name</param>
        /// <param name="code">contract name</param>
        /// <param name="action">method name</param>
        /// <param name="permission">permission name</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response11> Get_linksAsync(string account = null, string code = null, string action = null, string permission = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_links?");
            if (account != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&");
            }
            if (code != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&");
            }
            if (action != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("action") + "=").Append(Uri.EscapeDataString(ConvertToString(action, CultureInfo.InvariantCulture))).Append("&");
            }
            if (permission != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("permission") + "=").Append(Uri.EscapeDataString(ConvertToString(permission, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response11>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get all tokens</summary>
        /// <param name="account">account name</param>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_tokensAsync(string account, int? limit = null, int? skip = null, CancellationToken cancellationToken = default)
        {
            if (account == null)
                throw new ArgumentNullException("account");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_tokens?" + Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get controlled accounts by controlling accounts</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response12> Get_controlled_accountsAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_controlled_accounts");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response12>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get accounts by public key</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response13> Get_key_accountsPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_key_accounts");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response13>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }
    
            public T Object { get; }
    
            public string Text { get; }
        }
    
        public bool ReadResponseAsString { get; set; }
        
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = CustomAttributeExtensions.GetCustomAttribute(field, typeof(EnumMemberAttribute)) 
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array) value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public class SystemClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private HttpClient _httpClient;

        public SystemClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get proposals</summary>
        /// <param name="proposer">filter by proposer</param>
        /// <param name="proposal">filter by proposal name</param>
        /// <param name="account">filter by either requested or provided account</param>
        /// <param name="requested">filter by requested account</param>
        /// <param name="provided">filter by provided account</param>
        /// <param name="executed">filter by execution status</param>
        /// <param name="track">total results to track (count) [number or true]</param>
        /// <param name="skip">skip [n] actions (pagination)</param>
        /// <param name="limit">limit of [n] actions per page</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_proposalsAsync(string proposer = null, string proposal = null, string account = null, string requested = null, string provided = null, bool? executed = null, string track = null, int? skip = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_proposals?");
            if (proposer != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("proposer") + "=").Append(Uri.EscapeDataString(ConvertToString(proposer, CultureInfo.InvariantCulture))).Append("&");
            }
            if (proposal != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("proposal") + "=").Append(Uri.EscapeDataString(ConvertToString(proposal, CultureInfo.InvariantCulture))).Append("&");
            }
            if (account != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&");
            }
            if (requested != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("requested") + "=").Append(Uri.EscapeDataString(ConvertToString(requested, CultureInfo.InvariantCulture))).Append("&");
            }
            if (provided != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("provided") + "=").Append(Uri.EscapeDataString(ConvertToString(provided, CultureInfo.InvariantCulture))).Append("&");
            }
            if (executed != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("executed") + "=").Append(Uri.EscapeDataString(ConvertToString(executed, CultureInfo.InvariantCulture))).Append("&");
            }
            if (track != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("track") + "=").Append(Uri.EscapeDataString(ConvertToString(track, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get voters</summary>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <param name="producer">filter by voted producer (comma separated)</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response14> Get_votersAsync(int? limit = null, int? skip = null, string producer = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_voters?");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("skip") + "=").Append(Uri.EscapeDataString(ConvertToString(skip, CultureInfo.InvariantCulture))).Append("&");
            }
            if (producer != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("producer") + "=").Append(Uri.EscapeDataString(ConvertToString(producer, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response14>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }
    
            public T Object { get; }
    
            public string Text { get; }
        }
    
        public bool ReadResponseAsString { get; set; }
        
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = CustomAttributeExtensions.GetCustomAttribute(field, typeof(EnumMemberAttribute)) 
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array) value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public class StatsClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private HttpClient _httpClient;

        public StatsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get action and transaction stats for a given period</summary>
        /// <param name="period">analysis period</param>
        /// <param name="end_date">final date</param>
        /// <param name="unique_actors">compute unique actors</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_action_usageAsync(string period, string endDate = null, bool? uniqueActors = null, CancellationToken cancellationToken = default)
        {
            if (period == null)
                throw new ArgumentNullException("period");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/stats/get_action_usage?" + Uri.EscapeDataString("period") + "=").Append(Uri.EscapeDataString(ConvertToString(period, CultureInfo.InvariantCulture))).Append("&");
            if (endDate != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("end_date") + "=").Append(Uri.EscapeDataString(ConvertToString(endDate, CultureInfo.InvariantCulture))).Append("&");
            }
            if (uniqueActors != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("unique_actors") + "=").Append(Uri.EscapeDataString(ConvertToString(uniqueActors, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get missed blocks</summary>
        /// <param name="producer">filter by producer</param>
        /// <param name="after">filter after specified date (ISO8601)</param>
        /// <param name="before">filter before specified date (ISO8601)</param>
        /// <param name="min_blocks">min. blocks threshold</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<Response15> Get_missed_blocksAsync(string producer = null, string after = null, string before = null, int? minBlocks = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/stats/get_missed_blocks?");
            if (producer != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("producer") + "=").Append(Uri.EscapeDataString(ConvertToString(producer, CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("after") + "=").Append(Uri.EscapeDataString(ConvertToString(after, CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("before") + "=").Append(Uri.EscapeDataString(ConvertToString(before, CultureInfo.InvariantCulture))).Append("&");
            }
            if (minBlocks != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("min_blocks") + "=").Append(Uri.EscapeDataString(ConvertToString(minBlocks, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    var objectResponse = await ReadObjectResponseAsync<Response15>(response, headers, cancellationToken).ConfigureAwait(false);
                    if (objectResponse.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                    }
                    return objectResponse.Object;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get resource usage stats for a specific action</summary>
        /// <param name="code">contract</param>
        /// <param name="action">action name</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_resource_usageAsync(string code, string action, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            if (action == null)
                throw new ArgumentNullException("action");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/stats/get_resource_usage?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("action") + "=").Append(Uri.EscapeDataString(ConvertToString(action, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }
    
            public T Object { get; }
    
            public string Text { get; }
        }
    
        public bool ReadResponseAsString { get; set; }
        
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = CustomAttributeExtensions.GetCustomAttribute(field, typeof(EnumMemberAttribute)) 
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array) value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public class ChainClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private HttpClient _httpClient;

        public ChainClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Abi_bin_to_jsonGetAsync(Name code, Name action, string binargs, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            if (action == null)
                throw new ArgumentNullException("action");
    
            if (binargs == null)
                throw new ArgumentNullException("binargs");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_bin_to_json?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("action") + "=").Append(Uri.EscapeDataString(ConvertToString(action, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("binargs") + "=").Append(Uri.EscapeDataString(ConvertToString(binargs, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Abi_bin_to_jsonHeadAsync(Name code, Name action, string binargs, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            if (action == null)
                throw new ArgumentNullException("action");
    
            if (binargs == null)
                throw new ArgumentNullException("binargs");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_bin_to_json?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("action") + "=").Append(Uri.EscapeDataString(ConvertToString(action, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("binargs") + "=").Append(Uri.EscapeDataString(ConvertToString(binargs, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Abi_bin_to_jsonPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_bin_to_json");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Convert JSON object to binary</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Abi_json_to_binGetAsync(string binargs, CancellationToken cancellationToken = default)
        {
            if (binargs == null)
                throw new ArgumentNullException("binargs");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_json_to_bin?" + Uri.EscapeDataString("binargs") + "=").Append(Uri.EscapeDataString(ConvertToString(binargs, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Convert JSON object to binary</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Abi_json_to_binHeadAsync(string binargs, CancellationToken cancellationToken = default)
        {
            if (binargs == null)
                throw new ArgumentNullException("binargs");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_json_to_bin?" + Uri.EscapeDataString("binargs") + "=").Append(Uri.EscapeDataString(ConvertToString(binargs, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Convert JSON object to binary</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Abi_json_to_binPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_json_to_bin");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the ABI for a contract based on its account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_abiGetAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_abi?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the ABI for a contract based on its account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_abiHeadAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_abi?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the ABI for a contract based on its account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_abiPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_abi");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific account on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_accountGetAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_account?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific account on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_accountHeadAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_account?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific account on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_accountPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_account");
 
                using (var request = new HttpRequestMessage())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(body));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                    request.Content = content;
                    request.Method = new HttpMethod("POST");

                    var url = urlBuilder.ToString();
                    request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                    var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                        var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                        if (response.Content != null && response.Content.Headers != null)
                        {
                            foreach (var item in response.Content.Headers)
                                headers[item.Key] = item.Value;
                        }

                        var status = (int)response.StatusCode;
                        if (status == 200)
                        {
                            return;
                        }
                        else
                        {
                            var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                        }

                }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retreives the activated protocol features for producer node</summary>
        /// <param name="lower_bound">Lower bound</param>
        /// <param name="upper_bound">Upper bound</param>
        /// <param name="limit">The limit, default is 10</param>
        /// <param name="search_by_block_num">Flag to indicate it is has to search by block number</param>
        /// <param name="reverse">Flag to indicate it has to search in reverse</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_activated_protocol_featuresGetAsync(int? lowerBound = null, int? upperBound = null, int? limit = null, bool? searchByBlockNum = null, bool? reverse = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_activated_protocol_features?");
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (upperBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("upper_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(upperBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (searchByBlockNum != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("search_by_block_num") + "=").Append(Uri.EscapeDataString(ConvertToString(searchByBlockNum, CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("reverse") + "=").Append(Uri.EscapeDataString(ConvertToString(reverse, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retreives the activated protocol features for producer node</summary>
        /// <param name="lower_bound">Lower bound</param>
        /// <param name="upper_bound">Upper bound</param>
        /// <param name="limit">The limit, default is 10</param>
        /// <param name="search_by_block_num">Flag to indicate it is has to search by block number</param>
        /// <param name="reverse">Flag to indicate it has to search in reverse</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_activated_protocol_featuresHeadAsync(int? lowerBound = null, int? upperBound = null, int? limit = null, bool? searchByBlockNum = null, bool? reverse = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_activated_protocol_features?");
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (upperBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("upper_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(upperBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (searchByBlockNum != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("search_by_block_num") + "=").Append(Uri.EscapeDataString(ConvertToString(searchByBlockNum, CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("reverse") + "=").Append(Uri.EscapeDataString(ConvertToString(reverse, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retreives the activated protocol features for producer node</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_activated_protocol_featuresPostAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_activated_protocol_features");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific block on the blockchain.</summary>
        /// <param name="block_num_or_id">Provide a `block number` or a `block id`</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_blockGetAsync(string blockNumOrId, CancellationToken cancellationToken = default)
        {
            if (blockNumOrId == null)
                throw new ArgumentNullException("blockNumOrId");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block?" + Uri.EscapeDataString("block_num_or_id") + "=").Append(Uri.EscapeDataString(ConvertToString(blockNumOrId, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific block on the blockchain.</summary>
        /// <param name="block_num_or_id">Provide a `block number` or a `block id`</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_blockHeadAsync(string blockNumOrId, CancellationToken cancellationToken = default)
        {
            if (blockNumOrId == null)
                throw new ArgumentNullException("blockNumOrId");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block?" + Uri.EscapeDataString("block_num_or_id") + "=").Append(Uri.EscapeDataString(ConvertToString(blockNumOrId, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific block on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_blockPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the block header state</summary>
        /// <param name="block_num_or_id">Provide a block_number or a block_id</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_block_header_stateGetAsync(string blockNumOrId, CancellationToken cancellationToken = default)
        {
            if (blockNumOrId == null)
                throw new ArgumentNullException("blockNumOrId");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block_header_state?" + Uri.EscapeDataString("block_num_or_id") + "=").Append(Uri.EscapeDataString(ConvertToString(blockNumOrId, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the block header state</summary>
        /// <param name="block_num_or_id">Provide a block_number or a block_id</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_block_header_stateHeadAsync(string blockNumOrId, CancellationToken cancellationToken = default)
        {
            if (blockNumOrId == null)
                throw new ArgumentNullException("blockNumOrId");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block_header_state?" + Uri.EscapeDataString("block_num_or_id") + "=").Append(Uri.EscapeDataString(ConvertToString(blockNumOrId, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the block header state</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_block_header_statePostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block_header_state");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves contract code</summary>
        /// <param name="code_as_wasm">This must be 1 (true)</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_codeGetAsync(Name accountName, int codeAsWasm, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");

            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_code?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("code_as_wasm") + "=").Append(Uri.EscapeDataString(ConvertToString(codeAsWasm, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves contract code</summary>
        /// <param name="code_as_wasm">This must be 1 (true)</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_codeHeadAsync(Name accountName, int codeAsWasm, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");

            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_code?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("code_as_wasm") + "=").Append(Uri.EscapeDataString(ConvertToString(codeAsWasm, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves contract code</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_codePostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_code");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the current balance</summary>
        /// <param name="symbol">A symbol composed of capital letters between 1-7.</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_currency_balanceGetAsync(Name code, Name account, string symbol, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            if (account == null)
                throw new ArgumentNullException("account");
    
            if (symbol == null)
                throw new ArgumentNullException("symbol");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_balance?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("symbol") + "=").Append(Uri.EscapeDataString(ConvertToString(symbol, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the current balance</summary>
        /// <param name="symbol">A symbol composed of capital letters between 1-7.</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_currency_balanceHeadAsync(Name code, Name account, string symbol, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            if (account == null)
                throw new ArgumentNullException("account");
    
            if (symbol == null)
                throw new ArgumentNullException("symbol");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_balance?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("account") + "=").Append(Uri.EscapeDataString(ConvertToString(account, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("symbol") + "=").Append(Uri.EscapeDataString(ConvertToString(symbol, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the current balance</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_currency_balancePostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_balance");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves currency stats</summary>
        /// <param name="code">contract name</param>
        /// <param name="symbol">token symbol</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_currency_statsGetAsync(string code, string symbol, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            if (symbol == null)
                throw new ArgumentNullException("symbol");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_stats?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("symbol") + "=").Append(Uri.EscapeDataString(ConvertToString(symbol, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves currency stats</summary>
        /// <param name="code">contract name</param>
        /// <param name="symbol">token symbol</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_currency_statsHeadAsync(string code, string symbol, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            if (symbol == null)
                throw new ArgumentNullException("symbol");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_stats?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&" + Uri.EscapeDataString("symbol") + "=").Append(Uri.EscapeDataString(ConvertToString(symbol, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves currency stats</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_currency_statsPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_stats");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_infoGetAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_info");
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_infoHeadAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_info");
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_infoPostAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_info");
 
            using (var request = new HttpRequestMessage())
            {
                request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves producers list</summary>
        /// <param name="limit">total number of producers to retrieve</param>
        /// <param name="lower_bound">In conjunction with limit can be used to paginate through the results. For example, limit=10 and lower_bound=10 would be page 2</param>
        /// <param name="json">return result in JSON format</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_producersGetAsync(string limit = null, string lowerBound = null, bool? json = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_producers?");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("json") + "=").Append(Uri.EscapeDataString(ConvertToString(json, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves producers list</summary>
        /// <param name="limit">total number of producers to retrieve</param>
        /// <param name="lower_bound">In conjunction with limit can be used to paginate through the results. For example, limit=10 and lower_bound=10 would be page 2</param>
        /// <param name="json">return result in JSON format</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_producersHeadAsync(string limit = null, string lowerBound = null, bool? json = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_producers?");
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("json") + "=").Append(Uri.EscapeDataString(ConvertToString(json, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves producers list</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_producersPostAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_producers");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_raw_abiGetAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_abi?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_raw_abiHeadAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_abi?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_raw_abiPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_abi");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw code and ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_raw_code_and_abiGetAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_code_and_abi?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw code and ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_raw_code_and_abiHeadAsync(Name accountName, CancellationToken cancellationToken = default)
        {
            if (accountName == null)
                throw new ArgumentNullException("accountName");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_code_and_abi?" + Uri.EscapeDataString("account_name") + "=").Append(Uri.EscapeDataString(ConvertToString(accountName, CultureInfo.InvariantCulture))).Append("&");
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw code and ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_raw_code_and_abiPostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_code_and_abi");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the scheduled transaction</summary>
        /// <param name="lower_bound">Date/time string in the format YYYY-MM-DDTHH:MM:SS.sss</param>
        /// <param name="limit">The maximum number of transactions to return</param>
        /// <param name="json">true/false whether the packed transaction is converted to json</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_scheduled_transactionGetAsync(string lowerBound = null, int? limit = null, bool? json = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_scheduled_transaction?");
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("json") + "=").Append(Uri.EscapeDataString(ConvertToString(json, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the scheduled transaction</summary>
        /// <param name="lower_bound">Date/time string in the format YYYY-MM-DDTHH:MM:SS.sss</param>
        /// <param name="limit">The maximum number of transactions to return</param>
        /// <param name="json">true/false whether the packed transaction is converted to json</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_scheduled_transactionHeadAsync(string lowerBound = null, int? limit = null, bool? json = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_scheduled_transaction?");
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("json") + "=").Append(Uri.EscapeDataString(ConvertToString(json, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the scheduled transaction</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_scheduled_transactionPostAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_scheduled_transaction");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves table scope</summary>
        /// <param name="code">`name` of the contract to return table data for</param>
        /// <param name="table">Filter results by table</param>
        /// <param name="lower_bound">Filters results to return the first element that is not less than provided value in set</param>
        /// <param name="upper_bound">Filters results to return the first element that is greater than provided value in set</param>
        /// <param name="limit">Limit number of results returned.</param>
        /// <param name="reverse">Reverse the order of returned results</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_table_by_scopeGetAsync(string code, string table = null, string lowerBound = null, string upperBound = null, int? limit = null, bool? reverse = null, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_by_scope?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&");
            if (table != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("table") + "=").Append(Uri.EscapeDataString(ConvertToString(table, CultureInfo.InvariantCulture))).Append("&");
            }
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (upperBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("upper_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(upperBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("reverse") + "=").Append(Uri.EscapeDataString(ConvertToString(reverse, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves table scope</summary>
        /// <param name="code">`name` of the contract to return table data for</param>
        /// <param name="table">Filter results by table</param>
        /// <param name="lower_bound">Filters results to return the first element that is not less than provided value in set</param>
        /// <param name="upper_bound">Filters results to return the first element that is greater than provided value in set</param>
        /// <param name="limit">Limit number of results returned.</param>
        /// <param name="reverse">Reverse the order of returned results</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_table_by_scopeHeadAsync(string code, string table = null, string lowerBound = null, string upperBound = null, int? limit = null, bool? reverse = null, CancellationToken cancellationToken = default)
        {
            if (code == null)
                throw new ArgumentNullException("code");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_by_scope?" + Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&");
            if (table != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("table") + "=").Append(Uri.EscapeDataString(ConvertToString(table, CultureInfo.InvariantCulture))).Append("&");
            }
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (upperBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("upper_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(upperBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("limit") + "=").Append(Uri.EscapeDataString(ConvertToString(limit, CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("reverse") + "=").Append(Uri.EscapeDataString(ConvertToString(reverse, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves table scope</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_table_by_scopePostAsync(object body, CancellationToken cancellationToken = default)
        {
            if (body == null)
                throw new ArgumentNullException("body");
    
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_by_scope");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <param name="code">The name of the smart contract that controls the provided table</param>
        /// <param name="table">The name of the table to query</param>
        /// <param name="scope">The account to which this data belongs</param>
        /// <param name="index_position">Position of the index used, accepted parameters `primary`, `secondary`, `tertiary`, `fourth`, `fifth`, `sixth`, `seventh`, `eighth`, `ninth` , `tenth`</param>
        /// <param name="key_type">Type of key specified by index_position (for example - `uint64_t` or `name`)</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_table_rowsGetAsync(string code = null, string table = null, string scope = null, string indexPosition = null, string keyType = null, string encodeType = null, string upperBound = null, string lowerBound = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_rows?");
            if (code != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&");
            }
            if (table != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("table") + "=").Append(Uri.EscapeDataString(ConvertToString(table, CultureInfo.InvariantCulture))).Append("&");
            }
            if (scope != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("scope") + "=").Append(Uri.EscapeDataString(ConvertToString(scope, CultureInfo.InvariantCulture))).Append("&");
            }
            if (indexPosition != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("index_position") + "=").Append(Uri.EscapeDataString(ConvertToString(indexPosition, CultureInfo.InvariantCulture))).Append("&");
            }
            if (keyType != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("key_type") + "=").Append(Uri.EscapeDataString(ConvertToString(keyType, CultureInfo.InvariantCulture))).Append("&");
            }
            if (encodeType != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("encode_type") + "=").Append(Uri.EscapeDataString(ConvertToString(encodeType, CultureInfo.InvariantCulture))).Append("&");
            }
            if (upperBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("upper_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(upperBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <param name="code">The name of the smart contract that controls the provided table</param>
        /// <param name="table">The name of the table to query</param>
        /// <param name="scope">The account to which this data belongs</param>
        /// <param name="index_position">Position of the index used, accepted parameters `primary`, `secondary`, `tertiary`, `fourth`, `fifth`, `sixth`, `seventh`, `eighth`, `ninth` , `tenth`</param>
        /// <param name="key_type">Type of key specified by index_position (for example - `uint64_t` or `name`)</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_table_rowsHeadAsync(string code = null, string table = null, string scope = null, string indexPosition = null, string keyType = null, string encodeType = null, string upperBound = null, string lowerBound = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_rows?");
            if (code != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("code") + "=").Append(Uri.EscapeDataString(ConvertToString(code, CultureInfo.InvariantCulture))).Append("&");
            }
            if (table != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("table") + "=").Append(Uri.EscapeDataString(ConvertToString(table, CultureInfo.InvariantCulture))).Append("&");
            }
            if (scope != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("scope") + "=").Append(Uri.EscapeDataString(ConvertToString(scope, CultureInfo.InvariantCulture))).Append("&");
            }
            if (indexPosition != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("index_position") + "=").Append(Uri.EscapeDataString(ConvertToString(indexPosition, CultureInfo.InvariantCulture))).Append("&");
            }
            if (keyType != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("key_type") + "=").Append(Uri.EscapeDataString(ConvertToString(keyType, CultureInfo.InvariantCulture))).Append("&");
            }
            if (encodeType != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("encode_type") + "=").Append(Uri.EscapeDataString(ConvertToString(encodeType, CultureInfo.InvariantCulture))).Append("&");
            }
            if (upperBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("upper_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(upperBound, CultureInfo.InvariantCulture))).Append("&");
            }
            if (lowerBound != null)
            {
                urlBuilder.Append(Uri.EscapeDataString("lower_bound") + "=").Append(Uri.EscapeDataString(ConvertToString(lowerBound, CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder.Length--;
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("HEAD");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Get_table_rowsPostAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_rows");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This method expects a transaction in JSON format and will attempt to apply it to the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Push_transactionAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/push_transaction");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This method expects a transaction in JSON format and will attempt to apply it to the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Push_transactionsAsync(IEnumerable<object> body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/push_transactions");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This method expects a transaction in JSON format and will attempt to apply it to the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Send_transactionAsync(object body = null, CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/send_transaction");
 
            using (var request = new HttpRequestMessage())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body));
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content = content;
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Wildcard chain api handler</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task GetAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/*");
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Wildcard chain api handler</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task PostAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/*");
 
            using (var request = new HttpRequestMessage())
            {
                request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                request.Method = new HttpMethod("POST");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }
    
            public T Object { get; }
    
            public string Text { get; }
        }
    
        public bool ReadResponseAsString { get; set; }
        
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = CustomAttributeExtensions.GetCustomAttribute(field, typeof(EnumMemberAttribute)) 
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array) value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public class InternalClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private HttpClient _httpClient;

        public InternalClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task Stream_httpClientjsAsync(CancellationToken cancellationToken = default)
        {
            var urlBuilder = new StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/stream-client.js");
 
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");

                var url = urlBuilder.ToString();
                request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers = Enumerable.ToDictionary(response.Headers, h => h.Key, h => h.Value);
                if (response.Content != null && response.Content.Headers != null)
                {
                    foreach (var item in response.Content.Headers)
                        headers[item.Key] = item.Value;
                }

                var status = (int)response.StatusCode;
                if (status == 200)
                {
                    return;
                }
                else
                {
                    var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
                }
            }
        }
    
        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }
    
            public T Object { get; }
    
            public string Text { get; }
        }
    
        public bool ReadResponseAsString { get; set; }
        
        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(responseStream))
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = CustomAttributeExtensions.GetCustomAttribute(field, typeof(EnumMemberAttribute)) 
                            as EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = Enumerable.OfType<object>((Array) value);
                return string.Join(",", Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }

    /// <summary>sort direction</summary>
    public enum Sort
    {
        [EnumMember(Value = @"desc")]
        Desc = 0,
    
        [EnumMember(Value = @"asc")]
        Asc = 1,
    
        [EnumMember(Value = @"1")]
        _1 = 2,
    
        [EnumMember(Value = @"-1")]
        Minus1 = 3,
    
    }

    public class Body 
    {
        /// <summary>public key</summary>
        [JsonProperty("public_key", Required = Required.Always)]
        public string PublicKey { get; set; }
    }

    public class Name 
    {

    
    
    }

    public class Response 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("simple_actions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<SimpleActions> SimpleActions { get; set; }
    
        [JsonProperty("actions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Actions> Actions { get; set; }
    

    
    
    }

    public class Response2 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("deltas", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Deltas> Deltas { get; set; }
    }

    public class Response3 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }
    
        [JsonProperty("version", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Version { get; set; }
    
        [JsonProperty("producers", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Producers> Producers { get; set; }
    }

    public class Response4 
    {
        [JsonProperty("query_time", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTime { get; set; }
    
        [JsonProperty("last_irreversible_block", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double LastIrreversibleBlock { get; set; }
    
        [JsonProperty("actions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Actions2> Actions { get; set; }
    }

    public class Response5 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    
        [JsonProperty("number", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int Number { get; set; }
    
        [JsonProperty("previous_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string PreviousId { get; set; }
    
        [JsonProperty("status", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("producer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Producer { get; set; }
    
        [JsonProperty("transactions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Transactions> Transactions { get; set; }
    }

    public class Response6 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("query_time", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTime { get; set; }
    
        [JsonProperty("accounts", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Accounts> Accounts { get; set; }
    }

    public class Response7 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("creator", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Creator { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int BlockNum { get; set; }
    
        [JsonProperty("trx_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrxId { get; set; }
    
        [JsonProperty("indirect_creator", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string IndirectCreator { get; set; }
    }

    public class Response8 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object Account { get; set; }
    
        [JsonProperty("links", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Links> Links { get; set; }
    
        [JsonProperty("tokens", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Tokens> Tokens { get; set; }
    
        [JsonProperty("total_actions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double TotalActions { get; set; }
    
        [JsonProperty("actions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Actions3> Actions { get; set; }
    }

    public class Response9 
    {
        [JsonProperty("account_names", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> AccountNames { get; set; }
    
        [JsonProperty("permissions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Permissions> Permissions { get; set; }
    }

    public class Response10 
    {
        [JsonProperty("account_names", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> AccountNames { get; set; }
    }

    public class Response11 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("links", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Links2> Links { get; set; }
    }

    public class Response12 
    {
        [JsonProperty("controlled_accounts", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> ControlledAccounts { get; set; }
    }

    public class Response13 
    {
        [JsonProperty("account_names", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> AccountNames { get; set; }
    }

    public class Response14 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("voters", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Voters> Voters { get; set; }
    }

    public class Response15 
    {
        [JsonProperty("query_time_ms", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double QueryTimeMs { get; set; }
    
        [JsonProperty("cached", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [JsonProperty("hot_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool HotOnly { get; set; }
    
        [JsonProperty("lib", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [JsonProperty("total", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [JsonProperty("stats", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Stats Stats { get; set; }
    
        [JsonProperty("events", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Events> Events { get; set; }
    }

    public class Total 
    {
        [JsonProperty("value", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [JsonProperty("relation", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public class SimpleActions 
    {
        [JsonProperty("block", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Block { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("irreversible", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Irreversible { get; set; }
    
        [JsonProperty("contract", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Contract { get; set; }
    
        [JsonProperty("action", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
    
        [JsonProperty("actors", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Actors { get; set; }
    
        [JsonProperty("notified", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Notified { get; set; }
    
        [JsonProperty("transaction_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TransactionId { get; set; }
    
        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public class Actions 
    {
        [JsonProperty("@timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string timestamp { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }
    
        [JsonProperty("trx_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrxId { get; set; }
    
        [JsonProperty("act", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Act Act { get; set; }
    
        [JsonProperty("notified", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> Notified { get; set; }
    
        [JsonProperty("cpu_usage_us", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double CpuUsageUs { get; set; }
    
        [JsonProperty("net_usage_words", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double NetUsageWords { get; set; }
    
        [JsonProperty("account_ram_deltas", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<AccountRamDeltas> AccountRamDeltas { get; set; }
    
        [JsonProperty("global_sequence", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double GlobalSequence { get; set; }
    
        [JsonProperty("receiver", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [JsonProperty("producer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Producer { get; set; }
    
        [JsonProperty("parent", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Parent { get; set; }
    
        [JsonProperty("action_ordinal", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double ActionOrdinal { get; set; }
    
        [JsonProperty("creator_action_ordinal", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double CreatorActionOrdinal { get; set; }
    }

    public class Deltas 
    {
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("code", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
    
        [JsonProperty("scope", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Scope { get; set; }
    
        [JsonProperty("table", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Table { get; set; }
    
        [JsonProperty("primary_key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryKey { get; set; }
    
        [JsonProperty("payer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Payer { get; set; }
    
        [JsonProperty("present", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Present { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }
    
        [JsonProperty("block_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string BlockId { get; set; }
    
        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public class Producers 
    {
        [JsonProperty("producer_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ProducerName { get; set; }
    
        [JsonProperty("block_signing_key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string BlockSigningKey { get; set; }
    
        [JsonProperty("legacy_key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LegacyKey { get; set; }
    }

    public class Actions2 
    {
        [JsonProperty("account_action_seq", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double AccountActionSeq { get; set; }
    
        [JsonProperty("global_action_seq", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double GlobalActionSeq { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }
    
        [JsonProperty("block_time", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string BlockTime { get; set; }
    
        [JsonProperty("action_trace", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ActionTrace ActionTrace { get; set; }
    }

    public class Transactions 
    {
        [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    
        [JsonProperty("actions", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Actions4> Actions { get; set; }
    }

    public class Accounts 
    {
        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("trx_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrxId { get; set; }
    }

    public class Links 
    {
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("permission", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Permission { get; set; }
    
        [JsonProperty("code", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
    
        [JsonProperty("action", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
    }

    public class Tokens 
    {
        [JsonProperty("symbol", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Symbol { get; set; }
    
        [JsonProperty("precision", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int Precision { get; set; }
    
        [JsonProperty("amount", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Amount { get; set; }
    
        [JsonProperty("contract", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Contract { get; set; }
    }

    public class Actions3 
    {
        [JsonProperty("@timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string timestamp { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }
    
        [JsonProperty("trx_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrxId { get; set; }
    
        [JsonProperty("act", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Act2 Act { get; set; }
    
        [JsonProperty("notified", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> Notified { get; set; }
    
        [JsonProperty("cpu_usage_us", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double CpuUsageUs { get; set; }
    
        [JsonProperty("net_usage_words", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double NetUsageWords { get; set; }
    
        [JsonProperty("account_ram_deltas", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<AccountRamDeltas2> AccountRamDeltas { get; set; }
    
        [JsonProperty("global_sequence", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double GlobalSequence { get; set; }
    
        [JsonProperty("receiver", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [JsonProperty("producer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Producer { get; set; }
    
        [JsonProperty("parent", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Parent { get; set; }
    
        [JsonProperty("action_ordinal", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double ActionOrdinal { get; set; }
    
        [JsonProperty("creator_action_ordinal", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double CreatorActionOrdinal { get; set; }
    }

    public class Permissions 
    {
        [JsonProperty("owner", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Owner { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int BlockNum { get; set; }
    
        [JsonProperty("parent", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Parent { get; set; }
    
        [JsonProperty("last_updated", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LastUpdated { get; set; }
    
        [JsonProperty("auth", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object Auth { get; set; }
    
        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [JsonProperty("present", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Present { get; set; }
    }

    public class Links2 
    {
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }
    
        [JsonProperty("timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("permission", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Permission { get; set; }
    
        [JsonProperty("code", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
    
        [JsonProperty("action", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
    
        [JsonProperty("irreversible", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool Irreversible { get; set; }
    }

    public class Voters 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("weight", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Weight { get; set; }
    
        [JsonProperty("last_vote", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double LastVote { get; set; }
    
        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public class Stats 
    {
        [JsonProperty("by_producer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object ByProducer { get; set; }
    }

    public class Events 
    {
        [JsonProperty("@timestamp", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [JsonProperty("last_block", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double LastBlock { get; set; }
    
        [JsonProperty("schedule_version", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double ScheduleVersion { get; set; }
    
        [JsonProperty("size", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Size { get; set; }
    
        [JsonProperty("producer", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Producer { get; set; }
    }

    public class Act 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public class AccountRamDeltas 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("delta", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Delta { get; set; }
    }

    public class ActionTrace 
    {
        [JsonProperty("action_ordinal", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double ActionOrdinal { get; set; }
    
        [JsonProperty("creator_action_ordinal", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double CreatorActionOrdinal { get; set; }
    
        [JsonProperty("receipt", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Receipt Receipt { get; set; }
    
        [JsonProperty("receiver", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [JsonProperty("act", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Act3 Act { get; set; }
    
        [JsonProperty("trx_id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string TrxId { get; set; }
    
        [JsonProperty("block_num", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double BlockNum { get; set; }
    
        [JsonProperty("block_time", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string BlockTime { get; set; }
    }

    public class Actions4 
    {
        [JsonProperty("receiver", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("action", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Action { get; set; }
    
        [JsonProperty("authorization", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Authorization> Authorization { get; set; }
    
        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public class Act2 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public class AccountRamDeltas2 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("delta", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Delta { get; set; }
    }

    public class Receipt 
    {
        [JsonProperty("receiver", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [JsonProperty("global_sequence", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double GlobalSequence { get; set; }
    
        [JsonProperty("recv_sequence", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double RecvSequence { get; set; }
    
        [JsonProperty("auth_sequence", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<AuthSequence> AuthSequence { get; set; }
    }

    public class Act3 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [JsonProperty("authorization", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<object> Authorization { get; set; }
    
        [JsonProperty("data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
    
        [JsonProperty("hex_data", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string HexData { get; set; }
    }

    public class Authorization 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("permission", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Permission { get; set; }
    }

    public class AuthSequence 
    {
        [JsonProperty("account", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [JsonProperty("sequence", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public double Sequence { get; set; }
    }

    
    public class ApiException : Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    
    public class ApiException<TResult> : ApiException
    {
        public TResult Result { get; private set; }

        public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

}