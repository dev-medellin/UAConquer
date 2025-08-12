using System.Collections.Generic;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.Updates;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class DetachStatus
    {
        public static void Execute(GameClient user, InteractQuery Attack, Packet stream, Dictionary<ushort, MagicType.Magic> DBSpells)
        {
            if (!CanUseSpell.Verified(Attack, user, DBSpells, out var ClientSpell, out var DBSpell))
                return;
            switch (ClientSpell.ID)
            {
                case 6004:
                    {
                        MsgSpellAnimation MsgSpell3;
                        MsgSpell3 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target3, MapObjectType.Player))
                        {
                            Player attacked3;
                            attacked3 = target3 as Player;
                            if (attacked3.ContainFlag(MsgUpdate.Flags.Fly) && CanAttackPlayer.Verified(user, attacked3, DBSpell))
                            {
                                attacked3.RemoveFlag(MsgUpdate.Flags.Fly);
                                MsgSpell3.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked3.UID, 30u, MsgAttackPacket.AttackEffect.None));
                            }
                        }
                        MsgSpell3.SetStream(stream);
                        MsgSpell3.Send(user);
                        UpdateSpell.CheckUpdate(stream, user, Attack, 250u, DBSpells);
                        break;
                    }
                case 1050:
                    if (!user.Player.ContainFlag(MsgUpdate.Flags.XPList) || !AtributesStatus.IsWater(user.Player.Class))
                        break;
                    if (user.IsWatching())
                    { user.SendSysMesage("This spell not work on this map.."); 
                    
                    }
                    //else if (user.Player.NewbieProtection == Flags.NewbieExperience.Disable)
                    //{
                    //    user.SendSysMesage("Newbie~protection~is~enabled,~You~can't~Revive~any~player~as~they~can't~PK~you", MsgMessage.ChatMode.TopLeft);
                    //}
                    else
                    {
                        if (user.Player.Name.Contains("[GM]"))
                            break;
                        MsgSpellAnimation MsgSpell;
                        MsgSpell = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target, MapObjectType.Player))
                        {
                            Player attacked;
                            attacked = target as Player;
                            if (!attacked.Alive)
                            {
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0u, MsgAttackPacket.AttackEffect.None));
                                attacked.Revive(stream);
                                //if (user.Player.Map == 2072 || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && user.Player.Map == 8250))
                                //{
                                //    if (attacked.GuildID == user.Player.GuildID || attacked.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                                //        user.Player.ReviveC++;
                                //}
                                if(user.Player.Map == 1038)
                                {
                                    if (attacked.GuildID == user.Player.GuildID || attacked.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                                    { 
                                        MsgSchedules.GuildWar.AddPoints(user.Player, 1, 0); 
                                    }
                                }
                                if (user.Player.Map == 1125)
                                {
                                    if (attacked.ClanUID == user.Player.ClanUID || attacked.MyClan != null && user.Player.MyClan != null && user.Player.MyClan.Ally.ContainsKey(attacked.ClanUID))
                                        MsgSchedules.ClanWar.AddPoints(user.Player, 1, 0);
                                }
                                if (user.Player.Map == 2072 || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && user.Player.Map == 8250))
                                {
                                    if (attacked.GuildID == user.Player.GuildID || attacked.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                                    { 
                                        MsgSchedules.EliteGuildWar.AddPoints(user.Player, 1, 0); 
                                    }
                                }
                                if (user.Player.Map is 2058)
                                {
                                    if (attacked.GuildID == user.Player.GuildID || attacked.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                                    {
                                        MsgSchedules.CityWar.AddPoints(user.Player, 1, 0);
                                    }
                                }
                                if (user.Player.Map is 19891)
                                {
                                    if (attacked.GuildID == user.Player.GuildID || attacked.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                                    {
                                        MsgSchedules.CitywarTC.AddPoints(user.Player, 1, 0);
                                    }
                                }
                                if (user.Player.Map is 19892)
                                {
                                    if (attacked.GuildID == user.Player.GuildID || attacked.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked.GuildID))
                                    {
                                        MsgSchedules.CitywarPC.AddPoints(user.Player, 1, 0);
                                    }
                                }
                            }
                        }
                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(user);
                        UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                    }
                    break;
                case 1100:
                    {
                        if (user.IsWatching() || user.Player.Map == 700 || user.Player.Map == 1005)
                        {
                            user.SendSysMesage("This spell not work on this map..");
                            break;
                        }
                        //if (user.Player.NewbieProtection == Flags.NewbieExperience.Disable)
                        //{
                        //    user.SendSysMesage("Newbie~protection~is~enabled,~You~can't~Revive~any~player~as~they~can't~PK~you", MsgMessage.ChatMode.TopLeft);
                        //    break;
                        //}
                        MsgSpellAnimation MsgSpell2;
                        MsgSpell2 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target2, MapObjectType.Player))
                        {
                            Player attacked2;
                            attacked2 = target2 as Player;
                            if (attacked2.Alive)
                            {
                                user.Player.Mana += DBSpell.UseMana;
                                break;
                            }
                            MsgSpell2.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked2.UID, 0u, MsgAttackPacket.AttackEffect.None));
                            attacked2.Revive(stream);
                            if (user.Player.Map == 2072 || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && user.Player.Map == 8250))
                            {
                                if (attacked2.GuildID == user.Player.GuildID || attacked2.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked2.GuildID))
                                    MsgSchedules.EliteGuildWar.AddPoints(user.Player, 1, 0);
                            }
                            if (user.Player.Map is 1038)
                            {
                                if (attacked2.GuildID == user.Player.GuildID || attacked2.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked2.GuildID))
                                    MsgSchedules.GuildWar.AddPoints(user.Player, 1, 0);
                            }
                            if (user.Player.Map is 2058)
                            {
                                if (attacked2.GuildID == user.Player.GuildID || attacked2.MyGuild != null && user.Player.MyGuild != null && user.Player.MyGuild.Ally.ContainsKey(attacked2.GuildID))
                                    MsgSchedules.CityWar.AddPoints(user.Player, 1, 0);
                            }
                        }
                        MsgSpell2.SetStream(stream);
                        MsgSpell2.Send(user);
                        UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                        break;
                    }
            }
        }
    }
}
