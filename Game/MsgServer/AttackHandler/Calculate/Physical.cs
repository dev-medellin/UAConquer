using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer.AttackHandler.CheckAttack;
using TheChosenProject.Role;

namespace TheChosenProject.Game.MsgServer.AttackHandler.Calculate
{
    public class Physical
    {
        public static void OnMonster(Player player, MonsterRole monster, MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, byte MultipleDamage = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (monster.IsFloor)
            {
                SpellObj.Damage = 2;
                return;
            }
            if (DBSpell == null && Base.Dodged(player.Owner, monster))
            {
                SpellObj.Damage = 0;
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
            ushort rawDefense;
            rawDefense = monster.Family.Defense;
            Damage = Math.Max(0, Damage - rawDefense);
            if (DBSpell != null && DBSpell.Damage < 10f && DBSpell.ID != 10490)
                DBSpell.Damage = 10f;
            if (player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                Damage = ((monster.Family.ID == 4145) ? 10000 : ((monster.Boss == 1) ? Base.MulDiv(Damage, 125, 100) : Base.MulDiv(Damage, 500, 100)));
            else if (MultipleDamage != 0)
            {
                Damage *= MultipleDamage;
            }
            else if (DBSpell != null && DBSpell.ID == 12770)
            {
                Damage = ((monster.Boss != 1) ? Base.MulDiv(Damage, 500, 100) : Base.MulDiv(Damage, 350, 100));
            }
            else if (DBSpell == null || DBSpell.ID != 10490)
            {
                Damage = Base.MulDiv(Damage, (int)(DBSpell?.Damage ?? ((float)ServerKernel.PhysicalDamage)), 100);
            }
            if (player.ContainFlag(MsgUpdate.Flags.Oblivion))
            {
                player.OblivionMobs++;
                Damage = ((monster.Boss != 0) ? Base.MulDiv(Damage, 125, 100) : Base.MulDiv(Damage, 200, 100));
            }
            if (player.OblivionMobs > 32 && player.ContainFlag(MsgUpdate.Flags.Oblivion))
                player.RemoveFlag(MsgUpdate.Flags.Oblivion);
            Damage = Base.AdjustMinDamageUser2Monster(Damage, player.Owner);
            Damage = Base.CalcDamageUser2Monster(Damage, monster.Family.Defense, player.Level, monster.Level, false);
            Damage = (int)Base.BigMulDiv(Damage, monster.Family.Defense2, 10000L);
            if (monster.Family.Defense2 > 0)
                Damage = (int)Base.CalculateExtraAttack((uint)Damage, player.Owner.AjustPhysicalDamageIncrease(), 0);
            SpellObj.Damage = (uint)Math.Max(1, Damage);
            if (monster.Boss == 0)
            {
                if (player.ContainFlag(MsgUpdate.Flags.Superman))
                    SpellObj.Damage *= 10;
            }
            else if (player.ContainFlag(MsgUpdate.Flags.Superman))
            {
                SpellObj.Damage = (uint)((double)SpellObj.Damage * 1.5);
            }

            //if ((monster.Family.Settings & MonsterSettings.Guard) == MonsterSettings.Guard)
            //    SpellObj.Damage /= 10;

            //if (player.Owner.ProjectManager)
            //    SpellObj.Damage = 5000000;
        }

        public static void OnPlayer(Player player, Player target, MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, bool StackOver = false, int IncreaseAttack = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
            {
                SpellObj.Damage = 1;
                return;
            }
            bool update;
            update = false;
            if (DBSpell == null && Base.Dodged(player.Owner, target.Owner))
            {
                SpellObj.Damage = 0;
                return;
            }
            if (DBSpell != null && DBSpell.ID == 10490)
                DBSpell = null;
            int Damage;
            Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);
            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            uint rawDefense;
            rawDefense = target.Owner.AjustDefense;
            //if (target.Reborn == 1)
            //    Damage = (int)Math.Round((double)(Damage * 0.7));
            //else if (target.Reborn == 2)
            //    Damage = (int)Math.Round((double)(Damage * 0.5));
            Damage = ((Damage <= rawDefense) ? 1 : (Damage - (int)rawDefense));
            if (DBSpell != null)
            {
                if (DBSpell.ID == 12080)
                {
                    if (Core.GetDistance(player.X, player.Y, target.X, target.Y) <= 3)
                    {
                        Damage = Base.MulDiv(Damage, 135, 100);
                        update = true;
                    }
                }
                else if (DBSpell.ID == 12290)
                {
                    Damage = Base.MulDiv(Damage, 60, 100);
                    update = true;
                }
                else if (DBSpell.ID == 11050)
                {
                    Damage = Base.MulDiv(Damage, 50, 100);
                    update = true;
                }
            }
            if (!update && DBSpell != null)
            {
                DBSpell.Damage = (byte)(DBSpell.ID == 1000 || DBSpell.ID == 1001 ? 80 : DBSpell.Damage);
                Damage = Base.MulDiv(Damage, (int)(DBSpell?.Damage ?? ((float)ServerKernel.PhysicalDamage)), 100);
            }
            bool onbreak;
            onbreak = false;
            update = false;
            if (player.Owner.Status.CriticalStrike != 0 && !update && Base.GetRefinery(player.Owner.Status.CriticalStrike / 100, target.Owner.Status.Immunity / 100))
            {
                SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                Damage = Base.MulDiv(Damage, 120, 100);
                update = true;
            }
            if (!update && player.Owner.Status.Breakthrough != 0 && target.BattlePower > player.BattlePower && player.Owner.Status.Breakthrough > target.Owner.Status.Counteraction)
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
            double tortise_success = 0.70;
            if ((target.Class >= 21 && target.Class <= 25))
                tortise_success = 1;
            if ((target.Class >= 131 && target.Class <= 135))
                tortise_success = 1.3;
            var TortoisePercent = target.Owner.GemValues(Role.Flags.Gem.NormalTortoiseGem);
            if (TortoisePercent > 0)
                Damage -= (int)(Damage * Math.Min((int)TortoisePercent, 40) * tortise_success / 100);
            if (target.Reborn > 0)
                Damage = (int)Base.BigMulDiv(Damage, 7000L, 10000L);
            byte Bless;
            Bless = 0;
            if (target.Class >= 141 && target.Class <= 145)
                Bless = 10;
            Damage -= (int)(Damage * (target.Owner.Status.ItemBless + Bless) / 100);
            if (!onbreak)
                Damage = Disdain.UserAttackUser(player, target, ref Damage);
            Damage = (int)Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);
            SpellObj.Damage = (uint)Math.Max(1, Damage);
            if (player.Level >= 130 && AtributesStatus.IsWarrior(player.FirstClass) && AtributesStatus.IsWarrior(player.SecondClass) && (ItemType.IsTwoHand(player.Owner.Equipment.RightWeapon) || ItemType.IsOneHand(player.Owner.Equipment.RightWeapon)))
            {
                SpellObj.Damage += (uint)(SpellObj.Damage * 0.5);
            }
            if (player.Level >= 130 && AtributesStatus.IsFire(player.FirstClass) && AtributesStatus.IsFire(player.SecondClass) && ItemType.IsBackSword(player.Owner.Equipment.RightWeapon))
            {
                SpellObj.Damage -= (uint)(SpellObj.Damage * 0.6);
            }
            //SpellObj.Damage = (uint)Base.CalculatePotencyDamage((int)SpellObj.Damage, player.BattlePower, target.BattlePower, true);
            //if (player.Level >= 130 && AtributesStatus.IsWarrior(player.FirstClass) && AtributesStatus.IsWarrior(player.SecondClass))
            //{
            //    SpellObj.Damage += (uint)(SpellObj.Damage * 0.2); ;
            //}
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
            if (BlockRefect.CanUseReflect(player.Owner) && !StackOver && BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out var InRedirect))
                SpellObj = InRedirect;
            if (target.Owner.Equipment.ShieldID != 0)
            {
                double Block;
                Block = target.Owner.Status.Block / 100;
                if (DateTime.Now < target.ShieldBlockEnd)
                    Block += (double)target.ShieldBlockDamage;
                double Change;
                Change = Math.Min(70.0, Block);
                if (Base.Success(Change))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
                    SpellObj.Damage /= 2;
                }
            }
            if (Damage > 0 && player.BlessTime > 0 && Role.Core.PercentSuccess(Global.LUCKY_TIME_CRIT_RATE_MAGIC))
            {
                SpellObj.Damage *= 2;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var msg = rec.GetStream();
                    player.Owner.SendSysMesage("Lucky Effect, Doubled your damage!", MsgMessage.ChatMode.TopLeft);
                    player.SendString(msg, MsgStringPacket.StringID.Effect, true, "LuckyGuy");
                }
                //player.Owner.SendSysMesage("Lucky Strike: You had inflict double damage on the target.", MsgMessage.ChatMode.Action);
            }
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
            //Damage = (int)Base.BigMulDiv(Damage, 500000L, player.Owner.GetDefense2());
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