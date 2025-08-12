using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using System.Diagnostics;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;
using TheChosenProject.Client;
using DevExpress.XtraPrinting.Native;

namespace TheChosenProject.Role.Instance
{

    public class Inventory
    {
        private const byte File_Size = 40;

        public ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem> ClientItems = new ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>();

        public List<uint> SoulsList = new List<uint>();

        public List<uint> RefiendGemsList = new List<uint>();

        public List<uint> NormalGemsList = new List<uint>();

        public List<uint> MatrialList = new List<uint>();
        private readonly GameClient Owner;

        public int GetCountItem(uint ItemID)
        {
            int count = 0;
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ItemID)
                {
                    count += DataItem.StackSize > 1 ? DataItem.StackSize : 1;
                }
            }
            return count;
        }

        public bool VerifiedUpdateItem(List<uint> ItemsUIDS, uint ID, byte count, out Queue<Game.MsgServer.MsgGameItem> Items)
        {
            Queue<Game.MsgServer.MsgGameItem> ExistItems = new Queue<Game.MsgServer.MsgGameItem>();
            foreach (var DataItem in ClientItems.Values)
            {
                if (DataItem.ITEM_ID == ID)
                {
                    if (ItemsUIDS.Contains(DataItem.UID))
                    {
                        count--;
                        ItemsUIDS.Remove(DataItem.UID);
                        ExistItems.Enqueue(DataItem);
                    }
                }
            }
            Items = ExistItems;
            return ItemsUIDS.Count == 0 && count == 0;
        }

        //private Client.GameClient Owner;
        public Inventory(Client.GameClient _own)
        {
            Owner = _own;
        }
        public bool CollectedMoonBoxTokens(byte bound = 0)
        {
            return Contain(721010, 1, bound) && Contain(721011, 1, bound) && Contain(721012, 1, bound) && Contain(721013, 1, bound) && Contain(721014, 1, bound) && Contain(721015, 1, bound);
        }
        public void AddDBItem(Game.MsgServer.MsgGameItem item)
        {
            ClientItems.TryAdd(item.UID, item);
        }

        public void AddReturnedItem(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, ushort StackSize = 0)
        {

            byte x = 0;
            for (; x < count;)
            {
                x++;
                Database.ItemType.DBItem DbItem;
                if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                {

                    Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                    ItemDat.ITEM_ID = ID;
                    ItemDat.Effect = Effect;
                    ItemDat.StackSize = StackSize;
                    ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                    ItemDat.Plus = plus;
                    ItemDat.Bless = bless;
                    ItemDat.Enchant = Enchant;
                    ItemDat.SocketOne = sockone;
                    ItemDat.SocketTwo = socktwo;
                    ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                    ItemDat.Bound = (byte)(bound ? 1 : 0);
                    ItemDat.Mode = Flags.ItemMode.AddItemReturned;
                    ItemDat.WH_ID = ushort.MaxValue;
                    ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                    Owner.Warehouse.AddItem(ItemDat, ushort.MaxValue);

                    ItemDat.Send(Owner, stream);
                }
            }
        }

        public bool CollectedTokens(uint ID)
        {
            if (ID == 1044)
            {
                return Contain(721011, 1);
            }
            if (ID == 1046)
            {
                return Contain(721013, 1);
            }
            if (ID == 1048)
            {
                return Contain(721015, 1);
            }
            if (ID == 1045)
            {
                return Contain(721012, 1);
            }
            if (ID == 1043)
            {
                return Contain(721010, 1);
            }
            if (ID == 1047)
            {
                return Contain(721014, 1);
            }
            return false;
        }
        public bool HaveSpace(byte count)
        {
            return (ClientItems.Count + count) <= File_Size;
        }

        public bool TryGetItem(uint UID, out Game.MsgServer.MsgGameItem item)
        {
            return ClientItems.TryGetValue(UID, out item);
        }
        public bool SearchItemByID(uint ID, out Game.MsgServer.MsgGameItem item)
        {
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    item = msg_item;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool SearchItemByID(uint ID, byte count, out List<Game.MsgServer.MsgGameItem> Items)
        {
            byte increase = 0;
            Items = new List<Game.MsgServer.MsgGameItem>();
            foreach (var msg_item in ClientItems.Values)
            {
                if (msg_item.ITEM_ID == ID)
                {
                    Items.Add(msg_item);
                    increase++;
                    if (increase == count)
                    {
                        return true;
                    }
                }
            }
            Items = null;
            return false;
        }
        public bool Contain(uint ID, uint Amount, byte bound = 0)
        {
            if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == Database.ItemType.Meteor
                        || item.ITEM_ID == Database.ItemType.MeteorTear)
                    {
                        if (item.Bound == bound)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            else if (ID == Database.ItemType.MoonBox || ID == 723087)//execept for bound
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID)
                    {
                        count += item.StackSize;
                        if (count >= Amount)
                            return true;
                    }
                }
            }
            else
            {
                uint count = 0;
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ID)
                    {
                        if (item.Bound == bound)
                        {
                            count += item.StackSize;
                            if (count >= Amount)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool Remove(uint ID, uint count, ServerSockets.Packet stream)
        {
            if (Contain(ID, count) || Contain(ID, count, 1))
            {
                if (ID == Database.ItemType.Meteor || ID == Database.ItemType.MeteorTear)
                {
                    byte removed = 0;
                    for (byte x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == Database.ItemType.Meteor
                         || item.ITEM_ID == Database.ItemType.MeteorTear)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    Console.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                else
                {
                    byte removed = 0;
                    for (byte x = 0; x < count; x++)
                    {
                        foreach (var item in ClientItems.Values)
                        {
                            if (item.ITEM_ID == ID)
                            {
                                try
                                {
                                    Update(item, AddMode.REMOVE, stream);
                                }
                                catch (Exception e)
                                {
                                    Console.SaveException(e);
                                }
                                removed++;
                                if (removed == count)
                                    break;
                            }
                        }
                        if (removed == count)
                            break;
                    }
                }
                return true;
            }
            return false;
        }
        public bool AddSteed(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, bool bound = false, byte ProgresGreen = 0, byte ProgresBlue = 0, byte ProgresRed = 0)
        {
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                for (byte x = 0; x < count; x++)
                {
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;

                        ItemDat.ProgresGreen = ProgresGreen;
                        ItemDat.Enchant = ProgresBlue;
                        ItemDat.Bless = ProgresRed;
                        ItemDat.SocketProgress = (int)(ProgresGreen | (ProgresBlue << 8) | (ProgresRed << 16));
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            Console.SaveException(e);
                        }
                        if (x >= count)
                            return true;
                    }
                }
            }
            return false;
        }
        public bool AddSoul2(uint ID, uint SoulID, uint soullevel, uint souldays, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, Packet stream, bool bound)
        {
            if (SoulID != 0 && !Server.ItemsBase.TryGetValue(SoulID, out var _))
                return false;
            if (!Server.ItemsBase.TryGetValue(ID, out var ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                MsgGameItem ItemDat;
                ItemDat = new MsgGameItem
                {
                    UID = Server.ITEM_Counter.Next,
                    ITEM_ID = ID
                };
                ItemDat.Durability = (ItemDat.MaximDurability = ITEMDB.Durability);
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                if (hp > 0)
                    ItemDat.Enchant = (byte)new Random().Next(200, 255);
                ItemDat.Color = (Flags.Color)ServerKernel.NextAsync(3, 9);
                ItemDat.RemainingTime = ((ITEMDB.StackSize <= 1) ? uint.MaxValue : 0);
                MsgItemExtra.Purification purification;
                purification = default(MsgItemExtra.Purification);
                purification.AddedOn = DateTime.Now;
                purification.ItemUID = ItemDat.UID;
                purification.PurificationLevel = 6;
                purification.PurificationItemID = SoulID;
                MsgItemExtra.Purification purify;
                purify = purification;
                ItemDat.Purification.Typ = MsgItemExtra.Typing.PurificationEffect;
                ItemDat.Purification = purify;
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
                return true;
            }
            return false;
        }
        public bool AddMine(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
, Role.Flags.Gem sockone = Flags.Gem.NoSocket
 , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
, string another_text = "", int days = 0, int hours = 0, int mins = 0, bool mine = false)
        {
            if (ID == 1088000)
            {

            }
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count;)
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {

                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Effect = Effect;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                        // Console.WriteLine(Database.ItemType.GetItemPoints(DbItem, ItemDat));
                        if (SendMessage)
                        {
#if Arabic
                             Owner.CreateBoxDialog("You~received~a~" + DbItem.Name + "" + another_text);
#else
                            Owner.SendSysMesage("You~received~a~" + DbItem.Name + "" + another_text,
                                Game.MsgServer.MsgMessage.ChatMode.TopLeft,
                               Game.MsgServer.MsgMessage.MsgColor.red);

                            //Owner.SendSysMesage("You~received~a~" + DbItem.Name + "" + another_text);

                            //Owner.CreateBoxDialog("You~received~a~" + DbItem.Name + "" + another_text);
#endif

                        }

                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            Console.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }

            return false;
        }
        public bool Add(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0, byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
             , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false, Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
            , string another_text = "", int DaysActive = 0)
        {
            //if (ID == 1088000)
            //{

            //}
            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count;)
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {

                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Effect = Effect;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;
                        if (DaysActive != 0)
                            ItemDat.RemainingTime = (uint)(60/* * 60 * 24 * DaysActive*/);

                        // Console.WriteLine(Database.ItemType.GetItemPoints(DbItem, ItemDat));
                        if (SendMessage)
                        {
                            switch (another_text)
                            {
                                case "~from~mining!":
                                    {
                                        Owner.SendSysMesage("You~received~a~" + DbItem.Name + "" + another_text, MsgMessage.ChatMode.TopLeft);
                                        break;
                                    }
                                default:
                                    Owner.CreateBoxDialog("You~received~a~" + DbItem.Name + "" + another_text);
                                    break;
                            }
                        }

                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            Console.SaveException(e);
                        }

                    }
                }
                if (x >= count)
                    return true;
            }

            return false;
        }
        public bool AddRefinaryItem(uint ID, bool Bound, ServerSockets.Packet stream)
        {
            ID = ID + Database.ItemType.GetNextRefineryItem();
            if (ID == 724348 || ID == 724349)
                ID += 150;
            if (ID == 724449)
                ID = 724445;

            return Add(stream, ID, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, Bound);
        }
        public bool AddItemWitchStack(uint ID, byte Plus, ushort amount, Packet stream, bool bound = false)
        {
            if (Server.ItemsBase.TryGetValue(ID, out var DbItem))
            {
                if (DbItem.StackSize > 0)
                {
                    byte _bound;
                    _bound = 0;
                    if (bound)
                        _bound = 1;
                    foreach (MsgGameItem item in ClientItems.Values)
                    {
                        if (item.ITEM_ID == ID && item.Bound == _bound && item.StackSize + amount <= DbItem.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize += amount;
                            if (bound)
                                item.Bound = 1;
                            item.RemainingTime = ((DbItem.StackSize <= 1) ? uint.MaxValue : 0);
                            item.Send(Owner, stream);
                            return true;
                        }
                    }
                    if (amount > DbItem.StackSize)
                    {
                        if (HaveSpace((byte)((int)amount / (int)DbItem.StackSize)))
                        {
                            while (amount >= DbItem.StackSize)
                            {
                                MsgGameItem ItemDat2;
                                ItemDat2 = new MsgGameItem
                                {
                                    UID = Server.ITEM_Counter.Next,
                                    ITEM_ID = ID
                                };
                                ItemDat2.Durability = (ItemDat2.MaximDurability = DbItem.Durability);
                                ItemDat2.Plus = Plus;
                                ItemDat2.StackSize += DbItem.StackSize;
                                ItemDat2.Color = (Flags.Color)ServerKernel.NextAsync(3, 9);
                                ItemDat2.RemainingTime = ((DbItem.StackSize <= 1) ? uint.MaxValue : 0);
                                if (bound)
                                    ItemDat2.Bound = 1;
                                try
                                {
                                    Update(ItemDat2, AddMode.ADD, stream);
                                }
                                catch (Exception e2)
                                {
                                    ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
                                }
                                amount = (ushort)(amount - DbItem.StackSize);
                            }
                            if (amount > 0 && amount < DbItem.StackSize)
                            {
                                MsgGameItem ItemDat3;
                                ItemDat3 = new MsgGameItem
                                {
                                    UID = Server.ITEM_Counter.Next,
                                    ITEM_ID = ID
                                };
                                ItemDat3.Durability = (ItemDat3.MaximDurability = DbItem.Durability);
                                ItemDat3.Plus = Plus;
                                ItemDat3.StackSize += amount;
                                ItemDat3.RemainingTime = ((DbItem.StackSize <= 1) ? uint.MaxValue : 0);
                                ItemDat3.Color = (Flags.Color)ServerKernel.NextAsync(3, 9);
                                if (bound)
                                    ItemDat3.Bound = 1;
                                try
                                {
                                    Update(ItemDat3, AddMode.ADD, stream);
                                }
                                catch (Exception e3)
                                {
                                    ServerKernel.Log.SaveLog(e3.ToString(), false, LogType.EXCEPTION);
                                }
                            }
                            return true;
                        }
                        while (amount >= DbItem.StackSize)
                        {
                            AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, DbItem.StackSize);
                            amount = (ushort)(amount - DbItem.StackSize);
                        }
                        if (amount > 0 && amount < DbItem.StackSize)
                            AddReturnedItem(stream, ID, 1, Plus, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, bound, Flags.ItemEffect.None, amount);
                        return true;
                    }
                    if (HaveSpace(1))
                    {
                        MsgGameItem ItemDat;
                        ItemDat = new MsgGameItem
                        {
                            UID = Server.ITEM_Counter.Next,
                            ITEM_ID = ID
                        };
                        ItemDat.Durability = (ItemDat.MaximDurability = DbItem.Durability);
                        ItemDat.Plus = Plus;
                        ItemDat.StackSize = amount;
                        ItemDat.Color = (Flags.Color)ServerKernel.NextAsync(3, 9);
                        ItemDat.RemainingTime = ((DbItem.StackSize <= 1) ? uint.MaxValue : 0);
                        if (bound)
                            ItemDat.Bound = 1;
                        try
                        {
                            Update(ItemDat, AddMode.ADD, stream);
                        }
                        catch (Exception e)
                        {
                            ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                        }
                        return true;
                    }
                }
                for (int count = 0; count < amount; count++)
                {
                    Add(ID, Plus, DbItem, stream, bound);
                }
                return true;
            }
            return false;
        }
        public bool ContainItemWithStack(uint UID, ushort Count)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                return ItemDat.StackSize >= Count || Count == 1 && ItemDat.StackSize == 0;
            }
            return false;
        }

        public bool RemoveStackItem(uint UID, ushort Count, ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgGameItem ItemDat;
            if (ClientItems.TryGetValue(UID, out ItemDat))
            {
                if (ItemDat.StackSize > Count)
                {
                    ItemDat.StackSize -= Count;
                    ItemDat.Mode = Flags.ItemMode.Update;
                    ItemDat.Send(Owner, stream);
                }
                else
                {
                    ItemDat.StackSize = 1;
                    Update(ItemDat, AddMode.REMOVE, stream);
                    return true;
                }
            }
            else
            {

                foreach (var item in ClientItems.Values)
                {
                    if (0 == Count)
                        break;
                    if (item.ITEM_ID == UID)
                    {
                        if (item.StackSize > Count)
                        {
                            item.StackSize -= Count;
                            item.Mode = Flags.ItemMode.Update;
                            item.Send(Owner, stream);
                            Count = 0;
                        }
                        else
                        {
                            Count -= item.StackSize;
                            item.StackSize = 1;
                            Update(item, AddMode.REMOVE, stream);
                        }
                    }
                }
            }
            return false;
        }
        public bool AddSoul(uint ID, uint SoulID, byte plus, byte gem1, byte gem2, byte hp, byte daamge, byte times, ServerSockets.Packet stream, bool bound)
        {
            Database.ItemType.DBItem Soul = null;
            Database.ItemType.DBItem ITEMDB = null;
            if (SoulID != 0)
            {
                if (!Database.Server.ItemsBase.TryGetValue((uint)SoulID, out Soul))
                    return false;

            }
            if (!Database.Server.ItemsBase.TryGetValue((uint)ID, out ITEMDB))
                return false;
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = plus;
                ItemDat.SocketOne = (Flags.Gem)gem1;
                ItemDat.SocketTwo = (Flags.Gem)gem2;
                ItemDat.Bless = daamge;
                if (hp > 0)
                {
                    ItemDat.Enchant = (byte)(new System.Random().Next(200, 255));
                }
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                MsgItemExtra.Purification purify = new MsgItemExtra.Purification();
                purify.AddedOn = DateTime.Now;
                purify.ItemUID = ItemDat.UID;
                purify.PurificationLevel = 6;
                purify.PurificationItemID = SoulID;
                ItemDat.Purification.Typ = MsgItemExtra.Typing.PurificationEffect;
                ItemDat.Purification = purify;
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    Console.SaveException(e);
                }
                return true;
            }
            return false;

        }
        public bool ContainUID(uint UID, uint Amount)
        {
            uint count = 0;
            foreach (var item in ClientItems.Values)
            {
                if (item.UID == UID)
                {
                    count += item.StackSize;
                    if (count >= Amount)
                        return true;
                }
            }


            return false;
        }
        public bool RemoveByUID(uint UID, uint count, ServerSockets.Packet stream)
        {
            //if (ContainUID(UID, count))
            if (ContainUID(UID, count))
            {
                byte removed = 0;
                for (byte x = 0; x < count; x++)
                {
                    foreach (var item in ClientItems.Values)
                    {
                        if (item.UID == UID)
                        {
                            try
                            {
                                Update(item, AddMode.REMOVE, stream);
                            }
                            catch (Exception e)
                            {
                                Console.SaveException(e);
                            }

                            removed++;
                            if (removed == count)
                                break;
                        }
                    }

                    if (removed == count)
                        break;
                }


                return true;
            }

            return false;
        }

        public bool isArenaItem(ServerSockets.Packet stream, uint ID, byte count = 1, byte plus = 0, byte bless = 0,
            byte Enchant = 0
            , Role.Flags.Gem sockone = Flags.Gem.NoSocket
            , Role.Flags.Gem socktwo = Flags.Gem.NoSocket, bool bound = false,
            Role.Flags.ItemEffect Effect = Flags.ItemEffect.None, bool SendMessage = false
            , string another_text = "", int DaysActive = 0, uint SecondsActive = 0, bool isArenaItem = false)
        {
            if (ID == 1088000)
            {
            }

            if (count == 0)
                count = 1;
            if (HaveSpace(count))
            {
                byte x = 0;
                for (; x < count;)
                {
                    x++;
                    Database.ItemType.DBItem DbItem;
                    if (Database.Server.ItemsBase.TryGetValue(ID, out DbItem))
                    {
                        Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                        ItemDat.UID = Database.Server.ITEM_Counter.Next;
                        ItemDat.ITEM_ID = ID;
                        ItemDat.Effect = Effect;
                        ItemDat.Durability = ItemDat.MaximDurability = DbItem.Durability;
                        ItemDat.Plus = plus;
                        ItemDat.Bless = bless;
                        ItemDat.Enchant = Enchant;
                        ItemDat.SocketOne = sockone;
                        ItemDat.SocketTwo = socktwo;
                        ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                        ItemDat.Bound = (byte)(bound ? 1 : 0);
                        ItemDat.RemainingTime = (DbItem.StackSize > 1) ? 0 : uint.MaxValue;


                        if (DaysActive != 0)
                            ItemDat.RemainingTime = (uint)(60 * 60 * 24 * DaysActive);
                        if (SecondsActive != 0)
                            ItemDat.RemainingTime = SecondsActive;

                        // TODO User Time To Left As Item Date
                        ItemDat.AddedTime = DateTime.Now.Ticks;


                        //TODO Arena PLus12
                        ItemDat.isArena = isArenaItem;


                        // Console.WriteLine(Database.ItemType.GetItemPoints(DbItem, ItemDat));
                        if (SendMessage)
                        {
                            Owner.CreateBoxDialog("You~received~a~" + DbItem.Name + "" + another_text);
                        }

                        try
                        {
                            if (!Update(ItemDat, AddMode.ADD, stream))
                                return false;
                        }
                        catch (Exception e)
                        {
                            Console.SaveException(e);
                        }
                    }
                }

                if (x >= count)
                    return true;
            }

            return false;
        }

        public bool Add(uint ID, byte Plus, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream, bool bound = false)
        {
            if (ITEMDB.StackSize > 0)
            {
                byte _bound = 0;
                if (bound)
                    _bound = 1;
                foreach (var item in ClientItems.Values)
                {

                    if (item.ITEM_ID == ID && item.Bound == _bound)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            item.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                            if (bound)
                                item.Bound = 1;
                            item.Send(Owner, stream);

                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
                ItemDat.ITEM_ID = ID;
                ItemDat.Durability = ItemDat.MaximDurability = ITEMDB.Durability;
                ItemDat.Plus = Plus;
                ItemDat.Color = (Role.Flags.Color)Program.GetRandom.Next(3, 9);
                ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                if (bound)
                    ItemDat.Bound = 1;
                try
                {
                    Update(ItemDat, AddMode.ADD, stream);
                }
                catch (Exception e)
                {
                    Console.SaveException(e);
                }
                return true;
            }
            return false;

        }
        public bool Add(Game.MsgServer.MsgGameItem ItemDat, Database.ItemType.DBItem ITEMDB, ServerSockets.Packet stream)
        {
            if (ITEMDB.StackSize > 0)
            {
                foreach (var item in ClientItems.Values)
                {
                    if (item.ITEM_ID == ItemDat.ITEM_ID)
                    {
                        if (item.StackSize < ITEMDB.StackSize)
                        {
                            item.Mode = Flags.ItemMode.Update;
                            item.StackSize++;
                            ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                            item.Send(Owner, stream);
                            return true;
                        }
                    }
                }
            }
            if (HaveSpace(1))
            {
                ItemDat.RemainingTime = (ITEMDB.StackSize > 1) ? 0 : uint.MaxValue;
                Update(ItemDat, AddMode.ADD, stream);
                return true;
            }
            return false;

        }
        public bool AddItemWitchStack(Game.MsgServer.MsgGameItem ItemDat, byte amount, ServerSockets.Packet stream)
        {
            Database.ItemType.DBItem DbItem;
            if (Database.Server.ItemsBase.TryGetValue(ItemDat.ITEM_ID, out DbItem))
            {
                for (int count = 0; count < amount; count++)
                    Add(ItemDat, DbItem, stream);
                return true;
            }
            return false;
        }
        public unsafe bool Update(Game.MsgServer.MsgGameItem ItemDat, AddMode mode, ServerSockets.Packet stream, bool Removefull = false)
        {
            //if (ItemDat.ITEM_ID == 1088000)
            //{

            //}
            if (HaveSpace(1) || mode == AddMode.REMOVE)
            {
                string logs = "[Item]" + Owner.Player.Name + " [" + mode + "] [" + ItemDat.UID + "]" + ItemDat.ITEM_ID + " plus [" + ItemDat.Plus + "]";
                //string discordlogs = "";
                //if (Server.ItemsBase.TryGetValue(ItemDat.ITEM_ID, out var DBItem) && !Owner.ProjectManager)
                //{
                //    if(mode == AddMode.ADD)
                //    {
                //         discordlogs = "[Item]" + Owner.Player.Name +" [ PICK ] " + DBItem.Name + " plus [" + ItemDat.Plus + "]" + " 1Soc [" + ItemDat.SocketOne + "]" + " 2Soc [" + ItemDat.SocketTwo + "]" + " Bless [" + ItemDat.Bless + "]";
                //    }else if(mode == AddMode.REMOVE)
                //    {
                //         discordlogs = "[Item]" + Owner.Player.Name + " [ DROP ] " + DBItem.Name + " plus [" + ItemDat.Plus + "]" + " 1Soc [" + ItemDat.SocketOne + "]" + " 2Soc [" + ItemDat.SocketTwo + "]" + " Bless [" + ItemDat.Bless + "]";
                //    }
                //    Program.DiscordDropPick.Enqueue(discordlogs);
                //}
                Database.ServerDatabase.LoginQueue.Enqueue(logs);
                switch (mode)
                {
                    case AddMode.ADD:
                        {
                            CheakUp(ItemDat);
                            if (ItemDat.StackSize == 0)
                                ItemDat.StackSize = 1;
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            //if (Owner.IsConnectedInterServer())
                            //{
                            //    ItemDat.Send(Owner.PipeClient, stream);
                            //}
                            break;
                        }
                    case AddMode.MOVE:
                        {
                            CheakUp(ItemDat);
                            ItemDat.Position = 0;
                            ItemDat.Mode = Flags.ItemMode.AddItem;
                            ItemDat.Send(Owner, stream);
                            break;
                        }
                    case AddMode.REMOVE:
                        {
                            if (ItemDat.StackSize > 1 && ItemDat.Position < 40 && !Removefull)
                            {
                                ItemDat.StackSize -= 1;
                                ItemDat.Mode = Flags.ItemMode.Update;
                                ItemDat.Send(Owner, stream);
                                break;
                            }
                            Game.MsgServer.MsgGameItem item;
                            if (ClientItems.TryRemove(ItemDat.UID, out item))
                            {
                                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveInventory, item.UID, 0, 0, 0, 0, 0));
                            }
                            break;
                        }
                }
                if (ItemDat.ITEM_ID == 750000)
                {
                    Owner.DemonExterminator.ItemUID = ItemDat.UID;
                    if (mode == AddMode.REMOVE)
                        Owner.DemonExterminator.ItemUID = 0;
                }

                return true;

            }
            return false;
        }
        private void CheakUp(Game.MsgServer.MsgGameItem ItemDat)
        {
            if (ItemDat.UID == 0)
                ItemDat.UID = Database.Server.ITEM_Counter.Next;
            if (!ClientItems.TryAdd(ItemDat.UID, ItemDat))
            {
                do
                    ItemDat.UID = Database.Server.ITEM_Counter.Next;
                while
                  (ClientItems.TryAdd(ItemDat.UID, ItemDat) == false);
            }
        }

        public bool CheckMeteors(byte count, bool Removethat, ServerSockets.Packet stream)
        {

            if (Contain(1088001, count))
            {
                if (Removethat)
                    Remove(1088001, count, stream);
                return true;
            }
            else
            {
                byte Counter = 0;
                var RemoveThis = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
                var MyMetscrolls = GetMyMetscrolls();
                var MyMeteors = GetMyMeteors();
                foreach (var GameItem in MyMetscrolls.Values)
                {
                    Counter += 10;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    byte needSpace = (byte)(Counter - count);
                    if (HaveSpace(needSpace))
                    {
                        if (Removethat)
                        {
                            Add(stream, 1088001, needSpace);
                        }
                    }
                    else
                    {
                        Counter -= 10;
                        RemoveThis.Remove(RemoveThis.Values.First().UID);
                        byte needmetsss = (byte)(count - Counter);
                        if (needmetsss <= MyMeteors.Count)
                        {
                            foreach (var GameItem in MyMeteors.Values)
                            {
                                Counter += 1;
                                RemoveThis.Add(GameItem.UID, GameItem);
                                if (Counter >= count)
                                    break;
                            }
                            if (Removethat)
                            {
                                foreach (var GameItem in RemoveThis.Values)
                                    Update(GameItem, AddMode.REMOVE, stream);
                            }
                        }
                        else
                            return false;
                    }
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
                foreach (var GameItem in MyMeteors.Values)
                {
                    Counter += 1;
                    RemoveThis.Add(GameItem.UID, GameItem);
                    if (Counter >= count)
                        break;
                }
                if (Counter >= count)
                {
                    if (Removethat)
                    {
                        foreach (var GameItem in RemoveThis.Values)
                            Update(GameItem, AddMode.REMOVE, stream);
                    }
                    return true;
                }
            }

            return false;
        }
        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMetscrolls()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == 720027)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }
        private Dictionary<uint, Game.MsgServer.MsgGameItem> GetMyMeteors()
        {
            var array = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            foreach (var GameItem in ClientItems.Values)
            {
                if (GameItem.ITEM_ID == Database.ItemType.Meteor || GameItem.ITEM_ID == Database.ItemType.MeteorTear)
                {
                    if (!array.ContainsKey(GameItem.UID))
                        array.Add(GameItem.UID, GameItem);
                }
            }
            return array;
        }

        public void ShowALL(ServerSockets.Packet stream)
        {
            foreach (var msg_item in ClientItems.Values)
            {
                msg_item.Mode = Flags.ItemMode.AddItem;
                msg_item.Send(Owner, stream);
            }
        }
        public void Clear(ServerSockets.Packet stream)
        {
            var dictionary = ClientItems.Values.ToArray();
            foreach (var msg_item in dictionary)
                Update(msg_item, AddMode.REMOVE, stream, true);
        }
    }
}
