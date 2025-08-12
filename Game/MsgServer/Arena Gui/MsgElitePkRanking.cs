﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ElitePkRankingCreate(this ServerSockets.Packet stream, MsgElitePkRanking.RankType rank
            , uint Group, MsgElitePKBrackets.GuiTyp GroupStatus, uint Count, uint UID)
        {
            stream.InitWriter();

            stream.Write((uint)rank);//rank);//4
            stream.Write(Group);//8
            stream.Write((uint)GroupStatus);//12
            stream.Write(Count);//16
            stream.Write(2);//28
            stream.Write(UID);//20
            stream.Write(0);//24
            stream.Write(0);//28

            return stream;
        }
        public static unsafe void GetElitePkRanking(this ServerSockets.Packet stream, out MsgElitePkRanking.RankType rank
            , out uint Group, out MsgElitePKBrackets.GuiTyp GroupStatus, out uint Count, out uint UID)
        {

            rank = (MsgElitePkRanking.RankType)stream.ReadUInt32();
            Group = stream.ReadUInt32();
            GroupStatus = (MsgElitePKBrackets.GuiTyp)stream.ReadUInt32();
            Count = stream.ReadUInt32();
            stream.ReadUInt32();
            UID = stream.ReadUInt32();
            stream.SeekForward(8);
        }
        public static unsafe void GetItemElitePkRanking(this ServerSockets.Packet stream, out MsgTournaments.MsgEliteGroup.FighterStats status)
        {
            status = new MsgTournaments.MsgEliteGroup.FighterStats();
            status.CrossEliteRank = (uint)stream.ReadUInt32();
            status.Name = stream.ReadCString(16);
            status.Mesh = (uint)stream.ReadUInt32();
            stream.ReadUInt32();
            status.ServerID = stream.ReadUInt32();
            status.UID = stream.ReadUInt32();
            status.RealUID = stream.ReadUInt32();
            status.ClaimReward = (byte)stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet AddItemElitePkRanking(this ServerSockets.Packet stream, MsgTournaments.MsgEliteGroup.FighterStats status, uint Rank)
        {
            
            stream.Write(status.Name, 16);
            stream.Write(status.Mesh);
            stream.Write(0);//abouse for inter server
            stream.Write(status.UID);
            stream.Write(0);
            stream.Write(0);
            return stream;
        }
        public static unsafe ServerSockets.Packet InterServerElitePkRankingFinalize(this ServerSockets.Packet stream)
        {
            stream.ZeroFill(412 - stream.Position);
            //stream.Finalize(MsgInterServer.PacketTypes.InterServer_EliteRank);
            return stream;
        }

        public static unsafe ServerSockets.Packet ElitePkRankingFinalize(this ServerSockets.Packet stream)
        {
            stream.ZeroFill(412 - stream.Position);
            stream.Finalize(GamePackets.EliteRanks);
            return stream;
        }
        public static unsafe void GetElitePkRanking(this ServerSockets.Packet stream, out MsgElitePkRanking.RankType rank, out uint Group)
        {
            rank = (MsgElitePkRanking.RankType)stream.ReadUInt32();
            Group = stream.ReadUInt32();
        }
    }
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgElitePkRanking
    {
        public enum RankType : uint
        {
            Top8 = 0u,
            Top8Cross = 3u,
            Top3 = 2u,
            Top3Cross = 4u
        }

        [Packet(2223)]
        private static void Poroces(GameClient user, Packet stream)
        {
            try
            {
                stream.GetElitePkRanking(out var rank, out var Group);
                if (rank == RankType.Top8Cross)
                {
                    List<MsgEliteGroup.FighterStats> array;
                    array = new List<MsgEliteGroup.FighterStats>();
                    stream.ElitePkRankingCreate(RankType.Top8Cross, Group, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, user.Player.UID);
                    user.Send(stream.ElitePkRankingFinalize());
                    return;
                }
                MsgEliteGroup tournament;
                tournament = MsgEliteTournament.EliteGroups[Math.Min(3, (int)Group)];
                if (tournament.Top8 == null || tournament.Top8.Length == 0 || tournament.Top8[0] == null)
                    return;
                if ((int)tournament.State >= 8)
                {
                    if (tournament.State == MsgElitePKBrackets.GuiTyp.GUI_Top1)
                    {
                        if (tournament.Top8[2] != null)
                        {
                            stream.ElitePkRankingCreate(RankType.Top3, Group, tournament.State, 1, user.Player.UID);
                            stream.AddItemElitePkRanking(tournament.Top8[2], 3);
                            user.Send(stream.ElitePkRankingFinalize());
                        }
                    }
                    else
                    {
                        stream.ElitePkRankingCreate(RankType.Top3, Group, tournament.State, 3, user.Player.UID);
                        for (int j = 0; j < 3; j++)
                        {
                            stream.AddItemElitePkRanking(tournament.Top8[j], (uint)(j + 1));
                        }
                        user.Send(stream.ElitePkRankingFinalize());
                    }
                }
                else
                {
                    stream.ElitePkRankingCreate(RankType.Top8, Group, tournament.State, 8, user.Player.UID);
                    for (int i = 0; i < 8 && tournament.Top8[i] != null; i++)
                    {
                        stream.AddItemElitePkRanking(tournament.Top8[i], (uint)(i + 1));
                    }
                    user.Send(stream.ElitePkRankingFinalize());
                }
                if (tournament.Proces != ProcesType.Dead && (int)tournament.State >= 8)
                {
                    stream.ElitePKBracketsCreate(MsgElitePKBrackets.Action.RequestInformation, 0, 0, (MsgEliteTournament.GroupTyp)Group, tournament.State, 1, 0);
                    user.Send(stream.ElitePKBracketsFinalize());
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
