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
    class GTA5TelemetryPluginSettings
    {
        public bool SequentialFix = false;
        public bool NeutralGearInference1 = false;
        public bool NeutralGearInference2 = false;
        public bool WaypointsNavigation = false;
        public bool CaptureManualTransmissionGearing = true; // Active by default
        public float NeutralGearSpeedKMH = 10f; // A minimum speed (in KMH) for inferring the car is on Neutral gear
        public float NeutralGearIdleRPMs = 0.4f;  // A minimum rpms value for inferring the car is on Neutral gear
        public Int32 port = 20777; // The UDP communication port
        public string ManualTransmissionNeutralGearDecorator = "hunt_weapon";
    }

    class GTA5TelemetryPlugin : Script
    {
        TelemetryWriter DataWriter;
        TelemetryPacket Data = new TelemetryPacket();
        int PreviousGear = -1;
        GTA5TelemetryPluginSettings Settings = new GTA5TelemetryPluginSettings();

        public GTA5TelemetryPlugin()
        {
            try
            {
                string param = ConfigurationManager.AppSettings["WaypointsNavigation"].ToString();
                Settings.WaypointsNavigation = Int32.Parse(param) != 0;
            }
            catch { }
            try
            {
                string param = ConfigurationManager.AppSettings["ManualTransmissionGearing"].ToString();
                Settings.CaptureManualTransmissionGearing = Int32.Parse(param) != 0;
            }
            catch { }
            try
            {
                string param = ConfigurationManager.AppSettings["SequentialFix"].ToString();
                Settings.SequentialFix = Int32.Parse(param) != 0;
            }
            catch { }
            try
            {
                string param = ConfigurationManager.AppSettings["NeutralGearInference1"].ToString();
                Settings.NeutralGearInference1 = Int32.Parse(param) != 0;
            }
            catch { }
            try
            {
                string param = ConfigurationManager.AppSettings["NeutralGearInference2"].ToString();
                Settings.NeutralGearInference2 = Int32.Parse(param) != 0;
            }
            catch { }
            try
            {
                string param = ConfigurationManager.AppSettings["NeutralGearSpeedKMH"].ToString();
                Settings.NeutralGearSpeedKMH = Single.Parse((param));
            }
            catch { }
            try
            {
                string param = ConfigurationManager.AppSettings["NeutralGearIdleRPMs"].ToString();
                Settings.NeutralGearIdleRPMs = Single.Parse(param) / 100f;
            }
            catch { }
            try
            {
                string param = ConfigurationManager.AppSettings["Port"].ToString();
                Settings.port = Int32.Parse(param);
            }
            catch { }

            this.DataWriter = new TelemetryWriter(Settings.port);
            Tick += OnTick; // Add OnTick() as an event handler for the Tick event            
        }

        override protected void Dispose(bool disposing)
        {
            if (disposing) DataWriter.Dispose();
        }

        void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            if (player.IsInVehicle())
            {
                // Player in vehicle
                Vehicle vehicle = player.CurrentVehicle;

                Data.Speed = vehicle.Speed;
                Data.EngineRevs = vehicle.CurrentRPM;
                Data.WorldSpeedX = vehicle.Velocity.X;
                Data.WorldSpeedY = vehicle.Velocity.Y;
                Data.WorldSpeedZ = vehicle.Velocity.Z;
                Data.X = vehicle.Position.X;
                Data.Y = vehicle.Position.Y;
                Data.Z = vehicle.Position.Z;
                Data.XR = vehicle.Rotation.X;
                Data.ZR = vehicle.Rotation.Z;
                Data.Steer = vehicle.SteeringAngle;
                Data.Throttle = vehicle.Acceleration;
                Data.MaxRpm = 1;
                Data.IdleRpm = 0.2f;
                Data.FuelRemaining = vehicle.FuelLevel;
                

                // Helicopter/Airplane specific
                if (player.IsInAir || player.IsInHeli)
                {
                    Data.Steer = vehicle.Rotation.Y;
                    Data.Distance = vehicle.HeightAboveGround;               
                }
                else
                {                                        
                    bool manualTransmissionNeutral = false;

                    Data.Gear = vehicle.CurrentGear;    // unless Neutral inference or Manual Transission Gearing
                    
                    if (Settings.WaypointsNavigation)
                    {
                        // Experimental: use waypoint position for navigation
                        var waypoint = GTA.World.GetWaypointPosition();
                        var distanceToWaypoint = vehicle.Position.DistanceTo2D(waypoint);
                        Data.Distance = distanceToWaypoint;
                        GTA.UI.Notify("Distance to Waypoint: " + distanceToWaypoint);
                    }

                    if (Settings.CaptureManualTransmissionGearing)
                    {
                        try
                        {
                            // This is a cross-script communication with the Manual Transmission mod
                            // for capturing the simulated Neutral gear
                            GTA.Native.InputArgument[] args =
                                {new GTA.Native.InputArgument(vehicle), Settings.ManualTransmissionNeutralGearDecorator};

                            // cross-script communication
                            manualTransmissionNeutral =
                                Convert.ToBoolean(
                                    GTA.Native.Function.Call<Int32>(GTA.Native.Hash.DECOR_GET_INT, args));
                        }
                        catch { }
                    }

                    if (manualTransmissionNeutral)
                    {
                        Data.Gear = 0;
                    }
                    else if (vehicle.CurrentGear == 0)
                    {
                        // Reverse gear is the number 10 in the Codemasters F1 implementation
                        Data.Gear = 10;
                    }
                    else if (vehicle.CurrentGear == 1)
                    {
                        if (Settings.SequentialFix)
                        {
                            if (PreviousGear == 0)
                            {
                                // SEQUENTIAL FIX
                                // effectiveGear == 0 means 'R', shifting up means 'N'
                                // This fix is needed because in the original game the "Neutral" gear
                                // is not implemented, so you shift directly from R to 1
                                Data.Gear = 0;
                            }
                        }
                        if (Settings.NeutralGearInference1)
                        {
                            // Inference 1
                            // When te speed is very low, but the Engine RPMs are high
                            // it is very likely that the Gear is N (or the clutch is down)
                            if (vehicle.Speed * 3.9f <= Settings.NeutralGearSpeedKMH &&
                                vehicle.CurrentRPM >= Settings.NeutralGearIdleRPMs)
                            {
                                Data.Gear = 0;
                            }
                            // Inference 2
                            // When te speed is very low, but the Engine RPMs are high
                            // it is very likely that the Gear is N (or the clutch is down)
                            if (Settings.NeutralGearInference2)
                            {
                                if (vehicle.Acceleration < 0 && vehicle.CurrentRPM >= Settings.NeutralGearIdleRPMs)
                                {
                                    // Inference 3 (to be tested)
                                    // Acceleration negative but RPM high
                                    // it is very likely that the Gear is N (or the clutch is down)
                                    Data.Gear = 0;
                                }
                            }
                        }
                    }

                    PreviousGear = vehicle.CurrentGear;
                }
            }
            else
            {
                // Player on foot
                // We convert the player health in a "car-like" scale, from 0 to 200
                Data.Speed = (player.Health / 359f) * 200f;
                Data.Gear = Game.Player.WantedLevel;
                // We convert the armor value in a "0-1" scale
                Data.EngineRevs = player.Armor / Game.Player.MaxArmor;
                Data.X = player.Position.X;
                Data.Y = player.Position.Y;
                Data.Z = player.Position.Z;                
            }

            // Share data
            byte[] bytes = PacketUtilities.ConvertPacketToByteArray(Data);
            DataWriter.SendPacket(bytes);
        }
    }
}
