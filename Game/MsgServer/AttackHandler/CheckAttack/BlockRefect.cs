using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgTournaments;

namespace TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack
{
    public class BlockRefect
    {
        public static bool CanUseReflect(GameClient user)
        {
            if (MsgSchedules.CurrentTournament.Type == TournamentType.SkillTournament && MsgSchedules.CurrentTournament.Process == ProcesType.Alive && MsgSchedules.CurrentTournament.InTournament(user))
                return false;
            if (user.InST)
                return false;
            if (user.InPassTheBomb)
                return false;
            if (user.InTDM)
                return false;
            if (user.InLastManStanding)
                return false;
            if (user.InFIveOut)
                return false;
            return true;
        }
    }
}
