using TheChosenProject.Client;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.ServerSockets;
using System.Runtime.InteropServices;
using DevExpress.Utils.Behaviors.Common;
using DevExpress.Utils.Filtering.Internal;
using TheChosenProject.Game.ConquerStructures.MsgAutoHunting;
using System;
using TheChosenProject.Role;



namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static Packet TabInfoCreate(this Packet stream, MsgAutoHunt.MsgTabInfo.Action Mode, uint TabEnemyInvisible, uint TabAllyInvisible)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(TabEnemyInvisible);
            stream.Write(TabAllyInvisible);
            stream.Finalize(1036);
            return stream;
        }
        public static void GetTabInfo(this Packet stream, out MsgAutoHunt.MsgTabInfo.Action Mode, out uint TabEnemyInvisible, out uint TabAllyInvisible)
        {
            Mode = (MsgAutoHunt.MsgTabInfo.Action)stream.ReadUInt32();
            TabEnemyInvisible = stream.ReadUInt32();
            TabAllyInvisible = stream.ReadUInt32();
        }
        public static Packet AutoHuntingCreate(this Packet stream, MsgAutoHunt.Action Mode, uint UID, uint TabEnemyInvisible, uint TabAllyInvisible, ulong EnableAutoHunt, ushort VIPLooted, ushort SummoneGuild, ushort VIPStorage)
        {
            stream.InitWriter();
            stream.Write((uint)Mode);
            stream.Write(UID);
            stream.Write(TabEnemyInvisible);
            stream.Write(TabAllyInvisible);
            stream.Write(0u);
            stream.Write(EnableAutoHunt);
            stream.Write(VIPLooted);
            stream.Write(SummoneGuild);
            stream.Write(VIPStorage);
            stream.Finalize(2067);
            return stream;
        }
        public static void GetAutoHunt(this Packet stream, out MsgAutoHunt.Action Mode, out uint UID, out uint TabEnemyInvisible, out uint TabAllyInvisible, out ulong EnableAutoHunt, out ushort VIPLooted, out ushort SummoneGuild, out ushort VIPStorage)
        {
            Mode = (MsgAutoHunt.Action)stream.ReadUInt32();
            UID = stream.ReadUInt32();
            TabEnemyInvisible = stream.ReadUInt32();
            TabAllyInvisible = stream.ReadUInt32();
            uint unknow;
            unknow = stream.ReadUInt32();
            EnableAutoHunt = stream.ReadUInt64();
            VIPLooted = stream.ReadUInt16();
            SummoneGuild = stream.ReadUInt16();
            VIPStorage = stream.ReadUInt16();
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgAutoHunt
    {
        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct MsgTabInfo
        {
            public enum Action : uint
            {
                Show,
                TabEnemyInvisible,
                TabAllyInvisible
            }

            [Packet(1036)]
            public static void TabInfo(GameClient user, Packet stream)
            {
                stream.GetTabInfo(out var Mode, out var TabEnemyInvisible, out var TabAllyInvisible);
                ulong CP;
                CP = 0;//(ulong)user.Player.AutoHuntingCPS ;
                CP -= (ulong)((double)CP * 0.4);
                switch (Mode)
                {
                    case Action.Show:
                        TabEnemyInvisible = ((!user.Player.TabEnemyInvisible) ? 1u : 6000000u);
                        TabAllyInvisible = ((!user.Player.TabAllyInvisible) ? 1u : 6000000u);
                        user.Send(stream.TabInfoCreate(Action.Show, TabEnemyInvisible, TabAllyInvisible));
                        user.Send(stream.AutoHuntingCreate(MsgAutoHunt.Action.Show, user.Player.UID, 0u, 0u, (CP != 0) ? CP : user.Player.VipLevel, (ushort)(user.Player.VipLevel * 1000), (ushort)(user.Player.VipLevel * 1000), (ushort)((uint)user.Player.VipLevel * 1000)));
                        break;
                    case Action.TabEnemyInvisible:
                        if (user.Player.MyGuild != null && user.Player.AutoHunting != AutoStructures.Mode.Enable)
                            user.Player._DelayedTask(stream, "switching...", 2, TaskButton.TabEnemyInvisible);
                        break;
                    case Action.TabAllyInvisible:
                        if (user.Player.MyGuild != null && user.Player.AutoHunting != AutoStructures.Mode.Enable)
                            user.Player._DelayedTask(stream, "switching...", 2, TaskButton.TabAllyInvisible);
                        break;
                    default:
                        if (user.ProjectManager)
                            Console.WriteLine("UnKnow " + (ushort)2067 + " -> " + Mode);
                        break;
                }
            }
        }

        public enum Action : uint
        {
            EnableAutoHunt = 1u,
            DisableAndClaim,
            VIPLooted,
            SummoneGuild,
            Show,
            VIPStorage = 6

        }

        public enum TaskButton : uint
        {
            Show,
            TabEnemyInvisible,
            TabAllyInvisible,
            EnableAutoHunt,
            SummoneGuild,
            VIPStorage

        }

        [Packet(2067)]
        public static void HandlerAutoHunting(GameClient user, Packet packet)
        {
            packet.GetAutoHunt(out var ActionType, out var UID, out var TabEnemyInvisible, out var TabAllyInvisible, out var _, out var _, out var _, out ushort _);
            ulong CP;
            CP = 0;//(ulong)user.Player.AutoHuntingCPS
            CP -= (ulong)((double)CP * 0.4);
            switch (ActionType)
            {
                case Action.VIPLooted:
                    user.ActiveNpc = 4294967248;
                    NpcHandler.VIPBook(user, packet, 0, "", 0u);
                    break;
                case Action.SummoneGuild:
                    if (user.Player.MyGuild != null && (user.Player.GuildRank == Flags.GuildMemberRank.GuildLeader || user.Player.GuildRank == Flags.GuildMemberRank.LeaderSpouse))
                        user.Player._DelayedTask(packet, "Calling Members...", 2, TaskButton.SummoneGuild);
                    break;
                case Action.EnableAutoHunt:
                    user.CreateBoxDialog("Autohunt is not available on this server!");
                    //if (user.Player.AutoHunting == AutoStructures.Mode.NotActive)
                    //    user.Player._DelayedTask(packet, "Starting...", 2, TaskButton.EnableAutoHunt);
                    break;
                case Action.DisableAndClaim:
                    if (user.Player.AutoHunting == AutoStructures.Mode.Enable)
                    {
                        user.Player.AutoHunting = AutoStructures.Mode.Disable;
                        user.Player.SendString(packet, MsgStringPacket.StringID.Effect, true, "mammon");
                    }
                    break;
                case MsgAutoHunt.Action.VIPStorage:
                    user.ActiveNpc = 199005;
                    NpcHandler.VIPStorageBook(user, packet, (byte)0, "", 0);
                    break;
                case Action.Show:
                    if (user.Player.Level == ServerKernel.MAX_UPLEVEL && user.Player.VipLevel > 0)
                    {
                        uint lev;
                        lev = (uint)(ServerKernel.MAX_UPLEVEL - 1);
                        user.Player.SendUpdate(packet, lev, MsgUpdate.DataType.Level);
                        user.Player.leveldown = true;
                    }
                    user.Send(packet.AutoHuntingCreate((Action)user.Player.UID, UID, TabEnemyInvisible, TabAllyInvisible, (CP != 0) ? CP : user.Player.VipLevel, (ushort)(user.Player.VipLevel * 1000), (ushort)(user.Player.VipLevel * 1000), (ushort)((uint)user.Player.VipLevel * 1000)));
                    break;
                default:
                    if (user.ProjectManager)
                        Console.WriteLine("UnKnow " + (ushort)2067 + " -> " + ActionType);
                    break;
            }
        }
    }
}
