using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace Cosmos
{
    public class HttpServer
    {
        public HttpServer(string page)
        {
            Page = page;
        }
        public string Page { get; private set; }
        HttpListener httpListener;
        Func<string, string> onReceive;
        public event Func<string, string> OnReceive
        {
            add { onReceive += value; }
            remove { onReceive -= value; }
        }
        public void Run()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(Page);
            httpListener.Start();
            Utility.Debug.LogInfo("Http server start running");
            HandlePost();
        }
        async void HandlePost()
        {
            HttpListenerContext context = await httpListener.GetContextAsync();
            var reqData = context.Request.InputStream;
            var iptStream = context.Request.InputStream;
            byte[] readData = new byte[context.Request.ContentLength64];
            await iptStream.ReadAsync(readData,0, readData.Length);
            var readStr = Encoding.UTF8.GetString(readData);
            var responseData = onReceive?.Invoke(readStr);
            Stream optStream = context.Response.OutputStream;
            if (!string.IsNullOrEmpty(responseData))
            {
                var buffers = Encoding.UTF8.GetBytes(responseData);
                await optStream.WriteAsync(buffers, 0, responseData.Length);
                optStream.Close();
            }
            HandlePost();
        }
    }
}
