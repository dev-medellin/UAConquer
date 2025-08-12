using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;
using System.IO;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Game.MsgFloorItem
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetItemPacket(this ServerSockets.Packet stream, out uint uid)
        {
         //   uint stamp = stream.ReadUInt32();
            uid = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet ItemPacketCreate(this ServerSockets.Packet stream, MsgItemPacket Item)
        {
            stream.InitWriter();
            stream.Write(Item.m_UID);//8
            stream.Write(Item.m_ID);//12
            stream.Write(Item.m_X);//16
            stream.Write(Item.m_Y);//18
            stream.Write((ushort)Item.m_Color);//Item.m_Color);
            stream.Write((byte)Item.DropType);//22
            stream.Finalize(GamePackets.FloorMap);
            return stream;
        }
    }

    public unsafe class MsgItemPacket
    {
        public enum EffectMonsters : uint
        {
            None = 0,
            EarthquakeLeftRight = 1,
            EarthquakeUpDown = 2,
            Night = 4,
            EarthquakeAndNight = 5
        }

        public const uint
            DBShowerEffect = 17,
        TwilightDance = 40,
    NormalDaggerStorm = 50,
    SoulOneDaggerStorm = 41,
    SoulTwoDaggerStorm = 42,//46
    InfernalEcho = 1001390,
    WrathoftheEmperor = 1001380,
    AuroraLotus = 930,
    FlameLotus = 940,

    RageofWar = 1500,
    ShadowofChaser = 1550,

    HorrorofStomper = 1530,
    PeaceofStomper = 1540,

    Thundercloud = 3843;

        public uint m_UID;
        public uint m_ID;
        public ushort m_X;
        public ushort m_Y;
        public ushort MaxLife;
        public MsgDropID DropType;
        public uint Life;
        public byte m_Color;
        public byte m_Color2;
        public uint ItemOwnerUID;
        public byte DontShow;
        public uint GuildID;
        public byte FlowerType;
        public ulong Timer;
        public string Name;
        public uint UnKnow;
        public byte Plus;



        public ushort OwnerX;
        public ushort OwnerY;

        public static MsgItemPacket Create()
        {
            MsgItemPacket item = new MsgItemPacket();
            return item;
        }

        [PacketAttribute(GamePackets.FloorMap)]
        public static void FloorMap(GameClient client, Packet packet)
        {
            if (client.InTrade)
                return;
            packet.GetItemPacket(out var m_UID);
            if (!client.Map.View.TryGetObject<MsgItem>(m_UID, MapObjectType.Item, client.Player.X, client.Player.Y, out var MapItem))
                return;
            if (MapItem.ToMySelf && !MapItem.ExpireMySelf && MapItem.ItemOwner != client.Player.UID)
            {
                if (client.Team != null)
                {
                    if (MapItem.Typ != MsgItem.ItemType.Money && (!client.Team.IsTeamMember(MapItem.ItemOwner) || !client.Team.PickupItems))
                    {
                        client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                        return;
                    }
                    if (MapItem.Typ == MsgItem.ItemType.Money && !client.Team.PickupMoney)
                    {
                        client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                        return;
                    }
                }
                //else if (client.Team == null)
                //{
                //    _ = MapItem.Typ;
                //    _ = 1;
                //    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                //    return;
                //}
            }
            if (Core.GetDistance(client.Player.X, client.Player.Y, MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y) > 5)
                return;
            switch (MapItem.Typ)
            {
                case MsgItem.ItemType.Money:

                    client.Player.Money += (int)MapItem.Gold;
                    client.Player.SendUpdate(packet, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                    MapItem.SendAll(packet, MsgDropID.Remove);
                    client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~MapFlagType.Item;
                    client.Map.View.LeaveMap((IMapObj)MapItem);
                    client.SendSysMesage($"You have picked up {MapItem.Gold} silvers.");
                    break;
                case MsgItem.ItemType.Item:
                    {
                        if (!client.Inventory.HaveSpace(1) || !Server.ItemsBase.TryGetValue(MapItem.MsgFloor.m_ID, out var DBItem))
                            break;
                        if (DBItem.ID == 727385)
                        {
                            client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~MapFlagType.Item;
                            uint[] ItemTyper2;
                            ItemTyper2 = new uint[36]
                            {
                                151013, 152013, 410003, 420003, 421003, 430003, 440003, 450003, 460003, 480003,
                                481003, 490003, 500003, 601003, 510003, 530003, 560003, 561003, 580003, 900003,
                                130003, 131003, 141030, 133003, 134003, 150003, 142003, 120003, 121003, 111003,
                                160033, 113003, 114003, 117003, 118003, 121003
                            };
                            uint dwItemSort2;
                            dwItemSort2 = ItemTyper2[ServerKernel.NextAsync(0, ItemTyper2.Length)];
                            //uint idItemType2;
                            //idItemType2 = dwItemSort2 * 1000 + 3;
                            client.Inventory.Add(packet, dwItemSort2, 1, 1, 0, 0);
                            string name2;
                            name2 = Server.ItemsBase.GetItemName(dwItemSort2);
                            client.Map.View.LeaveMap((IMapObj)MapItem);
                            MapItem.SendAll(packet, MsgDropID.Remove);
                            client.SendSysMesage("You got a (+1)" + name2 + "! Check your inventory!");
                            if (client.Player.NotifTogggle)
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("A Lucky Player " + client.Player.Name + " has found (+1)" + name2 + " dropped at " + client.Map.Name + " #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(packet));
                            //Program.DiscordSpecialDrop.Enqueue("A Lucky Player " + client.Player.Name + " has found (+1)" + name2 + " dropped at " + client.Map.Name + " (" + MapItem.MsgFloor.m_X + "," + MapItem.MsgFloor.m_Y + ")!");
                        }
                        else if (DBItem.ID == 727384)
                        {
                            break;
                            //client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~MapFlagType.Item;
                            //ushort[] ItemTyper;
                            //ItemTyper = new ushort[33]
                            //{
                            //    410, 420, 421, 430, 440, 450, 460, 480, 481, 490,
                            //    500, 601, 510, 530, 560, 561, 580, 900, 130, 131,
                            //    132, 133, 134, 150, 151, 120, 121, 111, 112, 113,
                            //    114, 117, 118
                            //};
                            //uint dwItemSort;
                            //dwItemSort = ItemTyper[ServerKernel.NextAsync(0, ItemTyper.Length)];
                            //uint idItemType;
                            //idItemType = dwItemSort * 1000 + 3;
                            //client.Inventory.Add(packet, idItemType, 1, 2, 0, 0);
                            //string name;
                            //name = Server.ItemsBase.GetItemName(idItemType);
                            //client.Map.View.LeaveMap((IMapObj)MapItem);
                            //MapItem.SendAll(packet, MsgDropID.Remove);
                            //client.SendSysMesage("You got a (+2)" + name + "! Check your inventory!");
                        }
                        else
                        {
                            client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~MapFlagType.Item;
                            if (MapItem.ItemBase.StackSize > 1)
                                client.Inventory.Update(MapItem.ItemBase, AddMode.ADD, packet);
                            else
                                client.Inventory.Add(MapItem.ItemBase, DBItem, packet);
                            client.Map.View.LeaveMap((IMapObj)MapItem);
                            MapItem.SendAll(packet, MsgDropID.Remove);
                            client.SendSysMesage($"You have got a(an) {DBItem.Name}.");
                        }
                        break;
                    }
                //case MsgItem.ItemType.Cps:
                //    {
                //        if (Server.ItemsBase.TryGetValue(MapItem.MsgFloor.m_ID, out var DBItem2))
                //        {
                //            if (MapItem.ItemBase.ITEM_ID == 720662)
                //            {
                //                int val;
                //                val = Core.Random.Next(1, 15);
                //                int value;
                //                value = val;
                //                client.Player.ConquerPoints += value;
                //                client.SendSysMesage($"You have picked up {val} Conquer Points.");
                //            }
                //            else
                //                client.Inventory.Add(MapItem.ItemBase, DBItem2, packet);
                //            MapItem.SendAll(packet, MsgDropID.Remove);
                //            client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~MapFlagType.Item;
                //            client.Map.View.LeaveMap((IMapObj)MapItem);
                //        }
                //        break;
                //    }
            }
        }
    }
}
