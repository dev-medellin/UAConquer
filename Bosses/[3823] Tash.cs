using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game;

namespace TheChosenProject.Bosses
{
    public class MobID_3823 : MobBase
    {
        public MobID_3823(Mob _mob)
            : base(_mob)
        {
            ID = 3823;
        }

        public override void Run(Character C, Mob _mob, uint _damage, List<Character> PlayerTargets)
        {
            base.Run(C, _mob, _damage, PlayerTargets);

            if (MyMath.ChanceSuccess(10))
                FireBreath(_mob, 2000, PlayerTargets);

            else if (MyMath.ChanceSuccess(15))
                FireMeteor(_mob, 5, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                FireOfHell(_mob, 750, PlayerTargets);

            else if (MyMath.ChanceSuccess(15))
                Eruption(_mob, 750, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                Inferno(_mob, 1500, PlayerTargets);

            if (MyMath.ChanceSuccess(3))
                Penetration(80, C);
        }
    }
}
