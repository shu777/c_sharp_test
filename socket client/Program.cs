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
        static void Main(string[] args) // socket
        {
            byte[] bytes = new byte[1024];
           //try
            //{
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");  // 주소 
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 8090); // 포트
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // 소켓 open
            try
            {
                sender.Connect(remoteEP);
                NetworkStream nstrm = new NetworkStream(sender);
                BinaryWriter bwrt = new BinaryWriter(nstrm);

                DirectoryInfo dirInfo = new DirectoryInfo("./clientDir");
                FileInfo[] files = dirInfo.GetFiles();
                foreach(FileInfo file in files)
                {
                    bwrt.Write(file.Name); // 이름 전송
                    long fileLength = file.Length;
                    bwrt.Write(fileLength); // 파일크기 전송
                    FileStream fstrm = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                    while(fileLength>0)
                    {
                        int nReadLength = fstrm.Read(bytes, 0, Math.Min(1024, (int)fileLength));
                        bwrt.Write(bytes, 0, nReadLength);
                        fileLength -= nReadLength;
                    }
                    fstrm.Close();
                }
                //int bytesRec = sender.Receive(bytes);
                //Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRec));
                //sender.Shutdown(SocketShutdown.Both);
                //sender.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            }

        }
#if false
    static void oldhttpserver()//Main(string[] args) // http
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
            for (int i = 0; i < files.Length; i++){
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
#endif
}

