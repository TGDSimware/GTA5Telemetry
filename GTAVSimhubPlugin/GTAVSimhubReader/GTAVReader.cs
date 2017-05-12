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

        public Property(string n, Type t, Object v) { Name = n; Type = t; Value = v; }
    }

    [PluginName("GTA V Reader")]
    class GTAVReader : IPlugin, IDataPlugin
    {
        private string DEBUG = "";
        //Private properties: end

        Property[] Properties = {
            new Property("Gear", typeof(String), "") ,
            new Property("GearNumber", typeof(Int32), 0) ,
            new Property("SpeedKmh", typeof(Double), 0d ),
            new Property("Health", typeof(Int32), 0d ),
            new Property("Rpms", typeof(Double), 0d ),
            new Property("GameIsRunning", typeof(Int32), 0 ),
            new Property("InVehicle", typeof(Int32), 0 ),
            new Property("OnFire", typeof(Int32), 0 ),
            new Property("Name", typeof(String), "") ,
            new Property("VehicleName", typeof(String), "") ,
            new Property("Weapon", typeof(String), "")
        };

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
        }

        private void debug(string message)
        {
            DEBUG += System.DateTime.Now.ToString() + ": " + message + "\n";
            PluginManager.SetPropertyValue("DEBUG", this.GetType(), DEBUG);
        }

        /// <summary>
        /// Called after plugins startup. Required by the IPlugin interface
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            // Init properties
            foreach (var p in Properties)
            {
                pluginManager.AddProperty(p.Name, this.GetType(), p.Type);
            }            

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
