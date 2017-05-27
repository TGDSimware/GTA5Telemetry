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
        float _DistanceAtLastHint = -1;

        Vector3 _CurrentPos;
        Vector3 _LastPos;
        float _DPoint = 20;
        float _Delta;
        private Int32[] _Next = new Int32[10];

        Vehicle CurrentVehicle = null;

        private bool _DEBUG = false;
        private bool _TextNotes = false;
        private float _VolumeFactor = 1;
        private AudioManager _AudioManager;

        public Vector3 Destination { get; private set; }
        public bool DestinationReached { get; private set; }
        public bool Running { get; private set; } = false;

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
                    "Turn,Go straight,Follow,Inversion,Exit,Wrong Direction,Arrived at destination");
                NavText.Hint = hints.Split(',');

                string directions = scriptSettings.GetValue<string>("LANGUAGE", "DIRECTIONS", "-,Right,Left");
                NavText.Dir = directions.Split(',');

                NavText.Unit = scriptSettings.GetValue<string>("LANGUAGE", "UNIT", "Km");
                NavText.MessageOn = scriptSettings.GetValue<string>("LANGUAGE", "MESSAGEON", "Navigator active. Extimated distance");

                int updateInterval = scriptSettings.GetValue<int>("ENGINE", "UPDATEINTERVAL", 100);
                this.Interval = updateInterval;

                _DPoint = scriptSettings.GetValue<int>("ENGINE", "DPOINT", 20);

                _VolumeFactor = scriptSettings.GetValue<int>("ENGINE", "VOLUMEFACTOR", 100) / 100f;

                _TextNotes = scriptSettings.GetValue<bool>("UI", "TEXTNOTES", false);

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
                            this.AtPosition(vehicle.Position);
                        }
                    }
                    else if (Running)
                    {
                        // Arrived at destination
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
                ResetHint(0);

                // Preloads All Audio
                _AudioManager.VolumeFactor = _VolumeFactor;
                _AudioManager.Preload(NavVoices.Voices);

                Announce(NavVoices.Calculating);
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

        //A shortcut for single-voice phrases
        private void Announce(NavVoice voice, bool mute = false)
        {
            Announce(new NavVoice[] { voice }, mute);
        }

        private void Announce(NavVoice[] phrase, bool mute = false)
        {
            try
            {
                try
                {
                    string notify;

                    if (phrase.Length > 1)
                    {
                        notify = phrase[0].Dist + "m: " + NavText.Hint[(int)phrase[1].Hint] + " " +
                            NavText.Dir[(int)phrase[1].Dir];
                    }
                    else
                    {
                        notify = NavText.Hint[(int)phrase[0].Hint] + " " + NavText.Dir[(int)phrase[0].Dir];
                    }
                    if (_TextNotes) UI.Notify(notify);
                }
                catch (Exception problem)
                {
                    UI.Notify("Announce(): " + problem.Message);
                }

                if (!mute)
                {
                    foreach (var voice in phrase)
                    {
                        if (voice != null)
                        {
                            float duration = _AudioManager.Play(voice.Key);
                            Wait(Convert.ToInt32(duration * 1000f) + 20);
                        }
                    }
                }

            }
            catch (Exception problem)
            {
                UI.Notify("Announce(): " + problem.Message);
            }
            finally
            {
                _DistanceAtLastHint = DistanceRemaining;
            }
        }

        public void AtPosition(Vector3 pos)
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
            _Delta = Math.Abs(DistanceRemaining - _DistanceAtLastHint);
            int hint = (int)fHint;

            if (_DEBUG)
            {
                UI.ShowSubtitle("Hint " + hint + " @ " + disToNextTurn + "m delta = " + _Delta + "m");
            }

            try
            {
                if (hint != _LastHint && hint > 0) ResetHint(hint);

                switch (hint)
                {
                    case 0:
                        break;
                    case 1:
                        if (disToNextTurn == 0)
                        {
                            // No distance is known, use Next as a counter
                            SetNext(1, Next(1) + 1);
                            if (Next(1) >= 20)
                            {
                                Announce(NavVoices.WrongDirection);
                                SetNext(1, 0);
                            }
                        }
                        else
                        {
                            SetNext(1, 0);
                            DriveTo(hint, disToNextTurn, NavVoices.Inversion);
                        }
                        break;
                    case 2:
                        if (_LastHint != 2 & _LastHint != 7) Announce(NavVoices.Keep);
                        break;
                    case 3:
                        UI.Notify("Hint 3 not yet implemented");
                        break;
                    case 4:
                        DriveTo(hint, disToNextTurn, NavVoices.TurnL);
                        break;
                    case 5:
                        DriveTo(hint, disToNextTurn, NavVoices.TurnR);
                        break;
                    case 6:
                        UI.Notify("Hint 6 not yet implemented");
                        break;
                    case 7:
                        DriveTo(hint, disToNextTurn, NavVoices.KeepR);
                        break;
                    case 8:
                        if (FindHeading() < 0 && disToNextTurn <= 150)
                            DriveTo(hint, disToNextTurn, NavVoices.ExitL);
                        else
                            DriveTo(hint, disToNextTurn, NavVoices.KeepL);
                        break;
                    case 9:
                        DriveTo(hint, disToNextTurn, NavVoices.ExitR);
                        break;
                }

                if (DistanceRemaining < 500)
                {
                    // Begin driving to destination
                    DriveToDest(DistanceRemaining);
                }

               
                _LastHint = hint;
            }
            catch (Exception problem)
            {
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

        private void DriveToDest(float dist)
        {
            // Start a DriveTo in non-exclusive mode, text mode
            DriveTo(hint: 0, distance: dist, directive: NavVoices.Dest, exclusive: false, asText: true);

            if (!DestinationReached && InRange(dist, 0, _DPoint))
            {
                // Arrived at destination
                AnnounceDestAtSide();
                SetNext(0, -100);
                DestinationReached = true;
                ResetHint(0);
            }
        }

        private void AnnounceDestAtSide()
        {
            int side = FindSide();
            if (side == 1)
            {
                Announce(NavVoices.DestRight);
            }
            else if (side == -1)
            {
                Announce(NavVoices.DestLeft);
            }
            else
            {
                Announce(NavVoices.Dest);
            }
        }

        private void StartHint(int op)
        {
            for (int i = 0; i < _Next.Length; i++)
            {
                if (i != op) _Next[i] = -1;
            }
        }

        private void ResetHint(int op)
        {
            _Next[op] = -1;
        }

        private int Next(int op)
        {
            return _Next[op];
        }

        private void SetNext(int op, int dist)
        {
            _Next[op] = dist;
        }

        private int getNextPoint(int distance)
        {
            if (distance > 450) return (distance - distance % 100) - 200;
            if (distance > 150) return distance - distance % 50 - 100;
            if (distance <= 100) return -100;
            if (distance <= 150) return 20;
            return -1;
        }

        private void DriveTo(int hint, float distance, NavVoice directive, bool exclusive = true, bool asText = false)
        {
            if (hint != _LastHint && exclusive) StartHint(hint);     // Reset any other hint in progress but 'hint'            

            int d = Convert.ToInt32(distance);

            Int32 nextPoint = Next(hint);
            if (nextPoint == -1) nextPoint = d;
            if (nextPoint == -100) return;       // avoid looping

            //TODO: the range around nextpoint should be relative to the current speed               
            if (Math.Abs(d - nextPoint) <= 5)
            {
                int dTo50 = d - d % 50;
                int voiceIndex = dTo50 < 50 ? 0 : dTo50 / 50 - 1;
                if (voiceIndex >= NavVoices.Distances.Length) asText = true;    // No voice available for that distance

                if (asText)
                {
                    AnnounceText(d, NavText.Hint[(int)directive.Hint], NavText.Dir[(int)directive.Dir]);
                }
                else
                {
                    if (voiceIndex > 0 && voiceIndex < NavVoices.Distances.Length)
                    {
                        // "Intermediate" directive
                        Announce(new NavVoice[] { NavVoices.Distances[voiceIndex], directive });
                    }
                    else
                    {
                        // "Final" directive
                        // Sequences of final directive must be introduced by a "then"
                        // At this point if we had a lookahead directive
                        // we could Announce() something like:
                        // { directive, NavVoices.AndThen, lookaheadDirective }
                        
                        if (_LastHint >= 4 && _Delta <= 30)
                        {
                            Announce(new NavVoice[] { NavVoices.Then, directive });
                        }
                        else
                        {
                            Announce(directive);
                        }
                    }
                }

                // Set the next announce point, which will be the next multiple of 100
                // after at least other 100
                // E.g. 350 -> 200 -> 100
                // Or 450 -> 300 -> 200 - 100
                // I think that's better than going from 100 to 100
                // like 450 -> 350 -> 250 -> 150 -> 50
                if (_DEBUG) UI.Notify("Next announce at " + getNextPoint(d));
                SetNext(hint, getNextPoint(d));
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

        private static float CalculateTravelDistance(Vector3 pos1, Vector3 pos2)
        {
            InputArgument[] args = new InputArgument[] { pos1.GetHashCode(), pos2.GetHashCode() };
            OutputArgument out1 = new OutputArgument();

            //
            // v_3 = PATHFIND::CALCULATE_TRAVEL_DISTANCE_BETWEEN_POINTS(
            //  ENTITY::GET_ENTITY_COORDS(PLAYER::PLAYER_PED_ID(), 1), g_186AE);
            // https://github.com/brendan-rius/gta-v-decompiled-scripts/blob/master/taxiservice.c4
            //
            GTA.Native.Function.Call(GTA.Native.Hash.CALCULATE_TRAVEL_DISTANCE_BETWEEN_POINTS, args);
            return Convert.ToSingle(out1.GetResult<float>());
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
