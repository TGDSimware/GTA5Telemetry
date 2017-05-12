using System;
using System.Collections.Generic;
using GameReaderCommon;
using SimHub.Plugins;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace GTAVSimhub.Plugin
{
    class Property
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Object Value { get; set; }
    }

    [PluginName("GTA V Reader")]
    class GTAVReader : IPlugin, IDataPlugin
    {
        // Private properties: begin
        private Double RPM = 0.0;
        private Double Speed = 0;
        private String Gear = "N";
        private Int32 IsRunning = 0;

        private string DEBUG = "";
        //Private properties: end

        // Simhub "game-independent" Properties names
        const string P_CURRENTGEAR = "Gear";
        const string P_SPEED = "SpeedKmh";
        const string P_RPMS = "Rpms";
        const string P_GAMEISRUNNING = "GameIsRunning";
        const string P_DEBUG = "DEBUG";

        // IPlugin required Properties
        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        // The DataConsumer used for shared memory communication
        DataConsumer dataConsumer;

        public GTAVReader()
        {
            // Init the shared memory buffer
            dataConsumer = new DataConsumer("GTAVSimHubPlugin");
            //_GameData = new GameData();
            //GameData.GameRunning = true;           
        }

        private void debug(string message)
        {
            DEBUG += System.DateTime.Now.ToString() + ": " + message + "\n";
            PluginManager.SetPropertyValue(P_DEBUG, this.GetType(), DEBUG);
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
            pluginManager.AddProperty(P_GAMEISRUNNING, this.GetType(), this.IsRunning.GetType());
            pluginManager.AddProperty(P_DEBUG, this.GetType(), this.DEBUG.GetType());

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
            return new UserControl();
        }

        /// <summary>
        /// called one time per game data update. Required by the IDataPlugin interface
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data"></param>
        public void DataUpdate(PluginManager pluginManager, GameData data)
        {
            // The plugin will work only when the active gamemanager doesn't detect his game
            if (!data.GameRunning)
            {             
                //Process[] ps = Process.GetProcessesByName("GTA5");
                //if (ps.Length > 0) {

                Object rawData = dataConsumer.GetSharedData();

                if (rawData != null)
                {
                    //setGameData(rawData, data);
                    //pluginManager.SetPropertyValue(P_GAMEISRUNNING, this.GetType(), 1);

                    foreach (var s in (string[])rawData)
                    {
                        // Set plugin properties
                        //debug(s);
                        var property = getProperty(s);
                        //debug(property.Name + " " + this.GetType() + " " + property.Value);
                        pluginManager.SetPropertyValue(property.Name, this.GetType(), property.Value);

                    }
                }
            }
            //}
        }


        public void setGameData(Object rawData, GameData data)
        {
            string[] sa = (string[])rawData;

            if (rawData != null)
            {
                foreach (var s in sa)
                {
                    var property = getProperty(s);
                    if (data.NewData != null)
                    {
                        if (property.Name.Equals(P_SPEED))
                        {
                            data.NewData.SpeedKmh = Convert.ToDouble(property.Value);
                        }
                        else if (property.Name.Equals(P_RPMS))
                        {
                            data.NewData.Rpms = Convert.ToDouble(property.Value);
                        }
                        else if (property.Name.Equals(P_CURRENTGEAR))
                        {
                            data.NewData.Gear = Convert.ToString(property.Value);
                        }
                    }
                }
            }
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
                        value = Convert.ToDouble(token[2].Replace(',','.'));
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
