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
using System.IO;

// 파일,디렉토리,로그에 대한 기능 분류

/// <summary>
/// 목차
/// 1. MyClass_Files_Writer : 파일에 문자열 쓰는 클래스 (이어쓰기 지원, thread safty지원)
/// 2. MyClass_Files_Reader : 파일에서 문자열 읽어오는 클래스
/// 3. MyClass_Dprint : 디버그 프린트용 클래스
/// 4. MyClass_Logger : 파일로 로그를 남기는 클래스
/// </summary>



namespace ConsoleApplication_MyLibs
{

    class MyClass_ScanDirs
    {
        int count = 1;

        void DirFileSearch(string path, string file)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path, $"*.{file}");
                foreach(string f in files)
                {
                    // 이곳에 해당 파일을 찾아서 처리 할 코드 삽입
                    Debug.Print($"[{count++}] path = {f}");
                }
                if(dirs.Length > 0)
                {
                    foreach(string dir in dirs)
                    {
                        DirFileSearch(dir, file);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }
        public static void testClass()
        {
            MyClass_ScanDirs tmp = new MyClass_ScanDirs();
            tmp.DirFileSearch(@"D:\", "doc");

        }
    }


    // 여러개 쓰레드에서 동시에 file 기록 시 사용
    // thread safe한 file writer // append모드로 동작한다. 매번 file open / close..
    class MyClass_Files_Writer
    {
        private string filePath;
        private bool appendWriteOpen = true;

        public MyClass_Files_Writer(string filePath)
        {
            string current_path = System.IO.Directory.GetCurrentDirectory();
            if (System.IO.File.Exists(filePath))
            {
                if (appendWriteOpen)
                {

                }
                else
                {
                    System.IO.File.Delete(filePath);
                }
            }
            this.filePath = filePath;
        }
        public MyClass_Files_Writer(string filePath, bool appendWriteOpen)
        {
            string current_path = System.IO.Directory.GetCurrentDirectory();
            if (System.IO.File.Exists(filePath))
            {
                if (appendWriteOpen)
                {

                }
                else
                {
                    System.IO.File.Delete(filePath);
                }
            }
            this.filePath = filePath;
        }
        public string customNewLine = string.Empty;

        private static System.Threading.ReaderWriterLockSlim _readWriteLock = new System.Threading.ReaderWriterLockSlim();
        // thread safe한 file write.
        public void WriteToFile(string text)
        {
            _readWriteLock.EnterWriteLock();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(this.filePath, true))
            {
                if (!customNewLine.Equals(string.Empty))
                    sw.NewLine = customNewLine;
                //sw.NewLine = "\n"; // 기본은 \r\n 이다. // user defined newline symbol.
                sw.WriteLine(text);
                sw.Close();
            }
            _readWriteLock.ExitWriteLock();
        }
        ~MyClass_Files_Writer()
        {

        }
    }

    /// <summary>
    ///  file reader 파일에서 문자열을 읽어오는 클래스 
    /// </summary>
    class MyClass_Files_Reader
    {
        System.IO.StreamReader file = null;
        private string mFilePath = string.Empty;
        public MyClass_Files_Reader(string filePath)
        {
            //string current_path = System.IO.Directory.GetCurrentDirectory();
            file = new System.IO.StreamReader(filePath);
            mFilePath = filePath;

        }
        /// <summary>
        /// 파일 전체를 읽어서 new line 타입을 최초에 detect 되는 타입으로 판단/return 한다. 
        /// 2종류가 혼재되어 있지 않은 것을 전제한다.
        public string getNewLine()
        {
            //string[] readStr = this.readLines(mFilePath);
            string res = this.readAllText(mFilePath);
            if (res.Contains("\r\n"))
            {
                Debug.WriteLine(" new line with carriage return.");
                return "\r\n"; // 캐리지 리턴이 포함됨.
            }
            else if (res.Contains("\n"))
            {
                Debug.WriteLine(" new line");
                return "\n";
            }
            return string.Empty;
        }
        // 파일에서 한줄 읽어온다.
        public string readLine()
        {
            string res = file.ReadLine();
            Debug.WriteLine("ReadLine : {0} ", res);
            return res;
        }
        // 파일 내 전체 텍스트를 한개의 string에 읽어온다
        public string readAllText(string filePath)
        {
            string text = System.IO.File.ReadAllText(filePath);
            Debug.WriteLine("readAllText : {0} ", text);
            return text;
        }
        /// <summary>
        ///  파일 내 모든 string lines 읽어온다.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>string array</returns>
        public string[] readLines(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);
            return lines;
        }

