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

namespace ConsoleApplication_MyLibs
{
    //internal class MyClass_http_json
    //{
    //}
    class MyClass_http_json
    {

        public MyClass_http_json()
        {

        }
        ~MyClass_http_json() { }

        public static void requestUrlParsingSample()
        {
            // URL 파싱 샘플
            string testUrl = "rtsp://root:1234@127.0.0.1/token000001?res=1280x720";
            var uri_parse = new Uri(testUrl); // Uri로 변환
            var protocol = uri_parse.Scheme; // 프로토콜 종류 rtsp, http, 
            var loginUserInfo = uri_parse.UserInfo; // root:1234 , id/passwd로 파싱 필요
            string[] loginUserInfoParsed = loginUserInfo.Split(':');
            var loginID = loginUserInfoParsed[0]; // root
            var loginPasswd = loginUserInfoParsed[1]; // 1234
            var streamHostAddr = uri_parse.Host; // 타겟 host address
            var stremamPort = uri_parse.Port; // 타겟 서비스 port  ( -1 : null )
            var streamToken = uri_parse.Segments[1]; // 토큰  [0]은 split string..
            var streamOptionExtra = uri_parse.Query; // 토큰 뒤에 오는 값, 추가 파싱 필요  ?res=1280x720
            var tempStr = streamOptionExtra.Remove(0, 5);
            var resolution = tempStr.Split('x');
            var width = resolution[0];
            var height = resolution[1];
            var streamQueryAll = uri_parse.PathAndQuery; // 옵션 전체  /token000001?res=1280x720
        }
        public static void client_GET_Request() // GET 샘플
        {
            HttpClient client = new HttpClient();
            var res = client.GetAsync("http://127.0.0.1:8080/helloworld").Result;
            Console.WriteLine("Response : " + res.StatusCode + " - " + res.Content.ReadAsStringAsync().Result);
        }
        public static void client_POST_Request() // POST 샘플
        {
            HttpClient client = new HttpClient();
            var json_obj = new JObject();
            json_obj.Add("data1","some data");
            json_obj.Add("data2", "some more data");
            var content = new StringContent(json_obj.ToString(), Encoding.UTF8, "application/json");
            var res = client.PostAsync("http://127.0.0.1:8080/postTest", content).Result;
        }
        public static void client_POST_DIR_INFO()// 폴더내 파일 목록을 JSON으로 변환하여 서버로 POST 하는 샘플
        {
            string folder_target = "./folder";
            string[] files = System.IO.Directory.GetFiles(folder_target); // 디렉토리 내 파일리스트 겟
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);  // 파일명만 필요
            }
            var file_array_json = new JArray(files); // JARRAY로 변환
            var reqBody = new JObject();
            reqBody.Add("Folder_Name", folder_target);
            reqBody.Add("Files_List", file_array_json);
            string reqBodyStr = reqBody.ToString();

