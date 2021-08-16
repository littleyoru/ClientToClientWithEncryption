using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientToClientWithEncryption
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MyClient();
        }
    }

    class MyClient
    {
        public MyClient()
        {
            TcpClient client = new TcpClient();

            int port = 13999;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            client.Connect(endPoint);

            NetworkStream stream = client.GetStream();
            ReceiveMessage(stream);

            bool isRunning = true;
            while (isRunning)
            {
                // Send a message
                Console.WriteLine("Write your message here: ");
                string text = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(text);

                // Encrypt
                var encryption = new EncryptionV();
                var encryptedMessage = encryption.EncryptV(buffer);
                Console.WriteLine("encrypted message before sending to server ");
                for (int i = 0; i < encryptedMessage.Length; i++)
                {
                    Console.Write(encryptedMessage[i]);
                }
                Console.WriteLine("\n");

                stream.Write(encryptedMessage, 0, encryptedMessage.Length);
            }

            client.Close();
        }

        public async void ReceiveMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[256];

            bool isRunning = true;
            while (isRunning)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, 256);
                var exactBuffer = new byte[bytesRead];
                Array.Copy(buffer, exactBuffer, bytesRead);
                Console.WriteLine("encrypted message received back from server ");
                for (int i = 0; i < exactBuffer.Length; i++)
                {
                    Console.Write(exactBuffer[i]);
                }
                Console.WriteLine("\n");

                // call decryption method
                var crypt = new EncryptionV();
                var decryptedMessage = crypt.DecryptV(exactBuffer);
                Console.WriteLine("Decrypted bytes array: ");
                foreach (var b in decryptedMessage)
                    Console.Write(b);
                Console.WriteLine("\n");

                string messageReceived = Encoding.UTF8.GetString(decryptedMessage, 0, bytesRead);

                Console.WriteLine("message received decrypted: " + messageReceived);
            }
        }
    }
}
