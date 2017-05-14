using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace GTA5Reader
{
    public class TelemetryReader : IDisposable
    {
        private DataConsumer dataConsumer;

        private static string[] ProcessesName = { "GTA5", "GTAV", "chrome" }; //TODO: load from configurationManager, splitting on ';'
        public bool GameIsRunning { get; set; }
        
        private Thread telemCaptureThread;
        private bool disposedValue;
        public bool IsRunning
        {
            get;
            private set;
        }

        private TelemetryPacket latestData;
        public TelemetryPacket LatestData
        {
            get
            {
                return this.latestData;
            }
            set
            {
                this.latestData = value;
            }
        }

        public TelemetryReader()
        {
            try
            {
                Logging.Current.Debug("GTA5 telemetry reader is been building");
                dataConsumer = new DataConsumer("GTAVSimHubPlugin");
                this.StartListening();
                Application.ApplicationExit += new EventHandler(this.Application_ApplicationExit);
            }
            catch
            {
            }
        }

        public string GetGear(int gear)
        {
            if (this.GameIsRunning)
            {
                if (gear == 0)
                {
                    return "R";
                }
                if (gear == 1)
                {
                    return "N";
                }
                return (gear - 1).ToString();
            }
            else
            {
                if (gear == 0)
                {
                    return "N";
                }
                if (gear == -1 || gear == 10)
                {
                    return "R";
                }
                return gear.ToString();
            }
        }

        private void StartListening()
        {
            if (this.telemCaptureThread == null)
            {
                this.telemCaptureThread = new Thread(new ThreadStart(this.FetchData));
                this.telemCaptureThread.IsBackground = true;
                this.telemCaptureThread.Start();
            }
        }


        /* Infinite loop that executes in a thread reading data from game */
        private void FetchData()
        {
            while (true)
            {
                try
                {
                    byte[] bytes = dataConsumer.GetSharedData();
                    this.LatestData = PacketUtilities.ConvertToPacket(bytes);
                    if (!this.IsRunning)
                    {
                        this.GameIsRunning = false;
                        Process[] processes = Process.GetProcesses();
                        string[] names = TelemetryReader.ProcessesName;

                        foreach (Process p in processes)
                        {
                            foreach (string name in names)
                            {
                                if (p.ProcessName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    this.GameIsRunning = true;
                                    break;
                                }
                            }
                            if (this.GameIsRunning) break;
                        }
                        this.IsRunning = true;
                    }
                }
                catch (Exception)
                {
                    this.IsRunning = false;
                }
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            this.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing && this.telemCaptureThread != null)
                {
                    this.telemCaptureThread.Abort();
                    this.telemCaptureThread = null;
                }
                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }

}
