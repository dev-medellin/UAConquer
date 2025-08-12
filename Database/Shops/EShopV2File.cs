﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TheChosenProject.Database.Shops
{
    public static class EShopV2File
    {
        public static void Load()
        {
            string[] text = File.ReadAllLines(ServerKernel.CO2FOLDER + "\\shops\\emoneyshopV2.ini");
            ShopFile.Shop shop = new ShopFile.Shop();
            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x].Replace("_", " ");

                bool bound = false;
                if (line.Contains(" present"))
                    bound = true;
                string[] split = line.Split('=');

                if (split[0] == "ID")
                {
                    uint id = uint.Parse(split[1]);
                    if (EShopFile.Shops.ContainsKey(id))
                        shop = EShopFile.Shops[uint.Parse(split[1])];
                    else
                    {
                        shop = new ShopFile.Shop();
                 
                        shop.Items = new List<uint>();
                        shop.UID = id;
                        EShopFile.Shops.Add(id, shop);
                    }
                }
                else if (split[0] == "MoneyType")
                {
                    shop.MoneyType = (ShopFile.MoneyType)byte.Parse(split[1]);
                }
                else if (split[0].Contains("Item"))
                {

                    if (split[0].StartsWith("Item"))
                    {

                        uint ID = uint.Parse(split[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (bound)
                        {
                            if (!shop.BoundItems.Contains(ID))
                                shop.BoundItems.Add(ID);
                        }
                        else
                        {
                            if (!shop.Items.Contains(ID))
                                shop.Items.Add(ID);
                        }
                    }
                    else
                    {
                        uint ID = uint.Parse(split[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (bound)
                        {
                            if (!shop.BoundItems.Contains(ID))
                                shop.BoundItems.Add(ID);
                        }
                        else
                        {
                            if (!shop.Items.Contains(ID))
                                shop.Items.Add(ID);
                        }
                    }
                }
                else if (split[0].StartsWith("item"))
                {
                    try
                    {
                        uint ID = uint.Parse(split[1].Split(' ')[0]);
                        if (bound)
                        {
                            if (!shop.BoundItems.Contains(ID))
                                shop.BoundItems.Add(ID);
                        }
                        else
                        {
                            if (!shop.Items.Contains(ID))
                                shop.Items.Add(ID);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteException(e);
                    }
                }
                else if (split[0].Length != 0 && split[0] != "[recommend]" && split[0] != "Amount")
                {
                    try
                    {
                        uint ID = uint.Parse(split[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (bound)
                        {
                            if (!shop.BoundItems.Contains(ID))
                                shop.BoundItems.Add(ID);
                        }
                        else
                        {
                            if (!shop.Items.Contains(ID))
                                shop.Items.Add(ID);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                
            }
        }
    }
}
