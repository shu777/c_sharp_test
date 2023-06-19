using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using dll_test;

// external .exe process run sample
// external dll method call sample
// URL parsing sample
namespace externalProcessSample
{
    internal class Program
    {
        delegate void OutputDelegate(string s);//(ref int a, ref int b);
        delegate int method1Delegate(string input_str, out string output_str);
        delegate int method2_Delegate(string input_str, ref string output_str);
        static void Main(string[] args)
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "..\\externalApp\\externalApp.exe";
            pProcess.StartInfo.Arguments = "1"; //argument
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess.Start();
            /////////////////////
            System.Diagnostics.Process pProcess2 = new System.Diagnostics.Process();
            pProcess2.StartInfo.FileName = "..\\externalApp\\externalApp.exe";
            pProcess2.StartInfo.Arguments = "2"; //argument
            pProcess2.StartInfo.UseShellExecute = false;
            pProcess2.StartInfo.RedirectStandardOutput = true;
            pProcess2.StartInfo.RedirectStandardInput = true;
            pProcess2.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess2.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess2.Start();

            pProcess.StandardInput.Write(@"makeRestThread" + Environment.NewLine);
            //string output = pProcess.StandardOutput.ReadLine();//Read().ToString();
            //Console.WriteLine(output);
            pProcess.StandardInput.Write(@"http://127.0.0.1/request1" + Environment.NewLine);
            string output2 = pProcess.StandardOutput.ReadLine();//.ToString();
            Console.WriteLine(output2);

            pProcess2.StandardInput.Write(@"makeRestThread" + Environment.NewLine);
            //string output22 = pProcess2.StandardOutput.ReadLine();//Read().ToString();
            //Console.WriteLine(output22);
            pProcess2.StandardInput.Write(@"http://127.0.0.1/request2" + Environment.NewLine);
            string output222 = pProcess2.StandardOutput.ReadLine();//.ToString();
            Console.WriteLine(output222);



            pProcess.StandardInput.Close();
            pProcess.Close();
            pProcess2.StandardInput.Close();
            pProcess2.Close();


            Worker class_worker = new Worker(0);
            class_worker.Run("TOUCH_01");
            // dll의 namespace를 project의 namespace와 맞춰주면 바로 호출 가능 or using dll_test; 
            ClassNameTEST class_t = new ClassNameTEST(1);  //
            string outputStr5;
            var res = class_t.method1("intput string !!!", out outputStr5);


            var DLL = Assembly.LoadFile(Environment.CurrentDirectory + ".\\dll_test.dll");
            //var theType = DLL.GetType("dll_test.Class1"); // 네임스페이스. 클래스 명
            var theType = DLL.GetType("dll_test.Class1"); // 네임스페이스. 클래스 명
            if (theType == null)
            {
                // not found calss
            }

            // 1. 메서드 직접 호출이 아닌 type에서 GetMethod로 함수를 찾고 실행시키는 방법  
            var c = Activator.CreateInstance(theType);
            var method = theType.GetMethod("Output");
            method.Invoke(c, new object[] { @"Hello" });
            /*
            var method2 = theType.GetMethod("method1");
            object[] argss = new object[] { "inputTEst", "" };
            int result = (int)method.Invoke(null, argss);
            int z = (int)argss[1];
            Console.WriteLine(args[1]); // Prints Hello
            */
            // 2. 델리게이트를 이용한 방식            
            MethodInfo minfo = theType.GetMethod("Output");
            OutputDelegate _Output = (OutputDelegate)Delegate.CreateDelegate(typeof(OutputDelegate), null, minfo);
            _Output(@"Hello_2");

            MethodInfo method1_p = theType.GetMethod("method1");
            method1Delegate _method1 = (method1Delegate)Delegate.CreateDelegate(typeof(method1Delegate), null, method1_p);
            string outputStr = "";
            int result = (int)_method1(@"passArgumentTest", out outputStr);

            MethodInfo method2_p = theType.GetMethod("method2");
            method2_Delegate _method2 = (method2_Delegate)Delegate.CreateDelegate(typeof(method2_Delegate), null, method1_p);
            outputStr = "";
            result = (int)_method2(@"passArgumentTest", ref outputStr);
            //Console.ReadLine(); // 사용자 입력대기


            // URL 파싱 샘플
            string testUrl = "rtsp://root:1234@127.0.0.1/token000001?res=1280x720";
            var uri_parse = new Uri(testUrl); // Uri로 변환
            var protocol = uri_parse.Scheme; // 프로토콜 종류 rtsp, http, 
            var loginUserInfo = uri_parse.UserInfo; // root:1234 , id/passwd로 파싱 필요
                string [] loginUserInfoParsed = loginUserInfo.Split(':');
                var loginID = loginUserInfoParsed[0]; // root
                var loginPasswd = loginUserInfoParsed[1]; // 1234
            var streamHostAddr = uri_parse.Host; // 타겟 host address
            var stremamPort = uri_parse.Port; // 타겟 서비스 port  ( -1 : null )
            var streamToken = uri_parse.Segments[1]; // 토큰  [0]은 split string..
            var streamOptionExtra = uri_parse.Query; // 토큰 뒤에 오는 값, 추가 파싱 필요  ?res=1280x720
                var tempStr = streamOptionExtra.Remove(0,5);
                var resolution = tempStr.Split('x');
                var width = resolution[0];
                var height = resolution[1];
            var streamQueryAll = uri_parse.PathAndQuery; // 옵션 전체  /token000001?res=1280x720



            //return output;
        }
    }
}
