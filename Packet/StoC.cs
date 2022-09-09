using System.Collections.Generic;

namespace Packet
{
    namespace StoC
    {
        public class KeepAlive : PacketCommon
        {
            public KeepAlive()
                : base(Define.PacketIndex.StoC_KeepAlive)
            {

            }
        }

        public class ViewData : PacketCommon
        {
            public enum DataType
            {
                Directory,
                File,
            }

            public ViewData()
                : base(Define.PacketIndex.StoC_ViewData)
            {
                Name = "";
                CID = "";
                Type = DataType.File;
                listData = new List<ViewData>();
            }

            public string Name { get; set; }
            public string CID { get; set; }
            public DataType Type { get; set; }
            public List<ViewData> listData { get; set; }
        }

        public class Delete : PacketCommon
        {
            public Delete()
                : base(Define.PacketIndex.StoC_Delete)
            {
                Success = false;
            }

            public bool Success { get; set; }
        }

        public class Upload : PacketCommon
        {
            public enum State
            {
                Ready,
                Overlap,
                Fail,
                Complete,
            }

            public Upload()
                : base(Define.PacketIndex.StoC_Upload)
            {
                ResultState = State.Fail;
                Message = "";
            }

            public State ResultState { get; set; }
            public string Message { get; set; }
        }

        public class File : PacketCommon
        {
            public File()
                : base(Define.PacketIndex.StoC_File)
            {
                LoaclPath = "";
                SelectPath = "";
                FilePath = "";
            }

            public string LoaclPath { get; set; }
            public string SelectPath { get; set; }
            public string FilePath { get; set; }
        }
    }
}
