using System;
using System.Runtime.InteropServices;
/// <summary>
/// GTA V Codemasters Telemetru Plugin
/// 
/// This plugin enables GTA 5 to send telemetry data packets just like a Codemasters game (e.g. DiRT Rally) can do
/// Now you can use any Codemasters-compatible simracing dashboard with GTA5!
/// 
/// If this code works, it has been written by Carlo Iovino (carlo.iovino@outlook.com)
/// The Green Dragon Youtube Channel (www.youtube.com/carloxofficial)
/// 
/// </summary>
namespace CodemastersTelemetry
{
    public static class PacketUtilities
    {
        public static byte[] ConvertPacketToByteArray(TelemetryPacket packet)
        {
            int num = Marshal.SizeOf<TelemetryPacket>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<TelemetryPacket>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }

        public static TelemetryPacket ConvertToPacket(byte[] bytes)
        {
            GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            TelemetryPacket arg_2A_0 = (TelemetryPacket)Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(TelemetryPacket));
            gCHandle.Free();
            return arg_2A_0;
        }
    }

}
