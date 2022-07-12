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

// 쓰레드 관련 기능 모음

namespace ConsoleApplication_MyLibs
{
    /// 최대 100 개의 thread를 생성하고, thread safe하게 파일에 결과 값을 쓰는 예제
    /// </summary>
    class MyClass_Thread
    {
        private int max_thread;
        private System.Threading.Semaphore semaphore;
        public MyClass_Thread()
        {
            max_thread = 100; // 최대 생성기능 쓰레드 갯수 리미트
            semaphore = new System.Threading.Semaphore(this.max_thread, this.max_thread);
        }

        //private var semaphore = new System.Threading.Semaphore(this.max_thread, this.max_thread);
        public void StartConvertAndWrite(MyClass_Files_Writer fileWriter, string srcString, char delimiter, int fieldNumber)
        {
            // TODO threadpool부분 재확인
           // System.Threading.ThreadPool.QueueUserWorkItem((state) =>
           // {
                semaphore.WaitOne(); // 세마포어 wait
                try
                {
                    //DownloadItem(item);
                    var t = new System.Threading.Thread(() => ThreadRunMain(fileWriter, srcString, delimiter, fieldNumber));
                    t.Start();
                    //return t;
                }
                finally
                {
                    semaphore.Release(); // 세마포어 end
                }
           // });
        }
        /// <summary>
        /// thread run 메인
        /// </summary>
        /// <param name="fileWriter"></param>
        /// <param name="srcString"></param>
        /// <param name="delimiter"></param>
        /// <param name="fieldNumber"></param>
        private void ThreadRunMain(MyClass_Files_Writer fileWriter, string srcString, char delimiter, int fieldNumber)
        {
            MyClass_Strings strClass = new MyClass_Strings();
            // convert and write to file
            string convertedStr = strClass.convertStringMsg(srcString, delimiter, fieldNumber);
            {
                //  file.WriteLine(convertedStr);
                fileWriter.WriteToFile(convertedStr); // thread safty한 file writer...
                //System.IO.File.AppendAllText(filePath, convertedStr);
            }
        }
        ~MyClass_Thread()
        {

        }
    }
}
