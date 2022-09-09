namespace ServerModule.Network
{
    public class PacketBase<PacketIndex>
    {
        public PacketBase()
        {
            Index = default!;
        }

        public PacketBase(PacketIndex _Index)
        {
            Index = _Index;
        }

        public PacketIndex Index { get; set; }
    }
}
