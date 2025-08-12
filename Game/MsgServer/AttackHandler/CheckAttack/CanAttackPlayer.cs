using TheChosenProject.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.Role.Bot;

namespace TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanAttackPlayer
    {
        public static bool Verified(GameClient client, Player attacked, MagicType.Magic DBSpell, bool Archer = false)
        {
            foreach (var item in client.Equipment.CurentEquip)
            {
                if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon
                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRightWeapon
                    || item.Position == (ushort)Role.Flags.ConquerItem.Ring
                    || item.Position == (ushort)Role.Flags.ConquerItem.AleternanteRing
                    || item.Position == (ushort)Role.Flags.ConquerItem.Fan
                    || item.Position == (ushort)Role.Flags.ConquerItem.RidingCrop)
                {
                    if (item.Durability <= 0)
                        return false;
                }
            }

            if (client.Pet != null)
            {
                if (client.Pet.Owner.Player.UID == attacked.UID)
                    return false;
            }
            var bot = BotProcessring.Bots.Values.FirstOrDefault(x => x.Bot.Player.UID == attacked.UID);
            if (bot != null)
            {
                if (bot.ToStart > DateTime.Now)
                    return false;

            }
                if (client.Player.OnTransform)
                return false;
            if (attacked.Map == 700 && attacked.DynamicID == 0)
                return false;
            if (!attacked.Alive)
                return false;
            if (attacked.Invisible)
                return false;
            if (client.Player.PkMode == Flags.PKMode.Peace)
                return false;
            if (MsgSchedules.CurrentTournament.Type == TournamentType.LastmanStand && MsgSchedules.CurrentTournament.InTournament(client) && MsgSchedules.CurrentTournament.Process != ProcesType.Alive)
                return false;
            if (MsgSchedules.CurrentTournament.Type == TournamentType.SkillTournament && MsgSchedules.CurrentTournament.InTournament(client) && MsgSchedules.CurrentTournament.Process != ProcesType.Alive)
                return false;
            if (attacked.ContainFlag(MsgUpdate.Flags.Fly) && !Archer)
            {
                if (DBSpell == null)
                    return false;
                if (!DBSpell.AttackInFly)
                    return false;
            }
            if (attacked.Owner.IsWatching())
                return false;
            if (client.IsWatching())
                return false;
            if (!attacked.AllowAttack() && attacked.Owner.AIType == AIEnum.AIType.NotActive)
                return false;
            if (client.InTeamQualifier() && client.Team != null && client.Player.Map == 700)
            {
                if (!client.Team.Members.ContainsKey(attacked.UID))
                    return true;
                return false;
            }
            if (client.Player.Map == 2057 && !MsgSchedules.CaptureTheFlag.Attackable(attacked))
                return false;
            if (Program.BlockAttackMap.Contains(client.Player.Map) && client.Player.DynamicID == 0)
                return false;
            if (client.Player.DynamicID != 0 && Program.BlockAttackMap.Contains(client.Player.DynamicID))
                return false;
            if (client.Player.PkMode == Role.Flags.PKMode.PK)
            {
                if (client.Player.Map == 1080)
                {
                    if (client.Player.RedTeam && attacked.RedTeam)
                        return false;
                    else if (client.Player.BlueTeam && attacked.BlueTeam)
                        return false;
                }
                else if (client.EventBase != null && client.EventBase.Stage == Game.MsgEvents.EventStage.Fighting)
                {
                    if (!client.EventBase.FriendlyFire)
                        foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> Team in client.EventBase.Teams)
                            if (Team.Value.ContainsKey(client.Player.UID) && Team.Value.ContainsKey(attacked.UID))
                                return false;
                }
            }
            if (client.Player.PkMode == Flags.PKMode.Team)
            {
                if (client.Player.Map == 1080)
                {
                    if (client.Player.RedTeam && attacked.RedTeam)
                        return false;
                    else if (client.Player.BlueTeam && attacked.BlueTeam)
                        return false;
                }
                else if (client.EventBase != null && client.EventBase.Stage == Game.MsgEvents.EventStage.Fighting)
                {
                    if (!client.EventBase.FriendlyFire)
                        foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> Team in client.EventBase.Teams)
                            if (Team.Value.ContainsKey(client.Player.UID) && Team.Value.ContainsKey(attacked.UID))
                                return false;
                }
                if (client.Team != null && client.Team.Members.ContainsKey(attacked.UID))
                    return false;
                if (client.Player.Associate.Contain(1, attacked.UID))
                    return false;
                if (client.Player.MyGuild != null)
                {
                    if (client.Player.GuildID == attacked.GuildID)
                        return false;
                    if (attacked.MyGuild != null && client.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                        return false;
                }
                if (client.Player.MyClan != null)
                {
                    if (client.Player.ClanUID == attacked.ClanUID)
                        return false;
                    if (attacked.MyClan != null && client.Player.MyClan.Ally.ContainsKey(attacked.ClanUID))
                        return false;
                }
            }
            if (client.Player.PkMode != Flags.PKMode.Capture && client.Player.PkMode != Flags.PKMode.Peace)
            {
                if (!attacked.ContainFlag(MsgUpdate.Flags.FlashingName) && !attacked.ContainFlag(MsgUpdate.Flags.RedName) && !attacked.ContainFlag(MsgUpdate.Flags.BlackName) && !Program.FreePkMap.Contains(attacked.Map) && attacked.DynamicID == 0)
                    client.Player.AddFlag(MsgUpdate.Flags.FlashingName, 30, true);
                return true;
            }
            if (client.Player.PkMode == Flags.PKMode.Capture)
            {
                if (attacked.ContainFlag(MsgUpdate.Flags.FlashingName) || attacked.ContainFlag(MsgUpdate.Flags.BlackName))
                    return true;
                return false;
            }
            return true;
        }
    }
}
