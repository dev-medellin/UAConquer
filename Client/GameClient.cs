using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.Role;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.ServerCore;
using TheChosenProject.ServerSockets;
using TheChosenProject.ServerCore.Website;
using TheChosenProject.Database;
using System.Collections.Concurrent;
using System.Text;
using TheChosenProject.Cryptography;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Role.Instance;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using Extensions;
using System.Threading.Tasks;
using TheChosenProject.Struct;
using System.Linq;
using System.Windows.Forms;
using TheChosenProject.Game.MsgEvents;
using static TheChosenProject.DropRule;
using static MongoDB.Driver.WriteConcern;
using Mysqlx.Expr;
using MongoDB.Driver.Core.Configuration;
using System.Reflection;
using TheChosenProject.Game.MsgServer.AttackHandler;


namespace TheChosenProject.Client
{
    public class GameClient
    {
        #region Loader System
        ////////LoaderINFO///////
        public uint EncryptTokenSpell;
        public DateTime LoaderTime = DateTime.Now;
        public uint StampThreadMemory = 0;
        public uint StampThreadTimer = 0;

        public bool TerminateLoader = false;
        public bool ActiveClient = false;
        public List<string> OpenedProcesses = new List<string>();
        public CMsgGuardShield.GuardShield Guard;
        #endregion
        public Game.MsgMonster.MonsterPet Pet;
        
        public DateTime AimbotCheck = DateTime.Now;
        //public void UpdateEquipsLvl(GameClient client, ServerSockets.Packet stream)
        //{

        //    foreach (var item in Equipment.ClientItems.Values)
        //    {
        //        if (item.Position == (ushort)Role.Flags.ConquerItem.RightWeapon ||
        //            item.Position == (ushort)Role.Flags.ConquerItem.LeftWeapon ||
        //            item.Position == (ushort)Role.Flags.ConquerItem.Necklace ||
        //            item.Position == (ushort)Role.Flags.ConquerItem.Ring ||
        //            item.Position == (ushort)Role.Flags.ConquerItem.Boots ||
        //            item.Position == (ushort)Role.Flags.ConquerItem.Head ||
        //            item.Position == (ushort)Role.Flags.ConquerItem.Armor)
        //        {

                    
        //            bool suc;
        //            int nextid = Database.Server.ItemsBase[item.ITEM_ID].Level + 5;
        //            uint itemid = Database.Server.ItemsBase[item.ITEM_ID].ID;
        //            if (nextid < Player.Level && item.Bound == 1
        //                && nextid <= 120)

        //            {

        //                item.ITEM_ID = Database.Server.ItemsBase.UpdateItem(itemid, out suc);
        //                if (suc == true)
        //                {
        //                    item.Mode = Role.Flags.ItemMode.Update;
        //                    item.Send(this, stream);
        //                }
        //                if (item.Position != 0)
        //                    client.Equipment.QueryEquipment(client.Equipment.Alternante);
        //            }
                    
        //        }
        //    }
        //}


        internal int DBMobs2 = 0, MaxDB = 0;
        internal int EmeraldMobs = 0, MaxEmeraldMobs = 0;
        internal int HWMobs = 0, MaxHWMobs = 0;
        internal int PowerEXPBallMobs = 0, MaxPowerEXPBall = 0;
        internal int LetterMobs = 0, MaxLetter = 0;
        internal int FruitsMobs = 0, MaxFruits = 0;
        internal int CityMobs = 0, MaxCity = 0;
        internal int PumpkinsMobs = 0, MaxPumpkins = 0;
        internal int PumpkinSeedsMobs = 0, MaxPumpkinSeeds = 0;
        public DbDailyTraining DbDailyTraining;
        public DbKillMobsExterminator DbKillMobsExterminator;
        public OnlinePointsManager OnlinePointsManager;
        public TournamentsManager TournamentsManager;
        public RoyalPassManager RoyalPassManager;
        public LimitedDailyTimes LimitedDailyTimes;
        
        public DateTime LastVote;
        public Ai.Object Bot;
        public DateTime BotBuffersStampJump = DateTime.Now;
        public DateTime StampMonsterCityAlret = DateTime.Now;

        public int PointBufferLocation = -1;

        public bool petAttack = false;

        private bool _mining = false;
        public bool Mining
        {
            get { return _mining; }
            set
            {
                _mining = value;
            }
        }
        public bool Agree;
        public bool PassMining = false;
        public Extensions.Time32 NextMine;
        public int HitStackes = 0;
        public int MaxHitStackes = 0;
        public uint TargetObjBot = 0;
        //internal int SpeedHackSuspiction;
        //internal int SpeedHackSuspiction2;
        public void StopMining()
        {
            Mining = false;
            PassMining = false;
        }
        public uint TokenSpell = 0;
        public DateTime ProtectPingTime = DateTime.Now;
        public bool PrintProcesses = false;
        public DateTime SendPingStamp = DateTime.Now;
        public DateTime LastCheckTime = DateTime.Now;
        public List<string> ProcessesList = new List<string>();
        public DateTime SendCheckStamp = DateTime.Now;
        public bool LoaderDisconnect = false;

        public Translator.Language Language = Translator.Language.EN;

        private Time32 LastClientMove = Time32.Now;

        public List<string> Get_NpcDailog = new List<string>();

        public List<string> Get_NpcOption = new List<string>();

        public uint AddInputValue;

        public AIEnum.AIStatus AIStatus;

        public AIEnum.AIType AIType;

        public int _pathfinder_length;

        public PathFinder.Point[] pathfinder;

        public MonsterRole MobTarget;

        public Player Target;

        public SobNpc SobTarget;

        public byte BanCount;

        public Npc OnRemoveNpc;


