using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace socket_client
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i=0; i<args.Count(); i++)
                //string arg in args)
            {
                Debug.WriteLine(args[i]);


            }
            StartClientSync("test", true);
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
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];  // ipv6
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); //ipHostInfo.AddressList[1]; // ipv4
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 9876);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);
                    Console.WriteLine("[Client] Socket connected to " + sender.RemoteEndPoint.ToString());
                    Console.WriteLine("[Client] Send data to server" + data);
                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg); // 초기 메세기 send
                    //bytesSent = sender.Send(msg);
                    if (recvReply == true) // 서버로 부터 reply 받는 경우
                    {
                        // Receive the response from the remote device.
                        Console.WriteLine("[Client] wait reply...");
                        while (true)
                        {                            
                            int bytesRec = sender.Receive(bytes);
                            Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRec));
                            res = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            // 서버로 부터 받은 데이터 parsing 후 proc 여기 추가
                            // TODO
                            // process 후 replay send
                            byte[] StrByte = Encoding.UTF8.GetBytes("ACK");
                            sender.Send(StrByte);
                        }

                    }
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Debug.WriteLine("[Client] ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Debug.WriteLine("[Client] SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[Client] Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("[Client] {0}", e.ToString());
            }
            return res;
        }
    }
}
