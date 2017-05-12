using System;
using System.Collections.Generic;
using GameReaderCommon;
using SimHub.Plugins;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace GTAVSimhub.Plugin
{

    class Property {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Object Value { get; set; }
    }


    [PluginName("GTA V Reader")]
    class GTAVReader : IPlugin, IGameManager, IDataPlugin
    {
        DataConsumer dataConsumer;

        public GTAVReader() {
            // Init the shared memory buffer
            dataConsumer = new DataConsumer("GTAVSimHubPlugin");            
        }

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

        /// hold vehicle status
        private Boolean LightsOn = false;
        //private Boolean HandBrakeOn = false;
        private Double RPM;
        private Int32 Speed;
        private Int32 Gear;
        private int PlayerHealth;
        private int VehicleHealth;
        private bool OnFire;
        private bool SirenActive;
        private bool PlayerIsDead;
        private string VehicleName = "--";

        const string P_CURRENTGEAR = "GameData.NewData.Gear";
        const string P_SPEED = "GameData.NewData.SpeedKmh";
        const string P_RPMS = "GameData.NewData.Rpms";
        const string P_GAMEISRUNNING = "GameIsRunning";
        Int32 GameIsRunning = 0;

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// Called after plugins startup
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            // Init properties
            pluginManager.AddProperty(P_SPEED, this.GetType(), this.Speed.GetType());
            pluginManager.AddProperty(P_RPMS, this.GetType(), this.RPM.GetType());
            pluginManager.AddProperty(P_CURRENTGEAR, this.GetType(), this.Gear.GetType());
            pluginManager.AddProperty(P_GAMEISRUNNING, this.GetType(), this.GameIsRunning.GetType());

        }

        /// <summary>
        /// called one time per game data update
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data"></param>
        public void DataUpdate(PluginManager pluginManager, GameData data)
        {
            if (data.GameRunning)
            {
                pluginManager.SetPropertyValue(P_GAMEISRUNNING, this.GetType(), 1);
            }
            else
            {
                pluginManager.SetPropertyValue(P_GAMEISRUNNING, this.GetType(), 0);
            }
                string[] rawData = (string []) dataConsumer.GetSharedData();

                if (rawData != null)
                {
                    foreach (var s in rawData)
                    {
                        var property = getProperty(s);

                        pluginManager.SetPropertyValue(property.Name, this.GetType(), property.Value);
                    }
                }
            
        }

        /// <summary>
        /// Called at plugin manager stop, close/displose anything needed here !
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            dataConsumer.Dispose();
        }

        /// <summary>
        /// Return you winform settings control here, return null if no settings control
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public System.Windows.Forms.Control GetSettingsControl(PluginManager pluginManager)
        {
            return new UserControl();
        }

        

        
        public event NewLapDelegate NewLap;
        public event SessionRestartDelegate SessionRestart;
        public event TrackChangedDelegate TrackChanged;
        public event GameRunningChangedDelegate GameStateChanged;
        public event CarChangedDelegate CarChanged;
        public event DataUpdatedDelegate DataUpdated;

        public bool Enabled { get; set; }

        public GameData GameData { get; set; }
       

        public string GameName()
        {
            return "GTA V";
        }

        

        public GameCar GetCar(string carCode)
        {
            return new GameCar { Name = "Car", Picture = null };
        }

        public IEnumerable<string> GetProcesseNames()
        {
            String[] processNames = { "GTA5", "GTA5.exe", "GTA 5", "GTA 5.exe"};
            return processNames;
        }

        public object GetRawDataSample()
        {
            return "RAWDATASAMPLE";
        }

        public DataRecordBase GetReferenceMapRecord()
        {
            return null;
        }

        public GameTrack GetTrack(string trackCode)
        {
            return null;
        }

 
        public ReplayModes ReplayMode { get; set; }

        
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

        double _updateInterval = 15;
        public double UpdateInterval { get { return _updateInterval; } set { _updateInterval = UpdateInterval; } }
    }
}
