using System;

namespace Goos.XmppSimulator
{
    public class Program
    {
        private static XmppServer _server;
        private static XmppChat _chat;


        private static void Main(string[] args)
        {
            _server = new XmppServer();
            _server.Start();

            if (args.Length > 0 && args[0] == "emulate")
            {
                var connection = new XmppConnection();
                connection.CreateChat(11);
                _chat = connection.ConnectToChat(11, "Some user");
            }

            Console.ReadLine();
        }
    }
}
