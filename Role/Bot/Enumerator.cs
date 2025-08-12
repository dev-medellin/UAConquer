using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Role.Bot
{
    /// <summary>
    /// Description of BotLevel.
    /// </summary>
    public enum BotLevel
    {
        Easy = 1,
        Normal = 2,
        Medium = 3,
        Hard = 4,
        Insane = 5
    }

    /// <summary>
    /// Description of BotType.
    /// </summary>
    public enum BotType
    {
        EventBot,
        DuelBot,
        TournamentBot,
        AFKBot,
        ArenaBot,
        HuntingBot
    }

    /// <summary>
    /// Description of SkillType.
    /// </summary>
    public enum SkillType
    {
        ScentSword,
        FastBlade,
    }
}
