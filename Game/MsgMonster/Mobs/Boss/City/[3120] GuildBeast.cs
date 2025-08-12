using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;

namespace TheChosenProject.Mobs
{
    public class MobID_3120 : Base
    {
        public MobID_3120(MonsterFamily _mob)
            : base(_mob, BossType.City)
        {
            ID = IDMonster.GuildBeast;
            MapName = "in GuildArea";
            MapID = 1038;
            X = 68;
            Y = 98;
            LinkID = 6;
        }

        public override void Run()
        {
            base.Run();
        }
    }
}