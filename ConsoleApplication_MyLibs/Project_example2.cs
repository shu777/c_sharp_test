using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics; // debug print
using System.Net; // server
using System.Net.Sockets; //server
using System.IO; // stream reader

/// <summary>
/// 콘솔또는클라이언트에서요청한 빅파일을찾기 기능
/// 압축및암호화처리 기능 암호화는 두가지 방식 지원
/// 파일로출력또는요청 클라이언트에 전송 기능
/// 소켓 서버에서는 ACK ERR NUM의 3가지 커맨드 처리 
/// </summary>
namespace ConsoleApplication_MyLibs
{
    class Project_example2
    {

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
        static string encryptStrMethod2(string sourceStr, string keyword)
        {
            string result = "";
            List<char> keyword_data = keyword.ToCharArray().ToList<char>();
            List<char> convert_table = new List<char>();
            char[] chars = sourceStr.ToArray();

            foreach (char ch in keyword_data)
            {
                convert_table.Add(ch); // 키워드를 맨 앞에
            }
            for (int i = 0; i < 'Z' - 'A' + 1; i++)
            {
                char current = (char)('A' + i);
                if (convert_table.Contains(current))
                {
                    // 키워드에 해당하는 알파벳 제외
                }
                else
                {
                    convert_table.Add(current); // 나머지 알파벳 추가
                }
            }
            // 변환
            foreach (char ch in chars)
            {
                if (ch >= 'A' && ch <= 'Z')
                {
                    result += convert_table[ch - 'A'];
                    //if (ch - shift < 'A')
                    //    result += (char)((ch - shift) + ('Z' - 'A' + 1));
                    //else
                    //    result += (char)((ch - shift));
                }
                else
                    result += ch;
            }

            return result;

        }
        static string encryptStrMethod1(String sourceStr, int shift)
        {
            char[] chars = sourceStr.ToArray();
            string result = "";
            foreach (char ch in chars)
            {
                if (ch >= 'A' && ch <= 'Z')
                {
                    if (ch - shift < 'A')
                        result += (char)((ch - shift) + ('Z' - 'A' + 1));
                    else
                        result += (char)((ch - shift));
                }
                else
                    result += ch;
            }
            //char[] ctables = new char[] { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z };
            // 0                                                                          25//
            return result;

        }

