using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;

namespace TheChosenProject.Database
{
    //public class Disdain
    //{
    //    public class DisdainItem
    //    {
    //        public int type;

    //        public int delta_lev;

    //        public int usr_atk_mst;

    //        public int mst_atk;

    //        public int max_atk;

    //        public int max_xp_atk;

    //        public int exp_factor;

    //        public int xp_exp_factor;

    //        public int usr_atk_usr_min;

    //        public int usr_atk_usr_max;

    //        public int usr_atk_usr_overadj;

    //        public int usr_atk_usrx_min;

    //        public int usr_atk_usrx_max;

    //        public int usr_atk_usrx_overadj;

    //        public int usrx_atk_usr_min;

    //        public int usrx_atk_usr_max;

    //        public int usrx_atk_usr_overadj;

    //        public int usrx_atk_usrx_min;

    //        public int usrx_atk_usrx_max;

    //        public int usrx_atk_usrx_overadj;
    //    }

    //    public static Dictionary<int, DisdainItem> DisdainRecords = new Dictionary<int, DisdainItem>();

    //    private static int[] NonrebornInts = new int[5] { 10, 6, 6, 6, 6 };

    //    private static int[] RebornInts = new int[5] { 18, 18, 14, 27, 18 };

    //    public static void Load()
    //    {
    //        string[] baseText;
    //        baseText = File.ReadAllLines(ServerKernel.CO2FOLDER + "cq_disdain.txt");
    //        string[] array;
    //        array = baseText;
    //        foreach (string bas_line in array)
    //        {
    //            string[] line;
    //            line = bas_line.Split(',');
    //            DisdainItem obj;
    //            obj = new DisdainItem
    //            {
    //                type = int.Parse(line[1]),
    //                delta_lev = int.Parse(line[2]),
    //                usr_atk_mst = int.Parse(line[3]),
    //                mst_atk = int.Parse(line[4]),
    //                max_atk = int.Parse(line[5]),
    //                max_xp_atk = int.Parse(line[6]),
    //                exp_factor = int.Parse(line[7]),
    //                xp_exp_factor = int.Parse(line[8]),
    //                usr_atk_usr_min = int.Parse(line[9]),
    //                usr_atk_usr_max = int.Parse(line[10]),
    //                usr_atk_usr_overadj = int.Parse(line[11]),
    //                usr_atk_usrx_min = int.Parse(line[12]),
    //                usr_atk_usrx_max = int.Parse(line[13]),
    //                usr_atk_usrx_overadj = int.Parse(line[14]),
    //                usrx_atk_usr_min = int.Parse(line[15]),
    //                usrx_atk_usr_max = int.Parse(line[16]),
    //                usrx_atk_usr_overadj = int.Parse(line[17]),
    //                usrx_atk_usrx_min = int.Parse(line[18]),
    //                usrx_atk_usrx_max = int.Parse(line[19]),
    //                usrx_atk_usrx_overadj = int.Parse(line[20])
    //            };
    //            DisdainRecords.Add(obj.delta_lev, obj);
    //        }
    //    }

    //    public static DisdainItem LookupDisdain(int attacker, int target)
    //    {
    //        int index;
    //        index = attacker - target;
    //        if (index > 400)
    //            index = 400;
    //        else if (index < -400)
    //        {
    //            index = -400;
    //        }
    //        DisdainRecords.TryGetValue(index, out var record);
    //        return record;
    //    }

    //    public static int UserAttackMonster(Player attacker, MonsterRole target, int damage)
    //    {
    //        DisdainItem disdain;
    //        disdain = LookupDisdain(attacker.BattlePower, target.BattlePower);
    //        int factor;
    //        factor = disdain.max_atk;
    //        if (attacker.OnXPSkill() != MsgUpdate.Flags.Normal)
    //            factor = disdain.max_xp_atk;
    //        int maxDamage;
    //        maxDamage = target.Family.MaxHealth * (factor / 100);
    //        damage = Math.Min(damage, maxDamage);
    //        int extraDelta;
    //        extraDelta = target.BattlePower - attacker.BattlePower;
    //        if (extraDelta > 0)
    //        {
    //            factor = ((extraDelta >= 10) ? 1 : ((extraDelta >= 5) ? 5 : ((extraDelta < 1) ? 100 : 10)));
    //            damage = Base.MulDiv(damage, factor, 100);
    //        }
    //        return damage;
    //    }

