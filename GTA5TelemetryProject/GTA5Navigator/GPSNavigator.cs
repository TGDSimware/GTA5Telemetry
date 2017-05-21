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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Media;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Threading;
using System.Runtime.InteropServices;

namespace GTA5Navigator
{
    static class NavText
    {
        public static string[] Hint;
        public static string[] Dir;
        public static string MessageOn;
        public static string Unit;
    }

    class GPSNavigator : Script
    {
        private float LastHint = -1;
        private float LastHintDistance = -1;
        private bool IsRunning;
        private Vector3 CurrentPos;
        private Vector3 LastPos;
        int TurningL = 0;
        int TurningR = 0;
        int ToDest = 0;
        int Recalc = 0;
        float dPoint = 20;

        float LastTurnDistance = 0;
        Vehicle CurrentVehicle = null;
        private Vector3 Destination;

        private int[] tPoint = { 5, 50, 110, 160, 210 }; //Turning points
        private int _DEBUG = 0;

        public GPSNavigator()
        {            
            LoadSettings();
            
            IsRunning = false;
            Destination = new Vector3();
            Tick += OnTick; // Add OnTick() as an event handler for the Tick event            
        }

        private void LoadSettings()
        {
            try
            {
                ScriptSettings scriptSettings = ScriptSettings.Load(@"scripts\GTA5Navigator\Settings.ini");
                
                // Load settings                               
                string hints = scriptSettings.GetValue<string>("LANGUAGE", "HINTS",
                    "Girare,Proseguire dritto,Inversione,Uscita,Direzione Errata,Ricalcolo,Arrivo a destinazione");
                NavText.Hint = hints.Split(',');

                string directions = scriptSettings.GetValue<string>("LANGUAGE", "DIRECTIONS", "-,Right,Left");
                NavText.Dir = directions.Split(',');

                NavText.Unit = scriptSettings.GetValue<string>("LANGUAGE", "UNIT", "Km");
                NavText.MessageOn = scriptSettings.GetValue<string>("LANGUAGE", "MESSAGEON", "Navigatore attivo. Distanza stimata");

                string points = scriptSettings.GetValue<string>("SETTINGS", "TURNPOINTS", "5,40,105,155,210");
                string[] sPoints = points.Split(',');
                for (int i = 0; i < sPoints.Length; i++) tPoint[i] = Convert.ToInt32(sPoints[i]);

                int updateInterval = scriptSettings.GetValue<int>("SETTINGS", "UPDATEINTERVAL", 100);
                this.Interval = updateInterval;

                this.dPoint = scriptSettings.GetValue<float>("SETTINGS", "DPOINT", 20);                

                int volume = scriptSettings.GetValue<int>("SETTINGS", "VOLUME", 10);
                //VolumeControl.SetVolume(volume);

                _DEBUG = scriptSettings.GetValue<int>("SETTINGS", "DEBUG", 1);
                if (_DEBUG == 1)
                {
                    UI.Notify("LoadSettings(): Settings loaded");
                }
            }
            catch (Exception e)
            {
                UI.Notify("LoadSettings(): " + e.Message);
                _DEBUG = 1;
                this.Interval = 100;
            }
        }

        void OnTick(object sender, EventArgs e)
        {
            try
            {
                Ped player = Game.Player.Character;

                if (player.IsInVehicle())
                {
                    // Player in vehicle
                    Vehicle vehicle = player.CurrentVehicle;

                    if (Game.IsWaypointActive)
                    {
                        if (!World.GetWaypointPosition().Equals(Destination))
                        {
                            this.Start(vehicle, World.GetWaypointPosition());
                        }
                        else
                        {
                            this.setPosition(vehicle.Position);
                        }
                    }
                }
            }
            catch (Exception problem)
            {
                UI.Notify("OnTick: " + problem.Message);
            }
        }