        public static void StartSocketServer()
        {
            NetworkStream stream = null;
            TcpListener tcpListener = null;
            StreamReader reader = null;
            StreamWriter writer = null;
            Socket clientsocket = null;

            string inputFileName = "ABCDFILE.TXT";
            string outputFileName = inputFileName;
            string securityKey = "LGCNS";
            string inputMode = "";
            List<dataLine> dataLines = new List<dataLine>();

            try
            {
                //IP주소를 나타내는 객체를 생성,TcpListener를 생성시 인자로 사용할려고
                IPAddress ipAd = IPAddress.Parse("127.0.0.1");
                //TcpListener Class를 이용하여 클라이언트의 연결을 받아 들인다.
                tcpListener = new TcpListener(ipAd, 9876);
                Debug.WriteLine("=============== start server \n ");
                Debug.WriteLine("=============== wait for connection \n ");
                tcpListener.Start();
                //Client의 접속이 올때 까지 Block 되는 부분, 대개 이부분을 Thread로 만들어 보내 버린다.
                //백그라운드 Thread에 처리를 맡긴다.
                clientsocket = tcpListener.AcceptSocket();
                //클라이언트의 데이터를 읽고, 쓰기 위한 스트림을 만든다.
                stream = new NetworkStream(clientsocket);
                // Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                reader = new StreamReader(stream);//, encode);
                writer = new StreamWriter(stream);//, encode);

                Debug.WriteLine("client connected \n ");
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;

                ///// cmd 처리 위한 flags /////
                bool startRequest = false;
                int currentPosition = 0;

                while (true)
                {
                    do
                    {
                        numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                    }
                    while (stream.DataAvailable);
                    Debug.WriteLine(myCompleteMessage.ToString());

                    ///////////  인력받은 커맨드 파싱 /////////////
                    if (startRequest == false)
                    {
                        if (myCompleteMessage.ToString().Contains("#"))
                        {
                            string[] commands = myCompleteMessage.ToString().Split('#');
                            inputFileName = commands[0];
                            securityKey = commands[1];
                        }
                        // parsing cmd
                        getDataMain(inputFileName, inputMode, securityKey, ref dataLines, 0);
                        startRequest = true;
                        writer.WriteLine(dataLines[currentPosition].final_data); writer.Flush();
                        currentPosition++;
                    }
                    else
                    {
                        int line_number = 0;
                        if (myCompleteMessage.ToString().Equals("ACK"))
                        {
                            writer.WriteLine(dataLines[currentPosition].final_data); writer.Flush();
                            currentPosition++;
                            if (currentPosition >= dataLines.Count)
                                break;
                        }
                        else if (myCompleteMessage.ToString().Equals("ERR"))
                        {
                            currentPosition--; // 재송신
                            writer.WriteLine(dataLines[currentPosition].final_data); writer.Flush();
                            currentPosition++;
                            if (currentPosition >= dataLines.Count)
                                break;
                        }
                        else if (int.TryParse(myCompleteMessage.ToString(), out line_number))
                        {
                            Debug.WriteLine("=re parsing with new line:" + line_number);
                            // parsing cmd
                            dataLines.Clear();
                            currentPosition = 0;
                            getDataMain(inputFileName, inputMode, securityKey, ref dataLines, line_number - 1);
                            startRequest = true;
                            writer.WriteLine(dataLines[currentPosition].final_data); writer.Flush();
                            currentPosition++;
                        }
                        else
                        {
                            Debug.WriteLine("=Unknown cmd=");
                        }
                    }

                    myCompleteMessage.Clear(); // 버퍼클리어
                    // 다시 읽기 대기 모드 //
                }
                /*
                while (true)
                {
                    writer.WriteLine("WELCOM");
                    writer.Flush();
                    string str = reader.ReadLine(); // 보내는 쪽에서 개행을 넣지 않으면 block 상태로 영영 돌아오지 않는다.
                    Debug.WriteLine(str);
                    //getDataMain(inputFileName, inputMode, securityKey, ref dataLines);
                    writer.WriteLine(str);
                }
                */
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            finally
            {
                clientsocket.Close();
            }
        }
        
        static void getDataMain(string inputFileName, string inputMode, string securityKey, ref List<dataLine> dataLines, int start_line)
        {
            // 4.1. 중복 확인하여 count 까지 추가 후 list 형식으로 변환 
            //[// 연속된 중복라인 을제거하고 중복라인수를 라인앞에추가 하는 압축 example]

            // 1. 사용자 입력
            string userName = inputFileName;// Console.ReadLine();                                            
            string filePath = findFileFromSubFolder("./BIGFILE", userName);// 2. 폴더 스캔하여 찾기

            // 3. 파일들을 한줄씩 읽어서 array에 저장
            string[] lines = System.IO.File.ReadAllLines(filePath);
            string[] lines_new = new string[lines.Count() - start_line];
            Array.Copy(lines, start_line, lines_new, 0, lines.Count() - start_line);
            lines = lines_new; // 사용자 입력된 라인수부터 새로 파싱/압축/암호화 수행

            foreach (string str in lines) // 각 문자열 별로 갯수를 카운팅            
            {
                if (dataLines.Count > 0 && dataLines[dataLines.Count - 1].data.Equals(str)) // 최근 추가한 리스트와 중복되는지 확인해서 count up
                {
                    dataLines[dataLines.Count - 1].count++;
                }
                //int index = dataLines.FindIndex(x => x.data.Equals(str)); // fail 전체 list에서 중복 찾았었음. 질문 잘못 이해
                //if(index >= 0)
                //{
                //    dataLines[index].count++;
                //}
                else // 최근 추가한 리스트와 중복되지 않으면 리스트에 추가 후 +1
                {
                    dataLine tmp = new dataLine();
                    tmp.data = str;
                    tmp.count++;
                    dataLines.Add(tmp);
                }
            }
            // 4.2. 반복되는 3개 문자 압축
            foreach (dataLine tmp in dataLines)
            {
                char[] charArr = tmp.data.ToCharArray();
                List<strComp> strComps = new List<strComp>();

                foreach (char t in charArr) // 각 char 별로 갯수를 카운팅
                {
                    if (strComps.Count > 0 && strComps[strComps.Count - 1].data.Equals(t)) // 최근 추가한 리스트와 중복되는지 확인해서 count up
                    {
                        strComps[strComps.Count - 1].count++;
                    }
                    else // 최근 추가한 리스트와 중복되지 않으면 리스트에 추가 후 +1
                    {
                        strComp tmpC = new strComp();
                        tmpC.data = t;
                        tmpC.count++;
                        strComps.Add(tmpC);
                    }
                }
                // 출력 결과 가공
                string resultStr = "";
                foreach (strComp tmpStrComp in strComps)
                {
                    if (tmpStrComp.count >= 3) // 알파벳 3개 이상 반복되면  갯수+문자로 압축
                    {
                        resultStr += tmpStrComp.count.ToString();
                        resultStr += tmpStrComp.data;
                    }
                    else // 이외에는 카운팅 숫자만큼 추가
                    {
                        for (int i = 0; i < tmpStrComp.count; i++)
                            resultStr += tmpStrComp.data;
                    }
                }
                tmp.compressed_data = resultStr; // 연속되는 문자 3개 이상인 경우 압축
                // 4.3. 압축된 결과 encryption1 - 5문자 시저 암호화
                tmp.encrypted_data = encryptStrMethod1(tmp.compressed_data, 5);
                // 4.4. 압축된 결과 encryption2 - keyword 암호화
                tmp.encrypted_data_with_keyword = encryptStrMethod2(tmp.compressed_data, securityKey);
                // 최종 결과값 가공
                //final_data
                {
                    //string outputStr = tmp.data; // 5.1. output 파일로 문자열 압축 출력
                    //string outputStr = tmp.compressed_data; // 5.2. output 파일로 문자열 내 문자3회이상 반복 압축 출력
                    //string outputStr = tmp.encrypted_data; // 5.3. output 파일로 문자열 내 시저 암호화 결과 출력
                    string outputStr = tmp.encrypted_data_with_keyword; // 5.4. output 파일로 문자열 내 키워드 암호화 결과 출력
                    if (tmp.count > 1)
                    {
                        tmp.final_data = tmp.count.ToString() + "#" + outputStr;
                        Debug.WriteLine(tmp.count.ToString() + "#" + outputStr);
                    }
                    else
                    {
                        tmp.final_data = outputStr;
                        Debug.WriteLine(outputStr);
                    }
                }
            }
#if false // 파일로 출력
            System.IO.StreamWriter wfile = null;
            wfile = new System.IO.StreamWriter(outputFileName, false);
            foreach (dataLine t in dataLines)
            {
                    wfile.WriteLine(t.final_data);
                    Debug.WriteLine(t.final_data);                   
            }
            wfile.Close();
#endif

            int test = 0;

        }
        class strComp
        {
            public char data { get; set; }
            public int count { get; set; }

            public void Print() // 추가해놓으면 디버깅시 편하다
            {
                Debug.WriteLine("data:" + data + "count:" + count);
            }
        }
        class dataLine
        {
            public string data { get; set; }
            public string compressed_data { get; set; }
            public string encrypted_data { get; set; }
            public string encrypted_data_with_keyword { get; set; }
            public string final_data { get; set; }
            public int count { get; set; }

            public void Print() // 추가해놓으면 디버깅시 편하다
            {
                Debug.WriteLine("data:" + data + "count:" + count);
            }
        }
        public static void runExample2() // 빅파일 처리하는 example
        {
            StartSocketServer();
            // wait for client 
        }
    }
}
