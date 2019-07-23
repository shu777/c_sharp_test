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


// 1. MyClass_Program 외부 실행파일 실행 (with argument)후 output 가져옴

namespace ConsoleApplication_MyLibs
{
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
