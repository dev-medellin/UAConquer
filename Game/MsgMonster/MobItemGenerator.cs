using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Database;
using TheChosenProject.Role;

namespace TheChosenProject.Game.MsgMonster
{
    public class MobRateWatcher
    {
        private int tick;
        private int count;
        public static implicit operator bool(MobRateWatcher q)
        {
            bool result = false;
            q.count++;
            if (q.count == q.tick)
            {
                q.count = 0;
                result = true;
            }
            return result;
        }
        public MobRateWatcher(int Tick)
        {
            tick = Tick;
            count = 0;
        }
    }

    public struct SpecialItemWatcher
    {
        public uint ID;
        public MobRateWatcher Rate;
        public SpecialItemWatcher(uint ID, int Tick)
        {
            this.ID = ID;
            Rate = new MobRateWatcher(Tick);
        }
    }

    public class MobItemGenerator
    {
        private static ushort[] NecklaceType = new ushort[] { 120/*Necklace*/, 121 /*Bag*/};
        private static ushort[] BootsType = new ushort[] { 160 /*Boots*/};
        private static ushort[] RingType = new ushort[] { 150/*ring*/, 151/*heavyring*/, 152 /*brack*/};
        private static ushort[] ArmetType = new ushort[] { 111/*Warrior`sHelmet*/, 113/*Archer`sHat*/, 114/*Taoist`sCap*/, 117/*Earring*/, 118/*Coronet*/ };
        private static ushort[] ArmorType = new ushort[] { 130/*Trojan`sArmor*/, 131/*Warrior`sArmor*/, 133/*Archer`sCoat*/, 134/*Taoist`sRobe*/};
        private static ushort[] OneHanderType = new ushort[] { 410/*blade*/, 420/*sword*/, 421/*BackSword*/, 430/*Hook*/, 440/*Whip*/, 450/*Axe*/, 460/*Hammer*/, 480/*Club*/, 481/*Scepter*/, 490/*Dagger*/ };
        private static ushort[] TwoHanderType = new ushort[] { 510/*Glaive*/, 530/*PoleAxe*/, 560/*Spear*/, 561/*Wand*/, 580/*Halbert*/ , 900/*Sheild*/, 500/*bow*/ };

        private MonsterFamily Family;

        private MobRateWatcher Refined;

        private MobRateWatcher Unique;

        private MobRateWatcher Elite;

        private MobRateWatcher Super;

        private MobRateWatcher DropHp;

        private MobRateWatcher DropMp;
        //private MobRateWatcher PlusOne;
        //private MobRateWatcher PlusTwo;
        public MobItemGenerator(MonsterFamily family)
        {
            Family = family;
            Refined = new MobRateWatcher(10);
            Unique = new MobRateWatcher(15);
            Elite = new MobRateWatcher(20);
            Super = new MobRateWatcher(25);
            DropHp = new MobRateWatcher(50);
            DropMp = new MobRateWatcher(50);
         
        }
       
        public List<uint> Stones(ushort level, int count)
        {
            List<uint> items = new List<uint>();
            if (Database.ItemType.Stones.ContainsKey(level))
            {
                var array = Database.ItemType.Stones[level].Values.ToArray();
                for (int x = 0; x < (int)(count == 0 ? 1 : count); x++)
                {
                    int position = Program.GetRandom.Next(0, array.Length);
                    items.Add(array[position].ID);
                }
            }
            return items;
        }
        //public byte GeneratePurity2()
        //{
        //    if (PlusOne)
        //        return 1;
        //    if (PlusTwo)
        //        return 2;
        //    return 0;
        //}
        //public List<uint> GenerateSoulsItems(byte level, byte count)
        //{
        //    level = (byte)Core.Random.Next(3, level);
        //    List<uint> items;
        //    items = new List<uint>();
        //    if (ItemType.PurificationItems.ContainsKey(level))
        //    {
        //        ItemType.DBItem[] array;
        //        array = ItemType.PurificationItems[level].Values.ToArray();
        //        for (int x = 0; x < ((count == 0) ? 1 : count); x++)
        //        {
        //            int position;
        //            position = ServerKernel.NextAsync(0, array.Length);
        //            items.Add(array[position].ID);
        //        }
        //    }
        //    return items;
        //}

        //public List<uint> GenerateRefineryItems(byte level, byte count)
        //{
        //    level = (byte)Core.Random.Next(1, level);
        //    if (MyMath.Success(0.001))
        //        level = 3;
        //    List<uint> items;
        //    items = new List<uint>();
        //    if (ItemType.Refinary.ContainsKey(level))
        //    {
        //        Rifinery.Item[] array;
        //        array = ItemType.Refinary[level].Values.ToArray();
        //        for (int x = 0; x < ((count == 0) ? 1 : count); x++)
        //        {
        //            int position;
        //            position = ServerKernel.NextAsync(0, array.Length);
        //            items.Add(array[position].ItemID);
        //        }
        //    }
        //    return items;
        //}

