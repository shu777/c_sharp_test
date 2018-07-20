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


namespace ConsoleApplication_MyLibs
{

    /// <summary>
    /// async TCP socket 용
    /// State object for receiving data from remote device. 
    /// </summary>
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
    class MyClass_Networks
    {
        // The port number for the remote device.  
        private const int port = 2012;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone_client_async = new ManualResetEvent(false);
        private static ManualResetEvent sendDone_client_async = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone_client_async = new ManualResetEvent(false);
        public MyClass_Networks()
        {

        }
        ~MyClass_Networks() { }
        // The response from the remote device.  
        private static String response = String.Empty;

        // 동기 클라이언트 소켓
        public static void StartClientSync()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 2012 on the local computer.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];  // ipv6
                IPAddress ipAddress = ipHostInfo.AddressList[1]; // ipv4
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);
                    MyClass_Dprint.debugPrint("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);
                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    MyClass_Dprint.debugPrint("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
                    MyClass_Dprint.debugPrint("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    MyClass_Dprint.debugPrint("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    MyClass_Dprint.debugPrint("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
        }
        public static string StartClientSync(string data)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            string res = string.Empty;
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 2012 on the local computer.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[1]; // ipv4
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    MyClass_Dprint.debugPrint("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    MyClass_Dprint.debugPrint("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    res = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    MyClass_Dprint.debugPrint("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    MyClass_Dprint.debugPrint("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    MyClass_Dprint.debugPrint("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
            return res;
        }
        public static string StartClientSync(string data, bool recvReply)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            string res = string.Empty;
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 2012 on the local computer.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];  // ipv6
                IPAddress ipAddress = ipHostInfo.AddressList[1]; // ipv4
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);
                    MyClass_Dprint.debugPrint("[Client] Socket connected to {0}", sender.RemoteEndPoint.ToString());
                    MyClass_Dprint.debugPrint("[Client] Send data [{0}] to server", data);
                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);
                    //bytesSent = sender.Send(msg);
                    if (recvReply == true) // 서버로 부터 reply 받는 경우
                    {
                        // Receive the response from the remote device.
                        MyClass_Dprint.debugPrint("[Client] wait reply...");
                        int bytesRec = sender.Receive(bytes);
                        MyClass_Dprint.debugPrint("[Client] Echoed data : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
                        res = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    MyClass_Dprint.debugPrint("[Client] ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    MyClass_Dprint.debugPrint("[Client] SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    MyClass_Dprint.debugPrint("[Client] Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint("[Client] {0}", e.ToString());
            }
            return res;
        }
        
        /// <summary>
        /// recv packet 부분이 분리되서 처리
        /// </summary>
        public static void StartAsyncClient()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the   
                /*
				// remote device is "host.contoso.com".  
				IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");// ("host.contoso.com");
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				*/
                //현재 PC의 IP로 대체 

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];  // ipv6
                IPAddress ipAddress = ipHostInfo.AddressList[1]; // ipv4

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback_client_async), client);
                connectDone_client_async.WaitOne();

                // Send test data to the remote device.  
                Send_client_async(client, "This is a test<EOF>");
                sendDone_client_async.WaitOne();

                // Receive the response from the remote device.  
                Receive_client_async(client);
                receiveDone_client_async.WaitOne();

                // Write the response to the console.  
                MyClass_Dprint.debugPrint("Response received : {0}", response);

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
        }
        public static void StartAsyncClient(string data)
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the   
                /*
				// remote device is "host.contoso.com".  
				IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");// ("host.contoso.com");
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				*/
                //현재 PC의 IP로 대체 

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];  // ipv6
                IPAddress ipAddress = ipHostInfo.AddressList[1]; // ipv4

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback_client_async), client);
                connectDone_client_async.WaitOne();

                // Send test data to the remote device.  
                Send_client_async(client, data);
                sendDone_client_async.WaitOne();

                // Receive the response from the remote device.  
                Receive_client_async(client);
                receiveDone_client_async.WaitOne();

                // Write the response to the console.  
                MyClass_Dprint.debugPrint("Response received : {0}", response);

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
        }
        public static string StartAsyncClient(string data, bool recvReply)
        {
            string result = string.Empty;
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the   
                /*
				// remote device is "host.contoso.com".  
				IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");// ("host.contoso.com");
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				*/
                //현재 PC의 IP로 대체 

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];  // ipv6
                IPAddress ipAddress = ipHostInfo.AddressList[1]; // ipv4

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback_client_async), client);
                connectDone_client_async.WaitOne();

                // Send test data to the remote device.  
                Send_client_async(client, data);
                sendDone_client_async.WaitOne();

                if (recvReply == true)
                {
                    // Receive the response from the remote device.  
                    Receive_client_async(client);
                    receiveDone_client_async.WaitOne();

                    // Write the response to the console.  
                    MyClass_Dprint.debugPrint("Response received : {0}", response); // 별도 recv thread
                    result = response;
                }
                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
            return result;
        }

        private static void ConnectCallback_client_async(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                MyClass_Dprint.debugPrint("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone_client_async.Set();
            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
        }

        private static void Receive_client_async(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback_client_async), state);
            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
        }

        private static void ReceiveCallback_client_async(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback_client_async), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone_client_async.Set();
                }
            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
        }

        private static void Send_client_async(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback_client_async), client);
        }

        private static void SendCallback_client_async(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                MyClass_Dprint.debugPrint("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone_client_async.Set();
            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
        }
        //////////////////////////////
        //////////////////////////////
        //////////////////////////////
        // Thread signal.  
        public static ManualResetEvent allDone_server = new ManualResetEvent(false);

        public static void StartServerListeningAsync()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];  // ipv6
            IPAddress ipAddress = ipHostInfo.AddressList[1]; // ipv4
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone_server.Reset();

                    // Start an asynchronous socket to listen for connections.  
#if DEBUG
                    MyClass_Dprint.debugPrint("[Server] Waiting for a connection...");
#endif
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback_server),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone_server.WaitOne();
                }

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint(e.ToString());
            }
#if DEBUG
            //MyClass_Dprint.debugPrint("\nPress ENTER to continue...");
            MyClass_Dprint.debugPrint("[Server]  \nEnd server...");
#endif
            //			Console.Read();  

        }

        public static void AcceptCallback_server(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone_server.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback_server), state);
        }

        /// <summary>
        /// 클라이언트로 부터 recv 처리 callback
        /// </summary>
        /// <param name="ar"></param>
        public static void ReadCallback_server(IAsyncResult ar) // reveive file 
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                // connection이 남아있으면 EOF까지 계속 append되지만, connection close시에 reset됨.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
#if DEBUG
                MyClass_Dprint.debugPrint("[server] Receved: {0}", content);
#endif
                if (content.IndexOf("<EOF>") > -1) // stream end DETECT ***
                //if(content.IndexOf("\r\n") > -1 || content.IndexOf("\n") > -1  || content.IndexOf("\r") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
#if DEBUG
                    MyClass_Dprint.debugPrint("[Server] Read {0} bytes from socket. \n Data : {1}", content.Length, content);
#endif
                    if(content.Contains("test")) // check 샘플
                    {
                        MyClass_Dprint.debugPrint("[Server] test command received! ");
                    }
                    // 받은 메세지를 다시 client로 보낸다.
                    Send_Server(handler, content);
                }
                else // 연속된 data를 받아서 append하는 case.
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback_server), state);
                }
            }
        }

        private static void Send_Server(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);
