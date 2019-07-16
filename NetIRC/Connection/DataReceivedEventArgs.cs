using System;

namespace NetIRC.Connection
{
    /// <summary>
    /// Provides the data received from a connection
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Data received from the connection
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// Initializes a new instance of NetIRC.Connection.DataReceivedEventArgs
        /// </summary>
        /// <param name="data">Data received from the connection</param>
        public DataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }
}
