using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgNpc;
using Extensions;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Game;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Game.MsgMonster;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Role.Instance;
using System.IO;
using TheChosenProject.ServerCore;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;
using System.Windows.Forms;

namespace TheChosenProject.Role
{
    public static class MsgBuilder
    {
        public static Packet FairbattlePower(this Packet stream, Nobility nobility, Nobility.NobilityRank nRank)
        {
            stream.InitWriter();
            stream.Write(3);
            stream.Write(nobility.UID);
            string[] obj;
            obj = new string[7]
            {
                nobility.UID.ToString(),
                " ",
                0.ToString(),
                " ",
                null,
                null,
                null
            };
            byte b;
            b = (byte)nRank;
            obj[4] = b.ToString();
            obj[5] = " ";
            obj[6] = 0.ToString();
            string StrList;
            StrList = string.Concat(obj);
            stream.ZeroFill(20);
            stream.Write(StrList);
            stream.Finalize(2064);
            return stream;
        }
    }
    public class Player : IMapObj
    {
        public uint TCCaptainTimes = 0;
        public uint Kills = 0;
        public uint Death = 0;
        public uint ReviveC = 0;
        public byte BotType = 0;
        public ushort SkillType = 0;
        public long LimitHits = 0;
        public int KingRank = 0;
        public ushort PumpkinPoints = 0;
        public int VotePoints;
        public int VipClaimChance;
        public DateTime LeftDoorStamp, RightDoorStamp;
        public DateTime StampJump = new DateTime();
        public int StampJumpMiliSeconds = 0;
        internal DateTime JumpingStamp, PreviousJump;
        internal bool BlockMovementCo = false;
        internal DateTime BlockMovement;
        public bool IsStillBanned
        {
            get
            {
                return (this.BannedChatStamp >= DateTime.Now && this.IsBannedChat) || this.PermenantBannedChat;
            }
        }

        public DateTime BannedChatStamp;
        public bool PermenantBannedChat;
        public bool IsBannedChat;
        #region Surroundings

        public ConcurrentDictionary<uint, IMapObj> KnownObjects = new ConcurrentDictionary<uint, IMapObj>();

        public void UpdateSurroundings(ServerSockets.Packet stream, bool clear = false, bool dissconnect = false)
        {
            if (dissconnect)
            {
                //Remove me from all nearby entities when logout
                foreach (var knownObject in KnownObjects.Values)
                    if (knownObject is Player)
                        ((Player)knownObject).TryDespawn(stream, this);
            }
            else
            {
                if (clear)
                {
                    //Remove me from all nearby entities
                    foreach (var knownObject in KnownObjects.Values)
                        if (knownObject is Player)
                            ((Player)knownObject).TryDespawn(stream, this);

                    //Reset my Known Objects collection
                    KnownObjects.Clear();
                }

                var newObjects = View.Roles(Role.MapObjectType.Player).ToList();

                //Remove objects that are no longer on screen
                foreach (var testObject in KnownObjects.Values)
                    if (!newObjects.Contains(testObject))
                        TryDespawn(stream, testObject);

                //Skip objects that are already on screen
                foreach (var testObject in newObjects.ToArray())
                {
                    IMapObj x;
                    if (KnownObjects.TryGetValue(testObject.UID, out x))
                        newObjects.Remove(testObject);
                }

                //Add new objects
                foreach (var testObject in newObjects)
                    TryAdd(stream, testObject);
            }
        }

        public void TryAdd(ServerSockets.Packet stream, IMapObj obj)
        {
            var target = obj as Player;
            if (Map == 700)
            {
                if (Owner.InQualifier() && target.Owner.IsWatching())
                    return;
                if (target.Owner.IsWatching())
                    return;
            }

            if (Invisible == false && target.Invisible == true)
                return;
            if (target.Invisible)
                return;
            if (!KnownObjects.TryAdd(obj.UID, obj))
                return;
            Send(target.GetArray(stream, false));
            target.TryAdd(stream, this);
        }

        public unsafe void TryDespawn(ServerSockets.Packet stream, IMapObj t)
        {
            IMapObj x;
            if (!KnownObjects.TryRemove(t.UID, out x))
                return;
            if (t is Player)
            {
                var target = t as Player;
                Game.MsgServer.ActionQuery action = new Game.MsgServer.ActionQuery()
                {
                    ObjId = t.UID,
                    Type = Game.MsgServer.ActionType.RemoveEntity
                };
                Send(stream.ActionCreate(&action));
                target.TryDespawn(stream, this);
            }
        }

        #endregion Surroundings
        public void SendMsgBox(string Message, int Seconds, MsgStaticMessage.Messages Type)
        {
            MessageBox(Message, null, null, Seconds, Type);
        }
        public DateTime vote;

        public bool InCPRoom = false;
        public DateTime DeathHit = DateTime.Now;
        public bool GuildBeastClaimd = false;
        public bool SpawnGuildBeast = false;
        public bool skipore = false;
        public bool OnAccuracy = false;
        public bool OnDodge = false;
        public Extensions.Time32 OnAccuracyStamp = new Extensions.Time32();
        public Extensions.Time32 OnDodgeStamp = new Extensions.Time32();
        public uint HitRateAcc;
        public uint HitRateDodge;

        public DateTime LastPositionTime { get; set; } = DateTime.UtcNow;
        public ushort LastX { get; set; }
        public ushort LastY { get; set; }


