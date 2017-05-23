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
        public static NavSound In400m = new NavSound("in400m", Hint.TURN, Dir.LEFT, 200);        
        public static NavSound In300m = new NavSound("in300m", Hint.TURN, Dir.LEFT, 200);
        public static NavSound In250m = new NavSound("in250m", Hint.TURN, Dir.LEFT, 200);
        public static NavSound In200m = new NavSound("in200m", Hint.TURN, Dir.LEFT, 200);
        public static NavSound In150m = new NavSound("in150m", Hint.TURN, Dir.LEFT, 150);
        public static NavSound In100m = new NavSound("in100m", Hint.TURN, Dir.LEFT, 100);
        public static NavSound TurnL = new NavSound("turn-l", Hint.TURN, Dir.LEFT, 0);
        public static NavSound TurnR = new NavSound("turn-r", Hint.TURN, Dir.LEFT, 0);

        public static NavSound DestRight = new NavSound("dest-r", Hint.DEST, Dir.NONE, 0);
        public static NavSound DestLeft = new NavSound("dest-l", Hint.DEST, Dir.NONE, 0);
        public static NavSound Dest = new NavSound("dest", Hint.DEST, Dir.NONE, 0);

        public static NavSound Inversion = new NavSound("inversion", Hint.INVERSION, Dir.NONE, 0);
        public static NavSound ExitR = new NavSound("exit-r", Hint.EXIT, Dir.RIGHT, 0);
        public static NavSound ExitL = new NavSound("exit-l", Hint.EXIT, Dir.LEFT, 0);

        public static NavSound Keep = new NavSound("keep", Hint.KEEP, Dir.NONE, 0);
        public static NavSound Follow = new NavSound("follow", Hint.FOLLOW, Dir.NONE, 0);
        public static NavSound Calculating = new NavSound("calculating", Hint.RECOMP, Dir.NONE, 0);
        
        public static NavSound WrongDirection = new NavSound("wrong-direction", Hint.WRONG, Dir.NONE, 0);

        public static NavSound[] Voices =
            { In400m, In300m, In250m, In200m, In150m, In100m, DestRight, DestLeft, Dest, Inversion,
              TurnL, TurnR, ExitR, ExitL, Keep, Calculating, WrongDirection, Follow };
    }
}
