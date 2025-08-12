using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Filtering.Templates;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Database.Shops;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static class MsgItemUsuagePacket
    {
        public enum ItemUsuageID : uint
        {
            CreateSocketItem = 43u,
            BuyItemFromForging = 55u,
            GemCompose = 39u,
            ToristSuper = 51u,
            AddBless = 40u,
            GarmentShop = 53u,
            BuyItem = 1u,
            SellItem = 2u,
            RemoveInventory = 3u,
            Equip = 4u,
            SetEquipPosition = 5u,
            Unequip = 6u,
            UpgradeEnchant = 7u,
            ArrowReload = 8u,
            ShowWarehouseMoney = 9u,
            DepositWarehouse = 10u,
            WarehouseWithdraw = 11u,
            RepairItem = 14u,
            VIPRepairItem = 0x0F,
            UpdateDurability = 17u,
            RemoveEquipment = 18u,
            UpgradeDragonball = 19u,
            UpgradeMeteor = 20u,
            ShowVendingList = 21u,
            AddVendingItemGold = 22u,
            RemoveVendingItem = 23u,
            BuyVendingItem = 24u,
            UpdateArrowCount = 25u,
            ParticleEffect = 26u,
            Ping = 27u,
            UpdateEnchant = 28u,
            AddVendingItemConquerPts = 29u,
            UpdatePurity = 35u,
            DropItem = 37u,
            DropGold = 38u,
            RedeemGear = 32u,
            ClaimGear = 33u,
            UnAlternante = 44u,
            ActiveItems = 41u,
            Alternante = 45u,
            SocketTalismanWithItem = 35u,
            SocketTalismanWithCPs = 36u,
            MergeStackableItems = 48u,
            ReturnedItems = 50u,
            ShowItem = 52u,
            DegradeEquipment = 54u,
            OpenInventorySash = 56u,
            SplitStack = 49u
        }

        public static void GetUsageItem(this ServerSockets.Packet msg, out ItemUsuageID action, out uint id, out ulong dwParam, out uint timestamp, out uint dwParam2, out uint dwParam3, out uint dwparam4, out List<uint> args, out byte LoaderMessage)
        {
            msg.Seek(48);
            LoaderMessage = msg.ReadUInt8();//48
            msg.Seek(4);
            id = msg.ReadUInt32();//4
            dwParam = msg.ReadUInt32();//8
            action = (ItemUsuageID)msg.ReadInt32();//12

            timestamp = msg.ReadUInt32();//20
            dwParam2 = msg.ReadUInt32();//24
            dwParam3 = msg.ReadUInt32();//26

            dwparam4 = msg.ReadUInt32();
            msg.SeekForward(12 * sizeof(int));
            args = new List<uint>();
            if (dwParam2 > 0 && dwParam2 < 50)
            {
                //          Console.WriteLine(msg.Position);
                msg.SeekForward(4);
                for (int i = 0; i < dwParam2; i++)
                {
                    args.Add(msg.ReadUInt32());
                }
            }
        }

        public static Packet ItemUsageCreate(this Packet msg, ItemUsuageID action, uint id, ulong dwParam1, uint timestamp, uint dwParam2, uint dwParam3, uint dwparam4, List<uint> args = null)
        {
            msg.InitWriter();
            msg.Write(id);
            msg.Write((uint)dwParam1);
            msg.Write((uint)action);
            msg.Write((timestamp != 0) ? timestamp : Time32.Now.Value);
            msg.Write(dwParam2);
            msg.Write(dwParam3);
            msg.Write(dwparam4);
            msg.SeekForward(48);
            if (args != null)
            {
                for (int i = 0; i < ((ICollection)args).Count; i++)
                {
                    int arg;
                    arg = (int)((IList)args)[i];
                    msg.Write(arg);
                }
            }
            msg.Finalize(1009);
            return msg;
        }

        [Packet(GamePackets.Usage)]
        public static void ItemUsuage(GameClient client, Packet stream)
        {
            ItemUsuageID action;
            uint id;

            ulong dwParam;
            uint timestamp;
            uint dwParam2;
            uint dwParam3;
            uint dwparam4;//unknow
            List<uint> args;

            byte LoaderMessage;
            stream.GetUsageItem(out action, out id, out dwParam, out timestamp, out dwParam2, out dwParam3, out dwparam4, out args, out LoaderMessage);
            switch (action)
            {
                case ItemUsuageID.UpgradeMeteor:
                case ItemUsuageID.UpgradeDragonball:
                    {
                        uint ItemUID = (uint)dwParam;

                        MsgGameItem DataItem;
                        MsgGameItem itemuse;

                        if (client.TryGetItem(ItemUID, out itemuse) && client.TryGetItem(id, out DataItem))
                        {
                            ushort Position = Database.ItemType.ItemPosition(DataItem.ITEM_ID);
                            //anti proxy --------------------
                            if (!Database.ItemType.AllowToUpdate((Role.Flags.ConquerItem)Position))
                            {
                                client.SendSysMesage("This item's level cannot be upgraded anymore.");
                                return;
                            }
                            if (Database.ItemType.IsArrow(DataItem.ITEM_ID))
                                return;
                            if (DataItem.Durability < DataItem.MaximDurability)
                            {
                                client.SendSysMesage("please repair this items first .");
                                return;
                            }
                            //if (DataItem.Bound >= 1)
                            //{
                            //    client.SendSysMesage("you can't update free items");
                            //    return;
                            //}
                            //bool worked = true;
                            //------------------------
                            if (itemuse.ITEM_ID == Database.ItemType.DragonBall)
                            {
                                Database.ItemType.DBItem DBItem;
                                DataItem.SocketProcess = true;
                                if (Database.Server.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                {
                                    if (DataItem.ITEM_ID == 410501 || DataItem.ITEM_ID == 500301 || DataItem.ITEM_ID == 421301 || DataItem.ITEM_ID == 410301)
                                    {
                                        client.CreateBoxDialog("This item's cant be upgraded the quantity.");
                                        return;
                                    }

                                    if (DataItem.ITEM_ID % 10 == 9)
                                    {
                                        client.SendSysMesage("This item's cant be upgraded anymore.");
                                        return;
                                    }
                                    //byte Chance = (byte)(70 - ((DBItem.Level - (DBItem.Level > 100 ? 30 : 0)) / (10 - DataItem.ITEM_ID % 10)));
                                    //var Chance = Database.ItemType.ChanceToUpgradeQuality(DataItem.ITEM_ID);
                                    sbyte Chance = (sbyte)(100 - (DBItem.Level / 3));
                                    byte Quality = (byte)(DBItem.ID % 10);

                                    if (Quality == 6)
                                        Chance -= 10;
                                    else if (Quality == 7)
                                        Chance -= 15;
                                    else if (Quality == 8)
                                        Chance -= 25;

                                    if (Chance == 0 || !client.Inventory.Contain(DataItem.ITEM_ID, 1) || DataItem == null)
                                    {
                                        client.SendSysMesage("You cannot upgrade your " + DBItem.Name + " any further", MsgMessage.ChatMode.System);
                                        return;
                                    }

                                    if (client.Inventory.Contain(Database.ItemType.DragonBall, 1, 0))
                                    {
                                        if (Role.Core.PercentSuccess(Chance))
                                        {
                                            client.Player.DragonBallSocket += 1;
                                            dwParam = 1;
                                            double sock_chance = 0;
                                            uint oldid = DataItem.ITEM_ID;
                                            if (DataItem.ITEM_ID % 10 < 5)
                                                DataItem.ITEM_ID += 5 - DataItem.ITEM_ID % 10;
                                            DataItem.ITEM_ID = (uint)(DataItem.ITEM_ID + 1);
                                            if (DataItem.SocketOne == Role.Flags.Gem.NoSocket)
                                            {
                                                if (client.Player.DragonBallSocket >= 100)
                                                {
                                                    uint thousands = client.Player.DragonBallSocket / 100;
                                                    sock_chance = thousands * 0.01; // 0.001% per 1000
                                                }
                                                if (ServerKernel.ONE_SOC_RATE > 0)
                                                {
                                                    if (client.Player.DragonBallSocket >= 150 && Role.Core.PercentSuccess(0.03 + (client.Player.BlessTime > 0 ? Global.LUCKY_TIME_BONUS_SOCKET_RATE : 0) + sock_chance))
                                                    {
                                                        client.Player.DragonBallSocket = 0;
                                                        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                                        effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                                                        effect.DropType = MsgDropID.Earth;
                                                        Program.SendGlobalPackets.Enqueue(stream.ItemPacketCreate(effect));
                                                        DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the first socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the first socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                                        //Program.DiscordSocketMaster1.Enqueue("As a very lucky player, " + client.Player.Name + " has added the first socket to his/her " + DBItem.Name + "");
                                                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                                    }
                                                }
                                            }

                                            //if (DataItem.SocketOne != Role.Flags.Gem.NoSocket && DataItem.SocketTwo != Role.Flags.Gem.NoSocket)
                                            //{
                                            //    if (Role.Core.PercentSuccess((0.00 + (client.Player.BlessTime > 0 ? Global.LUCKY_TIME_BONUS_SOCKET_RATE : 1))/* * 1*/))
                                            //    {
                                            //        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                            //        effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                                            //        effect.DropType = MsgDropID.Earth;
                                            //        Program.SendGlobalPackets.Enqueue(stream.ItemPacketCreate(effect));
                                            //        DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                            //        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the second socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                                            //        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the second socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                            //        Program.DiscordSocketMaster2.Enqueue("As a very lucky player, " + client.Player.Name + " has added the second socket to his/her " + DBItem.Name + "");
                                            //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                            //    }
                                            //}
                                            DataItem.Durability = DataItem.MaximDurability;
                                            DataItem.Mode = Role.Flags.ItemMode.Update;
                                            DataItem.Failed = true;
                                            if (oldid != DataItem.ITEM_ID)
                                            {
                                                DataItem.Send(client, stream);//.Update(itemuse, Role.Instance.AddMode.REMOVE,stream);
                                                client.SendSysMesage("You have successfully upgraded the quality of your " + DBItem.Name + "", MsgMessage.ChatMode.TopLeft);

                                            }
                                            else
                                            {
                                                client.SendSysMesage("This item's level cannot be upgraded anymore.", MsgMessage.ChatMode.TopLeft);
                                            }
                                        }
                                        else
                                        {
                                            client.Player.DragonBallSocket += 1;
                                            if (client.Player.VipLevel != 6)
                                            {
                                                int minDura = (int)Math.Ceiling(DataItem.Durability / 2.0);
                                                int maxDura = DataItem.Durability;
                                                int randomDura = Role.Core.Random.Next(minDura, maxDura);
                                                DataItem.Durability = (ushort)randomDura;
                                                DataItem.Mode = Role.Flags.ItemMode.Update;
                                                DataItem.Failed = false;
                                                DataItem.Send(client, stream);
                                            }
                                            client.SendSysMesage("You have failed to upgrade the quality of your " + DBItem.Name + ".", MsgMessage.ChatMode.TopLeft);
                                        }
                                        client.Inventory.Remove(Database.ItemType.DragonBall, 1, stream);

                                    }
                                }
                            }
                            else if (itemuse.ITEM_ID == Database.ItemType.Meteor)
                            {
                                Database.ItemType.DBItem DBItem;
                                DataItem.SocketProcess = true;
                                if (Database.Server.ItemsBase.TryGetValue(DataItem.ITEM_ID, out DBItem))
                                {

                                    if (DataItem.ITEM_ID == 410501 || DataItem.ITEM_ID == 500301 || DataItem.ITEM_ID == 421301 || DataItem.ITEM_ID == 410301)
                                    {
                                        client.CreateBoxDialog("This item can't be upgraded.");
                                        return;
                                    }

                                    //if (DataItem.ITEM_ID % 10 == 9 || DataItem.ITEM_ID % 10 < 3)
                                    //{
                                    //    client.SendSysMesage("This item's cant be upgraded anymore.");
                                    //    return;
                                    //}
                                    //  var Chance = Database.ItemType.ChanceToUpgradeLevel(DataItem.ITEM_ID);
                                    byte Chance = 80;
                                    double sock_chance = 0;
                                    Chance -= (byte)(DBItem.Level / 10 * 3);
                                    Chance -= (byte)(((DataItem.ITEM_ID % 10) + 1) * 3);
                                    if (Chance == 0 || !client.Inventory.Contain(DataItem.ITEM_ID, 1) || DataItem == null)
                                    {
                                        client.SendSysMesage("The " + DBItem.Name + " cannot improve anymore try to visit WeaponMater at Market(182,193) ", MsgMessage.ChatMode.System);
                                        return;
                                    }

                                    bool succesed = false;

                                    uint nextItemId = Database.Server.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);

                                    if ((DBItem.Level >= 115 && Database.ItemType.Equipable(nextItemId, client) == false)
                                     && (Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.RightWeapon
                                     || Database.ItemType.ItemPosition(DataItem.ITEM_ID) == (ushort)Role.Flags.ConquerItem.LeftWeapon))
                                    {
                                        client.SendSysMesage("You can`t update this item.");
                                    }
                                    else if (DBItem.Level >= 115)
                                    {
                                        client.SendSysMesage("The " + DBItem.Name + " cannot improve anymore try to visit WeaponMater at Market(182,193) ", MsgMessage.ChatMode.System);
                                    }
                                    else
                                    {
                                        if (client.Inventory.Contain(Database.ItemType.Meteor, 1, 0))
                                        {
                                            if (Role.Core.PercentSuccess(Chance)) //46%
                                            {
                                                //dwParam = 1;
                                                client.Player.MeteorSocket += 1;
                                                DataItem.ITEM_ID = Database.Server.ItemsBase.UpdateItem(DataItem.ITEM_ID, out succesed);
                                                if (DataItem.SocketOne == Role.Flags.Gem.NoSocket)
                                                {
                                                    if (client.Player.MeteorSocket >= 1000)
                                                    {
                                                        uint thousands = client.Player.MeteorSocket / 1000;
                                                        sock_chance = thousands * 0.01; // 0.001% per 1000 //0.01
                                                    }
                                                    if (ServerKernel.ONE_SOC_RATE > 0)
                                                    {
                                                        if (client.Player.MeteorSocket >= 3500 && Role.Core.PercentSuccess(0.01 + (client.Player.BlessTime > 0 ? Global.LUCKY_TIME_BONUS_SOCKET_RATE : 0) + sock_chance))
                                                        {
                                                            client.Player.MeteorSocket = 0;
                                                            Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                                            effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                                                            effect.DropType = MsgDropID.Earth;
                                                            Program.SendGlobalPackets.Enqueue(stream.ItemPacketCreate(effect));
                                                            DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the first socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                                                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the first socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                                            //Program.DiscordSocketMaster1.Enqueue("As a very lucky player, " + client.Player.Name + " has added the first socket to his/her " + DBItem.Name + "");
                                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                                        }
                                                    }
                                                }
                                                //if (DataItem.SocketOne != Role.Flags.Gem.NoSocket && DataItem.SocketTwo != Role.Flags.Gem.NoSocket)
                                                //{
                                                //    if (Role.Core.PercentSuccess(0.00 + (client.Player.BlessTime > 0 ? Global.LUCKY_TIME_BONUS_SECOND_SOCKET_RATE : 1)))
                                                //    {
                                                //        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                                                //    effect.m_UID = (uint)Game.MsgFloorItem.MsgItemPacket.EffectMonsters.Night;
                                                //    effect.DropType = MsgDropID.Earth;
                                                //    Program.SendGlobalPackets.Enqueue(stream.ItemPacketCreate(effect));
                                                //    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                                //    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the second socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage).GetArray(stream));
                                                //    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("As a very lucky player, " + client.Player.Name + " has added the second socket to his/her " + DBItem.Name + "", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                                                //        Program.DiscordSocketMaster2.Enqueue("As a very lucky player, " + client.Player.Name + " has added the second socket to his/her " + DBItem.Name + "");
                                                //    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                                                //    }
                                                //}
                                                DataItem.Durability = DataItem.MaximDurability;
                                                DataItem.Mode = Role.Flags.ItemMode.Update;
                                                DataItem.Failed = true;
                                                DataItem.Send(client, stream);
                                                client.SendSysMesage("You have successfully upgraded the level of your " + DBItem.Name + "", MsgMessage.ChatMode.TopLeft);
                                            }
                                            else
                                            {
                                                //worked = false;
                                                client.Player.MeteorSocket += 1;
                                                if(client.Player.VipLevel != 6)
                                                {
                                                    int minDura = (int)Math.Ceiling(DataItem.Durability / 2.0);
                                                    int maxDura = DataItem.Durability;
                                                    int randomDura = Role.Core.Random.Next(minDura, maxDura);
                                                    DataItem.Durability = (ushort)randomDura;
                                                    DataItem.Mode = Role.Flags.ItemMode.Update;
                                                    DataItem.Failed = false;
                                                    DataItem.Send(client, stream);
                                                }
                                                client.SendSysMesage("You have failed to upgrade the level of your " + DBItem.Name + "", MsgMessage.ChatMode.TopLeft);

                                            }
                                        }
                                        client.Inventory.Remove(Database.ItemType.Meteor, 1, stream);
                                    }
                                }
                                if (DataItem.Position != 0)
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                //if (worked)
                                //    client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpgradeMeteor, ItemUID, dwParam, 0, 0, 0, 0));
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ReturnedItems:
                    try
                    {
                        if (!client.Inventory.HaveSpace(1))
                            client.CreateBoxDialog("Please make 1 more space in your inventory.");
                        else if (client.Warehouse.RemoveItem(id, 65535, stream))
                        {
                            client.Send(stream.ItemUsageCreate(ItemUsuageID.ReturnedItems, id, 0uL, timestamp, 1, 1, 1));
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.DegradeEquipment:
                    try
                    {
                        if (client.Inventory.TryGetItem(id, out var GameItem2) && GameItem2.IsEquip)
                        {
                            int value7;
                            value7 = 54;
                            if (client.Player.ConquerPoints >= 50000)
                            {
                                client.Player.ConquerPoints -= 50000;
                                GameItem2.ITEM_ID = Server.ItemsBase.DowngradeItem(GameItem2.ITEM_ID);
                                GameItem2.Mode = Flags.ItemMode.Update;
                                GameItem2.Send(client, stream);
                            }
                            else
                                client.SendSysMesage("You do not have 500 DSPoints with you.");
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                #region ShowItem
                case ItemUsuageID.ShowItem:
                    {
                        try
                        {
                            ShowChatItems.Item msg_item;
                            if (Program.GlobalItems.Items.TryGetValue((uint)id, out msg_item))
                            {
                                msg_item.aItem.Mode = Role.Flags.ItemMode.ChatItem;

                                msg_item.aItem.Send(client, stream);

                                msg_item.aItem.Mode = Role.Flags.ItemMode.AddItem;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteException(e);
                        }
                        break;
                    }
                #endregion
                case ItemUsuageID.ActiveItems:
                    try
                    {
                        if (client.Inventory.TryGetItem(id, out var GameItem))
                        {
                            uint iTEM_ID;
                            iTEM_ID = GameItem.ITEM_ID;
                            _ = 3005061;
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.SplitStack:
                    try
                    {
                        MsgGameItem MinorItem2;
                        MinorItem2 = null;
                        if (!client.Inventory.TryGetItem(id, out var MainItem2) || !Server.ItemsBase.TryGetValue(MainItem2.ITEM_ID, out var DBItem7) || MainItem2.StackSize <= 1 || MainItem2.StackSize > DBItem7.StackSize)
                            break;
                        if (!client.Inventory.HaveSpace(1))
                        {
                            client.SendSysMesage($"Not enough space to open the Item, please leave {1} empty spaces.");
                            break;
                        }
                        ushort Amount3;
                        Amount3 = (ushort)dwParam;
                        if (Amount3 <= DBItem7.StackSize && Amount3 > 0 && MainItem2.StackSize > Amount3)
                        {
                            MsgGameItem msgGameItem;
                            msgGameItem = MainItem2;
                            msgGameItem.StackSize -= Amount3;
                            MainItem2.Mode = Flags.ItemMode.Update;
                            MainItem2.Send(client, stream);
                            msgGameItem = new MsgGameItem();
                            msgGameItem.ITEM_ID = MainItem2.ITEM_ID;
                            MinorItem2 = msgGameItem;
                            msgGameItem = MinorItem2;
                            msgGameItem.StackSize += Amount3;
                            MinorItem2.Durability = MainItem2.Durability;
                            MinorItem2.MaximDurability = MainItem2.MaximDurability;
                            MinorItem2.Plus = MainItem2.Plus;
                            client.Inventory.Update(MinorItem2, AddMode.ADD, stream);
                        }
                        else
                        {
                            ServerKernel.Log.AppendGameLog($"{client.Player.Name} has been disconnected by detected [{action}]");
                            ServerKernel.Log.GmLog("item_usuage", $"{client.Player.Name} has been disconnected by Detected item edite.", true);
                            client.Socket.Disconnect("SHOP_EDITE");
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.MergeStackableItems:
                    try
                    {
                        if (client.Inventory.TryGetItem(id, out var MainItem) && client.Inventory.TryGetItem((uint)dwParam, out var MinorItem) && MainItem.UID != MinorItem.UID && MainItem.ITEM_ID == MinorItem.ITEM_ID && Server.ItemsBase.TryGetValue(MainItem.ITEM_ID, out var DBItem6))
                        {
                            if (MainItem.StackSize < 1)
                                MainItem.StackSize = 1;
                            if (MinorItem.StackSize < 1)
                                MinorItem.StackSize = 1;
                            if (MainItem.StackSize + MinorItem.StackSize <= DBItem6.StackSize)
                            {
                                MsgGameItem msgGameItem;
                                msgGameItem = MainItem;
                                msgGameItem.StackSize += MinorItem.StackSize;
                                MainItem.Mode = Flags.ItemMode.Update;
                                MainItem.Send(client, stream).Update(MinorItem, AddMode.REMOVE, stream, true);
                            }
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.RedeemGear:
                    {
                        Game.MsgServer.MsgDetainedItem DetainedItem;
                        if (client.Confiscator.RedeemContainer.TryGetValue((uint)id, out DetainedItem))
                        {
                            DetainedItem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(DetainedItem.Date).Ticks).Days);
                            if (DetainedItem.DaysLeft > 7)
                            {
#if Arabic
                                  client.SendSysMesage("This item is expired!");
#else
                                client.SendSysMesage("This item is expired!");
#endif

                                break;
                            }
                            if (!client.Inventory.HaveSpace(1))
                            {
#if Arabic
                                                                client.SendSysMesage("Please make 1 space in your container!");
#else
                                client.SendSysMesage("Please make 1 more space in your container!");
#endif

                                break;
                            }
                            if (client.Player.ConquerPoints >= Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) * 200)
                            {
                                client.Player.ConquerPoints -= (int)Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) * 200;


                                dwParam = client.Player.UID;
                                dwparam4 = (uint)Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) * 200;

                                client.Send(stream.ItemUsageCreate(ItemUsuageID.RedeemGear, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));


                                client.Inventory.Update(MsgDetainedItem.CopyTo(DetainedItem), Role.Instance.AddMode.ADD, stream);

#if Arabic
  string Messajj = "" + client.Player.Name + " redeemed his equipment (" + Database.Server.ItemsBase.GetItemName(DetainedItem.ItemID) + "), he paying " + Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) + " conquer points for " + DetainedItem.GainerName + ".";

#else
                                string Messajj = "" + client.Player.Name + " redeemed his equipment (" + Database.Server.ItemsBase.GetItemName(DetainedItem.ItemID) + "), he paying " + Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) * 200 + " conquer points for " + DetainedItem.GainerName + ".";

#endif

                                //string Messaj = "" + client.Player.Name + " redeemed his equipment, hereby obtained a ransom of " + DetainedItem.ConquerPointsCost + " points, all the days Stone Award to seize its equipment to help players " + DetainedItem.GainerName + "";
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(Messajj, MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));

                                if (client.Confiscator.RedeemContainer.TryRemove(DetainedItem.UID, out DetainedItem))
                                {
                                    Role.Instance.Confiscator GainerCointainer;
                                    if (Database.Server.QueueContainer.PollContainers.TryGetValue(DetainedItem.GainerUID, out GainerCointainer))
                                    {
                                        if (GainerCointainer.ClaimContainer.ContainsKey(DetainedItem.UID))
                                        {
                                            DetainedItem.Action = MsgDetainedItem.ContainerType.RewardCps;
                                            DetainedItem.RewardConquerPoints = Role.Instance.Confiscator.CalculateCpsCost(DetainedItem) * 200;
                                            GainerCointainer.ClaimContainer[DetainedItem.UID] = DetainedItem;

                                            Client.GameClient Gainer;
                                            if (Database.Server.GamePoll.TryGetValue(DetainedItem.GainerUID, out Gainer))
                                            {
                                                dwParam = Gainer.Player.UID;
                                                action = ItemUsuageID.ClaimGear;

                                                Gainer.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));



                                                GainerCointainer.ClaimContainer[DetainedItem.UID].Send(Gainer, stream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ClaimGear:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
#if Arabic
                              client.SendSysMesage("Please make 1 space in your container!");
#else
                            client.SendSysMesage("Please make 1 more space in your container!");
#endif

                            break;
                        }

                        Game.MsgServer.MsgDetainedItem ClaimItem;
                        if (client.Confiscator.ClaimContainer.TryGetValue(id, out ClaimItem))
                        {
                            if (ClaimItem.Bound && ClaimItem.DaysLeft > 7)
                            {

                                dwParam = client.Player.UID;

                                dwparam4 = (uint)ClaimItem.RewardConquerPoints;


                                client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));

                                client.Confiscator.ClaimContainer.TryRemove(ClaimItem.UID, out ClaimItem);
#if Arabic
                                 client.SendSysMesage("Unnclaimable Bound item!");
#else
                                client.SendSysMesage("Unnclaimable Bound item!");
#endif

                                break;
                            }
                            ClaimItem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Role.Instance.Confiscator.GetTimer(ClaimItem.Date).Ticks).Days);
                            if (ClaimItem.DaysLeft < 7 && ClaimItem.Action != MsgDetainedItem.ContainerType.RewardCps)
                            {
#if Arabic
                                 client.SendSysMesage("This item is not expired. You cannot claim it yet!");
#else
                                client.SendSysMesage("This item is not expired. You cannot claim it yet!");
#endif

                                break;
                            }
                            if (ClaimItem.RewardConquerPoints != 0)
                            {
                                //client.Player.ConquerPoints += (int)ClaimItem.RewardConquerPoints;

                                client.Confiscator.ClaimContainer.TryRemove(ClaimItem.UID, out ClaimItem);
                            }
                            else if (ClaimItem.DaysLeft > 7)
                            {
                                client.Inventory.Update(MsgDetainedItem.CopyTo(ClaimItem), Role.Instance.AddMode.ADD, stream);
                                client.Confiscator.ClaimContainer.TryRemove(ClaimItem.UID, out ClaimItem);
                            }
#if Arabic
                              string Messaj = "Congratulation! " + client.Player.Name + " has received " + ClaimItem.RewardConquerPoints + " conquer points for capture the red/black player called " + ClaimItem.OwnerName + "";//"Thank you for arresting red/black name players " + client.Entity.Name + " has recived " + item.ConquerPointsCost + " CPS . Congratulations!";
                           
#else
                            string Messaj = "Congratulation! " + client.Player.Name + " has received " + ClaimItem.RewardConquerPoints + " conquer points for capture the red/black player called " + ClaimItem.OwnerName + "";//"Thank you for arresting red/black name players " + client.Entity.Name + " has recived " + item.ConquerPointsCost + " CPS . Congratulations!";

#endif
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(Messaj, MsgMessage.MsgColor.white, MsgMessage.ChatMode.System).GetArray(stream));

                            dwParam = client.Player.UID;
                            dwparam4 = (uint)ClaimItem.RewardConquerPoints;
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));

                        }
                        break;
                    }
                case ItemUsuageID.GarmentShop:
                    try
                    {
                        uint GarmentID;
                        GarmentID = (uint)dwParam;
                        bool RightInfo;
                        RightInfo = true;
                        if (ItemType.ItemPosition(GarmentID) != 9)
                            break;
                        ItemType.DBItem DBItem9;
                        DBItem9 = Server.ItemsBase[GarmentID];
                        ushort Points2;
                        Points2 = 0;
                        Queue<MsgGameItem> RemoveItem;
                        RemoveItem = new Queue<MsgGameItem>((int)dwParam2);
                        foreach (uint itemUID in args)
                        {
                            if (client.Inventory.TryGetItem(itemUID, out var item8))
                            {
                                switch (item8.ITEM_ID)
                                {
                                    case 181355u:
                                    case 182435u:
                                    case 188435u:
                                        Points2 = (ushort)(Points2 + 150);
                                        break;
                                    case 183305u:
                                    case 183315u:
                                    case 183325u:
                                    case 183375u:
                                    case 191305u:
                                    case 191405u:
                                        Points2 = (ushort)(Points2 + 300);
                                        break;
                                    default:
                                        Points2 = (ushort)(Points2 + 50);
                                        break;
                                }
                                RemoveItem.Enqueue(item8);
                                continue;
                            }
                            RightInfo = false;
                            break;
                        }
                        if (!RightInfo)
                            break;
                        int price3;
                        price3 = DBItem9.ConquerPointsWorth;
                        if (Points2 >= price3)
                        {
                            client.Inventory.Add(stream, GarmentID, 1, 0, 0, 0);
                            break;
                        }
                        int value8;
                        value8 = price3 - Points2;
                        if (client.Player.ConquerPoints >= value8)
                        {
                            while (RemoveItem.Count > 0)
                            {
                                client.Inventory.Update(RemoveItem.Dequeue(), AddMode.REMOVE, stream);
                                client.Player.ConquerPoints -= value8;
                            }
                            client.Inventory.Add(stream, GarmentID, 1, 0, 0, 0);
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.AddBless:
                    try
                    {
                        if (!client.TryGetItem(id, out var DataItem3))
                            break;
                        ushort Position5;
                        Position5 = ItemType.ItemPosition(DataItem3.ITEM_ID);
                        if (!ItemType.AllowToUpdate((Flags.ConquerItem)Position5))
                            break;
                        if (Position5 == 18 && DataItem3.Bless >= 1)
                        {
                            if (Position5 == 18 && DataItem3.Bless > 1)
                            {
                                DataItem3.Bless = 1;
                                DataItem3.Mode = Flags.ItemMode.Update;
                                DataItem3.Send(client, stream);
                            }
                        }
                        else
                        {
                            if (!client.Inventory.VerifiedUpdateItem(args, ItemType.GetGemID(Flags.Gem.SuperTortoiseGem), (byte)dwParam2, out var LoseItems))
                                break;
                            byte OldBless;
                            OldBless = DataItem3.Bless;
                            if (DataItem3.Bless == 0 && LoseItems.Count == 5)
                                DataItem3.Bless = 1;
                            else if (DataItem3.Bless == 1 && LoseItems.Count == 1)
                            {
                                DataItem3.Bless = 3;
                            }
                            else if (DataItem3.Bless == 3 && LoseItems.Count == 3)
                            {
                                DataItem3.Bless = 5;
                            }
                            else if (DataItem3.Bless == 5 && LoseItems.Count == 5)
                            {
                                DataItem3.Bless = 7;
                            }
                            if (OldBless < DataItem3.Bless)
                            {
                                DataItem3.Mode = Flags.ItemMode.Update;
                                DataItem3.Send(client, stream);
                                if (DataItem3.Position != 0)
                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                while (LoseItems.Count > 0)
                                {
                                    client.Inventory.Update(LoseItems.Dequeue(), AddMode.REMOVE, stream);
                                }
                                client.Send(stream.ItemUsageCreate(ItemUsuageID.AddBless, 0, 1uL, 0, 0, 0, 0));
                                ServerKernel.Log.GmLog("bless_item", $"{client.Player.Name}({client.Player.UID}) blessed item:[id={DataItem3.ITEM_ID}, OldBless={OldBless}, NewBless={DataItem3.Bless}");
                            }
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.ToristSuper:
                    try
                    {
                         if (client.Player.Money >= 100000)
                        {
                            client.Player.Money -= 100000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            bool HaveAllGems;
                            HaveAllGems = false;
                            for (uint x4 = 0; x4 < 7; x4++)
                            {
                                uint ItemID4;
                                ItemID4 = 700002 + x4 * 10;
                                if (client.Inventory.Contain(ItemID4, 1, 0))
                                {
                                    HaveAllGems = true;
                                    continue;
                                }
                                HaveAllGems = false;
                                break;
                            }
                            if (HaveAllGems)
                            {
                                for (uint x3 = 0; x3 < 7; x3++)
                                {
                                    uint ItemID3;
                                    ItemID3 = 700002 + x3 * 10;
                                    client.Inventory.Remove(ItemID3, 1, stream);
                                }
                                client.Inventory.Add(stream, 700072, 1, 0, 0, 0);
                                client.Send(stream.ItemUsageCreate(ItemUsuageID.ToristSuper, 0, 1uL, 0, 0, 0, 0));
                            }
                        }
                        else
                            client.SendSysMesage("Sorry you don`t have 100,000 silver!.");
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.GemCompose:
                    try
                    {
                        if (id / 10000u == 70 && Enum.IsDefined(typeof(Flags.Gem), (byte)(id % 1000)))
                        {
                            int price2;
                            price2 = 0;
                            if (id % 10u == 1)
                                price2 = 10000;
                            else if (id % 10 == 2)
                            {
                                price2 = 800000;
                                if (id % 100 == 72)
                                    price2 = 1000000;
                            }
                            if (client.Player.Money >= price2)
                            {
                                if (client.Inventory.Contain(id, 15, 0) && client.Inventory.Remove(id, 15, stream))
                                {
                                    client.Player.Money -= price2;
                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                    client.Inventory.Add(stream, id + 1, 1, 0, 0, 0);
                                    dwParam = 1uL;
                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4, args));
                                }
                            }
                            else
                                client.SendSysMesage("Sorry you don`t have 100,000 silver!.");
                        }
                        else if (dwParam == 7 && dwParam2 == 500000)
                        {
                            client.Inventory.Add(stream, id, 1, (byte)dwParam3, 0, 0);
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.UpdateEnchant:
                    try
                    {
                        if (!client.TryGetItem(id, out var DataItem2))
                            break;
                        ushort Position4;
                        Position4 = ItemType.ItemPosition(DataItem2.ITEM_ID);
                        if (!ItemType.AllowToUpdate((Flags.ConquerItem)Position4) || !client.Inventory.ClientItems.TryGetValue((uint)dwParam, out var Gem) || !Enum.IsDefined(typeof(Flags.Gem), (byte)(Gem.ITEM_ID % 1000)))
                            break;
                        byte Enchant;
                        Enchant = 0;
                        switch (Gem.ITEM_ID % 10)
                        {
                            case 1u:
                                Enchant = (byte)ServerKernel.NextAsync(1, 59);
                                break;
                            case 2u:
                                Enchant = ((Gem.ITEM_ID != 700012) ? ((Gem.ITEM_ID != 700002 && Gem.ITEM_ID != 700052 && Gem.ITEM_ID != 700062) ? ((Gem.ITEM_ID != 700032) ? ((byte)ServerKernel.NextAsync(40, 89)) : ((byte)ServerKernel.NextAsync(80, 129))) : ((byte)ServerKernel.NextAsync(60, 109))) : ((byte)ServerKernel.NextAsync(100, 159)));
                                break;
                            default:
                                Enchant = ((Gem.ITEM_ID != 700013) ? ((Gem.ITEM_ID != 700003 && Gem.ITEM_ID != 700073 && Gem.ITEM_ID != 700033) ? ((Gem.ITEM_ID != 700063 && Gem.ITEM_ID != 700053) ? ((Gem.ITEM_ID != 700023) ? ((byte)ServerKernel.NextAsync(70, 119)) : ((byte)ServerKernel.NextAsync(90, 149))) : ((byte)ServerKernel.NextAsync(140, 199))) : ((byte)ServerKernel.NextAsync(170, 229))) : ((byte)ServerKernel.NextAsync(200, 256)));
                                break;
                        }
                        if (Enchant > DataItem2.Enchant)
                        {
                            DataItem2.Enchant = Enchant;
                            DataItem2.Mode = Flags.ItemMode.Update;
                            DataItem2.Send(client, stream).Update(Gem, AddMode.REMOVE, stream);
                            if (DataItem2.Position != 0)
                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                        else
                            client.Inventory.Update(Gem, AddMode.REMOVE, stream);
                        dwParam = Enchant;
                        client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        ServerKernel.Log.GmLog("item_enchant", $"{client.Player.Name}({client.Player.UID}) enchant item:[id={DataItem2.ITEM_ID}, Enchant={DataItem2.Enchant}]");
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.BuyItemFromForging:
                    try
                    {
                        client.SendSysMesage("Nice try this System is not Available");
                        return;
                        uint ItemID2;
                        ItemID2 = (uint)dwParam;
                        uint ItemsCount;
                        ItemsCount = dwParam2;
                        if ((ItemID2 != 1088001 && ItemID2 != 1088000 && ItemID2 != 730001 && ItemID2 != 730003 && ItemID2 != 730006 && ItemID2 != 723694 && ItemID2 != 723695 && ItemID2 != 700073 && ItemID2 != 1200005) || !client.Inventory.HaveSpace((byte)ItemsCount) || !Server.ItemsBase.TryGetValue(ItemID2, out var DBItem8))
                            break;
                        for (int x2 = 0; x2 < ItemsCount; x2++)
                        {
                            int value6;
                            value6 = DBItem8.ConquerPointsWorth;
                            if (client.Player.ConquerPoints >= DBItem8.ConquerPointsWorth)
                            {
                                client.Player.ConquerPoints -= DBItem8.ConquerPointsWorth;

                                if (ItemID2 % 730000u <= 9)
                                    client.Inventory.Add(DBItem8.ID, (byte)(ItemID2 % 730000), DBItem8, stream);
                                else
                                    client.Inventory.Add(DBItem8.ID, 0, DBItem8, stream);
                                continue;
                            }
                            break;
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.CreateSocketItem://bahaa
                    try
                    {
                        uint effectuid;
                        effectuid = 0;
                        uint effectdwparam1;
                        effectdwparam1 = 0;
                        switch (dwParam2)
                        {
                            case 1u:
                                {
                                    if (!client.TryGetItem(id, out var DataItem6))
                                        break;
                                    effectuid = DataItem6.UID;
                                    ushort Position8;
                                    Position8 = ItemType.ItemPosition(DataItem6.ITEM_ID);
                                    if (ItemType.AllowToUpdate((Flags.ConquerItem)Position8))
                                    {
                                        if (DataItem6.SocketOne == Flags.Gem.NoSocket)
                                        {
                                            if (client.Inventory.ClientItems.TryGetValue(args[0], out var LoseItem2) && LoseItem2.ITEM_ID == 1088001)
                                            {
                                                DataItem6.SocketOne = Flags.Gem.EmptySocket;
                                                DataItem6.Mode = Flags.ItemMode.Update;
                                                if (DataItem6.Position != 0)
                                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                                effectdwparam1 = 1;
                                                DataItem6.Send(client, stream).Update(LoseItem2, AddMode.REMOVE, stream);
                                                ServerKernel.Log.GmLog("socket_item", $"{client.Player.Name}({client.Player.UID}) socket item:[id={DataItem6.ITEM_ID}, SocketOne={DataItem6.SocketOne}");
                                                client.SendSysMesage("You successfully openned a socket in your item while upgrading.");
                                                string amessaj = "";
                                                if (Role.Core.IsBoy(client.Player.Body))
                                                    amessaj = "his";
                                                else if (Role.Core.IsGirl(client.Player.Body))
                                                    amessaj = "her";
                                                TheChosenProject.Game.MsgServer.MsgMessage messaj2 = new TheChosenProject.Game.MsgServer.MsgMessage("Congratulations, " + client.Player.Name + " get 1 Socket in " + amessaj + ", " + Database.Server.ItemsBase[DataItem6.ITEM_ID].Name + " !", TheChosenProject.Game.MsgServer.MsgMessage.MsgColor.red, TheChosenProject.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);

                                            }
                                        }
                                        else
                                        {
                                            if (DataItem6.SocketTwo != 0 || !client.Inventory.ClientItems.TryGetValue(args[0], out var LoseItem) || LoseItem.ITEM_ID != 1088001)
                                                break;
                                            if (Core.Rate(20))
                                            {
                                                effectdwparam1 = 1;
                                                DataItem6.SocketTwo = Flags.Gem.EmptySocket;
                                                DataItem6.Mode = Flags.ItemMode.Update;
                                                DataItem6.Send(client, stream).Update(LoseItem, AddMode.REMOVE, stream);
                                                if (DataItem6.Position != 0)
                                                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                                ServerKernel.Log.GmLog("socket_item", $"{client.Player.Name}({client.Player.UID}) socket item:[id={DataItem6.ITEM_ID}, SocketTwo={DataItem6.SocketTwo}");
                                                client.SendSysMesage("You successfully openned a socket in your item while upgrading.");
                                                string amessaj = "";
                                                if (Role.Core.IsBoy(client.Player.Body))
                                                    amessaj = "his";
                                                else if (Role.Core.IsGirl(client.Player.Body))
                                                    amessaj = "her";
                                                TheChosenProject.Game.MsgServer.MsgMessage messaj2 = new TheChosenProject.Game.MsgServer.MsgMessage("Congratulations, " + client.Player.Name + " get 2 Socket in " + amessaj + ", " + Database.Server.ItemsBase[DataItem6.ITEM_ID].Name + " !", TheChosenProject.Game.MsgServer.MsgMessage.MsgColor.red, TheChosenProject.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);

                                            }
                                            //else
                                            //{
                                            //    client.Inventory.Update(LoseItem, AddMode.REMOVE, stream);
                                            //    client.Inventory.Add(stream, 1200006, 1, 0, 0, 0);
                                            //}
                                        }
                                    }
                                    else
                                        client.SendSysMesage("Sorry can't make socket in this item !");
                                    break;
                                }
                            case 5u:
                                {
                                    if (!client.TryGetItem(id, out var DataItem7))
                                        break;
                                    effectuid = DataItem7.UID;
                                    ushort Position9;
                                    Position9 = ItemType.ItemPosition(DataItem7.ITEM_ID);
                                    if (ItemType.AllowToUpdate((Flags.ConquerItem)Position9))
                                    {
                                        if (DataItem7.SocketTwo == Flags.Gem.NoSocket && client.Inventory.VerifiedUpdateItem(args, 1088001, (byte)dwParam2, out var LoseItems4))
                                        {
                                            effectdwparam1 = 1;
                                            DataItem7.SocketTwo = Flags.Gem.EmptySocket;
                                            DataItem7.Mode = Flags.ItemMode.Update;
                                            DataItem7.Send(client, stream);
                                            if (DataItem7.Position != 0)
                                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                            ServerKernel.Log.GmLog("socket_item", $"{client.Player.Name}({client.Player.UID}) socket item:[id={DataItem7.ITEM_ID}, SocketTwo={DataItem7.SocketTwo}");
                                            while (LoseItems4.Count > 0)
                                            {
                                                client.Inventory.Update(LoseItems4.Dequeue(), AddMode.REMOVE, stream);
                                            }
                                            client.SendSysMesage("You successfully openned a socket in your item while upgrading.");
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(client.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(client.Player.Body))
                                                amessaj = "her";
                                            TheChosenProject.Game.MsgServer.MsgMessage messaj2 = new TheChosenProject.Game.MsgServer.MsgMessage("Congratulations, " + client.Player.Name + " get 2 Socket in " + amessaj + ", " + Database.Server.ItemsBase[DataItem7.ITEM_ID].Name + " !", TheChosenProject.Game.MsgServer.MsgMessage.MsgColor.red, TheChosenProject.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);

                                        }
                                    }
                                    else
                                        client.SendSysMesage("Sorry can't make socket in this item !");
                                    break;
                                }
                            case 7u:
                                {
                                    if (!client.TryGetItem(id, out var DataItem4))
                                        break;
                                    effectuid = DataItem4.UID;
                                    ushort Position6;
                                    Position6 = ItemType.ItemPosition(DataItem4.ITEM_ID);
                                    if (ItemType.AllowToUpdate((Flags.ConquerItem)Position6))
                                    {
                                        if (DataItem4.SocketTwo == Flags.Gem.NoSocket && client.Inventory.VerifiedUpdateItem(args, 1088001, (byte)dwParam2, out var LoseItems2))
                                        {
                                            effectdwparam1 = 1;
                                            DataItem4.SocketTwo = Flags.Gem.EmptySocket;
                                            DataItem4.Mode = Flags.ItemMode.Update;
                                            DataItem4.Send(client, stream);
                                            if (DataItem4.Position != 0)
                                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                            ServerKernel.Log.GmLog("socket_item", $"{client.Player.Name}({client.Player.UID}) socket item:[id={DataItem4.ITEM_ID}, SocketTwo={DataItem4.SocketTwo}");
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(client.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(client.Player.Body))
                                                amessaj = "her";
                                            TheChosenProject.Game.MsgServer.MsgMessage messaj2 = new TheChosenProject.Game.MsgServer.MsgMessage("Congratulations, " + client.Player.Name + " get 2 Socket in " + amessaj + ", " + Database.Server.ItemsBase[DataItem4.ITEM_ID].Name + " !", TheChosenProject.Game.MsgServer.MsgMessage.MsgColor.red, TheChosenProject.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);

                                            while (LoseItems2.Count > 0)
                                            {
                                                client.Inventory.Update(LoseItems2.Dequeue(), AddMode.REMOVE, stream);
                                            }
                                        }
                                    }
                                    else
                                        client.SendSysMesage("Sorry can't make socket in this item !");
                                    break;
                                }
                            case 12u:
                                {
                                    if (!client.TryGetItem(id, out var DataItem5))
                                        break;
                                    effectuid = DataItem5.UID;
                                    ushort Position7;
                                    Position7 = ItemType.ItemPosition(DataItem5.ITEM_ID);
                                    if (ItemType.AllowToUpdate((Flags.ConquerItem)Position7))
                                    {
                                        if (DataItem5.SocketOne == Flags.Gem.NoSocket && client.Inventory.VerifiedUpdateItem(args, 1088001, (byte)dwParam2, out var LoseItems3))
                                        {
                                            effectdwparam1 = 1;
                                            DataItem5.SocketOne = Flags.Gem.EmptySocket;
                                            DataItem5.Mode = Flags.ItemMode.Update;
                                            DataItem5.Send(client, stream);
                                            if (DataItem5.Position != 0)
                                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                                            while (LoseItems3.Count > 0)
                                            {
                                                client.Inventory.Update(LoseItems3.Dequeue(), AddMode.REMOVE, stream);
                                            }
                                        }
                                    }
                                    else
                                        client.SendSysMesage("Sorry can't make socket in this item !");
                                    break;
                                }
                        }
                        client.Send(stream.ItemUsageCreate(ItemUsuageID.CreateSocketItem, effectuid, effectdwparam1, 0, 0, 0, 0));
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.RemoveVendingItem:
                    try
                    {
                        if (client.IsVendor && client.MyVendor.Items.TryRemove(id, out var _))
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.BuyVendingItem:
                    {
                       
                        if (client.Inventory.HaveSpace(1))
                        {
                            Role.IMapObj Obj;
                            if (client.Player.View.TryGetValue((uint)dwParam, out Obj, Role.MapObjectType.SobNpc))
                            {
                                Role.SobNpc npc = Obj as Role.SobNpc;
                                if (npc.OwnerVendor.IsVendor)
                                {
                                    if (npc.OwnerVendor.Fake && npc.OwnerVendor.Bot != null)
                                    {
                                        if (npc.OwnerVendor.Bot.OwnerUID == client.Player.UID)
                                            return;
                                    }
                                    if (npc.OwnerVendor.Player.UID == client.Player.UID)
                                        return;
                                    if (npc.OwnerVendor.Inventory.ClientItems.ContainsKey(id) || npc.OwnerVendor.Fake)
                                    {
                                        Role.Instance.Vendor.VendorItem VItem = null;
                                        if (npc.OwnerVendor.MyVendor.Items.TryGetValue(id, out VItem))
                                        {
                                            bool RightBuy = false;
                                            if (VItem.CostType == MsgItemView.ActionMode.CPs)
                                            {
                                                if (RightBuy = (client.Player.ConquerPoints >= VItem.AmountCost))
                                                {
                                                    client.Player.ConquerPoints -= (int)VItem.AmountCost;
                                                    npc.OwnerVendor.Player.ConquerPoints += (int)VItem.AmountCost;
                                                }
                                            }
                                            else if (VItem.CostType == MsgItemView.ActionMode.Gold)
                                            {
                                                if (RightBuy = (client.Player.Money >= VItem.AmountCost))
                                                {
                                                    client.Player.Money -= (int)VItem.AmountCost;
                                                    npc.OwnerVendor.Player.Money += (int)VItem.AmountCost;

                                                    client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                                    npc.OwnerVendor.Player.SendUpdate(stream, npc.OwnerVendor.Player.Money, MsgUpdate.DataType.Money);
                                                }
                                            }
                                            if (RightBuy)
                                            {
                                                if (npc.OwnerVendor.MyVendor.Items.TryRemove(id, out VItem))
                                                {
                                                    client.Inventory.Update(VItem.DataItem, Role.Instance.AddMode.MOVE, stream);



                                                    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));

                                                    action = ItemUsuageID.RemoveVendingItem;
                                                    npc.OwnerVendor.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));


                                                    npc.OwnerVendor.Inventory.Update(VItem.DataItem, Role.Instance.AddMode.REMOVE, stream, true);


                                                    var sellit = Database.Server.ItemsBase[VItem.DataItem.ITEM_ID];
#if Arabic
                                                           string Messaj = "" + npc.OwnerVendor.Player.Name + " just sold " + sellit.Name + " to " + client.Player.Name + " for " + VItem.AmountCost + (VItem.CostType == MsgItemView.ActionMode.CPs ? " ConquerPoints." : " Gold.");
                                              
#else
                                                    string Messaj = "" + npc.OwnerVendor.Player.Name + " just sold " + sellit.Name + " to " + client.Player.Name + " for " + VItem.AmountCost + (VItem.CostType == MsgItemView.ActionMode.CPs ? " ConquerPoints." : " Gold.");

#endif
                                                    client.SendSysMesage(Messaj, MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red, true);
                                                    if (npc.OwnerVendor.Player.OfflineTraining == MsgOfflineTraining.Mode.Shopping && npc.OwnerVendor.MyVendor.Items.Count == 0)
                                                    {
                                                        npc.OwnerVendor.Player.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                                                        npc.OwnerVendor.Socket.Disconnect(npc.OwnerVendor.Player.OfflineTraining.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.ShowVendingList:
                    try
                    {
                        if (!client.Player.View.TryGetValue(id, out var Obj, MapObjectType.SobNpc))
                            break;
                        SobNpc npc;
                        npc = Obj as SobNpc;
                        if (!npc.OwnerVendor.IsVendor)
                            break;
                        MsgItemExtra itemExtra;
                        itemExtra = new MsgItemExtra();
                        foreach (Vendor.VendorItem item3 in npc.OwnerVendor.MyVendor.Items.Values)
                        {
                            client.Send(stream.ItemViewCreate(npc.UID, item3.AmountCost, item3.DataItem, item3.CostType));
                            if (item3.DataItem.Refinary.InLife)
                            {
                                item3.DataItem.Refinary.Typ = MsgItemExtra.Typing.RefinaryAdding;
                                if (item3.DataItem.Refinary.EffectDuration == 0)
                                    item3.DataItem.Refinary.Typ = MsgItemExtra.Typing.PermanentRefinery;
                                itemExtra.Refinerys.Add(item3.DataItem.Refinary);
                            }
                            if (item3.DataItem.Purification.InLife)
                            {
                                item3.DataItem.Purification.Typ = MsgItemExtra.Typing.PurificationAdding;
                                itemExtra.Purifications.Add(item3.DataItem.Purification);
                            }
                        }
                        if (itemExtra.Refinerys.Count != 0 || itemExtra.Purifications.Count != 0)
                            client.Send(itemExtra.CreateArray(stream));
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.AddVendingItemConquerPts:
                    try
                    {
                        if (client.IsVendor && client.Player.VerifiedPassword && client.Inventory.TryGetItem(id, out var item5) && !ItemType.unabletradeitem.Contains(item5.ITEM_ID) && !ItemType.undropeitem.Contains(item5.ITEM_ID) && client.MyVendor.AddItem(item5, MsgItemView.ActionMode.CPs, (uint)dwParam))
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4, args));
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.AddVendingItemGold:
                    try
                    {
                        if (client.IsVendor && client.Player.VerifiedPassword && client.Inventory.TryGetItem(id, out var item4) && !ItemType.undropeitem.Contains(item4.ITEM_ID) && !ItemType.unabletradeitem.Contains(item4.ITEM_ID) && client.MyVendor.AddItem(item4, MsgItemView.ActionMode.Gold, (uint)dwParam))
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4, args));
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.DropItem:
                    try
                    {
                        if (client.InTrade || !client.Player.VerifiedPassword || !client.Inventory.TryGetItem(id, out var item6) /*|| ShopFile.Not_Allowed_Dropped.Contains(item6.ITEM_ID)*/ || item6.Locked > 0 || item6.Inscribed != 0)
                            break;
                        if(item6.ITEM_ID == 2100245 || item6.ITEM_ID >= 722510 && item6.ITEM_ID <= 722515)
                        {
                            client.CreateBoxDialog("This Item cannot be dropped");
                            return;
                        }
                        if (ItemType.undropeitem.Contains(item6.ITEM_ID) ||ItemType.unabletradeitem.Contains(item6.ITEM_ID) || item6.Bound >= 1)
                        {
                            client.Inventory.Update(item6, AddMode.REMOVE, stream, true);
                            break;
                        }
                        ushort x;
                        x = client.Player.X;
                        ushort y;
                        y = client.Player.Y;
                        if (client.Map.AddGroundItem(ref x, ref y, 0))
                        {
                            client.Inventory.Update(item6, AddMode.REMOVE, stream, true);
                            MsgItem DropItem;
                            DropItem = new MsgItem(item6, x, y, MsgItem.ItemType.Item, 0, client.Player.DynamicID, client.Player.Map, client.Player.UID, false, client.Map);
                            if (client.Map.EnqueueItem(DropItem))
                                DropItem.SendAll(stream, MsgDropID.Visible);
                            ServerKernel.Log.GmLog("drop_item", $"{client.Player.Name}({client.Player.UID}) drop item:[id={item6.ITEM_ID}, dur={item6.Durability}, max_dur={item6.MaximDurability}");
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.VIPRepairItem:
                    {
                        
                        foreach (var item in client.Equipment.ClientItems.Values)
                        {
                            if (Database.ItemType.IsArrow(item.ITEM_ID))
                            {
                                client.CreateBoxDialog("Hey fool, you can't repair arrow don't be stupid!.");
                                return;
                            }
                            if (!Database.ItemType.Equipable(item, client))
                                continue;
                            if (item.Suspicious > 0)
                                continue;
                            //if (client.TryGetItem(id, out item))
                            {
                                if (item.Durability > 0 && item.Durability < item.MaximDurability)
                                {
                                    int Price = Database.Server.ItemsBase[item.ITEM_ID].GoldWorth;
                                    byte Quality = (byte)(item.ITEM_ID % 10);
                                    double QualityMultipier = 0;

                                    switch (Quality)
                                    {
                                        case 9: QualityMultipier = 1.125; break;
                                        case 8: QualityMultipier = 0.975; break;
                                        case 7: QualityMultipier = 0.9; break;
                                        case 6: QualityMultipier = 0.825; break;
                                        default: QualityMultipier = 0.75; break;
                                    }

                                    int nRepairCost = 0;
                                    if (Price > 0)
                                        nRepairCost = (int)Math.Ceiling((Price * (item.MaximDurability - item.Durability) / item.MaximDurability) * QualityMultipier);

                                    nRepairCost = Math.Max(1, nRepairCost);
                                    if (client.Player.Money >= nRepairCost)
                                    {
                                        client.Player.Money -= (int)nRepairCost;
                                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                                        item.Durability = item.MaximDurability;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                    }
                                }
                                /*else if (item.Durability == 0)
                                {
                                    if (client.Inventory.Remove(1088001, 5, stream))
                                    {
                                        item.Durability = item.MaximDurability;
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                    }
                                }*/
                            }
                        }
                        break;
                    }

                case ItemUsuageID.RepairItem:
                    try
                    {
                        if (!client.TryGetItem(id, out var item2))
                            break;

                        if (Database.ItemType.IsArrow(item2.ITEM_ID))
                        {
                            client.CreateBoxDialog("Hey , you can't repair arrow please removed it first!.");
                            return;
                        }
                        if (item2.Durability > 0 && item2.Durability < item2.MaximDurability)
                        {
                            int Price;
                            Price = Server.ItemsBase[item2.ITEM_ID].GoldWorth;
                            byte Quality;
                            Quality = (byte)(item2.ITEM_ID % 10);
                            double QualityMultipier;
                            QualityMultipier = 0.0;
                            switch (Quality)
                            {
                                case 9:
                                    QualityMultipier = 1.125;
                                    break;
                                case 8:
                                    QualityMultipier = 0.975;
                                    break;
                                case 7:
                                    QualityMultipier = 0.9;
                                    break;
                                case 6:
                                    QualityMultipier = 0.825;
                                    break;
                                default:
                                    QualityMultipier = 0.75;
                                    break;
                            }
                            int nRepairCost;
                            nRepairCost = 0;
                            if (Price != 0)
                                nRepairCost = (int)Math.Ceiling((double)(Price * (item2.MaximDurability - item2.Durability) / (long)item2.MaximDurability) * QualityMultipier);
                            nRepairCost = Math.Max(1, nRepairCost);
                            if (client.Player.Money >= nRepairCost)
                            {
                                client.Player.Money -= nRepairCost;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                item2.Durability = item2.MaximDurability;
                                item2.Mode = Flags.ItemMode.Update;
                                item2.Send(client, stream);
                            }
                        }
                        else if (item2.Durability == 0 && (client.Inventory.Remove(1088000, 2, stream) && client.Inventory.Remove(1088001, 5, stream)))
                        {
                            item2.Durability = item2.MaximDurability;
                            item2.Mode = Flags.ItemMode.Update;
                            item2.Send(client, stream);
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.ShowWarehouseMoney:
                    {
                       
                        dwParam = (ulong)client.Player.WHMoney;

                        client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        break;
                    }
                case ItemUsuageID.DepositWarehouse:
                    {
                       
                        if (client.Player.Money > (long)dwParam)
                        {

                            client.Player.WHMoney += (uint)dwParam;
                            client.Player.Money -= (int)dwParam;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        }
                        break;
                    }
                case ItemUsuageID.WarehouseWithdraw:
                    {
                       
                        if (client.Player.WHMoney >= (long)dwParam)
                        {
                            client.Player.Money += (int)dwParam;
                            client.Player.WHMoney -= (uint)dwParam;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                            client.Player.SendUpdate(stream, client.Player.WHMoney, MsgUpdate.DataType.WHMoney);
                            client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        }
                        break;
                    }
                case ItemUsuageID.SocketTalismanWithCPs:
                    {
                        MsgGameItem Talisman = null;
                        if (client.TryGetItem(id, out Talisman))
                        {
                            uint price = 0;
                            if (Talisman.SocketOne == Role.Flags.Gem.NoSocket)
                            {
                                double procent = 100 - (Talisman.SocketProgress * 25600 / 2048000);
                                if (100 - procent < 25)
                                    return;
                                price = (uint)(procent * 55);
                            }
                            else if (Talisman.SocketTwo == Role.Flags.Gem.NoSocket)
                            {
                                double procent = 100 - (Talisman.SocketProgress * 25600 / 5120000);
                                if (100 - procent < 25)
                                    return;
                                price = (uint)(procent * 110);
                            }
                            else
                                return;
                            if (client.Player.ConquerPoints >= price)
                            {
                                client.Player.ConquerPoints -= (int)price;

                                if (Talisman.SocketOne == Role.Flags.Gem.NoSocket)
                                    Talisman.SocketOne = Role.Flags.Gem.EmptySocket;
                                else if (Talisman.SocketTwo == Role.Flags.Gem.NoSocket)
                                    Talisman.SocketTwo = Role.Flags.Gem.EmptySocket;
                                Talisman.SocketProgress = 0;
                                Talisman.Mode = Role.Flags.ItemMode.Update;
                                Talisman.Send(client, stream);
                            }
                        }
                        break;
                    }
                case ItemUsuageID.SocketTalismanWithItem:
                    {
                      
                        MsgGameItem Talisman = null;
                        if (client.TryGetItem(id, out Talisman))
                        {
                            if (!Database.ItemType.IsTalisman(Talisman.ITEM_ID))
                                return;
                            foreach (var itemUID in args)
                            {
                                MsgGameItem Item = null;
                                if (client.Inventory.TryGetItem(itemUID, out Item))
                                {
                                    if (Item.ITEM_ID / 1000 == Talisman.ITEM_ID / 1000 || Item.Bound == 1 || Talisman.SocketTwo != Role.Flags.Gem.NoSocket)
                                        return;
                                    uint Points = 0;
                                    switch (Item.ITEM_ID % 10)
                                    {
                                        case 6: Points += 5; break;
                                        case 7: Points += 10; break;
                                        case 8: Points += 40; break;
                                        case 9: Points += 1000; break;
                                    }
                                    Points += (uint)Database.ItemType.TalismanExtra[Math.Min(Item.Plus, (byte)12)];

                                    int position = Database.ItemType.ItemPosition(Item.ITEM_ID);
                                    switch (position)
                                    {
                                        case 0: return;
                                        case 4:
                                        case 5:
                                            if (Item.ITEM_ID % 10 >= 8)
                                            {
                                                if (Item.SocketOne != Role.Flags.Gem.NoSocket)
                                                    Points += 160;
                                                if (Item.SocketTwo != Role.Flags.Gem.NoSocket)
                                                    Points += 800;
                                            }
                                            break;
                                        default:
                                            if (Item.ITEM_ID % 10 >= 8)
                                            {
                                                if (Item.SocketOne != Role.Flags.Gem.NoSocket)
                                                    Points += 2000;
                                                if (Item.SocketTwo != Role.Flags.Gem.NoSocket)
                                                    Points += 6000;
                                            }
                                            break;
                                    }
                                    Talisman.SocketProgress += (int)Points;
                                    if (Talisman.SocketOne == Role.Flags.Gem.NoSocket)
                                    {
                                        if (Talisman.SocketProgress >= 8000)
                                        {
                                            Talisman.SocketProgress -= 8000;
                                            Talisman.SocketOne = Role.Flags.Gem.EmptySocket;
                                        }
                                    }
                                    if (Talisman.SocketOne != Role.Flags.Gem.NoSocket)
                                    {
                                        if (Talisman.SocketProgress >= 20000)
                                        {
                                            Talisman.SocketProgress = 0;
                                            Talisman.SocketTwo = Role.Flags.Gem.EmptySocket;
                                        }
                                    }
                                    Talisman.Mode = Role.Flags.ItemMode.Update;
                                    Talisman.Send(client, stream).Update(Item, Role.Instance.AddMode.REMOVE, stream);
                                }
                            }
                        }
                        break;
                    }
                case ItemUsuageID.SellItem:
                    try
                    {
                        if (!client.Player.VerifiedPassword)
                            break;
                        uint ShopUID2;
                        ShopUID2 = id;
                        uint ItemUID;
                        ItemUID = (uint)dwParam;
                        if (!client.Map.SearchNpcInScreen(ShopUID2, client.Player.X, client.Player.Y, out var _) || !client.Inventory.TryGetItem(ItemUID, out var Item))
                            break;
                        if (Item.Locked > 0 || Item.Inscribed != 0)
                            client.SendSysMesage("This item can't be sold.");
                        else
                        {
                            if (!Server.ItemsBase.TryGetValue(Item.ITEM_ID, out var DBItem3))
                                break;
                            int price;
                            price = (DBItem3.GoldWorth / 3);
                            if (price > 0)
                            {
                                if (Item.Durability < Item.MaximDurability)
                                    price = price * Item.Durability / (int)Item.MaximDurability;
                                if (Item.Durability > 0 && Item.Durability <= Item.MaximDurability)
                                {
                                    client.Inventory.Update(Item, AddMode.REMOVE, stream);
                                    //if (DBItem3.Level < 110)
                                    //{
                                        client.Player.Money += price;
                                        client.SendSysMesage($"You sold {DBItem3.Name} for {price} gold!");
                                    //}
                                    //else
                                    //{
                                    //    client.Player.Money += price;
                                    //    client.SendSysMesage($"Item sold {price} gold!");
                                    //    // This is the dumb shit that u can remove or add and it will give cash
                                    //}
                                }
                                else
                                    client.Inventory.Update(Item, AddMode.REMOVE, stream);
                            }
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.BuyItem:
                    try
                    {
                        uint ShopUID;
                        ShopUID = id;
                        uint ItemID;
                        ItemID = (uint)dwParam;
                        uint Counts;
                        Counts = dwParam2;
                        if (ShopUID == 0)
                            break;
                        if (!client.Player.Alive)
                        {
                            client.SendSysMesage("You are not alive.");
                            break;
                        }
                        if (client.EventBase != null)
                        {
                            var events = Program.Events.Find(x => x.EventTitle == client.EventBase.EventTitle);
                            if (events.InTournament(client))
                            {
                                client.SendSysMesage("Item can't be buy during the event.");
                                return;
                            }
                        }

                        switch (ShopUID)
                        {
                            case 6000u:
                                {
                                    HonorShop.HonorShopBase shop2;
                                    shop2 = HonorShop.Shop;
                                    if (!shop2.Items.TryGetValue(ItemID, out var cost))
                                        break;
                                    if (client.Inventory.HaveSpace(1))
                                    {
                                        if (cost != 0)
                                        {
                                            if (client.HonorPoints >= cost)
                                            {
                                                client.HonorPoints -= cost;
                                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0, Flags.Gem.EmptySocket, Flags.Gem.EmptySocket, true);
                                                client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
                                            }
                                            else
                                                client.SendSysMesage("You do not have enough Honor Points");
                                        }
                                    }
                                    else
                                        client.SendSysMesage("Your inventory is full.");
                                    break;
                                }
                            case 6001u:
                                {
                                    if (!RacePointShop.Shop.Items.TryGetValue(ItemID, out var item) || item.Price * dwParam2 == 0)
                                        break;
                                    if (client.Inventory.HaveSpace((byte)dwParam2))
                                    {
                                        if (client.Player.ChampionPoints >= item.Price * dwParam2)
                                        {
                                            Player player;
                                            player = client.Player;
                                            player.ChampionPoints -= (int)item.Price * (int)dwParam2;
                                            client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                                            client.Inventory.Add(stream, item.ID, (byte)dwParam2, 0, 0, 0);
                                        }
                                        else
                                            client.SendSysMesage("You don't have {0} Donation Points!");
                                    }
                                    else
                                        client.SendSysMesage("Your inventory is full.");
                                    break;
                                }
                            default:
                                {
                                    if (!client.Map.SearchNpcInScreen(ShopUID, client.Player.X, client.Player.Y, out var _) && ShopUID != 2888)
                                        break;
                                    ShopFile.Shop shop;
                                    shop = new ShopFile.Shop();
                                    if (!ShopFile.Shops.TryGetValue(ShopUID, out shop) && !EShopFile.Shops.TryGetValue(ShopUID, out shop))
                                        shop = null;
                                    if (shop == null || shop.UID == 0)
                                        break;
                                    if (dwparam4 == 2)
                                    {
                                        if (shop.Items.Contains(ItemID) && Server.ItemsBase.TryGetValue(ItemID, out var _))
                                        {
                                            for (uint Amount2 = ((Counts == 0) ? 1u : Counts); Amount2 != 0; Amount2--)
                                            {
                                                ShopFile.MoneyType moneyType;
                                                moneyType = shop.MoneyType;
                                                _ = 3;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!shop.Items.Contains(ItemID) || !Server.ItemsBase.TryGetValue(ItemID, out var DBItem))
                                            break;
                                        for (uint Amount = ((Counts == 0) ? 1u : Counts); Amount != 0; Amount--)
                                        {
                                            switch (shop.MoneyType)
                                            {
                                                case ShopFile.MoneyType.ConquerPoints:
                                                    {
                                                        switch (shop.UID)
                                                        {
                                                            case 55170:
                                                                if (client.Player.Money < DBItem.ConquerPointsWorth)
                                                                {
                                                                    client.Player.MessageBox("You got " + client.Player.Money.ToString("0,0") + " GOLD its not enough to buy this item worth "+ DBItem.ConquerPointsWorth.ToString("0,0") + " GOLD", delegate
                                                                    {

                                                                    }, null);
                                                                }
                                                                else
                                                                {
                                                                    client.Player.MessageBox("You got " + client.Player.Money.ToString("0,0") + " GOLD Click 'OK' to pay\n(" + DBItem.Name + ")", delegate
                                                                    {
                                                                        if (client.Player.Money >= DBItem.ConquerPointsWorth)
                                                                        {
                                                                            client.Player.Money -= DBItem.ConquerPointsWorth;
                                                                            client.Inventory.Add(DBItem.ID, 0, DBItem, stream);
                                                                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.RaceShopPoints);
                                                                        }
                                                                    }, null);
                                                                }
                                                                break;
                                                                //if (DBItem.ConquerPointsWorth < 1)
                                                                //    break;
                                                                //int value;
                                                                //value = DBItem.ConquerPointsWorth;
                                                                //if (client.Player.ConquerPoints >= DBItem.ConquerPointsWorth)
                                                                //{
                                                                //    byte plus;
                                                                //    plus = 0;
                                                                //    if (DBItem.ID >= 730001 && DBItem.ID <= 730008)
                                                                //        plus = (byte)(DBItem.ID % 10);
                                                                //    client.Player.ConquerPoints -= DBItem.ConquerPointsWorth;
                                                                //    client.Inventory.Add(DBItem.ID, plus, DBItem, stream);
                                                                //}
                                                        }
                                                        break;
                                                    }
                                                case ShopFile.MoneyType.Gold:
                                                    if (DBItem.GoldWorth < 1)
                                                        break;
                                                    switch (shop.UID)
                                                    {
                                                        case 433:
                                                        case 2302:
                                                        case 2303:
                                                        case 2304:
                                                        case 2305:
                                                        case 2306:
                                                        case 6572:
                                                        case 999998:
                                                        case 8157:
                                                        case 9264:
                                                            switch ((Flags.ConquerItem)ItemType.ItemPosition(DBItem.ID))
                                                            {
                                                                case Flags.ConquerItem.Garment:
                                                                    {
                                                                        bool canequip;
                                                                        canequip = false;
                                                                        int ClientGender;
                                                                        ClientGender = (((int)client.Player.Body % 10000 < 1005) ? 1 : 2);
                                                                        if (DBItem.Gender == 2 && ClientGender == 2)
                                                                            canequip = true;
                                                                        if (DBItem.Gender == 1 && ClientGender == 1)
                                                                            canequip = true;
                                                                        if (DBItem.Gender == 0)
                                                                            canequip = true;
                                                                        if (!canequip)
                                                                            break;
                                                                        client.Player.AddSpecialGarment(stream, DBItem.ID);
                                                                        if (client.Player.ChampionPoints < DBItem.GoldWorth)
                                                                            break;
                                                                        client.Player.MessageBox("You got " + client.Player.ChampionPoints + " Divine Points Click 'OK' to pay\n(" + DBItem.Name + ")", delegate
                                                                        {
                                                                            if (client.Player.ChampionPoints >= DBItem.GoldWorth && client.Player.Map == 1009)
                                                                            {
                                                                                client.Player.ChampionPoints -= DBItem.GoldWorth;
                                                                                client.Inventory.Add(DBItem.ID, 0, DBItem, stream);
                                                                                client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                                                                            }
                                                                        }, null);
                                                                        break;
                                                                    }
                                                                case Flags.ConquerItem.RightWeaponAccessory:
                                                                case Flags.ConquerItem.LeftWeaponAccessory:
                                                                    client.Player.AddSpecialAccessory(stream, DBItem.ID);
                                                                    if (client.Player.ChampionPoints < DBItem.GoldWorth)
                                                                        break;
                                                                    client.Player.MessageBox("Click 'OK' to pay 1x(" + DBItem.Name + ")", delegate
                                                                    {
                                                                        if (client.Player.ChampionPoints >= DBItem.GoldWorth && client.Player.Map == 1009)
                                                                        {
                                                                            client.Player.ChampionPoints -= DBItem.GoldWorth;
                                                                            client.Inventory.Add(DBItem.ID, 0, DBItem, stream);
                                                                            client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                                                                        }
                                                                    }, null);
                                                                    break;
                                                            }
                                                            break;
                                                        case 2888:
                                                            if (client.Player.ChampionPoints < DBItem.GoldWorth)
                                                            {
                                                                client.Player.MessageBox("You got " + client.Player.ChampionPoints + " Divine Points its not enough to buy this item", delegate
                                                                {
                                                                 
                                                                }, null);
                                                            }
                                                            else
                                                            {
                                                                client.Player.MessageBox("You got " + client.Player.ChampionPoints + " Divine Points Click 'OK' to pay\n(" + DBItem.Name + ")", delegate
                                                                {
                                                                    if (client.Player.ChampionPoints >= DBItem.GoldWorth)
                                                                    {
                                                                        client.Player.ChampionPoints -= DBItem.GoldWorth;
                                                                        client.Inventory.Add(DBItem.ID, 0, DBItem, stream);
                                                                        client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                                                                    }
                                                                }, null);
                                                            }
                                                            break;
                                                        default:
                                                            if (DBItem.GoldWorth != 0 && client.Player.Money >= DBItem.GoldWorth)
                                                                client.Player.Money -= DBItem.GoldWorth;
                                                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                                                            client.Inventory.Add(DBItem.ID, 0, DBItem, stream);
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.Ping:
                    {
                        switch (LoaderMessage)
                        {
                            case 0://Loader is not attached or the breakpoint didn't hit successfully
                                {
                                    client.Socket.Disconnect();
                                    Console.WriteLine(client.Player.Name + " has been disconnected due to ping packet issue.");
                                    break;
                                }
                            case 0x10://Speed hack suspect
                                {
                                    if (!GuardShield.MsgCheatingPacket.UpdateSpeedFlags(client))
                                    {
                                        Console.WriteLine("Account: " + client.Player.Name + " has been banned due to using speed hacks (method 2).");
                                        client.Socket.Disconnect();
                                    }
                                    break;
                                }
                            case 0x1://Normal value
                                {
                                    break;
                                }
                        }
                        client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        //if (client.ProjectManager)
                        //    break;
                        //Console.WriteLine("Last Rec: " +);
                        int p = (DateTime.Now - client.Player.ReceivePing).Seconds;
                        if (p <= 5)
                        {
                            client.Player.ReceiveTest++;
                            client.Player.LastSuspect = DateTime.Now;
                        }
                        client.Player.ReceivePing = DateTime.Now;
                        if (client.Player.ReceiveTest >= 5)
                        {
                            client.Socket.Disconnect();
                            Console.WriteLine("Player suspected speed hack : " + client.Player.Name);
                        }
                        //if (DateTime.Now > client.Player.ReceivePing.AddSeconds(4))
                        //{
                        //    client.Player.ReceivePing = DateTime.Now;
                        //    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        //}
                        //else
                        //{
                        //    client.Player.ReceiveTest++;
                        //    client.Send(stream.ItemUsageCreate(action, id, dwParam, timestamp, dwParam2, dwParam3, dwparam4));
                        //    if (client.Player.ReceiveTest > 5)
                        //    {
                        //       // client.Socket.Disconnect();
                        //        Console.WriteLine(client.Player.Name + " -> Use Speed Hack from Cheat Engine");
                        //    }
                        //}
                        break;


                    }
                case ItemUsuageID.Unequip:
                    try
                    {
                        if (client.EventBase != null)
                        {
                            var events = Program.Events.Find(x => x.EventTitle == client.EventBase.EventTitle);
                            if (events != null)
                            {
                                if (events.InTournament(client))
                                {
                                    client.SendSysMesage("Item can't be unequiped during the event.");
                                    return;
                                }
                            }
                            else client.EventBase = null;
                        }
                        if (client.Player != null)
                        {
                            if (client.Player.Map == 1121)
                            {
                                client.SendSysMesage("Item can't be unequiped during the event.");
                                return;
                            }
                        }
                        uint Position2;
                        Position2 = (uint)dwParam;
                        if ((Position2 != 9 || client.Player.SpecialGarment == 0) && ((Position2 != 15 && Position2 != 4) || client.Player.RightSpecialAccessory == 0) && ((Position2 != 16 && Position2 != 5) || client.Player.LeftSpecialAccessory == 0) && client.Equipment.Remove((Flags.ConquerItem)Position2, stream))
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.Equip:
                    try
                    {
                        uint Position;
                        Position = (uint)dwParam;
                        if ((Position != 9 || client.Player.SpecialGarment == 0) && ((Position != 15 && Position != 4) || client.Player.RightSpecialAccessory == 0) && ((Position != 16 && Position != 5) || client.Player.LeftSpecialAccessory == 0))
                        {
                            if (dwParam < 20)
                                EquipItem(client, stream, id, (uint)dwParam, timestamp, dwParam2, dwParam3, dwparam4);
                            else
                                AlternanteEquipItem(client, stream, id, (uint)dwParam, timestamp, dwParam2, dwParam3, dwparam4);
                        }
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                case ItemUsuageID.UnAlternante:
                case ItemUsuageID.Alternante:
                    try
                    {
                        if (client.Player.ContainFlag(MsgUpdate.Flags.Fly) || client.Player.RightSpecialAccessory != 0 || client.Player.LeftSpecialAccessory != 0 || client.Player.SpecialGarment != 0)
                            break;
                        if (client.EventBase != null)
                        {
                            var events = Program.Events.Find(x => x.EventTitle == client.EventBase.EventTitle);
                            if (events != null)
                            {
                                if (events.InTournament(client))
                                {
                                    client.SendSysMesage("Item can't be switch equipment during the event.", MsgMessage.ChatMode.System);
                                    return;
                                }
                            }
                        }
                        if (client.Player != null)
                        {
                            if (client.Player.Map == 1121)
                            {
                                client.SendSysMesage("Item can't be switch equipment during the event.");
                                return;
                            }
                        }
                        client.OnAutoAttack = false;
                        if (client.Inventory.HaveSpace(1))
                        {
                            if (client.Equipment.TryGetEquip(Flags.ConquerItem.AleternanteLeftWeapon, out var item7) && item7.ITEM_ID / 1000u == 900 && (client.Player.Class < 21 || client.Player.Class > 25))
                            {
                                client.Equipment.Remove(Flags.ConquerItem.AleternanteLeftWeapon, stream);
                                break;
                            }
                            client.Equipment.Alternante = !client.Equipment.Alternante;
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            client.Player.View.ReSendView(stream);
                        }
                        else
                            client.SendSysMesage("Your inventory is full.");
                        break;
                    }
                    catch
                    {
                        ServerKernel.Log.SaveLog($"[{action}] could not get item value", false, LogType.EXCEPTION);
                        break;
                    }
                //case ItemUsuageID.RemoveInventory:
                //case ItemUsuageID.SetEquipPosition:
                //case ItemUsuageID.UpgradeEnchant:
                //case ItemUsuageID.ArrowReload:
                //case (ItemUsuageID)12u:
                //case (ItemUsuageID)13u:
                ////case (ItemUsuageID)15u:
                //case (ItemUsuageID)16u:
                //case ItemUsuageID.UpdateDurability:
                //case ItemUsuageID.RemoveEquipment:
                //case ItemUsuageID.UpdateArrowCount:
                //case ItemUsuageID.ParticleEffect:
                //case (ItemUsuageID)30u:
                //case (ItemUsuageID)31u:
                //case (ItemUsuageID)34u:
                //case ItemUsuageID.DropGold:
                //case (ItemUsuageID)42u:
                //case (ItemUsuageID)46u:
                //case (ItemUsuageID)47u:
                //    break;
            }
        }

        public static void AlternanteEquipItem(GameClient client, Packet stream, uint id, uint dwParam1, uint timestamp = 0, uint dwParam2 = 0, uint dwParam3 = 0, uint dwparam4 = 0)
        {
            uint Position;
            Position = dwParam1;
            Position -= 20;
            if (!client.Inventory.TryGetItem(id, out var item) || (ItemType.IsPistol(item.ITEM_ID) && (client.Equipment.FreeEquip(Flags.ConquerItem.RightWeapon) || !client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out var rapier) || !ItemType.IsRapier(rapier.ITEM_ID))))
                return;
            if (Position == 5 && ItemType.IsKnife(client.Equipment.RightWeapon) && !ItemType.IsKnife(item.ITEM_ID))
            {
                client.SendSysMesage("Remove the right-hand Knife.");
                return;
            }
            //if (Position == 4 && ItemType.IsTwoHand(client.Equipment.RightWeapon) && !ItemType.IsTwoHand(item.ITEM_ID))
            //{
            //    client.SendSysMesage("Remove the right-hand Two Handed.");
            //    return;
            //}
            if (Position == 4)
            {
                if (ItemType.IsKnife(client.Equipment.LeftWeapon) && !ItemType.IsKnife(item.ITEM_ID))
                {
                    client.SendSysMesage("Remove the left-hand Knife.");
                    return;
                }
                if (ItemType.IsShield(client.Equipment.LeftWeapon) && ItemType.IsKnife(item.ITEM_ID))
                {
                    client.SendSysMesage("Remove the left-hand Shield.");
                    return;
                }
            }
            bool can2hand;
            can2hand = false;
            bool can2wpn;
            can2wpn = false;
            if (client.Player.Class >= 11 && client.Player.Class <= 65)
                can2hand = true;
            if ((client.Player.Class >= 11 && client.Player.Class <= 15) || (client.Player.Class >= 20 && client.Player.Class <= 25) || (client.Player.Class >= 51 && client.Player.Class <= 65))
                can2wpn = true;
            if (client.Player.Class >= 40 && client.Player.Class <= 45)
            {
                if (ItemType.IsTwoHand(item.ITEM_ID) && client.Player.IsAsasin)
                    return;
                can2wpn = true;
            }
            if (!ItemType.Equipable(item, client))
                return;
            switch (ItemType.ItemPosition(item.ITEM_ID))
            {
                case 0:
                case 17:
                    return;
                case 5:
                    {
                        Position = 5;
                        if ((!can2hand && !can2wpn) || client.Equipment.FreeEquip(Flags.ConquerItem.AleternanteRightWeapon) || (client.Equipment.TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon, out var RightWeapon2) && RightWeapon2.ITEM_ID / 1000u != 500 && ItemType.IsArrow(item.ITEM_ID)))
                            return;
                        break;
                    }
            }
            if (ItemType.ItemPosition(item.ITEM_ID) == 4 && Position == 5 && (!can2hand || !can2wpn))
                return;
            if (((Position != 4 && Position != 5) || (ItemType.ItemPosition(item.ITEM_ID) != 4 && ItemType.ItemPosition(item.ITEM_ID) != 5)) && !ItemType.IsAccessory(item.ITEM_ID))
                Position = ItemType.ItemPosition(item.ITEM_ID);
            bool twohand;
            twohand = ItemType.IsTwoHand(item.ITEM_ID);
            if (!twohand && Position == 4 && client.Equipment.TryGetEquip(Flags.ConquerItem.AleternanteLeftWeapon, out var LeftWeapon) && client.Inventory.HaveSpace(1))
            {
                MsgGameItem RightWeapon;
                if (ItemType.IsArrow(LeftWeapon.ITEM_ID))
                    client.Equipment.Remove(Flags.ConquerItem.AleternanteLeftWeapon, stream);
                else if (client.Equipment.TryGetEquip(Flags.ConquerItem.AleternanteRightWeapon, out RightWeapon) && ItemType.IsTwoHand(RightWeapon.ITEM_ID))
                {
                    client.Equipment.Remove(Flags.ConquerItem.AleternanteRightWeapon, stream);
                }
            }
            Position += 20;
            if (client.Equipment.FreeEquip((Flags.ConquerItem)Position))
            {
                bool spaceinventory2;
                spaceinventory2 = true;
                if (twohand)
                {
                    if (client.Inventory.HaveSpace(1))
                        client.Equipment.Remove(Flags.ConquerItem.AleternanteLeftWeapon, stream);
                    else
                        spaceinventory2 = false;
                }
                if (spaceinventory2)
                {
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    item.Position = (ushort)Position;
                    client.Equipment.Add(item, stream);
                    item.Mode = Flags.ItemMode.Update;
                    item.Send(client, stream);
                }
            }
            else
            {
                bool spaceinventory;
                spaceinventory = true;
                if (twohand)
                {
                    if (client.Inventory.HaveSpace(1))
                        client.Equipment.Remove(Flags.ConquerItem.AleternanteLeftWeapon, stream);
                    else
                        spaceinventory = false;
                }
                if (spaceinventory)
                {
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    item.Position = (ushort)Position;
                    item.Mode = Flags.ItemMode.AddItem;
                    client.Equipment.Remove((Flags.ConquerItem)Position, stream);
                    client.Equipment.Add(item, stream);
                }
            }
            MsgShowEquipment equip;
            equip = new MsgShowEquipment
            {
                wParam = item.UID,
                Alternante = item.Position,
                UID = 5
            };
            client.Send(stream.ShowEquipmentCreate(equip));
            if (client.Equipment.Alternante)
                client.Equipment.QueryEquipment(true);
            else
                client.Equipment.SendAlowAlternante(stream);
        }

        public static void EquipItem(GameClient client, Packet stream, uint id, uint dwParam1, uint timestamp = 0, uint dwParam2 = 0, uint dwParam3 = 0, uint dwparam4 = 0)
        {
            uint Position;
            Position = dwParam1;
            if (!client.Inventory.TryGetItem(id, out var item))
                return;
            if (client.Mining)
                client.StopMining();
            if (client.EventBase != null)
            {
                var events = Program.Events.Find(x => x.EventTitle == client.EventBase.EventTitle);
                if (events != null)
                {
                    if (events.InTournament(client))
                    {
                        client.SendSysMesage("Item can't be equiped during the event.", MsgMessage.ChatMode.System);
                        return;
                    }
                }
            }

            uint iTEM_ID;
            iTEM_ID = item.ITEM_ID;
            if (iTEM_ID - 1200000 <= 2)
            {
                UseItem(item, client, stream);
                return;
            }
            if (Position == 17 && ItemType.ItemPosition(item.ITEM_ID) == 0)
                UseItem(item, client, stream);
            else if (Position == 0 && ItemType.ItemPosition(item.ITEM_ID) == 0)
            {
                UseItem(item, client, stream);
            }
            if (ItemType.IsShield(item.ITEM_ID) && client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out var twohanded))
            {
                if(ItemType.IsTwoHand(twohanded.ITEM_ID))
                {
                    client.SendSysMesage("You can't wear Shield while wearing Two Handed, while the DivineStaff is fixing the Jump issue #00");
                    return;
                }
            }
            if (Database.ItemType.IsShield(item.ITEM_ID) && Server.ItemsBase[item.ITEM_ID].Level >= 121)
            {
                client.SendSysMesage("You can't wear Shield above level 120 #00");
                return;
            }
            if (ItemType.IsPistol(item.ITEM_ID) && (client.Equipment.FreeEquip(Flags.ConquerItem.RightWeapon) || !client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out var rapier) || !ItemType.IsRapier(rapier.ITEM_ID)))
                return;
            bool can2hand;
            can2hand = false;
            bool can2wpn;
            can2wpn = false;
            if (client.Player.Class >= 11 && client.Player.Class <= 65)
                can2hand = true;
            if ((client.Player.Class >= 11 && client.Player.Class <= 15) || (client.Player.Class >= 20 && client.Player.Class <= 25) || (client.Player.Class >= 51 && client.Player.Class <= 65))
                can2wpn = true;
            if (client.Player.Class >= 40 && client.Player.Class <= 45)
            {
                if (ItemType.IsTwoHand(item.ITEM_ID) && client.Player.IsAsasin)
                    return;
                can2wpn = true;
            }
            if (!ItemType.Equipable(item, client))
                return;
            ushort po;
            po = ItemType.ItemPosition(item.ITEM_ID);
            if (ItemType.IsShield(item.ITEM_ID) && (client.Player.Class < 21 || client.Player.Class > 25))
                return;
            if (Position == 5)
            {
                bool isPrevArch;
                isPrevArch = client.Player.Reborn > 0 && (client.Player.FirstClass == 45 || client.Player.SecondClass == 45);
                if (!AtributesStatus.IsArcher(client.Player.Class) && !isPrevArch && ItemType.IsTwoHand(client.Equipment.RightWeapon) && !ItemType.IsShield(item.ITEM_ID))
                    return;
            }
            if (Position == 5 && ItemType.IsKnife(client.Equipment.RightWeapon) && !ItemType.IsKnife(item.ITEM_ID))
            {
                client.SendSysMesage("Remove the right-hand Knife.");
                return;
            }
            if (Position == 4)
            {
                if (ItemType.IsKnife(client.Equipment.LeftWeapon) && !ItemType.IsKnife(item.ITEM_ID))
                {
                    client.SendSysMesage("Remove the left-hand Knife.");
                    return;
                }
                if (ItemType.IsShield(client.Equipment.LeftWeapon) && ItemType.IsKnife(item.ITEM_ID))
                {
                    client.SendSysMesage("Remove the left-hand Shield.");
                    return;
                }
            }
            switch (po)
            {
                case 5:
                    {
                        Position = 5;
                        if ((!can2hand && !can2wpn) || client.Equipment.FreeEquip(Flags.ConquerItem.RightWeapon) || (client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out var RightWeapon2) && RightWeapon2.ITEM_ID / 1000u != 500 && ItemType.IsArrow(item.ITEM_ID)))
                            return;
                        break;
                    }
                case 0:
                    return;
            }
            if (ItemType.ItemPosition(item.ITEM_ID) == 4 && Position == 5 && (!can2hand || !can2wpn))
                return;
            if (((Position != 4 && Position != 5) || (ItemType.ItemPosition(item.ITEM_ID) != 4 && ItemType.ItemPosition(item.ITEM_ID) != 5)) && !ItemType.IsAccessory(item.ITEM_ID))
                Position = ItemType.ItemPosition(item.ITEM_ID);
            bool twohand;
            twohand = ItemType.IsTwoHand(item.ITEM_ID);
            if (!twohand && Position == 4 && client.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon, out var LeftWeapon) && client.Inventory.HaveSpace(1))
            {
                MsgGameItem RightWeapon;
                if (ItemType.IsArrow(LeftWeapon.ITEM_ID))
                    client.Equipment.Remove(Flags.ConquerItem.LeftWeapon, stream);
                else if (client.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out RightWeapon) && ItemType.IsTwoHand(RightWeapon.ITEM_ID))
                {
                    client.Equipment.Remove(Flags.ConquerItem.RightWeapon, stream);
                }
            }
            if (client.Equipment.FreeEquip((Flags.ConquerItem)Position))
            {
                bool spaceinventory2;
                spaceinventory2 = true;
                if (twohand)
                {
                    if (client.Inventory.HaveSpace(1))
                        client.Equipment.Remove(Flags.ConquerItem.LeftWeapon, stream);
                    else
                        spaceinventory2 = false;
                }
                if (spaceinventory2)
                {
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    item.Position = (ushort)Position;
                    client.Equipment.Add(item, stream);
                    item.Mode = Flags.ItemMode.Update;
                    item.Send(client, stream);
                }
            }
            else
            {
                bool spaceinventory;
                spaceinventory = true;
                if (twohand)
                {
                    if (client.Inventory.HaveSpace(1))
                        client.Equipment.Remove(Flags.ConquerItem.LeftWeapon, stream);
                    else
                        spaceinventory = false;
                }
                if (spaceinventory)
                {
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    item.Position = (ushort)Position;
                    item.Mode = Flags.ItemMode.AddItem;
                    client.Equipment.Remove((Flags.ConquerItem)Position, stream);
                    client.Equipment.Add(item, stream);
                }
            }
            client.Equipment.QueryEquipment(client.Equipment.Alternante);
        }

        public static void UseItem(MsgGameItem item, GameClient client, Packet stream)
        {
            if (!client.Player.Alive || DateTime.Now < client.ItemStamp.AddMilliseconds(200.0))
                return;
            client.ItemStamp = DateTime.Now;
            if (!Server.ItemsBase.TryGetValue(item.ITEM_ID, out var DBItem))
                return;
            if ((item.ITEM_ID >= 726001 && item.ITEM_ID <= 726108) || item.ITEM_ID == 722559 || (item.ITEM_ID >= 721177 && item.ITEM_ID <= 721202))
            {
                if (HouseTable.InHouse(client.Player.Map) && client.Player.DynamicID == client.Player.UID)
                {
                    NpcServerQuery Furniture;
                    Furniture = default(NpcServerQuery);
                    if (item.ITEM_ID == 722559)
                        Furniture.NpcType = Flags.NpcType.Talker;
                    else
                        Furniture.NpcType = Flags.NpcType.Furniture;
                    Furniture.Action = NpcServerReplay.Mode.Cursor;
                    NpcServer.Furniture Npc;
                    Npc = NpcServer.GetNpc(item.ITEM_ID);
                    if (Npc != null && client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        Furniture.Mesh = Npc.Mesh;
                        Furniture.ID = (NpcID)Npc.UID;
                        client.MoveNpcMesh = Furniture.Mesh;
                        client.MoveNpcUID = (uint)Furniture.ID;
                        client.Send(stream.NpcServerCreate(Furniture));
                    }
                }
                else
                    client.SendSysMesage("You have to be in your own house to be able to display it.");
                return;
            }
            if ((item.ITEM_ID >= 1000000 && item.ITEM_ID <= 1000040) || (item.ITEM_ID >= 1002000 && item.ITEM_ID < 1002030) || item.ITEM_ID == 1002050 || item.ITEM_ID == 725065 || item.ITEM_ID == 1003010)
            {
                if (client.EventBase != null && client.EventBase.PotionsAllowed == false && client.EventBase.Stage == MsgEvents.EventStage.Fighting)
                    return;
                if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(600) && !client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) && !UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) && client.Player.HitPoints != client.Status.MaxHitpoints && client.Inventory.Remove(item.ITEM_ID, 1, stream))
                {
                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "spilth");
                    client.Player.HitPoints = Math.Min(client.Player.HitPoints + DBItem.ItemHP, (int)client.Status.MaxHitpoints);
                    client.Player.MedicineStamp = DateTime.Now;
                }
                return;
            }
            if ((item.ITEM_ID >= 1001000 && item.ITEM_ID <= 1001040) || item.ITEM_ID == 1002030 || item.ITEM_ID == 1002040 || item.ITEM_ID == 725066 || item.ITEM_ID == 1004010)
            {
                if (!client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) /*&& client.Player.Map != 1005 && !UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)*/ && client.Player.Mana != client.Status.MaxMana && client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    client.Player.Mana = (ushort)Math.Min(client.Player.Mana + DBItem.ItemMP, (int)client.Status.MaxMana);
                return;
            }
            if ((item.ITEM_ID >= 725000 && item.ITEM_ID <= 725044) || item.ITEM_ID == 1060101 || item.ITEM_ID == 721158 || item.ITEM_ID == 721157 || item.ITEM_ID == 3007568 || item.ITEM_ID == 3007567 || item.ITEM_ID == 3007566 || (item.ITEM_ID >= 3006017 && item.ITEM_ID <= 3006020))
            {
                switch (item.ITEM_ID)
                {
                    case 725018u:
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1380, 0, 0, 0);
                        break;
                    case 725019u:
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1385, 0, 0, 0);
                        break;
                    case 725020u:
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1390, 0, 0, 0);
                        break;
                    case 725021u:
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1395, 0, 0, 0);
                        break;
                    case 725022u:
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1400, 0, 0, 0);
                        break;
                    case 725023u:
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1405, 0, 0, 0);
                        break;
                    case 725024u:
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.MySpells.Add(stream, 1410, 0, 0, 0);
                        break;
                    case 1060101u:
                        if (client.Player.Level < 84)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~84.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1165))
                        {
                            if (AtributesStatus.IsFire(client.Player.Class))
                            {
                                client.Inventory.Update(item, AddMode.REMOVE, stream);
                                client.MySpells.Add(stream, 1165, 0, 0, 0);
                                client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.FireofHell.ToString() + ".", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,this spell is just for Fire Taoist`s.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725016u:
                        if (client.Player.Level < 90)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~90.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1360))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 1360, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.NightDevil.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 721157u:
                        if (client.Player.Level < 40)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(11000))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 11000, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.DragonTail.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 721158u:
                        if (client.Player.Level < 40)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(11005))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 11005, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.ViperFang.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725005u:
                        if (client.Player.Level < 40)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1045))
                        {
                            if (client.MyProfs.CheckProf(410, 5))
                            {
                                client.Inventory.Update(item, AddMode.REMOVE, stream);
                                client.MySpells.Add(stream, 1045, 0, 0, 0);
                                client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.FastBlader.ToString() + ".", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~skill~before~you~practice~your~blade~to~level~5.~Please~train~harder.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725010u:
                        if (client.Player.Level < 40)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1046))
                        {
                            if (client.MyProfs.CheckProf(420, 5))
                            {
                                client.Inventory.Update(item, AddMode.REMOVE, stream);
                                client.MySpells.Add(stream, 1046, 0, 0, 0);
                                client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.ScrenSword.ToString() + ".", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~skill~before~you~practice~your~sword~to~level~5.~Please~train~harder.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725029u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(5030))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 5030, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Phoenix.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725042u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(7020))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 7020, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Rage.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725030u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(5040))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 5040, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Boom.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725040u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(7000))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 7000, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Seizer.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725041u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(7010))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 7010, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Earthquake.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725043u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(7030))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 7030, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Celestial.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725044u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(7040))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 7040, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Roamer.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725013u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1290))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 1290, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Penetration.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725026u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(5010))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 5010, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Snow.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725012u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1260))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 1260, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.SpeedGun.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725011u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1250))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 1250, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.WideStrike.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725027u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(5020))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 5020, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.StrandedMonster.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725014u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1300))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 1300, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Halt.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725031u:
                        if (client.Player.Level < 30)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~30.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(5050))
                        {
                            client.Inventory.Update(item, AddMode.REMOVE, stream);
                            client.MySpells.Add(stream, 5050, 0, 0, 0);
                            client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Boreas.ToString() + ".", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725000u:
                        if (!client.MySpells.ClientSpells.ContainsKey(1000))
                        {
                            if (client.Player.Spirit >= 20)
                            {
                                client.Inventory.Update(item, AddMode.REMOVE, stream);
                                client.MySpells.Add(stream, 1000, 0, 0, 0);
                                client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Thunder.ToString() + ".", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~20.~Please~train~harder.", "I~see.");
                        }
                        else
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        break;
                    case 725001u:
                        if (client.Player.Level < 40)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1001))
                        {
                            if (client.MySpells.CheckSpell(1000, 4))
                            {
                                if (client.Player.Spirit >= 80)
                                {
                                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, 1001, 0, 0, 0);
                                    client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Fire.ToString() + ".", "I~see.");
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~80.~Please~train~harder.", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~you~practice~Thunder~to~level~4.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725002u:
                        if (client.Player.Level < 90)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~90.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1002))
                        {
                            if (AtributesStatus.IsFire(client.Player.Class))
                            {
                                if (client.MySpells.CheckSpell(1001, 3))
                                {
                                    if (client.Player.Spirit >= 160)
                                    {
                                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                                        client.MySpells.Add(stream, 1002, 0, 0, 0);
                                        client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Tornado.ToString() + ".", "I~see.");
                                    }
                                    else
                                        client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~160.~Please~train~harder.", "I~see.");
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~you~practice~Fire~to~level~3.", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,this spell is just for Fire Taoist.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725003u:
                        if (!client.MySpells.ClientSpells.ContainsKey(1005))
                        {
                            if (client.Player.Spirit >= 30)
                            {
                                client.Inventory.Update(item, AddMode.REMOVE, stream);
                                client.MySpells.Add(stream, 1005, 0, 0, 0);
                                client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Cure.ToString() + ".", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~30.~Please~train~harder.", "I~see.");
                        }
                        else
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        break;
                    case 725015u:
                        if (client.Player.Level < 54)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~54.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1350))
                        {
                            if (AtributesStatus.IsWater(client.Player.Class))
                            {
                                client.Inventory.Update(item, AddMode.REMOVE, stream);
                                client.MySpells.Add(stream, 1350, 0, 0, 0);
                                client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.DivineHare.ToString() + ".", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,this spell is just for Water Taoist.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725004u:
                        if (!client.MySpells.ClientSpells.ContainsKey(1010))
                        {
                            if (AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                if (client.Player.Spirit >= 25)
                                {
                                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, 1010, 0, 0, 0);
                                    client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.Lightning.ToString() + ".", "I~see.");
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~25.~Please~train~harder.", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,this spell is just for Taoist`s.", "I~see.");
                        }
                        else
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        break;
                    case 725028u:
                        if (client.Player.Level < 70)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~70.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(5001))
                        {
                            if (AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                if (client.Player.Spirit >= 25)
                                {
                                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                                    client.MySpells.Add(stream, 10405, 0, 0, 0);
                                    client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.SoulShackle.ToString() + ".", "I~see.");
                                }
                                else
                                    client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Spirit~is~25.~Please~train~harder.", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,this spell is just for Taoist`s.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                    case 725025u:
                        if (client.Player.Level < 40)
                            client.CreateDialog(stream, "Sorry,~you~cannot~learn~this~spell~before~your~Level~is~40.~Please~train~harder.", "I~see.");
                        else if (!client.MySpells.ClientSpells.ContainsKey(1320))
                        {
                            if (AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                client.Inventory.Update(item, AddMode.REMOVE, stream);
                                client.MySpells.Add(stream, 1320, 0, 0, 0);
                                client.CreateDialog(stream, "You~have~learned~" + Flags.SpellID.FlyingMoon.ToString() + ".", "I~see.");
                            }
                            else
                                client.CreateDialog(stream, "Sorry,this spell is just for Warrior`s.", "I~see.");
                        }
                        else
                        {
                            client.CreateDialog(stream, "You already have this spell.", "I~see.");
                        }
                        break;
                }
                return;
            }
            switch (item.ITEM_ID)
            {
                case 721754u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 5 donation pts");
                        Player player;
                        player = client.Player;
                        player.ChampionPoints += 5;
                        client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                    }
                    break;
                case 721757u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 20 donation pts");
                        Player player;
                        player = client.Player;
                        player.ChampionPoints += 20;
                        client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                    }
                    break;
                case 721760u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 10 donation pts");
                        Player player;
                        player = client.Player;
                        player.ChampionPoints += 10;
                        client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                    }
                    break;
                case 721761:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 10 donation pts");
                        Player player;
                        player = client.Player;
                        player.ChampionPoints += 10000;
                        client.Player.SendUpdate(stream, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                    }
                    break;
                case 723713u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 300000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money); 
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 300000 silver");
                        client.SendSysMesage("You received 300000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723714u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 800000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 800000 silver");
                        client.SendSysMesage("You received 800000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723715u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 1200000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money); 
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 1200000 silver");
                        client.SendSysMesage("You received 1200000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723716u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 1800000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 1800000 silver");
                        client.SendSysMesage("You received 1800000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723717u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 5000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 5000000 silver");
                        client.SendSysMesage("You received 5000000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723718u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 20000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 20000000 silver");
                        client.SendSysMesage("You received 20000000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723719u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 25000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 25000000 silver");
                        client.SendSysMesage("You received 25.000.000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723720u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 80000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 80000000 silver");
                        client.SendSysMesage("You received 80.000.000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723721u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 100000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 100000000 silver");
                        client.SendSysMesage("You received 100.000.000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723722u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 300000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 300000000 silver");
                        client.SendSysMesage("You received 300000000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 723723u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.Money += 500000000;
                        client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name + " and got 500000000 silver");
                        client.SendSysMesage("You received 500000000 silver for opening the MoneyBag's.");
                    }
                    break;
                case 722178u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        SurpriseBox.GetReward(client, stream);
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name);
                    }
                    break;
                //case 722450:
                //    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                //    {
                //        SurpriseEventBox.GetReward(client, stream);
                //        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name);
                //    }
                    break;
                case 727786u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        List<uint> list;
                        list = new List<uint>();
                        list.Add(191405);
                        list.Add(191305);
                        list.Add(183325);
                        list.Add(183315);
                        list.Add(183375);
                        list.Add(183305);
                        list.Add(182335);
                        list.Add(181355);
                        list.Add(181395);
                        list.Add(182355);
                        list.Add(182375);
                        list.Add(182365);
                        list.Add(182345);
                        list.Add(182385);
                        list.Add(182325);
                        list.Add(181385);
                        list.Add(181375);
                        list.Add(182305);
                        list.Add(181365);
                        list.Add(181345);
                        list.Add(181335);
                        list.Add(182315);
                        list.Add(181305);
                        list.Add(181405);
                        list.Add(181505);
                        list.Add(181605);
                        list.Add(181705);
                        list.Add(181805);
                        list.Add(181905);
                        list.Add(181315);
                        list.Add(181415);
                        list.Add(181515);
                        list.Add(181615);
                        list.Add(181715);
                        list.Add(181815);
                        list.Add(181915);
                        list.Add(181325);
                        list.Add(181425);
                        list.Add(181525);
                        list.Add(181625);
                        list.Add(181725);
                        list.Add(181825);
                        list.Add(181925);
                        list.Add(722057);
                        List<uint> listGarments;
                        listGarments = list;
                        uint reward;
                        reward = listGarments[Core.Random.Next(0, listGarments.Count)];
                        client.Inventory.Add(stream, reward, 1, 0, 0, 0);
                        client.SendSysMesage("Congratulations! You have got rare prizes. Check your inventory!");
                        ServerKernel.Log.AppendGameLog(client.Player.Name + " has oppend " + DBItem.Name);
                    }
                    break;
                case 727317u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        ItemType.DBItem[] array;
                        array = ItemType.PurificationItems[6].Values.ToArray();
                        int position;
                        position = ServerKernel.NextAsync(0, array.Length);
                        uint reward2;
                        reward2 = array[position].ID;
                        client.Inventory.Add(stream, reward2, 1, 0, 0, 0);
                        client.SendSysMesage("Congratulations! You have got rare prizes. Check your inventory!");
                    }
                    break;
                case 720757u:
                    if (client.Inventory.HaveSpace(3))
                    {
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(722136, 0, 30, stream);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {30} empty spaces.");
                    break;
                case 721117u:
                    client.ActiveNpc = 4294967248;
                    NpcHandler.VIPBook(client, stream, 0, "", 0);
                    break;
                case 723583u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        switch (client.Player.Body)
                        {
                            case 1003:
                                client.Player.Body = 1004;
                                break;
                            case 1004:
                                client.Player.Body = 1003;
                                break;
                            case 2001:
                                client.Player.Body = 2002;
                                break;
                            case 2002:
                                client.Player.Body = 2001;
                                break;
                        }
                    }
                    break;
                #region BeginnerPack
                //case 723753:
                //{
                //    if (!client.Inventory.HaveSpace(10))
                //    {
                //        client.SendSysMesage("Please make 10 more spaces in your inventory.");
                //        break;
                //    }
                //    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //    //client.Inventory.AddItemWitchStack(722136, 0, 10, stream, true);
                //    // el plus kam3
                //    if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                //    {
                //        client.Inventory.Add(stream, 152017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//backlet
                //        client.Inventory.Add(stream, 121007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//bag
                //        client.Inventory.Add(stream, 160017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//boots
                //    }
                //    else
                //    {
                //        client.Inventory.Add(stream, 150007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//ring
                //        client.Inventory.Add(stream, 120007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//neck
                //        client.Inventory.Add(stream, 160017, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//boots
                //    }

                //    if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                //    {
                //        client.Inventory.Add(stream, 118007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//armor
                //        client.Inventory.Add(stream, 130007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//armor
                //        client.Inventory.Add(stream, 410007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//blade
                //        client.Inventory.Add(stream, 420007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//sword
                //        client.Inventory.Add(stream, 480007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//club
                //    }
                //    else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                //    {
                //        client.Inventory.Add(stream, 111007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//headgear
                //        client.Inventory.Add(stream, 131007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//armor
                //        client.Inventory.Add(stream, 410007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//blade
                //        client.Inventory.Add(stream, 420007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//sword
                //        client.Inventory.Add(stream, 480007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//club
                //        client.Inventory.Add(stream, 561007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//wand
                //        client.Inventory.Add(stream, 900007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//Shield
                //    }
                //    else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                //    {
                //        client.Inventory.AddItemWitchStack(1050000, 0, 5, stream);
                //        client.Inventory.Add(stream, 133007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//armor
                //        client.Inventory.Add(stream, 113007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//Helmet
                //        client.Inventory.Add(stream, 500007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//bow
                //    }
                //    //else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                //    //{
                //    //    client.Inventory.Add(stream, 135089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                //    //    client.Inventory.Add(stream, 601219, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                //    //    client.Inventory.Add(stream, 601219, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                //    //}
                //    //else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                //    //{
                //    //    client.Inventory.Add(stream, 136089, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                //    //    client.Inventory.Add(stream, 610209, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                //    //    client.Inventory.Add(stream, 610209, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                //    //}
                //    else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                //    {
                //        client.Inventory.Add(stream, 114007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);
                //        client.Inventory.Add(stream, 421007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//backsword
                //        client.Inventory.Add(stream, 134007, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, false);//armor
                //                                                                                                                  //152066
                //    }
                //    client.CreateBoxDialog("Welcome to Divine Siege \n [Pure Classic Server]! You got 5 days free VIP6 and Claim x2 Exp at LoveStone");
                //    break;
                //}
                #endregion
                case 723753:
                    {
                        if (!client.Inventory.HaveSpace(12))
                        {
                            client.SendSysMesage("Please make 12 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.Level >= 15)
                        {

                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            //client.Inventory.Add(stream, 723776, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);//BeginnerPack 100
                            client.Inventory.Add(stream, 1080001, 1);//1xEmerald
                            client.Inventory.Add(stream, 1088001, 1);//1xMeteor
                            //client.Inventory.Add(stream, 1200002, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);//1xPrayingStone(S)
                            client.Inventory.Add(stream, Database.ItemType.MoonBox, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);//1xPrayingStone(S)
                            client.Inventory.Add(stream, 160019, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//OxhideBoots
                            if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 152019, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//PeachBracelet
                                client.Inventory.Add(stream, 121009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//PerfumeBag
                            }
                            else
                            {
                                client.Inventory.Add(stream, 150009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//IronRing
                                client.Inventory.Add(stream, 120009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//LightNecklace
                            }

                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 130009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//LeatherArmor
                                client.Inventory.Add(stream, 410009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//SteelBlade
                                client.Inventory.Add(stream, 480009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//JustClub
                                client.Inventory.Add(stream, 420009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//VanSword
                                client.Inventory.Add(stream, 118009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//GuardCoronet
                            }
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 131009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//OxhideArmor
                                client.Inventory.Add(stream, 561009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//WoodWand
                                client.Inventory.Add(stream, 111009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//IronHelmet
                                                                                                                                                                          //client.Inventory.Add(stream, 150217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                                                                                                                                                                          //client.Inventory.Add(stream, 120187, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                            }
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 133009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//DeerskinCoat
                                client.Inventory.Add(stream, 500009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//BambooBow
                                client.Inventory.Add(stream, 113009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BadgerHat
                                                                                                                                                                          //client.Inventory.Add(stream, 150217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                                                                                                                                                                          //client.Inventory.Add(stream, 120187, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                client.Inventory.Add(stream, 1072031, 5);//5xEuxeniteOre

                            }
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 134009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 421009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedPhoenixGem, Role.Flags.Gem.RefinedPhoenixGem, true, Flags.ItemEffect.MP);//PeachBacksword
                                client.Inventory.Add(stream, 114009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//DestinyCap
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                          //152066
                            }
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 136009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 143009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 610009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//PeachBacksword
                                client.Inventory.Add(stream, 610009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//PeachBacksword
                                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //152066
                            }
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 112009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 135009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 601009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//MetalKatana
                                client.Inventory.Add(stream, 601009, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//MetalKatana
                                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //152066
                            }
                            break;
                        }
                        else
                        {
                            client.SendSysMesage("Please, make sure you are level 15 or more to open this BeginnerPack.");
                            break;
                        }
                    }
                #region BeginnerPack Lvl 15 1st
                case 723776:
                    {
                        client.SendSysMesage("Please make 12 more spaces in your inventory.");
                        break;
                        if (!client.Inventory.HaveSpace(12))
                        {
                            client.SendSysMesage("Please make 12 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.Level >= 15 && client.Player.Reborn == 1)
                        {

                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            client.Inventory.Add(stream, 723756, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);//BeginnerPack 100
                            client.Inventory.Add(stream, 1200002, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);//1xPrayingStone(S)
                            client.Inventory.Add(stream, 160017, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//OxhideBoots
                            if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 152017, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//PeachBracelet
                                client.Inventory.Add(stream, 121007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//PerfumeBag
                            }
                            else
                            {
                                client.Inventory.Add(stream, 150007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//IronRing
                                client.Inventory.Add(stream, 120007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//LightNecklace
                            }

                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 130007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//LeatherArmor
                                client.Inventory.Add(stream, 410007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//SteelBlade
                                client.Inventory.Add(stream, 480007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//JustClub
                                client.Inventory.Add(stream, 420007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//VanSword
                                client.Inventory.Add(stream, 118007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//GuardCoronet
                            }
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 131007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//OxhideArmor
                                client.Inventory.Add(stream, 561007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//WoodWand
                                client.Inventory.Add(stream, 111007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//IronHelmet
                                                                                                                                                                          //client.Inventory.Add(stream, 150217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                                                                                                                                                                          //client.Inventory.Add(stream, 120187, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                            }
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 133007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//DeerskinCoat
                                client.Inventory.Add(stream, 500007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//BambooBow
                                client.Inventory.Add(stream, 113007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BadgerHat
                                                                                                                                                                          //client.Inventory.Add(stream, 150217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                                                                                                                                                                          //client.Inventory.Add(stream, 120187, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                client.Inventory.Add(stream, 1072031, 5);//5xEuxeniteOre

                            }
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 134007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 421007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedPhoenixGem, Role.Flags.Gem.RefinedPhoenixGem, true, Flags.ItemEffect.MP);//PeachBacksword
                                client.Inventory.Add(stream, 114007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//DestinyCap
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                          //152066
                            }
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 136007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 143007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 610007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//PeachBacksword
                                client.Inventory.Add(stream, 610007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//PeachBacksword
                                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //152066
                            }
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 112007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 135007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 601007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//MetalKatana
                                client.Inventory.Add(stream, 601007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//MetalKatana
                                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //152066
                            }
                            break;
                        }
                        else
                        {
                            client.SendSysMesage("Please, make sure you are level 15 or more and 1nd reborn to open this BeginnerPack.");
                            break;
                        }
                    }
                #endregion
                #region BeginnerPack Lvl 15 2nd
                case 723756:
                    {
                        client.SendSysMesage("Please make 12 more spaces in your inventory.");
                        break;
                        if (!client.Inventory.HaveSpace(12))
                        {
                            client.SendSysMesage("Please make 12 more spaces in your inventory.");
                            break;
                        }
                        if (client.Player.Level >= 15 && client.Player.Reborn == 2)
                        {

                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);

                            
                            client.Inventory.Add(stream, 1200002, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);//1xPrayingStone(S)
                            client.Inventory.Add(stream, 160017, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//OxhideBoots
                            if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 152017, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//PeachBracelet
                                client.Inventory.Add(stream, 121007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//PerfumeBag
                            }
                            else
                            {
                                client.Inventory.Add(stream, 150007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//IronRing
                                client.Inventory.Add(stream, 120007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//LightNecklace
                            }

                            if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 130007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//LeatherArmor
                                client.Inventory.Add(stream, 410007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//SteelBlade
                                client.Inventory.Add(stream, 480007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//JustClub
                                client.Inventory.Add(stream, 420007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//VanSword
                                client.Inventory.Add(stream, 118007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//GuardCoronet
                            }
                            else if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 131007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//OxhideArmor
                                client.Inventory.Add(stream, 561007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//WoodWand
                                client.Inventory.Add(stream, 111007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//IronHelmet
                                                                                                                                                                          //client.Inventory.Add(stream, 150217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                                                                                                                                                                          //client.Inventory.Add(stream, 120187, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                            }
                            else if (Database.AtributesStatus.IsArcher(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 133007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//DeerskinCoat
                                client.Inventory.Add(stream, 500007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//BambooBow
                                client.Inventory.Add(stream, 113007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//BadgerHat
                                                                                                                                                                          //client.Inventory.Add(stream, 150217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//ring
                                                                                                                                                                          //client.Inventory.Add(stream, 120187, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//neck
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                client.Inventory.Add(stream, 1072031, 5);//5xEuxeniteOre

                            }
                            else if (Database.AtributesStatus.IsTaoist(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 134007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 421007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedPhoenixGem, Role.Flags.Gem.RefinedPhoenixGem, true, Flags.ItemEffect.MP);//PeachBacksword
                                client.Inventory.Add(stream, 114007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//DestinyCap
                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                          //152066
                            }
                            else if (Database.AtributesStatus.IsMonk(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 136007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 143007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 610007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//PeachBacksword
                                client.Inventory.Add(stream, 610007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//PeachBacksword
                                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //152066
                            }
                            else if (Database.AtributesStatus.IsNinja(client.Player.Class))
                            {
                                client.Inventory.Add(stream, 112007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 135007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//TaoRobe
                                client.Inventory.Add(stream, 601007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//MetalKatana
                                client.Inventory.Add(stream, 601007, 1, ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.RefinedDragonGem, Role.Flags.Gem.RefinedDragonGem, true);//MetalKatana
                                                                                                                                                                                          //client.Inventory.Add(stream, 160217, 1,ServerKernel.Bound_Equipments_Plus, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//boots
                                                                                                                                                                                          //client.Inventory.Add(stream, 121187, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //client.Inventory.Add(stream, 152207, 1, 3, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//bag
                                                                                                                                                                                          //152066
                            }
                            break;
                        }
                        else
                        {
                            client.SendSysMesage("Please, make sure you are level 15 or more and 2nd reborn to open this BeginnerPack.");
                            break;
                        }
                    }
                #endregion
                case 723584u:
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream) && client.Equipment.TryGetEquip(Flags.ConquerItem.Armor, out var Coat))
                        {
                            Coat.Color = Flags.Color.Black;
                            Coat.Mode = Flags.ItemMode.Update;
                            Coat.Send(client, stream);
                            client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                        }
                        break;
                    }
                case 720891u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        client.Inventory.Add(stream, 1088000, 3, 0, 0, 0);
                    break;
                case 720884u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        client.Inventory.Add(stream, 1088000, 5, 0, 0, 0);
                    break;
                case 1060030u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.HairColor = 3;
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                    break;
                case 1060040u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.HairColor = 9;
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                    break;
                case 1060050u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.HairColor = 8;
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                    break;
                case 1060060u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.HairColor = 7;
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                    break;
                case 1060070u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.HairColor = 6;
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                    break;
                case 1060080u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.HairColor = 5;
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                    break;
                case 1060090u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.HairColor = 4;
                    client.Player.SendUpdate(stream, client.Player.Hair, MsgUpdate.DataType.HairStyle);
                    break;
                case 720399u:
                    if (!client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        break;
                    if (Core.Rate(3, 10))
                    {
                        if (Core.Rate(1, 28))
                        {
                            client.Inventory.Add(stream, 724404, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (Bow).");
                        }
                        else if (Core.Rate(1, 27))
                        {
                            client.Inventory.Add(stream, 724409, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (1-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 26))
                        {
                            client.Inventory.Add(stream, 724414, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (2-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 25))
                        {
                            client.Inventory.Add(stream, 3006169, 1, 0, 0, 0);
                            client.SendSysMesage("You~received~a~Super~Hossu!");
                        }
                        else if (Core.Rate(1, 3))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724419, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Critical Strike material (Ring).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724424, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Skill Critical Strike material (Backsword).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724429, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Skill Critical Strike material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724434, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Immunity material (Armor).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724439, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Immunity material (Boots).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724444, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Intensification material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724453, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (1-Handed Weapon).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724458, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (2-Handed Weapon).");
                                    break;
                            }
                        }
                        else if (Core.Rate(1, 2))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724463, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (Bow).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724472, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (Ring).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724477, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Counteraction material (Armor).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724482, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Counteraction material (Necklace).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724487, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Headgear).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724492, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Armor).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724497, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Boots).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724352, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Necklace).");
                                    break;
                            }
                        }
                        else
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724357, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Block material (Headgear).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724362, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Block material (Shield).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724367, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Breakthrough material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724372, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Counteraction material (Bag).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724377, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Detoxication material (Bag).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724384, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Penetration material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724389, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Penetration material (Bag).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724394, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened the Superior Refinery Pack and received a Super Penetration material (Bracelet).");
                                    break;
                            }
                        }
                    }
                    else if (Core.Rate(1, 28))
                    {
                        client.Inventory.Add(stream, 724403, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (Bow).");
                    }
                    else if (Core.Rate(1, 27))
                    {
                        client.Inventory.Add(stream, 724408, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (1-Handed Weapon).");
                    }
                    else if (Core.Rate(1, 26))
                    {
                        client.Inventory.Add(stream, 724413, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (2-Handed Weapon).");
                    }
                    else if (Core.Rate(1, 25))
                    {
                        client.Inventory.Add(stream, 3006168, 1, 0, 0, 0);
                        client.SendSysMesage("You~received~an~Elite~Hossu!");
                    }
                    else if (Core.Rate(1, 3))
                    {
                        switch (ServerKernel.NextAsync(0, 8))
                        {
                            case 0:
                                client.Inventory.Add(stream, 724418, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Critical Strike material (Ring).");
                                break;
                            case 1:
                                client.Inventory.Add(stream, 724423, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Skill Critical Strike material (Backsword).");
                                break;
                            case 2:
                                client.Inventory.Add(stream, 724428, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Skill Critical Strike material (Bracelet).");
                                break;
                            case 3:
                                client.Inventory.Add(stream, 724433, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Immunity material (Armor).");
                                break;
                            case 4:
                                client.Inventory.Add(stream, 724438, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Immunity material (Boots).");
                                break;
                            case 5:
                                client.Inventory.Add(stream, 724443, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Intensification material (Headgear).");
                                break;
                            case 6:
                                client.Inventory.Add(stream, 724452, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (1-Handed Weapon).");
                                break;
                            case 7:
                                client.Inventory.Add(stream, 724457, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (2-Handed Weapon).");
                                break;
                        }
                    }
                    else if (Core.Rate(1, 2))
                    {
                        switch (ServerKernel.NextAsync(0, 8))
                        {
                            case 0:
                                client.Inventory.Add(stream, 724462, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (Bow).");
                                break;
                            case 1:
                                client.Inventory.Add(stream, 724471, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (Ring).");
                                break;
                            case 2:
                                client.Inventory.Add(stream, 724476, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Counteraction material (Armor).");
                                break;
                            case 3:
                                client.Inventory.Add(stream, 724481, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Counteraction material (Necklace).");
                                break;
                            case 4:
                                client.Inventory.Add(stream, 724486, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Headgear).");
                                break;
                            case 5:
                                client.Inventory.Add(stream, 724491, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Armor).");
                                break;
                            case 6:
                                client.Inventory.Add(stream, 724496, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Boots).");
                                break;
                            case 7:
                                client.Inventory.Add(stream, 724351, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Necklace).");
                                break;
                        }
                    }
                    else
                    {
                        switch (ServerKernel.NextAsync(0, 8))
                        {
                            case 0:
                                client.Inventory.Add(stream, 724356, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Block material (Headgear).");
                                break;
                            case 1:
                                client.Inventory.Add(stream, 724361, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Block material (Shield).");
                                break;
                            case 2:
                                client.Inventory.Add(stream, 724366, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Breakthrough material (Bracelet).");
                                break;
                            case 3:
                                client.Inventory.Add(stream, 724371, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Counteraction material (Bag).");
                                break;
                            case 4:
                                client.Inventory.Add(stream, 724376, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Detoxication material (Bag).");
                                break;
                            case 5:
                                client.Inventory.Add(stream, 724383, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Penetration material (Headgear).");
                                break;
                            case 6:
                                client.Inventory.Add(stream, 724388, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Penetration material (Bag).");
                                break;
                            case 7:
                                client.Inventory.Add(stream, 724393, 1, 0, 0, 0);
                                client.SendSysMesage("You opened the Superior Refinery Pack and received an Elite Penetration material (Bracelet).");
                                break;
                        }
                    }
                    break;
                case 720549u:
                    if (!client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        break;
                    if (Core.Rate(1, 30))
                    {
                        if (Core.Rate(1, 27))
                        {
                            client.Inventory.Add(stream, 724404, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received a Super Critical Strike material (Bow).");
                        }
                        else if (Core.Rate(1, 26))
                        {
                            client.Inventory.Add(stream, 724409, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received a Super Critical Strike material (1-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 25))
                        {
                            client.Inventory.Add(stream, 724414, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received a Super Critical Strike material (2-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 3))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724419, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Critical Strike material (Ring).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724424, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Skill Critical Strike material (Backsword).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724429, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Skill Critical Strike material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724434, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Immunity material (Armor).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724439, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Immunity material (Boots).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724444, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Intensification material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724453, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Breakthrough material (1-Handed Weapon).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724458, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Breakthrough material (2-Handed Weapon).");
                                    break;
                            }
                        }
                        else if (Core.Rate(1, 2))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724463, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Breakthrough material (Bow).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724472, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Breakthrough material (Ring).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724477, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Counteraction material (Armor).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724482, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Counteraction material (Necklace).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724487, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Detoxication material (Headgear).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724492, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Detoxication material (Armor).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724497, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Detoxication material (Boots).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724352, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Detoxication material (Necklace).");
                                    break;
                            }
                        }
                        else
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724357, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Block material (Headgear).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724362, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Block material (Shield).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724367, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Breakthrough material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724372, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Counteraction material (Bag).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724377, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Detoxication material (Bag).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724384, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Penetration material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724389, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Penetration material (Bag).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724394, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Super Penetration material (Bracelet).");
                                    break;
                            }
                        }
                    }
                    else if (Core.Rate(1, 10))
                    {
                        if (Core.Rate(1, 27))
                        {
                            client.Inventory.Add(stream, 724403, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (Bow).");
                        }
                        else if (Core.Rate(1, 26))
                        {
                            client.Inventory.Add(stream, 724408, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (1-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 25))
                        {
                            client.Inventory.Add(stream, 724413, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (2-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 3))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724418, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Critical Strike material (Ring).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724423, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Skill Critical Strike material (Backsword).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724428, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Skill Critical Strike material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724433, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Immunity material (Armor).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724438, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Immunity material (Boots).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724443, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Intensification material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724452, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (1-Handed Weapon).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724457, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (2-Handed Weapon).");
                                    break;
                            }
                        }
                        else if (Core.Rate(1, 2))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724462, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (Bow).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724471, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (Ring).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724476, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Counteraction material (Armor).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724481, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Counteraction material (Necklace).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724486, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Headgear).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724491, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Armor).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724496, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Boots).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724351, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Necklace).");
                                    break;
                            }
                        }
                        else
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724356, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Block material (Headgear).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724361, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Block material (Shield).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724366, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Breakthrough material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724371, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Counteraction material (Bag).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724376, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Detoxication material (Bag).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724383, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Penetration material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724388, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Penetration material (Bag).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724393, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received an Elite Penetration material (Bracelet).");
                                    break;
                            }
                        }
                    }
                    else if (Core.Rate(4, 9))
                    {
                        if (Core.Rate(1, 27))
                        {
                            client.Inventory.Add(stream, 724402, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (Bow).");
                        }
                        else if (Core.Rate(1, 26))
                        {
                            client.Inventory.Add(stream, 724407, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (1-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 25))
                        {
                            client.Inventory.Add(stream, 724412, 1, 0, 0, 0);
                            client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (2-Handed Weapon).");
                        }
                        else if (Core.Rate(1, 3))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724417, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Critical Strike material (Ring).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724422, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Skill Critical Strike material (Backsword).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724427, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Skill Critical Strike material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724432, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Immunity material (Armor).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724437, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Immunity material (Boots).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724442, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Intensification material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724451, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (1-Handed Weapon).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724456, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (2-Handed Weapon).");
                                    break;
                            }
                        }
                        else if (Core.Rate(1, 2))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724461, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (Bow).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724470, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (Ring).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724475, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Counteraction material (Armor).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724480, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Counteraction material (Necklace).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724485, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Headgear).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724490, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Armor).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724495, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Boots).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724350, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Necklace).");
                                    break;
                            }
                        }
                        else
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 724355, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Block material (Headgear).");
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 724360, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Block material (Shield).");
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 724365, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Breakthrough material (Bracelet).");
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 724370, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Counteraction material (Bag).");
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 724375, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Detoxication material (Bag).");
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 724382, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Penetration material (Headgear).");
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 724387, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Penetration material (Bag).");
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 724392, 1, 0, 0, 0);
                                    client.SendSysMesage("You opened a Refinery Pack and received a Unique Penetration material (Bracelet).");
                                    break;
                            }
                        }
                    }
                    else if (Core.Rate(1, 27))
                    {
                        client.Inventory.Add(stream, 724401, 1, 0, 0, 0);
                        client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (Bow).");
                    }
                    else if (Core.Rate(1, 26))
                    {
                        client.Inventory.Add(stream, 724406, 1, 0, 0, 0);
                        client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (1-Handed Weapon).");
                    }
                    else if (Core.Rate(1, 25))
                    {
                        client.Inventory.Add(stream, 724411, 1, 0, 0, 0);
                        client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (2-Handed Weapon).");
                    }
                    else if (Core.Rate(1, 3))
                    {
                        switch (ServerKernel.NextAsync(0, 8))
                        {
                            case 0:
                                client.Inventory.Add(stream, 724416, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Critical Strike material (Ring).");
                                break;
                            case 1:
                                client.Inventory.Add(stream, 724421, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Skill Critical Strike material (Backsword).");
                                break;
                            case 2:
                                client.Inventory.Add(stream, 724426, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Skill Critical Strike material (Bracelet).");
                                break;
                            case 3:
                                client.Inventory.Add(stream, 724431, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Immunity material (Armor).");
                                break;
                            case 4:
                                client.Inventory.Add(stream, 724436, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Immunity material (Boots).");
                                break;
                            case 5:
                                client.Inventory.Add(stream, 724441, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Intensification material (Headgear).");
                                break;
                            case 6:
                                client.Inventory.Add(stream, 724450, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (1-Handed Weapon).");
                                break;
                            case 7:
                                client.Inventory.Add(stream, 724455, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (2-Handed Weapon).");
                                break;
                        }
                    }
                    else if (Core.Rate(1, 2))
                    {
                        switch (ServerKernel.NextAsync(0, 8))
                        {
                            case 0:
                                client.Inventory.Add(stream, 724460, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (Bow).");
                                break;
                            case 1:
                                client.Inventory.Add(stream, 724465, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (Ring).");
                                break;
                            case 2:
                                client.Inventory.Add(stream, 724474, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Counteraction material (Armor).");
                                break;
                            case 3:
                                client.Inventory.Add(stream, 724479, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Counteraction material (Necklace).");
                                break;
                            case 4:
                                client.Inventory.Add(stream, 724484, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Headgear).");
                                break;
                            case 5:
                                client.Inventory.Add(stream, 724489, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Armor).");
                                break;
                            case 6:
                                client.Inventory.Add(stream, 724494, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Boots).");
                                break;
                            case 7:
                                client.Inventory.Add(stream, 724499, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Necklace).");
                                break;
                        }
                    }
                    else
                    {
                        switch (ServerKernel.NextAsync(0, 8))
                        {
                            case 0:
                                client.Inventory.Add(stream, 724354, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Block material (Headgear).");
                                break;
                            case 1:
                                client.Inventory.Add(stream, 724359, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Block material (Shield).");
                                break;
                            case 2:
                                client.Inventory.Add(stream, 724364, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Breakthrough material (Bracelet).");
                                break;
                            case 3:
                                client.Inventory.Add(stream, 724369, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Counteraction material (Bag).");
                                break;
                            case 4:
                                client.Inventory.Add(stream, 724374, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Detoxication material (Bag).");
                                break;
                            case 5:
                                client.Inventory.Add(stream, 724381, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Penetration material (Headgear).");
                                break;
                            case 6:
                                client.Inventory.Add(stream, 724386, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Penetration material (Bag).");
                                break;
                            case 7:
                                client.Inventory.Add(stream, 724391, 1, 0, 0, 0);
                                client.SendSysMesage("You opened a Refinery Pack and received a Refined Penetration material (Bracelet).");
                                break;
                        }
                    }
                    break;
                case 724186u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724382, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Unique Hemstitch.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724383, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Elite Hemstitch.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724384, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Super Hemstitch.");
                        }
                    }
                    break;
                case 724171u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724360, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Pin Pack and got the Unique Pin.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724361, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Pin Pack and got the Elite Pin.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724362, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Pin Pack and got the Super Pin.");
                        }
                    }
                    break;
                case 724147u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724355, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Hat Rope Pack and got the Unique Hat Rope.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724356, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Hat Rope Pack and got the Elite Hat Rope.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724357, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Hat Rope Pack and got the Super Hat Rope.");
                        }
                    }
                    break;
                case 724177u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724490, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Belt Pack and got the Unique Belt.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724491, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Belt Pack and got the Elite Belt.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724492, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Belt Pack and got the Super Belt.");
                        }
                    }
                    break;
                case 724174u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724485, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Unique Tri Plume.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724486, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Elite Tri Plume.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724487, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Super Tri Plume.");
                        }
                    }
                    break;
                case 724144u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724475, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Breastplate Pack and got the Unique Breastplate.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724476, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Breastplate Pack and got the Elite Breastplate.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724477, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Breastplate Pack and got the Super Breastplate.");
                        }
                    }
                    break;
                case 724165u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724456, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Cauterant Pack and got the Unique Cauterant.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724457, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Cauterant Pack and got the Elite Cauterant.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724458, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Cauterant Pack and got the Super Cauterant.");
                        }
                    }
                    break;
                case 724168u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724461, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Unique Bow Limb.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724462, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Elite Bow Limb.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724463, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Super Bow Limb.");
                        }
                    }
                    break;
                case 724162u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724451, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Cutter Pack and got the Unique Cutter.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724452, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Cutter Pack and got the Elite Cutter.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724453, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Cutter Pack and got the Super Cutter.");
                        }
                    }
                    break;
                case 724141u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724442, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Visor Pack and got the Unique Visor.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724443, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Visor Pack and got the Elite Visor.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724444, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Visor Pack and got the Super Visor.");
                        }
                    }
                    break;
                case 724138u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724432, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Plate Pack and got the Unique Plate.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724433, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Plate Pack and got the Elite Plate.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724434, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Plate Pack and got the Super Plate.");
                        }
                    }
                    break;
                case 724135u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724422, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Unique Long Tassel.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724423, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Elite Long Tassel.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724424, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Super Long Tassel.");
                        }
                    }
                    break;
                case 724159u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724412, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Unique Red Tassel.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724413, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Elite Red Tassel.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724414, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Super Red Tassel.");
                        }
                    }
                    break;
                case 724153u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724402, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Bowstring Pack and got the Unique Bowstring.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724403, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Bowstring Pack and got the Elite Bowstring.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724404, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Bowstring Pack and got the Super Bowstring.");
                        }
                    }
                    break;
                case 724156u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724407, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Veneer Pack and got the Unique Veneer.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724408, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Veneer Pack and got the Elite Veneer.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724409, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Superior Veneer Pack and got the Super Veneer.");
                        }
                    }
                    break;
                case 724131u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724391, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bell Pack and got the Refined Bell.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724392, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bell Pack and got the Unique Bell.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724393, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bell Pack and got the Elite Bell.");
                        }
                    }
                    break;
                case 724185u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724381, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Refined Hemstitch.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724382, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Unique Hemstitch.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724383, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Elite Hemstitch.");
                        }
                    }
                    break;
                case 724170u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724359, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Pin Pack and got the Refined Pin.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724360, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Pin Pack and got the Unique Pin.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724361, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Pin Pack and got the Elite Pin.");
                        }
                    }
                    break;
                case 724146u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724354, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Hat Rope Pack and got the Refined Hat Rope.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724355, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Hat Rope Pack and got the Unique Hat Rope.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724356, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Hat Rope Pack and got the Elite Hat Rope.");
                        }
                    }
                    break;
                case 724176u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724489, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Belt Pack and got the Refined Belt.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724490, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Belt Pack and got the Unique Belt.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724491, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Belt Pack and got the Elite Belt.");
                        }
                    }
                    break;
                case 724173u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724484, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Refined Tri Plume.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724485, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Unique Tri Plume.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724486, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Elite Tri Plume.");
                        }
                    }
                    break;
                case 724143u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724474, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Breastplate Pack and got the Refined Breastplate.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724475, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Breastplate Pack and got the Unique Breastplate.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724476, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Breastplate Pack and got the Elite Breastplate.");
                        }
                    }
                    break;
                case 724164u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724455, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Cauterant Pack and got the Refined Cauterant.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724456, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Cauterant Pack and got the Unique Cauterant.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724457, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Cauterant Pack and got the Elite Cauterant.");
                        }
                    }
                    break;
                case 724167u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724460, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Refined Bow Limb.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724461, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Unique Bow Limb.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724462, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Elite Bow Limb.");
                        }
                    }
                    break;
                case 724161u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724450, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Cutter Pack and got the Refined Cutter.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724451, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Cutter Pack and got the Unique Cutter.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724452, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Cutter Pack and got the Elite Cutter.");
                        }
                    }
                    break;
                case 724140u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724441, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Visor Pack and got the Refined Visor.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724442, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Visor Pack and got the Unique Visor.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724443, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Visor Pack and got the Elite Visor.");
                        }
                    }
                    break;
                case 724137u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724431, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Plate Pack and got the Refined Plate.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724432, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Plate Pack and got the Unique Plate.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724433, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Plate Pack and got the Elite Plate.");
                        }
                    }
                    break;
                case 724134u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724421, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Refined Long Tassel.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724422, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Unique Long Tassel.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724423, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Elite Long Tassel.");
                        }
                    }
                    break;
                case 724158u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724411, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Refined Red Tassel.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724412, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Unique Red Tassel.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724413, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Elite Red Tassel.");
                        }
                    }
                    break;
                case 724152u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724401, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bowstring Pack and got the Refined Bowstring.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724402, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bowstring Pack and got the Unique Bowstring.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724403, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Bowstring Pack and got the Elite Bowstring.");
                        }
                    }
                    break;
                case 724155u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724406, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Veneer Pack and got the Refined Veneer.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724407, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Veneer Pack and got the Unique Veneer.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724408, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Medium Veneer Pack and got the Elite Veneer.");
                        }
                    }
                    break;
                case 724130u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724390, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bell Pack and got the Normal Bell.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724391, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bell Pack and got the Refined Bell.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724392, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bell Pack and got the Unique Bell.");
                        }
                    }
                    break;
                case 724184u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724380, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Normal Hemstitch.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724381, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Refined Hemstitch.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724382, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Unique Hemstitch.");
                        }
                    }
                    break;
                case 724169u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724358, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Pin Pack and got the Normal Pin.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724359, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Pin Pack and got the Refined Pin.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724360, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Pin Pack and got the Unique Pin.");
                        }
                    }
                    break;
                case 724145u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724353, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Hat Rope Pack and got the Normal Hat Rope.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724354, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Hat Rope Pack and got the Refined Hat Rope.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724355, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Hat Rope Pack and got the Unique Hat Rope.");
                        }
                    }
                    break;
                case 724175u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724488, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Belt Pack and got the Normal Belt.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724489, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Belt Pack and got the Refined Belt.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724490, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Belt Pack and got the Unique Belt.");
                        }
                    }
                    break;
                case 750000:
                    {
                        if (client.DemonExterminator != null)
                        {
                            client.SendSysMesage("You currently have " + client.DemonExterminator.HuntKills + "KOs of the required " + item.Durability + ". Please keep in mind the jar will sometimes list incorrect amounts.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
                        }
                        break;
                    }
                case 724172u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724483, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Normal Tri Plume.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724484, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Refined Tri Plume.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724485, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Unique Tri Plume.");
                        }
                    }
                    break;
                case 724142u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724473, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Breastplate Pack and got the Normal Breastplate.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724474, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Breastplate Pack and got the Refined Breastplate.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724475, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Breastplate Pack and got the Unique Breastplate.");
                        }
                    }
                    break;
                case 724163u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724454, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Cauterant Pack and got the Normal Cauterant.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724455, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Cauterant Pack and got the Refined Cauterant.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724456, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Cauterant Pack and got the Unique Cauterant.");
                        }
                    }
                    break;
                case 724166u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724459, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Normal Bow Limb.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724460, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Refined Bow Limb.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724461, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Unique Bow Limb.");
                        }
                    }
                    break;
                case 724160u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724445, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Cutter Pack and got the Normal Cutter.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724450, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Cutter Pack and got the Refined Cutter.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724451, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Cutter Pack and got the Unique Cutter.");
                        }
                    }
                    break;
                case 724139u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724440, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Visor Pack and got the Normal Visor.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724441, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Visor Pack and got the Refined Visor.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724442, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Visor Pack and got the Unique Visor.");
                        }
                    }
                    break;
                case 724136u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724430, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Plate Pack and got the Normal Plate.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724431, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Plate Pack and got the Refined Plate.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724432, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Plate Pack and got the Unique Plate.");
                        }
                    }
                    break;
                case 724133u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724420, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Normal Long Tassel.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724421, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Refined Long Tassel.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724422, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Unique Long Tassel.");
                        }
                    }
                    break;
                case 727000u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        client.Inventory.Add(stream, 1050002, 1, 0, 0, 0);
                    break;
                case 724157u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724410, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Normal Red Tassel.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724411, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Refined Red Tassel.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724412, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Unique Red Tassel.");
                        }
                    }
                    break;
                case 724151u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724400, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bowstring Pack and got the Normal Bowstring.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724401, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bowstring Pack and got the Refined Bowstring.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724402, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Bowstring Pack and got the Unique Bowstring.");
                        }
                    }
                    break;
                case 724154u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (Core.Rate(60, 100))
                        {
                            client.Inventory.Add(stream, 724405, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Veneer Pack and got the Normal Veneer.");
                        }
                        else if (Core.Rate(30, 40))
                        {
                            client.Inventory.Add(stream, 724406, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Veneer Pack and got the Refined Veneer.");
                        }
                        else
                        {
                            client.Inventory.Add(stream, 724407, 1, 0, 0, 0);
                            client.SendSysMesage("You opened the Primary Veneer Pack and got the Unique Veneer.");
                        }
                    }
                    break;
                case 723130u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721390, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bell Pack and got the Normal Bell (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721391, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bell Pack and got the Refined Bell (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721392, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bell Pack and got the Unique Bell (B).");
                    }
                    break;
                case 723131u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721391, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bell Pack and got the Refined Bell (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721392, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bell Pack and got the Unique Bell (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721393, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bell Pack and got the Elite Bell (B).");
                    }
                    break;
                case 723132u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721392, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bell Pack and got the Unique Bell (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721393, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bell Pack and got the Elite Bell (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721394, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bell Pack and got the Super Bell (B).");
                    }
                    break;
                case 723684u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721380, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Normal Hemstitch (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721381, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Refined Hemstitch (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Unique Hemstitch (B).");
                    }
                    break;
                case 723685u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721381, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Refined Hemstitch (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Unique Hemstitch (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721383, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Hemstitch Pack and got the Elite Hemstitch (B).");
                    }
                    break;
                case 723686u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Unique Hemstitch (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721383, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Elite Hemstitch (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721384, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Hemstitch Pack and got the Super Hemstitch (B).");
                    }
                    break;
                case 724148u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724385, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Silk Rope Pack and got the Normal Silk Rope.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724386, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Silk Rope Pack and got the Refined Silk Rope.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724387, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Silk Rope Pack and got the Unique Silk Rope.");
                    }
                    break;
                case 724149u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724386, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Silk Rope Pack and got the Refined Silk Rope.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724387, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Silk Rope Pack and got the Unique Silk Rope.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724388, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Silk Rope Pack and got the Elite Silk Rope.");
                    }
                    break;
                case 724150u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724387, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Silk Rope Pack and got the Unique Silk Rope.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724388, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Silk Rope Pack and got the Elite Silk Rope.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724389, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Silk Rope Pack and got the Super Silk Rope.");
                    }
                    break;
                case 723133u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721420, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Normal Long Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721421, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Refined Long Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721422, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Long Tassel Pack and got the Unique Long Tassel (B).");
                    }
                    break;
                case 723134u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721421, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Refined Long Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721422, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Unique Long Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721423, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Long Tassel Pack and got the Elite Long Tassel (B).");
                    }
                    break;
                case 723135u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721422, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Unique Long Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721423, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Elite Long Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721424, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Long Tassel Pack and got the Super Long Tassel (B).");
                    }
                    break;
                case 724195u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724425, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bead Pack and got a Normal Bead.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724426, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bead Pack and got a Refined Bead.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724427, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bead Pack and got an Unique Bead.");
                    }
                    break;
                case 724196u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724426, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bead Pack and got a Refined Bead.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724427, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bead Pack and got an Unique Bead.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724428, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bead Pack and got an Elite Bead.");
                    }
                    break;
                case 724197u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724427, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bead Pack and got an Unique Bead.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724428, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bead Pack and got an Elite Bead.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724429, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bead Pack and got a Super Bead.");
                    }
                    break;
                case 723669u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721358, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Pin Pack and got the Normal Pin (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721359, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Pin Pack and got the Refined Pin (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721360, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Pin Pack and got the Unique Pin (B).");
                    }
                    break;
                case 723670u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721359, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Pin Pack and got the Refined Pin (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721360, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Pin Pack and got the Unique Pin (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721361, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Pin Pack and got the Elite Pin (B).");
                    }
                    break;
                case 723671u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721360, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Pin Pack and got the Unique Pin (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721361, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Pin Pack and got the Elite Pin (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721362, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Pin Pack and got the Super Pin (B).");
                    }
                    break;
                case 723651u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721400, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bowstring Pack and got the Normal Bowstring (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721401, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bowstring Pack and got the Refined Bowstring (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721402, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bowstring Pack and got the Unique Bowstring (B).");
                    }
                    break;
                case 723652u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721401, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bowstring Pack and got the Refined Bowstring (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721402, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bowstring Pack and got the Unique Bowstring (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721403, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bowstring Pack and got the Elite Bowstring (B).");
                    }
                    break;
                case 723653u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721402, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bowstring Pack and got the Unique Bowstring (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721403, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bowstring Pack and got the Elite Bowstring (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721404, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bowstring Pack and got the Super Bowstring (B).");
                    }
                    break;
                case 723654u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721405, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Veneer Pack and got the Normal Veneer (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721406, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Veneer Pack and got the Refined Veneer (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721407, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Veneer Pack and got the Unique Veneer (B).");
                    }
                    break;
                case 723655u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721406, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Veneer Pack and got the Refined Veneer (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721407, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Veneer Pack and got the Unique Veneer (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721408, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Veneer Pack and got the Elite Veneer (B).");
                    }
                    break;
                case 723656u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721407, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Veneer Pack and got the Unique Veneer (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721408, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Veneer Pack and got the Elite Veneer (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721409, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Veneer Pack and got the Super Veneer (B).");
                    }
                    break;
                case 723657u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721410, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Normal Red Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721411, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Refined Red Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721412, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Red Tassel Pack and got the Unique Red Tassel (B).");
                    }
                    break;
                case 723658u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721411, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Refined Red Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721412, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Unique Red Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721413, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Red Tassel Pack and got the Elite Red Tassel (B).");
                    }
                    break;
                case 723659u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721412, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Unique Red Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721413, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Elite Red Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721414, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Red Tassel Pack and got the Super Red Tassel (B).");
                    }
                    break;
                case 724215u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724415, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Amber Pack and got a Normal Amber.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724416, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Amber Pack and got a Refined Amber.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724417, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Amber Pack and got an Unique Amber.");
                    }
                    break;
                case 724193u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724416, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Amber Pack and got a Refined Amber.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724417, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Amber Pack and got an Unique Amber.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724418, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Amber Pack and got an Elite Amber.");
                    }
                    break;
                case 724194u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724417, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Amber Pack and got an Unique Amber.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724418, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Amber Pack and got an Elite Amber.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724419, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Amber Pack and got a Super Amber.");
                    }
                    break;
                case 723681u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721380, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Normal Hemstitch (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721381, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Refined Hemstitch (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721382, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Hemstitch Pack and got the Unique Hemstitch (B).");
                    }
                    break;
                case 723682u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721499, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Jasper Pack and got the Refined Jasper (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721350, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Jasper Pack and got the Unique Jasper (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721351, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Jasper Pack and got the Elite Jasper (B).");
                    }
                    break;
                case 723683u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721350, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Jasper Pack and got the Unique Jasper (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721351, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Jasper Pack and got the Elite Jasper (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721352, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Jasper Pack and got the Super Jasper (B).");
                    }
                    break;
                case 723672u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721483, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Normal Tri Plume (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721484, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Refined Tri Plume (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721485, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Tri Plume Pack and got the Unique Tri Plume (B).");
                    }
                    break;
                case 723673u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721484, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Refined Tri Plume (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721485, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Unique Tri Plume (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721486, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Tri Plume Pack and got the Elite Tri Plume (B).");
                    }
                    break;
                case 723674u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721485, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Unique Tri Plume (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721486, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Elite Tri Plume (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721487, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Tri Plume Pack and got the Super Tri Plume (B).");
                    }
                    break;
                case 723690u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721373, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Tassel Pack and got the Normal Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721374, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Tassel Pack and got the Refined Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721375, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Tassel Pack and got the Unique Tassel (B).");
                    }
                    break;
                case 723691u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721374, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Tassel Pack and got the Refined Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721375, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Tassel Pack and got the Unique Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721376, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Tassel Pack and got the Elite Tassel (B).");
                    }
                    break;
                case 723692u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721375, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Tassel Pack and got the Unique Tassel (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721376, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Tassel Pack and got the Elite Tassel (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721377, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Tassel Pack and got the Super Tassel (B).");
                    }
                    break;
                case 723675u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721488, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Belt Pack and got the Normal Belt (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721489, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Belt Pack and got the Refined Belt (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721490, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Belt Pack and got the Unique Belt (B).");
                    }
                    break;
                case 723676u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721489, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Belt Pack and got the Refined Belt (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721490, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Belt Pack and got the Unique Belt (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721491, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Belt Pack and got the Elite Belt (B).");
                    }
                    break;
                case 723677u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721490, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Belt Pack and got the Unique Belt (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721491, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Belt Pack and got the Elite Belt (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721492, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Belt Pack and got the Super Belt (B).");
                    }
                    break;
                case 723678u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721493, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Normal Cloud Silk (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721494, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Refined Cloud Silk (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721495, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Unique Cloud Silk (B).");
                    }
                    break;
                case 723679u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721494, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Refined Cloud Silk (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721495, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Unique Cloud Silk (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721496, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Elite Cloud Silk (B).");
                    }
                    break;
                case 723680u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721495, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Unique Cloud Silk (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721496, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Elite Cloud Silk (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721497, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Super Cloud Silk (B).");
                    }
                    break;
                case 723660u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721445, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cutter Pack and got the Normal Cutter (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721450, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cutter Pack and got the Refined Cutter (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721451, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cutter Pack and got the Unique Cutter (B).");
                    }
                    break;
                case 723661u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721450, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cutter Pack and got the Refined Cutter (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721451, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cutter Pack and got the Unique Cutter (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721452, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cutter Pack and got the Elite Cutter (B).");
                    }
                    break;
                case 723662u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721451, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cutter Pack and got the Unique Cutter (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721452, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cutter Pack and got the Elite Cutter (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721453, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cutter Pack and got the Super Cutter (B).");
                    }
                    break;
                case 723663u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721454, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cauterant Pack and got the Normal Cauterant (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721455, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cauterant Pack and got the Refined Cauterant (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721456, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Cauterant Pack and got the Unique Cauterant (B).");
                    }
                    break;
                case 723664u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721455, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cauterant Pack and got the Refined Cauterant (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721456, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cauterant Pack and got the Unique Cauterant (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721457, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Cauterant Pack and got the Elite Cauterant (B).");
                    }
                    break;
                case 723665u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721456, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cauterant Pack and got the Unique Cauterant (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721457, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cauterant Pack and got the Elite Cauterant (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721458, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Cauterant Pack and got the Super Cauterant (B).");
                    }
                    break;
                case 724201u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724363, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Wristband Pack and got a Normal Wristband.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724364, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Wristband Pack and got a Refined Wristband.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724365, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Wristband Pack and got an Unique Wristband.");
                    }
                    break;
                case 724202u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724364, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Wristband Pack and got a Refined Wristband.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724365, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Wristband Pack and got an Unique Wristband.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724366, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Wristband Pack and got an Elite Wristband.");
                    }
                    break;
                case 724203u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724365, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Wristband Pack and got an Unique Wristband.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724366, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Wristband Pack and got an Elite Wristband.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724367, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Wristband Pack and got a Super Wristband.");
                    }
                    break;
                case 723666u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721459, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Normal Bow Limb (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721460, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Refined Bow Limb (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721461, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Bow Limb Pack and got the Unique Bow Limb (B).");
                    }
                    break;
                case 723667u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721460, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Refined Bow Limb (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721461, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Unique Bow Limb (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721462, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Bow Limb Pack and got the Elite Bow Limb (B).");
                    }
                    break;
                case 723668u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 721461, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Unique Bow Limb (B).");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 721462, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Elite Bow Limb (B).");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 721463, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Bow Limb Pack and got the Super Bow Limb (B).");
                    }
                    break;
                case 724198u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724464, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Jadeite Pack and got a Normal Jadeite.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724465, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Jadeite Pack and got a Refined Jadeite.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724470, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Jadeite Pack and got an Unique Jadeite.");
                    }
                    break;
                case 724199u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724465, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Jadeite Pack and got a Refined Jadeite.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724470, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Jadeite Pack and got an Unique Jadeite.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724471, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Jadeite Pack and got an Elite Jadeite.");
                    }
                    break;
                case 724200u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724470, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Jadeite Pack and got an Unique Jadeite.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724471, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Jadeite Pack and got an Elite Jadeite.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724472, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Jadeite Pack and got a Super Jadeite.");
                    }
                    break;
                case 724207u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724368, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Fringe Pack and got a Normal Fringe.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724369, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Fringe Pack and got a Refined Fringe.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724370, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Fringe Pack and got an Unique Fringe.");
                    }
                    break;
                case 724208u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724369, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Fringe Pack and got a Refined Fringe.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724370, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Fringe Pack and got an Unique Fringe.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724371, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Fringe Pack and got an Elite Fringe.");
                    }
                    break;
                case 724209u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724370, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Fringe Pack and got an Unique Fringe.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724371, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Fringe Pack and got an Elite Fringe.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724372, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Fringe Pack and got a Super Fringe.");
                    }
                    break;
                case 724204u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724478, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Aglet Pack and got a Normal Aglet.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724479, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Aglet Pack and got a Refined Aglet.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724480, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Aglet Pack and got an Unique Aglet.");
                    }
                    break;
                case 724205u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724479, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Aglet Pack and got a Refined Aglet.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724480, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Aglet Pack and got an Unique Aglet.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724481, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Aglet Pack and got an Elite Aglet.");
                    }
                    break;
                case 724206u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724480, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Aglet Pack and got an Unique Aglet.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724481, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Aglet Pack and got an Elite Aglet.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724482, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Aglet Pack and got a Super Aglet.");
                    }
                    break;
                case 724210u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724435, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Spur Pack and got a Normal Spur.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724436, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Spur Pack and got a Refined Spur.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724437, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Primary Spur Pack and got an Unique Spur.");
                    }
                    break;
                case 724211u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724436, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Spur Pack and got a Refined Spur.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724437, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Spur Pack and got an Unique Spur.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724438, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Medium Spur Pack and got an Elite Spur.");
                    }
                    break;
                case 724212u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724437, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Spur Pack and got an Unique Spur.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724438, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Spur Pack and got an Elite Spur.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724439, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the Superior Spur Pack and got a Super Spur.");
                    }
                    break;
                case 725160u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725192, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Necklace.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725193, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Necklace.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725194, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Necklace.");
                    }
                    break;
                case 725161u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725193, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Necklace.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725194, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Necklace.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725195, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Necklace.");
                    }
                    break;
                case 725162u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725194, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Necklace.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725195, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Necklace.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725196, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Necklace.");
                    }
                    break;
                case 725163u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725197, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Bag.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725198, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Bag.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725199, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Bag.");
                    }
                    break;
                case 725164u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725198, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Bag.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725199, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Bag.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725200, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Bag.");
                    }
                    break;
                case 725165u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725199, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Bag.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725200, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Bag.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725201, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Bag.");
                    }
                    break;
                case 725166u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725202, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Bracelet.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725203, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Bracelet.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Bracelet.");
                    }
                    break;
                case 725167u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725203, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Bracelet.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Bracelet.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725205, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Bracelet.");
                    }
                    break;
                case 725168u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Bracelet.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725205, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Bracelet.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725206, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Bracelet.");
                    }
                    break;
                case 725169u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725202, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Normal) Material for a Bracelet.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725203, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Refined) Material for a Bracelet.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725204, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Primary) Pack and received a M-Defense (Unique) Material for a Bracelet.");
                    }
                    break;
                case 725170u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725208, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Refined) Material for a Heavy Ring or Ring.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725209, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Unique) Material for a Heavy Ring or Ring.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725210, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Medium) Pack and received a M-Defense (Elite) Material for a Heavy Ring or Ring.");
                    }
                    break;
                case 725171u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 725209, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Unique) Material for a Heavy Ring or Ring.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 725210, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Elite) Material for a Heavy Ring or Ring.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 725211, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                        client.SendSysMesage("You opened the M-Defense (Superior) Pack and received a M-Defense (Super) Material for a Heavy Ring or Ring.");
                    }
                    break;
                case 724178u:
                    client.Inventory.Remove(724178, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724493, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Normal Cloud Silk.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724494, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Refined Cloud Silk.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724495, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Cloud Silk Pack and got the Unique Cloud Silk.");
                    }
                    break;
                case 724179u:
                    client.Inventory.Remove(724179, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724494, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Refined Cloud Silk.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724495, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Unique Cloud Silk.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724496, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Cloud Silk Pack and got the Elite Cloud Silk.");
                    }
                    break;
                case 724180u:
                    client.Inventory.Remove(724180, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724495, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Unique Cloud Silk.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724496, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Elite Cloud Silk.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724497, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Cloud Silk Pack and got the Super Cloud Silk.");
                    }
                    break;
                case 724181u:
                    client.Inventory.Remove(724181, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724498, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Jasper Pack and got the Normal Jasper.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724499, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Jasper Pack and got the Refined Jasper.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724350, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Jasper Pack and got the Unique Jasper.");
                    }
                    break;
                case 724182u:
                    client.Inventory.Remove(724182, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724499, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Jasper Pack and got the Refined Jasper.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724350, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Jasper Pack and got the Unique Jasper.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724351, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Jasper Pack and got the Elite Jasper.");
                    }
                    break;
                case 724183u:
                    client.Inventory.Remove(724183, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724350, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Jasper Pack and got the Unique Jasper.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724351, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Jasper Pack and got the Elite Jasper.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724352, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Jasper Pack and got the Super Jasper.");
                    }
                    break;
                case 724190u:
                    client.Inventory.Remove(724190, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724373, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Tassel Pack and got the Normal Tassel.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724374, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Tassel Pack and got the Refined Tassel.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724375, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Tassel Pack and got the Unique Tassel.");
                    }
                    break;
                case 724191u:
                    client.Inventory.Remove(724191, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724374, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Tassel Pack and got the Refined Tassel.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724375, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Tassel Pack and got the Unique Tassel.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724376, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Medium Tassel Pack and got the Elite Tassel.");
                    }
                    break;
                case 724192u:
                    client.Inventory.Remove(724192, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724375, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Tassel Pack and got the Unique Tassel.");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724376, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Tassel Pack and got the Elite Tassel.");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724377, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Superior Tassel Pack and got the Super Tassel.");
                    }
                    break;
                case 725055u:
                    client.Inventory.Remove(725055, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724415, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Critical Strike Pack and received a Bound Normal Refinery material (Ring)!");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724416, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Critical Strike Pack and received a Bound Refined Refinery material (Ring)!");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724417, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Critical Strike Pack and received a Bound Unique Refinery material (Ring)!");
                    }
                    break;
                case 725056u:
                    client.Inventory.Remove(725056, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724425, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Skill Critical Strike Pack and received a Bound Normal Refinery material (Bracelet)!");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724426, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Skill Critical Strike Pack and received a Bound Refined Refinery material (Bracelet)!");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724427, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Skill Critical Strike Pack and received a Bound Unique Refinery material (Bracelet)!");
                    }
                    break;
                case 725057u:
                    client.Inventory.Remove(725057, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724464, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Normal Refinery material (Ring)!");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724465, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Refined Refinery material (Ring)!");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724470, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Unique Refinery material (Ring)!");
                    }
                    break;
                case 725058u:
                    client.Inventory.Remove(725058, 1, stream);
                    if (Core.Rate(60, 100))
                    {
                        client.Inventory.Add(stream, 724363, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Normal Refinery material (Bracelet)!");
                    }
                    else if (Core.Rate(30, 40))
                    {
                        client.Inventory.Add(stream, 724364, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Refined Refinery material (Bracelet)!");
                    }
                    else
                    {
                        client.Inventory.Add(stream, 724365, 1, 0, 0, 0);
                        client.SendSysMesage("You opened the Primary Breakthrough Pack and received a Bound Unique Refinery material (Bracelet)!");
                    }
                    break;
                //case 721017u:
                //    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                //    {
                //        int value3;
                //        value3 = 5;
                //        client.Player.ConquerPoints += value3;
                        
                //    }
                //    break;
                //case 721019u:
                //    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                //    {
                //        int value5;
                //        value5 = 25;
                //        client.Player.ConquerPoints += value5;
                //    }
                //    break;
                //case 721018u:
                //    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                //    {
                //        int value4;
                //        value4 = 20;
                //        client.Player.ConquerPoints += value4;
                //    }
                //    break;
                //case 721026u:
                //    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                //    {
                //        int value;
                //        value = 50;
                //        client.Player.ConquerPoints += value;
                //    }
                //    break;
                //case 721027u:
                //    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                //    {
                //        int value2;
                //        value2 = 100;
                //        client.Player.ConquerPoints += value2;
                //    }
                //    break;
                case 725067u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        client.Inventory.AddItemWitchStack(725065, 0, 5, stream);
                    break;
                case 725068u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        client.Inventory.AddItemWitchStack(725066, 0, 5, stream);
                    break;
                case 728525u:
                    client.ActiveNpc = 4294967291;
                    NpcHandler.Steed1(client, stream, 0, "", 0);
                    break;
                case 728526u:
                    client.ActiveNpc = 4294967290;
                    NpcHandler.Steed3(client, stream, 0, "", 0);
                    break;
                case 728527u:
                    client.ActiveNpc = 4294967289;
                    NpcHandler.Steed6(client, stream, 0, "", 0);
                    break;
                case 720049u:
                    client.ActiveNpc = 4294967281;
                    NpcHandler.NobleSteedPack(client, stream, 0, "", 0);
                    break;
                case 723855u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 0, 150, byte.MaxValue);
                        client.SendSysMesage("You received (+1) MaroonSteed.");
                    }
                    break;
                case 720609u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 0, 150, byte.MaxValue);
                        client.SendSysMesage("You received (+3) MaroonSteed.");
                    }
                    break;
                case 723856u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, 150, byte.MaxValue, 0);
                        client.SendSysMesage("You received (+1) WhiteSteed.");
                        client.SendSysMesage("You received (+1) WhiteSteed.");
                    }
                    break;
                case 720610u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 150, byte.MaxValue, 0);
                        client.SendSysMesage("You received (+3) WhiteSteed.");
                    }
                    break;
                case 723859u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 1, false, byte.MaxValue, 0, 150);
                        client.SendSysMesage("You received (+1) BlackSteed.");
                    }
                    break;
                case 723860u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 0, 150, byte.MaxValue);
                        client.SendSysMesage("You received (+3) MaroonSteed.");
                    }
                    break;
                case 723861u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, 150, byte.MaxValue, 0);
                        client.SendSysMesage("You received (+3) WhiteSteed.");
                    }
                    break;
                case 720611u:
                case 723862u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 3, false, byte.MaxValue, 0, 150);
                        client.SendSysMesage("You received (+3) BlackSteed.");
                    }
                    break;
                case 723863u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 6, item.Bound > 0, 0, 150, byte.MaxValue);
                        client.SendSysMesage("You received (+6) MaroonSteed.");
                    }
                    break;
                case 723864u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, 150, byte.MaxValue, 0);
                        client.SendSysMesage("You received (+6) WhiteSteed.");
                    }
                    break;
                case 723865u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Inventory.AddSteed(stream, 300000, 1, 6, false, byte.MaxValue, 0, 150);
                        client.SendSysMesage("You received (+6) BlackSteed.");
                    }
                    break;
                case 780001:

                    if (DateTime.Now > client.Player.ExpireVip)
                    {
                        client.Player.ExpireVip = DateTime.Now;
                        client.Player.ExpireVip = client.Player.ExpireVip.AddDays(7);
                        client.Player.VipLevel = 6;
                    }
                    else
                    {
                        client.Player.ExpireVip = client.Player.ExpireVip.AddDays(7);
                        client.Player.VipLevel = 6;
                    }
                    client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                    client.Player.UpdateVip(stream);
                    client.Inventory.Remove(item.ITEM_ID, 1, stream);
                    client.CreateBoxDialog("You claimed 7 Days VIP added to your VIP[Days]");
                    
                    break;
                case 780002:

                    if (DateTime.Now > client.Player.ExpireVip)
                    {
                        client.Player.ExpireVip = DateTime.Now;
                        client.Player.ExpireVip = client.Player.ExpireVip.AddDays(15);
                        client.Player.VipLevel = 6;
                    }
                    else
                    {
                        client.Player.ExpireVip = client.Player.ExpireVip.AddDays(15);
                        client.Player.VipLevel = 6;
                    }
                    client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                    client.Player.UpdateVip(stream);
                    client.Inventory.Remove(item.ITEM_ID, 1, stream);
                    client.CreateBoxDialog("You claimed 15 Days VIP added to your VIP[Days]");
                    break;
                case 780000:

                        if (DateTime.Now > client.Player.ExpireVip)
                        {
                            client.Player.ExpireVip = DateTime.Now;
                            client.Player.ExpireVip = client.Player.ExpireVip.AddDays(30);
                            client.Player.VipLevel = 6;
                        }
                        else
                        {
                            client.Player.ExpireVip = client.Player.ExpireVip.AddDays(30);
                            client.Player.VipLevel = 6;
                        }
                        client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                        client.Player.UpdateVip(stream);
                        client.Inventory.Remove(item.ITEM_ID, 1, stream);
                        client.Player.AddHeavenBlessing(stream, 2592000);
                        client.CreateBoxDialog("You claimed 30 Days VIP added to your VIP[Days]");
                    break;
                case 780010:
                    if (DateTime.Now > client.Player.ExpireVip)
                    {
                        client.Player.ExpireVip = DateTime.Now;
                        client.Player.ExpireVip = client.Player.ExpireVip.AddHours(1);
                    }
                    else
                    {
                        client.Player.ExpireVip = client.Player.ExpireVip.AddHours(1);

                        client.Player.VipLevel = 6;

                        client.Player.SendUpdate(stream, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);

                        client.Player.UpdateVip(stream);
                    }
                    client.Inventory.Remove(item.ITEM_ID, 1, stream);
                    client.CreateBoxDialog("You claimed 1 Hours VIP added to your VIP[Days]");
                    break;
                case 780057:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (client.Player.VipLevel == 6)
                            client.CreateDialog(stream, "You aready have vip level 6", "I~see.");
                        else
                            client.Player.AddVIPLevel(6, DateTime.Now.AddHours(1.0), stream);
                    }
                    break;
                case 710212:
                    {
                        client.Inventory.Add(stream, 711504);
                        client.SendSysMesage("You received a (x1) SmallLotteryTicket using Lottery Ticket!");
                    break;
                    }
                case 721167:
                case 720135:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        var RndAccessory = Database.ItemType.RefiendGems;
                        var Position = Program.GetRandom.Next(0, RndAccessory.Count);
                        var ReceiveItem = RndAccessory.ToArray()[Position];
                        client.Inventory.Add(stream, ReceiveItem);
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(client.Player.Name + "You`ve received a Refiend " + Database.Server.ItemsBase[ReceiveItem].Name + " from Random SuperGem Box!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                      
                    }
                    break;

                case 723912u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.Player.QuizPoints += 50;
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "eidolon");
                    }
                    break;
                case 1060020u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map) 
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID) 
                        || client.InFIveOut || client.InTDM || client.InLastManStanding 
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                        client.Teleport(428, 378, 1002);
                    break;
                case 1060021:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(500, 650, 1000);
                    break;
                case 1060022u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(565, 562, 1020);
                    break;
                case 1060023u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(188, 264, 1011);
                    break;
                case 1060024u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(717, 571, 1015);
                    break;
                case 1060039u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(535, 558, 1075);
                    break;
                case 1060025u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(405, 655, 1002);
                    break;
                case 1060026u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(65, 252, 1002);
                    break;
                case 1060027u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(443, 372, 1002);
                    break;
                case 1060028u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(537, 767, 1011);
                    break;
                case 1060029u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(734, 448, 1011);
                    break;
                case 1060038u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(64, 426, 1011);
                    break;
                case 1060031u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(819, 602, 1020);
                    break;
                case 1060032u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(492, 726, 1020);
                    break;
                case 1060033u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(105, 396, 1020);
                    break;
                case 1060034u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(226, 201, 1000);
                    break;
                case 1060035u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(797, 547, 1000);
                    break;
                case 1060037u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    if (Program.BlockTeleportMap.Contains(client.Player.Map)
                        || UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID)
                        || client.InFIveOut || client.InTDM || client.InLastManStanding
                        || client.InPassTheBomb || client.InST
                        || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && client.Player.Map == 8250)
                        )
                    {
                        client.SendSysMesage("You Can't teleport from events");
                        return;
                    }
                    client.Teleport(476, 365, 1001);
                    break;
                case 721540u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            switch (ServerKernel.NextAsync(0, 8))
                            {
                                case 0:
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                                case 1:
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    client.Inventory.Add(stream, 1088001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                                case 2:
                                    client.Inventory.Add(stream, 1088000, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                                case 3:
                                    client.Inventory.Add(stream, 700031, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                                case 4:
                                    client.Inventory.Add(stream, 700001, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                                case 5:
                                    client.Inventory.Add(stream, 700011, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                                case 6:
                                    client.Inventory.Add(stream, 700062, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                                case 7:
                                    client.Inventory.Add(stream, 700022, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound >= 1);
                                    break;
                            }
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 720598u:
                    if (DateTime.Now < client.Player.LastDragonPill.AddHours(24.0))
                        client.CreateDialog(stream, "You`re already used DragonPill once today. You can use it next time on " + client.Player.LastDragonPill.AddHours(24.0).ToString(), "I~see");
                    else if (client.Player.Map == 2056)
                    {
                        if (Core.GetDistance(client.Player.X, client.Player.Y, 322, 331) <= 18)
                        {
                            if (!client.Map.ContainMobID(20060))
                            {
                                client.Player.LastDragonPill = DateTime.Now;
                                if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                                    Server.AddMapMonster(stream, client.Map, 20060, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                            }
                            else
                                client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");
                        }
                        else
                            client.CreateDialog(stream, "The Dragon Pill is used to summon the Terato Dragon. You can use it in the Frozen Grotto Floor 6 (322,331).", "I~see.");
                    }
                    else
                    {
                        client.CreateDialog(stream, "The Dragon Pill is used to summon the Terato Dragon. You can use it in the Frozen Grotto Floor 6 (322,331).", "I~see.");
                    }
                    break;
                case 720842u:
                    if (DateTime.Now < client.Player.LastSwordSoul.AddHours(24.0))
                        client.CreateDialog(stream, "You`re already used SwordSoul once today. You can use it next time on " + client.Player.LastSwordSoul.AddHours(24.0).ToString(), "I~see");
                    else if (client.Player.Level >= 99)
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (!client.Map.ContainMobID(6643, client.Player.UID))
                            {
                                if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                                    Server.AddMapMonster(stream, client.Map, 6643, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                            }
                            else
                                client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");
                        }
                        else
                            client.CreateDialog(stream, "You have to be in your own house to be able to display it", "I~see.");
                    }
                    else
                        client.CreateDialog(stream, "Sorry,~you~cannot~open~this~monster~before~your~Level~is~99.~Please~train~harder.", "I~see.");
                    break;
                case 3004259://snow
                    if (client.Player.Level >= 99)
                    {
                        if (client.MyHouse != null && client.Player.DynamicID == client.Player.UID)
                        {
                            if (!client.Map.ContainMobID(20070, client.Player.UID))
                            {
                                if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                                    Server.AddMapMonster(stream, client.Map, 20070, client.Player.X, client.Player.Y, 1, 1, 1, client.Player.DynamicID, true, MsgItemPacket.EffectMonsters.EarthquakeAndNight);
                            }
                            else
                                client.CreateDialog(stream, "Sorry,but the monster is already spawned..", "I~see.");
                        }
                        else
                            client.CreateDialog(stream, "This item can only be used in your house.", "Got~it.");
                    }
                    else
                        client.CreateDialog(stream, "Sorry,~you~cannot~open~this~monster~before~your~Level~is~99.~Please~train~harder.", "I~see.");
                    break;
                case 720030u:
                    if (client.Player.Stamina >= 100)
                    {
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Player.Stamina -= 100;
                        client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                    }
                    break;
                case 720031u:
                    if (client.Player.Stamina >= 100)
                    {
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-1love");
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Player.Stamina -= 100;
                        client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                    }
                    break;
                case 720032u:
                    if (client.Player.Stamina >= 100)
                    {
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-2love");
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Player.Stamina -= 100;
                        client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                    }
                    break;
                case 720021u:
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery npcServerQuery;
                        npcServerQuery = default(NpcServerQuery);
                        npcServerQuery.NpcType = Flags.NpcType.Talker;
                        npcServerQuery.Action = NpcServerReplay.Mode.Cursor;
                        NpcServerQuery Furniture5;
                        Furniture5 = npcServerQuery;
                        MsgGuildWar.GuildConductor conductor4;
                        conductor4 = MsgSchedules.GuildWar.GuildConductors[NpcID.TeleGuild1];
                        Furniture5.Mesh = (ushort)((conductor4.Npc.Mesh != 0) ? conductor4.Npc.Mesh : 1457);
                        Furniture5.ID = NpcID.TeleGuild1;
                        client.MoveNpcMesh = Furniture5.Mesh;
                        client.MoveNpcUID = (uint)Furniture5.ID;
                        client.Send(stream.NpcServerCreate(Furniture5));
                        break;
                    }
                case 720022u:
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery npcServerQuery;
                        npcServerQuery = default(NpcServerQuery);
                        npcServerQuery.NpcType = Flags.NpcType.Talker;
                        npcServerQuery.Action = NpcServerReplay.Mode.Cursor;
                        NpcServerQuery Furniture4;
                        Furniture4 = npcServerQuery;
                        MsgGuildWar.GuildConductor conductor3;
                        conductor3 = MsgSchedules.GuildWar.GuildConductors[NpcID.TeleGuild2];
                        Furniture4.Mesh = (ushort)((conductor3.Npc.Mesh != 0) ? conductor3.Npc.Mesh : 1467);
                        Furniture4.ID = NpcID.TeleGuild2;
                        client.MoveNpcMesh = Furniture4.Mesh;
                        client.MoveNpcUID = (uint)Furniture4.ID;
                        client.Send(stream.NpcServerCreate(Furniture4));
                        break;
                    }
                case 720023u:
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery npcServerQuery;
                        npcServerQuery = default(NpcServerQuery);
                        npcServerQuery.NpcType = Flags.NpcType.Talker;
                        npcServerQuery.Action = NpcServerReplay.Mode.Cursor;
                        NpcServerQuery Furniture3;
                        Furniture3 = npcServerQuery;
                        MsgGuildWar.GuildConductor conductor2;
                        conductor2 = MsgSchedules.GuildWar.GuildConductors[NpcID.TeleGuild3];
                        Furniture3.Mesh = (ushort)((conductor2.Npc.Mesh != 0) ? conductor2.Npc.Mesh : 1477);
                        Furniture3.ID = NpcID.TeleGuild3;
                        client.MoveNpcMesh = Furniture3.Mesh;
                        client.MoveNpcUID = (uint)Furniture3.ID;
                        client.Send(stream.NpcServerCreate(Furniture3));
                        break;
                    }
                case 720024u:
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery npcServerQuery;
                        npcServerQuery = default(NpcServerQuery);
                        npcServerQuery.NpcType = Flags.NpcType.Talker;
                        npcServerQuery.Action = NpcServerReplay.Mode.Cursor;
                        NpcServerQuery Furniture2;
                        Furniture2 = npcServerQuery;
                        MsgGuildWar.GuildConductor conductor;
                        conductor = MsgSchedules.GuildWar.GuildConductors[NpcID.TeleGuild4];
                        Furniture2.Mesh = (ushort)((conductor.Npc.Mesh != 0) ? conductor.Npc.Mesh : 1487);
                        Furniture2.ID = NpcID.TeleGuild4;
                        client.MoveNpcMesh = Furniture2.Mesh;
                        client.MoveNpcUID = (uint)Furniture2.ID;
                        client.Send(stream.NpcServerCreate(Furniture2));
                        break;
                    }
                
                case 720020u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.UseItem = item.ITEM_ID;
                        NpcServerQuery npcServerQuery;
                        npcServerQuery = default(NpcServerQuery);
                        npcServerQuery.ID = NpcID.Pharmacist;
                        npcServerQuery.Mesh = 1130;
                        npcServerQuery.Action = NpcServerReplay.Mode.Cursor;
                        npcServerQuery.NpcType = (Flags.NpcType)9;
                        NpcServerQuery npc;
                        npc = npcServerQuery;
                        client.Send(stream.NpcServerCreate(npc));
                    }
                    break;
                case 727999u:
                    if (Program.BlockTeleportMap.Contains(client.Player.Map) || client.Player.Map == 1038 || client.InFIveOut || client.InTDM || client.InLastManStanding || client.InPassTheBomb || client.InST)
                        client.SendSysMesage("You can`t use it in " + client.Map.Name + " ");
                    else if (client.Player.MyGuild == null || client.Player.GuildRank != Flags.GuildMemberRank.GuildLeader || client.Player.GuildRank != Flags.GuildMemberRank.LeaderSpouse)
                    {
                        client.SendSysMesage("You need to be GuildLeader to use this!");
                    }
                    else if (DateTime.Now < client.Player.MyGuild.SummonGuild.AddMinutes(5.0))
                    {
                        client.SendSysMesage("You need to wait 5 minutes before summoning again.");
                    }
                    else
                    {
                        if (!client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            break;
                        client.Player.MyGuild.SummonGuild = DateTime.Now;
                        ushort X;
                        X = client.Player.X;
                        ushort Y;
                        Y = client.Player.Y;
                        uint Map;
                        Map = client.Player.Map;
                        uint Dynamic;
                        Dynamic = client.Player.DynamicID;
                        foreach (GameClient member in Server.GamePoll.Values.Where((GameClient e) => e.Player.GuildID != 0 && e.Player.GuildID == client.Player.GuildID && e.Player.UID != client.Player.UID))
                        {
                            member.Player.MessageBox("Your guild leader has summoned you to " + client.Map.Name + "! Would you like to go?", delegate (GameClient user)
                            {
                                user.Teleport((ushort)(X + Core.Random.Next(0, 5)), (ushort)(Y + Core.Random.Next(0, 5)), Map, Dynamic);
                            }, null, 60);
                        }
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " guild leader of " + client.Player.MyGuild.GuildName + " has summoned his members to " + client.Map.Name, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                    }
                    break;
                case 723711u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 1088002, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 722383u:
                    if (client.Inventory.HaveSpace(20))
                    {
                        if (client.Inventory.Remove(722383, 1, stream))
                            client.Inventory.Add(stream, 722384, 20, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {20} empty spaces.");
                    break;
                #region MegaMetsPack
                case 720547://MeteorScroll
                    {
                        if (client.Inventory.HaveSpace(10))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            if (item.Bound == 1)
                            {
                                client.Inventory.Add(stream, 720027, 10, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                                client.Inventory.Add(stream, 720027, 10, 0);
                        }
                        else
                        {
#if Arabic
                                            client.SendSysMesage("Please make 10 more spaces in your inventory.");
#else
                            client.SendSysMesage("Please make 10 more spaces in your inventory.");
#endif

                        }
                        break;
                    }
                #endregion
                #region MegaDBPack
                case 720546://MegaDBPack
                    {
                        if (client.Inventory.HaveSpace(10))
                        {
                            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                            if (item.Bound == 1)
                            {
                                client.Inventory.Add(stream, 720028, 10, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                            }
                            else
                                client.Inventory.Add(stream, 720028, 10, 0);
                        }
                        else
                        {
#if Arabic
                                            client.SendSysMesage("Please make 10 more spaces in your inventory.");
#else
                            client.SendSysMesage("Please make 10 more spaces in your inventory.");
#endif

                        }
                        break;
                    }
                #endregion
                case 720027u:
                    if (client.Inventory.HaveSpace(9))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            if (item.Bound == 1)
                                client.Inventory.Add(stream, 1088001, 10, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            else
                                client.Inventory.Add(stream, 1088001, 10, 0, 0, 0);
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {10} empty spaces.");
                    break;
                case 720028u:
                    if (client.Inventory.HaveSpace(9))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 1088000, 10, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {9} empty spaces.");
                    break;
                case 723726u:
                    if (DateTime.Now > client.Player.MedicineStamp.AddMilliseconds(890.0) && !client.Player.ContainFlag(MsgUpdate.Flags.PoisonStar) && client.Player.Map != 1005 && !UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID))
                    {
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                        client.Player.Mana = (ushort)client.Status.MaxMana;
                        client.Player.MedicineStamp = DateTime.Now;
                    }
                    break;
                case 723725u:
                    if (client.Inventory.HaveSpace(9))
                    {
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 723726, 10, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {9} empty spaces.");
                    break;
                //case 723712u:
                //    if (client.Inventory.HaveSpace(4))
                //    {
                //        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                //        {
                //            if (item.Bound == 1)
                //                client.Inventory.Add(stream, 730001, 5, 1, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                //            else
                //                client.Inventory.Add(stream, 730001, 5, 1, 0, 0);
                //        }
                //    }
                //    else
                //        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                //    break;
                case 727347u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            return;
                            if (item.Bound == 1)
                                client.Inventory.Add(stream, 730002, 5, 1, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            else
                                client.Inventory.Add(stream, 730002, 5, 1, 0, 0);
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 723832u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            if (item.Bound == 1)
                                client.Inventory.Add(stream, 730003, 5, 1, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            else
                                client.Inventory.Add(stream, 730003, 5, 1, 0, 0);
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 727060u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700121, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727061u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700101, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727062u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700041, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727063u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700031, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727064u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700021, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727065u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700011, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727066u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700001, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727067u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700051, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727068u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700061, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727069u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                            client.Inventory.Add(stream, 700071, 5, 0, 0, 0);
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 727464u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            if (item.Bound == 1)
                                client.Inventory.Add(stream, 723700, 5, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, true);
                            else
                                client.Inventory.Add(stream, 723700, 5, 0, 0, 0);
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 723700u:
                    if (client.Player.Level >= ServerKernel.MAX_UPLEVEL)
                        break;
                    if (client.Player.ExpBallUsed < 10)
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            Player player;
                            player = client.Player;
                            player.ExpBallUsed++;
                            client.GainExpBall(600.0, false, Flags.ExperienceEffect.angelwing);
                        }
                    }
                    else
                        client.SendSysMesage("You can use only ten exp balls a day. Try tomorrow.", MsgMessage.ChatMode.TopLeft);
                    break;
                case 722136u:
                case 723834u:
                    if (client.Player.Level < ServerKernel.MAX_UPLEVEL && client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        if (item.ITEM_ID == 722136)
                            client.GainExpBall(1200.0, false, Flags.ExperienceEffect.angelwing);
                        else
                            client.GainExpBall(600.0, false, Flags.ExperienceEffect.angelwing);
                    }
                    break;
                case 722057u:
                case 723744u:
                    if (client.Player.Level < ServerKernel.MAX_UPLEVEL)
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            DBLevExp nextlevel;
                            nextlevel = Server.LevelInfo[DBLevExp.Sort.User][(byte)client.Player.Level];
                            ulong exp;
                            exp = nextlevel.Experience * 10 / 100uL;
                            Player player;
                            player = client.Player;
                            player.Experience += exp + 1;
                            client.Player.SendUpdate(stream, (long)client.Player.Experience, MsgUpdate.DataType.Experience);
                            if (client.Player.Experience >= nextlevel.Experience)
                            {
                                ushort level;
                                level = (ushort)(client.Player.Level + 1);
                                client.UpdateLevel(stream, level, true);
                            }
                        }
                    }
                    else
                        client.SendSysMesage("As your character has reached the highest level, you cannot use this.");
                    break;
                case 720692u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.GainExpBall(3000.0, true, Flags.ExperienceEffect.angelwing);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Wind Pill and received the EXP worth 5 EXP Balls!");
                    }
                    break;
                case 720686u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.GainExpBall(300.0);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.CreateBoxDialog("You used the Mystery Pill and received EXP worth 3 and 1/3 EXP Balls!");
                    }
                    break;
                case 720828u:
                    if (client.Map.BaseID == 6000 || client.Map.BaseID == 6001 || client.Map.BaseID == 1126 || client.Map.BaseID == 1801 || client.Map.BaseID == 8883 || (client.Map.BaseID == 1005 && client.Player.Map != 1005) || client.Map.BaseID == 700)
                        client.SendSysMesage("You can't teleport in this map.", MsgMessage.ChatMode.TopLeft);
                    else
                        item.SendAgate(client);
                    break;
                case 720128u:
                case 723727u:
                    if (client.Player.Map >= 6000 && client.Player.Map <= 6004)
                        client.SendSysMesage("You can`t use this in jail.");
                    else if (client.Player.PKPoints > 30)
                    {
                        client.Player.PKPoints -= 30;
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.SendSysMesage("You've removed 30 Pk Points.");
                    }
                    else
                    {
                        client.SendSysMesage("You can not use this item for now since your Pk points are less than 30.");
                    }
                    break;
                case 723094u:
                    if (client.Player.SubClass != null)
                    {
                        client.Player.SubClass.AddStudyPoints(client, ushort.MaxValue, stream);
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                    }
                    break;
                case 727833u:
                    if (client.Inventory.HaveSpace(1))
                    {
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(723342, 0, 6, stream);
                        client.CreateBoxDialog("You received 6~Modesty~Books!");
                    }
                    else
                        client.SendSysMesage("Please make 1 more spaces in your inventory.");
                    break;
                case 727834u:
                    if (client.Inventory.HaveSpace(1))
                    {
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Inventory.AddItemWitchStack(723342, 0, 4, stream);
                        client.CreateBoxDialog("You received 4~Modesty~Books!");
                    }
                    else
                        client.SendSysMesage("Please make 1 more spaces in your inventory.");
                    break;
                case 720774u:
                case 720775u:
                    if (client.Player.SubClass != null)
                    {
                        client.Player.SubClass.AddStudyPoints(client, 50, stream);
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                    }
                    break;
                case 723340u:
                    if (client.Player.SubClass != null)
                    {
                        client.Player.SubClass.AddStudyPoints(client, 5, stream);
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                    }
                    break;
                case 723341u:
                    if (client.Player.SubClass != null)
                    {
                        client.Player.SubClass.AddStudyPoints(client, 20, stream);
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                    }
                    break;
                case 723342u:
                    if (client.Player.SubClass != null)
                    {
                        client.Player.SubClass.AddStudyPoints(client, 500, stream);
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                    }
                    break;
                case 720394u:
                    client.SendSysMesage("From now on, you can get 4x experience for the next two hours.");
                    client.Player.RateExp = 4;
                    client.Player.DExpTime = 7200;
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.CreateExtraExpPacket(stream);
                    break;
                case 720393u:
                    client.SendSysMesage("From now on, you can get triple experience for the next two hours.");
                    client.Player.RateExp = 3;
                    client.Player.DExpTime = 7200;
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.CreateExtraExpPacket(stream);
                    break;
                case 723017u:
                    client.Player.RateExp = 2;
                    client.Player.DExpTime = 3600;
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.CreateExtraExpPacket(stream);
                    client.SendSysMesage("You get double exp. time of one hour.");
                    break;
                case 720985u:
                case 723724u:
                    client.Player.TransformInfo = new ClientTransform(client.Player);
                    client.Player.TransformInfo.CreateTransform(stream, (uint)client.Player.HitPoints, Tranformation.GetRandomTransform(), 60, 1360, 0);
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    break;
                case 720837u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.TransformInfo = new ClientTransform(client.Player);
                    client.Player.TransformInfo.CreateTransform(stream, 817, 201, 60, 0, 0);
                    break;
                case 1200000u:
                    client.SendSysMesage("ONLY VIP 6 USERS CAN USE THIS #43 !");
                    return;
                    client.Player.AddHeavenBlessing(stream, 259200);
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    break;
                case 1200001u:
                    client.SendSysMesage("ONLY VIP 6 USERS CAN USE THIS #43 !");
                    return;
                    client.Player.AddHeavenBlessing(stream, 604800);
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    break;
                case 1200002u:
                    client.SendSysMesage("ONLY VIP 6 USERS CAN USE THIS #43 !");
                    return;
                    client.Player.AddHeavenBlessing(stream, 2592000);
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    break;
                case 721625u:
                    if (client.Player.Level > ServerKernel.MAX_UPLEVEL - 30)
                    {
                        client.SendSysMesage("sorry, the max level is 110, for active this.");
                        break;
                    }
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.ActiveAttackPotion(30);
                    break;
                case 721626u:
                    if (client.Player.Level > ServerKernel.MAX_UPLEVEL - 30)
                    {
                        client.SendSysMesage("sorry, the max level is 110, for active this.");
                        break;
                    }
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.Player.ActiveDefensePotion(30);
                    break;
                case 720894u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 723341, 5, 1, 0, 0);
                            client.CreateBoxDialog("You have received 5 Endurance Books.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 728920u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 723341, 4, 1, 0, 0);
                            client.CreateBoxDialog("You have received 4 Endurance Books.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {4} empty spaces.");
                    break;
                case 720717://SuperElitePKChampionPack
                    if (client.Inventory.HaveSpace(11))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 3, 0, 0, 0);//5xUltimatePack
                            //client.Inventory.Add(stream, 722057, 5, 0, 0, 0);//4xPowerExpball
                            //client.Inventory.Add(stream, 730005, 1, 0, 0, 0);//+5 stone
                            client.Inventory.Add(stream, 720547, 1, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 5, 0, 0, 0);//2xMegaDBPack
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperElitePKChampion >> MegaMetsPack | 5xDragonBall.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {11} empty spaces.");
                    break;
                case 720721://SuperElitePK2ndPlacePack
                    if (client.Inventory.HaveSpace(12))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 2, 0, 0, 0);//2xUltimatePack
                            //client.Inventory.Add(stream, 722057, 3, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730004, 1, 0, 0, 0);//+4 stone
                            client.Inventory.Add(stream, 720547, 3, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 3, 0, 0, 0);//DBScroll
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperElitePK2ndPlace >>  3xMeteorScroll | 3xDragonBall.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {12} empty spaces.");
                    break;
                case 720725://SuperElitePK3rdPlacePack
                    if (client.Inventory.HaveSpace(6))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 1, 0, 0, 0);//2xUltimatePack
                            //client.Inventory.Add(stream, 722057, 1, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730003, 1, 0, 0, 0);//+4 stone
                            client.Inventory.Add(stream, 720027, 3, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 2, 0, 0, 0);//DBScroll
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperElitePK3rdPlace >>  3xMeteorScroll | 2xDragonBall.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {6} empty spaces.");
                    break;
                case 720729://SuperElitePKTop8Pack
                    if (client.Inventory.HaveSpace(9))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 1, 0, 0, 0);//2xUltimatePack
                            //client.Inventory.Add(stream, 722057, 1, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730002, 1, 0, 0, 0);//+2 stone
                            client.Inventory.Add(stream, 720027, 2, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 1, 0, 0, 0);//DB
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperElitePKTop8 >>  2xMeteorScroll | 1xDragonBall .", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {9} empty spaces.");
                    break;
                case 720797://SuperTeamPKChampionPack
                    if (client.Inventory.HaveSpace(11))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 3, 0, 0, 0);//5xUltimatePack
                            //client.Inventory.Add(stream, 722057, 5, 0, 0, 0);//4xPowerExpball
                            //client.Inventory.Add(stream, 730005, 1, 0, 0, 0);//+5 stone
                            client.Inventory.Add(stream, 720547, 1, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 5, 0, 0, 0);//DBScroll
                            client.Player.Money += 10000000;
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperTeamPKChampion >>  MegaMetsPack | 5xDragonBall | 10,000,000 GOLD .", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {11} empty spaces.");
                    break;
                case 720801://SuperTeamPK2ndPlacePack
                    if (client.Inventory.HaveSpace(12))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 2, 0, 0, 0);//2xUltimatePack
                            //client.Inventory.Add(stream, 722057, 3, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730004, 1, 0, 0, 0);//+4 stone
                            client.Inventory.Add(stream, 720027, 5, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 3, 0, 0, 0);//DBScroll
                            client.Player.Money += 8000000;
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperElitePK2ndPlace >> 5xMeteorScroll | 3xDragonBall | 8,000,000 GOLD .", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {12} empty spaces.");
                    break;
                case 720805://SuperTeamPK3rdPlacePack
                    if (client.Inventory.HaveSpace(6))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 1, 0, 0, 0);//2xUltimatePack
                            //client.Inventory.Add(stream, 722057, 1, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730003, 1, 0, 0, 0);//+4 stone
                            client.Inventory.Add(stream, 720027, 5, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 2, 0, 0, 0);//DBScroll
                            client.Player.Money += 5000000;
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperTeamPK3rdPlace >> 3xMeteorScroll | 2xDragonBall | 5,000,000 GOLD", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {6} empty spaces.");
                    break;
                case 720809://SuperTeamPKTop8Pack
                    if (client.Inventory.HaveSpace(9))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            //client.Inventory.Add(stream, 721169, 1, 0, 0, 0);//2xUltimatePack
                            //client.Inventory.Add(stream, 722057, 1, 0, 0, 0);//3xPowerExpball
                            //client.Inventory.Add(stream, 730002, 1, 0, 0, 0);//+2 stone
                            client.Inventory.Add(stream, 720027, 3, 0, 0, 0);//MegaMetsPack
                            client.Inventory.Add(stream, 1088000, 1, 0, 0, 0);//DB
                            client.Player.Money += 3000000;
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " was rewarded a SuperTeamPKTop8 >> 3xMeteorScroll | 1xDragonBall | 3,000,000 GOLD", MsgMessage.MsgColor.red, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {9} empty spaces.");
                    break;
                case 721799u:
                    if (Core.GetDistance(55, 109, client.Player.X, client.Player.Y) <= 10 && client.Player.Map == 1792)
                    {
                        client.Inventory.Remove(721799, 1, stream);
                        Npc np;
                        np = TheChosenProject.Game.MsgNpc.Npc.Create();
                        np.UID = 4584;
                        np.NpcType = Flags.NpcType.Talker;
                        np.Mesh = 3920;
                        np.Map = 1792;
                        np.X = 55;
                        np.Y = 109;
                        Server.ServerMaps[np.Map].AddNpc(np);
                        client.Player.View.Role();
                        client.Player.AddMapEffect(stream, 55, 109, "eddy");
                    }
                    else
                        client.SendSysMesage("Please burn the Incense in Swan Lake (55,109).");
                    break;
                case 720706u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 561138, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 131068, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 117068, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received an~Elite~L70~Iron~Wand,~an~Elite~L70~Light~Armor,~an~Elite~L67~Heart~of~Ocean.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720707u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 420138, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 130068, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 118068, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received an~Elite~L70~Shark~Sword,~an~Elite~L70~Rage~Armor,~an~Elite~L67~War~Coronet.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720708u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 500128, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 133048, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 113048, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received an~Elite~L70~Goose~Bow,~an~Elite~L67~Ape~Coat,~an~Elite~L72~Ape~Hat.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720709u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 601138, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 135068, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 112068, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received an~Elite~L70~Rain~Katana,~an~Elite~L70~TigerVest,~an~Elite~L67~BloodVeil.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720710u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 421138, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 134068, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 114068, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received an~Elite~L70~Great~Backsword,~an~Elite~L70~Crane~Vestment,~an~Elite~L67~Shark~Cap~.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720711u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 120128, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 150138, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 160138, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received an~Elite~L67~Platina~Necklace,~an~Elite~L70~Pearl~Ring,~an~Elite~L70~Snakeskin~Boots");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720712u:
                    if (client.Inventory.HaveSpace(4))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 152128, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 121128, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 160138, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received an~Elite~L65~Bone~Bracelet,~an~Elite~L67~Ambergris~Bag,~an~Elite~L70~Snakeskin~Boots~");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720704u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 150179, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 120159, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 160179, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received a~Super~L90~Crystal~Ring,~a~Super~L82~Basalt~Necklace,~a~Super~L90~Leopard~Boots.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720705u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 152169, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 121159, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 160179, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received a~Super~L85~Jade~Bracelet,~a~Super~L82~Jade~Bag,~a~Super~L90~Leopard~Boots.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720699u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 561179, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 131079, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 111079, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received a~Super~L90~Exorcising~Wand,~a~Super~L87~Lion~Armor,~a~Super~L82~Dragon~Helmet~.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720700u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 420179, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 130079, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 118079, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received a~Super~L90~Long~Sword,~a~Super~L87~Sacred~Armor,~a~Super~L82~Hercules~Coronet.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720701u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 500169, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 133069, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 113059, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received a~Super~L90~Star~Bow,~a~Super~L87~Shark~Coat,~a~Super~L82~Marten~Hat.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 720702u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 601179, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 135079, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 112079, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received a~Super~L90~Ise~Katana,~a~Super~L87~Bear~Vest,~a~Super~L82~Stealth~Veil~.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 721261://bomb
                    {
                        if (client.Player.Map == 1038)
                        {
                            MsgUpdate upd;
                            upd = new MsgUpdate(stream, MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].UID);
                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate].HitPoints > 3000000)
                            {
                                if (client.Player.View.Contains(MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.LeftGate]))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    MsgTournaments.MsgSchedules.GuildWar.Bomb(stream, client, Role.SobNpc.StaticMesh.LeftGate);
                                    break;
                                }
                            }
                            else
                            {
                                client.SendSysMesage("Bomb can't use on gate anymore");
                            }

                            if (MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate].HitPoints > 3000000)
                            {
                                if (client.Player.View.Contains(MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.RightGate]))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                                    MsgTournaments.MsgSchedules.GuildWar.Bomb(stream, client, Role.SobNpc.StaticMesh.RightGate);
                                    break;
                                }
                            }
                            else
                            {
                                client.SendSysMesage("Bomb can't use on gate anymore");
                            }
                        }
                        else
                        {
#if Arabic
                                        client.SendSysMesage("Use this on gate in Guild Arena");
#else
                            client.SendSysMesage("Use this on gate in Guild Arena");
#endif

                        }
                        break;
                    }
                case 720703u:
                    if (client.Inventory.HaveSpace(5))
                    {
                        if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                        {
                            client.Inventory.Add(stream, 421179, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 134079, 0, 0, 0, 0);
                            client.Inventory.Add(stream, 114079, 0, 0, 0, 0);
                            client.CreateBoxDialog("You received a~Super~L90~War~Backsword,~a~Super~L87~Full~Frock,~a~Super~L82~Dragon~Cap.");
                        }
                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {5} empty spaces.");
                    break;
                case 723467u:
                    client.ActiveNpc = 4294967269;
                    NpcHandler.GoldPrizeToken(client, stream, 0, "", 0);
                    break;
                case 721169u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        int rnd;
                        rnd = Core.Random.Next(0, 2);
                        List<uint> list;
                        list = new List<uint>();
                        list.Add(360421);
                        list.Add(360422);
                        list.Add(360423);
                        list.Add(360172);
                        list.Add(360404);
                        list.Add(360338);
                        list.Add(360452);
                        list.Add(360406);
                        list.Add(360405);
                        list.Add(360339);
                        list.Add(360454);
                        list.Add(360455);
                        list.Add(360492);
                        list.Add(360449);
                        list.Add(360450);
                        list.Add(360340);
                        list.Add(360415);
                        list.Add(360414);
                        list.Add(360077);
                        list.Add(360054);
                        list.Add(360164);
                        list.Add(360165);
                        list.Add(360114);
                        list.Add(360115);
                        //list.Add(360116);
                        list.Add(360117);
                        //list.Add(360118);
                        list.Add(185205);
                        list.Add(185215);
                        list.Add(185225);
                        list.Add(185235);
                        list.Add(185245);
                        list.Add(196565);
                        list.Add(196575);
                        list.Add(196585);
                        list.Add(196595);
                        list.Add(196605);
                        list.Add(196615);
                        list.Add(196625);
                        list.Add(196635);
                        list.Add(196645);
                        List<uint> listGarments2;
                        listGarments2 = list;
                        if (rnd == 0)
                        {
                            client.Inventory.Add(stream, listGarments2[Core.Random.Next(0, listGarments2.Count)], 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound == 1);
                            break;
                        }
                        uint[] RareAccessorys;
                        RareAccessorys = ItemType.RareAccessories.ToArray();
                        int position2;
                        position2 = ServerKernel.NextAsync(0, RareAccessorys.Length);
                        client.Inventory.Add(stream, RareAccessorys[position2], 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, item.Bound == 1);
                    }
                    break;
                case 720671u:
                    client.ActiveNpc = 4294967263;
                    NpcHandler.HeavenDemonBox(client, stream, 0, "", 0);
                    break;
                case 720672u:
                    client.ActiveNpc = 4294967262;
                    NpcHandler.ChaosDemonBox(client, stream, 0, "", 0);
                    break;
                case 722077u:
                    client.ActiveNpc = 4294967261;
                    NpcHandler.SacredDemonBox(client, stream, 0, "", 0);
                    break;
                case 720674u:
                    client.ActiveNpc = 4294967260;
                    NpcHandler.AuroraDemonBox(client, stream, 0, "", 0);
                    break;
                case 720650u:
                    client.ActiveNpc = 4294967259;
                    NpcHandler.DemonBox(client, stream, 0, "", 0);
                    break;
                //case 720679: //69000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 69000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 69000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Frost CP Pack and received 69000 CPs!");
                //        break;
                //    }
                //case 720678: //13500 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 13500 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 13500;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Life CP Pack and received 13500 CPs!");
                //        break;
                //    }
                //case 720677: //1000 BloodCPPack
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 1000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 1000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Blood CP Pack and received 1000 CPs!");
                //        break;
                //    }
                //case 720676: //500 SoulCPPack
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 500 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 500;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Soul CP Pack and received 500 CPs!");
                //        break;
                //    }
                //case 720675: //250 GhostCPPack
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 250 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 250;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Ghost CP Pack and received 250 CPs!");
                //        break;
                //    }
                case 720680u:
                    if (client.Inventory.Remove(item.ITEM_ID, 1, stream))
                    {
                        client.GainExpBall();
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                        client.SendSysMesage("You used the Heaven Pill and received EXP worth 2 and 1/2 EXP Balls!");
                    }
                    break;
                //case 720685: //138000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 138000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 138000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Nimbus CP Pack and received 138000 CPs!");
                //        break;
                //    }
                //case 720684: //27000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 27000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 27000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Butterfly CP Pack and received 27000 CPs!");
                //        break;
                //    }
                //case 720683: //2000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 2000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 2000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Heart CP Pack and received 2000 CPs!");
                //        break;
                //    }
                //case 720682: //1000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 1000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 1000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Flower CP Pack and received 1000 CPs!");
                //        break;
                //    }
                //case 720681: //69000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 500 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 500;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Deity CP Pack and received 500 CPs!");
                //        break;
                //    }
                //case 720687: //1000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 1000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 1000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Cloud CP Pack and received 1000 CPs!");
                //        break;
                //    }
                //case 720688: //2000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 2000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 2000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Jewel CP Pack and received 2000 CPs!");
                //        break;
                //    }
                //case 720689: //4000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 4000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 4000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Shadow CP Pack and received 4000 CPs!");
                //        break;
                //    }
                //case 720690: //54000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 54000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 54000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Rainbow CP Pack and received 54000 CPs!");
                //        break;
                //    }
                //case 720691: //276000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 276000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 276000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Kylin CP Pack and received 276000 CPs!");
                //        break;
                //    }
                //case 720693: //2500 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 2500 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 2500;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Fog CP Pack and received 2500 CPs!");
                //        break;
                //    }
                //case 720694: //5000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 5000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 5000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Moon CP Pack and received 5000 CPs!");
                //        break;
                //    }
                //case 723411: //5000 
                //    {
                //        string logs = "[RoyalPass]" + client.Player.Name + " get 20,000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 20000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Moon CP Pack and received 20,000 CPs!");
                //        break;
                //    }
                //case 720695: //10000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 10000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 10000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Earth CP Pack and received 10000 CPs!");
                //        break;
                //    }
                //case 720696: //135000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 135000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 135000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Zephyr CP Pack and received 135000 CPs!");
                //        break;
                //    }
                case 720145:
                    client.ActiveNpc = (uint)Game.MsgNpc.NpcID.OPBox;
                    Game.MsgNpc.NpcHandler.OPBox(client, stream, 0, "", 0);
                    break;
                //case 720697: //690000 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 690000 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 690000;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Pilgrim CP Pack and received 690000 CPs!");
                //        break;
                //    }
                //case 720698u:
                    client.Inventory.Update(item, AddMode.REMOVE, stream);
                    client.GainExpBall(3200.0);
                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                    client.CreateBoxDialog("You used the Moon Pill and received the EXP worth 8 and 1/3 EXP Balls!");
                    break;
                //case 720654: //1380 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 1380 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 1380;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Joy CP Pack and received 1380 CPs from Demon Box.");
                //        break;
                //    }
                //case 720653: //270 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 270 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 270;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Dream CP Pack and received 270 CPs from Demon Box.");
                //        break;
                //    }
                //case 720655: //20 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 20 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 20;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Mammon CP Pack and received 20 CPs from Demon Box.");
                //        break;
                //    }
                //case 720656: //10 
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 10 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 10;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Mascot CP Pack and received 10 CPs from Demon Box.");
                //        break;
                //    }
                //case 720657: //5
                //    {
                //        string logs = "[DemonBox]" + client.Player.Name + " get 5 he have " + client.Player.ConquerPoints + "";
                //        Database.ServerDatabase.LoginQueue.Enqueue(logs);
                //        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //        client.Player.ConquerPoints += 5;
                //        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "eidolon");
                //        client.CreateBoxDialog("You used the Hope CP Pack and received 5 CPs from Demon Box.");
                //        break;
                //    }
                #region AmazingVIPPack
                //case 3001183:
                //    {
                //        if (client.Inventory.HaveSpace(1))
                //        {
                //            client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                //            client.Player.ConquerPoints += 10000000;
                //            client.Inventory.Add(stream, 3303745, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);//50 dps
                //            client.SendSysMesage("Congratulations!~You~received~a~10kK~CPs~and~[50]DonatePoints!");
                //            //Role.Core.SendGlobalMessage(stream, "Congratulations!~" + client.Player.Name + "~received~a~10kk~CPs~and~[50]Donate Points~for~reach~100~USD~in~Donate!", MsgMessage.ChatMode.System);
                //            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession");
                //        }
                //        else
                //        {
                //            client.SendSysMesage("Your~inventory~is~full.");
                //        }
                //        break;
                //    }
                #endregion
                case 720124:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e300");
                        if (MyMath.ChanceSuccess(2))
                        {
                            client.Inventory.Add(stream, 1088000);
                            client.SendSysMesage("Congratulations you received a DragonBall!");
                            Role.Core.SendGlobalMessage(stream, "Lucky Player " + client.Player.Name + " has opened a ColoredStone and received a DragonBall!", MsgMessage.ChatMode.System);

                        }
                        else if (MyMath.ChanceSuccess(30))
                        {
                            List<uint> list;
                            list = new List<uint>();
                            list.Add(191405);
                            list.Add(191305);
                            list.Add(183325);
                            list.Add(183315);
                            list.Add(183375);
                            list.Add(183305);
                            list.Add(182335);
                            list.Add(181355);
                            list.Add(181395);
                            list.Add(182355);
                            list.Add(182375);
                            list.Add(182365);
                            list.Add(182345);
                            list.Add(182385);
                            list.Add(182325);
                            list.Add(181385);
                            list.Add(181375);
                            list.Add(182305);
                            list.Add(181365);
                            list.Add(181345);
                            list.Add(181335);
                            list.Add(182315);
                            list.Add(181305);
                            list.Add(181405);
                            list.Add(181505);
                            list.Add(181605);
                            list.Add(181705);
                            list.Add(181805);
                            list.Add(181905);
                            list.Add(181315);
                            list.Add(181415);
                            list.Add(181515);
                            list.Add(181615);
                            list.Add(181715);
                            list.Add(181815);
                            list.Add(181915);
                            list.Add(181325);
                            list.Add(181425);
                            list.Add(181525);
                            list.Add(181625);
                            list.Add(181725);
                            list.Add(181825);
                            list.Add(181925);
                            list.Add(722057);
                            List<uint> listGarments;
                            listGarments = list;
                            uint reward;
                            reward = listGarments[Core.Random.Next(0, listGarments.Count)];
                            client.Inventory.Add(stream, reward, 1, 0, 0, 0);

                                client.SendSysMesage($"Congratulations you received a {DBItem.Name}!");
                                Role.Core.SendGlobalMessage(stream, "Lucky Player " + client.Player.Name + $" has opened a ColoredStone and received a Garment {DBItem.Name}!", MsgMessage.ChatMode.System);

                            
                        }
                        else if (MyMath.ChanceSuccess(40))
                        {
                            client.Inventory.Add(stream, 720547);
                            client.SendSysMesage("Congratulations you received a MegaMetsPack!");
                        }
                        else if (MyMath.ChanceSuccess(50))
                        {
                            client.Player.Money += 2000000;
                            client.SendSysMesage("Congratulations! You have received 2,000,000 silvers!");
                        }
                        else
                        {
                            client.Player.Money += 1000000;
                            client.GainExpBall(600 * 2, true);
                            client.SendSysMesage($"You have received 1,000,000 silvers!");
                        }
                        break;
                    }
                case 720121:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        int x = Program.Rand.Next(1, 5);
                        switch (x)
                        {
                            case 1:
                                {
                                    client.Inventory.Add(stream, 720027);
                                    client.SendSysMesage("Congratulations you received a MeteorScroll!");
                                    break;
                                }
                            case 2:
                                {
                                    client.Inventory.Add(stream, 1088000, 2);
                                    client.Inventory.Add(stream, 723725);
                                    client.SendSysMesage("Congratulations you received 2 DragonBall, a LifeFruitBasket!");
                                    break;
                                }
                            case 3:
                                {
                                    if (MyMath.ChanceSuccess(95))
                                    {
                                        client.Player.TournamentsPoints += 1;
                                        client.Inventory.Add(stream, 720031);

                                        client.SendSysMesage("Congratulations you received 1 TournamentsPoints and some FireWorks!");
                                    }
                                    else
                                    {
                                        client.Player.TournamentsPoints += 3;
                                        client.Inventory.Add(stream, 723725);
                                        client.SendSysMesage("Congratulations you received 3 TournamentsPoints and a LifeFruitBasket!");
                                    }
                                    break;
                                }
                            default:
                                {
                                    //if (MyMath.ChanceSuccess(15))
                                    //{
                                    //    client.Inventory.Add(stream, 730002);
                                    //    client.SendSysMesage("Congratulations you received +2 Stone!");
                                    //}
                                    //else
                                    //{
                                        client.Inventory.Add(stream, 730002);
                                        client.SendSysMesage("Congratulations you received +2 Stone!");
                                    //}
                                    break;
                                }
                        }
                        break;
                    }
                case 720122:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        int x = Program.Rand.Next(1, 5);
                        switch (x)
                        {
                            case 1:
                                {
                                    client.Inventory.Add(stream, 720027);
                                    client.SendSysMesage("Congratulations you received a MeteorScroll!");
                                    break;
                                }
                            case 2:
                                {
                                    client.Inventory.Add(stream, 1088000, 2);
                                    client.Inventory.Add(stream, 723725);

                                    client.SendSysMesage("Congratulations you received 2 DragonBall, a LifeFruitBasket!");
                                    break;
                                }
                            case 3:
                                {
                                    if (MyMath.ChanceSuccess(95))
                                    {
                                        client.Player.TournamentsPoints += 1;
                                        client.Inventory.Add(stream, 720031);

                                        client.SendSysMesage("Congratulations you received 1 TournamentsPoints and some FireWorks!");
                                    }
                                    else
                                    {
                                        client.Player.TournamentsPoints += 3;
                                        client.Inventory.Add(stream, 723725);
                                        client.SendSysMesage("Congratulations you received 3 TournamentsPoints and a LifeFruitBasket!");
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (MyMath.ChanceSuccess(15))
                                    {
                                        client.Inventory.Add(stream, 730002);
                                        client.SendSysMesage("Congratulations you received +2 Stone!");
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 730002);
                                        client.SendSysMesage("Congratulations you received +2 Stone!");
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 720123:
                    {
                        client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream);
                        int x = Program.Rand.Next(1, 5);
                        switch (x)
                        {
                            case 1:
                                {
                                    client.Inventory.Add(stream, 720027);
                                    client.SendSysMesage("Congratulations you received a MeteorScroll!");
                                    break;
                                }
                            case 2:
                                {
                                    client.Inventory.Add(stream, 1088000, 2);
                                    client.Inventory.Add(stream, 723725);
                                    
                                    client.SendSysMesage("Congratulations you received 2 DragonBall, a LifeFruitBasket!");
                                    break;
                                }
                            case 3:
                                {
                                    if (MyMath.ChanceSuccess(95))
                                    {
                                        client.Player.TournamentsPoints += 1;
                                        client.Inventory.Add(stream, 720031);
                                       
                                        client.SendSysMesage("Congratulations you received 1 TournamentsPoints and some FireWorks!");
                                    }
                                    else
                                    {
                                        client.Player.TournamentsPoints += 3;
                                        client.Inventory.Add(stream, 723725);                                        
                                        client.SendSysMesage("Congratulations you received 3 TournamentsPoints and a LifeFruitBasket!");
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (MyMath.ChanceSuccess(15))
                                    {
                                        client.Inventory.Add(stream, 730002);
                                        client.SendSysMesage("Congratulations you received +2 Stone!");
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 730002);
                                        client.SendSysMesage("Congratulations you received +2 Stone!");
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 720758:
                    if (client.Inventory.HaveSpace(10))
                    {
                        client.Inventory.Update(item, AddMode.REMOVE, stream);
                        client.Inventory.Add(stream, 722057, 10);

                    }
                    else
                        client.SendSysMesage($"Not enough space to open the Item, please leave {10} empty spaces.");
                    break;
                case 720668u:
                    client.SendSysMesage($"Item is not available in server!");
                    //return;
                    //client.Inventory.Update(item, AddMode.REMOVE, stream);
                    //client.GainExpBall(100.0);
                    //client.CreateBoxDialog("You used the Magic Ball and received EXP worth 1/6 of an EXP Ball from Demon Box.");
                    break;
            }
        }
    }
}
