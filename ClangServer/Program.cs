using System;
using System.Linq;

namespace ClangServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ClangServer clangServer = new ClangServer(1337, args[0], args.Skip(1).ToArray());

            clangServer.Start();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            clangServer.Stop();
        }
    }
}
