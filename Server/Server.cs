using System;
using System.Collections.Generic;

using ServerModule.Network;
using ServerModule.Utility;
using Packet;
using Packet.CtoS;

namespace Server
{
    public class KeepAliveInfo
    {
        public KeepAliveInfo(SessionSocket _socket)
        {
            m_socket = _socket;
            m_recvtime = DateTime.Now;
        }

        public SessionSocket m_socket;
        public DateTime m_recvtime;
    }

    public class UploadFileInfo
    {
        public UploadFileInfo()
        {
            FilePath = "";
            IsUploaded = false;
        }

        public string FilePath { get; set; }
        public bool IsUploaded { get; set; }
    }

    public class UploadInfo
    {
        public UploadInfo()
        {
            IPFSPath = "";
            LocalPath = "";
            SelectPath = "";
            dicFile = new Dictionary<string, UploadFileInfo>();
        }

        public string IPFSPath { get; set; }
        public string LocalPath { get; set; }
        public string SelectPath { get; set; }
        public Dictionary<string, UploadFileInfo> dicFile { get; set; }
    }

    public class Server
    {
        private Server()
        {
            UploadPath = "";

            m_uploadinfo = null;
            m_loackUploadInfo = new object();

            m_timerManager = new TimerManager();
            m_lockKeepAliveCheck = new object();
            m_llKeepAliveCheck = new LinkedList<KeepAliveInfo>();
            m_serverSocket = new ServerSocket<Define.PacketIndex>();

            m_timerManager.Regist(1000.0, OnTimer_KeepAlive);
        }

        public static Server Instance
        {
            get { return m_Instance; }
            private set { m_Instance = value; }
        }
        private static Server m_Instance = new Server();

        public bool Start(int port, string ipfsaddress, string uploadpath)
        {
            UploadPath = uploadpath;

            IPFS = new IPFS(ipfsaddress);

            m_serverSocket.RegistHandler<KeepAlive>(Define.PacketIndex.CtoS_KeepAlive, NetworkLogic.KeepAlive);
            m_serverSocket.RegistHandler<ViewData>(Define.PacketIndex.CtoS_ViewData, NetworkLogic.ViewData);
            m_serverSocket.RegistHandler<Delete>(Define.PacketIndex.CtoS_Delete, NetworkLogic.Delete);
            m_serverSocket.RegistHandler<Upload>(Define.PacketIndex.CtoS_Upload, NetworkLogic.Upload);
            m_serverSocket.RegistHandler<File>(Define.PacketIndex.CtoS_File, NetworkLogic.File);

            if (false == m_serverSocket.Start(port))
            {
                return false;
            }

            return true;
        }

        public bool Send<PacketObject>(SessionSocket _sessionsocket, PacketObject _packet_object)
        {
            if (null == m_serverSocket.Converter ||
                null == _packet_object)
            {
                return false;
            }

            byte[]? byte_data = null;
            if (false == m_serverSocket.Converter.Serialize(_packet_object!, typeof(PacketObject), out byte_data))
            {
                return false;
            }

            if (null == byte_data)
            {
                return false;
            }

            return _sessionsocket.Send(byte_data);
        }

        public void ConnectSocket(SessionSocket _socket)
        {
            lock (m_lockKeepAliveCheck)
            {
                _socket.StateObject = m_llKeepAliveCheck.AddLast(new KeepAliveInfo(_socket));
            }
        }

        public void DisconnectSocket(SessionSocket _socket)
        {
            if (null == _socket ||
                null == _socket.StateObject)
            {
                return;
            }

            LinkedListNode<KeepAliveInfo>? node = _socket.StateObject as LinkedListNode<KeepAliveInfo>;
            if (null == node)
            {
                return;
            }

            lock (m_lockKeepAliveCheck)
            {
                m_llKeepAliveCheck.Remove(node);
            }
        }

        private void OnTimer_KeepAlive()
        {
            List<LinkedListNode<KeepAliveInfo>> listTimeOverNode = new List<LinkedListNode<KeepAliveInfo>>();
            Packet.StoC.KeepAlive keepalive = new Packet.StoC.KeepAlive();
            DateTime now = DateTime.Now;

            lock (m_lockKeepAliveCheck)
            {
                for (LinkedListNode<KeepAliveInfo>? node = m_llKeepAliveCheck.First; node != null; node = node.Next)
                {
                    if (now > node.ValueRef.m_recvtime.AddSeconds(10.0))
                    {
                        //listTimeOverNode.Add(node);
                    }
                    else if (now >= node.ValueRef.m_recvtime.AddSeconds(5.0))
                    {
                        Send(node.ValueRef.m_socket, keepalive);
                    }
                }

                foreach (LinkedListNode<KeepAliveInfo> node in listTimeOverNode)
                {
                    node.ValueRef.m_socket.Disconnect();
                }
            }
        }

