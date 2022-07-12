using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;// 암호화
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;//정규표현식
using System.Runtime.InteropServices;

using System.Reflection; //for  Assembly

/// <summary>
/// 0. 외부 DLL을 import하여 method를 실행하는 샘플
/// // 1. MyClass_Program 외부 실행파일 실행 (with argument)후 output 가져옴
/// </summary>


namespace ConsoleApplication_MyLibs
{


    class MyClass_CallDll
    {

        //[DllImport("")]
        //public static extern void DisplayHelloFromDLL();
        IntPtr hModule = IntPtr.Zero;
        public MyClass_CallDll()
        {

        }

        delegate void OutputDelegate(string s);//(ref int a, ref int b);
        delegate int method1Delegate(string input_str, out string output_str);
        delegate int method2_Delegate(string input_str, ref string output_str);
        public static void testClass(string dllPath)
        {
            var DLL = Assembly.LoadFile(dllPath);
            var theType = DLL.GetType("dll_test.Class1"); // 네임스페이스. 클래스 명
            if(theType == null)
            {
                // not found calss
            }


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
            /*
            // 1. 메서드 직접 호출이 아닌 type에서 GetMethod로 함수를 찾고 실행시키는 방법  
            var c = Activator.CreateInstance(theType);
            var method = theType.GetMethod("Output");
            method.Invoke(c, new object[] { @"Hello" });
            */

            //Console.ReadLine(); // 사용자 입력대기
        }

    }
    /// <summary>
    /// 외부 프로그램을 argument와 함께 실행하여 결과 값을 string으로 return 한다.
    /// </summary>
    class MyClass_Program
    {
        public MyClass_Program()
        {

        }

        public static void multiProcessSample()
        {
            ////////////////// create Process 0
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "..\\externalApp\\externalApp.exe";
            pProcess.StartInfo.Arguments = "1"; //argument
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess.Start();
            ///////////////////// create Process 1
            System.Diagnostics.Process pProcess2 = new System.Diagnostics.Process();
            pProcess2.StartInfo.FileName = "..\\externalApp\\externalApp.exe";
            pProcess2.StartInfo.Arguments = "2"; //argument
            pProcess2.StartInfo.UseShellExecute = false;
            pProcess2.StartInfo.RedirectStandardOutput = true;
            pProcess2.StartInfo.RedirectStandardInput = true;
            pProcess2.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess2.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess2.Start();

            //////////////////// pass command
            pProcess.StandardInput.Write(@"makeRestThread" + Environment.NewLine);
            //string output = pProcess.StandardOutput.ReadLine();//Read().ToString();
            //Console.WriteLine(output);
            /////////////////// pass value
            pProcess.StandardInput.Write(@"http://127.0.0.1/request1" + Environment.NewLine);
            string output2 = pProcess.StandardOutput.ReadLine();//.ToString();
            Console.WriteLine(output2);
            ///////////////////// pass command
            pProcess2.StandardInput.Write(@"makeRestThread" + Environment.NewLine);
            //string output22 = pProcess2.StandardOutput.ReadLine();//Read().ToString();
            //Console.WriteLine(output22);
            //////////////////// pass value
            pProcess2.StandardInput.Write(@"http://127.0.0.1/request2" + Environment.NewLine);
            string output222 = pProcess2.StandardOutput.ReadLine();//.ToString();
            Console.WriteLine(output222);

            pProcess.StandardInput.Close();
            pProcess.Close();
            pProcess2.StandardInput.Close();
            pProcess2.Close();
        }
        
        // 외부프로그램을 argument와 함께 실행하고, 순차적으로 명령을 보내는 샘플
        public static void externalProgramStdinoutRedirectionSample()
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "..\\externalApp\\externalApp.exe";
            pProcess.StartInfo.Arguments = "-a 2"; //argument
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess.Start();
            /////////////////////

            pProcess.StandardInput.Write(@"HELLO" + Environment.NewLine);
            string output = pProcess.StandardOutput.ReadLine();
            Console.WriteLine(output);
            pProcess.StandardInput.Write(@"command push" + Environment.NewLine);
            string output2 = pProcess.StandardOutput.ReadLine();
            Console.WriteLine(output2);

            pProcess.StandardInput.Close();
            pProcess.Close();
        }

        /// <summary>
        /// 스트링 return 타입의 external program을 실행.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="argument">컨버팅 할 문자열</param>
        /// <returns>컨버팅 된 문자열</returns>
        public static string runExternalProgram(string program, string argument)
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = program;
            pProcess.StartInfo.Arguments = argument; //argument
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd(); //The output result
            pProcess.WaitForExit();
            return output;
        }

        ~MyClass_Program()
        {

        }
    }

}
