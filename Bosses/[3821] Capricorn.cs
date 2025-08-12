using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game;

namespace TheChosenProject.Bosses
{
    public class MobID_3821 : MobBase
    {
        public MobID_3821(Mob _mob)
            : base(_mob)
        {
            ID = 3821;
        }

        public override void Run(Character C, Mob _mob, uint _damage, List<Character> PlayerTargets)
        {
            base.Run(C, _mob, _damage, PlayerTargets);

            if (MyMath.ChanceSuccess(10))
                Grasscutter(_mob, 500, PlayerTargets);

            else if (MyMath.ChanceSuccess(15))
                Cataclysm(_mob, 5, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                PoisonousFog(_mob, 750, PlayerTargets);

            else if (MyMath.ChanceSuccess(15))
                SolarBeam(_mob, 2500, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                Aromatherapy(_mob, 1000, PlayerTargets);

            if (MyMath.ChanceSuccess(3))
                Penetration(80, C);
        }
    }
}
