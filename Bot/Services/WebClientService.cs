using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class WebClientService
    {
        public WebClientService(Uri uri, string apiKey)
        {
            Uri = uri ?? new Uri("https://msging.net/messages");
            AuthorizationHeader = new AuthenticationHeaderValue("Key", apiKey);
        }

        private readonly Uri Uri;

        private readonly AuthenticationHeaderValue AuthorizationHeader;

        private const int MaxRetries = 3;

        public async Task<HttpResponseMessage> SendMessageAsync(object jsonMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SendAsync(Uri, HttpMethod.Post, jsonMessage, cancellationToken);
        }

        public async Task<HttpResponseMessage> SendMessageAsync(string jsonMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SendAsync(Uri, HttpMethod.Post, jsonMessage, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendAsync<T>(Uri uri, HttpMethod httpMethod, T payload, CancellationToken cancellationToken)
        {
            using (var webClient = GetWebClient(MaxRetries))
            {
                HttpContent content = null;

                if (payload != null)
                {

                    string json = await Task.Run(() => JsonConvert.SerializeObject(payload));
                    content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                }

                using (var request = new HttpRequestMessage
                {
                    Content = content,
                    RequestUri = uri,
                    Method = httpMethod
                })
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await webClient.SendAsync(request, cancellationToken);
                    if (response.IsSuccessStatusCode)
                        return response;

                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        private HttpClient GetWebClient(int retries)
        {
            var client = new HttpClient(new HttpRetryHandler(new HttpClientHandler(), retries));
            client.DefaultRequestHeaders.Authorization = AuthorizationHeader;
            return client;
        }
    }
}