using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game;

namespace TheChosenProject.Bosses
{
    public class MobID_4172 : MobBase
    {
        public MobID_4172(Mob _mob)
            : base(_mob)
        {
            ID = 4172;
        }

        public override void Run(Character C, Mob _mob, uint _damage, List<Character> PlayerTargets)
        {
            base.Run(C, _mob, _damage, PlayerTargets);

            if (MyMath.ChanceSuccess(10))
                RainDance(_mob, 2000, PlayerTargets);

            else if (MyMath.ChanceSuccess(15))
                Whirlpool(_mob, 5, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                Blizzard(_mob, 5, PlayerTargets);

            else if (MyMath.ChanceSuccess(15))
                Pervade(_mob, 750, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                AquaRing(_mob, 1500, PlayerTargets);

            if (MyMath.ChanceSuccess(3))
                Penetration(80, C);
        }
    }
}