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

        public static Dictionary<string, string> questionsAnswers = new Dictionary<string, string>();
        public static List<string> names = new List<string>() { " Shura", " Elfi", " Donny", " Vladi", " Serush", " Jora", " Dmutku", " Stasik", " Huyan", " Anonim" };

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(ip, 8888);
            clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("Server started!");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine("Connection complete. Client has been connected!");

            GetDataFromClientAndSendAnswer();
        }

        private static void GetDataFromClientAndSendAnswer()
        {
            using (networkStream = clientSocket.GetStream())
            {
                AddQuestionsAnswers();
                while (true)
                {
                    byte[] byteFrom = new byte[clientSocket.ReceiveBufferSize];
                    if (networkStream.DataAvailable)
                    {
                        int bytesRead = networkStream.Read(byteFrom, 0, clientSocket.ReceiveBufferSize);
                        string dataFromClient = Encoding.ASCII.GetString(byteFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("\0"));
                        Console.WriteLine("Client : " + dataFromClient);

                        string[] questions = questionsAnswers.Keys.ToArray();
                        foreach (KeyValuePair<string, string> item in questionsAnswers)
                        {
                            if (dataFromClient == item.Key)
                            {
                                string result = item.Value;
                                ReturnAnswer(result);
                            }
                        }

                        

                        IEnumerable<string> namesX = from name in names where name.Any() select name;
                        foreach (string name in namesX)
                        {
                            if (dataFromClient == questions[0] + name)
                            {
                                string toRemove = "my name is";
                                string result = string.Empty;
                                int k = dataFromClient.IndexOf(toRemove);
                                if (k >= 0)
                                {
                                    result = dataFromClient.Remove(k, toRemove.Length);
                                }
                                ReturnAnswer(result);
                            }

                            if (!questionsAnswers.ContainsKey(dataFromClient) && dataFromClient != name)
                            {
                                string result = "I dont know";
                                ReturnAnswer(result);
                            }

                            /* if (!questionsAnswers.ContainsKey(dataFromClient) && dataFromClient != questions[0] + name)
                             {
                                 string result = "I dont know";
                                 ReturnAnswer(result);
                             }*/
                        }
                        /*for (int i = 0; i < names.Count; i++)
                        {
                            if (dataFromClient == questions[0] + names[i])
                            {
                                string toRemove = "my name is";
                                string result = string.Empty;
                                int k = dataFromClient.IndexOf(toRemove);
                                if (k >= 0)
                                {
                                    result = dataFromClient.Remove(k, toRemove.Length);
                                }
                                ReturnAnswer(result);
                            }
                        }

                        if (!questionsAnswers.ContainsKey(dataFromClient) && dataFromClient != questions[0] + names[i])
                        {
                            IEnumerable<string> namesX = from name in names where name.Any() select name;
                            foreach (string name in namesX)
                            {
                                string result = "I dont know";
                                ReturnAnswer(result);
                            }
                        }*/




                    }
                }
            }
        }

        public static void ReturnAnswer(string answerX)
        {
            byte[] bytesOfAnswerX = Encoding.ASCII.GetBytes(answerX);
            int answerXBytesLenght = bytesOfAnswerX.Length;
            Console.WriteLine("Sending back : " + answerX);
            networkStream.Write(bytesOfAnswerX, 0, answerXBytesLenght);
        }

        public static void AddQuestionsAnswers()
        {
            questionsAnswers.Add("hi, my name is", "Hi! Nice to meet you! Im BetaServer#01.");
            questionsAnswers.Add("can i ask you?", "Yes of course...");
            questionsAnswers.Add("when did World War 1 start?", "On July 28, 1914");
            questionsAnswers.Add("when did World War 2 start?", "On September 1, 1939");
            questionsAnswers.Add("do you know a lot of things?", "I know many more things");
        }
    }
}
