using TheChosenProject.Client;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Mobs;
using TheChosenProject.Role;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgAutoHunting;

namespace TheChosenProject
{
    public class DropRule
    {
        public enum CheckUpType : byte
        {
            Normal,
            Boss
        }

        public enum Status : byte
        {
            All,
            DB,
            Letter,
            Fruits,
            City,
            PowerEXPBall,
            Pumpkins,
            PumpkinSeeds,
            Count,
            Cps,
            Silver,
        }

        public class StaticDrop
        {
            public uint MaxValue;
            public uint MinValue;
            public bool InInventory;

            public bool InFloor
            {
                get { return MinValue > 0 && MaxValue > 0; }
            }

            public double Rate;

            public StaticDrop()
            {
                MinValue = 0;
                MaxValue = 0;
                Rate = 0;
                InInventory = false;
            }

            public string ToString()
            {
                return $"{MinValue}/{MaxValue}/{Rate}/{InInventory}";
            }

            public void Load(string str)
            {
                var array = str.Split('/');
                MinValue = uint.Parse(array[0]);
                MaxValue = uint.Parse(array[1]);
                Rate = double.Parse(array[2]);
                InInventory = bool.Parse(array[3]);
            }
        }

        public class DropItems
        {
            public uint ItemID;
            public uint Count;
            public double Rate;

            public string ToString()
            {
                return $"{ItemID}/{Count}/{Rate}";
            }

            public void Load(string str)
            {
                var array = str.Split('/');
                ItemID = uint.Parse(array[0]);
                Count = uint.Parse(array[1]);
                Rate = double.Parse(array[2]);
            }
        }

        public class AttrSouls
        {
            public uint Count;
            public double Rate;

            public string ToString()
            {
                return $"{Count}/{Rate}";
            }

            public void Load(string str)
            {
                var array = str.Split('/');
                Count = uint.Parse(array[0]);
                Rate = double.Parse(array[1]);
            }
        }

        public class Money
        {
            public ulong Value;
            public uint Count;
            public double Rate;

            public Money()
            {
                Value = 100000;
                Count = 10;
                Rate = 90;
            }

            public string ToString()
            {
                return $"{Value}/{Count}/{Rate}";
            }

            public void Load(string str)
            {
                var array = str.Split('/');
                Value = ulong.Parse(array[0]);
                Count = uint.Parse(array[1]);
                Rate = double.Parse(array[2]);
            }
        }

        public class MonsterInfo
        {
            public uint ID;
            public uint Map;
            public Money Money;
            public AttrSouls Soul3;
            public AttrSouls Soul4;
            public AttrSouls Soul5;
            public AttrSouls Soul6;
            public uint[] Ranks;
            public List<DropItems> Items;

            public MonsterInfo()
            {
                Money = new Money();
                Soul3 = new AttrSouls();
                Soul4 = new AttrSouls();
                Soul5 = new AttrSouls();
                Soul6 = new AttrSouls();
                Items = new List<DropItems>();
                Ranks = new uint[3];
            }
        }

        public class Drop
        {
            public string Tag;
            public StaticDrop Silver;
            public StaticDrop Cps;
            public StaticDrop DB;
            public StaticDrop PowerEXPBall;
            public StaticDrop Letter, Fruits, City, PumpkinSeeds, Pumpkins;

            public Drop(string _tag)
            {
                Tag = _tag;
                Silver = new StaticDrop();
                Cps = new StaticDrop();
                DB = new StaticDrop();
                PowerEXPBall = new StaticDrop();
                Letter = new StaticDrop();
                Fruits = new StaticDrop();
                City = new StaticDrop();
                PumpkinSeeds = new StaticDrop();
                Pumpkins = new StaticDrop();
            }

