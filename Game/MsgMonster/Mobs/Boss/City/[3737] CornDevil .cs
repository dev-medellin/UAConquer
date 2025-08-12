using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_3737 : Base
    {
        public static List<coords> Ganoderma = new List<coords>() {
            new coords(313,250) { },
            new coords(262,241) { },
            new coords(205,231) { },
            new coords(241,201) { },
            new coords(578,601) { }
            };
        public MobID_3737(MonsterFamily _mob)
            : base(_mob, BossType.City)
        {
            var spawnLoc = Ganoderma[Program.Rand.Next(0, Ganoderma.Count)];

            ID = IDMonster.CornDevil;
            MapID = 1015;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 2;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}