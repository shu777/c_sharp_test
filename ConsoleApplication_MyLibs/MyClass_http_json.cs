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

        public static void server_GET_Context()
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
                    case "helloworld": // API name
                        int test1 = 0;
                        break;
                    case "postTest": // API name
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