            public void Load(WindowsAPI.IniFile reader)
            {
                Silver.Load(reader.ReadString(Tag, "Silver", ""));
                Cps.Load(reader.ReadString(Tag, "Cps", ""));
                DB.Load(reader.ReadString(Tag, "DB", ""));
                PowerEXPBall.Load(reader.ReadString(Tag, "PowerEXPBall", ""));
                Letter.Load(reader.ReadString(Tag, "Letter", ""));
                Fruits.Load(reader.ReadString(Tag, "Fruits", ""));
                City.Load(reader.ReadString(Tag, "City", ""));
                PumpkinSeeds.Load(reader.ReadString(Tag, "PumpkinSeeds", ""));
                Pumpkins.Load(reader.ReadString(Tag, "Pumpkins", ""));
            }

            public string[] Save()
            {
                List<string> Lines = new List<string>();
                Lines.Add($"[{Tag}]");
                Lines.Add($"Silver={Silver.ToString()}");
                Lines.Add($"Cps={Cps.ToString()}");
                Lines.Add($"DB={DB.ToString()}");
                Lines.Add($"PowerEXPBall={PowerEXPBall.ToString()}");
                Lines.Add($"Letter={Letter.ToString()}");
                Lines.Add($"Fruits={Fruits.ToString()}");
                Lines.Add($"City={City.ToString()}");
                Lines.Add($"Pumpkins={Pumpkins.ToString()}");
                Lines.Add($"PumpkinSeeds={PumpkinSeeds.ToString()}");
                Lines.Add($"");
                return Lines.ToArray();
            }
        }



        public ConcurrentDictionary<uint, MonsterInfo> BossPool = new ConcurrentDictionary<uint, MonsterInfo>();

        public Drop Normal;
        public Drop Vip3;
        public Drop Vip6;
        public Drop Vip3_AutoHunt;
        public Drop Vip6_AutoHunt;

