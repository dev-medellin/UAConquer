using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game;

namespace TheChosenProject.Bosses
{
    public class MobID_4152 : MobBase
    {
        public MobID_4152(Mob _mob)
            : base(_mob)
        {
            ID = 4152;
        }

        public override void Run(Character C, Mob _mob, uint _damage, List<Character> PlayerTargets)
        {
            foreach (Character player in PlayerTargets)
                player.MyClient.AddSend(Packets.ChatMessage(0, "SYSTEM", "ALLUSERS", _mob.Name + $" HP: {_mob.CurrentHP } / { _mob.MaxHP }", 0x83c, 0));

            if (MyMath.ChanceSuccess(3))
                FireBreath(_mob, 1500, PlayerTargets);
        }
    }
}
