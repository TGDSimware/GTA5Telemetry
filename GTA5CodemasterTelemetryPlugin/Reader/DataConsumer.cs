using System;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GTA5Reader
{
    class DataConsumer : IDisposable
    {
        private BinaryFormatter binaryFormatter = new BinaryFormatter();
        private SharedMemory.SharedArray<byte> sharedBuffer = null;

        private Object toObject(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(65535))
            {
                try
                {
                    ms.Write(data, 0, data.Length);
                    ms.Position = 0;

                    return binaryFormatter.Deserialize(ms);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        // Class Constructor
        public DataConsumer(string memId)
        {
            try
            {
                // Get the shared array
                sharedBuffer = new SharedMemory.SharedArray<byte>(name: memId);
            }
            catch (Exception e)
            {
                // Creates the shared array if it doesn't exist
                sharedBuffer = new SharedMemory.SharedArray<byte>(name: memId, length: 65535);
                sharedBuffer.AcquireWriteLock();
                sharedBuffer.Write(new byte[] { 0, 0, 0, 0 });
                sharedBuffer.ReleaseWriteLock();
            }
        }

        public byte[] GetSharedData()
        {
            try
            {
                sharedBuffer.AcquireReadLock();
                byte[] data = null;

                // Get the message size, 0 = no data available
                if (sharedBuffer.Length > 4)
                {
                    // Get the message size (first 4 bytes), 0 = no data available
                    byte[] b2 = sharedBuffer.Take(4).ToArray();

                    int dataSize = BitConverter.ToInt32(b2, 0);

                    if (dataSize > 0)
                    {
                        // Get the serialized object
                        data = sharedBuffer.Skip(4).Take(dataSize).ToArray<Byte>();

                        //o = toObject(data);
                    }
                    else return null;
                }

                sharedBuffer.ReleaseReadLock();
                return data;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Per rilevare chiamate ridondanti

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti).
                    sharedBuffer.Dispose();
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.

                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~DataConsumer() {
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