        public List<uint> GenerateBossFamily()
        {
            List<uint> Items;
            Items = new List<uint>();
            byte rand;
            rand = (byte)ServerKernel.NextAsync(1, 7);
            for (int x = 0; x < 4; x++)
            {
                byte dwItemQuality;
                dwItemQuality = GenerateQuality();
                uint dwItemSort;
                dwItemSort = 0;
                uint dwItemLev;
                dwItemLev = 0;
                switch (rand)
                {
                    case 1:
                        dwItemSort = NecklaceType[ServerKernel.NextAsync(0, NecklaceType.Length)];
                        dwItemLev = Family.DropNecklace;
                        break;
                    case 2:
                        dwItemSort = RingType[ServerKernel.NextAsync(0, RingType.Length)];
                        dwItemLev = Family.DropRing;
                        break;
                    case 3:
                        dwItemSort = ArmorType[ServerKernel.NextAsync(0, ArmorType.Length)];
                        dwItemLev = Family.DropArmor;
                        break;
                    case 4:
                        dwItemSort = TwoHanderType[ServerKernel.NextAsync(0, TwoHanderType.Length)];
                        dwItemLev = ((dwItemSort == 900) ? Family.DropShield : Family.DropWeapon);
                        break;
                    default:
                        dwItemSort = OneHanderType[ServerKernel.NextAsync(0, OneHanderType.Length)];
                        dwItemLev = Family.DropWeapon;
                        break;
                }
                dwItemLev = AlterItemLevel(dwItemLev, dwItemSort);
                uint idItemType;
                idItemType = dwItemSort * 1000 + dwItemLev * 10 + dwItemQuality;
                if (Server.ItemsBase.ContainsKey(idItemType))
                    Items.Add(idItemType);
            }
            return Items;
        }

        public uint GenerateItemId(uint map, out byte dwItemQuality, out bool Special, out ItemType.DBItem DbItem)
        {
            Special = false;
            //SpecialItemWatcher[] dropSpecials;
            //dropSpecials = Family.DropSpecials;
            //for (int i = 0; i < dropSpecials.Length; i++)
            //{
            //    SpecialItemWatcher sp;
            //    sp = dropSpecials[i];
            //    if ((bool)sp.Rate)
            //    {
            //        Special = true;
            //        dwItemQuality = (byte)(sp.ID % 10);
            //        if (Server.ItemsBase.TryGetValue(sp.ID, out DbItem))
            //            return sp.ID;
            //    }
            //}
            //if ((bool)DropHp)
            //{
            //    dwItemQuality = 0;
            //    Special = true;
            //    if (Server.ItemsBase.TryGetValue(Family.DropHPItem, out DbItem))
            //        return Family.DropHPItem;
            //}
            //if ((bool)DropMp)
            //{
            //    dwItemQuality = 0;
            //    Special = true;
            //    if (Server.ItemsBase.TryGetValue(Family.DropMPItem, out DbItem))
            //        return Family.DropMPItem;
            //}
            if (Role.Core.Rate(0.001))
            {
                if (Role.Core.Rate(0.01))
                    dwItemQuality = 8;
                else
                    dwItemQuality = GenerateQuality();
            }
            else
            {
                dwItemQuality = 3;//normal
            }
            
            uint dwItemSort = 0;
            uint dwItemLev = 0;

            int nRand = Program.GetRandom.Next(0, 1200);// % 100;
            if (nRand >= 10 && nRand < 20) // 0.17%
            {
                dwItemSort = 160;
                dwItemLev = Family.DropBoots;
            }
            else if (nRand >= 20 && nRand < 50) // 0.25%
            {
                dwItemSort = NecklaceType[Program.GetRandom.Next(0, NecklaceType.Length)];
                dwItemLev = Family.DropNecklace;
            }
            else if (nRand >= 50 && nRand < 100) // 4.17%
            {
                dwItemSort = RingType[Program.GetRandom.Next(0, RingType.Length)];
                dwItemLev = Family.DropRing;
            }
            else if (nRand >= 100 && nRand < 400) // 25%
            {
                dwItemSort = ArmetType[Program.GetRandom.Next(0, ArmetType.Length)];
                dwItemLev = Family.DropArmet;
            }
            else if (nRand >= 400 && nRand < 700) // 25%
            {
                dwItemSort = ArmorType[Program.GetRandom.Next(0, ArmorType.Length)];
                dwItemLev = Family.DropArmor;
            }
            else // 45%
            {
                int nRate = Program.GetRandom.Next(0, 1000) % 100;
                if (nRate >= 10 && nRate < 20) // 20% of 45% (= 9%) - Backswords
                {
                    dwItemSort = 421;
                }
                else if (nRate >= 40 && nRate < 80)	// 40% of 45% (= 18%) - One handers
                {
                    dwItemSort = OneHanderType[Program.GetRandom.Next(0, OneHanderType.Length)];
                    dwItemLev = Family.DropWeapon;
                }
                else if (nRate >= 80 && nRate < 100)// 20% of 45% (= 9%) - Two handers (and shield)
                {
                    dwItemSort = TwoHanderType[Program.GetRandom.Next(0, TwoHanderType.Length)];
                    dwItemLev = ((dwItemSort == 900) ? Family.DropShield : Family.DropWeapon);
                }
            }
            /*if (dwItemLev == 99)
            {
               dwItemLev =0;
            }*/

            if (dwItemLev != 99)
            {
                dwItemLev = AlterItemLevel(dwItemLev, dwItemSort);

                uint idItemType = (dwItemSort * 1000) + (dwItemLev * 10) + dwItemQuality;
                // Database.ItemType.DBItem DbItem;
                if (Database.Server.ItemsBase.TryGetValue(idItemType, out DbItem))
                {
                    ushort position = Database.ItemType.ItemPosition(idItemType);
                    byte level = Database.ItemType.ItemMaxLevel((Role.Flags.ConquerItem)position);
                    if (DbItem.Level > level)
                        return 0;
                    return idItemType;
                }
            }
            DbItem = null;
            return 0;
        }

