using ServerModule.Network;

namespace Packet
{
    public class PacketCommon : PacketBase<Define.PacketIndex>
    {
        public PacketCommon()
            : base(0)
        {
            CheckValue = Define.CheckValue;
        }

        protected PacketCommon(Define.PacketIndex _Index)
            : base(_Index)
        {
            CheckValue = Define.CheckValue;
        }

        public string CheckValue { get; set; }
    }
}
