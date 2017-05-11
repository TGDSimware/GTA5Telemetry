using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

        /// hold vehicle status
        private Boolean LightsOn = false;
        //private Boolean HandBrakeOn = false;

        const string P_CURRENTGEAR = "DataCorePlugin.GameData.NewData.Gear";
        const string P_SPEED = "DataCorePlugin.GameData.NewData.SpeedKmh";
        const string P_RPMS = "DataCorePlugin.GameData.NewData.Rpms";

        public GTAVSimHubClient()
        {
            dataProducer = new DataProducer("GTAVSimHubPlugin");
        }

        string dataRow(string property, object value)
        {
            string type;

            type = value.GetType().Name;
            string r = property + ":" + type + ":" + value.ToString();
            return r;
        }

        void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;
            List<string> dataList = new List<string>();            

            if (player.IsInVehicle())
            {
                // Player in vehicle
                Vehicle vehicle = player.CurrentVehicle;

                dataList.Add(dataRow(P_RPMS, vehicle.CurrentRPM));
                dataList.Add(dataRow(P_SPEED, vehicle.Speed));
                dataList.Add(dataRow(P_CURRENTGEAR, vehicle.CurrentGear));                
            }
            else
            {
                dataList.Add(dataRow(P_RPMS, player.IsInCombat ? 100 : 0));
                dataList.Add(dataRow(P_SPEED, player.Health));
                dataList.Add(dataRow(P_CURRENTGEAR, 0));
            }

            // Share data
            string[] dataArray = dataList.ToArray();
            dataProducer.Share(dataArray);
        }
    }
}
