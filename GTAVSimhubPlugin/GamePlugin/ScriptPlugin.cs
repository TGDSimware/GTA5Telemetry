using System;
using GTA;
using CodemastersReader;

/// <summary>
/// GTA V Simhub Plugin
/// 
/// If this code works, it has been written by Carlo Iovino (carlo.iovino@outlook.com)
/// The Green Dragon Youtube Channel (www.youtube.com/carloxofficial
/// 
/// </summary>
namespace GTAVSimhub.Plugin
{
    class GTAVSimHubClient : Script
    {
        TelemetryWriter dataWriter;
        TelemetryPacket data = new TelemetryPacket();

        public GTAVSimHubClient()
        {
            int port = 20777;
          
            this.dataWriter = new TelemetryWriter(port);

            Tick += OnTick; // Add OnTick as an event handler for the Tick event
            //Interval = 15;  // Set the update interval            
            //ScriptSettings.load(fileName)
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
                if (vehicle.EngineRunning)
                {
                    data.Gear = 0;
                }
                else
                {
                    data.Gear = vehicle.CurrentGear == 0 ? 10 : vehicle.CurrentGear;
                }
                
                data.Lap = vehicle.CurrentGear;               
                data.MaxRpm = 1;
                data.IdleRpm = 0.2f;
                data.FuelRemaining = vehicle.FuelLevel;
            }
            else
            {
                // Player on foot
                data.Speed = player.Health;
                data.EngineRevs = player.IsShooting ? 1 : 0;
                data.Gear = 1;
            }

            // Share data
            byte[] bytes = PacketUtilities.ConvertPacketToByteArray(data);
            dataWriter.SendPacket(bytes);
        }
    }
}
