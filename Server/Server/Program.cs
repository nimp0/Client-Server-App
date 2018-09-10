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
        /*public static List<string> names = new List<string>()
        {
            " Shura", " Elfi", " Donny", " Vladi",
            " Serush", " Jora", " Dmutku", " Stasik",
            " Huyan", " Anonim"
        };*/

        public const string MyNameIsKey = "hi, my name is";

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
                List<string> answerKeysToSearch = new List<string>();

                foreach (var key in questionsAnswers.Keys)
                {
                    answerKeysToSearch.Add(key);
                }
                while (true)
                {
                    byte[] byteFrom = new byte[clientSocket.ReceiveBufferSize];
                    if (networkStream.DataAvailable)
                    {
                        int bytesRead = networkStream.Read(byteFrom, 0, clientSocket.ReceiveBufferSize);
                        string dataFromClient = Encoding.ASCII.GetString(byteFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("\0"));
                        Console.WriteLine("Client : " + dataFromClient);

                        bool predicate(string neededKey)
                        {
                            bool res = answerKeysToSearch.Any(key => dataFromClient.Contains(neededKey));
                            return res;
                        }

                        string answerKey = answerKeysToSearch.Where(predicate).FirstOrDefault();
                        if (answerKey == null)
                        {
                            string defaultAnswer = "I dont know";
                            ReturnAnswer(defaultAnswer);
                            continue;
                        }

                        string answer = questionsAnswers[answerKey];
                        if (answerKey == MyNameIsKey)
                        {
                            int startIndex = dataFromClient.IndexOf(MyNameIsKey);
                            string textThatIsNotPartOfKey = dataFromClient.Substring(startIndex + MyNameIsKey.Length);
                            answer = string.Format(answer, textThatIsNotPartOfKey);
                        }
                        ReturnAnswer(answer);
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
            questionsAnswers.Add(MyNameIsKey, "Hi {0}! Nice to meet you! Im BetaServer#01.");
            questionsAnswers.Add("can i ask you?", "Yes of course...");
            questionsAnswers.Add("when did World War 1 start?", "On July 28, 1914");
            questionsAnswers.Add("when did World War 2 start?", "On September 1, 1939");
            questionsAnswers.Add("do you know a lot of things?", "I know many more things");
        }
    }
}
