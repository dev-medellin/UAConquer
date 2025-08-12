using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Role;
namespace TheChosenProject.Game.MsgServer.AttackHandler.Calculate
{
    public static class Base
    {
        public class StatusConstants
        {
            public const int AdjustSet = -30000;

            public const int AdjustFull = -32768;

            public const int AdjustPercent = 30000;

            public const int NAME_GREEN = 1;

            public const int NAME_WHITE = 2;

            public const int NAME_RED = 3;

            public const int NAME_BLACK = 4;
        }

        public static SafeRandom MyRandom = new SafeRandom();

        public static int CutTrail(int x, int y)
        {
            if (x < y)
                return y;
            return x;
        }

        public static uint CalculatePoisonDamage(uint Hitpoints, byte Level)
        {
            Hitpoints = (uint)(Hitpoints * (10 * Math.Min(Level + 1, 5)) / 100);
            if (Hitpoints == 0)
                Hitpoints = 1;
            return Hitpoints;
        }

        public static uint CalculatePoisonDamageFog(uint hitpoints, double percent, int rounds)
        {
            if (hitpoints <= 1 || rounds <= 0 || percent <= 0)
                return 0;

            // Calculate total intended damage (max up to hitpoints - 1)
            double totalDamage = hitpoints * (percent / 100.0);
            totalDamage = Math.Min(totalDamage, hitpoints - 1); // prevent killing the player

            // Split the total damage evenly over the rounds
            uint baseDamage = (uint)(totalDamage / rounds);

            return Math.Max(1, baseDamage);
        }

        public static int MulDiv(int number, int numerator, int denominator)
        {
            return number * numerator / denominator;
        }

        public static int MulDivgem(int number, int numerator, int denominator)
        {
            return (number * numerator + denominator / 2) / denominator;
        }

        public static bool Dodged(GameClient attacker, GameClient target)
        {
            if (attacker == null || target == null)
                return false;
            int atkHit;
            atkHit = attacker.Player.Agility;
            int atkdDodge;
            atkdDodge = (int)target.Status.Dodge;
            int hitRate;
            hitRate = Math.Min(90, atkdDodge - atkHit);
            if (hitRate < 0)
                return false;
            return MyMath.ChanceSuccess(hitRate);
        }

        public static bool Dodged(GameClient attacker, MonsterRole target)
        {
            return false;
        }

        public static int AdjustHitrate(int hitrate, int power)
        {
            int addHitrate;
            addHitrate = 0;
            addHitrate += Math.Max(0, AdjustDataEx(hitrate, power)) - hitrate;
            return hitrate + addHitrate;
        }

        public static int GetNameType(int nAtkerLev, int nMonsterLev)
        {
            int nDeltaLev;
            nDeltaLev = nAtkerLev - nMonsterLev;
            if (nDeltaLev >= 3)
                return 1;
            if (nDeltaLev >= 0)
                return 2;
            if (nDeltaLev >= -5)
                return 3;
            return 4;
        }

        public static int CalcDamageUser2Monster(int nAtk, int nDef, int nAtkLev, int nDefLev, bool Range)
        {
            //if (Range && nAtkLev >= 120)
            //    nAtkLev = 120;
            int nDamage;
            nDamage = nAtk - nDef;
            if (GetNameType(nAtkLev, nDefLev) > 2)
                return Math.Max(0, nDamage);
            int nDeltaLev;
            nDeltaLev = nAtkLev - nDefLev;
            if (nDeltaLev >= 0 && nDeltaLev < 3)
                nAtk = MulDiv(nAtk, 120, 100);
            else if (nDeltaLev >= 3 && nDeltaLev <= 5)
            {
                nAtk = MulDiv(nAtk, 150, 100);
            }
            else if (nDeltaLev > 5 && nDeltaLev <= 10)
            {
                nAtk = MulDiv(nAtk, 200, 100);
            }
            else if (nDeltaLev > 10 && nDeltaLev <= 20)
            {
                nAtk = MulDiv(nAtk, 250, 100);
            }
            else if (nDeltaLev > 20)
            {
                nAtk = MulDiv(nAtk, 300, 100);
            }
            return Math.Max(0, nAtk - nDef);
        }

        public static int AdjustMinDamageUser2Monster(int nDamage, GameClient pAtker)
        {
            int nMinDamage;
            nMinDamage = 1;
            nMinDamage += (int)pAtker.Player.Level / 10;
            nMinDamage += (int)(pAtker.Equipment.LeftWeapon % 10);
            nMinDamage += (int)(pAtker.Equipment.RightWeapon % 10);
            return Math.Max(nMinDamage, nDamage);
        }

        public static long BigMulDiv(long number, long numerator, long denominator)
        {
            return (number * numerator + denominator / 2) / denominator;
        }

        public static int AdjustDataEx(int data, int adjust, int maxData = 0)
        {
            if (adjust >= 30000)
                return MulDiv(data, adjust - 30000, 100);
            if (adjust <= -30000)
                return -1 * adjust + -30000;
            if (adjust == -32768)
                return maxData;
            return data + adjust;
        }