            HttpClient client = new HttpClient();
            var content = new StringContent(reqBodyStr, Encoding.UTF8, "application/json");
            var res = client.PostAsync("http://127.0.0.1:8080/postFolderInfo", content).Result;
        }

        public static void server_GET_POST_Context()  // post와 get을 받는 서버 샘플
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://127.0.0.1:8080/");
            server.Start();

            while (true)
            {
                var context = server.GetContext();
                Console.WriteLine("Request : " + context.Request.Url);
                var request = context.Request;
                string methodName = context.Request.Url.Segments[1].Replace("/", "");
                switch (methodName) //
                {
                    case "helloworld": // API name // GET request 샘플
                        int test1 = 0;
                        break;
                    case "postTest": // PORT request 샘플
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
                            Console.WriteLine("End of data:");
                            reader.Close();
                            body.Close();

                            if (methodName == "postFolderInfo") // 클라이언트에서 받은 폴더내 파일 정보 서버에 파일로 저장하는 샘플
                            {
                                DateTime time_now = DateTime.Now; // get current TIME string
                                string curTime = time_now.ToString("yyyyMMdd_HHmmss"); // 현재시간.json 파일에 POST 로 받은 json 데이타 저장
                                string filePath = curTime + ".json";
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true))
                                {
                                    sw.WriteLine(s);
                                    sw.Close();
                                }

                            }

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

        public static void server_POST_Context()
        {
            HttpListener server = new HttpListener();
            var context = server.GetContext();
            var request = context.Request;
            string text;
           // using (var reader = new StreamReader(request.InputStream,
             //                                    request.ContentEncoding))
           // {
            //    text = reader.ReadToEnd();
           // }
            if (request.HasEntityBody)
            {
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var reader = new StreamReader(body, encoding);
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}", request.ContentType);
                }
                Console.WriteLine("Client data content length {0}", request.ContentLength64);

                Console.WriteLine("Start of data:");
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                Console.WriteLine("End of data:");
                reader.Close();
                body.Close();
            }
            // Use text here
        }
        public static void json_make_sample()
        {
            var dictSample = new Dictionary<string, string>();
            dictSample.Add("id", "root");
            dictSample.Add("passwd", "1234");
            dictSample.Add("token", "asdfg1234567");
            var dictSampleMain = new Dictionary<string, object>();
            dictSampleMain.Add("sample1", dictSample);
            var json_from_dict = JObject.FromObject(dictSample);
            Console.WriteLine(json_from_dict.ToString());

            // jobject 
            var json = new JObject(); /// dictionary 처럼 사용하는 jobject
            json.Add("id", "Luna");
            json.Add("name", "Silver");
            json.Add("age", 19);
            Console.WriteLine(json.ToString());

            var json2 = JObject.Parse("{ id : \"Luna\" , name : \"Silver\" , age : 19 }");
            json2.Add("blog", "devluna.blogspot.kr");
            Console.WriteLine(json2.ToString());

            var json4 = JObject.FromObject(new { id = "J01", name = "June", age = 23 });
            Console.WriteLine(json4.ToString());

            var json5 = JObject.Parse("{ id : \"sjy\" , name : \"seok-joon\" , age : 27 }");
            json5.Add("friend1", json);
            json5.Add("friend2", json2);
            json5.Add("friend4", json4);
            Console.WriteLine(json5.ToString());

            // jarray
            var jarray = new JArray();   // 간단한 string array 샘플
            jarray.Add(1);
            jarray.Add("Luna");
            jarray.Add(DateTime.Now);

            Console.WriteLine(jarray.ToString());

            var jFriends = new JArray(); // jobject의 jarray 샘플
            jFriends.Add(json);
            jFriends.Add(json2);
            // jFriends.Add(json3);
            jFriends.Add(json4);
            Console.WriteLine(jFriends.ToString());

            json2.Add("Friends", jFriends);
            Console.WriteLine(json2.ToString());

            ////////////////////////////읽어오기 ////////////////
            string test = json2.GetValue("blog").ToString();  // jobject내 value 가져오기 샘플

            foreach (JObject item in jFriends) // jarray 값 foreach로 가져오기 샘픔
            {
                string id = item.GetValue("id").ToString();
                string name = item.GetValue("name").ToString();
                // ...
            }

            /*
             *  // vote.json //
             * {
                    "AI": "1",
                    "AJ": "0",
                    "AM": "0",
                    "AN": "0",
                    "BK": "5",
                    "BL": "8",
                    "BM": "0",
                    "BN": "0",
                    "BO": "4",
                    "CJ": "0",
                    "CK": "2"
                }
             */
            ///////////////// jobject 정렬 //////////////
            string voteJson = File.ReadAllText("vote.json"); // file 에서 json 읽기
            JObject voteObj = JObject.Parse(voteJson); // json Object로 파싱
            var sortedObj = new JObject(
                voteObj.Properties().OrderByDescending(p => (int)p.Value) //값으로 내림차순 정렬해서 새로 저장
            );
            string output = sortedObj.ToString();
            JProperty firstProp = sortedObj.Properties().First(); // 제일 큰 값을 get
            Console.WriteLine("Winner: " + firstProp.Name + " (" + firstProp.Value + " votes)");
        }
    }
}
