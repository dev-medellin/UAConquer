using System;
using System.Collections.Generic;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.Updates;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class AttachStatus
    {
        public unsafe static void Execute(GameClient user, InteractQuery Attack, Packet stream, Dictionary<ushort, MagicType.Magic> DBSpells)
        {
            if (!CanUseSpell.Verified(Attack, user, DBSpells, out var ClientSpell, out var DBSpell))
                return;
            //if (ClientSpell.ID != 9876)
            //{
            //    user.SendSysMesage("You can't use skill on this map!");
            //    return;
            //}
            switch (ClientSpell.ID)
            {
                case 9000:
                    Attack.SpellID = ClientSpell.ID;
                    Attack.SpellLevel = ClientSpell.Level;
                    user.Player.View.SendView(stream.InteractionCreate(&Attack), true);
                    user.Player.IntensifyStamp = Time32.Now;
                    user.Player.InUseIntensify = true;
                    user.Player.IntensifyDamage = (int)DBSpell.Damage;
                    UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                    break;
                case 11160:
                    {
                        if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                            user.Player.RemoveFlag(MsgUpdate.Flags.Ride);
                        MsgSpellAnimation MsgSpell2;
                        MsgSpell2 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        if (!user.Player.RemoveFlag(MsgUpdate.Flags.DefensiveStance))
                        {
                            user.Player.AddFlag(MsgUpdate.Flags.DefensiveStance, (int)DBSpell.Duration, false);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.DefensiveStance, DBSpell.Duration, (uint)DBSpell.Damage, ClientSpell.Level, MsgUpdate.DataType.DefensiveStance, true);
                        }
                        else
                            user.Player.RemoveFlag(MsgUpdate.Flags.DefensiveStance);
                        MsgSpell2.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        MsgSpell2.SetStream(stream);
                        MsgSpell2.Send(user);
                        break;
                    }
                case 6002:
                    {
                        MsgSpellAnimation MsgSpell7;
                        MsgSpell7 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target3, MapObjectType.Player))
                        {
                            Player attacked3;
                            attacked3 = target3 as Player;
                            if (!CanAttackPlayer.Verified(user, attacked3, DBSpell))
                                break;
                            int rate2;
                            rate2 = (user.Player.BattlePower - attacked3.BattlePower + 10) * 7;
                            if (attacked3.BattlePower - user.Player.BattlePower >= 10)
                                rate2 = 10;
                            if (user.Player.BattlePower >= attacked3.BattlePower)
                                rate2 = 100;
                            if (Base.Success(rate2))
                            {
                                attacked3.AddSpellFlag(MsgUpdate.Flags.PoisonStar, (int)DBSpell.Duration, true);
                                MsgSpell7.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked3.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                            }
                            else
                            {
                                MsgSpellAnimation.SpellObj clientobj;
                                clientobj = new MsgSpellAnimation.SpellObj(attacked3.UID, MsgSpell7.SpellID, MsgAttackPacket.AttackEffect.None)
                                {
                                    Hit = 0u
                                };
                                MsgSpell7.Targets.Enqueue(clientobj);
                            }
                        }
                        MsgSpell7.SetStream(stream);
                        MsgSpell7.Send(user);
                        UpdateSpell.CheckUpdate(stream, user, Attack, 250u, DBSpells);
                        break;
                    }
                case 1095:
                    {
                        MsgSpellAnimation MsgSpell9;
                        MsgSpell9 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        IMapObj target4;
                        if (user.Player.UID == Attack.OpponentUID)
                        {
                            user.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Invisibility) || user.Player.ContainFlag(MsgUpdate.Flags.Invisible))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.Invisibility);
                                MsgSpell9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            user.Player.Owner.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, you will receive additional 30% of your physical damage");
                            MsgSpell9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            //user.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                            //MsgSpell9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target4, MapObjectType.Monster))
                        {
                            MonsterRole attacked5;
                            attacked5 = target4 as MonsterRole;
                            attacked5.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                            MsgSpell9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked5.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target4, MapObjectType.Player))
                        {
                            Player attacked4;
                            attacked4 = target4 as Player;
                            attacked4.AddSpellFlag(MsgUpdate.Flags.Stigma, (int)DBSpell.Duration, true);
                            if (attacked4.ContainFlag(MsgUpdate.Flags.Invisibility))
                            {
                                attacked4.RemoveFlag(MsgUpdate.Flags.Invisibility);
                                MsgSpell9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked4.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            attacked4.Owner.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, you will receive additional 30% of your physical damage");
                            MsgSpell9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked4.UID, 0, MsgAttackPacket.AttackEffect.None));
                            //MsgSpell9.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked4.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                        MsgSpell9.SetStream(stream);
                        MsgSpell9.Send(user);
                        break;
                    }
                case 1090:
                    {
                        MsgSpellAnimation MsgSpell12;
                        MsgSpell12 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        IMapObj target6;
                        if (user.Player.UID == Attack.OpponentUID)
                        {
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.MagicShield))
                            {
                                user.Player.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)DBSpell.Duration, true);
                                user.Player.Owner.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, your physical barrier increased by 30%.");
                                MsgSpell12.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                            }
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target6, MapObjectType.Monster))
                        {
                            MonsterRole attacked9;
                            attacked9 = target6 as MonsterRole;
                            if (!attacked9.ContainFlag(MsgUpdate.Flags.MagicShield))
                            {
                                attacked9.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)DBSpell.Duration, true);
                                attacked9.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, your physical barrier increased by 30%.");
                                MsgSpell12.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked9.UID, 0u, MsgAttackPacket.AttackEffect.None));
                            }
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target6, MapObjectType.Player))
                        {
                            Player attacked8;
                            attacked8 = target6 as Player;
                            if (!attacked8.ContainFlag(MsgUpdate.Flags.MagicShield))
                            {
                                attacked8.AddSpellFlag(MsgUpdate.Flags.MagicShield, (int)DBSpell.Duration, true);
                                MsgSpell12.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked8.UID, 0u, MsgAttackPacket.AttackEffect.None));
                            }
                        }
                        UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                        MsgSpell12.SetStream(stream);
                        MsgSpell12.Send(user);
                        break;
                    }
                case 10405:
                    {
                        MsgSpellAnimation MsgSpell;
                        MsgSpell = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        bool pass;
                        pass = false;
                        if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target, MapObjectType.Player))
                        {
                            if (target.Alive || user.Player.Map == 1005 || user.Player.Map == 700 || (MsgSchedules.CurrentTournament.Type == TournamentType.TreasureThief && MsgSchedules.CurrentTournament.InTournament(user) && MsgSchedules.CurrentTournament.Process != ProcesType.Alive)|| (MsgSchedules.CurrentTournament.Type == TournamentType.FindTheBox && MsgSchedules.CurrentTournament.InTournament(user) && MsgSchedules.CurrentTournament.Process != ProcesType.Alive))
                                break;
                            Player attacked;
                            attacked = target as Player;
                            if (!attacked.ContainFlag(MsgUpdate.Flags.SoulShackle) && DateTime.Now > attacked.RemovedShackle.AddSeconds(2.0))
                            {
                                int rate;
                                rate = (user.Player.BattlePower - attacked.BattlePower + 10) * 7;
                                if (attacked.BattlePower - user.Player.BattlePower >= 10)
                                {
                                    user.SendSysMesage("Your BattlePower is lower than your opponent get a better BattlePower to be able to use this skill on him .");
                                    break;
                                }
                                if (user.Player.BattlePower >= attacked.BattlePower)
                                    rate = 100;
                                if (rate == 100 || Core.Rate(rate))
                                {
                                    attacked.SendUpdate(stream, MsgUpdate.Flags.SoulShackle, DBSpell.Duration, 0u, ClientSpell.Level, MsgUpdate.DataType.SoulShackle);
                                    attacked.AddSpellFlag(MsgUpdate.Flags.SoulShackle, (int)DBSpell.Duration, true);
                                    if (MsgSchedules.CaptureTheFlag.Proces == ProcesType.Alive && user.Player.Map == 2057)
                                        user.Player.MyGuildMember.CTF_Exploits++;
                                    pass = true;
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0u, MsgAttackPacket.AttackEffect.None));
                                }
                                else
                                    user.SendSysMesage("[Miss] You failed to shackle him due to BP difference.", MsgMessage.ChatMode.TopLeft);
                            }
                        }
                        if (pass)
                        {
                            UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                        }
                        break;
                    }
                case 1085:
                    {
                        MsgSpellAnimation MsgSpell11;
                        MsgSpell11 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        IMapObj target5;
                        if (user.Player.UID == Attack.OpponentUID)
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Invisibility))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.Invisibility);
                                MsgSpell11.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            if (!user.Player.Owner.Player.OnAccuracy)
                            {
                                user.Player.Owner.Player.OnAccuracy = true;
                                user.Player.Owner.Player.OnAccuracyStamp = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);
                                user.Player.Owner.Status.Accuracy += (uint)DBSpell.Rate;
                                user.Player.Owner.Player.HitRateAcc = (uint)DBSpell.Rate;
                                user.Player.Owner.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, your hit rate accuracy will increase by " + (int)DBSpell.Rate);
                            }
                            user.Player.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                            MsgSpell11.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target5, MapObjectType.Monster))
                        {
                            MonsterRole attacked7;
                            attacked7 = target5 as MonsterRole;
                            attacked7.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                            MsgSpell11.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked7.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target5, MapObjectType.Player))
                        {
                            Player attacked6;
                            attacked6 = target5 as Player;
                            if (!attacked6.Owner.Player.OnAccuracy)
                            {
                                attacked6.Owner.Player.OnAccuracy = true;
                                attacked6.Owner.Player.OnAccuracyStamp = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);
                                attacked6.Owner.Status.Accuracy += (uint)DBSpell.Rate;
                                attacked6.Owner.Player.HitRateAcc = (uint)DBSpell.Rate;
                                attacked6.Owner.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, your hit rate accuracy will increase by " + (int)DBSpell.Rate);
                            }
                            attacked6.AddSpellFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                            MsgSpell11.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked6.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                        MsgSpell11.SetStream(stream);
                        MsgSpell11.Send(user);
                        break;
                    }
                case 1075:
                    {
                        MsgSpellAnimation MsgSpell14;
                        MsgSpell14 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        IMapObj target7;
                        if (user.Player.UID == Attack.OpponentUID)
                        {
                            user.Player.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Stigma))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.Stigma);
                                user.Player.Owner.SendSysMesage("Additional damage is removed due to invisibility");
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Dodge))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.Dodge);
                                user.Player.Owner.SendSysMesage("Dodge~has~been~removed~due~to~invisibility.");
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.MagicShield))
                            {
                                user.Player.RemoveFlag(MsgUpdate.Flags.MagicShield);
                                user.Player.Owner.SendSysMesage("MagicShield~has~been~removed~due~to~invisibility.");
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            if (user.Player.ContainFlag(MsgUpdate.Flags.StarOfAccuracy))
                            {
                                user.Player.Owner.Player.OnAccuracy = false;
                                user.Player.Owner.Status.Accuracy -= (uint)DBSpell.Rate;
                                user.Player.Owner.Player.OnAccuracyStamp = Extensions.Time32.Now;
                                user.Player.Owner.Player.HitRateAcc -= (uint)DBSpell.Rate;
                                user.Player.Owner.SendSysMesage("Hit rate is removed due to invisibility");
                                user.Player.RemoveFlag(MsgUpdate.Flags.StarOfAccuracy);
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            user.Player.Owner.SendSysMesage("You~have~become~invisible;~physical-type~monsters~can~no~longer~detect~you.");
                            MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target7, MapObjectType.Monster))
                        {
                            MonsterRole attacked11;
                            attacked11 = target7 as MonsterRole;
                            attacked11.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                            MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked11.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target7, MapObjectType.Player))
                        {
                            Player attacked10;
                            attacked10 = target7 as Player;
                            attacked10.AddSpellFlag(MsgUpdate.Flags.Invisibility, (int)DBSpell.Duration, true);
                            if (attacked10.ContainFlag(MsgUpdate.Flags.Stigma))
                            {
                                attacked10.RemoveFlag(MsgUpdate.Flags.Stigma);
                                attacked10.Owner.Player.Owner.SendSysMesage("Additional damage is removed due to invisibility");
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked10.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            if (attacked10.ContainFlag(MsgUpdate.Flags.Dodge))
                            {
                                attacked10.RemoveFlag(MsgUpdate.Flags.Dodge);
                                attacked10.Owner.Player.Owner.SendSysMesage("Dodge~has~been~removed~due~to~invisibility.");
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked10.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            if (attacked10.ContainFlag(MsgUpdate.Flags.MagicShield))
                            {
                                attacked10.RemoveFlag(MsgUpdate.Flags.MagicShield);
                                attacked10.Owner.Player.Owner.SendSysMesage("MagicShield~has~been~removed~due~to~invisibility.");
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked10.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            if (attacked10.ContainFlag(MsgUpdate.Flags.StarOfAccuracy))
                            {
                                attacked10.Owner.Player.OnAccuracy = false;
                                attacked10.Owner.Status.Accuracy -= (uint)DBSpell.Rate;
                                attacked10.Owner.Player.OnAccuracyStamp = Extensions.Time32.Now;
                                attacked10.Owner.Player.HitRateAcc -= (uint)DBSpell.Rate;
                                attacked10.Owner.SendSysMesage("Hit rate is removed due to invisibility");
                                attacked10.RemoveFlag(MsgUpdate.Flags.StarOfAccuracy);
                                MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked10.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            attacked10.Owner.SendSysMesage("You~have~become~invisible;~physical-type~monsters~can~no~longer~detect~you.");
                            MsgSpell14.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked10.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                        MsgSpell14.SetStream(stream);
                        MsgSpell14.Send(user);
                        break;
                    }
                case 30000:
                    {
                        MsgSpellAnimation MsgSpell3;
                        MsgSpell3 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        int Time;
                        Time = 15 * ClientSpell.Level + 30;
                        IMapObj target2;
                        if (user.Player.UID == Attack.OpponentUID)
                        {
                            user.Player.AzureShieldLevel = (byte)ClientSpell.Level;
                            user.Player.AzureShieldDefence = (ushort)(3000 * ClientSpell.Level);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.AzureShield, (uint)Time, user.Player.AzureShieldDefence, user.Player.AzureShieldLevel, MsgUpdate.DataType.AzureShield);
                            user.Player.AddSpellFlag(MsgUpdate.Flags.AzureShield, Time, true);
                            MsgSpell3.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target2, MapObjectType.Player))
                        {
                            Player attacked2;
                            attacked2 = target2 as Player;
                            attacked2.AzureShieldLevel = (byte)ClientSpell.Level;
                            attacked2.AzureShieldDefence = (ushort)(3000 * ClientSpell.Level);
                            user.Player.SendUpdate(stream, MsgUpdate.Flags.AzureShield, (uint)Time, user.Player.AzureShieldDefence, user.Player.AzureShieldLevel, MsgUpdate.DataType.AzureShield);
                            attacked2.AddSpellFlag(MsgUpdate.Flags.AzureShield, Time, true);
                            MsgSpell3.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked2.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        }
                        UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                        MsgSpell3.SetStream(stream);
                        MsgSpell3.Send(user);
                        break;
                    }
                case 1020:
                    if (user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                    {
                        MsgSpellAnimation MsgSpell16;
                        MsgSpell16 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        user.Player.AddFlag(MsgUpdate.Flags.MagicShield, (int)DBSpell.Duration, true);
                        MsgSpell16.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                        MsgSpell16.SetStream(stream);
                        MsgSpell16.Send(user);
                    }
                    break;
                case 1015:
                    if (user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                    {
                        MsgSpellAnimation MsgSpell15;
                        MsgSpell15 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        user.Player.AddFlag(MsgUpdate.Flags.StarOfAccuracy, (int)DBSpell.Duration, true);
                        MsgSpell15.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        MsgSpell15.SetStream(stream);
                        MsgSpell15.Send(user);
                    }
                    break;
                case 8002:
                    {
                        if (!user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                            break;
                        MsgSpellAnimation MsgSpell5;
                        MsgSpell5 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        if (user.Player.OnTransform || user.Player.ContainFlag(MsgUpdate.Flags.Ride) || user.Player.ContainFlag(MsgUpdate.Flags.Shield))
                        {
                            user.SendSysMesage("You can't use this skill right now!");
                            break;
                        }
                        if (user.Player.ContainFlag(MsgUpdate.Flags.Cyclone))
                        {
                            user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                        }
                        if (user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                            user.Player.UpdateFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true, 0);
                        else
                            user.Player.AddFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);
                        user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        MsgSpell5.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                        MsgSpell5.SetStream(stream);
                        MsgSpell5.Send(user);
                        break;
                    }
                case 8003:
                    {
                        MsgSpellAnimation MsgSpell6;
                        MsgSpell6 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        if (user.Player.OnTransform || user.Player.ContainFlag(MsgUpdate.Flags.Ride) || user.Player.ContainFlag(MsgUpdate.Flags.Shield))
                        {
                            user.SendSysMesage("You can't use this skill right now!");
                            break;
                        }
                        if (user.Player.ContainFlag(MsgUpdate.Flags.Cyclone))
                        {
                            user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                        }
                        if (user.Player.ContainFlag(MsgUpdate.Flags.Fly))
                            user.Player.UpdateFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true, 0);
                        else
                            user.Player.AddFlag(MsgUpdate.Flags.Fly, (int)DBSpell.Duration, true);
                        MsgSpell6.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, DBSpell.Duration, MsgAttackPacket.AttackEffect.None));
                        MsgSpell6.SetStream(stream);
                        MsgSpell6.Send(user);
                        break;
                    }
                case 9876:
                    {
                        MsgSpellAnimation MsgSpell4;
                        MsgSpell4 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        user.Player.AddFlag(MsgUpdate.Flags.CastPray, 2592000, true);
                        MsgSpell4.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        MsgSpell4.SetStream(stream);
                        MsgSpell4.Send(user);
                        break;
                    }
                case 6011:
                    if (user.Player.Map == 1038 || user.Player.Map == 3868)
                        user.SendSysMesage("You can't use this skill right now!");
                    else if (user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                    {
                        MsgSpellAnimation MsgSpell8;
                        MsgSpell8 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        user.Player.OpenXpSkill(MsgUpdate.Flags.FatalStrike, 60);
                        MsgSpell8.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        MsgSpell8.SetStream(stream);
                        MsgSpell8.Send(user);
                    }
                    break;
                case 1110:
                    if (user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                    {
                        MsgSpellAnimation MsgSpell10;
                        MsgSpell10 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        user.Player.OpenXpSkill(MsgUpdate.Flags.Cyclone, 20);
                        MsgSpell10.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        MsgSpell10.SetStream(stream);
                        MsgSpell10.Send(user);
                    }
                    break;
                case 1025:
                    if (user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                    {
                        MsgSpellAnimation MsgSpell13;
                        MsgSpell13 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                        user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                        user.Player.OpenXpSkill(MsgUpdate.Flags.Superman, 20);
                        MsgSpell13.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
                        MsgSpell13.SetStream(stream);
                        MsgSpell13.Send(user);
                    }
                    break;
                case 3080:
                    {
                        MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                        , 0, Attack.X, Attack.Y, ClientSpell.ID
                        , ClientSpell.Level, ClientSpell.UseSpellSoul);

                        if (user.Player.UID == Attack.OpponentUID)
                        {
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.Dodge))
                            {
                                user.Player.Owner.Player.OnDodge = true;
                                user.Player.Owner.Player.OnDodgeStamp = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);
                                user.Player.Owner.Status.Dodge += 10;
                                user.Player.Owner.Player.HitRateDodge -= 10;
                            }
                            user.Player.AddSpellFlag(MsgUpdate.Flags.Dodge, (int)DBSpell.Duration, true);
                            user.Player.Owner.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, you will receive additional 30% dodge rate increase.");
                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                        }
                        else
                        {
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                attacked.AddSpellFlag(MsgUpdate.Flags.Dodge, (int)DBSpell.Duration, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));
                            }
                            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                Role.Player attacked = target as Role.Player;
                                if (!attacked.Owner.Player.ContainFlag(MsgUpdate.Flags.Dodge))
                                {
                                    attacked.Owner.Player.OnDodge = true;
                                    attacked.Owner.Player.OnDodgeStamp = Extensions.Time32.Now.AddSeconds((int)DBSpell.Duration);
                                    attacked.Owner.Status.Dodge += 10;
                                    attacked.Owner.Player.HitRateDodge -= 10;
                                }
                                attacked.Owner.SendSysMesage("For " + (int)DBSpell.Duration + " seconds, you will receive additional 30% dodge rate increase.");
                                attacked.AddSpellFlag(MsgUpdate.Flags.Dodge, (int)DBSpell.Duration, true);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, 0, MsgAttackPacket.AttackEffect.None));

                            }
                        }

                        Updates.UpdateSpell.CheckUpdate(stream, user, Attack, DBSpell.Duration, DBSpells);
                        MsgSpell.SetStream(stream);
                        MsgSpell.Send(user);
                        break;
                    }
            }
        }
    }
}