        ~MyClass_Files_Reader()
        {
            file.Close();
        }
    }

    /// <summary>
    /// writing log to output windows of VS
    /// </summary>
    public class MyClass_Dprint
    {
        public static void debugPrint(string str)
        {
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(str);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name + "] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(str);
        }
        public static void debugPrint(object obj)
        {
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(obj);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name + "] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(obj);
        }
        public static void debugPrint(string format, params object[] arg)
        {
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(format, arg);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name + "] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(format, arg);
        }

        public static void debugPrint(string format, object arg0)
        {
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(format, arg0);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name + "] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(format, arg0);
        }
    }


    class MyClass_Files
    {
        public MyClass_Files()
        {

        }
        ~MyClass_Files() { }
        // 폴더 내 전체 파일 스캔
        public static void TreeScan(string sDir)
        {
            foreach (string f in System.IO.Directory.GetFiles(sDir))
            {
                Debug.WriteLine("File: " + f); // or some other file processing
            }

            foreach (string d in System.IO.Directory.GetDirectories(sDir))
            {
                Debug.WriteLine("dir: " + d); // 디렉토리 name 출력
                TreeScan(d); // recursive call to get files of directory
            }
        }
        /// <summary> 
        /// 서브 폴더에서 입력한 이름의 file의 전체경로 제공
        /// 조건 : 해당 폴더들 내에서 file 이름은 단일 존재한다.
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string findFileFromSubFolder(string targetPath, string fileName)
        {
            string res = string.Empty;
            string scanedFileName = string.Empty;
            foreach (string f in System.IO.Directory.GetFiles(targetPath))
            {
                Debug.WriteLine("File: " + f); // or some other file processing
                                        // 파일 name 출력
                scanedFileName = System.IO.Path.GetFileName(f);
                if (scanedFileName.Equals(fileName))
                {
                    return f;
                }
            }

            foreach (string d in System.IO.Directory.GetDirectories(targetPath))
            {
                Debug.WriteLine("dir: " + d); // 디렉토리 name 출력
                string resSub = findFileFromSubFolder(d, fileName); // recursive call to get files of directory
                if (!resSub.Equals(string.Empty))
                    return resSub;
            }
            return res;
        }
        /// <summary>
        /// 지정된 폴더 내의 지정된 형식(filePrefix_x.x.x) 지정한 확장자의 파일 들을 스캔
        /// </summary>
        public static string[] scanFolderAndUpdate_Filelists(string targetPath, string extension)
        {
            List<string> strings = new List<string>();
            if (!System.IO.Directory.Exists(targetPath)) // 없으면
            {
                Debug.WriteLine("error. tar folder not found. ");
                return strings.ToArray();
            }
            string[] filePaths = System.IO.Directory.GetFiles(targetPath + "\\", "*." + extension);
            int lengthA = filePaths.Length;
            // 내림차순 정렬... 
            var desc = from s in filePaths
                       orderby s descending
                       select s;
            foreach (string s in desc)
            {
                // get file name only
                string fileName = System.IO.Path.GetFileNameWithoutExtension(s);
                // 파일 이름에서 특정 패턴 매칭하여 버전등의 정보만 추출
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(fileName, @"filePrefix_([A-Za-z0-9\-\.]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    // Finally, we get the Group value and display item
                    string key = match.Groups[1].Value;
                    // add to string list.
                    strings.Add(key);
                }
            }
            if (strings.Count <= 0)
            {
                Debug.WriteLine("[scanFolder] error. no file in dir. ");
                return strings.ToArray();
            }
            return strings.ToArray();
        }
    }//end class


    class MyClass_Logger
    {
        private static Mutex mutex = new Mutex();
        private System.IO.StreamWriter output;
        /// <summary>
		/// FileLogger 생성자, Application 실행 디렉토리/log 에 파일을 생성
		/// </summary>
		/// <param name="name">file path</param>
		public MyClass_Logger(string name)
        {
            Debug.WriteLine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            Debug.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            Debug.WriteLine(System.Environment.CurrentDirectory);
            Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());
            Debug.WriteLine(Environment.CurrentDirectory);

            //To get the location the assembly normally resides on disk or the install directory
            string _path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            //once you have the path you get the directory with:
            var directory = System.IO.Path.GetDirectoryName(_path);
            // string absPath = new Uri(directory).AbsolutePath;
            directory = directory.Replace(@"file:\", "");
            string path = directory + "\\log\\";
            string fullname = path + name;

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            output = new System.IO.StreamWriter(fullname, true);
        }

        /// <summary>
        /// 목록 전체 기록을 위한 WriteEntry 구현
        /// </summary>
        /// <param name="entry"></param>
        public void WriteEntry(System.Collections.ArrayList entry)
        {
            try
            {
                mutex.WaitOne();

                System.Collections.IEnumerator line = entry.GetEnumerator();
                while (line.MoveNext())
                {
                    output.WriteLine(line.Current);
                }
                output.WriteLine();
                output.Flush();

                mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 한줄 기록을 위한 WriteEntry 구현
        /// </summary>
        /// <param name="entry"></param>
        public void WriteEntry(string entry)
        {
            try
            {
                mutex.WaitOne();

                output.WriteLine("[" + DateTime.Now.ToString() + "]" + entry); // 로그 포맷~
                output.Flush();

                mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Logger Close
        /// </summary>
        public void Close()
        {
            this.output.Flush();
            this.output.Close();
        }
    }

}
