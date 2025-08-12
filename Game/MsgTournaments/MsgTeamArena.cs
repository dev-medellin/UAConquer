using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using Extensions;
using TheChosenProject.Client;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgTeamArena
    {
        public class User
        {
            public MsgTeamArenaInfo Info;

            public string Name = "None";

            public uint UID;

            public ushort Level;

            public byte Class;

            public uint Mesh;

            public uint LastSeasonArenaPoints;

            public uint LastSeasonWin;

            public uint LastSeasonLose;

            public uint LastSeasonRank;

            public uint Cheers;

            public byte GetGender
            {
                get
                {
                    if (Mesh % 10u >= 3)
                        return 0;
                    return 1;
                }
            }

            public void Reset()
            {
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
                Info = MsgTeamArenaInfo.Create();
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
                    Info.TodayWin = reader.Read((uint)0);
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

            public uint dinamicID;

            public bool Imported;

            private uint UID;

            public DateTime DoneStamp;

            public DateTime StartTimer;

            public Team[] Teams;

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
                    if (!Watchers.TryAdd(client.Player.UID, client))
                        return;
                    stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.RequestView, 0, 0, 0, Teams[0].Cheers, Teams[1].Cheers);
                    client.Send(stream.ArenaWatchersFinalize());
                    stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);
                    GameClient[] array;
                    array = Watchers.Values.ToArray();
                    for (int x = 0; x < Watchers.Count; x++)
                    {
                        stream.AddItemArenaWatchers(array[x].TeamArenaStatistic);
                    }
                    stream.ArenaWatchersFinalize();
                    foreach (GameClient user2 in Watchers.Values)
                    {
                        user2.Send(stream);
                    }
                    Team[] teams;
                    teams = Teams;
                    foreach (Team team in teams)
                    {
                        foreach (GameClient user in team.GetMembers())
                        {
                            if (user.Player.DynamicID == dinamicID)
                                user.Send(stream);
                        }
                    }
                    SendScore();
                    client.TeamArenaWatchingGroup = this;
                    client.Teleport((ushort)ServerKernel.NextAsync(35, 70), (ushort)ServerKernel.NextAsync(35, 70), 700, dinamicID);
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
                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);
                        GameClient[] array;
                        array = Watchers.Values.ToArray();
                        for (int x2 = 0; x2 < Watchers.Count; x2++)
                        {
                            stream.AddItemArenaWatchers(array[x2].TeamArenaStatistic);
                        }
                        client.Send(stream.ArenaWatchersFinalize());
                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);
                        for (int x = 0; x < Watchers.Count; x++)
                        {
                            stream.AddItemArenaWatchers(array[x].TeamArenaStatistic);
                        }
                        stream.ArenaWatchersFinalize();
                        foreach (GameClient user2 in Watchers.Values)
                        {
                            user2.Send(stream);
                        }
                        Team[] teams;
                        teams = Teams;
                        foreach (Team team in teams)
                        {
                            foreach (GameClient user in team.GetMembers())
                            {
                                if (user.Player.DynamicID == dinamicID)
                                    user.Send(stream);
                            }
                        }
                        stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, 0, 0, 0, 0);
                        client.Send(stream.ArenaWatchersFinalize());
                    }
                    SendScore();
                    client.TeamArenaWatchingGroup = null;
                    client.TeleportCallBack();
                }
                client.TeamArenaWatchingGroup = null;
            }

            public void DoCheer(Packet stream, GameClient client, uint uid)
            {
                if (!client.IsWatching() || Cheerers.Contains(client.Player.UID))
                    return;
                Cheerers.Add(client.Player.UID);
                if (Teams[0].Members.ContainsKey(uid))
                    Teams[0].Cheers++;
                else if (Teams[1].Members.ContainsKey(uid))
                {
                    Teams[1].Cheers++;
                }
                stream.ArenaWatchersCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, 0, (uint)Watchers.Count, Teams[0].Cheers, Teams[1].Cheers);
                GameClient[] array;
                array = Watchers.Values.ToArray();
                for (int x = 0; x < Watchers.Count; x++)
                {
                    stream.AddItemArenaWatchers(array[x].TeamArenaStatistic);
                }
                stream.ArenaWatchersFinalize();
                foreach (GameClient user2 in Watchers.Values)
                {
                    user2.Send(stream);
                }
                Team[] teams;
                teams = Teams;
                foreach (Team team in teams)
                {
                    foreach (GameClient user in team.GetMembers())
                    {
                        if (user.Player.DynamicID == dinamicID)
                            user.Send(stream);
                    }
                }
                SendScore();
            }

            public Team Winner()
            {
                return Teams.Where((Team p) => p.Status != Team.TournamentProces.Loser && p.Status != Team.TournamentProces.None).SingleOrDefault();
            }

            public Team Loser()
            {
                return Teams.Where((Team p) => p.Status == Team.TournamentProces.Loser).SingleOrDefault();
            }

            public Match(Team team1, Team team2, uint _uid)
            {
                Teams = new Team[2];
                Teams[0] = team1;
                Teams[1] = team2;
                UID = _uid;
                DoneStamp = default(DateTime);
                team1.TeamArenaMatch = (team2.TeamArenaMatch = this);
                Team[] teams;
                teams = Teams;
                foreach (Team team3 in teams)
                {
                    team3.Cheers = 0;
                    team3.Damage = 0;
                    team3.Status = Team.TournamentProces.None;
                }
                StartTimer = DateTime.Now;
            }

            public bool TryGetOpponent(uint MyUID, out Team Opponentteam)
            {
                Team[] teams;
                teams = Teams;
                foreach (Team team in teams)
                {
                    if (team.UID != MyUID)
                    {
                        Opponentteam = team;
                        return true;
                    }
                }
                Opponentteam = null;
                return false;
            }

            public void SendSignUp(GameClient user)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    user.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.StartCountDown, MsgTeamArenaSignup.DialogButton.SignUp, user));
                }
            }

            public void Export()
            {
                if (!Imported)
                    return;
                MsgSchedules.TeamArena.MatchesRegistered.TryRemove(UID, out var _);
                foreach (GameClient user2 in Watchers.Values)
                {
                    DoLeaveWatching(user2);
                }
                Team[] teams;
                teams = Teams;
                foreach (Team team in teams)
                {
                    foreach (GameClient user in team.GetMembers())
                    {
                        if (user.Player.Map == 700 && user.Player.DynamicID == dinamicID)
                        {
                            user.TeleportCallBack();
                            user.Player.RestorePkMode();
                        }
                    }
                }
            }

            public void Win(Team winner, Team loser)
            {
                winner.Status = Team.TournamentProces.Winner;
                loser.Status = Team.TournamentProces.Loser;
                if (winner.TeamArenaMatch == null || loser.TeamArenaMatch == null)
                    return;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    winner.TeamArenaMatch = null;
                    loser.TeamArenaMatch = null;
                    int diff;
                    diff = ServerKernel.NextAsync(30, 50);
                    Team[] teams;
                    teams = Teams;
                    foreach (Team team in teams)
                    {
                        foreach (GameClient user in team.GetMembers())
                        {
                            user.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.NotSignedUp;
                            user.TeamArenaStatistic.Info.Send(user);
                        }
                        team.ArenaState = Team.StateType.FindMatch;
                    }
                    foreach (GameClient user5 in winner.GetMembers())
                    {
                        user5.TeamArenaPoints += (uint)diff;
                        user5.TeamArenaStatistic.Info.TodayWin++;
                        user5.TeamArenaStatistic.Info.TotalWin++;
                        user5.TeamArenaStatistic.Info.TodayBattles++;
                        if (user5.TeamArenaStatistic.Info.TodayWin == 9)
                        {
                            if (user5.Inventory.HaveSpace(1))
                                user5.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                            else
                                user5.Inventory.AddReturnedItem(stream, 723912, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false, Flags.ItemEffect.None, 0);
                        }
                        if (user5.TeamArenaStatistic.Info.TodayBattles == 20)
                        {
                            if (user5.Inventory.HaveSpace(1))
                                user5.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                            else
                                user5.Inventory.AddReturnedItem(stream, 723912, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false, Flags.ItemEffect.None, 0);
                        }
                    }
                    foreach (GameClient user4 in loser.GetMembers())
                    {
                        if (user4.TeamArenaPoints > diff)
                            user4.TeamArenaPoints -= (uint)diff;
                        user4.TeamArenaStatistic.Info.TodayBattles++;
                        user4.TeamArenaStatistic.Info.TotalLose++;
                        if (user4.TeamArenaStatistic.Info.TodayBattles == 20)
                        {
                            if (user4.Inventory.HaveSpace(1))
                                user4.Inventory.AddItemWitchStack(723912, 0, 1, stream);
                            else
                                user4.Inventory.AddReturnedItem(stream, 723912, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false, Flags.ItemEffect.None, 0);
                        }
                    }
                    UpdateRank();
                    StringBuilder builder;
                    builder = new StringBuilder();
                    if (winner.Leader.Player.MyGuild != null)
                    {
                        builder.Append("(");
                        builder.Append(winner.Leader.Player.MyGuild.GuildName.ToString());
                        builder.Append(") ");
                    }
                    builder.Append(winner.Leader.Player.Name);
                    builder.Append(" has defeated ");
                    if (loser.Leader.Player.MyGuild != null)
                    {
                        builder.Append("(");
                        builder.Append(loser.Leader.Player.MyGuild.GuildName.ToString());
                        builder.Append(") ");
                    }
                    builder.Append(loser.Leader.Player.Name);
                    if (winner.Leader.TeamArenaStatistic.Info.TodayRank != 0)
                    {
                        builder.Append(" in the Qualifier, and is currently ranked No. ");
                        builder.Append(winner.Leader.TeamArenaStatistic.Info.TodayRank);
                    }
                    else
                        builder.Append(" in the Qualifier");
                    builder.Append(".");
                    Program.SendGlobalPackets.Enqueue(new MsgMessage(builder.ToString(), MsgMessage.MsgColor.red, MsgMessage.ChatMode.Qualifier).GetArray(stream));
                    foreach (GameClient user3 in loser.GetMembers())
                    {
                        user3.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog2, MsgTeamArenaSignup.DialogButton.SignUp, user3));
                        user3.TeamArenaStatistic.Reset();
                        user3.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.NotSignedUp;
                        user3.TeamArenaStatistic.Info.Send(user3);
                    }
                    foreach (GameClient user2 in winner.GetMembers())
                    {
                        user2.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog2, MsgTeamArenaSignup.DialogButton.Win, user2));
                        user2.TeamArenaStatistic.Reset();
                        user2.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.NotSignedUp;
                        user2.TeamArenaStatistic.Info.Send(user2);
                    }
                    winner.ResetTeamArena();
                    loser.ResetTeamArena();
                    MsgSchedules.TeamArena.MatchesRegistered.TryRemove(UID, out var _);
                }
            }

            public void End()
            {
                End((Teams[0].Damage > Teams[1].Damage) ? Teams[1] : Teams[0]);
            }

            public void End(Team loser)
            {
                if (Done)
                    return;
                if (Teams[0].UID == loser.UID)
                {
                    Teams[0].Status = Team.TournamentProces.Loser;
                    Teams[1].Status = Team.TournamentProces.Winner;
                }
                else
                {
                    Teams[1].Status = Team.TournamentProces.Loser;
                    Teams[0].Status = Team.TournamentProces.Winner;
                }
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    foreach (GameClient user2 in loser.GetMembers())
                    {
                        if (user2.Player.DynamicID == dinamicID)
                            user2.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog, MsgTeamArenaSignup.DialogButton.Lose, loser.Leader));
                    }
                    foreach (GameClient user in Winner().GetMembers())
                    {
                        if (user.Player.DynamicID == dinamicID)
                            user.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Dialog, MsgTeamArenaSignup.DialogButton.Win, Winner().Leader));
                    }
                    Done = true;
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
                Team[] teams;
                teams = Teams;
                foreach (Team team in teams)
                {
                    team.Damage = 0;
                    ushort x;
                    x = 0;
                    ushort y;
                    y = 0;
                    map.GetRandCoord(ref x, ref y);
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        foreach (GameClient user2 in team.GetMembers())
                        {
                            user2.Teleport(x, y, 700, dinamicID);
                            user2.Player.SetPkMode(Flags.PKMode.Team);
                            user2.Player.ProtectJumpAttack(10);
                        }
                        if (!TryGetOpponent(team.UID, out var Opponent))
                            continue;
                        stream.TeamArenaInfoPlayersCreate(MsgTeamArenaInfoPlayers.KindOfParticipants.Opponents, team.Leader.Player.UID, (uint)team.Members.Count);
                        foreach (GameClient user in team.GetMembers())
                        {
                            stream.AddItemTeamArenaInfoPlayers(user.TeamArenaStatistic);
                        }
                        stream.TeamArenaInfoPlayersFinalize();
                        Opponent.SendTeam(stream, 0);
                        Opponent.SendTeam(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.StartTheFight, MsgTeamArenaSignup.DialogButton.SignUp, team.Leader), 0);
                        Opponent.SendTeam(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.Match, MsgTeamArenaSignup.DialogButton.MatchOn, team.Leader), 0);
                    }
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
                    stream.TeamArenaMatchScoreCreate(Teams[0].Leader.Player.UID, Teams[0].Leader.TeamArenaStatistic.Info.TodayRank, Teams[0].TeamName, Teams[0].Damage, Teams[1].Leader.Player.UID, Teams[1].Leader.TeamArenaStatistic.Info.TodayRank, Teams[1].TeamName, Teams[1].Damage);
                    Team[] teams;
                    teams = Teams;
                    foreach (Team team in teams)
                    {
                        team.SendTeam(stream, 0);
                    }
                    foreach (GameClient user in Watchers.Values)
                    {
                        user.Send(stream);
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

        public Time32 CreateMatchesStamp = Time32.Now.AddMilliseconds(900);

        public Time32 VerifyMatchesStamp = Time32.Now.AddMilliseconds(980);

        public Time32 CheckGroupsStamp = Time32.Now.AddMilliseconds(960);

        public ProcesType Proces { get; set; }

        public MsgTeamArena()
        {
            Proces = ProcesType.Dead;
            Registered = new ConcurrentDictionary<uint, GameClient>();
            MatchesRegistered = new ConcurrentDictionary<uint, Match>();
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
            CheckGroupsStamp.Value = clock.Value + 960;
        }

        public void CreateMatches(Time32 clock)
        {
            if (!(clock > CreateMatchesStamp))
                return;
            DateTime Timer;
            Timer = DateTime.Now;
            if (Registered.Count < 2)
                return;
            if (Timer.Second % 6 == 0)
            {
                ConcurrentQueue<GameClient> Remover;
                Remover = new ConcurrentQueue<GameClient>();
                GameClient[] array;
                array = Registered.Values.ToArray();
                GameClient[] Players;
                Players = array.OrderByDescending((GameClient p) => p.TeamArenaStatistic.Info.ArenaPoints).ToArray();
                GameClient user2;
                user2 = null;
                GameClient user3;
                user3 = null;
                GameClient[] array2;
                array2 = Players;
                foreach (GameClient user in array2)
                {
                    if (user.Team == null || user.InQualifier() || !user.Socket.Alive)
                        Remover.Enqueue(user);
                    else if (!BlockArenaMaps.Contains(user.Player.Map) && user.Team.ArenaState == Team.StateType.FindMatch && user.TeamArenaStatistic.Info.Status == MsgTeamArenaInfo.Action.WaitingForOpponent)
                    {
                        if (user2 == null)
                            user2 = user;
                        else if (user3 == null)
                        {
                            user3 = user;
                        }
                        if (user2 != null && user3 != null)
                            break;
                    }
                }
                if (user2 != null && user3 != null)
                {
                    if (user2.Team == null)
                    {
                        Remover.Enqueue(user2);
                        return;
                    }
                    if (user3.Team == null)
                    {
                        Remover.Enqueue(user3);
                        return;
                    }
                    user2.Team.ArenaState = (user3.Team.ArenaState = Team.StateType.WaitForBox);
                    user2.Team.AcceptBoxShow = (user3.Team.AcceptBoxShow = DateTime.Now);
                    user2.TeamArenaStatistic.Info.Status = (user3.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.WaitingInactive);
                    user2.TeamArenaStatistic.Info.Send(user2);
                    user3.TeamArenaStatistic.Info.Send(user3);
                    Match match;
                    match = new Match(user2.Team, user3.Team, MatchCounter.Next);
                    match.SendSignUp(user2);
                    match.SendSignUp(user3);
                    MatchesRegistered.TryAdd(match.MatchUID, match);
                    UnRegistered(user2);
                    UnRegistered(user3);
                }
                GameClient remover;
                while (Remover.TryDequeue(out remover))
                {
                    Registered.TryRemove(remover.Player.UID, out remover);
                }
            }
            CreateMatchesStamp.Value = clock.Value + 900;
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
                if (match.Teams[0].Members.Count == 0)
                {
                    match.End(match.Teams[0]);
                    return;
                }
                if (match.Teams[1].Members.Count == 0)
                {
                    match.End(match.Teams[1]);
                    return;
                }
                if (match.Teams[0] == null || match.Teams[1] == null)
                    continue;
                if ((match.Teams[0].ArenaState == Team.StateType.WaitForBox || match.Teams[1].ArenaState == Team.StateType.WaitForBox) && DateTime.Now > match.Teams[0].AcceptBoxShow.AddSeconds(60.0))
                {
                    if (match.Teams[0].ArenaState == Team.StateType.WaitForBox)
                        match.Win(match.Teams[1], match.Teams[0]);
                    else
                        match.Win(match.Teams[0], match.Teams[1]);
                    return;
                }
                if (match.Teams[0].ArenaState == Team.StateType.WaitForOther && !match.Teams[0].AcceptBox)
                    match.Win(match.Teams[1], match.Teams[0]);
                else if (match.Teams[1].ArenaState == Team.StateType.WaitForOther && !match.Teams[1].AcceptBox)
                {
                    match.Win(match.Teams[0], match.Teams[1]);
                }
                else
                {
                    if (match.Teams[0].ArenaState != Team.StateType.WaitForOther || match.Teams[1].ArenaState != Team.StateType.WaitForOther)
                        continue;
                    if (!match.Teams[0].AcceptBox || !match.Teams[1].AcceptBox)
                    {
                        if (!match.Teams[0].AcceptBox)
                            match.Win(match.Teams[1], match.Teams[0]);
                        else
                            match.Win(match.Teams[0], match.Teams[1]);
                    }
                    else
                    {
                        match.Teams[0].ArenaState = (match.Teams[1].ArenaState = Team.StateType.Fight);
                        match.Import();
                    }
                }
            }
            VerifyMatchesStamp.Value = clock.Value + 980;
        }

        public void DoSignup(GameClient client)
        {
            if (client.Team == null)
            {
                client.SendSysMesage("[Team]You do not have a team yet.", MsgMessage.ChatMode.Agate);
                return;
            }
            if (!client.Team.TeamLider(client))
            {
                client.SendSysMesage("[Team]You are not the leader and cannot join.", MsgMessage.ChatMode.Agate);
                return;
            }
            if (client.TeamArenaStatistic.Info.Status != 0)
            {
                client.SendSysMesage("You are already waiting for a match in the arena qualifier.");
                return;
            }
            if (client.InQualifier())
            {
                client.SendSysMesage("You are already waiting for a match in the arena qualifier.");
                return;
            }
            if (BlockArenaMaps.Contains(client.Player.Map))
            {
                client.SendSysMesage($"You need to leave {client.Map.Name} first!");
                return;
            }
            if (!ArenaPoll.ContainsKey(client.Player.UID))
                ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);
            Registered.TryAdd(client.Player.UID, client);
            client.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.WaitingForOpponent;
            client.Team.ArenaState = Team.StateType.FindMatch;
            client.TeamArenaStatistic.Info.Send(client);
        }

        public void UnRegistered(GameClient client)
        {
            Registered.TryRemove(client.Player.UID, out var _);
        }

        public void DoQuit(GameClient client, bool InMathat = false)
        {
            if (client.Team != null && client.Team.TeamLider(client))
            {
                if (client.Team.TeamArenaMatch != null)
                    client.Team.TeamArenaMatch.End(client.Team);
                else
                {
                    client.Team.ResetTeamArena();
                    client.TeamArenaStatistic.Reset();
                }
                UnRegistered(client);
                client.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.NotSignedUp;
                client.TeamArenaStatistic.Info.Send(client);
            }
        }

        public void DoGiveUp(GameClient client)
        {
            if (client.Team == null || !client.Team.TeamLider(client))
                return;
            if (client.Team.ArenaState == Team.StateType.WaitForBox)
            {
                client.Team.AcceptBox = false;
                client.Team.ArenaState = Team.StateType.WaitForOther;
                return;
            }
            client.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.WaitingInactive;
            client.TeamArenaStatistic.Info.Send(client);
            if (!((client.Team != null) & (client.Team.TeamArenaMatch != null)))
                return;
            if (client.Team.TeamArenaMatch.TryGetOpponent(client.Team.UID, out var Opponent))
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Opponent.Leader.Send(stream.TeamArenaSignupCreate(MsgTeamArenaSignup.DialogType.OpponentGaveUp, MsgTeamArenaSignup.DialogButton.SignUp, Opponent.Leader));
                }
                Opponent.Leader.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.NotSignedUp;
                client.TeamArenaStatistic.Info.Status = MsgTeamArenaInfo.Action.NotSignedUp;
                client.TeamArenaStatistic.Info.Send(client);
                Opponent.Leader.TeamArenaStatistic.Info.Send(Opponent.Leader);
            }
            if (!client.Team.TeamArenaMatch.Done)
                client.Team.TeamArenaMatch.End(client.Team);
            else if (Opponent != null)
            {
                client.Team.TeamArenaMatch.Win(Opponent, client.Team);
            }
            else
            {
                client.Team.TeamArenaMatch.End(client.Team);
            }
        }
    }
}
