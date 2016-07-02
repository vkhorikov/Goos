using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;

namespace Goos.XmppSimulator
{
    public class XmppServer
    {
        public const string PipeName = "Auction Sniper";

        private readonly Dictionary<int, Queue<Message>> _messageQueue = new Dictionary<int, Queue<Message>>();


        public void Start()
        {
            Task.Run(() => AcceptNewConnection());
        }


        private void AcceptNewConnection()
        {
            using (var pipe = new NamedPipeServerStream(
                PipeName, PipeDirection.InOut, 254, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                Task.Factory
                    .FromAsync(pipe.BeginWaitForConnection(null, null), pipe.EndWaitForConnection)
                    .ContinueWith(_ => ReadMessage(pipe))
                    .Wait();
            }

            Start();
        }


        private void ReadMessage(NamedPipeServerStream pipe)
        {
            using (var reader = new StreamReader(pipe))
            {
                using (var writer = new StreamWriter(pipe))
                {
                    string request = reader.ReadLine();
                    if (request == null)
                        return;

                    ProcessRequest(request, writer);
                    pipe.WaitForPipeDrain();
                }
            }

            if (pipe.IsConnected)
            {
                pipe.Disconnect();
            }
        }


        private void ProcessRequest(string request, StreamWriter writer)
        {
            string[] data = request.Split('|');
            if (data.Length == 0)
                throw new InvalidOperationException();

            if (data[0] == "New Chat")
            {
                Console.WriteLine(request);
                ProcessNewChatRequest(int.Parse(data[1]), writer);
                return;
            }

            if (data[0] == "Poll")
            {
                ProcessPollRequest(int.Parse(data[1]), data[2], writer);
                return;
            }

            if (data[0] == "Send")
            {
                Console.WriteLine(request);
                ProcessSendRequest(int.Parse(data[1]), data[2], data[3], writer);
                return;
            }

            throw new InvalidOperationException();
        }


        private void ProcessSendRequest(int chatId, string message, string user, StreamWriter writer)
        {
            _messageQueue[chatId].Enqueue(new Message(user, message));
            SendResponse("Ok", writer);
        }


        private void ProcessPollRequest(int chatId, string user, StreamWriter writer)
        {
            if (!_messageQueue.ContainsKey(chatId) || !_messageQueue[chatId].Any())
            {
                SendResponse(string.Empty, writer);
                return;
            }

            Message message = _messageQueue[chatId].Peek();
            if (message.User == user)
            {
                SendResponse(string.Empty, writer);
                return;
            }

            SendResponse(message.Content, writer);
            _messageQueue[chatId].Dequeue();

            Console.WriteLine("User: " + user + ", Message dequeued: " + message);
        }


        private void ProcessNewChatRequest(int chatId, StreamWriter writer)
        {
            if (_messageQueue.ContainsKey(chatId))
                throw new ArgumentException("The chat Id " + chatId + " already exists");

            _messageQueue.Add(chatId, new Queue<Message>());
            SendResponse("Ok", writer);
        }


        private void SendResponse(string response, StreamWriter writer)
        {
            writer.WriteLine(response);
            writer.Flush();
        }


        private struct Message
        {
            public readonly string User;
            public readonly string Content;


            public Message(string user, string content)
            {
                User = user;
                Content = content;
            }


            public override string ToString()
            {
                return Content;
            }
        }
    }
}
