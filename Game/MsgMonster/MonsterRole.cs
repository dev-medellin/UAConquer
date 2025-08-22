using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgFloorItem;
//using TheChosenProject.MsgInterServer.Packets;
using TheChosenProject.Game.MsgNpc;
using System.Collections.Concurrent;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer.AttackHandler;
using TheChosenProject.Role;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgAutoHunting;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;
using Extensions;
using static DevExpress.XtraEditors.Filtering.DataItemsExtension;
using static DevExpress.Data.Filtering.Helpers.SubExprHelper;
using TheChosenProject.Mobs;
using static TheChosenProject.Game.MsgTournaments.ITournamentsAlive;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Google.Protobuf.WellKnownTypes;
using static MongoDB.Driver.WriteConcern;

namespace TheChosenProject.Game.MsgMonster
{
    public class MonsterRole : IMapObj
    {
        public bool isTargetToSolomon = false;

        public class ScoreBoard
        {
            public string Name;

            public uint ScoreDmg;
        }
        public Mobs.Base MonsterBase = null;

        public Dictionary<uint, ScoreBoard> Scores = new Dictionary<uint, ScoreBoard>();

        public int StampFloorSeconds;

        public static List<uint> SpecialMonsters = new List<uint> { 20060 , 20160 , 20300 , 20070 , 6643, 9111, 3102,20070, 4145, 3130, 3134, 20160, 20060, 19890, 19891, 19892, 19893, 19894, 6699, 6688, 19896, 19897, 19898, 19899, 3156, };

        public MagicType.Magic DBSpell;

        public bool IsFloor;

        public MsgItemPacket FloorPacket;

        public bool BlackSpot;

        public string pluItem;

        public Time32 Stamp_BlackSpot;

        public byte PoisonLevel;

        private Time32 DeadStamp;

        private Time32 FadeAway;

        public Time32 RespawnStamp;

        public Time32 MoveStamp;

        public MonsterFamily Family;

        public MonsterView View;

        public MobStatus State;

        public Player Target;

        public Time32 AttackSpeed;

        public StatusFlagsBigVector32 BitVector;

        public string Name = "";

        public byte Boss;

        public uint Mesh;

        public byte Level;

        public uint HitPoints;

        public ushort RespawnX;

        public ushort RespawnY;

        public ushort PX;

        public ushort PY;

        public ushort _xx;

        public ushort _yy;

        public Flags.ConquerAction Action;

        public Flags.ConquerAngle Facing = Flags.ConquerAngle.East;

        public string LocationSpawn = "";

        public GameMap GMap;

        public bool RemoveOnDead;

        private DateTime LastScore;

        public bool InLine { get; set; }

        public int ExtraDamage => Family.extra_damage;

        public int BattlePower => Family.extra_battlelev;

        public bool AllowDynamic { get; set; }

        public uint IndexInScreen { get; set; }

        public int SizeAdd => Family.AttackRange;

        public Position Position => new Position((int)Map, X, Y);

        public uint Map { get; set; }

        public uint DynamicID { get; set; }

        public MapObjectType ObjType { get; set; }

        public uint UID { get; set; }

        public ushort X
        {
            get
            {
                return _xx;
            }
            set
            {
                PX = _xx;
                _xx = value;
            }
        }

        public ushort Y
        {
            get
            {
                return _yy;
            }
            set
            {
                PY = _yy;
                _yy = value;
            }
        }

        public bool Alive => HitPoints != 0;

        public bool Taken_Selected { get; internal set; }

        public bool IsTrap()
        {
            return false;
        }

        public bool CanRespawn(GameMap map)
        {

            Time32 Now;
            Now = Time32.Now;
            if (Now > RespawnStamp && !map.MonsterOnTile(RespawnX, RespawnY))
                return true;
            return false;
        }

        public unsafe void Respawn(bool SendEffect = true)
        {
            using (RecycledPacket rev = new RecycledPacket())
            {
                Packet stream;
                stream = rev.GetStream();

                ClearFlags();
                HitPoints = (uint)Family.MaxHealth;
                State = MobStatus.Idle;
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = UID;
                actionQuery.Type = ActionType.RemoveEntity;
                ActionQuery action;
                action = actionQuery;
                Send(stream.ActionCreate(&action));
                Send(GetArray(stream, false));
                if (SendEffect)
                {
                    action.Type = ActionType.ReviveMonster;
                    Send(stream.ActionCreate(&action));
                }
                if (Family.MaxHealth > 65535)
                {
                    MsgUpdate Upd;
                    Upd = new MsgUpdate(stream, UID, 2u);
                    stream = Upd.Append(stream, MsgUpdate.DataType.MaxHitpoints, Family.MaxHealth);
                    stream = Upd.Append(stream, MsgUpdate.DataType.Hitpoints, HitPoints);
                    Send(Upd.GetArray(stream));
                }
            }
        }