        public static int AdjustAttack(int attack, int power, int defense)
        {
            int addAttack;
            addAttack = 0;
            if (defense > 0)
                addAttack += Math.Max(0, AdjustDataEx(defense, power, 100)) - attack;
            addAttack += Math.Max(0, AdjustDataEx(attack, power)) - attack;
            return attack + addAttack;
        }

        public static int AdjustDefense(int defense, int power, int decrease = 0)
        {
            int addDefense;
            addDefense = 0;
            addDefense += Math.Max(0, AdjustDataEx(defense, power, decrease)) - defense;
            return defense + addDefense;
        }

        public static bool Rate(int value)
        {
            return value > MyRandom.Next() % 100;
        }

        public static bool Success(double Chance)
        {
            return (double)Generate(1, 1000000) / 10000.0 >= 100.0 - Chance;
        }

        public static Int32 Generate(Int32 Min, Int32 Max)
        {
            if (Max != Int32.MaxValue)
                Max++;

            Int32 Value = 0;
            /*lock (Rand) { */
            Value = Program.LiteRandom.Next(Min, Max); /*}*/
            return Value;
        }

        public static bool GetRefinery(uint attacker, uint Attacked)
        {
            if (attacker <= Attacked)
                return false;
            return Success((int)(attacker - Attacked));
        }

        public static bool GetRefinery()
        {
            return Rate(40);
        }

        public static uint MathMin(uint val1, uint val2)
        {
            if (val1 < val2)
                return val1;
            if (val2 <= val1)
                return val2;
            return 0;
        }

        public static uint MathMax(uint val1, uint val2)
        {
            if (val1 < val2)
                return val2;
            if (val2 <= val1)
                return val1;
            return 0;
        }

        public static short GetDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            short x;
            x = 0;
            short y;
            y = 0;
            if (X >= X2)
                x = (short)(X - X2);
            else if (X2 >= X)
            {
                x = (short)(X2 - X);
            }
            if (Y >= Y2)
                y = (short)(Y - Y2);
            else if (Y2 >= Y)
            {
                y = (short)(Y2 - Y);
            }
            if (x > y)
                return x;
            return y;
        }

        public static uint GetDamage(uint MaxAttack, uint MinAttack)
        {
            if (MaxAttack == MinAttack)
                MaxAttack++;
            if (MaxAttack < MinAttack)
                MaxAttack = MinAttack + 1;
            return (uint)(MinAttack + MyRandom.Next((int)(MaxAttack - MinAttack)) / 2);
        }

        public static uint CalculateBless(uint Damage, uint bless)
        {
            uint BDamage;
            BDamage = Damage * bless / 100;
            Damage = ((Damage <= BDamage) ? 1u : (Damage - BDamage));
            return Damage;
        }

        public static uint CalcaulateDeffence(uint Damage, uint Defense)
        {
            Damage = ((Damage <= Defense) ? 1u : (Damage - Defense));
            return Damage;
        }

        public static uint CalculateExtraAttack(uint Damage, uint pattack, uint pdeffence)
        {
            Damage += pattack;
            Damage = ((Damage <= pdeffence) ? 1u : (Damage - pdeffence));
            return Damage;
        }

        public static uint GetFinalDmg(uint Damage, uint mindmg, uint maxdmag)
        {
            if (Damage > maxdmag)
                Damage = GetDamage(mindmg, maxdmag);
            return Damage;
        }

        public static uint CalculateSoul(uint Damage, byte LevelSoul)
        {
            return Damage;
        }

        public static uint CalculateArtefactsDmg(uint Damage, uint AtackerPercent, uint TargetPercent)
        {
            if (AtackerPercent == TargetPercent)
                return Damage;
            if (AtackerPercent > TargetPercent)
            {
                uint Power;
                Power = AtackerPercent - TargetPercent;
                Damage += (uint)MulDiv((int)Damage, (int)(Power / 100), 100);
            }
            return Damage;
        }

        public static int CalculatePotencyDamage(int Damage, int AttackerBattle, int TargetBattle, bool range = false)
        {
            if (AttackerBattle == TargetBattle)
                return Damage;
            int power = AttackerBattle - TargetBattle;
            if (power != 0)
            {
                power = power * (range ? 10 : 3);
                //3
                if (power > 0)
                {
                    power = Math.Min(60, power);
                }
                else if (power < 0)
                {
                    power = Math.Max(-30, power);
                }

                Damage = Base.MulDiv(Damage, 100 + power, 100);
            }
            return Damage;
        }

        public static int CalculateSoulsDamage(int Damage, int AttackerBattle, int TargetBattle)
        {
            if (AttackerBattle == TargetBattle)
                return Damage;
            int power;
            power = AttackerBattle - TargetBattle;
            if (power != 0)
            {
                if (power > 0)
                    power = Math.Min(60, power);
                else if (power < 0)
                {
                    power = Math.Max(-60, power);
                }
                Damage = MulDiv(Damage, 100 + power, 100);
            }
            return Damage;
        }

        public static uint CalculateHealtDmg(uint Damage, uint MaxHitPoints, uint MinHitPoints)
        {
            if (MaxHitPoints == MinHitPoints)
                return 0;
            if (MaxHitPoints > MinHitPoints)
            {
                uint deference;
                deference = MaxHitPoints - MinHitPoints;
                if (deference >= Damage)
                    return Damage;
                return deference;
            }
            return 0;
        }
    }
}
