using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack;
using TheChosenProject.Game.MsgServer.AttackHandler.Updates;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class Attack
    {
        public static void Execute(GameClient user, InteractQuery Attack, Packet stream, Dictionary<ushort, MagicType.Magic> DBSpells)
        {
            if (!user.Player.ContainFlag(MsgUpdate.Flags.Poisoned) && (user.Equipment.RightWeaponEffect == Flags.ItemEffect.Poison || user.Equipment.LeftWeaponEffect == Flags.ItemEffect.Poison))
                if (user.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.Poison && user.Equipment.LeftWeaponEffect == Role.Flags.ItemEffect.Poison && Base.Success(13.0))
                {
                        user.Player.Owner.SendSysMesage(" You got 15% luck on casting poison.", MsgMessage.ChatMode.Talk);
                        Poison.Execute(user, Attack, stream, DBSpells);
                }
                if(user.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.Poison && user.Equipment.LeftWeaponEffect != Role.Flags.ItemEffect.Poison && Base.Success(8.0)
                || user.Equipment.RightWeaponEffect != Role.Flags.ItemEffect.Poison && user.Equipment.LeftWeaponEffect == Role.Flags.ItemEffect.Poison && Base.Success(8.0))
                {

                        user.Player.Owner.SendSysMesage(" You got 10% luck on casting poison.", MsgMessage.ChatMode.Talk);
                        Poison.Execute(user, Attack, stream, DBSpells);
                }
                //user.Player.Owner.SendSysMesage(" You got 15% luck on casting poison.", MsgMessage.ChatMode.Action);
                //Poison.Execute(user, Attack, stream, DBSpells);
            //if (!user.Player.ContainFlag(MsgUpdate.Flags.Poisoned) && (user.Equipment.RightWeaponEffect == Flags.ItemEffect.Poison || user.Equipment.LeftWeaponEffect == Flags.ItemEffect.Poison) && (Base.Success(20.0) || Attack.SpellID == 3306))
            //    Poison.Execute(user, Attack, stream, DBSpells);
            //else if (user.Equipment.RightWeaponEffect == Flags.ItemEffect.Stigma && Base.Success(20.0))
            //{
            //    MsgSpellAnimation MsgSpell7;
            //    MsgSpell7 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, 1095, 3, 0);
            //    if (!user.Player.ContainFlag(MsgUpdate.Flags.Stigma))
            //    {
            //        user.Player.AddSpellFlag(MsgUpdate.Flags.Stigma, 15, true);
            //        MsgSpell7.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0u, MsgAttackPacket.AttackEffect.None));
            //    }
            //}
            else
            {
                if (!CanUseSpell.Verified(Attack, user, DBSpells, out var ClientSpell, out var DBSpell))
                    return;
                switch (ClientSpell.ID)
                {
                    case 1320:
                        {
                            if (!user.Player.ContainFlag(MsgUpdate.Flags.XPList))
                                return;
                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            MsgSpellAnimation MsgSpell6;
                            MsgSpell6 = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience6;
                            Experience6 = 0u;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target6, MapObjectType.Monster))
                            {
                                MonsterRole attacked18;
                                attacked18 = target6 as MonsterRole;
                                if (CanAttackMonster.Verified(user, attacked18, DBSpell))
                                {
                                    Magic.OnMonster(user.Player, attacked18, DBSpell, out var AnimationObj18);
                                    AnimationObj18.Damage = Base.CalculateSoul(AnimationObj18.Damage, ClientSpell.UseSpellSoul);
                                    Experience6 += Monster.Execute(stream, AnimationObj18, user, attacked18);
                                    MsgSpell6.Targets.Enqueue(AnimationObj18);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target6, MapObjectType.Player))
                            {
                                TheChosenProject.Role.Player attacked17;
                                attacked17 = target6 as TheChosenProject.Role.Player;
                                if (CanAttackPlayer.Verified(user, attacked17, DBSpell))
                                {
                                    Magic.OnPlayer(user.Player, attacked17, DBSpell, out var AnimationObj17);
                                    AnimationObj17.Damage = Base.CalculateSoul(AnimationObj17.Damage, ClientSpell.UseSpellSoul);
                                    TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj17, user, attacked17);
                                    MsgSpell6.Targets.Enqueue(AnimationObj17);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target6, MapObjectType.SobNpc))
                            {
                                SobNpc attacked16;
                                attacked16 = target6 as SobNpc;
                                if (CanAttackNpc.Verified(user, attacked16, DBSpell))
                                {
                                    Magic.OnNpcs(user.Player, attacked16, DBSpell, out var AnimationObj16);
                                    AnimationObj16.Damage = Base.CalculateSoul(AnimationObj16.Damage, ClientSpell.UseSpellSoul);
                                    Experience6 += Npc.Execute(stream, AnimationObj16, user, attacked16);
                                    MsgSpell6.Targets.Enqueue(AnimationObj16);
                                }
                            }
                            IncreaseExperience.Up(stream, user, Experience6);
                            UpdateSpell.CheckUpdate(stream, user, Attack, Experience6, DBSpells);
                            MsgSpell6.SetStream(stream);
                            MsgSpell6.Send(user);
                            return;
                        }
                    case 6000:
                        {
                            MsgSpellAnimation MsgSpell;
                            MsgSpell = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience;
                            Experience = 0u;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target, MapObjectType.Monster))
                            {
                                MonsterRole attacked3;
                                attacked3 = target as MonsterRole;
                                if (CanAttackMonster.Verified(user, attacked3, DBSpell))
                                {
                                    Physical.OnMonster(user.Player, attacked3, DBSpell, out var AnimationObj3, 0);
                                    AnimationObj3.Damage = Base.CalculateSoul(AnimationObj3.Damage, ClientSpell.UseSpellSoul);
                                    Experience += Monster.Execute(stream, AnimationObj3, user, attacked3);
                                    MsgSpell.Targets.Enqueue(AnimationObj3);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, MapObjectType.Player))
                            {
                                TheChosenProject.Role.Player attacked2;
                                attacked2 = target as TheChosenProject.Role.Player;
                                if (CanAttackPlayer.Verified(user, attacked2, DBSpell))
                                {
                                    Physical.OnPlayer(user.Player, attacked2, DBSpell, out var AnimationObj2);
                                    AnimationObj2.Damage = (uint)((double)AnimationObj2.Damage * 0.85);
                                    TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj2, user, attacked2);
                                    MsgSpell.Targets.Enqueue(AnimationObj2);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, MapObjectType.SobNpc))
                            {
                                SobNpc attacked;
                                attacked = target as SobNpc;
                                if (CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    Physical.OnNpcs(user.Player, attacked, DBSpell, out var AnimationObj);
                                    AnimationObj.Damage = Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            IncreaseExperience.Up(stream, user, Experience);
                            UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            return;
                        }
                    case 11030:
                        {
                            MsgSpellAnimation MsgSpell3;
                            MsgSpell3 = new MsgSpellAnimation(user.Player.UID, user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience3;
                            Experience3 = 0u;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target3, MapObjectType.Monster))
                            {
                                MonsterRole attacked9;
                                attacked9 = target3 as MonsterRole;
                                if (CanAttackMonster.Verified(user, attacked9, DBSpell))
                                {
                                    Physical.OnMonster(user.Player, attacked9, DBSpell, out var AnimationObj9, 0);
                                    AnimationObj9.Damage = Base.CalculateSoul(AnimationObj9.Damage, ClientSpell.UseSpellSoul);
                                    Experience3 += Monster.Execute(stream, AnimationObj9, user, attacked9);
                                    MsgSpell3.Targets.Enqueue(AnimationObj9);
                                    UpdateSpell.CheckUpdate(stream, user, Attack, Experience3, DBSpells);
                                    MsgSpell3.SetStream(stream);
                                    MsgSpell3.Send(user);
                                    if (attacked9.BlackSpot && Base.Success(80.0))
                                    {
                                        MsgSpellAnimation RemoveCloudDown2;
                                        RemoveCloudDown2 = new MsgSpellAnimation(user.Player.UID, 0u, user.Player.X, user.Player.Y, 11130, 4, 0);
                                        RemoveCloudDown2.Targets.Enqueue(new MsgSpellAnimation.SpellObj
                                        {
                                            UID = user.Player.UID,
                                            Damage = 11030u,
                                            Hit = 1u
                                        });
                                        RemoveCloudDown2.SetStream(stream);
                                        RemoveCloudDown2.JustMe(user);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target3, MapObjectType.Player))
                            {
                                TheChosenProject.Role.Player attacked8;
                                attacked8 = target3 as TheChosenProject.Role.Player;
                                if (CanAttackPlayer.Verified(user, attacked8, DBSpell))
                                {
                                    Physical.OnPlayer(user.Player, attacked8, DBSpell, out var AnimationObj8);
                                    AnimationObj8.Damage = Base.CalculateSoul(AnimationObj8.Damage, ClientSpell.UseSpellSoul);
                                    TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj8, user, attacked8);
                                    MsgSpell3.Targets.Enqueue(AnimationObj8);
                                    UpdateSpell.CheckUpdate(stream, user, Attack, Experience3, DBSpells);
                                    MsgSpell3.SetStream(stream);
                                    MsgSpell3.Send(user);
                                    if (attacked8.BlackSpot && Base.Success(80.0))
                                    {
                                        MsgSpellAnimation RemoveCloudDown;
                                        RemoveCloudDown = new MsgSpellAnimation(user.Player.UID, 0u, user.Player.X, user.Player.Y, 11130, 4, 0);
                                        RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj
                                        {
                                            UID = user.Player.UID,
                                            Damage = 11030u,
                                            Hit = 1u
                                        });
                                        RemoveCloudDown.SetStream(stream);
                                        RemoveCloudDown.JustMe(user);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target3, MapObjectType.SobNpc))
                            {
                                SobNpc attacked7;
                                attacked7 = target3 as SobNpc;
                                if (CanAttackNpc.Verified(user, attacked7, DBSpell))
                                {
                                    Physical.OnNpcs(user.Player, attacked7, DBSpell, out var AnimationObj7);
                                    AnimationObj7.Damage = Base.CalculateSoul(AnimationObj7.Damage, ClientSpell.UseSpellSoul);
                                    Experience3 += Npc.Execute(stream, AnimationObj7, user, attacked7);
                                    MsgSpell3.Targets.Enqueue(AnimationObj7);
                                    UpdateSpell.CheckUpdate(stream, user, Attack, Experience3, DBSpells);
                                    MsgSpell3.SetStream(stream);
                                    MsgSpell3.Send(user);
                                }
                                IncreaseExperience.Up(stream, user, Experience3);
                            }
                            return;
                        }
                    case (ushort)Role.Flags.SpellID.RapidFire:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);

                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    Console.WriteLine(AnimationObj.Damage);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    AnimationObj.Damage = AnimationObj.Damage * 10 / 100;
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


                            break;
                        }
                    case 1002:
                        {
                            MsgSpellAnimation MsgSpell5;
                            MsgSpell5 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience5;
                            Experience5 = 0u;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target5, MapObjectType.Monster))
                            {
                                MonsterRole attacked15;
                                attacked15 = target5 as MonsterRole;
                                if (CanAttackMonster.Verified(user, attacked15, DBSpell))
                                {
                                    Magic.OnMonster(user.Player, attacked15, DBSpell, out var AnimationObj15);
                                    AnimationObj15.Damage = Base.CalculateSoul(AnimationObj15.Damage, ClientSpell.UseSpellSoul);
                                    Experience5 += Monster.Execute(stream, AnimationObj15, user, attacked15);
                                    MsgSpell5.Targets.Enqueue(AnimationObj15);
                                    if (attacked15.Alive)
                                        MsgAttackPacket.CreateAutoAtack(Attack, user);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target5, MapObjectType.Player))
                            {
                                TheChosenProject.Role.Player attacked14;
                                attacked14 = target5 as TheChosenProject.Role.Player;
                                if (CanAttackPlayer.Verified(user, attacked14, DBSpell))
                                {
                                    Magic.OnPlayer(user.Player, attacked14, DBSpell, out var AnimationObj14);
                                    AnimationObj14.Damage = Base.CalculateSoul(AnimationObj14.Damage, ClientSpell.UseSpellSoul);
                                    TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj14, user, attacked14);
                                    MsgSpell5.Targets.Enqueue(AnimationObj14);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target5, MapObjectType.SobNpc))
                            {
                                SobNpc attacked13;
                                attacked13 = target5 as SobNpc;
                                if (CanAttackNpc.Verified(user, attacked13, DBSpell))
                                {
                                    Magic.OnNpcs(user.Player, attacked13, DBSpell, out var AnimationObj13);
                                    AnimationObj13.Damage = Base.CalculateSoul(AnimationObj13.Damage, ClientSpell.UseSpellSoul);
                                    Experience5 += Npc.Execute(stream, AnimationObj13, user, attacked13);
                                    MsgSpell5.Targets.Enqueue(AnimationObj13);
                                    MsgAttackPacket.CreateAutoAtack(Attack, user);
                                }
                            }
                            IncreaseExperience.Up(stream, user, Experience5);
                            UpdateSpell.CheckUpdate(stream, user, Attack, Experience5, DBSpells);
                            MsgSpell5.SetStream(stream);
                            MsgSpell5.Send(user);
                            return;
                        }
                    default:
                        {
                            bool IncreaseDamage;
                            IncreaseDamage = false;
                            if (ItemType.IsTwoHand(user.Equipment.RightWeapon) || !AtributesStatus.IsTaoist(user.Player.Class))
                            {
                                if (ItemType.IsBackSword(user.Equipment.RightWeapon) & ItemType.IsShield(user.Equipment.LeftWeapon))
                                    IncreaseDamage = true;
                                else if (ItemType.IsBackSword(user.Equipment.RightWeapon) && !AtributesStatus.IsTaoist(user.Player.Class))
                                {
                                    IncreaseDamage = true;
                                }
                            }
                            MsgSpellAnimation MsgSpell4;
                            MsgSpell4 = new MsgSpellAnimation(user.Player.UID, 0u, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            uint Experience4;
                            Experience4 = 0u;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out var target4, MapObjectType.Monster))
                            {
                                MonsterRole attacked12;
                                attacked12 = target4 as MonsterRole;
                                if (CanAttackMonster.Verified(user, attacked12, DBSpell))
                                {
                                    Magic.OnMonster(user.Player, attacked12, DBSpell, out var AnimationObj12);
                                    AnimationObj12.Damage = Base.CalculateSoul(AnimationObj12.Damage, ClientSpell.UseSpellSoul);
                                    if (IncreaseDamage)
                                        AnimationObj12.Damage = (uint)Core.Random.Next(1, 1000);
                                    Experience4 += Monster.Execute(stream, AnimationObj12, user, attacked12);
                                    MsgSpell4.Targets.Enqueue(AnimationObj12);
                                    if ((ClientSpell.ID == 1000 || ClientSpell.ID == 1002) && attacked12.Alive)
                                        MsgAttackPacket.CreateAutoAtack(Attack, user);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target4, MapObjectType.Player))
                            {
                                TheChosenProject.Role.Player attacked11;
                                attacked11 = target4 as TheChosenProject.Role.Player;
                                if (CanAttackPlayer.Verified(user, attacked11, DBSpell))
                                {
                                    Magic.OnPlayer(user.Player, attacked11, DBSpell, out var AnimationObj11);
                                    AnimationObj11.Damage = Base.CalculateSoul(AnimationObj11.Damage, ClientSpell.UseSpellSoul);
                                    if (IncreaseDamage)
                                        AnimationObj11.Damage = (uint)Core.Random.Next(1, 1000);
                                    TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj11, user, attacked11);
                                    MsgSpell4.Targets.Enqueue(AnimationObj11);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target4, MapObjectType.SobNpc))
                            {
                                SobNpc attacked10;
                                attacked10 = target4 as SobNpc;
                                if (CanAttackNpc.Verified(user, attacked10, DBSpell))
                                {
                                    Magic.OnNpcs(user.Player, attacked10, DBSpell, out var AnimationObj10);
                                    AnimationObj10.Damage = Base.CalculateSoul(AnimationObj10.Damage, ClientSpell.UseSpellSoul);
                                    if (IncreaseDamage)
                                        AnimationObj10.Damage = (uint)Core.Random.Next(1, 1000);
                                    Experience4 += Npc.Execute(stream, AnimationObj10, user, attacked10);
                                    MsgSpell4.Targets.Enqueue(AnimationObj10);
                                    if (ClientSpell.ID == 1001 || ClientSpell.ID == 1000)
                                        MsgAttackPacket.CreateAutoAtack(Attack, user);
                                }
                            }
                            IncreaseExperience.Up(stream, user, Experience4);
                            UpdateSpell.CheckUpdate(stream, user, Attack, Experience4, DBSpells);
                            MsgSpell4.SetStream(stream);
                            MsgSpell4.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
