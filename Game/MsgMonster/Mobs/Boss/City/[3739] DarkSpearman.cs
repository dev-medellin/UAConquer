using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_3739 : Base
    {
        public static List<coords> DarkSpearman = new List<coords>() {
            new coords(440,444) { },
            new coords(440,444) { },
            new coords(440,444) { },
            new coords(440,444) { },
            new coords(440,444) { }
            };
        public MobID_3739(MonsterFamily _mob)
            : base(_mob, BossType.City)
        {
            var spawnLoc = DarkSpearman[Program.Rand.Next(0, DarkSpearman.Count)];

            ID = IDMonster.DarkSpearman;
            MapID = 1002;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 4;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}