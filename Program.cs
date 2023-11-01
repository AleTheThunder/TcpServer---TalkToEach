using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    class Program
    {
        static async Task Main()
        {
            TcpListener server = null;
            string password = "meinPasswort";

            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                int port = 8080;
                server = new TcpListener(localAddr, port);
                server.Start();

                Console.WriteLine($"Server gestartet auf {localAddr}:{port}");

                int counter = 0;

                while (counter < 10)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    Console.WriteLine("Verbunden mit einem Client");

                    NetworkStream stream = client.GetStream();

                    byte[] buffer = new byte[256];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string receivedPassword = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (receivedPassword == password)
                    {
                        Console.WriteLine("Passwort korrekt!");

                        byte[] successMessage = Encoding.ASCII.GetBytes("Erfolgreich authentifiziert!");
                        await stream.WriteAsync(successMessage, 0, successMessage.Length);

                        while (true)
                        {
                            buffer = new byte[256];
                            bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            if (message.ToLower() == "exit")
                            {
                                break;
                            }

                            Console.WriteLine($"Client sagt: {message}");

                            byte[] response = Encoding.ASCII.GetBytes($"Du hast gesagt: {message}");
                            await stream.WriteAsync(response, 0, response.Length);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Falsches Passwort!");

                        byte[] failureMessage = Encoding.ASCII.GetBytes("Falsches Passwort!");
                        await stream.WriteAsync(failureMessage, 0, failureMessage.Length);
                    }

                    client.Close();
                    counter++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
