using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_3130 : Base
    {
        public static List<coords> Ganoderma = new List<coords>() {
            new coords(313,250) { },
            new coords(262,241) { },
            new coords(205,231) { },
            new coords(241,201) { },
            new coords(578,601) { }
            };
        public MobID_3130(MonsterFamily _mob)
            : base(_mob, BossType.City)
        {
            var spawnLoc = Ganoderma[Program.Rand.Next(0, Ganoderma.Count)];
            ID = IDMonster.Ganoderma;
            MapName = "in Phoenix Castle";
            MapID = 1011;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 1;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}