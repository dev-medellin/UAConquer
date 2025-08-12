using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;

namespace TheChosenProject.Role.Instance
{
    public class Flowers : IEnumerable<Flowers.Flower>, IEnumerable
    {
        public class FlowersRankingToday
        {
            public const int File_Size = 30;

            public Dictionary<uint, uint> Flowers;

            public FlowersRankingToday()
            {
                Flowers = new Dictionary<uint, uint>();
            }

            public void UpdateRank(uint UID, uint Amount)
            {
                if (Flowers.Count < 30)
                {
                    Calculate(UID, Amount);
                    return;
                }
                uint[] array;
                array = Flowers.Values.ToArray();
                if (array[29] <= Amount)
                    Calculate(UID, Amount);
                else if (Flowers.ContainsKey(UID))
                {
                    Calculate(UID, Amount);
                }
            }

            public void Calculate(uint UID, uint Amount)
            {
                lock (Flowers)
                {
                    if (!Flowers.ContainsKey(UID))
                        Flowers.Add(UID, Amount);
                    uint[] clients;
                    clients = Flowers.Values.ToArray();
                    KeyValuePair<uint, uint>[] array;
                    array = Flowers.OrderByDescending((KeyValuePair<uint, uint> p) => p.Value).ToArray();
                    int Rank;
                    Rank = 1;
                    Flowers.Clear();
                    KeyValuePair<uint, uint>[] array2;
                    array2 = array;
                    for (int i = 0; i < array2.Length; i++)
                    {
                        KeyValuePair<uint, uint> user_power;
                        user_power = array2[i];
                        if (Rank <= 30 && Flowers.Count < 30)
                        {
                            if (!Flowers.ContainsKey(user_power.Key))
                                Flowers.Add(user_power.Key, user_power.Value);
                            Rank++;
                            continue;
                        }
                        break;
                    }
                }
            }
        }

        public class FlowerRanking
        {
            public const int File_Size = 100;

            public bool Girls = true;

            public object SynRoot;

            public Dictionary<uint, Flower> RedRoses;

            public Dictionary<uint, Flower> Lilies;

            public Dictionary<uint, Flower> Orchids;

            public Dictionary<uint, Flower> Tulips;

            public FlowerRanking(bool _Girls = true)
            {
                Girls = _Girls;
                SynRoot = new object();
                RedRoses = new Dictionary<uint, Flower>(100);
                Lilies = new Dictionary<uint, Flower>(100);
                Orchids = new Dictionary<uint, Flower>(100);
                Tulips = new Dictionary<uint, Flower>(100);
            }

            public void UpdateRank(Flower AllFlower, MsgFlower.FlowersType Typ)
            {
                lock (SynRoot)
                {
                    switch (Typ)
                    {
                        case MsgFlower.FlowersType.Rouse:
                            CreateRank(RedRoses, AllFlower);
                            break;
                        case MsgFlower.FlowersType.Lilies:
                            CreateRank(Lilies, AllFlower);
                            break;
                        case MsgFlower.FlowersType.Orchids:
                            CreateRank(Orchids, AllFlower);
                            break;
                        case MsgFlower.FlowersType.Tulips:
                            CreateRank(Tulips, AllFlower);
                            break;
                    }
                }
            }

            public void CreateRank(Dictionary<uint, Flower> AllRank, Flower AllFlower)
            {
                if (AllRank.Count < 100)
                {
                    Calculate(AllRank, AllFlower);
                    return;
                }
                Flower[] array;
                array = AllRank.Values.ToArray();
                if (array[99].Amount <= AllFlower.Amount)
                    Calculate(AllRank, AllFlower);
                else if (AllRank.ContainsKey(AllFlower.UID))
                {
                    Calculate(AllRank, AllFlower);
                }
            }

            public void Calculate(Dictionary<uint, Flower> FlowerRank, Flower Flower)
            {
                lock (FlowerRank)
                {
                    if (!FlowerRank.ContainsKey(Flower.UID))
                        FlowerRank.Add(Flower.UID, Flower);
                    Flower[] clients;
                    clients = FlowerRank.Values.ToArray();
                    Flower[] array;
                    array = clients.OrderByDescending((Flower p) => p.Amount).ToArray();
                    int Rank;
                    Rank = 1;
                    FlowerRank.Clear();
                    Flower[] array2;
                    array2 = array;
                    foreach (Flower user_power in array2)
                    {
                        int OldRank;
                        OldRank = user_power.Rank;
                        user_power.Rank = Rank;
                        if (user_power.UID == Flower.UID)
                            Flower.Rank = Rank;
                        if (Rank <= 100 && FlowerRank.Count < 100)
                        {
                            if (!FlowerRank.ContainsKey(user_power.UID))
                                FlowerRank.Add(user_power.UID, user_power);
                        }
                        else
                        {
                            Rank = 0;
                            Flower.Rank = 0;
                            user_power.Rank = 0;
                        }
                        if (OldRank != user_power.Rank && Girls)
                            UpdateRank(Flower.UID, Flower);
                        Rank++;
                    }
                }
            }

            public void UpdateRank(uint UID, Flower Flower)
            {
                if (Server.GamePoll.TryGetValue(UID, out var user))
                    user.Player.Flowers.UpdateMyRank(user, true);
            }
        }

        public class Flower
        {
            public MsgFlower.FlowersType Type;

            public uint Amount;

            public uint Amount2day;

            public int Rank;

            public string Name;

            public uint UID;

            public Flower(MsgFlower.FlowersType typ, uint _uid, string _name)
            {
                UID = _uid;
                Name = _name;
                Type = typ;
            }

