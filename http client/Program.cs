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
    // ##### 1
    public class QueueInfo // json deserialize를 위한 구조체 class
    {
        public int inputQueueCount { get; set; }//가변 크기// Add / Remove 가능 // 사용권장
        //public string[] inputQueueURIs;//고정 크기 생성 시 크기 정해짐 추가/삭제 불편
        public List<string> inputQueueURIs { get; set; }
        public string outputQueueURI { get; set; }
    }
    // @@@@@ 1
    public class InputData // json deserialize를 위한 구조체 class
    {
        public long timestamp { get; set; }
        public string value { get; set; }
        // custom 추가
        public string queueNumber { get; set; }
    }
    
    internal class Program
    {
        // ##### 2 HTTP GET 1
        public static QueueInfo GetQueueInfo() // detailed.. // http GET 후 response 체크 하고, 200일 경우 가져온 result를 json parsing return.
        {
             string url = "http://127.0.0.1:8080/qInfo";
             try
             {
                 using (HttpClient client = new HttpClient())
                 {
                     var res = client.GetAsync(url).Result;
                     if (!res.IsSuccessStatusCode)
                     {
                         Console.WriteLine($"HTTP Error: {res.StatusCode}");
                         return null;
                     }
                     string json = res.Content.ReadAsStringAsync().Result;
                     return JsonConvert.DeserializeObject<QueueInfo>(json);
                 }
             }
             catch (HttpRequestException e)
             {
                 Console.WriteLine("Request error: " + e.Message);
             }
             catch (Exception e)
             {
                 Console.WriteLine("Unexpected error: " + e.Message);
             }
             return null;        
         }
        // ##### 2' HPPT GET 1-1
        public static QueueInfo_Easy GetQueueInfo() // easy  // api 에서 제공하는 result check 사용 // 코드 짧음.
        {
            string url = "http://127.0.0.1:8080/qInfo";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = client.GetStringAsync(url).Result;            
                    QueueInfo queueInfo = JsonConvert.DeserializeObject<QueueInfo>(json);            
                    return queueInfo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return null;
            }    
         }
        // @@@@@ 2 // HTTP GET 2
         public static InputData RequestQueueInfo_Easy(string target, string queueNumber) // easy  // api 에서 제공하는 result check 사용 // 코드 짧음.
         {
            string url = target;
            Console.WriteLine(url);
            if (url == null)
            {
                return null;
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var res = client.GetAsync(url).Result;
                    if (!res.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"HTTP Error: {res.StatusCode}");
                        return null;
                    }
                    string json = res.Content.ReadAsStringAsync().Result;
                    InputData result = JsonConvert.DeserializeObject<InputData>(json); // 받은 값 처리위해 return.
                    result.queueNumber = queueNumber; // pass number 추가 (각 thread에서 처리 위해 추가 정보.)
                    Console.WriteLine($"received Q:{queueNumber} timestamp={result.timestamp}, value={result.value}");
                    return result;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error: " + e.Message);
            }
            return null;        
        }

        // @@@@@ 5
        public static void PassResult(string targetUri, string resultStr)
        {
            string url = targetUri;
            try
            {
                using (HttpClient client = new HttpClient()) // HTTP POST 1
                {
                    // JSON 객체 생성
                    var bodyObj = new
                    {
                        result = resultStr // lavel and value
                    };
                    // JSON 문자열 변환
                    string json = JsonConvert.SerializeObject(bodyObj);
                    Console.WriteLine("post req: {0} {1} {2}", json, resultStr, bodyObj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var res = client.PostAsync(targetUri, content).Result;
                    if (!res.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"HTTP Error: {res.StatusCode}");
                        return;
                    }
                    string response = res.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Output Response: {0} " + response);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error: " + e.Message);
            }
            return;        
        }
        
        static void Main(string[] args) // http
        {

            /*  // HTTP GET sample. simple.
            HttpClient client = new HttpClient();            
            var res = client.GetAsync("http://127.0.0.1:8080/helloworld").Result;
            Console.WriteLine("Response : " + res.StatusCode + " - " + res.Content.ReadAsStringAsync().Result);
            */
            /*  // HTTP POST sample. simple.
            HttpClient client = new HttpClient();
            var json_obj = new JObject();
            json_obj.Add("data1", "some data");
            json_obj.Add("data2", "some more data");
            var content = new StringContent(json_obj.ToString(), Encoding.UTF8, "application/json");
            var res = client.PostAsync("http://127.0.0.1:8080/postTest", content).Result;
            */

            // ##### 3
            QueueInfo queueInfo = GetQueueInfo(); // Http client GET -> response check -> json to struct deserialize

            // @@@@@ 3
            for (int i = 0; i < queueInfo.inputQueueURIs.Count; i++)
            {
                int queueNo = i;
                string uri = queueInfo.inputQueueURIs[i];
            
                Task.Run(() => // 각 http server들을 thread 로 분리하여 실행. (비동기 필수)
                {
                    Console.WriteLine("worker run ", queueNo);
                    while (true)
                    {
                        Console.WriteLine(".");
                        //InputData result = RequestQueueInfo_Easy(uri, i.ToString());//Task.Run 안에서 i.ToString() 쓰면 위험
                        InputData result = RequestQueueInfo_Easy(uri, queueNo.ToString());
                        if (result == null)
                        {
                            Console.WriteLine($"Q:{queueNo} result is null");
                            continue;
                        }
                        // @@@@@ 4
                        string workerResult = procWorker(result);
                        if (workerResult != null) // return값 validation 체크를 안해서 big issue.
                        {
                            Console.WriteLine("procWorker RES: {0} {1} ", workerResult, queueInfo.outputQueueURI);
                            // @@@@@ 5
                            PassResult(queueInfo.outputQueueURI, workerResult);
                        }
                    }
                });
            }
            

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