        public static void Reward(Client.GameClient user, ServerSockets.Packet stream, string Name)
        {
            string mymsg = "";
        jmp:
            byte rand = (byte)Program.GetRandom.Next(0, 4);
            switch (rand)
            {
                case 0://money
                    {
                        int value = (int)Program.GetRandom.Next(10000, 200000);
                        user.Player.Money += value;
                        user.Player.SendUpdate(stream, user.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                        mymsg = "[EVENT]" + user.Player.Name + " got " + value.ToString() + " Money from " + Name + "!";
                        MsgSchedules.SendSysMesage(mymsg, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                        break;
                    }
                case 1://experience
                    {
                        if (user.Player.Level == 137)
                            goto jmp;
                        user.GainExpBall(600, true, Role.Flags.ExperienceEffect.angelwing);
                        mymsg = "[EVENT]" + user.Player.Name + " got 2x ExpBalls from " + Name + "!";
                        MsgSchedules.SendSysMesage(mymsg, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);

                        break;
                    }
                //case 2://cps
                //    {
                //        uint value = (uint)Program.GetRandom.Next(20, 50);
                //        user.Player.ConquerPoints += value;
                //        MsgSchedules.SendSysMesage(user.Player.Name + " got " + value.ToString() + " ConquerPoints from " + Name + "!", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                //        break;
                //    }
                case 2://item.
                    {
                        uint[] Items = new uint[]
                        {
                            //Database.ItemType.DragonBall,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.PowerExpBall,
                            Database.ItemType.CleanWater,
                            Database.ItemType.LotteryTick
                        };
                        uint ItemID = Items[Program.GetRandom.Next(0, Items.Length)];
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (user.Inventory.HaveSpace(1))
                                user.Inventory.Add(stream, DBItem.ID);
                            //else
                            //    user.Inventory.AddReturnedItem(stream, DBItem.ID);
                            mymsg = "[EVENT]" + user.Player.Name + " got " + DBItem.Name + " from " + Name + "!";
                            MsgSchedules.SendSysMesage(mymsg, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                        }
                        break;
                    }

                case 3:
                    {
                        uint[] Items = new uint[]
                        {
                            Database.ItemType.LotteryTick,
                            Database.ItemType.CleanWater,
                            Database.ItemType.MeteorScroll,
                            Database.ItemType.ExperiencePotion,
                           Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                            Database.ItemType.Meteor,
                        };
                        uint ItemID = Items[Program.GetRandom.Next(0, Items.Length)];
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            if (user.Inventory.HaveSpace(1))
                                user.Inventory.Add(stream, DBItem.ID);
                            //else
                            //    user.Inventory.AddReturnedItem(stream, DBItem.ID);

                            mymsg = "[EVENT]" + user.Player.Name + " got " + DBItem.Name + " from " + Name + "!";
                            MsgSchedules.SendSysMesage(mymsg, Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                        }
                        break;
                    }
            }
            Database.ServerDatabase.LoginQueue.Enqueue(mymsg);

        }
        public Extensions.Time32 StampArenaScore = new Extensions.Time32();
        public Extensions.Time32 StampGuildWarScore = new Extensions.Time32();
        public Extensions.Time32 StampKick = new Extensions.Time32();
        public Extensions.Time32 StampDropStats = new Extensions.Time32();
        #region Temp bank
        public uint NPhoenixGem;
        public uint RPhoenixGem;
        public uint SPhoenixGem;

        public uint NDragonGem;
        public uint RDragonGem;
        public uint SDragonGem;

        public uint NFuryGem;
        public uint RFuryGem;
        public uint SFuryGem;

        public uint NRainbowGem;
        public uint RRainbowGem;
        public uint SRainbowGem;

        public uint NKylinGem;
        public uint RKylinGem;
        public uint SKylinGem;

        public uint NVioletGem;
        public uint RVioletGem;
        public uint SVioletGem;

        public uint NMoonGem;
        public uint RMoonGem;
        public uint SMoonGem;

        public uint NTortoiseGem;
        public uint RTortoiseGem;
        public uint STortoiseGem;

        public uint MetScrolls;
        public uint DBscrolls;
        #endregion
        #region Temp VIPbank
        public uint VIPNPhoenixGem;
        public uint VIPRPhoenixGem;
        public uint VIPSPhoenixGem;

        public uint VIPNDragonGem;
        public uint VIPRDragonGem;
        public uint VIPSDragonGem;

        public uint VIPNFuryGem;
        public uint VIPRFuryGem;
        public uint VIPSFuryGem;

        public uint VIPNRainbowGem;
        public uint VIPRRainbowGem;
        public uint VIPSRainbowGem;

        public uint VIPNKylinGem;
        public uint VIPRKylinGem;
        public uint VIPSKylinGem;

        public uint VIPNVioletGem;
        public uint VIPRVioletGem;
        public uint VIPSVioletGem;

        public uint VIPNMoonGem;
        public uint VIPRMoonGem;
        public uint VIPSMoonGem;

        public uint VIPNTortoiseGem;
        public uint VIPRTortoiseGem;
        public uint VIPSTortoiseGem;

        public uint VIPMetScrolls;
        public uint VIPDBscrolls;
        public uint VIPStone;
        #endregion
        public uint HitShoot = 0;
        public uint MisShoot = 0;
        public uint ArenaDeads = 0;
        public uint ArenaKills = 0;
        //internal Extensions.Time32 JumpingStamp, PreviousJump;
        //internal Extensions.Time32 WalkingStamp, PreviousWalk;
        public DateTime LastMove;
        public DateTime LastSuccessCaptcha = DateTime.Now;
        public int NextCaptcha = 5;
        public Extensions.Time32 KillCountCaptchaStamp;
        //internal DateTime BlockMovement;
        public bool WaitingKillCaptcha;
        public string KillCountCaptcha;
        public uint ReceiveTest = 0;
        public DateTime ReceivePing = DateTime.Now;
        public DateTime LastSuspect = DateTime.Now;
        public bool RedTeam = false;
        public bool BlueTeam = false;
        public uint LastMan = 0;
        public uint PTB = 0;
        public uint DragonWar = 0;
        public uint Get5Out = 0;
        public uint SSFB = 0;

        //
        public uint FreezeWar = 20;

        public uint Infection = 0;
        public uint TheCaptain = 0;
        public uint Kungfu = 0;
        public uint VampireWar = 0;
        public uint WhackTheThief = 0;
        public uint TempPassword;
        public string TempNewPassword;

        public byte ConquerLetter;
        public byte LavaQuest;

        public bool IsAsasin;

        public bool OnRemoveLukyAmulet;

        public DateTime LastDragonPill;
        public DateTime LastSwordSoul;

        public MsgOfflineTraining.Mode OfflineTraining;

        public uint TestPacket;

        public int GiveFlowersToPerformer;

        public ushort ExtraAtributes;
        public Extensions.Time32 NextMine;

        private DateTime _AIBotExpire;

        private Regions mRegion;

        public ushort SetLocationType;

        public DateTime KingOfTheHillStamp;

        public uint KingOfTheHillScore;

        public uint SkillTournamentLifes;

        public const ushort MaxInventorySashCount = 1000;

        public ushort InventorySashCount;

        public bool TabEnemyInvisible = false;

        public bool TabAllyInvisible = false;

        public bool Invisible = false;

        public DateTime StampBloodyScytle;

        public DateTime MedicineStamp;

        public uint CountSpeedHack;

        public ushort CountryID;

        public ushort NameEditCount;

        public uint CurrentTreasureBoxes;

        public bool StartVote;

        public Time32 StartVoteStamp;

        public DateTime AzurePillStamp;

        public bool OnAttackPotion;

        public Time32 OnAttackPotionStamp;

        public bool OnDefensePotion;

        public Time32 OnDefensePotionStamp;

        public Time32 DelayedTaskStamp = Time32.Now;

        public bool DelayedTask;

        public MsgAutoHunt.TaskButton DelayedTaskOption;

        public byte Away;

        public Time32 LastAttack;

        public uint AparenceType;

        public bool VerifiedPassword;

        public uint SecurityPassword;

        public uint OnReset;

        public DateTime ResetSecurityPassowrd;

        public bool InElitePk;

        public bool InTeamPk;

        public int ChampionPoints;

        public Dictionary<MsgTitle.TitleType, DateTime> SpecialTitles = new Dictionary<MsgTitle.TitleType, DateTime>();

        public uint SpecialTitleID;

        public List<byte> Titles = new List<byte>();

        public byte MyTitle;

        public uint SpecialGarment;

        public uint RightSpecialAccessory;

        public uint LeftSpecialAccessory;

        public Flags.PKMode PreviousPkMode = Flags.PKMode.Capture;

        public DateTime EnlightenTime;

        public int CursedTimer;

        public bool Delete = false;

        public ushort RandomSpell;

        public byte LotteryEntries;
        public uint MiningAttempts = 140;
        public uint KeyBoxTRY = 3;
        public uint lettersTRY = 3;
        public uint LavaTRY = 3;

        public LotteryTable.LotteryItem LotteryItem;

        public bool Reincarnation;

        private Clan.Member clanmemb;

        public Clan MyClan;

        public uint ClanUID;

        public ushort ClanRank;

        public Guild.Member MyGuildMember;

        public Guild MyGuild;

        public uint TargetGuild;

        private uint _extbattle;

        public Flags.GuildMemberRank GuildRank;

        public uint GuildID;

        private uint guildBP;

        private uint _clanbp;

        private uint _mentorBp;

        public uint targetTrade;

        public Associate.MyAsociats MyMentor;

        public Associate.MyAsociats Associate;

        public uint TradePartner;

        public uint TargetFriend;

        public Nobility Nobility;

        private Nobility.NobilityRank _NobilityRank;

        public Flowers Flowers;

        public uint FlowerRank;

        public bool OnFairy;
        public bool NotifTogggle = true;
        public MsgTransformFairy FairySpawn;

        public ushort ActiveDance;

        public GameClient ObjInteraction;

        public InteractQuery InteractionEffect;

        public bool OnInteractionEffect;

        public SubClass SubClass;

        public uint SubClassHasPoints;

        public DBLevExp.Sort ActiveSublass;

        public Random MyRandom = new Random(Program.GetRandom.Next());

        public bool BlackSpot;

        public Time32 Stamp_BlackSpot;

        public byte UseStamina;

        public Time32 Protect;

        private Time32 ProtectedJumpAttack;

        public uint ShieldBlockDamage;

        public MsgUpdate.Flags UseAura = MsgUpdate.Flags.Normal;

        public MagicType.Magic Aura;

        public uint SpouseUID;

        public Extensions.Time32 AttackStamp = Time32.Now;

        public bool ActivePassive = true;

        public Time32 SpellAttackStamp;

        public ClientTransform TransformInfo;

        public double PoisonLevel;

        public byte PoisonLevehHu;

        public bool ActivateCounterKill;

        public Action<GameClient> MessageOK;

        public Action<GameClient> MessageCancel;

        public Time32 StartMessageBox;

        public bool InUseIntensify;

        public Time32 IntensifyStamp;

        public bool Intensify;

        public int IntensifyDamage;

        private ushort azuredef;

        public byte AzureShieldLevel;

        public Time32 XPListStamp;

        public StatusFlagsBigVector32 BitVector;

        public uint StatusFlag;

        public ushort Dead_X;

        public ushort Dead_Y;

        public bool GetPkPkPoints;

        public bool CompleteLogin;

        public DateTime GhostStamp;

        public Time32 PkPointsStamp;

        public uint BlessTime;

        public Time32 CastPrayStamp;

        public Time32 CastPrayActionsStamp;

        public MsgUpdate.Flags UseXPSpell;

        private uint _KO;

        private ushort _xpc;

        public DateTime DeathStamp;

        public Time32 DeadStamp;

        public ushort Avatar;

        public long WHMoney;

        public Time32 LastWorldMessaj;

        public Flags.PKMode PkMode = Flags.PKMode.Capture;

        public GameClient Owner;

        public RoleView View;

        public Quests QuestGUI;

        public int Day;

        public string Name = "";

        public string ClanName = "";

        private string _spouse = "None";

        public ushort Agility;

        public ushort Vitality;

        public ushort Spirit;

        public ushort Strength;

        public ushort Atributes;

        private byte _class;

        public int BossPoints;

        public byte FirstRebornLevel;

        public byte SecoundeRebornLevel;

        public byte FirstClass;

        public byte SecondClass;

        private ushort _level;

        public string KillerHunting = "None";

        public Time32 JumpHunterStamp;

        public Time32 AttackHunterStamp;

        public int AutoHuntingCPS;

        public bool IsHunting;

        private AutoStructures.Mode _auto_hunting;

        private Flags.FairbattlePower _fairbattlePower;

        //private Flags.NewbieExperience _newbieprotection;

        private byte _reborn;

        private int _Money;

        private int _cps;

        private int _onlinepoints;

        private int _TournamentsPoints;
        private int _RoyalPassPoints;
        public uint CountVote = 0;

        private int _emoneypoints;

        public ulong Experience;

        public uint VirtutePoints;

        private int _minhitpoints;

        private ushort _mana;

        private ushort _pkpoints;

        private uint _quizpoints;
        public byte Quest2rbStage = 0;
        public uint Quest2rbS2Point = 0;
        public byte Quest2rbBossesOrderby = 0;
        public byte OldVIPLevel;

        public ushort Enilghten;

        private DateTime _ExpireVip;

        private DateTime _VendorTime;

        private byte _viplevel;

        public ushort EnlightenReceive;

        private ushort face;

        public Time32 UpdateBossocationStamp;

        public Time32 UpdateNotficationLavaStamp;

        public int TotalHits;

        public int Hits;

        public int Chains;

        public int MaxChains;

        public ushort Hair;

        public bool DarkMode;

        private uint _mmmap;

        private ushort xx;

        private ushort yy;

        public ushort dummyX;

        public ushort dummyY;

        public ushort dummyX2;

        public ushort dummyY2;

        public byte dummies;

        public ushort Px;

        public ushort Py;

        public ushort PMapX;

        public ushort PMapY;

        public uint PMap;

        public Flags.ConquerAngle Angle = Flags.ConquerAngle.East;

        public Flags.ConquerAction Action;

        public byte ExpBallUsed;

        public byte BDExp;

        public DateTime JoinOnflineTG;

        public Time32 OnlineTrainingTime;

        public Time32 ReceivePointsOnlineTraining;

        public Time32 HeavenBlessTime;

        public int HeavenBlessing;

        public uint OnlineTrainingPoints;

        public uint HuntingBlessing;

        public DateTime LastLoginClient;

        public uint DExpTime;

        public uint RateExp = 1u;

        public uint MeteorSocket = 0u;

        public uint DragonBallSocket = 0u;

        private ushort body;

        private ushort _transformationid;

        public uint ArenaRankingCP;

        public bool ShowGemEffects = true;

        public uint HeadId;

        public uint GarmentId;

        public uint ArmorId;

        public uint LeftWeaponId;

        public uint RightWeaponId;

        public uint LeftWeaponAccessoryId;

        public uint RightWeaponAccessoryId;

        public uint SteedId;

        public uint MountArmorId;

        public ushort ColorArmor;

        public ushort ColorShield;

        public ushort ColorHelment;

        public uint SteedPlus;

        public uint SteedColor;

        public uint HeadSoul;

        public uint ArmorSoul;

        public uint LeftWeapsonSoul;

        public uint RightWeapsonSoul;

        public uint RealUID;

        public uint InitTransfer;

        public ushort ServerID;

        public string NewUser = "";

        public bool LootExpBall;

        public bool LootDragonBall;
        public bool Storage;

        public bool LootLetters;

        public bool LootMeteor;

        public bool StonesLoot;

        public bool SuperLoot;

        public bool EliteLoot;

        public bool GemsLoot;

        public bool ItemPlusLoot;
        public bool ProfTokenLoot;

        private ushort _stamina;

        public int KillTheCaptain;

        public bool leveldown = true;

        public DateTime Last_LevelStamp = DateTime.Now;

        internal bool SendAllies = true;

        internal int OblivionMobs;

        internal bool WhirlWind;

        internal bool DeadState = true;

        internal DateTime ShieldBlockEnd = DateTime.Now;

        //internal int FiveNOut;

        public uint JailerUID;

        internal DateTime KickedOffSteed = DateTime.Now;

        internal DateTime RemovedShackle;

        public bool InLine { get; set; }
        public void AddTempEquipment(ServerSockets.Packet stream, uint ID, Flags.ConquerItem pos)
        {
            Game.MsgServer.MsgGameItem item = new MsgGameItem();
            item.ITEM_ID = ID;
            item.UID = Server.ITEM_Counter.Next;
            item.Color = Flags.Color.Red;
            item.Position = (ushort)pos;
            item.Durability = Server.ItemsBase[ID].Durability;
            item.Fake = true;
            if (pos == Flags.ConquerItem.LeftWeapon)
                Owner.Equipment.TempLeftWeapon = item;
            if (pos == Flags.ConquerItem.RightWeapon)
                Owner.Equipment.TempRightWeapon = item;
            if (pos == Flags.ConquerItem.Garment)
                Owner.Equipment.TempGarment = item;
            Owner.Equipment.Add(item, stream);
            Owner.Equipment.AppendItems(true, Owner.Equipment.CurentEquip, stream);
            View.SendView(GetArray(stream, false), false);
        }

        public void RemoveTempEquipment(ServerSockets.Packet stream)
        {
            MsgGameItem item;
            if (Owner.Equipment.TempGarment != null)
                if (Owner.Equipment.ClientItems.TryRemove(Owner.Equipment.TempGarment.UID, out item))
                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));
            if (Owner.Equipment.TempLeftWeapon != null)
                if (Owner.Equipment.ClientItems.TryRemove(Owner.Equipment.TempLeftWeapon.UID, out item))
                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, (ushort)Flags.ConquerItem.LeftWeapon, 0, 0, 0, 0));
            if (Owner.Equipment.TempRightWeapon != null)
                if (Owner.Equipment.ClientItems.TryRemove(Owner.Equipment.TempRightWeapon.UID, out item))
                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, (ushort)Flags.ConquerItem.RightWeapon, 0, 0, 0, 0));
            Owner.Equipment.TempLeftWeapon = null;
            Owner.Equipment.TempRightWeapon = null;
            Owner.Equipment.TempGarment = null;
            var items = Owner.Equipment.ClientItems.Values.Where(i => i.Fake).ToList();
            for (int i = 0; i < items.Count; i++)
                Owner.Equipment.ClientItems.TryRemove(items[i].UID, out item);
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeapon, out item))
            {
                LeftWeaponId = item.ITEM_ID;
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, (ushort)Flags.ConquerItem.LeftWeapon, 0, 0, 0, 0));
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.RightWeapon, out item))
            {
                RightWeaponId = item.ITEM_ID;
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, (ushort)Flags.ConquerItem.RightWeapon, 0, 0, 0, 0));
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out item))
            {
                RightWeaponId = item.ITEM_ID;
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            View.SendView(GetArray(stream, false), false);
            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
        }
        public DateTime AIBotExpire
        {
            get
            {
                return _AIBotExpire;
            }
            set
            {
                _AIBotExpire = value;
            }
        }

        public Regions MapRegion
        {
            get
            {
                return mRegion;
            }
            set
            {
                if (mRegion == value)
                    return;
                string str;
                str = ((mRegion != null) ? mRegion.Name : string.Empty);
                mRegion = value;
                if (MapRegion != null && MapRegion.Name != str)
                {
                    if (MapRegion.Name != string.Empty)
                        Owner.SendSysMesage("You entered the region " + MapRegion.Name + " (Lineage: " + MapRegion.Lineage + ").", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.purple);
                    else
                        Owner.SendSysMesage("You left the region " + str, MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.purple);
                }
            }
        }

        public Position Position => new Position((int)Map, X, Y);

        public bool AllowDynamic { get; set; }

        public uint IndexInScreen { get; set; }

        public Clan.Member MyClanMember
        {
            get
            {
                if (clanmemb == null)
                    MyClan?.Members.TryGetValue(UID, out clanmemb);
                return clanmemb;
            }
            set
            {
                clanmemb = value;
            }
        }

        public uint ExtraBattlePower
        {
            get
            {
                return _extbattle;
            }
            set
            {
                _extbattle = value;
            }
        }

        public uint GuildBattlePower
        {
            get
            {
                return guildBP;
            }
            set
            {
                ExtraBattlePower -= guildBP;
                guildBP = value;
                ExtraBattlePower += guildBP;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    SendUpdate(stream, guildBP, MsgUpdate.DataType.GuildBattlePower);
                }
            }
        }

        public uint ClanBp
        {
            get
            {
                return _clanbp;
            }
            set
            {
                ExtraBattlePower -= _clanbp;
                _clanbp = value;
                ExtraBattlePower += _clanbp;
            }
        }

        public uint MentorBp
        {
            get
            {
                return _mentorBp;
            }
            set
            {
                ExtraBattlePower -= _mentorBp;
                ExtraBattlePower += value;
                _mentorBp = value;
            }
        }

        public Nobility.NobilityRank NobilityRank
        {
            get
            {
                return _NobilityRank;
            }
            set
            {
                _NobilityRank = value;
                if (MyGuild != null && MyGuildMember != null)
                    MyGuildMember.NobilityRank = (uint)_NobilityRank;
            }
        }

        public bool ContainReflect
        {
            get
            {
                if (MsgSchedules.CurrentTournament.Type == TournamentType.SkillTournament && MsgSchedules.CurrentTournament.Process != ProcesType.Dead && MsgSchedules.CurrentTournament.InTournament(Owner))
                    return false;
                return AtributesStatus.IsWarrior(SecondClass);
            }
        }

        public bool OnTransform => TransformationID != 0;

        public int BattlePower
        {
            get
            {
                int val;
                val = (int)(Level + Reborn * 5 + Owner.Equipment.BattlePower);//+ (int)NobilityRank + ExtraBattlePower
                if (val > MaxBP())
                    return MaxBP();
                return Math.Min(385, val);
            }
        }

        public int RealBattlePower
        {
            get
            {
                int val;
                val = Level + Reborn * 5 + Owner.Equipment.BattlePower;//+ (int)NobilityRank
                if (val > MaxBP())
                    return MaxBP();
                return val;
            }
        }

        public ushort AzureShieldDefence
        {
            get
            {
                return azuredef;
            }
            set
            {
                azuredef = value;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    SendUpdate(stream, MsgUpdate.Flags.AzureShield, 60u, value, AzureShieldLevel, MsgUpdate.DataType.AzureShield, true);
                }
            }
        }

        public ushort Stamina
        {
            get
            {
                if (UnlimitedArenaRooms.Maps.ContainsValue(DynamicID))
                    return 150;
                return _stamina;
            }
            set
            {
                _stamina = value;
            }
        }

        public uint KillCounter
        {
            get
            {
                return _KO;
            }
            set
            {
                _KO = value;
            }
        }

        public ushort XPCount
        {
            get
            {
                return _xpc;
            }
            set
            {
                _xpc = value;
            }
        }

        public MapObjectType ObjType { get; set; }

        public uint UID { get; set; }

        public string Spouse
        {
            get
            {
                return _spouse;
            }
            set
            {
                _spouse = value;
            }
        }

        public byte Class
        {
            get
            {
                return _class;
            }
            set
            {
                _class = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        SendUpdate(stream, value, MsgUpdate.DataType.Class);
                    }
                    if (MyGuildMember != null)
                        MyGuildMember.Class = value;
                }
            }
        }

        public ushort Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                if (_level >= ServerKernel.MAX_UPLEVEL && !Owner.ProjectManager)
                {
                    _level = ServerKernel.MAX_UPLEVEL;
                    Experience = 0uL;
                }
            }
        }

        public AutoStructures.Mode AutoHunting
        {
            get
            {
                return _auto_hunting;
            }
            set
            {
                _auto_hunting = value;
                if (!Owner.FullLoading)
                    return;
                switch (AutoHunting)
                {
                    case AutoStructures.Mode.NotActive:
                        AutoHuntingCPS = 0;
                        break;
                    case AutoStructures.Mode.Enable:
                        if (AutoStructures.Validated(Owner))
                        {
                            SetPkMode(Flags.PKMode.Capture);
                            AddFlag(MsgUpdate.Flags.AutoHunting, 2592000, false);
                            Owner.SendSysMesage("Auto-Hunting~has~been~enabled,~The~CPs~gained~by~auto-hunting~won`t~be~counted~when~ended.", MsgMessage.ChatMode.TopLeft);
                            Owner.AIStatus = AIEnum.AIStatus.Idle;
                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;
                                stream = rec.GetStream();
                                SendUpdate(stream, 1L, MsgUpdate.DataType.OnlineTraining);
                                SendString(stream, MsgStringPacket.StringID.Effect, true, "ride_screen");
                            }
                            IsHunting = true;
                        }
                        else
                            AutoHunting = AutoStructures.Mode.NotActive;
                        break;
                    case AutoStructures.Mode.Disable:
                        if (ContainFlag(MsgUpdate.Flags.AutoHunting))
                        {
                            RemoveFlag(MsgUpdate.Flags.AutoHunting);
                            Owner.SendSysMesage("Auto-Hunting~has~been~canceled", MsgMessage.ChatMode.TopLeft);
                            IsHunting = false;
                        }
                        AutoHunting = AutoStructures.Mode.Recived;
                        break;
                    case AutoStructures.Mode.Recived:
                        {
                            //if (AutoHuntingCPS > 0)
                            //{
                            //    CurrentPoint(Flags.CurrentPoint.GiftCPS, Flags.CurrentPointAction.Add, (int)AutoHuntingCPS);
                            //    Owner.CreateBoxDialog(string.Format("You`ve gained {0} CPS since you started auto-hunting.", AutoHuntingCPS.ToString("0,0")));
                            //}
                            using (RecycledPacket recycledPacket = new RecycledPacket())
                            {
                                Packet stream2;
                                stream2 = recycledPacket.GetStream();
                                SendUpdate(stream2, 4L, MsgUpdate.DataType.OnlineTraining);
                            }
                            Owner.MobTarget = null;
                            Owner.pathfinder = null;
                            Owner.OnAutoAttack = false;
                            AutoHunting = AutoStructures.Mode.NotActive;
                            break;
                        }
                }
            }
        }

        public Flags.FairbattlePower FairbattlePower
        {
            get
            {
                return _fairbattlePower;
            }
            set
            {
                _fairbattlePower = value;
                if (!Owner.FullLoading)
                    return;
                switch (FairbattlePower)
                {
                    case Flags.FairbattlePower.NotActive:
                        {
                            NobilityRank = Nobility.Rank;
                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;
                                stream = rec.GetStream();
                                Owner.Send(stream.NobilityIconCreate(Nobility));
                                break;
                            }
                        }
                    case Flags.FairbattlePower.UpdateToSerf:
                        {
                            NobilityRank = Nobility.NobilityRank.Serf;
                            using (RecycledPacket recycledPacket2 = new RecycledPacket())
                            {
                                Packet stream2;
                                stream2 = recycledPacket2.GetStream();
                                Owner.Send(stream2.FairbattlePower(Nobility, NobilityRank));
                                break;
                            }
                        }
                    case Flags.FairbattlePower.UpdateToKing:
                        {
                            NobilityRank = Nobility.NobilityRank.King;
                            using (RecycledPacket recycledPacket = new RecycledPacket())
                            {
                                Packet stream3;
                                stream3 = recycledPacket.GetStream();
                                Owner.Send(stream3.FairbattlePower(Nobility, NobilityRank));
                                break;
                            }
                        }
                }
            }
        }

        //public Flags.NewbieExperience NewbieProtection
        //{
        //    get
        //    {
        //        return _newbieprotection;
        //    }
        //    set
        //    {
        //        _newbieprotection = value;
        //        if (!Owner.FullLoading)
        //            return;
        //        switch (NewbieProtection)
        //        {
        //            case Flags.NewbieExperience.NotActive:
        //                NewbieProtection = Flags.NewbieExperience.Enable;
        //                break;
        //            case Flags.NewbieExperience.Disable:
        //                if (ContainFlag(MsgUpdate.Flags.GodlyShield))
        //                    RemoveFlag(MsgUpdate.Flags.GodlyShield);
        //                Owner.SendSysMesage("Newbies~Protection~has~been~disabled,~You~are~now~not~protected.", MsgMessage.ChatMode.TopLeft);
        //                break;
        //            case Flags.NewbieExperience.Enable:
        //                if (Reborn < 2)
        //                {
        //                    AddFlag(MsgUpdate.Flags.GodlyShield, 2592000, false);
        //                    Owner.SendSysMesage("Newbies~Protection~has~been~enabled,~You~are~now~protected.", MsgMessage.ChatMode.TopLeft);
        //                }
        //                else
        //                    NewbieProtection = Flags.NewbieExperience.Remove;
        //                break;
        //            case Flags.NewbieExperience.Remove:
        //                if (ContainFlag(MsgUpdate.Flags.GodlyShield))
        //                    RemoveFlag(MsgUpdate.Flags.GodlyShield);
        //                Owner.SendSysMesage("You~are~no~longer~able~to~use~this~feature,~This~feature~is~only~avaible~for~newbies~(Not~2nd-rebirth)", MsgMessage.ChatMode.TopLeft);
        //                break;
        //        }
        //    }
        //}

        public byte Reborn
        {
            get
            {
                return _reborn;
            }
            set
            {
                _reborn = value;
                //if (Owner.FullLoading && _reborn >= 2)
                //    NewbieProtection = Flags.NewbieExperience.Remove;
            }
        }

        public int Money
        {
            get
            {
                return _Money;
            }
            set
            {
                _Money = value;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    SendUpdate(stream, value, MsgUpdate.DataType.Money);
                }
            }
        }

        public int ConquerPoints
        {
            get
            {
                return _cps;
            }
            set
            {
                //_cps = value;
                if (value > 1094967295)
                {
                    //banned
                    string logs = "[CallStack2]" + Name + " get " + value + " he have " + _cps + "";
                    logs += Environment.StackTrace;
                    Database.ServerDatabase.LoginQueue.Enqueue(logs);
                    //Database.SystemBannedAccount.AddBan((UID, Name, 999999, SystemBannedAccount._Type.UsingCheat));
                    SystemBannedAccount.AddBan(UID, Name, 999999, SystemBannedAccount._Type.UsingCheat);
                    Owner.SendSysMesage("You Account was Banned by [PM]/[GM].");
                    Owner.Socket.Disconnect();
                }
                if (Owner.FullLoading)
                {
                    if (value > _cps)
                    {
                        int get_cps = value - _cps;
                        if (get_cps > 59)
                        {
                            string logs = "[CallStack]" + Name + " get " + get_cps + " he have " + _cps + "";
                            //  logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                    }
                    else
                    {
                        int lost_cps = _cps - value;
                        if (lost_cps > 59)
                        {
                            string logs = "[CallStack]" + Name + " lost " + lost_cps + " he have " + _cps + "";
                            // logs += Environment.StackTrace;
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                    }
                }
                _cps = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        MsgUpdate packet;
                        packet = new MsgUpdate(stream, UID);
                        stream = packet.Append(stream, MsgUpdate.DataType.ConquerPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        public int TournamentsPoints
        {
            get
            {
                return _TournamentsPoints;
            }
            set
            {
                _TournamentsPoints = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        MsgUpdate packet;
                        packet = new MsgUpdate(stream, UID);
                        stream = packet.Append(stream, MsgUpdate.DataType.TournamentsPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        public int RoyalPassPoints
        {
            get
            {
                return _RoyalPassPoints;
            }
            set
            {
                _RoyalPassPoints = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        MsgUpdate packet;
                        packet = new MsgUpdate(stream, UID);
                        stream = packet.Append(stream, MsgUpdate.DataType.RoyalPassPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        public int OnlinePoints
        {
            get
            {
                return _onlinepoints;
            }
            set
            {
                _onlinepoints = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        MsgUpdate packet;
                        packet = new MsgUpdate(stream, UID);
                        stream = packet.Append(stream, MsgUpdate.DataType.OnlinePoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }

        public unsafe int EmoneyPoints
        {
            get
            {
                return _emoneypoints;
            }
            set
            {
                if (Owner.FullLoading && value > 0)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.dwParam = 1197u;
                        actionQuery.Type = ActionType.OpenCustom;
                        actionQuery.ObjId = UID;
                        ActionQuery Emoney;
                        Emoney = actionQuery;
                        Owner.Send(stream.ActionCreate(&Emoney));
                    }
                }
                _emoneypoints = value;
            }
        }

        public int HitPoints
        {
            get
            {
                if (UnlimitedArenaRooms.Maps.ContainsValue(DynamicID))
                    return (int)Owner.Status.MaxHitpoints;
                return _minhitpoints;
            }
            set
            {
                if (value > 0)
                    DeadState = false;
                else
                    DeadState = true;
                _minhitpoints = value;
                if (Owner.Team != null)
                {
                    Team.MemberInfo TeamMember;
                    TeamMember = Owner.Team.GetMember(UID);
                    if (TeamMember != null)
                    {
                        TeamMember.Info.MaxHitpoints = (ushort)Owner.Status.MaxHitpoints;
                        TeamMember.Info.MinMHitpoints = (ushort)value;
                        Owner.Team.SendTeamInfo(TeamMember);
                    }
                }
                if (Owner.FullLoading)
                    SendUpdateHP();
            }
        }

        public ushort Mana
        {
            get
            {
                return _mana;
            }
            set
            {
                _mana = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        SendUpdate(stream, value, MsgUpdate.DataType.Mana);
                    }
                }
            }
        }

        public ushort PKPoints
        {
            get
            {
                return _pkpoints;
            }
            set
            {
                _pkpoints = value;
                if (PKPoints > 100)
                {
                    Owner.SendSysMesage("You have done too much killing. You will pay for what you have done.");
                    RemoveFlag(MsgUpdate.Flags.RedName);
                    AddFlag(MsgUpdate.Flags.BlackName, 2592000, false, 360);
                }
                else if (PKPoints > 29)
                {
                    Owner.SendSysMesage("You have done too much killing. You will pay for what you have done.");
                    AddFlag(MsgUpdate.Flags.RedName, 2592000, false, 360);
                    RemoveFlag(MsgUpdate.Flags.BlackName);
                }
                else if (PKPoints < 30)
                {
                    RemoveFlag(MsgUpdate.Flags.RedName);
                    RemoveFlag(MsgUpdate.Flags.BlackName);
                }
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    SendUpdate(stream, PKPoints, MsgUpdate.DataType.PKPoints);
                }
            }
        }

        public uint QuizPoints
        {
            get
            {
                return _quizpoints;
            }
            set
            {
                _quizpoints = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        SendUpdate(stream, QuizPoints, MsgUpdate.DataType.QuizPoints);
                    }
                }
            }
        }

        public DateTime VendorTime
        {
            get
            {
                return _VendorTime;
            }
            set
            {
                _VendorTime = value;
            }
        }

        public DateTime ExpireVip
        {
            get
            {
                return _ExpireVip;
            }
            set
            {
                _ExpireVip = value;
            }
        }

        public byte VipLevel
        {
            get
            {
                return _viplevel;
            }
            set
            {
                _viplevel = value;
                if (!Owner.FullLoading)
                    return;
                switch (VipLevel)
                {
                    case 5:
                        {
                            ////if (ContainFlag(MsgUpdate.Flags.VIPSpecial_Jump))
                            ////    RemoveFlag(MsgUpdate.Flags.VIPSpecial_Jump);
                            //if (ContainFlag(MsgUpdate.Flags.VIP))
                            //    RemoveFlag(MsgUpdate.Flags.VIP);
                            ////if (AutoHunting == AutoStructures.Mode.Enable)
                            ////    AutoHunting = AutoStructures.Mode.Disable;
                            //bool tabEnemyInvisible;
                            //tabEnemyInvisible = (Owner.OnAutoAttack = true);
                            //LootDragonBall = (LootExpBall = (LootLetters = (LootMeteor = (GemsLoot = (ProfTokenLoot = (ItemPlusLoot = (StonesLoot = (TabAllyInvisible = (TabEnemyInvisible = tabEnemyInvisible)))))))));
                            //if (OfflineTraining == MsgOfflineTraining.Mode.Hunting)
                            //{
                            //    OfflineTraining = MsgOfflineTraining.Mode.Completed;
                            //    Owner.Socket.Disconnect(OfflineTraining.ToString());
                            //}
                            //break;
                            //if (this.ContainFlag(MsgUpdate.Flags.VIPSpecial_Jump))
                            //    this.RemoveFlag(MsgUpdate.Flags.VIPSpecial_Jump);
                            if (this.ContainFlag(MsgUpdate.Flags.VIP))
                                this.RemoveFlag(MsgUpdate.Flags.VIP);
                            if (this.AutoHunting == AutoStructures.Mode.Enable)
                                this.AutoHunting = AutoStructures.Mode.Disable;
                            this.LootDragonBall = this.LootExpBall = this.LootLetters = this.LootMeteor = this.GemsLoot = this.ItemPlusLoot = this.StonesLoot = this.TabAllyInvisible = this.TabEnemyInvisible = this.Owner.OnAutoAttack = false;
                            if (this.OfflineTraining != MsgOfflineTraining.Mode.Hunting)
                                break;
                            this.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                            this.Owner.Socket.Disconnect(this.OfflineTraining.ToString());
                            break;
                        }
                    case 6:
                        if (!ContainFlag(MsgUpdate.Flags.VIP))
                        {
                            byte power;
                            power = 5;
                            //AddFlag(MsgUpdate.Flags.VIPSpecial_Jump, 2592000, false, 0, power, power);
                            AddFlag(MsgUpdate.Flags.VIP, 2592000, false, 0, power, power);
                            Owner.SendSysMesage("You've received VIP effect thanks for your support!");
                        }
                        break;
                }
            }
        }

        public ushort Face
        {
            get
            {
                return face;
            }
            set
            {
                face = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        SendUpdate(stream, Mesh, MsgUpdate.DataType.Mesh);
                    }
                }
            }
        }

        public byte HairColor
        {
            get
            {
                return (byte)((int)Hair / 100);
            }
            set
            {
                Hair = (ushort)(value * 100 + (int)Hair % 100);
            }
        }

        public Game.MsgTournaments.MsgFreezeWar.Team.TeamType FreezeTeamType;

        public uint PDinamycID { get; set; }

        public uint DynamicID { get; set; }

        public uint Map
        {
            get
            {
                return _mmmap;
            }
            set
            {
                _mmmap = value;
            }
        }

        public ushort X
        {
            get
            {
                return xx;
            }
            set
            {
                Px = X;
                xx = value;
            }
        }

        public ushort Y
        {
            get
            {
                return yy;
            }
            set
            {
                Py = Y;
                yy = value;
            }
        }

        public byte GetGender
        {
            get
            {
                if (Body % 10 >= 3)
                    return 0;
                else
                    return 1;
            }
        }

        public ushort Body
        {
            get
            {
                return body;
            }
            set
            {
                body = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        SendUpdate(stream, Mesh, MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }

        public ushort TransformationID
        {
            get
            {
                return _transformationid;
            }
            set
            {
                _transformationid = value;
                if (Owner.FullLoading)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        SendUpdate(stream, Mesh, MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }

        public bool Alive => HitPoints > 0;

        public uint Mesh => (uint)(TransformationID * 10000000 + Face * 10000 + Body);

        //public bool CurrentPoint(Flags.CurrentPoint _currentPoint, Flags.CurrentPointAction _action, int value)
        //{
        //    bool Allow;
        //    Allow = false;
        //    switch (_currentPoint)
        //    {
        //        case Flags.CurrentPoint.CPS:
        //            switch (_action)
        //            {
        //                case Flags.CurrentPointAction.Add:
        //                    {
        //                        if (value != 0)
        //                            break;
        //                        int _MaxValue4;
        //                        _MaxValue4 = value + ConquerPoints;
        //                        if (_MaxValue4 < int.MaxValue)
        //                        {
        //                            ConquerPoints += value;
        //                            Allow = true;
        //                            break;
        //                        }
        //                        using (RecycledPacket rec = new RecycledPacket())
        //                        {
        //                            Packet stream;
        //                            stream = rec.GetStream();
        //                            ServerKernel.Log.ServerLogCheats(Name + " has send to jail used changed cps calue");
        //                            ServerKernel.Log.GmLog("CPCheat", Name + " has send to jail used changed cps calue");
        //                            SystemBannedAccount.AddBan(UID, Name, 255u, SystemBannedAccount._Type.Silence);
        //                            Owner.Teleport(100, 100, 6003u);
        //                            ConquerPoints = 0;
        //                            string _message;
        //                            _message = "Sod.." + Name + "..override the law, now is gone to the prison of hell Bot Jail, the signing of the Administration";
        //                            Program.SendGlobalPackets.Enqueue(new MsgMessage(_message, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.WhiteVibrate).GetArray(stream));
        //                        }
        //                        break;
        //                    }
        //                case Flags.CurrentPointAction.Remove:
        //                    if (value <= 0 && ConquerPoints >= value)
        //                    {
        //                        ConquerPoints -= value;
        //                        Allow = true;
        //                    }
        //                    break;
        //            }
        //            break;
        //        case Flags.CurrentPoint.GiftCPS:
        //            switch (_action)
        //            {
        //                case Flags.CurrentPointAction.Add:
        //                    if (value != 0)
        //                    {
        //                        uint _MaxValue2;
        //                        _MaxValue2 = (uint)(value + EmoneyPoints);
        //                        if (_MaxValue2 < int.MaxValue)
        //                        {
        //                            EmoneyPoints += (int)value;
        //                            Allow = true;
        //                        }
        //                    }
        //                    break;
        //                case Flags.CurrentPointAction.Remove:
        //                    if (value != 0 && EmoneyPoints >= value)
        //                    {
        //                        EmoneyPoints -= (int)value;
        //                        Allow = true;
        //                    }
        //                    break;
        //            }
        //            break;
        //        case Flags.CurrentPoint.OTP:
        //            switch (_action)
        //            {
        //                case Flags.CurrentPointAction.Add:
        //                    if (value != 0)
        //                    {
        //                        uint _MaxValue;
        //                        _MaxValue = (uint)(value + OnlinePoints);
        //                        if (_MaxValue < int.MaxValue)
        //                        {
        //                            OnlinePoints += (int)value;
        //                            Allow = true;
        //                        }
        //                    }
        //                    break;
        //                case Flags.CurrentPointAction.Remove:
        //                    if (value != 0 && OnlinePoints >= value)
        //                    {
        //                        OnlinePoints -= (int)value;
        //                        Allow = true;
        //                    }
        //                    break;
        //            }
        //            break;
        //        case Flags.CurrentPoint.Silver:
        //            switch (_action)
        //            {
        //                case Flags.CurrentPointAction.Add:
        //                    if (value != 0)
        //                    {
        //                        int _MaxValue3;
        //                        _MaxValue3 = value + Money;
        //                        if (_MaxValue3 < int.MaxValue)
        //                        {
        //                            Money += value;
        //                            Allow = true;
        //                        }
        //                    }
        //                    break;
        //                case Flags.CurrentPointAction.Remove:
        //                    if (value != 0 && Money >= value)
        //                    {
        //                        Money -= value;
        //                        Allow = true;
        //                    }
        //                    break;
        //            }
        //            break;
        //    }
        //    return Allow;
        //}

        public bool IsBoy()
        {
            return Core.IsBoy(Body);
        }

        public bool IsGirl()
        {
            return Core.IsGirl(Body);
        }

        public bool GetOfflineMode(MsgOfflineTraining.Mode type)
        {
            bool can;
            can = false;
            if (type == OfflineTraining)
                can = true;
            return can;
        }

        public void AddExtraAtributes(Packet stream, ushort value)
        {
            if (ExtraAtributes + value <= 300)
            {
                ExtraAtributes += value;
                Atributes += value;
                SendUpdate(stream, Atributes, MsgUpdate.DataType.Atributes);
            }
            else
            {
                value = (ushort)(ExtraAtributes - 300);
                ExtraAtributes += value;
                Atributes += value;
                SendUpdate(stream, Atributes, MsgUpdate.DataType.Atributes);
            }
        }

        public void ActiveAttackPotion(int Timer)
        {
            OnAttackPotion = true;
            OnAttackPotionStamp = Time32.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.Stigma, Timer * 60, true);
            Owner.SendSysMesage($"Stigma for {Timer * 60} seconds, your attack will be increased by {60} percent.");
        }

        public void ActiveDefensePotion(int Timer)
        {
            OnDefensePotion = true;
            OnDefensePotionStamp = Time32.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.MagicShield, Timer * 60, true);
            Owner.SendSysMesage($"Shield for {Timer * 60} seconds, your defense will be increased by {60} times.");
        }

        public unsafe void _DelayedTask(Packet stream, string Name, ushort timer, MsgAutoHunt.TaskButton _option)
        {
            SendString(stream, MsgStringPacket.StringID.Effect, true, "hunpo02");
            DelayedTaskStamp = Time32.Now.AddSeconds(timer);
            Owner.Send(stream.ActionPick(UID, 1, timer, Name));
            DelayedTaskOption = _option;
            DelayedTask = true;
            ActionQuery actionQuery;
            actionQuery = default(ActionQuery);
            actionQuery.ObjId = UID;
            actionQuery.Type = (ActionType)1165;
            actionQuery.wParam1 = 277;
            actionQuery.wParam2 = 2050;
            ActionQuery action;
            action = actionQuery;
            Owner.Send(stream.ActionCreate(&action));
        }

        public void RemovePick(Packet stream)
        {
            DelayedTask = false;
            Owner.Send(stream.ActionPick(UID, 3, 0, Name));
        }

        public bool IsTrap()
        {
            return false;
        }

        public void AddSpecialTitle(Packet stream, MsgTitle.TitleType type, bool aSwitch = false)
        {
            if (TheChosenProject.Database.SpecialTitles.Titles.TryGetValue((uint)type, out var dbtitle) && !SpecialTitles.ContainsKey(type))
            {
                DateTime EndsOn;
                EndsOn = DateTime.Now.AddMinutes(dbtitle.Time);
                SpecialTitles.Add((MsgTitle.TitleType)dbtitle.ID, EndsOn);
                Owner.Send(stream.TitleCreate(UID, (byte)type, MsgTitle.QueueTitle.Enqueue));
                Owner.SendSysMesage("You received " + dbtitle.Name + " special title will remove on " + EndsOn.ToString("d/M/yyyy (H:mm)") + ".");
                if (aSwitch)
                    SwitchTitle((byte)type);
            }
        }

        public void RemoveSpecialTitle(MsgTitle.TitleType type, Packet stream)
        {
            if (TheChosenProject.Database.SpecialTitles.Titles.TryGetValue((uint)type, out var dbtitle))
            {
                if (SpecialTitles.ContainsKey((MsgTitle.TitleType)dbtitle.ID))
                    SpecialTitles.Remove((MsgTitle.TitleType)dbtitle.ID);
                MyTitle = 0;
                Owner.Send(stream.TitleCreate(UID, (byte)type, MsgTitle.QueueTitle.Dequeue));
            }
        }

        public void AddTitle(byte _title, bool aSwitch)
        {
            if (!Titles.Contains(_title))
            {
                Titles.Add(_title);
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Owner.Send(stream.TitleCreate(UID, _title, MsgTitle.QueueTitle.Enqueue));
                }
                if (aSwitch)
                    SwitchTitle(_title);
            }
        }

        public void SwitchTitle(byte ntitle)
        {
            if (Titles.Contains(ntitle) || ntitle == 0)
            {
                MyTitle = ntitle;
                using (RecycledPacket recycledPacket = new RecycledPacket())
                {
                    Packet stream2;
                    stream2 = recycledPacket.GetStream();
                    Owner.Send(stream2.TitleCreate(UID, ntitle, MsgTitle.QueueTitle.Change));
                }
            }
            if (SpecialTitles.ContainsKey((MsgTitle.TitleType)ntitle) || ntitle == 0)
            {
                MyTitle = ntitle;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    Owner.Send(stream.TitleCreate(UID, ntitle, MsgTitle.QueueTitle.Change));
                }
            }
        }


        public void RemoveSpecialGarment(Packet stream)
        {
            SpecialGarment = 0u;
            GarmentId = 0u;
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out var item))
            {
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, 9uL, 0u, 0u, 0u, 0u));
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
        }

        public void AddSpecialGarment(Packet stream, uint ID)
        {
            SpecialGarment = ID;
            GarmentId = SpecialGarment;
            Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, 4294967294u, 9uL, 0u, 0u, 0u, 0u));
            MsgGameItem item;
            item = new MsgGameItem
            {
                ITEM_ID = ID,
                Mode = Flags.ItemMode.AddItem,
                UID = 4294967294u,
                Color = Flags.Color.Red,
                Position = 9,
                Durability = Server.ItemsBase[ID].Durability
            };
            item.Send(Owner, stream);
            Owner.Equipment.AppendItems(true, Owner.Equipment.CurentEquip, stream);
            View.SendView(GetArray(stream, false), false);
        }

        public void RemoveSpecialAccessory(Packet stream)
        {
            if (RightSpecialAccessory != 0)
            {
                RightSpecialAccessory = (RightWeaponAccessoryId = 0u);
                if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.RightWeaponAccessory, out var item2))
                {
                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item2.UID, 15uL, 0u, 0u, 0u, 0u));
                    item2.Mode = Flags.ItemMode.AddItem;
                    item2.Send(Owner, stream);
                }
                Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
            }
            if (LeftSpecialAccessory != 0)
            {
                LeftSpecialAccessory = (LeftWeaponAccessoryId = 0u);
                if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.LeftWeaponAccessory, out var item))
                {
                    Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, 16uL, 0u, 0u, 0u, 0u));
                    item.Mode = Flags.ItemMode.AddItem;
                    item.Send(Owner, stream);
                }
                Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
            }
        }
        public Player Target = null;
        public void AddSpecialAccessory(Packet stream, uint ID)
        {
            if (ItemType.IsAccessoryShield.Contains(ID) && ItemType.IsShield(Owner.Equipment.LeftWeapon))
            {
                LeftSpecialAccessory = ID;
                LeftWeaponAccessoryId = LeftSpecialAccessory;
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, 4294967292u, 16uL, 0u, 0u, 0u, 0u));
                MsgGameItem item2;
                item2 = new MsgGameItem
                {
                    ITEM_ID = ID,
                    Mode = Flags.ItemMode.AddItem,
                    UID = 4294967292u,
                    Color = Flags.Color.Red,
                    Position = 16,
                    Durability = Server.ItemsBase[ID].Durability
                };
                item2.Send(Owner, stream);
                Owner.Equipment.AppendItems(true, Owner.Equipment.CurentEquip, stream);
                View.SendView(GetArray(stream, false), false);
            }
            else
            {
                RightSpecialAccessory = ID;
                RightWeaponAccessoryId = RightSpecialAccessory;
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, 4294967293u, 15uL, 0u, 0u, 0u, 0u));
                MsgGameItem item;
                item = new MsgGameItem
                {
                    ITEM_ID = ID,
                    Mode = Flags.ItemMode.AddItem,
                    UID = 4294967293u,
                    Color = Flags.Color.Red,
                    Position = 15,
                    Durability = Server.ItemsBase[ID].Durability
                };
                item.Send(Owner, stream);
                Owner.Equipment.AppendItems(true, Owner.Equipment.CurentEquip, stream);
                View.SendView(GetArray(stream, false), false);
            }
        }

        public unsafe void SetPkMode(Flags.PKMode pkmode)
        {
            PreviousPkMode = PkMode;
            PkMode = pkmode;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = UID;
                actionQuery.dwParam = (uint)PkMode;
                actionQuery.Type = ActionType.SetPkMode;
                ActionQuery action;
                action = actionQuery;
                Owner.Send(stream.ActionCreate(&action));
            }
        }

        public unsafe void RestorePkMode()
        {
            PkMode = PreviousPkMode;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = UID;
                actionQuery.dwParam = (uint)PkMode;
                actionQuery.Type = ActionType.SetPkMode;
                ActionQuery action;
                action = actionQuery;
                Owner.Send(stream.ActionCreate(&action));
            }
        }

        public void AddCursed(int time)
        {
            if (time != 0)
            {
                if (ContainFlag(MsgUpdate.Flags.Cursed))
                    RemoveFlag(MsgUpdate.Flags.Cursed);
                CursedTimer += time;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    SendUpdate(stream, CursedTimer, MsgUpdate.DataType.CursedTimer);
                }
                AddFlag(MsgUpdate.Flags.Cursed, CursedTimer, false, 1);
            }
        }

        public int MaxBP()
        {
            if (Nobility == null)
                return 0;
            switch (Nobility.Rank)
            {
                case Nobility.NobilityRank.King:
                    return 385;
                case Nobility.NobilityRank.Prince:
                    return 382;
                case Nobility.NobilityRank.Duke:
                    return 380;
                default:
                    return 379;
            }
        }

        public void SetBattlePowers(uint val, uint Potency)
        {
            MentorBp = val;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgUpdate upd;
                upd = new MsgUpdate(stream, UID, 2u);
                stream = upd.Append(stream, MsgUpdate.DataType.ExtraBattlePower, val);
                stream = upd.Append(stream, MsgUpdate.DataType.ExtraBattlePower, Potency);
                stream = upd.GetArray(stream);
                Owner.Send(stream);
            }
        }

        public bool Rate(int value)
        {
            return value > MyRandom.Next() % 100;
        }

        internal void ProtectAttack(int StampMiliSeconds)
        {
            Protect = Time32.Now.AddMilliseconds(StampMiliSeconds);
        }

        internal void ProtectJumpAttack(int Seconds)
        {
            ProtectedJumpAttack = Time32.Now.AddSeconds(Seconds);
        }

        internal bool AllowAttack()
        {
            if (Time32.Now > Protect)
                return Time32.Now > ProtectedJumpAttack;
            return false;
        }

        internal void CheckAura()
        {
            if (UseAura != MsgUpdate.Flags.Normal)
                IncreaseStatusAura(UseAura, Aura);
        }

        internal bool AddAura(MsgUpdate.Flags flag, MagicType.Magic new_aura, int Timer)
        {
            if (flag == UseAura)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = MsgUpdate.Flags.Normal;
                return false;
            }
            if (UseAura != MsgUpdate.Flags.Normal)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = MsgUpdate.Flags.Normal;
            }
            UseAura = flag;
            Aura = new_aura;
            IncreaseStatusAura(flag, new_aura);
            AddFlag(flag, Timer, true);
            MsgFlagIcon.ShowIcon icon;
            icon = MsgFlagIcon.ShowIcon.EarthAura;
            switch (flag)
            {
                case MsgUpdate.Flags.FeandAura:
                    icon = MsgFlagIcon.ShowIcon.FeandAura;
                    break;
                case MsgUpdate.Flags.TyrantAura:
                    icon = MsgFlagIcon.ShowIcon.TyrantAura;
                    break;
                case MsgUpdate.Flags.MetalAura:
                    icon = MsgFlagIcon.ShowIcon.MetalAura;
                    break;
                case MsgUpdate.Flags.WoodAura:
                    icon = MsgFlagIcon.ShowIcon.WoodAura;
                    break;
                case MsgUpdate.Flags.WaterAura:
                    icon = MsgFlagIcon.ShowIcon.WaterAura;
                    break;
                case MsgUpdate.Flags.FireAura:
                    icon = MsgFlagIcon.ShowIcon.FireAura;
                    break;
                case MsgUpdate.Flags.EartAura:
                    icon = MsgFlagIcon.ShowIcon.EarthAura;
                    break;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Owner.Send(stream.FlagIconCreate(UID, icon, new_aura.Level, (uint)new_aura.Damage));
            }
            return true;
        }

        private void DecreaseStatusAura(MsgUpdate.Flags flag)
        {
            switch (flag)
            {
                case MsgUpdate.Flags.FeandAura:
                    Owner.Status.Immunity -= (uint)(Aura.Damage * 100f);
                    break;
                case MsgUpdate.Flags.TyrantAura:
                    Owner.Status.CriticalStrike -= (uint)(Aura.Damage * 100f);
                    break;
                case MsgUpdate.Flags.MetalAura:
                    Owner.Status.MetalResistance -= (uint)Aura.Damage;
                    break;
                case MsgUpdate.Flags.WoodAura:
                    Owner.Status.WoodResistance -= (uint)Aura.Damage;
                    break;
                case MsgUpdate.Flags.WaterAura:
                    Owner.Status.WaterResistance -= (uint)Aura.Damage;
                    break;
                case MsgUpdate.Flags.FireAura:
                    Owner.Status.FireResistance -= (uint)Aura.Damage;
                    break;
                case MsgUpdate.Flags.EartAura:
                    Owner.Status.EarthResistance -= (uint)Aura.Damage;
                    break;
            }
        }

        private void IncreaseStatusAura(MsgUpdate.Flags flag, MagicType.Magic new_aura)
        {
            switch (flag)
            {
                case MsgUpdate.Flags.FeandAura:
                    Owner.Status.Immunity += (uint)(new_aura.Damage * 100f);
                    break;
                case MsgUpdate.Flags.TyrantAura:
                    Owner.Status.CriticalStrike += (uint)(new_aura.Damage * 100f);
                    break;
                case MsgUpdate.Flags.MetalAura:
                    Owner.Status.MetalResistance += (uint)new_aura.Damage;
                    break;
                case MsgUpdate.Flags.WoodAura:
                    Owner.Status.WoodResistance += (uint)new_aura.Damage;
                    break;
                case MsgUpdate.Flags.WaterAura:
                    Owner.Status.WaterResistance += (uint)new_aura.Damage;
                    break;
                case MsgUpdate.Flags.FireAura:
                    Owner.Status.FireResistance += (uint)new_aura.Damage;
                    break;
                case MsgUpdate.Flags.EartAura:
                    Owner.Status.EarthResistance += (uint)new_aura.Damage;
                    break;
            }
        }

        public void MessageBox(string text, Action<GameClient> msg_ok, Action<GameClient> msg_cancel, int Seconds = 0, MsgStaticMessage.Messages messaj = MsgStaticMessage.Messages.None)
        {
            if (Program.BlockTeleportMap.Contains(Owner.Player.Map) || Owner.InFIveOut || Owner.InTDM || Owner.InLastManStanding || Owner.InPassTheBomb || Owner.InST)
            {
                if (Owner != null && Owner.Map != null)
                    Owner.SendSysMesage($"You need to leave {Owner.Map.Name} first!");
                return;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MessageOK = msg_ok;
                MessageCancel = msg_cancel;
                Dialog dialog;
                dialog = new Dialog(Owner, stream);
                dialog.CreateMessageBox(text).FinalizeDialog(true);
                StartMessageBox = Time32.Now.AddHours(24);
                if (Seconds != 0)
                {
                    StartMessageBox = Time32.Now.AddSeconds(Seconds);
                    if (messaj != 0)
                        Owner.Send(stream.StaticMessageCreate(messaj, MsgStaticMessage.Action.Append, (uint)Seconds));
                    else
                        CountDown(Seconds);
                }
            }
        }

        public unsafe void CountDown(int seconds)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = UID;
                actionQuery.Type = ActionType.CountDown;
                actionQuery.dwParam = (uint)seconds;
                ActionQuery action;
                action = actionQuery;
                Owner.Send(stream.ActionCreate(&action));
            }
        }

        public void RemoveBuffersMovements(Packet stream)
        {
            InUseIntensify = false;
            RemoveFlag(MsgUpdate.Flags.Praying);
            RemoveFlag(MsgUpdate.Flags.CastPray);
            if (ContainFlag(MsgUpdate.Flags.MagicDefender))
            {
                RemoveFlag(MsgUpdate.Flags.MagicDefender);
                SendUpdate(stream, MsgUpdate.Flags.MagicDefender, 0u, 0u, 0u, MsgUpdate.DataType.AzureShield, true);
            }
        }

        public ushort GetAddStamina()
        {
            Flags.ConquerAction action;
            action = Action;
            if (action == Flags.ConquerAction.Sit)
                    return 12;
            return 5;
        }

        public void AddSpellFlag(MsgUpdate.Flags Flag, int Seconds, bool RemoveOnDead, int StampSeconds = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Seconds, RemoveOnDead, StampSeconds);
        }

        public bool AddFlag(MsgUpdate.Flags Flag, int Seconds, bool RemoveOnDead, int StampSeconds = 0, uint showamount = 0u, uint amount = 0u)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                StatusFlag |= (uint)Flag;
                BitVector.TryAdd((int)Flag, Seconds, RemoveOnDead, StampSeconds);
                UpdateFlagOffset();
                if (Flag >= MsgUpdate.Flags.VIPSpecial_Jump && Flag <= MsgUpdate.Flags.Confused)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        View.SendView(stream.GameUpdateCreate(UID, (MsgGameUpdate.DataType)Flag, true, showamount, (uint)Seconds, amount), true);
                    }
                }
                return true;
            }
            return false;
        }

        public bool RemoveFlag(MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                StatusFlag &= (uint)(~Flag);
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                if (Flag == MsgUpdate.Flags.Oblivion)
                {
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        Owner.IncreaseExperience(stream2, Owner.ExpOblivion);
                    }
                    Owner.ExpOblivion = 0uL;
                }
                if (Flag >= MsgUpdate.Flags.VIPSpecial_Jump && Flag <= MsgUpdate.Flags.Confused)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        Owner.Send(stream.GameUpdateCreate(UID, (MsgGameUpdate.DataType)Flag, false, 0u, 0u));
                    }
                }
                return true;
            }
            return false;
        }

        public bool UpdateFlag(MsgUpdate.Flags Flag, int Seconds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Seconds, SetNewTimer, MaxTime);
        }

        public void ClearFlags()
        {
            BitVector.GetClear();
            UpdateFlagOffset();
        }

        public bool ContainFlag(MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }

        public bool CheckInvokeFlag(MsgUpdate.Flags Flag, Time32 timer32)
        {
            return BitVector.CheckInvoke((int)Flag, timer32);
        }

        public void UpdateFlagOffset()
        {
            SendUpdate(BitVector.bits, MsgUpdate.DataType.StatusFlag, true);
        }

        public void SendUpdateHP()
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet MyStream;
                MyStream = rec.GetStream();
                MsgUpdate Upd;
                Upd = new MsgUpdate(MyStream, UID, 2u);
                MyStream = Upd.Append(MyStream, MsgUpdate.DataType.MaxHitpoints, Owner.Status.MaxHitpoints);
                MyStream = Upd.Append(MyStream, MsgUpdate.DataType.Hitpoints, HitPoints);
                MyStream = Upd.GetArray(MyStream);
                Owner.Send(MyStream);
            }
        }

        public static KillerSystem KillSystem = new KillerSystem();
        public static string GetKillerMessage()
        {
            List<string> stringList = new List<string>()
              {
                "LOL U CANT AIM FOR SHIT! #10 #00",
                "You are too damn slow o_O #04",
                "Why Swing that Sword like that who is your mentor #01",
                "bruh put that blade down! #18 #39",
                "Better luck next time buddie! #19 #39",
                "Only 1 Truth Divine Player HERE PUNK! #28 #16",
                "bring me someone who can help u your dog shit#54 #58",
                "BETTER LUCK NEXT TIME BUDDIE! GO PLAY REALCO! #06"
              };
            int index = new Random().Next(stringList.Count);
            return stringList[index];
        }

        public unsafe void Dead(Player killer, ushort DeadX, ushort DeadY, uint KillerUID)
        {

            //if (killer != null)
            //{
            //    using (RecycledPacket rec = new RecycledPacket())
            //    {
            //        Packet stream;
            //        stream = rec.GetStream();
            //        string Msg;
            //        Msg = Translator.GetTranslatedString(GetKillerMessage(), Translator.Language.EN, Owner.Language);
            //        killer.Owner.Player.View.SendView(new MsgMessage(Msg, Owner.Player.Name, killer.Owner.Player.Name, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream), true);
            //        Owner.Player.View.SendView(new MsgMessage(Msg, Owner.Player.Name, killer.Owner.Player.Name, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream), true);
            //        KillSystem.Update(killer.Owner);
            //        KillSystem.CheckDead(Owner.Player.UID);
            //    }
            //}
            if (Owner.EventBase != null)
            {
                if (Owner.EventBase.EventTitle == "SkyFight")
                {
                    if (Owner.EventBase.Stage == Game.MsgEvents.EventStage.Fighting)
                    {
                        if (killer != null)
                        {
                            Owner.EventBase.Kill(killer.Owner, this.Owner);
                            return;
                        }
                    }
                }
            }
            if (Owner.EventBase != null)
            {
                if (Owner.EventBase.EventTitle == "DragonWar")
                {
                    if (Owner.EventBase.Stage == Game.MsgEvents.EventStage.Fighting)
                    {
                        if (killer != null)
                        {
                            Owner.EventBase.Kill(killer.Owner, this.Owner);
                        }
                    }
                }
            }
            if (Program.MapCounterHits.Contains(Map) || UnlimitedArenaRooms.Maps.ContainsValue(DynamicID))
            {
                killer.ArenaKills += 1;
                ArenaDeads += 1;
            }

            if (Owner.Player.Owner.Player.ContainFlag(MsgUpdate.Flags.Dodge))
            {
                Owner.Player.Owner.Status.Dodge -= 10;
                Owner.Player.Owner.Player.RemoveFlag(MsgUpdate.Flags.Dodge);
                Owner.Player.OnDodge = false;
            }

            if (!Program.BlockAttackMap.Contains(Map) && !Program.FreePkMap.Contains(Map))
            {
                uint cpsDrop = (uint)Role.Core.Random.Next(5, 10);
                int MoneyAmount = (int)Owner.Player.Money * 10 / 100;
                MoneyAmount = (int)Role.Core.Random.Next((MoneyAmount * 50 / 100), MoneyAmount);
                if (Owner.Player.ConquerPoints > cpsDrop)
                {
                    Owner.Player.ConquerPoints -= (int)cpsDrop;
                    Owner.SendSysMesage("You are dead and drop " + cpsDrop + " cps.");
                }
                if (Owner.Player.ContainFlag(MsgUpdate.Flags.Confused))
                {
                    Owner.Player.RemoveFlag(MsgUpdate.Flags.Confused);
                }
                if (Owner.Player.Money > (uint)MoneyAmount)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Owner.Player.Money -= MoneyAmount;
                        uint ItemID = Database.ItemType.MoneyItemID((uint)MoneyAmount);
                        Owner.SendSysMesage("You drop " + MoneyAmount + " amount of GOLD #51 #13", MsgMessage.ChatMode.System, MsgMessage.MsgColor.violet);
                        Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = ItemID;
                        DropItem(stream, Owner.Player.UID, Owner.Map, ItemID, xx, yy, Game.MsgFloorItem.MsgItem.ItemType.Money, (uint)MoneyAmount, false, 0, Owner);
                    }
                }
            }

            if (UnlimitedArenaRooms.Maps.ContainsValue(this.DynamicID))
                return;
            //if (killer.Owner.AIType == AIEnum.AIType.PKFighting)
            //    AIStructures.GetMsgKills(killer.Owner, this.Name);
            if (this.OnTransform && this.TransformInfo != null)
                this.TransformInfo.FinishTransform();
            else if (this.OnTransform)
                this.TransformationID = (ushort)0;
            this.GhostStamp = DateTime.Now.AddMilliseconds(1000.0);
            this.Owner.OnAutoAttack = false;
            this.Owner.SendSysMesage("You are dead.");
            if (this.ContainFlag(MsgUpdate.Flags.FatalStrike))
                this.RemoveFlag(MsgUpdate.Flags.FatalStrike);
            if (this.ContainFlag(MsgUpdate.Flags.Freeze))
                this.RemoveFlag(MsgUpdate.Flags.Freeze);
            this.GetPkPkPoints = true;
            if (this.ContainFlag(MsgUpdate.Flags.RedName) || this.ContainFlag(MsgUpdate.Flags.BlackName) || this.ContainFlag(MsgUpdate.Flags.FlashingName))
                this.GetPkPkPoints = false;
            using (RecycledPacket recycledPacket = new RecycledPacket())
            {
                Packet stream = recycledPacket.GetStream();
                if (Owner.Pet != null)
                    Owner.Pet.DeAtach(stream);
                if (!Program.FreePkMap.Contains(this.Map) && this.Associate != null && killer != null && killer.Owner.AIType == AIEnum.AIType.NotActive)
                {
                    killer.Associate.AddPKExplorer(killer.Owner, this);
                    this.Associate.AddEnemy(this.Owner, killer);
                }
                if (this.BlackSpot)
                {
                    this.BlackSpot = false;
                    this.View.SendView(stream.BlackspotCreate(false, this.UID), true);
                }
                this.Dead_X = DeadX;
                this.Dead_Y = DeadY;
                this.DeadStamp = Time32.Now;
                this.DeathStamp = DateTime.Now;
                this.HitPoints = 0;
                this.ClearFlags();
                this.AddFlag(MsgUpdate.Flags.Dead, 2592000, true);
                if (this.Map == 700U)
                    this.Owner.EndQualifier();
                if (killer != null && killer.Owner.AIType == AIEnum.AIType.NotActive)
                {
                    if (killer.Map == 2072 || (MsgSchedules.EliteGuildWar.Proces != ProcesType.Dead && killer.Map == 8250))
                    {
                        if (killer.GuildID != GuildID && killer.MyGuild != null && MyGuild != null && !MyGuild.Ally.ContainsKey(killer.GuildID))
                            MsgSchedules.EliteGuildWar.AddPoints(killer, 1, 0);
                        MsgSchedules.EliteGuildWar.AddPoints(this, 0, 1);
                    }
                    if (killer.Map == 1038)
                    {
                        if (killer.GuildID != GuildID && killer.MyGuild != null && MyGuild != null && !MyGuild.Ally.ContainsKey(killer.GuildID))
                            MsgSchedules.GuildWar.AddPoints(killer, 1, 0);
                        MsgSchedules.GuildWar.AddPoints(this, 0, 1);
                    }
                    if (killer.Map == 19891)
                    {
                        if (killer.GuildID != GuildID && killer.MyGuild != null && MyGuild != null && !MyGuild.Ally.ContainsKey(killer.GuildID))
                            MsgSchedules.CitywarTC.AddPoints(killer, 1, 0);
                        MsgSchedules.CitywarTC.AddPoints(this, 0, 1);
                    }
                    if (killer.Map == 19892)
                    {
                        if (killer.GuildID != GuildID && killer.MyGuild != null && MyGuild != null && !MyGuild.Ally.ContainsKey(killer.GuildID))
                            MsgSchedules.CitywarPC.AddPoints(killer, 1, 0);
                        MsgSchedules.CitywarPC.AddPoints(this, 0, 1);
                    }
                    if (killer.Map == 1125)
                    {
                        if (killer.ClanUID != ClanUID && killer.MyClan != null && MyClan != null && !MyClan.Ally.ContainsKey(killer.ClanUID))
                            MsgSchedules.ClanWar.AddPoints(killer, 1, 0);
                        MsgSchedules.ClanWar.AddPoints(this, 0, 1);
                    }
                    if (killer.Map == 2058)
                    {
                        if (killer.GuildID != GuildID && killer.MyGuild != null && MyGuild != null && !MyGuild.Ally.ContainsKey(killer.GuildID))
                            MsgSchedules.CityWar.AddPoints(killer, 1, 0);
                        MsgSchedules.CityWar.AddPoints(this, 0, 1);
                    }
                    ++killer.XPCount;
                    if (this.AutoHunting == AutoStructures.Mode.Enable)
                        this.KillerHunting = killer.Name;
                    if (MsgSchedules.CurrentTournament.InTournament(this.Owner) && MsgSchedules.CurrentTournament.Type == TournamentType.LastmanStand)
                    {
                        MsgLastManStand currentTournament = MsgSchedules.CurrentTournament as MsgLastManStand;
                        this.Owner.Teleport((ushort)428, (ushort)378, 1002U);
                        currentTournament.KillSystem.Update(killer.Owner);
                        currentTournament.KillSystem.CheckDead(this.UID);
                    }
                    InteractQuery interactQuery = new InteractQuery()
                    {
                        UID = killer.UID,
                        X = DeadX,
                        Y = DeadY,
                        AtkType = MsgAttackPacket.AttackID.Death,
                        KillCounter = killer.KillCounter,
                        SpellID = TheChosenProject.Database.ItemType.IsBow(killer.Owner.Equipment.RightWeapon) ? (ushort)5 : (ushort)1,
                        OpponentUID = this.UID
                    };
                    this.View.SendView(stream.InteractionCreate(&interactQuery), true);
                    if (Program.NoDropItems.Contains(this.Map) || Program.FreePkMap.Contains(this.Map))
                        return;
                    this.CheckDropItems(killer, stream);
                    if (this.PKPoints >= (ushort)100)
                    {
                        this.Owner.Teleport((ushort)029, (ushort)070, 6000U, revive: false);
                        this.JailerUID = killer.UID;
                    }
                    this.CheckPkPoints(killer);
                }
                else
                {
                    InteractQuery interactQuery = new InteractQuery()
                    {
                        UID = KillerUID,
                        X = DeadX,
                        Y = DeadY,
                        AtkType = MsgAttackPacket.AttackID.Death,
                        OpponentUID = this.UID
                    };
                    this.View.SendView(stream.InteractionCreate(&interactQuery), true);
                    if (Program.NoDropItems.Contains(this.Map) || Program.FreePkMap.Contains(this.Map))
                        return;
                    this.CheckDropItems(killer, stream);
                }
            }
        }

        public void DropItem(ServerSockets.Packet stream, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, Game.MsgFloorItem.MsgItem.ItemType typ
, uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null, bool itemverf = false)
        {

            if (ItemID == 1088000 || ItemID == 1088001)
            {
                if (!itemverf)
                    return;
            }
            Game.MsgServer.MsgGameItem DataItem = new Game.MsgServer.MsgGameItem();

            DataItem.ITEM_ID = ItemID;
            if (DataItem.Durability > 100)
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(100, DataItem.Durability / 10);
                DataItem.MaximDurability = DataItem.Durability;
            }

            else
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(1, 10);
                DataItem.MaximDurability = 10;
            }

            DataItem.Color = Role.Flags.Color.Red;
            Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, DynamicID, Map, OwnerItem, true, map);

            if (map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Visible);
            }
        }

        private void CheckPkPoints(Player killer)
        {
            if ((Map == 1011 && DynamicID != 0) || Program.FreePkMap.Contains(Map) || ContainFlag(MsgUpdate.Flags.RedName) || ContainFlag(MsgUpdate.Flags.BlackName))
                return;
            //if (HeavenBlessing > 0)
            //{
            //    if (killer.HeavenBlessing > 0)
            //        //Owner.LoseDeadExperience(killer.Owner);
            //    else
            //    {
            //        Owner.SendSysMesage("Your Heaven Blessing takes effect! You lose no EXP!");
            //        killer.AddCursed(300);
            //    }
            //}
            //else
            //    Owner.LoseDeadExperience(killer.Owner);
            if (!GetPkPkPoints)
                return;
            if (killer.MyGuild != null && killer.MyGuild.Enemy.ContainsKey(GuildID))
            {
                killer.PKPoints += 3;
                if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                    killer.MyGuildMember.PkDonation += 5u;
                if (Server.MapName.ContainsKey(Map) && (int)GuildRank >= 890)
                    killer.MyGuild.SendMessajGuild("The (" + killer.GuildRank.ToString() + ")" + killer.Name + " killed (" + GuildRank.ToString() + ")" + Name + " from guild " + MyGuild.GuildName + " in " + Server.MapName[Map]);
            }
            else if (killer.MyClan != null && killer.MyClan.Enemy.ContainsKey(ClanUID))
            {
                killer.PKPoints += 3;
                if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                    killer.MyGuildMember.PkDonation += 5u;
            }
            else if (killer.Associate.Contain(2, UID))
            {
                killer.PKPoints += 5;
                if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                    killer.MyGuildMember.PkDonation += 10u;
            }
            else
            {
                killer.PKPoints += 10;
                if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                    killer.MyGuildMember.PkDonation += 10u;
            }
        }

        public void CheckDropItems(Player killer, Packet stream)
        {
            if (AutoHunting != 0)
                return;
            ushort x;
            x = X;
            ushort y;
            y = Y;
            if (x > 5 && y > 5)
            {
                MsgGameItem[] inventoryItems;
                inventoryItems = Owner.Inventory.ClientItems.Values.ToArray();
                if ((inventoryItems.Length / 4) > 0)
                {
                    //count = (uint)ServerKernel.NextAsync(1, inventoryItems.Length / 4);
                    uint count = (uint)Program.GetRandom.Next(1, (int)(inventoryItems.Length / 4));
                    for (int index = 0; index < count; index++)
                    {
                        try
                        {
                            if (inventoryItems.Length <= index || inventoryItems[index] == null)
                                continue;
                            MsgGameItem item3;
                            item3 = inventoryItems[index];
                            if (item3.Position != 27 && item3.Position != 7 && item3.Locked == 0 && item3.Inscribed == 0 && item3.Bound == 0 && !ItemType.unabletradeitem.Contains(item3.ITEM_ID) && !ItemType.undropeitem.Contains(item3.ITEM_ID) && !ItemType.IsSash(item3.ITEM_ID))
                            {
                                ushort New_X;
                                New_X = (ushort)ServerKernel.NextAsync((ushort)(x - 5), (ushort)(x + 5));
                                ushort New_Y;
                                New_Y = (ushort)ServerKernel.NextAsync((ushort)(y - 5), (ushort)(y + 5));
                                if (Owner.Map.AddGroundItem(ref New_X, ref New_Y, 0))
                                    DropItem(item3, New_X, New_Y, stream);
                            }
                        }
                        catch (Exception e)
                        {
                            ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
                        }
                    }
                }
            }
            if (PKPoints < 30 || killer == null || Program.FreePkMap.Contains(Map))
                return;
            int Count_DropItem;
            Count_DropItem = ((PKPoints >= 30 && PKPoints <= 99) ? 1 : 2);
            MsgGameItem[] EquipmentArray;
            EquipmentArray = Owner.Equipment.CurentEquip.Where((MsgGameItem p) => p != null && p.Position != 7 && p.Position != 27 && p.Position != 9 && p.Position != 29 && p.Position != 12 && p.Position != 17 && p.Position != 15 && p.Position != 16).ToArray();
            if (EquipmentArray.Length == 0)
                return;
            int trying;
            trying = 0;
            int Dropable;
            Dropable = 0;
            Dictionary<uint, MsgGameItem> ItemsDrop;
            ItemsDrop = new Dictionary<uint, MsgGameItem>();
            while (trying != 14)
            {
                byte ArrayPosition;
                ArrayPosition = (byte)ServerKernel.NextAsync(0, EquipmentArray.Length);
                MsgGameItem Element;
                Element = EquipmentArray[ArrayPosition];
                if (!ItemsDrop.ContainsKey(Element.UID))
                {
                    ItemsDrop.Add(Element.UID, Element);
                    Dropable++;
                }
                trying++;
                if (Dropable >= Count_DropItem)
                    break;
            }
            foreach (MsgGameItem item2 in ItemsDrop.Values)
            {
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item2.UID, item2.Position, 0u, 0u, 0u, 0u));
                Owner.Equipment.ClientItems.TryRemove(item2.UID, out var _);
                if (item2.Inscribed == 1)
                {
                    if (MyGuild != null && MyGuild.MyArsenal != null)
                        MyGuild.MyArsenal.Remove(Guild.Arsenal.GetArsenalPosition(item2.ITEM_ID), item2.UID);
                    item2.Inscribed = 0u;
                }
            }
            if (MyGuild != null)
                GuildBattlePower = MyGuild.ShareMemberPotency(GuildRank);
            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
            //--------------------------------

            //add container Item //bahaa
            foreach (var item in ItemsDrop.Values)
                DropItem(item, killer.X, killer.Y, stream);
            //foreach (var item in ItemsDrop.Values)
            //    Owner.Confiscator.AddItem(Owner, killer.Owner, item, stream);
            //--------------------------------
        }

        public void DropItem(MsgGameItem item, ushort x, ushort y, Packet stream)
        {
            MsgItem DropItem;
            DropItem = new MsgItem(item, x, y, MsgItem.ItemType.Item, 0u, DynamicID, Map, UID, false, Owner.Map);
            if (Owner.Map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, MsgDropID.Visible);
                Owner.Inventory.Update(item, AddMode.REMOVE, stream, true);
            }
        }

        public void Revive(Packet stream)
        {
            ProtectAttack(2000);
            HitPoints = (int)Owner.Status.MaxHitpoints;
            Mana = (ushort)Owner.Status.MaxMana;
            ClearFlags();
            TransformationID = 0;
            XPCount = 0;
            SendUpdate(stream, XPCount, MsgUpdate.DataType.XPCircle);
            Stamina = 150;
            SendUpdate(stream, Stamina, MsgUpdate.DataType.Stamina);
            Send(stream.MapStatusCreate(Map, Map, Owner.Map.TypeStatus));
            View.SendView(GetArray(stream, false), false);
        }

        public void OpenXpSkill(MsgUpdate.Flags flag, int Timer, int StampExec = 0)
        {
            if (IsHunting == true)
                return;
            if (AutoHunting == AutoStructures.Mode.Enable)
                return;
            if (OfflineTraining == MsgOfflineTraining.Mode.Hunting)
                return;
            XPCount = 0;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                SendUpdate(stream, XPCount, MsgUpdate.DataType.XPCircle);
            }
            MsgUpdate.Flags UseSpell;
            UseSpell = OnXPSkill();
            if (UseSpell == MsgUpdate.Flags.Normal)
            {
                KillCounter = 0u;
                UseXPSpell = flag;
                AddFlag(flag, Timer, true, StampExec);
            }
            else if (UseSpell != flag)
            {
                RemoveFlag(UseSpell);
                UseXPSpell = flag;
                AddFlag(flag, Timer, true, StampExec);
            }
            else if (flag == MsgUpdate.Flags.Cyclone || flag == MsgUpdate.Flags.Superman)
            {
                UpdateFlag(flag, Timer, true, 20);
            }
            else
            {
                UpdateFlag(flag, Timer, true, 60);
            }
        }

        public MsgUpdate.Flags OnXPSkill()
        {
            if (ContainFlag(MsgUpdate.Flags.Cyclone))
                return MsgUpdate.Flags.Cyclone;
            if (ContainFlag(MsgUpdate.Flags.Superman))
                return MsgUpdate.Flags.Superman;
            if (ContainFlag(MsgUpdate.Flags.Oblivion))
                return MsgUpdate.Flags.Oblivion;
            if (ContainFlag(MsgUpdate.Flags.FatalStrike))
                return MsgUpdate.Flags.FatalStrike;
            if (ContainFlag(MsgUpdate.Flags.ShurikenVortex))
                return MsgUpdate.Flags.ShurikenVortex;
            if (ContainFlag(MsgUpdate.Flags.ChaintBolt))
                return MsgUpdate.Flags.ChaintBolt;
            if (ContainFlag(MsgUpdate.Flags.BlackbeardsRage))
                return MsgUpdate.Flags.BlackbeardsRage;
            if (ContainFlag(MsgUpdate.Flags.CannonBarrage))
                return MsgUpdate.Flags.CannonBarrage;
            return MsgUpdate.Flags.Normal;
        }

        public void UpdateXpSkill()
        {
            if ((UseXPSpell == MsgUpdate.Flags.Cyclone || UseXPSpell == MsgUpdate.Flags.Superman) && ContainFlag(UseXPSpell))
                UpdateFlag(UseXPSpell, 1, false, 20);
        }

        public unsafe void SendScrennXPSkill(IMapObj obj)
        {
            if (OnXPSkill() != MsgUpdate.Flags.Normal)
            {
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    InteractQuery interactQuery;
                    interactQuery = default(InteractQuery);
                    interactQuery.UID = UID;
                    interactQuery.KilledMonster = true;
                    interactQuery.X = X;
                    interactQuery.Y = Y;
                    interactQuery.AtkType = MsgAttackPacket.AttackID.Death;
                    interactQuery.KillCounter = KillCounter;
                    InteractQuery action;
                    action = interactQuery;
                    obj.Send(stream.InteractionCreate(&action));
                }
            }
        }

        public void Send(Packet msg)
        {
            Owner.Send(msg);
        }

        public Player(GameClient _own)
        {
            AllowDynamic = false;
            Owner = _own;
            ObjType = MapObjectType.Player;
            View = new RoleView(Owner);
            BitVector = new StatusFlagsBigVector32(128);
            QuestGUI = new Quests(this);
        }

        public void AddVIPLevel(byte _viplevel, DateTime TimeLeft, Packet stream)
        {
            if (_viplevel >= 2)
            {

                VipLevel = _viplevel;
                ExpireVip = TimeLeft;
                SendUpdate(stream, VipLevel, MsgUpdate.DataType.VIPLevel);
                UpdateVip(stream);
                Owner.CreateBoxDialog($"You`ve received VIP{_viplevel} and a Heaven Bleesing Left on: {TimeLeft:d/M/yyyy (H:mm)}.");

            }
        }

        public void RemovVIPLevel(byte _viplevel, DateTime TimeLeft, byte _oldVip, Packet stream)
        {
            if (_viplevel >= 0)
            {
                VipLevel = _viplevel;
                ExpireVip = TimeLeft;
                HeavenBlessing = 0;
                this.RemoveFlag(MsgUpdate.Flags.HeavenBlessing);
                this.HeavenBlessing = 0;
                this.HeavenBlessTime = Time32.Now;
                SendUpdate(stream, 0, MsgUpdate.DataType.HeavensBlessing);
                SendUpdate(stream, VipLevel, MsgUpdate.DataType.VIPLevel);
                UpdateVip(stream);
                Owner.CreateBoxDialog($"You`ve remove VIP{_oldVip}.");
            }
        }

        public void ClearPreviouseCoord()
        {
            Px = 0;
            Py = 0;
        }

        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(X, Y, X2, Y2);
        }

        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(Px, Py, X2, Y2);
        }

        public bool InView(ushort X2, ushort Y2, byte distance)
        {
            if (OldGetDistance(X2, Y2) > distance)
                return GetMyDistance(X2, Y2) <= distance;
            return false;
        }

        public void CreateExtraExpPacket(Packet stream)
        {
            MsgUpdate update;
            update = new MsgUpdate(stream, UID);
            stream = update.Append(stream, MsgUpdate.DataType.DoubleExpTimer, new uint[4]
            {
                DExpTime,
                0u,
                RateExp * 100,
                0u
            });
            stream = update.GetArray(stream);
            Owner.Send(stream);
        }

        public void AddHeavenBlessing(Packet stream, int Time)
        {
            if (!ContainFlag(MsgUpdate.Flags.HeavenBlessing))
                HeavenBlessTime = Time32.Now;
            if (Time > 86400)
                Owner.SendSysMesage("You`ve received " + Time / 86400 + " days` blessing time.");
            else
                Owner.SendSysMesage("You`ve received " + Time / 60 / 60 + " hours` blessing time.");
            bool None;
            None = HeavenBlessing == 0;
            HeavenBlessTime = HeavenBlessTime.AddSeconds(Time);
            HeavenBlessing += Time;
            CreateHeavenBlessPacket(stream, None);
            if (MyMentor != null)
            {
                MyMentor.Mentor_Blessing += (uint)(Time / 10000);
                if (MyMentor.Associat.ContainsKey(5) && MyMentor.Associat[5].TryGetValue(UID, out var mee))
                    mee.Blessing += (uint)(Time / 10000);
            }
        }

        public void CreateHeavenBlessPacket(Packet stream, bool ResetOnlineTraining)
        {
            if (HeavenBlessing > 0)
            {
                if (ResetOnlineTraining)
                {
                    ReceivePointsOnlineTraining = Time32.Now.AddMinutes(2);
                    OnlineTrainingTime = Time32.Now.AddMinutes(15);
                }
                AddFlag(MsgUpdate.Flags.HeavenBlessing, 2592000, false);
                SendUpdate(stream, HeavenBlessing, MsgUpdate.DataType.HeavensBlessing);
                SendUpdate(stream, 0L, MsgUpdate.DataType.OnlineTraining);
                if (Map == 601 || Map == 1039)
                    SendUpdate(stream, 1L, MsgUpdate.DataType.OnlineTraining);
                SendString(stream, MsgStringPacket.StringID.Effect, true, "bless");
            }
        }

        public void SendUpdate(Packet stream, long Value, MsgUpdate.DataType datatype, bool scren = false)
        {
            MsgUpdate packet;
            packet = new MsgUpdate(stream, UID);
            stream = packet.Append(stream, datatype, Value);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }

        public void SendUpdate(Packet stream, MsgUpdate.Flags Flag, uint Time, uint Dmg, uint Level, MsgUpdate.DataType datatype, bool scren = false)
        {
            MsgUpdate packet;
            packet = new MsgUpdate(stream, UID);
            stream = packet.Append(stream, datatype, (byte)Flag, Time, Dmg, Level);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }

        public void SendUpdate(Packet stream, MsgUpdate.DataType datatype, uint Time, uint Dmg, uint Level, bool scren = false)
        {
            MsgUpdate packet;
            packet = new MsgUpdate(stream, UID);
            stream = packet.Append(stream, datatype, 0u, Time, Dmg, Level);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }

        public void SendUpdate(uint[] Value, MsgUpdate.DataType datatype, bool scren = false)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgUpdate packet;
                packet = new MsgUpdate(stream, UID);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Owner.Send(stream);
                if (scren)
                    View.SendView(stream, false);
            }
        }

        public void UpdateVip(Packet stream)
        {
            SendUpdate(stream, VipLevel, MsgUpdate.DataType.VIPLevel);
            if (VipLevel > 1)
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.FullVip));
            else
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.None));
        }

        public void SendString(Packet stream, MsgStringPacket.StringID id, bool SendScreen, params string[] args)
        {
            MsgStringPacket packet;
            packet = new MsgStringPacket
            {
                ID = id,
                UID = UID,
                Strings = args
            };
            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }

        public void SendGemString(Packet stream, MsgStringPacket.StringID id, bool SendScreen, params string[] args)
        {
            MsgStringPacket packet;
            packet = new MsgStringPacket
            {
                ID = id,
                UID = UID,
                Strings = args
            };
            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true, true, ShowGemEffects);
            else if (ShowGemEffects)
            {
                Owner.Send(stream.StringPacketCreate(packet));
            }
        }

        public void SendString(Packet stream, MsgStringPacket.StringID id, uint _uid, bool SendScreen, params string[] args)
        {
            MsgStringPacket packet;
            packet = new MsgStringPacket
            {
                ID = id,
                UID = _uid,
                Strings = args
            };
            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }

        public void AddMapEffect(Packet stream, ushort x, ushort y, params string[] effect)
        {
            MsgStringPacket packet;
            packet = new MsgStringPacket
            {
                ID = MsgStringPacket.StringID.LocationEffect,
                X = x,
                Y = y,
                Strings = effect
            };
            View.SendView(stream.StringPacketCreate(packet), true);
        }

        public void ClearItemsSpawn()
        {
            HeadId = (GarmentId = (ArmorId = (LeftWeaponId = (RightWeaponId = (LeftWeaponAccessoryId = (RightWeaponAccessoryId = (SteedId = (MountArmorId = 0u))))))));
            ColorArmor = (ColorShield = (ColorHelment = 0));
            SteedPlus = (SteedColor = 0u);
            HeadSoul = (ArmorSoul = (LeftWeapsonSoul = (RightWeapsonSoul = 0u)));
        }

        public Packet GetArray(Packet stream, bool WindowsView)
        {
            stream.InitWriter();
            stream.Write(Mesh);
            stream.Write(UID);
            stream.Write(GuildID);
            stream.Write((ushort)GuildRank);
            stream.ZeroFill(4);
            for (int x = 0; x < BitVector.bits.Length; x++)
            {
                stream.Write(BitVector.bits[x]);
            }
            stream.Write((ushort)AparenceType);
            stream.Write(HeadId);
            if (Owner.Equipment.TempGarment != null)
            {
                stream.Write(Owner.Equipment.TempGarment.ITEM_ID);
            }
            else stream.Write(GarmentId);
            stream.Write(ArmorId);
            if (Owner.Equipment.TempLeftWeapon != null)
                stream.Write(Owner.Equipment.TempLeftWeapon.ITEM_ID);
            else stream.Write(LeftWeaponId);
            if (Owner.Equipment.TempRightWeapon != null)
                stream.Write(Owner.Equipment.TempRightWeapon.ITEM_ID);
            else stream.Write(RightWeaponId);
            stream.Write(LeftWeaponAccessoryId);
            stream.Write(RightWeaponAccessoryId);
            stream.Write(SteedId);
            stream.Write(MountArmorId);
            stream.Write(0u);
            stream.Write(HitPoints);
            stream.Write(Hair);
            stream.Write(X);
            stream.Write(Y);
            stream.Write((byte)Angle);
            stream.Write((ushort)Action);
            stream.Write((byte)0);
            stream.Write(0);
            stream.Write(Reborn);
            stream.Write(Level);
            stream.Write((byte)(WindowsView ? 1u : 0u));
            stream.Write(Away);
            stream.Write(ExtraBattlePower);
            stream.Write((ushort)0);
            stream.Write(0);
            stream.Write((ushort)0);
            stream.Write(FlowerRank + 10000);
            stream.Write((uint)NobilityRank);
            stream.Write(ColorArmor);
            stream.Write(ColorShield);
            stream.Write(ColorHelment);
            stream.Write(QuizPoints);
            stream.Write(SteedPlus);
            stream.Write((ushort)0);
            stream.Write(SteedColor);
            stream.Write((byte)Enilghten);
            stream.Write((ushort)0);
            stream.Write(0);
            stream.Write(0);
            stream.Write((byte)0);
            stream.Write(ClanUID);
            stream.Write((uint)ClanRank);
            stream.Write(0u);
            stream.Write((ushort)MyTitle);
            stream.Write(0u);
            stream.Write(0u);
            stream.Write(0u);
            stream.Write((byte)0);
            stream.Write(HeadSoul);
            stream.Write(ArmorSoul);
            stream.Write(LeftWeapsonSoul);
            stream.Write(RightWeapsonSoul);
            stream.Write(0);
            stream.Write(0);
            stream.ZeroFill(1);
            stream.Write((ushort)FirstClass);
            stream.Write((ushort)SecondClass);
            stream.Write((ushort)Class);
            if (Owner.Team != null)
                stream.Write(Owner.Team.UID);
            else
                stream.Write(0);
            stream.ZeroFill(1);
            stream.Write(Name, string.Empty, ClanName, string.Empty, string.Empty, (MyGuild != null) ? MyGuild.GuildName : string.Empty, string.Empty);
            stream.Finalize(10014);
            return stream;
        }

        public uint GetShareBattlePowers(uint target_battlepower)
        {
            return (uint)TutorInfo.ShareBattle(Owner, (int)target_battlepower);
        }

        internal string AgatesString()
        {
            string Agates;
            Agates = "";
            foreach (MsgGameItem item2 in Owner.Inventory.ClientItems.Values.Where((MsgGameItem e) => e.ITEM_ID == 720828))
            {
                Agates = Agates + item2.UID + "#";
                foreach (string coord2 in item2.Agate_map.Values)
                {
                    Agates = Agates + coord2 + "#";
                }
                Agates += "$";
            }
            foreach (ConcurrentDictionary<uint, MsgGameItem> wh in Owner.Warehouse.ClientItems.Values)
            {
                foreach (MsgGameItem item in wh.Values.Where((MsgGameItem e) => e.ITEM_ID == 720828))
                {
                    Agates = Agates + item.UID + "#";
                    foreach (string coord in item.Agate_map.Values)
                    {
                        Agates = Agates + coord + "#";
                    }
                    Agates += "$";
                }
            }
            return Agates;
        }

        internal void LoadAgates(string str)
        {
            if (!(str != ""))
                return;
            string[] allAgates;
            allAgates = str.Split(new string[1] { "$" }, StringSplitOptions.RemoveEmptyEntries);
            string[] array;
            array = allAgates;
            foreach (string item in array)
            {
                string[] agate;
                agate = item.Split('#');
                uint Uid;
                Uid = uint.Parse(agate[0]);
                string[] agate_data;
                agate_data = item.Replace(Uid + "#", "").Split(new string[1] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                if (Owner.Inventory.ClientItems.TryGetValue(Uid, out var itemc))
                {
                    int key2;
                    key2 = 0;
                    string[] array2;
                    array2 = agate_data;
                    foreach (string agate_item2 in array2)
                    {
                        itemc.Agate_map.Add((uint)key2++, agate_item2);
                    }
                }
                foreach (ConcurrentDictionary<uint, MsgGameItem> wh in Owner.Warehouse.ClientItems.Values)
                {
                    if (wh.TryGetValue(Uid, out itemc))
                    {
                        int key;
                        key = 0;
                        string[] array3;
                        array3 = agate_data;
                        foreach (string agate_item in array3)
                        {
                            itemc.Agate_map.Add((uint)key++, agate_item);
                        }
                    }
                }
            }
        }
        internal void SolveCaptcha()
        {
            Owner.MobsKilled = 0;
            WaitingKillCaptcha = false;
            KillCountCaptcha = "";
            LastSuccessCaptcha = DateTime.Now;
            NextCaptcha = Role.Core.Random.Next(15, 30);
        }
    }
}