        public bool Checkup(ServerSockets.Packet stream, CheckUpType type, GameClient killer, MonsterRole MobRole, TheChosenProject.Mobs.Base _base = null)
        {
            switch (type)
            {
                case CheckUpType.Normal:
                    {
                        if (killer == null)
                            return false;
                        killer.DBMobs++;
                        killer.PowerEXPBallMobs++;
                        killer.LetterMobs++;
                        killer.FruitsMobs++;
                        killer.CityMobs++;
                        killer.PumpkinsMobs++;
                        killer.PumpkinSeedsMobs++;
                        //#region Halloween
                        //if (killer.PumpkinsMobs > killer.MaxPumpkins)
                        //{
                        //    if (killer.GeneratorItemDrop(Status.Pumpkins))
                        //    {
                        //        MobRole.DropItemID22(killer, 722176, stream, 3, true);
                        //    }
                        //}
                        //if (killer.PumpkinSeedsMobs > killer.MaxPumpkinSeeds)
                        //{
                        //    if (killer.GeneratorItemDrop(Status.PumpkinSeeds))
                        //    {
                        //        MobRole.DropItemID22(killer, 710587, stream, 3, true);
                        //    }
                        //}
                        //#endregion
                        //#region CityMobs

                        //if (killer.CityMobs > killer.MaxCity)
                        //{
                        //    if (killer.GeneratorItemDrop(Status.City))
                        //    {
                        //        byte rand2 = (byte)Program.GetRandom.Next(0, 7);
                        //        switch (rand2)
                        //        {
                        //            case 0:
                        //                {
                        //                    MobRole.DropItemID(killer, 720364, stream);
                        //                    killer.SendSysMesage("BeanStalk Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            case 1:
                        //                {
                        //                    MobRole.DropItemID(killer, 720362, stream);
                        //                    killer.SendSysMesage("FatPumpkin Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            case 2:
                        //                {
                        //                    if (Role.MyMath.Success(90))
                        //                    {
                        //                        MobRole.DropItemID(killer, 720365, stream);
                        //                        killer.SendSysMesage("Shampoo Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    }
                        //                    else
                        //                    {
                        //                        MobRole.DropItemID(killer, 720157, stream);
                        //                        killer.SendSysMesage("XmasCandy Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                        string MSG = "#53 XmasCandy Dropped by " + killer.Player.Name + " #53.";
                        //                        Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                        //                    }
                        //                    break;
                        //                }
                        //            case 3:
                        //                {
                        //                    MobRole.DropItemID(killer, 710968, stream);
                        //                    killer.SendSysMesage("Chocolate Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            case 4:
                        //                {
                        //                    MobRole.DropItemID(killer, 720157, stream);
                        //                    killer.SendSysMesage("XmasCandy Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            default:
                        //                {
                        //                    goto case 2;
                        //                    break;
                        //                }
                        //        }
                        //        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel4");
                        //        return true;
                        //    }
                        //}

                        //#endregion CityMobs
                        //#region FruitsMobs

                        //if (killer.MaxFruits > 0 && killer.FruitsMobs >= killer.MaxFruits)
                        //{
                        //    if (killer.GeneratorItemDrop(Status.Fruits))
                        //    {
                        //        byte rand2 = (byte)Program.GetRandom.Next(0, 7);
                        //        switch (rand2)
                        //        {
                        //            case 0:
                        //                {
                        //                    MobRole.DropItemID(killer, 711301, stream);
                        //                    killer.SendSysMesage("Tomato Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            case 1:
                        //                {
                        //                    MobRole.DropItemID(killer, 711302, stream);
                        //                    killer.SendSysMesage("Guava Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            case 2:
                        //                {
                        //                    MobRole.DropItemID(killer, 711303, stream);
                        //                    killer.SendSysMesage("Watermelon Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            case 3:
                        //                {
                        //                    MobRole.DropItemID(killer, 711304, stream);
                        //                    killer.SendSysMesage("Pear Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    break;
                        //                }
                        //            case 4:
                        //                {
                        //                    if (Role.MyMath.Success(90))
                        //                    {
                        //                        MobRole.DropItemID(killer, 711305, stream);
                        //                        killer.SendSysMesage("Grape Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                    }
                        //                    else
                        //                    {
                        //                        MobRole.DropItemID(killer, 711302, stream);
                        //                        killer.SendSysMesage("Guava Dropped.", MsgMessage.ChatMode.TopLeft);
                        //                        string MSG = "#53 Guava Dropped by " + killer.Player.Name + " #53.";
                        //                        Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
                        //                    }
                        //                    break;
                        //                }
                        //            default:
                        //                {
                        //                    goto case 4;
                        //                    break;
                        //                }
                        //        }
                        //        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel5");
                        //        return true;
                        //    }
                        //}

                        //#endregion FruitsMobs

                        return true;
                    }
            }
            return false;
        }

