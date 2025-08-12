using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Mobs
{
    public class MobID_6643 : Base
    {
        //public static List<coords> NemesisTyrant = new List<coords>() {
        //    new coords(209, 223) { },
        //    new coords(209, 223) { },
        //    new coords(209, 223) { },
        //    new coords(209, 223) { },
        //    new coords(209, 223) { }
        //    };

        public static List<coords> NemesisTyrant = new List<coords>() {
            new coords(196, 179) { },
            new coords(196, 179) { },
            new coords(196, 179) { },
            new coords(196, 179) { },
            new coords(196, 179) { }
            };
        public MobID_6643(MonsterFamily _mob)
            : base(_mob, BossType.General)
        {
            var spawnLoc = NemesisTyrant[Program.Rand.Next(0, NemesisTyrant.Count)];

            ID = IDMonster.SowrdMaster;
            MapID = 8250; //new map 1121
            X = (ushort)spawnLoc.X;
            Y = (ushort)spawnLoc.Y;
            LinkID = 9;
        }
        public override void Run()
        {
            base.Run();
        }
        //public override void Reward(MonsterRole MobRole, Client.GameClient killer)
        //{
        //    base.Reward(MobRole, killer);
        //    killer.DbDailyTraining.SwordMaster++;
        //}
    }
}