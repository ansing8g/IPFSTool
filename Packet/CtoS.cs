using System.Collections.Generic;

namespace Packet
{
    namespace CtoS
    {
        public class KeepAlive : PacketCommon
        {
            public KeepAlive()
                : base(Define.PacketIndex.CtoS_KeepAlive)
            {

            }
        }

        public class ViewData : PacketCommon
        {
            public ViewData()
                : base(Define.PacketIndex.CtoS_ViewData)
            {

            }
        }

        public class Delete : PacketCommon
        {
            public Delete()
                : base(Define.PacketIndex.CtoS_Delete)
            {
                Path = "";
            }

            public string Path { get; set; }
        }

        public class Upload : PacketCommon
        {
            public Upload()
                : base(Define.PacketIndex.CtoS_Upload)
            {
                IPFSPath = "";
                LocalPath = "";
                SelectPath = "";
                listFile = new List<string>();
            }

            public string IPFSPath { get; set; }
            public string LocalPath { get; set; }
            public string SelectPath { get; set; }
            public List<string> listFile { get; set; }
        }

        public class File : PacketCommon
        {
            public File()
                : base(Define.PacketIndex.CtoS_File)
            {
                FilePath = "";
                Data = null;
            }

            public string FilePath { get; set; }
            public byte[]? Data { get; set; }
        }
    }
}