        public StaticDrop GetStatic(Client.GameClient killer, Status type)
        {
            StaticDrop TempStatic = null;
            switch (type)
            {
                case Status.Pumpkins:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.Pumpkins;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.Pumpkins;
                            else TempStatic = Normal.Pumpkins;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.Pumpkins;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.Pumpkins;
                            else TempStatic = Normal.Pumpkins;
                        }
                        break;
                    }
                case Status.PumpkinSeeds:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.PumpkinSeeds;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.PumpkinSeeds;
                            else TempStatic = Normal.PumpkinSeeds;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.PumpkinSeeds;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.PumpkinSeeds;
                            else TempStatic = Normal.PumpkinSeeds;
                        }
                        break;
                    }
                case Status.Cps:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.Cps;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.Cps;
                            else TempStatic = Normal.Cps;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.Cps;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.Cps;
                            else TempStatic = Normal.Cps;
                        }
                        break;
                    }
                case Status.Silver:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.Silver;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.Silver;
                            else TempStatic = Normal.Silver;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.Silver;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.Silver;
                            else TempStatic = Normal.Silver;
                        }
                        break;
                    }
                case Status.DB:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.DB;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.DB;
                            else TempStatic = Normal.DB;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.DB;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.DB;
                            else TempStatic = Normal.DB;
                        }
                        break;
                    }
                case Status.PowerEXPBall:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.PowerEXPBall;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.PowerEXPBall;
                            else TempStatic = Normal.PowerEXPBall;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.PowerEXPBall;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.PowerEXPBall;
                            else TempStatic = Normal.PowerEXPBall;
                        }
                        break;
                    }
                case Status.Letter:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.Letter;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.Letter;
                            else TempStatic = Normal.Letter;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.Letter;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.Letter;
                            else TempStatic = Normal.Letter;
                        }
                        break;
                    }
                case Status.Fruits:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.Fruits;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.Fruits;
                            else TempStatic = Normal.Fruits;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.Fruits;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.Fruits;
                            else TempStatic = Normal.Fruits;
                        }
                        break;
                    }
                case Status.City:
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6_AutoHunt.City;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3_AutoHunt.City;
                            else TempStatic = Normal.City;
                        }
                        else
                        {
                            if (killer.Player.VipLevel >= 4) TempStatic = Vip6.City;
                            else if (killer.Player.VipLevel > 0) TempStatic = Vip3.City;
                            else TempStatic = Normal.City;
                        }
                        break;
                    }
                default:
                    {
                        if (type != Status.All)
                        {
                            TempStatic = new StaticDrop();
                            System.Console.WriteLine(string.Format("Can`t Find Static Drop Type :", type.ToString()));

                        }
                        break;
                    }
            }
            return TempStatic;
        }

        #region Private Function Helper

        private uint StoneId(UInt16 Plus)
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

        //private uint GenerateGold(out uint ItemID, MonsterRole Mob)
        //{
        //    uint amount = 0;
        //    if (Mob.Map == 1002)
        //    {
        //        amount = (uint)Program.GetRandom.Next(0, (int)(((100 * Mob.Family.Level / 2) / 2) + 1));
        //    }
        //    else
        //    {
        //        if (Mob.Boss != 0)
        //            amount = (uint)Program.GetRandom.Next(Mob.Family.DropMoney, Mob.Family.DropMoney * 10);
        //        else
        //        {
        //            amount = (uint)Program.GetRandom.Next(0, (int)(((1000 * Mob.Family.Level / 2) / 2) + 1));
        //            amount /= 2;
        //        }
        //    }
        //    ItemID = Database.ItemType.MoneyItemID((uint)amount);
        //    return amount;
        //}
        private uint GenerateGold(out uint ItemID, MonsterRole Mob)
        {
            uint amount = 0;
            if (Mob.Map == 1002)
            {
                // Lower the multiplier here to reduce overall gold drop
                amount = (uint)Program.GetRandom.Next(0, (int)(((50 * Mob.Family.Level / 2) / 2) + 1));
            }
            else
            {
                if (Mob.Boss != 0)
                {
                    // Reduce the upper limit multiplier for boss mobs
                    amount = (uint)Program.GetRandom.Next(Mob.Family.DropMoney, Mob.Family.DropMoney * 5);
                }
                else
                {
                    // Lower the multiplier here as well for non-boss mobs
                    amount = (uint)Program.GetRandom.Next(0, (int)(((500 * Mob.Family.Level / 2) / 2) + 1));
                    amount /= 2;
                }
            }
            ItemID = Database.ItemType.MoneyItemID((uint)amount);
            return amount;
        }


        private void DropItem(ServerSockets.Packet stream, MonsterRole Mob, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, Game.MsgFloorItem.MsgItem.ItemType typ, uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null)
        {
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
                                DataItem.Plus = 2;
                            }
                            else if (Core.Rate(60))
                            {
                                DataItem.Bless = (byte)Program.GetRandom.Next(1, 3);
                            }
                            else if (Core.Rate(70))
                            {
                                DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                            }
                            else DataItem.Plus = 1;
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
                if (DataItem.Bless > 0)
                {
                    if (user.Inventory.HaveSpace(1))
                    {
                        if (DBItem != null)
                        {
                            user.Inventory.Add(DataItem, DBItem, stream);
                            user.SendSysMesage("A " + DBItem.Name + "[-" + DataItem.Bless + " Blessed] got autopacked.", MsgMessage.ChatMode.Talk);
                            return;
                        }
                    }
                    else
                    {
                        user.SendSysMesage("Please empty ur items to get your picked up a " + DBItem.Name + "[-" + DataItem.Bless + " Blessed] !", MsgMessage.ChatMode.Talk);
                    }
                }
                if (DataItem.Plus > 0)
                {
                    if (user.Inventory.HaveSpace(1))
                    {
                        if (DBItem != null)
                        {
                            user.Inventory.Add(DataItem, DBItem, stream);
                            user.SendSysMesage("A " + DBItem.Name + "[+" + DataItem.Plus + "] got autopacked.", MsgMessage.ChatMode.Talk);
                            return;
                        }
                    }
                    else
                    {
                        user.SendSysMesage("Please empty ur items to get your picked up a " + DBItem.Name + "[+" + DataItem.Plus + "] !", MsgMessage.ChatMode.Talk);
                    }
                }
                if (DataItem.SocketTwo > 0)
                {
                    if (user.Inventory.HaveSpace(1))
                    {
                        if (DBItem != null)
                        {
                            user.Inventory.Add(DataItem, DBItem, stream);
                            user.SendSysMesage("A " + DBItem.Name + "[2soc] got autopacked.", MsgMessage.ChatMode.Talk);
                            return;
                        }
                    }
                    else
                    {
                        user.SendSysMesage("Please empty ur items to get your picked up a " + DBItem.Name + "[2soc] !", MsgMessage.ChatMode.Talk);
                    }
                }
                if (ID_Quality == 9)
                {
                    if (user.Inventory.HaveSpace(1))
                    {
                        if (DBItem != null)
                        {
                            user.Inventory.Add(DataItem, DBItem, stream);
                            user.SendSysMesage("A " + DBItem.Name + "[Super] got autopacked.", MsgMessage.ChatMode.Talk);
                            return;
                        }
                    }
                    else
                    {
                        user.SendSysMesage("Please empty ur items to get your picked up a  " + DBItem.Name + "[Super] !", MsgMessage.ChatMode.Talk);
                    }
                }
            }
            Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, Mob.DynamicID, Mob.Map, OwnerItem, true, map);

            if (map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Visible);
            }
        }

        //private uint GenerateItemId(MonsterRole Mob, uint map, out byte dwItemQuality, out bool Special, out Database.ItemType.DBItem DbItem)
        //{
        //    Special = false;
        //    if (Role.Core.Rate(75))
        //    {
        //        dwItemQuality = Mob.Family.ItemGenerator.GenerateQuality();
        //    }
        //    else dwItemQuality = 9;//super

        //    uint dwItemSort = 0;
        //    uint dwItemLev = 0;
        //    int nRand = Extensions.BaseFunc.RandGet(1200, false);
        //    if (nRand >= 0 && nRand < 20) // 0.17%
        //    {
        //        dwItemSort = 160;
        //        dwItemLev = Mob.Family.DropBoots;
        //    }
        //    else if (nRand >= 20 && nRand < 50) // 0.25%
        //    {
        //        dwItemSort = MobItemGenerator.NecklaceType[Extensions.BaseFunc.RandGet(MobItemGenerator.NecklaceType.Length, false)];
        //        dwItemLev = Mob.Family.DropNecklace;
        //    }
        //    else if (nRand >= 50 && nRand < 100) // 4.17%
        //    {
        //        dwItemSort = MobItemGenerator.RingType[Extensions.BaseFunc.RandGet(MobItemGenerator.RingType.Length, false)];
        //        dwItemLev = Mob.Family.DropRing;
        //    }
        //    else if (nRand >= 100 && nRand < 400) // 25%
        //    {
        //        dwItemSort = MobItemGenerator.ArmetType[Extensions.BaseFunc.RandGet(MobItemGenerator.ArmetType.Length, false)];
        //        dwItemLev = Mob.Family.DropArmet;
        //    }
        //    else if (nRand >= 400 && nRand < 700) // 25%
        //    {
        //        dwItemSort = MobItemGenerator.ArmorType[Extensions.BaseFunc.RandGet(MobItemGenerator.ArmorType.Length, false)];
        //        dwItemLev = Mob.Family.DropArmor;
        //    }
        //    else // 45%
        //    {
        //        int nRate = Extensions.BaseFunc.RandGet(100, false);
        //        if (nRate >= 0 && nRate < 20) // 20% of 45% (= 9%) - Backswords
        //        {
        //            dwItemSort = 421;
        //        }
        //        else if (nRate >= 40 && nRate < 80)	// 40% of 45% (= 18%) - One handers
        //        {
        //            dwItemSort = MobItemGenerator.OneHanderType[Extensions.BaseFunc.RandGet(MobItemGenerator.OneHanderType.Length, false)];
        //            dwItemLev = Mob.Family.DropWeapon;
        //        }
        //        else if (nRand >= 80 && nRand < 100)// 20% of 45% (= 9%) - Two handers (and shield)
        //        {
        //            dwItemSort = MobItemGenerator.TwoHanderType[Extensions.BaseFunc.RandGet(MobItemGenerator.TwoHanderType.Length, false)];
        //            dwItemLev = ((dwItemSort == 900) ? Mob.Family.DropShield : Mob.Family.DropWeapon);
        //        }
        //    }
        //    if (dwItemLev != 99)
        //    {
        //        dwItemLev = Mob.Family.ItemGenerator.AlterItemLevel(dwItemLev, dwItemSort);
        //        uint idItemType = (dwItemSort * 1000) + (dwItemLev * 10) + dwItemQuality;
        //        if (Server.ItemsBase.TryGetValue(idItemType, out DbItem))
        //        {
        //            ushort position = Database.ItemType.ItemPosition(idItemType);
        //            byte level = Database.ItemType.ItemMaxLevel((Role.Flags.ConquerItem)position);
        //            if (DbItem.Level > level)
        //                return 0;
        //            return idItemType;
        //        }
        //    }
        //    DbItem = null;
        //    return 0;
        //}

        #endregion Private Function Helper

        public void Save()
        {
            List<string> lines = new List<string>();
            lines.Add($"[Mobs]");
            lines.Add($"Amount={BossPool.Count}");
            uint Count = 0;
            foreach (var mob in BossPool.Values)
            {
                lines.Add($"");
                lines.Add($"[{Count}]");
                lines.Add($"ID={mob.ID}");
                lines.Add($"Map={mob.Map}");
                lines.Add($"Money={mob.Money.ToString()}");
                lines.Add($"Soul3={mob.Soul3.ToString()}");
                lines.Add($"Soul4={mob.Soul4.ToString()}");
                lines.Add($"Soul5={mob.Soul5.ToString()}");
                lines.Add($"Soul6={mob.Soul6.ToString()}");
                string ranks = "";
                for (int r = 0; r < mob.Ranks.Length; r++)
                {
                    if ((r + 1) == mob.Ranks.Length)
                        ranks += $"{mob.Ranks[r]}";
                    else ranks += $"{mob.Ranks[r]}/";
                }
                lines.Add($"Ranks={ranks}");
                lines.Add($"AmountDrop={mob.Items.Count}");
                for (int i = 0; i < mob.Items.Count; i++)
                {
                    lines.Add($"Drop{i}={mob.Items[i].ToString()}");
                }
                Count++;
            }
            lines.Add($"");
            string[] array = Normal.Save();
            foreach (var str in array)
                lines.Add(str);
            array = Vip3_AutoHunt.Save();
            foreach (var str in array)
                lines.Add(str);
            array = Vip6_AutoHunt.Save();
            foreach (var str in array)
                lines.Add(str);
            array = Vip3.Save();
            foreach (var str in array)
                lines.Add(str);
            array = Vip6.Save();
            foreach (var str in array)
                lines.Add(str);
            File.WriteAllLines(ServerKernel.CO2FOLDER + "\\DropRule.ini", lines.ToArray());
            foreach (var client in Server.GamePoll.Values)
                client.GeneratorItemDrop(Status.All);
        }

        public void Load()
        {
            try
            {
                Normal = new Drop("Normal");
                Vip3_AutoHunt = new Drop("Vip3AutoHunt");
                Vip6_AutoHunt = new Drop("Vip6AutoHunt");
                Vip3 = new Drop("Vip3");
                Vip6 = new Drop("Vip6");
                WindowsAPI.IniFile reader = new WindowsAPI.IniFile(ServerKernel.CO2FOLDER + "\\DropRule.ini", true);
                try
                {
                    Normal.Load(reader);
                    Vip3_AutoHunt.Load(reader);
                    Vip6_AutoHunt.Load(reader);
                    Vip3.Load(reader);
                    Vip6.Load(reader);
                }
                catch { goto L1; }
            L1:
                int Count = reader.ReadInt32("Mobs", "Amount", 0);
                for (int x = 0; x < Count; x++)
                {
                    try
                    {
                        MonsterInfo mi = new MonsterInfo()
                        {
                            ID = reader.ReadUInt32($"{x}", "ID", 0),
                            Map = reader.ReadUInt32($"{x}", "Map", 0),
                        };

                        try
                        {
                            mi.Money.Load(reader.ReadString($"{x}", "Money", ""));
                            mi.Soul3.Load(reader.ReadString($"{x}", "Soul3", ""));
                            mi.Soul4.Load(reader.ReadString($"{x}", "Soul4", ""));
                            mi.Soul5.Load(reader.ReadString($"{x}", "Soul5", ""));
                            mi.Soul6.Load(reader.ReadString($"{x}", "Soul6", ""));
                        }
                        catch { goto L2; }
                        try
                        {
                            var CountRank = reader.ReadString($"{x}", "Ranks", "");
                            if (CountRank != null && CountRank != "")
                            {
                                var arrayRank = CountRank.Split('/');
                                if (arrayRank != null && arrayRank.Length > 0)
                                {
                                    mi.Ranks = new uint[arrayRank.Length];
                                    for (int r = 0; r < mi.Ranks.Length; r++)
                                    {
                                        mi.Ranks[r] = uint.Parse(arrayRank[r]);
                                    }
                                }
                            }
                        }
                        catch { goto L2; }
                    L2:
                        mi.Items = new List<DropItems>();
                        uint AmountDrop = reader.ReadUInt32($"{x}", "AmountDrop", 0);
                        for (int i = 0; i < AmountDrop; i++)
                        {
                            try
                            {
                                var str = reader.ReadString($"{x}", $"Drop{i}", "");
                                if (str != "")
                                {
                                    var array = str.Split('/');
                                    if (array != null && array.Length >= 3)
                                    {
                                        DropItems di = new DropItems()
                                        {
                                            ItemID = uint.Parse(array[0]),
                                            Count = byte.Parse(array[1]),
                                            Rate = double.Parse(array[2])
                                        };
                                        mi.Items.Add(di);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteException(e);
                                continue;
                            }
                        }
                        BossPool.TryAdd(mi.ID + mi.Map, mi);
                    }
                    catch (Exception e)
                    {
                        Console.WriteException(e);
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteException(e);
            }
        }
    }
}