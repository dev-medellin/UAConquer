using System;
using System.Collections.Generic;
using System.Text;

namespace TheChosenProject
{
    public class Global
    {
        /*
         * Calculate Drop Rates 

                1x Meteor       : 1,8% =             1 of 511 Kills   (METZONE 2x)
                1x Meteor       : 1,0% =             1 of 900 Kills
                1x dragonball : 0.18% =           1 of 5455 Kills (METZONE 2x)
                1x dragonball : 0,1%  =             1 of 9990 Kills

                +1 items = 0,45%          = 1 of 2100 kills 

                1x Dragonball(+1) : 0.01% = 100 x 99,9% = 9990 x 100      = 1 of 999.000 Kills  (only drop if LuckyTime)
                1x Meteor(+1)     : 0,03% = 33,33~ x 99,7% = 3323 x 100    = 1 of 332.333 Kills (only drop if LuckyTime)

                +1 Stone = 0,24%          =  1 of 4066 kills (only drop if luckytime)
                Expball =  0,38%            = 1 of 2531 kills (only drop if luckytime)
        */
        public const double
            LUCKY_BLUE_MOUSE_RATE = 0.009,
        LUCKY_TIME_EXP_RATE = 0.002,
        LUCKY_TIME_PLUS_RATE = 0.1,
        LUCKY_TIME_BONUS_SOCKET_RATE = 0.01,//This is ADDED to the existing socket rate
        LUCKY_TIME_BONUS_SECOND_SOCKET_RATE = .01,//This is ADDED to the existing socket rate
        LUCKY_TIME_CRIT_RATE_RANGED = .9,//One in 20 monsters hit with lucky time will take double dmg
        LUCKY_TIME_CRIT_RATE_PHYSICAL = .15,//One in 20 monsters hit with lucky time will take double dmg
        LUCKY_TIME_CRIT_RATE_MAGIC = .7,//One in 20 monsters hit with lucky time will take double dmg
        LUCKY_TIME_CRIT_RATE_MONSTER = 5,
           MINING_DROP_GEMS = 0.138,
            MINING_DROP_GEMS_SUPER = 0.45,
            MINING_DROP_GEMS_REFIND = 3.33,
            MINING_DROP_DRAGONBALL = 0.024;


    }
}
