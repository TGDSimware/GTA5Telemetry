using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTAVSimhub.Plugin;
using CodemastersReader;
using System.Threading;

namespace UdpClientTest
{
    

    class Program
    {
        static void Main(string[] args)
        {
            TelemetryWriter dataWriter = new TelemetryWriter(20777);
            

            for (int i = 0; ; i++)
            {
                //List<string> data = new List<string>();
                float v = 80.22231f + (float)i;

                TelemetryPacket data = new TelemetryPacket();
                // Player on foot
                data.Speed = v;
                data.IdleRpm = 0;
                data.EngineRevs = ((float)i / 10f );
                data.MaxRpm = 1;
                data.IdleRpm = 0.2f;
                data.Throttle = i * 4;
                
                data.Gear = 10;

                //data.Add(producer.packet("SpeedKmh", Convert.ToDouble(v)));

                // Share data
                byte[] bytes = PacketUtilities.ConvertPacketToByteArray(data);
                dataWriter.SendPacket(bytes);

                Thread.Sleep(1000);
            }
         

        }
    }
}
