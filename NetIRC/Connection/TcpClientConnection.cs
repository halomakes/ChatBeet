using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    /// <summary>
    /// Represents a TCP connection to an IRC server
    /// </summary>
    public class TcpClientConnection : IConnection
    {
        private readonly TcpClient tcpClient = new TcpClient();

        private StreamReader streamReader;
        private StreamWriter streamWriter;

        /// <summary>
        /// Indicates that data has been received through the connection
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Indicates that the TCP connection is completed
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Indicates that the TCP connection was closed
        /// </summary>
        public event EventHandler Disconnected;

        private static string crlf = "\r\n";

        /// <summary>
        /// Connects the client to an IRC server using the specified address and port number
        /// as an asynchronous operation
        /// </summary>
        /// <param name="address">The address of the IRC server</param>
        /// <param name="port">The port number</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ConnectAsync(string address, int port)
        {
            await tcpClient.ConnectAsync(address, port);

            streamReader = new StreamReader(tcpClient.GetStream());
            streamWriter = new StreamWriter(tcpClient.GetStream());

            Connected?.Invoke(this, EventArgs.Empty);

            RunDataReceiver();
        }

        private async void RunDataReceiver()
        {
            string line;

            try
            {
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(line));
                }
            }
            finally
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sends raw data to the IRC server
        /// </summary>
        /// <param name="data">Data to be sent</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendAsync(string data)
        {
            if (!data.EndsWith(crlf))
            {
                data += crlf;
            }

            await streamWriter.WriteAsync(data);
            await streamWriter.FlushAsync();
        }

        /// <summary>
        /// Disposes streams and the TcpClient
        /// </summary>
        public void Dispose()
        {
            if (streamReader != null)
            {
                streamReader.Dispose();
            }

            if (streamWriter != null)
            {
                streamWriter.Dispose();
            }

            tcpClient.Dispose();
        }
    }
}