        public bool UploadStart(string ipfspath, string localpath, string selectpath, List<string> listfile)
        {
            lock (m_lockKeepAliveCheck)
            {
                if (null != m_uploadinfo)
                {
                    return false;
                }

                if (true == System.IO.Directory.Exists(UploadPath))
                {
                    System.IO.Directory.Delete(UploadPath, true);
                }

                m_uploadinfo = new UploadInfo();
                m_uploadinfo.IPFSPath = ipfspath;
                m_uploadinfo.LocalPath = localpath;
                m_uploadinfo.SelectPath = selectpath;
                foreach (string file in listfile)
                {
                    if (false == m_uploadinfo.dicFile.ContainsKey(m_uploadinfo.SelectPath + file))
                    {
                        m_uploadinfo.dicFile.Add(m_uploadinfo.SelectPath + file, new UploadFileInfo() { FilePath = file, IsUploaded = false });
                    }
                }
            }

            return true;
        }

        public void UploadCancel()
        {
            lock (m_lockKeepAliveCheck)
            {
                if (true == System.IO.Directory.Exists(UploadPath))
                {
                    System.IO.Directory.Delete(UploadPath, true);
                }

                m_uploadinfo = null;
            }
        }

        public void UploadSuccess()
        {
            lock (m_lockKeepAliveCheck)
            {
                if (true == System.IO.Directory.Exists(UploadPath))
                {
                    //System.IO.Directory.Delete(UploadPath, true);
                }

                m_uploadinfo = null;
            }
        }

        public bool UploadGetFile(out string localpath, out string selectpath, out string filepath)
        {
            localpath = "";
            selectpath = "";
            filepath = "";

            lock (m_lockKeepAliveCheck)
            {
                if (null == m_uploadinfo)
                {
                    return false;
                }

                foreach (KeyValuePair<string, UploadFileInfo> info in m_uploadinfo.dicFile)
                {
                    if (false == info.Value.IsUploaded)
                    {
                        localpath = m_uploadinfo.LocalPath;
                        selectpath = m_uploadinfo.SelectPath;
                        filepath = info.Value.FilePath;
                        return true;
                    }
                }
            }

            return true;
        }

        public bool UploadSaveFile(string filepath, byte[] data)
        {
            lock (m_lockKeepAliveCheck)
            {
                if (null == m_uploadinfo ||
                    false == m_uploadinfo.dicFile.ContainsKey(m_uploadinfo.SelectPath + filepath))
                {
                    return false;
                }

                string fpath = (UploadPath + filepath).Replace('/', '\\');
                string? dpath = System.IO.Path.GetDirectoryName(fpath);
                if(null == dpath)
                {
                    return false;
                }

                if (false == System.IO.Directory.Exists(dpath))
                {
                    System.IO.Directory.CreateDirectory(dpath);
                }

                System.IO.File.WriteAllBytes(fpath, data);

                m_uploadinfo.dicFile[m_uploadinfo.SelectPath + filepath].IsUploaded = true;
            }

            return true;
        }

        public bool UploadGetInfo(out string ipfs, out string select, out string local, out List<string> listfile)
        {
            ipfs = "";
            select = "";
            local = "";
            listfile = new List<string>();

            lock (m_lockKeepAliveCheck)
            {
                if(null == m_uploadinfo)
                {
                    return false;
                }

                ipfs = m_uploadinfo.IPFSPath;
                select = m_uploadinfo.SelectPath;
                local = m_uploadinfo.LocalPath;
                foreach(KeyValuePair<string, UploadFileInfo> info in m_uploadinfo.dicFile)
                {
                    listfile.Add(info.Value.FilePath);
                }
            }

            return true;
        }

        public IPFS? IPFS { get; private set; }
        public string UploadPath;

        private UploadInfo? m_uploadinfo;
        private object m_loackUploadInfo;

        private TimerManager m_timerManager;
        private object m_lockKeepAliveCheck;
        private LinkedList<KeepAliveInfo> m_llKeepAliveCheck;
        private ServerSocket<Define.PacketIndex> m_serverSocket;
    }
}
