using System;
using System.Threading.Tasks;

namespace Goos.XmppSimulator
{
    public class XmppChat
    {
        public event Action<string> MessageReceived;

        internal readonly int ChatId;
        internal readonly string User;
        private readonly XmppConnection _connection;


        internal XmppChat(int chatId, string user, XmppConnection connection)
        {
            User = user;
            ChatId = chatId;
            _connection = connection;
        }


        public void SendMessage(string message)
        {
            Task.Run(() => _connection.SendMessage(ChatId, User, message));
        }


        public void ProcessMessage(string message)
        {
            MessageReceived?.Invoke(message);
        }
    }
}
