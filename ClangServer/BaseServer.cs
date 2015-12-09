using System;
using NHttp;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace ClangServer
{
    public class BaseServer
    {
        private HttpServer _httpServer = new HttpServer();
        private Dictionary<string, Action<HttpRequest, HttpResponse>> _handlers = new Dictionary<string, Action<HttpRequest, HttpResponse>>();

        public BaseServer(ushort httpPort)
        {
            _httpServer.EndPoint = new IPEndPoint(IPAddress.Any, httpPort);
            _httpServer.RequestReceived += this.HandleRequest;

            var methods = this.GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttribute(typeof(RouteAttribute)) != null);
            foreach (MethodInfo method in methods)
            {
                RouteAttribute route = method.GetCustomAttribute<RouteAttribute>();
                _handlers.Add(route.Path, this.InvokeHandler(method));
            }
        }

        public void Start()
        {
            _httpServer.Start();
        }

        public void Stop()
        {
            _httpServer.Stop();
        }

        private Action<HttpRequest, HttpResponse> InvokeHandler(MethodInfo method)
        {
            Type paramType = method.GetParameters().First().ParameterType;
            return (req, res) =>
            {
                using (var streamReader = new StreamReader(req.InputStream))
                {
                    object reqParam = JsonConvert.DeserializeObject(streamReader.ReadToEnd(), paramType);
                    object respObj = method.Invoke(this, new object[] { reqParam });
                    using (var streamWriter = new StreamWriter(res.OutputStream))
                    {
                        string respJson = JsonConvert.SerializeObject(respObj);
                        streamWriter.Write(respJson);
                    }
                }
            };
        }

        private void HandleRequest(object self, HttpRequestEventArgs args)
        {
            var request = args.Request;
            var response = args.Response;

            try
            {
                Action<HttpRequest, HttpResponse> action;
                if (_handlers.TryGetValue(request.Path.ToString(), out action))
                {
                    action(request, response);
                }
                else
                {
                    args.Response.Status = "404 Not Found";
                    using (var streamWriter = new StreamWriter(response.OutputStream))
                    {
                        streamWriter.Write("Sorry, Not Found!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