        private void Start(Vehicle v, Vector3 destination)
        {
            try
            {
                Announce(Voice.Calculating);
                LoadSettings();
                CurrentVehicle = v;
                LastPos = v.Position;
                Destination = destination;
                ToDest = 0;
                IsRunning = true;
                var travelDistance = Math.Round(Math.Ceiling(v.Position.DistanceTo2D(Destination)) / 1000d, 1);

                if (travelDistance > 0)
                {
                    UI.Notify(NavText.MessageOn + ": " + travelDistance + NavText.Unit);
                }
            }
            catch (Exception problem)
            {
                UI.Notify("Start(): " + problem.Message);
            }
        }

        private float DistanceRemaining
        {
            get
            {
                return CurrentPos.DistanceTo2D(Destination);
            }
        }

        private void Announce(VoiceEntry voice, bool mute = false)
        {
            try
            {
                try
                {
                    string notify;
                    if (voice.Dist > 0)
                    {
                        notify = voice.Dist + "m: " + NavText.Hint[(int)voice.Hint] + " " + NavText.Dir[(int)voice.Dir];
                    }
                    else
                    {
                        notify = NavText.Hint[(int)voice.Hint] + " " + NavText.Dir[(int)voice.Dir];
                    }
                    UI.Notify(notify);
                }
                catch(Exception problem)
                {
                    UI.Notify("Announce(): " + problem.Message);
                }                

                using (System.Media.SoundPlayer snd = new System.Media.SoundPlayer())
                {
                    if (!mute)
                    {
                        String wavFilename = @"scripts\GTA5Navigator\gpsvoices\" + voice.File + ".wav";
                        snd.SoundLocation = wavFilename;
                        snd.Load();
                        snd.Play();
                        Wait(2500);
                    }
                }
            }
            catch (Exception problem)
            {
                UI.Notify("Announce(): " + problem.Message);
            }
        }

        public void setPosition(Vector3 pos)
        {
            CurrentPos = pos;
            try
            {
                //Position = value;
                var dtc = GenerateDirectionsToCoord(Destination);

                float disToNextTurn = Convert.ToSingle(Math.Ceiling(Convert.ToSingle(dtc.Item3)));
                int hint = Convert.ToInt32(dtc.Item1);
                string boh = dtc.Item2.ToString();

                Process(hint, disToNextTurn, 10);
            }
            catch (Exception problem)
            {
                UI.Notify("setPosition(): " + problem.Message);
            }
        }


        private bool InRange(float arg, float min, float max)
        {
            return min <= arg && arg <= max;
        }

        public void Process(float hint, float disToNextTurn, float disToWaypoint)
        {
            float distance = DistanceRemaining;
            float delta = Math.Abs(distance - LastHintDistance);

            if (_DEBUG == 1)
            {
                UI.ShowSubtitle("Hint " + hint + " @ " + disToNextTurn + "m delta = " + delta + "m");
            }

            switch (hint)
            {
                case 0:
                    break;
                case 1:
                    ResetDirective();
                    if (disToNextTurn == 0)
                    {
                        Recalc++;
                        if (Recalc >= 20)
                        {
                            Announce(Voice.WrongDirection);
                            Recalc = 0;
                        }
                    }
                    else
                    {
                        Recalc = 0;
                        AnnounceInversion(disToNextTurn);
                    }
                    break;
                case 2:
                    ResetDirective();
                    if (LastHint != 2) Announce(Voice.Keep);
                    break;
                case 3:
                    ResetDirective();
                    // Never!
                    break;
                case 4:
                    AnnounceTurnL(disToNextTurn);
                    break;
                case 5:
                    AnnounceTurnR(disToNextTurn);
                    break;
                case 6:
                    // Never!
                    break;
                case 7: //Keep right
                    ResetDirective();
                    break;
                case 8:
                    if (FindHeading() < -.8)
                        AnnounceInversion(disToNextTurn);
                    else
                        AnnounceExitLeft(disToNextTurn);
                    break;
                case 9:
                    AnnounceExitRight(disToNextTurn);
                    break;
            }

            if (distance < tPoint[3])
            {
                AnnounceDest(distance);
            }

            LastHintDistance = DistanceRemaining;
            LastHint = hint;
        }

