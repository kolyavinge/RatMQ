using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RatMQ.Contracts;

namespace RatMQ.Client
{
    internal class ConnectionContext
    {
        private IPAddress _brokerIp;
        private int _brokerPort;
        private int _clientPort;

        public ConnectionContext(string brokerIp, int brokerPort, int clientPort)
        {
            _brokerIp = IPAddress.Parse(brokerIp);
            _brokerPort = brokerPort;
            _clientPort = clientPort;
        }

        public string ClientId { get { return $"{Environment.MachineName}_{_clientPort}"; } }

        public TResponseData SendToBroker<TResponseData>(object requestData)
        {
            var request = new Request
            {
                JsonDataTypeName = requestData.GetType().AssemblyQualifiedName,
                JsonData = JsonSerializer.ToJson(requestData)
            };
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(_brokerIp, _brokerPort);
                using (var stream = tcpClient.GetStream())
                {
                    var json = JsonSerializer.ToJson(request);
                    var readBuffer = Encoding.UTF8.GetBytes(json);
                    var binary = new BinaryWriter(stream);
                    binary.Write(readBuffer.Length);
                    stream.Write(readBuffer);
                    stream.Flush();

                    var binaryReader = new BinaryReader(stream);
                    var responseSize = binaryReader.ReadInt32();
                    var resultBuffer = new byte[10 * 1024 * 1024];
                    int resultBufferCurrentLength = 0;
                    readBuffer = new byte[1024 * 1024];
                    for (int i = 0; resultBufferCurrentLength != responseSize; i++)
                    {
                        var readBufferLength = stream.Read(readBuffer);
                        if (readBufferLength + resultBufferCurrentLength > resultBuffer.Length)
                        {
                            Array.Resize(ref resultBuffer, 2 * resultBuffer.Length);
                        }
                        Array.Copy(readBuffer, 0, resultBuffer, resultBufferCurrentLength, readBufferLength);
                        resultBufferCurrentLength += readBufferLength;
                    }

                    json = Encoding.UTF8.GetString(resultBuffer, 0, resultBufferCurrentLength);
                    var response = JsonSerializer.FromJson<Response>(json);
                    var responseData = JsonSerializer.FromJson<TResponseData>(response.JsonData);

                    return responseData;
                }
            }
        }

        public delegate void ListenToBrokerCallback(byte[] buffer, int count);

        public void ListenToBroker(ListenToBrokerCallback callback)
        {
            Task.Factory.StartNew(() =>
            {
                var ipAddress = IPAddress.Parse("127.0.0.1");
                var tcpListener = new TcpListener(ipAddress, _clientPort);
                tcpListener.Start();
                while (true)
                {
                    using (var tcpClient = tcpListener.AcceptTcpClient())
                    using (var stream = tcpClient.GetStream())
                    {
                        var binaryReader = new BinaryReader(stream);
                        int requestSize = binaryReader.ReadInt32();

                        var resultBuffer = new byte[10 * 1024 * 1024];
                        var readBuffer = new byte[1024 * 1024];
                        int resultBufferCurrentLength = 0;
                        int readBufferLength;
                        for (int i = 0; resultBufferCurrentLength != requestSize; i++)
                        {
                            readBufferLength = stream.Read(readBuffer);
                            if (readBufferLength + resultBufferCurrentLength > resultBuffer.Length)
                            {
                                Array.Resize(ref resultBuffer, 2 * resultBuffer.Length);
                            }
                            Array.Copy(readBuffer, 0, resultBuffer, resultBufferCurrentLength, readBufferLength);
                            resultBufferCurrentLength += readBufferLength;
                        }
                        callback(resultBuffer, resultBufferCurrentLength);
                    }
                }
            });
        }
    }
}
