using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_3738 : Base
    {
        public static List<coords> MummySkeleton = new List<coords>() {
            new coords(313,250) { },
            new coords(262,241) { },
            new coords(205,231) { },
            new coords(241,201) { },
            new coords(578,601) { }
            };
        public MobID_3738(MonsterFamily _mob)
            : base(_mob, BossType.City)
        {
            var spawnLoc = MummySkeleton[Program.Rand.Next(0, MummySkeleton.Count)];

            ID = IDMonster.MummySkeleton;
            MapID = 1000;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 3;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}