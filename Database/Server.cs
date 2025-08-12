using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgFloorItem;
using System.IO;
using TheChosenProject.Game.MsgTournaments;
using Extensions;
using TheChosenProject.Role;
using TheChosenProject.WindowsAPI;
using TheChosenProject.Client;
using TheChosenProject.Database.Shops;
using TheChosenProject.Game.Ai;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerCore.Website;
using TheChosenProject.ServerCore;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.ServerSockets;
using TheChosenProject.Managers;
using TheChosenProject.Mobs;
using TheChosenProject.Game.MsgServer.AttackHandler.ReceiveAttack;
using OpenSSL;
using static TheChosenProject.Game.MsgTournaments.ITournamentsAlive;
using static TheChosenProject.Role.KOBoard;

namespace TheChosenProject.Database
{
    public class Server
    {
        public static Counter BotCounter = new Counter(1100000);
        public static uint CountDbDailyTraining = 0;

        public static Dictionary<uint, string> Last_Loser_Duel = new Dictionary<uint, string>();

        public static Dictionary<uint, string> Last_Winner_Duel = new Dictionary<uint, string>();
        public static ushort[] ExpBallProf = new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 12, 12, 12, 12, 12, 12, 12 };

        public static InsultManager Insults;
        public static Time32 ResetStamp = Time32.Now.AddMilliseconds(6000);

        private static Dictionary<int, string> dailyEventSchedule = new Dictionary<int, string>();
        private static int lastScheduledDay = -1;
        private static readonly string dailyEventSchedulePath = "Config/DailyEventSchedule.txt";
        private static readonly string[] allEvents = { "SkillsTournament", "TeamDeathMatch", "Get5HitOut", "DivineDetonation", "ThroneSiege", "CycloneRace", "Theif" };

        public static ushort[] PriceUpdatePorf = new ushort[20]
        {
            600, 600, 600, 600, 600, 600, 1200, 1800, 3000, 3600,
            6000, 7200, 7200, 7200, 7200, 8318, 12170, 17735, 25639, 31537
        };

        public static Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>> LevelInfo = new Dictionary<DBLevExp.Sort, Dictionary<byte, DBLevExp>>();

        public static ConcurrentDictionary<uint, TheCrimeTable> TheCrimePoll = new ConcurrentDictionary<uint, TheCrimeTable>();

        //public static Dictionary<Flags.BaseClassType, List<SkillLearn>> SkillForLearning = new Dictionary<Flags.BaseClassType, List<SkillLearn>>();

        public static Dictionary<ushort, ushort> WeaponSpells = new Dictionary<ushort, ushort>();

        public static MagicType Magic = new MagicType();

        //public static List<uint> ActiveSquamas = new List<uint>();

        public static SubProfessionInfo SubClassInfo = new SubProfessionInfo();

        public static Dictionary<uint, MonsterFamily> MonsterFamilies = new Dictionary<uint, MonsterFamily>();

        public static Dictionary<uint, MonsterRole> MonsterRole = new Dictionary<uint, MonsterRole>();

        public static Counter ITEM_Counter = new Counter(1);

        public static LotteryTable Lottery = new LotteryTable();

        public static Rifinery RifineryItems;

        public static RefinaryBoxes DBRerinaryBoxes;

        public static ItemType ItemsBase;

        public static Dictionary<uint, GameMap> ServerMaps;

        public static ConcurrentDictionary<uint, GameClient> GamePoll;

        public static List<int> NameUsed;

        public static RebornInfomations RebornInfo;

        public static ArenaTable Arena = new ArenaTable();

        public static TeamArenaTable TeamArena = new TeamArenaTable();

        public static Counter ClientCounter = new Counter(1000000);

        public static ConfiscatorTable QueueContainer = new ConfiscatorTable();

        public static bool FullLoading = false;
        public static WindowsAPI.IniFile Pets;

        public static Dictionary<uint, string> MapName = new Dictionary<uint, string>
        {
            { 1015, "BirdIsland" },
            { 1011, "PhoenixCastle" },
            { 1000, "DesertCity" },
            { 1020, "ApeMountain" },
            { 1002, "TwinCity" }
        };

        public static uint ResetServerDay = 0;
        public ushort ID;
        public static bool ResetedAlready = false;

