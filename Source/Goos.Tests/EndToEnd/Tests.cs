using System;
using System.Diagnostics;
using Goos.XmppSimulator;
using TestStack.White;

namespace Goos.Tests.EndToEnd
{
    public class Tests : IDisposable
    {
        private const string ApplicationPath = @"C:\Sources\Goos\WithMocks\Source\Goos.UI\bin\Debug\Goos.UI.exe";
        private const string ApplicationProcessName = "Goos.UI";
        private const string ServerPath = @"C:\Sources\Goos\WithMocks\Source\Goos.XmppSimulator\bin\Debug\Goos.XmppSimulator.exe";
        private const string ServerProcessName = "Goos.XmppSimulator";

        protected readonly ApplicationRunner _application;
        protected readonly XmppConnection _connection;


        public Tests()
        {
            KillRunningApplication(ApplicationProcessName);
            KillRunningApplication(ServerProcessName);

            Process.Start(ServerPath);

            Application application = Application.Launch(ApplicationPath);
            _application = new ApplicationRunner(application);
            _connection = new XmppConnection();
        }


        private void KillRunningApplication(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }


        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
