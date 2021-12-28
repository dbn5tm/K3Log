using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace K3Log
{
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    public class TelnetThread : IDisposable
    {
        IPAddress remoteIPAddress;
        IPEndPoint ep;
        Socket tnSocket;

        public bool StopTelnet;

        public class RcvdDataEventArgs : EventArgs
        {
            public string rcvdMsg;
            public bool state;
        }
        public RcvdDataEventArgs args = new RcvdDataEventArgs();
        public event EventHandler<RcvdDataEventArgs> IPDataRcvd;
        protected virtual void OnPageRcvd(object sender, RcvdDataEventArgs e)
        {
            IPDataRcvd?.Invoke(this, e);
        }
        
        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private static String response = String.Empty;

        public String url { get; set; }
        public String port { get; set; }
        public String callsign { get; set; }
        public bool connected { get; set; }

        public TelnetThread()
        {
            StopTelnet = false;
        }


        public void Logon()
        {
            if (Connect(url, port, callsign) != -1)
            {
                this.connected = true;
            }
            else
            {
                this.connected = false;
            }
        }

        private int Connect(String url, String port, String command)
        {
            IPHostEntry PIPAddress;
            String ip;
            try
            {
                PIPAddress = Dns.GetHostEntry(url);
                ip = PIPAddress.AddressList[0].ToString();
            }
            catch
            {
                return -11;
            }
            // Get the IP Address and the Port and create an IPEndpoint (ep)
            remoteIPAddress = IPAddress.Parse(ip.Trim());
            ep = new IPEndPoint(remoteIPAddress, Convert.ToInt16(port.Trim()));

            // Set the socket up (type etc)
            tnSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Convert the ASCII command into bytes, adding a line termination on (vbCrLf)
            byte[] SendBytes = Encoding.ASCII.GetBytes(command + "\r\n");
            // LRecvString: data returned from the telnet socet
            String RecvString = String.Empty;
            // Create a byte array for recieving bytes from the telnet socket
            byte[] RecvBytes = new byte[255];
            // NumBytes: Number of bytes return from telnet socket (count)
            

            try
            {
                // Connect
                tnSocket.BeginConnect(ep, ConnectCallback, tnSocket);
                connectDone.WaitOne();
                              
                Send(tnSocket, command + "\r\n");
                sendDone.WaitOne();

                
                response = ""; 
                Receive(tnSocket);
                receiveDone.WaitOne();

                              
                return 0;
                
            }
            catch (SocketException oEX)
            { // error
                // You will need to do error cleanup here e.g killing the socket
                // and exiting the procedure.
                args.rcvdMsg = oEX.ToString();
                IPDataRcvd(this,args);
                tnSocket = null;
                return -1;
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            
            try
            {
                
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);
                //string RecvString = "";
                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                    receiveDone.Set();
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                        state.sb.Clear();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                    
                }
                response = state.sb.ToString();
                state.sb.Clear();
                args.rcvdMsg = response;
                response = "";
                IPDataRcvd(this, args);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void sendCommand(String command)
        {
            try
            {
                byte[] SendBytes = Encoding.ASCII.GetBytes(command + "\r\n");

                // ' Double check we are connected
                if (tnSocket.Connected)
                {
                    //' Send the command
                    tnSocket.Send(SendBytes, SendBytes.Length, SocketFlags.None);
                }
            }
            catch (Exception)
            {

                //throw;
            }
        }

        public void Dispose()
        {
            ((IDisposable)tnSocket).Dispose();
        }
    }
}
