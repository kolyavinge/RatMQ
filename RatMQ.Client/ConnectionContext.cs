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
        private string _clientId;
        private int _clientPort;

        public ConnectionContext(string brokerIp, int brokerPort, string clientId, int clientPort)
        {
            _brokerIp = IPAddress.Parse(brokerIp);
            _brokerPort = brokerPort;
            _clientId = clientId;
            _clientPort = clientPort;
        }

        public string ClientId { get { return _clientId; } }

        public TResponseData SendToBroker<TResponseData>(object requestData)
        {
            var request = new Request
            {
                JsonDataTypeName = requestData.GetType().AssemblyQualifiedName,
                JsonData = JsonSerializer.ToJson(requestData)
            };
            using (var client = new TcpClient())
            {
                client.Connect(_brokerIp, _brokerPort);
                using (var stream = client.GetStream())
                {
                    var json = JsonSerializer.ToJson(request);
                    var bytes = Encoding.UTF8.GetBytes(json);
                    stream.Write(bytes);

                    bytes = new byte[1024];
                    var count = stream.Read(bytes);
                    json = Encoding.UTF8.GetString(bytes, 0, count);
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
                var server = new TcpListener(ipAddress, _clientPort);
                server.Start();
                while (true)
                {
                    var client = server.AcceptTcpClient();
                    using (var stream = client.GetStream())
                    {
                        var buffer = new byte[1024];
                        var count = stream.Read(buffer);
                        callback(buffer, count);
                    }
                }
            });
        }
    }
}
