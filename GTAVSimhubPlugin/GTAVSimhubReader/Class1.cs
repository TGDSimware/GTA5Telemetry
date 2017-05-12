using System;
using System.Collections.Generic;
using GameReaderCommon;
using SimHub.Plugins;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace GTAVReader
{
    [PluginName("GTA 5")]
    class GTAVManager : GameManagerBase<byte[], string[]>, IPlugin

    {
        public PluginManager PluginManager { get;  set; }

        public GTAVManager(string gameName, params string[] processnames) : base(gameName, processnames)
        {
            gameName = "GTA 5";
            processnames = new String[] { "GTA5.exe"};
        }

        public override void Exit()
        {
            
        }

        protected override byte[] GetNewRawData()
        {
            return null;
        }

        public override byte[] GetDataSample()
        {
            return null;
        }

        public override bool IsGameInRace()
        {
            return true;
        }

        protected override bool IsGamePaused()
        {
            return false;
        }

        protected override bool IsGameRunning()
        {
            return false;
        }

        protected override void PreParseData()
        {

        }

        protected override TimeSpan GD_AllTimeBest()
        {
            return new TimeSpan();
        }

        protected override TimeSpan GD_BestLapTime()
        {
            return new TimeSpan();
        }

        protected override double GD_Brake()
        {
            return 0;
        }

        protected override double GD_BrakeTemperature1()
        {
            return 0;
        }

        protected override double GD_BrakeTemperature2()
        {
            return 0;
        }

        protected override double GD_BrakeTemperature3()
        {
            return 0;
        }

        protected override double GD_BrakeTemperature4()
        {
            return 0;
        }

        protected override double[] GD_CarCoordinates()
        {
            return null;
        }

        protected override double GD_CarDamage1()
        {
            return 0;
        }

        protected override double GD_CarDamage2()
        {
            return 0;
        }

        protected override double GD_CarDamage3()
        {
            return 0;
        }

        protected override double GD_CarDamage4()
        {
            return 0;
        }

        protected override double GD_CarDamage5()
        {
            return 0;
        }

        protected override string GD_CarModel()
        {
            return "UNKNOWN";
        }

        protected override double GD_Clutch()
        {
            return 0;
        }

        protected override int GD_CompletedLaps()
        {
            return 0;
        }

        protected override TimeSpan GD_CurrentLapTime()
        {
            return new TimeSpan(0);
        }

        protected override int GD_CurrentSectorIndex()
        {
            return 0;
        }

        protected override double? GD_DeltaToAllTimeBest()
        {
            return 0;
        }

        protected override double? GD_DeltaToSessionBest()
        {
            return 0;
        }

        protected override double GD_Engine_OilPressure()
        {
            return 0;
        }

        protected override double GD_Engine_OilTemperature()
        {
            return 0;
        }

        protected override double GD_Engine_WaterTemperature()
        {
            return 0;
        }

        protected override int GD_Flag_Black()
        {
            return 0;
        }

        protected override int GD_Flag_Blue()
        {
            return 0;
        }

        protected override int GD_Flag_Checkered()
        {
            return 0;
        }

        protected override int GD_Flag_White()
        {
            return 0;
        }

        protected override int GD_Flag_Yellow()
        {
            return 0;
        }

        protected override double GD_Fuel()
        {
            return 0;
        }

        protected override double GD_Gas()
        {
            return 0;
        }

        protected override string GD_Gear()
        {
            return "N";
        }

        protected override int GD_IsInPit()
        {
            return 0;
        }

        protected override int GD_IsInPitLane()
        {
            return 0;
        }

        protected override TimeSpan GD_LastLapTime()
        {
            return new TimeSpan(0);
        }

        protected override TimeSpan GD_LastSectorTime()
        {
            return new TimeSpan(0);
        }

        protected override double GD_MaxFuel()
        {
            return 0;
        }

        protected override double GD_MaxRpm()
        {
            return 0;
        }

        protected override double GD_MaxTurbo()
        {
            return 0;
        }

        protected override int GD_PitLimiterOn()
        {
            return 0;
        }

        protected override int GD_Position()
        {
            return 0;
        }

        protected override double GD_Rpms()
        {
            return 0;
        }

        protected override TimeSpan GD_SessionTimeLeft()
        {
            return new TimeSpan(0);
        }

        protected override string GD_SessionTypeName()
        {
            return "UNKNOWN";
        }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        protected override double GD_SpeedKmh()
        {
            return 0;
        }

        protected override int GD_TotalLaps()
        {
            return 0;
        }

        protected override string GD_TrackConfig()
        {
            return "UNKNOWN";
        }

        protected override string GD_TrackName()
        {
            return "UNKNOWN";
        }

        protected override double GD_Turbo()
        {
            return 0;
        }

        protected override double GD_TyreCoreTemperature1()
        {
            return 0;
        }

        protected override double GD_TyreCoreTemperature2()
        {
            return 0;
        }

        protected override double GD_TyreCoreTemperature3()
        {
            return 0;
        }

        protected override double GD_TyreCoreTemperature4()
        {
            return 0;
        }

        protected override double GD_TyreDirtyLevel1()
        {
            return 0;
        }

        protected override double GD_TyreDirtyLevel2()
        {
            return 0;
        }

        protected override double GD_TyreDirtyLevel3()
        {
            return 0;
        }

        protected override double GD_TyreDirtyLevel4()
        {
            return 0;
        }

        protected override double GD_TyreWear1()
        {
            return 0;
        }

        protected override double GD_TyreWear2()
        {
            return 0;
        }

        protected override double GD_TyreWear3()
        {
            return 0;
        }

        protected override double GD_TyreWear4()
        {
            return 0;
        }

        public void Init(PluginManager pluginManager)
        {
         
        }

        public void End(PluginManager pluginManager)
        {
            
        }

        public Control GetSettingsControl(PluginManager pluginManager)
        {
            return new UserControl();
        }
    }
}
