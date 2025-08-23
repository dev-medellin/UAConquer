using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerCore;
using System;
using System.Collections.Generic;

namespace TheChosenProject
{
  public static class ServerKernel
  {
        public static byte AWARED_EXPERINCE_FROM_BOT = 1;
        //public static double AWARED_EXPERINCE_FROM_BOT = 1;
        public static byte MAX_CHAINS_HITS = 255;
    public static byte MAX_BOT_TYPE = 1;//bahaa
    public static byte MIN_Offline_ = 1;
    public static byte MAX_Offline_ = 7;
    public static int QUICK_UPGRADE_ITEM_LEVEL_COST = 215;
    public const uint EXCHANGE_OFFICER_CPS = 100;
    public static byte MAX_UPLEVEL = 137;
    public static int TOTAL_CPS_HUNTING = 0;
    public static int TOTAL_Silver_HUNTING = 0;
    public static byte Max_PLUS = 12;
        public const sbyte pScreenDistance = 19;
        public static byte Max_Enchant = byte.MaxValue;
    public static byte Max_Bless = 7;
    public static int Jail_Wanted = 100000;
    public static byte MAXIMUM_LETTER_DAILY_TIMES = 3;
    public static byte MONSTER_SPWANS = 3;
    public static bool MonsterFromText = false;
    public static bool MonsterDBSpawn = false;
    public static string XtremeTopLink = "https://www.xtremetop100.com/in.php?site=1132377669";
    public static readonly int[] MONEY_MONSTER_DROP_RATE = new int[2]
    {
      100,
      300
    };
    public static readonly int[] CP_MONSTER_DROP_RATE = new int[2]
    {
      1,
      2
    };
    public static double CHANCE_LETTERS = 0.002;
    public static double CHANCE_PLUS_ONE = 0.005;
    public static double CHANCE_METEOR = 0.002;
    public static double CHANCE_GEMS = 0.003;
    public static double CHANCE_MATRIAL = 0.003;
    public static double CHANCE_STONE_ONE_ITEM = 1;
    public static double CHANCE_STONE_TWO_ITEM = 0.004;
    public static double CHANCE_DRAGONBALL_ITEM = 1;
    public static double CHANCE_EXP = 0.005;
    public static double CHANCE_Key = 0.005;
    public static readonly uint[] ARENA_DAILY_RANKING = new uint[3]
    {
      15000U,
      10000U,
      5000U
    };
    public static readonly int[] SASH_UPGRADE = new int[2]
    {
      1075,
      3225
    };
    public static readonly int[] SUPER_KIT_MERCHANT_GEARS = new int[2]
    {
      749,
      449
    };
    public static readonly int[] PK_LESS_POINT = new int[3]
    {
      250000000,
      50000000,
      5000000
    };
    public static int NAME_CHANGE = 20000;
    public static int NAME_CHANGE_RESET_LIMIT = 150000;
    public static int GENDAR_CHANGE = 1075;
    public static int CREATE_CLAN = 1075;
    public static int[] UpdateLevelAmount = new int[4]
    {
      1000000,
      2000000,
      3500000,
      7500000
    };
    public static int[] UpdateBPAmount = new int[4]
    {
      2500000,
      12000000,
      35000000,
      120000000
    };
    public const int MOON_BOX_PRICE = 200;
    public const int EMERALD_PRICE = 100;
    public const int EUXENITE_ORE_PRICE = 20;
    public static readonly int[] KINGS_ROOM_REWARD = new int[4]
    {
      1000,
      2000,
      3000,
      4000
    };
    public static bool StaticGUIType = true;
    public static uint POLE_DOMINATION_REWARD = 2150;
    public static uint SmallCityGuilWar_REWARD = 2150;
    public static uint Guild_DeathMatch_REWARD = 2150;
    public static int STAY_ONLINE = 215;
    public static int CONQUER_LETTER_REWARD = 2150;
    public const int HIDEN_SEEK = 2150;
    public static int MONTHLY_PK_REWARD = 10000;
    public static int QUALIFIER_PK_REWARD = 1075;
    public static int QUALIFIER_HONOR_REWARD = 25;
    public static int ELITE_GUILD_WAR_Reward = 100000;
    public static int GUILD_WAR_REWARD = 500000;
    public static int CLASS_PK_WAR_REWARD = 200000;
    public static int CLASSIC_CLAN_WAR_REWARD = 100000;
    public static double ONE_SOC_RATE = 0.002;
    public static double TWO_SOC_RATE = 0.002;
    public static uint CAPTURE_THE_FLAG_WAR_REWARD_CPS = 300000;
    public static uint CAPTURE_THE_FLAG_WAR_REWARD_MONEY = 300000000;
    public static int TREASURE_THIEF_MIN = 1000;
    public static int TREASURE_THIEF_MAX = 1000;
    public static readonly uint[] KILLER_SYSTEM_REWARD = new uint[5]
    {
      0U,
      100U,
      200U,
      300U,
      400U
    };
    public static readonly uint[] ELITE_PK_TOURNAMENT_REWARD = new uint[4]
    {
      100000U,
      80000U,
      60000U,
      30000U
    };
    public static readonly uint[] TEAM_PK_TOURNAMENT_REWARD = new uint[4]
    {
      100000U,
      80000U,
      60000U,
      30000U
    };
    public static readonly uint[] SKILL_PK_TOURNAMENT_REWARD = new uint[4]
    {
      100000U,
      80000U,
      60000U,
      30000U
    };
    public static bool AutoMaintenance = true;
    public static DateTime Maintenance = DateTime.Today.AddHours(15);
        //public static string CO2FOLDER = "C:\\Users\\lukal\\OneDrive\\Desktop\\Database\\Database";
        public static string CO2FOLDER = DatabaseConfig.DatabaseLoc;
        public static string VOTE_EXTREME = "https://www.xtremetop100.com/in.php?site=1132377271";
    public static string LoginServerAddress = "";
    public static ushort GameServerPort = 5818;
    public static ushort LoginServerPort = 9959;
    public static string ServerName = "";
    public static string DiscordLink = "https://discord.gg/dSGSPGm9pK";
    public static string WEBSITE = "https://www.dsconquer.com/";
    public static ushort Port_BackLog;
    public static ushort Port_ReceiveSize = 8191;
    public static ushort Port_SendSize = 8191;
    //public static uint SPELL_RATE = 1;
    public static double SPELL_RATE = 1;
    public static double PROF_RATE = 1;
    //public static uint PROF_RATE = 1;
    public static uint EXP_RATE = 1;
    public static uint MAX_USER_LOGIN_ON_PC = 7;
    public static uint PhysicalDamage = 100;
    public static string Blowfish = "";
    public static ushort MaxOnlinePlayer = 800;
    public static bool Test_Center = false;
    public static bool Allow_Server_Translate = false;
    public static byte ViewThreshold = 18;
    public static byte Bound_Equipments_Plus = 0;
    public static string DiscordAPI = "https://discord.com/api/webhooks/1328842658357182474/bbpaQp9PH9n4CwHugdTTcjP_SprboduQBQk_MgcV9183gA-BSfvki7ewZLWXjUBVAGjZ";
    public static string DiscordAPI2 = "https://discord.com/api/webhooks/1328842658357182474/bbpaQp9PH9n4CwHugdTTcjP_SprboduQBQk_MgcV9183gA-BSfvki7ewZLWXjUBVAGjZ";
    public static byte Allow_Monthly = 9;
    public static bool EnableServer = true;
    public static string Allow_User = "CO";
    public static string Allow_Password = "test";
    public static uint Allow_Code = 1989;
    public static DateTime StartDate;
    public static Dictionary<uint, IEventRewards> EventRewards = new Dictionary<uint, IEventRewards>();
    public static Dictionary<string, ChatClient> ChatClients = new Dictionary<string, ChatClient>();
    public static LogWriter Log = new LogWriter(Environment.CurrentDirectory + "\\");
    public static List<TheChosenProject.Database.SilentWords.Words> SilentWords = new List<TheChosenProject.Database.SilentWords.Words>();
    public static MapRegions MapRegions;

    public static ServerManager ServerManager { get; set; }

    public static EncryptFiles EncryptFiles { get; set; }

    public static string GetStrTask(GameClient user, QuestInfo.DBQuest dbquest)
    {
      if (user.Player.QuestGUI.CheckQuest((uint) dbquest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
        return "Underway";
      if (user.Player.QuestGUI.CheckQuest((uint) dbquest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
        return "Complete";
      return user.Player.Reborn >= (byte) 0 ? "Acceptable" : "";
    }

    public static int NextAsync(int minValue, int maxValue)
    {
      return Program.GetRandom.Next(minValue, maxValue);
    }

    public static bool IsBetweenTwoPoints(
      ushort x,
      ushort y,
      ushort startx,
      ushort starty,
      ushort endx,
      ushort endy)
    {
      return (int) x >= (int) startx && (int) y >= (int) starty && (int) x <= (int) endx && (int) y <= (int) endy;
    }
  }
}
