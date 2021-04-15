using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.IO;

namespace Cosmos
{
    public class HttpClientPeer
    {
        HttpClient client;
        Func<string, string> onReceive;
        public event Func<string, string> OnReceive
        {
            add { onReceive += value; }
            remove { onReceive -= value; }
        }
        public HttpClientPeer(string page)
        {
            Page = page;
        }
        public string Page { get; private set; }
        public void Run()
        {
            client = new HttpClient();
            Utility.Debug.LogInfo("Http client start running");
        }
        public async void GetFile(string context, Action<byte[]> responeCallback)
        {
            HttpResponseMessage response = await client.GetAsync(Page + context);
            var repData= await response.Content.ReadAsByteArrayAsync();
            responeCallback?.Invoke(repData);
        }
        public async void Get(string context, Action<string> responeCallback)
        {
            HttpResponseMessage response = await client.GetAsync(Page + context);
            var repData = await response.Content.ReadAsByteArrayAsync();
            var repStr = Encoding.UTF8.GetString(repData);
            responeCallback?.Invoke(repStr);
        }
        public async void Post(string context, Action<string> responeCallback)
        {
            StringContent stringContent = new StringContent(context);
            HttpResponseMessage response = await client.PostAsync(Page, stringContent);
            var repData = await response.Content.ReadAsByteArrayAsync();
            var repStr = Encoding.UTF8.GetString(repData);
            responeCallback?.Invoke(repStr);
        }
    }
}
