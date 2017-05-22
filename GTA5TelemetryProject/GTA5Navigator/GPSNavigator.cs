/// <summary>
/// GTA V Navigator Mod
///  
/// If this code works, it has been written by Carlo Iovino (carlo.iovino@outlook.com)
/// The Green Dragon Youtube Channel (www.youtube.com/carloxofficial)
/// 
/// </summary>
using System;
using GTA;
using GTA.Math;
using GTA.Native;

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
        float _LastHint = -1;
        float _LastHintDistance = -1;
        bool DestinationReached;
        bool Running = false;
        Vector3 _CurrentPos;
        Vector3 _LastPos;
        float dPoint = 20;
        private int[] _Progress = new int[10];

        float LastTurnDistance = 0;
        Vehicle CurrentVehicle = null;
        private Vector3 Destination;

        private int[] tPoint = { 5, 50, 110, 160, 210 }; //Turning points
        private bool _DEBUG = false;
        private bool TextNotes = false;
        private int AnnounceIdleTime = 2500;
        private float VolumeFactor = 1;
        private AudioManager _AudioManager;

        public GPSNavigator()
        {
            try
            {
                LoadSettings();
                
                Destination = new Vector3();
                _AudioManager = new AudioManager();
                _AudioManager.BasePath = @"scripts\GTA5Navigator\gpsvoices";
                
                Tick += OnTick; // Add OnTick() as an event handler for the Tick event            
            }
            catch (Exception problem)
            {
                UI.Notify("Cannot Instantiate GPS Navigator: " + problem.Message);
            }
        }

        private void LoadSettings()
        {
            try
            {
                ScriptSettings scriptSettings = ScriptSettings.Load(@"scripts\GTA5Navigator\Settings.ini");

                // Load settings                               
                string hints = scriptSettings.GetValue<string>("LANGUAGE", "HINTS",
                    "Girare,Proseguire dritto,Seguire la strada,Inversione,Uscita,Direzione Errata,Ricalcolo,Arrivo a destinazione");
                NavText.Hint = hints.Split(',');

                string directions = scriptSettings.GetValue<string>("LANGUAGE", "DIRECTIONS", "-,Right,Left");
                NavText.Dir = directions.Split(',');

                NavText.Unit = scriptSettings.GetValue<string>("LANGUAGE", "UNIT", "Km");
                NavText.MessageOn = scriptSettings.GetValue<string>("LANGUAGE", "MESSAGEON", "Navigatore attivo. Distanza stimata");

                string points = scriptSettings.GetValue<string>("ENGINE", "TURNPOINTS", "5,40,105,155,210");
                string[] sPoints = points.Split(',');
                for (int i = 0; i < sPoints.Length; i++) tPoint[i] = Convert.ToInt32(sPoints[i]);

                int updateInterval = scriptSettings.GetValue<int>("ENGINE", "UPDATEINTERVAL", 100);
                this.Interval = updateInterval;

                this.dPoint = scriptSettings.GetValue<int>("ENGINE", "DPOINT", 20);
                AnnounceIdleTime = scriptSettings.GetValue<int>("ENGINE", "ANNOUNCEIDLETIME", 2500);

                this.VolumeFactor = scriptSettings.GetValue<int>("ENGINE", "VOLUMEFACTOR", 100) / 100f;

                TextNotes = scriptSettings.GetValue<bool>("UI", "TEXTNOTES", false);

                _DEBUG = scriptSettings.GetValue<bool>("ENGINE", "DEBUG", false);
                if (_DEBUG)
                {
                    UI.Notify("LoadSettings(): Settings loaded");
                }
            }
            catch (Exception e)
            {
                UI.Notify("LoadSettings(): " + e.Message);
                _DEBUG = true;
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
                            this.StartNavigation(vehicle, World.GetWaypointPosition());
                            Running = true;
                        }
                        else
                        {
                            this.setPosition(vehicle.Position);
                        }
                    }
                    else if (Running)
                    {
                        AnnounceDestAtSide();
                        Running = false;
                    }
                }
            }
            catch (Exception problem)
            {
                UI.Notify("OnTick: " + problem.Message);
            }
        }

        private void StartNavigation(Vehicle v, Vector3 destination)
        {
            try
            {
                LoadSettings();             
                CurrentVehicle = v;
                _LastPos = v.Position;
                Destination = destination;
                DestinationReached = true;
                ResetDriving(0);

                // Preloads All Audio
                _AudioManager.VolumeFactor = VolumeFactor;
                _AudioManager.Preload(NavVoice.Voices);

                Announce(NavVoice.Calculating);
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
                return _CurrentPos.DistanceTo2D(Destination);
            }
        }

        private void AnnounceText(float dist, string hint, string dir)
        {
            string notify = Math.Round(dist) + "m: " + hint + " " + dir;
            UI.Notify(notify);
        }

        private void Announce(NavSound voice, bool mute = false)
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
                    if (TextNotes) UI.Notify(notify);
                }
                catch (Exception problem)
                {
                    UI.Notify("Announce(): " + problem.Message);
                }

                if (!mute)
                {
                    float duration = _AudioManager.Play(voice.Key);
                    Wait(Convert.ToInt32(duration * 1000f));
                }

            }
            catch (Exception problem)
            {
                UI.Notify("Announce(): " + problem.Message);
            }
        }

        public void setPosition(Vector3 pos)
        {
            _CurrentPos = pos;
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

        public void Process(float fHint, float disToNextTurn, float disToWaypoint)
        {
            float distance = DistanceRemaining;
            float delta = Math.Abs(distance - _LastHintDistance);
            int hint = (int)fHint;

            if (_DEBUG)
            {
                UI.ShowSubtitle("Hint " + hint + " @ " + disToNextTurn + "m delta = " + delta + "m");
            }

            try
            {
                if (hint != _LastHint && hint > 0) ResetDriving(hint);

                switch (hint)
                {
                    case 0:
                        break;
                    case 1:
                        if (disToNextTurn == 0)
                        {
                            SetProgress(1, Progress(1) + 1);
                            if (Progress(1) >= 25)
                            {
                                Announce(NavVoice.WrongDirection);
                                SetProgress(1, 0);
                            }
                        }
                        else
                        {
                            SetProgress(1, 0);
                            AnnounceInversion(hint, disToNextTurn);
                        }
                        break;
                    case 2:
                        if (_LastHint != 2) Announce(NavVoice.Keep);
                        break;
                    case 3:
                        // Never!
                        break;
                    case 4:
                        AnnounceTurnL(hint, disToNextTurn);
                        break;
                    case 5:
                        AnnounceTurnR(hint, disToNextTurn);
                        break;
                    case 6:
                        // Never!
                        break;
                    case 7:
                        if (_LastHint != 7 && _LastHint != 2) Announce(NavVoice.Follow);
                        break;
                    case 8:
                        /*if (FindHeading() < -.8)
                            AnnounceInversion(disToNextTurn);
                        else*/
                        AnnounceExitLeft(hint, disToNextTurn);
                        break;
                    case 9:
                        AnnounceExitRight(hint, disToNextTurn);
                        break;
                }

                if (distance < tPoint[3])
                {
                    AnnounceDest(distance);
                }

                _LastHintDistance = DistanceRemaining;
                _LastHint = hint;
            }
            catch (Exception problem) {
                throw new Exception("Process(): " + problem.Message + " when hint=" + hint + ", distance=" + disToNextTurn);

            }
        }

        private int FindSide()
        {
            int side = 0;
            Vector3 Pos = CurrentVehicle.Position;
            Vector2 DCAR = new Vector2(Pos.X - _LastPos.X, Pos.Y - _LastPos.Y);
            Vector2 DDST = new Vector2(Destination.X - _LastPos.X, Destination.Y - _LastPos.Y);
            float HCAR = DCAR.ToHeading();
            float HDST = DDST.ToHeading();
            side = HCAR > HDST ? 1 : -1;    // 1 = right side, -1, left side
            return side;
        }

        private float FindHeading()
        {
            Vector3 Pos = CurrentVehicle.Position;
            Vector2 DCAR = new Vector2(Pos.X - _LastPos.X, Pos.Y - _LastPos.Y);
            Vector2 DDST = new Vector2(Destination.X - _LastPos.X, Destination.Y - _LastPos.Y);
            float HCAR = DCAR.ToHeading();
            float HDST = DDST.ToHeading();
            return Convert.ToSingle(Math.Cos(Math.Abs(HCAR - HDST))); // 1 = right, 0 normal, -1 opposite
        }

        private void AnnounceDest(float distance)
        {
            if (Progress(0) <= 0 && distance > tPoint[3])
            {
                AnnounceText(distance, NavText.Hint[(int)NavVoice.Dest.Hint], NavText.Dir[(int)NavVoice.Dest.Dir]);
            }
            else if (Progress(0) <= 0 && InRange(distance, tPoint[2], tPoint[3]))
            {
                SetProgress(0, 1);
                _LastPos = CurrentVehicle.Position;
                Announce(NavVoice.In150mDest, true);
            }
            else if (Progress(0) <= 1 && InRange(distance, tPoint[1], tPoint[2]))
            {
                SetProgress(0, 2);
                _LastPos = CurrentVehicle.Position;
                Announce(NavVoice.In100mDest, true);
            }
            else if (!DestinationReached && InRange(distance, 0, dPoint))
            {
                AnnounceDestAtSide();
                SetProgress(0, 4);
                DestinationReached = true;
                ResetDriving(0);
            }
        }

        private void AnnounceDestAtSide()
        {
            int side = FindSide();
            if (side == 1)
            {
                Announce(NavVoice.DestRight);
            }
            else if (side == -1)
            {
                Announce(NavVoice.DestLeft);
            }
            else
            {
                Announce(NavVoice.Dest);
            }
        }

        private void BeginDriving(int op)
        {
            for(int i=0; i< _Progress.Length; i++)
            {
                if (i != op) _Progress[i] = 0;
            }
        }

        private void ResetDriving(int op)
        {
            _Progress[op] = 0;
        }

        private int Progress(int op)
        {
            return _Progress[op];
        }

        private void SetProgress(int op, int state)
        {
            _Progress[op] = state;
        }

        private void DriveTo(int hint, float distance, NavSound[] navVoice)
        {
            BeginDriving(hint);

            if (distance > LastTurnDistance)
            {
                // We are going back
                if (InRange(distance,tPoint[0],tPoint[1]))
                {
                    SetProgress(hint, 3);
                }
                else if (InRange(distance, tPoint[1], tPoint[2]))
                {
                    SetProgress(hint, 2);
                }
                else if (InRange(distance, tPoint[2], tPoint[3]))
                {
                    SetProgress(hint, 1);
                }
                else if (InRange(distance, tPoint[3], tPoint[4]))
                {
                    SetProgress(hint, 0);
                }
            }

            if (Progress(hint) <= 0 && distance > tPoint[4])
            {
                AnnounceText(distance, NavText.Hint[(int)navVoice[0].Hint], NavText.Dir[(int)navVoice[0].Dir]);
            }
            else if (Progress(hint) <= 0 && InRange(distance, tPoint[3], tPoint[4]))
            {
                SetProgress(hint, 1);
                Announce(navVoice[1]);
            }
            else if (Progress(hint) <= 1 && InRange(distance, tPoint[2], tPoint[3]))
            {
                SetProgress(hint, 2);
                Announce(navVoice[2]);
            }
            else if (Progress(hint) <= 2 && InRange(distance, tPoint[1], tPoint[2]))
            {
                SetProgress(hint, 3);
                Announce(navVoice[3]);
            }
            else if (Progress(hint) <= 3 && InRange(distance, tPoint[0], tPoint[1]))
            {
                SetProgress(hint, 4);
                Announce(navVoice[4]);
            }
            LastTurnDistance = distance;
        }

        private void AnnounceTurnL(int hint, float distance)
        {
            DriveTo(hint, distance, 
                new[]{ NavVoice.NowTurnL, NavVoice.In200mTurnL, NavVoice.In150mTurnL, NavVoice.In100mTurnL, NavVoice.NowTurnL });
        }

        private void AnnounceTurnR(int hint, float distance)
        {
            DriveTo(hint, distance,
                new[] { NavVoice.NowTurnR, NavVoice.In200mTurnR, NavVoice.In150mTurnR, NavVoice.In100mTurnR, NavVoice.NowTurnR });
        }

        private void AnnounceExitRight(int hint, float distance)
        {
            DriveTo(hint, distance,
                new[] { NavVoice.ExitR, NavVoice.In200mExitR, NavVoice.In150mExitR, NavVoice.In100mExitR, NavVoice.ExitR });
        }

        private void AnnounceExitLeft(int hint, float distance)
        {
            DriveTo(hint, distance,
                new[] { NavVoice.ExitL, NavVoice.In200mExitL, NavVoice.In150mExitL, NavVoice.In100mExitL, NavVoice.ExitR });
        }

        private void AnnounceInversion(int hint, float distance)
        {
            DriveTo(hint, distance,
                new[] { NavVoice.NowInversion, NavVoice.In200mInversion, NavVoice.In150mInversion, NavVoice.In100mInversion, NavVoice.NowInversion });
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
