using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace GTAVSimhub.Plugin
{
    class Property
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Object Value { get; set; }
    }


    class DataConsumer : IDisposable
    {
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
            }
        }

        public Object GetSharedData()
        {
            try
            {
                sharedBuffer.AcquireReadLock();
                Object o = null;

                // Get the message size, 0 = no data available
                if (sharedBuffer.Length > 2)
                {
                    // Get the message size (first 4 bytes), 0 = no data available
                    byte[] b2 = sharedBuffer.Take(4).ToArray();

                    int dataSize = BitConverter.ToInt32(b2, 0);

                    if (dataSize > 0)
                    {
                        // Get the serialized object
                        Byte[] data = sharedBuffer.Skip(4).Take(dataSize).ToArray<Byte>();

                        o = toObject(data);
                    }
                }

                sharedBuffer.ReleaseReadLock();
                return o;
            }
            catch (TimeoutException e)
            {
                return null;
            }
        }

        static void Main(string[] args)
        {

            DataConsumer dc = new DataConsumer("GTAV");
            {

                while (true)
                {
                    string[] a = (string[])dc.GetSharedData();

                    if (a != null) { 
                    foreach (string s in a)
                    {
                        Property p = dc.getProperty(s);

                        Console.WriteLine(p.Name + " : " + p.Type + " : " + p.Value);
                    }
                }
            }
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
