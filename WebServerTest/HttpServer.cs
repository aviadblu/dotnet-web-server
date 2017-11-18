using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebServerTest
{
    public class HttpServer
    {
        public const String MSG_DIR = "/root/msg/";
        public const String WEB_DIR = "/root/web/";
        public const String HTTP_VERSION = "HTTP/1.1";
        public const String NAME = "Aviad HTTP Server v0.1";

        private bool running = false;

        private TcpListener listener;

        public HttpServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }

        private void Run()
        {
            running = true;
            listener.Start();

            while (running)
            {

                Console.WriteLine("Waiting for connection...");

                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Client connected!");

                HandleClient(client);

                client.Close();
            }

            running = false;

            listener.Stop();

        }

        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());

            String msg = "";

            while(reader.Peek() != -1)
            {
                msg += reader.ReadLine() + "\n";
            }

            Debug.WriteLine("Request \n" + msg);

            Request req = Request.GetRequest(msg);
            Response res = Response.From(req);
            res.Post(client.GetStream());
        }
    }
}
