using System.Windows;
using Goos.XmppSimulator;

namespace Goos.UI
{
    public partial class App
    {
        internal readonly XmppConnection XmppConnection;

        public App()
        {
            XmppConnection = new XmppConnection();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            XmppConnection.Dispose();
            base.OnExit(e);
        }
    }
}
