namespace Packet
{
    public class Define
    {
        public enum PacketIndex : uint
        {
            CtoS_KeepAlive = 1,
            CtoS_ViewData,
            CtoS_Delete,
            CtoS_Upload,
            CtoS_File,

            StoC_KeepAlive = 1000,
            StoC_ViewData,
            StoC_Delete,
            StoC_Upload,
            StoC_File,
        }

        public static string CheckValue = "Test Check Value";
    }
}