        public byte GenerateBless()
        {
            if (ServerKernel.NextAsync(0, 1000) < 250)
            {
                int selector;
                selector = ServerKernel.NextAsync(0, 100);
                if (selector < 1)
                    return 5;
                if (selector < 6)
                    return 3;
            }
            return 0;
        }

        public byte GenerateSocketCount(uint ItemID)
        {
            if (ItemID >= 410000 && ItemID <= 601999)
            {
                int nRate;
                nRate = ServerKernel.NextAsync(0, 1000) % 100;
                if (nRate < 5)
                    return 2;
                if (nRate < 20)
                    return 1;
            }
            return 0;
        }

        private byte GenerateQuality()
        {
            if ((bool)Refined)
                return 6;
            if ((bool)Unique)
                return 7;
            if ((bool)Elite)
                return 8;//8
            //if ((bool)Super)
            //    return 9;//9
            return 3;
        }

        public uint GenerateGold(out uint ItemID, bool normal = false, bool twin = false)
        {
            //uint amount = (uint)Program.GetRandom.Next(1, 2000);
            uint amount = 0;
            if (twin)
            {
                amount = (uint)Program.GetRandom.Next(50, 399);
            }
            else
            {
                if (normal)
                    amount = (uint)Program.GetRandom.Next(Family.DropMoney, Family.DropMoney + 250/* * 10*/);
                else
                {
                    amount = (uint)Program.GetRandom.Next(0, (int)(((250 * Family.Level / 2) / 2) + 1));

                    //amount /= 2;
                }
            }

            if(amount > 399)
            {
                amount = (uint)Program.GetRandom.Next(100, 399);
            }

            if (amount < 100)
            {
                ItemID = 1090000u; //silver //1090000u
            }
            else if (amount < 199)
            {
                ItemID = 1090010u; // syce //1090010u
            }
            else if (amount < 399)
            {
                ItemID = 1090020u; //gold // 1090020u
            }
            else
            {
                ItemID = 1091020u;
            }

            //ItemID = Database.ItemType.MoneyItemID((uint)amount);
            return amount;
        }

        private uint AlterItemLevel(uint dwItemLev, uint dwItemSort)
        {
            int nRand;
            nRand = Extensions.BaseFunc.RandGet(100, true);
            if (nRand < 50)
            {
                uint dwLev;
                dwLev = dwItemLev;
                dwItemLev = (uint)Extensions.BaseFunc.RandGet((int)(dwLev / 2u + dwLev / 3), false);
                if (dwItemLev > 1)
                    dwItemLev--;
            }
            else if (nRand > 80)
            {
                dwItemLev = (((dwItemSort < 110 || dwItemSort > 114) && (dwItemSort < 130 || dwItemSort > 134) && (dwItemSort < 900 || dwItemSort > 999)) ? Math.Min(dwItemLev + 1, 23) : Math.Min(dwItemLev + 1, 9));
            }
            return dwItemLev;
        }
    }
}