        public void SendSysMesage(string Messaj, MsgMessage.ChatMode ChatType = MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor color = MsgMessage.MsgColor.white)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                Program.SendGlobalPackets.Enqueue(new MsgMessage(Messaj, MsgMessage.MsgColor.white, ChatType).GetArray(stream));
            }
        }

        public void SendBossSysMesage(string KillerName, int StudyPoints, MsgMessage.ChatMode ChatType = MsgMessage.ChatMode.Center, MsgMessage.MsgColor color = MsgMessage.MsgColor.red)
        {
            SendSysMesage("The " + Name.ToString() + " has been destroyed by the team " + KillerName.ToString() + "`s ! All team members received " + StudyPoints + " Study Points!", ChatType, color);
        }

        public unsafe void Dead(Packet stream, GameClient killer, uint aUID, GameMap GameMap, bool CounterKill = false, uint BossID = 0u)
        {
            if (!Alive)
                return;
            if (IsFloor)
            {
                FloorPacket.DropType = MsgDropID.RemoveEffect;
                HitPoints = 0u;
                GameMap.SetMonsterOnTile(X, Y, false);
                return;
            }
            if (Map == 1700)
                RespawnStamp = Time32.Now.AddSeconds(8 + Family.RespawnTime);
            else
                RespawnStamp = Time32.Now.AddSeconds(6 + Family.RespawnTime);
            if (BlackSpot)
            {
                Send(stream.BlackspotCreate(false, UID));
                BlackSpot = false;
            }
            ClearFlags();
            HitPoints = 0u;
            if (killer != null)
                killer.OnAutoAttack = false;
            AddFlag(MsgUpdate.Flags.Dead, 2592000, true);
            DeadStamp = Time32.Now;
            var Pet = Database.Server.GamePoll.Values.Where(p => p != null && p.Pet != null && p.Pet.monster.UID == aUID).FirstOrDefault();
            if (Pet != null)
                Pet.Pet.DeAtach(stream);//bahaa
            InteractQuery interactQuery;
            interactQuery = default(InteractQuery);
            interactQuery.UID = aUID;
            interactQuery.KilledMonster = true;
            interactQuery.X = X;
            interactQuery.Y = Y;
            interactQuery.AtkType = MsgAttackPacket.AttackID.Death;
            interactQuery.OpponentUID = UID;
            InteractQuery action;
            action = interactQuery;
            if (killer != null && killer.AIType == AIEnum.AIType.NotActive && !CounterKill)
            {
                if (killer.DemonExterminator != null)
                    killer.DemonExterminator.UppdateJar(killer, Family.ID);
                if (killer.Player.QuestGUI.CheckQuest(20198, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                {
                    if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_300K_MOBS, 300000u))
                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_300K_MOBS, 1u);
                    else
                        killer.SendSysMesage("You`ve~killed~300,000~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                }
                if (killer.Player.QuestGUI.CheckQuest(20199, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                {
                    if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_100K_MOBS, 100000u))
                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_100K_MOBS, 1u);
                    else
                        killer.SendSysMesage("You`ve~killed~100,000~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                }
                //if (Core.Rate(ServerKernel.CHANCE_LETTERS) && killer.Player.ConquerLetter < ServerKernel.MAXIMUM_LETTER_DAILY_TIMES)
                //{
                //    uint amount3;
                //    amount3 = (uint)Core.RandFromGivingNums(711214, 711215, 711216, 711217, 711218, 711219, 711220);
                //    uint ItemID10;
                //    ItemID10 = amount3;
                //    DropItemID(killer, ItemID10, stream, 3);
                //}
                //if (Core.Rate(ServerKernel.CHANCE_GEMS))
                //{
                //    uint[] Items;
                //    Items = new uint[8] { 700011, 700021,  700041, 700061, 700001, 700031, 700071, 700051 };
                //    uint ItemID9;
                //    ItemID9 = Items[ServerKernel.NextAsync(0, Items.Length)];
                //    DropItemID(killer, ItemID9, stream, 3);
                //}
                //if (Core.Rate(ServerKernel.CHANCE_STONE_ONE_ITEM))
                //{
                //    uint ItemID8;
                //    ItemID8 = 730001;
                //    DropItemID(killer, ItemID8, stream, 3);
                //}
                //if (Core.Rate(ServerKernel.CHANCE_GEMS))
                //{
                //    uint ItemID8;
                //    ItemID8 = 722384;
                //    DropItemID(killer, ItemID8, stream, 3);
                //}
                //if (Core.Rate(ServerKernel.CHANCE_STONE_TWO_ITEM))
                //{
                //    uint ItemID7;
                //    ItemID7 = 730002;
                //    DropItemID(killer, ItemID7, stream, 3);
                //    //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Hell Yeah   " + killer.Player.Name + " he/she found Stone +2 ", "ALLUSERS", "[Drop]", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                //}

                //if (Core.Rate(ServerKernel.CHANCE_PLUS_TWO))
                //{
                //    uint amount2;
                //    amount2 = 727384;
                //    uint ItemID5;
                //    ItemID5 = amount2;
                //    DropItemID(killer, ItemID5, stream, 3);
                //}
                //if (Role.Core.ChanceSuccess(ServerKernel.CHANCE_PLUS_ONE))
                //{
                //    uint ItemID4;
                //    ItemID4 = 727385;
                //    DropItemID(killer, ItemID4, stream, 3);
                //    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("As Lucky  " + killer.Player.Name + " he/she found +1 Coupon", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                //}
                if (Family.ID != 8415)
                {
                    killer.CountItemsMobs++;
                }
                //if (1 > 0)
                //{
                //    killer.CountItemsMobs = 0;
                //    uint amount;
                //    amount = (uint)Core.RandFromGivingNums(727385, 727385);
                //    uint ItemIDPlus;
                //    ItemIDPlus = amount;
                //    if (Role.Core.ChanceSuccess((killer.Player.VipLevel == 6 ? ServerKernel.CHANCE_PLUS_ONE + 0.1 : ServerKernel.CHANCE_PLUS_ONE)))//+1rate
                //    {
                //        DropItemID(killer, ItemIDPlus, stream, 1);
                //        //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Lucky  " + killer.Player.Name + " has found +1StoneCoupon #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                //        //killer.SendSysMesage("As Lucky  " + killer.Player.Name + " he/she found +1 Coupon" + "Kill " + killer.MaxCountItemMobs, MsgMessage.ChatMode.BroadcastMessage, MsgMessage.MsgColor.white, false);
                //        //Program.DiscordLevelAPI.Enqueue("As Lucky  " + killer.Player.Name + " he/she found +1 Coupon" + "Kill " + killer.MaxCountItemMobs);
                //    }
                //    killer.MaxCountItemMobs = Core.Random.Next(800, 900); //123 -> percentage 0.5 
                //}
                //if (Role.Core.ChanceSuccess(ServerKernel.CHANCE_DRAGONBALL_ITEM))//DragonBall
                //{
                //    uint ItemID6;
                //    ItemID6 = 1088000;
                //    DropItemID(killer, ItemID6, stream, 1);
                //    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("As Lucky  " + killer.Player.Name + " he/she found DragonBall #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                //}
                //if (Family.ID == 0015 || Family.ID == 0074)
                //{
                //    killer.EmeraldMobs++;
                //}
                //if (killer.EmeraldMobs > killer.MaxEmeraldMobs)
                //{
                //    killer.EmeraldMobs = 0;
                //    uint amount;
                //    amount = (uint)Core.RandFromGivingNums(1080001, 1080001); //Emerald
                //    uint ItemIDB;
                //    ItemIDB = amount;
                //    if (Role.Core.ChanceSuccess((killer.Player.VipLevel == 6 ? 70 : 50))) //8 : 6
                //    {
                //        DropItemID(killer, ItemIDB, stream, 1);
                //        //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Lucky  " + killer.Player.Name + " has found Emerald #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                //    }
                //    killer.MaxEmeraldMobs = (int)Program.GetRandom.Next(700, 1000); //100, 110
                //}
                //if (Family.ID == 0058 || Family.ID == 0083)
                //{
                //    killer.HWMobs++;
                //}
                //if (killer.HWMobs > killer.MaxHWMobs)
                //{
                //    killer.HWMobs = 0;
                //    uint amount;
                //    amount = (uint)Core.RandFromGivingNums(723030, 723030); //HealthWine
                //    uint ItemIDB;
                //    ItemIDB = amount;
                //    if (Role.Core.ChanceSuccess((killer.Player.VipLevel == 6 ? 40 : 30))) //8 : 6
                //    {
                //        DropItemID(killer, ItemIDB, stream, 1);
                //        //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Lucky  " + killer.Player.Name + " has found HealthWine #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                //    }
                //    killer.MaxHWMobs = (int)Program.GetRandom.Next(700, 1000);
                //}
                if (Family.ID != 8415)
                {
                    killer.DBMobs++;
                }
                if (1 > 0)
                {
                    killer.DBMobs = 0;
                    uint amount;
                    amount = (uint)Core.RandFromGivingNums(1088000, 1088000);
                    uint ItemIDB;
                    ItemIDB = amount;
                    if (Role.Core.ChanceSuccess((killer.Player.VipLevel == 6 ? ServerKernel.CHANCE_DRAGONBALL_ITEM + 0.1 : ServerKernel.CHANCE_DRAGONBALL_ITEM)))//DragonBall
                    {
                        DropItemID(killer, ItemIDB, stream, 1);
                        //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("As Lucky  " + killer.Player.Name + " he/she found DragonBall #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                    }
                    killer.MaxDBMobs = Core.Random.Next(500, 650); //DBRateDrop
                }
                if (Family.ID != 8415)
                {
                    killer.MetsMobs++;
                }
                if (killer.MetsMobs > killer.MaxMetsMobs)
                {
                    killer.MetsMobs = 0;
                    uint amount;
                    amount = (uint)Core.RandFromGivingNums(1088001, 1088001);
                    uint ItemIDMets;
                    ItemIDMets = amount;
                    if (Role.Core.ChanceSuccess((killer.Player.VipLevel == 6 ? ServerKernel.CHANCE_METEOR + 1 : ServerKernel.CHANCE_METEOR)))
                    {
                        DropItemID(killer, ItemIDMets, stream, 1);
                        //Program.DiscordLevelAPI.Enqueue("As Lucky  " + killer.Player.Name + " he/she found Meteor");
                        //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("As Lucky  " + killer.Player.Name + " he/she found Meteor #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                    }
                    killer.MaxMetsMobs = Core.Random.Next(500, 700); //MetsRateDrop
                }
                //if (Role.Core.ChanceSuccess(ServerKernel.METEOR_CHANCE))
                //    DropItemID(killer, 1088001, stream, 1);//ExpBall(Event)
                if(killer.Player.Map != 1002)
                {
                    killer.ProfTokenMobs++;
                }
                if (killer.ProfTokenMobs > killer.MaxProfTokenMobs)
                {
                    killer.ProfTokenMobs = 0;
                    if (Core.Rate(ServerKernel.CHANCE_Key))
                    {
                        DropItemID(killer, 722384, stream, 1);//boxkey
                    }
                    killer.MaxProfTokenMobs = Core.Random.Next(500, 700); //MetsRateDrop
                }
                #region AncientDevil
                if (Map == 1082)
                {
                    #region TrojanGuard
                    if (Family.ID == 9000)
                    {
                        if (Role.Core.ChanceSuccess(50))
                            DropItemID(killer, 710017, stream);
                    }

                    #endregion
                    #region WarriorGuard
                    if (Family.ID == 9001)
                    {
                        if (Role.Core.ChanceSuccess(50))

                            DropItemID(killer, 710016, stream);
                    }

                    #endregion
                    #region ArcherGuard
                    if (Family.ID == 9002)
                    {
                        if (Role.Core.ChanceSuccess(50))

                            DropItemID(killer, 710020, stream);
                    }

                    #endregion
                    #region WaterGuard
                    if (Family.ID == 9004)
                    {
                        if (Role.Core.ChanceSuccess(50))

                            DropItemID(killer, 710019, stream);
                    }

                    #endregion
                    #region FireGuard
                    if (Family.ID == 9007)
                    {
                        if (Role.Core.ChanceSuccess(50))

                            DropItemID(killer, 710018, stream);
                    }

                    #endregion
                    #region Devil
                    if (Family.ID == 9111)
                    {
                        Game.MsgTournaments.MsgSchedules.SpawnDevil = false;
                        if (Role.Core.Rate(15))
                        {
                            DropItemNull(721330, stream);
                        }
                        if (Role.Core.Rate(15))
                        {
                            // DropItemID(killer, 721331, stream);
                            DropItemNull(721331, stream);

                        }
                        if (Role.Core.Rate(15))
                        {
                            //     DropItemID(killer, 721332, stream);
                            DropItemNull(721332, stream);

                        }


                        if (Role.Core.Rate(5))//dances book
                        {
                            //   DropItemID(killer, (uint)(725018 + Program.GetRandom.Next(1, 7)), stream);
                            DropItemNull((uint)(725018 + Program.GetRandom.Next(1, 7)), stream);
                        }
                        if (Role.Core.Rate(100))
                        {
                            uint amount = (uint)Program.GetRandom.Next(50000);
                            var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                            for (ushort i = 0; i < 10; i++)
                                DropItemNull(ItemID, stream, MsgItem.ItemType.Money, amount);
                        }
                        else
                        {
                            for (ushort i = 0; i < 10; i++)
                                DropItemNull(Database.ItemType.Meteor, stream);

                            DropItemNull(Database.ItemType.DragonBall, stream);
                        }

                    }
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The "+Family.Name+" killed by "+killer.Name+ " and Drop [Special Items 50% change.]", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    #endregion
                }
                #endregion
                //#region MeteorDove
                //if (Map == 1210)
                //{
                //    if (Family.ID == 8415)
                //    {
                //        if (Role.Core.Rate(0.001))
                //        {
                //            DropItemID(killer, Database.ItemType.DragonBall, stream);
                //            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("A Dragonball was dropped by the meteordove monster by! " + killer.Player.Name + ".", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                //        }
                //        else if (Role.Core.Rate(20))
                //        {
                //            DropItemID(killer, Database.ItemType.Meteor, stream);
                //        }
                //        else if (Role.Core.Rate(10))
                //        {
                //            for (int i = 0; i < 2; i++)
                //                DropItemID(killer, Database.ItemType.Meteor, stream);
                //        }
                //        else if (Role.Core.Rate(1))
                //        {
                //            for (int i = 0; i < 5; i++)
                //                DropItemID(killer, Database.ItemType.Meteor, stream);
                //        }
                //        else if (Role.Core.Rate(5))
                //        {
                //            for (int i = 0; i < 3; i++)
                //                DropItemID(killer, Database.ItemType.Meteor, stream);
                //        }

                //    }
                //}
                //#endregion
                #region SkyPass
                if (Map == 1040)
                {
                    if (Family.ID == 3000)
                    {
                        if (Role.Core.Rate(10))
                        {
                            DropItemID(killer, 721100, stream);
                        }
                    }
                    if (Family.ID == 3009)
                    {
                        if (Role.Core.Rate(10))
                        {
                            DropItemID(killer, 721101, stream);
                        }
                    }
                    if (Family.ID == 3014)
                    {
                        if (Role.Core.Rate(15))
                        {
                            DropItemID(killer, 721102, stream);
                        }
                    }
                    if (Family.ID == 3019)
                    {
                        if (Role.Core.Rate(15))
                        {
                            DropItemID(killer, 721103, stream);
                        }
                    }
                    if (Family.ID == 3020)
                    {
                        if (Role.Core.Rate(15))
                        {
                            DropItemID(killer, 721108, stream);
                        }
                    }
                }
                #endregion
                #region LavaBeast
    //                if (Boss > 0 && Family.ID == 20055)
    //                {
    //                    uint[] DropSpecialItems = new uint[] {Database.ItemType.MeteorScroll,Database.ItemType.DragonBallScroll, Database.ItemType.PowerExpBall, Database.ItemType.ExpAmrita };

    //                    ushort xx = X;
    //                    ushort yy = Y;
    //                    if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
    //                    {
    //                        uint IDDrop = DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)];
    //                        DropItem(stream, killer.Player.UID, killer.Map, IDDrop, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);

    //                        string drop_name = Database.Server.ItemsBase[IDDrop].Name;
    //#if Arabic
    //                                     SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                         
    //#else
    //                        SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he drop " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

    //#endif
    //                    }

    //                    uint[] DropSpecialItems2 = new uint[] { 720300, 720301, 720302, 720303, 720304, 720305, 720306 };

    //                    ushort xx2 = X;
    //                    ushort yy2 = Y;
    //                    if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
    //                    {
    //                        uint IDDrop2 = DropSpecialItems2[Program.GetRandom.Next(0, DropSpecialItems2.Length)];
    //                        DropItem(stream, killer.Player.UID, killer.Map, IDDrop2, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
    //                    }
    //                    return;
    //                }
                #endregion
                // Guild Beast 
                if (Family.ID == 3122 && killer.Player.SpawnGuildBeast && Map == 1038)
                {
                    killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e290");
                    MsgSchedules.SendSysMesage($"{killer.Player.Name} he/she kill the GuildBeast and get reward DragonBall");
                    //DropItemID(killer, Database.ItemType.DragonBall, stream);
                    #region Drop = DragonballPiece
                    for (int x = 0; x < 10; x++)
                    {
                        if (x <= 10 || (x > 10 && Role.Core.Rate(45)))
                        {
                            uint id = 710834;//DragonballPiece
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = Dragonball
                    for (int x = 0; x < 2; x++)
                    {
                        if (x <= 2 || (x > 2 && Role.Core.Rate(45)))
                        {
                            uint id = 1088000;//MeteorScroll

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = Meteor
                    for (int x = 0; x < 10; x++)
                    {
                        if (x <= 10 || (x > 10 && Role.Core.Rate(45)))
                        {
                            uint id = 1088001;//Meteor

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    List<uint> list;
                    list = new List<uint>();
                    list.Add(754009);//999Tulips
                    list.Add(753009);//999Orchids
                    list.Add(751009);
                    List<uint> listFlower;
                    listFlower = list;
                    uint reward;
                    #region Drop = Flowers
                    for (int x = 0; x < 3; x++)
                    {
                        if (x <= 3 || (x > 3 && Role.Core.Rate(45)))
                        {
                            reward = listFlower[Core.Random.Next(0, listFlower.Count)];
                            uint id = reward;
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = Gold
                    if (Role.Core.Rate(100))
                    {
                        uint amount = (uint)Program.GetRandom.Next(100000);
                        var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = ItemID;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        for (ushort i = 0; i < 10; i++)
                        {
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        //DropItemNull(ItemID, stream, MsgItem.ItemType.Money, amount);
                    }
                    #endregion
                    killer.Player.SpawnGuildBeast = false;
                    DropItemID(killer, Database.ItemType.DragonBall, stream);
                    MsgSchedules.SendSysMesage($"{killer.Player.Name} he/she kill the {Name} and dropped DragonBall in {killer.Map.Name} #43#43");
                }
                if (Family.ID == 61190 || Family.ID == 3143 || Family.ID == 3146 || Family.ID == 3149 || Family.ID == 31569)
                {
                    if (ServerKernel.ServerManager.MonsterDBSpawn)
                    {
                        DropItemID(killer, Database.ItemType.DragonBall, stream);
                        MsgSchedules.SendSysMesage($"{killer.Player.Name} he/she kill the {Name} and dropped DragonBall in {killer.Map.Name} #43#43");
                    }
                    else
                    {
                        MsgSchedules.SendSysMesage($"{killer.Player.Name} he/she kill the {Name} get nothing better luck next time #31#31");
                    }
                }
                // emerald
                //if (Map == 1000)
                //{
                //    if (Family.ID == 0015)
                //    {
                //        if (Role.Core.Rate(1))
                //        {
                //            DropItemID(killer, 1080001, stream);
                //        }
                //    }
                //}
                //if (Map == 1001)// healing wine
                //{
                //    if (Family.ID == 0058)
                //    {
                //        if (Role.Core.Rate(1))
                //        {
                //            DropItemID(killer, 723030, stream);
                //        }
                //    }
                //}
                int min;
                min = (int)((double)ServerKernel.MONEY_MONSTER_DROP_RATE[0] * 0.4);
                int Money;
                Money = Core.Random.Next(min, ServerKernel.MONEY_MONSTER_DROP_RATE[0]);

                if (killer.Player.Map == 1002 || killer.Player.Map == 1011 || killer.Player.Map == 1020)
                {
                    Money = (int)Program.GetRandom.Next(50, 100);
                }

                if (killer.Player.Map == 1011)
                {
                    DateTime Now64 = DateTime.Now;
                    if (Family.ID == 7 && (Now64.DayOfWeek == DayOfWeek.Saturday || Now64.DayOfWeek == DayOfWeek.Sunday))
                    {
                        if (Role.Core.Rate(1))
                        {
                            DropItemID22(killer, 721263, stream);
                        }
                    }
                }

                #region drop CPS rate with panel
                if (Core.Rate(ServerKernel.CP_MONSTER_DROP_RATE[0]))
                {
                    int minCP;
                    minCP = ServerKernel.CP_MONSTER_DROP_RATE[1];
                    int ConquerPoints;
                    ConquerPoints = (int)minCP;
                    if (killer.Player.VipLevel == 6)
                    {
                        min = (int)((double)ServerKernel.MONEY_MONSTER_DROP_RATE[1] * 0.4);
                        ConquerPoints = (int)ServerKernel.CP_MONSTER_DROP_RATE[1] * 2;
                        Money = Core.Random.Next(min, ServerKernel.MONEY_MONSTER_DROP_RATE[1]);
                    }

                    if (killer.Player.VipLevel == 6)
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            //killer.Player.AutoHuntingCPS += ConquerPoints;
                            killer.Player.Money += Money;
                            killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                        }
                        else
                        {
                            killer.Player.ConquerPoints += ConquerPoints;
                            killer.Player.Money += Money;
                            killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);
                            killer.SendSysMesage($"Congratulations! You have got {ConquerPoints} Conquer Points[VIPRate]!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.white, false);

                        }
                        ServerKernel.TOTAL_CPS_HUNTING += ConquerPoints;
                        ServerKernel.TOTAL_Silver_HUNTING += Money;
                    }

                    else
                    {
                        if (killer.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            killer.Player.AutoHuntingCPS += (int)ConquerPoints;
                            killer.Player.Money += Money;
                            killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);

                        }
                        else
                        {
                            killer.Player.ConquerPoints += ConquerPoints;
                            killer.Player.Money += Money;
                            killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);                            
                            killer.SendSysMesage($"Congratulations! You have got {ConquerPoints} Conquer Points!", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.white, false);
                        }
                        ServerKernel.TOTAL_CPS_HUNTING += ConquerPoints;
                        ServerKernel.TOTAL_Silver_HUNTING += Money;
                    }
                }
                #endregion

                if (killer.Player.AutoHunting != AutoStructures.Mode.Enable && killer.Player.OfflineTraining != MsgOfflineTraining.Mode.Hunting)
                    killer.DbKillMobsExterminator.EnqueueKills(Map);
                if (killer.Player.OnXPSkill() == MsgUpdate.Flags.Cyclone || killer.Player.OnXPSkill() == MsgUpdate.Flags.Superman)
                {
                    killer.Player.XPCount++;
                    killer.Player.KillCounter++;
                    if (killer.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
                    {
                        action.KillCounter = killer.Player.KillCounter;
                        killer.Player.UpdateXpSkill();
                    }
                }
                else if (killer.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                {
                    killer.Player.XPCount++;
                }
            }

            if (InLine)
                InLine = false;
            Send(stream.InteractionCreate(&action));

            if (RemoveOnDead)
            {
                AddFlag(MsgUpdate.Flags.FadeAway, 10, false);
                GMap.View.LeaveMap((IMapObj)this);
                if (GMap.IsFlagPresent(X, Y, MapFlagType.Monster))
                    GMap.cells[X, Y] &= ~MapFlagType.Monster;
            }

            if (BossID != 0 && killer == null)
                BossDatabase.Bosses[Family.ID].Alive = false;

            if (killer == null)
                return;
            #region EasterEggs Quest
                //if (Role.Core.ChanceSuccess(0.218))
                //{
                //    uint ID = 710060 + (uint)(Role.Core.Random.Next(0, 4));
                //    DropItemID(killer, ID, stream);
                //}
                //else if (Role.Core.ChanceSuccess(0.110))
                //{
                //    uint ID = 710065 + (uint)(Role.Core.Random.Next(0, 7));
                //    if (DropItemID2(killer, ID, stream))
                //    {
                //        Database.ItemType.DBItem DBItem;
                //        if (Server.ItemsBase.TryGetValue(ID, out DBItem))
                //        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(""+ killer.Name + " just killed a " + Name + " and it dropped a " + DBItem.Name + "!!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));

                //    }
                //}
            #endregion EasterEggs Quest
            #region Halloween
            //if (killer.PumpkinsMobs > killer.MaxPumpkins)
            //{
            //    if (Role.Core.ChanceSuccess(0.218))
            //    {
            //        DropItemID2(killer, 722176, stream, 3, true);
            //    }
            //}
            //if (killer.PumpkinSeedsMobs > killer.MaxPumpkinSeeds)
            //{
            //     if (Role.Core.ChanceSuccess(0.110))
            //    {
            //        DropItemID2(killer, 710587, stream, 3, true);
            //    }
            //}
            #endregion
            #region Demon box
            //if (killer.MyHouse != null && killer.Player.DynamicID == killer.Player.UID)
            //{
            //    switch (Family.ID)
            //    {
            //        case 2435:
            //            if (Core.Rate(1, 10000))
            //            {
            //                DropItemID(killer, 720679, stream, 3);
            //                killer.CreateBoxDialog("You killed a Heaven Demon and found a Frost CP Pack (69000CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Frost CP Pack (69000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(10, 9999))
            //            {
            //                DropItemID(killer, 720678, stream, 3);
            //                killer.CreateBoxDialog("You killed a Heaven Demon and found a Life CP Pack (13500CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Life CP Pack (13500CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(3700, 9989))
            //            {
            //                DropItemID(killer, 720677, stream, 3);
            //                killer.CreateBoxDialog("You killed a Heaven Demon and found a Blood CP Pack (1000CPs)!");
            //            }
            //            else if (Core.Rate(1289, 6289))
            //            {
            //                DropItemID(killer, 720676, stream, 3);
            //                killer.CreateBoxDialog("You killed a Heaven Demon and found a Soul CP Pack (500CPs)!");
            //            }
            //            else if (Core.Rate(1000, 5000))
            //            {
            //                DropItemID(killer, 720675, stream, 3);
            //                killer.CreateBoxDialog("You killed a Heaven Demon and found a Ghost CP Pack (250CPs)!");
            //            }
            //            else
            //            {
            //                DropItemID(killer, 720680, stream, 3);
            //                killer.CreateBoxDialog("You killed a Heaven Demon and found a Heaven Pill equal to the EXP of 2 and a half EXP Balls!");
            //            }
            //            break;
            //        case 2436:
            //            if (Core.Rate(1, 10000))
            //            {
            //                DropItemID(killer, 720685, stream, 3);
            //                killer.CreateBoxDialog("You killed a Chaos Demon and found a Nimbus CP Pack (138000CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Nimbus CP Pack (138000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(10, 9999))
            //            {
            //                DropItemID(killer, 720684, stream, 3);
            //                killer.CreateBoxDialog("You killed a Chaos Demon and found a Butterfly CP Pack (27000CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Butterfly CP Pack (27000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(3700, 9989))
            //            {
            //                DropItemID(killer, 720683, stream, 3);
            //                killer.CreateBoxDialog("You killed a Chaos Demon and found a Heart CP Pack (2000CPs)!");
            //            }
            //            else if (Core.Rate(1289, 6289))
            //            {
            //                DropItemID(killer, 720682, stream, 3);
            //                killer.CreateBoxDialog("You killed a Chaos Demon and found a Flower CP Pack (1000CPs)!");
            //            }
            //            else if (Core.Rate(1000, 5000))
            //            {
            //                DropItemID(killer, 720681, stream, 3);
            //                killer.CreateBoxDialog("You killed a Chaos Demon and found a Deity CP Pack (500CPs)!");
            //            }
            //            else
            //            {
            //                DropItemID(killer, 720686, stream, 3);
            //                killer.CreateBoxDialog("You killed a Chaos Demon and found a Mystery Pill equal to the EXP of 2 and 1/3 EXP Balls!");
            //            }
            //            break;
            //        case 2437:
            //            if (Core.Rate(1, 10000))
            //            {
            //                DropItemID(killer, 720691, stream, 3);
            //                killer.CreateBoxDialog("You killed a Sacred Demon and found a Kylin CP Pack (276000CPs)");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Kylin CP Pack (276000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(10, 9999))
            //            {
            //                DropItemID(killer, 720690, stream, 3);
            //                killer.CreateBoxDialog("You killed a Sacred Demon and found a Rainbow CP Pack (54000CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Rainbow CP Pack (54000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(3700, 9989))
            //            {
            //                DropItemID(killer, 720689, stream, 3);
            //                killer.CreateBoxDialog("You killed a Sacred Demon and found a Shadow CP Pack (4000CPs)!");
            //            }
            //            else if (Core.Rate(1289, 6289))
            //            {
            //                DropItemID(killer, 720688, stream, 3);
            //                killer.CreateBoxDialog("You killed a Sacred Demon and found a Jewel CP Pack (2000CPs)!");
            //            }
            //            else if (Core.Rate(1000, 5000))
            //            {
            //                DropItemID(killer, 720687, stream, 3);
            //                killer.CreateBoxDialog("You killed a Sacred Demon and found a Cloud CP Pack (1000CPs)!");
            //            }
            //            else
            //            {
            //                DropItemID(killer, 720692, stream, 3);
            //                killer.CreateBoxDialog("You killed a Sacred Demon and found a Wind Pill equal to the EXP of 5 EXP Balls!");
            //            }
            //            break;
            //        case 2438:
            //            if (Core.Rate(1, 10000))
            //            {
            //                DropItemID(killer, 720697, stream, 3);
            //                killer.CreateBoxDialog("You killed an Aurora Demon and found a Pilgrim CP Pack (690000CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " got a Pilgrim CP Pack (690000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(10, 9999))
            //            {
            //                DropItemID(killer, 720696, stream, 3);
            //                killer.CreateBoxDialog("You killed an Aurora Demon and found a Zephyr CP Pack (135000CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Zephyr CP Pack (135000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(3700, 9989))
            //            {
            //                DropItemID(killer, 720695, stream, 3);
            //                killer.CreateBoxDialog("You killed an Aurora Demon and found an Earth CP Pack (10000CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found an Earth CP Pack (10000CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(1289, 6289))
            //            {
            //                DropItemID(killer, 720694, stream, 3);
            //                killer.CreateBoxDialog("You killed an Aurora Demon and found a Moon CP Pack (5000CPs)!");
            //            }
            //            else if (Core.Rate(1000, 5000))
            //            {
            //                DropItemID(killer, 720693, stream, 3);
            //                killer.CreateBoxDialog("You killed an Aurora Demon and found a Fog CP Pack (2500CPs)!");
            //            }
            //            else
            //            {
            //                DropItemID(killer, 720698, stream, 3);
            //                killer.CreateBoxDialog("You killed an Aurora Demon and got a Wind Pill equal to the EXP of 8 and 1/3 EXP Balls!");
            //            }
            //            break;
            //        case 2420:
            //            if (Core.Rate(1, 10000))
            //            {
            //                DropItemID(killer, 720654, stream, 3);
            //                killer.CreateBoxDialog("You killed a Demon and found a Joy CP Pack (1380CPs)!");
            //                Program.SendGlobalPackets.Enqueue(new MsgMessage(killer.Player.Name + " found a Joy CP Pack (1380CPs)!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //            }
            //            else if (Core.Rate(10, 9999))
            //            {
            //                DropItemID(killer, 720653, stream, 3);
            //                killer.CreateBoxDialog("You killed a Demon and found a Dream CP Pack (270CPs)!");
            //            }
            //            else if (Core.Rate(3700, 9989))
            //            {
            //                DropItemID(killer, 720655, stream, 3);
            //                killer.CreateBoxDialog("You killed a Demon and found a Mammon CP Pack (20CPs)!");
            //            }
            //            else if (Core.Rate(1289, 6289))
            //            {
            //                DropItemID(killer, 720656, stream, 3);
            //                killer.CreateBoxDialog("You killed a Demon and found a Mascot CP Pack (10CPs)!");
            //            }
            //            else if (Core.Rate(1000, 5000))
            //            {
            //                DropItemID(killer, 720657, stream, 3);
            //                killer.CreateBoxDialog("You killed a Demon and found a Hope CP Pack (5CPs)!");
            //            }
            //            else
            //            {
            //                DropItemID(killer, 720668, stream, 3);
            //                killer.CreateBoxDialog("You killed a Demon and found a Magic Ball equal to the EXP of 1/6 of an EXP Ball!");
            //            }
            //            break;
            //    }
            //}
            #endregion
            #region DailyQuestEnvoy tasks
            if (killer.AIType == AIEnum.AIType.NotActive)
            {
                if (Family.ID == 1 && killer.Player.QuestGUI.CheckQuest(20191, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                {
                    if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_PHEASANT, 10000))
                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_PHEASANT, 1u);
                    else
                        killer.SendSysMesage("You`ve~killed~100,000~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                }
                if (Family.ID == 58 && killer.Player.QuestGUI.CheckQuest(20192, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                {
                    if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_RED_DEVILES, 30000))
                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_RED_DEVILES, 1u);
                    else
                        killer.SendSysMesage("You`ve~killed~100,000~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                }
                if (Family.ID == 18 && killer.Player.QuestGUI.CheckQuest(20197, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                {
                    if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_BIRD_MAN, 20000))
                        killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_BIRD_MAN, 1u);
                    else
                        killer.SendSysMesage("You`ve~killed~100,000~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                }
            }
            #endregion
            #region SnakeKing
            if (Family.ID == 3102)
            {
                if (killer.Player.DynamicID != killer.Player.UID)
                {

                    #region Drop = Meteor
                    //if (Role.Core.Rate(25))
                    //{
                    //    for (int x = 0; x < 5; x++)
                    //    {
                    //        if (x <= 5 || (x > 5 && Role.Core.Rate(60)))
                    //        {
                    //            uint id = Database.ItemType.Meteor;

                    //            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //            DataItem.ITEM_ID = id;
                    //            Database.ItemType.DBItem DBItem;
                    //            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //            {
                    //                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //            }
                    //            DataItem.Color = Role.Flags.Color.Red;
                    //            ushort xx = (ushort)Program.GetRandom.Next(X - 3, X + 3);
                    //            ushort yy = (ushort)Program.GetRandom.Next(Y - 3, Y + 3);
                    //            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //            {
                    //                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //                if (killer.Map.EnqueueItem(DropItem))
                    //                {
                    //                    uint IDDrop = DataItem.ITEM_ID;
                    //                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //                    string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                    //                    SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                    //                }
                    //            }
                    //        }
                    //    }
                    //    SendSysMesage("The mighty SnakeMan has fallen! It will rise again in 2 hours. Stay vigilant, heroes!", ChatMode.System);
                    //    return;
                    //}
                    #endregion
                    #region Drop = MagicExpball
                    //if (Role.Core.Rate(25))
                    //{
                    //    for (int x = 0; x < 2; x++)
                    //    {
                    //        if (x <= 1 || (x > 1 && Role.Core.Rate(45)))
                    //        {
                    //            uint id = 720668;//MagicBallExp

                    //            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //            DataItem.ITEM_ID = id;
                    //            Database.ItemType.DBItem DBItem;
                    //            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //            {
                    //                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //            }
                    //            DataItem.Color = Role.Flags.Color.Red;
                    //            ushort xx = (ushort)Program.GetRandom.Next(X - 3, X + 3);
                    //            ushort yy = (ushort)Program.GetRandom.Next(Y - 3, Y + 3);
                    //            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //            {
                    //                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //                if (killer.Map.EnqueueItem(DropItem))
                    //                {
                    //                    uint IDDrop = DataItem.ITEM_ID;
                    //                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //                    string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                    //                    SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                    //                }
                    //            }
                    //        }
                    //    }
                    //    SendSysMesage("The mighty SnakeMan has fallen! It will rise again in 2 hours. Stay vigilant, heroes!", ChatMode.System);
                    //    return;
                    //}
                    #endregion
                    #region DragonBall-x1
                        if (Role.Core.Rate(10))
                        {
                            for (int x = 0; x < 1; x++)
                            {
                                if (x <= 1 || (x > 1 && Role.Core.Rate(45)))
                                {
                                    uint id = 1088000;//DragonBall

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 3, X + 3);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 3, Y + 3);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            uint IDDrop = DataItem.ITEM_ID;
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                            string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                                            SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                                        }
                                    }
                                }
                            }
                            SendSysMesage("The mighty SnakeMan has fallen! It will rise again in 2 hours. Stay vigilant, heroes!", ChatMode.System);
                            return;
                        }
                    #endregion
                    #region Drop = PowerExpBall
                    //if (Role.Core.Rate(45))
                    //{
                    //    for (int x = 0; x < 1; x++)
                    //    {
                    //        if (x <= 1)
                    //        {
                    //            uint id = Database.ItemType.PowerExpBall;

                    //            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //            DataItem.ITEM_ID = id;
                    //            Database.ItemType.DBItem DBItem;
                    //            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //            {
                    //                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //            }
                    //            DataItem.Color = Role.Flags.Color.Red;
                    //            ushort xx = (ushort)Program.GetRandom.Next(X - 3, X + 3);
                    //            ushort yy = (ushort)Program.GetRandom.Next(Y - 3, Y + 3);
                    //            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //            {
                    //                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //                if (killer.Map.EnqueueItem(DropItem))
                    //                {
                    //                    uint IDDrop = DataItem.ITEM_ID;
                    //                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //                    string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                    //                    SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region Drop = MegaMetsPack
                    //else if (Role.Core.Rate(43))
                    //{
                    //    for (int x = 0; x < 1; x++)
                    //    {
                    //        if (x <= 5 || (x > 1 && Role.Core.Rate(45)))
                    //        {
                    //            uint id = 720547;//MegaMetsPack

                    //            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //            DataItem.ITEM_ID = id;
                    //            Database.ItemType.DBItem DBItem;
                    //            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //            {
                    //                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //            }
                    //            DataItem.Color = Role.Flags.Color.Red;
                    //            ushort xx = (ushort)Program.GetRandom.Next(X - 3, X + 3);
                    //            ushort yy = (ushort)Program.GetRandom.Next(Y - 3, Y + 3);
                    //            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //            {
                    //                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //                if (killer.Map.EnqueueItem(DropItem))
                    //                {
                    //                    uint IDDrop = DataItem.ITEM_ID;
                    //                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //                    string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                    //                    SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region DragonBallScrap
                        if (Role.Core.Rate(25))
                        {
                            for (int x = 0; x < 3; x++)
                            {
                                if (x <= 3 || (x > 3 && Role.Core.Rate(45)))
                                {
                                    uint id = 710834;

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 3, X + 3);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 3, Y + 3);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            uint IDDrop = DataItem.ITEM_ID;
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                            string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                                            SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                                        }
                                    }
                                }
                            }
                            SendSysMesage("The mighty SnakeMan has fallen! It will rise again in 2 hours. Stay vigilant, heroes!", ChatMode.System);
                            return;
                        }
                    #endregion
                    else
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            if (x <= 5 || (x > 5 && Role.Core.Rate(60)))
                            {
                                uint id = Database.ItemType.Meteor;

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 3, X + 3);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 3, Y + 3);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {
                                        uint IDDrop = DataItem.ITEM_ID;
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                                        SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

                                    }
                                }
                            }
                        }
                        //#region Drop = Gold
                        //if (Role.Core.Rate(100))
                        //{
                        //    uint amount = (uint)Program.GetRandom.Next(50000, 80000);
                        //    var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                        //    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        //    DataItem.ITEM_ID = ItemID;
                        //    Database.ItemType.DBItem DBItem;
                        //    if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        //    {
                        //        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        //    }
                        //    DataItem.Color = Role.Flags.Color.Red;
                        //    for (ushort i = 0; i < 1; i++)
                        //    {
                        //        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                        //        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                        //        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        //        {
                        //            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx), (ushort)(yy), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                        //            if (killer.Map.EnqueueItem(DropItem))
                        //            {
                        //                SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! and he get  " + amount + " amount of GOLD! ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                        //                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                        //            }
                        //        }
                        //    }
                        //    //DropItemNull(ItemID, stream, MsgItem.ItemType.Money, amount);
                        //}
                        //#endregion 
                    }
                    SendSysMesage("The mighty SnakeMan has fallen! It will rise again in 2 hours. Stay vigilant, heroes!", ChatMode.System);
                    Game.MsgTournaments.MsgSchedules.SnakeManLastDeathTime = DateTime.Now;
                }


                return;
            }

            #endregion
            #region WaterLord

            if (Family.ID == 8500 && Map == 1212)
            {
                if (Role.Core.Rate(75))
                {
                    uint[] DropSpecialItems = new uint[] { Database.ItemType.CleanWater};

                    ushort xx = X;
                    ushort yy = Y;
                    if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
                    {
                        uint IDDrop = DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)];
                        DropItem(stream, killer.Player.UID, killer.Map, DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)], xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);

                        string drop_name = Database.Server.ItemsBase[IDDrop].Name;
#if Arabic
                                     SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                         
#else
                        SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                        //Program.DiscordWaterLord.Enqueue("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + ". ");
#endif
                    }
                }
                else
                {
                    SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! no drop better luck next time #35 ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                    //Program.DiscordWaterLord.Enqueue("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! no drop better luck next time. ");
                }
            }

            #endregion WaterLord
            #region ganoderma
            if (Map == 1011)
            {
                if (Family.ID == 3130)//titan/ ganoderma
                {
                    if (Role.Core.Rate(50))
                    {
                        uint[] DropSpecialItems = new uint[] { Database.ItemType.ExperiencePotion, Database.ItemType.ExpAmrita, 710834 };

                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
                        {
                            
                            uint IDDrop = DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)];
                            DropItem(stream, killer.Player.UID, killer.Map, IDDrop, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);

                            string drop_name = Database.Server.ItemsBase[IDDrop].Name;
#if Arabic
                                     SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                         
#else
                            SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

#endif
                        }

                    }
                    if (Family.Mesh == 133 && killer.Player.QuestGUI.CheckQuest(20196, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {
                        if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_GANDORMA_BOSS, 10))
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_GANDORMA_BOSS, 1);
                        else
                            killer.CreateBoxDialog("You`ve~killed~50~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                    }
                }
            }
            #endregion
            #region Titan & ganoderma
            if (Map == 1020)
            {
                if (Family.ID == 3134)//titan/ ganoderma
                {
                    if (Role.Core.Rate(50))
                    {
                        uint[] DropSpecialItems = new uint[] { Database.ItemType.ExperiencePotion, Database.ItemType.ExpAmrita, 710834 };

                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
                        {
                            uint IDDrop = DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)];
                            DropItem(stream, killer.Player.UID, killer.Map, IDDrop, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);

                            string drop_name = Database.Server.ItemsBase[IDDrop].Name;
#if Arabic
                                     SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);
                         
#else
                            SendSysMesage("The " + Name.ToString() + " has been destroyed by the " + killer.Player.Name.ToString() + " ! he received one " + drop_name + " ", Game.MsgServer.MsgMessage.ChatMode.Center, MsgMessage.MsgColor.red);

#endif
                        }

                    }
                    if (Family.Mesh == 153 && killer.Player.QuestGUI.CheckQuest(20194, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {
                        if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_TITAN_BOSS, 10))
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_TITAN_BOSS, 1);
                        else
                            killer.CreateBoxDialog("You`ve~killed~50~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                    }
                }
            }
            #endregion
            #region BossDomination
            if (Family.ID == 6699)
            {
                if (killer.Player.DynamicID != killer.Player.UID)
                {
                    #region Small Boss [BossDomination] - drop Minotaurs


                    for (int x = 0; x < 5; x++)
                    {
                        uint id = 780010;//VIPCard~L6[1-Hour]
                        if (x == 0)
                            id = 720027;//PowerExpBall
                        if (x == 1)
                            id = 720027;//+2 Stone
                        if (x == 2)
                            id = 720027;//+2 Stone
                        if (x == 3)
                            id = 720027;//+2 Stone
                        if (x == 4)
                            id = 723725;//LifeFruitBasket
                        if (x == 5)
                            id = 720676;//LifeFruitBasket

                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                        string MSG;
                        MSG = "Congratulation to " + killer.Player.Name + " ! he/she Kill Minotaurs BossDomination and drop [3x+2Stone, 1xPowerExpball, LifeFruitBasket, 1H-VIP]!";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));

                    }

                    #endregion
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        var GMap = Database.Server.ServerMaps[3020];
                        var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p =>
                        (p as MonsterRole).Family.ID == 6699 && p.Alive);
                        if (mobs.Count() == 0)
                        {
                            if (!GMap.ContainMobID(6688))
                            {
                                Database.Server.AddMapMonster(stream2, GMap, 6688, 51, 50, 1, 1, 1);
                                string MSG;
                                MSG = "Gravebeast at BossDomination has spawned join the event and, kill them!";
                                Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));

                            }
                        }
                    }

                }
                return;
            }
            
            #region Big Boss [BossDomination]
                if (Family.ID == 6688)
                {
                    if (killer.Player.DynamicID != killer.Player.UID)
                    {

                        for (int x = 0; x < 9; x++)
                        {
                            uint id = 727385;//+1StoneCoupon
                            if (x == 0)
                                id = 727385;//+1StoneCoupon
                            if (x == 1)
                                id = 727385;//+1StoneCoupon
                            if (x == 2)
                                id = 727385;//+1StoneCoupon
                            if (x == 3)
                                id = 727385;//+1StoneCoupon
                            if (x == 4)
                                id = 727385;//+1StoneCoupon
                            if (x == 5)
                                id = 780010;//1H-VIP
                            if (x == 6)
                                id = 720393;//2H-x2DoubleEXP
                            if (x == 7)
                                id = 720393;//UpToken
                            if (x == 8)
                                id = 720393;//UpToken
                            if (x == 8)
                                id = 720393;//UpToken

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                            string MSG;
                            MSG = "Congratulation to " + killer.Player.Name + " ! he/she Kill Gravebeast at BossDomination and drop [+1StoneCoupons,1H-VIP,GoldBullion]!";
                            Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));

                        }

                    }
                    return;
                }
            #endregion
            #endregion
            #region ExpMob drop
            //if (Family.ID == 21060)
            //{
            //    //if (killer.Player.TaskExpMob != 1)
            //    //{
            //    //    killer.Player.PassPoints += 500;
            //    //    killer.Player.TaskExpMob += 1;
            //    //}
            //    killer.Player.ConquerPoints += 1000;
            //    killer.Player.Money += 5000000;
            //    killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);

            //    if (killer.Inventory.HaveSpace(5))
            //    {
            //        killer.Inventory.Add(stream, 720757, 5, 0);

            //    }
            //    else
            //    {
            //        killer.Inventory.AddReturnedItem(stream, 720757, 5);
            //    }
            //    //DistributeBossPoints();

            //    SendSysMesage(killer.Player.Name + " killed EXP Mob and received [5]ExpBallPack, 5,000,000 Silver and 1000 ConquerPoints!.", MsgServer.MsgMessage.ChatMode.TopLeft, MsgServer.MsgMessage.MsgColor.white);

            //}
            #endregion
            #region CpBoss
            if (Family.ID == 3156)
            {
                //if (killer.Player.TaskExpMob != 1)
                //{
                //    killer.Player.PassPoints += 500;
                //    killer.Player.TaskExpMob += 1;
                //}
                
                //killer.Player.ConquerPoints += 3000;
                killer.Player.Money += 5000000;
                killer.Player.SendUpdate(stream, killer.Player.Money, MsgUpdate.DataType.Money);

                if (killer.Inventory.HaveSpace(5))
                {
                    killer.Inventory.Add(stream, 720757, 5, 0);
                    killer.Inventory.Add(stream, 722178, 3, 0);

                }
                else
                {
                    killer.Inventory.AddReturnedItem(stream, 720757, 5);
                    killer.Inventory.AddReturnedItem(stream, 722178, 3);
                }
                //DistributeBossPoints();

                SendSysMesage(killer.Player.Name + " killed CpBoss and received [5]ExpBallPack, 5,000,000 Silver and 3000 ConquerPoints!.", MsgServer.MsgMessage.ChatMode.TopLeft, MsgServer.MsgMessage.MsgColor.white);

                if (Role.Core.Rate(50))
                {
                    uint[] DropSpecialItems = new uint[] {Database.ItemType.PowerExpBall, Database.ItemType.ExpAmrita };

                    ushort xx = X;
                    ushort yy = Y;
                    if (killer.Map.AddGroundItem(ref xx, ref yy, 4))
                    {
                        uint IDDrop = DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)];
                        DropItem(stream, killer.Player.UID, killer.Map, DropSpecialItems[Program.GetRandom.Next(0, DropSpecialItems.Length)], xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);

                        string drop_name = Database.Server.ItemsBase[IDDrop].Name;
                    }
                }
            }
            #endregion

            #region FruitsMobs

            //if (killer.MaxFruits > 0 && killer.FruitsMobs >= killer.MaxFruits)
            //{
            //    if (killer.GeneratorItemDrop(Status.Fruits))
            //    {
            //        byte rand2 = (byte)Program.GetRandom.Next(0, 7);
            //        switch (rand2)
            //        {
            //            case 0:
            //                {
            //                    MobRole.DropItemID(killer, 711301, stream);
            //                    killer.SendSysMesage("Tomato Dropped.", MsgMessage.ChatMode.TopLeft);
            //                    break;
            //                }
            //            case 1:
            //                {
            //                    MobRole.DropItemID(killer, 711302, stream);
            //                    killer.SendSysMesage("Guava Dropped.", MsgMessage.ChatMode.TopLeft);
            //                    break;
            //                }
            //            case 2:
            //                {
            //                    MobRole.DropItemID(killer, 711303, stream);
            //                    killer.SendSysMesage("Watermelon Dropped.", MsgMessage.ChatMode.TopLeft);
            //                    break;
            //                }
            //            case 3:
            //                {
            //                    MobRole.DropItemID(killer, 711304, stream);
            //                    killer.SendSysMesage("Pear Dropped.", MsgMessage.ChatMode.TopLeft);
            //                    break;
            //                }
            //            case 4:
            //                {
            //                    if (Role.MyMath.Success(90))
            //                    {
            //                        MobRole.DropItemID(killer, 711305, stream);
            //                        killer.SendSysMesage("Grape Dropped.", MsgMessage.ChatMode.TopLeft);
            //                    }
            //                    else
            //                    {
            //                        MobRole.DropItemID(killer, 711302, stream);
            //                        killer.SendSysMesage("Guava Dropped.", MsgMessage.ChatMode.TopLeft);
            //                        string MSG = "#53 Guava Dropped by " + killer.Player.Name + " #53.";
            //                        Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            //                    }
            //                    break;
            //                }
            //            default:
            //                {
            //                    goto case 4;
            //                    break;
            //                }
            //        }
            //        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "dispel5");
            //        return true;
            //    }
            //}

            #endregion FruitsMobs
            #region Soliman
            if (Map == 3845)
            {
                TheChosenProject.Game.MsgNpc.Scripts.Quests.Solomon.isTargetToSolomonDead(killer, this);
                return;
            }
            #endregion
            #region 2rbQuest
            if (Map == 1700)
            {// first stage -- drop normal moss / dreams / soul

                if (Family.ID == 3616 || Family.ID == 3602 || Family.ID == 3628 || Family.ID == 3608 || Family.ID == 3624
                    || Family.ID == 3621 || Family.ID == 3606 || Family.ID == 3631 || Family.ID == 3620 || Family.ID == 3625 || Family.ID == 3601 || Family.ID == 3618
                    || Family.ID == 3626 || Family.ID == 3613 || Family.ID == 3609 || Family.ID == 3605 || Family.ID == 3629 || Family.ID == 3604 || Family.ID == 3600
                    || Family.ID == 3622 || Family.ID == 3617 || Family.ID == 3610 || Family.ID == 3612 || Family.ID == 3614)
                {
                    if (killer.Player.Quest2rbStage == 1)
                    {
                        if (Role.Core.Rate(0.002))
                        {
                            DropItemID(killer, Database.ItemType.SoulAroma, stream);
                        }
                        else if (Role.Core.Rate(0.002))
                        {
                            DropItemID(killer, Database.ItemType.DreamGrass, stream);
                        }
                        else if (Role.Core.Rate(0.002))
                        {
                            DropItemID(killer, Database.ItemType.Moss, stream);
                        }
                    }
                }
                else
                {
                    if (killer.Player.Quest2rbS2Point <= 70000 && killer.Player.Quest2rbStage == 2)
                    {
                        if (Family.ID == 3611 || Family.ID == 3619 || Family.ID == 3603 || Family.ID == 3615 || Family.ID == 3627 || Family.ID == 3631 || Family.ID == 3607)
                        {
                            if (killer.Player.Quest2rbS2Point < 70000)
                            {
                                killer.Player.Quest2rbS2Point += 50;
                                killer.SendSysMesage($"Stage2 : Raze the mountin of Grievanceto the ground +50 / {killer.Player.Quest2rbS2Point}");
                            }
                        }
                        else
                        {
                            if (killer.Player.Quest2rbS2Point < 70000)
                            {
                                killer.Player.Quest2rbS2Point += 20;
                                killer.SendSysMesage($"Stage2 : Raze the mountin of Grievanceto the ground +20 / {killer.Player.Quest2rbS2Point}");
                            }

                        }
                    }
                    else if(killer.Player.Quest2rbS2Point >= 70000 && killer.Player.Quest2rbStage == 2)
                    {
                        killer.SendSysMesage("you have finish the stage 2 go to talk to Bruce at (602,643)");
                    }

                }


                // stage 2 
                if (Family.ID == 3632)
                {
                    if (killer.Player.UID == aUID)
                    {
                        if (Role.Core.Rate(100))
                        {
                            DropItemID(killer, 722722, stream);
                        }
                    }
                }
                else if (Family.ID == 3633)
                {
                    if (killer.Player.UID == aUID)
                    {
                        if (Role.Core.Rate(100))
                        {
                            DropItemID(killer, 722726, stream);
                        }
                    }
                }
                else if (Family.ID == 3634)
                {
                    if (killer.Player.UID == aUID)
                    {
                        if (Role.Core.Rate(100))
                        {
                            DropItemID(killer, 722729, stream);
                        }
                    }
                }
                else if (Family.ID == 3635)
                {// if some one else kill this monser reset all the quest !
                    if (killer.Player.UID == aUID)
                    {
                        if (Role.Core.Rate(100))
                        {
                            DropItemID(killer, 722731, stream);
                            //Program.DiscordQuestRebirth.Enqueue($"Lucky Player {killer.Name} killed the {Family.Name} and drop PureVigor to finish the stage 1");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { "fire1" });
                        }
                    }
                    else
                    {
                        killer.SendSysMesage("Your unlucky CleansingDevil drop nothing!");
                    }
                }


                //third sage
                if (killer.Player.Quest2rbStage == 3)
                {
                    if (Family.ID == 3636 && killer.Player.Quest2rbBossesOrderby == 0) // andrew
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        killer.SendSysMesage("Congratulations You'h killed Boss Andrew, now go kill Peter");
                    }
                    else if (Family.ID == 3637 && killer.Player.Quest2rbBossesOrderby == 1) // peter
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        killer.SendSysMesage("Congratulations You'h killed Boss Peter, now go kill Philip");
                    }
                    else if (Family.ID == 3638 && killer.Player.Quest2rbBossesOrderby == 2) // phillip
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        killer.SendSysMesage("Congratulations You'h killed Boss Philip, now go kill Timothy");
                    }
                    else if (Family.ID == 3639 && killer.Player.Quest2rbBossesOrderby == 3) // Timothy
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        killer.SendSysMesage("Congratulations You'h killed Boss Timothy, now go kill Daphne ");
                    }
                    else if (Family.ID == 3640 && killer.Player.Quest2rbBossesOrderby == 4) // Daphne
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        killer.SendSysMesage("Congratulations You'h killed Boss Daphne, now go kill Victoria ");
                    }
                    else if (Family.ID == 3641 && killer.Player.Quest2rbBossesOrderby == 5) // Victoria
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        killer.SendSysMesage("Congratulations You'h killed Boss Victoria, now go kill Wayne ");
                    }
                    else if (Family.ID == 3642 && killer.Player.Quest2rbBossesOrderby == 6) // Wayne
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        killer.SendSysMesage("Congratulations You'h killed Boss Wayne, now go kill Theodore ");
                    }
                    else if (Family.ID == 3643 && killer.Player.Quest2rbBossesOrderby == 7) // Theodore
                    {
                        killer.Player.Quest2rbBossesOrderby += 1;
                        //Program.DiscordQuestRebirth.Enqueue($"Lucky Player {killer.Name} killed all the Boss now third sage done go talk to Stanley");
                        killer.SendSysMesage("Congratulations You'h killed Boss Theodore, now third sage done go talk to Stanley");
                        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, new string[1] { "tj" });

                    }

                    else if (killer.Player.Quest2rbBossesOrderby == 8 && Family.ID == 3611 || Family.ID == 3619 || Family.ID == 3603 || Family.ID == 3615 || Family.ID == 3627 || Family.ID == 3631 || Family.ID == 3607)
                    {
                        if (Role.Core.Rate(50))
                        {
                            DropItemID(killer, 722727, stream);
                            //Program.DiscordQuestRebirth.Enqueue($"Lucky Player {killer.Name} killed The Lord, and he dropped SquamaBead go to spawn Satan at(326,342)");
                            killer.SendSysMesage("Congratulations You'h killed The Lord, and he dropped SquamaBead go to spawn Satan at(326,342)");
                        }
                        else
                        {
                            killer.SendSysMesage("Better luck next time, go find another Lord and kill it!");
                        }
                        return;
                    }
                    else if (killer.Player.Quest2rbBossesOrderby == 8 && Family.ID == 3644)//satan
                    {
                        Database.Server.AddMapMonster(stream, killer.Map, 3645, killer.Player.X, killer.Player.Y, 1, 1, 1);
                    }
                    else if (killer.Player.Quest2rbBossesOrderby == 8 && Family.ID == 3645)//satan
                    {
                        Database.Server.AddMapMonster(stream, killer.Map, 3646, killer.Player.X, killer.Player.Y, 1, 1, 1);
                    }
                    else if (killer.Player.Quest2rbBossesOrderby == 8 && Family.ID == 3646)//Furysatan
                    {
                        if (killer.Player.Quest2rbStage == 3)
                        {
                            if(killer.Player.Reborn == 2)
                            {
                                killer.Player.Quest2rbStage = 4;
                            }
                            else
                            {
                                killer.Player.Quest2rbS2Point = 0;
                                killer.Player.Quest2rbStage = 1;
                                killer.Player.Quest2rbBossesOrderby = 0;
                            }
                            DropItemID(killer, 723701, stream);
                            //Program.DiscordQuestRebirth.Enqueue("Congratulations! " + killer.Player.Name + " he/she finish the 2rb quest .");
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + killer.Player.Name + " he/she finish the 2rb quest and got Exemption Token .", Game.MsgServer.MsgMessage.MsgColor.yellow, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }
                        //killer.Player.Quest2rbStage += 1;// 2!
                    }
                }
                else if (killer.Player.Quest2rbS2Point <= 70000 && killer.Player.Quest2rbStage == 2)
                {
                    if (Family.ID == 3611 || Family.ID == 3619 || Family.ID == 3603 || Family.ID == 3615 || Family.ID == 3627 || Family.ID == 3631 || Family.ID == 3607)
                    {
                        if (killer.Player.Quest2rbS2Point < 70000)
                        {
                            killer.Player.Quest2rbS2Point += 500;
                            killer.SendSysMesage($"Stage2 : Raze the mountin of Grievanceto the ground +500 / {killer.Player.Quest2rbS2Point}");
                        }
                    }
                    else
                    {
                        if (killer.Player.Quest2rbS2Point < 70000)
                        {
                            killer.Player.Quest2rbS2Point += 20;
                            killer.SendSysMesage($"Stage2 : Raze the mountin of Grievanceto the ground +20 / {killer.Player.Quest2rbS2Point}");
                        }

                    }
                }
                else if(killer.Player.Quest2rbS2Point >= 70000 && killer.Player.Quest2rbStage == 2)
                {
                    killer.SendSysMesage("you have finish the stage 2 go to talk to Bruce at (602,643)");
                }
            }
            #endregion
            #region DungeonDomination
            if (Map == 3080)
            {
                if (killer.Player.DynamicID != killer.Player.UID)
                {
                    #region stage 1
                    if (Family.ID == 19896)
                    {
                        #region Stage - drop


                        for (int x = 0; x < 11; x++)
                        {
                            uint id = 723459;//99Roses
                            if (x == 0)
                                id = 810032;//ValorStone
                            if (x == 1)
                                id = 710834;//DragonBallPeice
                            if (x == 2)
                                id = 710834;//DragonBallPeice
                            if (x == 3)
                                id = 710834;//DragonBallPeice
                            if (x == 4)
                                id = 1088001;//Meteor
                            if (x == 5)
                                id = 1088001;//Meteor
                            if (x == 6)
                                id = 1088001;//Meteor
                            if (x == 7)
                                id = 1088001;//Meteor
                            if (x == 8)
                                id = 1088001;//Meteor
                            if (x == 9)
                                id = 1088001;//Meteor
                            if (x == 10)
                                id = 723459;//99Roses

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }

                        for (ushort i = 0; i < 6; i++)
                        {
                            uint amount = (uint)Program.GetRandom.Next(45000, 50000);
                            var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ItemID;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;

                            ushort xx = (ushort)Program.GetRandom.Next(X - 8, X + 8);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 8, Y + 8);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }

                        #endregion
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            var GMap = Database.Server.ServerMaps[3080];
                            var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p =>
                            (p as MonsterRole).Family.ID == 19896 && p.Alive);
                            if (mobs.Count() == 0)
                            {
                                if (!GMap.ContainMobID(19897))
                                {
                                    Database.Server.AddMapMonster(stream2, GMap, 19897, 61, 73, 1, 1, 1);
                                }
                            }
                        }
                    }
                    #endregion
                    #region stage 2
                    if (Family.ID == 19897)
                    {
                        #region Stage - drop


                        for (int x = 0; x < 18; x++)
                        {
                            uint id = 753099;//99Orchids
                            if (x == 0)
                                id = 753099;//99Orchids
                            if (x == 1)
                                id = 720027;//MeteorScroll
                            if (x == 2)
                                id = 720027;//MeteorScroll
                            if (x == 3)
                                id = 720027;//MeteorScroll
                            if (x == 4)
                                id = 720027;//MeteorScroll
                            if (x == 5)
                                id = 720027;//MeteorScroll
                            if (x == 6)
                                id = 720027;//MeteorScroll
                            if (x == 7)
                                id = 1088001;//Meteor
                            if (x == 8)
                                id = 1088001;//Meteor
                            if (x == 9)
                                id = 1088001;//Meteor
                            if (x == 10)
                                id = 1088001;//Meteor
                            if (x == 11)
                                id = 1088001;//Meteor
                            if (x == 12)
                                id = 1088001;//Meteor
                            if (x == 13)
                                id = 1088001;//Meteor
                            if (x == 14)
                                id = 1088001;//Meteor
                            if (x == 15)
                                id = 1088001;//Meteor
                            if (x == 16)
                                id = 1088001;//Meteor
                            if (x == 17)
                                id = 810033;//DragonBall

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }

                        for (ushort i = 0; i < 6; i++)
                        {
                            uint amount = (uint)Program.GetRandom.Next(45000, 50000);
                            var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ItemID;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;

                            ushort xx = (ushort)Program.GetRandom.Next(X - 8, X + 8);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 8, Y + 8);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }

                        #endregion

                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            var GMap = Database.Server.ServerMaps[3080];
                            var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p =>
                            (p as MonsterRole).Family.ID == 19897 && p.Alive);
                            if (mobs.Count() == 0)
                            {
                                if (!GMap.ContainMobID(19898))
                                {
                                    Database.Server.AddMapMonster(stream2, GMap, 19898, 61, 73, 1, 1, 1);
                                }
                            }
                        }
                    }
                    #endregion
                    #region stage 3
                    if (Family.ID == 19898)
                    {
                        #region Stage - drop


                        for (int x = 0; x < 13; x++)
                        {
                            uint id = 752099;//PowerExpBall
                            if (x == 0)
                                id = 752099;//99Lilly
                            if (x == 1)
                                id = 753099;//99Orchids
                            if (x == 2)
                                id = 710834;//DragonBallPiece
                            if (x == 3)
                                id = 710834;//DragonBallPiece
                            if (x == 4)
                                id = 710834;//DragonBallPiece
                            if (x == 5)
                                id = 710834;//DragonBallPiece
                            if (x == 6)
                                id = 710834;//DragonBallPiece
                            if (x == 7)
                                id = 1088000;//DragonBall
                            if (x == 8)
                                id = 1088000;//DragonBall
                            if (x == 9)
                                id = 1088000;//DragonBall
                            if (x == 10)
                                id = 1088000;//DragonBall
                            if (x == 11)
                                id = 1088000;//DragonBall
                            if (x == 12)
                                id = 810034;//DragonBall

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }

                        for (ushort i = 0; i < 6; i++)
                        {
                            uint amount = (uint)Program.GetRandom.Next(45000, 50000);
                            var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = ItemID;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;

                            ushort xx = (ushort)Program.GetRandom.Next(X - 8, X + 8);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 8, Y + 8);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }

                        #endregion
                        using (RecycledPacket recycledPacket = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket.GetStream();
                            var GMap = Database.Server.ServerMaps[3080];
                            var mobs = GMap.View.GetAllMapRoles(MapObjectType.Monster).Where(p =>
                            (p as MonsterRole).Family.ID == 19898 && p.Alive);
                            if (mobs.Count() == 0)
                            {
                                if (!GMap.ContainMobID(19899))
                                {
                                    Database.Server.AddMapMonster(stream2, GMap, 19899, 61, 73, 1, 1, 1);
                                }
                            }
                        }
                    }
                    #endregion
                    #region stage 4
                    if (Family.ID == 19899)
                    {
                        #region Stage - drop


                        for (int x = 0; x < 9; x++)
                        {
                            uint id = 727385;//+1StoneCoupon
                            if (x == 0)
                                id = 727385;//+1StoneCoupon
                            if (x == 1)
                                id = 727385;//+1StoneCoupon
                            if (x == 2)
                                id = 727385;//+1StoneCoupon
                            if (x == 3)
                                id = 727385;//+1StoneCoupon
                            if (x == 4)
                                id = 727385;//+1StoneCoupon
                            if (x == 5)
                                id = 780010;//1H-VIP
                            if (x == 6)
                                id = 720393;//2H-x2DoubleEXP
                            if (x == 7)
                                id = 720393;//UpToken
                            if (x == 8)
                                id = 720393;//UpToken
                            if (x == 8)
                                id = 720393;//UpToken



                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }

                        #endregion
                    }
                    #endregion
                }
                return;
            }

            
            #endregion
            #region moonbox
            if (Map == 1043 || Map == 1044 || Map == 1045 || Map == 1046 || Map == 1047 || Map == 1048 || Map == 1049)
            {// moon box
                if (Family.ID == 6000 || Family.ID == 6001 || Family.ID == 6002 || Family.ID == 6003 || Family.ID == 6004 || Family.ID == 6005)//Moonbox
                {
                    uint itemid = 0;
                    if (Family.ID == 6000 && Map == 1043)//Chaos
                        itemid = 721010;
                    if (Family.ID == 6001 && Map == 1044)//Peace
                        itemid = 721011;
                    if (Family.ID == 6002 && Map == 1045)//Deserted
                        itemid = 721012;
                    if (Family.ID == 6003 && Map == 1046)//Prosperous
                        itemid = 721013;
                    if (Family.ID == 6004 && Map == 1047)//Disturbed
                        itemid = 721014;
                    if (Family.ID == 6005 && Map == 1048)//Calmed
                        itemid = 721015;

                    if (Role.Core.Rate(0.005))
                    {
                        ushort xx = X;
                        ushort yy = Y;
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            DropItem(stream, killer.Player.UID, killer.Map, itemid, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }

                    }
                }
            }
            #endregion
            #region lab tokens
            if (Map == 1351 || Map == 1352 || Map == 1353 || Map == 1354)
            {
                if (Family.ID == 3142)
                {
                    if (Role.Core.Rate(0.01))//SkyToken
                    {
                        DropItemID(killer, 721537, stream);
                    }
                }
                //if (Family.ID == 3141)//lAB1
                //{
                //    if (Role.Core.Rate(0.05))
                //    {
                //        DropItemID(killer, 721533, stream);
                //    }
                //}
                //if (Family.ID == 3144)//lab2
                //{
                //    if (Role.Core.Rate(0.05))
                //    {
                //        DropItemID(killer, 721534, stream);
                //    }

                //}
                if (Family.ID == 3145)
                {
                    if (Role.Core.Rate(0.01))
                    {
                        DropItemID(killer, 721538, stream);
                    }
                }
                //if (Family.ID == 3147)//lab3 1353
                //{
                //    if (Role.Core.Rate(0.5))
                //    {
                //        DropItemID(killer, 721535, stream);
                //    }
                //}
                if (Family.ID == 3148)
                {
                    if (Role.Core.Rate(0.01))
                    {
                        DropItemID(killer, 721539, stream);//soul tokken
                    }
                }
                //if (Family.ID == 3155 || Family.ID == 3156)//lab3 1353
                //{
                //    if (Role.Core.Rate(0.002))
                //    {
                //        DropItemID(killer, 721536, stream);
                //    }
                //}

            }
            #endregion
            #region Boss

            //bool IsBoss = false;
            //if (System.Enum.IsDefined(typeof(TheChosenProject.Mobs.IDMonster), (TheChosenProject.Mobs.IDMonster)Family.ID))
            //{
            //    var boss = MobsHandler.CallUp(Family, (IDMonster)Family.ID);
            //    if (boss != null)
            //    {
            //        boss.Reward(this, killer);
            //        IsBoss = true;
            //    }
            //}

            #endregion Boss
            #region RoyalPass Bosses
            //if (killer.Player.Map == 1121)
            //{
                if (Family.ID == 20060 || Family.ID == 20160 || Family.ID == 20300 || Family.ID == 20070 || Family.ID == 6643)
                {
                    if (killer.Player.DynamicID != killer.Player.UID)
                    {
                        //if (Family.ID == 20300)
                        //    killer.DbDailyTraining.NemesisTyrant++;
                        //    killer.Player.RoyalPassPoints += 10;

                        //if (Family.ID == 6643)
                        //    killer.DbDailyTraining.SwordMaster++;
                        //    killer.Player.RoyalPassPoints += 10;

                        //if (Family.ID == 20060 || Family.ID == 20160 || Family.ID == 20300 || Family.ID == 20070)
                        //    killer.DbDailyTraining.CityBosses++;
                        //    killer.Player.RoyalPassPoints += 10;

                        List<uint> list;
                        list = new List<uint>();
                        list.Add(754009);//999Tulips
                        list.Add(753009);//999Orchids
                        list.Add(751009);
                        List<uint> listFlower;
                        listFlower = list;
                        uint reward;
                    #region Drop = SurpriseBox
                    for (int x = 0; x < 5; x++)
                        {
                            if (x <= 5 || (x > 2 && Role.Core.Rate(100)))
                            {
                                uint id = 722178;//SurpriseBox

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        #endregion
                    #region Drop = +4 Stone
                    //for (int x = 0; x < 3; x++)
                    //{
                    //    if (x <= 5 || (x > 2 && Role.Core.Rate(45)))
                    //    {
                    //        uint id = 730004;//+3 Stone

                    //        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //        DataItem.ITEM_ID = id;
                    //        Database.ItemType.DBItem DBItem;
                    //        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //        {
                    //            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //        }
                    //        DataItem.Color = Role.Flags.Color.Red;
                    //        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                    //        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                    //        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //        {
                    //            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //            if (killer.Map.EnqueueItem(DropItem))
                    //            {

                    //                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region Drop = +5 Stone
                    //for (int x = 0; x < 1; x++)
                    //{
                    //    if (x <= 5 || (x > 2 && Role.Core.Rate(45)))
                    //    {
                    //        uint id = 730005;//+4 Stone

                    //        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //        DataItem.ITEM_ID = id;
                    //        Database.ItemType.DBItem DBItem;
                    //        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //        {
                    //            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //        }
                    //        DataItem.Color = Role.Flags.Color.Red;
                    //        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                    //        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                    //        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //        {
                    //            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //            if (killer.Map.EnqueueItem(DropItem))
                    //            {

                    //                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region Drop = RoyalCPPack
                    //for (int x = 0; x < 1; x++)
                    //{
                    //    if (x <= 5 || (x > 2 && Role.Core.Rate(45)))
                    //    {
                    //        uint id = 723411;//RoyalCPPack

                    //        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //        DataItem.ITEM_ID = id;
                    //        Database.ItemType.DBItem DBItem;
                    //        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //        {
                    //            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //        }
                    //        DataItem.Color = Role.Flags.Color.Red;
                    //        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                    //        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                    //        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //        {
                    //            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //            if (killer.Map.EnqueueItem(DropItem))
                    //            {

                    //                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region Drop = DragonBallScrap
                    for (int x = 0; x < 10; x++)
                        {
                            if (x <= 10 || (x > 10 && Role.Core.Rate(45)))
                            {
                                uint id = 710834;//Scrap

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                    #endregion
                        #region Drop = DragonBall
                            for (int x = 0; x < 3; x++)
                        {
                            if (x <= 3 || (x > 3 && Role.Core.Rate(100)))
                            {
                                uint id = 1088000;//DB

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 9, X + 9);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 9, Y + 9);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        #endregion
                    #region Drop = Metscroll
                    for (int x = 0; x < 10; x++)
                        {
                            if (x <= 10 || (x > 10 && Role.Core.Rate(100)))
                            {
                                uint id = 720027;//DB

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 9, X + 9);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 9, Y + 9);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Drop = Mets
                        for (int x = 0; x < 20; x++)
                        {
                            if (x <= 20 || (x > 20 && Role.Core.Rate(45)))
                            {
                                uint id = 1088001;//Mets

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Drop = Flowers
                        for (int x = 0; x < 5; x++)
                        {
                            if (x <= 5 || (x > 5 && Role.Core.Rate(100)))
                            {
                                reward = listFlower[Core.Random.Next(0, listFlower.Count)];
                                uint id = reward;
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = id;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                DataItem.Color = Role.Flags.Color.Red;
                                ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                                ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                                if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                {
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                    if (killer.Map.EnqueueItem(DropItem))
                                    {

                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Drop = Gold
                        if (Role.Core.Rate(100))
                        {
                            for (ushort i = 0; i < 10; i++)
                            {
                                uint amount = (uint)Program.GetRandom.Next(80000, 110000);
                                var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                DataItem.ITEM_ID = ItemID;
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                                {
                                    DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                }
                                    DataItem.Color = Role.Flags.Color.Red;
                    
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                            }
                            //DropItemNull(ItemID, stream, MsgItem.ItemType.Money, amount);
                        }
                    #endregion
                        #region Drop = Emerald
                        for (int x = 0; x < 3; x++)
                            {
                                if (x <= 3 || (x > 3 && Role.Core.Rate(45)))
                                {
                                    uint id = 1080001;//Emerald

                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                        #endregion
                    #region Drop = MoonBox
                    for (int x = 0; x < 3; x++)
                            {
                                if (x <= 5 || (x > 3 && Role.Core.Rate(100)))
                                {
                                    uint id = 721080;//MoonBox

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                        #endregion
                        #region Drop = Proftoken
                        for (int x = 0; x < 5; x++)
                            {
                                if (x <= 5 || (x > 5 && Role.Core.Rate(100)))
                                {
                                    uint id = 722384;//SuperGemBox

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                        #endregion
                    killer.Player.BossPoints += 1;
                    string msg1 = $"{killer.Player.Name} killed the {Family.Name}, give him/her a +1 Boss Points and dropped massive reward at {killer.Map.Name} and more ... #35#35";
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg1, Game.MsgServer.MsgMessage.MsgColor.yellow, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                }
                return;
                }
            //}
            #endregion
            if(Family.ID == 3737)
            {
                List<uint> list;
                list = new List<uint>();
                list.Add(754009);//999Tulips
                list.Add(753009);//999Orchids
                list.Add(751009);
                List<uint> listFlower;
                listFlower = list;
                uint reward;
                #region Drop = DragonBallScrap
                for (int x = 0; x < 10; x++)
                {
                    if (x <= 10 || (x > 10 && Role.Core.Rate(45)))
                    {
                        uint id = 710834;//Scrap

                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                }
                #endregion
                #region Drop = DragonBall
                for (int x = 0; x < 4; x++)
                {
                    if (x <= 4 || (x > 4 && Role.Core.Rate(100)))
                    {
                        uint id = 1088000;//DB

                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 9, X + 9);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 9, Y + 9);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                }
                #endregion
                #region Drop = Metscroll
                for (int x = 0; x < 10; x++)
                {
                    if (x <= 10 || (x > 10 && Role.Core.Rate(100)))
                    {
                        uint id = 720027;//DB

                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 9, X + 9);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 9, Y + 9);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                }
                #endregion
                #region Drop = Mets
                for (int x = 0; x < 15; x++)
                {
                    if (x <= 15 || (x > 15 && Role.Core.Rate(45)))
                    {
                        uint id = 1088001;//Mets

                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                }
                #endregion
                #region Drop = Flowers
                for (int x = 0; x < 5; x++)
                {
                    if (x <= 5 || (x > 5 && Role.Core.Rate(100)))
                    {
                        reward = listFlower[Core.Random.Next(0, listFlower.Count)];
                        uint id = reward;
                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                }
                #endregion
                #region Drop = PromotionStone
                    List<uint> stonelist;
                    stonelist = new List<uint>();
                    stonelist.Add(810032);//999Tulips
                    stonelist.Add(810033);//999Orchids
                    stonelist.Add(810034);
                    List<uint> listStone;
                    listFlower = stonelist;
                    uint rewardStone;
                    for (int x = 0; x < 3; x++)
                    {
                        if (x <= 5 || (x > 2 && Role.Core.Rate(100)))
                        {
                        rewardStone = listFlower[Core.Random.Next(0, stonelist.Count)];
                            uint id = rewardStone;
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                #endregion
                #region Drop = Gold
                if (Role.Core.Rate(100))
                {
                    for (ushort i = 0; i < 10; i++)
                    {
                        uint amount = (uint)Program.GetRandom.Next(80000, 110000);
                        var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = ItemID;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;

                        ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                    //DropItemNull(ItemID, stream, MsgItem.ItemType.Money, amount);
                }
                #endregion
                #region Drop = MagicBallExp
                //for (int x = 0; x < 3; x++)
                //{
                //    if (x <= 3 || (x > 3 && Role.Core.Rate(45)))
                //    {
                //        uint id = 720668;//Mets

                //        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                //        DataItem.ITEM_ID = id;
                //        Database.ItemType.DBItem DBItem;
                //        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                //        {
                //            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                //        }
                //        DataItem.Color = Role.Flags.Color.Red;
                //        ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
                //        ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
                //        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                //        {
                //            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                //            if (killer.Map.EnqueueItem(DropItem))
                //            {

                //                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                //            }
                //        }
                //    }
                //}
                #endregion
                #region Drop = UpToken
                for (int x = 0; x < 1; x++)
                {
                    if (x <= 1 || (x > 1 && Role.Core.Rate(100)))
                    {
                        uint id = 720310;//SuperGemBox

                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                }
                #endregion
                #region Drop = Proftoken
                for (int x = 0; x < 2; x++)
                {
                    if (x <= 5 || (x > 2 && Role.Core.Rate(100)))
                    {
                        uint id = 722384;//SuperGemBox

                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = id;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                        {
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                            if (killer.Map.EnqueueItem(DropItem))
                            {

                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                            }
                        }
                    }
                }
                #endregion
                killer.Player.BossPoints += 1;
                string msg1 = $"{killer.Player.Name} killed the Corn Devil Boss, and dropped massive reward, [DragonBall],[Uptoken],[Proftoken] and more ... #35#35";
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg1, Game.MsgServer.MsgMessage.MsgColor.yellow, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

            }
            #region Bosses
            if (BossDatabase.Bosses.ContainsKey(Family.ID) && BossDatabase.Bosses[Family.ID].Alive)
            {
                LookFace mesh;
                mesh = (LookFace)Family.Mesh;
                if (mesh == LookFace.TwinCityBoss 
                    || mesh == LookFace.CastleCityBoss 
                    || mesh == LookFace.ApeCityBoss 
                    || mesh == LookFace.DesertCityBoss 
                    || mesh == LookFace.BirdCityBoss 
                    )
                {
                    Boss boss;
                    boss = BossDatabase.Bosses[Family.ID];
                    if (killer.Player.QuestGUI.CheckQuest(20193, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                    {
                        if (!killer.Player.QuestGUI.CheckObjectives(Flags.MissionsFlag.DEFEAT_FIVE_BOSS, 15))
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, Flags.MissionsFlag.DEFEAT_FIVE_BOSS, 1);
                        else
                            killer.CreateBoxDialog("You`ve~killed~15~" + Family.Name + ".~Now~go~find~DailyQuestEnvoy!.");
                    }
                    //killer.DbDailyTraining.CityBosses++;
                    List<uint> list;
                    list = new List<uint>();
                    list.Add(754999);//999Tulips
                    list.Add(753999);//999Orchids
                    List<uint> listFlower;
                    listFlower = list;
                    uint reward;
                    if (killer.Player.DynamicID != killer.Player.UID)
                    {
                        for (int x = 0; x < boss.Items.Count; x++)
                        {
                            uint id;
                            id = uint.Parse(boss.Items[x]);
                            MsgGameItem DataItem2;
                            DataItem2 = new MsgGameItem
                            {
                                ITEM_ID = id
                            };
                            if (Server.ItemsBase.TryGetValue(id, out var DBItem2))
                                DataItem2.Durability = (DataItem2.MaximDurability = DBItem2.Durability);
                            DataItem2.Color = Flags.Color.Red;
                            ushort xx5;
                            xx5 = (ushort)ServerKernel.NextAsync(X - 7, X + 7);
                            ushort yy5;
                            yy5 = (ushort)ServerKernel.NextAsync(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx5, ref yy5, 3))
                            {
                                MsgItem DropItem2;
                                DropItem2 = new MsgItem(DataItem2, xx5, yy5, MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);
                                if (killer.Map.EnqueueItem(DropItem2))
                                    DropItem2.SendAll(stream, MsgDropID.Visible);
                            }
                        }
                        
                    }
                    #region Drop = +1
                    //for (int x = 0; x < 20; x++)
                    //{
                    //    if (x <= 20 || (x > 20 && Role.Core.Rate(45)))
                    //    {
                    //        Database.ItemType.DBItem DbItem = null;
                    //        byte ID_Quality;
                    //        bool ID_Special;

                    //        uint id = Family.ItemGenerator.GenerateItemId(Map, out ID_Quality, out ID_Special, out DbItem);
                    //        //uint plus = 1;

                    //        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                    //        DataItem.ITEM_ID = id;
                    //        Database.ItemType.DBItem DBItem;
                    //        if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                    //        {
                    //            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                    //            DataItem.Plus = 1;

                    //        }
                    //        DataItem.Color = Role.Flags.Color.Red;
                    //        ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                    //        ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                    //        if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                    //        {
                    //            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                    //            if (killer.Map.EnqueueItem(DropItem))
                    //            {

                    //                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                    #region Drop = DragonballPiece
                    for (int x = 0; x < 10; x++)
                    {
                        if (x <= 10 || (x > 10 && Role.Core.Rate(45)))
                        {
                            uint id = 710834;//DragonballPiece
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = Dragonball
                    for (int x = 0; x < 2; x++)
                    {
                        if (x <= 2 || (x > 2 && Role.Core.Rate(45)))
                        {
                            uint id = 1088000;//MeteorScroll

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {
                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = MeteorScroll
                    for (int x = 0; x < 5; x++)
                    {
                        if (x <= 5 || (x > 5 && Role.Core.Rate(45)))
                        {
                            uint id = 720027;//MeteorScroll

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = Meteor
                    for (int x = 0; x < 10; x++)
                    {
                        if (x <= 10 || (x > 10 && Role.Core.Rate(45)))
                        {
                            uint id = 1088001;//Meteor

                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = Flowers
                    for (int x = 0; x < 3; x++)
                    {
                        if (x <= 3 || (x > 3 && Role.Core.Rate(45)))
                        {
                            reward = listFlower[Core.Random.Next(0, listFlower.Count)];
                            uint id = reward;
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            DataItem.ITEM_ID = id;
                            Database.ItemType.DBItem DBItem;
                            if (Database.Server.ItemsBase.TryGetValue(id, out DBItem))
                            {
                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                            }
                            DataItem.Color = Role.Flags.Color.Red;
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Drop = Gold
                    if (Role.Core.Rate(100))
                    {
                        uint amount = (uint)Program.GetRandom.Next(100000);
                        var ItemID = Database.ItemType.MoneyItemID((uint)amount);
                        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                        DataItem.ITEM_ID = ItemID;
                        Database.ItemType.DBItem DBItem;
                        if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                        {
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        DataItem.Color = Role.Flags.Color.Red;
                        for (ushort i = 0; i < 10; i++)
                        {
                            ushort xx = (ushort)Program.GetRandom.Next(X - 7, X + 7);
                            ushort yy = (ushort)Program.GetRandom.Next(Y - 7, Y + 7);
                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                            {
                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), MsgFloorItem.MsgItem.ItemType.Money, amount, 0, Map, 0, false, GMap);
                                if (killer.Map.EnqueueItem(DropItem))
                                {

                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                }
                            }
                        }
                        //DropItemNull(ItemID, stream, MsgItem.ItemType.Money, amount);
                    }
                    #endregion

                    DistributeBossPoints(boss.ConquerPointScores, boss.ItemDropScores, boss.BossPointScores);
                    killer.Player.ConquerPoints += (int)boss.ConquerPointDropped;
                }
                BossDatabase.Bosses[Family.ID].Alive = false;
            }
            #endregion

            //if (!IsBoss && (Family.Settings & MonsterSettings.DropItemsOnDeath) != MonsterSettings.DropItemsOnDeath)
            //    return;
            if (/*!IsBoss && */(Family.Settings & MonsterSettings.DropItemsOnDeath) == MonsterSettings.DropItemsOnDeath)
            {

                #region GenerateBossFamily  - Disable
                //if (Family.MaxHealth > 100000 && Family.MaxHealth < 7000000|| Boss == 1)
                //{
                //    List<uint> DropIems = Family.ItemGenerator.GenerateBossFamily();
                //    foreach (var ids in DropIems)
                //    {
                //        MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                //        DataItem.ITEM_ID = ids;
                //        Database.ItemType.DBItem DBItem;
                //        if (Database.Server.ItemsBase.TryGetValue(ids, out DBItem))
                //        {
                //            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                //        }
                //        DataItem.Color = Role.Flags.Color.Red;
                //        ushort xx = X;
                //        ushort yy = Y;
                //        if (killer.Map.AddGroundItem(ref xx, ref yy))
                //        {
                //            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                //            if (killer.Map.EnqueueItem(DropItem))
                //            {
                //                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                //            }
                //        }
                //    }
                //    return;
                //}
                #endregion


                #region GenerateGold
                ushort rand = (ushort)(killer.Player.MyRandom.Next() % 1000);
                byte count = 1;
                //if (Role.Core.Rate(10))
                //{
                //    if (killer.Player.BlessTime > 0 ? rand > 150 && rand < 700 : rand > 200 && rand < 800)
                //    {
                //        ushort xx = X;
                //        ushort yy = Y;
                //        for (byte i = 0; i < 1; i++)
                //        {
                //            if (killer.Map.AddGroundItem(ref xx, ref yy))
                //            {
                //                uint ItemID = 0;
                //                uint Amount = 0;

                //                if (Map == 1002)
                //                {
                //                    Amount = Family.ItemGenerator.GenerateGold(out ItemID, false, true);
                //                }
                //                else
                //                {
                //                    Amount = Family.ItemGenerator.GenerateGold(out ItemID);
                //                }

                //                if (killer.Player.VipLevel > 6)
                //                {
                //                    killer.Player.Money += (int)Amount;
                //                    killer.SendSysMesage($"You got an {Amount} Money in your inventory From Auto Pick.", MsgMessage.ChatMode.TopLeft);
                //                }
                //                else
                //                {
                //                    DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Money, Amount, false, 0);
                //                }
                //            }
                //        }
                //    }
                //}
                #endregion
                #region DropHPItem
                if (rand > 500 && rand < 600)
                {

                    ushort xx = X;
                    ushort yy = Y;
                    for (byte i = 0; i < count; i++)
                    {
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            uint ItemID = Family.DropHPItem;

                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                    }
                }
                #endregion
                #region DropMPItem
                else if (rand > 600 && rand < 700)
                {
                    ushort xx = X;
                    ushort yy = Y;
                    for (byte i = 0; i < count; i++)
                    {
                        if (killer.Map.AddGroundItem(ref xx, ref yy))
                        {
                            uint ItemID = Family.DropMPItem;

                            DropItem(stream, killer.Player.UID, killer.Map, ItemID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, false, 0);
                        }
                    }
                }
                #endregion
                #region GenerateItemId
                else if (rand > 700)//&& rand < 770)
                {
                    ushort xx = X;
                    ushort yy = Y;
                    for (byte i = 0; i < count; i++)
                    {

                        Database.ItemType.DBItem DbItem = null;
                        byte ID_Quality;
                        bool ID_Special;
                        uint ID = Family.ItemGenerator.GenerateItemId(Map, out ID_Quality, out ID_Special, out DbItem);
                        if (ID != 0)
                        {
                            bool drop = true;

                            #region DragonBall
                            //if (ID == 1088000 && killer != null)
                            //    if (killer.Player.VipLevel == 6 && killer.Player.LootDragonBall)
                            //    {
                            //        if (killer.Inventory.HaveSpace(1))
                            //        {
                            //            killer.Inventory.Add(stream, 1088000, 1);
                            //            drop = false;
                            //            if (killer.Inventory.Contain(1088000, 10) && killer.Player.VipLevel >= 1)
                            //            {
                            //                killer.Inventory.Remove(1088000, 10, stream);
                            //                killer.Inventory.Add(stream, 720028, 1);
                            //                killer.SendSysMesage("[VIP] DragonBall got autopacked.", MsgMessage.ChatMode.TopLeft);
                            //                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("As Lucky  " + killer.Player.Name + " he/she found DragonBall ", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            //                killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "downnumber1");

                            //            }
                            //        }
                            //        else
                            //        {
                                        //ActionQuery action2;
                                        //action2 = new ActionQuery()
                                        //{
                                        //    ObjId = killer.Player.UID,
                                        //    Type = ActionType.DragonBall
                                        //};
                                        //killer.Send(stream.ActionCreate(&action2));
                            //            killer.SendSysMesage("A DragonBall dropped at at " + xx + "," + yy + "!");
                            //            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("As Lucky  " + killer.Player.Name + " he/she drop DragonBall ", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            //            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "downnumber1");

                            //        }
                            //    }
                            //    else
                            //    {
                            //        ActionQuery action2;
                            //        action2 = new ActionQuery()
                            //        {
                            //            ObjId = killer.Player.UID,
                            //            Type = ActionType.DragonBall
                            //        };
                            //        killer.Send(stream.ActionCreate(&action2));
                            //        killer.SendSysMesage("A DragonBall dropped at at " + xx + "," + yy + "!");
                            //        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("As Lucky  " + killer.Player.Name + " he/she drop DragonBall ", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            //        killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "downnumber1");

                            //    }
                            #endregion
                            #region Meteor
                            //if (ID == 1088001 && killer != null)
                            //{
                            //    if (killer.Player.VipLevel == 6 /*&& killer.Player.LootMets*/)
                            //    {
                            //        if (killer.Inventory.HaveSpace(1))
                            //        {
                            //            killer.Inventory.Add(stream, 1088001, 1);
                            //            drop = false;
                            //            if (killer.Inventory.Contain(1088001, 10) && killer.Player.VipLevel >= 1)
                            //            {
                            //                killer.Inventory.Remove(1088001, 10, stream);
                            //                killer.Inventory.Add(stream, 720027, 1);
                            //                killer.SendSysMesage("[VIP] Meteor got autopacked.", MsgMessage.ChatMode.TopLeft);
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion
                            #region ExpBallEvent
                            //if (ID == 722136 && killer != null)
                            //    if (killer.Player.VipLevel == 6 && killer.Player.LootExpBall)
                            //    {
                            //        if (killer.Inventory.HaveSpace(1))
                            //        {
                            //            killer.Inventory.AddItemWitchStack(722136, 0, 1, stream);
                            //            drop = false;
                            //            if (killer.Inventory.Contain(722136, 10) && killer.Player.VipLevel >= 1)
                            //            {
                            //                killer.Inventory.Remove(722136, 10, stream);
                            //                killer.Inventory.Add(stream, 720757, 1);
                            //                killer.SendSysMesage("[VIP] Exp balls got autopacked.", MsgMessage.ChatMode.TopLeft);
                            //            }
                            //        }
                            //        else
                            //            killer.SendSysMesage("A monster you killed has dropped a ExpBallEvent at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);



                            //    }
                            #endregion
                            if(ID_Quality == 8 && killer.Player.VipLevel == 6 && killer.Player.EliteLoot)
                            {
                                killer.Inventory.Add(stream, ID, 1,0,0,0,Flags.Gem.NoSocket, Flags.Gem.NoSocket,false,Flags.ItemEffect.None,false,"!",0);
                                killer.SendSysMesage($"You got lucky, a monster you killed just dropped a Elite {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                                //Program.DiscordSpecialDrop.Enqueue("A Lucky Player " + killer.Player.Name + " has found Elite" + DbItem.Name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")!");
                            }
                            else if(ID_Quality == 9 && killer.Player.VipLevel == 6 && killer.Player.SuperLoot)
                            {
                                killer.Inventory.Add(stream, ID, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false, Flags.ItemEffect.None, false, "!", 0);
                                killer.SendSysMesage($"You got lucky, a monster you killed just dropped a Super {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                                //Program.DiscordSpecialDrop.Enqueue("A Lucky Player " + killer.Player.Name + " has found Super" + DbItem.Name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")!");
                            }
                            else 
                            {
                                if (killer.Map.AddGroundItem(ref xx, ref yy) && drop)
                                {
                                    if (ID_Quality == 8)
                                    {
                                        killer.SendSysMesage($"You got lucky, a monster you killed just dropped a Elite {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                                    }
                                    else if (ID_Quality == 9)
                                    {
                                        killer.SendSysMesage($"You got lucky, a monster you killed just dropped a Super {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                                    }
                                    DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, ID_Special, ID_Quality, killer, DbItem);
                                    if (ID_Special)
                                        break;
                                }
                            }
                            //if (ID_Quality == 9)
                            //{
                            //    killer.SendSysMesage($"You got lucky, a monster you killed just dropped a Super {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                            //}
                            //else if (ID_Quality == 8)
                            //{
                            //    killer.SendSysMesage($"You got lucky, a monster you killed just dropped a Elite {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                            //}
                            //else if (ID_Quality == 7)
                            //{
                            //    killer.SendSysMesage($"a monster you killed just dropped a Unique {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                            //}
                            //else if (ID_Quality == 6)
                            //{
                            //    killer.SendSysMesage($"a monster you killed just dropped a Refined/Rar {DbItem.Name} at ({xx},{yy}).", MsgMessage.ChatMode.System);
                            //}
                        }

                    }

                }
                //if (Program.DropRuleBase.Checkup(stream, DropRule.CheckUpType.Normal, killer, this))
                //{
                //    killer.MobsKilled++;
                //    if (killer.TotalMobsKilled < 10000)
                //    {
                //        killer.TotalMobsKilled++;
                //    }
                //    //if (killer.TotalMobsKilled2 < 300000)
                //    //{
                //    //    killer.TotalMobsKilled2++;
                //    //}
                //    //if (killer.TotalSouls < 15000)
                //    //{
                //    //    killer.TotalSouls++;
                //    //}
                //}

                #endregion

            }

        }
        public void DropItem2(ServerSockets.Packet stream, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, MsgFloorItem.MsgItem.ItemType typ
    , uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null, bool CHECKITEM = false, bool tomyself = true)
        {
            if (user != null)
            {
                if (user.Player.AutoHunting == AutoStructures.Mode.Enable && user.Player.VipLevel >= 6)
                {
                    if (ItemID == Database.ItemType.Meteor)
                    {
                        if (user.Inventory.HaveSpace(1))
                        {
                            user.Inventory.AddItemWitchStack(ItemID, 0, 1, stream);
                            if (user.Inventory.Contain(ItemID, 10))
                            {
                                user.Inventory.Remove(ItemID, 10, stream);
                                user.Inventory.Add(stream, 720027, 1);
                            }
                            return;
                        }
                    }
                }
            }

            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
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
            byte sockets = 0;
            bool lucky = false;
            if (typ == MsgFloorItem.MsgItem.ItemType.Item)
            {
                if (DataItem.IsEquip)
                {
                    if (DBItem != null)
                    {
                        if (!special)
                        {
                            lucky = (ID_Quality > 7);
                            //if (!lucky)
                            //    lucky = (DataItem.Plus = Family.ItemGenerator.GeneratePurity2()) != 0;
                            //if (!lucky)
                            //    lucky = (DataItem.Bless = Family.ItemGenerator.GenerateBless()) != 0;
                            if (!lucky)
                            {
                                if (DataItem.IsWeapon)
                                {
                                    sockets = Family.ItemGenerator.GenerateSocketCount(DataItem.ITEM_ID);
                                    if (sockets >= 1)
                                        DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                    else if (sockets == 2)
                                        DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                                }
                            }
                            if (ID_Quality > 5 || lucky || sockets > 0)
                            {
                                AddMapEffect(stream, XX, YY, "box_weapon");
                                AddMapEffect(stream, XX, YY, "endureXPdeath");
                                string extra = "";
                                if (DataItem.Plus > 0)
                                {
                                    extra += $"(+{DataItem.Plus}) ";
                                }
                                if (sockets > 0)
                                {
                                    extra += $"{DataItem.SocketOne} Soc ";
                                }
                                if (DataItem.Bless > 0)
                                {
                                    extra += $"-{DataItem.Bless} Bless ";
                                }
                                if (ID_Quality == 9)
                                {
                                    user.SendSysMesage($"You got lucky, a monster you killed just dropped a Super {DBItem.Name}{extra}Item at ({XX},{YY}).", MsgMessage.ChatMode.System);
                                }
                                else if (ID_Quality == 8)
                                {
                                    user.SendSysMesage($"You got lucky, a monster you killed just dropped a Elite {DBItem.Name}{extra}Item at ({XX},{YY}).", MsgMessage.ChatMode.System);
                                }
                                else if (ID_Quality == 7)
                                {
                                    user.SendSysMesage($"a monster you killed just dropped a Unique {DBItem.Name}{extra}Item at ({XX},{YY}).", MsgMessage.ChatMode.System);
                                }
                                else if (ID_Quality == 6)
                                {
                                    user.SendSysMesage($"a monster you killed just dropped a Refined/Rar {DBItem.Name}{extra}Item at ({XX},{YY}).", MsgMessage.ChatMode.System);
                                }
                                else if (extra != "")
                                {
                                    user.SendSysMesage($"a monster you killed just dropped a {DBItem.Name}{extra}Item at ({XX},{YY}).", MsgMessage.ChatMode.System);
                                }
                            }
                        }
                        if (Role.Core.Rate(3))
                        {
                            AddMapEffect(stream, XX, YY, "endureXPdeath");
                            DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                        }
                        else
                        {
                            DataItem.Durability = (ushort)Program.GetRandom.Next(1, DBItem.Durability / 2 + 10);
                            DataItem.MaximDurability = DBItem.Durability;
                        }
                    }
                }
                else
                {
                    if (DBItem != null)
                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                }
            }
            bool Drop = true;
            if (DataItem.ITEM_ID == 730001)
            {
                DataItem.Plus = 1;
            }
            else if (DataItem.ITEM_ID == 730002)
            {
                DataItem.Plus = 2;
            }
            else if (DataItem.ITEM_ID == 730003)
            {
                DataItem.Plus = 3;
            }
            else if (DataItem.ITEM_ID == 730004)
            {
                DataItem.Plus = 4;
            }
            else if (DataItem.ITEM_ID == 730005)
            {
                DataItem.Plus = 5;
            }
            else if (DataItem.ITEM_ID == 730006)
            {
                DataItem.Plus = 6;
            }
            else if (DataItem.ITEM_ID == 730007)
            {
                DataItem.Plus = 7;
            }
            else if (DataItem.ITEM_ID == 730008)
            {
                DataItem.Plus = 8;
            }
            else
            {
                if (DataItem.IsEquip && DataItem.Durability != DataItem.MaximDurability && !lucky && sockets == 0 && DataItem.Plus == 0 && ID_Quality < 6)
                    Drop = false;
            }
            if (Drop)
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, DynamicID, Map, OwnerItem, tomyself, map);

                if (map.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                }
            }
        }
        private void DropItem(Packet stream, uint OwnerItem, GameMap map, uint ItemID, ushort XX, ushort YY, MsgItem.ItemType typ, uint amount, bool special, byte ID_Quality, GameClient user = null, ItemType.DBItem DBItem = null)
        {
            MsgGameItem DataItem;
            DataItem = new MsgGameItem
            {
                ITEM_ID = ItemID
            };
            if (DataItem.Durability > 100)
            {
                DataItem.Durability = (ushort)ServerKernel.NextAsync(100, (int)DataItem.Durability / 10);
                DataItem.MaximDurability = DataItem.Durability;
            }
            else
            {
                DataItem.Durability = (ushort)ServerKernel.NextAsync(1, 10);
                DataItem.MaximDurability = 10;
            }
            DataItem.Color = Flags.Color.Red;
            if (typ == MsgItem.ItemType.Item)
            {
                if (DataItem.IsEquip)
                {
                    if (!special)
                    {
                        bool lucky;
                        lucky = ID_Quality > 7;
                        //if (!lucky)
                        //    lucky = (DataItem.Bless = Family.ItemGenerator.GenerateBless()) != 0;
                        if (!lucky && DataItem.IsWeapon)
                        {
                            byte sockets;
                            sockets = Family.ItemGenerator.GenerateSocketCount(DataItem.ITEM_ID);
                            if (sockets >= 1)
                                DataItem.SocketOne = Flags.Gem.EmptySocket;
                            else if (sockets == 2)
                            {
                                DataItem.SocketTwo = Flags.Gem.EmptySocket;
                            }
                        }
                    }
                    if (DBItem != null)
                    {
                        DataItem.Durability = (ushort)ServerKernel.NextAsync(1, (int)DBItem.Durability / 10 + 10);
                        DataItem.MaximDurability = (DataItem.MaximDurability = DBItem.Durability);
                    }
                }
                else if (DBItem != null)
                {
                    DataItem.Durability = (DataItem.MaximDurability = DBItem.Durability);
                }
            }
            MsgItem DropItem;
            DropItem = new MsgItem(DataItem, XX, YY, typ, amount, DynamicID, Map, OwnerItem, true, map);
            if (map.EnqueueItem(DropItem))
                DropItem.SendAll(stream, MsgDropID.Visible);
        }

        public void AddFadeAway(int time)
        {
            if (!Alive)
            {
                Time32 timer;
                timer = new Time32(time);
                if (timer > DeadStamp.AddSeconds(3) && AddFlag(MsgUpdate.Flags.FadeAway, 2592000, true))
                    FadeAway = timer;
            }
        }

        public unsafe bool RemoveView(int time, GameMap map)
        {
            if (ContainFlag(MsgUpdate.Flags.FadeAway) && State != MobStatus.Respawning)
            {
                Time32 timer;
                timer = new Time32(time);
                if (timer > FadeAway.AddSeconds(1))
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = UID;
                        actionQuery.Type = ActionType.RemoveEntity;
                        ActionQuery action;
                        action = actionQuery;
                        Send(stream.ActionCreate(&action));
                    }
                    State = MobStatus.Respawning;
                    map.View.MoveTo((IMapObj)this, (int)RespawnX, (int)RespawnY);
                    X = RespawnX;
                    Y = RespawnY;
                    Target = null;
                    return true;
                }
            }
            return false;
        }
        public bool DropItemID2(Client.GameClient killer, uint itemid, ServerSockets.Packet stream, byte range = 3, bool CHECKITEM = false)
        {
            if (itemid == 1088000 || itemid == 1088001)
            {
                if (!CHECKITEM)
                    return false;
            }
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
            DataItem.ITEM_ID = itemid;
            Database.ItemType.DBItem DBItem;
            if (Server.ItemsBase.TryGetValue(itemid, out DBItem))
            {
                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
            }
            DataItem.Color = Role.Flags.Color.Red;
            ushort xx = X;
            ushort yy = Y;
            bool drop = true;
            if (killer.Map.AddGroundItem(ref xx, ref yy, range))
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);
                if (itemid == 1088000)
                {
                    //ActionQuery action2;
                    //action2 = new ActionQuery()
                    //{
                    //    ObjId = killer.Player.UID,
                    //    Type = ActionType.DragonBall
                    //};
                    //killer.Send(stream.ActionCreate(&action2));
                    killer.SendSysMesage("A DragonBall dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage($"A Monster {Family.Name} killed by " + killer.Player.Name + " just dropped a DragonBall at (" + xx + "," + yy + ")!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                }
                if (itemid == 722057)
                {
                    killer.SendSysMesage("A PowerEXPBall dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage($"A Monster {Family.Name} killed by " + killer.Player.Name + " just dropped a PowerEXPBall at (" + xx + "," + yy + ")!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                }
                if (itemid == 1088001)
                {
                    killer.SendSysMesage("A Meteors dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                }
                if (itemid == 720549)
                {
                    killer.SendSysMesage("A RefPack dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                }
                if (killer.Map.EnqueueItem(DropItem) && drop)
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    return true;
                }
            }
            return false;
        }
        public unsafe bool DropItemID22(Client.GameClient killer, uint itemid, ServerSockets.Packet stream, byte range = 3, bool CHECKITEM = false)
        {
            if (itemid == 1088000 || itemid == 1088001)
            {
                if (!CHECKITEM)
                    return false;
            }
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
            DataItem.ITEM_ID = itemid;
            Database.ItemType.DBItem DBItem;
            if (Server.ItemsBase.TryGetValue(itemid, out DBItem))
            {
                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
            }
            DataItem.Color = Role.Flags.Color.Red;
            ushort xx = X;
            ushort yy = Y;
            bool drop = true;
            if (killer.Map.AddGroundItem(ref xx, ref yy, range))
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);
                if (itemid == 1088000)
                {
                    ActionQuery action2;
                    action2 = new ActionQuery()
                    {
                        ObjId = killer.Player.UID,
                        Type = ActionType.DragonBall
                    };
                    killer.Send(stream.ActionCreate(&action2));
                    killer.SendSysMesage("A DragonBall dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage($"A Monster {Family.Name} killed by " + killer.Player.Name + " just dropped a DragonBall at (" + xx + "," + yy + ")!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                }
                if (itemid == 722057)
                {
                    killer.SendSysMesage("A PowerEXPBall dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage($"A Monster {Family.Name} killed by " + killer.Player.Name + " just dropped a PowerEXPBall at (" + xx + "," + yy + ")!", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                }
                if (itemid == 1088001)
                {
                    killer.SendSysMesage("A Meteors dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                }
                if (itemid == 720549)
                {
                    killer.SendSysMesage("A RefPack dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                }
                killer.SendSysMesage("A " + DBItem.Name + " dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.Talk);
                if (killer.Map.EnqueueItem(DropItem) && drop)
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                    return true;
                }
            }
            return false;
        }

        public unsafe void DropItemID(GameClient killer, uint itemid, Packet stream, byte range = 1)
        {
            MsgGameItem DataItem;
            DataItem = new MsgGameItem
            {
                ITEM_ID = itemid
            };
            if (Server.ItemsBase.TryGetValue(itemid, out var DBItem))
                DataItem.Durability = (DataItem.MaximDurability = DBItem.Durability);
            DataItem.Color = Flags.Color.Red;
            ushort xx;
            xx = X;
            ushort yy;
            yy = Y;
            if (!killer.Map.AddGroundItem(ref xx, ref yy, range))
                return;
            MsgItem DropItem;
            DropItem = new MsgItem(DataItem, xx, yy, MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);
            bool drop;
            drop = true;
            switch (itemid)
            {
                #region drop mets
                case 1088001:
                    if (killer.Inventory.HaveSpace(1) && killer.Player.LootMeteor)
                    {
                        drop = false;
                        killer.Inventory.Add(stream, itemid, 1, 0, 0, 0);
                        if (killer.Player.VipLevel >= 4 && killer.Inventory.Contain(itemid, 10, 0))
                        {
                            killer.Inventory.Remove(itemid, 10, stream);
                            killer.Inventory.Add(stream, 720027, 1, 0, 0, 0);
                        }
                        killer.SendSysMesage($"You have got a(an) {DBItem.Name}.", MsgMessage.ChatMode.Monster);
                        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("A Lucky Player " + killer.Player.Name + " has found " + DBItem.Name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")! #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Tip).GetArray(stream));
                    }
                    else if (!killer.Inventory.HaveSpace(1) && killer.Player.Storage && killer.Player.VipLevel == 6)
                    {
                        byte i = 0;
                        foreach (var item in killer.Inventory.ClientItems.Values)
                        {
                            if (item.ITEM_ID == 720027)
                                i++;
                        }
                        if (i > 0)
                        {
                            if (killer.Player.VIPMetScrolls + i <= 255)
                            {
                                killer.Player.VIPMetScrolls += i;
                                for (int j = 0; j < i; j++)
                                    killer.Inventory.Remove(720027, i, stream);

                                killer.SendSysMesage("You successfully stored " + i + " [MetScrolls]! You have " + killer.Player.VIPMetScrolls + " [MetScrolls] in you storage!.", MsgMessage.ChatMode.System);

                            }
                            else
                                killer.SendSysMesage("You can't have more than 255 [MetScrolls] stored!", MsgMessage.ChatMode.System);

                        }


                    }
                    break;
                #endregion
                #region drop dragonball // work
                case 1088000:
                    {
                        if (killer.Inventory.HaveSpace(1) && killer.Player.LootDragonBall)
                        {
                            drop = false;
                            killer.Inventory.Add(stream, itemid, 1, 0, 0, 0);
                            if (killer.Player.VipLevel >= 4 && killer.Inventory.Contain(itemid, 10, 0))
                            {
                                killer.Inventory.Remove(itemid, 10, stream);
                                killer.Inventory.Add(stream, 720028, 1, 0, 0, 0);
                            }
                            killer.SendSysMesage($"You have got a(an) {DBItem.Name}.", MsgMessage.ChatMode.Monster);

                        }
                        else if (!killer.Inventory.HaveSpace(1) && killer.Player.Storage && killer.Player.VipLevel == 6)
                        {
                            byte i = 0;
                            foreach (var item in killer.Inventory.ClientItems.Values)
                            {
                                if (item.ITEM_ID == 720028)
                                    i++;
                            }
                            if (i > 0)
                            {
                                if (killer.Player.VIPDBscrolls + i <= 255)
                                {
                                    killer.Player.VIPDBscrolls += i;
                                    for (int j = 0; j < i; j++)
                                        killer.Inventory.Remove(720028, i, stream);

                                    killer.SendSysMesage("You successfully stored " + i + " [DbScroll]! You have " + killer.Player.VIPDBscrolls + " [DbScroll] in you storage!.", MsgMessage.ChatMode.System);

                                }
                                else
                                    killer.SendSysMesage("You can't have more than 255 [DbScroll] stored!", MsgMessage.ChatMode.System);

                            }
                            

                        }
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.Type = ActionType.DragonBall;
                        actionQuery.ObjId = killer.Player.UID;
                        actionQuery.wParam1 = killer.Player.X;
                        actionQuery.wParam2 = killer.Player.Y;
                        ActionQuery action;
                        action = actionQuery;
                        killer.Send(stream.ActionCreate(&action));
                        //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("A Lucky Player " + killer.Player.Name + " has found " + DBItem.Name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")! #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Tip).GetArray(stream));
                        //Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("[VIP]:{" + killer.Player.Name + "} has found a DragonBall.#52#52", "ALLUSERS", "[Drop]", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        //if (killer.Player.VipLevel < 6)
                        //Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgServer.MsgMessage("{" + killer.Player.Name + "} has found a DragonBall", MsgColor.white, ChatMode.TopLeft).GetArray(stream));
                        //else
                        //Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgServer.MsgMessage("[VIP]:{" + killer.Player.Name + "} has found a DragonBall.#52#52", MsgColor.white, ChatMode.Center).GetArray(stream));

                        //killer.SendSysMesage($"#07#42 [VIP]  As Lucky  " + killer.Player.Name + " he/she found " + DBItem.Name + " ", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.white, true);

                        break;

                    }
                #endregion
                #region drop expball
                case 722136://ExpBall(Event)
                    if (killer.Inventory.HaveSpace(1) && killer.Player.LootExpBall)
                    {
                        drop = false;
                        killer.Inventory.AddItemWitchStack(722136, 0, 1, stream);
                        if (killer.Player.VipLevel >= 4 && killer.Inventory.Contain(722136, 30, 0))
                        {
                            killer.Inventory.Remove(722136, 30, stream);
                            killer.Inventory.Add(stream, 720757, 1, 0, 0, 0);
                        }
                        killer.SendSysMesage($"You have got a(an) {DBItem.Name}.", MsgMessage.ChatMode.Monster);
                    }
                    break;
                #endregion
                #region drop Conquer latter
                case 711214://latter
                case 711215:
                case 711216:
                case 711217:
                case 711218:
                case 711219:
                case 711220:
                    if (killer.Inventory.HaveSpace(1) && killer.Player.LootLetters)
                    {
                        drop = false;
                        killer.Inventory.AddItemWitchStack(itemid, 0, 1, stream);
                        killer.SendSysMesage($"You have got a(an) {DBItem.Name}.", MsgMessage.ChatMode.Monster);
                    }
                    break;
                #endregion
                #region drop Prof token
                case 722384:
                    if (killer.Inventory.HaveSpace(1) && killer.Player.ProfTokenLoot)
                    {
                        drop = false;
                        killer.Inventory.AddItemWitchStack(itemid, 0, 1, stream);
                        killer.SendSysMesage($"You have got a(an) {DBItem.Name}.", MsgMessage.ChatMode.Monster);
                    }
                    break;
                #endregion
                #region drop item +1 / +2
                case 727384:
                case 727385:
                    if (killer.Inventory.HaveSpace(1) && killer.Player.ItemPlusLoot)
                    {
                        if (DBItem.ID == 727385)
                        {
                            drop = false;
                            uint[] ItemTyper2;
                            ItemTyper2 = new uint[36]
                            {
                                151013, 152013, 410003, 420003, 421003, 430003, 440003, 450003, 460003, 480003,
                                481003, 490003, 500003, 601003, 510003, 530003, 560003, 561003, 580003, 900003,
                                130003, 131003, 141030, 133003, 134003, 150003, 142003, 120003, 121003, 111003,
                                160033, 113003, 114003, 117003, 118003, 121003
                            };
                            uint dwItemSort2;
                            dwItemSort2 = ItemTyper2[ServerKernel.NextAsync(0, ItemTyper2.Length)];
                            //uint idItemType2;
                            //idItemType2 = dwItemSort2 * 1000 + 3;
                            killer.Inventory.Add(stream, dwItemSort2, 1, 1, 0, 0);
                            string name2;
                            name2 = Server.ItemsBase.GetItemName(dwItemSort2);
                            pluItem = name2;
                            killer.SendSysMesage("You got a (+1)" + name2 + "! Check your inventory!");//drop item +1
                            if (killer.Player.NotifTogggle)
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("A Lucky Player " + killer.Player.Name + " has found (+1)" + name2 + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")! #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                        else if (DBItem.ID == 727384)
                        {
                            drop = false;
                            uint[] ItemTyper;
                            ItemTyper = new uint[36]
                            {
                                151013, 152013, 410003, 420003, 421003, 430003, 440003, 450003, 460003, 480003,
                                481003, 490003, 500003, 601003, 510003, 530003, 560003, 561003, 580003, 900003,
                                130003, 131003, 141030, 133003, 134003, 150003, 142003, 120003, 121003, 111003,
                                160033, 113003, 114003, 117003, 118003, 121003
                            };
                            uint dwItemSort;
                            dwItemSort = ItemTyper[ServerKernel.NextAsync(0, ItemTyper.Length)];
                            //uint idItemType;
                            //idItemType = dwItemSort * 1000 + 3;
                            killer.Inventory.Add(stream, dwItemSort, 1, 2, 0, 0);
                            string name;
                            name = Server.ItemsBase.GetItemName(dwItemSort);
                            killer.SendSysMesage("You got a (+2)" + name + "! Check your inventory!");//drop item +2
                            if (killer.Player.NotifTogggle)
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("A Lucky Player " + killer.Player.Name + " found (+1)" + name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")! #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Tip).GetArray(stream));
                        }
                    }
                    break;
                #endregion
                #region drop stone +1/+2
                //case 730001:
                //case 730002:
                //    if (killer.Inventory.HaveSpace(1) && killer.Player.StonesLoot)
                //    {
                //        drop = false;
                //        killer.Inventory.Add(stream, itemid, 1, 0, 0, 0);
                //        if (killer.Player.VipLevel >= 4)
                //        {
                //            if (killer.Inventory.Contain(730001, 5, 0))
                //            {
                //                killer.Inventory.Remove(730001, 5, stream);
                //                killer.Inventory.Add(stream, 723712, 1, 0, 0, 0);
                //            }
                //            if (killer.Inventory.Contain(730002, 5, 0))
                //            {
                //                killer.Inventory.Remove(730002, 5, stream);
                //                killer.Inventory.Add(stream, 727347, 1, 0, 0, 0);
                //            }

                //        }
                //    }
                //    else if (!killer.Inventory.HaveSpace(1) && killer.Player.Storage && killer.Player.VipLevel == 6)
                //    {
                //        byte i = 0;
                //        foreach (var item in killer.Inventory.ClientItems.Values)
                //        {
                //            if (item.ITEM_ID == 723712)
                //                i++;
                //        }
                //        if (i > 0)
                //        {
                //            if (killer.Player.VIPStone + i <= 255)
                //            {
                //                killer.Player.VIPStone += i;
                //                for (int j = 0; j < i; j++)
                //                    killer.Inventory.Remove(723712, i, stream);

                //                killer.SendSysMesage("You successfully stored " + i + " [MetScrolls]! You have " + killer.Player.VIPStone + " [MetScrolls] in you storage!.", MsgMessage.ChatMode.System);

                //            }
                //            else
                //                killer.SendSysMesage("You can't have more than 255 [MetScrolls] stored!", MsgMessage.ChatMode.System);

                //        }


                //    }
                //    //killer.SendSysMesage($"You have got a(an) { DBItem.Name}.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white, false);
                //    //if (killer.Player.VipLevel < 6)
                //    //    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("{" + killer.Player.Name + "} he/she found " + DBItem.Name + " ", "ALLUSERS", "[Drop]", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                //    //else
                //    //    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("#07#42 [VIP]  As Lucky  " + killer.Player.Name + " he/she found " + DBItem.Name + " ", "ALLUSERS", "[Drop]", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                //    //killer.SendSysMesage($"#07#42 [VIP]  As Lucky  " + killer.Player.Name + " he/she found " + DBItem.Name + " ", MsgMessage.ChatMode.Center, MsgMessage.MsgColor.white, true);


                //    break;
                #endregion
                #region drop Gems
                //case 700001:
                //case 700011:
                //case 700021:
                //case 700031:
                //case 700041:
                //case 700051:
                //case 700061:
                //case 700071:
                ////case 700101:
                ////case 700121:
                //    if (!killer.Inventory.HaveSpace(1) || !killer.Player.GemsLoot)
                //        break;
                //    drop = false;
                //    killer.Inventory.AddItemWitchStack(itemid, 0, 1, stream);
                //    if (killer.Player.VipLevel >= 4)
                //    {
                //        if (killer.Inventory.Contain(700121, 5, 0))
                //        {
                //            killer.Inventory.Remove(700121, 5, stream);
                //            killer.Inventory.Add(stream, 727060, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700101, 5, 0))
                //        {
                //            killer.Inventory.Remove(700101, 5, stream);
                //            killer.Inventory.Add(stream, 727061, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700041, 5, 0))
                //        {
                //            killer.Inventory.Remove(700041, 5, stream);
                //            killer.Inventory.Add(stream, 727062, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700031, 5, 0))
                //        {
                //            killer.Inventory.Remove(700031, 5, stream);
                //            killer.Inventory.Add(stream, 727063, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700021, 5, 0))
                //        {
                //            killer.Inventory.Remove(700021, 5, stream);
                //            killer.Inventory.Add(stream, 727064, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700011, 5, 0))
                //        {
                //            killer.Inventory.Remove(700011, 5, stream);
                //            killer.Inventory.Add(stream, 727065, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700001, 5, 0))
                //        {
                //            killer.Inventory.Remove(700001, 5, stream);
                //            killer.Inventory.Add(stream, 727066, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700051, 5, 0))
                //        {
                //            killer.Inventory.Remove(700051, 5, stream);
                //            killer.Inventory.Add(stream, 727067, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700061, 5, 0))
                //        {
                //            killer.Inventory.Remove(700061, 5, stream);
                //            killer.Inventory.Add(stream, 727068, 1, 0, 0, 0);
                //        }
                //        if (killer.Inventory.Contain(700071, 5, 0))
                //        {
                //            killer.Inventory.Remove(700071, 5, stream);
                //            killer.Inventory.Add(stream, 727069, 1, 0, 0, 0);
                //        }
                //    }
                //    killer.SendSysMesage($"You have got a(an) { DBItem.Name}.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white, false);

                //    break;
                #endregion
            }
            if (killer.Map.EnqueueItem(DropItem) && drop)
            {
                //AddMapEffect(stream, (ushort)(xx - 1), (ushort)(yy + 1), "box_weapon");
                //AddMapEffect(stream, xx, yy, "DropEffect");
                //AddMapEffect(stream, xx, yy, "attackup20");
                //AddMapEffect(stream, xx, yy, "endureXPdeath");
                //AddMapEffect(stream, xx, yy, "ssch_wlhd_hit");

                if (DropItem.ItemBase.Plus > 0 || DBItem.ID == 727385 || DBItem.ID == 727384)
                    AddMapEffect(stream, (ushort)(xx - 1), (ushort)(yy - 1), "endureXPdeath");

                if (DropItem.ItemBase.ITEM_ID == 1088000)
                    AddMapEffect(stream, (ushort)(xx - 1), (ushort)(yy - 1), "ssch_wlhd_hit");

                if (DropItem.ItemBase.ITEM_ID == 1088001)
                    AddMapEffect(stream, (ushort)(xx - 1), (ushort)(yy - 1), "ssch_wlhd_fir");

                if(DBItem.ID != 727385 && killer.Player.Map != 1700)
                {
                    if(killer.Player.NotifTogggle)
                        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("A Lucky Player " + killer.Player.Name + " has found " + DBItem.Name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")! #07 #07", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                }
                
                //if (DBItem.ID != 727385 && killer.Player.Map != 1700)
                    //Program.DiscordSpecialDrop.Enqueue("A Lucky Player " + killer.Player.Name + " has found " + DBItem.Name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")!");
                //else if(killer.Player.Map != 1700)
                    //Program.DiscordSpecialDrop.Enqueue("A Lucky Player " + killer.Player.Name + " has found (+1)" + pluItem + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")!");
                //killer.SendSysMesage("A " + DBItem.Name + " dropped at (" + xx + "," + yy + ")!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.white, false);
                DropItem.SendAll(stream, MsgDropID.Visible);

            }

            if(killer.Map.ID == 1700 && DropItem.UID == 723701)
            {
                //Program.DiscordSpecialDrop.Enqueue("A Lucky Player " + killer.Player.Name + " has found " + DBItem.Name + " dropped at " + killer.Map.Name + " (" + xx + "," + yy + ")!");
            }
        }
        public void AddMapEffect(ServerSockets.Packet stream, ushort x, ushort y, params string[] effect)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = MsgStringPacket.StringID.LocationEffect;
            packet.X = x;
            packet.Y = y;
            packet.Strings = effect;
            View.SendScreen(stream.StringPacketCreate(packet), GMap);
        }
        public void AddSpellFlag(MsgUpdate.Flags Flag, int Seconds, bool RemoveOnDead, int Secondstamp = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Seconds, RemoveOnDead, Secondstamp);
        }

        public bool AddFlag(MsgUpdate.Flags Flag, int Seconds, bool RemoveOnDead, int StampSeconds = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Seconds, RemoveOnDead, StampSeconds);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }

        public bool RemoveFlag(MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }

        public bool UpdateFlag(MsgUpdate.Flags Flag, int Seconds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Seconds, SetNewTimer, MaxTime);
        }

        public void ClearFlags(bool SendScreem = false)
        {
            BitVector.GetClear();
            UpdateFlagOffset(SendScreem);
        }

        public bool ContainFlag(MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }

        private void UpdateFlagOffset(bool SendScreem = true)
        {
            if (SendScreem)
                SendUpdate(BitVector.bits, MsgUpdate.DataType.StatusFlag);
        }

        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(X, Y, X2, Y2);
        }

        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(PX, PY, X2, Y2);
        }

        public bool InView(ushort X2, ushort Y2, byte distance)
        {
            if (OldGetDistance(X2, Y2) >= distance)
                return GetMyDistance(X2, Y2) < distance;
            return false;
        }

        public void Send(Packet msg)
        {
            View.SendScreen(msg, GMap);
            SendScores(msg);
        }

        public void UpdateMonsterView(Role.RoleView Target, ServerSockets.Packet stream)
        {
            foreach (var player in View.Roles(GMap, Role.MapObjectType.Player))
            {
                if (InView(player.X, player.Y, MonsterView.ViewThreshold))
                    player.Send(GetArray(stream, false));
            }
        }

        //public void UpdateMonsterView(Packet stream)
        //{
        //    foreach (IMapObj player in View.Roles(GMap, MapObjectType.Player))
        //    {
        //        if (InView(player.X, player.Y, 18))
        //            player.Send(GetArray(stream, false));
        //    }
        //}
        public void UpdateMonsterView2(Role.RoleView Target, ServerSockets.Packet stream)
        {
            foreach (var player in View.Roles(GMap, Role.MapObjectType.Player))
            {
                if (InView(player.X, player.Y, MonsterView.ViewThreshold))
                    player.Send(GetArray(stream, false));
            }
        }
        public bool UpdateMapCoords(ushort New_X, ushort New_Y, GameMap _map)
        {
            if (!_map.IsFlagPresent(New_X, New_Y, MapFlagType.Monster))
            {
                _map.SetMonsterOnTile(X, Y, false);
                _map.SetMonsterOnTile(New_X, New_Y, true);
                _map.View.MoveTo(this, New_X, New_Y);
                X = New_X;
                Y = New_Y;
                return true;
            }
            return false;
        }

        public void SendString(Packet stream, MsgStringPacket.StringID id, params string[] args)
        {
            MsgStringPacket packet;
            packet = new MsgStringPacket
            {
                ID = id,
                UID = UID,
                Strings = args
            };
            Send(stream.StringPacketCreate(packet));
        }

        public MonsterRole(MonsterFamily Famil, uint _UID, string locationspawn, GameMap _map)
        {
            AllowDynamic = false;
            GMap = _map;
            LocationSpawn = locationspawn;
            ObjType = MapObjectType.Monster;
            Name = Famil.Name;
            Family = Famil;
            UID = _UID;
            Mesh = Famil.Mesh;
            Level = (byte)Famil.Level;
            HitPoints = (uint)Famil.MaxHealth;
            View = new MonsterView(this);
            State = MobStatus.Idle;
            BitVector = new StatusFlagsBigVector32(128);
            Boss = Family.Boss;
            Facing = (Flags.ConquerAngle)ServerKernel.NextAsync(0, 8);
        }

        public Packet GetArray(Packet stream, bool view)
        {
            if (IsFloor && Mesh != 980)
                return stream.ItemPacketCreate(FloorPacket);
            stream.InitWriter();
            stream.Write(Mesh);
            stream.Write(UID);
            stream.ZeroFill(10);
            for (int x = 0; x < BitVector.bits.Length; x++)
            {
                stream.Write(BitVector.bits[x]);
            }
            stream.ZeroFill(42);
            if (Boss > 0)
            {
                if (IsFloor)
                    stream.Write(StampFloorSeconds);
                else
                {
                    uint key;
                    key = (ushort)(Family.MaxHealth / 10000);
                    if (key != 0)
                        stream.Write((ushort)(HitPoints / key));
                    else
                        stream.Write((ushort)(HitPoints * Family.MaxHealth));
                }
            }
            else if (IsFloor)
            {
                stream.Write((ushort)StampFloorSeconds);
            }
            else
            {
                stream.Write((ushort)HitPoints);
            }
            stream.Write((ushort)Level);
            stream.Write((ushort)0);
            stream.Write(X);
            stream.Write(Y);
            stream.Write((byte)Facing);
            stream.Write((byte)Action);
            stream.ZeroFill(89);
            stream.Write(Boss);
            stream.ZeroFill(36);
            stream.Write(Name, string.Empty, string.Empty, string.Empty);
            stream.Finalize(10014);
            return stream;
        }

        public void SendUpdate(uint[] Value, MsgUpdate.DataType datatype)
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgUpdate packet;
                packet = new MsgUpdate(stream, UID);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Send(stream);
            }
        }

        internal void SendScores(Packet stream)
        {
            if (DateTime.Now < LastScore.AddSeconds(2.0))
                return;
            LastScore = DateTime.Now;
            if (!ConfirmBoss())
                return;
            View.SendScreen(new MsgMessage("*Top 5 ScoreBoard: " + Name + " *", MsgMessage.MsgColor.red, MsgMessage.ChatMode.FirstRightCorner).GetArray(stream), GMap);
            int counter;
            counter = 1;
            foreach (KeyValuePair<uint, ScoreBoard> player in Scores.OrderByDescending((KeyValuePair<uint, ScoreBoard> e) => e.Value.ScoreDmg).Take(5))
            {
                View.SendScreen(new MsgMessage("N° " + counter++ + ": " + player.Value.Name + " - " + player.Value.ScoreDmg, MsgMessage.MsgColor.red, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream), GMap);
            }
        }

        private bool ConfirmBoss()
        {
            if (BossDatabase.Bosses.ContainsKey(Family.ID))
                return true;
            return false;
        }

        internal void DistributeBossPoints(uint cps, uint itemprize, uint coins)
        {
            if (!ConfirmBoss())
                return;
            byte x;
            x = 1;
            foreach (KeyValuePair<uint, ScoreBoard> player in Scores.OrderByDescending((KeyValuePair<uint, ScoreBoard> e) => e.Value.ScoreDmg).Take(3))
            {
                foreach (GameClient pclient in Server.GamePoll.Values)
                {
                    if (!(pclient.Player.Name == player.Value.Name))
                        continue;
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        pclient.Inventory.Add(stream, itemprize, 1, 0, 0, 0);
                        //if (Core.Rate(50))
                        {
                            pclient.Player.BossPoints += (int)coins;
                            pclient.CreateBoxDialog("You got " + coins + " Points!");

                        }
                        int min;
                        min = (int)((double)cps * 0.5);
                        int value;
                        value = (int)ServerKernel.NextAsync(min, (int)cps);
                        //pclient.Player.ConquerPoints += value;
                        //pclient.SendSysMesage("You got SurpriseBox and " + value + " Conquer Points!", MsgMessage.ChatMode.System);
                        string rank;
                        rank = "";
                        switch (x)
                        {
                            case 1:
                                rank = "1st";
                                pclient.DbDailyTraining.OneGreatBoss++;
                                string msg1 = $"{pclient.Player.Name} has won " + value + " ConquerMoney, SurpriseBox and Compelet daily sessions for get rank " + rank + " by hight score damge in boardscore boss.";
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg1, Game.MsgServer.MsgMessage.MsgColor.yellow, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                                break;
                            case 2:
                                rank = "2nd";
                                break;
                            case 3:
                                rank = "3rd";
                                break;
                        }
                        string MSG;
                        MSG = "Congratulation to " + pclient.Player.Name + " ! he/she managed to get rank " + rank + " on Boss and claimed SurpriseBox!";
                        Program.SendGlobalPackets.Enqueue(new MsgMessage(MSG, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
                    }
                    x = (byte)(x + 1);
                }
            }
        }

        internal void UpdateScores(Player player, uint p)
        {
            if (ConfirmBoss())
            {
                if (!Scores.ContainsKey(player.UID))
                    Scores.Add(player.UID, new ScoreBoard
                    {
                        Name = player.Name,
                        ScoreDmg = p
                    });
                else
                    Scores[player.UID].ScoreDmg += p;
            }
        }
        public void DropItemNull(uint Itemid, ServerSockets.Packet stream, MsgItem.ItemType type = MsgItem.ItemType.Item, uint gold = 0)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
            DataItem.ITEM_ID = Itemid;
            var DBItem = Database.Server.ItemsBase[Itemid];
            DataItem.Durability = DBItem.Durability;
            DataItem.MaximDurability = DBItem.Durability;
            DataItem.Color = Role.Flags.Color.Red;
            ushort xx = X;
            ushort yy = Y;
            if (GMap.AddGroundItem(ref xx, ref yy, 5))
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, (ushort)(xx - Program.GetRandom.Next(5)), (ushort)(yy - Program.GetRandom.Next(5)), type, gold, 0, Map, 0, false, GMap);
                if (GMap.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                }
            }
        }
    }
}
