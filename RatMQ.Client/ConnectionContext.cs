using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
                var socket = tcpClient.Client;

                var json = JsonSerializer.ToJson(request);
                var bytes = Encoding.UTF8.GetBytes(json);
                socket.Send(bytes);

                bytes = new byte[1024];
                var count = socket.Receive(bytes);
                json = Encoding.UTF8.GetString(bytes, 0, count);
                var response = JsonSerializer.FromJson<Response>(json);
                var responseData = JsonSerializer.FromJson<TResponseData>(response.JsonData);

                return responseData;
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
                    var tcpClient = tcpListener.AcceptTcpClient();
                    var socket = tcpClient.Client;
                    var resultBuffer = new byte[10 * 1024 * 1024];
                    var readBuffer = new byte[1024 * 1024];
                    int resultBufferLength = 0;
                    int readBufferLength;
                    do
                    {
                        readBufferLength = socket.Receive(readBuffer);
                        Array.Copy(readBuffer, 0, resultBuffer, resultBufferLength, readBufferLength);
                        resultBufferLength += readBufferLength;
                    }
                    while (socket.Available > 0);
                    callback(resultBuffer, resultBufferLength);
                }
            });
        }
    }
}
