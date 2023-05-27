using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace socket_server
{
    internal class Program
    {
        static void Main(string[] args) // http

        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://127.0.0.1:8080/");
            server.Start();

            while(true)
            {
                var context = server.GetContext();
                Console.WriteLine("Request : " + context.Request.Url);
                var request = context.Request;
                string methodName = context.Request.Url.Segments[1].Replace("/", "");
                switch (methodName)
                {
                    case "helloworld": // API name
                        int test1 = 0;
                        break;
                    case "postTest":
                    case "postFolderInfo":
                        if (request.HasEntityBody) // POST의 경우 body가 존재
                        {
                            var body = request.InputStream;
                            var encoding = request.ContentEncoding;
                            var reader = new StreamReader(body, encoding);
                            if (request.ContentType != null)
                            {
                                Console.WriteLine("Client data content type {0}", request.ContentType); // 컨텐츠타입 application/json
                            }
                            Console.WriteLine("Client data content length {0}", request.ContentLength64);

                            Console.WriteLine("Start of data:");
                            string recv_string = reader.ReadToEnd(); // client의 json 요청 -> parsing
                            var jobjRecvStr = JObject.Parse(recv_string);
                            var folderNameStr = jobjRecvStr.GetValue("Folder_Name").ToString(); // Jobj에서 value get
                            var folderNameStr2 = (string)jobjRecvStr["Folder_Name"];
                            var fileListJArray = (JArray)jobjRecvStr["Files_List"]; // Jobj에서 JArray get
                            // String Array 타입의 JArray를 string Array로 변환
                            List<string> fileListStr = JsonConvert.DeserializeObject<List<string>>(fileListJArray.ToString()); 
                            Console.WriteLine(recv_string);


                            if(methodName == "postFolderInfo") // POST method 에 따른 분기 처리
                            {                                
                                DateTime time_now = DateTime.Now; // get current TIME string
                                string curTime =  time_now.ToString("yyyyMMdd_HHmmss"); // 현재시간.json 파일에 POST 로 받은 json 데이타 저장
                                string filePath = curTime + ".json";
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true))
                                {
                                    sw.WriteLine(recv_string);
                                    sw.Close();
                                }

                            }
                            Console.WriteLine("End of data:");
                            reader.Close();
                            body.Close();
                        }
                        break;

                    default:
                        break;

                }
                // response data
                //var file_array_json = new JArray([); // JARRAY로 변환
                var reqBody = new JObject();
                reqBody.Add("result", "ffffxxdeddc");
                //reqBody.Add("Files_List", file_array_json);
                string reqBodyStr = reqBody.ToString();               
                //var content = new StringContent(reqBodyStr, Encoding.UTF8, "application/json");
                ///
                byte[] content = Encoding.UTF8.GetBytes(reqBodyStr);
                context.Response.OutputStream.Write(content, 0, content.Length);
                context.Response.StatusCode = 200;
                context.Response.Close();

                //byte[] data = Encoding.UTF8.GetBytes("HelloWorld");
                //context.Response.OutputStream.Write(data, 0, data.Length);

            }
        }
    }
}
