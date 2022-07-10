using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics; // debug print
using System.Threading; // thread

/// <summary>
/// / 로그파일 파싱하는 기능
///   파싱한 로그의 종류별 갯수를 파일로 출력
///   파싱한 로그의 body를 외부 프로그램을 사용하여 변환 후 결과 파일로 출력
///   외부파일 이용한 변환 및 결과 파일 출력을 멀티 쓰레드를 사용하여 출력
/// </summary>

namespace ConsoleApplication_MyLibs
{
    class log_parsed_data
    {
        //2017-03-14T10:08:27#EE#M1068042C 형식 파싱
        public string logTime { get; set; }
        public string logType { get; set; }
        public string logDetail { get; set; }

        public void Print() // 추가해놓으면 디버깅시 편하다
        {
            Debug.WriteLine("logTime:" + logTime + "logType:" + logType + "logDetail:" + logDetail);
        }
    }

    class type_count_data // parsed data로 부터 획득한 결과값 저장 및 result output file write 
    {
        public string logType { get; set; }
        public int count { get; set; }
        public System.IO.StreamWriter wfile_output { get; set; }
    }

    // 클래스 리스트의 중복제거를 위한 커스텀 comparer 구현 //
    // Custom comparer for the Product class
    class TypeComparer : IEqualityComparer<type_count_data>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(type_count_data x, type_count_data y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.logType == y.logType;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(type_count_data product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = product.logType == null ? 0 : product.logType.GetHashCode();

            //Get hash code for the Code field.
            // int hashProductCode = product.Code.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName;// ^ hashProductCode;
        }
    }

    class Project_example1
    {
        // 입력 출력 파일 선언
        static string inputFileNameQ1 = "LOGFILE_A.TXT";
        static string inputFileNameQ2_4 = "LOGFILE_B.TXT";
        static string inputFileNameQ5 = "LOGFILE_C.TXT";
        static string outputFileName = "REPORT_{0}.TXT";
        static string outputTypeLg = "TYPELOG_{0}_{1}.TXT";
        static string external_progName = "CODECONV.EXE";

        static void parseLog(string data, ref List<log_parsed_data> logParsedData)
        {
            log_parsed_data parsedData = new log_parsed_data();
            string[] words = data.Split('#');
            if (words.Count() >= 3)
            {
                parsedData.logTime = words[0];
                parsedData.logType = words[1];
                parsedData.logDetail = words[2];
                logParsedData.Add(parsedData);
            }
        }
        private static int max_thread;
        private static System.Threading.Semaphore semaphore;
        private static System.Threading.ReaderWriterLockSlim _readWriteLock = new System.Threading.ReaderWriterLockSlim();

        public static void runAsyncProc(log_parsed_data data, List<type_count_data> logTypesCnt)
        {
            // 시간 오래 걸리는 외부 프로그램 실행부
            data.logDetail = runExternalProgram(external_progName, data.logDetail).Replace("\r\n", string.Empty);
            // 타입 별 output 쓰기 (append모드)

            System.IO.StreamWriter wfile_type = null;
            foreach (type_count_data t in logTypesCnt)
            {
                if (data.logType == t.logType)
                    wfile_type = t.wfile_output;//new System.IO.StreamWriter(String.Format(outputTypeLg, "1", t.logType), false);
            }
            //wfile_type = new System.IO.StreamWriter(String.Format(outputTypeLg, "1", data.logType), true);

            _readWriteLock.EnterWriteLock(); // thread safty 위해 lock
            {

                if (wfile_type != null)
                {
                    wfile_type.WriteLine(data.logTime + "#" + data.logType + data.logDetail);
                    //Debug.WriteLine(data.logTime + "#" + data.logType + "#" + data.logDetail);
                }
                //wfile.WriteLineAsync
                wfile_type.Flush();
            }
            _readWriteLock.ExitWriteLock();
        }

        public static string runExternalProgram(string program, string argument) // 외부 exe 프로그램 실행
        {
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = program;
            pProcess.StartInfo.Arguments = argument; //argument 전달
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            bool result = pProcess.Start();
            string output = pProcess.StandardOutput.ReadToEnd(); //The output result
            pProcess.WaitForExit();
            return output;
        }

