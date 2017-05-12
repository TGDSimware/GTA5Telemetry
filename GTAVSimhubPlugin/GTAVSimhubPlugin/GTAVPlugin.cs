using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Globalization;

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
        DataProducer dataProducer;

        const string P_GEAR = "Gear";
        const string P_GEARNUMBER = "GearNumber";
        const string P_NAME = "VehicleOrWeapon";
        const string P_SPEED = "SpeedKmh";
        const string P_RPMS = "Rpms";
        const string P_HEALTH = "Health";
        const string P_GAMEISRUNNING = "GameIsRunning";
        const string P_ONFIRE = "OnFire";
        const string P_VEHICLENAME = "VehicleName";
        const string P_INVEHICLE = "InVehicle";
        const string P_WEAPON = "Weapon";

        const string P_DEBUG = "DEBUG";
        static string[] GEARS = { "R", "N", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public GTAVSimHubClient()
        {
            dataProducer = new DataProducer("GTAVSimHubPlugin");

            Tick += OnTick; // Add OnTick as an event handler for the Tick event
            //Interval = 15;  // Set the update interval            
            //ScriptSettings.load(fileName)
        }

        string Packet(string property, object value)
        {
            string type;

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            type = value.GetType().Name;
            if (value.GetType() == typeof(Double))
            {

                value = ((Double)value).ToString(CultureInfo.InvariantCulture.NumberFormat);
            }
            string r = property + ":" + type + ":" + value;
            return r;
        }

        override protected void Dispose(bool disposing)
        {
            if (disposing) dataProducer.Dispose();
        }

        void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;
            List<string> dataList = new List<string>();            

            if (player.IsInVehicle())
            {
                // Player in vehicle
                Vehicle vehicle = player.CurrentVehicle;
                dataList.Add(Packet(P_INVEHICLE, 1));
                dataList.Add(Packet(P_ONFIRE, vehicle.IsOnFire ? 1 : 0));                
                dataList.Add(Packet(P_RPMS, Convert.ToDouble(vehicle.CurrentRPM)));
                dataList.Add(Packet(P_SPEED, Convert.ToDouble(vehicle.Speed * 2.51f /*as miles*/ * 1.6f /*as kilometers*/)));
                dataList.Add(Packet(P_NAME, vehicle.FriendlyName));
                dataList.Add(Packet(P_HEALTH, vehicle.Health));

                dataList.Add(Packet(P_GEARNUMBER, vehicle.CurrentGear));

                if (vehicle.CurrentGear < 9)
                {
                    dataList.Add(Packet(P_GEAR, GEARS[vehicle.CurrentGear + 1]));
                }
            }
            else
            {
                // Player on foot
                dataList.Add(Packet(P_INVEHICLE, 0));
                dataList.Add(Packet(P_RPMS, Convert.ToDouble(player.Health) / 100d));
                dataList.Add(Packet(P_SPEED, 0));
                
                dataList.Add(Packet(P_GEARNUMBER, Convert.ToInt32(0)));
                dataList.Add(Packet(P_GEAR, "N"));
                
            }

            dataList.Add(Packet(P_HEALTH, player.Health));
            dataList.Add(Packet(P_NAME, player.Weapons.Current.Name));
            dataList.Add(Packet(P_WEAPON, player.Weapons.Current.Name));

            // Share data
            string[] dataArray = dataList.ToArray();
            dataProducer.Share(dataArray);
        }
    }
}
