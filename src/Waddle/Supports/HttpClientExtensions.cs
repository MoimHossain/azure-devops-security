

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Waddle.Supports
{
    public static class HttpClientExtensions
    {
        public static async Task<string> GetRestJsonAsync(
            this Uri baseAddress, string requestPath, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);
                var response = await client.GetAsync(requestPath);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            return default(string);
        }


        public static async Task<byte[]> GetImageRestAsync(
            this Uri baseAddress, string requestPath, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);
                var response = await client.GetAsync(requestPath);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }

            return default(byte[]);
        }

        public static async Task<TPayload> GetRestAsync<TPayload>(
            this Uri baseAddress, string requestPath, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);
                var response = await client.GetAsync(requestPath);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadContentAsync<TPayload>();
                }
            }

            return default(TPayload);
        }

        public static async Task<bool> DeleteRestAsync(
            this Uri baseAddress, string requestPath, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);

                var response = await client.DeleteAsync(requestPath);

                return response.IsSuccessStatusCode;
            }
        }

        public static async Task<string> PutRestAsync(
            this Uri baseAddress, string requestPath, object payload, Action<HttpClient> configureClient)
        {
            return await PutRestAsync(baseAddress, requestPath, JsonConvert.SerializeObject(payload), configureClient);
        }

        public static async Task<string> PutRestAsync(
          this Uri baseAddress, string requestPath, string payload, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);

                var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(requestPath, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return string.Empty;
        }

        public static async Task<TResponsePayload> PutRestAsync<TRequestPayload, TResponsePayload>(
           this Uri baseAddress, string requestPath, TRequestPayload payload, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);
                var jsonString = JsonConvert.SerializeObject(payload,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(requestPath, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadContentAsync<TResponsePayload>();
                }
            }

            return default(TResponsePayload);
        }

        public static async Task<HttpResponseMessage> PatchAsync(
            HttpClient client, string requestUri, StringContent iContent)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent
            };

            var response = new HttpResponseMessage();

            response = await client.SendAsync(request);

            return response;
        }

        public static async Task<string> PatchRestAsync(
         this Uri baseAddress, string requestPath, string payload, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);

                var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await PatchAsync(client, requestPath, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return string.Empty;
        }

        public static async Task<string> PostRestAsync(
         this Uri baseAddress, string requestPath, object payload, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);

                var jsonString = string.Empty;
                if (payload is string)
                {
                    jsonString = payload.ToString();
                }
                else
                {
                    jsonString = JsonConvert.SerializeObject(payload,
                   new JsonSerializerSettings
                   {
                       ContractResolver = new CamelCasePropertyNamesContractResolver()
                   });
                }


                var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestPath, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return string.Empty;
        }

        public static async Task<TResponseType> PostRestAsync<TResponseType>(
         this Uri baseAddress, string requestPath, object payload, Action<HttpClient> configureClient)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = baseAddress;
                configureClient(client);
                var jsonString = JsonConvert.SerializeObject(payload,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestPath, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadContentAsync<TResponseType>();
                }
            }

            return default(TResponseType);
        }

        public async static Task<HttpResponseMessage> PostJsonAsync(this Uri uri, object body, string bearer)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = uri
                };
                request.Headers.AcceptCharset.Clear();
                request.Headers.Accept.Clear();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));

                var content = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                return await client.SendAsync(request);
            }
        }

        public static async Task<TPayload> ReadContentAsync<TPayload>(this HttpContent content)
        {
            var contentString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TPayload>(contentString);
        }
    }
}
