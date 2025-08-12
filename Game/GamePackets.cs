using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Game
{
    public class GamePackets
    {
        public const ushort
       MsgAuthCallBack = 5700,
MsgAuthGame = 5701,
MsgLoginGameCallBack = 5702,
MsgLoginGame = 5703,
MsgPlayerInfo = 5704,
MsgCloseGame = 5705,
MsgBanGame = 5706,
MsgDiscordInfo = 5707;

        public const ushort LoginInfo = 35112;

        public const ushort MsgProtect = 41110;

        public const ushort LOGIN_AUTH_REQUEST = 11;

        public const ushort LOGIN_AUTH_CONFIRM = 12;

        public const ushort LOGIN_COMPLETE_AUTHENTICATION = 13;

        public const ushort LOGIN_REQUEST_ONLINE_NUMBER = 20;

        public const ushort LOGIN_REQUEST_USER_ONLINE = 21;

        public const ushort LOGIN_REQUEST_KICKOUT = 22;

        public const ushort LOGIN_REQUEST_DISCONNECTION = 23;

        public const ushort LOGIN_REQUEST_SERVER_INFO = 24;

        public const ushort LOGIN_REQUEST_SERVER_STATE = 25;

        public const ushort LOGIN_REQUEST_USER_SIGNIN = 26;

        public const ushort LOGIN_RECEIVE_INFORMATION = 50;

        public const ushort CheatPacket = 2500;

        public const ushort Update = 10017;

        public const ushort SpawnPlayer = 10014;

        public const ushort DataMap = 10010;

        public const ushort Movement = 10005;

        public const ushort MsgMagicColdTime = 1153;

        public const ushort Weather = 1016;

        public const ushort MsgTick = 1012;

        public const ushort CMsgPCServerConfig = 1049;

        public const ushort BuyFromExchangeShop = 2443;

        public const ushort ExchangeShop = 2441;

        public const ushort ArenaCrossServer = 2507;

        public const ushort MsgFamilyOccupy = 1313;

        public const ushort MsgSignIn = 3200;

        public const ushort MsgCoatStorage = 3300;

        public const ushort MsgTitleStorage = 3301;

        public const ushort CMsgMagicEffectTime = 1152;

        public const ushort MsgEquipRefineRank = 3256;

        public const ushort MsgOsShop = 3002;

        public const ushort MsgActivityTasks = 2820;

        public const ushort MsgActivityRewardsInfo = 2823;

        public const ushort MsgActivityClaims = 2822;

        public const ushort MsgPetAttack = 2812;

        public const ushort MsgTaskReward = 2811;

        public const ushort MsgSameGroupServerList = 2500;

        public const ushort MsgGoldLeaguePoint = 2600;

        public const ushort ChiInfo = 2534;

        public const ushort ChiMessage = 2535;

        public const ushort HandleChi = 2533;

        public const ushort MsgInterServerIdentifier = 2501;

        public const ushort CountryFlag = 2430;

        public const ushort MsgUsrLogin = 2078;

        public const ushort FlagIcon = 2410;

        public const ushort SubClass = 2320;

        public const ushort SecondaryPassword = 2261;

        public const ushort SkillEliteSetTeamName = 2260;

        public const ushort SkillElitePkTop = 2253;

        public const ushort SkillElitePkBrackets = 2252;

        public const ushort SkillElitePKMatchStats = 2251;

        public const ushort SkillElitePKMatchUI = 2250;

        public const ushort MsgTeamArenaInfoPlayers = 2247;

        public const ushort MsgTeamArenaMatchScore = 2246;

        public const ushort MsgTeamArenaInfo = 2245;

        public const ushort MsgTeamArenaRank10 = 2244;

        public const ushort MsgTeamArenaRanking = 2243;

        public const ushort MsgTeamArenaMatches = 2242;

        public const ushort MsgTeamArenaSignup = 2241;

        public const ushort TeamEliteSetTeamName = 2240;

        public const ushort TeamElitePkTop = 2233;

        public const ushort TeamElitePkBrackets = 2232;

        public const ushort TeamElitePKMatchStats = 2231;

        public const ushort TeamElitePKMatchUI = 2230;

        public const ushort ReceiveRecruit = 2227;

        public const ushort Advertise = 2226;

        public const ushort AdvertiseGui = 2225;

        public const ushort Recruit = 2225;

        public const ushort MsgCaptureTheFlagUpdate = 2224;

        public const ushort EliteRanks = 2223;

        public const ushort PkExploit = 2220;

        public const ushort MsgElitePKMatchStats = 2222;

        public const ushort MsgElitePk = 2219;

        public const ushort ElitePKMatchUI = 2218;

        public const ushort MsgElitePkWatch = 2211;

        public const ushort MsgArenaWatchers = 2211;

        public const ushort MsgArenaMatchScore = 2210;

        public const ushort MsgArenaInfo = 2209;

        public const ushort MsgArenaRank10 = 2208;

        public const ushort MsgArenaRanking = 2207;

        public const ushort MsgArenaMatches = 2206;

        public const ushort MsgArenaSignup = 2205;

        public const ushort FastArsenal = 2204;

        public const ushort GetArsenal = 2203;

        public const ushort PageArsenal = 2202;

        public const ushort ArsenalInfo = 2201;

        public const ushort MsgShowHandKick = 2088;

        public const ushort PokerDrawCards = 2091;

        public const ushort PokerPlayerTurn = 2092;

        public const ushort PokerHand = 2093;

        public const ushort PokerShowAllCards = 2094;

        public const ushort PokerRoundResult = 2095;

        public const ushort PokerLeaveTable = 2096;

        public const ushort PokerPlayerInfo = 2090;

        public const ushort MsgShowHandLostInfo = 2098;

        public const ushort PokerUpdateTableLocation = 2171;

        public const ushort PokerTable = 2172;

        public const ushort MemoryAgate = 2110;

        public const ushort GuildMembers = 2102;

        public const ushort GuildRanks = 2101;

        public const ushort Blackspot = 2081;

        public const ushort NameChange = 2080;

        public const ushort ExtraItem = 2077;

        public const ushort AddExtra = 2076;

        public const ushort GameUpdate = 2075;

        public const ushort RacePotion = 2072;

        public const ushort PopupInfo = 2071;

        public const ushort Fairy = 2070;

        public const ushort MentorPrize = 2067;

        public const ushort QuizShow = 2068;

        public const ushort MentorInfomation = 2066;

        public const ushort MentorAndApprentice = 2065;

        public const ushort Nobility = 2064;

        public const ushort MsgPetInfo = 2035;

        public const ushort MsgBroadcast = 2050;

        public const ushort MsgBroadcastliest = 2051;

        public const ushort ItemLock = 2048;

        public const ushort TratePartnerInfo = 2047;

        public const ushort TradePartner = 2046;

        public const ushort LeaderSchip = 2045;

        public const ushort OfflineMode = 2044;

        public const ushort OfflineTGStats = 2043;

        public const ushort MapTraps = 2400;

        public const ushort Compose = 2036;

        public const ushort KnowPersInfo = 2033;

        public const ushort NpcServerRequest = 2032;

        public const ushort NpcServerReplay = 2031;

        public const ushort NpcSpawn = 2030;

        public const ushort MsgMachineResponse = 1352;

        public const ushort MsgMachine = 1351;

        public const ushort MsgLottery = 1314;

        public const ushort Clan = 1312;

        public const ushort FlowerPacket = 1150;

        public const ushort GenericRanking = 1151;

        public const ushort Achievement = 1136;

        public const ushort QuestData = 1135;

        public const ushort QuestList = 1134;

        public const ushort Title = 1130;

        public const ushort MsgVip = 1129;

        public const ushort MsgVipHandler = 1128;

        public const ushort Enlight = 1127;

        public const ushort MsgStaticMessage = 1126;

        public const ushort Authentificaton = 1124;

        public const ushort InterAction = 1114;

        public const ushort MapStaus = 1110;

        public const ushort SobNpcs = 1109;

        public const ushort ItemView = 1108;

        public const ushort ProcesGuild = 1107;

        public const ushort Guild = 1106;

        public const ushort SpellUse = 1105;

        public const ushort UpgradeSpellExperience = 1104;

        public const ushort Spell = 1103;

        public const ushort Warehause = 1102;

        public const ushort FloorMap = 1101;

        public const ushort RaceRecord = 1071;

        public const ushort Reincarnation = 1066;

        public const ushort ElitePKWagersList = 1065;

        public const ushort ElitePKWager = 1064;

        public const ushort CaptureTheFlagRankings = 1063;

        public const ushort GuildMinDonations = 1061;

        public const ushort GuildInfo = 1058;

        public const ushort Trade = 1056;

        public const ushort LoginGame = 1052;

        public const ushort MapLoading = 1044;

        public const ushort MsgStauts = 1040;

        public const ushort Stabilization = 1038;

        public const ushort AutoHuntingInfo = 1036;

        public const ushort DetainedItem = 1034;

        public const ushort ServerInfo = 1033;

        public const ushort EmbedSocket = 1027;

        public const ushort TeamMemberInfo = 1026;

        public const ushort Proficiency = 1025;

        public const ushort AtributeSet = 1024;

        public const ushort Team = 1023;

        public const ushort Attack = 1022;

        public const ushort KnowPersons = 1019;

        public const ushort String_ = 1015;

        public const ushort Usage = 1009;

        public const ushort Item = 1008;

        public const ushort HeroInfo = 1006;

        public const ushort Chat = 1004;

        public const ushort NewClient = 1001;
    }
}
