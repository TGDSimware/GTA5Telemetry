using System;
using System.Collections.Generic;
using GameReaderCommon;
using SimHub.Plugins;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace GTAVSimhub.Plugin
{
    class Property
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Object Value { get; set; }
    }

    [PluginName("GTA V Reader")]
    class GTAVReader : IPlugin, IGameManager, IDataPlugin
    {
        // IDGameManager delegates: begin
        public event NewLapDelegate NewLap;
        public event SessionRestartDelegate SessionRestart;
        public event TrackChangedDelegate TrackChanged;
        public event GameRunningChangedDelegate GameStateChanged;
        public event CarChangedDelegate CarChanged;
        public event DataUpdatedDelegate DataUpdated;
        // IDGameManager delegates: end

        // Private properties: begin
        private Double RPM = 0.0;
        private Int32 Speed = 0;
        private Int32 Gear = 0;
        private Int32 PlayerHealth = 0;
        private Int32 GameIsRunning = 0;
        private double _updateInterval = 15;
        private string DEBUG = "OK";
        //Private properties: end

        // Simhub "game-independent" Properties names
        const string P_CURRENTGEAR = "GameData.NewData.Gear";
        const string P_SPEED = "GameData.NewData.SpeedKmh";
        const string P_RPMS = "GameData.NewData.Rpms";
        const string P_GAMEISRUNNING = "GameIsRunning";
        const string P_DEBUG = "DEBUG";

        // IPlugin required Properties
        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        // IGameManager required Properties: begin
        public bool Enabled { get; set; }
        public GameData GameData { get; }
        public ReplayModes ReplayMode { get; set; }
        public double UpdateInterval { get { return _updateInterval; } set { _updateInterval = UpdateInterval; } }
        // IGameManager required Properties: end

        // The DataConsumer used for shared memory communication
        DataConsumer dataConsumer;

        public GTAVReader()
        {
            // Init the shared memory buffer
            dataConsumer = new DataConsumer("GTAVSimHubPlugin");
            GameData = new GameData();
            GameData.GameRunning = true;
        }

        private void debug(string message)
        {
            PluginManager.SetPropertyValue(P_DEBUG, this.GetType(), message);
        }

        public void OnGameStateChanged(bool running, IGameManager manager)
        {
            debug("OnGameStateChanged: " + System.DateTime.Now.ToString());

            if (running)
            {
                GameData.GameRunning = true;
                GameData.NewData.SpeedKmh = 0;
                GameData.NewData.Rpms = 0;
                GameData.NewData.Gear = "N";
            }
        }

        /// <summary>
        /// Called after plugins startup. Required by the IPlugin interface
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            // Init properties
            pluginManager.AddProperty(P_SPEED, this.GetType(), this.Speed.GetType());
            pluginManager.AddProperty(P_RPMS, this.GetType(), this.RPM.GetType());
            pluginManager.AddProperty(P_CURRENTGEAR, this.GetType(), this.Gear.GetType());
            pluginManager.AddProperty(P_GAMEISRUNNING, this.GetType(), this.GameIsRunning.GetType());
            pluginManager.AddProperty(P_DEBUG, this.GetType(), this.DEBUG.GetType());

            GameStateChanged += OnGameStateChanged; //Add an handler for game state changed
            Enabled = true;
            ReplayMode = ReplayModes.Live;

            // Here GameData == null
            // Here PluginManager == pluginManager          
        }

        /// <summary>
        /// Called at plugin manager stop, close/displose anything needed here !
        /// Required by the IPlugin interface
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            pluginManager.ClearProperties(this.GetType());
            dataConsumer.Dispose();
        }

        /// <summary>
        /// Return you winform settings control here, return null if no settings control
        /// Required from the IPlugin interface
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Forms.Control GetSettingsControl(PluginManager pluginManager)
        {
            return null;
        }

        /// <summary>
        /// called one time per game data update. Required by the IDataPlugin interface
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data"></param>
        public void DataUpdate(PluginManager pluginManager, GameData data)
        {
            if (data.GameRunning)
            {
                pluginManager.SetPropertyValue(P_GAMEISRUNNING, this.GetType(), 1);
                GameData.GameRunning = true;
            }
            else
            {
                pluginManager.SetPropertyValue(P_GAMEISRUNNING, this.GetType(), 0);
                GameData.GameRunning = false;
            }

            string[] rawData = (string[])dataConsumer.GetSharedData();

            if (rawData != null)
            {
                foreach (var s in rawData)
                {
                    var property = getProperty(s);
                    pluginManager.SetPropertyValue(property.Name, this.GetType(), property.Value);
                }
            }
        }

        // IGameManager required methods: begin
        public string GameName()
        {
            return "GTA5";
        }

        public GameCar GetCar(string carCode)
        {
            return new GameCar { Name = "Car", Picture = null };
        }

        public IEnumerable<string> GetProcesseNames()
        {
            String[] processNames = { "GTA5", "GTA5.exe", "GTA 5", "GTA 5.exe", "chrome.exe", "chrome" };
            return processNames;
        }

        public object GetRawDataSample()
        {
            debug("GETRAWDATASAMPLE: " +
               System.DateTime.Now.ToString());

            string[] rawData = (string[])dataConsumer.GetSharedData();

            if (rawData != null)
            {
                foreach (var s in rawData)
                {
                    var property = getProperty(s);

                    if (property.Name.Equals(P_SPEED))
                    {
                        GameData.NewData.SpeedKmh = Convert.ToDouble(property.Value);
                    }
                    else if (property.Name.Equals(P_RPMS))
                    {
                        GameData.NewData.Rpms = Convert.ToDouble(property.Value);
                    }
                    else if (property.Name.Equals(P_CURRENTGEAR))
                    {
                        GameData.NewData.Gear = Convert.ToString(property.Value);
                    }
                }
            }

            return rawData;
        }

        public DataRecordBase GetReferenceMapRecord()
        {
            return null;
        }

        public GameTrack GetTrack(string trackCode)
        {
            return null;
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public double[] ToAbsoluteCoordinates(double[] source)
        {
            return source;
        }
        // IGameManager required methods: end

        public Property getProperty(string s)
        {
            string[] token = s.Split(':');

            string name = "Unknown";
            Object value = 0;

            if (token != null)
            {
                if (token.Length == 3)
                {
                    name = token[0];
                    var type = token[1];

                    if (type.Equals("String"))
                    {
                        value = token[2];
                    }
                    else if (type.Equals("Double"))
                    {
                        value = Convert.ToDouble(token[2]);
                    }
                    else if (type.Equals("Int32"))
                    {
                        value = Convert.ToInt32(token[2]);
                    }
                }
            }
            return new Property { Name = name, Type = value.GetType(), Value = value };
        }
    }
}
