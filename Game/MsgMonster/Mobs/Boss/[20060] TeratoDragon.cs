using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_20060 : Base
    {
        public static List<coords> TeratoDragon = new List<coords>() {
            new coords(156, 248) { },
            new coords(156, 248) { },
            new coords(156, 248) { },
            new coords(156, 248) { },
            new coords(156, 248) { }
            };
        //public static List<coords> TeratoDragon = new List<coords>() {
        //    new coords(565, 793) { },
        //    new coords(565, 793) { },
        //    new coords(565, 793) { },
        //    new coords(565, 793) { },
        //    new coords(565, 793) { }
        //    };
        public MobID_20060(MonsterFamily _mob)
            : base(_mob, BossType.General)
        {
            var spawnLoc = TeratoDragon[Program.Rand.Next(0, TeratoDragon.Count)];

            ID = IDMonster.TeratoDragon;
            MapID = 1121;
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 6;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}