    //    public static int MonsterAttackUser(MonsterRole attacker, Player target, int damage)
    //    {
    //        int extraDelta;
    //        extraDelta = target.BattlePower - attacker.BattlePower;
    //        int factor;
    //        factor = ((extraDelta < 5) ? 100 : ((extraDelta < 10) ? 80 : ((extraDelta < 15) ? 60 : ((extraDelta >= 20) ? 30 : 40))));
    //        int adjustDamage;
    //        adjustDamage = Base.MulDiv((int)target.Owner.Status.MaxHitpoints, factor * attacker.ExtraDamage, 1000000);
    //        return Math.Max(adjustDamage, damage);
    //    }

    //    public static int UserAttackUser(Player attacker, Player target, ref int damage)
    //    {
    //        int BP_Diff;
    //        BP_Diff = Math.Min(6, Math.Abs(attacker.BattlePower - target.BattlePower));
    //        if (attacker.BattlePower < target.BattlePower)
    //            damage -= BP_Diff * 1500;
    //        if (damage <= 0)
    //            return 1;
    //        return damage;
    //    }

    //    private static int UserAttackUserGetFactor(Player target)
    //    {
    //        int index;
    //        index = ((!AtributesStatus.IsTrojan(target.Class)) ? (AtributesStatus.IsWarrior(target.Class) ? 1 : (AtributesStatus.IsArcher(target.Class) ? 2 : (((target.Class >= 100 && target.Class <= 102) || AtributesStatus.IsFire(target.Class)) ? 3 : ((!AtributesStatus.IsWater(target.Class)) ? 1 : 4)))) : 0);
    //        if (target.Reborn > 0)
    //            return RebornInts[index];
    //        return NonrebornInts[index];
    //    }
    //}

    public class Disdain
    {
        public class DisdainItem
        {
            public int type;
            public int delta_lev;
            public int usr_atk_mst;
            public int mst_atk;
            public int max_atk;
            public int max_xp_atk;
            public int exp_factor;
            public int xp_exp_factor;
            public int usr_atk_usr_min;
            public int usr_atk_usr_max;
            public int usr_atk_usr_overadj;
            public int usr_atk_usrx_min;
            public int usr_atk_usrx_max;
            public int usr_atk_usrx_overadj;
            public int usrx_atk_usr_min;
            public int usrx_atk_usr_max;
            public int usrx_atk_usr_overadj;
            public int usrx_atk_usrx_min;
            public int usrx_atk_usrx_max;
            public int usrx_atk_usrx_overadj;
        }

        public static Dictionary<int, DisdainItem> DisdainRecords = new Dictionary<int, DisdainItem>();

        public static void Load()
        {
            string[] baseText = File.ReadAllLines(ServerKernel.CO2FOLDER + "cq_disdain.txt");
            foreach (var bas_line in baseText)
            {
                string[] line = bas_line.Split(',');
                DisdainItem obj = new DisdainItem();
                obj.type = int.Parse(line[1]);
                obj.delta_lev = int.Parse(line[2]);
                obj.usr_atk_mst = int.Parse(line[3]);
                obj.mst_atk = int.Parse(line[4]);
                obj.max_atk = int.Parse(line[5]);
                obj.max_xp_atk = int.Parse(line[6]);
                obj.exp_factor = int.Parse(line[7]);
                obj.xp_exp_factor = int.Parse(line[8]);
                obj.usr_atk_usr_min = int.Parse(line[9]);
                obj.usr_atk_usr_max = int.Parse(line[10]);
                obj.usr_atk_usr_overadj = int.Parse(line[11]);
                obj.usr_atk_usrx_min = int.Parse(line[12]);
                obj.usr_atk_usrx_max = int.Parse(line[13]);
                obj.usr_atk_usrx_overadj = int.Parse(line[14]);
                obj.usrx_atk_usr_min = int.Parse(line[15]);
                obj.usrx_atk_usr_max = int.Parse(line[16]);
                obj.usrx_atk_usr_overadj = int.Parse(line[17]);
                obj.usrx_atk_usrx_min = int.Parse(line[18]);
                obj.usrx_atk_usrx_max = int.Parse(line[19]);
                obj.usrx_atk_usrx_overadj = int.Parse(line[20]);
                DisdainRecords.Add(obj.delta_lev, obj);
            }
        }

        public static DisdainItem LookupDisdain(int attacker, int target)
        {
            int index = attacker - target;
            if (index > 400) index = 400;
            else if (index < -400)
                index = -400;

            DisdainItem record = null;
            DisdainRecords.TryGetValue(index, out record);
            return record;
        }

