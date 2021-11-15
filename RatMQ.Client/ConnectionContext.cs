using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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
            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(_brokerIp, _brokerPort);
                using (var stream = tcpClient.GetStream())
                {
                    // send request
                    var buffer = BinarySerializer.ToBinary(requestData);
                    var binary = new BinaryWriter(stream);
                    binary.Write(buffer.Length);
                    stream.Write(buffer);
                    stream.Flush();
                    // read response
                    var binaryReader = new BinaryReader(stream);
                    var responseSize = binaryReader.ReadInt32();
                    // use same buffer array
                    int bufferCurrentLength = 0;
                    buffer = new byte[responseSize];
                    for (int i = 0; bufferCurrentLength != responseSize; i++)
                    {
                        bufferCurrentLength += stream.Read(buffer, bufferCurrentLength, buffer.Length - bufferCurrentLength);
                    }
                    var responseData = (TResponseData)BinarySerializer.FromBinary(buffer);

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
                        // read message from broker
                        var binaryReader = new BinaryReader(stream);
                        int requestSize = binaryReader.ReadInt32();
                        int bufferCurrentLength = 0;
                        var buffer = new byte[requestSize];
                        for (int i = 0; bufferCurrentLength != requestSize; i++)
                        {
                            bufferCurrentLength += stream.Read(buffer, bufferCurrentLength, buffer.Length - bufferCurrentLength);
                        }
                        callback(buffer, bufferCurrentLength);
                    }
                }
            });
        }
    }
}
