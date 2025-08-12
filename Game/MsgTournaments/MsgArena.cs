using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Database;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using static TheChosenProject.Role.Flags;

namespace TheChosenProject.Game.MsgTournaments
{
    public unsafe static partial class MsgBuilder
    {
        public static Packet ArenaDuelCreate(this Packet stream, MsgArenaSignup.DialogType DialogID, MsgArenaSignup.DialogButton OptionID)
        {
            stream.InitWriter();
            stream.Write((uint)DialogID);
            stream.Write((uint)OptionID);
            stream.Finalize(2205);
            return stream;
        }


    }

    public class MsgArena
    {
        public enum ArenaIDs : uint
        {
            ShowPlayerRankList = 10u,
            QualifierList = 6u
        }

        public class User
        {
            public enum StateType : byte
            {
                None,
                FindMatch,
                WaitForBox,
                WaitForOther,
                Fight
            }

            public enum MatchStatus : byte
            {
                None,
                Winner,
                Loser
            }

            public bool CanCountine = true;

            public bool InDuelRoom;

            public byte DuelHitted;

            public MsgArenaInfo Info;

            public string Name = "None";

            public uint UID;

            public ushort Level;

            public byte Class;

            public uint Mesh;

            public StateType ArenaState;

            public DateTime AcceptBoxShow;

            public bool AcceptBox;

            public MatchStatus QualifierStatus;

            public uint Damage;

            public uint LastSeasonArenaPoints;

            public uint LastSeasonWin;

            public uint LastSeasonLose;

            public uint LastSeasonRank;

            public int LastBetting;

            public uint Cheers;

            public void Reset()
            {
                LastBetting = 0;
                DuelHitted = 0;
                ArenaState = StateType.None;
                AcceptBox = false;
                Info.Status = MsgArenaInfo.Action.NotSignedUp;
                InDuelRoom = false;
                Cheers = 0;
            }

            public void ApplayInfo(Player player)
            {
                Name = player.Name;
                UID = player.UID;
                Level = player.Level;
                Class = player.Class;
                Mesh = player.Mesh;
            }

            public User()
            {
                Info = new MsgArenaInfo();
            }

            public override string ToString()
            {
                WriteLine writer;
                writer = new WriteLine('/');
                writer.Add(UID).Add(Name).Add(Level)
                    .Add(Class)
                    .Add(Mesh)
                    .Add(Info.ArenaPoints)
                    .Add(Info.CurrentHonor)
                    .Add(Info.HistoryHonor)
                    .Add(Info.TodayBattles)
                    .Add(Info.TodayWin)
                    .Add(Info.TotalLose)
                    .Add(Info.TotalWin)
                    .Add(LastSeasonArenaPoints)
                    .Add(LastSeasonWin)
                    .Add(LastSeasonLose)
                    .Add(LastSeasonRank);
                return writer.Close();
            }

            internal void Load(string Line)
            {
                if (Line != null)
                {
                    ReadLine reader;
                    reader = new ReadLine(Line, '/');
                    UID = reader.Read((uint)0);
                    Name = reader.Read("None");
                    Level = reader.Read((ushort)0);
                    Class = reader.Read((byte)0);
                    Mesh = reader.Read((uint)0);
                    Info.ArenaPoints = reader.Read((uint)0);
                    Info.CurrentHonor = reader.Read((uint)0);
                    Info.HistoryHonor = reader.Read((uint)0);
                    Info.TodayBattles = reader.Read((uint)0);
                    Info.TodayWin = reader.Read((uint)0);
                    Info.TotalLose = reader.Read((uint)0);
                    Info.TotalWin = reader.Read((uint)0);
                    LastSeasonArenaPoints = reader.Read((uint)0);
                    LastSeasonWin = reader.Read((uint)0);
                    LastSeasonLose = reader.Read((uint)0);
                    LastSeasonRank = reader.Read((uint)0);
                }
            }
        }

        public class Match
        {
            public List<uint> Cheerers = new List<uint>();

