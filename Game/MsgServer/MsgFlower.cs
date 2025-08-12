using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.Database;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{

    public static unsafe partial class MsgBuilder
    {

        public static unsafe void GetFlower(this ServerSockets.Packet stream, out MsgFlower.FlowerAction action
            , out  uint UID, out uint ItemUID, out string SenderName, out string ReceiverName, out uint SendAmount)
        {
            action = (MsgFlower.FlowerAction)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();
            SenderName = stream.ReadCString(16);
            ReceiverName = stream.ReadCString(16);
            SendAmount = stream.ReadUInt32();
        }

        public static unsafe ServerSockets.Packet FlowerCreate(this ServerSockets.Packet stream, MsgFlower.FlowerAction action
            , uint UID, uint ItemUID, string SenderName, string ReceiverName, uint SendAmount, MsgFlower.FlowersType FlowerTyp
            , MsgFlower.FlowerEffect Effect)
        {
            stream.InitWriter();

            stream.Write((uint)action);
            stream.Write(UID);
            stream.Write(ItemUID);
            stream.Write(SenderName, 16);
            stream.Write(ReceiverName, 16);
            stream.Write(SendAmount);
            stream.Write((uint)FlowerTyp);
            stream.Write((uint)Effect);

            stream.Finalize(GamePackets.FlowerPacket);

            return stream;
        }

        public static unsafe ServerSockets.Packet FlowerCreate(this ServerSockets.Packet stream, MsgFlower.FlowerAction action
         , uint UID = 0, uint ItemUID = 0, uint RedRoses = 0, uint RedRoses2day = 0, uint Lilies = 0, uint Lilies2day = 0
            , uint Orchids = 0, uint Orchids2day = 0, uint Tulips = 0, uint Tulips2day = 0
            , uint SendAmount = 0, MsgFlower.FlowersType FlowerTyp = MsgFlower.FlowersType.Rouse
            , MsgFlower.FlowerEffect Effect = MsgFlower.FlowerEffect.None)
        {
            stream.InitWriter();

            stream.Write((uint)1);
            stream.Write(0);
            stream.Write(0);
            stream.Write(RedRoses);
            stream.Write(RedRoses2day);
            stream.Write(Lilies);
            stream.Write(Lilies2day);
            stream.Write(Orchids);
            stream.Write(Orchids2day);
            stream.Write(Tulips);
            stream.Write(Tulips2day);
            stream.Write(Orchids);
            stream.Write((uint)FlowerTyp);
            stream.Write((uint)Effect);

            stream.Finalize(GamePackets.FlowerPacket);

            return stream;
        }
    }
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgFlower
    {
        public enum FlowerAction
        {
            None,
            GirlSend,
            FlowerSender,
            Flower
        }

        public enum FlowersType : uint
        {
            Rouse,
            Lilies,
            Orchids,
            Tulips,
            Kiss,
            love,
            Tins,
            Jade
        }

        public enum FlowerEffect : uint
        {
            None = 0u,
            Rouse = 1u,
            Lilies = 2u,
            Orchids = 3u,
            Tulips = 4u,
            Kiss = 1u,
            love = 2u,
            Tins = 3u,
            Jade = 4u
        }

        [Packet(1150)]
        public static void Handler(GameClient user, Packet packet)
        {
            packet.GetFlower(out var action, out var UID, out var ItemUID, out var _, out var _, out var SendAmount);
            if (Core.IsBoy(user.Player.Body) && action == FlowerAction.None)
            {
                if (ItemUID == 0)
                {
                    if (user.Player.Flowers.FreeFlowers == 0 || !user.Player.View.TryGetValue(UID, out var obj4, MapObjectType.Player))
                        return;
                    Player Target4;
                    Target4 = obj4 as Player;
                    if (Core.IsGirl(Target4.Body))
                    {
                        if (!TheChosenProject.Role.Instance.Flowers.ClientPoll.ContainsKey(Target4.UID))
                            TheChosenProject.Role.Instance.Flowers.ClientPoll.TryAdd(Target4.UID, Target4.Flowers);
                        Target4.Flowers.RedRoses += user.Player.Flowers.FreeFlowers;
                        Target4.View.SendView(packet.FlowerCreate(action, UID, ItemUID, user.Player.Name, Target4.Name, user.Player.Flowers.FreeFlowers, FlowersType.Rouse, FlowerEffect.Rouse), true);
                        user.Player.Flowers.FreeFlowers = 0u;
                    }
                }
                else
                {
                    if (!user.Inventory.TryGetItem(ItemUID, out var GameItem2) || !user.Player.View.TryGetValue(UID, out var obj3, MapObjectType.Player))
                        return;
                    Player Target3;
                    Target3 = obj3 as Player;
                    if (!Core.IsGirl(Target3.Body))
                        return;
                    if (!TheChosenProject.Role.Instance.Flowers.ClientPoll.ContainsKey(Target3.UID))
                        TheChosenProject.Role.Instance.Flowers.ClientPoll.TryAdd(Target3.UID, Target3.Flowers);
                    SendAmount = GameItem2.ITEM_ID % 1000u;
                    if (SendAmount == Server.ItemsBase[GameItem2.ITEM_ID].Durability)
                    {
                        FlowersType FlowerTyp2;
                        FlowerTyp2 = GetFlowerTyp(GameItem2.ITEM_ID);
                        Flowers.Flower Flowers2;
                        Flowers2 = Target3.Flowers.SingleOrDefault((Flowers.Flower p) => p.Type == FlowerTyp2);
                        if (Flowers2 != null)
                        {
                            Flowers2 += SendAmount;
                            Program.GirlsFlowersRanking.UpdateRank(Flowers2, FlowerTyp2);
                            uint FlowersToday;
                            FlowersToday = Target3.Flowers.AllFlowersToday();
                            Program.FlowersRankToday.UpdateRank(Target3.UID, FlowersToday);
                            Target3.View.SendView(packet.FlowerCreate(action, UID, ItemUID, user.Player.Name, Target3.Name, SendAmount, FlowerTyp2, (FlowerEffect)(FlowerTyp2 + 1)), true);
                            user.Inventory.Update(GameItem2, AddMode.REMOVE, packet);
                        }
                    }
                }
            }
            else
            {
                if (!Core.IsGirl(user.Player.Body) || action != FlowerAction.GirlSend)
                    return;
                if (ItemUID == 0)
                {
                    if (user.Player.Flowers.FreeFlowers == 0 || !user.Player.View.TryGetValue(UID, out var obj2, MapObjectType.Player))
                        return;
                    Player Target2;
                    Target2 = obj2 as Player;
                    if (Core.IsBoy(Target2.Body))
                    {
                        if (!TheChosenProject.Role.Instance.Flowers.ClientPoll.ContainsKey(Target2.UID))
                            TheChosenProject.Role.Instance.Flowers.ClientPoll.TryAdd(Target2.UID, Target2.Flowers);
                        Target2.Flowers.RedRoses += user.Player.Flowers.FreeFlowers;
                        Target2.View.SendView(packet.FlowerCreate(action, UID, ItemUID, user.Player.Name, Target2.Name, user.Player.Flowers.FreeFlowers, FlowersType.Kiss, FlowerEffect.Rouse), true);
                        user.Player.Flowers.FreeFlowers = 0u;
                    }
                }
                else
                {
                    if (!user.Inventory.TryGetItem(ItemUID, out var GameItem) || !user.Player.View.TryGetValue(UID, out var obj, MapObjectType.Player))
                        return;
                    Player Target;
                    Target = obj as Player;
                    if (!Core.IsBoy(Target.Body))
                        return;
                    if (!TheChosenProject.Role.Instance.Flowers.ClientPoll.ContainsKey(Target.UID))
                        TheChosenProject.Role.Instance.Flowers.ClientPoll.TryAdd(Target.UID, Target.Flowers);
                    SendAmount = GameItem.ITEM_ID % 1000u;
                    if (SendAmount == Server.ItemsBase[GameItem.ITEM_ID].Durability)
                    {
                        FlowersType FlowerTyp;
                        FlowerTyp = GetFlowerTyp(GameItem.ITEM_ID);
                        Flowers.Flower Flowers;
                        Flowers = Target.Flowers.SingleOrDefault((Flowers.Flower p) => p.Type == FlowerTyp);
                        if (Flowers != null)
                        {
                            Flowers += SendAmount;
                            Program.BoysFlowersRanking.UpdateRank(Flowers, FlowerTyp);
                            Target.View.SendView(packet.FlowerCreate(action, UID, ItemUID, user.Player.Name, Target.Name, SendAmount, FlowerTyp + 4, (FlowerEffect)(FlowerTyp + 1)), true);
                            user.Inventory.Update(GameItem, AddMode.REMOVE, packet);
                        }
                    }
                }
            }
        }

        public static FlowersType GetFlowerTyp(uint ID)
        {
            if ((ID >= 751001 && ID <= 751999) || (ID >= 755001 && ID <= 755999))
                return FlowersType.Rouse;
            if ((ID >= 752001 && ID <= 752999) || (ID >= 756001 && ID <= 756999))
                return FlowersType.Lilies;
            if ((ID >= 753001 && ID <= 753999) || (ID >= 757001 && ID <= 757999))
                return FlowersType.Orchids;
            if ((ID >= 754001 && ID <= 754999) || (ID >= 758001 && ID <= 758999))
                return FlowersType.Tulips;
            return FlowersType.Rouse;
        }
    }
}
