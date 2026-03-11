using DataPlatform.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using DataPlatform.Models.APIClass;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace DataPlatform.API
{
    public static class HTTPAPI
    {
        private static HttpListener httpListener;
        private const string _prefix = "http://*:28080/writeData/";
        public static event Func<writeDataClass, httpRecClass> OnDataWriteEvent;
        /// <summary>
        /// 启动HTTP监听器
        /// </summary>
        public static void Start(IEnumerable<string> prefixes)
        {
            if (prefixes.Count() <= 0) return;
            try
            {
                httpListener = new HttpListener();
                foreach (var item in prefixes)
                {
                    httpListener.Prefixes.Add("item");
                }
                httpListener.Start();
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("HTTP监听器启动失败", ex);
                return;
            }
            httpListener.BeginGetContext(ProcessRequest, null);
        }

        private static void ProcessRequest(IAsyncResult ar)
        {
            var content = httpListener.EndGetContext(ar);
            httpListener.BeginGetContext(ProcessRequest, null);
            var request = content.Request;
            var response = content.Response;
            if (request.HttpMethod == "POST" && request.InputStream != null)
            {
                var data = GetData(request);
                //分析调用的哪个接口
                string methodName = request.RawUrl.Replace("/", "");
                switch (methodName)
                {
                    case "writeData":
                        writeData(data, response, request);
                        break;
                    default:
                        NoFoundMethod(methodName, response, request);
                        break;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
            }
        }

        private static void writeData(string data, HttpListenerResponse response, HttpListenerRequest request)
        {
            try
            {
                var h = JsonConvert.DeserializeObject<writeDataClass>(data);
                httpRecClass rcv = new httpRecClass()
                {
                    code = 301,
                    msg = "写入过程出现未知错误",
                };
                if (h != null)
                {
                    rcv = OnDataWriteEvent?.Invoke(h);
                }
                else
                {
                    rcv.code = 500;
                    rcv.msg = $"无法解析的数据内容[{data}]";
                }
                var responseBody = JsonConvert.SerializeObject(rcv);
                var bytes = Encoding.UTF8.GetBytes(responseBody);
                response.ContentType = request.ContentType;
                using (var output = response.OutputStream)
                {
                    output.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("数据写入接口异常。", ex);
            }
        }

        private static void NoFoundMethod(string methodName, HttpListenerResponse response, HttpListenerRequest request)
        {
            try
            {
                httpRecClass rcv = new httpRecClass()
                {
                    code = 404,
                    msg = "未知的接口地址",
                };
                var responseBody = JsonConvert.SerializeObject(rcv);
                var bytes = Encoding.UTF8.GetBytes(responseBody);
                response.ContentType = request.ContentType;
                using (var output = response.OutputStream)
                {
                    output.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("未知的接口数据。接口地址：[" + methodName + "]", ex);
            }
        }


        static string GetData(HttpListenerRequest request)
        {
            using (StreamReader sr = new StreamReader(request.InputStream))
            {
                var requestBody = sr.ReadToEnd();
                return requestBody;
            }
        }
    }
}
