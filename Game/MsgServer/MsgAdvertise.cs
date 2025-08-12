using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet SendWarehouseInfo(this ServerSockets.Packet stream, uint NPCID)
        {
            stream.InitWriter();
            stream.Write(NPCID);
            stream.Finalize(4000);
            return stream;
        }
        public static unsafe ServerSockets.Packet SendCloseWarehouseInfo(this ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Finalize(4001);
            return stream;
        }
        public static unsafe ServerSockets.Packet CreatePlayerInfo(this ServerSockets.Packet stream, uint UID, string PlayerName)
        {
            stream.InitWriter();
            stream.Write(UID);
            stream.Write(PlayerName, 32);
            stream.Finalize((ushort)GamePackets.MsgPlayerInfo);
            return stream;
        }
        public static unsafe ServerSockets.Packet CloseClient(this ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Finalize(GamePackets.MsgCloseGame);
            return stream;
        }
        public static unsafe ServerSockets.Packet LoginGameCallBack(this ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Finalize(GamePackets.MsgLoginGameCallBack);
            return stream;
        }
        public static unsafe void GetAdvertise(this ServerSockets.Packet stream,out int AtCount)
        {
            AtCount = stream.ReadInt32();
        }
        public static unsafe ServerSockets.Packet AdvertiseCreate(this ServerSockets.Packet stream, int AtCount, int count, int AllRegistred, int PacketNo)
        {
            stream.InitWriter();

            stream.Write(AtCount);
            stream.Write(count);
            stream.Write(AllRegistred);
            stream.Write(PacketNo);
            stream.Write(0);//unknow;

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemAdvertise(this ServerSockets.Packet stream, Role.Instance.Guild Guild)
        {
            stream.Write(Guild.Info.GuildID);

            if (Guild.AdvertiseRecruit.Buletin != null)
            {
                stream.Write(Guild.AdvertiseRecruit.Buletin, 255);
            }
            else
            {
                stream.ZeroFill(255);
            }
            if (Guild.GuildName != null)
            {
                stream.Write(Guild.GuildName, 36);
            }
            else
            {
                stream.ZeroFill(36);
            }
            if (Guild.Info.LeaderName != null)
            {
                stream.Write(Guild.Info.LeaderName, 17);
            }
            else
            {
                stream.ZeroFill(17);
            }
            stream.Write((uint)Guild.Info.Level);
            stream.Write((uint)Guild.Info.MembersCount);
            stream.Write((long)Guild.Info.SilverFund);
            stream.Write((ushort)(Guild.AdvertiseRecruit.AutoJoin ? 1 : 0));
            stream.Write((ushort)Guild.AdvertiseRecruit.NotAllowFlag);
            stream.ZeroFill(12);

            return stream;
        }
        public static unsafe ServerSockets.Packet AdvertiseFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.Advertise);

            return stream;
        }
    }
    public class MsgAdvertise
    {
        [Packet(4002)]
        private static unsafe void NewLoginHandler(Client.GameClient client, ServerSockets.Packet stream)
        {
            var action = new Game.MsgServer.ActionQuery()
            {
                ObjId = client.Player.UID,
                Type = ActionType.HideGui,//158 if you don't have it add it in the same place
                dwParam = DialogCommands.Warehouse,
            };
            client.Send(stream.ActionCreate(&action));
            client.Send(stream.SendCloseWarehouseInfo());
            client.CreateBoxDialog("You Need To Press Alt Button To Use This System");
            client.SendSysMesage("You Need To Press Alt Button To Use This System", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false);
        }
        [Packet(2226)]
        private static void Process(GameClient user, Packet stream)
        {
            _ = Guild.Advertise.AdvertiseRanks.Length;
            stream.GetAdvertise(out var Receive_count);
            if (Receive_count != 0 && Receive_count % 4 != 0)
                return;
            List<Guild> AdvGuilds;
            AdvGuilds = new List<Guild>();
            for (ushort x5 = 0; x5 < 4; x5 = (ushort)(x5 + 1))
            {
                ushort getposition;
                getposition = (ushort)(Receive_count + x5);
                if (Guild.Advertise.AdvertiseRanks.Length <= getposition)
                    break;
                AdvGuilds.Add(Guild.Advertise.AdvertiseRanks[getposition]);
            }
            if (AdvGuilds.Count <= 2)
            {
                stream.AdvertiseCreate(Receive_count, AdvGuilds.Count, Guild.Advertise.AdvertiseRanks.Length, 1);
                for (ushort x4 = 0; x4 < AdvGuilds.Count; x4 = (ushort)(x4 + 1))
                {
                    Guild element4;
                    element4 = AdvGuilds[x4];
                    stream.AddItemAdvertise(element4);
                }
                user.Send(stream.AdvertiseFinalize());
            }
            else if (AdvGuilds.Count == 3)
            {
                stream.AdvertiseCreate(Receive_count, 2, Guild.Advertise.AdvertiseRanks.Length, 1);
                for (ushort x3 = 0; x3 < 2; x3 = (ushort)(x3 + 1))
                {
                    Guild element3;
                    element3 = AdvGuilds[x3];
                    stream.AddItemAdvertise(element3);
                }
                user.Send(stream.AdvertiseFinalize());
                stream.AdvertiseCreate(Receive_count, 1, Guild.Advertise.AdvertiseRanks.Length, 0);
                stream.AddItemAdvertise(AdvGuilds.Last());
                user.Send(stream.AdvertiseFinalize());
            }
            else if (AdvGuilds.Count == 4)
            {
                stream.AdvertiseCreate(Receive_count, 2, Guild.Advertise.AdvertiseRanks.Length, 1);
                for (ushort x2 = 0; x2 < 2; x2 = (ushort)(x2 + 1))
                {
                    Guild element;
                    element = AdvGuilds[x2];
                    stream.AddItemAdvertise(element);
                }
                user.Send(stream.AdvertiseFinalize());
                stream.AdvertiseCreate(Receive_count, 2, Guild.Advertise.AdvertiseRanks.Length, 0);
                for (ushort x = 2; x < 4; x = (ushort)(x + 1))
                {
                    Guild element2;
                    element2 = AdvGuilds[x];
                    stream.AddItemAdvertise(element2);
                }
                user.Send(stream.AdvertiseFinalize());
            }
        }
    }
}