        public static void Reset(Time32 Clock)
        {
            if (!(Clock > ResetStamp))
                return;
            if (DateTime.Now.DayOfYear != ResetServerDay)
            {
                try
                {
                    //ResetedAlready = true;
                    Arena.ResetArena();
                    TeamArena.ResetArena();
                    foreach (var flowerclient in Role.Instance.Flowers.ClientPoll.Values)
                    {
                        foreach (var flower in flowerclient)
                            flower.Amount2day = 0;
                    }
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        foreach (GameClient client in GamePoll.Values)
                        {
                            client.Player.TCCaptainTimes = 0;
                            client.DemonExterminator.FinishToday = 0;
                            //client.Player.MiningAttempts = 200;
                            client.Player.ConquerLetter = 0;
                            client.Player.LavaQuest = 0;
                            client.Player.LotteryEntries = 0;
                            client.Player.Day = DateTime.Now.DayOfYear;
                            client.Player.BDExp = 0;
                            client.Player.ExpBallUsed = 0;
                            //if (client.Player.Flowers != null)
                            //    client.Player.Flowers.FreeFlowers = 1;
                            //foreach (Flowers.Flower flower in client.Player.Flowers)
                            //{
                            //    flower.Amount2day = 0;
                            //}
                            if (!client.Fake)
                            {


                                client.Player.Flowers.FreeFlowers = 1;
                                foreach (var flower in client.Player.Flowers)
                                    flower.Amount2day = 0;


                                if (client.Player.Flowers.FreeFlowers > 0)
                                {
                                    client.Send(stream.FlowerCreate(Role.Core.IsBoy(client.Player.Body)
                                        ? Game.MsgServer.MsgFlower.FlowerAction.FlowerSender
                                        : Game.MsgServer.MsgFlower.FlowerAction.Flower
                                        , 0, 0, client.Player.Flowers.FreeFlowers));
                                }

                                if (client.Player.Level >= 90)
                                {
                                    client.Player.Enilghten = ServerDatabase.CalculateEnlighten(client.Player);
                                    client.Player.SendUpdate(stream, client.Player.Enilghten, Game.MsgServer.MsgUpdate.DataType.EnlightPoints);
                                }
                            }
                            if (client.Player.QuestGUI.CheckQuest(20195, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20195);
                            if (client.Player.QuestGUI.CheckQuest(20199, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20199);
                            if (client.Player.QuestGUI.CheckQuest(20198, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20198);
                            if (client.Player.QuestGUI.CheckQuest(20197, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20197);
                            if (client.Player.QuestGUI.CheckQuest(20193, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20193);
                            if (client.Player.QuestGUI.CheckQuest(20191, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20191);
                            if (client.Player.QuestGUI.CheckQuest(20192, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20192);
                            if (client.Player.QuestGUI.CheckQuest(20196, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20196);
                            if (client.Player.QuestGUI.CheckQuest(20194, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20194);
                            if (client.Player.QuestGUI.CheckQuest(20200, MsgQuestList.QuestListItem.QuestStatus.Finished))
                                client.Player.QuestGUI.RemoveQuest(20200);
                            //if (client.Player.Flowers.FreeFlowers != 0)
                            //    client.Send(stream.FlowerCreate(Core.IsBoy(client.Player.Body) ? MsgFlower.FlowerAction.FlowerSender : MsgFlower.FlowerAction.Flower, 0, 0, client.Player.Flowers.FreeFlowers));
                            //if (client.Player.Level >= 90)
                            //{
                            //    client.Player.Enilghten = ServerDatabase.CalculateEnlighten(client.Player);
                            //    client.Player.SendUpdate(stream, client.Player.Enilghten, MsgUpdate.DataType.EnlightPoints);
                            //}
                            client.Player.ArenaKills = client.Player.ArenaDeads = 0;
                            client.Player.HitShoot = client.Player.MisShoot = 0;
                            client.OnlinePointsManager.Reset();
                            client.TournamentsManager.Reset();
                            client.LimitedDailyTimes.Reset();
                            client.RoyalPassManager.Reset();
                            client.Player.KeyBoxTRY = 3;
                            client.Player.lettersTRY = 3;
                            client.Player.LavaTRY = 3;
                            client.SendSysMesage("A new day has begun and all missions have been reset.");
                        }
                    }
                    //if (ActiveSquamas.Count <= 3)
                    //    NpcServer.LoadServerTraps();
                    ResetServerDay = (uint)DateTime.Now.DayOfYear;
                }
                catch (Exception e)
                {
                    ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                }
            }
            ResetStamp.Value = Clock.Value + 6000;
        }

        public static void LoadFileConfiguration()
        {
            IniFile PanelFile;
            PanelFile = new IniFile(Directory.GetCurrentDirectory() + "\\ServerConfiguration.ini", true);
            if (File.Exists(Directory.GetCurrentDirectory() + "\\ServerConfiguration.ini"))
            {
                ServerKernel.EncryptFiles.Decrypt(Directory.GetCurrentDirectory() + "\\ServerConfiguration.ini");
                IniFile IniFile;
                IniFile = new IniFile(Directory.GetCurrentDirectory() + "\\shell.ini", true);
                ServerKernel.Test_Center = IniFile.ReadString("GameServer", "TEST_CENTER", "False") == "True";
                ServerKernel.MonsterFromText = IniFile.ReadString("GameServer", "MonsterFromText", "False") == "True";
                ServerKernel.Allow_Server_Translate = IniFile.ReadString("GameServer", "Translator", "False") == "True";
                ServerKernel.CHANCE_LETTERS = PanelFile.ReadDouble("drop_chances", "CHANCE_LETTERS", 0.0);
                ServerKernel.CHANCE_PLUS_ONE = PanelFile.ReadDouble("drop_chances", "CHANCE_PLUS_ONE", 0.0);
                ServerKernel.CHANCE_METEOR = PanelFile.ReadDouble("drop_chances", "CHANCE_METEOR", 0.0);
                ServerKernel.CHANCE_GEMS = PanelFile.ReadDouble("drop_chances", "CHANCE_GEMS", 0.0);
                ServerKernel.CHANCE_STONE_ONE_ITEM = PanelFile.ReadDouble("drop_chances", "CHANCE_STONE_ONE_ITEM", 0.0);
                ServerKernel.CHANCE_STONE_TWO_ITEM = PanelFile.ReadDouble("drop_chances", "CHANCE_STONE_TWO_ITEM", 0.0);
                ServerKernel.CHANCE_DRAGONBALL_ITEM = PanelFile.ReadDouble("drop_chances", "CHANCE_DRAGONBALL_ITEM", 0.0);
                ServerKernel.CHANCE_EXP = PanelFile.ReadDouble("drop_chances", "CHANCE_EXPBALL", 0.0);
                ServerKernel.CHANCE_Key = PanelFile.ReadDouble("drop_chances", "CHANCE_Key", 0.0);
                //ServerKernel.SPELL_RATE = PanelFile.ReadUInt32("drop_chances", "SPELL_RATE", 0);
                ServerKernel.SPELL_RATE = PanelFile.ReadDouble("drop_chances", "SPELL_RATE", 0.0);
                ServerKernel.PROF_RATE = PanelFile.ReadDouble("drop_chances", "PROF_RATE", 0.0);

                //ServerKernel.PROF_RATE = PanelFile.ReadUInt32("drop_chances", "PROF_RATE", 0);
                ServerKernel.EXP_RATE = PanelFile.ReadUInt32("drop_chances", "EXP_RATE", 0);
                ServerKernel.AWARED_EXPERINCE_FROM_BOT = PanelFile.ReadByte("drop_chances", "AWARED_EXPERINCE_FROM_BOT", 0);
                ServerKernel.Max_PLUS = PanelFile.ReadByte("drop_chances", "Max_PLUS", 0);
                ServerKernel.Max_Bless = PanelFile.ReadByte("drop_chances", "Max_Bless", 0);
                ServerKernel.Max_Enchant = PanelFile.ReadByte("drop_chances", "Max_Enchant", 0);
                ServerKernel.MAX_UPLEVEL = PanelFile.ReadByte("drop_chances", "MAX_UPLEVEL", 0);
                ServerKernel.Bound_Equipments_Plus = PanelFile.ReadByte("drop_chances", "Bound_Equipments_Plus", 0);
                ServerKernel.ViewThreshold = PanelFile.ReadByte("drop_chances", "ViewThreshold", 18);
                ServerKernel.ONE_SOC_RATE = PanelFile.ReadInt32("drop_rate", "ONE_SOC_RATE", 0);
                ServerKernel.MAXIMUM_LETTER_DAILY_TIMES = PanelFile.ReadByte("drop_rate", "MAXIMUM_LETTER_DAILY_TIMES", 0);
                ServerKernel.CP_MONSTER_DROP_RATE[0] = PanelFile.ReadInt32("drop_rate", "CP_MONSTER_DROP_RATE0", 0);
                ServerKernel.CP_MONSTER_DROP_RATE[1] = PanelFile.ReadInt32("drop_rate", "CP_MONSTER_DROP_RATE1", 0);
                ServerKernel.NAME_CHANGE = (int)PanelFile.ReadUInt32("drop_rate", "NAME_CHANGE", 0);
                ServerKernel.NAME_CHANGE_RESET_LIMIT = (int)PanelFile.ReadUInt32("drop_rate", "NAME_CHANGE_RESET_LIMIT", 0);
                ServerKernel.GENDAR_CHANGE = (int)PanelFile.ReadUInt32("drop_rate", "GENDAR_CHANGE", 0);
                ServerKernel.CREATE_CLAN = (int)PanelFile.ReadUInt32("drop_rate", "CREATE_CLAN", 0);
                ServerKernel.STAY_ONLINE = (int)PanelFile.ReadUInt32("drop_rate", "STAY_ONLINE", 0);
                ServerKernel.CONQUER_LETTER_REWARD = (int)PanelFile.ReadUInt32("drop_rate", "CONQUER_LETTER_REWARD", 0);
                ServerKernel.TWO_SOC_RATE = PanelFile.ReadInt32("drop_rate", "TWO_SOC_RATE", 0);
                ServerKernel.MONTHLY_PK_REWARD = PanelFile.ReadInt32("drop_rate", "MONTHLY_PK_REWARD", 0);
                ServerKernel.QUALIFIER_PK_REWARD = PanelFile.ReadInt32("drop_rate", "QUALIFIER_PK_REWARD", 0);
                ServerKernel.QUALIFIER_HONOR_REWARD = PanelFile.ReadInt32("drop_rate", "QUALIFIER_HONOR_REWARD", 0);
                ServerKernel.ELITE_GUILD_WAR_Reward = PanelFile.ReadInt32("drop_rate", "ELITE_GUILD_WAR_Reward", 0);
                ServerKernel.GUILD_WAR_REWARD = PanelFile.ReadInt32("drop_rate", "GUILD_WAR_REWARD", 0);
                ServerKernel.CLASS_PK_WAR_REWARD = PanelFile.ReadInt32("drop_rate", "CLASS_PK_WAR_REWARD", 0);
                ServerKernel.CLASSIC_CLAN_WAR_REWARD = PanelFile.ReadInt32("drop_rate", "CLASSIC_CLAN_WAR_REWARD", 0);
                ServerKernel.CAPTURE_THE_FLAG_WAR_REWARD_CPS = PanelFile.ReadUInt32("drop_rate", "CAPTURE_THE_FLAG_WAR_REWARD_CPS", 0);
                ServerKernel.CAPTURE_THE_FLAG_WAR_REWARD_MONEY = PanelFile.ReadUInt32("drop_rate", "CAPTURE_THE_FLAG_WAR_REWARD_MONEY", 0);
                ServerKernel.ELITE_PK_TOURNAMENT_REWARD[3] = PanelFile.ReadUInt32("drop_rate", "ELITE_PK_TOURNAMENT_REWARD4", 0);
                ServerKernel.ELITE_PK_TOURNAMENT_REWARD[2] = PanelFile.ReadUInt32("drop_rate", "ELITE_PK_TOURNAMENT_REWARD3", 0);
                ServerKernel.ELITE_PK_TOURNAMENT_REWARD[1] = PanelFile.ReadUInt32("drop_rate", "ELITE_PK_TOURNAMENT_REWARD2", 0);
                ServerKernel.ELITE_PK_TOURNAMENT_REWARD[0] = PanelFile.ReadUInt32("drop_rate", "ELITE_PK_TOURNAMENT_REWARD1", 0);
                ServerKernel.TEAM_PK_TOURNAMENT_REWARD[3] = PanelFile.ReadUInt32("drop_rate", "TEAM_PK_TOURNAMENT_REWARD4", 0);
                ServerKernel.TEAM_PK_TOURNAMENT_REWARD[2] = PanelFile.ReadUInt32("drop_rate", "TEAM_PK_TOURNAMENT_REWARD3", 0);
                ServerKernel.TEAM_PK_TOURNAMENT_REWARD[1] = PanelFile.ReadUInt32("drop_rate", "TEAM_PK_TOURNAMENT_REWARD2", 0);
                ServerKernel.TEAM_PK_TOURNAMENT_REWARD[0] = PanelFile.ReadUInt32("drop_rate", "TEAM_PK_TOURNAMENT_REWARD1", 0);
                ServerKernel.SKILL_PK_TOURNAMENT_REWARD[3] = PanelFile.ReadUInt32("drop_rate", "SKILL_PK_TOURNAMENT_REWARD4", 0);
                ServerKernel.SKILL_PK_TOURNAMENT_REWARD[2] = PanelFile.ReadUInt32("drop_rate", "SKILL_PK_TOURNAMENT_REWARD3", 0);
                ServerKernel.SKILL_PK_TOURNAMENT_REWARD[1] = PanelFile.ReadUInt32("drop_rate", "SKILL_PK_TOURNAMENT_REWARD2", 0);
                ServerKernel.SKILL_PK_TOURNAMENT_REWARD[0] = PanelFile.ReadUInt32("drop_rate", "SKILL_PK_TOURNAMENT_REWARD1", 0);
                ServerKernel.KILLER_SYSTEM_REWARD[3] = PanelFile.ReadUInt32("drop_rate", "KILLER_SYSTEM_REWARD4", 0);
                ServerKernel.KILLER_SYSTEM_REWARD[2] = PanelFile.ReadUInt32("drop_rate", "KILLER_SYSTEM_REWARD3", 0);
                ServerKernel.KILLER_SYSTEM_REWARD[1] = PanelFile.ReadUInt32("drop_rate", "KILLER_SYSTEM_REWARD2", 0);
                ServerKernel.KILLER_SYSTEM_REWARD[0] = PanelFile.ReadUInt32("drop_rate", "KILLER_SYSTEM_REWARD1", 0);
                ServerKernel.TREASURE_THIEF_MIN = PanelFile.ReadInt32("drop_rate", "TREASURE_THIEF_MIN", 0);
                ServerKernel.TREASURE_THIEF_MAX = PanelFile.ReadInt32("drop_rate", "TREASURE_THIEF_MAX", 0);
                ServerKernel.ARENA_DAILY_RANKING[0] = PanelFile.ReadUInt32("drop_rate", "ARENA_DAILY_RANKING_1ST", 0);
                ServerKernel.ARENA_DAILY_RANKING[1] = PanelFile.ReadUInt32("drop_rate", "ARENA_DAILY_RANKING_2ND", 0);
                ServerKernel.ARENA_DAILY_RANKING[2] = PanelFile.ReadUInt32("drop_rate", "ARENA_DAILY_RANKING_3TH", 0);
                ServerKernel.MONEY_MONSTER_DROP_RATE[0] = PanelFile.ReadInt32("drop_rate", "MONEY_MONSTER_DROP_RATE_NORMAL", 0);
                ServerKernel.MONEY_MONSTER_DROP_RATE[1] = PanelFile.ReadInt32("drop_rate", "MONEY_MONSTER_DROP_RATE_VIP", 0);
                ServerKernel.MONSTER_SPWANS = PanelFile.ReadByte("monsters", "MONSTER_SPWANS", 0);
                ServerKernel.EncryptFiles.Encrypt(Directory.GetCurrentDirectory() + "\\ServerConfiguration.ini");
            }
            else
                ServerKernel.Log.SaveLog("can not load drop-panel", true, LogType.WARNING);
        }

        public static void Initialize()
        {
            ServerKernel.EncryptFiles = new EncryptFiles();
            ServerMaps = new Dictionary<uint, GameMap>();
            GamePoll = new ConcurrentDictionary<uint, GameClient>();
            NameUsed = new List<int>();
            ServerKernel.EncryptFiles.Decrypt(Directory.GetCurrentDirectory() + "\\StartupUser.ini");
            IniFile Cfg;
            Cfg = new IniFile(Directory.GetCurrentDirectory() + "\\StartupUser.ini", true);
            ServerKernel.Allow_User = Cfg.ReadString("config", "User", "");
            ServerKernel.Allow_Password = Cfg.ReadString("config", "Password", "");
            ServerKernel.Allow_Code = Cfg.ReadUInt16("config", "Code", 0);
            ServerKernel.EncryptFiles.Encrypt(Directory.GetCurrentDirectory() + "\\StartupUser.ini");
            IniFile IniFile;
            IniFile = new IniFile(Directory.GetCurrentDirectory() + "\\shell.ini", true);
            ServerKernel.Test_Center = IniFile.ReadString("GameServer", "TEST_CENTER", "False") == "True";
            //WorldGameChannel = new DiscordWorld(ServerKernel.DiscordAPI2);
            ServerKernel.LoginServerAddress = IniFile.ReadString("GameServer", "IPAddress", "");
            ServerKernel.GameServerPort = IniFile.ReadUInt16("GameServer", "Port", 5818);
            ServerKernel.ServerName = IniFile.ReadString("GameServer", "ServerName", "");
            ServerKernel.WEBSITE = IniFile.ReadString("GameServer", "WebSite", "");
            ServerKernel.DiscordLink = IniFile.ReadString("GameServer", "Discord", "");
            ServerKernel.MaxOnlinePlayer = IniFile.ReadUInt16("GameServer", "MaxOnlinePlayer", 500);
            ServerKernel.LoginServerPort = IniFile.ReadUInt16("LoginServer", "Port", 100);
            TopRankings.db_name = IniFile.ReadString("LoginServer", "DB", "");
            TopRankings.db_password = IniFile.ReadString("LoginServer", "Password", "");
            ServerKernel.Port_BackLog = IniFile.ReadUInt16("InternetPort", "BackLog", 100);
            ServerKernel.Port_ReceiveSize = IniFile.ReadUInt16("InternetPort", "ReceiveSize", 8194);
            ServerKernel.Port_SendSize = IniFile.ReadUInt16("InternetPort", "SendSize", 1024);
            ServerKernel.Blowfish = IniFile.ReadString("Blowfish", "Key", "");
            ServerKernel.CO2FOLDER = IniFile.ReadString("Database", "CO2FOLDER", "");
            if (ServerKernel.Test_Center)
                ServerKernel.Log.SaveLog("Test Center Mode: Enabled", true, LogType.DEBUG);
            else
                ServerKernel.Log.SaveLog("Test Center Mode: Disabled", true, LogType.DEBUG);
            RebornInfo = new RebornInfomations();
            RebornInfo.Load();
            ITEM_Counter.Set(IniFile.ReadUInt32("Database", "ItemUID", 0));
            _ = ITEM_Counter.Next;
            ClientCounter.Set(IniFile.ReadUInt32("Database", "ClientUID", 1000000));
            _ = ClientCounter.Next;
            ResetServerDay = IniFile.ReadUInt32("Database", "Day", 0);
            TheChosenProject.Game.MsgNpc.Scripts.Quests.Solomon.TimeRealMonsterIsDead = DateTime.FromBinary(IniFile.ReadInt64("QuestSolomon", "TimeRealMonsterIsDead", 0));
            //MsgSchedules.PkWar.WinnerUID = IniFile.ReadUInt32("Tournaments", "PkWarWinner", 0);
            //MsgSchedules.MonthlyPKWar.WinnerUID = IniFile.ReadUInt32("Tournaments", "MonthlyPKWar", 0);
            LoadFileConfiguration();
            ItemsBase = new ItemType();
            //RifineryItems = new Rifinery();
            //DBRerinaryBoxes = new RefinaryBoxes();
            Rayzo_Panle.LoadAttack();
            ItemsBase.Loading();
            Insults = new InsultManager();
            Insults.Load();           
            Pets = new WindowsAPI.IniFile("Pets.ini");
            EShopFile.Load();
            //EShopV2File.Load();
            HonorShop.Load();
            RacePointShop.Load();
            ShopFile.Load();
            SystemBannedPC.Load();
            SystemBannedAccount.Load();
            GroupServerList.Load();
            //VIPExperience.Load();
            LoadExpInfo();
            DataCore.AtributeStatus.Load();
            ServerKernel.MapRegions = new MapRegions();
            ServerKernel.MapRegions.Load();
            GameMap.LoadMaps();
            NpcServer.LoadNpcs();
            NpcServer.LoadServerTraps();
            Magic.Load();
            LoadMonsters();
            Tranformation.Int();
            QuestInfo.Init();
            LoadPortals();
            Program.DropRuleBase = new DropRule();
            Program.DropRuleBase.Load();
            //SubClassInfo.Load();
            FlowersTable.Load();
            NobilityTable.Load();
            Associate.Load();
            NpcServer.LoadSobNpcs();
            GuildTable.Load();
            ClanTable.Load();
            MsgSchedules.ClassPkWar.Load();
            MsgSchedules.ElitePkTournament.Load();
            MsgSchedules.TeamPkTournament.Load();
            MsgSchedules.SkillTeamPkTournament.Load();
            //Game.MsgTournaments.MsgSchedules.MrConquer.Load();
            //Game.MsgTournaments.MsgSchedules.MsConquer.Load();
            //Game.MsgTournaments.MsgSchedules.CouplesPKWar.Load();

            SpecialTitles.LoadDBInformation();
            TheCrimeTable.Load();
            Statue.Load();
            KOBoard.KOBoardRanking.Load();
            Disdain.Load();
            Arena.Load();
            TeamArena.Load();
            TutorInfo.Load();
            Artificialintelligence.Load();
            InfoDemonExterminators.Create();
            QueueContainer.Load();
            Lottery.LoadLotteryItems();
            LoadMapName();
            BossDatabase.Load();
            SilentWords.Load();
            ISchedule.Load();
            LoadDatabase();
            MiningTable.Load();
            MsgSchedules.ScoresWar.CreateFurnitures();
            FullLoading = true;
        }

        public static byte NameChangeCount(byte vipLevel)
        {
            byte chance;
            chance = 1;
            switch (vipLevel)
            {
                case 1:
                    chance = 2;
                    break;
                case 2:
                    chance = 3;
                    break;
                case 3:
                    chance = 4;
                    break;
                case 4:
                    chance = 5;
                    break;
                case 5:
                    chance = 10;
                    break;
                case 6:
                    chance = 30;
                    break;
            }
            return chance;
        }

        public static void LoadMapName()
        {
            if (!File.Exists(ServerKernel.CO2FOLDER + "GameMapEx.ini"))
                return;
            foreach (GameMap map in ServerMaps.Values)
            {
                IniFile ini;
                ini = new IniFile("GameMapEx.ini");
                map.Name = ini.ReadString(map.ID.ToString(), "Name", ServerKernel.ServerName);
            }
        }

        public static void LoadExpInfo()
        {
            if (File.Exists(ServerKernel.CO2FOLDER + "levexp.txt"))
            {
                using (StreamReader read = File.OpenText(ServerKernel.CO2FOLDER + "levexp.txt"))
                {
                    while (true)
                    {
                        string GetLine;
                        GetLine = read.ReadLine();
                        if (GetLine == null)
                            break;
                        string[] line;
                        line = GetLine.Split(' ');
                        DBLevExp exp;
                        exp = new DBLevExp
                        {
                            Action = (DBLevExp.Sort)byte.Parse(line[0]),
                            Level = byte.Parse(line[1]),
                            Experience = ulong.Parse(line[2]),
                            UpLevTime = int.Parse(line[3]),
                            MentorUpLevTime = int.Parse(line[4])
                        };
                        if (!LevelInfo.ContainsKey(exp.Action))
                            LevelInfo.Add(exp.Action, new Dictionary<byte, DBLevExp>());
                        LevelInfo[exp.Action].Add(exp.Level, exp);
                    }
                    return;
                }
            }
            GC.Collect();
        }
        public static void LoadMonsters()
        {
            try
            {
                IniFile ini;
                ini = new IniFile("");
                string[] files;
                files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Monsters\\");
                foreach (string fname in files)
                {
                    if (string.IsNullOrEmpty(fname))
                        continue;
                    ini.FileName = fname;
                    MonsterFamily Family;
                    Family = new MonsterFamily
                    {
                        ID = ini.ReadUInt32("cq_monstertype", "id", 0),
                        Name = ini.ReadString("cq_monstertype", "name", "INVALID_MOB"),
                        Level = ini.ReadUInt16("cq_monstertype", "level", 0),
                        MaxAttack = ini.ReadInt32("cq_monstertype", "attack_max", 0),
                        MinAttack = ini.ReadInt32("cq_monstertype", "attack_min", 0),
                        HeadId = ini.ReadUInt32("cq_monstertype", "helmet_type", 0),
                        ArmorId = ini.ReadUInt32("cq_monstertype", "armor_type", 0),
                        RightWeaponId = ini.ReadUInt32("cq_monstertype", "weaponr_type", 0),
                        LeftWeaponId = ini.ReadUInt32("cq_monstertype", "weaponl_type", 0)
                    };
                    if (Family.Name == "INVALID_MOB" || Family.Level == 0 || Family.ID == 0 || Family.MinAttack > Family.MaxAttack)
                    {
                        ServerKernel.Log.SaveLog("MONSTER FILE CORRUPT: \r\n" + fname + "\r\n", true, LogType.WARNING);
                        continue;
                    }
                    Family.Defense = ini.ReadUInt16("cq_monstertype", "defence", 0);
                    Family.Mesh = ini.ReadUInt16("cq_monstertype", "lookface", 0);
                    Family.MaxHealth = ini.ReadInt32("cq_monstertype", "life", 0);
                    Family.ViewRange = 16;
                    Family.AttackRange = ini.ReadSByte("cq_monstertype", "attack_range", 0);
                    Family.Dodge = ini.ReadByte("cq_monstertype", "dodge", 0);
                    Family.DropBoots = ini.ReadByte("cq_monstertype", "drop_shoes", 0);
                    Family.DropNecklace = ini.ReadByte("cq_monstertype", "drop_necklace", 0);
                    Family.DropRing = ini.ReadByte("cq_monstertype", "drop_ring", 0);
                    Family.DropArmet = ini.ReadByte("cq_monstertype", "drop_armet", 0);
                    Family.DropArmor = ini.ReadByte("cq_monstertype", "drop_armor", 0);
                    Family.DropShield = ini.ReadByte("cq_monstertype", "drop_shield", 0);
                    Family.DropWeapon = ini.ReadByte("cq_monstertype", "drop_weapon", 0);
                    Family.DropMoney = ini.ReadUInt16("cq_monstertype", "drop_money", 0);
                    Family.DropHPItem = ini.ReadUInt32("cq_monstertype", "drop_hp", 0);
                    Family.DropMPItem = ini.ReadUInt32("cq_monstertype", "drop_mp", 0);
                    Family.Boss = ini.ReadByte("cq_monstertype", "Boss", 0);
                    Family.Defense2 = ini.ReadInt32("cq_monstertype", "defence2", 0);
                    Family.AttackRange = ini.ReadSByte("cq_monstertype", "attack_range", 0);
                    Family.MoveSpeed = ini.ReadInt32("cq_monstertype", "move_speed", 0);
                    Family.AttackSpeed = ini.ReadInt32("cq_monstertype", "attack_speed", 0);
                    Family.SpellId = ini.ReadUInt32("cq_monstertype", "magic_type", 0);
                    Family.ExtraCritical = ini.ReadUInt32("cq_monstertype", "critical", 0);
                    Family.ExtraBreack = ini.ReadUInt32("cq_monstertype", "break", 0);
                    Family.extra_battlelev = ini.ReadInt32("cq_monstertype", "extra_battlelev", 0);
                    Family.extra_exp = ini.ReadInt32("cq_monstertype", "extra_exp", 0);
                    Family.extra_damage = ini.ReadInt32("cq_monstertype", "extra_damage", 0);
                    //Family.DropSpecials = new SpecialItemWatcher[ini.ReadInt32("SpecialDrop", "Count", 0)];
                    //for (int i = 0; i < Family.DropSpecials.Length; i++)
                    //{
                    //    string[] Data;
                    //    Data = ini.ReadString("SpecialDrop", i.ToString(), "", 32).Split(',');
                    //    Family.DropSpecials[i] = new SpecialItemWatcher(uint.Parse(Data[0]), int.Parse(Data[1]));
                    //}
                    Family.CreateItemGenerator();
                    Family.CreateMonsterSettings();
                    MonsterFamilies.Add(Family.ID, Family);
                }
                if (ServerKernel.MonsterFromText)
                {
                    using (StreamReader reader = new StreamReader(ServerKernel.CO2FOLDER + "\\Spawns.txt"))
                    {
                        string[] values;
                        values = reader.ReadToEnd().Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] array;
                        array = values;
                        foreach (string line in array)
                        {
                            string[] data;
                            data = line.Split(',');
                            uint ID2;
                            ID2 = uint.Parse(data[0]);
                            //if (DateTime.Now.Minute == 10)//holloween
                            //{
                            //    ID2 = 977;
                            //}
                            //else
                                ID2 = uint.Parse(data[0]);
                            uint MapId;
                            MapId = uint.Parse(data[1]);
                            MobCollection colletion;
                            colletion = new MobCollection(MapId);
                            _ = 8800;
                            if (colletion.ReadMap() && MonsterFamilies.TryGetValue(ID2, out var famil) && !TheChosenProject.Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(famil.ID))
                            {
                                MonsterFamily Monster2;
                                Monster2 = famil.Copy();
                                Monster2.SpawnX = ushort.Parse(data[2]);
                                Monster2.SpawnY = ushort.Parse(data[3]);
                                Monster2.MaxSpawnX = (ushort)(Monster2.SpawnX + ushort.Parse(data[4]));
                                Monster2.MaxSpawnY = (ushort)(Monster2.SpawnY + ushort.Parse(data[5]));
                                Monster2.MapID = MapId;
                                if (Monster2.MapID == 1082)
                                    Monster2.SpawnCount = 1;//"maxnpc", 0);//max_per_gen", 0);
                                else
                                    Monster2.SpawnCount = int.Parse(data[6]);
                                Monster2.rest_secs = int.Parse(data[7]);
                                if (Monster2.MapID == 1011 || Monster2.MapID == 3071 || Monster2.MapID == 1770 || Monster2.MapID == 1771 || Monster2.MapID == 1772 || Monster2.MapID == 1773 || Monster2.MapID == 1774 || Monster2.MapID == 1775 || Monster2.MapID == 1777 || Monster2.MapID == 1782 || Monster2.MapID == 1785 || Monster2.MapID == 1786 || Monster2.MapID == 1787 || Monster2.MapID == 1794)
                                    Monster2.SpawnCount = int.Parse(data[8]);
                                if (Monster2.ID == 18)
                                    Monster2.SpawnCount *= 2;
                                colletion.Add(Monster2);
                            }
                        }
                    }
                }
               // else
                {
                    string[] directories;
                    directories = Directory.GetDirectories(ServerKernel.CO2FOLDER + "\\MonsterSpawns\\");
                    foreach (string fmap in directories)
                    {
                        if (!uint.TryParse(fmap.Remove(0, (ServerKernel.CO2FOLDER + "\\MonsterSpawns\\").Length), out var tMapID))
                            continue;
                        MobCollection Linker;
                        Linker = new MobCollection(tMapID);
                        if (!Linker.ReadMap())
                            continue;
                        string[] directories2;
                        directories2 = Directory.GetDirectories(fmap);
                        foreach (string fmobtype in directories2)
                        {
                            string[] files2;
                            files2 = Directory.GetFiles(fmobtype);
                            foreach (string ffile in files2)
                            {
                                ini.FileName = ffile;
                                uint ID;
                                ID = ini.ReadUInt32("cq_generator", "npctype", 0);
                                if (MonsterFamilies.TryGetValue(ID, out var Spawn) && !BossDatabase.Bosses.ContainsKey(Spawn.ID) && !TheChosenProject.Game.MsgMonster.MonsterRole.SpecialMonsters.Contains(Spawn.ID))
                                {
                                    MonsterFamily Monster;
                                    Monster = Spawn.Copy();
                                    Monster.SpawnX = ini.ReadUInt16("cq_generator", "bound_x", 0);
                                    Monster.SpawnY = ini.ReadUInt16("cq_generator", "bound_y", 0);
                                    Monster.MaxSpawnX = (ushort)(Monster.SpawnX + ini.ReadUInt16("cq_generator", "bound_cx", 0));
                                    Monster.MaxSpawnY = (ushort)(Monster.SpawnY + ini.ReadUInt16("cq_generator", "bound_cy", 0));
                                    Monster.MapID = ini.ReadUInt32("cq_generator", "mapid", 0);
                                    Monster.maxnpc = ini.ReadByte("cq_generator", "maxnpc", 0);
                                    Monster.SpawnCount = ini.ReadByte("cq_generator", "max_per_gen", 0);
                                    Monster.rest_secs = ini.ReadByte("cq_generator", "rest_secs", 0);
                                    Linker.Add(Monster);
                                }
                            }
                        }
                    }
                }
                GC.Collect();
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }
        public static unsafe void AddMapMonster2(ServerSockets.Packet stream, Role.GameMap map, uint ID, ushort x, ushort y, ushort max_x, ushort max_y, byte count, uint DinamicID = 0, bool RemoveOnDead = true
    , Game.MsgFloorItem.MsgItemPacket.EffectMonsters m_effect = Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None, string streffect = "", Mobs.Base @base = null)
        {
            if (map.MonstersColletion == null)
            {
                map.MonstersColletion = new Game.MsgMonster.MobCollection(map.ID);
            }
            if (map.MonstersColletion.ReadMap())
            {
                Game.MsgMonster.MonsterFamily famil;
                if (MonsterFamilies.TryGetValue(ID, out famil))
                {
                    Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                    Monster.SpawnX = x;
                    Monster.SpawnY = y;
                    Monster.MaxSpawnX = (ushort)(x + max_x);
                    Monster.MaxSpawnY = (ushort)(y + max_y);
                    Monster.MapID = map.ID;
                    Monster.SpawnCount = count;
                    
                    Game.MsgMonster.MonsterRole rolemonster = map.MonstersColletion.Add2(Monster, RemoveOnDead, DinamicID, true, false, @base);
                    if (rolemonster.MonsterBase != null)
                    {
                        if (rolemonster.MonsterBase.ID == IDMonster.NemesisTyrant)
                        {
                            rolemonster.HitPoints = rolemonster.HitPoints / 2;
                        }
                    }
                    if (rolemonster == null)
                    {
                        //MyLogger.GameServer.WriteError("Error: Monster Spawn ID:{0} MAPID:{1} X:{2} Y:{3}.", ID, map.ID, x, y);
                        ServerKernel.Log.SaveLog("Error: Monster Spawn ID:"+0+" MAPID:"+1+" X:"+2+" Y:"+3+""+ID+","+map.ID+","+ x+","+ y+"", true, LogType.WARNING);

                        return;
                    }

                    Game.MsgServer.ActionQuery action = new Game.MsgServer.ActionQuery()
                    {
                        ObjId = rolemonster.UID,
                        Type = Game.MsgServer.ActionType.RemoveEntity
                    };
                    rolemonster.Send(stream.ActionCreate(&action));
                    rolemonster.Send(rolemonster.GetArray(stream, false));

                    if (streffect != null)
                    {
                        rolemonster.SendString(stream, MsgStringPacket.StringID.Effect, streffect);
                    }

                    if (m_effect != Game.MsgFloorItem.MsgItemPacket.EffectMonsters.None && rolemonster != null)
                    {
                        Game.MsgFloorItem.MsgItemPacket effect = Game.MsgFloorItem.MsgItemPacket.Create();
                        effect.m_UID = (uint)m_effect;
                        effect.m_X = rolemonster.X;
                        effect.m_Y = rolemonster.Y;
                        effect.DropType = MsgDropID.Earth;
                        rolemonster.Send(stream.ItemPacketCreate(effect));
                        rolemonster.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, "glebesword");
                    }
                    if (rolemonster.HitPoints > 65535)
                    {
                        Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, rolemonster.UID, 2);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, rolemonster.Family.MaxHealth);
                        stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, rolemonster.HitPoints);
                        stream = Upd.GetArray(stream);
                        rolemonster.Send(stream);
                    }
                    //if (Monster.Boss == 1)
                    //{
                    //    if (Monster.Name != "LavaBeast")
                    //    {
                    //        Program.DiscordCBosses.Enqueue($"``{Monster.Name} has appeared around ({Monster.SpawnX}, {Monster.SpawnY}) on the {map.Name} Hurry and go defeat the beast``");
                    //    }
                    //}
                }
            }
        }


        public unsafe static void AddMapMonster(Packet stream, GameMap map, uint ID, ushort x, ushort y, ushort max_x, ushort max_y, byte count, uint DinamicID = 0, bool RemoveOnDead = true, MsgItemPacket.EffectMonsters m_effect = MsgItemPacket.EffectMonsters.None, string streffect = "")
        {
            if (map.MonstersColletion == null)
                map.MonstersColletion = new MobCollection(map.ID);
            if (!map.MonstersColletion.ReadMap() || !MonsterFamilies.TryGetValue(ID, out var famil))
                return;

            MonsterFamily Monster;
                Monster = famil.Copy();
            Monster.SpawnX = x;
            Monster.SpawnY = y;
            Monster.MaxSpawnX = (ushort)(x + max_x);
            Monster.MaxSpawnY = (ushort)(y + max_y);
            Monster.MapID = map.ID;
            Monster.SpawnCount = count;
            MonsterRole rolemonster;
            rolemonster = map.MonstersColletion.Add(Monster, RemoveOnDead, DinamicID, true);
            if (rolemonster == null)
            {
                ServerKernel.Log.SaveLog("Could not load monster spwan ", true, LogType.WARNING);
                return;
            }
            ActionQuery actionQuery;
            actionQuery = default(ActionQuery);
            actionQuery.ObjId = rolemonster.UID;
            actionQuery.Type = ActionType.RemoveEntity;
            ActionQuery action;
            action = actionQuery;
            rolemonster.Send(stream.ActionCreate(&action));
            rolemonster.Send(rolemonster.GetArray(stream, false));
            if (streffect != null)
                rolemonster.SendString(stream, MsgStringPacket.StringID.Effect, streffect);
            if (m_effect != 0 && rolemonster != null)
            {
                MsgItemPacket effect;
                effect = MsgItemPacket.Create();
                effect.m_UID = (uint)m_effect;
                effect.m_X = rolemonster.X;
                effect.m_Y = rolemonster.Y;
                effect.DropType = MsgDropID.Earth;
                rolemonster.Send(stream.ItemPacketCreate(effect));
                rolemonster.SendString(stream, MsgStringPacket.StringID.Effect, "glebesword");
            }
            if (rolemonster.HitPoints > 65535)
            {
                MsgUpdate Upd;
                Upd = new MsgUpdate(stream, rolemonster.UID, 2);
                stream = Upd.Append(stream, MsgUpdate.DataType.MaxHitpoints, rolemonster.Family.MaxHealth);
                stream = Upd.Append(stream, MsgUpdate.DataType.Hitpoints, rolemonster.HitPoints);
                stream = Upd.GetArray(stream);
                rolemonster.Send(stream);
            }
            if (Monster.Boss == 1)
            {
                if (Monster.Name != "LavaBeast")
                {
                    if (map.Name == "Lucky7")
                    {
                        map.Name = "UAConquer New Map";
                    }
                    //if(DatabaseConfig.discord_stat)
                        //Program.DiscordCBosses.Enqueue($"`{Monster.Name} has appeared around ({Monster.SpawnX}, {Monster.SpawnY}) on the {map.Name} Hurry and go defeat the beast`");
                    /// Program.DiscordEventsAPI.Enqueue
                }
            }
            else
            {
                if (Monster.Name != "LavaBeast")
                {
                    if(map.Name == "Lucky7")
                    {
                        map.Name = "UAConquer New Map";
                    }
                    //Program.DiscordCBosses.Enqueue($"`{Monster.Name} has appeared around ({Monster.SpawnX}, {Monster.SpawnY}) on the {map.Name} Hurry and go defeat the beast`");
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The "+ Monster.Name +" have spawned in the "+ map.Name  + "! Hurry to kill them.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    /// Program.DiscordEventsAPI.Enqueue
                }
            }
        }

        public static void LoadDatabase()
        {
            try
            {
                string[] files;
                files = Directory.GetFiles(ServerKernel.CO2FOLDER + "\\Users\\");
                foreach (string fname in files)
                {
                    IniFile IniFile;
                    IniFile = new IniFile(fname)
                    {
                        FileName = fname
                    };
                    string name;
                    name = IniFile.ReadString("Character", "Name", "");
                    NameUsed.Add(name.GetHashCode());
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void SaveDatabase()
        {
            if (!FullLoading)
                return;
            try
            {
                try
                {
                    Save(Associate.Save);
                }
                catch (Exception e3)
                {
                    ServerKernel.Log.SaveLog(e3.ToString(), false, LogType.EXCEPTION);
                }
                try
                {
                    Save(GuildTable.Save);
                }
                catch (Exception e2)
                {
                    ServerKernel.Log.SaveLog(e2.ToString(), false, LogType.EXCEPTION);
                }
                IniFile IniFile;
                IniFile = new IniFile("")
                {
                    FileName = Directory.GetCurrentDirectory() + "\\shell.ini"
                };
                IniFile.Write("Database", "ItemUID", ITEM_Counter.Count);
                IniFile.Write("Database", "ClientUID", ClientCounter.Count);
                IniFile.Write("Database", "Day", ResetServerDay);
                IniFile.Write("Tournaments", "PkWarWinner", MsgSchedules.PkWar.WinnerUID);
                IniFile.Write("Tournaments", "MonthlyPKWar", MsgSchedules.MonthlyPKWar.WinnerUID);
                IniFile.Write<long>("QuestSolomon", "TimeRealMonsterIsDead", TheChosenProject.Game.MsgNpc.Scripts.Quests.Solomon.TimeRealMonsterIsDead.Ticks);
                Save(ClanTable.Save);
                Save(QueueContainer.Save);
                Save(MsgSchedules.GuildWar.Save);
                Save(MsgSchedules.EliteGuildWar.Save);
                Save(TheCrimeTable.Save);
                Save(Arena.Save);
                Save(BossDatabase.Save);
                Save(ISchedule.Save);
                Save(TeamArena.Save);
                Save(MsgSchedules.ClassPkWar.Save);
                Save(MsgSchedules.ElitePkTournament.Save);
                Save(MsgSchedules.TeamPkTournament.Save);
                Save(MsgSchedules.SkillTeamPkTournament.Save);
                Save(SystemBannedPC.Save);
                Save(SystemBannedAccount.Save);
                Save(MsgSchedules.CityWar.Save);
                //Save(VIPExperience.Save);
                IniFile = new IniFile("")
                {
                    FileName = Directory.GetCurrentDirectory() + "\\shell.ini"
                };
                IniFile.Write("Database", "ItemUID", ITEM_Counter.Count);
                IniFile.Write("Database", "ClientUID", ClientCounter.Count);
                IniFile.Write("Database", "Day", ResetServerDay);
                IniFile.Write("Tournaments", "MonthlyPKWar", MsgSchedules.MonthlyPKWar.WinnerUID);
                IniFile.Write("Tournaments", "PkWarWinner", MsgSchedules.PkWar.WinnerUID);
                IniFile.Write<long>("QuestSolomon", "TimeRealMonsterIsDead", TheChosenProject.Game.MsgNpc.Scripts.Quests.Solomon.TimeRealMonsterIsDead.Ticks);
                Save(Statue.Save);
                Save(KOBoard.KOBoardRanking.Save);
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void Save(Action obj)
        {
            try
            {
                obj();
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public static void LoadPortals()
        {
            ServerKernel.Log.SaveLog("Loading portals data...", true);
            if (File.Exists(ServerKernel.CO2FOLDER + "portals.ini"))
            {
                using (StreamReader read = File.OpenText(ServerKernel.CO2FOLDER + "portals.ini"))
                {
                    ushort count;
                    count = 0;
                    while (true)
                    {
                        string lines;
                        lines = read.ReadLine();
                        if (lines == null)
                            break;
                        ushort.Parse(lines.Split('[')[1].ToString().Split(']')[0]);
                        ushort Count;
                        Count = ushort.Parse(read.ReadLine().Split('=')[1]);
                        for (ushort x = 0; x < Count; x = (ushort)(x + 1))
                        {
                            Portal portal;
                            portal = new Portal();
                            string[] line;
                            line = read.ReadLine().Split('=')[1].Split(' ');
                            portal.MapID = ushort.Parse(line[0]);
                            portal.X = ushort.Parse(line[1]);
                            portal.Y = ushort.Parse(line[2]);
                            string[] dline;
                            dline = read.ReadLine().Split('=')[1].Split(' ');
                            portal.Destiantion_MapID = ushort.Parse(dline[0]);
                            portal.Destiantion_X = ushort.Parse(dline[1]);
                            portal.Destiantion_Y = ushort.Parse(dline[2]);
                            if (ServerMaps.ContainsKey(portal.MapID))
                                ServerMaps[portal.MapID].Portals.Add(portal);
                            count = (ushort)(count + 1);
                        }
                    }
                }
            }
            GC.Collect();
        }
    }
}
