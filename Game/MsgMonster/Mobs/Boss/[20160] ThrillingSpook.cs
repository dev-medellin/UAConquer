using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_20160 : Base
    {
        public static List<coords> ThrillingSpook = new List<coords>() {
            new coords(087, 089) { },
            new coords(087, 089) { },
            new coords(087, 089) { },
            new coords(087, 089) { },
            new coords(087, 089) { }
            };
        //public static List<coords> ThrillingSpook = new List<coords>() {
        //    new coords(041, 040) { },
        //    new coords(041, 040) { },
        //    new coords(041, 040) { },
        //    new coords(041, 040) { },
        //    new coords(041, 040) { }
        //    };
        public MobID_20160(MonsterFamily _mob)
            : base(_mob, BossType.General)
        {
            var spawnLoc = ThrillingSpook[Program.Rand.Next(0, ThrillingSpook.Count)];

            ID = IDMonster.ThrillingSpook;
            MapID = 1121;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 8;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}