﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;

namespace TheChosenProject.Game.MsgServer.AttackHandler
{
    public class Poison
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell = DBSpells[0];

            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                , 0, Attack.X, Attack.Y, (ushort)Role.Flags.SpellID.Poison
                , 0, 0);

            Role.IMapObj target;
            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
            {
                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                {
                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                    {
                        if (attacked.Boss == 1 || attacked.Boss == 9) { return; }
                        MsgSpellAnimation.SpellObj AnimationObj;
                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                        ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                        MsgSpell.Targets.Enqueue(AnimationObj);
                        if (attacked.Alive)
                        {
                            attacked.PoisonLevel = 1;
                            attacked.AddFlag(MsgUpdate.Flags.Poisoned, 15, true, 3);
                        }
                    }
                }
            }
            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
            {
                var attacked = target as Role.Player;
                DBSpell.Range = (byte)(DBSpell.ID == 1000 || DBSpell.ID == 1001 ? 100 : DBSpell.Range);
                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                {
                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                    {
                        MsgSpellAnimation.SpellObj AnimationObj;
                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                        if((AtributesStatus.IsTrojan(user.Player.Class) || AtributesStatus.IsWarrior(user.Player.Class)) && user.Player.BattlePower > attacked.Owner.Player.BattlePower)
                        {
                            AnimationObj.Damage = attacked.Owner.Status.MaxHitpoints;
                        }
                        if (AnimationObj.Damage > 0 && user.Player.BlessTime > 0 && Role.Core.PercentSuccess(Global.LUCKY_TIME_CRIT_RATE_MAGIC))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                AnimationObj.Damage = AnimationObj.Damage * 2;
                                var msg = rec.GetStream();
                                user.SendSysMesage("Lucky Effect, Doubled your damage!", MsgMessage.ChatMode.TopLeft);
                                user.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "LuckyGuy");
                            }
                            //player.Owner.SendSysMesage("Lucky Strike: You had inflict double damage on the target.", MsgMessage.ChatMode.Action);
                        }
                        //if (!AtributesStatus.IsTaoist(user.Player.Class))
                        //{
                        //    AnimationObj.Damage = (attacked.Owner.Status.MaxHitpoints * 50 / 100);
                        //    if (AnimationObj.Damage > 0 && user.Player.BlessTime > 0 && Role.Core.PercentSuccess(Global.LUCKY_TIME_CRIT_RATE_MAGIC))
                        //    {
                        //        AnimationObj.Damage = attacked.Owner.Status.MaxHitpoints;
                        //        using (var rec = new ServerSockets.RecycledPacket())
                        //        {
                        //            var msg = rec.GetStream();
                        //            user.SendSysMesage("Lucky Effect, Doubled your damage!", MsgMessage.ChatMode.TopLeft);
                        //            user.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "LuckyGuy");
                        //        }
                        //        //player.Owner.SendSysMesage("Lucky Strike: You had inflict double damage on the target.", MsgMessage.ChatMode.Action);
                        //    }
                        //}
                        //else
                        //{
                        //    if (AnimationObj.Damage > 0 && user.Player.BlessTime > 0 && Role.Core.PercentSuccess(Global.LUCKY_TIME_CRIT_RATE_MAGIC))
                        //    {
                        //        AnimationObj.Damage *= 2;
                        //        using (var rec = new ServerSockets.RecycledPacket())
                        //        {
                        //            var msg = rec.GetStream();
                        //            user.SendSysMesage("Lucky Effect, Doubled your damage!",MsgMessage.ChatMode.TopLeft);
                        //            user.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "LuckyGuy");
                        //        }
                        //        //player.Owner.SendSysMesage("Lucky Strike: You had inflict double damage on the target.", MsgMessage.ChatMode.Action);
                        //    }
                        //}
                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                        //AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                        MsgSpell.Targets.Enqueue(AnimationObj);
                        if (attacked.Alive)
                        {
                            attacked.PoisonLevel = 0;
                            attacked.AddFlag(MsgUpdate.Flags.Poisoned, 15, true, 3);
                        }
                    }
                }

            }
            else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
            {
                var attacked = target as Role.SobNpc;
                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
                {

                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                    {
                        return;
                        MsgSpellAnimation.SpellObj AnimationObj;
                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                        ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                        MsgSpell.Targets.Enqueue(AnimationObj);
                    }
                }

            }
            MsgSpell.SetStream(stream);
            MsgSpell.Send(user);

        }

        //public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        //{
        //    Database.MagicType.Magic DBSpell = DBSpells[0];

        //    MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
        //        , 0, Attack.X, Attack.Y, (ushort)Role.Flags.SpellID.Poison
        //        , 0, 0);

        //    Role.IMapObj target;
        //    if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
        //    {
        //        MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
        //        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
        //        {
        //            if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
        //            {
        //                MsgSpellAnimation.SpellObj AnimationObj;
        //                Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
        //                ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
        //                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
        //                MsgSpell.Targets.Enqueue(AnimationObj);
        //                if (attacked.Alive)
        //                {
        //                    attacked.PoisonLevel = 1;
        //                    attacked.AddFlag(MsgUpdate.Flags.Poisoned, 15, true, 3);
        //                }
        //            }
        //        }
        //    }
        //    else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
        //    {
        //        var attacked = target as Role.Player;
        //        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
        //        {

        //            if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
        //            {

        //                MsgSpellAnimation.SpellObj AnimationObj;
        //                Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
        //                AnimationObj.Damage = 1;
        //                ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
        //                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
        //                MsgSpell.Targets.Enqueue(AnimationObj);
        //                if (attacked.Alive)
        //                {
        //                    attacked.PoisonLevel = 0;
        //                    attacked.AddFlag(MsgUpdate.Flags.Poisoned, 15, true, 3);
        //                }
        //            }
        //        }

        //    }
        //    else if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
        //    {
        //        var attacked = target as Role.SobNpc;
        //        if (Role.Core.GetDistance(user.Player.X, user.Player.Y, target.X, target.Y) < DBSpell.Range)
        //        {

        //            if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
        //            {
        //                MsgSpellAnimation.SpellObj AnimationObj;
        //                Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
        //                ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
        //                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
        //                MsgSpell.Targets.Enqueue(AnimationObj);
        //            }
        //        }

        //    }
        //    MsgSpell.SetStream(stream);
        //    MsgSpell.Send(user);

        //}
    }
}