        private void ResetDirective()
        {
            TurningR = 0;
            TurningL = 0;
        }

        private int FindSide()
        {
            int side = 0;
            Vector3 Pos = CurrentVehicle.Position;
            Vector2 DCAR = new Vector2(Pos.X - LastPos.X, Pos.Y - LastPos.Y);
            Vector2 DDST = new Vector2(Destination.X - LastPos.X, Destination.Y - LastPos.Y);
            float HCAR = DCAR.ToHeading();
            float HDST = DDST.ToHeading();
            side = HCAR > HDST ? 1 : -1;    // 1 = right side, -1, left side
            return side;
        }

        private float FindHeading()
        {
            Vector3 Pos = CurrentVehicle.Position;
            Vector2 DCAR = new Vector2(Pos.X - LastPos.X, Pos.Y - LastPos.Y);
            Vector2 DDST = new Vector2(Destination.X - LastPos.X, Destination.Y - LastPos.Y);
            float HCAR = DCAR.ToHeading();
            float HDST = DDST.ToHeading();
            return Convert.ToSingle(Math.Cos(Math.Abs(HCAR - HDST))); // 1 = right, 0 normal, -1 opposite
        }

        private void AnnounceDest(float distance)
        {
            if (ToDest <= 0 && InRange(distance, tPoint[2], tPoint[3]))
            {
                ToDest = 1;
                LastPos = CurrentVehicle.Position;
                Announce(Voice.in150mDest, true);
            }
            else if (ToDest <= 1 && InRange(distance, tPoint[1], tPoint[2]))
            {
                ToDest = 2;
                LastPos = CurrentVehicle.Position;
                Announce(Voice.in100mDest, true);
            }
            else if (IsRunning && InRange(distance, 0, dPoint))
            {
                AnnounceDestAtSide();
                ToDest = 4;
                IsRunning = false;
                ResetDirective();
            }
        }

        private void AnnounceDestAtSide()
        {
            int side = FindSide();
            if (side == 1)
            {
                Announce(Voice.DestRight);
            }
            else if (side == -1)
            {
                Announce(Voice.DestLeft);
            }
            else
            {
                Announce(Voice.Dest);
            }
        }

        private void AnnounceTurnL(float distance)
        {
            TurningR = 0;
            if (distance > LastTurnDistance) TurningL = 0;

            if (TurningL <= 0 && InRange(distance, tPoint[3], tPoint[4]))
            {
                TurningL = 1;
                Announce(Voice.In200mTurnL);
            }
            else if (TurningL <= 1 && InRange(distance, tPoint[2], tPoint[3]))
            {
                TurningL = 2;
                Announce(Voice.In150mTurnL);
            }
            else if (TurningL <= 2 && InRange(distance, tPoint[1], tPoint[2]))
            {
                TurningL = 3;
                Announce(Voice.In100mTurnL);
            }
            else if (TurningL <= 3 && InRange(distance, tPoint[0], tPoint[1]))
            {                
                TurningL = 4;
                Announce(Voice.NowTurnL);
            }
            LastTurnDistance = distance;
        }

        private void AnnounceTurnR(float distance)
        {
            TurningL = 0;
            if (distance > LastTurnDistance) TurningR = 0;
            if (TurningR <= 0 && InRange(distance, tPoint[3], tPoint[4]))
            {
                TurningR = 1;
                Announce(Voice.In200mTurnR);
            }
            else if (TurningR <= 1 && InRange(distance, tPoint[2], tPoint[3]))
            {
                TurningR = 2;
                Announce(Voice.In150mTurnR);
            }
            else if (TurningR <= 2 && InRange(distance, tPoint[1], tPoint[2]))
            {
                TurningR = 3;
                Announce(Voice.In100mTurnR);
            }
            else if (TurningR <= 3 && InRange(distance, tPoint[0], tPoint[1]))
            {
                TurningR = 4;
                Announce(Voice.NowTurnR);
            }
            LastTurnDistance = distance;
        }