        public static int UserAttackMonster(Role.Player attacker, Game.MsgMonster.MonsterRole target, int damage)
        {
            var disdain = LookupDisdain(attacker.BattlePower, target.BattlePower);
            int factor = disdain.max_atk;
            if (attacker.OnXPSkill() != Game.MsgServer.MsgUpdate.Flags.Normal)
                factor = disdain.max_xp_atk;

            int maxDamage = target.Family.MaxHealth * (factor / 100);
            damage = Math.Min(damage, maxDamage);

            int extraDelta = target.BattlePower - attacker.BattlePower;
            if (extraDelta > 0)
            {
                if (extraDelta >= 10) factor = 1;
                else if (extraDelta >= 5) factor = 5;
                else if (extraDelta >= 1) factor = 10;
                else factor = 100;
                damage = Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(damage, factor, 100);
            }

            return damage;
        }

        public static int MonsterAttackUser(Game.MsgMonster.MonsterRole attacker, Role.Player target, int damage)
        {
            int extraDelta = target.BattlePower - attacker.BattlePower;
            int factor;
            if (extraDelta < 5) factor = 100;
            else if (extraDelta < 10) factor = 80;
            else if (extraDelta < 15) factor = 60;
            else if (extraDelta < 20) factor = 40;
            else factor = 30;

            int adjustDamage = Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)target.Owner.Status.MaxHitpoints, factor * attacker.ExtraDamage, 1000000); // ExtraDamage is the monster's extra_damage field in the database

            return Math.Max(adjustDamage, damage);
        }

        public static int UserAttackUser(Role.Player attacker, Role.Player target, ref int damage)
        {
            var disdain = LookupDisdain(attacker.BattlePower, target.BattlePower);
            int min, max, overAdjust;
            if (attacker.Level < 110)
            {
                if (target.Level < 110)
                {
                    min = disdain.usr_atk_usr_min;
                    max = disdain.usr_atk_usr_max;
                    overAdjust = disdain.usr_atk_usr_overadj;
                }
                else
                {
                    min = disdain.usr_atk_usrx_min;
                    max = disdain.usr_atk_usrx_max;
                    overAdjust = disdain.usr_atk_usrx_overadj;
                }
            }
            else
            {
                if (target.Level < 110)
                {
                    min = disdain.usrx_atk_usr_min;
                    max = disdain.usrx_atk_usr_max;
                    overAdjust = disdain.usrx_atk_usr_overadj;
                }
                else
                {
                    min = disdain.usrx_atk_usrx_min;
                    max = disdain.usrx_atk_usrx_max;
                    overAdjust = disdain.usrx_atk_usrx_overadj;
                }
            }
            int factor = UserAttackUserGetFactor(target);
            int targetLev = target.Level;
            int minDamage = min * targetLev * factor / 100;
            if (damage < minDamage)
                return minDamage;
            int maxDamage = max * targetLev * factor / 100;
            if (damage > maxDamage)
            {
                var nDamage = Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(damage - maxDamage, overAdjust, 100);
                return (nDamage + maxDamage);
            }
            return damage;
        }

        public static int UserAttackUser(int attacker, int target, int damage)
        {
            var disdain = LookupDisdain(attacker, target);

            int min, max, overAdjust;
            {
                min = disdain.usrx_atk_usrx_min;
                max = disdain.usrx_atk_usrx_max;
                overAdjust = disdain.usrx_atk_usrx_overadj;
            }

            int factor = 18;//UserAttackUserGetFactor(target);
            int targetLev = 137;//target.Level;

            int minDamage = min * targetLev * factor / 100;
            if (damage < minDamage)
                return minDamage;
            int maxDamage = max * targetLev * factor / 100;

            if (damage > maxDamage)
            {
                var nDamage = Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(damage - maxDamage, overAdjust, 100);
                return (nDamage + maxDamage);
            }

            return damage;
        }

        private static int[] NonrebornInts = { 10, 6, 6, 6, 6 };
        private static int[] RebornInts = { 18, 18, 14, 27, 18 };

        private static int UserAttackUserGetFactor(Role.Player target)
        {
            int index;
            if (Database.AtributesStatus.IsTrojan(target.Class)) index = 0;
            else if (Database.AtributesStatus.IsWarrior(target.Class)) index = 1;
            else if (Database.AtributesStatus.IsArcher(target.Class)) index = 2;
            else if (target.Class >= 100 && target.Class <= 102 || Database.AtributesStatus.IsFire(target.Class)) index = 3;
            else if (Database.AtributesStatus.IsWater(target.Class)) index = 4;
            else index = 1;

            if (target.Reborn > 0)
                return RebornInts[index];
            else
                return NonrebornInts[index];
        }
    }
}
