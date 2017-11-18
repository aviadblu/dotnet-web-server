using System;

namespace WebServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8080;
            Console.WriteLine("Http server is listening on port {0}", port);
            HttpServer server = new HttpServer(port);
            server.Start();
        }
    }
}
