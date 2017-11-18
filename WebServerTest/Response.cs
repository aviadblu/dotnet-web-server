using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WebServerTest
{
    public class Response
    {

        private Byte[] data = null;
        private String status;
        private String mime;

        private Response(String status, String mime, Byte[] data)
        {
            this.data = data;
        }

        public static Response From(Request request)
        {
            if (request == null)
                return MakeNullRequest();

            if (request.Type == "GET")
            {
                String file_dir = Environment.CurrentDirectory + HttpServer.WEB_DIR + request.URL;
                Console.WriteLine("file_dir: " + file_dir);
                FileInfo f = new FileInfo(file_dir);
                if(f.Exists && f.Extension.Contains("."))
                {
                    return MakeFromFile(f);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(f + "");
                    if (!di.Exists)
                        return MakePageNotFound();

                    FileInfo[] files = di.GetFiles();
                    foreach(FileInfo ff in files)
                    {
                        String n = ff.Name;
                        if(n.Contains("defaults.html") || 
                            n.Contains("defaults.htm") ||
                            n.Contains("index.html") || 
                            n.Contains("index.htm"))
                        {
                            return MakeFromFile(ff);
                        }
                    }
                }


                if (!f.Exists)
                    return MakePageNotFound();

            }
            else
                return MakeMethodNotAllowed();


            return MakePageNotFound();
        }

        private static Response MakeFromFile(FileInfo file)
        {
            FileStream fs = file.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("200 Ok", "text/html", d);
        }

        private static Response MakeNullRequest()
        {
            String file_dir = Environment.CurrentDirectory + HttpServer.MSG_DIR + "400.html";
            FileInfo file = new FileInfo(file_dir);
            FileStream fs = file.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("400 Bad Request", "text/html", d);
        }

        private static Response MakeMethodNotAllowed()
        {
            String file_dir = Environment.CurrentDirectory + HttpServer.MSG_DIR + "405.html";
            FileInfo file = new FileInfo(file_dir);
            FileStream fs = file.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("405 Method Not Allowed", "text/html", d);
        }

        private static Response MakePageNotFound()
        {
            String file_dir = Environment.CurrentDirectory + HttpServer.MSG_DIR + "404.html";
            FileInfo file = new FileInfo(file_dir);
            FileStream fs = file.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();

            return new Response("404 Page Not Found", "text/html", d);
        }

        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Range: bytes\r\nContent-Length: {4}\r\n",
                HttpServer.HTTP_VERSION, status, HttpServer.HTTP_VERSION, mime, data.Length));
            writer.Flush();
            stream.Write(data, 0, data.Length);
        }
    }
}
