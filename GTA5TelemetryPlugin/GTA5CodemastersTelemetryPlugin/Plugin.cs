using System;
using System.Configuration;
using GTA;
using CodemastersTelemetry;

/// <summary>
/// GTA V Codemasters Telemetry Plugin
/// 
/// This plugin enables GTA 5 to send telemetry data packets just like a Codemasters game (e.g. DiRT Rally) can do
/// Now you can use any Codemasters-compatible simracing dashboard with GTA5!
/// 
/// If this code works, it has been written by Carlo Iovino (carlo.iovino@outlook.com)
/// The Green Dragon Youtube Channel (www.youtube.com/carloxofficial)
/// 
/// </summary>
namespace GTA5Telemetry
{
    class GTA5TelemetryPlugin : Script
    {
        TelemetryWriter dataWriter;
        TelemetryPacket data = new TelemetryPacket();
        int previousGear = -1;
        bool doSequentialFix = false;
        bool doNeutralGearInference = true;
        float neutralGearSpeedKMH = 10f; // A minimum speed (in KMH) for inferring the car is on Neutral gear
        float neutralGearIdleRPMs = 0.4f; // A minimum rpms value for inferring the car is on Neutral gear
        int port = 20777;

        public GTA5TelemetryPlugin()
        {                        
            try
            {
                string param = ConfigurationManager.AppSettings["SequentialFix"].ToString();
                doSequentialFix = Int32.Parse(param) != 0;
                param = ConfigurationManager.AppSettings["NeutralGearInference"].ToString();
                doNeutralGearInference = Int32.Parse(param) != 0;
                param = ConfigurationManager.AppSettings["NeutralGearSpeedKMH"].ToString();
                neutralGearSpeedKMH = Single.Parse((param));
                param = ConfigurationManager.AppSettings["NeutralGearIdleRPMs"].ToString();
                neutralGearIdleRPMs = Single.Parse(param) / 100f;
                param = ConfigurationManager.AppSettings["Port"].ToString();
                port = Int32.Parse(param);

            }
            catch (Exception e)
            {

            }

            this.dataWriter = new TelemetryWriter(port);
            Tick += OnTick; // Add OnTick() as an event handler for the Tick event            
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

                if (vehicle.CurrentGear == 0)
                {
                    // Reverse gear is the number 10 in the Codemasters F1 implementation
                    data.Gear = 10;
                }
                else if (vehicle.CurrentGear == 1)
                {
                    if (doSequentialFix)
                    {
                        if (previousGear == 0)
                        {
                            // SEQUENTIAL FIX
                            // effectiveGear == 0 means 'R', shifting up means 'N'
                            // This fix is needed because in the original game the "Neutral" gear
                            // is not implemented, so you shift directly from R to 1
                            data.Gear = 0;
                        }
                    }
                    if (doNeutralGearInference)
                    {                       
                        // Inference
                        // When te speed is very low, but the Engine RPMs are high
                        // it is very likely that the Gear is N (or the clutch is down)
                        if (vehicle.Speed * 3.9f <= neutralGearSpeedKMH && vehicle.CurrentRPM >= neutralGearIdleRPMs)
                        {
                            data.Gear = 0;
                        }
                        else
                        {
                            data.Gear = vehicle.CurrentGear;
                        }
                    }
                    else
                    {
                        data.Gear = vehicle.CurrentGear;
                    }
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
                previousGear = vehicle.CurrentGear;
            }
            else
            {
                // Player on foot
                // We convert the player health in a "car-like" scale, from 0 to 200
                data.Speed = (player.Health / 359f) * 200f;
                data.Gear = Game.Player.WantedLevel;
                // We convert the armor value in a "0-1" scale
                data.EngineRevs = player.Armor / Game.Player.MaxArmor;
            }

            // Share data
            byte[] bytes = PacketUtilities.ConvertPacketToByteArray(data);
            dataWriter.SendPacket(bytes);
        }
    }
}