            public ConcurrentDictionary<uint, GameClient> Watchers = new ConcurrentDictionary<uint, GameClient>();

            public bool Done;

            public bool Imported;

            public uint dinamicID;

            private readonly uint UID;

            public DateTime DoneStamp;

            public DateTime StartTimer;

            public GameClient[] Players;

            public uint MatchUID => UID;

            public void BeginWatching(Packet stream, GameClient client)
            {
                if (dinamicID == 0)
                    return;
                if (!client.Player.Alive)
                    client.SendSysMesage("You are not alive.");
                else if (client.InQualifier() || client.IsWatching())
                {
                    client.SendSysMesage("You are already waiting for a match in the arena qualifier.");
                }
                else
                {
                    if (Watchers.ContainsKey(client.Player.UID))
                        return;
                    client.ArenaWatchingGroup = this;
                    client.Teleport((ushort)ServerKernel.NextAsync(35, 70), (ushort)ServerKernel.NextAsync(35, 70), 700, dinamicID);
                    client.ArenaWatchingGroup = this;
                    if (!Watchers.TryAdd(client.Player.UID, client))
                        return;
                    stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.RequestView, 0, 0, 0, Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                    client.Send(stream.ArenaWatchersFinalize());
                    stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                    GameClient[] array;
                    array = Watchers.Values.ToArray();
                    for (int x = 0; x < Watchers.Count; x++)
                    {
                        stream.AddItemArenaWatchers(array[x].ArenaStatistic);
                    }
                    stream.ArenaWatchersFinalize();
                    foreach (GameClient user2 in Watchers.Values)
                    {
                        user2.Send(stream);
                    }
                    GameClient[] players;
                    players = Players;
                    foreach (GameClient user in players)
                    {
                        user.Send(stream);
                    }
                    SendScore();
                }
            }

