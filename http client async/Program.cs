using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;


namespace ConsoleApp2
{
    internal class Program
    {
        public static async Task<HttpResponseMessage> PostAsyncHttp()
        {
            HttpClient _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer Ask2O1C1EUq0iNdBGAVGx4Jm9mUvcSLmfvgV61OH1nY");

            //var parameters = new Dictionary<string, string>();
           // parameters.Add("message", "안녕하세요");
           // var encodedContent = new FormUrlEncodedContent(parameters);

           // var response = await _httpClient.PostAsync("https://notify-api.line.me/api/notify", encodedContent).ConfigureAwait(false);

            // 폴더내 파일 목록을 JSON으로 변환하여 서버로 POST 하는 샘플
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

            // 비동기식 // 
            HttpClient client = new HttpClient();
            var content = new StringContent(reqBodyStr, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://127.0.0.1:8080/postTest", content).ConfigureAwait(false);
            //var contents = res.Content.ReadAsStringAsync().Result;

            Console.WriteLine("비동기 PostAsyncHttp() 호출");
            return response;
        }


        static async Task Main(string[] args) // http
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("비동기 3회 시작");
            Task<HttpResponseMessage> first = PostAsyncHttp();
            Task<HttpResponseMessage> second = PostAsyncHttp();
            Task<HttpResponseMessage> third = PostAsyncHttp();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("다른 로직 수행 : " + i);
            }

            HttpResponseMessage fr = await first;
            HttpResponseMessage sec = await second;
            HttpResponseMessage th = await third;
            var resStr = fr.Content.ReadAsStringAsync().Result;
            var jobject = JObject.Parse(resStr);
            string result_recv = jobject.GetValue("result").ToString();
            Console.WriteLine(result_recv.ToString());


            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "ms");

            Thread.Sleep(5000);



        }
    }
}

