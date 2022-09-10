using System;
using System.Collections.Generic;

using ServerModule.Network;
using Packet.CtoS;

namespace Server
{
    public class NetworkLogic
    {
        public static void KeepAlive(SessionSocket _socket, KeepAlive _recvdata)
        {
            if (_socket.StateObject is LinkedListNode<KeepAliveInfo>)
            {
                ((LinkedListNode<KeepAliveInfo>)_socket.StateObject).ValueRef.m_recvtime = DateTime.Now;
            }
        }

        public static void ViewData(SessionSocket _socket, ViewData _recvdata)
        {
            Packet.StoC.ViewData? viewdata = Server.Instance.IPFS?.ls().Result;
            if (null == viewdata)
            {
                Console.WriteLine($"[{DateTime.Now.ToString()}.{DateTime.Now.Millisecond}]IPFS ls Fail");
                Server.Instance.Send(_socket, new Packet.StoC.ViewData());
                return;
            }

            viewdata.Name = "/";
            Server.Instance.Send(_socket, viewdata);
        }

        public static void Delete(SessionSocket _socket, Delete _recvdata)
        {
            bool? result = Server.Instance.IPFS?.Delete(_recvdata.Path).Result;
            Server.Instance.Send(_socket, new Packet.StoC.Delete()
            {
                Success = null == result ? false : result.Value
            });
        }

        public static void Upload(SessionSocket _socket, Upload _recvdata)
        {
            if (false == Server.Instance.UploadStart(_recvdata.IPFSPath, _recvdata.LocalPath, _recvdata.SelectPath, _recvdata.listFile))
            {
                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Overlap,
                    Message = "Upload Start Fail"
                });
                return;
            }

            if (false == Server.Instance.UploadGetFile(out string localpath, out string selectpath, out string filepath))
            {
                Server.Instance.UploadCancel();

                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Fail,
                    Message = "Upload Start and GetFile Fail"
                });
                return;
            }

            Server.Instance.Send(_socket, new Packet.StoC.Upload()
            {
                ResultState = Packet.StoC.Upload.State.Ready
            });

            Server.Instance.Send(_socket, new Packet.StoC.File()
            {
                LoaclPath = localpath,
                SelectPath = selectpath,
                FilePath = filepath
            });
        }

        public static void File(SessionSocket _socket, File _recvdata)
        {
            if (null == _recvdata.Data)
            {
                Server.Instance.UploadCancel();

                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Fail,
                    Message = "Upload File Data Null"
                });
                return;
            }

            if (false == Server.Instance.UploadSaveFile(_recvdata.FilePath, _recvdata.Data))
            {
                Server.Instance.UploadCancel();

                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Fail,
                    Message = "Upload SaveFile Fail"
                });
                return;
            }

            if (false == Server.Instance.UploadGetFile(out string localpath, out string selectpath, out string filepath))
            {
                Server.Instance.UploadCancel();

                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Fail,
                    Message = "Upload GetFile Fail"
                });
                return;
            }

            if (false == string.IsNullOrEmpty(filepath))
            {
                Server.Instance.Send(_socket, new Packet.StoC.File()
                {
                    LoaclPath = localpath,
                    SelectPath = selectpath,
                    FilePath = filepath
                });
                return;
            }

            if (false == Server.Instance.UploadGetInfo(out string ipfspath, out selectpath, out localpath, out List<string> listfile))
            {
                Server.Instance.UploadCancel();

                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Fail,
                    Message = "Upload GetPath Fail"
                });
                return;
            }

            if (null == Server.Instance.IPFS)
            {
                Server.Instance.UploadCancel();

                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Fail,
                    Message = "IPFS Not Found Setting"
                });
                return;
            }

            if (false == Server.Instance.IPFS.Upload(ipfspath).Result)
            {
                Server.Instance.UploadCancel();

                Server.Instance.Send(_socket, new Packet.StoC.Upload()
                {
                    ResultState = Packet.StoC.Upload.State.Fail,
                    Message = "IPFS Upload Fail."
                });
                return;
            }

            Server.Instance.UploadSuccess();
            Server.Instance.Send(_socket, new Packet.StoC.Upload()
            {
                ResultState = Packet.StoC.Upload.State.Complete
            });
        }
    }
}
