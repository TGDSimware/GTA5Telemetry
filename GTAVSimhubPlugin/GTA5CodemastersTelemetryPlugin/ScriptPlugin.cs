using System;
using GTA;
using CodemastersTelemetry;

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
namespace GTAVSimhub.Plugin
{
    class GTA5TelemetryPlugin : Script
    {
        TelemetryWriter dataWriter;
        TelemetryPacket data = new TelemetryPacket();

        public GTA5TelemetryPlugin()
        {
            int port = 20777;
          
            this.dataWriter = new TelemetryWriter(port);

            Tick += OnTick; // Add OnTick as an event handler for the Tick event            
        }

        override protected void Dispose(bool disposing)
        {
            if (disposing) dataWriter.Dispose();
        }

        void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;
                 
            if (player.IsInVehicle())
            {
                // Player in vehicle
                Vehicle vehicle = player.CurrentVehicle;
                data.Speed = vehicle.Speed;
                data.EngineRevs = vehicle.CurrentRPM;

                if (vehicle.CurrentGear == 0) {
                    data.Gear = 10;
                }
                else if (vehicle.CurrentGear > 0)
                {
                    data.Gear = vehicle.CurrentGear;
                }
                else
                {
                    data.Gear = 0;
                }

                data.Steer = vehicle.SteeringScale;
                data.Throttle = vehicle.Acceleration;                
                data.MaxRpm = 1;
                data.IdleRpm = 0.2f;
                data.FuelRemaining = vehicle.FuelLevel;                
            }
            else
            {
                // Player on foot
                data.Speed = player.Health;
                data.Gear = Game.Player.WantedLevel;
                data.EngineRevs = player.Armor / Game.Player.MaxArmor;
            }

            // Share data
            byte[] bytes = PacketUtilities.ConvertPacketToByteArray(data);
            dataWriter.SendPacket(bytes);
        }
    }
}
