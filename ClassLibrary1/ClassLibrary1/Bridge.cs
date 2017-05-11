using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;

namespace GTAVSimHubBridge
{
    public class Bridge
    {


        /// hold vehicle status
        private Boolean LightsOn = false;
        private Boolean HandBrakeOn = false;
        private int RPM = 0;

        // maps an input value to the corresponding outpu value
        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        private UIContainer mContainer = null;

        void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;


            if (player.IsInVehicle())
            {
                Vehicle vehicle = player.CurrentVehicle;

                if (vehicle.EngineRunning)
                {
                    int speed = (int) vehicle.Speed;
                    int gear = vehicle.CurrentGear;


                    RPM = ((int)Remap(vehicle.CurrentRPM, 0, 1, 180, 0));


                    /*if (Game.IsControlPressed(2, GTA.Control.VehicleHandbrake))
                    {
                        if (HandBrakeOn == false)
                        {
                        }
                    }
                    else
                    {
                        if (HandBrakeOn == true)
                        {
                            HandBrakeOn = false;
                        }
                    }*/

                    if (vehicle.LightsOn != LightsOn)
                    {

                        LightsOn = vehicle.LightsOn;
                    }
                }
                else
                {
                    // engine not running  
                }


            }
            else
            {
               // player out of vehicle
            }


        }
    }
}
