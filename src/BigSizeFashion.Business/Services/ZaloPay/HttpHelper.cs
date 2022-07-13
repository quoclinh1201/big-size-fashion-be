﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BigSizeFashion.Business.Services.ZaloPay
{
    public class HttpHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<T> PostAsync<T>(string uri, HttpContent content)
        {
            var response = await httpClient.PostAsync(uri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static Task<Dictionary<string, object>> PostAsync(string uri, HttpContent content)
        {
            return PostAsync<Dictionary<string, object>>(uri, content);
        }

        public static Task<T> PostFormAsync<T>(string uri, Dictionary<string, string> data)
        {
            return PostAsync<T>(uri, new FormUrlEncodedContent(data));
        }

        public static Task<Dictionary<string, object>> PostFormAsync(string uri, Dictionary<string, string> data)
        {
            return PostFormAsync<Dictionary<string, object>>(uri, data);
        }

        public static async Task<T> GetAsync<T>(string uri)
        {
            var response = await httpClient.GetAsync(uri);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static Task<T> GetFormAsync<T>(string uri)
        {
            return GetAsync<T>(uri);
        }

        public static Task<Dictionary<string, object>> GetFormAsync(string uri)
        {
            return GetFormAsync<Dictionary<string, object>>(uri);
        }

        public static async Task<T> GetJson<T>(string uri)
        {
            var response = await httpClient.GetAsync(uri);
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static Task<Dictionary<string, object>> GetJson(string uri)
        {
            return GetJson<Dictionary<string, object>>(uri);
        }
    }
}
