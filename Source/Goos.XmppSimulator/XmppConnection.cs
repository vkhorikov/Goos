using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goos.XmppSimulator
{
    public class XmppConnection : IDisposable
    {
        private readonly List<XmppChat> _activeChats = new List<XmppChat>();
        private volatile bool _isDisposed;


        public XmppConnection()
        {
            StartListeningForMessages();
        }


        private void StartListeningForMessages()
        {
            if (_isDisposed)
                return;

            Task.Run(() => ListenForMessages());
        }


        private void ListenForMessages()
        {
            var client = new XmppClient();

            foreach (XmppChat chat in CloneActiveChats())
            {
                if (_isDisposed)
                    return;

                string response = client.Request($"Poll|{chat.ChatId}|{chat.User}");
                if (!string.IsNullOrEmpty(response))
                {
                    chat.ProcessMessage(response);
                }

                Task.Delay(100).Wait();
            }

            StartListeningForMessages();
        }


        public List<XmppChat> CloneActiveChats()
        {
            lock (_activeChats)
            {
                return _activeChats.ToList();
            }
        }


        public void CreateChat(int chatId)
        {
            var client = new XmppClient();

            string response = client.Request("New Chat|" + chatId);
            if (response != "Ok")
                throw new InvalidOperationException();
        }


        public XmppChat ConnectToChat(int chatId, string user)
        {
            lock (_activeChats)
            {
                var chat = new XmppChat(chatId, user, this);
                _activeChats.Add(chat);

                return chat;
            }
        }


        internal void SendMessage(int chatId, string user, string message)
        {
            var client = new XmppClient();
            client.Request($"Send|{chatId}|{message}|{user}");
        }


        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}
