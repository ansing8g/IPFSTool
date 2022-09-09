using System;
using System.Collections.Generic;

using ServerModule.Network;
using Packet;

namespace Server
{
    public class ServerSocket<PacketIndex> : ServerSocketEvent where PacketIndex : notnull
    {
        public ServerSocket()
        {
            Converter = new JsonConverter();

            m_accept_socket = new AcceptSocket(this);
            m_dispatcher = new Dispatcher<SessionSocket, Define.PacketIndex>();
        }

        public virtual void OnError(SocketErrorType _error_type, Exception _exception, SessionSocket? _sessionsocket)
        {
            System.Net.IPEndPoint? remote = _sessionsocket?.RemoteEndPoint as System.Net.IPEndPoint;
            Console.WriteLine($"Error EndPoint={remote}, Type={_error_type}, Message={_exception.Message}");
        }

        public virtual void OnAccept(SessionSocket _sessionsocket)
        {
            Server.Instance.ConnectSocket(_sessionsocket);

            System.Net.IPEndPoint? remote = _sessionsocket.RemoteEndPoint as System.Net.IPEndPoint;
            Console.WriteLine($"OnAccept Remote EndPoint={remote?.ToString()}");
        }

        public virtual void OnDisconnect(SessionSocket _sessionsocket)
        {
            System.Net.IPEndPoint? remote = _sessionsocket.RemoteEndPoint as System.Net.IPEndPoint;
            Console.WriteLine($"OnDisconnect Remote EndPoint={remote?.ToString()}");

            Server.Instance.DisconnectSocket(_sessionsocket);
        }

        public virtual void OnSend(SessionSocket _sessionsocket)
        {

        }

        public virtual void OnReceive(SessionSocket _sessionsocket, byte[] _data)
        {
            PacketCommon? packet_base;
            if (false == Converter.Deserialize<PacketCommon>(_data, out packet_base))
            {
                return;
            }

            if (null == packet_base ||
                false == packet_base.CheckValue.Equals(Define.CheckValue))
            {
                _sessionsocket.Disconnect();
                return;
            }

            FunctionBase<SessionSocket, Define.PacketIndex>? func_handler;
            Type? packet_type = null;
            if (false == m_dispatcher.GetFunction(packet_base.Index, out func_handler, out packet_type))
            {
                return;
            }

            if (null == func_handler ||
                null == packet_type)
            {
                return;
            }

            object? packet_object = null;
            if (false == Converter.Deserialize(_data, packet_type!, out packet_object))
            {
                return;
            }

            if (null == packet_object)
            {
                return;
            }

            func_handler.ExecuteFunction(_sessionsocket, (packet_object as PacketBase<Define.PacketIndex>)!);

            if (_sessionsocket.StateObject is LinkedListNode<KeepAliveInfo>)
            {
                ((LinkedListNode<KeepAliveInfo>)_sessionsocket.StateObject).ValueRef.m_recvtime = DateTime.Now;
            }
        }

        public bool Start(int _port, int _listen_count = 3, uint _buf_size = 1024000, uint _total_buf_size = 2048000)
        {
            return m_accept_socket.Start(_port, _listen_count, _buf_size, _total_buf_size);
        }

        public void RegistHandler<PacketObject>(Define.PacketIndex _packet_index, Action<SessionSocket, PacketObject> _func) where PacketObject : PacketBase<Define.PacketIndex>
        {
            m_dispatcher.RegistFunction(_packet_index, _func);
        }

        public IConverter Converter { get; private set; }

        private AcceptSocket m_accept_socket;
        private Dispatcher<SessionSocket, Define.PacketIndex> m_dispatcher;
    }
}
