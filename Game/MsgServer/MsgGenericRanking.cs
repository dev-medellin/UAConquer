using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetGenericRanking(this ServerSockets.Packet stream, out MsgGenericRanking.Action Mode, out MsgGenericRanking.RankType ranktyp
            , out ushort RegisteredCount, out ushort Page, out int Count)
        {
            Mode = (MsgGenericRanking.Action)stream.ReadUInt32();
            ranktyp = (MsgGenericRanking.RankType)stream.ReadUInt32();

            //   RegisteredCount = stream.ReadUInt16();
            RegisteredCount = 0;
            Page = stream.ReadUInt16();

            Count = stream.ReadInt32();
        }
        public static unsafe ServerSockets.Packet GenericRankingCreate(this ServerSockets.Packet stream, MsgGenericRanking.Action Mode, MsgGenericRanking.RankType ranktyp
            , ushort RegisteredCount, ushort Page, int Count)
        {
            stream.InitWriter();

            stream.Write((uint)Mode);
            stream.Write((uint)ranktyp);
            stream.Write(RegisteredCount);
            stream.Write(Page);
            stream.Write(Count);
            stream.Write((uint)0);//unknow

            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGenericRankingCreate(this ServerSockets.Packet stream, int Rank, uint Amount, uint UID, string name)
        {
            stream.Write((long)Rank);
           // stream.Write(Rank);
            stream.Write((long)Amount);
            //stream.Write(Amount);
            stream.Write(UID);
            stream.Write(UID);
            stream.Write(name, 16);
            stream.Write(name, 16);
            //stream.Write(0);
            //stream.Write(0);
            //stream.Write(0);
            //stream.Write(0);
            //stream.Write((long)(0));
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGenericRankingCreate(this ServerSockets.Packet stream, int Rank, uint Amount, uint UID, string name
           , uint Level , uint Class, uint Mesh)
        {
            stream.Write((long)Rank);
            stream.Write((long)Amount);
            stream.Write(UID);
            stream.Write(UID);
            stream.Write(name, 16);
            stream.Write(name, 16);
            stream.Write(Level);
            stream.Write(Class);
            stream.Write(Mesh);
            stream.Write(0);
            stream.Write((long)(0));
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemGenericRankingCreate(this ServerSockets.Packet stream, int Rank, uint Amount, uint UID1, uint UID2, string name
          , uint Level, uint Class, uint Mesh)
        {
            stream.Write((long)Rank);
            stream.Write((long)Amount);
            stream.Write(UID1);
            stream.Write(UID2);
            stream.Write(name, 16);
            stream.Write(name, 16);
            stream.Write(Level);
            stream.Write(Class);
            stream.Write(Mesh);
            stream.Write(0);
            stream.Write((long)(0));
            return stream;
        }
        public static unsafe ServerSockets.Packet GenericRankingFinalize(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.GenericRanking);
            return stream;
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct MsgGenericRanking
    {
        public enum Action : uint
        {
            Ranking = 1u,
            QueryCount = 2u,
            UpdateScreen = 4u,
            InformationRequest = 5u,
            PrestigeRanks = 6u
        }

        public enum RankType : uint
        {
            None = 0u,
            RoseFairy = 30000002u,
            LilyFairy = 30000102u,
            OrchidFairy = 30000202u,
            TulipFairy = 30000302u,
            Kiss = 30000402u,
            Love = 30000502u,
            Tins = 30000602u,
            Jade = 30000702u,
            Chi = 60000000u,
            DragonChi = 60000001u,
            PhoenixChi = 60000002u,
            TigerChi = 60000003u,
            TurtleChi = 60000004u,
            InnerPower = 70000000u,
            PrestigeRank = 80000000u,
            TopTrojans = 80000001u,
            TopWarriors = 80000002u,
            TopArchers = 80000003u,
            TopNinjas = 80000004u,
            TopMonks = 80000005u,
            TopPirates = 80000006u,
            TopDraonWarriors = 80000007u,
            TopWaters = 80000008u,
            TopFires = 80000009u,
            TopWindWalker = 80000010u
        }

        public static object SynRoot = new object();

        [Packet(1151)]
        private static void Process(GameClient user, Packet stream)
        {
            try
            {
                stream.GetGenericRanking(out var Mode, out var ranktyp, out var RegisteredCount, out var Page, out var Count);
                switch (Mode)
                {
                    case Action.QueryCount:
                        lock (SynRoot)
                        {
                            if (Core.IsGirl(user.Player.Body))
                                user.Player.Flowers.UpdateMyRank(user);
                            else
                            {
                                if (!Core.IsBoy(user.Player.Body))
                                    break;
                                foreach (Flowers.Flower Flower in user.Player.Flowers)
                                {
                                    if (Flower.Rank > 0 && Flower.Rank <= 100)
                                    {
                                        stream.GenericRankingCreate(Action.QueryCount, (RankType)user.Player.Flowers.CreateBoyIcon(Flower), RegisteredCount, Page, 1);
                                        for (byte x3 = 0; x3 < Count; x3 = (byte)(x3 + 1))
                                        {
                                            stream.AddItemGenericRankingCreate(Flower.Rank, Flower.Amount, Flower.UID, Flower.Name);
                                        }
                                        user.Send(stream.GenericRankingFinalize());
                                    }
                                }
                                stream.GenericRankingCreate(Action.InformationRequest, ranktyp, RegisteredCount, Page, 0);
                                stream.AddItemGenericRankingCreate(0, 0, 0, "");
                                user.Send(stream.GenericRankingFinalize());
                            }
                            break;
                        }
                    case Action.Ranking:
                        {
                            RankType OldRank;
                            OldRank = ranktyp;
                            if (ranktyp >= RankType.RoseFairy && ranktyp <= RankType.TulipFairy)
                            {
                                Flowers.Flower[] Powers2;
                                Powers2 = null;
                                switch (ranktyp)
                                {
                                    case RankType.RoseFairy:
                                        Powers2 = Program.GirlsFlowersRanking.RedRoses.Values.ToArray();
                                        break;
                                    case RankType.OrchidFairy:
                                        Powers2 = Program.GirlsFlowersRanking.Orchids.Values.ToArray();
                                        break;
                                    case RankType.LilyFairy:
                                        Powers2 = Program.GirlsFlowersRanking.Lilies.Values.ToArray();
                                        break;
                                    case RankType.TulipFairy:
                                        Powers2 = Program.GirlsFlowersRanking.Tulips.Values.ToArray();
                                        break;
                                }
                                if (Powers2 != null)
                                {
                                    int offset2;
                                    offset2 = Page * 10;
                                    int count2;
                                    count2 = Math.Min(10, Powers2.Length);
                                    stream.GenericRankingCreate(Action.Ranking, ranktyp, 100, Page, count2);
                                    byte x2;
                                    x2 = 0;
                                    while (x2 < count2 && x2 + offset2 < Powers2.Length)
                                    {
                                        Flowers.Flower entity2;
                                        entity2 = Powers2[x2 + offset2];
                                        stream.AddItemGenericRankingCreate(entity2.Rank, entity2.Amount, entity2.UID, entity2.Name);
                                        x2 = (byte)(x2 + 1);
                                    }
                                    user.Send(stream.GenericRankingFinalize());
                                }
                            }
                            else
                            {
                                if (ranktyp < RankType.Kiss || ranktyp > RankType.Jade)
                                    break;
                                Flowers.Flower[] Powers;
                                Powers = null;
                                switch (ranktyp)
                                {
                                    case RankType.Kiss:
                                        Powers = Program.BoysFlowersRanking.RedRoses.Values.ToArray();
                                        break;
                                    case RankType.Tins:
                                        Powers = Program.BoysFlowersRanking.Orchids.Values.ToArray();
                                        break;
                                    case RankType.Love:
                                        Powers = Program.BoysFlowersRanking.Lilies.Values.ToArray();
                                        break;
                                    case RankType.Jade:
                                        Powers = Program.BoysFlowersRanking.Tulips.Values.ToArray();
                                        break;
                                }
                                if (Powers != null)
                                {
                                    int offset;
                                    offset = Page * 10;
                                    int count;
                                    count = Math.Min(10, Powers.Length);
                                    stream.GenericRankingCreate(Action.Ranking, ranktyp, 100, Page, count);
                                    byte x;
                                    x = 0;
                                    while (x < count && x + offset < Powers.Length)
                                    {
                                        Flowers.Flower entity;
                                        entity = Powers[x + offset];
                                        stream.AddItemGenericRankingCreate(entity.Rank, entity.Amount, entity.UID, entity.Name);
                                        x = (byte)(x + 1);
                                    }
                                    user.Send(stream.GenericRankingFinalize());
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
    }
}
