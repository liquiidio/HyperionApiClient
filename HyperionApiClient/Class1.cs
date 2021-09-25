using Newtonsoft.Json;

namespace EosRio.HyperionApi
{
    public partial class StatusClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private System.Net.Http.HttpClient _httpClient;

        public StatusClient(System.Net.Http.HttpClient httpClient)
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
        public async System.Threading.Tasks.Task HealthAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/health");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        
        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
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
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public partial class HistoryClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private System.Net.Http.HttpClient _httpClient;

        public HistoryClient(System.Net.Http.HttpClient httpClient)
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
        public async System.Threading.Tasks.Task Get_abi_snapshotAsync(string contract, int? block = null, bool? fetch = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (contract == null)
                throw new System.ArgumentNullException("contract");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_abi_snapshot?" + System.Uri.EscapeDataString("contract") + "=").Append(System.Uri.EscapeDataString(ConvertToString(contract, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (block != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("block") + "=").Append(System.Uri.EscapeDataString(ConvertToString(block, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (fetch != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("fetch") + "=").Append(System.Uri.EscapeDataString(ConvertToString(fetch, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task<Response> Get_actionsGetAsync(int? limit = null, int? skip = null, string account = null, string track = null, string filter = null, Sort? sort = null, string after = null, string before = null, bool? simple = null, bool? hot_only = null, bool? noBinary = null, bool? checkLib = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_actions?");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (account != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (track != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("track") + "=").Append(System.Uri.EscapeDataString(ConvertToString(track, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (filter != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("filter") + "=").Append(System.Uri.EscapeDataString(ConvertToString(filter, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (sort != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("sort") + "=").Append(System.Uri.EscapeDataString(ConvertToString(sort, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("after") + "=").Append(System.Uri.EscapeDataString(ConvertToString(after, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("before") + "=").Append(System.Uri.EscapeDataString(ConvertToString(before, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (simple != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("simple") + "=").Append(System.Uri.EscapeDataString(ConvertToString(simple, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (hot_only != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("hot_only") + "=").Append(System.Uri.EscapeDataString(ConvertToString(hot_only, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (noBinary != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("noBinary") + "=").Append(System.Uri.EscapeDataString(ConvertToString(noBinary, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (checkLib != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("checkLib") + "=").Append(System.Uri.EscapeDataString(ConvertToString(checkLib, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task<Response2> Get_deltasAsync(int? limit = null, int? skip = null, string code = null, string scope = null, string table = null, string payer = null, string after = null, string before = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_deltas?");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (code != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (scope != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("scope") + "=").Append(System.Uri.EscapeDataString(ConvertToString(scope, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (table != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("table") + "=").Append(System.Uri.EscapeDataString(ConvertToString(table, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (payer != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("payer") + "=").Append(System.Uri.EscapeDataString(ConvertToString(payer, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("after") + "=").Append(System.Uri.EscapeDataString(ConvertToString(after, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("before") + "=").Append(System.Uri.EscapeDataString(ConvertToString(before, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response2>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task<Response3> Get_scheduleAsync(string producer = null, string key = null, string after = null, string before = null, int? version = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_schedule?");
            if (producer != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("producer") + "=").Append(System.Uri.EscapeDataString(ConvertToString(producer, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (key != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("key") + "=").Append(System.Uri.EscapeDataString(ConvertToString(key, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("after") + "=").Append(System.Uri.EscapeDataString(ConvertToString(after, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("before") + "=").Append(System.Uri.EscapeDataString(ConvertToString(before, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (version != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("version") + "=").Append(System.Uri.EscapeDataString(ConvertToString(version, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response3>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get transaction by id</summary>
        /// <param name="id">transaction id</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_transactionGetAsync(string id, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (id == null)
                throw new System.ArgumentNullException("id");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_transaction?" + System.Uri.EscapeDataString("id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get actions</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response4> Get_actionsPostAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_actions");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response4>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get transaction by id</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_transactionPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_transaction");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get block traces</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response5> Get_blockAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/trace_api/get_block");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response5>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        
        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
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
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public partial class AccountsClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private System.Net.Http.HttpClient _httpClient;

        public AccountsClient(System.Net.Http.HttpClient httpClient)
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
        public async System.Threading.Tasks.Task<Response6> Get_created_accountsAsync(string account, int? limit = null, int? skip = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account == null)
                throw new System.ArgumentNullException("account");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_created_accounts?" + System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response6>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get account creator</summary>
        /// <param name="account">created account</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response7> Get_creatorAsync(string account, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account == null)
                throw new System.ArgumentNullException("account");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/history/get_creator?" + System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response7>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get account summary</summary>
        /// <param name="account">account name</param>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response8> Get_accountAsync(string account, int? limit = null, int? skip = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account == null)
                throw new System.ArgumentNullException("account");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_account?" + System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response8>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task<Response9> Get_key_accountsGetAsync(string public_key, int? limit = null, int? skip = null, bool? details = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (public_key == null)
                throw new System.ArgumentNullException("public_key");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_key_accounts?" + System.Uri.EscapeDataString("public_key") + "=").Append(System.Uri.EscapeDataString(ConvertToString(public_key, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (details != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("details") + "=").Append(System.Uri.EscapeDataString(ConvertToString(details, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response9>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get accounts by public key</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response10> Get_key_accountsPostAsync(Body body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_key_accounts");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response10>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task<Response11> Get_linksAsync(string account = null, string code = null, string action = null, string permission = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_links?");
            if (account != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (code != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (action != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("action") + "=").Append(System.Uri.EscapeDataString(ConvertToString(action, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (permission != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("permission") + "=").Append(System.Uri.EscapeDataString(ConvertToString(permission, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response11>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_tokensAsync(string account, int? limit = null, int? skip = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account == null)
                throw new System.ArgumentNullException("account");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_tokens?" + System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get controlled accounts by controlling accounts</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response12> Get_controlled_accountsAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_controlled_accounts");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response12>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get accounts by public key</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response13> Get_key_accountsPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/history/get_key_accounts");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response13>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        
        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
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
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public partial class SystemClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private System.Net.Http.HttpClient _httpClient;

        public SystemClient(System.Net.Http.HttpClient httpClient)
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
        public async System.Threading.Tasks.Task Get_proposalsAsync(string proposer = null, string proposal = null, string account = null, string requested = null, string provided = null, bool? executed = null, string track = null, int? skip = null, int? limit = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_proposals?");
            if (proposer != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("proposer") + "=").Append(System.Uri.EscapeDataString(ConvertToString(proposer, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (proposal != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("proposal") + "=").Append(System.Uri.EscapeDataString(ConvertToString(proposal, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (account != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (requested != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("requested") + "=").Append(System.Uri.EscapeDataString(ConvertToString(requested, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (provided != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("provided") + "=").Append(System.Uri.EscapeDataString(ConvertToString(provided, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (executed != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("executed") + "=").Append(System.Uri.EscapeDataString(ConvertToString(executed, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (track != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("track") + "=").Append(System.Uri.EscapeDataString(ConvertToString(track, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get voters</summary>
        /// <param name="limit">limit of [n] results per page</param>
        /// <param name="skip">skip [n] results</param>
        /// <param name="producer">filter by voted producer (comma separated)</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Response14> Get_votersAsync(int? limit = null, int? skip = null, string producer = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/state/get_voters?");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (skip != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("skip") + "=").Append(System.Uri.EscapeDataString(ConvertToString(skip, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (producer != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("producer") + "=").Append(System.Uri.EscapeDataString(ConvertToString(producer, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response14>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        
        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
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
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public partial class StatsClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private System.Net.Http.HttpClient _httpClient;

        public StatsClient(System.Net.Http.HttpClient httpClient)
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
        public async System.Threading.Tasks.Task Get_action_usageAsync(string period, string end_date = null, bool? unique_actors = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (period == null)
                throw new System.ArgumentNullException("period");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/stats/get_action_usage?" + System.Uri.EscapeDataString("period") + "=").Append(System.Uri.EscapeDataString(ConvertToString(period, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (end_date != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("end_date") + "=").Append(System.Uri.EscapeDataString(ConvertToString(end_date, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (unique_actors != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("unique_actors") + "=").Append(System.Uri.EscapeDataString(ConvertToString(unique_actors, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task<Response15> Get_missed_blocksAsync(string producer = null, string after = null, string before = null, int? min_blocks = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/stats/get_missed_blocks?");
            if (producer != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("producer") + "=").Append(System.Uri.EscapeDataString(ConvertToString(producer, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (after != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("after") + "=").Append(System.Uri.EscapeDataString(ConvertToString(after, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (before != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("before") + "=").Append(System.Uri.EscapeDataString(ConvertToString(before, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (min_blocks != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("min_blocks") + "=").Append(System.Uri.EscapeDataString(ConvertToString(min_blocks, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");
                request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Response15>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    return objectResponse_.Object;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>get resource usage stats for a specific action</summary>
        /// <param name="code">contract</param>
        /// <param name="action">action name</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_resource_usageAsync(string code, string action, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            if (action == null)
                throw new System.ArgumentNullException("action");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v2/stats/get_resource_usage?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("action") + "=").Append(System.Uri.EscapeDataString(ConvertToString(action, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        
        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
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
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public partial class ChainClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private System.Net.Http.HttpClient _httpClient;

        public ChainClient(System.Net.Http.HttpClient httpClient)
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
        public async System.Threading.Tasks.Task Abi_bin_to_jsonGetAsync(Name code, Name action, string binargs, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            if (action == null)
                throw new System.ArgumentNullException("action");
    
            if (binargs == null)
                throw new System.ArgumentNullException("binargs");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_bin_to_json?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("action") + "=").Append(System.Uri.EscapeDataString(ConvertToString(action, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("binargs") + "=").Append(System.Uri.EscapeDataString(ConvertToString(binargs, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Abi_bin_to_jsonHeadAsync(Name code, Name action, string binargs, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            if (action == null)
                throw new System.ArgumentNullException("action");
    
            if (binargs == null)
                throw new System.ArgumentNullException("binargs");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_bin_to_json?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("action") + "=").Append(System.Uri.EscapeDataString(ConvertToString(action, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("binargs") + "=").Append(System.Uri.EscapeDataString(ConvertToString(binargs, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Abi_bin_to_jsonPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_bin_to_json");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Convert JSON object to binary</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Abi_json_to_binGetAsync(string binargs, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (binargs == null)
                throw new System.ArgumentNullException("binargs");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_json_to_bin?" + System.Uri.EscapeDataString("binargs") + "=").Append(System.Uri.EscapeDataString(ConvertToString(binargs, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Convert JSON object to binary</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Abi_json_to_binHeadAsync(string binargs, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (binargs == null)
                throw new System.ArgumentNullException("binargs");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_json_to_bin?" + System.Uri.EscapeDataString("binargs") + "=").Append(System.Uri.EscapeDataString(ConvertToString(binargs, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Convert JSON object to binary</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Abi_json_to_binPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/abi_json_to_bin");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the ABI for a contract based on its account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_abiGetAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_abi?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the ABI for a contract based on its account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_abiHeadAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_abi?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the ABI for a contract based on its account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_abiPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_abi");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific account on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_accountGetAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_account?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific account on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_accountHeadAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_account?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific account on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_accountPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_account");
 
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");

                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                    var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            return;
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_activated_protocol_featuresGetAsync(int? lower_bound = null, int? upper_bound = null, int? limit = null, bool? search_by_block_num = null, bool? reverse = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_activated_protocol_features?");
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (upper_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("upper_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(upper_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (search_by_block_num != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("search_by_block_num") + "=").Append(System.Uri.EscapeDataString(ConvertToString(search_by_block_num, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("reverse") + "=").Append(System.Uri.EscapeDataString(ConvertToString(reverse, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_activated_protocol_featuresHeadAsync(int? lower_bound = null, int? upper_bound = null, int? limit = null, bool? search_by_block_num = null, bool? reverse = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_activated_protocol_features?");
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (upper_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("upper_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(upper_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (search_by_block_num != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("search_by_block_num") + "=").Append(System.Uri.EscapeDataString(ConvertToString(search_by_block_num, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("reverse") + "=").Append(System.Uri.EscapeDataString(ConvertToString(reverse, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retreives the activated protocol features for producer node</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_activated_protocol_featuresPostAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_activated_protocol_features");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific block on the blockchain.</summary>
        /// <param name="block_num_or_id">Provide a `block number` or a `block id`</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_blockGetAsync(string block_num_or_id, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (block_num_or_id == null)
                throw new System.ArgumentNullException("block_num_or_id");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block?" + System.Uri.EscapeDataString("block_num_or_id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(block_num_or_id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific block on the blockchain.</summary>
        /// <param name="block_num_or_id">Provide a `block number` or a `block id`</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_blockHeadAsync(string block_num_or_id, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (block_num_or_id == null)
                throw new System.ArgumentNullException("block_num_or_id");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block?" + System.Uri.EscapeDataString("block_num_or_id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(block_num_or_id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about a specific block on the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_blockPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the block header state</summary>
        /// <param name="block_num_or_id">Provide a block_number or a block_id</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_block_header_stateGetAsync(string block_num_or_id, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (block_num_or_id == null)
                throw new System.ArgumentNullException("block_num_or_id");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block_header_state?" + System.Uri.EscapeDataString("block_num_or_id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(block_num_or_id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the block header state</summary>
        /// <param name="block_num_or_id">Provide a block_number or a block_id</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_block_header_stateHeadAsync(string block_num_or_id, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (block_num_or_id == null)
                throw new System.ArgumentNullException("block_num_or_id");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block_header_state?" + System.Uri.EscapeDataString("block_num_or_id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(block_num_or_id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the block header state</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_block_header_statePostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_block_header_state");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves contract code</summary>
        /// <param name="code_as_wasm">This must be 1 (true)</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_codeGetAsync(Name account_name, int code_as_wasm, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            if (code_as_wasm == null)
                throw new System.ArgumentNullException("code_as_wasm");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_code?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("code_as_wasm") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code_as_wasm, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves contract code</summary>
        /// <param name="code_as_wasm">This must be 1 (true)</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_codeHeadAsync(Name account_name, int code_as_wasm, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            if (code_as_wasm == null)
                throw new System.ArgumentNullException("code_as_wasm");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_code?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("code_as_wasm") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code_as_wasm, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves contract code</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_codePostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_code");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the current balance</summary>
        /// <param name="symbol">A symbol composed of capital letters between 1-7.</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_currency_balanceGetAsync(Name code, Name account, string symbol, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            if (account == null)
                throw new System.ArgumentNullException("account");
    
            if (symbol == null)
                throw new System.ArgumentNullException("symbol");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_balance?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("symbol") + "=").Append(System.Uri.EscapeDataString(ConvertToString(symbol, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the current balance</summary>
        /// <param name="symbol">A symbol composed of capital letters between 1-7.</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_currency_balanceHeadAsync(Name code, Name account, string symbol, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            if (account == null)
                throw new System.ArgumentNullException("account");
    
            if (symbol == null)
                throw new System.ArgumentNullException("symbol");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_balance?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("account") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("symbol") + "=").Append(System.Uri.EscapeDataString(ConvertToString(symbol, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the current balance</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_currency_balancePostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_balance");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves currency stats</summary>
        /// <param name="code">contract name</param>
        /// <param name="symbol">token symbol</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_currency_statsGetAsync(string code, string symbol, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            if (symbol == null)
                throw new System.ArgumentNullException("symbol");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_stats?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("symbol") + "=").Append(System.Uri.EscapeDataString(ConvertToString(symbol, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves currency stats</summary>
        /// <param name="code">contract name</param>
        /// <param name="symbol">token symbol</param>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_currency_statsHeadAsync(string code, string symbol, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            if (symbol == null)
                throw new System.ArgumentNullException("symbol");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_stats?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&" + System.Uri.EscapeDataString("symbol") + "=").Append(System.Uri.EscapeDataString(ConvertToString(symbol, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves currency stats</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_currency_statsPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_currency_stats");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_infoGetAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_info");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_infoHeadAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_info");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing various details about the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_infoPostAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_info");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_producersGetAsync(string limit = null, string lower_bound = null, bool? json = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_producers?");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("json") + "=").Append(System.Uri.EscapeDataString(ConvertToString(json, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_producersHeadAsync(string limit = null, string lower_bound = null, bool? json = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_producers?");
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("json") + "=").Append(System.Uri.EscapeDataString(ConvertToString(json, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves producers list</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_producersPostAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_producers");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_raw_abiGetAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_abi?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_raw_abiHeadAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_abi?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_raw_abiPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_abi");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw code and ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_raw_code_and_abiGetAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_code_and_abi?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw code and ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_raw_code_and_abiHeadAsync(Name account_name, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (account_name == null)
                throw new System.ArgumentNullException("account_name");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_code_and_abi?" + System.Uri.EscapeDataString("account_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(account_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves raw code and ABI for a contract based on account name</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_raw_code_and_abiPostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_raw_code_and_abi");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_scheduled_transactionGetAsync(string lower_bound = null, int? limit = null, bool? json = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_scheduled_transaction?");
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("json") + "=").Append(System.Uri.EscapeDataString(ConvertToString(json, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_scheduled_transactionHeadAsync(string lower_bound = null, int? limit = null, bool? json = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_scheduled_transaction?");
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (json != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("json") + "=").Append(System.Uri.EscapeDataString(ConvertToString(json, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves the scheduled transaction</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_scheduled_transactionPostAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_scheduled_transaction");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_table_by_scopeGetAsync(string code, string table = null, string lower_bound = null, string upper_bound = null, int? limit = null, bool? reverse = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_by_scope?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (table != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("table") + "=").Append(System.Uri.EscapeDataString(ConvertToString(table, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (upper_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("upper_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(upper_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("reverse") + "=").Append(System.Uri.EscapeDataString(ConvertToString(reverse, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_table_by_scopeHeadAsync(string code, string table = null, string lower_bound = null, string upper_bound = null, int? limit = null, bool? reverse = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (code == null)
                throw new System.ArgumentNullException("code");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_by_scope?" + System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            if (table != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("table") + "=").Append(System.Uri.EscapeDataString(ConvertToString(table, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (upper_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("upper_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(upper_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (limit != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("limit") + "=").Append(System.Uri.EscapeDataString(ConvertToString(limit, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (reverse != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("reverse") + "=").Append(System.Uri.EscapeDataString(ConvertToString(reverse, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieves table scope</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_table_by_scopePostAsync(object body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (body == null)
                throw new System.ArgumentNullException("body");
    
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_by_scope");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_table_rowsGetAsync(string code = null, string table = null, string scope = null, string index_position = null, string key_type = null, string encode_type = null, string upper_bound = null, string lower_bound = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_rows?");
            if (code != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (table != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("table") + "=").Append(System.Uri.EscapeDataString(ConvertToString(table, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (scope != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("scope") + "=").Append(System.Uri.EscapeDataString(ConvertToString(scope, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (index_position != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("index_position") + "=").Append(System.Uri.EscapeDataString(ConvertToString(index_position, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (key_type != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("key_type") + "=").Append(System.Uri.EscapeDataString(ConvertToString(key_type, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (encode_type != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("encode_type") + "=").Append(System.Uri.EscapeDataString(ConvertToString(encode_type, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (upper_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("upper_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(upper_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        public async System.Threading.Tasks.Task Get_table_rowsHeadAsync(string code = null, string table = null, string scope = null, string index_position = null, string key_type = null, string encode_type = null, string upper_bound = null, string lower_bound = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_rows?");
            if (code != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("code") + "=").Append(System.Uri.EscapeDataString(ConvertToString(code, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (table != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("table") + "=").Append(System.Uri.EscapeDataString(ConvertToString(table, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (scope != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("scope") + "=").Append(System.Uri.EscapeDataString(ConvertToString(scope, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (index_position != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("index_position") + "=").Append(System.Uri.EscapeDataString(ConvertToString(index_position, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (key_type != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("key_type") + "=").Append(System.Uri.EscapeDataString(ConvertToString(key_type, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (encode_type != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("encode_type") + "=").Append(System.Uri.EscapeDataString(ConvertToString(encode_type, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (upper_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("upper_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(upper_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (lower_bound != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("lower_bound") + "=").Append(System.Uri.EscapeDataString(ConvertToString(lower_bound, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("HEAD");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Returns an object containing rows from the specified table.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Get_table_rowsPostAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/get_table_rows");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This method expects a transaction in JSON format and will attempt to apply it to the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Push_transactionAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/push_transaction");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This method expects a transaction in JSON format and will attempt to apply it to the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Push_transactionsAsync(System.Collections.Generic.IEnumerable<object> body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/push_transactions");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>This method expects a transaction in JSON format and will attempt to apply it to the blockchain.</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task Send_transactionAsync(object body = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/send_transaction");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                request_.Content = content_;
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Wildcard chain api handler</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task GetAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/*");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
        }
    
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Wildcard chain api handler</summary>
        /// <returns>Default Response</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task PostAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/chain/*");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                request_.Method = new System.Net.Http.HttpMethod("POST");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        
        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
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
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }
    
    
    public partial class InternalClient 
    {
        private string _baseUrl = "https://api.wax.liquidstudios.io/";
        private System.Net.Http.HttpClient _httpClient;

        public InternalClient(System.Net.Http.HttpClient httpClient)
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
        public async System.Threading.Tasks.Task Stream_httpClientjsAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            var urlBuilder_ = new System.Text.StringBuilder(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/stream-client.js");
 
            using (var request_ = new System.Net.Http.HttpRequestMessage())
            {
                request_.Method = new System.Net.Http.HttpMethod("GET");

                var url_ = urlBuilder_.ToString();
                request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                var response_ = await _httpClient.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                var status_ = (int)response_.StatusCode;
                if (status_ == 200)
                {
                    return;
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
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
        
        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
        
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
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
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var typedBody = JsonConvert.DeserializeObject<T>(jsonTextReader.ToString());
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }
        
            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
        
                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }

    /// <summary>sort direction</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v11.0.0.0)")]
    public enum Sort
    {
        [System.Runtime.Serialization.EnumMember(Value = @"desc")]
        Desc = 0,
    
        [System.Runtime.Serialization.EnumMember(Value = @"asc")]
        Asc = 1,
    
        [System.Runtime.Serialization.EnumMember(Value = @"1")]
        _1 = 2,
    
        [System.Runtime.Serialization.EnumMember(Value = @"-1")]
        Minus1 = 3,
    
    }

    public partial class Body 
    {
        /// <summary>public key</summary>
        [Newtonsoft.Json.JsonProperty("public_key", Required = Newtonsoft.Json.Required.Always)]
        public string Public_key { get; set; }
    }

    public partial class Name 
    {

    
    
    }

    public partial class Response 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("simple_actions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Simple_actions> Simple_actions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("actions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Actions> Actions { get; set; }
    

    
    
    }

    public partial class Response2 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total2 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("deltas", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Deltas> Deltas { get; set; }
    }

    public partial class Response3 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total3 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Version { get; set; }
    
        [Newtonsoft.Json.JsonProperty("producers", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Producers> Producers { get; set; }
    }

    public partial class Response4 
    {
        [Newtonsoft.Json.JsonProperty("query_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time { get; set; }
    
        [Newtonsoft.Json.JsonProperty("last_irreversible_block", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Last_irreversible_block { get; set; }
    
        [Newtonsoft.Json.JsonProperty("actions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Actions2> Actions { get; set; }
    }

    public partial class Response5 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total4 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("number", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Number { get; set; }
    
        [Newtonsoft.Json.JsonProperty("previous_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Previous_id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Status { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("producer", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Producer { get; set; }
    
        [Newtonsoft.Json.JsonProperty("transactions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Transactions> Transactions { get; set; }
    }

    public partial class Response6 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total5 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("query_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time { get; set; }
    
        [Newtonsoft.Json.JsonProperty("accounts", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Accounts> Accounts { get; set; }
    }

    public partial class Response7 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total6 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("creator", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Creator { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("trx_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Trx_id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("indirect_creator", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Indirect_creator { get; set; }
    }

    public partial class Response8 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total7 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("links", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Links> Links { get; set; }
    
        [Newtonsoft.Json.JsonProperty("tokens", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Tokens> Tokens { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total_actions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Total_actions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("actions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Actions3> Actions { get; set; }
    }

    public partial class Response9 
    {
        [Newtonsoft.Json.JsonProperty("account_names", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Account_names { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permissions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Permissions> Permissions { get; set; }
    }

    public partial class Response10 
    {
        [Newtonsoft.Json.JsonProperty("account_names", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Account_names { get; set; }
    }

    public partial class Response11 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total8 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("links", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Links2> Links { get; set; }
    }

    public partial class Response12 
    {
        [Newtonsoft.Json.JsonProperty("controlled_accounts", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Controlled_accounts { get; set; }
    }

    public partial class Response13 
    {
        [Newtonsoft.Json.JsonProperty("account_names", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Account_names { get; set; }
    }

    public partial class Response14 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total9 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("voters", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Voters> Voters { get; set; }
    }

    public partial class Response15 
    {
        [Newtonsoft.Json.JsonProperty("query_time_ms", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Query_time_ms { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cached", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Cached { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hot_only", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Hot_only { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lib", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Lib { get; set; }
    
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Total10 Total { get; set; }
    
        [Newtonsoft.Json.JsonProperty("stats", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Stats Stats { get; set; }
    
        [Newtonsoft.Json.JsonProperty("events", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Events> Events { get; set; }
    }

    public partial class Total 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Simple_actions 
    {
        [Newtonsoft.Json.JsonProperty("block", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("irreversible", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Irreversible { get; set; }
    
        [Newtonsoft.Json.JsonProperty("contract", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Contract { get; set; }
    
        [Newtonsoft.Json.JsonProperty("action", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Action { get; set; }
    
        [Newtonsoft.Json.JsonProperty("actors", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Actors { get; set; }
    
        [Newtonsoft.Json.JsonProperty("notified", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Notified { get; set; }
    
        [Newtonsoft.Json.JsonProperty("transaction_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Transaction_id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public partial class Actions 
    {
        [Newtonsoft.Json.JsonProperty("@timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("trx_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Trx_id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("act", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Act Act { get; set; }
    
        [Newtonsoft.Json.JsonProperty("notified", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Notified { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cpu_usage_us", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Cpu_usage_us { get; set; }
    
        [Newtonsoft.Json.JsonProperty("net_usage_words", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Net_usage_words { get; set; }
    
        [Newtonsoft.Json.JsonProperty("account_ram_deltas", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Account_ram_deltas> Account_ram_deltas { get; set; }
    
        [Newtonsoft.Json.JsonProperty("global_sequence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Global_sequence { get; set; }
    
        [Newtonsoft.Json.JsonProperty("receiver", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [Newtonsoft.Json.JsonProperty("producer", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Producer { get; set; }
    
        [Newtonsoft.Json.JsonProperty("parent", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Parent { get; set; }
    
        [Newtonsoft.Json.JsonProperty("action_ordinal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Action_ordinal { get; set; }
    
        [Newtonsoft.Json.JsonProperty("creator_action_ordinal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Creator_action_ordinal { get; set; }
    }

    public partial class Total2 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Deltas 
    {
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("code", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Code { get; set; }
    
        [Newtonsoft.Json.JsonProperty("scope", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Scope { get; set; }
    
        [Newtonsoft.Json.JsonProperty("table", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Table { get; set; }
    
        [Newtonsoft.Json.JsonProperty("primary_key", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Primary_key { get; set; }
    
        [Newtonsoft.Json.JsonProperty("payer", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Payer { get; set; }
    
        [Newtonsoft.Json.JsonProperty("present", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Present { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Block_id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public partial class Total3 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Producers 
    {
        [Newtonsoft.Json.JsonProperty("producer_name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Producer_name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_signing_key", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Block_signing_key { get; set; }
    
        [Newtonsoft.Json.JsonProperty("legacy_key", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Legacy_key { get; set; }
    }

    public partial class Actions2 
    {
        [Newtonsoft.Json.JsonProperty("account_action_seq", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Account_action_seq { get; set; }
    
        [Newtonsoft.Json.JsonProperty("global_action_seq", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Global_action_seq { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Block_time { get; set; }
    
        [Newtonsoft.Json.JsonProperty("action_trace", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Action_trace Action_trace { get; set; }
    }

    public partial class Total4 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Transactions 
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("actions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Actions4> Actions { get; set; }
    }

    public partial class Total5 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Accounts 
    {
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("trx_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Trx_id { get; set; }
    }

    public partial class Total6 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Total7 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Links 
    {
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permission", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Permission { get; set; }
    
        [Newtonsoft.Json.JsonProperty("code", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Code { get; set; }
    
        [Newtonsoft.Json.JsonProperty("action", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Action { get; set; }
    }

    public partial class Tokens 
    {
        [Newtonsoft.Json.JsonProperty("symbol", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Symbol { get; set; }
    
        [Newtonsoft.Json.JsonProperty("precision", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Precision { get; set; }
    
        [Newtonsoft.Json.JsonProperty("amount", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Amount { get; set; }
    
        [Newtonsoft.Json.JsonProperty("contract", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Contract { get; set; }
    }

    public partial class Actions3 
    {
        [Newtonsoft.Json.JsonProperty("@timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("trx_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Trx_id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("act", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Act2 Act { get; set; }
    
        [Newtonsoft.Json.JsonProperty("notified", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<string> Notified { get; set; }
    
        [Newtonsoft.Json.JsonProperty("cpu_usage_us", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Cpu_usage_us { get; set; }
    
        [Newtonsoft.Json.JsonProperty("net_usage_words", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Net_usage_words { get; set; }
    
        [Newtonsoft.Json.JsonProperty("account_ram_deltas", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Account_ram_deltas2> Account_ram_deltas { get; set; }
    
        [Newtonsoft.Json.JsonProperty("global_sequence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Global_sequence { get; set; }
    
        [Newtonsoft.Json.JsonProperty("receiver", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [Newtonsoft.Json.JsonProperty("producer", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Producer { get; set; }
    
        [Newtonsoft.Json.JsonProperty("parent", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Parent { get; set; }
    
        [Newtonsoft.Json.JsonProperty("action_ordinal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Action_ordinal { get; set; }
    
        [Newtonsoft.Json.JsonProperty("creator_action_ordinal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Creator_action_ordinal { get; set; }
    }

    public partial class Permissions 
    {
        [Newtonsoft.Json.JsonProperty("owner", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Owner { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("parent", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Parent { get; set; }
    
        [Newtonsoft.Json.JsonProperty("last_updated", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Last_updated { get; set; }
    
        [Newtonsoft.Json.JsonProperty("auth", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Auth { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("present", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Present { get; set; }
    }

    public partial class Total8 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Links2 
    {
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permission", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Permission { get; set; }
    
        [Newtonsoft.Json.JsonProperty("code", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Code { get; set; }
    
        [Newtonsoft.Json.JsonProperty("action", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Action { get; set; }
    
        [Newtonsoft.Json.JsonProperty("irreversible", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool Irreversible { get; set; }
    }

    public partial class Total9 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Voters 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("weight", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Weight { get; set; }
    
        [Newtonsoft.Json.JsonProperty("last_vote", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Last_vote { get; set; }
    
        [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public partial class Total10 
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Value { get; set; }
    
        [Newtonsoft.Json.JsonProperty("relation", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Relation { get; set; }
    }

    public partial class Stats 
    {
        [Newtonsoft.Json.JsonProperty("by_producer", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object By_producer { get; set; }
    }

    public partial class Events 
    {
        [Newtonsoft.Json.JsonProperty("@timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    
        [Newtonsoft.Json.JsonProperty("last_block", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Last_block { get; set; }
    
        [Newtonsoft.Json.JsonProperty("schedule_version", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Schedule_version { get; set; }
    
        [Newtonsoft.Json.JsonProperty("size", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Size { get; set; }
    
        [Newtonsoft.Json.JsonProperty("producer", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Producer { get; set; }
    }

    public partial class Act 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public partial class Account_ram_deltas 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("delta", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Delta { get; set; }
    }

    public partial class Action_trace 
    {
        [Newtonsoft.Json.JsonProperty("action_ordinal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Action_ordinal { get; set; }
    
        [Newtonsoft.Json.JsonProperty("creator_action_ordinal", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Creator_action_ordinal { get; set; }
    
        [Newtonsoft.Json.JsonProperty("receipt", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Receipt Receipt { get; set; }
    
        [Newtonsoft.Json.JsonProperty("receiver", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [Newtonsoft.Json.JsonProperty("act", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Act3 Act { get; set; }
    
        [Newtonsoft.Json.JsonProperty("trx_id", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Trx_id { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_num", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Block_num { get; set; }
    
        [Newtonsoft.Json.JsonProperty("block_time", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Block_time { get; set; }
    }

    public partial class Actions4 
    {
        [Newtonsoft.Json.JsonProperty("receiver", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("action", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Action { get; set; }
    
        [Newtonsoft.Json.JsonProperty("authorization", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Authorization> Authorization { get; set; }
    
        [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Data { get; set; }
    }

    public partial class Act2 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public partial class Account_ram_deltas2 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("delta", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Delta { get; set; }
    }

    public partial class Receipt 
    {
        [Newtonsoft.Json.JsonProperty("receiver", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Receiver { get; set; }
    
        [Newtonsoft.Json.JsonProperty("global_sequence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Global_sequence { get; set; }
    
        [Newtonsoft.Json.JsonProperty("recv_sequence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Recv_sequence { get; set; }
    
        [Newtonsoft.Json.JsonProperty("auth_sequence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<Auth_sequence> Auth_sequence { get; set; }
    }

    public partial class Act3 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; set; }
    
        [Newtonsoft.Json.JsonProperty("authorization", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.ICollection<object> Authorization { get; set; }
    
        [Newtonsoft.Json.JsonProperty("data", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public object Data { get; set; }
    
        [Newtonsoft.Json.JsonProperty("hex_data", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Hex_data { get; set; }
    }

    public partial class Authorization 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("permission", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Permission { get; set; }
    }

    public partial class Auth_sequence 
    {
        [Newtonsoft.Json.JsonProperty("account", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Account { get; set; }
    
        [Newtonsoft.Json.JsonProperty("sequence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double Sequence { get; set; }
    }

    
    public partial class ApiException : System.Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
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

    
    public partial class ApiException<TResult> : ApiException
    {
        public TResult Result { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

}