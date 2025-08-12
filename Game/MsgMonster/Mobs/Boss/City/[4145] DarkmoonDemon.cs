using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_4145 : Base
    {
        public static List<coords> DarkmoonDemon = new List<coords>() {
            new coords(313,250) { },
            new coords(262,241) { },
            new coords(205,231) { },
            new coords(241,201) { },
            new coords(578,601) { }
            };
        public MobID_4145(MonsterFamily _mob)
            : base(_mob, BossType.City)
        {
            var spawnLoc = DarkmoonDemon[Program.Rand.Next(0, DarkmoonDemon.Count)];

            ID = IDMonster.DarkmoonDemon;
            MapID = 1020;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 5;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}