﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TheChosenProject.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ArenaWatchersCreate(this ServerSockets.Packet stream, MsgArenaWatchers.WatcherTyp Mode
            , uint dwParam, uint UID, uint Count, uint Player1Cheers, uint Player2Cheers)
        {
            stream.InitWriter();

            stream.Write((ushort)Mode);
            stream.Write(dwParam);
            stream.Write(UID);
            stream.Write(Count);
            stream.Write(Player1Cheers);
            stream.Write(Player2Cheers);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemArenaWatchers(this ServerSockets.Packet stream, MsgTournaments.MsgTeamArena.User user)
        {
            stream.Write(user.Mesh);
            stream.Write(user.Name, 16);
            stream.Write(user.UID);
            stream.Write((uint)user.Level);
            stream.Write((uint)user.Class);
            stream.Write((uint)user.Info.TodayRank);

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemArenaWatchers(this ServerSockets.Packet stream, MsgTournaments.MsgArena.User user)
        {
            stream.Write(user.Mesh);
            stream.Write(user.Name, 16);
            stream.Write(user.UID);
            stream.Write((uint)user.Level);
            stream.Write((uint)user.Class);
            stream.Write((uint)user.Info.TodayRank);

            return stream;
        }
        public static unsafe ServerSockets.Packet ArenaWatchersFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.MsgArenaWatchers);
            return stream;
        }
        public static unsafe void GetArenaWatchers(this ServerSockets.Packet stream, out MsgArenaWatchers.WatcherTyp Mode, out uint UID)
        {
            Mode = (MsgArenaWatchers.WatcherTyp)stream.ReadUInt16();
            uint unknow = stream.ReadUInt32();
            UID = stream.ReadUInt32();
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MsgArenaWatchers
    {
        public enum WatcherTyp : ushort
        {
            RequestView = 0,
            QuitButton = 1,
            Watchers = 2,
            Leave = 3,
            Fighters = 4
        }
        [PacketAttribute(GamePackets.MsgArenaWatchers)]
        private static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            MsgArenaWatchers.WatcherTyp Mode; uint UID;
            stream.GetArenaWatchers(out Mode, out UID);

            switch (Mode)
            {
                case WatcherTyp.RequestView:
                    {
                       // if (!MsgTournaments.MsgSchedules.Arena.BlockArenaMaps.Contains(user.Player.Map))
                        {
                            /*if (user.Player.BattlePower < 200)
                            {
                                user.SendSysMesage("Only the heroes with 200+ BP can watch the fight.", MsgMessage.ChatMode.System);
                                break;
                            }*/
                            Client.GameClient target;
                            if (Database.Server.GamePoll.TryGetValue(UID, out target))
                            {
                                if (target.InQualifier())
                                {
                                    if (target.ArenaMatch != null)
                                    {
                                        target.ArenaMatch.BeginWatching(stream, user);
                                    }
                                    else if (target.Team != null && target.Team.TeamArenaMatch != null)
                                    {
                                        target.Team.TeamArenaMatch.BeginWatching(stream, user);
                                    }
                                    else if (target.ElitePkMatch != null)
                                        target.ElitePkMatch.BeginWatching(stream, user);
                                    if (target.Team != null && target.Team.PkMatch != null)
                                    {
                                        target.Team.PkMatch.BeginWatching(stream, user);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case WatcherTyp.QuitButton:
                    {
                        if (user.IsWatching())
                        {
                            if (user.ArenaWatchingGroup != null)
                                user.ArenaWatchingGroup.DoLeaveWatching(user);
                            else if (user.TeamArenaWatchingGroup != null)
                                user.TeamArenaWatchingGroup.DoLeaveWatching(user);
                            else if (user.ElitePkWatchingGroup != null)
                                user.ElitePkWatchingGroup.DoLeaveWatching(user);
                            else if (user.TeamElitePkWatchingGroup != null)
                                user.TeamElitePkWatchingGroup.DoLeaveWatching(user);
                               
                        }

                        break;
                    }
                case WatcherTyp.Fighters:
                    {
                        if (user.IsWatching())
                        {
                            if (user.ArenaWatchingGroup != null)
                                user.ArenaWatchingGroup.DoCheer(stream, user, UID);
                            else if (user.TeamArenaWatchingGroup != null)
                                user.TeamArenaWatchingGroup.DoCheer(stream, user, UID);
                            else if (user.ElitePkWatchingGroup != null)
                                user.ElitePkWatchingGroup.DoCheer(user, UID);
                            else if (user.TeamElitePkWatchingGroup != null)
                                user.TeamElitePkWatchingGroup.DoCheer(user, UID);
                        }
                        break;
                    }
            }
        }
    }
}
