using System;
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
namespace CodemastersTelemetry
{
    public struct TelemetryPacket
    {
        public float Time;

        public float LapTime;

        public float LapDistance;

        public float Distance;

        public float X;

        public float Y;

        public float Z;

        public float Speed;

        public float WorldSpeedX;

        public float WorldSpeedY;

        public float WorldSpeedZ;

        public float XR;

        public float Roll;

        public float ZR;

        public float XD;

        public float YD;

        public float ZD;

        public float SuspensionPositionRearLeft;

        public float SuspensionPositionRearRight;

        public float SuspensionPositionFrontLeft;

        public float SuspensionPositionFrontRight;

        public float SuspensionVelocityRearLeft;

        public float SuspensionVelocityRearRight;

        public float SuspensionVelocityFrontLeft;

        public float SuspensionVelocityFrontRight;

        public float WheelSpeedReadLeft;

        public float WheelSpeedRearRight;

        public float WheelSpeedFrontLeft;

        public float WheelSpeedFrontRight;

        public float Throttle;

        public float Steer;

        public float Brake;

        public float Clutch;

        public float Gear;

        public float LateralAcceleration;

        public float LongitudinalAcceleration;

        public float Lap;

        public float EngineRevs;

        public float SliProNativeSupport;

        public float RacePosition;

        public float KersRemaining;

        public float KersMaxLevel;

        public float DrsStatus;

        public float TractionControl;

        public float AntiLock;

        public float FuelRemaining;

        public float FuelCapacity;

        public float InPits;

        public float Sector;

        public float TimeSector1;

        public float TimeSector2;

        public float BrakeTemperatureRearLeft;

        public float BrakeTemperatureRearRight;

        public float BrakeTemperatureFrontLeft;

        public float BrakeTemperatureFrontRight;

        public float WheelPressureRearLeft;

        public float WheelPressureRearRight;

        public float WheelPressureFrontLeft;

        public float WheelPressureFrontRight;

        public float CompletedLapsInRace;

        public float TotalLapsInRace;

        public float TrackLength;

        public float PreviousLapTime;

        public float MaxRpm;

        public float IdleRpm;

        public float MaxGears;

        public float SessionType;

        public float DrsAllowed;

        public float TrackNumber;

        public float FIAFlags;

        public float SpeedInKmPerHour
        {
            get
            {
                return this.Speed * 3.6f;
            }
        }

        public bool IsSittingInPits
        {
            get
            {
                return Math.Abs(this.LapTime - 0f) < 1E-05f && Math.Abs(this.Speed - 0f) < 1E-05f;
            }
        }

        public bool IsInPitLane
        {
            get
            {
                return Math.Abs(this.LapTime - 0f) < 1E-05f;
            }
        }

        public string SessionTypeName
        {
            get
            {
                if (Math.Abs(this.SessionType - 9.5f) < 0.0001f)
                {
                    return "Race";
                }
                if (Math.Abs(this.SessionType - 10f) < 0.0001f)
                {
                    return "Time Trial";
                }
                if (Math.Abs(this.SessionType - 170f) < 0.0001f)
                {
                    return "Qualifying or Practice";
                }
                return "Other";
            }
        }
    }
}