        public Extensions.Time32 BuffersStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.User_Buffers);
        //public Extensions.Time32 JumpStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.JUMP);
        public Extensions.Time32 StaminStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.User_Stamina);
        public Extensions.Time32 AttackStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.User_AutoAttack);
        public Extensions.Time32 XPCountStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.User_StampXPCount);
        public Extensions.Time32 CheckSecondsStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.User_CheckSeconds);
        public Extensions.Time32 CheckItemsView = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.User_CheckItems);
        public Extensions.Time32 AutoHuntingStamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.AUTO_HUNTING);
        public Extensions.Time32 AI_Bot_Stamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.AI_Bot);
        //public Extensions.Time32 AI_vendor_stamp = Extensions.Time32.Now.AddMilliseconds(MapGroupThread.AI_vendor);
        public uint MobsKilled = 0, TotalMobsKilled = 0;


        public uint OTPsInput;

        public int TerainMask;

        public ulong ExpOblivion;

        public byte TRyDisconnect = 2;

        public Time32 LastVIPTeleport;

        public Time32 LastRefresh;

        public Time32 LastVIPTeamTeleport;

        public MsgTeamArena.User TeamArenaStatistic;

        public MsgArena.User ArenaStatistic;

        public MsgArena.Match ArenaMatch;

        public MsgArena.Match ArenaWatchingGroup;

        public MsgTeamArena.Match TeamArenaWatchingGroup;

        public MsgTeamEliteGroup.Match TeamElitePkWatchingGroup;

        public MsgEliteGroup.FighterStats ElitePKStats;

        private MsgEliteGroup.Match _tet;

        public MsgEliteGroup.Match ElitePkWatchingGroup;

        public const int DefaultDefense2 = 10000;

        public House MyHouse;

        public ushort MoveNpcMesh;

        public uint MoveNpcUID;

        public uint UseItem;

        public ConcurrentDictionary<RoleStatus.StatuTyp, RoleStatus> ExtraStatus;

        public uint RebornGem;

        //public Vendor MyVendor;
        public Role.Instance.Vendor MyVendor;
        public bool IsVendor
        {
            get
            {
                if (MyVendor != null)
                    return MyVendor.InVending;
                return false;
            }
        }
        public Trade MyTrade;

        public bool FullLoading;

        public uint Vigor;

        //public InteractQuery AutoAttack;

        //private bool _OnAutoAttack;

        public uint AcceptedGuildID;

        public uint ConnectionUID;

        public MsgLoginClient OnLogin;
        public Game.MsgEvents.Events EventBase;

        public uint ActiveNpc;

        public ServerFlag ClientFlag;

        public Player Player;
        public Role.Instance.DemonExterminator DemonExterminator;

        public GameCryptography Cryptography;

        public DHKeyExchange.ServerKeyExchange DHKeyExchance;

        public SecuritySocket Socket;

        public GameMap Map;

        public Team Team;

        public ushort[] Gems = new ushort[13];

        public MsgStatus Status = new MsgStatus();

        public Warehouse Warehouse;

        public Equip Equipment;

        public Inventory Inventory;

        public Proficiency MyProfs;

        public Spell MySpells;

        public Confiscator Confiscator;

        public bool Fake;

        private uint uidnext = 550;

        public DateTime ItemStamp;

        internal int DBMobs;
        internal int CountItemsMobs;
        internal int MaxCountItemMobs;

        internal int MetsMobs;

        internal int ProfTokenMobs;

        internal int MaxDBMobs;

        internal int MaxMetsMobs;
        internal int MaxProfTokenMobs;
        public uint EntityID
        { get { return Player.UID; } }
        public object Name
        { get { return Player.Name; } }
        internal string IP;
        public void ExitToTwin()
        {
            SendSysMesage("", MsgMessage.ChatMode.FirstRightCorner);
            switch (Program.Rand.Next(14))
            {
                case 1: Teleport(439, 368, 1002, 0, true, true); break;
                case 2: Teleport(439, 368, 1002, 0, true, true); break;
                case 3: Teleport(439, 368, 1002, 0, true, true); break;
                case 4: Teleport(439, 368, 1002, 0, true, true); break;
                case 5: Teleport(439, 368, 1002, 0, true, true); break;
                case 6: Teleport(439, 368, 1002, 0, true, true); break;
                case 7: Teleport(439, 368, 1002, 0, true, true); break;
                case 8: Teleport(439, 368, 1002, 0, true, true); break;
                case 9: Teleport(439, 368, 1002, 0, true, true); break;
                case 10: Teleport(439, 368, 1002, 0, true, true); break;
                case 11: Teleport(439, 368, 1002, 0, true, true); break;
                case 12: Teleport(439, 368, 1002, 0, true, true); break;
                default: Teleport(439, 368, 1002, 0, true, true); break;
            }
        }
        public MsgEliteGroup.Match ElitePkMatch
        {
            get
            {
                return _tet;
            }
            set
            {
                _tet = value;
            }
        }

        public uint ArenaPoints
        {
            get
            {
                if (ArenaStatistic == null)
                    return 0;
                return ArenaStatistic.Info.ArenaPoints;
            }
            set
            {
                if (ArenaStatistic != null)
                    ArenaStatistic.Info.ArenaPoints = value;
            }
        }

        public async void CharReadyCreate(ServerSockets.Packet stream)
        {
            Task delay = Task.Delay(1000);
            await delay;
            unsafe
            {
                if (Socket.Alive)
                {
                    ActionQuery action = new ActionQuery()
                    {
                        Type = ActionType.OpenCustom,
                        dwParam = Game.MsgServer.CustomCommands.LoginScreen,
                    };
                    Send(stream.ActionCreate(&action));
                    //Socket.Disconnect();
                }
            }
        }

        public uint TeamArenaPoints
        {
            get
            {
                if (TeamArenaStatistic == null)
                    return 0;
                return TeamArenaStatistic.Info.ArenaPoints;
            }
            set
            {
                if (TeamArenaStatistic != null)
                    TeamArenaStatistic.Info.ArenaPoints = value;
            }
        }

        public uint HonorPoints
        {
            get
            {
                if (ArenaStatistic == null)
                    return 0;
                return ArenaStatistic.Info.CurrentHonor;
            }
            set
            {
                if (ArenaStatistic != null)
                    ArenaStatistic.Info.CurrentHonor = value;
            }
        }

        public uint TeamArenaHonorPoints
        {
            get
            {
                if (TeamArenaStatistic == null)
                    return 0;
                return TeamArenaStatistic.Info.CurrentHonor;
            }
            set
            {
                if (TeamArenaStatistic != null)
                    TeamArenaStatistic.Info.CurrentHonor = value;
            }
        }

        //public bool IsVendor
        //{
        //    get
        //    {
        //        if (MyVendor != null)
        //            return MyVendor.InVending;
        //        return false;
        //    }
        //}

        public bool ProjectManager
        {
            get
            {
                //if (ServerKernel.Test_Center)
                //    return true;
                return Player.Name.Contains("[OWNER]");
            }
        }

        public bool GeneralManager
        {
            get
            {
                //if (ServerKernel.Test_Center)
                //    return true;
                return Player.Name.Contains("[GM]");
            }
        }
        public bool HelpDesk
        {
            get
            {
                //if (ServerKernel.Test_Center)
                //    return true;
                return Player.Name.Contains("[HP]");
            }
        }
        public bool InTrade
        {
            get
            {
                if (MyTrade != null)
                    return MyTrade.WindowOpen;
                return false;
            }
        }
        public unsafe Game.MsgServer.InteractQuery AutoAttack = default(Game.MsgServer.InteractQuery);
        private bool _OnAutoAttack = false;

        public bool OnAutoAttack
        {

            get { return _OnAutoAttack; }
            set
            {
                _OnAutoAttack = value;
                if (value == false)
                {
                }
            }
        }

        public uint AjustDefense
        {
            get
            {
                uint defence;
                defence = Status.Defence;
                uint nDefence;
                nDefence = 0;
                if (Player.ContainFlag(MsgUpdate.Flags.MagicShield) || Player.OnDefensePotion || Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Shield))
                    nDefence += (uint)(Base.MulDiv((int)defence, 120, 100) - (int)defence);
                return defence + nDefence;
            }
        }

      

        public bool InLastManStanding = false;
        public bool InQuarantine = false;
        public bool InBombMadness = false;
        public bool InSSFB = false;
        public bool InST = false;
        public bool InPassTheBomb = false;
        public byte FIveOutHits = 0;
        public bool InFIveOut = false;
        public byte TDMHits = 0;
        public bool InTDM = false;
        public bool OnWhiteTeam = false;
        public bool GeneratorItemDrop(Status type)
        {
            StaticDrop TempStatic = Program.DropRuleBase.GetStatic(this, type);
            if (TempStatic != null && !TempStatic.InFloor)
                return false;
            switch (type)
            {
                case DropRule.Status.Pumpkins:
                    {
                        PumpkinsMobs = 0;
                        MaxPumpkins = Program.GetRandom.Next((int)TempStatic.MinValue, (int)TempStatic.MaxValue);
                        break;
                    }
                case DropRule.Status.PumpkinSeeds:
                    {
                        PumpkinSeedsMobs = 0;
                        MaxPumpkinSeeds = Program.GetRandom.Next((int)TempStatic.MinValue, (int)TempStatic.MaxValue);
                        break;
                    }
                case DropRule.Status.All:
                    {
                        for (byte i = 1; i < (byte)DropRule.Status.Count; i++)
                        {
                            GeneratorItemDrop((Status)i);
                        }
                        break;
                    }
                case DropRule.Status.DB:
                    {
                        DBMobs = 0;
                        MaxDB = Program.GetRandom.Next((int)TempStatic.MinValue, (int)TempStatic.MaxValue);
                        break;
                    }
                case DropRule.Status.PowerEXPBall:
                    {
                        PowerEXPBallMobs = 0;
                        MaxPowerEXPBall = Program.GetRandom.Next((int)TempStatic.MinValue, (int)TempStatic.MaxValue);
                        break;
                    }
                case DropRule.Status.Letter:
                    {
                        LetterMobs = 0;
                        MaxLetter = Program.GetRandom.Next((int)TempStatic.MinValue, (int)TempStatic.MaxValue);
                        break;
                    }
                case DropRule.Status.Fruits:
                    {
                        FruitsMobs = 0;
                        MaxFruits = Program.GetRandom.Next((int)TempStatic.MinValue, (int)TempStatic.MaxValue);
                        break;
                    }
                case DropRule.Status.City:
                    {
                        CityMobs = 0;
                        MaxCity = Program.GetRandom.Next((int)TempStatic.MinValue, (int)TempStatic.MaxValue);
                        break;
                    }
            }
            TempStatic = null;
            return true;
        }

        public bool CheckMove(uint Timestamp)
        {
            Time32 ReceiveTime;
            ReceiveTime = new Time32(Timestamp);
            if (!HasFlag())
            {
                if (ReceiveTime.Value - LastClientMove.Value <= 500)//bahaa
                {
                    Player.CountSpeedHack++;
                    if (Player.CountSpeedHack == 3)
                    {
                        //ServerKernel.Log.ServerLogCheats(Player.Name + " been suspected of speed hacking.");
                        System.Console.WriteLine(string.Format(""+Player.Name + " been suspected of speed hacking."));
                        //Socket.Disconnect();
                        return false;
                    }
                }
                else
                    Player.CountSpeedHack = 0;
            }
            LastClientMove = ReceiveTime;
            return true;
        }
        public IDisposable[] Subscribers;
        private bool disposedSubscribers;
        internal bool CheckTerminateTimers()
        {
            if (Subscribers != null)
            {
                bool available = Socket.Alive && Player != null;
                if (!available && !disposedSubscribers)
                {
                    disposedSubscribers = true;
                    foreach (var subscriber in Subscribers)
                        subscriber.Dispose();
                    return true;
                }
                return !available;
            }
            return true;
        }
        public bool HasFlag()
        {
            bool ContainFlag;
            ContainFlag = false;
            if (Player.ContainFlag(MsgUpdate.Flags.Cyclone))
                ContainFlag = true;
            if (Player.ContainFlag(MsgUpdate.Flags.Fly))
                ContainFlag = true;
            if (Player.ContainFlag(MsgUpdate.Flags.Oblivion))
                ContainFlag = true;
            if (Player.ContainFlag(MsgUpdate.Flags.Ride))
                ContainFlag = true;
            if (Player.OnTransform)
                ContainFlag = true;
            return ContainFlag;
        }

        public bool InSkillTeamPk()
        {
            if (Team != null && Team.PkMatch != null && Team.PkMatch.elitepkgroup.PKTournamentID == 2250)
                return Player.InTeamPk;
            return false;
        }

        internal bool IsWatching()
        {
            if (ArenaWatchingGroup == null && TeamArenaWatchingGroup == null && ElitePkWatchingGroup == null)
                return TeamElitePkWatchingGroup != null;
            return true;
        }

        internal bool InQualifier()
        {
            if ((ArenaStatistic.ArenaState == MsgArena.User.StateType.None || ArenaMatch == null) && (Team == null || Team.TeamArenaMatch == null) && ElitePkMatch == null)
            {
                if (Team != null)
                    return Team.PkMatch != null;
                return false;
            }
            return true;
        }

        internal bool InTeamQualifier()
        {
            if (Team != null)
            {
                if (Team.TeamArenaMatch == null)
                    return Team.PkMatch != null;
                return true;
            }
            return false;
        }

        internal void EndQualifier()
        {
            ArenaMatch?.End(this);
            if (Team != null && Team.TeamArenaMatch != null)
            {
                if (Team.TeamLider(this) && Team.Members.Count <= 1)
                {
                    Team.TeamArenaMatch.End(Team);
                    return;
                }
                if (Team.IsDead(700))
                    Team.TeamArenaMatch.End(Team);
            }
            ElitePkMatch?.End(this, true);
            if (Team != null && Team.PkMatch != null)
            {
                if (Team.TeamLider(this) && Team.Members.Count <= 1)
                    Team.PkMatch.End(Team, true);
                else if (Team.IsDead(700))
                {
                    Team.PkMatch.End(Team, true);
                }
            }
        }

        internal void UpdateQualifier(GameClient client, GameClient target, uint damage)
        {
            if (client.Player.Map != 700)
                return;
            if (ArenaMatch != null)
            {
                if (ArenaStatistic.InDuelRoom)
                {
                    target.ArenaStatistic.Damage--;
                    ArenaMatch.SendScore();
                    if (target.ArenaStatistic.Damage == 0)
                    {
                        Server.Last_Winner_Duel.Clear();
                        Server.Last_Loser_Duel.Clear();
                        Server.Last_Winner_Duel.Add(client.ArenaStatistic.DuelHitted, client.Player.Name);
                        Server.Last_Loser_Duel.Add(target.ArenaStatistic.DuelHitted - client.ArenaStatistic.Damage, target.Player.Name);
                        client.ArenaMatch.Export();
                        client.ArenaMatch.Win(client, target);


                    }
                }
                else
                {
                    client.ArenaStatistic.Damage += damage;
                    ArenaMatch.SendScore();
                }
            }
            if (Team != null)
            {
                if (Team.TeamArenaMatch != null)
                {
                    Team.Damage += damage;
                    Team.TeamArenaMatch.SendScore();
                }
                if (Team.PkMatch != null)
                {
                    Team.PKStats.Points += damage;
                    Team.PkMatch.UpdateScore();
                }
            }
            if (ElitePKStats != null && ElitePkMatch != null)
            {
                ElitePKStats.Points += damage;
                ElitePkMatch.UpdateScore();
            }
        }

        public bool IsInSpellRange(uint UID, byte range)
        {
            if (range == 0)
                range = 10;
            if (Player.View.TryGetValue(UID, out var target, MapObjectType.Monster))
                return Core.GetDistance(Player.X, Player.Y, target.X, target.Y) <= range;
            if (Player.View.TryGetValue(UID, out target, MapObjectType.Player))
                return Core.GetDistance(Player.X, Player.Y, target.X, target.Y) <= range;
            if (Player.View.TryGetValue(UID, out target, MapObjectType.SobNpc))
                return Core.GetDistance(Player.X, Player.Y, target.X, target.Y) <= range;
            return false;
        }

        internal void LoseDeadExperience(GameClient killer)
        {
            if (AIType != 0 || Player.Level >= ServerKernel.MAX_UPLEVEL)
                return;
            DBLevExp nextlevel;
            nextlevel = Server.LevelInfo[DBLevExp.Sort.User][(byte)Player.Level];
            if (nextlevel.Experience == 0L)
                return;
            ulong loseexp;
            loseexp = nextlevel.Experience * 5 / 100uL; //Player.Experience * (uint)(nextlevel.UpLevTime * nextlevel.MentorUpLevTime) / nextlevel.Experience;
            double LoseExpPercent;
            //loseexp = (ulong)(Player.Experience * 0.10);
            LoseExpPercent = loseexp; //(double)loseexp / (double)nextlevel.Experience;
            if (Player.Experience > loseexp && Player.VipLevel != 6)
            {
                Player.Owner.SendSysMesage($"You lost 5% of you experience points!");
                Player.Experience -= loseexp;
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream2;
                    stream2 = recycledPacket.GetStream();
                    Player.SendUpdate(stream2, (long)Player.Experience, MsgUpdate.DataType.Experience);
                }
            }
            if (killer.Player.Level >= Player.Level)
                return;
            DBLevExp killernextlevel;
            killernextlevel = Server.LevelInfo[DBLevExp.Sort.User][(byte)killer.Player.Level];
            if (killernextlevel.Experience == 0L)
                return;
            double GetExp;
            GetExp = 100.0 / (double)killernextlevel.Experience * (double)(loseexp * 100);
            killer.Player.Experience += (uint)GetExp;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                killer.Player.SendUpdate(stream, (long)killer.Player.Experience, MsgUpdate.DataType.Experience);
            }
        }
        public bool BackJump = false;
        public Extensions.Time32 LastClientJump = new Extensions.Time32();
        public Extensions.Time32 LastServerJump = new Extensions.Time32();
        public Extensions.Time32 LastClientWalk = new Extensions.Time32();
        public bool AllowUseSpellOnSteed(ushort Spell)
        {
            if (!Player.ContainFlag(MsgUpdate.Flags.Ride))
                return true;
            if (Equipment.RidingCrop != 0)
                return true;
            if (Spell == 7002 || Spell == 7003 || Spell == 7001)
                return true;
            return false;
        }

        

        public void IncreaseExperience(Packet stream, double Experience, Flags.ExperienceEffect effect = Flags.ExperienceEffect.None)
        {

            int rainbowCount = (int)GemValues(Flags.Gem.NormalRainbowGem);

            if (Player.CursedTimer > 2 || Player.Level >= ServerKernel.MAX_UPLEVEL)
                return;

            if (effect != 0)
                Player.SendString(stream, MsgStringPacket.StringID.Effect, true, effect.ToString());

            if (Player.Level <= 130)
            {
                Experience *= (double)ServerKernel.EXP_RATE;
            }

            if (GemValues(Flags.Gem.NormalRainbowGem) != 0)
            {
                Experience += (Experience * (1 + rainbowCount / 100));
            }

            if (Player.Level >= 130)
            {
                Experience /= 3;
            }

            //if (Player.Map == 1001 || Player.Map == 1354 || Player.Map == 1762 || Player.Map == 2054 || Player.Map == 2056)
            //    Experience *= 2.0;
            if (Player.DExpTime != 0)
                Experience *= (double)Player.RateExp;

            if (Player.Map == 1039)
                Experience /= 100.0;

            //if (Player.Map == 1002 && Player.X >= 427 && Player.Y <= 392)
            //    Experience *= 50;

            Player.Experience += (ulong)Experience;
            while (Player.Experience >= Server.LevelInfo[DBLevExp.Sort.User][(byte)Player.Level].Experience)
            {
                Player.Experience -= Server.LevelInfo[DBLevExp.Sort.User][(byte)Player.Level].Experience;
                ushort newlev;
                newlev = (ushort)(Player.Level + 1);
                UpdateLevel(stream, newlev);
                if (Player.Level >= ServerKernel.MAX_UPLEVEL)
                {
                    Player.Experience = 0;
                    break;
                }
            }
            UpdateRebornLastLevel(stream);
            Player.SendUpdate(stream, (long)Player.Experience, MsgUpdate.DataType.Experience);
        }

        public void UpdateRebornLastLevel(Packet stream)
        {
            if (Player.Reborn <= 0)
                return;
            if (Player.Reincarnation)
            {
                if (Player.Level >= 110 && Player.Level < Player.SecoundeRebornLevel)
                    UpdateLevel(stream, Player.SecoundeRebornLevel, true);
            }
            else if (Player.Reborn == 1)
            {
                if (Player.Level >= 130 && Player.Level < Player.FirstRebornLevel)
                    UpdateLevel(stream, Player.FirstRebornLevel, true);
            }
            else if (Player.Reborn == 2 && Player.Level >= 130 && Player.Level < Player.SecoundeRebornLevel)
            {
                UpdateLevel(stream, Player.SecoundeRebornLevel, true);
            }
        }

        public ulong GainExpBall(double amount = 600.0, bool sendMsg = false, Flags.ExperienceEffect effect = Flags.ExperienceEffect.None, bool JustCalculate = false, bool mentorexp = true)
        {
            if (Player.Level >= ServerKernel.MAX_UPLEVEL)
                return 0uL;
            if (sendMsg)
                SendSysMesage("You have gained experience worth " + amount * 1.0 / 600.0 + " exp ball(s).");
            if (effect != 0)
            {
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream5;
                    stream5 = recycledPacket.GetStream();
                    Player.SendString(stream5, MsgStringPacket.StringID.Effect, true, effect.ToString());
                }
            }
            DBLevExp LevelDBExp;
            LevelDBExp = Server.LevelInfo[DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
                return 0uL;
            double ReceiveExp;
            ReceiveExp = (double)((long)Player.Experience * (long)LevelDBExp.UpLevTime) / (double)LevelDBExp.Experience;
            byte LEV;
            LEV = (byte)(ServerKernel.MAX_UPLEVEL - 1);
            if (ReceiveExp < 0.0 && Player.Level == LEV)
            {
                ReceiveExp = 0.0;
                using (RecycledPacket recycledPacket2 = new RecycledPacket())
                {
                    Packet stream4;
                    stream4 = recycledPacket2.GetStream();
                    UpdateLevel(stream4, ServerKernel.MAX_UPLEVEL, true);
                }
                return (uint)ReceiveExp;
            }
            ReceiveExp += amount;
            byte IncreaseLevel;
            IncreaseLevel = (byte)Player.Level;
            int times;
            times = LevelDBExp.UpLevTime;
            while (IncreaseLevel < ServerKernel.MAX_UPLEVEL && !(ReceiveExp < (double)times))
            {
                ReceiveExp -= (double)times;
                IncreaseLevel = (byte)(IncreaseLevel + 1);
                LevelDBExp = Server.LevelInfo[DBLevExp.Sort.User][IncreaseLevel];
                if (LevelDBExp == null)
                    break;
                times = LevelDBExp.UpLevTime;
            }
            if (times < 1)
                return 0uL;
            if (!JustCalculate)
            {
                using (RecycledPacket recycledPacket3 = new RecycledPacket())
                {
                    Packet stream3;
                    stream3 = recycledPacket3.GetStream();
                    UpdateLevel(stream3, IncreaseLevel, false, mentorexp);
                }
            }
            ReceiveExp /= (double)times;
            LevelDBExp = Server.LevelInfo[DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
                return 0uL;
            ulong CalculateEXp;
            CalculateEXp = (ulong)(ReceiveExp * (double)LevelDBExp.Experience);
            if (!JustCalculate)
            {
                Player.Experience = CalculateEXp;
                using (RecycledPacket recycledPacket4 = new RecycledPacket())
                {
                    Packet stream2;
                    stream2 = recycledPacket4.GetStream();
                    Player.SendUpdate(stream2, (long)Player.Experience, MsgUpdate.DataType.Experience);
                }
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    UpdateRebornLastLevel(stream);
                }
            }
            return CalculateEXp;
        }

        public ulong CalcExpBall(double amount, out ushort nextlevel)
        {
            if (Player.Level >= ServerKernel.MAX_UPLEVEL)
            {
                nextlevel = 0;
                return 0uL;
            }
            DBLevExp LevelDBExp;
            LevelDBExp = Server.LevelInfo[DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
            {
                nextlevel = 0;
                return 0uL;
            }
            double ReceiveExp;
            ReceiveExp = (double)((long)Player.Experience * (long)LevelDBExp.UpLevTime) / (double)LevelDBExp.Experience;
            ReceiveExp += amount;
            byte IncreaseLevel;
            IncreaseLevel = (byte)Player.Level;
            int times;
            times = LevelDBExp.UpLevTime;
            while (IncreaseLevel < 140 && !(ReceiveExp < (double)times))
            {
                ReceiveExp -= (double)times;
                IncreaseLevel = (byte)(IncreaseLevel + 1);
                LevelDBExp = Server.LevelInfo[DBLevExp.Sort.User][IncreaseLevel];
                if (LevelDBExp == null)
                    break;
                times = LevelDBExp.UpLevTime;
            }
            if (times < 1)
            {
                nextlevel = IncreaseLevel;
                return 0uL;
            }
            ReceiveExp /= (double)times;
            LevelDBExp = Server.LevelInfo[DBLevExp.Sort.User][(byte)Player.Level];
            if (LevelDBExp == null)
            {
                nextlevel = IncreaseLevel;
                return 0uL;
            }
            ulong CalculateEXp;
            CalculateEXp = (ulong)(ReceiveExp * (double)LevelDBExp.Experience);
            nextlevel = IncreaseLevel;
            return CalculateEXp;
        }

        public void AddGem(Flags.Gem gem, ushort value)
        {
            if (value == 15 || value == 10 || value == 5)
            {
                if (gem == Flags.Gem.NormalDragonGem || gem == Flags.Gem.RefinedDragonGem || gem == Flags.Gem.SuperDragonGem)
                    Status.PhysicalPercent += value;
                else
                    Status.MagicPercent += value;
            }
            Gems[(byte)((int)gem / 10)] += value;
        }

        public uint GemValues(Flags.Gem gem)
        {
            return Gems[(byte)((int)gem / 10)];
        }

        public uint AjustAttack(uint Damage)
        {
            uint nAttack;
            nAttack = 0;
            if (Player.ContainFlag(MsgUpdate.Flags.Stigma) || Player.OnAttackPotion)
                nAttack += (uint)(Base.MulDiv((int)Damage, 130, 100) - (int)Damage);
            if (Status.PhysicalPercent != 0)
                nAttack += (uint)Base.MulDiv((int)Damage, (int)Status.PhysicalPercent, 100);
            if (Player.Intensify)
            {
                Player.Intensify = false;
                nAttack += (uint)(Base.MulDiv((int)Damage, Player.IntensifyDamage, 100) - (int)Damage);
            }
            if (Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.Invisibility))
            {
                nAttack -= (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, 130, 100) - Damage;
            }

            return Damage + nAttack;
        }
        public void AjustDodge()
        {
            if (!Player.ContainFlag(MsgUpdate.Flags.Dodge))
            {
                Player.Owner.Status.Dodge += 10;
            }
            return;
        }

        public int GetDefense2()
        {
            if (Player.Reborn < 2)
                return 10000;
            return 5000;
        }

        public uint AjustCriticalStrike()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreasePStrike, out var Power) && (bool)Power)
                return Status.CriticalStrike + (uint)Power;
            return Status.CriticalStrike;
        }

        public uint AjustMCriticalStrike()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseMStrike, out var Power) && (bool)Power)
                return Status.SkillCStrike + (uint)Power;
            return Status.SkillCStrike;
        }

        public uint AjustImunity()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseImunity, out var Power) && (bool)Power)
                return Status.Immunity + (uint)Power;
            return Status.Immunity;
        }

        public uint AjustBreakthrough()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseBreack, out var Power) && (bool)Power)
                return Status.Breakthrough + (uint)Power;
            return Status.Breakthrough;
        }

        public uint AjustAntiBreack()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseAntiBreack, out var Power) && (bool)Power)
                return Status.Counteraction + (uint)Power;
            return Status.Counteraction;
        }

        public uint AjustMagicDamageIncrease()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseFinalMAttack, out var Power) && (bool)Power)
                return Status.MagicDamageIncrease + (uint)Power;
            return Status.MagicDamageIncrease;
        }

        public uint AjustMagicDamageDecrease()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseFinalMDamage, out var Power) && (bool)Power)
                return Status.MagicDamageDecrease + (uint)Power;
            return Status.MagicDamageDecrease;
        }

        public uint AjustPhysicalDamageIncrease()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseFinalPAttack, out var Power) && (bool)Power)
                return Status.PhysicalDamageIncrease + (uint)Power;
            return Status.PhysicalDamageIncrease;
        }

        public uint AjustPhysicalDamageDecrease()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseFinalPDamage, out var Power) && (bool)Power)
                return Status.PhysicalDamageDecrease + (uint)Power;
            return Status.PhysicalDamageDecrease;
        }

        public uint AjustMagic(uint Damage)
        {
            uint mAttack = 0;
            //if (Status.MagicPercent > 0)
            //{
            //    mAttack += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.MagicPercent - 50, 100);// -Damage / 2;
            //    //(uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.PhysicalPercent, 100);
            //}
            if (Status.MagicPercent == 105) //1soc set
            {

                mAttack += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.MagicPercent + 30, 100);// -Damage / 2;
                //(uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.PhysicalPercent, 100);
            }
            if (Status.MagicPercent > 105) //2soc set
            {

                mAttack += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.MagicPercent + 40, 100);// -Damage / 2;
                //(uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.PhysicalPercent, 100);
            }
            if (Status.MagicPercent > 15 && Status.MagicPercent <= 90) // 2weapon 2soc or full missing 1 to get full soc
            {

                mAttack += (uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.MagicPercent + 20, 100);// -Damage / 2;
                //(uint)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)Damage, (int)Status.PhysicalPercent, 100);
            }


            return Damage + mAttack;
        }

        public uint AjustMagicAttack()
        {
            Role.Instance.RoleStatus Power;
            uint nAttack;
            nAttack = 0;
            if (ExtraStatus.TryGetValue(Role.Instance.RoleStatus.StatuTyp.IncreaseMAttack, out Power))
            {
                if (Power)
                    return Status.MagicAttack + Power;
            }
            if (Status.MagicPercent != 0)
                nAttack += (uint)Base.MulDiv((int)Status.MagicAttack, (int)Status.MagicPercent, 100);

            return (uint)(Status.MagicAttack + nAttack);
            //if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseMAttack, out var Power) && (bool)Power)
            //    return Status.MagicAttack + (uint)Power;
            //return Status.MagicAttack;
        }

        public uint AjustMaxHitpoints()
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreaseMaxHp, out var Power) && (bool)Power)
                return Status.MaxHitpoints + (uint)Power;
            return Status.MaxHitpoints;
        }

        public uint AjustMaxAttack(uint damage)
        {
            if (ExtraStatus.TryGetValue(RoleStatus.StatuTyp.IncreasePAttack, out var Power) && (bool)Power)
                return damage + (uint)Power;
            return damage;
        }

        public void ClanShareBP()
        {
            if (Team == null)
                return;
            if (Team.TeamLider(this))
            {
                foreach (GameClient memeber in Team.GetMembers())
                {
                    Team.GetClanShareBp(memeber);
                }
                return;
            }
            Team.GetClanShareBp(this);
        }

        public unsafe void Shift(ushort X, ushort Y, Packet stream, bool SendData = true)
        {
            Player.Px = Player.X;
            Player.Py = Player.Y;
            if (SendData)
            {
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = Player.UID;
                actionQuery.Type = ActionType.FlashStep;
                actionQuery.wParam1 = X;
                actionQuery.wParam2 = Y;
                ActionQuery action;
                action = actionQuery;
                Player.View.SendView(stream.ActionCreate(&action), true);
                Map.View.MoveTo((IMapObj)Player, (int)X, (int)Y);
                Player.X = X;
                Player.Y = Y;
                Player.View.Role(false, stream);
            }
            else
            {
                Map.View.MoveTo((IMapObj)Player, (int)X, (int)Y);
                Player.X = X;
                Player.Y = Y;
                Player.View.Role();
            }
        }

        public GameClient(SecuritySocket _socket)
        {
            ExtraStatus = new ConcurrentDictionary<RoleStatus.StatuTyp, RoleStatus>();
            ArenaStatistic = new MsgArena.User();
            DemonExterminator = new Role.Instance.DemonExterminator();
            Confiscator = new Confiscator();
            MaxHWMobs = (int)Program.GetRandom.Next(700, 800);
            MaxEmeraldMobs = (int)Program.GetRandom.Next(700, 800);
            MaxMetsMobs = (int)Program.GetRandom.Next(700, 800); //100-500 //458
            MaxDBMobs = (int)Program.GetRandom.Next(500, 600);
            MaxProfTokenMobs = (int)Program.GetRandom.Next(500, 700);
            MaxCountItemMobs = (int)Program.GetRandom.Next(500, 700);
            //MaxCount_Stone = (uint)Program.GetRandom.Next(2500, 3000);
            ClientFlag |= ServerFlag.None;
            if (_socket != null)
            {
                Socket = _socket;
                Socket.Client = this;
                Socket.Game = this;
                Cryptography = new GameCryptography(Encoding.ASCII.GetBytes("QW3P3M8ZQUbllpBZ"));
                Socket.SetCrypto(Cryptography);
            }
            Player = new Player(this);
            DHKeyExchance = new DHKeyExchange.ServerKeyExchange();
            if (_socket != null)
                Send(DHKeyExchance.CreateServerKeyPacket());
        }

        public void Send(Packet msg)
        {
            try
            {
                if (!Fake && Socket.Alive)
                    Socket.Send(msg);
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public unsafe void Send(byte[] buffer)
        {
            try
            {
                if (Fake || Socket.Alive == false)
                    return;

                ushort length = BitConverter.ToUInt16(buffer, 0);
                if (length == 0)
                {
                    Poker.Packets.Packet.WriteUInt16((ushort)(buffer.Length - 8), 0, buffer);
                }
                Poker.Packets.Packet.WriteString("TQServer", buffer.Length - 8, buffer);

                ServerSockets.Packet stream = new ServerSockets.Packet(buffer);
                Socket.Send(stream);

            }
            catch (Exception e)
            {
                Console.WriteException(e);
            }
        }

        public void SendSysMesage(string Messaj, MsgMessage.ChatMode ChatType = MsgMessage.ChatMode.System, MsgMessage.MsgColor color = MsgMessage.MsgColor.red, bool SendScren = false)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                if (SendScren)
                    Player.View.SendView(new MsgMessage(Messaj, color, ChatType).GetArray(stream), true);
                else
                    Send(new MsgMessage(Messaj, color, ChatType).GetArray(stream));
            }
        }

        public void SendWhisper(string Messaj, string from, string to, uint face)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgMessage X;
                X = new MsgMessage(Messaj, to, from, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Whisper)
                {
                    Mesh = face,
                    Color = uint.MaxValue
                };
                uidnext++;
                X.MessageUID1 = uidnext;
                Packet x2;
                x2 = X.GetArray(stream);
                Send(x2);
            }
        }

        public void SendScreen(byte[] msg, bool self = true)
        {
            Player.View.SendView(msg, self);
        }

        public void CreateDialog(Packet stream, string Text, string OptionText)
        {
            Dialog dialog;
            dialog = new Dialog(this, stream);
            dialog.AddText(Text);
            if (OptionText != "")
                dialog.AddOption(OptionText, byte.MaxValue);
            dialog.FinalizeDialog();
        }

        public void CreateBoxDialog(string Text)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Dialog dialog;
                dialog = new Dialog(this, stream);
                dialog.CreateMessageBox(Text).FinalizeDialog(true);
            }
        }

        public IEnumerable<MsgGameItem> AllMyItems()
        {
            foreach (MsgGameItem value in Inventory.ClientItems.Values)
            {
                if (!value.Fake)
                    yield return value;
            }
            foreach (MsgGameItem value2 in Equipment.ClientItems.Values)
            {
                if (!value2.Fake)
                    yield return value2;
            }
            foreach (ConcurrentDictionary<uint, MsgGameItem> Wh in Warehouse.ClientItems.Values)
            {
                foreach (MsgGameItem value3 in Wh.Values)
                {
                    if (!value3.Fake)
                        yield return value3;
                }
            }
        }

        public int GetItemsCount()
        {
            int count;
            int FakeCount = 0;
            count = 0;
            count += Inventory.ClientItems.Count;
            count += Equipment.ClientItems.Count;
            foreach (ConcurrentDictionary<uint, MsgGameItem> Wh in Warehouse.ClientItems.Values)
            {
                count += Wh.Count;
            }
            foreach (var item in Equipment.ClientItems.Values)
                if (item.Fake)
                    FakeCount++;
            foreach (var item in Inventory.ClientItems.Values)
                if (item.Fake)
                    FakeCount++;
            return count - FakeCount;
        }

        public bool TryGetItem(uint UID, out MsgGameItem item)
        {
            if (Equipment.TryGetValue(UID, out item))
                return true;
            if (Inventory.TryGetItem(UID, out item))
                return true;
            item = null;
            return false;
        }

        public ushort CalculateHitPoint()
        {
            ushort valor;
            valor = 0;
            switch ((Flags.ProfessionType)Player.Class)
            {
                case Flags.ProfessionType.TROJAN:
                    return (ushort)(valor + (ushort)((double)(int)Player.Agility * 3.15 + (double)(int)Player.Spirit * 3.15 + (double)(int)Player.Strength * 3.15 + (double)(int)Player.Vitality * 25.2));
                case Flags.ProfessionType.VETERAN_TROJAN:
                    return (ushort)(valor + (ushort)((double)(int)Player.Agility * 3.24 + (double)(int)Player.Spirit * 3.24 + (double)(int)Player.Strength * 3.24 + (double)(int)Player.Vitality * 25.9));
                case Flags.ProfessionType.TIGER_TROJAN:
                    return (ushort)(valor + (ushort)((double)(int)Player.Agility * 3.3 + (double)(int)Player.Spirit * 3.3 + (double)(int)Player.Strength * 3.3 + (double)(int)Player.Vitality * 26.4));
                case Flags.ProfessionType.DRAGON_TROJAN:
                    return (ushort)(valor + (ushort)((double)(int)Player.Agility * 3.36 + (double)(int)Player.Spirit * 3.36 + (double)(int)Player.Strength * 3.36 + (double)(int)Player.Vitality * 26.8));
                case Flags.ProfessionType.TROJAN_MASTER:
                    return (ushort)(valor + (ushort)((double)(int)Player.Agility * 3.45 + (double)(int)Player.Spirit * 3.45 + (double)(int)Player.Strength * 3.45 + (double)(int)Player.Vitality * 27.6));
                default:
                    return (ushort)(valor + (ushort)(Player.Agility * 3 + Player.Spirit * 3 + Player.Strength * 3 + Player.Vitality * 24));
            }
        }

        public ushort CalculateMana()
        {
            ushort valor;
            valor = 0;
            switch (Player.Class)
            {
                case 132:
                case 142:
                    return (ushort)(valor + (ushort)(Player.Spirit * 15));
                case 133:
                case 143:
                    return (ushort)(valor + (ushort)(Player.Spirit * 20));
                case 134:
                case 144:
                    return (ushort)(valor + (ushort)(Player.Spirit * 25));
                case 135:
                case 145:
                    return (ushort)(valor + (ushort)(Player.Spirit * 30));
                default:
                    return (ushort)(valor + (ushort)(Player.Spirit * 5));
            }
        }

        public void Pullback(bool autohunt = false)
        {
            Teleport(Player.X, Player.Y, Player.Map, Player.DynamicID, true, false, autohunt);
        }

        public void TeleportCallBack()
        {
            Teleport(Player.PMapX, Player.PMapY, Player.PMap, Player.PDinamycID);
        }

        public unsafe void Teleport(ushort x, ushort y, uint MapID, uint DinamycID = 0, bool revive = true, bool CanTeleport = false, bool autohunt = false)
        {
            if (InPassTheBomb)
                PassTheBomb.OnTeleport(this);
            if (InTDM)
                TeamDeathMatch.OnTeleport(this);
            if(InFIveOut)
                Get5HitOut.OnTeleport(this);
            if(InLastManStanding)
                LastManStanding.OnTeleport(this);
            //if (InST)
            //    SkillsTournament.OnTeleport(this);


            if (EventBase != null)
            {
                var events = Program.Events.Find(e => e.EventTitle == EventBase.EventTitle);
                if (events != null)
                {
                    if (!events.InTournament(this, true, MapID, DinamycID))
                    {
                        events.RemovePlayer(this, false, false);
                    }
                }
            }
            
            if (Mining)
                StopMining();
            if (MapID == 1036 && Player.TransformInfo != null)
                Player.TransformInfo.FinishTransform();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream2;
                stream2 = rec.GetStream();
                if (Player.SpecialGarment != 0)
                    Player.RemoveSpecialGarment(stream2);
                if (Player.RightSpecialAccessory != 0 || Player.LeftSpecialAccessory != 0)
                    Player.RemoveSpecialAccessory(stream2);
            }
            if ((MapID == 1038 || MapID == 700 || EventBase != null || MapID == 1005 || MapID == 2071 || MapID == 1764 || MapID == 1505 || MsgSchedules.CurrentTournament.InTournament(this)) && Player.ContainFlag(MsgUpdate.Flags.Ride))
                Player.RemoveFlag(MsgUpdate.Flags.Ride);
            Player.FairbattlePower = Flags.FairbattlePower.NotActive;
            if (!autohunt)
            {
                if (Player.AutoHunting == AutoStructures.Mode.Enable)
                    Player.AutoHunting = AutoStructures.Mode.Disable;
            }
            if ((!ProjectManager && ((Player.Map == 6001 && !CanTeleport) || (Player.Map == 6003 && !CanTeleport))) || (Player.Map == 1038 && !Player.Alive && !CanTeleport))
                return;
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                Packet stream;
                stream = recycledPacket.GetStream();
                if ((Program.MapCounterHits.Contains(Player.Map) || MsgSchedules.CurrentTournament.InTournament(this)) && MapID != Player.Map)
                    SendSysMesage("", MsgMessage.ChatMode.FirstRightCorner);
                if (Socket != null && !Socket.Alive)
                    return;
                if (Player.Map == 1038 && Player.Alive == false && CanTeleport == false)
                    return;
                if (this.Pet != null) this.Pet.DeAtach(stream);

                if (IsWatching() && Player.Map == 700)
                {
                    if (ArenaWatchingGroup != null)
                        ArenaWatchingGroup.DoLeaveWatching(this);
                    else if (TeamArenaWatchingGroup != null)
                    {
                        TeamArenaWatchingGroup.DoLeaveWatching(this);
                    }
                    else
                    {
                        ElitePkWatchingGroup?.DoLeaveWatching(this);
                    }
                }
                if (IsVendor)
                    MyVendor.StopVending(stream);
                if (InTrade)
                    MyTrade.CloseTrade();
                if ((MapID == 601 || MapID == 1039) && Player.HeavenBlessing > 0)
                    Player.SendUpdate(stream, 1L, MsgUpdate.DataType.OnlineTraining);
                if ((Player.Map == 601 || Player.Map == 1039) && MapID != 601 && MapID != 1039)
                    Player.SendUpdate(stream, 2L, MsgUpdate.DataType.OnlineTraining);
                if (!Role.GameMap.CheckMap(MapID))
                {
                    MapID = 1002;
                    x = 429;
                    y = 378;
                }

                if (Server.ServerMaps.TryGetValue(MapID, out var GameMap))
                {
                    OnAutoAttack = false;
                    Player.RemoveBuffersMovements(stream);
                    Player.View.Clear(stream);
                    ActionQuery actionQuery;
                    if (GameMap.BaseID != 0)
                    {
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = Player.UID;
                        actionQuery.Type = ActionType.Teleport;
                        actionQuery.dwParam = GameMap.BaseID;
                        actionQuery.wParam1 = x;
                        actionQuery.wParam2 = y;
                        actionQuery.dwParam3 = GameMap.BaseID;
                        ActionQuery daction;
                        daction = actionQuery;
                        Send(stream.ActionCreate(&daction));
                    }
                    else
                    {
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = Player.UID;
                        actionQuery.Type = ActionType.Teleport;
                        actionQuery.dwParam = MapID;
                        actionQuery.wParam1 = x;
                        actionQuery.wParam2 = y;
                        actionQuery.dwParam3 = MapID;
                        ActionQuery aaction;
                        aaction = actionQuery;
                        Send(stream.ActionCreate(&aaction));
                    }
                    if (Player.Map != 700)
                    {
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = Player.UID;
                        actionQuery.Type = (ActionType)157;
                        actionQuery.dwParam = 2;
                        actionQuery.wParam1 = x;
                        actionQuery.wParam2 = y;
                        actionQuery.dwParam3 = MapID;
                        ActionQuery aaaction;
                        aaaction = actionQuery;
                        Send(stream.ActionCreate(&aaaction));
                    }
                    actionQuery = default(ActionQuery);
                    actionQuery.ObjId = Player.UID;
                    actionQuery.Type = ActionType.GetSurroundings;
                    actionQuery.dwParam = MapID;
                    actionQuery.wParam1 = x;
                    actionQuery.wParam2 = y;
                    actionQuery.dwParam3 = MapID;
                    ActionQuery action;
                    action = actionQuery;
                    Send(stream.ActionCreate(&action));
                    if (MapID == Player.Map && Player.DynamicID == DinamycID)
                    {
                        Map.Denquer(this);
                        Player.X = x;
                        Player.Y = y;
                        Server.ServerMaps[MapID].Enquer(this);
                    }
                    else
                    {
                        Player.PDinamycID = Player.DynamicID;
                        Player.PMapX = Player.X;
                        Player.PMapY = Player.Y;
                        Map.Denquer(this);
                        Player.DynamicID = DinamycID;
                        Player.X = x;
                        Player.Y = y;
                        Player.PMap = Player.Map;
                        Player.Map = MapID;
                        Server.ServerMaps[MapID].Enquer(this);
                    }
                    if (Player.Map == 700)
                    {
                        if (InTeamQualifier())
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, 19568946643047uL));
                        else if (ElitePkWatchingGroup != null || ElitePkMatch != null)
                        {
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, 18173880847630407uL));
                        }
                        else
                        {
                            Send(stream.MapStatusCreate(Map.ID, Map.ID, Map.TypeStatus));
                        }
                    }
                    else if (GameMap.BaseID != 0)
                    {
                        Send(stream.MapStatusCreate(Map.BaseID, Map.BaseID, Map.TypeStatus));
                    }
                    else
                    {
                        Send(stream.MapStatusCreate(Map.ID, Map.ID, Map.TypeStatus));
                    }
                    Player.View.Role(true);
                    if (!Player.Alive && revive && Player.Map != 1038)
                        Player.Revive(stream);
                    if (Player.ObjInteraction != null && Player.Map != 700 && Core.IsBoy(Player.Body))
                        Player.ObjInteraction.Teleport(x, y, MapID, DinamycID);
                    if (Player.Map == 1038 && Player.ContainFlag(MsgUpdate.Flags.FatalStrike))
                        Player.RemoveFlag(MsgUpdate.Flags.FatalStrike);
                    Player.UpdateSurroundings(stream, true);

                }
            }
            if (Player.Map == 6000 && !Map.ValidLocation(x, y))
                Teleport(32, 73, 6000);
            if (Program.MonsterCity.Count > 0)
            {
                var MobCity = Program.MonsterCity.Values.FirstOrDefault(m => m != null && m.MapID == Player.Map);
                MobCity?.SendAlret(this);
            }
            ClanShareBP();
            Player.ProtectAttack(2000);
        }

        public unsafe void UpdateLevel(Packet stream, ushort Level, bool REsetExp = false, bool mentorexp = true)
        {
            if ((int)Level == (int)this.Player.Level)
                return;
            if (this.Player.MyGuildMember != null)
                this.Player.MyGuildMember.Level = (uint)Level;
            if (REsetExp)
                this.Player.Experience = 0UL;
            uint level = (uint)this.Player.Level;
            this.Player.Level = Level;
            this.Player.SendUpdate(stream, (long)this.Player.Level, MsgUpdate.DataType.Level);
            ActionQuery actionQuery = new ActionQuery()
            {
                Type = ActionType.Leveled,
                ObjId = this.Player.UID,
                wParam1 = Level
            };
            this.Player.View.SendView(stream.ActionCreate(&actionQuery), true);
            if (this.Player.Reborn == (byte)0 && (AtributesStatus.IsWater(this.Player.Class) ? (Level < (ushort)111 ? 1 : (level >= 110U ? 0 : (Level > (ushort)110 ? 1 : 0))) : (Level < (ushort)121 ? 1 : (level >= 120U ? 0 : (Level > (ushort)120 ? 1 : 0)))) != 0)
            {
                DataCore.AtributeStatus.GetStatus(this.Player);
                this.Player.SendUpdate(stream, (long)this.Player.Strength, MsgUpdate.DataType.Strength);
                this.Player.SendUpdate(stream, (long)this.Player.Agility, MsgUpdate.DataType.Agility);
                this.Player.SendUpdate(stream, (long)this.Player.Spirit, MsgUpdate.DataType.Spirit);
                this.Player.SendUpdate(stream, (long)this.Player.Vitality, MsgUpdate.DataType.Vitality);
                this.Player.SendUpdate(stream, (long)this.Player.Atributes, MsgUpdate.DataType.Atributes);
            }
            else if (level < (uint)Level)
            {
                this.Player.Atributes += (ushort)(((int)Level - (int)level) * 3);
                this.Player.SendUpdate(stream, (long)this.Player.Atributes, MsgUpdate.DataType.Atributes);
            }
            Server.RebornInfo.Reborn(this.Player, (byte)0, stream);
            if (this.Player.MyMentor != null & mentorexp)
            {
                DBLevExp dbLevExp = Server.LevelInfo[DBLevExp.Sort.User][(byte)level];
                this.Player.MyMentor.Mentor_ExpBalls += (uint)dbLevExp.MentorUpLevTime;
                Associate.Member member;
                if (this.Player.MyMentor.Associat.ContainsKey((byte)5) && this.Player.MyMentor.Associat[(byte)5].TryGetValue(this.Player.UID, out member))
                    member.ExpBalls += (uint)dbLevExp.MentorUpLevTime;
            }
            this.Equipment.QueryEquipment(this.Equipment.Alternante, false);
            this.Player.HitPoints = (int)this.Status.MaxHitpoints;
            //if (this.Player.Level <= (ushort)70 && this.Team != null)
            //{
            //    GameClient leader = this.Team.Leader;
            //    if ((int)leader.Player.UID != (int)this.Player.UID && (int)Core.GetDistance(leader.Player.X, leader.Player.Y, this.Player.X, this.Player.Y) < (int)RoleView.ViewThreshold)
            //    {
            //        if ((int)leader.Player.Map != (int)this.Player.Map || !leader.Player.Alive || leader.Player.Level < (ushort)70)
            //            return;
            //        leader.Player.VirtutePoints += (uint)this.Player.Level * 10;
            //        this.Team.SendTeam(new MsgMessage("Congratulations to leader, he have earned " + ((int)this.Player.Level * 10).ToString() + " VirtuePoints by leveling up newbies!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Team).GetArray(stream), 0U);
            //    }
            //}
            this.UpdateRebornLastLevel(stream);
            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + Player.Name + " has reached level " + Player.Level + "!!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.CrosTheServer).GetArray(stream));
            //Program.DiscordLevelAPI.Enqueue("Congratulations! " + Player.Name + " has reached level " + Player.Level + "!!");
        }

        internal static GameClient CharacterFromName(string p, uint uid = 0)
        {
            foreach (GameClient x in Server.GamePoll.Values)
            {
                if (uid != 0)
                {
                    if (uid == x.Player.UID)
                        return x;
                }
                else if (p == x.Player.Name)
                {
                    return x;
                }
            }
            return null;
        }

        public string MacAdd()
        {
            string mac;
            mac = "";
            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("select Mac from synshield where UID='" + Player.UID + "'", conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        mac = reader.GetString("Mac");
                }
            }
            return mac;
        }

        public string AccountName()
        {
            string username;
            username = "";
            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("select Username from accounts where EntityID='" + Player.UID + "'", conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        username = reader.GetString("Username");
                }
            }
            return username;
        }

        public void UnlinkAccountFromEntity(uint entityId)
        {
            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            {
                conn.Open();
                using (MySql.Data.MySqlClient.MySqlCommand updateCmd = new MySql.Data.MySqlClient.MySqlCommand("UPDATE accounts SET EntityID = NULL WHERE EntityID = @EntityID", conn))
                {
                    updateCmd.Parameters.AddWithValue("@EntityID", entityId);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateVoteTime(uint entityId, DateTime voteTime)
        {
            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("UPDATE vote_system SET vote_created = @VoteTime WHERE EntityID = @EntityID", conn))
            {
                cmd.Parameters.AddWithValue("@EntityID", entityId);
                cmd.Parameters.AddWithValue("@VoteTime", voteTime);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePassword(uint entityId, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("UPDATE accounts SET Password = @Password WHERE EntityID = @EntityID", conn))
            {
                cmd.Parameters.AddWithValue("@EntityID", entityId);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DateTime? GetLastVoteTimeByMac(string macAddress, uint excludeUid)
        {
            using (var conn = new MySqlConnection(TopRankings.ConnectionString))
            using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT MAX(vote_created) FROM vote_system WHERE MacAdd = @mac AND EntityID != @uid", conn))
            {
                cmd.Parameters.AddWithValue("@mac", macAddress);
                cmd.Parameters.AddWithValue("@uid", excludeUid);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                    return Convert.ToDateTime(result);
                return null;
            }
        }

        public void InsertVoteTime(uint entityId, DateTime voteTime, string mac)
        {
            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("INSERT INTO vote_system (EntityID, vote_created, MacAdd) VALUES (@EntityID, @VoteTime, @MacAddress)", conn))
            {
                cmd.Parameters.AddWithValue("@EntityID", entityId);
                cmd.Parameters.AddWithValue("@VoteTime", voteTime);
                cmd.Parameters.AddWithValue("@MacAddress", mac);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DateTime? VoteSystemDb()
        {
            DateTime? vote_time = null;

            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT vote_created FROM vote_system WHERE EntityID = @EntityID AND MacAdd = @MacAdd", conn))
            {
                cmd.Parameters.AddWithValue("@EntityID", Player.UID); // prevent SQL injection
                cmd.Parameters.AddWithValue("@MacAdd", Player.Owner.OnLogin.MacAddress); // prevent SQL injection
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vote_time = reader.GetDateTime("vote_created"); // get DateTime properly
                    }
                }
            }

            return vote_time;
        }

        public string AccountPassword()
        {
            string username;
            username = "";
            using (MySqlConnection conn = new MySqlConnection(TopRankings.ConnectionString))
            using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("select Password from accounts where EntityID='" + Player.UID + "'", conn))
            {
                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        username = reader.GetString("Password");
                }
            }
            return username;
        }

        public string GenerateCaptcha(int len)
        {
            string str;
            str = "";
            while (len-- > 0)
            {
                str += (char)ServerKernel.NextAsync(48, 57);
            }
            return str;
        }

    }
}
