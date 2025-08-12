using TheChosenProject.Database;
using TheChosenProject.Game;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role.Instance;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Database.ItemType;
using static TheChosenProject.Program;
using static TheChosenProject.Role.Instance.Vendor;
using DevExpress.ClipboardSource.SpreadsheetML;

namespace TheChosenProject.Ai
{
    public unsafe static class DataVendor
    {
        public struct Shopflag
        {
            public ushort X;
            public ushort Y;
        }

        private static Dictionary<byte, Shopflag> MarketPlace = new Dictionary<byte, Shopflag>();

        public static void AddPlaceMarket()
        {
            MarketPlace.Add((byte)1, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)218
            });
            MarketPlace.Add((byte)2, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)214
            });
            MarketPlace.Add((byte)3, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)210
            });
            MarketPlace.Add((byte)4, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)206
            });
            MarketPlace.Add((byte)5, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)202
            });
            MarketPlace.Add((byte)6, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)198
            });
            MarketPlace.Add((byte)7, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)194
            });
            MarketPlace.Add((byte)8, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)190
            });
            MarketPlace.Add((byte)9, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)186
            });
            MarketPlace.Add((byte)10, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)182
            });
            MarketPlace.Add((byte)11, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)178
            });
            MarketPlace.Add((byte)12, new Shopflag()
            {
                X = (ushort)271,
                Y = (ushort)174
            });
            MarketPlace.Add((byte)13, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)218
            });
            MarketPlace.Add((byte)14, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)214
            });
            MarketPlace.Add((byte)15, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)210
            });
            MarketPlace.Add((byte)16, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)206
            });
            MarketPlace.Add((byte)17, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)202
            });
            MarketPlace.Add((byte)18, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)198
            });
            MarketPlace.Add((byte)19, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)194
            });
            MarketPlace.Add((byte)20, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)190
            });
            MarketPlace.Add((byte)21, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)186
            });
            MarketPlace.Add((byte)22, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)182
            });
            MarketPlace.Add((byte)23, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)178
            });
            MarketPlace.Add((byte)24, new Shopflag()
            {
                X = (ushort)264,
                Y = (ushort)174
            });
            MarketPlace.Add((byte)25, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)217
            });
            MarketPlace.Add((byte)26, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)213
            });
            MarketPlace.Add((byte)27, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)209
            });
            MarketPlace.Add((byte)28, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)205
            });
            MarketPlace.Add((byte)29, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)201
            });
            MarketPlace.Add((byte)30, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)197
            });
            MarketPlace.Add((byte)31, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)193
            });
            MarketPlace.Add((byte)32, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)189
            });
            MarketPlace.Add((byte)33, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)185
            });
            MarketPlace.Add((byte)34, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)181
            });
            MarketPlace.Add((byte)35, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)177
            });
            MarketPlace.Add((byte)36, new Shopflag()
            {
                X = (ushort)239,
                Y = (ushort)173
            });
            MarketPlace.Add((byte)37, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)217
            });
            MarketPlace.Add((byte)38, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)213
            });
            MarketPlace.Add((byte)39, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)209
            });
            MarketPlace.Add((byte)40, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)205
            });
            MarketPlace.Add((byte)41, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)201
            });
            MarketPlace.Add((byte)42, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)197
            });
            MarketPlace.Add((byte)43, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)193
            });
            MarketPlace.Add((byte)44, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)189
            });
            MarketPlace.Add((byte)45, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)185
            });
            MarketPlace.Add((byte)46, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)181
            });
            MarketPlace.Add((byte)47, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)177
            });
            MarketPlace.Add((byte)48, new Shopflag()
            {
                X = (ushort)232,
                Y = (ushort)173
            });

            var array = MarketPlace.Values.ToArray();
            MarketPlace.Clear();
            int count = array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                count--;
                MarketPlace.Add((byte)(i + 1), array[count]);
            }
        }


        public static ConcurrentDictionary<uint, Ai.Object> AIPoll = new ConcurrentDictionary<uint, Ai.Object>();

        public static void CreateOfflineVendor(Client.GameClient client)
        {
            var myVendor = AIPoll.Values.Where(p => p.OwnerUID == client.Player.UID).FirstOrDefault();
            if (myVendor != null)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    client.ActiveNpc = (uint)NpcID.DataVendor;
                    Script(client, stream, 0, "", 0);
                }
                return;
            }
            if (client.MyVendor != null && client.MyVendor.Items.Count > 0)
            {
                Ai.Object booth = new Ai.Object(true)
                {
                    Map = Database.Server.ServerMaps[1036],
                    OwnerUID = client.Player.UID
                };
                var name = client.Player.Name;
                if (name.Length >= 11)
                {
                    name = "";
                    var array = client.Player.Name.ToArray();
                    for (int i = 0; i < 11; i++)
                        name += array[i];
                }
                name += "[Bot]";
                if (booth.Add(false, true, client.Player.X, client.Player.Y, name, 0, client))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        foreach (var item in client.MyVendor.Items.Values)
                        {
                            booth.BEntity.MyVendor.Items.TryAdd(item.DataItem.UID, item);
                            client.Inventory.Update(item.DataItem, AddMode.REMOVE, stream);
                        }
                        client.Player.VendorTime = DateTime.Now;
                        client.Player.VendorTime = client.Player.VendorTime.AddDays(1);
                        client.MyVendor.StopVending(stream);
                        client.Teleport((ushort)(client.Player.X + 3), client.Player.Y, client.Player.Map);
                        ServerDatabase.SaveClientItems(client);//bahaa
                        booth.isVendor = true;
                        AIPoll.TryAdd(booth.BEntity.Player.UID, booth);
                        Save(booth);
                    }
                }
            }
        }

        public static void TimerVendor(Client.GameClient client, ServerSockets.Packet stream)
        {
            var myVendor = AIPoll.Values.Where(p => p.OwnerUID == client.Player.UID).FirstOrDefault();
            if(myVendor == null)
            {
                return;
            }
            int countitems = myVendor.BEntity.MyVendor.Items.Count;
            countitems = myVendor.BEntity.MyVendor.Items.Count;
            if (countitems > 0)
            {
                if (client.Inventory.HaveSpace((byte)countitems))
                {
                    foreach (var item in myVendor.BEntity.MyVendor.Items.Values)
                    {
                        if (Database.Server.ItemsBase.TryGetValue(item.DataItem.ITEM_ID, out var dBItem))
                        {
                            client.Inventory.Add(item.DataItem, dBItem, stream);
                        }
                    }
                    int cps = myVendor.BEntity.Player.ConquerPoints;
                    if (myVendor.BEntity.Player.ConquerPoints > 0)
                        client.Player.ConquerPoints += myVendor.BEntity.Player.ConquerPoints;
                    myVendor.BEntity.Player.ConquerPoints = 0;
                    long money = myVendor.BEntity.Player.Money;
                    if (myVendor.BEntity.Player.Money > 0)
                        client.Player.Money += myVendor.BEntity.Player.Money;
                    myVendor.BEntity.Player.Money = 0;
                    myVendor.BEntity.MyVendor.Items.Clear();
                    myVendor.Leave();
                    ServerDatabase.SaveClient(client);
                }
            }
        }

        [NpcAttribute(NpcID.DataVendor)]
        public static void Script(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var myVendor = AIPoll.Values.Where(p => p.OwnerUID == client.Player.UID).FirstOrDefault();
            if (myVendor == null)
            {
                client.CreateBoxDialog("You don`t have any vendor bot in market.");
                return;
            }
            int countitems = myVendor.BEntity.MyVendor.Items.Count;
            Dialog data = new Dialog(client, stream);
            data.AddAvatar(90);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText($"Hello {client.Player.Name}, i`m vendor bot manager in market how i can help you?");
                        data.AddText($"\nBot Name: {myVendor.BEntity.Player.Name}\nCount Items: {countitems}\nCps: {myVendor.BEntity.Player.ConquerPoints}\nMoney: {myVendor.BEntity.Player.Money}");
                        data.AddOption("Cancel Bot.", 1);
                        data.AddOption("Collect Cps.", 2);
                        data.AddOption("Collect Money.", 3);
                        data.AddOption("I`ll~think~it~over.", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        countitems = myVendor.BEntity.MyVendor.Items.Count;
                        if (countitems > 0)
                        {
                            if (client.Inventory.HaveSpace((byte)countitems))
                            {
                                foreach (var item in myVendor.BEntity.MyVendor.Items.Values)
                                {
                                    if (Database.Server.ItemsBase.TryGetValue(item.DataItem.ITEM_ID, out var dBItem))
                                    {
                                        client.Inventory.Add(item.DataItem, dBItem, stream);
                                    }
                                }
                                int cps = myVendor.BEntity.Player.ConquerPoints;
                                if (myVendor.BEntity.Player.ConquerPoints > 0)
                                    client.Player.ConquerPoints += myVendor.BEntity.Player.ConquerPoints;
                                myVendor.BEntity.Player.ConquerPoints = 0;
                                long money = myVendor.BEntity.Player.Money;
                                if (myVendor.BEntity.Player.Money > 0)
                                    client.Player.Money += myVendor.BEntity.Player.Money;
                                myVendor.BEntity.Player.Money = 0;
                                myVendor.BEntity.MyVendor.Items.Clear();
                                client.Player.VendorTime = DateTime.Now;
                                myVendor.Leave();
                                ServerDatabase.SaveClient(client);
                                data.AddText($"You collect from vendor bot:");
                                data.AddText($"\nItems: {countitems}.");
                                data.AddText($"\nCps: {cps}.");
                                data.AddText($"\nMoney: {money}.");
                                data.AddOption("Thanks!", 255);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText($"Please make {countitems + 1} more space in your inventory.");
                                data.AddOption("I~see.", 255);
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        int cps = myVendor.BEntity.Player.ConquerPoints;
                        if (myVendor.BEntity.Player.ConquerPoints > 0)
                            client.Player.ConquerPoints += myVendor.BEntity.Player.ConquerPoints;
                        myVendor.BEntity.Player.ConquerPoints = 0;
                        ServerDatabase.SaveClient(client);
                        Save(myVendor);
                        data.AddText($"You collect from vendor bot:");
                        data.AddText($"\nCps: {cps}.");
                        data.AddOption("Thanks!", 255);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        long money = myVendor.BEntity.Player.Money;
                        if (myVendor.BEntity.Player.Money > 0)
                            client.Player.Money += myVendor.BEntity.Player.Money;
                        myVendor.BEntity.Player.Money = 0;
                        ServerDatabase.SaveClient(client);
                        Save(myVendor);
                        data.AddText($"You collect from vendor bot:");
                        data.AddText($"\nMoney: {money}.");
                        data.AddOption("Thanks!", 255);
                        data.FinalizeDialog();
                        break;
                    }
            }
        }

        public static void Remove(uint uid)
        {
            if (File.Exists(ServerKernel.CO2FOLDER + "\\AiVendor\\Items\\" + uid + ".bin"))
                File.Delete(ServerKernel.CO2FOLDER + "\\AiVendor\\Items\\" + uid + ".bin");
            if (File.Exists(ServerKernel.CO2FOLDER + "\\AiVendor\\" + uid + ".ini"))
                File.Delete(ServerKernel.CO2FOLDER + "\\AiVendor\\" + uid + ".ini");
        }

        public static void Save(Ai.Object ai)
        {
            if (ai.isVendor && ai.BEntity.MyVendor != null)
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\AiVendor\\" + ai.UID + ".ini");
                write.Write<uint>("Character", "UID", ai.UID);
                write.Write<string>("Character", "Name", ai.BEntity.Player.Name);
                write.Write<uint>("Character", "OwnerUID", ai.OwnerUID);
                write.Write<ushort>("Character", "X", ai.BEntity.Player.X);
                write.Write<ushort>("Character", "Y", ai.BEntity.Player.Y);
                write.Write<uint>("Character", "Map", ai.BEntity.Player.Map);
                write.Write<int>("Character", "CPs", ai.BEntity.Player.ConquerPoints);
                write.Write<long>("Character", "Money", ai.BEntity.Player.Money);
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(ServerKernel.CO2FOLDER + "\\AiVendor\\Items\\" + ai.UID + ".bin", FileMode.Create))
                {
                    ClientItems.DBItem DBItem = new ClientItems.DBItem();
                    int ItemCount = ai.BEntity.MyVendor.Items.Count;
                    int AmountOfBytes = sizeof(ClientItems.DBItem);
                    binary.Write(&ItemCount, sizeof(int));
                    binary.Write(&AmountOfBytes, sizeof(int));
                    foreach (var items in ai.BEntity.MyVendor.Items.Values)
                    {
                        DBItem.GetDBItem(items.DataItem);
                        byte CostType = (byte)items.CostType;
                        long AmountCost = items.AmountCost;
                        binary.Write(&CostType, sizeof(byte));
                        binary.Write(&AmountCost, sizeof(uint));
                        if (!binary.Write(&DBItem, AmountOfBytes))
                            Console.WriteLine($"Error Save Data Bot {ai.BEntity.Player.Name}.");
                    }
                    binary.Close();
                }
            }
        }

        public static void Load()
        {
            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
            foreach (string fname in System.IO.Directory.GetFiles(ServerKernel.CO2FOLDER + "\\AiVendor\\"))
            {
                try
                {
                    ini.FileName = fname;
                    uint UID = ini.ReadUInt32("Character", "UID", 0);
                    uint OwnerUID = ini.ReadUInt32("Character", "OwnerUID", 0);
                    string Name = ini.ReadString("Character", "Name", "");
                    ushort x = ini.ReadUInt16("Character", "X", 0);
                    ushort y = ini.ReadUInt16("Character", "Y", 0);
                    ushort Map = ini.ReadUInt16("Character", "Map", 0);
                    uint CPs = ini.ReadUInt32("Character", "CPs", 0);
                    uint Money = ini.ReadUInt32("Character", "Money", 0);

                    Ai.Object booth = new Ai.Object(true)
                    {
                        Map = Database.Server.ServerMaps[1036],
                        OwnerUID = OwnerUID
                    };

                    if (booth.Add(false, true, x, y, Name, UID))
                    {
                        WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                        if (binary.Open(ServerKernel.CO2FOLDER + "\\AiVendor\\Items\\" + UID + ".bin", FileMode.Open))
                        {
                            ClientItems.DBItem Item;
                            int ItemCount;
                            int AmountOfBytes;
                            binary.Read(&ItemCount, sizeof(int));
                            binary.Read(&AmountOfBytes, sizeof(int));
                            for (int I = 0; I < ItemCount; I++)
                            {
                                byte CostType = 0;
                                uint AmountCost = 0;
                                binary.Read(&CostType, sizeof(byte));
                                binary.Read(&AmountCost, sizeof(uint));
                                binary.Read(&Item, AmountOfBytes);
                                if (AmountCost > 0)
                                {
                                    var vendorItem = new VendorItem();
                                    vendorItem.AmountCost = AmountCost;
                                    vendorItem.CostType = (MsgItemView.ActionMode)CostType;
                                    vendorItem.DataItem = Item.GetDataItem();
                                    booth.BEntity.MyVendor.Items.TryAdd(vendorItem.DataItem.UID, vendorItem);
                                }
                            }
                            binary.Close();
                        }
                        booth.isVendor = true;
                        AIPoll.TryAdd(booth.UID, booth);
                    }
                }
                catch
                {

                }
            }

        }

        public static bool ChanceSuccess(int value)
        {
            if (value <= 0)
                return false;

            return value >= Generate(1, 120);
        }

        public static Int32 Generate(Int32 Min, Int32 Max)
        {
            if (Max != Int32.MaxValue)
                Max++;
            Int32 Value = Role.Core.Random.Next(Min, Max);
            return Value;
        }

        public static void LoadGMBots(int num = 1)
        {
            var Gear = Server.ItemsBase.Values.Where(e =>
            (EquipPassJobReq(e, 10) == true || EquipPassJobReq(e, 20) == true || EquipPassJobReq(e, 40) == true || EquipPassJobReq(e, 190) == true) &&
            (Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Armor ||
             Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Necklace ||
             Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Head ||
             Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Ring ||
             Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.Boots)
             && (e.Level >= 70 && e.Level <= 125) && (e.ID % 10) == 6).ToArray();

            var Weap = Server.ItemsBase.Values.Where(e =>
            (Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon ||
             Database.ItemType.ItemPosition(e.ID) == (ushort)Role.Flags.ConquerItem.RightWeapon)
             && e.Level < 130 && (e.ID % 10) >= 6).ToArray();


            ushort _x = 232;
            ushort _y = 217 + 4;

            uint id = Server.ClientCounter.Next;

            for (int x = 0; x < num; x++)
            {
                bool getPlace = false;
                foreach (var place in MarketPlace.Values)
                {
                    if (getPlace)
                        break;
                    _x = place.X;
                    _y = place.Y;
                    Object booth = new Object() { Map = Database.Server.ServerMaps[1036] };
                    if (booth.Add(false, true, _x, _y))
                    {
                        int ItemAmount = Role.Core.Random.Next(20, 25);
                        for (int i = 0; i < ItemAmount; i++)
                        {
                            //if (Role.MyMath.ChanceSuccess(15))
                            //{
                            //    uint itemid = 730002;
                            //    if (Role.MyMath.ChanceSuccess(70))
                            //        itemid = 730003;
                            //    Database.ItemType.DBItem dBItem;
                            //    if (Server.ItemsBase.TryGetValue(itemid, out dBItem))
                            //    {
                            //        var vendor = new VendorItem();
                            //        vendor.CostType = MsgItemView.ActionMode.Gold;
                            //        vendor.DataItem = new MsgGameItem();
                            //        vendor.DataItem.UID = Server.ITEM_Counter.Next;
                            //        vendor.DataItem.ITEM_ID = dBItem.ID;
                            //        vendor.DataItem.Durability = dBItem.Durability;
                            //        vendor.DataItem.MaximDurability = dBItem.Durability;
                            //        vendor.DataItem.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                            //        vendor.AmountCost = (uint)(itemid == 730002 ? 15000 : 30000);
                            //        vendor.AmountCost /= 2;
                            //        booth.BEntity.MyVendor.AddItem(vendor.DataItem, vendor.CostType, vendor.AmountCost);
                            //    }
                            //}

                            //else
                            if (Role.MyMath.ChanceSuccess(45))
                            {
                                #region Items/Weap

                                if (Role.MyMath.ChanceSuccess(70))
                                {
                                    var CIBI = Gear[Role.Core.Random.Next(0, Gear.Length)];
                                    var vendor = new VendorItem();
                                    vendor.CostType = MsgItemView.ActionMode.Gold;
                                    vendor.DataItem = new MsgGameItem();
                                    vendor.DataItem.UID = Server.ITEM_Counter.Next;
                                    vendor.DataItem.ITEM_ID = CIBI.ID;
                                    vendor.DataItem.Durability = CIBI.Durability;
                                    vendor.DataItem.MaximDurability = CIBI.Durability;
                                    vendor.DataItem.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                    //if (Role.MyMath.Success(70))
                                    //{
                                    //    vendor.DataItem.Bless = 1;
                                    //    vendor.AmountCost = 100000;
                                    //}
                                    vendor.AmountCost = 500000;
                                    booth.BEntity.MyVendor.AddItem(vendor.DataItem, vendor.CostType, vendor.AmountCost);
                                }
                                else if (Role.MyMath.ChanceSuccess(50))
                                {
                                    var CIBI = Weap[Role.Core.Random.Next(0, Weap.Length)];
                                    var vendor = new VendorItem();
                                    vendor.CostType = MsgItemView.ActionMode.Gold;
                                    vendor.DataItem = new MsgGameItem();
                                    vendor.DataItem.UID = Server.ITEM_Counter.Next;
                                    vendor.DataItem.ITEM_ID = CIBI.ID;
                                    vendor.DataItem.Durability = CIBI.Durability;
                                    vendor.DataItem.MaximDurability = CIBI.Durability;
                                    vendor.DataItem.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                    //if (Role.MyMath.Success(70))
                                    //{
                                    //    vendor.DataItem.Bless = 1;
                                    //    vendor.AmountCost = 100000 * 3;
                                    //}
                                    vendor.AmountCost = 500000;
                                    booth.BEntity.MyVendor.AddItem(vendor.DataItem, vendor.CostType, vendor.AmountCost);
                                }
                                //else
                                //{//Super Item
                                //    var CIBI = Gear[Role.Core.Random.Next(0, Gear.Length)];
                                //    var vendor = new VendorItem();
                                //    vendor.CostType = MsgItemView.ActionMode.CPs;
                                //    vendor.DataItem = new MsgGameItem();
                                //    vendor.DataItem.UID = Server.ITEM_Counter.Next;
                                //    vendor.DataItem.ITEM_ID = CIBI.ID + 3;
                                //    vendor.DataItem.Durability = CIBI.Durability;
                                //    vendor.DataItem.MaximDurability = CIBI.Durability;
                                //    vendor.DataItem.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                                //    if (Role.MyMath.Success(70))
                                //    {
                                //        vendor.DataItem.Bless = 1;
                                //        vendor.AmountCost = 100000;
                                //    }
                                //    else
                                //    {
                                //        vendor.DataItem.Bless = 3;
                                //        vendor.AmountCost = 100000 * 3;
                                //    }
                                //    vendor.AmountCost /= 2;
                                //    booth.BEntity.MyVendor.AddItem(vendor.DataItem, vendor.CostType, vendor.AmountCost);
                                //}

                                #endregion Items/Weap
                            }
                            //else
                            //{
                            //    #region TortoiseGem
                            //    if (Role.MyMath.ChanceSuccess(15))
                            //    {
                            //        uint itemid = 700071;
                            //        if (Role.MyMath.ChanceSuccess(70))
                            //            id++;
                            //        Database.ItemType.DBItem dBItem;
                            //        if (Server.ItemsBase.TryGetValue(itemid, out dBItem))
                            //        {
                            //            var vendor = new VendorItem();
                            //            vendor.CostType = MsgItemView.ActionMode.CPs;
                            //            vendor.DataItem = new MsgGameItem();
                            //            vendor.DataItem.UID = Server.ITEM_Counter.Next;
                            //            vendor.DataItem.ITEM_ID = dBItem.ID;
                            //            vendor.DataItem.Durability = dBItem.Durability;
                            //            vendor.DataItem.MaximDurability = dBItem.Durability;
                            //            vendor.DataItem.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                            //            if (itemid == 700072)
                            //                vendor.AmountCost = 20000 * 3;
                            //            if (itemid == 700071)
                            //                vendor.AmountCost = 10000 * 3;
                            //            if (itemid == 700073)
                            //                vendor.AmountCost = 300000 * 3;
                            //            vendor.AmountCost /= 2;
                            //            booth.BEntity.MyVendor.AddItem(vendor.DataItem, vendor.CostType, vendor.AmountCost);
                            //        }
                            //    }

                            //    #endregion TortoiseGem
                            //}
                        }
                        booth.isVendor = true;
                        getPlace = true;

                        AIPoll.TryAdd(booth.UID, booth);
                    }
                }
            }
        }
    }
}
