using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgTournaments;

namespace TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CheckLineSpells
    {
        public static bool CheckUp(GameClient user, ushort spellid)
        {
            if (user.EventBase != null)
            {
                if (!user.EventBase.MagicAllowed && (user.EventBase?.Stage == MsgEvents.EventStage.Fighting || user.EventBase?.Stage == MsgEvents.EventStage.Countdown))
                {
                    if (user.EventBase?.AllowedSkills != null)
                    {
                        if (user.EventBase?.AllowedSkills.Count > 0)
                        {
                            if (!user.EventBase.AllowedSkills.Contains(spellid))
                            {
                                user.SendSysMesage("This skill cannot be used in this event!");
                                return false;
                            }
                        }
                        else
                            return false;
                    }
                    else
                    {
                        user.SendSysMesage("This skill cannot be used in this event!");
                        return false;
                    }
                }
            }
            if (user.InPassTheBomb && spellid != 1045 && spellid != 1046 && spellid != 11005 && spellid != 11000)
            {
                user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword/ViperFang/DragonTrail)");
                return false;
            }
            if (user.InFIveOut && spellid != 1045 && spellid != 1046 && spellid != 11005 && spellid != 11000)
            {
                user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword/ViperFang/DragonTrail)");
                return false;
            }
            if (user.InTDM && spellid != 1045 && spellid != 1046 && spellid != 11005 && spellid != 11000)
            {
                user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword/ViperFang/DragonTrail)");
                return false;
            }
            if (user.InST && spellid != 1045 && spellid != 1046 && spellid != 11005 && spellid != 11000)
            {
                user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword/ViperFang/DragonTrail)");
                return false;
            }
            if (UnlimitedArenaRooms.Maps.ContainsValue(user.Player.DynamicID) && spellid != 1045 && spellid != 1046 && spellid != 11005 && spellid != 11000)
            {
                user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword/ViperFang/DragonTrail)");
                return false;
            }
            if (user.InQualifier() && user.ArenaStatistic.InDuelRoom && spellid != 1045 && spellid != 1046 && spellid != 11005 && spellid != 11000)
            {
                user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword/ViperFang/DragonTrail)");
                return false;
            }
            return true;
        }
    }
}
