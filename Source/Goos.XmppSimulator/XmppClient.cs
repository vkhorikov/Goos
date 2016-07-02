using System;
using System.IO;
using System.IO.Pipes;

namespace Goos.XmppSimulator
{
    internal class XmppClient
    {
        public string Request(string request)
        {
            using (var pipe = new NamedPipeClientStream(".", XmppServer.PipeName, PipeDirection.InOut, PipeOptions.None))
            {
                try
                {
                    pipe.Connect(1000);
                }
                catch (TimeoutException)
                {
                    return string.Empty;
                }

                using (var reader = new StreamReader(pipe))
                using (var writer = new StreamWriter(pipe))
                {
                    writer.WriteLine(request);
                    writer.Flush();
                    pipe.WaitForPipeDrain();

                    return reader.ReadLine();
                }
            }
        }
    }
}
