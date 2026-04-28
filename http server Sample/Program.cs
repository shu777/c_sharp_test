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

    public class Route
    {
        public string pathPrefix { get; set; }
        public string url { get; set; }
    }
    public class RoutingRule
    {
        public int port { get; set; }
        public List<Route> routes { get; set; }
    }

    internal class Program
    {

        static void Forward(HttpListenerContext context, string targetBaseUrl)// 실제 요청을 다른 서버로 전달 (프록시 핵심)
        {
            HttpListenerRequest request = context.Request;// 원본 요청
            HttpListenerResponse response = context.Response; // 클라이언트 응답
        
            try
            {
                // 전달할 최종 URL 생성 // (Path + Query 그대로 유지)
                string targetUrl = targetBaseUrl + request.Url.AbsolutePath + request.Url.Query;
        
                using (HttpClient client = new HttpClient())
                {
                    // HTTP Method 유지 (GET / POST)
                    HttpRequestMessage forwardRequest =
                        new HttpRequestMessage(new HttpMethod(request.HttpMethod), targetUrl);
        
                    // POST일 경우 Body 전달
                    /*if (request.HttpMethod == "POST" && request.HasEntityBody)
                    {
                        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string body = reader.ReadToEnd();// 요청 body 읽기
                            forwardRequest.Content =
                                new StringContent(body, Encoding.UTF8, request.ContentType ?? "application/json");// ContentType 유지하면서 body 전달
                        }
                    }*/
                    if (request.HttpMethod == "POST")
                    {
                        string body = "";//POST 전달 시 body가 없더라도 Content-Length: 0이 있어야
        
                        //if (request.HasEntityBody) 
                        if (request.ContentLength64 > 0)//ContentLength64 == 0이면 body를 읽지않는다.
                        {
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                body = reader.ReadToEnd();// 요청 body 읽기
                            }
                        }
        
                        string mediaType = "application/json";
        
                        if (!string.IsNullOrEmpty(request.ContentType))
                        {
                            mediaType = request.ContentType.Split(';')[0].Trim(); // ; charset=utf-8 제거? //StringContent 세 번째 인자는 media type만 기대
                        }
        
                        //forwardRequest.Content = new StringContent(body, Encoding.UTF8, request.ContentType ?? "application/json");// ContentType 유지하면서 body 전달
                        forwardRequest.Content = new StringContent(body, Encoding.UTF8, mediaType);
                    }
        
                    // 실제 target 서버로 요청 전송
                    HttpResponseMessage targetResponse = client.SendAsync(forwardRequest).Result;
                    // 응답 Status 그대로 전달
                    response.StatusCode = (int)targetResponse.StatusCode;
        
                    if (targetResponse.Content != null)
                    {// Content-Type 그대로 전달
                        string contentType = targetResponse.Content.Headers.ContentType?.ToString();
                        if (!string.IsNullOrEmpty(contentType))
                        {
                            response.ContentType = contentType;
                        }
                        // 응답 Body 그대로 전달
                        byte[] bodyBytes = targetResponse.Content.ReadAsByteArrayAsync().Result;
                        response.ContentLength64 = bodyBytes.Length; // Response도 Content-Length 지정
                        response.OutputStream.Write(bodyBytes, 0, bodyBytes.Length); // 같은 요청 처리 흐름 안에서는 어디서든 사용 가능 // 클라이언트 요청 하나에 대한 응답
                    }
                    // 응답 종료
                    //response.Close();
                }
        
            }
            catch (Exception e)
            {
                byte[] errorBytes = Encoding.UTF8.GetBytes("{\"result\":\"" + e.Message + "\"}");
                response.StatusCode = 500;
                response.ContentType = "application/json";
                response.ContentLength64 = errorBytes.Length;
                response.OutputStream.Write(errorBytes, 0, errorBytes.Length);
            }
            finally // response.Close는 finally로
            {
                response.Close();
            }
        
        
        }
        static void HandleRequest(HttpListenerContext context, RoutingRule rules)
        {
            // 요청된 URL의 Path (/front/abc)
            string path = context.Request.Url.AbsolutePath;
        
            // ckass 구조체의 route목록에서 pathPrefix가 일치하는 것 찾기 // 길이가 긴 prefix 우선 (ex: /front/api > /front)
            Route matchedRoute = rules.routes
                .Where(r => path.StartsWith(r.pathPrefix))// prefix 매칭
                .OrderByDescending(r => r.pathPrefix.Length)// 긴 것 우선
                .FirstOrDefault();// 첫 번째 선택
        
            if (matchedRoute == null)// 매칭되는 route 없으면 404
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
                return;
            }
        
            Forward(context, matchedRoute.url);// 매칭된 route의 url로 요청 전달
        }


        
        static void Main(string[] args) // http
        {


            ////// Prefix 방식 샘플 -> HandleRequest
            if (args.Length < 1) // arg num 체크
            {
                Console.WriteLine("Routing rule file path required.");
                return;
            }
            
            // json 파일 -> class 구조체
            string rulePath = args[0];// JSON 파일 경로
            string json = File.ReadAllText(rulePath);// 파일 읽어서 JSON 문자열 가져오기
            RoutingRule rules = JsonConvert.DeserializeObject<RoutingRule>(json);// JSON → C# 객체로 변환
            
            
            // http 서버 listen ASYNC //
            HttpListener listener = new HttpListener(); // HTTP 서버 생성
            listener.Prefixes.Add($"http://127.0.0.1:{rules.port}/");// 해당 포트로 모든 요청 수신
            listener.Start();
            Console.WriteLine($"Server started on port {rules.port}");
            while (true)// 무한 루프로 요청 계속 받기
            {
                HttpListenerContext context = listener.GetContext(); // 클라이언트 요청 하나 받음 (blocking)
                //HandleRequest(context, rules); // 요청 처리
                Task.Run(() => HandleRequest(context, rules));// 비동기 요청처리
            }

            ///////////////////////////////////////////////////////////////////////
            ///// Segments 기반 샘플 //// URL 구조를 쪼개서 판단 URL 파싱 RPC 스타일 유연성 낮음 케이스 많으면 복잡            
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
                // response data ///////////////////////////////
                var reqBody = new JObject();
                reqBody.Add("result", "ffffxxdeddc");
                string reqBodyStr = reqBody.ToString();
                byte[] content = Encoding.UTF8.GetBytes(reqBodyStr); // json -> string -> UTF8 -> response contents body
                context.Response.OutputStream.Write(content, 0, content.Length);
                context.Response.StatusCode = 200; // response 결과
                context.Response.Close();

            }
        }
    }
}
