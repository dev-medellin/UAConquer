using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Game;

namespace TheChosenProject.Bosses
{
    public class MobID_3822 : MobBase
    {
        public MobID_3822(Mob _mob)
            : base(_mob)
        {
            ID = 3822;
        }

        public override void Run(Character C, Mob _mob, uint _damage, List<Character> PlayerTargets)
        {
            base.Run(C, _mob, _damage, PlayerTargets);

            if (MyMath.ChanceSuccess(10))
                SpeedLightning(_mob, 750, PlayerTargets);
            else if (MyMath.ChanceSuccess(15))
                Discharge(_mob, 5, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                ThunderWave(_mob, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                BoltStrike(_mob, 3000, PlayerTargets);

            else if (MyMath.ChanceSuccess(10))
                Thunderbolt(_mob, 1000, PlayerTargets);

            if (MyMath.ChanceSuccess(3))
                Penetration(80, C);
        }
    }
}
