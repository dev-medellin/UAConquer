using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    
    public class MobID_20070 : Base
    {
        //public static List<coords> SnowBanshee = new List<coords>() {
        //    new coords(246, 158) { },
        //    new coords(246, 158) { },
        //    new coords(246, 158) { },
        //    new coords(246, 158) { },
        //    new coords(246, 158) { }
        //    };
        public static List<coords> SnowBanshee = new List<coords>() {
            new coords(401, 439) { },
            new coords(401, 439) { },
            new coords(401, 439) { },
            new coords(401, 439) { },
            new coords(401, 439) { }
            };
        public MobID_20070(MonsterFamily _mob)
            : base(_mob, BossType.General)
        {
            var spawnLoc = SnowBanshee[Program.Rand.Next(0, SnowBanshee.Count)];

            ID = IDMonster.SnowBanshee;
            MapID = 2054;//new map 1121
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 7;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}