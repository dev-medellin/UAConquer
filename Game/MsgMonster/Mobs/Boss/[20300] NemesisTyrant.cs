using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_20300 : Base
    {
        public static List<coords> NemesisTyrant = new List<coords>() {
            new coords(092, 167) { },
            new coords(092, 167) { },
            new coords(092, 167) { },
            new coords(092, 167) { },
            new coords(092, 167) { }
            };
        public MobID_20300(MonsterFamily _mob)
            : base(_mob, BossType.General)
        {
            var spawnLoc = NemesisTyrant[Program.Rand.Next(0, NemesisTyrant.Count)];

            ID = IDMonster.NemesisTyrant;
            MapID = 1121;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 9;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}