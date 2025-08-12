using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TheChosenProject.WindowsAPI;

namespace TheChosenProject.Database.Shops
{
    public static class ShopFile
    {
        public class Shop
        {
            public uint UID;

            public MoneyType MoneyType;

            public List<uint> Items;

            public List<uint> BoundItems = new List<uint>();

            public int Count => Items.Count;
        }

        public enum MoneyType
        {
            Gold,
            ConquerPoints,
            HonorPoints,
            BoundConquerPoints
        }

        public static List<uint> Not_Allowed_Dropped = new List<uint>();

        public static Dictionary<uint, Shop> Shops;

        public static void Load()
        {
            Shops = new Dictionary<uint, Shop>();
            IniFile reader;
            reader = new IniFile("\\shops\\Shop.dat");
            int Count;
            Count = reader.ReadInt32("Header", "Amount", 0);
            for (int x = 0; x < Count; x++)
            {
                Shop shop;
                shop = new Shop
                {
                    UID = reader.ReadUInt32("Shop" + x, "ID", 0),
                    MoneyType = (MoneyType)reader.ReadUInt32("Shop" + x, "MoneyType", 0)
                };
                int Items;
                Items = reader.ReadInt32("Shop" + x, "ItemAmount", 0);
                shop.Items = new List<uint>();
                for (int i = 0; i < Items; i++)
                {
                    shop.Items.Add(reader.ReadUInt32("Shop" + x, "Item" + i, 0));
                    if ((shop.UID >= 2302 && shop.UID <= 2306) || shop.UID == 6586 || shop.UID == 6572 || shop.UID == 19896)
                    {
                        if (shop.UID == 6572 || shop.UID == 19896)
                            ItemType.RareGarments.Add(reader.ReadUInt32("Shop" + x, "Item" + i, 0));
                        else
                            ItemType.RareAccessories.Add(reader.ReadUInt32("Shop" + x, "Item" + i, 0));
                        Not_Allowed_Dropped.Add(reader.ReadUInt32("Shop" + x, "Item" + i, 0));
                    }
                }
                if (!Shops.ContainsKey(shop.UID))
                    Shops.Add(shop.UID, shop);
            }
        }
    }
}