            public void DoLeaveWatching(GameClient client)
            {
                if (client.IsWatching() && Watchers.TryRemove(client.Player.UID, out var _) && client.Player.Map == 700 && client.Player.DynamicID == dinamicID)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, (uint)Watchers.Count, Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                        GameClient[] array;
                        array = Watchers.Values.ToArray();
                        for (int x2 = 0; x2 < Watchers.Count; x2++)
                        {
                            stream.AddItemArenaWatchers(array[x2].ArenaStatistic);
                        }
                        client.Send(stream.ArenaWatchersFinalize());
                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                        for (int x = 0; x < Watchers.Count; x++)
                        {
                            stream.AddItemArenaWatchers(array[x].ArenaStatistic);
                        }
                        stream.ArenaWatchersFinalize();
                        foreach (GameClient user2 in Watchers.Values)
                        {
                            user2.Send(stream);
                        }
                        GameClient[] players;
                        players = Players;
                        foreach (GameClient user in players)
                        {
                            user.Send(stream);
                        }
                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, 0, 0, 0);
                        client.Send(stream.ArenaWatchersFinalize());
                    }
                    SendScore();
                    client.ArenaWatchingGroup = null;
                    client.TeleportCallBack();
                }
                client.ArenaWatchingGroup = null;
            }

            public void DoCheer(Packet stream, GameClient client, uint uid)
            {
                if (!client.IsWatching() || Cheerers.Contains(client.Player.UID))
                    return;
                Cheerers.Add(client.Player.UID);
                if (Players[0].Player.UID == uid)
                    Players[0].ArenaStatistic.Cheers++;
                else if (Players[1].ArenaStatistic.UID == uid)
                {
                    Players[1].ArenaStatistic.Cheers++;
                }
                stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Players[0].ArenaStatistic.Cheers, Players[1].ArenaStatistic.Cheers);
                GameClient[] array;
                array = Watchers.Values.ToArray();
                for (int x = 0; x < Watchers.Count; x++)
                {
                    stream.AddItemArenaWatchers(array[x].ArenaStatistic);
                }
                stream.ArenaWatchersFinalize();
                foreach (GameClient user2 in Watchers.Values)
                {
                    user2.Send(stream);
                }
                GameClient[] players;
                players = Players;
                foreach (GameClient user in players)
                {
                    user.Send(stream);
                }
                SendScore();
            }

            public GameClient Winner()
            {
                GameClient client;
                client = Players.Where((GameClient p) => p.ArenaStatistic.QualifierStatus != User.MatchStatus.Loser && p.ArenaStatistic.QualifierStatus != User.MatchStatus.None).SingleOrDefault();
                if (client == null)
                    return Players[0];
                return client;
            }

            public GameClient Loser()
            {
                GameClient client;
                client = Players.Where((GameClient p) => p.ArenaStatistic.QualifierStatus == User.MatchStatus.Loser).SingleOrDefault();
                if (client == null)
                    return Players[0];
                return client;
            }

            public Match(GameClient user1, GameClient user2, uint _uid)
            {
                Players = new GameClient[2];
                Players[0] = user1;
                Players[1] = user2;
                UID = _uid;
                DoneStamp = default(DateTime);
                user1.ArenaMatch = (user2.ArenaMatch = this);
                user1.ArenaStatistic.QualifierStatus = (user2.ArenaStatistic.QualifierStatus = User.MatchStatus.None);
                StartTimer = DateTime.Now;
            }

            public bool TryGetOpponent(uint MyUID, out GameClient client)
            {
                GameClient[] players;
                players = Players;
                foreach (GameClient user in players)
                {
                    if (MyUID != user.Player.UID)
                    {
                        client = user;
                        return true;
                    }
                }
                client = null;
                return false;
            }

            public void SendSignUp(Packet stream, GameClient user)
            {
                user.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.StartCountDown, MsgArenaSignup.DialogButton.SignUp, user));
            }

            public void Export()
            {
                if (!Imported)
                    return;
                MsgSchedules.Arena.MatchesRegistered.TryRemove(UID, out var _);
                foreach (GameClient user2 in Watchers.Values)
                {
                    DoLeaveWatching(user2);
                }
                GameClient[] players;
                players = Players;
                foreach (GameClient user in players)
                {
                    if (user.ArenaStatistic.InDuelRoom)
                    {
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            user.Player.RemoveTempEquipment(stream);
                        }
                    }
                    user.TeleportCallBack();
                    user.Player.RestorePkMode();
                }
            }

            public void Win(GameClient winner, GameClient loser)
            {
                winner.ArenaStatistic.QualifierStatus = User.MatchStatus.Winner;
                loser.ArenaStatistic.QualifierStatus = User.MatchStatus.Loser;
                if (winner.ArenaMatch == null || loser.ArenaMatch == null)
                    return;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    winner.ArenaMatch = null;
                    loser.ArenaMatch = null;
                    if (!winner.ArenaStatistic.InDuelRoom && !loser.ArenaStatistic.InDuelRoom)
                    {
                        int diff;
                        diff = ServerKernel.NextAsync(30, 50);
                        winner.ArenaStatistic.Info.Status = MsgArenaInfo.Action.NotSignedUp;
                        winner.Send(stream.ArenaInfoCreate(winner.ArenaStatistic.Info));
                        winner.ArenaStatistic.ArenaState = User.StateType.FindMatch;
                        loser.ArenaStatistic.Info.Status = MsgArenaInfo.Action.NotSignedUp;
                        loser.Send(stream.ArenaInfoCreate(loser.ArenaStatistic.Info));
                        loser.ArenaStatistic.ArenaState = User.StateType.FindMatch;
                        winner.DbDailyTraining.ArenaWin++;
                        loser.DbDailyTraining.ArenaLose++;
                        winner.ArenaPoints += (uint)diff;
                        if (winner.ArenaStatistic.LastBetting == ServerKernel.QUALIFIER_PK_REWARD && loser.ArenaStatistic.LastBetting == ServerKernel.QUALIFIER_PK_REWARD)
                        {
                            int reward;
                            reward = winner.ArenaStatistic.LastBetting * 2;
                       
                                winner.Player.Money += reward;
                                IEventRewards.Add("Arena Money", (uint)reward, 0, "", "[" + winner.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                                winner.HonorPoints += (uint)ServerKernel.QUALIFIER_HONOR_REWARD;
                                winner.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession5");
                            
                        }
                        winner.ArenaStatistic.Info.ArenaWinnerMatches++;
                        if (loser.ArenaPoints > diff)
                            loser.ArenaPoints -= (uint)diff;
                        winner.ArenaStatistic.Info.TodayWin++;
                        winner.ArenaStatistic.Info.TotalWin++;
                        loser.ArenaStatistic.Info.TodayBattles++;
                        loser.ArenaStatistic.Info.TotalLose++;
                        UpdateRank();
                        StringBuilder builder;
                        builder = new StringBuilder();
                        if (winner.Player.MyGuild != null)
                        {
                            builder.Append("(");
                            builder.Append(winner.Player.MyGuild.GuildName.ToString());
                            builder.Append(") ");
                        }
                        builder.Append(winner.Player.Name);
                        builder.Append(" has defeated ");
                        if (loser.Player.MyGuild != null)
                        {
                            builder.Append("(");
                            builder.Append(loser.Player.MyGuild.GuildName.ToString());
                            builder.Append(") ");
                        }
                        builder.Append(loser.Player.Name);
                        if (winner.ArenaStatistic.Info.TodayRank != 0)
                        {
                            builder.Append(" in the Qualifier, and is currently ranked No. ");
                            builder.Append(winner.ArenaStatistic.Info.TodayRank);
                        }
                        else
                            builder.Append(" in the Qualifier");
                        builder.Append(".");
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(builder.ToString(), MsgMessage.MsgColor.red, MsgMessage.ChatMode.Qualifier).GetArray(stream));
                        loser.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog2, MsgArenaSignup.DialogButton.SignUp, loser));
                        winner.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog2, MsgArenaSignup.DialogButton.Win, winner));
                    }
                    else
                    {
                        loser.Send(stream.ArenaDuelCreate(MsgArenaSignup.DialogType.Dialog2, MsgArenaSignup.DialogButton.SignUp));
                        winner.Send(stream.ArenaDuelCreate(MsgArenaSignup.DialogType.Dialog2, MsgArenaSignup.DialogButton.Win));
                        winner.ArenaStatistic.CanCountine = (loser.ArenaStatistic.CanCountine = false);
                    }
                    winner.ArenaStatistic.Reset();
                    loser.ArenaStatistic.Reset();
                    winner.Send(stream.ArenaInfoCreate(winner.ArenaStatistic.Info));
                    loser.Send(stream.ArenaInfoCreate(loser.ArenaStatistic.Info));
                    MsgSchedules.Arena.MatchesRegistered.TryRemove(UID, out var _);
                }
            }

            public void End()
            {
                End((Players[0].ArenaStatistic.Damage > Players[1].ArenaStatistic.Damage) ? Players[1] : Players[0]);
            }

            public void End(GameClient loser)
            {
                if (!Done)
                {
                    Done = true;
                    Players[0].Player.ProtectAttack(5000);
                    Players[1].Player.ProtectAttack(5000);
                    if (Players[0].Player.UID == loser.Player.UID)
                    {
                        Players[0].ArenaStatistic.QualifierStatus = User.MatchStatus.Loser;
                        Players[1].ArenaStatistic.QualifierStatus = User.MatchStatus.Winner;
                    }
                    else
                    {
                        Players[1].ArenaStatistic.QualifierStatus = User.MatchStatus.Loser;
                        Players[0].ArenaStatistic.QualifierStatus = User.MatchStatus.Winner;
                    }
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        loser.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog, MsgArenaSignup.DialogButton.Lose, loser));
                        Winner().Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Dialog, MsgArenaSignup.DialogButton.Win, Winner()));
                    }
                    DoneStamp = DateTime.Now;
                }
            }

            public void Import()
            {
                if (Imported)
                    return;
                GameMap map;
                map = Server.ServerMaps[700u];
                dinamicID = map.GenerateDynamicID();
                GameClient[] players;
                players = Players;
                foreach (GameClient user in players)
                {
                    user.ArenaStatistic.Damage = 0;
                    if (!user.ArenaStatistic.InDuelRoom)
                    {
                            user.Player.Money -= (int)ServerKernel.QUALIFIER_PK_REWARD;
                            user.ArenaStatistic.LastBetting = (int)ServerKernel.QUALIFIER_PK_REWARD;
                    }
                    else
                        user.ArenaStatistic.Damage = user.ArenaStatistic.DuelHitted;
                    ushort x;
                    x = 0;
                    ushort y;
                    y = 0;
                    map.GetRandCoord(ref x, ref y);
                    user.Teleport(x, y, 700, dinamicID);
                    //if (user.ArenaStatistic.InDuelRoom)
                    //{
                    //    using (RecycledPacket rec = new RecycledPacket())
                    //    {
                    //        Packet stream;
                    //        stream = rec.GetStream();
                    //        //if (!user.MySpells.ClientSpells.ContainsKey(1045))
                    //        //{
                    //        //    user.MySpells.Add(stream, 1045, 4);
                    //        //}
                    //        //else
                    //        //{
                    //        //    user.MySpells.ClientSpells[1045].Level = 4;
                    //        //    user.Send(stream.SpellCreate(user.MySpells.ClientSpells[1045]));
                    //        //}
                    //        //if (!user.MySpells.ClientSpells.ContainsKey(1046))
                    //        //{
                    //        //    user.MySpells.Add(stream, 1046, 4);
                    //        //}
                    //        //else
                    //        //{
                    //        //    user.MySpells.ClientSpells[1046].Level = 4;
                    //        //    user.Send(stream.SpellCreate(user.MySpells.ClientSpells[1046]));
                    //        //}
                    //        //user.Player.AddTempEquipment(stream, 410239, ConquerItem.RightWeapon);
                    //        //user.Player.AddTempEquipment(stream, 420239, ConquerItem.LeftWeapon);
                    //    }
                    //}
                    user.Player.FairbattlePower = Flags.FairbattlePower.UpdateToSerf;
                    user.Player.ProtectJumpAttack(10);
                    if (TryGetOpponent(user.Player.UID, out var Opponent))
                    {
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            Opponent.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.StartTheFight, MsgArenaSignup.DialogButton.SignUp, user));
                            Opponent.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.Match, MsgArenaSignup.DialogButton.MatchOn, user));
                        }
                    }
                    user.Player.SetPkMode(Flags.PKMode.PK);
                }
                Imported = true;
                SendScore();
            }

            public void SendScore()
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    stream = stream.ArenaMatchScoreCreate(Players[0].Player.UID, Players[0].ArenaStatistic.Name, Players[0].ArenaStatistic.Damage, Players[1].Player.UID, Players[1].ArenaStatistic.Name, Players[1].ArenaStatistic.Damage);
                    GameClient[] players;
                    players = Players;
                    foreach (GameClient user in players)
                    {
                        user.Send(stream);
                    }
                    foreach (GameClient user2 in Watchers.Values)
                    {
                        user2.Send(stream);
                    }
                }
            }
        }

        public static ConcurrentDictionary<uint, User> ArenaPoll = new ConcurrentDictionary<uint, User>();

        public static User[] Top10 = new User[10];

        public static User[] Top1000Today = new User[1000];

        public static User[] Top1000 = new User[1000];

        public List<uint> BlockArenaMaps = new List<uint>
        {
            1858, 1860, 4020, 4000, 4003, 4006, 4008, 4009, 6000, 6001u,
            1017, 1080, 1081, 2060, 6002, 6003, 601, 700, 1038, 1764u,
            1036, 1764, 2068u
        };

        public ConcurrentDictionary<uint, GameClient> Registered;

        public Counter MatchCounter = new Counter(1);

        public ConcurrentDictionary<uint, Match> MatchesRegistered;

        public Time32 CreateMatchesStamp = Time32.Now.AddMilliseconds(1100);

        public Time32 VerifyMatchesStamp = Time32.Now.AddMilliseconds(1200);

        public Time32 CheckGroupsStamp = Time32.Now.AddMilliseconds(1150);

        public ProcesType Proces { get; set; }

        public void CreateRankTop10()
        {
            lock (Top10)
            {
                Top10 = new User[10];
                User[] array;
                array = ArenaPoll.Values.ToArray();
                User[] Rank;
                Rank = array.OrderByDescending((User p) => p.LastSeasonArenaPoints).ToArray();
                for (int x = 0; x < Rank.Length && x != 10; x++)
                {
                    User element;
                    element = Rank[x];
                    Top10[x] = element;
                }
            }
        }

        public void CreateRankTop1000()
        {
            lock (Top1000)
            {
                Top1000 = new User[1000];
                User[] array;
                array = ArenaPoll.Values.ToArray();
                User[] Rank;
                Rank = array.OrderByDescending((User p) => p.Info.CurrentHonor).ToArray();
                for (int x = 0; x < Rank.Length && x != 1000; x++)
                {
                    Top1000[x] = Rank[x];
                }
            }
        }

        public MsgArena()
        {
            Proces = ProcesType.Dead;
            Registered = new ConcurrentDictionary<uint, GameClient>();
            MatchesRegistered = new ConcurrentDictionary<uint, Match>();
        }

        public void CheckGroups(Time32 clock)
        {
            if (!(clock > CheckGroupsStamp))
                return;
            if (MatchesRegistered.Count > 0)
            {
                DateTime Now;
                Now = DateTime.Now;
                Match[] ArrayMatches;
                ArrayMatches = MatchesRegistered.Values.ToArray();
                Match[] array;
                array = ArrayMatches;
                foreach (Match group in array)
                {
                    if (!(Now > group.StartTimer.AddSeconds(5.0)))
                        continue;
                    if (!group.Done)
                    {
                        if (Now > group.StartTimer.AddMinutes(3.0))
                            group.End();
                    }
                    else if (Now > group.DoneStamp.AddSeconds(4.0))
                    {
                        group.DoneStamp = DateTime.Now.AddDays(1.0);
                        group.Export();
                        group.Win(group.Winner(), group.Loser());
                    }
                }
            }
            CheckGroupsStamp.Value = clock.Value + 1150;
        }

        public void CreateMatches(Time32 clock)
        {
            if (!(clock > CreateMatchesStamp))
                return;
            DateTime Timer;
            Timer = DateTime.Now;
            if (Registered.Count < 2)
                return;
            if (Timer.Second % 3 == 0)
            {
                GameClient[] array;
                array = Registered.Values.ToArray();
                GameClient[] Players;
                Players = array.OrderByDescending((GameClient p) => p.ArenaStatistic.Info.ArenaPoints).ToArray();
                GameClient user2;
                user2 = null;
                GameClient user3;
                user3 = null;
                ConcurrentQueue<GameClient> Remover;
                Remover = new ConcurrentQueue<GameClient>();
                GameClient[] array2;
                array2 = Players;
                foreach (GameClient user in array2)
                {
                    if (BlockArenaMaps.Contains(user.Player.Map) || !user.Player.Alive || user.InQualifier() || !user.Socket.Alive)
                        Remover.Enqueue(user);
                    else if ((user.ArenaStatistic.InDuelRoom || user.Player.Money >= ServerKernel.QUALIFIER_PK_REWARD) && user.ArenaStatistic.ArenaState == User.StateType.FindMatch && user.ArenaStatistic.Info.Status == MsgArenaInfo.Action.WaitingForOpponent)
                    {
                        if (user2 == null)
                            user2 = user;
                        else if (user3 == null && user2.IP != user.IP)
                        {
                            user3 = user;
                        }
                        if (user2 != null && user3 != null)
                            break;
                    }
                }
                if (user2 != null && user3 != null)
                {
                    user2.ArenaStatistic.ArenaState = (user3.ArenaStatistic.ArenaState = User.StateType.WaitForBox);
                    user3.ArenaStatistic.AcceptBoxShow = (user2.ArenaStatistic.AcceptBoxShow = DateTime.Now.AddSeconds(60.0));
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        user2.Send(stream.ArenaInfoCreate(user2.ArenaStatistic.Info));
                        user3.Send(stream.ArenaInfoCreate(user3.ArenaStatistic.Info));
                        Match match;
                        match = new Match(user2, user3, MatchCounter.Next);
                        match.SendSignUp(stream, user2);
                        match.SendSignUp(stream, user3);
                        MatchesRegistered.TryAdd(match.MatchUID, match);
                        UnRegistered(user2);
                        UnRegistered(user3);
                    }
                }
                GameClient remover;
                while (Remover.TryDequeue(out remover))
                {
                    Registered.TryRemove(remover.Player.UID, out remover);
                }
            }
            CreateMatchesStamp.Value = clock.Value + 1100;
        }

        public void VerifyMatches(Time32 clock)
        {
            if (!(clock > VerifyMatchesStamp) || MatchesRegistered.Count == 0)
                return;
            Match[] Array;
            Array = MatchesRegistered.Values.ToArray();
            Match[] array;
            array = Array;
            foreach (Match match in array)
            {
                if (match.Players[0] == null || match.Players[1] == null)
                    continue;
                if (match.Players[0].Player.Map != 700 && BlockArenaMaps.Contains(match.Players[0].Player.Map))
                    match.Win(match.Players[1], match.Players[0]);
                if (match.Players[1].Player.Map != 700 && BlockArenaMaps.Contains(match.Players[1].Player.Map))
                    match.Win(match.Players[0], match.Players[1]);
                if ((match.Players[0].ArenaStatistic.ArenaState == User.StateType.WaitForBox || match.Players[1].ArenaStatistic.ArenaState == User.StateType.WaitForBox) && DateTime.Now > match.Players[0].ArenaStatistic.AcceptBoxShow)
                {
                    if (match.Players[0].ArenaStatistic.ArenaState == User.StateType.WaitForBox)
                        match.Win(match.Players[1], match.Players[0]);
                    else
                        match.Win(match.Players[0], match.Players[1]);
                    return;
                }
                if (match.Players[0].ArenaStatistic.ArenaState == User.StateType.WaitForOther && !match.Players[0].ArenaStatistic.AcceptBox)
                    match.Win(match.Players[1], match.Players[0]);
                else if (match.Players[1].ArenaStatistic.ArenaState == User.StateType.WaitForOther && !match.Players[1].ArenaStatistic.AcceptBox)
                {
                    match.Win(match.Players[0], match.Players[1]);
                }
                else
                {
                    if (match.Players[0].ArenaStatistic.ArenaState != User.StateType.WaitForOther || match.Players[1].ArenaStatistic.ArenaState != User.StateType.WaitForOther)
                        continue;
                    if (!match.Players[0].ArenaStatistic.AcceptBox || !match.Players[1].ArenaStatistic.AcceptBox)
                    {
                        if (!match.Players[0].ArenaStatistic.AcceptBox)
                            match.Win(match.Players[1], match.Players[0]);
                        else
                            match.Win(match.Players[0], match.Players[1]);
                    }
                    else
                    {
                        match.Players[0].ArenaStatistic.ArenaState = (match.Players[1].ArenaStatistic.ArenaState = User.StateType.Fight);
                        match.Import();
                    }
                }
            }
            VerifyMatchesStamp.Value = clock.Value + 1200;
        }

        public static void UpdateRank()
        {
            lock (Top1000Today)
            {
                Top1000Today = new User[1000];
                User[] array;
                array = ArenaPoll.Values.ToArray();
                User[] array2;
                array2 = array;
                foreach (User user in array2)
                {
                    user.Info.TodayRank = 0;
                }
                User[] Rank;
                Rank = array.OrderByDescending((User p) => p.Info.ArenaPoints).ToArray();
                for (int x = 0; x < Rank.Length && x != 1000; x++)
                {
                    Top1000Today[x] = Rank[x];
                    Rank[x].Info.TodayRank = (uint)(x + 1);
                }
            }
        }

        public void DoSignup(Packet stream, GameClient client)
        {
            string BannedMessage;
            if (client.Player.Money < ServerKernel.QUALIFIER_PK_REWARD)
                client.Player.MessageBox($"You need at least {ServerKernel.QUALIFIER_PK_REWARD} Conquer Money to play in the arena qualifier.", null, null);
            else if (SystemBannedAccount.IsArena(client.ConnectionUID, out BannedMessage))
            {
                client.Player.MessageBox(BannedMessage, null, null);
            }
            else if (client.ArenaStatistic.Info.Status != 0)
            {
                client.SendSysMesage("You are already waiting for a match in the arena qualifier.", MsgMessage.ChatMode.Agate);
            }
            else if (client.InQualifier())
            {
                client.SendSysMesage("You are already waiting for a match in the arena qualifier.", MsgMessage.ChatMode.Agate);
            }
            else if (client.ArenaStatistic.Info.ArenaPoints != 0 && !BlockArenaMaps.Contains(client.Player.Map))
            {
                client.ArenaStatistic.CanCountine = true;
                if (!ArenaPoll.ContainsKey(client.Player.UID))
                    ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);
                Registered.TryAdd(client.Player.UID, client);
                client.ArenaStatistic.Info.Status = MsgArenaInfo.Action.WaitingForOpponent;
                client.ArenaStatistic.ArenaState = User.StateType.FindMatch;
                client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
            }
        }

        public void UnRegistered(GameClient client)
        {
            Registered.TryRemove(client.Player.UID, out var _);
        }

        public void DoQuit(Packet stream, GameClient client)
        {
            if (client.ArenaMatch != null)
                client.ArenaMatch.End(client);
            else
                client.ArenaStatistic.Reset();
            UnRegistered(client);
            client.ArenaStatistic.Info.Status = MsgArenaInfo.Action.NotSignedUp;
            client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
        }

        public void DoGiveUp(Packet stream, GameClient client)
        {
            if (client.ArenaStatistic.ArenaState == User.StateType.WaitForBox)
            {
                client.ArenaStatistic.AcceptBox = false;
                client.ArenaStatistic.ArenaState = User.StateType.WaitForOther;
                return;
            }
            client.ArenaStatistic.Info.Status = MsgArenaInfo.Action.WaitingInactive;
            client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
            if (client.ArenaMatch != null)
            {
                if (client.ArenaMatch.TryGetOpponent(client.Player.UID, out var Opponent))
                {
                    Opponent.Send(stream.ArenaSignupCreate(MsgArenaSignup.DialogType.OpponentGaveUp, MsgArenaSignup.DialogButton.SignUp, client));
                    Opponent.ArenaStatistic.Info.Status = MsgArenaInfo.Action.NotSignedUp;
                    client.ArenaStatistic.Info.Status = MsgArenaInfo.Action.NotSignedUp;
                    Opponent.Send(stream.ArenaInfoCreate(Opponent.ArenaStatistic.Info));
                    client.Send(stream.ArenaInfoCreate(client.ArenaStatistic.Info));
                }
                if (!client.ArenaMatch.Done)
                    client.ArenaMatch.End(client);
                else if (Opponent != null)
                {
                    client.ArenaMatch.Win(Opponent, client);
                }
                else
                {
                    client.ArenaMatch.End(client);
                }
            }
        }
    }
}