#if DEBUG
            MyClass_Dprint.debugPrint("[Server] send data:[{0}] to client", data);
#endif
            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback_Server), handler);
        }

        private static void SendCallback_Server(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
#if DEBUG
                MyClass_Dprint.debugPrint("Sent {0} bytes to client.", bytesSent);
#endif
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                MyClass_Dprint.debugPrint("[Server] {0}", e.ToString());
            }
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
            if(System.IO.File.Exists(filePath))
            {
                if(appendWriteOpen)
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
    ///  file reader 
    /// </summary>
    class MyClass_Files_Reader
    {
        System.IO.StreamReader file = null;
        private string mFilePath = string.Empty;
        public MyClass_Files_Reader(string filePath)
        {
            string current_path = System.IO.Directory.GetCurrentDirectory();
            file = new System.IO.StreamReader(filePath);
            mFilePath = filePath;

        }
        /// <summary>
        /// 파일 전체를 읽어서 new line 타입을 최초에 detect 되는 타입으로 판단/return 한다. // 2종류가 혼재되어 있지 않은 것을 전제한다.
        /// </summary>
        /// <returns></returns>
        public string getNewLine()
        {
            //string[] readStr = this.readLines(mFilePath);
            string res = this.readAllText(mFilePath);
            if (res.Contains("\r\n"))
            {
                MyClass_Dprint.debugPrint(" new line with carriage return.");
                return "\r\n"; // 캐리지 리턴이 포함됨.
            }
            else if (res.Contains("\n"))
            {
                MyClass_Dprint.debugPrint(" new line");
                return "\n";
            }
            return string.Empty;
        }
        public string readLine()
        {
            string res = file.ReadLine();
            MyClass_Dprint.debugPrint("ReadLine : {0} ", res);
            return res;
        }
        public string readAllText(string filePath)
        {
            string text = System.IO.File.ReadAllText(filePath);
            MyClass_Dprint.debugPrint("readAllText : {0} ", text);
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
            //lines[0].
            return lines;
        }

        ~MyClass_Files_Reader()
        {
            file.Close();
        }
    }
    /// <summary>
    /// 문자열 처리 클래스
    /// 
    /// </summary>

    class MyClass_Strings
    {
        public MyClass_Strings()
        {

        }
        public static char GetFirstChar(char[] S)
        {
            Dictionary<char, int> ht = new Dictionary<char, int>();

            foreach (char ch in S)
            {
                if (!ht.ContainsKey(ch))
                    ht[ch] = 1;
                else
                    ht[ch] += 1;
            }

            foreach (char ch in S)
            {
                if (ht[ch] == 1)
                    return ch;
            }




            return '\0';
        }
        public static void getStringCombination(string s)
        {
            //StringBuilder는 새로운 객체를 생성하지 않고 자신의 내부 버퍼 값만 변경함으로써 값을 추가
            StringBuilder sb = new StringBuilder();
            StringCombination(s, sb, 0);
        }

        private static void StringCombination(string s, StringBuilder sb, int index)
        {
            for (int i = index; i < s.Length; i++)
            {
                // 1) 한 문자 추가
                sb.Append(s[i]);

                // 2) 구한 문자조합 출력
                MyClass_Dprint.debugPrint(sb.ToString());

                // 3) 나머지 문자들에 대한 조합 구하기
                StringCombination(s, sb, i + 1);

                // 위의 1에서 추가한 문자 삭제 
                sb.Remove(sb.Length - 1, 1);
            }
        }
        /// <summary>
        /// src string 내의 문자들이 target string 내에 순서와 상관없이 포함되어 있는지 확인 // coffee -> foecofee
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <returns>0 매치됨, else 타겟에 몇개 문자 더 있음, -1 타겟에 문자 부족</returns>
        public static int checkCharsInString(string src, string target, ref string result)
        {
            int res = 0;
            string src_tmp = src;
            string target_tmp = target;
            foreach(char c in src) // 각 문자를 순서대로 비교
            {
                int index = target_tmp.IndexOf(c); // 일치하는 문자 위치 찾음
                if(index != -1)
                {
                    MyClass_Dprint.debugPrint("match index:{0}", index);
                    target_tmp = target_tmp.Remove(index, 1); // 일치하는 문자 제거
                }
                else
                {
                    // not found char!!!!
                    return -1; // 문자가 포함되지 않는다면 error return
                }
                //if (target_tmp.Contains(c))
                {

                }

            }
            result = target_tmp; // 제거하고 남은 문자열 return
            if (target_tmp.Length != 0) // 남아있는 문자가 있다면 남은 문자 갯수 return
                res = target_tmp.Length;

            return res; // 문자 수가 일치하면 return 0
        }
        /// <summary>
        ///  remove duplicates from string Array
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        public static string[] RemoveDuplicates(string[] myList)
        {
            System.Collections.ArrayList newList = new System.Collections.ArrayList();

            foreach (string str in myList)
                if (!newList.Contains(str))
                    newList.Add(str);
            return (string[])newList.ToArray(typeof(string));
        }
        /// <summary>
        ///  string to char array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static char[] strToCharArray(string str)
        {
            return str.ToCharArray();
        }
        /// <summary>
        /// string to int
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int strToInt(string number)
        {
            return Convert.ToInt32(number);
        }
        /// <summary>
        /// char to int convert
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int charToInt(char c)
        {
            return (int)(c - '0');
        }
        // 구분자로 string을 나눈다.
        public string[] StringSplit(string src, char delimiter)
        {
            string[] words = src.Split(delimiter);
            return words;
        }
        // 문자열에 해당 문자열이 포함되어 있는지를 체크한다.
        public bool checkContains(string src, string value)
        {
            return src.Contains(value);
        }

        /// <summary>
        /// 스트링 내 해당 문자열 매치 갯수 구한다.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static int countMatches(string src, string pattern)
        {
            int countResult = Regex.Matches(src, pattern).Count;
            return countResult;
        }
        // 로그 내 type 갯수 추출
        public int countingAllTypes(string[] src, char delimiter, int field_number)
        {
            //string[] types = new string[];
            List<string> types = new List<string>();
            // scan types
            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
                //Class_Strings.
                if (types.Any(data[1].Contains)) // 겹치는 타입이 있는 경우는 skip
                {
                    // skip
                }
                else
                {
                    types.Add(data[1]); // new type Add
                }
            }
            return types.Count(); // type count 전달
        }
        // 로그 내 type 종류 추출
        public string[] getAllTypes(string[] src, char delimiter, int field_number)
        {
            //string[] types = new string[];
            List<string> types = new List<string>();
            // scan types
            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
                //Class_Strings.
                if (types.Any(data[1].Contains)) // type list에 존재하는지 확인하고,
                {
                    // 중복되는 경우는 skip
                }
                else
                {
                    types.Add(data[1]); // 없으면 리스트에 추가.
                }
            }
            return types.ToArray();
        }
        // 로그 내 특정 type의 발생 갯수 counting
        public int getCountOfType(string[] src, char delimiter, int field_number, string type)
        {
            int total = 0;
            List<string> types = new List<string>();

            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
                //Class_Strings.
                if (data[field_number].Equals(type))
                    total++;
            }
            return total;
        }
        // 로그 내 특정 타입의 문자열들 추출
        public string[] getLinesOfType(string[] src, char delimiter, int field_number, string type)
        {
            int total = 0;
            List<string> strings = new List<string>();

            foreach (string s in src)
            {
                string[] data = this.StringSplit(s, delimiter);
                //Class_Strings.
                if (data[field_number].Equals(type))
                    //total++;
                    strings.Add(s);
            }
            return strings.ToArray();
        }

        /// <summary>
        /// 스트링을 입력받아 분리하고, data field의 내용을 외부 컨버터를 사용해 변환 후 해당 스트링에 값을 swap하여 반환
        /// </summary>
        /// <param name="src"></param>
        /// <param name="delimiter"></param>
        /// <param name="field_number"></param>
        /// <returns></returns>
        public string convertStringMsg(string src, char delimiter, int field_number)
        {
            string[] data = this.StringSplit(src, delimiter); // 구분자로 나눠서 문자열 분리
            //MyClass_Program ttest = new MyClass_Program();
            string tttt = MyClass_Program.runExternalProgram("CODECONV.EXE", data[field_number]);
            return src.Replace(data[field_number], tttt);// 문자열 내에서 매칭되는 문자를 target문자로 replace한다.
        }
        ~MyClass_Strings()
        {

        }
    }
    /// <summary>
    /// 
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
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                semaphore.WaitOne(); // 세마포어 wait
                try
                {
                    //DownloadItem(item);
                    var t = new System.Threading.Thread(() => RealStart(fileWriter, srcString, delimiter, fieldNumber));
                    t.Start();
                    //return t;
                }
                finally
                {
                    semaphore.Release(); // 세마포어 end
                }
            });
        }
        /// <summary>
        /// thread run 메인
        /// </summary>
        /// <param name="fileWriter"></param>
        /// <param name="srcString"></param>
        /// <param name="delimiter"></param>
        /// <param name="fieldNumber"></param>
        private void RealStart(MyClass_Files_Writer fileWriter, string srcString, char delimiter, int fieldNumber)
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
    /// <summary> 
    /// 링버퍼
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyClass_CircularBuffer<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        private int capacity;
        private int size;
        private int head;
        private int tail;
        private T[] buffer;

        [NonSerialized()]
        private object syncRoot;

        public MyClass_CircularBuffer(int capacity)
            : this(capacity, false)
        {
        }

        public MyClass_CircularBuffer(int capacity, bool allowOverflow)
        {
            if (capacity < 0)
                throw new ArgumentException("The buffer capacity must be greater than or equal to zero.", "capacity");

            this.capacity = capacity;
            size = 0;
            head = 0;
            tail = 0;
            buffer = new T[capacity];
            AllowOverflow = allowOverflow;
        }

        public bool AllowOverflow
        {
            get;
            set;
        }

        public int Capacity
        {
            get { return capacity; }
            set
            {
                if (value == capacity)
                    return;

                if (value < size)
                    throw new ArgumentOutOfRangeException("value", "The new capacity must be greater than or equal to the buffer size.");

                var dst = new T[value];
                if (size > 0)
                    CopyTo(dst);
                buffer = dst;

                capacity = value;
            }
        }

        public int Size
        {
            get { return size; }
        }

        public bool Contains(T item)
        {
            int bufferIndex = head;
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < size; i++, bufferIndex++)
            {
                if (bufferIndex == capacity)
                    bufferIndex = 0;

                if (item == null && buffer[bufferIndex] == null)
                    return true;
                else if ((buffer[bufferIndex] != null) &&
                    comparer.Equals(buffer[bufferIndex], item))
                    return true;
            }

            return false;
        }

        public void Clear()
        {
            size = 0;
            head = 0;
            tail = 0;
        }

        public int Put(T[] src)
        {
            return Put(src, 0, src.Length);
        }

        public int Put(T[] src, int offset, int count)
        {
            if (!AllowOverflow && count > capacity - size)
                throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");

            int srcIndex = offset;
            for (int i = 0; i < count; i++, tail++, srcIndex++)
            {
                if (tail == capacity)
                    tail = 0;
                buffer[tail] = src[srcIndex];
            }
            size = Math.Min(size + count, capacity);
            return count;
        }

        public void Put(T item)
        {
            if (!AllowOverflow && size == capacity)
                throw new InvalidOperationException("The buffer does not have sufficient capacity to put new items.");

            buffer[tail] = item;
            if (++tail == capacity)
                tail = 0;
            size++;
        }

        public void Skip(int count)
        {
            head += count;
            if (head >= capacity)
                head -= capacity;
        }

        public T[] Get(int count)
        {
            var dst = new T[count];
            Get(dst);
            return dst;
        }

        public int Get(T[] dst)
        {
            return Get(dst, 0, dst.Length);
        }

        public int Get(T[] dst, int offset, int count)
        {
            int realCount = Math.Min(count, size);
            int dstIndex = offset;
            for (int i = 0; i < realCount; i++, head++, dstIndex++)
            {
                if (head == capacity)
                    head = 0;
                dst[dstIndex] = buffer[head];
            }
            size -= realCount;
            return realCount;
        }

        public T Get()
        {
            if (size == 0)
                throw new InvalidOperationException("The buffer is empty.");

            var item = buffer[head];
            if (++head == capacity)
                head = 0;
            size--;
            return item;
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(0, array, arrayIndex, size);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (count > size)
                throw new ArgumentOutOfRangeException("count", "The read count cannot be greater than the buffer size.");

            int bufferIndex = head;
            for (int i = 0; i < count; i++, bufferIndex++, arrayIndex++)
            {
                if (bufferIndex == capacity)
                    bufferIndex = 0;
                array[arrayIndex] = buffer[bufferIndex];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            int bufferIndex = head;
            for (int i = 0; i < size; i++, bufferIndex++)
            {
                if (bufferIndex == capacity)
                    bufferIndex = 0;

                yield return buffer[bufferIndex];
            }
        }

        public T[] GetBuffer()
        {
            return buffer;
        }

        public T[] ToArray()
        {
            var dst = new T[size];
            CopyTo(dst);
            return dst;
        }

        #region ICollection<T> Members

        int ICollection<T>.Count
        {
            get { return Size; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<T>.Add(T item)
        {
            Put(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            if (size == 0)
                return false;

            Get();
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICollection Members

        int ICollection.Count
        {
            get { return Size; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (syncRoot == null)
                    Interlocked.CompareExchange(ref syncRoot, new object(), null);
                return syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CopyTo((T[])array, arrayIndex);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        #endregion
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

    /// <summary>
    /// 
    /// 로그를 파싱 하는 test sample. 파싱 구분자 # 으로 고정
    /// </summary>
    class MyClass_Parse_Log
    {
        private char delimiter;
        public MyClass_Parse_Log()
        {
            delimiter = '#'; // 로그 파싱 할 구분자
        }
        public MyClass_Parse_Log(char _delimiter)
        {
            delimiter = _delimiter; // 로그 파싱 할 구분자
        }
        /// <summary>
        /// 샘플 1 솔루션
        /// </summary>
        /// <param name="inputLog"></param>
        /// <param name="outputTxt"></param>
        public void MyClass_Parse_And_Report1(string inputLog, string outputTxt)
        {
            //////////////////////////////////////////////////////
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
            MyClass_Dprint.debugPrint("read lines:" + num);
            ///////////////////////////////////////////////////////
            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = { "EE", "WW", "SS" }; // 세개의 로그타입
            ////////////////////////////////////////////////////////
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(outputTxt, true))
            {
                // scan types
                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
        public void MyClass_Parse_And_Report2(string inputLog, string outputTxt)
        {
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
            MyClass_Dprint.debugPrint("read lines:" + num);

            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = strClass.getAllTypes(line, delimiter, 1);

            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile =
                new System.IO.StreamWriter(outputTxt, true))
            {
                //outFile.WriteLine("Fourth line");
                // scan types
                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
        public void MyClass_Parse_And_Report3(string inputLog, string outputTxt)
        {
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
            MyClass_Dprint.debugPrint("read lines:" + num);

            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = strClass.getAllTypes(line, delimiter, 1);
            MyClass_Thread myThread = new MyClass_Thread();
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile =
                new System.IO.StreamWriter(outputTxt, false))
            {

                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);

                    string[] strResult = strClass.getLinesOfType(line, delimiter, 1, s);// 타입별 로그를 추출한다.
                    foreach (string _line in strResult)
                    {

                        // convert and write to file
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter("TYPELOG_3_" + s + ".TXT", false))
                        {
                            string convertedStr = strClass.convertStringMsg(_line, delimiter, 2);
                            {
                                file.WriteLine(convertedStr);
                            }
                        }

                    }
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
        public void MyClass_Parse_And_Report4(string inputLog, string outputTxt)
        {
            // log 포맷 시간(19)#타입(2)#메시지코드(9)
            // 입력받은 text log 파일을 열어, #으로 split 한 후 각 type 별로 분리하고 카운팅한 결과를 output text에 저장
            MyClass_Files_Reader srcFile = new MyClass_Files_Reader(inputLog);
            MyClass_Strings strClass = new MyClass_Strings();
            string res = srcFile.readLine(); // 1 line read
            string[] line = srcFile.readLines(inputLog); // read all lines
            int num = line.Count(); // total count
            MyClass_Dprint.debugPrint("read lines:" + num);

            int typeCount = strClass.countingAllTypes(line, delimiter, 1);
            string[] logTypes = strClass.getAllTypes(line, delimiter, 1);
            MyClass_Thread myThread = new MyClass_Thread();
            // Example #4: Append new text to an existing file.
            // The using statement automatically flushes AND CLOSES the stream and calls 
            // IDisposable.Dispose on the stream object.
            using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(outputTxt, false))
            {
                foreach (string s in logTypes)
                {
                    //string[] data = strClass.StringSplit(s, delimiter);
                    int cnt = strClass.getCountOfType(line, delimiter, 1, s);
                    // 타입별 출력 파일 생성
                    MyClass_Files_Writer fileWriter = new MyClass_Files_Writer("TYPELOG_4_" + s + ".TXT");
                    // file writer 클래스에 thread safty하도록 lock 추가. (여러 thread에서 동시에 write의 경우 처리)


                    string[] strResult = strClass.getLinesOfType(line, delimiter, 1, s);// 입력된 타입에 해당되는 로그 만 추출한다.
                    foreach (string _line in strResult)
                    {
                        // 이부분 multi thread로 바꾼다.
                        MyClass_Dprint.debugPrint("type: " + s);
                        MyClass_Dprint.debugPrint("line: " + _line);
                        myThread.StartConvertAndWrite(fileWriter, _line, delimiter, 2);
                    }
                    MyClass_Dprint.debugPrint("type:" + s + " total num:" + cnt);
                    outFile.WriteLine(s + "#" + cnt);
                }
            }
        }
    }

    public class MyClass_Security
    {
        public MyClass_Security()
        {

        }
        ~MyClass_Security()
        {

        }
        public static string encodeBase64(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
        public static string decodeBase64(string value)
        {
            return Convert.FromBase64String(value).ToString();
        }
        public static string CaesarCipherEncrypt(string value, int shift)
        {
            bool isLowLetter = false;

            char[] buffer = value.ToCharArray();
            for (int i = 0; i < buffer.Length; i++)
            {
                // Letter.
                char letter = buffer[i];
                isLowLetter = char.IsLower(letter);
                if(!char.IsLetter(letter)) // encrypt only letter 
                {
                    buffer[i] = letter;
                    continue;
                }
                // Add shift to all.
                letter = (char)(letter + shift);
                // Subtract 26 on overflow.
                // Add 26 on underflow.
                if (isLowLetter)
                {
                    if (letter > 'z')
                    {
                        letter = (char)(letter - 26);
                    }
                    else if (letter < 'a')
                    {
                        letter = (char)(letter + 26);
                    }
                }
                else
                {
                    if (letter > 'Z')
                    {
                        letter = (char)(letter - 26);
                    }
                    else if (letter < 'A')
                    {
                        letter = (char)(letter + 26);
                    }
                }
                // Store.
                buffer[i] = letter;
            }
            return new string(buffer);
        }
        public static string Decrypt(string textToDecrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
        public static string Encrypt(string textToEncrypt, string key)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }
    }

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
#if DEBUG
            MyClass_Dprint.debugPrint(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            MyClass_Dprint.debugPrint(System.AppDomain.CurrentDomain.BaseDirectory);
            MyClass_Dprint.debugPrint(System.Environment.CurrentDirectory);
            MyClass_Dprint.debugPrint(System.IO.Directory.GetCurrentDirectory());
            MyClass_Dprint.debugPrint(Environment.CurrentDirectory);
#endif
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
                MyClass_Dprint.debugPrint(e.Message);
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
                MyClass_Dprint.debugPrint(e.Message);
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
                MyClass_Dprint.debugPrint("File: " + f); // or some other file processing
                                                 // 파일 name 출력
            }

            foreach (string d in System.IO.Directory.GetDirectories(sDir))
            {
#if DEBUG
                MyClass_Dprint.debugPrint("dir: " + d); // 디렉토리 name 출력
#endif
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
#if DEBUG
                MyClass_Dprint.debugPrint("File: " + f); // or some other file processing
                                                 // 파일 name 출력
#endif
                scanedFileName = System.IO.Path.GetFileName(f); 
                if(scanedFileName.Equals(fileName))
                {
                    return f;
                }
            }

            foreach (string d in System.IO.Directory.GetDirectories(targetPath))
            {
#if DEBUG
                MyClass_Dprint.debugPrint("dir: " + d); // 디렉토리 name 출력
#endif
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
#if DEBUG
                MyClass_Dprint.debugPrint("error. tar folder not found. ");
#endif
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
#if DEBUG
                MyClass_Dprint.debugPrint("[scanFolder] error. no file in dir. ");
#endif
                return strings.ToArray();
            }
            return strings.ToArray();
        }
    }//end class
    /// <summary>
    /// 링크드 리스트의 예
    /// </summary>
    public class MyClass_linkedList
    {
      
        public static void test()
        {
            LinkedList<string> list = new LinkedList<string>();
            list.AddLast("ZApple");
            list.AddLast("CBanana");
            list.AddLast("Lemon");

            LinkedListNode<string> node = list.Find("Banana");
            LinkedListNode<string> newNode = new LinkedListNode<string>("Grape");

            // 새 Grape 노드를 Banana 노드 뒤에 추가
            if(node != null)
                list.AddAfter(node, newNode);

            // 리스트 출력
            list.ToList().ForEach(p => MyClass_Dprint.debugPrint(p));

            // Enumerator를 이용한 리스트 출력
            foreach (var m in list)
            {
                MyClass_Dprint.debugPrint(m);
            }
        }

    }

    
    /// <summary>
    /// 1원, 10원, 50원, 100원짜리 동전이 제한없이 있다고 가정했을 때, 총 90원을 만드는 방법의 수를 구하는 함수
    /// </summary>
    public class MyClass_coinChangeCount
    {
        public void Test()
        {
            int[] coins = { 1, 10, 50, 100 };

            // CoinChangeCount c = new CoinChangeCount();
            // 1.  Recursive 방식 해결
            int ans = this.Count(coins, 4, 90);
            MyClass_Dprint.debugPrint("Count={0}", ans);

            // 2. 중간 결과를 저장(Memoization)하여 Lookup하는 방식 해결
            Dictionary<Tuple<int, int>, int> hash = new Dictionary<Tuple<int, int>, int>();
            ans = this.DPCount(coins, 4, 90, hash);
            MyClass_Dprint.debugPrint("DPCount={0}", ans);
        }

        public int Count(int[] coins, int m, int n)
        {
            if (n == 0) return 1;
            if (n < 0) return 0;
            if (m <= 0) return 0;
            MyClass_Dprint.debugPrint("m:{0}, n:{1}", m, n);

            return Count(coins, m - 1, n) + Count(coins, m, n - coins[m - 1]);
        }

        public int DPCount(int[] coins, int m, int n, Dictionary<Tuple<int, int>, int> hash)
        {
            if (n == 0) return 1;
            if (n < 0) return 0;
            if (m <= 0) return 0;

            Tuple<int, int> pair = new Tuple<int, int>(m, n);
            if (!hash.ContainsKey(pair))
            {
                int result = DPCount(coins, m - 1, n, hash) + DPCount(coins, m, n - coins[m - 1], hash);
                hash.Add(pair, result);
            }
            return hash[pair];
        }
    }
    public class SimpleHashTable
    {
        private const int INITIAL_SIZE = 16;
        private int size;
        private Node[] buckets;

        public SimpleHashTable()
        {
            this.size = INITIAL_SIZE;
            this.buckets = new Node[size];
        }

        public SimpleHashTable(int capacity)
        {
            this.size = capacity;
            this.buckets = new Node[size];
        }

        public void Put(object key, object value)
        {
            int index = HashFunction(key);
            if (buckets[index] == null)
            {
                buckets[index] = new Node(key, value);
            }
            else
            {
                Node newNode = new Node(key, value);
                newNode.Next = buckets[index];
                buckets[index] = newNode;
            }
        }

        public object Get(object key)
        {
            int index = HashFunction(key);

            if (buckets[index] != null)
            {
                for (Node n = buckets[index]; n != null; n = n.Next)
                {
                    if (n.Key == key)
                    {
                        return n.Value;
                    }
                }
            }
            return null;
        }

        public bool Contains(object key)
        {
            int index = HashFunction(key);
            if (buckets[index] != null)
            {
                for (Node n = buckets[index]; n != null; n = n.Next)
                {
                    if (n.Key == key)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual int HashFunction(object key)
        {
            return Math.Abs(key.GetHashCode() + 1 +
                (((key.GetHashCode() >> 5) + 1) % (size))) % size;
        }

        private class Node
        {
            public object Key { get; set; }
            public object Value { get; set; }
            public Node Next { get; set; }

            public Node(object key, object value)
            {
                this.Key = key;
                this.Value = value;
                this.Next = null;
            }
        }
    }
    /// <summary>
    /// 해시 테이블 사용
    /// </summary>
    public class MyClass_hashMap
    {
        struct Gameharacter
        {
            //Gameharacter(){ }
            public Gameharacter(int CharCd, int Level, int Money)
            {
                _CharCd = CharCd;
                _Level = Level;
                _Money = Money;
            }
            public int _CharCd;
            public int _Level;
            public int _Money;
        };
        /*
        //create comparer
        internal class PersonComparer : IComparer<Gameharacter>
        {
            public int Compare(Gameharacter x, Gameharacter y)
            {
                //first by age
                int result = x._CharCd.CompareTo(y._CharCd);

                //then name
                if (result == 0)
                    result = x._Level.CompareTo(y._Level);

                //a third sort
                if (result == 0)
                    result = x._Money.CompareTo(y._Money);

                return result;
            }
        }
        */
        public void test()
        {
            Hashtable ht = new Hashtable();
            ht.Add("irina", "Irina SP");
            ht.Add("tom", "Tom Cr");

            if (ht.Contains("tom"))
            {
                MyClass_Dprint.debugPrint(ht["tom"]);
            }
            ///////////////////////////////

            Hashtable htt = new Hashtable(); // 해시 테이블 생성
            Gameharacter Character1 = new Gameharacter(12, 7, 1000); // 구조체1
            htt.Add(12, Character1); // 추가
            Gameharacter Character2 = new Gameharacter(5, 200, 111000); // 구조체  2
            htt.Add(15, Character2); // 추가
            Gameharacter Character3 = new Gameharacter(200, 34, 3345000); // 구조체 3
            htt.Add(200, Character3); // 추가

            MyClass_Dprint.debugPrint("before");
            foreach (DictionaryEntry entry in htt) // 상태 출력
            {
                Gameharacter getStr = (Gameharacter)entry.Value;
                MyClass_Dprint.debugPrint(entry.Key + ":" + getStr._CharCd);
            }
            htt.Remove(200); // key 200 제거
            MyClass_Dprint.debugPrint("after");
            foreach (DictionaryEntry entry in htt) // 상태 출력
            {
                Gameharacter getStr = (Gameharacter)entry.Value;
                MyClass_Dprint.debugPrint(entry.Key + ":" + getStr._CharCd);
            }
            // IDictionary<string, Gameharacter> d = htt; // your hash table
            //var ordered = d.OrderBy(p => p.Key).ToList();
            // foreach (var p in ordered)
            // {
            //    MyClass_Dprint.debugPrint("Key: {0} Value: {1}", p.Key, p.Value);
            //  }
            SortedList sorter2 = new SortedList(htt);
            foreach (DictionaryEntry entry in htt) // 상태 출력
            {
                Gameharacter getStr = (Gameharacter)entry.Value;
                MyClass_Dprint.debugPrint(entry.Key + ":" + getStr._CharCd + "|" + getStr._Level + ":"+getStr._Money);
            }


            int test = 0;
        }
    }
    /// <summary>
    /// writing log to output windows of VS
    /// </summary>
    public class MyClass_Dprint
    {
        public static void debugPrint(string str)
        {
#if DEBUG
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(str);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name + "] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(str);
#endif
        }
        public static void debugPrint(object obj)
        {
#if DEBUG
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(obj);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name+"] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(obj);
#endif
        }
        public static void debugPrint(string format, params object[] arg)
        {
#if DEBUG
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(format, arg);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name + "] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(format, arg);
#endif
        }
  
        public static void debugPrint(string format, object arg0)
        {
#if DEBUG
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            //Console.WriteLine(format, arg0);
            string outputStr = "[Debug:" + stackTrace.GetFrame(1).GetMethod().Name + "] ";
            System.Diagnostics.Debug.Write(outputStr);
            System.Diagnostics.Debug.WriteLine(format, arg0);
#endif
        }
    }
    /// <summary>
    /// 
    /// array를 list로 변환하여 핸들링
    /// </summary>
    //public class MyClass_list_sort<T>// : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    public class MyClass_list_sort
    {
        class Product_struct
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }

        }       
        static public void test()
        {
            //Product_struct[] Product_structArray = new[24] Product_struct;
            List<Product_struct> Product = new List<Product_struct>();
            //Product_struct.Add();
            var tmp = new Product_struct();
            tmp.ProductID = 001;
            tmp.ProductName = "바둑이";
            Product.Add(tmp);
            var tmp1 = new Product_struct();
            tmp1.ProductID = 001;
            tmp1.ProductName = "바둑이2";
            Product.Add(tmp1);
            var tmp2 = new Product_struct();
            tmp2.ProductID = 010;
            tmp2.ProductName = "다람쥐";
            Product.Add(tmp2);
            var tmp22 = new Product_struct();
            tmp22.ProductID = 010;
            tmp22.ProductName = "다람쥐2";
            Product.Add(tmp22);

            /// SORT
            //람다식 sort
            Product.Sort((p1, p2) => p1.ProductName.CompareTo(p2.ProductName));
            //람다식 sort2
            Product.Sort((p1, p2) => p1.ProductID.CompareTo(p2.ProductID));
            // 또 다른 방식의 sort // 지정한 structure member를 기준으로 sorting
            List<Product_struct> _Product = new List<Product_struct>();
            _Product = Product.OrderBy(order => order.ProductName).ToList(); // IEnumerable을 반환하니 list로 변환

            /// 중복제거
            // list내 중복 제거 // 전체가 완전히 같은 경우만 제거
            List<Product_struct> _Product_struct1 = new List<Product_struct>();
            _Product_struct1 = Product.Distinct().ToList();

            // list내 중복 제거 with ramda // 지정한 structure member를 기준으로 중복확인/제거
            List<Product_struct> _Product_struct2 = new List<Product_struct>();
            _Product_struct2 = Product.GroupBy(c => c.ProductID).Select(grp => grp.First()).ToList();

            /// 검색
            // list 내 검색 방법
            Product_struct result = _Product.Find(x => x.ProductID == 001);

            /// 검색 & count // list내에서 다람쥐로 시작하는 data 갯수 구함
            int count = Product.FindAll(x => x.ProductName.StartsWith("다람쥐")).Count;

            // list 내 검색 + 제거 
            _Product.RemoveAll(x => x.ProductID == 001);
            int test_break = 0;
        }


    }
    public class myClassBlockChainHeader
    {
        public myClassBlockChainHeader(byte[] previousBlockHash, object[] transactions)
        {
            this.previousBlockHash = previousBlockHash;
            this.merkleRootHash = transactions.GetHashCode();
        }
        /// <summary>
        /// 소프트웨어, 또는 프로토콜 등의 업그레이드를 추적하기 위해 사용되는 버전 정보
        /// </summary>
        private int version;
        /// <summary>
        /// 블록체인 상의 이전 블록(부모 블록)의 해시값
        /// </summary>
        private byte[] previousBlockHash;
        /// <summary>
        /// 머클트리의 루트에 대한 해시값
        /// </summary>
        private int merkleRootHash;
        /// <summary>
        /// 해당 블록의 생성 시각
        /// </summary>
        private int timestamp;
        /// <summary>
        ///  채굴과정에서 필요한 작업 증명(Proof of Work) 알고리즘의 난이도 목표
        /// </summary>
        private static uint difficultyTarget = 5;
        /// <summary>
        ///  채굴과정의 작업 증명에서 사용되는 카운터
        /// </summary>
        private static int nonce = 0;


        /// <summary>
        /// 작업증명
        /// </summary>
        public int ProofOfWorkCount()
        {
            using (SHA256Managed hashstring = new SHA256Managed())
            {
                byte[] bt;
                string sHash = string.Empty;
                while (sHash == string.Empty || sHash.Substring(0, (int)difficultyTarget) != ("").PadLeft((int)difficultyTarget, '0'))
                {
                    bt = Encoding.UTF8.GetBytes(merkleRootHash + nonce.ToString());
                    sHash = MyClassBlockChain.ByteArrayToString(hashstring.ComputeHash(bt));
                    nonce++;
                }
                return nonce;
            }
        }

        /// <summary>
        /// 거래내역을 byte로 반환한다.
        /// </summary>
        /// <returns></returns>
        public byte[] toByteArray()
        {
            string tmpStr = "";
            //이전 블록이 있다면 처음에 해시를 추가한다.
            if (previousBlockHash != null)
            {
                tmpStr += Convert.ToBase64String(previousBlockHash);
            }
            tmpStr += merkleRootHash.ToString();
            return Encoding.UTF8.GetBytes(tmpStr);
        }

    }
    public class MyClassBlockChain
    {
        public MyClassBlockChain(myClassBlockChainHeader blockHeader, Object[] transactions)
        {
            this.blockHeader = blockHeader;
            this.transactions = transactions;
        }

        private int blockSize;
        private myClassBlockChainHeader blockHeader;
        private int transactionCount;
        private object[] transactions;

        /// <summary>
        /// 블록 헤더를 해시화하여 반환한다.
        /// </summary>
        /// <returns></returns>
        public string getBlockHash()
        {
            byte[] bytes = blockHeader.toByteArray();
            using (SHA256Managed hashstring = new SHA256Managed())
            {
                // 해시화를 두 번 연속으로 설정.
                byte[] blockHash = hashstring.ComputeHash(bytes);
                blockHash = hashstring.ComputeHash(blockHash);

                return ByteArrayToString(blockHash);
            }
        }

        /// <summary>
        /// 바이트어레이를 String으로 변환
        /// </summary>
        /// <param name="bts"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] bts)
        {
            StringBuilder strBld = new StringBuilder();
            foreach (byte bt in bts)
                strBld.AppendFormat("{0:X2}", bt);

            return strBld.ToString();
        }
    }
        
        
    /// <summary>
    /// 2차원 배열을 사용한 sort 
    /// </summary>
    public class MyClass_array_sort
    {
        // 2차원 배열 input 받아 sorting 후 2차원 배열로 output
        public static int[][]sortedArray(int [][]inData)
        {
            int ln_length = inData.GetLength(0); // 배열 크기
            int ln_length2 = inData[0].GetLength(0); // 배열 크기

            int[][] outArr = new int[ln_length][];
            for (int i = 0; i < inData.GetLength(0); i++)
            {
                outArr[i] = (int[])inData[i].Clone(); // array 복제 (반환되는 object를 array 타입케스팅하여 사용한다.)
            }

            Comparer<int> comparer = Comparer<int>.Default;
            // 람다식을 이용한 IComparer 구현
            Array.Sort<int[]>(inData, (x, y) => comparer.Compare(x[1], y[1])); // 1번째 배열로 오름차순 정렬
            //Array.Sort<int[]>(inData, (x, y) => comparer.Compare(x[0], y[0])); // 0번째 배열로 오름차순 정렬
            Array.Reverse(inData); // 내림차순 정렬로 변환

            // 이중 for문으로 접근
            for(int i=0;i<inData.GetLength(0); i++)
            {
                for(int ii=0; ii< inData[0].GetLength(0); ii++)
                {
                    MyClass_Dprint.debugPrint("sorted array>" + inData[i][ii]);
                }
                MyClass_Dprint.debugPrint("###");
            }
            return inData;
        }
        
        /// <summary>
        /// array sorting 예제
        /// </summary>
        public static void test()
        {
            // could just as easily be string...
            int[][] data = new int[][] {
                new int[] {10,100},
                new int[] {2,30},
                new int[] {3,500},
                new int[] {4,70}
            };
            int[][] outData = MyClass_array_sort.sortedArray(data);
            int test = 0;
        }
    }
    public class MyClass_Book
    {
        public int BNum
        {
            get;
            private set;
        }
        public string Title
        {
            get;
            private set;
        }
        public string Author
        {
            get;
            private set;
        }
        public string Publisher
        {
            get;
            private set;
        }
        public int Price
        {
            get;
            private set;
        }
        public MyClass_Book(int bnum, string title, string author, string pub, int price)
        {
            BNum = bnum;
            Title = title;
            Author = author;
            Publisher = pub;
            Price = price;
        }
        public override string ToString()
        {
            return Title;
        }
    }
    class MyClass_Books
    {
        MyClass_Book[] books = new MyClass_Book[100];
        public MyClass_Book this[int bnum]
        {
            get
            {
                int i = 0;
                for (i = 0; i < books.Length; i++)
                {
                    if (books[i] == null)
                    {
                        break;
                    }
                    if (books[i].BNum == bnum)
                    {
                        return books[i];
                    }
                }
                return null;
            }
            set
            {
                int i = 0;
                for (i = 0; i < books.Length; i++)
                {
                    if ((books[i] == null) || (books[i].BNum == bnum))
                    {
                        break;
                    }
                }
                if (i == books.Length)
                {
                    return;
                }
                books[i] = value;
            }
        }

        internal void ViewAll()
        {
            for (int i = 0; i < books.Length; i++)
            {
                if (books[i] == null)
                {
                    break;
                }
                ViewBook(books[i]);
            }
        }

        private void ViewBook(MyClass_Book book)
        {
            Console.WriteLine("<{0}>", book.BNum);
            Console.WriteLine("\t제목:{0}", book.Title);
            Console.WriteLine("\t출판사:{0}", book.Publisher);
            Console.WriteLine("\t저자:{0}", book.Author);
            Console.WriteLine("\t가격:{0}", book.Price);
        }
    }
    public class MyClass_BookManager
    {
        MyClass_Books books = new MyClass_Books();
        internal void Run()
        {
            ConsoleKey key = ConsoleKey.NoName;
            while ((key = SelectMenu()) != ConsoleKey.Escape)//반복(메뉴 선택 한 것이 ESC가 아니라면)
            {
                switch (key)//선택한 메뉴에 따라
                {
                    case ConsoleKey.F1: Insert(); break;//F1이면 추가
                    case ConsoleKey.F2: Delete(); break;//F2이면 삭제
                    case ConsoleKey.F3: Search(); break;//F3이면 조회
                    case ConsoleKey.F4: books.ViewAll(); break;//F4이면 전체 보기
                    default: Console.WriteLine("잘못 선택하였습니다."); break;
                }
                Console.WriteLine("아무키나 누르세요.");
                Console.ReadKey(true);
            }

        }



        private void ViewBook(MyClass_Book book)
        {
            Console.WriteLine("<{0}>", book.BNum);
            Console.WriteLine("\t제목:{0}", book.Title);
            Console.WriteLine("\t출판사:{0}", book.Publisher);
            Console.WriteLine("\t저자:{0}", book.Author);
            Console.WriteLine("\t가격:{0}", book.Price);
        }

        private void Search()
        {
            //조회할 번호 입력
            int num = 0;
            Console.WriteLine("조회할 도서 번호를 입력:");
            num = int.Parse(Console.ReadLine());

            if (books[num] == null)
            {
                Console.WriteLine("{0}번: 입력하지 않음", num);
            }
            else
            {
                ViewBook(books[num]);
            }
        }

        private void Delete()
        {
            //삭제할 번호 입력
            int num = 0;
            Console.WriteLine("삭제할 도서 번호를 입력:");
            num = int.Parse(Console.ReadLine());

            if (books[num] != null)
            {
                books[num] = null;
            }
        }

        private void Insert()
        {
            //추가할 번호 입력
            int num = 0;
            Console.WriteLine("추가할 도서 번호를 입력:");
            num = int.Parse(Console.ReadLine());

            if (books[num] != null)
            {
                Console.WriteLine("이미 존재합니다.");
                return;
            }
            string title;
            Console.WriteLine("제목");
            title = Console.ReadLine();
            string author;
            Console.WriteLine("저자");
            author = Console.ReadLine();
            string publisher;
            Console.WriteLine("출판사");
            publisher = Console.ReadLine();
            int price = 0;
            Console.WriteLine("가격");
            int.TryParse(Console.ReadLine(), out price);
            books[num] = new MyClass_Book(num, title, author, publisher, price);
        }

        private ConsoleKey SelectMenu()
        {
            Console.Clear();
            Console.WriteLine("도서 관리 프로그램(인덱서 실습) 메뉴");
            Console.WriteLine("F1: 도서 추가 F2:도서 삭제 F3:도서 조회 F4:전체 보기");
            Console.WriteLine("ESC: 프로그램 종료");
            return Console.ReadKey(true).Key;
        }


    }



    public class MyTest
    {
        public MyTest()
        {

        }
        ~MyTest() { }

        // run tcp socket server thread
        static private void serverThreaRun()
        {
            MyClass_Dprint.debugPrint("start listening");
            MyClass_Networks.StartServerListeningAsync();
        }

        protected static byte[] GenerateRandomData(int length)
        {
            var data = new byte[length];
            rnd.NextBytes(data);
            return data;
        }
        private static Random rnd;
        public void run_Test()
        {

            // 도서관리 sample
            MyClass_BookManager bm = new MyClass_BookManager();
            bm.Run();

            // 100의 bitcoins 전송 sample //
            List<MyClassBlockChain> blockchain = new List<MyClassBlockChain>();
            // Genesis block
            string[] transactions = { "Jone Sent 100 Bitcoins to Bob." };
            // 최초 노드 : genesisBlock
            myClassBlockChainHeader blockheader = new myClassBlockChainHeader(null, transactions);
            MyClassBlockChain genesisBlock = new MyClassBlockChain(blockheader, transactions);
            Console.WriteLine("Block Hash : {0}", genesisBlock.getBlockHash());

            Stopwatch stopw = new Stopwatch();//시간 측정 클래스
            MyClassBlockChain previousBlock = genesisBlock;
            for (int i = 0; i < 5; i++)
            {
                myClassBlockChainHeader secondBlockheader = new myClassBlockChainHeader(Encoding.UTF8.GetBytes(previousBlock.getBlockHash()), transactions);
                MyClassBlockChain nextBlock = new MyClassBlockChain(secondBlockheader, transactions);
                stopw.Start();
                int count = secondBlockheader.ProofOfWorkCount();
                stopw.Stop();
                Console.WriteLine("{0} th Block Hash : {1}", i.ToString(), nextBlock.getBlockHash());
                Console.WriteLine("   └ COUNT of Proof of Work : {0} th loop", count);
                Console.WriteLine("   └ Delay : {0} millisecond", stopw.ElapsedMilliseconds);
                previousBlock = nextBlock;
                stopw.Reset();
            }

            //Console.Write("Any key to exit");
            //Console.ReadKey();


            // Array to List
            int[] ints = new[] { 10, 20, 10, 34, 113 };
            List<int> lst = ints.OfType<int>().ToList();
            lst.AddRange(new int[] { 10, 20, 10, 34, 113 });
            // List to Array
            int [] intsArray = lst.ToArray();

            //중복되지 않는 첫번째 문자 구하기 // dictionary 사용
            string s = "abcabdefe";
            char ch = MyClass_Strings.GetFirstChar(s.ToCharArray());
            MyClass_Dprint.debugPrint(ch);
 
            // 문자열 내 문자로 가능한 조합 구하기
            MyClass_Strings.getStringCombination("ABC");

            // string 내 패턴 스트링 카운팅
            string sampleData = "asdfkk;lkasldfajsdlkfj999kajsdlfkjasdlfkj9999kljflaskdflkajsdlfkjasdlkfj999lkajsdlfkjasldfkja999";
            // 정규식 매칭 확인 + count
            //int countResult = System.Text.RegularExpressions.Regex.Matches(sampleData, "999").Count;
            int countResult = MyClass_Strings.countMatches(sampleData, "999");
            Match matchRes = Regex.Match(sampleData, "999");
            // recursive하게 sub folder에서 유일한 file 찾기 예
            string fullPath = MyClass_Files.findFileFromSubFolder(".", "test.txt"); 

            // 스트링내 캐릭터 조합 비교 예 
            string res = string.Empty;
            int ress = MyClass_Strings.checkCharsInString("coffee", "fofeefac", ref res);

            // 라인브레이크 사용자설정 예  // append mode on/off 예
            MyClass_Files_Writer fileWriter = new MyClass_Files_Writer("fileWrite.TXT", false);
            fileWriter.customNewLine = "\r\n";
            fileWriter.WriteToFile("asdfasdfasdfasdf");
            fileWriter.WriteToFile("asdfasdfasdfasdf");
            fileWriter.WriteToFile("asdfasdfasdfasdf");
            // 소스의 라인브레이크 디텍션 예    
            MyClass_Files_Reader reader = new MyClass_Files_Reader("fileWrite.TXT");
            string getNewLineChar = reader.getNewLine(); // newLine 형식 읽어와서 file writer의 customNewLine에도 맞춰주면 된다.

            // 암호화 예
            string str_test = "3#ABCDEFGHIJKLMNOPQRSTUVWXYZ";            
            string str_Enc = MyClass_Security.CaesarCipherEncrypt(str_test, 20);
            string str_test2 = "21#abcdefghijklmnopqrstuvwxyz";
            string str_Enc2 = MyClass_Security.CaesarCipherEncrypt(str_test2, 20);

            //


            // 메서드로 바꿀것
            // ABC 부품을 갖고 A7 B7 C7이 순서대로 오면 제품 조립 ok 
            // 총 완성 제품 counting
            string str_Product_struct = "A2B3A7B7C7B2C7A9B4A9B8C7A2B7C9";
            char[] array_Product_struct = str_Product_struct.ToCharArray();
            int status = 0; //1:A ok, 2:B ok, 3:C ok
            int totalProductCount = 0;
            for(int i = 0; i<array_Product_struct.Length; i=i+2)
            {
                string num = string.Empty;
                    num += array_Product_struct[i + 1];
                int part_count = Convert.ToInt32(num);
                //int level2 = Convert.ToInt32("-1024");
                //int level = (int)Char.GetNumericValue(array_Product_struct[i + 1]);//Convert.ToInt32(array_Product_struct[i + 1]);
                if (array_Product_struct[i] == 'A')
                {
                    if (part_count >= 7)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }
                }
                if (array_Product_struct[i] == 'B' && status == 1)
                {
                    if(part_count >= 7 )
                    {
                        status = 2;
                    }
                    else
                    {
                        status = 0;
                    }

                }
                if (array_Product_struct[i] == 'C' && status == 2)
                {
                    if (part_count >= 7)
                    {
                        //status = 3;
                        // count up 
                        status = 0;
                        totalProductCount++;
                    }
                    else
                    {
                        status = 0;
                    }
                }
            }
            MyClass_Dprint.debugPrint("totalProductCount: " + totalProductCount);


            // 구조체 타입 list의 정렬 예
            //MyClass_list_sort<int> lSortTest = new MyClass_list_sort<int>();
            MyClass_list_sort.test();
            //MyClass_array_sort sortTest = new MyClass_array_sort();
            MyClass_array_sort.test();

            // 헤시맵을 이용한 정렬 TODO
            MyClass_hashMap hashTest = new MyClass_hashMap();  // TODO...
            hashTest.test();

            // 링크드리스트 사용 예
            //MyClass_linkedList list = new MyClass_linkedList();
            MyClass_linkedList.test();

            // 동전바꿈 예
            MyClass_coinChangeCount cnt = new MyClass_coinChangeCount();
            cnt.Test();

            
            // MyClass_Parse_Log parse = new MyClass_Parse_Log('#'); // delimitor
            // 문제 sample //
            // parse.MyClass_Parse_And_Report1("LOGFILE_A.TXT", "REPORT_1.TXT");            // 문제 2
            //parse.MyClass_Parse_And_Report2("LOGFILE_B.TXT", "REPORT_2.TXT");
            //parse.MyClass_Parse_And_Report3("LOGFILE_B.TXT", "REPORT_3.TXT");
            //parse.MyClass_Parse_And_Report4("LOGFILE_B.TXT", "REPORT_4.TXT");


            // 암호/복호화 sample //
            String originalText = "plain text";
            String key = "key";
            String en = MyClass_Security.Encrypt(originalText, key);
            String de = MyClass_Security.Decrypt(en, key);
            MyClass_Dprint.debugPrint("Original Text is " + originalText);
            MyClass_Dprint.debugPrint("Encrypted Text is " + en);
            MyClass_Dprint.debugPrint("Decrypted Text is " + de);

            // TCP 소켓 server/client sample //
            //MyClass_Networks.StartListeningAsync();
            // bool threadStop = false;
            var server_t = new System.Threading.Thread(() => serverThreaRun()); // 서버는 별도 thread에서 실행
            server_t.Start(); // 시작
            MyClass_Dprint.debugPrint("start StartClientSync");
            //string testSend = MyClass_Networks.StartClientSync("1111111");
            string testSend2 = MyClass_Networks.StartClientSync("send data 111111 ", false);
            //testSend2 = MyClass_Networks.StartClientSync("222222 ", false);
            testSend2 = MyClass_Networks.StartClientSync("test <EOF>", true);
            // MyClass_Dprint.debugPrint("start StartAsyncClient");
            //MyClass_Networks.StartAsyncClient("test <EOF>");
            //MyClass_Networks.StartAsyncClient("<EOF>");
            MyClass_Dprint.debugPrint("abort socket server thread");
            server_t.Abort(); // kill server thread.
            server_t.Join();


            // 폴더 스캔 샘플 //
            MyClass_Files.TreeScan(".\\..");
            // 지정한 확장자를 가진 파일을 폴더내에서 찾아 list로 update
            string[] resultt = MyClass_Files.scanFolderAndUpdate_Filelists(".", "cs");

            // 로그파일 쓰기 예 //
            MyClass_Logger log = new MyClass_Logger("restLog.txt");
            log.WriteEntry("TTTTTTTTTTTTTTTTTTTTTTTTT");

            // 링버퍼 샘플 //
            rnd = new Random(); // random 초기화..
            var data = GenerateRandomData(10); // 100개의 random 데이터 생성
            // MyClass_CircularBuffer<타입>(갯수)
            var buffer = new MyClass_CircularBuffer<byte>(100); // 링버퍼 생성
            buffer.Put(data); // 10 byte push
            data = GenerateRandomData(10);
            buffer.Put(data);
            //TestTools.UnitTesting.CollectionAssert.AreEqual(data, buffer.ToArray());
            var ret = new byte[10];//[buffer.Size];
            buffer.Get(ret);
            buffer.Get(ret); // buffer size만큼 get

            //CollectionAssert.AreEqual(data, ret);
            //Assert.IsTrue(buffer.Size == 0);

            int test = 2;
        }

    }// end class
}// end namespace

