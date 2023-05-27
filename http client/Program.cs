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

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args) // http
        {

            /*
            HttpClient client = new HttpClient();            
            var res = client.GetAsync("http://127.0.0.1:8080/helloworld").Result;
            Console.WriteLine("Response : " + res.StatusCode + " - " + res.Content.ReadAsStringAsync().Result);
            */
            /*
            HttpClient client = new HttpClient();
            var json_obj = new JObject();
            json_obj.Add("data1", "some data");
            json_obj.Add("data2", "some more data");
            var content = new StringContent(json_obj.ToString(), Encoding.UTF8, "application/json");
            var res = client.PostAsync("http://127.0.0.1:8080/postTest", content).Result;
            */

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
            
            // 동기식 // 
            HttpClient client = new HttpClient();
            var content = new StringContent(reqBodyStr, Encoding.UTF8, "application/json");
            var res = client.PostAsync("http://127.0.0.1:8080/postTest", content).Result;
            var contents = res.Content.ReadAsStringAsync().Result;

            var jobject = JObject.Parse(contents);
            string result_recv = jobject.GetValue("result").ToString();
            Console.WriteLine(result_recv.ToString());


            /*
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/postTest");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string json = "{\"deviceName\":\"name\"," +
                    //              "\"parameter\":\"123456789\"}";

                    streamWriter.Write(reqBodyStr);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //httpResponse.StatusCode
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject voteObj = JObject.Parse(result);
                    string resultStr = voteObj.GetValue("result").ToString();
                    Console.WriteLine(result.ToString());
                }
            }
            */


        }
    }
}