        public static void runExample1() // 로그파싱하는 example
        {
            List<string> logTypes = new List<string>();
            List<log_parsed_data> logParsedData = new List<log_parsed_data>();
            List<type_count_data> logTypesCnt = new List<type_count_data>();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            // read log  // parse log
            System.IO.StreamReader file = null;
            file = new System.IO.StreamReader(String.Format("{0}", inputFileNameQ1));
            string res;
            while ((res = file.ReadLine()) != null) // 한줄씩 읽어서 parse func에 밀어 넣음 // 결과는 list로 저장
            {
                parseLog(res, ref logParsedData);
            }
            // get log types count
            //Debug.Print("가져온 데이터 확인 ");
            for (int nIndex = 0; nIndex < logParsedData.Count; nIndex++) // list내 log type들 전부 가져옴
            {
                //단순한 string list
                logTypes.Add(logParsedData[nIndex].logType);
                //class list
                type_count_data data = new type_count_data();
                data.logType = logParsedData[nIndex].logType;
                logTypesCnt.Add(data);
            }
            // string list에서 중복제거
            logTypes = logTypes.Distinct().ToList(); // 중복 제거해서 log type들을 가려냄
            // class list에서 중복제거 1
            logTypesCnt = logTypesCnt.Distinct(new TypeComparer()).ToList(); // 커스텀 비교자를 만들어서 중복제거에 적용
            // 이렇게 클래스 리스트 만들어서, 한번에 중복제거 정렬 까지 하면 코드 간편화 가능

            // 클래스 정렬
            logTypesCnt.Sort(delegate (type_count_data A, type_count_data B) // delegate로 정렬 구현
            {
                return A.logType.CompareTo(B.logType); // 오름차순 문자 정렬 ABC->
            });

            // 각 타입별 로그 횟수 카운팅 (for문 돌려서 찾는 방법)
            for (int nIndex = 0; nIndex < logTypesCnt.Count; nIndex++)
            {
                Debug.WriteLine(logTypesCnt[nIndex].logType + " " + logTypesCnt[nIndex].count);

                foreach (log_parsed_data parsedDatTmp in logParsedData)
                {
                    if (logTypesCnt[nIndex].logType == parsedDatTmp.logType)
                        logTypesCnt[nIndex].count++;
                }
            }
            // List Method를 사용하여 각 타입별 로그 횟수 카운팅
            for (int nIndex = 0; nIndex < logTypesCnt.Count; nIndex++)
            {
                logTypesCnt[nIndex].count = logParsedData.FindAll(x => x.logType.Equals(logTypesCnt[nIndex].logType)).Count;
               // int count2 = logParsedData.Where(x => x.logType.StartsWith(logTypesCnt[nIndex].logType)).Count();
            }

            // 각 타입별 로그 횟수 카운팅 결과 파일로 출력
            System.IO.StreamWriter wfile = null;
            wfile = new System.IO.StreamWriter(String.Format(outputFileName, "1"), true);
            foreach (type_count_data t in logTypesCnt)
            {
                wfile.WriteLine(t.logType + "#" + t.count.ToString());
                Debug.WriteLine(t.logType + "#" + t.count.ToString());
                t.wfile_output = new System.IO.StreamWriter(String.Format(outputTypeLg, "1", t.logType), false); // 타입별 쓰기 핸들 미리열기//
            }
            //wfile.WriteLineAsync
            wfile.Close();

            sw.Start();
#if false
            // SYNC
            // 변환기 이용한 변환 수행
            //foreach (log_parsed_data parsedDatTmp in logParsedData)
            for (int nIndex = 0; nIndex < logParsedData.Count; nIndex++) // for문 써야 다시 값을 갱신하는데 편하다.
            {
                // 시간 오래 걸린다.
                logParsedData[nIndex].logDetail = runExternalProgram(external_progName, logParsedData[nIndex].logDetail).Replace("\r\n", string.Empty);
                // 타입 별 output 쓰기 (append모드)
                {
                    System.IO.StreamWriter wfile_type = null;
                    wfile_type = new System.IO.StreamWriter(String.Format(outputTypeLg, "1", logParsedData[nIndex].logType), true);
                    //foreach (type_count_data t in logTypesCnt)
                    {
                        wfile_type.WriteLine(logParsedData[nIndex].logTime + "#" + logParsedData[nIndex].logType + logParsedData[nIndex].logDetail);
                        //Debug.WriteLine(logParsedData[nIndex].logTime+"#"+ logParsedData[nIndex].logType+"#"+logParsedData[nIndex].logDetail);
                    }
                    //wfile.WriteLineAsync
                    wfile_type.Close();
                }

            }
#else
            //ASYNC           
            max_thread = 100; // 최대 생성기능 쓰레드 갯수 리미트
            semaphore = new System.Threading.Semaphore(max_thread, max_thread);

            List<Thread> threads = new List<Thread>();

            for (int nIndex = 0; nIndex < logParsedData.Count; nIndex++) // for문 써야 다시 값을 갱신하는데 편하다.
            {
                log_parsed_data t = new log_parsed_data();
                t = logParsedData[nIndex];
                //t.Print();
                //Parallel.For(0, 100, new ParallelOptions { MaxDegreeOfParallelism = 100 }, (i)=> runAsyncProc(t));

                //System.Threading.ThreadPool.QueueUserWorkItem((state) =>
                //{
                semaphore.WaitOne(); // 세마포어 wait
                try
                {
                    var thread = new System.Threading.Thread(() => runAsyncProc(t, logTypesCnt));
                    threads.Add(thread);
                    thread.Start();
                }
                finally
                {
                    semaphore.Release(); // 세마포어 end
                }
                //});
            }
#endif
            // Await threads
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            sw.Stop();
            Debug.WriteLine("elasped time:" + sw.ElapsedMilliseconds.ToString() + "mSec");
            int test = 0;

        }
    }
    
}