        private void AnnounceExitRight(float distance)
        {
            TurningL = 0;
            if (TurningR <= 0 && InRange(distance, tPoint[3], tPoint[4]))
            {
                TurningR = 1;
                Announce(Voice.In200mExitR);
            }
            else if (TurningR <= 1 && InRange(distance, tPoint[2], tPoint[3]))
            {
                TurningR = 2;
                Announce(Voice.In150mExitR);
            }
            else if (TurningR <= 3 && InRange(distance, tPoint[0], tPoint[2]))
            {
                TurningR = 4;
                Announce(Voice.ExitR);
            }
        }

        private void AnnounceExitLeft(float distance)
        {
            TurningR = 0;
            if (TurningL <= 0 && InRange(distance, tPoint[3], tPoint[4]))
            {
                TurningL = 1;
                Announce(Voice.In200mExitL);
            }
            else if (TurningL <= 1 && InRange(distance, tPoint[2], tPoint[3]))
            {
                TurningL = 2;
                Announce(Voice.In150mExitL);
            }
            else if (TurningL <= 3 && InRange(distance, tPoint[0], tPoint[2]))
            {
                TurningL = 4;
                Announce(Voice.ExitL);
            }
        }

        private void AnnounceInversion(float distance)
        {
            TurningR = 0;
            if (TurningL <= 0 && InRange(distance, tPoint[3], tPoint[4]))
            {
                TurningL = 1;
                Announce(Voice.In200mInversion);
            }
            if (TurningL <= 1 && InRange(distance, tPoint[2], tPoint[3]))
            {
                TurningL = 2;
                Announce(Voice.In150mInversion);
            }
            else if (TurningL <= 3 && InRange(distance, tPoint[0], tPoint[2]))
            {
                TurningL = 4;
                Announce(Voice.NowInversion);
            }
        }

        public static Tuple<string, float, float> GenerateDirectionsToCoord(Vector3 dest)
        {
            OutputArgument outputArgument = new OutputArgument();
            OutputArgument outputArgument2 = new OutputArgument();
            OutputArgument outputArgument3 = new OutputArgument();
            GTA.Native.Function.Call(Hash.GENERATE_DIRECTIONS_TO_COORD, new InputArgument[]
            {
                dest.X,
                dest.Y,
                dest.Z,
                true,
                outputArgument,
                outputArgument2,
                outputArgument3
            });
            string text = Convert.ToString(outputArgument.GetResult<float>());
            return new Tuple<string, float, float>(text.Substring(0, 1), outputArgument2.GetResult<float>(), outputArgument3.GetResult<float>());
        }

        private static float CalculateTravelDistance(Vehicle v, Vector3 pos)
        {

            Entity[] destEntity = World.GetNearbyEntities(pos, 100);
            if (destEntity != null)
            {
                InputArgument[] args = new InputArgument[] { v, destEntity[0] };
                OutputArgument out1 = new OutputArgument();

                GTA.Native.Function.Call(GTA.Native.Hash.CALCULATE_TRAVEL_DISTANCE_BETWEEN_POINTS, args);
                return Convert.ToSingle(out1.GetResult<float>());
            }
            else return -1;
        }

        private int getTraffic(Vector3 pos)
        {
            Vehicle[] traffic = World.GetNearbyVehicles(pos, 200);
            return traffic.Length;
        }

        private static void LoadAllPathNodes(bool load)
        {
            GTA.Native.InputArgument[] args = { load };
            GTA.Native.Function.Call(GTA.Native.Hash.LOAD_ALL_PATH_NODES, args);
        }
    }
}
