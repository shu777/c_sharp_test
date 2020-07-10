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
using System;
using System.Reflection;

// 1. MyClass_Program 외부 실행파일 실행 (with argument)후 output 가져옴

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

        public static void testClass(string dllPath)
        {
            var DLL = Assembly.LoadFile(dllPath);
            var theType = DLL.GetType("dll_test.Class1"); // 네임스페이스. 클래스 명
            if(theType == null)
            {
                // not found calss
            }

            // 1. 메서드 직접 호출이 아닌 type에서 GetMethod로 함수를 찾고 실행시키는 방법  
            var c = Activator.CreateInstance(theType);
            var method = theType.GetMethod("Output");         
            method.Invoke(c, new object[] { @"Hello" });

            // 2. 델리게이트를 이용한 방식            
            MethodInfo minfo = theType.GetMethod("Output"); 
            OutputDelegate delSay = (OutputDelegate)Delegate.CreateDelegate(typeof(OutputDelegate), null, minfo);
            delSay("호출");


            Console.ReadLine();
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
