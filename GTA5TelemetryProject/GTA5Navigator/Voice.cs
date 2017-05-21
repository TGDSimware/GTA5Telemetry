using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA5Navigator
{
    public enum Hint { TURN = 0, KEEP, INVERSION, EXIT, WRONG, RECOMP, DEST }
    public enum Dir { NONE = 0, RIGHT, LEFT };

    public class VoiceEntry
    {
        public string File;
        public Hint Hint;
        public Dir Dir;
        public int Dist;
    }

    static public class Voice
    {
        public static VoiceEntry In200mTurnL = new VoiceEntry { File = "in200m-turn-l", Hint = Hint.TURN, Dir = Dir.LEFT, Dist = 200 };
        public static VoiceEntry In150mTurnL = new VoiceEntry { File = "in150m-turn-l", Hint = Hint.TURN, Dir = Dir.LEFT, Dist = 150 };
        public static VoiceEntry In100mTurnL = new VoiceEntry { File = "in100m-turn-l", Hint = Hint.TURN, Dir = Dir.LEFT, Dist = 100 };
        public static VoiceEntry NowTurnL = new VoiceEntry { File = "now-turn-l", Hint = Hint.TURN, Dir = Dir.LEFT, Dist = 0 };

        public static VoiceEntry In200mTurnR = new VoiceEntry { File = "in200m-turn-r", Hint = Hint.TURN, Dir = Dir.RIGHT, Dist = 200 };
        public static VoiceEntry In150mTurnR = new VoiceEntry { File = "in150m-turn-r", Hint = Hint.TURN, Dir = Dir.RIGHT, Dist = 150 };
        public static VoiceEntry In100mTurnR = new VoiceEntry { File = "in100m-turn-r", Hint = Hint.TURN, Dir = Dir.RIGHT, Dist = 100 };
        public static VoiceEntry NowTurnR = new VoiceEntry { File = "now-turn-r", Hint = Hint.TURN, Dir = Dir.RIGHT, Dist = 0 };

        public static VoiceEntry in200mDest = new VoiceEntry { File = "in200m-dest", Hint = Hint.DEST, Dir = Dir.NONE, Dist = 200 };
        public static VoiceEntry in150mDest = new VoiceEntry { File = "in150m-dest", Hint = Hint.DEST, Dir = Dir.NONE, Dist = 150 };
        public static VoiceEntry in100mDest = new VoiceEntry { File = "in100m-dest", Hint = Hint.DEST, Dir = Dir.NONE, Dist = 100 };
        public static VoiceEntry DestRight = new VoiceEntry { File = "dest-r", Hint = Hint.DEST, Dir = Dir.NONE, Dist = 0 };
        public static VoiceEntry DestLeft = new VoiceEntry { File = "dest-l", Hint = Hint.DEST, Dir = Dir.NONE, Dist = 0 };
        public static VoiceEntry Dest = new VoiceEntry { File = "dest", Hint = Hint.DEST, Dir = Dir.NONE, Dist = 0 };

        public static VoiceEntry In200mInversion = new VoiceEntry { File = "in200m-inversion", Hint = Hint.INVERSION, Dir = Dir.NONE, Dist = 200 };
        public static VoiceEntry In150mInversion = new VoiceEntry { File = "in150m-inversion", Hint = Hint.INVERSION, Dir = Dir.NONE, Dist = 150 };
        public static VoiceEntry In100mInversion = new VoiceEntry { File = "in100m-inversion", Hint = Hint.INVERSION, Dir = Dir.NONE, Dist = 100 };
        public static VoiceEntry NowInversion = new VoiceEntry { File = "now-inversion", Hint = Hint.INVERSION, Dir = Dir.NONE, Dist = 0 };

        public static VoiceEntry In200mExitR = new VoiceEntry { File = "in200m-exit-r", Hint = Hint.EXIT, Dir = Dir.RIGHT, Dist = 200 };
        public static VoiceEntry In150mExitR = new VoiceEntry { File = "in150m-exit-r", Hint = Hint.EXIT, Dir = Dir.RIGHT, Dist = 150 };
        public static VoiceEntry In100mExitR = new VoiceEntry { File = "in100m-exit-r", Hint = Hint.EXIT, Dir = Dir.RIGHT, Dist = 100 };
        public static VoiceEntry ExitR = new VoiceEntry { File = "exit-r", Hint = Hint.EXIT, Dir = Dir.RIGHT, Dist = 0 };

        public static VoiceEntry In200mExitL = new VoiceEntry { File = "in200m-exit-l", Hint = Hint.EXIT, Dir = Dir.LEFT, Dist = 200 };
        public static VoiceEntry In150mExitL = new VoiceEntry { File = "in150m-exit-l", Hint = Hint.EXIT, Dir = Dir.LEFT, Dist = 150 };
        public static VoiceEntry In100mExitL = new VoiceEntry { File = "in100m-exit-l", Hint = Hint.EXIT, Dir = Dir.LEFT, Dist = 100 };
        public static VoiceEntry ExitL = new VoiceEntry { File = "exit-l", Hint = Hint.EXIT, Dir = Dir.LEFT, Dist = 0 };

        public static VoiceEntry Keep = new VoiceEntry { File = "keep", Hint = Hint.KEEP, Dir = Dir.NONE, Dist = 0 };

        public static VoiceEntry Calculating = new VoiceEntry { File = "calculating", Hint = Hint.RECOMP, Dir = Dir.NONE, Dist = 0 };
        //public static VoiceEntry Recomputing = new VoiceEntry { File = "recomputing", Hint = Hint.RECOMP, Dir = Dir.NONE, Dist = 0 };
        public static VoiceEntry WrongDirection = new VoiceEntry { File = "wrong-direction", Hint = Hint.WRONG, Dir = Dir.NONE, Dist = 0 };
    }
}
