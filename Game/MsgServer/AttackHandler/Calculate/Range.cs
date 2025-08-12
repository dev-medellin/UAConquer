using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Role;

namespace TheChosenProject.Game.MsgServer.AttackHandler.Calculate
{
    public class Range
    {
        public static void OnMonster(Player player, MonsterRole monster, MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, byte MultipleDamage = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (monster.IsFloor)
            {
                SpellObj.Damage = 1;
                return;
            }
            if (monster.Map == 3845)
            {
                if (monster.isTargetToSolomon)
                    SpellObj.Damage = 2000;
                else SpellObj.Damage = 1;
                return;
            }
            int Damage;
            Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);
            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);
            if (player.Level > monster.Level)
                Damage *= 2;
            if (MultipleDamage != 0)
                Damage *= MultipleDamage;
            Damage = ((DBSpell == null) ? Base.MulDiv(Damage, (int)(DBSpell?.Damage ?? ((float)ServerKernel.PhysicalDamage)), 100) : Base.MulDiv(Damage, (int)(DBSpell?.Damage ?? ((float)ServerKernel.PhysicalDamage)), 100));
            ushort rawDefense;
            rawDefense = monster.Family.Defense;
            Damage = Math.Max(0, Damage - rawDefense);
            Damage = (int)Base.BigMulDiv(Damage, monster.Family.Defense2, 10000L);
            Damage = Base.MulDiv(Damage, 100 - (int)((double)(int)monster.Family.Dodge * 0.4), 100);
            Damage = Base.CalcDamageUser2Monster(Damage, monster.Family.Defense, player.Level, monster.Level, false);
            Damage = Base.AdjustMinDamageUser2Monster(Damage, player.Owner);
            Damage = (int)Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, 0);
            if (monster.Family.Defense2 == 0)
                Damage = 1;
            SpellObj.Damage = (uint)Math.Max(1, Damage);
            if (monster.Boss == 0 && player.ContainFlag(MsgUpdate.Flags.Superman))
                SpellObj.Damage *= 10;
            if (Base.GetRefinery() && player.Owner.Status.CriticalStrike != 0)
            {
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                SpellObj.Damage += SpellObj.Damage * (player.Owner.AjustCriticalStrike() / 100) / 100;
            }

        }

        public static void OnPlayer(Player player, Player target, MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, int increasedmg = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
            {
                SpellObj.Damage = 1;
                return;
            }
            if (DBSpell == null && Base.Dodged(player.Owner, target.Owner))
            {
                SpellObj.Damage = 0;
                return;
            }
            int Damage;
            Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);
            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            uint rawDefense;
            rawDefense = target.Owner.AjustDefense;
            Damage = ((Damage <= rawDefense) ? 1 : (Damage - (int)rawDefense));
            if (0 == 0)
                Damage = Base.MulDiv(Damage, (int)(DBSpell?.Damage ?? ((float)ServerKernel.PhysicalDamage)), 100);
            if (target.Owner.GemValues(Flags.Gem.SuperTortoiseGem) != 0)
            {
                int reduction;
                reduction = Base.MulDiv((int)target.Owner.GemValues(Flags.Gem.SuperTortoiseGem), 50, 100);
                Damage = Base.MulDiv(Damage, 100 - Math.Min(67, reduction), 100);
            }
            if (target.ContainFlag(MsgUpdate.Flags.Dodge))
            {
                Damage = Damage * (int)(110 - target.Owner.Status.Dodge) / 100;
            }
            Damage = Base.MulDiv(Damage, 50, 100);
            Damage = (int)Base.BigMulDiv(Damage, player.Owner.GetDefense2(), 10000L);
            bool onbreak;
            onbreak = false;
            if (player.Owner.Status.CriticalStrike != 0 && Base.GetRefinery(player.Owner.Status.CriticalStrike / 100, target.Owner.Status.Immunity / 100))
            {
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                Damage = Base.MulDiv(Damage, 120, 100);
            }
            if (player.Owner.Status.Breakthrough != 0 && player.BattlePower > target.BattlePower && player.Owner.Status.Breakthrough > target.Owner.Status.Counteraction)
            {
                double Power;
                Power = player.Owner.Status.Breakthrough - target.Owner.Status.Counteraction;
                Power /= 10.0;
                if (Base.Success(Power))
                {
                    onbreak = true;
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Break;
                }
            }
            if (!onbreak && !player.Owner.InSkillTeamPk())
                Damage = Base.CalculatePotencyDamage(Damage, player.BattlePower, target.BattlePower, true);
            Damage = (int)Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);
            SpellObj.Damage = (uint)Math.Max(1, Damage);
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
            {
                if (SpellObj.Damage > target.AzureShieldDefence)
                {
                    AzureShield.CreateDmg(player, target, target.AzureShieldDefence);
                    target.RemoveFlag(MsgUpdate.Flags.AzureShield);
                    SpellObj.Damage -= target.AzureShieldDefence;
                }
                else
                {
                    target.AzureShieldDefence -= (ushort)SpellObj.Damage;
                    AzureShield.CreateDmg(player, target, SpellObj.Damage);
                    SpellObj.Damage = 1;
                }
            }
            if (target.ContainFlag(MsgUpdate.Flags.DefensiveStance))
            {
                SpellObj.Damage = Base.CalculateBless(SpellObj.Damage, 40);
                SpellObj.Effect = MsgAttackPacket.AttackEffect.Block;
                return;
            }
            if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out var InRedirect))
                SpellObj = InRedirect;
            if (Damage > 0 && player.BlessTime > 0 && Role.Core.PercentSuccess(Global.LUCKY_TIME_CRIT_RATE_MAGIC))
            {
                Damage *= 2;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var msg = rec.GetStream();
                    player.Owner.SendSysMesage("Lucky Effect, Doubled your damage!", MsgMessage.ChatMode.TopLeft);
                    player.SendString(msg, MsgStringPacket.StringID.Effect, true, "LuckyGuy");
                }
                //player.Owner.SendSysMesage("Lucky Strike: You had inflict double damage on the target.", MsgMessage.ChatMode.Action);
            }
            //if (target.Owner.Equipment.ShieldID != 0)
            //{
            //    double Block;
            //    Block = target.Owner.Status.Block / 100;
            //    if (DateTime.Now < target.ShieldBlockEnd)
            //        Block += (double)(target.ShieldBlockDamage / 100);
            //    double Change;
            //    Change = Math.Min(70.0, Block);
            //    if (Base.Success(Change))
            //    {
            //        SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
            //        SpellObj.Damage /= 2;
            //    }
            //}
            if (player.Owner.ArenaStatistic.InDuelRoom)
                SpellObj.Damage = 1;
            #region ExtraPanleDamge
            if (DBSpell != null)
            {
                Rayzo_Panle.ExtraPanleDamge(DBSpell, SpellObj);
            }
            #endregion
        }

        public static void OnNpcs(Player player, SobNpc target, MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            int Damage;
            Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);
            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            Damage = Base.MulDiv(Damage, (int)(DBSpell?.Damage ?? ((float)ServerKernel.PhysicalDamage)), 100);
            //Damage = (int)Base.BigMulDiv(Damage, 10000000L, player.Owner.GetDefense2());
            SpellObj.Damage = (uint)Math.Max(1, Damage);
            if (Base.GetRefinery() && player.Owner.Status.CriticalStrike != 0)
            {
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                SpellObj.Damage = Base.CalculateArtefactsDmg(SpellObj.Damage, player.Owner.Status.CriticalStrike, 0);
            }
            SpellObj.Damage = Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.PhysicalDamageIncrease, 0);
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
                SpellObj.Damage = 100;
        }
    }
}