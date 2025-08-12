﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Game.MsgTournaments
{

    public class MsgSkillTeamPkTournament
    {
        public ProcesType Proces;
        public static MsgTeamEliteGroup[] EliteGroups;

        public MsgSkillTeamPkTournament()
        {
            Create();
        }
        public void Create()
        {
            Proces = ProcesType.Dead;
            EliteGroups = new MsgTeamEliteGroup[(byte)MsgEliteTournament.GroupTyp.Count];

            for (MsgEliteTournament.GroupTyp x = MsgEliteTournament.GroupTyp.EPK_Lvl100Minus; x < MsgEliteTournament.GroupTyp.Count; x++)
            {
                EliteGroups[(byte)x] = new MsgTeamEliteGroup(x, GamePackets.SkillElitePKMatchUI);
            }
        }
        public void Start()
        {
            if (Proces == ProcesType.Dead)
            {
                foreach (var group in EliteGroups)
                    group.CreateWaitingMap();

                Proces = ProcesType.Idle;
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    client.Player.MessageBox("", new Action<Client.GameClient>(p => p.Teleport(448, 251, 1002, 0)), null, 60, MsgServer.MsgStaticMessage.Messages.SkillTeamPKTournament);
                }//The Skill Team PK Tournament will start at 22:00. Prepare yourself and sign up for it as a team!
            }
        }
        public void Save()
        {
            Database.DBActions.Write writer = new Database.DBActions.Write("\\SkillTeamPK.ini");
            for (int x = 0; x < EliteGroups.Length; x++)
            {
                var Tournament = EliteGroups[x];

                for (int i = 0; i < Tournament.Top8.Length; i++)
                {
                    Database.DBActions.WriteLine writerline = new Database.DBActions.WriteLine('/');
                    var element = Tournament.Top8[i];
                    writerline.Add(x).Add(i).Add(element.UID).Add(element.Name).Add(element.Mesh).Add(element.ClaimReward).Add(element.LeaderUID);
                    writer.Add(writerline.Close());
                }

            }
            writer.Execute(Database.DBActions.Mode.Open);
        }
        public void Load()
        {
            Database.DBActions.Read Reader = new Database.DBActions.Read("\\SkillTeamPK.ini");
            if (Reader.Reader())
            {
                int count = Reader.Count;
                for (int x = 0; x < count; x++)
                {
                    Database.DBActions.ReadLine Readline = new Database.DBActions.ReadLine(Reader.ReadString(""), '/');
                    byte Tournament = Readline.Read((byte)0);
                    byte Rank = Readline.Read((byte)0);
                    MsgTeamEliteGroup.FighterStats status = new MsgTeamEliteGroup.FighterStats(Readline.Read((uint)0), Readline.Read(""), Readline.Read((uint)0), null);
                    status.ClaimReward = Readline.Read((byte)0);
                    status.LeaderUID = Readline.Read((uint)0);
                    EliteGroups[Tournament].Top8[Rank] = status;
                }
            }
        }
        public bool SignUp(Client.GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                if (client.Team == null)
                    return false;
                MsgEliteTournament.GroupTyp group = GetGroup(client);
                EliteGroups[(byte)group].SignUp(client);
                return true;
            }
            return false;
        }
        public MsgEliteTournament.GroupTyp GetGroup(Client.GameClient client)
        {
            MsgEliteTournament.GroupTyp tournament = MsgEliteTournament.GroupTyp.EPK_Lvl100Minus;
            ushort level = client.Team.Leader.Player.Level;
            //if (level >= 130)
            //    tournament = MsgEliteTournament.GroupTyp.EPK_Lvl130Plus;
            //else if (level >= 120)
            //    tournament = MsgEliteTournament.GroupTyp.EPK_Lvl120To129;
            //else if (level >= 100)
            //    tournament = MsgEliteTournament.GroupTyp.EPK_Lvl100To119;
            tournament = MsgEliteTournament.GroupTyp.EPK_Lvl130Plus;

            return tournament;
        }

        public bool GetReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            foreach (var tournament in EliteGroups)
            {
                byte Rank = 0;
                if (tournament.GetReward(client, out Rank))
                {
                    ReceiceTitle(tournament, Rank, client);
                    
                    if (Rank == 1)
                    {
                        uint ItemID = GetItemID(tournament, Rank);
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (client.Inventory.HaveSpace(1))
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            else
                                client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);


                        }
                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team Tournament and claimed " + DBItem.Name + ".";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }
                    else if (Rank == 2)
                    {
                        uint ItemID = GetItemID(tournament, Rank);
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (client.Inventory.HaveSpace(1))
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            else
                                client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);


                        }
                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team Tournament and claimed " + DBItem.Name + ".";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }
                    else if (Rank == 3)
                    {
                        uint ItemID = GetItemID(tournament, Rank);
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (client.Inventory.HaveSpace(1))
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            else
                                client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);


                        }
                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team Tournament and claimed " + DBItem.Name + ".";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }
                    else
                    {
                        uint ItemID = GetItemID(tournament, Rank);
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (client.Inventory.HaveSpace(1))
                                client.Inventory.Add(stream, ItemID, 1, 0, 0, 0);
                            else
                                client.Inventory.AddReturnedItem(stream, ItemID, 1, 0, 0, 0);


                        }
                        string MSG = "Congratulation to " + client.Player.Name + " ! he/she managed to get rank " + Rank + " on Skill Team Tournament and claimed " + DBItem.Name + ".";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
                    }

                    return true;
                }
            }
            return false;
        }
        public void GetTitle(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!GetReward(client, stream))
            {
                foreach (var tournament in EliteGroups)
                {
                    byte Rank = 0;
                    if (!tournament.GetReward(client, out Rank) && Rank != 0)
                    {
                        ReceiceTitle(tournament, Rank, client);
                        break;
                    }
                }
            }
        }
        public uint GetItemID(MsgTeamEliteGroup tournament, byte Rank)
        {
            return (uint)(720794 + 1 * (byte)tournament.GroupTyp + Math.Min(3, (Rank - 1)) * 4);
        }
        public void ReceiceTitle(MsgTournaments.MsgTeamEliteGroup tournament, byte Rank, Client.GameClient client)
        {
            if (tournament.GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl130Plus)
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion_High_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_High_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place__High_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8_High_, true);
            }
            else
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion__Low_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_Low_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place_Low_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8__Low_, true);
            }
        }
    }
    public class MsgTeamPkTournament
    {
        public ProcesType Proces;
        public static MsgTeamEliteGroup[] EliteGroups;

        public MsgTeamPkTournament()
        {
            Create();
        }
        public void Create()
        {
            Proces = ProcesType.Dead;
            EliteGroups = new MsgTeamEliteGroup[(byte)MsgEliteTournament.GroupTyp.Count];

            for (MsgEliteTournament.GroupTyp x = MsgEliteTournament.GroupTyp.EPK_Lvl100Minus; x < MsgEliteTournament.GroupTyp.Count; x++)
            {
                EliteGroups[(byte)x] = new MsgTeamEliteGroup(x, GamePackets.TeamElitePKMatchUI);
            }
        }
        public void Start()
        {
            if (Proces == ProcesType.Dead)
            {
                foreach (var group in EliteGroups)
                    group.CreateWaitingMap();

                Proces = ProcesType.Idle;
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    client.Player.MessageBox("", new Action<Client.GameClient>(p => p.Teleport(434, 337, 1002, 0)), null, 60, MsgServer.MsgStaticMessage.Messages.TeamPkTournament);
                }
            }
        }
        public void Save()
        {
            Database.DBActions.Write writer = new Database.DBActions.Write("\\TeamElitePK.ini");
            for (int x = 0; x < EliteGroups.Length; x++)
            {
                var Tournament = EliteGroups[x];

                for (int i = 0; i < Tournament.Top8.Length; i++)
                {
                    Database.DBActions.WriteLine writerline = new Database.DBActions.WriteLine('/');
                    var element = Tournament.Top8[i];
                    writerline.Add(x).Add(i).Add(element.UID).Add(element.Name).Add(element.Mesh).Add(element.ClaimReward).Add(element.LeaderUID);
                    writer.Add(writerline.Close());
                }

            }
            writer.Execute(Database.DBActions.Mode.Open);
        }
        public void Load()
        {
            Database.DBActions.Read Reader = new Database.DBActions.Read("\\TeamElitePK.ini");
            if (Reader.Reader())
            {
                int count = Reader.Count;
                for (int x = 0; x < count; x++)
                {
                    Database.DBActions.ReadLine Readline = new Database.DBActions.ReadLine(Reader.ReadString(""), '/');
                    byte Tournament = Readline.Read((byte)0);
                    byte Rank = Readline.Read((byte)0);
                    MsgTeamEliteGroup.FighterStats status = new MsgTeamEliteGroup.FighterStats(Readline.Read((uint)0), Readline.Read(""), Readline.Read((uint)0), null);
                    status.ClaimReward = Readline.Read((byte)0);
                    status.LeaderUID = Readline.Read((uint)0);
                    EliteGroups[Tournament].Top8[Rank] = status;
                }
            }
        }
        public bool SignUp(Client.GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                if (client.Team == null)
                    return false;
                MsgEliteTournament.GroupTyp group = GetGroup(client);
                EliteGroups[(byte)group].SignUp(client);
                return true;
            }
            return false;
        }
        public MsgEliteTournament.GroupTyp GetGroup(Client.GameClient client)
        {
            MsgEliteTournament.GroupTyp tournament = MsgEliteTournament.GroupTyp.EPK_Lvl100Minus;
            ushort level = client.Team.Leader.Player.Level;
            //if (level >= 130)
            //    tournament = MsgEliteTournament.GroupTyp.EPK_Lvl130Plus;
            //else if (level >= 120)
            //    tournament = MsgEliteTournament.GroupTyp.EPK_Lvl120To129;
            //else if (level >= 100)
            //    tournament = MsgEliteTournament.GroupTyp.EPK_Lvl100To119;
            tournament = MsgEliteTournament.GroupTyp.EPK_Lvl130Plus;

            return tournament;
        }

        public bool GetReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            foreach (var tournament in EliteGroups)
            {
                byte Rank = 0;
                if (tournament.GetReward(client, out Rank))
                {
                    ReceiceTitle(tournament, Rank, client);
                    
                    if (Rank == 1)
                    {
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
                    else if (Rank == 2)
                    {
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
                    else if (Rank == 3)
                    {
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
                    else
                    {
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
                    return true;
                }
            }
            return false;
        }
        public uint GetItemID(MsgTeamEliteGroup tournament, byte Rank)
        {
            return (uint)(720794 + 1 * (byte)tournament.GroupTyp + Math.Min(3, (Rank - 1)) * 4);
        }
        public void GetTitle(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!GetReward(client, stream))
            {
                foreach (var tournament in EliteGroups)
                {
                    byte Rank = 0;
                    if (!tournament.GetReward(client, out Rank) && Rank != 0)
                    {
                        ReceiceTitle(tournament, Rank, client);
                        break;
                    }
                }
            }
        }
        public void ReceiceTitle(MsgTournaments.MsgTeamEliteGroup tournament, byte Rank, Client.GameClient client)
        {
            if (tournament.GroupTyp == MsgEliteTournament.GroupTyp.EPK_Lvl130Plus)
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion_High_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_High_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place__High_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8_High_, true);
            }
            else
            {
                if (Rank == 1) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Champion__Low_, true);
                else if (Rank == 2) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_2nd_Place_Low_, true);
                else if (Rank == 3) client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_3rd_Place_Low_, true);
                else client.Player.AddTitle(MsgEliteTournament.top_typ.Elite_PK_Top_8__Low_, true);
            }
        }
    }
    public class MsgTeamEliteGroup
    {
        public const ushort WaitingAreaID = 2068;

        public class FighterStats
        {
            public enum StatusFlag : uint
            {
                None = 0,
                Fighting = 2,
                Lost = 4,
                Qualified = 3,
                Waiting = 1,
            }
            public Extensions.SafeDictionary<uint, Client.GameClient> Wagers;
            public string Name;
            public uint UID;
            public uint Mesh;
            public uint Wager;
            public uint Cheers;
            public uint Points;
            public byte ClaimReward = 0;
            public Role.Instance.Team Team;

            uint _leaderuid = 0;
            public uint LeaderUID
            {
                get
                {
                    if (Team != null) return Team.Leader.Player.UID;
                    else return _leaderuid;
                }
                set
                {
                    _leaderuid = value;
                }
            }

            public uint LeaderMesh
            {
                get
                {
                    if (Team != null) return Team.Leader.Player.Mesh;
                    return Mesh;
                }
            }

            public override string ToString()
            {
                Database.DBActions.WriteLine writer = new Database.DBActions.WriteLine('/');
                writer.Add(Name).Add(UID).Add(Mesh).Add(ClaimReward).Add(LeaderUID);
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

            public FighterStats(uint id, string name, uint mesh, Role.Instance.Team _team)
            {
                Wagers = new Extensions.SafeDictionary<uint, Client.GameClient>();
                UID = id;
                Team = _team;
                Name = name;
                Mesh = mesh;
                if (_team == null)
                    LeaderUID = id;
                else if (Team.Leader != null)
                    LeaderUID = Team.Leader.Player.UID;
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
                FighterStats stats = new FighterStats(UID, Name, Mesh, Team);
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
                SwitchOut = 3,
                OK = 2,
            }

            public uint TimeLeft
            {
                get
                {
                    int val = (int)((ImportTime.AddSeconds(60 * 2).AllMilliseconds - Extensions.Time32.Now.AllMilliseconds) / 1000);
                    if (val < 0) val = 0;
                    return (uint)val;
                }
            }
            private Extensions.Time32 ImportTime;
            public ushort Index;
            public uint ID;
            public StatusFlag Flag;
            public Extensions.SafeDictionary<uint, Role.Instance.Team> Teams;
            public DateTime ExportTimer = new DateTime();
            public Extensions.SafeDictionary<uint, FighterStats> MatchStats;

            public List<uint> Cheerers = new List<uint>();
            public Extensions.SafeDictionary<uint, Client.GameClient> Watchers = new Extensions.SafeDictionary<uint, Client.GameClient>();
            public Role.Instance.Team[] TeamsFighting
            {
                get
                {
                    return TeamsArray.Where(p => IsFightingTeam(p)).ToArray();
                }
            }


            public bool IsFightingTeam(Role.Instance.Team team)
            {
                foreach (var user in team.Members.Values)
                {
                    if (Predicat()(user.client))
                        return true;
                }
                return false;
            }
            private Func<Client.GameClient, bool> Predicat()
            {
                return new Func<Client.GameClient, bool>(p => p.Player.Map == 700 && p.Player.DynamicID == DinamicID && !p.IsWatching());
            }
            public Role.Instance.Team TeamWaiting = null;

            public FighterStats[] GetMatchStats { get { return MatchStats.GetValues(); } }
            public Role.Instance.Team[] TeamsArray { get { return Teams.GetValues(); } }
            public int GroupID { get { return (int)ID / 100000 - 1; } }
            public int Count { get { return Teams.Count; } }

            private bool Done = false;
            private bool Exported = false;
            private bool Imported = false;

            private Role.GameMap Map;
            public uint DinamicID;

            public MsgTeamEliteGroup elitepkgroup;

            public bool IsFinishd() { return Exported; }
            public Match(MsgTeamEliteGroup elitegroup)
            {
                elitepkgroup = elitegroup;
                Teams = new Extensions.SafeDictionary<uint, Role.Instance.Team>();

                Map = Database.Server.ServerMaps[700];
                DinamicID = Map.GenerateDynamicID();

                Flag = StatusFlag.AcceptingWagers;

                MatchStats = new Extensions.SafeDictionary<uint, FighterStats>();
            }
            public Match AddPlayer(Role.Instance.Team team, FighterStats.StatusFlag flag = FighterStats.StatusFlag.None)
            {
                Teams.Add(team.UID, team);
                team.PkMatch = this;
                team.PKStats = new FighterStats(team.PKStats.UID, team.TeamName, team.Leader.Player.Mesh, team);

                team.PKStats.Flag = flag;


                MatchStats.Add(team.UID, team.PKStats);

                return this;
            }
            public void AddWaiting()
            {
                if (Count == 3)
                {
                    TeamWaiting = TeamsArray[0];
                    TeamWaiting.PKStats.Flag = FighterStats.StatusFlag.Waiting;

                }
            }
            public void CheckFinish()
            {
                if (Count == 1)
                {
                    Done = Exported = true;
                    Flag = StatusFlag.OK;

                    foreach (var user in TeamsArray)
                        user.PKStats.Flag = FighterStats.StatusFlag.Qualified;

                    return;
                }
            }
            public bool CheckTeams()
            {

                if (TeamsArray.Length == 2)
                {
                    var team1 = TeamsArray[0];
                    var team2 = TeamsArray[1];

                    if (!team1.ReadyForTeamPK())
                    {
                        End(team1, true);
                        return false;
                    }
                    if (!team2.ReadyForTeamPK())
                    {
                        End(team2, true);
                        return false;
                    }

                }
                else if (TeamsArray.Length == 3)
                {
                    var team1 = TeamsArray[0];
                    var team2 = TeamsArray[1];
                    var team3 = TeamsArray[2];

                    if (!team1.ReadyForTeamPK())
                    {
                        End(team1, true);
                        return false;
                    }
                    if (!team2.ReadyForTeamPK())
                    {
                        End(team2, true);
                        return false;
                    }
                    if (!team3.ReadyForTeamPK())
                    {
                        End(team3, true);
                        return false;
                    }
                }
                return true;
            }
            public unsafe void SendBrackets(Client.GameClient user, ServerSockets.Packet stream)
            {
                stream.MsgTeamEliteBracketsCreate(MsgServer.MsgTeamElitePKBrackets.Action.StaticUpdate, 0, 0, 1, elitepkgroup.GroupTyp, elitepkgroup.State, 0, 1);
                stream.AddItemTeamElitePKBrackets(this);
                stream.TeamElitePKBracketsFinalize((ushort)(elitepkgroup.PKTournamentID + 2));
                user.Send(stream);
            }
            public unsafe void Import(ServerSockets.Packet stream)
            {
                if (Count == 1)
                {
                    foreach (var user in TeamsArray)
                        user.PKStats.Flag = FighterStats.StatusFlag.Qualified;
                    Flag = StatusFlag.OK;

                    Exported = Done = true;
                    return;
                }

                if (CheckTeams())
                {
                    if (Imported)
                        return;

                    Imported = true;
                    if (Done)
                        return;

                    Flag = StatusFlag.Watchable;

                    ImportTime = Extensions.Time32.Now;

                    if (TeamsArray.Length == 2)
                    {
                        Import(stream, TeamsArray[0], TeamsArray[1]);
                        Import(stream, TeamsArray[1], TeamsArray[0]);
                    }
                    else if (TeamsArray.Length == 3)
                    {
                        if (TeamWaiting.PKStats.Flag != FighterStats.StatusFlag.Lost)
                            TeamWaiting.PKStats.Flag = FighterStats.StatusFlag.Waiting;

                        if (elitepkgroup.pState == States.T_Finished)
                        {
                            Flag = StatusFlag.SwitchOut;

                            var Winner = TeamsArray.Where(p => p.PKStats.Flag == FighterStats.StatusFlag.Qualified).SingleOrDefault();

                            Import(stream, TeamWaiting, Winner);
                            Import(stream, Winner, TeamWaiting);
                        }
                        else
                        {
                            var array = TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Waiting && p.PKStats.Flag != FighterStats.StatusFlag.Lost).ToArray();

                            Import(stream, array[0], array[1]);
                            Import(stream, array[1], array[0]);
                        }
                    }
                    UpdateScore();
                }
            }
            public unsafe void Import(ServerSockets.Packet stream, Role.Instance.Team team, Role.Instance.Team OpponentTeam)
            {
                team.PKStats.Flag = FighterStats.StatusFlag.Fighting;

                team.UpdatePlayers(null,
                    new Action<Client.GameClient>(user =>
                    {
                        ushort x = 0;
                        ushort y = 0;
                        Map.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, 700, DinamicID);
                        user.Player.ProtectJumpAttack(10);

                        user.Player.InTeamPk = true;
                        user.Player.SetPkMode(Role.Flags.PKMode.PK);

                        user.Send(stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.BeginMatch, MsgElitePKMatchUI.EffectTyp.Effect_Lose, OpponentTeam.Leader.Player.UID, OpponentTeam.TeamName, TimeLeft));


                    }));
            }

            public unsafe void UpdateScore()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.TeamElitePKMatchStatsCreate((ushort)(elitepkgroup.PKTournamentID + 1), this);

                    foreach (var team in TeamsArray)
                        team.SendFunc(Predicat(), stream);
                    foreach (var user in Watchers.GetValues())
                    {
                        if (user.Player.Map == 700 && user.Player.DynamicID == DinamicID)
                            user.Send(stream);
                    }

                }
            }
            public unsafe void End(Role.Instance.Team loser, bool Fource)
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

                            var Winner = GetOpponent(loser.UID);

                            Winner.UpdatePlayers(Predicat(), new Action<Client.GameClient>(user =>
                                {
                                    user.ArenaPoints += 1000;
                                    if (user.Inventory.HaveSpace(1))
                                        user.Inventory.Add(stream, 723912, 1);
                                    else
                                        user.Inventory.AddReturnedItem(stream, 723912, 1);

                                }));
                            loser.UpdatePlayers(Predicat(), new Action<Client.GameClient>(user =>
                            {
                                user.ArenaPoints += 1000;
                                if (user.Inventory.HaveSpace(1))
                                    user.Inventory.Add(stream, 723912, 1);
                                else
                                    user.Inventory.AddReturnedItem(stream, 723912, 1);
                            }));

                            Flag = StatusFlag.OK;
                            Winner.PKStats.Flag = FighterStats.StatusFlag.Qualified;
                            loser.PKStats.Flag = FighterStats.StatusFlag.Lost;

                            loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                            Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                            loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                            Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));


                            Console.WriteLine(Winner.TeamName + "= winner " + loser.TeamName + " == loser");
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }
                }
                else if (Count == 3)
                {
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        if (loser.UID == TeamWaiting.UID)
                        {
                            TeamWaiting.PKStats.Flag = FighterStats.StatusFlag.Lost;
                            return;
                        }
                    }

                    if (Done)
                        return;

                    ExportTimer = DateTime.Now.AddSeconds(4);

                    Done = true;

                    if (!Imported)
                        Exported = true;

                    var Winner = GetOpponent(loser.UID);

                    if (Flag == StatusFlag.SwitchOut)
                        Flag = StatusFlag.OK;

                    Winner.PKStats.Flag = FighterStats.StatusFlag.Qualified;
                    loser.PKStats.Flag = FighterStats.StatusFlag.Lost;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Lose, 0, "", 0));
                        Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.Effect, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                        loser.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));
                        Winner.SendFunc(Predicat(), stream.TeamElitePKMatchUICreate(elitepkgroup.PKTournamentID, MsgElitePKMatchUI.State.EndMatch, MsgElitePKMatchUI.EffectTyp.Effect_Win, 0, "", 0));

                        Console.WriteLine(Winner.TeamName + "= winner " + loser.TeamName + " == loser");
                    }
                }
            }
            public Role.Instance.Team GetOpponent(uint UID)
            {
                if (Count == 2)
                {
                    foreach (var team in TeamsArray)
                    {
                        if (team.UID != UID)
                            return team;
                    }
                }
                else if (Count == 3)
                {
                    if (TeamWaiting.PKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        foreach (var team in TeamsArray)
                        {
                            if (team.UID != UID && TeamWaiting.PKStats.UID != team.PKStats.UID)
                                return team;
                        }
                    }
                    if (Flag != StatusFlag.SwitchOut)
                    {
                        foreach (var team in TeamsArray)
                        {
                            if (team.UID != UID && team.PKStats.Flag != FighterStats.StatusFlag.Waiting)
                                return team;
                        }
                    }
                    else if (Flag == StatusFlag.SwitchOut)
                    {
                        foreach (var team in TeamsArray)
                        {
                            if (team.UID != UID && team.PKStats.Flag != FighterStats.StatusFlag.Lost)
                                return team;
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

                    foreach (var team in TeamsFighting)
                    {
                        team.UpdatePlayers(Predicat(), new Action<Client.GameClient>(
                            user =>
                            {
                                ushort x = 0;
                                ushort y = 0;
                                elitepkgroup.Map.GetRandCoord(ref x, ref y);
                                user.Teleport(x, y, MsgEliteGroup.WaitingAreaID, elitepkgroup.DinamycID);
                                user.Player.RestorePkMode();

#if TEST
                                Console.WriteLine("name = " + user.Player.Name + " has teleported to X=" + x + " Y=" + y + "");
#endif
                            }));
                    }
                }


            }
            public void CheckMatch()
            {
                if (TimeLeft == 0)
                {
                    if (Count == 2 && !Done)
                    {
                        var team1 = TeamsArray[0];
                        var team2 = TeamsArray[1];

                        try
                        {
                            if (team1.PKStats.Points > team2.PKStats.Points)
                                End(team2, false);
                            else
                                End(team1, false);
                        }
                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                    }
                    else if (Count == 3 && !Done)
                    {
                        if (Flag != StatusFlag.SwitchOut)
                        {
                            var array = TeamsArray.Where(p => p.UID != TeamWaiting.UID).ToArray();
                            var team1 = array[0];
                            var team2 = array[1];

                            try
                            {
                                if (team1.PKStats.Points > team2.PKStats.Points)
                                    End(team2, false);
                                else
                                    End(team1, false);
                            }
                            catch (Exception e) { Console.WriteLine(e.ToString()); }
                        }
                        else
                        {
                            var team1 = TeamWaiting;
                            var team2 = TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost && p.PKStats.UID != team1.PKStats.UID).SingleOrDefault();

                            try
                            {
                                if (team1.PKStats.Points > team2.PKStats.Points)
                                    End(team2, false);
                                else
                                    End(team1, false);
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
                    if (TeamWaiting.PKStats.Flag == FighterStats.StatusFlag.Lost)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    if (TeamsArray.Where(p => p.UID != TeamWaiting.UID).Where(p => p.PKStats.Flag == FighterStats.StatusFlag.Lost).ToArray().Length == 2)
                    {
                        Flag = StatusFlag.OK;
                        return;
                    }
                    Imported = false;
                    Done = false;
                    Exported = false;
                    Flag = StatusFlag.SwitchOut;
                }
            }

            public unsafe void BeginWatching(ServerSockets.Packet stream, Client.GameClient client)
            {
                if (!client.Player.Alive)
                {
                    client.SendSysMesage("Please revive your character to watching our match.");
                    return;
                }
                if (client.InQualifier() || client.IsWatching())
                {
                    client.SendSysMesage("You're already in a match.");
                    return;
                }
                if (TeamsFighting.Length == 2)
                {
                    if (!Watchers.ContainsKey(client.Player.UID))
                    {
                        Watchers.Add(client.Player.UID, client);
                    }
                    try
                    {

                        stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.RequestView, ID, (uint)TeamsFighting.First().UID, 0, 0, 0);
                        client.Send(stream.ElitePKWatchFinalize());

                        client.TeamElitePkWatchingGroup = this;
                        client.Teleport((ushort)Program.GetRandom.Next(35, 70), (ushort)Program.GetRandom.Next(35, 70), 700, DinamicID);
                        UpdateScore();
                        UpdateWatchers();

                    }
                    catch (Exception e) { Console.WriteLine(e.ToString()); }

                }
            }
            public unsafe void UpdateWatchers()
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Watchers, 0, ID, (uint)Watchers.Count, TeamsFighting[0].PKStats.Cheers, TeamsFighting[1].PKStats.Cheers);

                    foreach (var watch in Watchers.Values)
                        stream.AddItemElitePKWatch(watch.Player.Mesh, watch.Player.Name);

                    stream.ElitePKWatchFinalize();

                    foreach (var user in Watchers.Values)
                        user.Send(stream);
                    foreach (var team in TeamsFighting)
                        team.SendFunc(Predicat(), stream);

                }
            }
            public unsafe void DoLeaveWatching(Client.GameClient client)
            {

                if (client.IsWatching() && Watchers.ContainsKey(client.Player.UID) && client.Player.Map == 700 && client.Player.DynamicID == DinamicID)
                {
                    Watchers.Remove(client.Player.UID);
                    if (TeamsFighting.Length == 2)
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
                    client.TeamElitePkWatchingGroup = null;
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
                        stream.ElitePKWatchCreate(MsgArenaWatchers.WatcherTyp.Fighters, 0, ID, (uint)TeamsFighting.Length, TeamsFighting[0].PKStats.Cheers, TeamsFighting[1].PKStats.Cheers);



                        if (TeamsFighting[0].Members.ContainsKey(uid))
                        {
                            stream.AddItemElitePKWatch(TeamsFighting[0].TeamName, 0);
                            TeamsFighting[0].PKStats.Cheers++;
                        }
                        else if (TeamsFighting.Length > 1 && TeamsFighting[1].Members.ContainsKey(uid))
                        {
                            stream.AddItemElitePKWatch(TeamsFighting[1].TeamName, 0);
                            TeamsFighting[1].PKStats.Cheers++;
                        }
                        else if (TeamsFighting.Length > 2 && TeamsFighting[2].Members.ContainsKey(uid))
                        {
                            stream.AddItemElitePKWatch(TeamsFighting[2].TeamName, 0);
                            TeamsFighting[2].PKStats.Cheers++;
                        }
                        stream.AddItemElitePKWatch(client.Player.Name, 0);
                        stream = stream.ElitePKWatchFinalize();

                        foreach (var team in TeamsArray)
                            team.SendFunc(Predicat(), stream);

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
                        if (Top8[x].LeaderUID == client.Player.UID || Top8[x].Name == client.Player.Name)
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

        public MsgServer.MsgTeamElitePKBrackets.GuiTyp State;
        private States pState = States.T_Organize;
        public FighterStats[] Top8 = new FighterStats[0];

        public uint MatchIndex = 0;
        public DateTime StartTimer = new DateTime();
        public DateTime WaitForFinish = new DateTime();

        public Extensions.SafeDictionary<uint, Role.Instance.Team> Teams;

        public Extensions.SafeDictionary<uint, Match> Matches;
        public Extensions.SafeDictionary<uint, Match> Top4Matches;
        public Extensions.SafeDictionary<uint, Match> ThreeQualiferMatch;
        public Extensions.SafeDictionary<uint, Match> FinalMatch;

        private Extensions.Counter MatchCounter;
        private Extensions.Time32 pStamp;

        public ProcesType Proces;
        public MsgEliteTournament.GroupTyp GroupTyp;
        public ushort PKTournamentID;
        public MsgTeamEliteGroup(MsgEliteTournament.GroupTyp group, ushort _PKTournamentID)
        {
            PKTournamentID = _PKTournamentID;
            Proces = ProcesType.Dead;
            GroupTyp = group;
            Teams = new Extensions.SafeDictionary<uint, Role.Instance.Team>();
            MatchCounter = new Extensions.Counter((uint)((uint)group * 100000 + 100000));

            Top8 = new FighterStats[8];
            for (int x = 0; x < Top8.Length; x++)
                Top8[x] = new FighterStats(0, "None", 0, null);
        }
        public void CreateWaitingMap()
        {

            Map = Database.Server.ServerMaps[WaitingAreaID];
            DinamycID = Map.GenerateDynamicID();

            StartTimer = DateTime.Now.AddSeconds(60 * 15);//4 * 60);//60 * 2 + (byte)GroupTyp * 60);

            if (!Program.BlockAttackMap.Contains(DinamycID))
                Program.BlockAttackMap.Add(DinamycID);

            SubscribeTimer();

        }
        public Extensions.Time32 TimerStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TeamPkStamp);

        public void SubscribeTimer()
        {
            Proces = ProcesType.Idle;
        }

        public unsafe void SignUp(Client.GameClient client)
        {
            if (Proces == ProcesType.Idle)
            {
                if (!Teams.ContainsKey(client.Team.UID))
                    Teams.Add(client.Team.UID, client.Team);



                client.Team.PKStats = new FighterStats(client.Team.UID, client.Team.TeamName, client.Player.Mesh, client.Team);
                client.Player.InTeamPk = true;

                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, WaitingAreaID, DinamycID);

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.TeamElitePKMatchUICreate(PKTournamentID, MsgElitePKMatchUI.State.Information, MsgElitePKMatchUI.EffectTyp.Effect_Lose, client.Player.UID, client.Player.Name, 0);
                    client.Send(stream);

                    if (client.Team.TeamLider(client))
                    {
                        client.Send(stream.TeamEliteSetTeamName((ushort)(PKTournamentID + 10), MsgTeamEliteSetTeamName.State.Apprend, client.Team.UID, client.Team.TeamName));
                    }
                }
            }
        }
        public void CheckTeams()
        {
            List<Role.Instance.Team> Remover = new List<Role.Instance.Team>();
            foreach (var team in Teams.GetValues())
            {
                if (!team.ReadyForTeamPK())
                    Remover.Add(team);
            }
            foreach (var team in Remover)
                Teams.Remove(team.UID);
        }
        public void CreateDoubleMatchs(Extensions.SafeDictionary<uint, Match> Array)
        {

            foreach (var user in Teams.GetValues())
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
            foreach (var user in Teams.GetValues())
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
            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

            Proces = ProcesType.Dead;
            if (Teams != null)
                Teams.Clear();
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
                    if (Teams.Count == 0)
                        return;
                    if (DateTime.Now > StartTimer && Proces == ProcesType.Idle)
                    {
                        Top8 = new FighterStats[8];
                        for (int x = 0; x < Top8.Length; x++)
                            Top8[x] = new FighterStats(0, "None", 0, null);
                        Proces = ProcesType.Alive;
                    }
                    if (Proces == ProcesType.Alive)
                    {
                        if (State == MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking)
                        {

                            CheckTeams();
                            if (Teams.Count == 1)
                            {
                                State = MsgTeamElitePKBrackets.GuiTyp.GUI_ReconstructTop;
                                WaitForFinish = DateTime.Now.AddMinutes(3);
                                ActiveArena(false);
                            }
                            else if (Teams.Count == 2)
                                State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top1;
                            else if (Teams.Count > 2 && Teams.Count <= 4)
                                State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top2Qualifier;
                            else if (Teams.Count > 4 && Teams.Count <= 8)
                                State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top4Qualifier;
                            else if (Teams.Count <= 24)
                                State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Qualifier;
                            else
                                State = MsgTeamElitePKBrackets.GuiTyp.GUI_Knockout;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                foreach (var team in Teams.GetValues())
                                {
                                    foreach (var user in team.Temates)
                                    {
                                        user.client.Send(stream.TeamEliteSetTeamName((ushort)(PKTournamentID + 10), MsgTeamEliteSetTeamName.State.SuccessfulName, team.UID, team.TeamName));
                                        if (team.TeamLider(user.client))
                                        {
                                            user.client.Send(stream.TeamEliteSetTeamName((ushort)(PKTournamentID + 10), MsgTeamEliteSetTeamName.State.Remove, team.UID, team.TeamName));
                                        }

                                    }
                                }
                            }
                            pState = States.T_Organize;
                        }
                        switch (State)
                        {
                            case MsgTeamElitePKBrackets.GuiTyp.GUI_ReconstructTop:
                                {
                                    if (DateTime.Now > WaitForFinish)
                                    {
                                        Finish();
                                    }
                                    break;
                                }
                            case MsgTeamElitePKBrackets.GuiTyp.GUI_Knockout:
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
                                                        State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                        foreach (var team in Teams.GetValues())
                                                        {
                                                            if (team.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                                removers.Add(team);
                                                        }
                                                        foreach (var team in removers)
                                                            Teams.Remove(team.UID);

                                                        Matches.Clear();
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Qualifier:
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
                                                    //SendBrackets(Matches.GetValues(), null, true, 0);
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
                                                    //SendBrackets(Matches.GetValues(), null, true, 0);
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
                                                        State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                        foreach (var team in Teams.GetValues())
                                                        {
                                                            if (team.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                                removers.Add(team);
                                                        }
                                                        foreach (var team in removers)
                                                            Teams.Remove(team.UID);

                                                        Matches.Clear();
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;

                                }
                            case MsgTeamElitePKBrackets.GuiTyp.GUI_Top4Qualifier:
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

                                                //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
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
                                                    //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
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
                                                        State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                        int i = 7;
                                                        foreach (var user in Teams.GetValues())
                                                        {
                                                            if (user.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            {
                                                                removers.Add(user);
                                                                Top8[i--] = user.PKStats.Clone();

                                                            }
                                                        }
                                                        foreach (var team in removers)
                                                            Teams.Remove(team.UID);


                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MsgTeamElitePKBrackets.GuiTyp.GUI_Top2Qualifier:
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
                                                    n_match.AddPlayer(arraymatchs[0].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                    n_match.AddPlayer(arraymatchs[2].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                    Match m_match = new Match(this);
                                                    m_match.Index = (ushort)MatchIndex++;
                                                    m_match.ID = MatchCounter.Next;
                                                    m_match.AddPlayer(arraymatchs[1].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());
                                                    if (arraymatchs.Length > 3)
                                                        m_match.AddPlayer(arraymatchs[3].TeamsArray.Where(p => p.PKStats.Flag != FighterStats.StatusFlag.Lost).SingleOrDefault());

                                                    Top4Matches.Add(m_match.ID, m_match);
                                                    Top4Matches.Add(n_match.ID, n_match);
                                                }


                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in Top4Matches.GetValues())
                                                    match.CheckFinish();
                                                //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                // SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);


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

                                                    //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                    //SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

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


                                                        List<Role.Instance.Team> removers = new List<Role.Instance.Team>();
                                                        ThreeQualiferMatch = new Extensions.SafeDictionary<uint, Match>();

                                                        foreach (var user in Teams.GetValues())
                                                        {
                                                            if (user.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            {
                                                                removers.Add(user);
                                                            }
                                                        }
                                                        if (removers.Count == 1)//for 3 players.
                                                        {
                                                            foreach (var team in removers)
                                                            {
                                                                Teams.Remove(team.UID);
                                                                Top8[2] = team.PKStats;
                                                            }
                                                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        }
                                                        else
                                                        {
                                                            State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top3;
                                                            pState = States.T_Organize;

                                                            MatchIndex = 0;
                                                            Match n_match = new Match(this);
                                                            n_match.Index = (ushort)MatchIndex++;
                                                            n_match.ID = MatchCounter.Next;
                                                            foreach (var team in removers)
                                                            {
                                                                Teams.Remove(team.UID);
                                                                n_match.AddPlayer(team, FighterStats.StatusFlag.Waiting);
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
                            case MsgTeamElitePKBrackets.GuiTyp.GUI_Top3:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in ThreeQualiferMatch.GetValues())
                                                    match.CheckFinish();



                                                //SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                //SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

                                                ActiveArena(true);


                                                FinalMatch = new Extensions.SafeDictionary<uint, Match>();
                                                CreateDoubleMatchs(FinalMatch);
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

                                                    // SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                    //if (Top4Matches != null)
                                                    //SendBrackets(Top4Matches.GetValues(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);
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
                                                        State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                        foreach (var match in ThreeQualiferMatch.GetValues())
                                                        {
                                                            foreach (var team in match.TeamsArray)
                                                            {
                                                                if (team.PKStats.Flag != FighterStats.StatusFlag.Qualified)
                                                                {
                                                                    Top8[3] = team.PKStats.Clone();
                                                                }
                                                                else
                                                                    Top8[2] = team.PKStats.Clone();
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case MsgTeamElitePKBrackets.GuiTyp.GUI_Top1:
                                {
                                    switch (pState)
                                    {
                                        case States.T_Organize:
                                            {
                                                if (FinalMatch == null)
                                                {
                                                    ThreeQualiferMatch = null;
                                                    FinalMatch = new Extensions.SafeDictionary<uint, Match>();

                                                    MatchIndex = 0;
                                                    CreateDoubleMatchs(FinalMatch);

                                                }
                                                pStamp = Extensions.Time32.Now.AddSeconds(60);
                                                pState = States.T_Import;

                                                foreach (var match in FinalMatch.GetValues())
                                                    match.CheckFinish();

                                                //if (Matches != null)
                                                //{
                                                //    SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                //    SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);
                                                //}
                                                //else
                                                //    SendBrackets(ArrayMatchesTop3(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

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

                                                    //if (Matches != null)
                                                    //{
                                                    //    SendBrackets(Matches.GetValues(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.GUIEdit);
                                                    //    SendBrackets(ArrayMatchesTop3(), null, true, 1, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);
                                                    //}
                                                    //else
                                                    //SendBrackets(ArrayMatchesTop3(), null, true, 0, MsgServer.MsgTeamElitePKBrackets.Action.UpdateList);

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

                                                        List<Role.Instance.Team> removers = new List<Role.Instance.Team>();

                                                        foreach (var user in Teams.GetValues())
                                                        {
                                                            if (user.PKStats.Flag == FighterStats.StatusFlag.Lost)
                                                            {
                                                                removers.Add(user);
                                                            }
                                                            else
                                                                Top8[0] = user.PKStats.Clone();
                                                        }

                                                        foreach (var team in removers)
                                                        {
                                                            Top8[1] = team.PKStats.Clone();
                                                            Teams.Remove(team.UID);
                                                        }

                                                        State = MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking;

                                                    }
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                    }
                    TimerStamp.Value = clock.Value + KernelThread.TeamPkStamp;
                }
            }
            catch (Exception e)
            {
                Console.WriteException(e);
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

        public unsafe void ActiveArena(bool active)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.MsgTeamEliteBracketsCreate(MsgTeamElitePKBrackets.Action.EPK_State, 0, 0, 0, GroupTyp, MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Ranking, 0, (uint)(active ? 1 : 0));
                stream.TeamElitePKBracketsFinalize((ushort)(PKTournamentID + 2));
                Program.SendGlobalPackets.Enqueue(stream);
            }
        }
        public unsafe void SendBrackets(Match[] matches, Client.GameClient user, bool sendtoall = false, ushort page = 0
            , MsgServer.MsgTeamElitePKBrackets.Action type = MsgServer.MsgTeamElitePKBrackets.Action.InitialList, bool sendmatch = false)
        {
            int lim = 0, count = 0;
            if (matches == null)
                return;

            if (State == MsgTeamElitePKBrackets.GuiTyp.GUI_Top8Qualifier)
            {
                const byte Max_Count = 5;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.MsgTeamEliteBracketsCreate(MsgTeamElitePKBrackets.Action.InitialList, 0, 0, (ushort)matches.Length, GroupTyp, State, TimeLeft, (ushort)Math.Min(matches.Length, Max_Count));
                    for (int x = 0; x < Max_Count && x < matches.Length; x++)
                        stream.AddItemTeamElitePKBrackets(matches[x]);
                    stream.TeamElitePKBracketsFinalize((ushort)(PKTournamentID + 2));

                    if (user != null) user.Send(stream);
                    if (sendtoall) Program.SendGlobalPackets.Enqueue(stream);


                    if (sendmatch && user != null)
                    {
                        for (int x = 0; x < Max_Count && x < matches.Length; x++)
                            matches[x].SendBrackets(user, stream);
                    }

                    if (matches.Length > Max_Count)
                    {
                        stream.MsgTeamEliteBracketsCreate(MsgTeamElitePKBrackets.Action.InitialList, 0, 1, (ushort)matches.Length, GroupTyp, State, TimeLeft, (ushort)(matches.Length - Max_Count));
                        for (int x = Max_Count; x < matches.Length; x++)
                            stream.AddItemTeamElitePKBrackets(matches[x]);
                        stream.TeamElitePKBracketsFinalize((ushort)(PKTournamentID + 2));

                        if (user != null) user.Send(stream);
                        if (sendtoall) Program.SendGlobalPackets.Enqueue(stream);

                        if (sendmatch && user != null)
                        {
                            for (int x = Max_Count; x < matches.Length; x++)
                                matches[x].SendBrackets(user, stream);
                        }
                    }
                }
            }
            else if (State == MsgTeamElitePKBrackets.GuiTyp.GUI_Knockout)
            {
                if (State == MsgTeamElitePKBrackets.GuiTyp.GUI_Knockout)
                {
                    if (page > matches.Length / 5) page = 0;
                    lim = 5;
                }
                else
                    lim = matches.Length;

                count = Math.Min(lim, matches.Length - page * lim);


                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.MsgTeamEliteBracketsCreate(MsgTeamElitePKBrackets.Action.InitialList, page, 0, (ushort)matches.Length, GroupTyp, State, TimeLeft, (ushort)count);
                    //deci asta e la primul
                    for (int i = page * lim; i < page * lim + count; i++)
                    {
                        stream.AddItemTeamElitePKBrackets(matches[i]);
                    }
                    stream.TeamElitePKBracketsFinalize((ushort)(PKTournamentID + 2));

                    if (user != null) user.Send(stream);
                    if (sendtoall) Program.SendGlobalPackets.Enqueue(stream);

                    if (sendmatch && user != null)
                    {
                        for (int i = page * lim; i < page * lim + count; i++)
                        {
                            matches[i].SendBrackets(user, stream);
                        }
                    }
                }
            }

            else
            {
                count = matches.Length;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    stream.MsgTeamEliteBracketsCreate(MsgTeamElitePKBrackets.Action.InitialList, page, 0, (ushort)(matches.Length * 2), GroupTyp, State, TimeLeft, (ushort)(count));

                    for (int i = 0; i < count; i++)
                    {
                        /*for (int x = 0; x < matches[i].GetMatchStats.Length; x++)
                        {
                            var elemenet = matches[i].GetMatchStats[x];
                            Console.WriteLine(elemenet.Name);
                            Console.WriteLine(elemenet.Flag.ToString());
                        }*/


                        stream.AddItemTeamElitePKBrackets(matches[i]);
                    }
                    stream.TeamElitePKBracketsFinalize((ushort)(PKTournamentID + 2));

                    if (user != null) user.Send(stream);
                    if (sendtoall) Program.SendGlobalPackets.Enqueue(stream);

                    if (sendmatch && user != null)
                    {
                        // for (int i = 0; i < count; i++)
                        //   matches[i].SendBrackets(user, stream);
                    }
                }
            }
        }
    }
}
