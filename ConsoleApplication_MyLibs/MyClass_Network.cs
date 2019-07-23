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


// 네트워크 서버/클라이언트 기능 정리

    //순서
    // 1. 
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



        // run tcp socket server thread
        private static void serverThreaRun()
        {
#if DEBUG_PRINT_ENABLE
            MyClass_Dprint.debugPrint("start listening");
#endif
            MyClass_Networks.StartServerListeningAsync();
        }
        public static void StartServerWithThread()
        { 
            // TCP 소켓 server/client sample //
            //MyClass_Networks.StartListeningAsync();
            // bool threadStop = false;
           /// var 
                server_t = new System.Threading.Thread(() => serverThreaRun()); // 서버는 별도 thread에서 실행
            server_t.Start(); // 시작
    #if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("start StartClientSync");
    #endif
        }
        private static Thread server_t;
        public static void StopServerWithThread()
        {
            server_t.Abort(); // kill server thread.
            server_t.Join();
        }

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
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("Socket connected to {0}", sender.RemoteEndPoint.ToString());
#endif
                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");
                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);
                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
#endif
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("ArgumentNullException : {0}", ane.ToString());
#endif
                }
                catch (SocketException se)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("SocketException : {0}", se.ToString());
#endif
                }
                catch (Exception e)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("Unexpected exception : {0}", e.ToString());
#endif
                }

            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("Socket connected to {0}", sender.RemoteEndPoint.ToString());
#endif
                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
#endif
                    res = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("ArgumentNullException : {0}", ane.ToString());
#endif
                }
                catch (SocketException se)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("SocketException : {0}", se.ToString());
#endif
                }
                catch (Exception e)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("Unexpected exception : {0}", e.ToString());
#endif
                }

            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("[Client] Socket connected to {0}", sender.RemoteEndPoint.ToString());
                    MyClass_Dprint.debugPrint("[Client] Send data [{0}] to server", data);
#endif
                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);
                    //bytesSent = sender.Send(msg);
                    if (recvReply == true) // 서버로 부터 reply 받는 경우
                    {
#if DEBUG_PRINT_ENABLE
                        // Receive the response from the remote device.
                        MyClass_Dprint.debugPrint("[Client] wait reply...");
#endif
                        int bytesRec = sender.Receive(bytes);
#if DEBUG_PRINT_ENABLE
                        MyClass_Dprint.debugPrint("[Client] Echoed data : {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
#endif
                        res = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("[Client] ArgumentNullException : {0}", ane.ToString());
#endif
                }
                catch (SocketException se)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("[Client] SocketException : {0}", se.ToString());
#endif
                }
                catch (Exception e)
                {
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("[Client] Unexpected exception : {0}", e.ToString());
#endif
                }

            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("[Client] {0}", e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("Response received : {0}", response);
#endif

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("Response received : {0}", response);
#endif
                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("Response received : {0}", response); // 별도 recv thread
#endif
                    result = response;
                }
                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());
#endif
                // Signal that the connection has been made.  
                connectDone_client_async.Set();
            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("Sent {0} bytes to server.", bytesSent);
#endif

                // Signal that all bytes have been sent.  
                sendDone_client_async.Set();
            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
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
#if DEBUG_PRINT_ENABLE
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint(e.ToString());
#endif
            }
#if DEBUG_PRINT_ENABLE
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("[server] Receved: {0}", content);
#endif
                if (content.IndexOf("<EOF>") > -1) // stream end DETECT ***
                //if(content.IndexOf("\r\n") > -1 || content.IndexOf("\n") > -1  || content.IndexOf("\r") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
#if DEBUG_PRINT_ENABLE
                    MyClass_Dprint.debugPrint("[Server] Read {0} bytes from socket. \n Data : {1}", content.Length, content);
#endif
                    if (content.Contains("test")) // check 샘플
                    {
#if DEBUG_PRINT_ENABLE
                        MyClass_Dprint.debugPrint("[Server] test command received! ");
#endif
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
#if DEBUG_PRINT_ENABLE
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
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("Sent {0} bytes to client.", bytesSent);
#endif
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
#if DEBUG_PRINT_ENABLE
                MyClass_Dprint.debugPrint("[Server] {0}", e.ToString());
#endif
            }
        }
    }
}
