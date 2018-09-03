using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class ClientFunctioonality
    {
        public static TcpClient clientSocket;
        public static NetworkStream serverStream;
        public static string serverIP = "127.0.0.1";
        public static int port = 8888;

        static void Main(string[] args)
        {
            clientSocket = new TcpClient();
            clientSocket.Connect(serverIP, port);
            serverStream = clientSocket.GetStream();
            Console.WriteLine("U have been connected to the server");
            SendDataToServerAndGetAnswer();
        }

        private static void SendDataToServerAndGetAnswer()
        {
            while (true)
            {
                byte[] outStream = Encoding.ASCII.GetBytes(Console.ReadLine());
                serverStream.Write(outStream, 0, outStream.Length);

                byte[] bytesToRead = new byte[clientSocket.ReceiveBufferSize];
                int bytesRead = serverStream.Read(bytesToRead, 0, clientSocket.ReceiveBufferSize);
                Console.WriteLine("Server : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
                //serverStream.Flush();
            }
        }
    }
}
