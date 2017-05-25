using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA5Navigator
{
    public enum Hint { TURN = 0, KEEP, FOLLOW, INVERSION, EXIT, WRONG, RECOMP, DEST, DISTANCE}
    public enum Dir { NONE = 0, RIGHT, LEFT };

    public class NavVoice : ISound
    {
        public string Key { get; }
        public Hint Hint { get; }
        public Dir Dir { get; }
        public int Dist { get; }

        public NavVoice(string k, Hint h, Dir dir, int dst)
        {
            Key = k;
            Hint = h;
            Dir = dir;
            Dist = dst;
        }
    }

    static public class NavVoices
    {
        public static NavVoice In450m = new NavVoice("in450m", Hint.DISTANCE, Dir.NONE, 200);
        public static NavVoice In400m = new NavVoice("in400m", Hint.DISTANCE, Dir.NONE, 200);
        public static NavVoice In350m = new NavVoice("in350m", Hint.DISTANCE, Dir.NONE, 200);
        public static NavVoice In300m = new NavVoice("in300m", Hint.DISTANCE, Dir.NONE, 200);
        public static NavVoice In250m = new NavVoice("in250m", Hint.DISTANCE, Dir.NONE, 200);
        public static NavVoice In200m = new NavVoice("in200m", Hint.DISTANCE, Dir.NONE, 200);
        public static NavVoice In150m = new NavVoice("in150m", Hint.DISTANCE, Dir.NONE, 150);
        public static NavVoice In100m = new NavVoice("in100m", Hint.DISTANCE, Dir.NONE, 100);
        public static NavVoice TurnL = new NavVoice("turn-l", Hint.TURN, Dir.LEFT, 0);
        public static NavVoice TurnR = new NavVoice("turn-r", Hint.TURN, Dir.RIGHT, 0);

        public static NavVoice DestRight = new NavVoice("dest-r", Hint.DEST, Dir.NONE, 0);
        public static NavVoice DestLeft = new NavVoice("dest-l", Hint.DEST, Dir.NONE, 0);
        public static NavVoice Dest = new NavVoice("dest", Hint.DEST, Dir.NONE, 0);

        public static NavVoice Inversion = new NavVoice("inversion", Hint.INVERSION, Dir.NONE, 0);
        public static NavVoice ExitR = new NavVoice("exit-r", Hint.EXIT, Dir.RIGHT, 0);
        public static NavVoice ExitL = new NavVoice("exit-l", Hint.EXIT, Dir.LEFT, 0);

        public static NavVoice Keep = new NavVoice("keep", Hint.KEEP, Dir.NONE, 0);
        public static NavVoice Follow = new NavVoice("follow", Hint.FOLLOW, Dir.NONE, 0);
        public static NavVoice Calculating = new NavVoice("calculating", Hint.RECOMP, Dir.NONE, 0);
        
        public static NavVoice WrongDirection = new NavVoice("wrong-direction", Hint.WRONG, Dir.NONE, 0);

        public static NavVoice[] Voices =
            { In400m, In300m, In250m, In200m, In150m, In100m, DestRight, DestLeft, Dest, Inversion,
              TurnL, TurnR, ExitR, ExitL, Keep, Calculating, WrongDirection, Follow };

        public static NavVoice[] Distances = { null, In100m, In150m, In200m, In250m, In300m, In350m, In400m, In450m };
    }
}
