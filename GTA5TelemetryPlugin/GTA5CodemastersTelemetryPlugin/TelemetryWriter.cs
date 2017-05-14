using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/// <summary>
/// GTA V Codemasters Telemetru Plugin
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
    sealed class TelemetryWriter : IDisposable
    {
        private IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, 0);
        private UdpClient udpClient;

        // Class constructor
        public TelemetryWriter(int port)
        {
            try
            {
                this.InitUdp(port);
            }
            catch (Exception e)
            {
                
            }
        }

        private void InitUdp(int port)
        {
            try
            {
                if (this.udpClient == null)
                {
                    this.udpClient = new UdpClient();
                    this.udpClient.Connect("127.0.0.1", port);                    
                }
            }
            catch
            {
                this.udpClient = null;
            }
        }

        public void SendPacket(byte[] data)
        {
            this.udpClient.SendAsync(data, data.Length);
        }

        #region IDisposable Support
        private bool disposedValue = false; // Per rilevare chiamate ridondanti

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    udpClient.Close();
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.

                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~TelemetryWriter() {
        //   // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
        //   Dispose(false);
        // }

        // Questo codice viene aggiunto per implementare in modo corretto il criterio Disposable.
        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
            Dispose(true);
            // TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override del finalizzatore.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
