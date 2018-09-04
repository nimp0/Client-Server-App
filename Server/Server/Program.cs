using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class ServerFunctionality
    {
        public static TcpListener serverSocket;
        public static TcpClient clientSocket;
        public static NetworkStream networkStream;
        public static List<string> clientQuestions = new List<string>() { "hi", "im Yaroslav", "can i ask you?", "when did World War 2 start?", "do you know a lot of things?" };
        public static List<string> serverAnswers = new List<string>() { "Hi!", "Nice to meet you! Im BetaServer#01.", "Yes of course...", "On September 1, 1939", "I know many more things" };


        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(ip, 8888);
            clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("Server started!");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine("Connection complete. Client has been connected!");
            networkStream = clientSocket.GetStream();
            GetDataFromClientAndSendAnswer();
        }

        static private void GetDataFromClientAndSendAnswer()
        {
            while (true)
            {
                try
                {
                    byte[] byteFrom = new byte[clientSocket.ReceiveBufferSize];
                    if (networkStream.DataAvailable)
                    {
                        int bytesRead = networkStream.Read(byteFrom, 0, clientSocket.ReceiveBufferSize);
                        string dataFromClient = Encoding.ASCII.GetString(byteFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("\0"));
                        Console.WriteLine("Client : " + dataFromClient);

                        for (int i = 0; i < clientQuestions.Count; i++)
                        {
                            if (dataFromClient == clientQuestions[i])
                            {
                                string answer = serverAnswers[i];
                                byte[] bytesOfAnswer = Encoding.ASCII.GetBytes(answer);
                                int answerLenghtInBytes = bytesOfAnswer.Length;
                                Console.WriteLine("Sending back : " + answer);
                                networkStream.Write(bytesOfAnswer, 0, answerLenghtInBytes);
                            }
                        }

                        if (dataFromClient != clientQuestions[0] && dataFromClient != clientQuestions[1] && dataFromClient != clientQuestions[2]
                            && dataFromClient != clientQuestions[3] && dataFromClient != clientQuestions[4])
                        {
                            ReturnDefaultAnswer();
                        }
                    }
                }
                catch (Exception exeption)
                {
                    Console.WriteLine(exeption.ToString());
                }
            }
        }

        public static void ReturnDefaultAnswer()
        {
            string defaultAnswer = "I dont know";
            byte[] bytesOfDefaultAnswer = Encoding.ASCII.GetBytes(defaultAnswer);
            int defaultAnswerLenghtInBytes = bytesOfDefaultAnswer.Length;
            Console.WriteLine("Sending back : " + defaultAnswer);
            networkStream.Write(bytesOfDefaultAnswer, 0, defaultAnswerLenghtInBytes);
        }
    }
}
