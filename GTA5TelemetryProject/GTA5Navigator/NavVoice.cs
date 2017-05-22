using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA5Navigator
{
    public enum Hint { TURN = 0, KEEP, FOLLOW, INVERSION, EXIT, WRONG, RECOMP, DEST}
    public enum Dir { NONE = 0, RIGHT, LEFT };

    public class NavSound : ISound
    {
        public string Key { get; }
        public Hint Hint { get; }
        public Dir Dir { get; }
        public int Dist { get; }

        public NavSound(string k, Hint h, Dir dir, int dst)
        {
            Key = k;
            Hint = h;
            Dir = dir;
            Dist = dst;
        }
    }

    static public class NavVoice
    {
        public static NavSound In200mTurnL = new NavSound("in200m-turn-l", Hint.TURN, Dir.LEFT, 200);
        public static NavSound In150mTurnL = new NavSound("in150m-turn-l", Hint.TURN, Dir.LEFT, 150);
        public static NavSound In100mTurnL = new NavSound("in100m-turn-l", Hint.TURN, Dir.LEFT, 100);
        public static NavSound NowTurnL = new NavSound("now-turn-l", Hint.TURN, Dir.LEFT, 0);

        public static NavSound In200mTurnR = new NavSound("in200m-turn-r", Hint.TURN, Dir.RIGHT, 200);
        public static NavSound In150mTurnR = new NavSound("in150m-turn-r", Hint.TURN, Dir.RIGHT, 150);
        public static NavSound In100mTurnR = new NavSound("in100m-turn-r", Hint.TURN, Dir.RIGHT, 100);
        public static NavSound NowTurnR = new NavSound("now-turn-r", Hint.TURN, Dir.RIGHT, 0);

        public static NavSound In200mDest = new NavSound("in200m-dest", Hint.DEST, Dir.NONE, 200);
        public static NavSound In150mDest = new NavSound("in150m-dest", Hint.DEST, Dir.NONE, 150);
        public static NavSound In100mDest = new NavSound("in100m-dest", Hint.DEST, Dir.NONE, 100);
        public static NavSound DestRight = new NavSound("dest-r", Hint.DEST, Dir.NONE, 0);
        public static NavSound DestLeft = new NavSound("dest-l", Hint.DEST, Dir.NONE, 0);
        public static NavSound Dest = new NavSound("dest", Hint.DEST, Dir.NONE, 0);

        public static NavSound In200mInversion = new NavSound("in200m-inversion", Hint.INVERSION, Dir.NONE, 200);
        public static NavSound In150mInversion = new NavSound("in150m-inversion", Hint.INVERSION, Dir.NONE, 150);
        public static NavSound In100mInversion = new NavSound("in100m-inversion", Hint.INVERSION, Dir.NONE, 100);
        public static NavSound NowInversion = new NavSound("now-inversion", Hint.INVERSION, Dir.NONE, 0);

        public static NavSound In200mExitR = new NavSound("in200m-exit-r", Hint.EXIT, Dir.RIGHT, 200);
        public static NavSound In150mExitR = new NavSound("in150m-exit-r", Hint.EXIT, Dir.RIGHT, 150);
        public static NavSound In100mExitR = new NavSound("in100m-exit-r", Hint.EXIT, Dir.RIGHT, 100);
        public static NavSound ExitR = new NavSound("exit-r", Hint.EXIT, Dir.RIGHT, 0);

        public static NavSound In200mExitL = new NavSound("in200m-exit-l", Hint.EXIT, Dir.LEFT, 200);
        public static NavSound In150mExitL = new NavSound("in150m-exit-l", Hint.EXIT, Dir.LEFT, 150);
        public static NavSound In100mExitL = new NavSound("in100m-exit-l", Hint.EXIT, Dir.LEFT, 100);
        public static NavSound ExitL = new NavSound("exit-l", Hint.EXIT, Dir.LEFT, 0);

        public static NavSound Keep = new NavSound("keep", Hint.KEEP, Dir.NONE, 0);
        public static NavSound Follow = new NavSound("follow", Hint.FOLLOW, Dir.NONE, 0);
        public static NavSound Calculating = new NavSound("calculating", Hint.RECOMP, Dir.NONE, 0);
        //public static NavSound Recomputing = new NavSound( "recomputing", Hint.RECOMP,Dir.NONE,0 );
        public static NavSound WrongDirection = new NavSound("wrong-direction", Hint.WRONG, Dir.NONE, 0);

        public static NavSound[] Voices =
            { In200mTurnL, In150mTurnL, In100mTurnL, NowTurnL, In200mTurnR, In150mTurnR, In100mTurnR, NowTurnR, In200mDest,
        In150mDest, In100mDest, DestRight, DestLeft, Dest, In200mInversion, In150mInversion, In100mInversion, NowInversion,
        In200mExitR, In150mExitR, In100mExitR, ExitR, In200mExitL, In150mExitL, In100mExitL, ExitL, Keep, Calculating, WrongDirection, Follow };
    }
}