            public byte GetGrade()
            {
                switch (Type)
                {
                    case MsgFlower.FlowersType.Rouse:
                        return 1;
                    case MsgFlower.FlowersType.Tulips:
                        return 2;
                    case MsgFlower.FlowersType.Orchids:
                        return 3;
                    case MsgFlower.FlowersType.Lilies:
                        return 4;
                    default:
                        return 0;
                }
            }

            public static implicit operator uint(Flower Data)
            {
                return Data.Amount;
            }

            public static Flower operator +(Flower Data, uint amount)
            {
                Data.Amount += amount;
                Data.Amount2day += amount;
                return Data;
            }
        }

        public uint FreeFlowers;

        public int Day;

        public Flower RedRoses;

        public Flower Lilies;

        public Flower Orchids;

        public Flower Tulips;

        public static ConcurrentDictionary<uint, Flowers> ClientPoll = new ConcurrentDictionary<uint, Flowers>();

        public uint CreateFlowerIcon(Flower flow, bool Today = false, byte Rank = 0)
        {
            uint ID;
            ID = (Today ? 30000001u : 30000002u);
            if (flow == null)
                return ID + GetRank(Rank);
            if (flow.Rank == 0 || flow.Rank > 100)
                return 0u;
            return (uint)((int)ID + 100 * (byte)flow.Type + GetRank(flow.Rank));
        }

        public uint CreateBoyIcon(Flower flow)
        {
            return (uint)(30000402 + 100 * (byte)flow.Type + GetRank(flow.Rank));
        }

        public ushort GetRank(int rank)
        {
            if (rank == 1)
                return 0;
            if (rank == 2)
                return 10000;
            if (rank == 3)
                return 20000;
            if (rank > 3)
                return 30000;
            return 0;
        }

        public Flowers(uint UID, string Name)
        {
            FreeFlowers = 1u;
            RedRoses = new Flower(MsgFlower.FlowersType.Rouse, UID, Name);
            Lilies = new Flower(MsgFlower.FlowersType.Lilies, UID, Name);
            Orchids = new Flower(MsgFlower.FlowersType.Orchids, UID, Name);
            Tulips = new Flower(MsgFlower.FlowersType.Tulips, UID, Name);
        }

        public uint AllFlowersToday()
        {
            return (uint)this.Sum((Flower x) => x.Amount2day);
        }

        public void UpdateMyRank(GameClient user, bool ReloadScreen = false)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                stream.GenericRankingCreate(MsgGenericRanking.Action.InformationRequest, MsgGenericRanking.RankType.None, 0, 0, 1);
                user.Send(stream.GenericRankingFinalize());
                Flower[] array;
                array = this.Where((Flower f1) => f1.Rank >= 1 && f1.Rank <= 100).ToArray();
                Array.Sort(array, delegate (Flower f1, Flower f2)
                {
                    int num;
                    num = f1.Rank.CompareTo(f2.Rank);
                    return (f2.Rank == f1.Rank) ? f2.GetGrade().CompareTo(f1.GetGrade()) : num;
                });
                if (array.Length != 0)
                {
                    user.Player.FlowerRank = CreateFlowerIcon(array[0], false, 0);
                    Flower[] array2;
                    array2 = array;
                    foreach (Flower Flowers in array2)
                    {
                        stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, (MsgGenericRanking.RankType)user.Player.Flowers.CreateFlowerIcon(Flowers, false, 0), 0, 0, 1);
                        stream.AddItemGenericRankingCreate(Flowers.Rank, Flowers.Amount, Flowers.UID, Flowers.Name);
                        user.Send(stream.GenericRankingFinalize());
                    }
                }
                else if (Program.FlowersRankToday.Flowers.ContainsKey(user.Player.UID))
                {
                    Dictionary<uint, uint> RankToday;
                    RankToday = Program.FlowersRankToday.Flowers;
                    int Rank;
                    Rank = 1;
                    foreach (KeyValuePair<uint, uint> FToday in RankToday)
                    {
                        if (FToday.Key == user.Player.UID)
                        {
                            user.Player.FlowerRank = CreateFlowerIcon(null, true, (byte)Rank);
                            stream.GenericRankingCreate(MsgGenericRanking.Action.QueryCount, (MsgGenericRanking.RankType)CreateFlowerIcon(null, true, (byte)Rank), 0, 0, 1);
                            stream.AddItemGenericRankingCreate(Rank, FToday.Value, user.Player.UID, user.Player.Name);
                            user.Send(stream.GenericRankingFinalize());
                            break;
                        }
                        Rank++;
                    }
                }
                stream.GenericRankingCreate(MsgGenericRanking.Action.InformationRequest, MsgGenericRanking.RankType.None, 0, 0, 0);
                stream.AddItemGenericRankingCreate(0, 0u, 0u, "");
                user.Send(stream.GenericRankingFinalize());
                if (ReloadScreen)
                    user.Player.View.SendView(user.Player.GetArray(stream, false), false);
            }
        }

        public IEnumerator<Flower> GetEnumerator()
        {
            yield return RedRoses;
            yield return Lilies;
            yield return Orchids;
            yield return Tulips;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            WriteLine writer;
            writer = new WriteLine('/');
            writer.Add(FreeFlowers);
            using (IEnumerator<Flower> enumerator = GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Flower Flow;
                    Flow = enumerator.Current;
                    writer.Add(Flow.Amount).Add(Flow.Amount2day);
                }
            }
            return writer.Close();
        }
    }
}