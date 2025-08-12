﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Database.DBActions;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgEliteTournament
    {
        public enum GroupTyp : ushort
        {
            EPK_Lvl100Minus,
            EPK_Lvl100To119,
            EPK_Lvl120To129,
            EPK_Lvl130Plus,
            Count
        }

        public class top_typ
        {
            public const byte Elite_PK_Champion__Low_ = 12;

            public const byte Elite_PK_2nd_Place_Low_ = 13;

            public const byte Elite_PK_3rd_Place_Low_ = 14;

            public const byte Elite_PK_Top_8__Low_ = 15;

            public const byte Elite_PK_Champion_High_ = 16;

            public const byte Elite_PK_2nd_Place_High_ = 17;

            public const byte Elite_PK_3rd_Place__High_ = 18;

            public const byte Elite_PK_Top_8_High_ = 19;
        }

        public ProcesType Proces;

        public static MsgEliteGroup[] EliteGroups;

        public MsgEliteTournament()
        {
            Create();
        }

        public void Create()
        {
            Proces = ProcesType.Dead;
            EliteGroups = new MsgEliteGroup[4];
            GroupTyp x;
            x = GroupTyp.EPK_Lvl100Minus;
            while ((int)x < 4)
            {
                EliteGroups[(byte)x] = new MsgEliteGroup(x);
                x++;
            }
        }

        public void Start()
        {
            if (Proces != ProcesType.Dead)
                return;
            MsgEliteGroup[] eliteGroups;
            eliteGroups = EliteGroups;
            foreach (MsgEliteGroup group in eliteGroups)
            {
                group.CreateWaitingMap();
            }
            Proces = ProcesType.Idle;
            foreach (GameClient client in Server.GamePoll.Values)
            {
                client.Player.MessageBox("", delegate (GameClient p)
                {
                    p.Teleport(425, 330, 1002);
                }, null, 60, MsgStaticMessage.Messages.ElitePKTournament);
            }
            try
            {
                ITournamentsAlive.Tournments.Add(13, ": started at(" + DateTime.Now.ToString("H:mm)"));
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not start ElitePK Tournament", true, LogType.WARNING);
            }
        }

        public void Save()
        {
            Write writer;
            writer = new Write("\\ElitePk.ini");
            for (int x = 0; x < EliteGroups.Length; x++)
            {
                MsgEliteGroup Tournament;
                Tournament = EliteGroups[x];
                for (int i = 0; i < Tournament.Top8.Length; i++)
                {
                    WriteLine writerline;
                    writerline = new WriteLine('/');
                    MsgEliteGroup.FighterStats element;
                    element = Tournament.Top8[i];
                    writerline.Add(x).Add(i).Add(element.UID)
                        .Add(element.Name)
                        .Add(element.Mesh)
                        .Add(element.ClaimReward)
                        .Add(element.ServerID);
                    writer.Add(writerline.Close());
                }
            }
            writer.Execute(Mode.Open);
        }

        public void Load()
        {
            Read Reader;
            Reader = new Read("\\ElitePk.ini");
            if (Reader.Reader())
            {
                int count;
                count = Reader.Count;
                for (int x = 0; x < count; x++)
                {
                    ReadLine Readline;
                    Readline = new ReadLine(Reader.ReadString(""), '/');
                    byte Tournament;
                    Tournament = Readline.Read((byte)0);
                    byte Rank;
                    Rank = Readline.Read((byte)0);
                    MsgEliteGroup.FighterStats status;
                    status = new MsgEliteGroup.FighterStats(Readline.Read((uint)0), Readline.Read(""), Readline.Read((uint)0), 0, 0)
                    {
                        ClaimReward = Readline.Read((byte)0),
                        ServerID = Readline.Read((byte)0)
                    };
                    EliteGroups[Tournament].Top8[Rank] = status;
                }
            }
        }

        public bool SignUp(GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                GroupTyp group;
                group = GetGroup(client);
                EliteGroups[(byte)group].SignUp(client);
                return true;
            }
            return false;
        }

        public GroupTyp GetGroup(GameClient client)
        {
            return GroupTyp.EPK_Lvl130Plus;
        }

        public bool GetReward(GameClient client, Packet stream)
        {
            MsgEliteGroup[] eliteGroups;
            eliteGroups = EliteGroups;
            foreach (MsgEliteGroup tournament in eliteGroups)
            {
                if (!tournament.GetReward(client, out var Rank))
                    continue;
                ReceiceTitle(tournament, Rank, client);
                //GetItemID(tournament, Rank);
                if (tournament.GroupTyp == GroupTyp.EPK_Lvl130Plus)
                {
                    if ((uint)(Rank - 1) <= 2)
                    {
                        //int Reward;
                        //Reward = (int)ServerKernel.ELITE_PK_TOURNAMENT_REWARD[Rank - 1];
                        //IEventRewards.Add("ElitePK[" + Rank + "]", (uint)Reward, 0, "", "[" + client.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                        //client.Player.CurrentPoint(Flags.CurrentPoint.GiftCPS, Flags.CurrentPointAction.Add, (uint)Reward);
                        //client.CreateBoxDialog("You've received " + Reward.ToString("0,0") + " ConquerPoints.");
                        uint ItemID = GetItemID(tournament, Rank);
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (client.Inventory.HaveSpace(1))
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            else
                                client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);


                        }
                        string MSG2;
                        MSG2 = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournamentand claimed " + DBItem.Name + "!";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG2, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
                    }
                    else
                    {
                        //int Reward;
                        //Reward = (int)ServerKernel.ELITE_PK_TOURNAMENT_REWARD[3];
                        //IEventRewards.Add("ElitePK[" + Rank + "]", (uint)Reward, 0, "", "[" + client.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                        //client.Player.CurrentPoint(Flags.CurrentPoint.GiftCPS, Flags.CurrentPointAction.Add, (uint)Reward);
                        //client.CreateBoxDialog("You've received " + Reward.ToString("0,0") + " ConquerPoints.");
                        uint ItemID = GetItemID(tournament, Rank);
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (client.Inventory.HaveSpace(1))
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            else
                                client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);


                        }
                        string MSG;
                        MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Elite PK Tournament and claimed " + DBItem.Name + "!";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
                    }
                }
                return true;
            }
            return false;
        }

        public void GetTitle(GameClient client, Packet stream)
        {
            if (GetReward(client, stream))
                return;
            MsgEliteGroup[] eliteGroups;
            eliteGroups = EliteGroups;
            foreach (MsgEliteGroup tournament in eliteGroups)
            {
                if (!tournament.GetReward(client, out var Rank) && Rank != 0)
                {
                    ReceiceTitle(tournament, Rank, client);
                    break;
                }
            }
        }

        public void ReceiceTitle(MsgEliteGroup tournament, byte Rank, GameClient client)
        {
            if (tournament.GroupTyp == GroupTyp.EPK_Lvl130Plus)
            {
                switch (Rank)
                {
                    case 1:
                        client.Player.AddTitle(16, true);
                        break;
                    case 2:
                        client.Player.AddTitle(17, true);
                        break;
                    case 3:
                        client.Player.AddTitle(18, true);
                        break;
                    default:
                        client.Player.AddTitle(19, true);
                        break;
                }
            }
            else
            {
                switch (Rank)
                {
                    case 1:
                        client.Player.AddTitle(12, true);
                        break;
                    case 2:
                        client.Player.AddTitle(13, true);
                        break;
                    case 3:
                        client.Player.AddTitle(14, true);
                        break;
                    default:
                        client.Player.AddTitle(15, true);
                        break;
                }
            }
        }

        public uint GetItemID(MsgEliteGroup tournament, byte Rank)
        {//720797
            return (uint)(720714 + (byte)tournament.GroupTyp + Math.Min(3, Rank - 1) * 4);
        }
    }
    public class MsgEliteGroup
    {
        public const ushort WaitingAreaID = 2068;

        public class FighterStats
        {
            public enum StatusFlag : uint
            {
                None = 0,
                Fighting = 1,
                Lost = 2,
                Qualified = 3,
                Waiting = 4,
                Bye = 5,
                Inactive = 7,
                WonMatch = 8
            }
            public uint CrossEliteRank = 0;
            public Extensions.SafeDictionary<uint, Client.GameClient> Wagers;
            public string Name;
            public uint UID;
            public uint RealUID;
            public uint Mesh;
            public uint Wager;
            public uint Cheers;
            public uint Points;
            public byte ClaimReward = 0;
            public uint ServerID = 0;

            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(Name).Add(UID).Add(Mesh).Add(ClaimReward);
                return writer.Close();
            }

            StatusFlag _flg;
            public StatusFlag Flag
            {
                get { return _flg; }
                set
                {
                    _flg = value;
                    if (_flg == StatusFlag.Qualified)
                        Advanced = true;
                }
            }
            public bool Advanced = false;
            public FighterStats()
            {

            }
            public FighterStats(uint id, string name, uint mesh, uint _ServerID, uint _RealUID)
            {
                Wagers = new Extensions.SafeDictionary<uint, Client.GameClient>();
                UID = id;
                Name = name;
                ServerID = _ServerID;
                Mesh = mesh;
                RealUID = _RealUID;

            }

            public void Reset(bool setflag = false)
            {
                Wagers.Clear();
                Wager = 0;
                Points = 0;
                Cheers = 0;
                Flag = StatusFlag.None;
                if (setflag)
                    Flag = StatusFlag.None;
            }

            public FighterStats Clone()
            {
                FighterStats stats = new FighterStats(UID, Name, Mesh, ServerID, RealUID);
                stats.Points = this.Points;
                stats.Flag = this.Flag;
                stats.Wager = this.Wager;
                return stats;
            }
        }
        public class Match
        {
            public enum StatusFlag : ushort
            {
                AcceptingWagers = 0,
                Watchable = 1,
                SwitchOut = 2,
                InFight = 3,
                OK = 4
            }

            public uint TimeLeft
            {
                get
                {
                    int val = (int)((ImportTime.AddSeconds(60 * 3).AllMilliseconds - Extensions.Time32.Now.AllMilliseconds) / 1000);
                    if (val < 0) val = 0;
                    return (uint)val;
                }
            }
            private Extensions.Time32 ImportTime;

            public ushort Index;
            public uint ID;
            public StatusFlag Flag;
            public Extensions.SafeDictionary<uint, Client.GameClient> Players;
            public DateTime ExportTimer = new DateTime();
            public Extensions.SafeDictionary<uint, FighterStats> MatchStats;

            public List<uint> Cheerers = new List<uint>();
            public Extensions.SafeDictionary<uint, Client.GameClient> Watchers = new Extensions.SafeDictionary<uint, Client.GameClient>();
            public Client.GameClient[] PlayersFighting
            {
                get
                {
                    return PlayersArray.Where(p => p.Player.Map == 700 && p.Player.DynamicID == DinamicID && !p.IsWatching()).ToArray();
                }
            }

            public Client.GameClient PlayerWaiting = null;

            public FighterStats[] GetMatchStats
            {
                get
                {
                    if (elitepkgroup.State == MsgElitePKBrackets.GuiTyp.GUI_Top8Qualifier)
                    {
                        FighterStats[] stats = new FighterStats[PlayersArray.Length];
                        for (int x = 0; x < stats.Length; x++)
                            stats[x] = PlayersArray[x].ElitePKStats;
                        return stats;
                    }
                    else
                        return MatchStats.GetValues();
                }
            }//MatchStats.GetValues(); } }
            public Client.GameClient[] PlayersArray { get { return Players.GetValues(); } }
            public int GroupID { get { return (int)ID / 100000 - 1; } }
            public int Count { get { return Players.Count; } }

            private bool Done = false;
            private bool Exported = false;
            private bool Imported = false;

            private Role.GameMap Map;
            public uint DinamicID;

            public MsgEliteGroup elitepkgroup;

            public bool IsFinishd() { return Exported; }
            public Match(MsgEliteGroup elitegroup)
            {
                elitepkgroup = elitegroup;
                Players = new Extensions.SafeDictionary<uint, Client.GameClient>();

                Map = Database.Server.ServerMaps[700];
                DinamicID = Map.GenerateDynamicID();

                Flag = StatusFlag.AcceptingWagers;

                MatchStats = new Extensions.SafeDictionary<uint, FighterStats>();
            }
            public Match AddPlayer(Client.GameClient user, FighterStats.StatusFlag flag = FighterStats.StatusFlag.None)
            {
                Players.Add(user.Player.UID, user);

                user.ElitePkMatch = this;
                user.ElitePKStats = new FighterStats(user.Player.UID, user.Player.Name, user.Player.Mesh, user.Player.ServerID, user.Player.RealUID);

                user.ElitePKStats.Flag = flag;

                MatchStats.Add(user.Player.UID, user.ElitePKStats);

                return this;
            }
            public void AddWaiting()
            {
                if (Count == 3)
                {
                    PlayerWaiting = PlayersArray[0];
                    PlayerWaiting.ElitePKStats.Flag = FighterStats.StatusFlag.Waiting;

                }
            }
            public void CheckFinish()
            {
                if (Count == 1)
                {
                    Done = Exported = true;
                    Flag = StatusFlag.OK;

                    foreach (var user in PlayersArray)
                        user.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;

                    return;
                }
            }
            public bool CheckPlayers()
            {

                if (PlayersArray.Length == 2)
                {
                    var user1 = PlayersArray[0];
                    var user2 = PlayersArray[1];

                    if (user1.Socket != null && (!user1.Socket.Alive || !user1.Player.InElitePk))
                    {
                        End(user1, true);
                        return false;
                    }
                    if (user2.Socket != null && (!user2.Socket.Alive || !user2.Player.InElitePk))
                    {
                        End(user2, true);
                        return false;
                    }
                }
                else if (PlayersArray.Length == 3)
                {
                    var user1 = PlayerWaiting;

                    var array = PlayersArray.Where(a => a.Player.UID != PlayerWaiting.Player.UID).ToArray();
                    var user2 = array[0];
                    var user3 = array[1];

                    if (user1.Socket != null && (!user1.Socket.Alive || !user1.Player.InElitePk))
                    {
                        End(user1, true);
                        return false;
                    }
                    if (user2.Socket != null && (!user2.Socket.Alive || !user2.Player.InElitePk))
                    {
                        End(user2, true);
                        return false;
                    }
                    if (user3.Socket != null && (!user3.Socket.Alive || !user3.Player.InElitePk))
                    {
                        End(user3, true);
                        return false;
                    }
                }
                return true;
            }
            public unsafe void SendBrackets(Client.GameClient user, ServerSockets.Packet stream)
            {
                stream.ElitePKBracketsCreate(MsgServer.MsgElitePKBrackets.Action.StaticUpdate, 0, 1, elitepkgroup.GroupTyp, elitepkgroup.State, 0, 1);
                stream.AddItemElitePKBrackets(this);
                stream.ElitePKBracketsFinalize();
                user.Send(stream);
            }
            public unsafe void Import(ServerSockets.Packet stream)
            {
                if (Count == 1)
                {
                    foreach (var user in PlayersArray)
                        user.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;
                    Flag = StatusFlag.OK;

                    Exported = Done = true;
                    return;
                }

                if (CheckPlayers())
                {
                    if (Imported)
                        return;

                    Imported = true;
                    if (Done)
                        return;

                    Flag = StatusFlag.Watchable;

                    ImportTime = Extensions.Time32.Now;

                    if (PlayersArray.Length == 2)
                    {
                        Import(stream, PlayersArray[0], PlayersArray[1]);
                        Import(stream, PlayersArray[1], PlayersArray[0]);
                    }
                    else if (PlayersArray.Length == 3)
                    {
                        if (PlayerWaiting.ElitePKStats.Flag != FighterStats.StatusFlag.Lost)
                            PlayerWaiting.ElitePKStats.Flag = FighterStats.StatusFlag.Waiting;

                        if (elitepkgroup.pState == States.T_Finished)
                        {
                            Flag = StatusFlag.SwitchOut;

                            var Winner = PlayersArray.Where(p => p.ElitePKStats.Flag == FighterStats.StatusFlag.Qualified).SingleOrDefault();

                            Import(stream, PlayerWaiting, Winner);
                            Import(stream, Winner, PlayerWaiting);
                        }
                        else
                        {
                            var array = PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Waiting && p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).ToArray();

                            Import(stream, array[0], array[1]);
                            Import(stream, array[1], array[0]);
                        }
                    }
                    UpdateScore();
                }
            }
            public unsafe void Import(ServerSockets.Packet stream, Client.GameClient user, Client.GameClient Opponent)
            {
                user.ElitePKStats.Flag = FighterStats.StatusFlag.Fighting;
                user.ElitePkMatch = this;
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, 700, DinamicID);
                user.Player.ProtectJumpAttack(10);
                user.Player.SetPkMode(Role.Flags.PKMode.PK);

                stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.BeginMatch, MsgElitePKMatchUI.EffectTyp.Effect_Lose, Opponent.Player.UID, Opponent.Player.Name, TimeLeft);

                user.Send(stream);

            }

            public unsafe void UpdateScore()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKMatchStatsCreate(this);

                    foreach (var user in PlayersArray)
                    {
                        if (user.Player.Map == 700 && user.Player.DynamicID == DinamicID)
                            user.Send(stream);
                    }
                    foreach (var user in Watchers.GetValues())
                    {
                        if (user.Player.Map == 700 && user.Player.DynamicID == DinamicID)
                            user.Send(stream);
                    }

                }
            }
            public unsafe void End(Client.GameClient loser, bool Fource)
            {


                if (Count == 2)
                {
                    if (!Imported)
                        Exported = true;
                    if (Done)
                        return;

                    ExportTimer = DateTime.Now.AddSeconds(4);

                    Done = true;

                    try
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            var Winner = GetOpponent(loser.Player.UID);


                            Winner.ArenaPoints += 1000;
                            loser.ArenaPoints += 1000;

                            if (Winner.Inventory.HaveSpace(1))
                                Winner.Inventory.Add(stream, 723912, 1);
                            else
                                Winner.Inventory.AddReturnedItem(stream, 723912, 1);


                            if (loser.Inventory.HaveSpace(1))
                                loser.Inventory.Add(stream, 723912, 1);
                            else
                                loser.Inventory.AddReturnedItem(stream, 723912, 1);



                            Flag = StatusFlag.OK;
                            Winner.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;
                            loser.ElitePKStats.Flag = FighterStats.StatusFlag.Lost;


                            loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                            Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                            loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                            Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

#if TEST
                            Console.WriteLine(Winner.Player.Name +"= winner " + loser.Player.Name + " == loser");
#endif
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }
                }
                else if (Count == 3)
                {
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        if (loser.Player.UID == PlayerWaiting.Player.UID)
                        {
                            PlayerWaiting.ElitePKStats.Flag = FighterStats.StatusFlag.Lost;
                            return;
                        }
                    }

                    if (Done)
                        return;

                    ExportTimer = DateTime.Now.AddSeconds(4);

                    Done = true;

                    if (!Imported)
                        Exported = true;

                    var Winner = GetOpponent(loser.Player.UID);

                    if (Flag == StatusFlag.SwitchOut)
                        Flag = StatusFlag.OK;

                    Winner.ElitePKStats.Flag = FighterStats.StatusFlag.Qualified;
                    loser.ElitePKStats.Flag = FighterStats.StatusFlag.Lost;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                        Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                        loser.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                        Winner.Send(stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
#if TEST
                        Console.WriteLine(Winner.Player.Name + "= winner " + loser.Player.Name + " == loser");
#endif
                    }
                }
            }
            public Client.GameClient GetOpponent(uint UID)
            {
                if (Count == 2)
                {
                    foreach (var user in PlayersArray)
                    {
                        if (user.Player.UID != UID)
                            return user;
                    }
                }
                else if (Count == 3)
                {
                    if (PlayerWaiting.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.UID != UID && PlayerWaiting.ElitePKStats.UID != user.ElitePKStats.UID)
                                return user;
                        }
                    }
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.UID != UID && user.ElitePKStats.Flag != FighterStats.StatusFlag.Waiting)
                                return user;
                        }
                    }
                    else if (Flag == StatusFlag.SwitchOut)
                    {
                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.UID != UID && user.ElitePKStats.Flag != FighterStats.StatusFlag.Lost)
                                return user;
                        }
                    }
                }
                return null;
            }
            public void Export()
            {
                if (!Imported)
                {
                    return;
                }

                if (DateTime.Now > ExportTimer && Done)
                {
                    if (Exported)
                        return;

                    Exported = true;

                    foreach (var user in Watchers.GetValues())
                    {
                        DoLeaveWatching(user);
                    }

                    foreach (var user in PlayersFighting)
                    {

                        ushort x = 0;
                        ushort y = 0;
                        elitepkgroup.Map.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, MsgEliteGroup.WaitingAreaID, elitepkgroup.DinamycID);
                        user.Player.RestorePkMode();
                        user.ElitePkMatch = null;
                        Console.WriteLine("name = " + user.Player.Name + " has teleported to X=" + x + " Y=" + y + "");
                    }
                }


            }
            public void CheckMatch()
            {
                if (TimeLeft == 0)
                {
                    if (Count == 2 && !Done)
                    {
                        var user1 = PlayersArray[0];
                        var user2 = PlayersArray[1];

                        try
                        {
                            if (user1.ElitePKStats.Points > user2.ElitePKStats.Points)
                                End(user2, false);
                            else
                                End(user1, false);
                        }
                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                    }
                    else if (Count == 3 && !Done)
                    {
                        if (Flag != StatusFlag.SwitchOut)
                        {
                            var array = PlayersArray.Where(p => p.Player.UID != PlayerWaiting.Player.UID).ToArray();
                            var user1 = array[0];
                            var user2 = array[1];

                            try
                            {
                                if (user1.ElitePKStats.Points > user2.ElitePKStats.Points)
                                    End(user2, false);
                                else
                                    End(user1, false);
                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                        }
                        else
                        {
                            var user1 = PlayerWaiting;


                            var user2 = PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost && p.ElitePKStats.UID != user1.ElitePKStats.UID).SingleOrDefault();

                            try
                            {
                                if (user1.ElitePKStats.Points > user2.ElitePKStats.Points)
                                    End(user2, false);
                                else
                                    End(user1, false);
                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                        }
                    }
                }
            }
            public void Switch()
            {
                if (Count == 3)
                {
                    if (PlayerWaiting.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    if (PlayersArray.Where(p => p.Player.UID != PlayerWaiting.Player.UID).Where(p => p.ElitePKStats.Flag == FighterStats.StatusFlag.Lost).ToArray().Length == 2)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    Imported = false;
                    Done = false;
                    Exported = false;
                    Flag = StatusFlag.SwitchOut;
                    PlayerWaiting.ElitePkMatch = this;
                    PlayerWaiting.ElitePKStats = new FighterStats(PlayerWaiting.Player.UID, PlayerWaiting.Player.Name, PlayerWaiting.Player.Mesh, PlayerWaiting.Player.ServerID, PlayerWaiting.Player.RealUID);
                }
            }

            public unsafe void BeginWatching(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (!client.Player.Alive)
                {
                    client.SendSysMesage("Please revive your character to watching that match");
                    return;
                }
                if (client.InQualifier() || client.IsWatching())
                {
                    client.SendSysMesage("You're already in a match.");
                    return;
                }
                if (PlayersFighting.Length == 2)
                {
                    if (!Watchers.ContainsKey(client.Player.UID))
                    {
                        Watchers.Add(client.Player.UID, client);

                        try
                        {

                            stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.RequestView, 0, ID, (uint)PlayersFighting.Length, PlayersFighting[0].ElitePKStats.Cheers, PlayersFighting[1].ElitePKStats.Cheers);

                            foreach (var Fighter in PlayersFighting)
                                stream.AddItemElitePKWatch(Fighter.Player.UID, Fighter.Player.Name);

                            client.Send(stream.ElitePKWatchFinalize());

                            client.ElitePkWatchingGroup = this;
                            client.Teleport((ushort)Program.GetRandom.Next(35, 70), (ushort)Program.GetRandom.Next(35, 70), 700, DinamicID);
                            UpdateScore();
                            UpdateWatchers();

                        }
                        catch (Exception e) { Console.WriteLine(e.ToString()); }

                    }
                }
            }
            public unsafe void UpdateWatchers()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, ID, (uint)Watchers.Count, PlayersFighting[0].ElitePKStats.Cheers, PlayersFighting[1].ElitePKStats.Cheers);

                    foreach (var watch in Watchers.Values)
                        stream.AddItemElitePKWatch(watch.Player.Mesh, watch.Player.Name);

                    stream.ElitePKWatchFinalize();

                    foreach (var user in Watchers.Values)
                        user.Send(stream);
                    foreach (var user in PlayersFighting)
                        user.Send(stream);

                }
            }
            public unsafe void DoLeaveWatching(Client.GameClient client)
            {

                if (client.IsWatching() && Watchers.ContainsKey(client.Player.UID) && client.Player.Map == 700 && client.Player.DynamicID == DinamicID)
                {
                    Watchers.Remove(client.Player.UID);
                    if (PlayersFighting.Length == 2)
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Leave, 0, ID, 0, 0, 0);
                            client.Send(stream.ElitePKWatchFinalize());
                        }


                    }
                    UpdateWatchers();
                    UpdateScore();
                    client.ElitePkWatchingGroup = null;
                    client.TeleportCallBack();

                }
                client.ElitePkWatchingGroup = null;
            }
            public unsafe void DoCheer(Client.GameClient client, uint uid)
            {
                if (client.IsWatching() && !Cheerers.Contains(client.Player.UID))
                {
                    Cheerers.Add(client.Player.UID);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Fighters, 0, ID, (uint)PlayersFighting.Length, PlayersFighting[0].ElitePKStats.Cheers, PlayersFighting[1].ElitePKStats.Cheers);



                        if (PlayersFighting[0].Player.UID == uid)
                        {
                            stream.AddItemElitePKWatch(PlayersFighting[0].Player.Name, 0);
                            PlayersFighting[0].ElitePKStats.Cheers++;
                        }
                        else if (PlayersFighting[1].ElitePKStats.UID == uid)
                        {
                            stream.AddItemElitePKWatch(PlayersFighting[1].Player.Name, 0);
                            PlayersFighting[1].ElitePKStats.Cheers++;
                        }
                        stream.AddItemElitePKWatch(client.Player.Name, 0);
                        stream = stream.ElitePKWatchFinalize();

                        foreach (var user in PlayersArray)
                        {
                            if (user.Player.Map == 700 && user.Player.DynamicID == DinamicID)
                                user.Send(stream);
                        }
                        foreach (var user in Watchers.GetValues())
                        {
                            if (user.Player.Map == 700 && user.Player.DynamicID == DinamicID)
                                user.Send(stream);
                        }
                    }
                    UpdateWatchers();
                    UpdateScore();
                }
            }

        }
        public enum States : byte
        {
            T_Organize = 0,
            T_CreateMatches = 1,
            T_Import = 2,
            T_Fights = 3,
            T_Finished = 4,
            T_ReOrganize = 5
        }
        public bool GetReward(Client.GameClient client, out byte Rank)
        {
            if (Top8.Length == 8)
            {
                for (int x = 0; x < Top8.Length; x++)
                {
                    if (Top8[x] != null)
                    {
                        if (Top8[x].UID == client.Player.UID)
                        {
                            if (Top8[x].ClaimReward == 0)
                            {
                                Top8[x].ClaimReward = 1;
                                Rank = (byte)(x + 1);
                                return true;
                            }
                            else
                            {
                                Rank = (byte)(x + 1);
                                return false;
                            }
                        }
                    }
                }
            }
            Rank = 0;
            return false;
        }
        public Role.GameMap Map;
        public uint DinamycID;
        //private IDisposable Subscriber;

        public MsgServer.MsgElitePKBrackets.GuiTyp State;
        private States pState = States.T_Organize;
        public FighterStats[] Top8 = new FighterStats[0];

        public uint MatchIndex = 0;
        public DateTime StartTimer = new DateTime();
        public DateTime WaitForFinish = new DateTime();

        public Extensions.SafeDictionary<uint, Client.GameClient> Players;

        public Extensions.SafeDictionary<uint, Match> Matches;
        public Extensions.SafeDictionary<uint, Match> Top4Matches;
        public Extensions.SafeDictionary<uint, Match> ThreeQualiferMatch;
        public Extensions.SafeDictionary<uint, Match> FinalMatch;

        private Extensions.Counter MatchCounter;
        private Extensions.Time32 pStamp;

        public ProcesType Proces;
        public MsgEliteTournament.GroupTyp GroupTyp;
        public MsgEliteGroup(MsgEliteTournament.GroupTyp group)
        {
            Proces = ProcesType.Dead;
            GroupTyp = group;
            Players = new Extensions.SafeDictionary<uint, Client.GameClient>();
            MatchCounter = new Extensions.Counter((uint)((uint)group * 100000 + 100000));

            Top8 = new FighterStats[8];
            for (int x = 0; x < Top8.Length; x++)
                Top8[x] = new FighterStats(0, "None", 0, 0, 0);
        }
        public void CreateWaitingMap()
        {

            Map = Database.Server.ServerMaps[WaitingAreaID];
            DinamycID = Map.GenerateDynamicID();

            StartTimer = DateTime.Now.AddMinutes(5);     //+ (byte)GroupTyp * 60);

            if (!Program.BlockAttackMap.Contains(DinamycID))
                Program.BlockAttackMap.Add(DinamycID);

            SubscribeTimer();

        }

        public Extensions.Time32 TimerStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.ElitePkStamp);

        public void SubscribeTimer()
        {
            Proces = ProcesType.Idle;
        }

        public unsafe void SignUp(Client.GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                if (!Players.ContainsKey(client.Player.UID))
                    Players.Add(client.Player.UID, client);



                client.ElitePKStats = new FighterStats(client.Player.UID, client.Player.Name, client.Player.Mesh, client.Player.ServerID, client.Player.RealUID);
                client.Player.InElitePk = true;

                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, WaitingAreaID, DinamycID);

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKMatchUICreate(MsgElitePKMatchUI.State.Information, MsgElitePKMatchUI.EffectTyp.Effect_Lose, client.Player.UID, client.Player.Name, 0);
                    client.Send(stream);
                }
            }
        }
        public void CheckPlayers()
        {
            List<Client.GameClient> Remover = new List<Client.GameClient>();
            foreach (var user in Players.GetValues())
            {
                if ((user.Player.Map != WaitingAreaID || user.Player.DynamicID != DinamycID) && user.IsWatching() == false)
                {
                    if (user.Player.Map != 700)
                        Remover.Add(user);
                }
                if (user.Fake == false)
                {
                    if (user.Socket.Alive == false)
                        Remover.Add(user);
                }
            }
            foreach (var user in Remover)
                Players.Remove(user.Player.UID);
        }
        public void CreateDoubleMatchs(Extensions.SafeDictionary<uint, Match> Array)
        {

            foreach (var user in Players.GetValues())
            {
                Match match = GetDoubleImcompleteMatch(Array);
                match.AddPlayer(user);
                if (!Array.ContainsKey(match.ID))
                {
                    Array.Add(match.ID, match);
                }
            }
        }
        public Match GetDoubleImcompleteMatch(Extensions.SafeDictionary<uint, Match> Array)
        {
            foreach (var match in Array.GetValues())
            {
                if (match.Count < 2)
                    return match;
            }
            Match n_match = new Match(this);
            n_match.Index = (ushort)MatchIndex++;
            n_match.ID = MatchCounter.Next;
            return n_match;
        }
        public Match GetTripleImcompleteMatch(Extensions.SafeDictionary<uint, Match> Array)
        {
            foreach (var match in Array.GetValues())
            {
                if (match.Count == 1)
                    return match;
            }
            foreach (var match in Array.GetValues())
            {
                if (match.Count <= 2)
                    return match;
            }
            Match n_match = new Match(this);
            n_match.Index = (ushort)MatchIndex++;
            n_match.ID = MatchCounter.Next;
            return n_match;
        }
        public void CreateTripleMatchs(Extensions.SafeDictionary<uint, Match> Array)
        {
            foreach (var user in Players.GetValues())
            {
                if (Complete8Match())
                {
                    Match match = GetDoubleImcompleteMatch(Array);
                    match.AddPlayer(user);
                    if (!Array.ContainsKey(match.ID))
                        Array.Add(match.ID, match);

                }
                else
                {
                    GetTripleImcompleteMatch(Array).AddPlayer(user).AddWaiting();

                }
            }
        }
        public bool Complete8Match()
        {
            return Matches.Count < 8;
        }
        public ushort TimeLeft
        {
            get
            {
                int value = (int)((pStamp.AllMilliseconds - Extensions.Time32.Now.AllMilliseconds) / 1000);
                if (value < 0) return 0;
                return (ushort)value;
            }
        }
        public void Finish()
        {

            //if (Program.ServerConfig.IsInterServer)
            //{
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        stream.ElitePkRankingCreate(MsgElitePkRanking.RankType.Top8Cross, (uint)this.GroupTyp, State, (uint)Top8.Length, 0);
            //        for (int x = 0; x < Top8.Length; x++)
            //            stream.AddItemElitePkRanking(Top8[x], (byte)(x + 1));
            //        stream.InterServerElitePkRankingFinalize();
            //        MsgInterServer.PipeServer.Send(stream);
            //    }
            //}
            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

            Proces = ProcesType.Dead;
            if (Players != null)
                Players.Clear();
            if (Matches != null)
                Matches.Clear();
            if (Top4Matches != null)
                Top4Matches.Clear();
            if (ThreeQualiferMatch != null)
                ThreeQualiferMatch.Clear();
            if (FinalMatch != null)
                FinalMatch.Clear();
        }
        public void timerCallback(Extensions.Time32 clock)
        {
            try
            {
                if (clock > TimerStamp)
                {

                    if (DateTime.Now > StartTimer && Proces == ProcesType.Idle)
                    {
                        Top8 = new FighterStats[8];
                        for (int x = 0; x < Top8.Length; x++)
                            Top8[x] = new FighterStats(0, "None", 0, 0, 0);
                        Proces = ProcesType.Alive;

                        if (Players.Count == 0)
                        {
                            Finish();
                            return;

                        }
                    }
                    if (Players.Count == 0)
                        return;
                    if (Proces == ProcesType.Alive)
                    {
                        if (State == MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking)
                        {

                            CheckPlayers();
                            if (Players.Count == 1)
                            {
                                State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_ReconstructTop;
                                WaitForFinish = DateTime.Now.AddMinutes(3);
                                ActiveArena(false);
                            }
                            else if (Players.Count == 2)
                                State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top1;
                            else if (Players.Count > 2 && Players.Count <= 4)
                                State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top2Qualifier;
                            else if (Players.Count > 4 && Players.Count <= 8)
                                State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top4Qualifier;
                            else if (Players.Count <= 24)
                                State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Qualifier;
                            else
                                State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Knockout;

                            pState = States.T_Organize;
                        }
                        switch (State)
                        {
                            case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_ReconstructTop:
                                {
                                    if (DateTime.Now > WaitForFinish)
                                    {
                                        Finish();
                                    }
                                    break;
                                }
                            case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Knockout:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                MatchIndex = 0;
                                                Matches = new Extensions.SafeDictionary<uint, Match>();
                                                CreateDoubleMatchs(Matches);
                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in Matches.GetValues())
                                                    match.CheckFinish();

                                                //SendBrackets(Matches.GetValues(), null, true, 0);
                                                ActiveArena(true);

                                                break;
                                            }
                                        case States.T_Import:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();

                                                        foreach (var match in Matches.GetValues())
                                                            match.Import(stream);
                                                    }
                                                    pState = States.T_Fights;
                                                }
                                                break;
                                            }
                                        case States.T_Fights:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    int FinishMatchs = 0;
                                                    foreach (var match in Matches.GetValues())
                                                    {
                                                        match.CheckMatch();
                                                        match.Export();
                                                        if (match.IsFinishd())
                                                            FinishMatchs++;
                                                    }
                                                    if (FinishMatchs == Matches.Count)
                                                    {
                                                        State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        List<Client.GameClient> removers = new List<Client.GameClient>();
                                                        foreach (var user in Players.GetValues())
                                                        {
                                                            if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                                removers.Add(user);
                                                        }
                                                        foreach (var player in removers)
                                                            Players.Remove(player.Player.UID);

                                                        Matches.Clear();
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Qualifier:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                MatchIndex = 0;
                                                Matches = new Extensions.SafeDictionary<uint, Match>();
                                                CreateTripleMatchs(Matches);

                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in Matches.GetValues())
                                                    match.CheckFinish();

                                                //SendBrackets(Matches.GetValues(), null, true, 0);
                                                ActiveArena(true);
                                                break;
                                            }
                                        case States.T_Import:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();

                                                        foreach (var match in Matches.GetValues())
                                                        {
                                                            match.Import(stream);
                                                        }
                                                    }
                                                    pState = States.T_Fights;
                                                }
                                                break;
                                            }
                                        case States.T_Fights:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    int FinishMatchs = 0;
                                                    foreach (var match in Matches.GetValues())
                                                    {
                                                        match.CheckMatch();
                                                        match.Export();
                                                        if (match.IsFinishd())
                                                            FinishMatchs++;
                                                    }
                                                    if (FinishMatchs == Matches.Count)
                                                    {
                                                        pState = States.T_ReOrganize;
                                                    }
                                                }
                                                break;
                                            }
                                        case States.T_ReOrganize:
                                            {
                                                foreach (var match in Matches.GetValues())
                                                {
                                                    match.Switch();
                                                }
                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                //SendBrackets(Matches.GetValues(), null, true, 0);

                                                pState = States.T_Finished;
                                                break;
                                            }

                                        case States.T_Finished:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();

                                                        foreach (var match in Matches.GetValues())
                                                        {
                                                            match.Import(stream);

                                                        }
                                                    }
                                                    pState = States.T_CreateMatches;
                                                }
                                                break;
                                            }
                                        case States.T_CreateMatches:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    int FinishMatchs = 0;
                                                    foreach (var match in Matches.GetValues())
                                                    {
                                                        match.CheckMatch();
                                                        match.Export();
                                                        if (match.IsFinishd())
                                                            FinishMatchs++;
                                                    }
                                                    if (FinishMatchs == Matches.Count)
                                                    {
                                                        State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        List<Client.GameClient> removers = new List<Client.GameClient>();
                                                        foreach (var user in Players.GetValues())
                                                        {
                                                            if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                                removers.Add(user);
                                                        }
                                                        foreach (var player in removers)
                                                            Players.Remove(player.Player.UID);

                                                        Matches.Clear();
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;

                                }
                            case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top4Qualifier:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                MatchIndex = 0;
                                                Matches = new Extensions.SafeDictionary<uint, Match>();
                                                CreateDoubleMatchs(Matches);
                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in Matches.GetValues())
                                                    match.CheckFinish();

                                                //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                                //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);
                                                ActiveArena(true);

                                                break;
                                            }
                                        case States.T_Import:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();

                                                        foreach (var match in Matches.GetValues())
                                                            match.Import(stream);
                                                    }
                                                    pState = States.T_Fights;
                                                }
                                                break;
                                            }
                                        case States.T_Fights:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    int FinishMatchs = 0;
                                                    foreach (var match in Matches.GetValues())
                                                    {
                                                        match.CheckMatch();
                                                        match.Export();
                                                        if (match.IsFinishd())
                                                            FinishMatchs++;
                                                    }
                                                    if (FinishMatchs == Matches.Count)
                                                    {
                                                        int i = 4;
                                                        foreach (var match in Matches.Values)
                                                        {
                                                            foreach (var user in match.Players.GetValues())
                                                            {
                                                                if (user.ElitePKStats != null && user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                                {
                                                                    Top8[Math.Min(7, i++)] = user.ElitePKStats.Clone();
                                                                }
                                                            }

                                                        }

                                                        List<Client.GameClient> removers = new List<Client.GameClient>();
                                                        foreach (var user in Players.GetValues())
                                                        {
                                                            if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            {
                                                                removers.Add(user);
                                                            }
                                                        }
                                                        foreach (var player in removers)
                                                            Players.Remove(player.Player.UID);

                                                        State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top2Qualifier:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                MatchIndex = 0;
                                                if (Top4Matches == null)
                                                    Top4Matches = new Extensions.SafeDictionary<uint, Match>();
                                                if (Matches == null || Matches != null && Matches.Count == 0)
                                                {
                                                    Matches = new Extensions.SafeDictionary<uint, Match>();
                                                    CreateDoubleMatchs(Top4Matches);
                                                    if (Matches.Count == 0)
                                                    {
                                                        foreach (var match in Top4Matches.GetValues())
                                                            Matches.Add(match.ID, match);
                                                    }
                                                }
                                                else
                                                {
                                                    Match n_match = new Match(this);
                                                    n_match.Index = (ushort)MatchIndex++;
                                                    n_match.ID = MatchCounter.Next;

                                                    var arraymatchs = Matches.GetValues();
                                                    n_match.AddPlayer(arraymatchs[0].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                    n_match.AddPlayer(arraymatchs[2].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                    Match m_match = new Match(this);
                                                    m_match.Index = (ushort)MatchIndex++;
                                                    m_match.ID = MatchCounter.Next;
                                                    m_match.AddPlayer(arraymatchs[1].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                    if (arraymatchs.Length > 3)
                                                        m_match.AddPlayer(arraymatchs[3].PlayersArray.Where(p => p.ElitePKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                    Top4Matches.Add(m_match.ID, m_match);
                                                    Top4Matches.Add(n_match.ID, n_match);
                                                }


                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in Top4Matches.GetValues())
                                                    match.CheckFinish();

                                                //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                                //if (Top4Matches != null)
                                                //    SendBrackets(Top4Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);

                                                ActiveArena(true);

                                                break;
                                            }
                                        case States.T_Import:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();

                                                        foreach (var match in Top4Matches.GetValues())
                                                            match.Import(stream);
                                                    }
                                                    pState = States.T_Fights;
                                                }
                                                break;
                                            }
                                        case States.T_Fights:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    int FinishMatchs = 0;
                                                    foreach (var match in Top4Matches.GetValues())
                                                    {
                                                        match.CheckMatch();
                                                        match.Export();
                                                        if (match.IsFinishd())
                                                            FinishMatchs++;
                                                    }
                                                    if (FinishMatchs == Top4Matches.Count)
                                                    {


                                                        List<Client.GameClient> removers = new List<Client.GameClient>();
                                                        ThreeQualiferMatch = new Extensions.SafeDictionary<uint, Match>();

                                                        foreach (var user in Players.GetValues())
                                                        {
                                                            if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            {
                                                                removers.Add(user);
                                                            }
                                                        }
                                                        if (removers.Count == 1)//for 3 players.
                                                        {
                                                            foreach (var player in removers)
                                                            {
                                                                Players.Remove(player.Player.UID);
                                                                Top8[2] = player.ElitePKStats;
                                                            }
                                                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        }
                                                        else
                                                        {
                                                            State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top3;
                                                            pState = States.T_Organize;

                                                            MatchIndex = 0;
                                                            Match n_match = new Match(this);
                                                            n_match.Index = (ushort)MatchIndex++;
                                                            n_match.ID = MatchCounter.Next;
                                                            foreach (var player in removers)
                                                            {
                                                                Players.Remove(player.Player.UID);
                                                                n_match.AddPlayer(player);
                                                            }
                                                            ThreeQualiferMatch.Add(n_match.ID, n_match);
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top3:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in ThreeQualiferMatch.GetValues())
                                                    match.CheckFinish();

                                                //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                                //if (ThreeQualiferMatch != null)
                                                //    SendBrackets(ThreeQualiferMatch.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);
                                                ActiveArena(true);

                                                break;
                                            }
                                        case States.T_Import:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();

                                                        foreach (var match in ThreeQualiferMatch.GetValues())
                                                            match.Import(stream);
                                                    }
                                                    pState = States.T_Fights;
                                                }
                                                break;
                                            }
                                        case States.T_Fights:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    int FinishMatchs = 0;
                                                    foreach (var match in ThreeQualiferMatch.GetValues())
                                                    {
                                                        match.CheckMatch();
                                                        match.Export();
                                                        if (match.IsFinishd())
                                                            FinishMatchs++;
                                                    }
                                                    if (FinishMatchs == ThreeQualiferMatch.Count)
                                                    {
                                                        State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        foreach (var match in ThreeQualiferMatch.GetValues())
                                                        {
                                                            foreach (var user in match.PlayersArray)
                                                            {
                                                                if (user.ElitePKStats.Flag != FighterStats.StatusFlag.Qualified)
                                                                {
                                                                    Top8[3] = user.ElitePKStats.Clone();
                                                                }
                                                                else
                                                                    Top8[2] = user.ElitePKStats.Clone();
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top1:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                ThreeQualiferMatch = null;
                                                FinalMatch = new Extensions.SafeDictionary<uint, Match>();

                                                MatchIndex = 0;
                                                CreateDoubleMatchs(FinalMatch);

                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in FinalMatch.GetValues())
                                                    match.CheckFinish();

                                                //if (Matches != null)
                                                //    SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.GUIEdit);
                                                //if (Top4Matches != null)
                                                //    SendBrackets(Top4Matches.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);
                                                //if (FinalMatch != null)
                                                //    SendBrackets(FinalMatch.GetValues(), null, true, 0, MsgServer.MsgElitePKBrackets.Action.UpdateList);

                                                ActiveArena(true);

                                                break;
                                            }
                                        case States.T_Import:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    using (var rec = new ServerSockets.RecycledPacket())
                                                    {
                                                        var stream = rec.GetStream();

                                                        foreach (var match in FinalMatch.GetValues())
                                                            match.Import(stream);
                                                    }

                                                    pState = States.T_Fights;
                                                }
                                                break;
                                            }
                                        case States.T_Fights:
                                            {
                                                if (TimeLeft == 0)
                                                {
                                                    int FinishMatchs = 0;
                                                    foreach (var match in FinalMatch.GetValues())
                                                    {
                                                        match.CheckMatch();
                                                        match.Export();
                                                        if (match.IsFinishd())
                                                            FinishMatchs++;
                                                    }
                                                    if (FinishMatchs == FinalMatch.Count)
                                                    {

                                                        List<Client.GameClient> removers = new List<Client.GameClient>();

                                                        foreach (var user in Players.GetValues())
                                                        {
                                                            if (user.ElitePKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            {
                                                                removers.Add(user);
                                                            }
                                                            else
                                                                Top8[0] = user.ElitePKStats.Clone();
                                                        }

                                                        foreach (var player in removers)
                                                        {
                                                            Top8[1] = player.ElitePKStats.Clone();
                                                            Players.Remove(player.Player.UID);
                                                        }

                                                        State = MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                    }
                    TimerStamp.Value = clock.Value + KernelThread.ElitePkStamp;
                }
            }
            catch (Exception e)
            {
                Console.WriteException(e);
            }
        }
        public unsafe void ActiveArena(bool active)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.ElitePKBracketsCreate(MsgElitePKBrackets.Action.EPK_State, 0, 0, GroupTyp, MsgElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, (uint)(active ? 1 : 0));
                stream.ElitePKBracketsFinalize();
                Program.SendGlobalPackets.Enqueue(stream);
            }
        }

        public Match[] ArrayMatchesTop3()
        {
            Match[] array = new Match[(Top4Matches != null ? Top4Matches.Count : 0)
                + (ThreeQualiferMatch != null ? ThreeQualiferMatch.Count : 0) + (FinalMatch != null ? FinalMatch.Count : 0)];
            int position = -1;
            for (int x = 0; x < (Top4Matches != null ? Top4Matches.Count : 0); x++)
                array[++position] = Top4Matches.GetValues()[x];
            for (int x = 0; x < (ThreeQualiferMatch != null ? ThreeQualiferMatch.Count : 0); x++)
                array[++position] = ThreeQualiferMatch.GetValues()[x];
            for (int x = 0; x < (FinalMatch != null ? FinalMatch.Count : 0); x++)
                array[++position] = FinalMatch.GetValues()[x];
            return array;
        }

        public unsafe void SendBrackets(Match[] matches, Client.GameClient user, bool sendtoall = false, ushort page = 0
            , MsgServer.MsgElitePKBrackets.Action type = MsgServer.MsgElitePKBrackets.Action.InitialList, bool sendmatch = false, ushort PacketNo = 0)
        {
            int lim = 0, count = 0;
            if (matches == null)
                return;
            if (State == MsgServer.MsgElitePKBrackets.GuiTyp.GUI_Knockout)
            {
                if (page > matches.Length / 5) 
                    page = 0;
                lim = 5;
            }
            else
                lim = matches.Length;

            count = Math.Min(lim, matches.Length - page * lim);


            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                {

                    stream.ElitePKBracketsCreate(type, page, (ushort)matches.Length, GroupTyp, State, TimeLeft, (ushort)count, PacketNo);

                    for (int i = page * lim; i < page * lim + count; i++)
                        stream.AddItemElitePKBrackets(matches[i]);

                    stream.ElitePKBracketsFinalize();

                    if (user != null)
                        user.Send(stream);
                    if (sendtoall)
                        Program.SendGlobalPackets.Enqueue(stream);
                }



                if (sendmatch && user != null)
                {
                    /* for (int i = page * lim; i < page * lim + count; i++)
                        {
                            matches[i].SendBrackets(user, stream);
                        }*/
                }
            }
        }
    }
}
