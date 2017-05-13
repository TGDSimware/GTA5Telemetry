using GameReaderCommon;
using System;

namespace GTA5Reader
{
    public class GTA5Manager :  GameManagerBase<TelemetryPacket, TelemetryReader>
    {
        public GTA5Manager() : base("GTA5", new string[]
        {
            "GTA5", "chrome"
        })
        {
            /*int port = 20777;
            if (!int.TryParse(ConfigurationManager.AppSettings["CodemastersUDPPort"], out port))
            {
                port = 20777;
            }*/
            this.datareader = new TelemetryReader();
        }

        public GTA5Manager(int port) : base("GTA5", new string[]
		{
            "GTA5", "chrome"
        })
		{
            this.datareader = new TelemetryReader();
        }

        /*public CodemastersManager(int port) : base("GTA 5", new string[]
        {
            "GTA 5"
        })
        {
            this.datareader = new TelemetryReader(port);
        }*/

        protected override TelemetryPacket GetNewRawData()
        {
            return this.datareader.LatestData;
        }

        protected override int GD_CurrentSectorIndex()
        {
            return 0;
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
            return this.datareader.IsRunning;
        }

        public override void Exit()
        {
            this.datareader.Dispose();
        }

        public override TelemetryPacket GetDataSample()
        {
            return default(TelemetryPacket);
        }

        protected override double GD_SpeedKmh()
        {
            return (double)base.NewRawData.SpeedKMH;
        }

        protected override double GD_Rpms()
        {
            return 0;
        }

        protected override string GD_Gear()
        {
            return this.datareader.GetGear((int)base.NewRawData.Gear);
        }

        /******************************************************************/
        protected override TimeSpan GD_AllTimeBest()
        {
            return TimeSpan.Zero;
        }

        protected override TimeSpan GD_BestLapTime()
        {
            return TimeSpan.Zero;
        }

        protected override double GD_Brake()
        {
            return 0.0;
        }

        protected override double GD_BrakeTemperature1()
        {
            return 0.0;
        }

        protected override double GD_BrakeTemperature2()
        {
            return 0.0;
        }

        protected override double GD_BrakeTemperature3()
        {
            return 0.0;
        }

        protected override double GD_BrakeTemperature4()
        {
            return 0.0;
        }

        protected override double[] GD_CarCoordinates()
        {
            return new double[]
            {
                0,0,0
            };
        }

        protected override double GD_CarDamage1()
        {
            return 0.0;
        }

        protected override double GD_CarDamage2()
        {
            return 0.0;
        }

        protected override double GD_CarDamage3()
        {
            return 0.0;
        }

        protected override double GD_CarDamage4()
        {
            return 0.0;
        }

        protected override double GD_CarDamage5()
        {
            return 0.0;
        }

        protected override string GD_CarModel()
        {
            return null;
        }

        protected override double GD_Clutch()
        {
            return 0.0;
        }

        protected override int GD_CompletedLaps()
        {
            return 0;
        }

        protected override TimeSpan GD_CurrentLapTime()
        {
            return TimeSpan.Zero;
        }

        protected override double? GD_DeltaToAllTimeBest()
        {
            return null;
        }

        protected override double? GD_DeltaToSessionBest()
        {
            return null;
        }

        protected override double GD_Engine_OilPressure()
        {
            return 0.0;
        }

        protected override double GD_Engine_OilTemperature()
        {
            return 0.0;
        }

        protected override double GD_Engine_WaterTemperature()
        {
            return 0.0;
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
            return 0; // fuel remaining
        }

        protected override double GD_Gas()
        {
            return 0;
        }

        protected override int GD_IsInPit()
        {
                return 0;        
        }

        protected override int GD_PitLimiterOn()
        {
            return 0;
        }

        protected override int GD_IsInPitLane()
        {            
                return 0;            
        }

        protected override TimeSpan GD_LastLapTime()
        {
            return TimeSpan.Zero;
        }

        protected override TimeSpan GD_LastSectorTime()
        {
            return TimeSpan.Zero;
        }

        protected override double GD_MaxFuel()
        {
            return 100.0;
        }

        protected override double GD_MaxRpm()
        {
            return 1.0;
        }

        protected override double GD_MaxTurbo()
        {
            return 1.0;
        }

        protected override int GD_Position()
        {
            return 0;
        }

        protected override TimeSpan GD_SessionTimeLeft()
        {
            return TimeSpan.Zero;
        }

        protected override string GD_SessionTypeName()
        {
            return "";
        }

        protected override int GD_TotalLaps()
        {
            return 0;
        }

        protected override string GD_TrackConfig()
        {
            return null;
        }

        protected override string GD_TrackName()
        {
            return "unknown";
        }

        protected override double GD_Turbo()
        {
            return 0.0;
        }

        protected override double GD_TyreCoreTemperature1()
        {
            return 0.0;
        }

        protected override double GD_TyreCoreTemperature2()
        {
            return 0.0;
        }

        protected override double GD_TyreCoreTemperature3()
        {
            return 0.0;
        }

        protected override double GD_TyreCoreTemperature4()
        {
            return 0.0;
        }

        protected override double GD_TyreDirtyLevel1()
        {
            return 0.0;
        }

        protected override double GD_TyreDirtyLevel2()
        {
            return 0.0;
        }

        protected override double GD_TyreDirtyLevel3()
        {
            return 0.0;
        }

        protected override double GD_TyreDirtyLevel4()
        {
            return 0.0;
        }

        protected override double GD_TyreWear1()
        {
            return 0.0;
        }

        protected override double GD_TyreWear2()
        {
            return 0.0;
        }

        protected override double GD_TyreWear3()
        {
            return 0.0;
        }

        protected override double GD_TyreWear4()
        {
            return 0.0;
        }

        protected override void PreParseData()
        {
        }
    }
}
