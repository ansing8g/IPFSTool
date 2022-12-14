using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace ServerModule.Network
{
    public class ConnectSocket
    {
        public ConnectSocket(ClientSocketEvent _event)
        {
            m_event = _event;
            m_socket = null;
            m_buf = new byte[0];
            m_bufsize = 0;
            m_total_buf = new byte[0];
            m_total_bufsize = 0;
            m_offset = 0;

            StateObject = null;
        }

        public bool Connect(string _ip, int _port, uint _bufsize = 1024000, uint _total_bufsize = 2048000)
        {
            Disconnect();

            try
            {
                m_buf = new byte[_bufsize];
                m_bufsize = _bufsize;
                m_total_buf = new byte[_total_bufsize];
                m_total_bufsize = _total_bufsize;
                m_offset = 0;

                IPAddress? ipaddress = null;
                if(false == IPAddress.TryParse(_ip, out ipaddress))
                {
                    IPAddress[] arripaddress = Dns.GetHostAddresses(_ip);
                    if(null == arripaddress ||
                        0 >= arripaddress.Length)
                    {
                        return false;
                    }

                    ipaddress = arripaddress[0];
                }

                if(ipaddress == null)
                {
                    return false;
                }

                IPEndPoint end_point = new IPEndPoint(ipaddress, _port);

                m_socket = new Socket(end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                m_socket.NoDelay = true;
                m_socket.LingerState = new LingerOption(true, 0);

                m_socket.BeginConnect(end_point, ConnectCallback, null);
            }
            catch (Exception e)
            {
                if (null != m_event)
                {
                    m_event.OnError(SocketErrorType.Connect, e, this);
                }

                return false;
            }

            return true;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                if (null == m_socket)
                {
                    return;
                }

                m_socket.EndConnect(ar);

                if (null != m_event)
                {
                    m_event.OnConnect(this);
                }

                if (false == Receive())
                {
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                if (null != m_event)
                {
                    m_event.OnError(SocketErrorType.Connect, e, this);
                }

                Disconnect();
            }
        }

        public bool Connected()
        {
            return null == m_socket ? false : m_socket.Connected;
        }

        public void Disconnect(bool _check_connect = true)
        {
            try
            {
                if (true == _check_connect &&
                    null == m_socket)
                {
                    return;
                }

                if (null != m_socket)
                {
                    m_socket.Close();
                    m_socket.Dispose();
                    m_socket = null;
                }

                if (null != m_event)
                {
                    m_event.OnDisconnect(this);
                }

                m_offset = 0;
                StateObject = null;
            }
            catch (Exception e)
            {
                if (null != m_event)
                {
                    m_event.OnError(SocketErrorType.Disconnect, e, this);
                }
            }
        }

        public bool Send(byte[] data)
        {
            try
            {
                if (false == Connected() ||
                    null == data ||
                    0 >= data.Length ||
                    m_bufsize < data.Length)
                {
                    return false;
                }

                byte[] total_data = BitConverter.GetBytes(data.Length).Concat(data).ToArray();

                //SocketError error;
                //int sendsize = m_socket.Send(total_data, 0, total_data.Length, SocketFlags.None, out error);

                SocketError error;
                m_socket!.BeginSend(total_data, 0, total_data.Length, SocketFlags.None, out error, SendCallback, this);

                if (SocketError.Success != error &&
                    SocketError.IOPending != error)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                if (null != m_event)
                {
                    m_event.OnError(SocketErrorType.Send, e, this);
                }

                return false;
            }

            return false;
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (null == m_socket)
                {
                    return;
                }

                int sendsize = m_socket.EndSend(ar);

                if (null != m_event)
                {
                    m_event.OnSend(this);
                }
            }
            catch (Exception e)
            {
                if (null != m_event)
                {
                    m_event.OnError(SocketErrorType.Send, e, this);
                }
            }
        }

        internal bool Receive()
        {
            try
            {
                if (false == Connected())
                {
                    return false;
                }

                SocketError error;
                m_socket!.BeginReceive(m_buf, 0, m_buf.Length, SocketFlags.None, out error, ReceiveCallback, this);

                if (SocketError.Success != error &&
                    SocketError.IOPending != error)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                if (null != m_event)
                {
                    m_event.OnError(SocketErrorType.Receive, e, this);
                }

                return false;
            }

            return true;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if(null == m_socket)
                {
                    return;
                }

                int recvsize = m_socket.EndReceive(ar);

                if (0 >= recvsize)
                {
                    Disconnect(false);
                    return;
                }

                if (m_total_bufsize < m_offset + recvsize)
                {
                    Disconnect();
                    return;
                }

                int start_offset = 0;
                int packet_size = 0;
                Buffer.BlockCopy(m_buf, 0, m_total_buf, (int)m_offset, recvsize);
                m_offset += (uint)recvsize;

                while (start_offset + sizeof(int) <= m_offset)
                {
                    packet_size = BitConverter.ToInt32(m_total_buf, start_offset);
                    if (0 >= packet_size ||
                        start_offset + packet_size > m_total_bufsize)
                    {
                        Disconnect();
                        return;
                    }

                    if (start_offset + sizeof(int) + packet_size > m_offset)
                    {
                        break;
                    }

                    start_offset += sizeof(int);

                    byte[] packet_data = new byte[packet_size];
                    Buffer.BlockCopy(m_total_buf, start_offset, packet_data, 0, packet_size);
                    start_offset += packet_size;

                    if (null != m_event)
                    {
                        m_event.OnReceive(this, packet_data);
                    }
                }

                if (false == Connected())
                {
                    Disconnect(false);
                    return;
                }

                if (0 < start_offset)
                {
                    if (0 < m_offset - start_offset)
                    {
                        Buffer.BlockCopy(m_total_buf, start_offset, m_total_buf, 0, (int)(m_offset - (uint)start_offset));
                    }

                    m_offset -= (uint)start_offset;
                }

                Receive();
            }
            catch (Exception e)
            {
                if (null != m_event)
                {
                    m_event.OnError(SocketErrorType.Receive, e, this);
                }

                Disconnect(false);

                return;
            }
        }

        private ClientSocketEvent m_event;
        private Socket? m_socket;
        private byte[] m_buf;
        private uint m_bufsize;
        private byte[] m_total_buf;
        private uint m_total_bufsize;
        private uint m_offset;

        public object? StateObject;
    }
}
