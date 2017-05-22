/// <summary>
/// GTA V Navigator Mod
///  
/// If this code works, it has been written by Carlo Iovino (carlo.iovino@outlook.com)
/// The Green Dragon Youtube Channel (www.youtube.com/carloxofficial)
/// 
/// </summary>
using System;
using System.IO;
using System.Collections.Generic;

namespace Audio
{
    public struct WavHeader
    {
        public byte[] riffID; // "riff"
        public uint size;
        public byte[] wavID;
        public byte[] fmtID;
        public uint fmtSize;
        public ushort format;
        public ushort channels;
        public uint sampleRate;
        public uint bytePerSec;
        public ushort blockSize;
        public ushort bit;
        public byte[] dataID;
        public uint dataSize;
    }

    public class Wav : IDisposable
    {
        private WavHeader Header;
        private List<short> RData;
        private List<short> LData;

        public static float Play(string fileName, float volumeFactor = 1)
        {
            using (System.Media.SoundPlayer snd = new System.Media.SoundPlayer())
            {
                Wav wav = Wav.Parse(fileName, volumeFactor);

                using (var ms = new MemoryStream())
                {
                    try
                    {
                        wav.Write(ms);
                        ms.Position = 0;
                        snd.Stream = ms;
                        snd.Play();
                    }
                    finally
                    {
                        if (ms != null) ms.Close();
                    }
                }

                return wav.Duration;
            }
        }

        public float Play(float volumeFactor = 1)
        {
            using (System.Media.SoundPlayer snd = new System.Media.SoundPlayer())
            {

                using (var ms = new MemoryStream())
                {
                    this.Write(ms);
                    snd.Stream = ms;
                    ms.Position = 0;
                    snd.Play();
                    ms.Close();
                }

                return Duration;
            }
        }

        private Wav(WavHeader h, List<short> ld, List<short> rd)
        {
            Header = h;
            RData = rd;
            LData = ld;
        }

        public float Duration
        {
            get
            {
                try { 
                    float bytes = (float)(LData.Count + RData.Count) * 2f;
                    return bytes / (float)Header.bytePerSec;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static Wav Parse(string fileName, float volumeFactor = 1)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    return Parse(fs, volumeFactor);
                }
                finally
                {
                    if (fs != null) fs.Close();
                }
            }
        }

        public static Wav Parse(Stream stream, float volumeFactor = 1)
        {
            WavHeader Header = new WavHeader();
            List<short> lDataList = new List<short>();
            List<short> rDataList = new List<short>();

            using (BinaryReader br = new BinaryReader(stream))
            {
                try
                {
                    Header.riffID = br.ReadBytes(4);
                    Header.size = br.ReadUInt32();
                    Header.wavID = br.ReadBytes(4);
                    Header.fmtID = br.ReadBytes(4);
                    Header.fmtSize = br.ReadUInt32();
                    Header.format = br.ReadUInt16();
                    Header.channels = br.ReadUInt16();
                    Header.sampleRate = br.ReadUInt32();
                    Header.bytePerSec = br.ReadUInt32();
                    Header.blockSize = br.ReadUInt16();
                    Header.bit = br.ReadUInt16();
                    Header.dataID = br.ReadBytes(4);
                    Header.dataSize = br.ReadUInt32();

                    short factor = Convert.ToInt16(100 * volumeFactor);

                    for (int i = 0; i < Header.dataSize / Header.blockSize; i++)
                    {
                        lDataList.Add((short)((float)br.ReadInt16() * volumeFactor));
                        rDataList.Add((short)((float)br.ReadInt16() * volumeFactor));
                    }
                }
                finally
                {
                    if (br != null)
                    {
                        br.Close();
                    }
                }
            }
            return new Wav(Header, lDataList, rDataList);
        }

        public void Write(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                Write(fs);
            }
        }

        public void Write(Stream stream)
        {
            var lNewDataList = LData;
            var rNewDataList = RData;
            var Header = this.Header;

            Header.dataSize = (uint)Math.Max(lNewDataList.Count, rNewDataList.Count) * 4;

            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write(Header.riffID);
            bw.Write(Header.size);
            bw.Write(Header.wavID);
            bw.Write(Header.fmtID);
            bw.Write(Header.fmtSize);
            bw.Write(Header.format);
            bw.Write(Header.channels);
            bw.Write(Header.sampleRate);
            bw.Write(Header.bytePerSec);
            bw.Write(Header.blockSize);
            bw.Write(Header.bit);
            bw.Write(Header.dataID);
            bw.Write(Header.dataSize);

            for (int i = 0; i < Header.dataSize / Header.blockSize; i++)
            {
                if (i < lNewDataList.Count)
                {
                    bw.Write((ushort)lNewDataList[i]);
                }
                else
                {
                    bw.Write(0);
                }

                if (i < rNewDataList.Count)
                {
                    bw.Write((ushort)rNewDataList[i]);
                }
                else
                {
                    bw.Write(0);
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
                    LData.Clear();
                    RData.Clear();
                    LData = null;
                    RData = null;
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.

                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~Wav() {
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
