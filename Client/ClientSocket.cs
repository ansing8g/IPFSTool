using ServerModule.Network;
using Packet;

namespace Client
{
    public class SocketEventInfo
    {
        public enum EventType
        {
            None,
            OnError,
            OnConnect,
            OnDisconnect,
            OnSend,
            OnReceive,
        }

        public SocketEventInfo(EventType _Event)
        {
            this.Event = _Event;
            this.PacketIndex = 0;
            this.Packet = null;
            this.ErrorType = null;
            this.Exception = null;
        }
        public SocketEventInfo(EventType _Event, Define.PacketIndex _PacketIndex, byte[] _Packet)
        {
            this.Event = _Event;
            this.PacketIndex = _PacketIndex;
            this.Packet = _Packet;
            this.ErrorType = null;
            this.Exception = null;
        }
        public SocketEventInfo(EventType _Event, SocketErrorType _ErrorType, Exception _Exception)
        {
            this.Event = _Event;
            this.PacketIndex = 0;
            this.Packet = null;
            this.ErrorType = _ErrorType;
            this.Exception = _Exception;
        }

        public EventType Event;
        public Define.PacketIndex PacketIndex;
        public byte[]? Packet;
        public SocketErrorType? ErrorType;
        public Exception? Exception;
    }

    public class ClientSocket : ClientSocketEvent
    {
        public ClientSocket(Form _Form)
            : base()
        {
            Converter = new JsonConverter();
            Dispatcher = new Dispatcher<ClientSocket, Define.PacketIndex>();

            m_connectSocket = new ConnectSocket(this);

            m_form = _Form;
            m_lockEvent = new object();
            m_queueEvent = new Queue<SocketEventInfo>();

            Dispatcher.RegistFunction<Packet.StoC.KeepAlive>(Define.PacketIndex.StoC_KeepAlive, m_form.m_NetworkLogic.KeepAlive);
            Dispatcher.RegistFunction<Packet.StoC.ViewData>(Define.PacketIndex.StoC_ViewData, m_form.m_NetworkLogic.ViewData);
            Dispatcher.RegistFunction<Packet.StoC.Delete>(Define.PacketIndex.StoC_Delete, m_form.m_NetworkLogic.Delete);
            Dispatcher.RegistFunction<Packet.StoC.Upload>(Define.PacketIndex.StoC_Upload, m_form.m_NetworkLogic.Upload);
            Dispatcher.RegistFunction<Packet.StoC.File>(Define.PacketIndex.StoC_File, m_form.m_NetworkLogic.File);
        }

        public void OnError(SocketErrorType _error_type, Exception _exception, ConnectSocket? _connectsocket)
        {
            lock (m_lockEvent)
            {
                m_queueEvent.Enqueue(new SocketEventInfo(SocketEventInfo.EventType.OnError, _error_type, _exception));
            }
        }

        public void OnConnect(ConnectSocket _connectsocket)
        {
            lock (m_lockEvent)
            {
                m_queueEvent.Enqueue(new SocketEventInfo(SocketEventInfo.EventType.OnConnect));
            }
        }

        public void OnDisconnect(ConnectSocket _connectsocket)
        {
            lock (m_lockEvent)
            {
                m_queueEvent.Enqueue(new SocketEventInfo(SocketEventInfo.EventType.OnDisconnect));
            }
        }

        public void OnSend(ConnectSocket _connectsocket)
        {

        }

        public void OnReceive(ConnectSocket _connectsocket, byte[] _data)
        {
            PacketCommon? packet_base = null;
            if (false == Converter.Deserialize(_data, out packet_base))
            {
                return;
            }

            if (null == packet_base)
            {
                return;
            }

            lock (m_lockEvent)
            {
                m_queueEvent.Enqueue(new SocketEventInfo(SocketEventInfo.EventType.OnReceive, packet_base.Index, _data));
            }

            //FunctionBase<ClientSocket, Define.PacketIndex>? func_handler;
            //Type? packet_type = null;
            //if (false == m_dispatcher.GetFunction(packet_base.Index, out func_handler, out packet_type))
            //{
            //    return;
            //}
            //
            //if (null == func_handler)
            //{
            //    return;
            //}
            //
            //func_handler.ExecuteFunction(this, packet_base);
        }

        public bool Send<PacketObject>(PacketObject _packet_object)
        {
            if (null == _packet_object)
            {
                return false;
            }

            byte[]? byte_data = null;
            if (false == Converter.Serialize(_packet_object!, typeof(PacketObject), out byte_data))
            {
                return false;
            }

            if (null == byte_data)
            {
                return false;
            }

            return this.m_connectSocket.Send(byte_data);
        }

        public bool Connect(string ip, int port)
        {
            return m_connectSocket.Connect(ip, port);
        }

        public void Disconnect()
        {
            m_connectSocket.Disconnect();
        }

        public bool Connected
        {
            get { return m_connectSocket.Connected(); }
            private set { }
        }

        public bool PopEvent(out SocketEventInfo _SocketEventInfo)
        {
            lock (m_lockEvent)
            {
                if (0 < m_queueEvent.Count)
                {
                    _SocketEventInfo = m_queueEvent.Dequeue();
                    return true;
                }
                else
                {
                    _SocketEventInfo = new SocketEventInfo(SocketEventInfo.EventType.None);
                    return false;
                }
            }
        }

        public JsonConverter Converter;
        public Dispatcher<ClientSocket, Define.PacketIndex> Dispatcher;

        private ConnectSocket m_connectSocket;

        private Form m_form;
        private object m_lockEvent;
        private Queue<SocketEventInfo> m_queueEvent;
    }
}
