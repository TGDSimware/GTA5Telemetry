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
        int previousGear = 0;
        bool doSequentialFix = false;
        float idleSpeedKMH = 3f;        // A minimum speed (in KMH) for inferring the car is on Neutral gear
        float highRPMs = .2f;           // A minimum rpms value for inferring the car is on Neutral gear

        public GTA5TelemetryPlugin()
        {
            int port = 20777;

            this.dataWriter = new TelemetryWriter(port);
            try
            {
                string param = ConfigurationManager.AppSettings["SequentialFix"].ToString();
                if (Int32.Parse(param) != 0)
                {
                    doSequentialFix = true;
                }
            }
            catch (Exception e)
            {

            }

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
                previousGear = vehicle.CurrentGear;
                data.Speed = vehicle.Speed;
                data.EngineRevs = vehicle.CurrentRPM;

                if (vehicle.CurrentGear == 0)
                {
                    // Reverse gear is the number 10 in the Codemasters F1 implementation
                    data.Gear = 10;
                }
                else if (vehicle.CurrentGear > 0)
                {
                    if (doSequentialFix)
                    {
                        if (previousGear == 0 && vehicle.CurrentGear == 1)
                        {
                            // SEQUENTIAL FIX
                            // effectiveGear == 0 means 'R', shifting up means 'N'
                            // This fix is needed because in the original game the "Neutral" gear
                            // is not implemented, so you shift directly from R to 1
                            data.Gear = 0;
                        }
                    }
                    else
                    {
                        if (vehicle.CurrentGear > 0)
                        {
                            // Inference
                            // if the car is going backwards but the current gear is 1 or more
                            // it is very likely that the Gear is N :)
                            if (vehicle.Speed < 0)
                            {
                                data.Gear = 0;
                            }
                            // Inference
                            // When te speed is very low, but the Engine RPMs are high
                            // it is very likely that the Gear is N (or the clutch is down)
                            else if (vehicle.Speed * 3.9f <= idleSpeedKMH && vehicle.CurrentRPM >= highRPMs)
                            {
                                data.Gear = 0;
                            }                            
                        }
                        else
                        {
                            data.Gear = vehicle.CurrentGear;
                        }
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
            }
            else
            {
                // Player on foot
                // We convert the player health in a "car-like" scale, from 0 to 200
                data.Speed = (player.Health/359f)*200f;
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
