﻿using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public static class HttpClientExtensions
    {
        public static async Task<string> GetRestJsonAsync(
            this HttpClient client, string requestPath)
        {
            var response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }


        public static async Task<byte[]> GetImageRestAsync(
            this HttpClient client, string requestPath)
        {
            var response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return default;
        }

        public static async Task<TPayload> GetRestAsync<TPayload>(
            this HttpClient client, string requestPath,
            Action<HttpResponseMessage> processResponse = null)
        {

            var response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                processResponse?.Invoke(response);
                return await response.Content.ReadContentAsync<TPayload>();
            }

            return default(TPayload);
        }

        public static async Task<bool> DeleteRestAsync(
            this HttpClient client, string requestPath)
        {

            var response = await client.DeleteAsync(requestPath);

            return response.IsSuccessStatusCode;

        }

        public static async Task<string> PutRestAsync(
            this HttpClient client, string requestPath, object payload)
        {
            return await PutRestAsync(client, requestPath, JsonConvert.SerializeObject(payload));
        }

        public static async Task<string> PutRestAsync(
          this HttpClient client, string requestPath, string payload)
        {


            var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestPath, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }


            return string.Empty;
        }

        public static async Task<TResponsePayload> PutRestAsync<TRequestPayload, TResponsePayload>(
           this HttpClient client, string requestPath, TRequestPayload payload)
        {
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


            return default;
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
         this HttpClient client, string requestPath, object payload)
        {
            var plString = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return await PatchRestAsync(client, requestPath, plString);
        }

        public static async Task<string> PatchRestAsync(this HttpClient client, string requestPath, string payload)
        {
            var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await PatchAsync(client, requestPath, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

        public static async Task<string> PostRestAsync(this HttpClient client, string requestPath, object payload)
        {
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
            return string.Empty;
        }

        public static async Task<TResponseType> PostRestAsync<TResponseType>(
         this HttpClient client, string requestPath, object payload)
        {
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
            return default;
        }

        public static async Task<bool> PostWithoutResponseBodyAsync(this HttpClient client,
            string requestPath, object payload)
        {
            var jsonString = JsonConvert.SerializeObject(payload,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestPath, jsonContent);
            //var contentString = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode;
        }

        public static async Task<TPayload> ReadContentAsync<TPayload>(this HttpContent content)
        {
            //var contentString = await content.ReadAsStringAsync();
            //return JsonConvert.DeserializeObject<TPayload>(contentString);
            return await content.ReadAsAsync<TPayload>();            
        }
    }
}
