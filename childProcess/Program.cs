using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Management;
using System.Runtime.InteropServices;

namespace child_test
{
    internal class Program
    {
        private static void WriteProcessInfo(Process processInfo)
        {
            Console.WriteLine("Process : {0}", processInfo.ProcessName);
            Console.WriteLine("시작시간 : {0}", processInfo.StartTime);
            Console.WriteLine("프로세스 PID : {0}", processInfo.Id);
            Console.WriteLine("메모리 : {0}", processInfo.VirtualMemorySize);
        }

        /// <summary>
        /// 메일 프로세스가 kill되면 child process도 강제로 kill 해준다.
        /// </summary>
        /// <param name="Handler"></param>
        /// <param name="Add"></param>
        /// <returns></returns>
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    Console.WriteLine("CTRL+C received!");
                    Debug.WriteLine("CTRL+C received!");
                    foreach (var process in procCtx.Values)
                    {
                        process.Kill();
                    }
                    Console.WriteLine("exit");
                    Environment.Exit(0);
                    break;
                case CtrlTypes.CTRL_BREAK_EVENT:
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    Console.WriteLine("User is logging off!");
                    break;
            }
            return true;
        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var process in procCtx.Values)
            {
                process.Close();
            }
            Console.WriteLine("exit");
        }

        public static Dictionary<int, System.Diagnostics.Process> procCtx = new Dictionary<int, System.Diagnostics.Process>();

        static string executeFileName = "TestChildProcess";
        static void Main(string[] args)
        {

            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            int procCount = 2;


            if (args.Length == 0) // MAIN PROC
            {
                Console.WriteLine("MAIN PROC");
                //
                // Main Launcher Process

                // CREATE CHILD PROCESSES
                for (int i = 0; i < procCount; i++)
                {
                    System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                    pProcess.StartInfo.FileName = executeFileName + ".exe";//"child_test.exe";
                    pProcess.StartInfo.Arguments = i.ToString(); //argument process number
                    pProcess.StartInfo.UseShellExecute = false;
                    pProcess.StartInfo.RedirectStandardOutput = true;
                    pProcess.StartInfo.RedirectStandardInput = true;
                    pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                    bool res = pProcess.Start();

                    procCtx.Add(i, pProcess);

                    string requestTargetStr = "http://127.0.0.1/request" + i.ToString();
                    var json = new JObject(); /// dictionary 처럼 사용하는 jobject
                    json.Add("processNum", i.ToString());
                    json.Add("processName", "process");
                    json.Add("urlRestTarget", requestTargetStr);// @"http://127.0.0.1/request1");
                    json.Add("parentsProcID", Process.GetCurrentProcess().Id.ToString());
                    //Console.WriteLine(json.ToString());
                    //////////////////// pass json command data to child process
                    // 콘솔 전송을 위한 변환 //
                    string nStr = json.ToString();
                    nStr = nStr.Replace('"', '\"');
                    nStr = nStr.Replace('\r', ' ');
                    nStr = nStr.Replace('\n', ' ') + Environment.NewLine;

                    procCtx[i].StandardInput.Write(nStr);
                    string output2 = procCtx[i].StandardOutput.ReadLine();
                    Console.WriteLine(output2);
                }
                do
                {
                    Thread.Sleep(3000);
                    Process mainProcess = Process.GetCurrentProcess();
                    Console.WriteLine("\n\n\n****** 메인 프로세스 정보 ******");
                    WriteProcessInfo(mainProcess);
                    Process[] procs = Process.GetProcessesByName("executeFileName");
                    Process child = procs[0];
                    Console.WriteLine("****** child 프로세스 정보 ****** cnt:{0}", procs.Length - 1);
                    foreach (Process proc in procs)
                        if (mainProcess.Id != proc.Id)
                        {
                            child = proc;
                            WriteProcessInfo(child);
                        }
                    // procCtx[0].

                }
                while (true);

            }
            else // CHILD PROC
            {
                // Sub Proc body
                //Console.WriteLine("create sub process - num: {0}", args[0]);
                do
                {
                    String inputCmd1 = Console.ReadLine();
                    if (String.IsNullOrEmpty(inputCmd1)) continue;

                    JObject js_cmd = JObject.Parse(inputCmd1);
                    string parents_id = js_cmd.GetValue("parentsProcID").ToString();
                    int nParentsID = Convert.ToInt32(parents_id);

                    /*
                    // check main proc
                    Process[] procs = Process.GetProcessesByName("executeFileName");
                    Process child = procs[0];
                    bool found = false;
                    foreach (Process p in procs) { 
                        if (nParentsID == p.Id)
                        {
                            found = true;
                            inputCmd1 = " found main process";
                        }
                    }
                    if (!found) return; // main proc exit cse // 
                    */

                    Console.WriteLine(inputCmd1);
                } while (true);
            }
        }
    }
}
