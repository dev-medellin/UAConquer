using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetItemPacketPacket(this ServerSockets.Packet stream, out MsgGameItem item)
        {
            item = new MsgGameItem();
            item.UID = stream.ReadUInt32();
            item.ITEM_ID = stream.ReadUInt32();
            item.Durability = stream.ReadUInt16();
            item.MaximDurability = stream.ReadUInt16();
            item.Mode = (Role.Flags.ItemMode)stream.ReadUInt16();
            item.Position = stream.ReadUInt16();
            item.SocketProgress = (int)stream.ReadUInt32();
            item.SocketOne = (Role.Flags.Gem)stream.ReadUInt8();
            item.SocketTwo = (Role.Flags.Gem)stream.ReadUInt8();
            stream.SeekForward(sizeof(ushort));
            item.Effect = (Role.Flags.ItemEffect)stream.ReadUInt32();
            stream.SeekForward(sizeof(byte));
            item.Plus = stream.ReadUInt8();
            item.Bless = stream.ReadUInt8();
            item.Bound = stream.ReadUInt8();
            item.Enchant = stream.ReadUInt8();
            stream.SeekForward(9);
            item.Locked = stream.ReadUInt8();
            stream.SeekForward(sizeof(byte));
            item.Color = (Role.Flags.Color)stream.ReadUInt32();
            item.PlusProgress = stream.ReadUInt32();
            item.Inscribed = stream.ReadUInt32();
            stream.ReadUInt32();
            stream.ReadUInt32();
            item.StackSize = stream.ReadUInt16();
        }
    }
    public class MsgGameItem
    {
        public MsgItemExtra.Purification Purification;

        public MsgItemExtra.Refinery Refinary;

        public bool SocketProcess = false;

        public bool Failed;

        public string AgateStr;

        public ushort Leng;

        public ushort PacketID;

        public uint UID;

        public uint ITEM_ID;

        public ushort Durability;

        public ushort MaximDurability;

        public Flags.ItemMode Mode;

        public ushort Position;

        public int SocketProgress;

        public uint RemainingTime;
        public bool Fake = false;
        public int IDEvent;

        public Flags.Gem SocketOne;

        public Flags.Gem SocketTwo;

        public ushort padding;

        public Flags.ItemEffect Effect;

        public byte Plus;

        public byte Bless;

        public byte Bound;

        public byte Enchant;

        public uint ProgresGreen;

        public byte Suspicious;

        public byte Locked;

        public Flags.Color Color;

        public uint PlusProgress;

        public uint Inscribed;

        public uint Activate;

        public uint TimeLeftInMinutes;

        public ushort StackSize;

        public ushort UnKnow;

        public uint WH_ID;

        public int UnLockTimer;


        // Custom Mario
        public long AddedTime;
        public bool isRemoved;
        public bool isArena;

        public Dictionary<uint, string> Agate_map { get; set; }

        public bool IsWeapon
        {
            get
            {
                if (ItemType.ItemPosition(ITEM_ID) == 4 || ItemType.ItemPosition(ITEM_ID) == 5)
                    return !ItemType.IsArrow(ITEM_ID);
                return false;
            }
        }

        public bool IsEquip => ItemType.ItemPosition(ITEM_ID) != 0;

        public MsgGameItem()
        {
            Agate_map = new Dictionary<uint, string>(10);
        }

        public Inventory Send(GameClient client, Packet stream)
        {
            if (Mode == Flags.ItemMode.Update)
            {
                string logs;
                logs = "[Item]" + client.Player.Name + " update [" + UID + "]" + ITEM_ID + " plus [" + Plus + "] s1[" + SocketOne.ToString() + "]s2[" + SocketTwo.ToString() + "]";
                ServerKernel.Log.GmLog("item_update", logs);
            }
            //if (MaximDurability == 0 && Server.ItemsBase.TryGetValue(ITEM_ID, out var DBItem))
            //    MaximDurability = DBItem.Durability;
            Database.ItemType.DBItem DBItem;
            if (MaximDurability == 0)
            {
                if (Database.Server.ItemsBase.TryGetValue(ITEM_ID, out DBItem))
                    MaximDurability = DBItem.Durability;
            }
            if (SocketProcess)
            {
                if (!Failed)
                {
                    if (Database.Server.ItemsBase.TryGetValue(ITEM_ID, out DBItem))
                    {
                        MaximDurability = DBItem.Durability;
                        Failed = false;
                    }
                }
                else
                {
                    if (Database.Server.ItemsBase.TryGetValue(ITEM_ID, out DBItem))
                    {
                        Durability = MaximDurability = DBItem.Durability;
                    }
                }
            }
            ushort GetPosition;
            GetPosition = ItemType.ItemPosition(ITEM_ID);
            if (GetPosition == 15 || GetPosition == 16)
            {
                Activate = 1;
                TimeLeftInMinutes = uint.MaxValue;
            }
            switch ((Flags.ConquerItem)GetPosition)
            {
                case Flags.ConquerItem.Head:
                case Flags.ConquerItem.Necklace:
                case Flags.ConquerItem.Armor:
                case Flags.ConquerItem.RightWeapon:
                case Flags.ConquerItem.LeftWeapon:
                case Flags.ConquerItem.Ring:
                case Flags.ConquerItem.Boots:
                case Flags.ConquerItem.AleternanteHead:
                case Flags.ConquerItem.AleternanteNecklace:
                case Flags.ConquerItem.AleternanteArmor:
                case Flags.ConquerItem.AleternanteRightWeapon:
                case Flags.ConquerItem.AleternanteLeftWeapon:
                case Flags.ConquerItem.AleternanteRing:
                case Flags.ConquerItem.AleternanteBoots:
                    if (Bless > ServerKernel.Max_Bless)
                        Bless = ServerKernel.Max_Bless;
                    if (Plus > ServerKernel.Max_PLUS)
                        Plus = ServerKernel.Max_PLUS;
                    if (Enchant > ServerKernel.Max_Enchant)
                        Enchant = ServerKernel.Max_Enchant;
                    break;
                case Flags.ConquerItem.Garment:
                //case Flags.ConquerItem.Steed:
                case Flags.ConquerItem.RightWeaponAccessory:
                case Flags.ConquerItem.LeftWeaponAccessory:
                //case Flags.ConquerItem.RidingCrop:
                case Flags.ConquerItem.AleternanteBottle:
                case Flags.ConquerItem.AleternanteGarment:
                    if (Bless > 1)
                        Bless = 1;
                    if (Plus > ServerKernel.Max_PLUS)
                        Plus = ServerKernel.Max_PLUS;
                    if (SocketOne != 0)
                        SocketOne = Flags.Gem.NoSocket;
                    if (SocketTwo != 0)
                        SocketTwo = Flags.Gem.NoSocket;
                    break;
                //case Flags.ConquerItem.Fan:
                //case Flags.ConquerItem.Tower:
                //    if (Bless > 1)
                //        Bless = 1;
                //    if (Plus > ServerKernel.Max_PLUS)
                //        Plus = ServerKernel.Max_PLUS;
                //    if (Enchant != 0)
                //        Enchant = 0;
                //    break;
                case Flags.ConquerItem.Bottle:
                //case Flags.ConquerItem.SteedMount:
                    if (Bless > 1)
                        Bless = 0;
                    if (Plus != 0)
                        Plus = 0;
                    if (SocketOne != 0)
                        SocketOne = Flags.Gem.NoSocket;
                    if (SocketTwo != 0)
                        SocketTwo = Flags.Gem.NoSocket;
                    if (Enchant != 0)
                        Enchant = 0;
                    break;
            }
            if (Plus > 0 && GetPosition == 0)
                Plus = 0;
            if (ITEM_ID >= 730001 && ITEM_ID <= 730008)
                Plus = (byte)(ITEM_ID % 10);
            client.Send(ItemCreate(stream, this));
            SendItemExtra(client, stream);
            SendItemLocked(client, stream);
            return client.Inventory;
        }

        public void SendItemExtra(GameClient client, Packet stream)
        {
            if (Purification.ItemUID == 0 && Refinary.ItemUID == 0)
                return;
            MsgItemExtra extra;
            extra = new MsgItemExtra();
            if (Purification.InLife)
            {
                if (Purification.SecondsLeft == 0)
                    Purification.Typ = MsgItemExtra.Typing.Stabilization;
                else
                    Purification.Typ = MsgItemExtra.Typing.PurificationAdding;
                extra.Purifications.Add(Purification);
            }
            if (Refinary.InLife)
            {
                Refinary.Typ = MsgItemExtra.Typing.RefinaryAdding;
                if (Refinary.EffectDuration == 0)
                    Refinary.Typ = MsgItemExtra.Typing.PermanentRefinery;
                extra.Refinerys.Add(Refinary);
            }
            client.Send(extra.CreateArray(stream));
        }

        public void SendItemLocked(GameClient client, Packet stream)
        {
            if (Locked == 2)
            {
                if (UnLockTimer == 0)
                {
                    Locked = 0;
                    Mode = Flags.ItemMode.Update;
                    client.Send(ItemCreate(stream, this));
                }
                else if (DateTime.Now > Core.GetTimer(UnLockTimer))
                {
                    Locked = 0;
                    Mode = Flags.ItemMode.Update;
                    client.Send(ItemCreate(stream, this));
                }
                else
                {
                    client.Send(stream.ItemLockCreate(UID, MsgItemLock.TypeLock.UnlockDate, 0, (uint)UnLockTimer));
                }
            }
        }

        public Packet ItemCreate(Packet stream, MsgGameItem item)
        {
            stream.InitWriter();
            stream.Write(item.UID);
            stream.Write(item.ITEM_ID);
            stream.Write(item.Durability);
            stream.Write(item.MaximDurability);
            stream.Write((ushort)item.Mode);
            stream.Write(item.Position);
            stream.Write(item.SocketProgress);
            stream.Write((byte)item.SocketOne);
            stream.Write((byte)item.SocketTwo);
            stream.Write((ushort)0);
            stream.Write((uint)item.Effect);
            stream.Write((byte)0);
            stream.Write(item.Plus);
            stream.Write(item.Bless);
            stream.Write(item.Bound);
            stream.Write(item.Enchant);
            stream.ZeroFill(3);
            stream.Write(item.ProgresGreen);
            stream.Write((ushort)item.Suspicious);
            stream.Write((ushort)item.Locked);
            stream.Write((uint)item.Color);
            stream.Write(item.PlusProgress);
            stream.Write(item.Inscribed);
            stream.Write(RemainingTime);
            stream.Write(item.StackSize);
            stream.Finalize(1008);
            return stream;
        }

        internal void SendAgate(GameClient client)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet str;
                str = rec.GetStream();
                str.InitWriter();
                str.Write(0);
                str.Write(UID);
                str.Write((uint)Agate_map.Count);
                str.Write((ulong)Agate_map.Count);
                str.Write((uint)Durability);
                str.Write((uint)Agate_map.Count);
                if (Agate_map.Count > 0)
                {
                    for (uint i = 0; i < Agate_map.Count; i++)
                    {
                        str.Write(i);
                        str.Write(uint.Parse(Agate_map[i].Split('~')[0].ToString()));
                        str.Write(uint.Parse(Agate_map[i].Split('~')[1].ToString()));
                        str.Write(uint.Parse(Agate_map[i].Split('~')[2].ToString()));
                        str.ZeroFill(32);
                    }
                }
                str.Finalize(2110);
                client.Send(str);
            }
        }
    }
}
