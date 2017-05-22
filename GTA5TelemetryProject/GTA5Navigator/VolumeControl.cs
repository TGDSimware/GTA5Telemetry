using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GTA5Navigator
{
    [System.Flags]
    public enum PlaySoundFlags
    {
        /// <summary>play synchronously (default)</summary>
        SND_SYNC = 0x0000,
        /// <summary>play asynchronously</summary>
        SND_ASYNC = 0x0001,
        /// <summary>silence (!default) if sound not found</summary>
        SND_NODEFAULT = 0x0002,
        /// <summary>pszSound points to a memory file</summary>
        SND_MEMORY = 0x0004,
        /// <summary>loop the sound until next sndPlaySound</summary>
        SND_LOOP = 0x0008,
        /// <summary>don’t stop any currently playing sound</summary>
        SND_NOSTOP = 0x0010,
        /// <summary>Stop Playing Wave</summary>
        SND_PURGE = 0x40,
        /// <summary>don’t wait if the driver is busy</summary>
        SND_NOWAIT = 0x00002000,
        /// <summary>name is a registry alias</summary>
        SND_ALIAS = 0x00010000,
        /// <summary>alias is a predefined id</summary>
        SND_ALIAS_ID = 0x00110000,
        /// <summary>name is file name</summary>
        SND_FILENAME = 0x00020000,
        /// <summary>name is resource name or atom</summary>
        SND_RESOURCE = 0x00040004
    }

    public static class WavPlayer
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [System.Runtime.InteropServices.DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        private static extern bool PlaySound(string szSound, System.IntPtr hMod, PlaySoundFlags flags);

        static IntPtr intPtr = new System.IntPtr();
        static System.Media.SoundPlayer snd = new System.Media.SoundPlayer();

        public static int SetVolume(int newVolumeInPercent)
        {
            UInt32 newVol = (UInt32)((double)(newVolumeInPercent * ushort.MaxValue) / 100.0);
            newVol = newVol + (newVol << 16);

            int resultSetVolume = waveOutSetVolume(GetModuleHandle(null), newVol);

            return (int)Math.Round((double)resultSetVolume * 100 / ushort.MaxValue);
        }

        public static void Play(string fileName) {
            PlaySound(fileName, IntPtr.Zero, PlaySoundFlags.SND_SYNC);
        }

        public static void PlayAsync(string fileName)
        {
            PlaySound(fileName, GetModuleHandle(null), PlaySoundFlags.SND_ASYNC);
        }
    }
}
