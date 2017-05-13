﻿using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Globalization;
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
                data.IdleRpm = vehicle.CurrentRPM;
                data.Gear = vehicle.CurrentGear;
            }
            else
            {
                // Player on foot
                data.Speed = player.Health;
                data.IdleRpm = player.IsShooting ? 100f : 0f;
                data.Gear = 1;
            }

            // Share data
            byte[] bytes = PacketUtilities.ConvertPacketToByteArray(data);
            dataWriter.SendPacket(bytes);
        }
    }
}