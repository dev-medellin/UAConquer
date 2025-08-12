using TheChosenProject.Client;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject
{
    public class DropMob
    {
        public unsafe static bool Checkup(ServerSockets.Packet stream, GameClient killer, MonsterRole Mob)
        {
            killer.Drop_DB++;
            killer.Drop_Meteors++;
            killer.Drop_Item++;
            killer.Drop_Stone++;

            ushort xx = Mob.X;
            ushort yy = Mob.Y;

            #region DragonBall
            if (killer.Drop_DB >= killer.MaxCount_DB)
            {
                killer.Drop_DB = 0;
                killer.MaxCount_DB = (uint)Program.GetRandom.Next(4500, 15000);

                uint ID = 1088000;

                if ((killer.AutoHunting.Enable && killer.AutoHunting.DBalls))
                {
                    ActionQuery action2;

                    action2 = new ActionQuery()
                    {
                        ObjId = killer.Player.UID,
                        Type = ActionType.DragonBall
                    };
                    killer.Send(stream.ActionCreate(&action2));

                    if (killer.Inventory.HaveSpace(1))
                    {
                        if (killer.Inventory.Contain(ID, 9))
                        {
                            killer.Inventory.Remove(ID, 9, stream);
                            killer.Inventory.Add(stream, 720028, 1);
                        }
                        else
                        {
                            killer.Inventory.Add(stream, ID, 1);
                        }
                        killer.SendSysMesage("[AutoHunt] DragonBall got autopacked.", MsgMessage.ChatMode.Talk);
                    }
                    else
                    {
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            Mob.DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy,
                                Game.MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0, killer, null, true);
                        }
                    }
                }
                else if (killer.Map.AddGroundItem(ref xx, ref yy))
                {
                    Mob.DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy,
                        Game.MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0, killer, null, true);
                }
                return true;
            }
            #endregion

            #region Meteros
            if (killer.Drop_Meteors >= killer.MaxCount_Meteors)
            {
                killer.Drop_Meteors = 0;
                killer.MaxCount_Meteors = (uint)Program.GetRandom.Next(200, 950);

                uint ID = 1088001;

                if ((killer.AutoHunting.Enable && killer.AutoHunting.Meteors))
                { 
                    if (killer.Inventory.HaveSpace(1))
                    {
                        if (killer.Inventory.Contain(ID, 9))
                        {
                            killer.Inventory.Remove(ID, 9, stream);
                            killer.Inventory.Add(stream, 720027, 1);
                        }
                        else killer.Inventory.Add(stream, ID, 1);
                        killer.SendSysMesage("[AutoHunt] 1xMeteor got autopacked.", MsgMessage.ChatMode.Talk);
                        return true;
                    }
                    else
                    {
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            Mob.DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy,
                                Game.MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0, killer, null, true);
                            return true;
                        }
                    }
                }
                else if (killer.Map.AddGroundItem(ref xx, ref yy))
                {
                    Mob.DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy,
                        Game.MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0, killer, null, true);
                    return true;
                }
            }
            #endregion

            #region Items
            if (killer.Drop_Item >= killer.MaxCount_Item)
            {
                killer.Drop_Item = 0;
                killer.MaxCount_Item = (uint)Program.GetRandom.Next(4500, 5000);
                Database.ItemType.DBItem DbItem = null;
                byte ID_Quality;
                bool ID_Special;
                uint ID = GenerateItemId(Mob, Mob.Map, out ID_Quality, out ID_Special, out DbItem);
                if (killer.Map.AddGroundItem(ref xx, ref yy))
                {
                    DropItem(stream,Mob, killer.Player.UID, killer.Map, ID, xx, yy, Game.MsgFloorItem.MsgItem.ItemType.Item, 0, ID_Special, ID_Quality, killer, DbItem);
                }
                return true;
            }
            #endregion

            #region Stone
            if (killer.Drop_Stone >= killer.MaxCount_Stone)
            {
                killer.Drop_Stone = 0;
                killer.MaxCount_Stone = (uint)Program.GetRandom.Next(2500, 3000);

                uint ID = StoneId(1);

                if ((killer.AutoHunting.Enable && killer.AutoHunting.PlusItems))
                {
                    if (killer.Inventory.HaveSpace(1))
                    {
                        killer.Inventory.Add(stream, ID, 1, 1);
                        killer.SendSysMesage("[AutoHunt] Stone(+1) got autopacked.", MsgMessage.ChatMode.Talk);
                        return true;
                    }
                    else
                    {
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            Mob.DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy,
                        Game.MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                            return true;
                        }
                    }
                }
                else if (killer.Map.AddGroundItem(ref xx, ref yy))
                {
                    Mob.DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy,
                        Game.MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                    return true;
                }
            }
            #endregion

            return false;
        }
        public static uint StoneId(UInt16 Plus)
        {
            switch (Plus)
            {
                case 1: return 730001;
                case 2: return 730002;
                case 3: return 730003;
                case 4: return 730004;
                case 5: return 730005;
                case 6: return 730006;
                case 7: return 730007;
                case 8: return 730008;
            }
            return 0;
        }
        public static void DropItem(ServerSockets.Packet stream, MonsterRole Mob, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, Game.MsgFloorItem.MsgItem.ItemType typ
            , uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null)
        {
            try
            {
                if (DBItem == null) return;

                Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();

                DataItem.ITEM_ID = ItemID;
                if (DataItem.Durability > 100)
                {
                    DataItem.Durability = (ushort)Program.GetRandom.Next(100, DataItem.Durability / 10);
                    DataItem.MaximDurability = DataItem.Durability;
                }

                else
                {
                    DataItem.Durability = (ushort)Program.GetRandom.Next(1, 10);
                    DataItem.MaximDurability = 10;
                }
                DataItem.Color = Role.Flags.Color.Red;
                if (typ == Game.MsgFloorItem.MsgItem.ItemType.Item)
                {
                    if (DataItem.IsEquip)
                    {
                        if (!special)
                        {
                            if (ID_Quality == 3)
                            {
                                if (Core.Rate(50))
                                {
                                    DataItem.Plus = 1;
                                }
                                else if (Core.Rate(60))
                                {
                                    DataItem.Bless = 1;
                                }
                                else if (Core.Rate(70))
                                {
                                    DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                }
                            }
                            if (DBItem != null)
                            {
                                DataItem.Durability = (ushort)Program.GetRandom.Next(1, DBItem.Durability / 10 + 10);
                                DataItem.MaximDurability = (ushort)Program.GetRandom.Next(DataItem.Durability, DBItem.Durability);
                            }
                        }
                    }
                    else
                    {
                        if (DBItem != null)
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    }
                    if (user.AutoHunting.Enable && user.AutoHunting.BlessedItems)
                    {
                        if (DataItem.Bless > 0)
                        {
                            if (user.Inventory.HaveSpace(1))
                            {
                                user.Inventory.Add(DataItem, DBItem, stream);
                                user.SendSysMesage("[AutoHunt] " + DBItem.Name + "[-" + DataItem.Bless + " Blessed] got autopacked.", MsgMessage.ChatMode.Talk);
                                return;
                            }
                        }
                    }
                    if (user.AutoHunting.Enable && user.AutoHunting.PlusItems)
                    {
                        if (DataItem.Plus > 0)
                        {
                            if (user.Inventory.HaveSpace(1))
                            {
                                user.Inventory.Add(DataItem, DBItem, stream);
                                user.SendSysMesage("[AutoHunt] " + DBItem.Name + "[+" + DataItem.Plus + "] got autopacked.", MsgMessage.ChatMode.Talk);
                                return;
                            }
                        }
                    }
                    if (user.AutoHunting.Enable && user.AutoHunting.SocketedItems)
                    {
                        if (DataItem.SocketTwo > 0)
                        {
                            if (user.Inventory.HaveSpace(1))
                            {
                                user.Inventory.Add(DataItem, DBItem, stream);
                                user.SendSysMesage("[AutoHunt] " + DBItem.Name + "[2soc] got autopacked.", MsgMessage.ChatMode.Talk);
                                return;
                            }
                        }
                    }
                    if (user.AutoHunting.Enable && user.AutoHunting.QualityItems)
                    {
                        if (ID_Quality == 9)
                        {
                            if (user.Inventory.HaveSpace(1))
                            {
                                user.Inventory.Add(DataItem, DBItem, stream);
                                user.SendSysMesage("[AutoHunt] " + DBItem.Name + "[Super] got autopacked.", MsgMessage.ChatMode.Talk);
                                return;
                            }
                        }
                    }
                }
                Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, Mob.DynamicID, Mob.Map, OwnerItem, true, map);

                if (map.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Visible);
                }
            }
            catch(Exception e)
            {
                Console.SaveException(e);
            }
        }

        public static uint GenerateItemId(MonsterRole Mob, uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        {
            Special = false;
            if (Role.Core.Rate(75))
            {
                dwItemQuality = Mob.Family.ItemGenerator.GenerateQuality();
            }
            else dwItemQuality = 9;//super
            uint dwItemSort = 0;
            uint dwItemLev = 0;
            int nRand = Extensions.BaseFunc.RandGet(1200, false);
            if (nRand >= 0 && nRand < 20) // 0.17%
            {
                dwItemSort = 160;
                dwItemLev = Mob.Family.DropBoots;
            }
            else if (nRand >= 20 && nRand < 50) // 0.25%
            {
                dwItemSort = MobItemGenerator.NecklaceType[Extensions.BaseFunc.RandGet(MobItemGenerator.NecklaceType.Length, false)];
                dwItemLev = Mob.Family.DropNecklace;
            }
            else if (nRand >= 50 && nRand < 100) // 4.17%
            {
                dwItemSort = MobItemGenerator.RingType[Extensions.BaseFunc.RandGet(MobItemGenerator.RingType.Length, false)];
                dwItemLev = Mob.Family.DropRing;
            }
            else if (nRand >= 100 && nRand < 400) // 25%
            {
                dwItemSort = MobItemGenerator.ArmetType[Extensions.BaseFunc.RandGet(MobItemGenerator.ArmetType.Length, false)];
                dwItemLev = Mob.Family.DropArmet;
            }
            else if (nRand >= 400 && nRand < 700) // 25%
            {
                dwItemSort = MobItemGenerator.ArmorType[Extensions.BaseFunc.RandGet(MobItemGenerator.ArmorType.Length, false)];
                dwItemLev = Mob.Family.DropArmor;
            }
            else // 45%
            {
                int nRate = Extensions.BaseFunc.RandGet(100, false);
                if (nRate >= 0 && nRate < 20) // 20% of 45% (= 9%) - Backswords
                {
                    dwItemSort = 421;
                }
                else if (nRate >= 40 && nRate < 80)	// 40% of 45% (= 18%) - One handers
                {
                    dwItemSort = MobItemGenerator.OneHanderType[Extensions.BaseFunc.RandGet(MobItemGenerator.OneHanderType.Length, false)];
                    dwItemLev = Mob.Family.DropWeapon;
                }
                else if (nRand >= 80 && nRand < 100)// 20% of 45% (= 9%) - Two handers (and shield)
                {
                    dwItemSort = MobItemGenerator.TwoHanderType[Extensions.BaseFunc.RandGet(MobItemGenerator.TwoHanderType.Length, false)];
                    dwItemLev = ((dwItemSort == 900) ? Mob.Family.DropShield : Mob.Family.DropWeapon);
                }
            }
            if (dwItemLev != 99)
            {
                dwItemLev = Mob.Family.ItemGenerator.AlterItemLevel(dwItemLev, dwItemSort);
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

    }
}
