using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Web;
using System.Collections.Generic;

class Program
{



    static void HandleClient(TcpClient client)
    {



        Dictionary<string, string> routes = new()
        {
            { "/", "/" }
        };

        // Handle the HTTP request for this client
        // Close the client connection when done



        // Create a network stream for reading and writing data
        NetworkStream stream = client.GetStream();

        // Read the HTTP request
        byte[] buffer = new byte[4096];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        string[] requestLineParts = request.Split(' ');
        string httpMethod = requestLineParts[0];
        string requestTarget = requestLineParts[1];
        string httpVersion = requestLineParts[2];
        bool routeExists = routes.TryGetValue(requestTarget, out requestTarget);

        if (!routeExists)
        {
            string notFoundHeaders = "HTTP/1.1 400 Bad Request\r\nContent-Type: text/html; charset=utf-8\r\nContent - Length: " + 0 + "\r\n\r\n";
            byte[] notFoundHeaderBytes = Encoding.ASCII.GetBytes(notFoundHeaders);
            stream.Write(notFoundHeaderBytes, 0, notFoundHeaderBytes.Length);

        }

        switch (requestTarget)
        {
            case "/":
                string filePath = "./www/index.html";
                byte[] fileData = File.ReadAllBytes(filePath);

                // Write the HTTP response headers.
                string responseHeaders = "HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=utf-8\r\nContent - Length: " + fileData.Length + "\r\n\r\n";
                byte[] headerBytes = Encoding.ASCII.GetBytes(responseHeaders);
                stream.Write(headerBytes, 0, headerBytes.Length);

                // Write the file data to the network stream.
                stream.Write(fileData, 0, fileData.Length);
                break;
            default:
                string notFoundFilePath = "./www/404.html";
                byte[] notFoundFileData = File.ReadAllBytes(notFoundFilePath);

                // Write the HTTP response headers.
                string notFoundHeaders = "HTTP/1.1 404 Not Found\r\nContent-Type: text/html; charset=utf-8\r\nContent - Length: " + notFoundFileData.Length + "\r\n\r\n";
                byte[] notFoundHeaderBytes = Encoding.ASCII.GetBytes(notFoundHeaders);
                stream.Write(notFoundHeaderBytes, 0, notFoundHeaderBytes.Length);


                // Write the file data to the network stream.
                stream.Write(notFoundFileData, 0, notFoundFileData.Length);
                break;
        }



        // Close the client connection
        client.Close();
    }

    static void Main(string[] args)
    {
        // Create a TCP listener on port 80
        TcpListener listener = new TcpListener(IPAddress.Loopback, 3000);
        listener.Start();
        Console.WriteLine("HTTP server is running on port 3000...");


        while (true)
        {
            // Accept incoming client connections
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(() => HandleClient(client));
            thread.Start();
            Console.WriteLine($"New client connected.  Thread Id: {thread.ManagedThreadId}");
        }
    }
}
