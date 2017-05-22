/// <summary>
/// GTA V Navigator Mod
///  
/// If this code works, it has been written by Carlo Iovino (carlo.iovino@outlook.com)
/// The Green Dragon Youtube Channel (www.youtube.com/carloxofficial)
/// 
/// </summary>
using System;
using System.Collections.Generic;

namespace GTA5Navigator
{    
    public interface ISound
    {
        string Key { get; }
    }

    public class AudioManager : IDisposable
    {
        // The Dictionary is used for wav preloading in memory
        private Dictionary<string, Audio.Wav> Store;
        public float VolumeFactor { get; set; } = 1;
        public string BasePath { get; set; } = "";
        public string Extension { get; set; } = "wav";

        public AudioManager()
        {
            Store = new Dictionary<string, Audio.Wav>();
        }

        public void Preload(ISound[] sounds)
        {
            Store.Clear();
            foreach (var sound in sounds)
            {
                var wav = Audio.Wav.Parse(BasePath + @"\" + sound.Key + "." + Extension, VolumeFactor);
                Store.Add(sound.Key, wav);
            }
        }

        public Audio.Wav GetWav(string key)
        {
            return Store[key];
        }

        public float Play(string key)
        {
            try
            {
                var wav = Store[key];
                return wav.Play();
            }
            catch
            {
                throw new Exception("SoundManager: key '" + key + "' not found in Store");
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
                    Store.Clear();
                    Store = null;
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.
                
                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~AudioManager() {
        //   // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
        //   Dispose(false);
        // }

        // Questo codice viene aggiunto per implementare in modo corretto il criterio Disposable.
        void IDisposable.Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
            Dispose(true);
            // TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override del finalizzatore.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
