using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


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

        const string P_CURRENTGEAR = "Gear";
        const string P_SPEED = "SpeedKmh";
        const string P_RPMS = "Rpms";
        const string P_GAMEISRUNNING = "GameIsRunning";
        const string P_DEBUG = "DEBUG";

        public GTAVSimHubClient()
        {
            dataProducer = new DataProducer("GTAVSimHubPlugin");

            Tick += OnTick; // Add OnTick as an event handler for the Tick event
            //Interval = 15;  // Set the update interval            
            //ScriptSettings.load(fileName)
        }

        string dataRow(string property, object value)
        {
            string type;

            type = value.GetType().Name;
            string r = property + ":" + type + ":" + value.ToString();
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

                dataList.Add(dataRow(P_RPMS, Convert.ToDouble(vehicle.CurrentRPM)));
                dataList.Add(dataRow(P_SPEED, Convert.ToDouble(vehicle.Speed)));
                dataList.Add(dataRow(P_CURRENTGEAR, vehicle.CurrentGear == 0 ? "N" : vehicle.CurrentGear.ToString()));                
            }
            else
            {
                dataList.Add(dataRow(P_RPMS, Convert.ToDouble(player.IsInCombat ? 100 : 0)));
                dataList.Add(dataRow(P_SPEED, Convert.ToDouble(player.Health)));
                dataList.Add(dataRow(P_CURRENTGEAR, Convert.ToInt32(0)));
            }

            // Share data
            string[] dataArray = dataList.ToArray();
            dataProducer.Share(dataArray);
        }
    }
}
