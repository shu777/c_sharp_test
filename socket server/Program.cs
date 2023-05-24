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
        static void Main(string[] args) // socket 
        {
            // 클라이언트로 부터 파일을 수신받아 서버에 기록하는 샘플
             byte[] bytes = new Byte[1024];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8090);  
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                Socket handler = listener.Accept();

                NetworkStream nstrm = new NetworkStream(handler);
                BinaryReader breader = new BinaryReader(nstrm);
                FileStream fstrm = null;

                string filename; 
                while((filename = breader.ReadString()) != null )
                {
                    // file length recv
                    int length = (int)breader.ReadInt64();
                    fstrm = new FileStream("./recv_files" + '/' + filename, FileMode.Create);
                    while (length > 0)
                    {
                        // file body receive
                        int receiveLength = breader.Read(bytes, 0, Math.Min(1024, length));
                        fstrm.Write(bytes, 0, receiveLength);
                        length -= receiveLength;
                    }
                    fstrm.Close();
                    Console.WriteLine(filename + "is received");
                }               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("[Server]  \nEnd server...");

        }
        static void _Main(string[] args) // http

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
                            string s = reader.ReadToEnd(); // client의 json 요청 -> parsing
                            Console.WriteLine(s);
                            if(methodName == "postFolderInfo")
                            {                                
                                DateTime time_now = DateTime.Now; // get current TIME string
                                string curTime =  time_now.ToString("yyyyMMdd_HHmmss"); // 현재시간.json 파일에 POST 로 받은 json 데이타 저장
                                string filePath = curTime + ".json";
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true))
                                {
                                    sw.WriteLine(s);
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
                // responset data
                byte[] data = Encoding.UTF8.GetBytes("HelloWorld");
                context.Response.OutputStream.Write(data, 0, data.Length);
                context.Response.StatusCode = 200;
                context.Response.Close();

            }
        }
    }
}
