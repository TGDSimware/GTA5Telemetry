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


    [PluginName("GTAV Simhub plugin")]
    class GTAVPlugin : IPlugin, IDataPlugin
    {
        DataConsumer dataConsumer;

        public Property getProperty(string s)
        {

            string[] token = s.Split(':');

            var name = token[0];
            var type = token[1];
            Object value = null;

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

            return new Property { Name = name, Type = value.GetType(), Value = value };
        }

        /// hold vehicle status
        private Boolean LightsOn = false;
        //private Boolean HandBrakeOn = false;
        private int RPM;
        private int Speed;
        private int Gear;
        private int PlayerHealth;
        private int VehicleHealth;
        private bool OnFire;
        private bool SirenActive;
        private bool PlayerIsDead;
        private string VehicleName = "--";

        const string P_CURRENTGEAR = "DataCorePlugin.GameData.NewData.Gear";
        const string P_SPEED = "DataCorePlugin.GameData.NewData.SpeedKmh";
        const string P_RPMS = "DataCorePlugin.GameData.NewData.Rpms";

        /// <summary>
        /// Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        /// called one time per game data update
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data"></param>
        public void DataUpdate(PluginManager pluginManager, GameData data)
        {
            pluginManager.SetPropertyValue("CurrentDateTime", this.GetType(), DateTime.Now);

            if (data.GameRunning)
            {
                string[] rawData = (string []) dataConsumer.GetSharedData();
                foreach(var s in rawData)
                {
                    var property = getProperty(s);

                    pluginManager.SetPropertyValue(property.Name, this.GetType(), property.Value);
                }


                /*
                if (data.OldData != null && data.NewData != null)
                {
                    pluginManager.SetPropertyValue(P_SPEED, this.GetType(), this.Speed);
                    pluginManager.SetPropertyValue(P_RPMS, this.GetType(), this.RPM);
                    pluginManager.SetPropertyValue(P_CURRENTGEAR, this.GetType(), this.Gear);                     
                }*/
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

        /// <summary>
        /// Called after plugins startup
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            // Init the shared memory buffer
            dataConsumer = new DataConsumer("GTAVSimHubPlugin");

            // Init properties
            pluginManager.AddProperty(P_SPEED, this.GetType(), this.Speed.GetType());
            pluginManager.AddProperty(P_RPMS, this.GetType(), this.RPM.GetType());
            pluginManager.AddProperty(P_CURRENTGEAR, this.GetType(), this.Gear.GetType());            

        }
    }
}
