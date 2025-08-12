using System;
using System.Linq;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgFloorItem;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Database;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using Extensions;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using TheChosenProject.Game.MsgAutoHunting;
using System.Collections.Generic;
using static TheChosenProject.Game.MsgServer.MsgWarehouse;
using System.IO;
using TheChosenProject.Game.MsgMonster;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Xpo.DB;
using static DevExpress.XtraEditors.RoundedSkinPanel;
using TheChosenProject.Struct;
using TheChosenProject.Ai;

namespace TheChosenProject.Client
{
    public class PoolProcesses
    {
        private static bool Valid(GameClient client)
        {
            if (!client.Socket.Alive || client.Player == null
                || client == null || client.Player.View == null || !client.FullLoading)
            {
                client.Socket.Disconnect();
                return false;
            }
            return true;
        }

        public static unsafe void AiThread(Client.GameClient client)
        {
            if (!client.FullLoading)
                return;
            if (!Valid(client))
                return;
            var Now = Time32.Now;

            if (client.Pet != null && client.Pet.monster != null)
            {
                short distance = Core.GetDistance(client.Pet.monster.X, client.Pet.monster.Y, client.Player.X, client.Player.Y);
                if (distance >= 8)
                {
                    ushort X = (ushort)(client.Player.X + Program.GetRandom.Next(2, 5));
                    ushort Y = (ushort)(client.Player.Y + Program.GetRandom.Next(2, 5));
                    if (!client.Map.ValidLocation(X, Y))
                    {
                        X = client.Player.X;
                        Y = client.Player.Y;
                    }


                    var data = new ActionQuery()
                    {
                        Type = ActionType.Jump,
                        dwParam = (uint)((Y << 16) | X),
                        wParam1 = X,
                        wParam2 = Y,
                        ObjId = client.Pet.monster.UID

                    };

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (stream != null)
                        {
                            client.Player.View.SendView(stream.ActionCreate(&data), true);
                            client.Pet.monster.Facing = Role.Core.GetAngle(client.Pet.monster.X, client.Pet.monster.Y, X, Y);
                            client.Pet.monster.Action = Role.Flags.ConquerAction.Jump;
                            client.Map.View.MoveTo<Role.IMapObj>(client.Pet.monster, X, Y);
                            client.Pet.monster.X = X;
                            client.Pet.monster.Y = Y;
                            client.Pet.monster.UpdateMonsterView2(null, stream);
                        }
                    }
                }
                else if (distance > 4)
                {
                    var facing = Role.Core.GetAngle(client.Pet.monster.X, client.Pet.monster.Y, client.Player.X, client.Player.Y);
                    if (!client.Pet.Move(facing))
                    {
                        var x = client.Pet.monster.X;
                        var y = client.Pet.monster.Y;
                        facing = (Flags.ConquerAngle)Program.GetRandom.Next(7);
                        if (client.Pet.Move(facing))
                        {
                            client.Pet.monster.Facing = facing;
                            var move = new WalkQuery()
                            {
                                Direction = (uint)facing,
                                UID = client.Pet.monster.UID,
                                Running = 1
                            };

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (stream != null)
                                {
                                    client.Player.View.SendView(stream.MovementCreate(&move), true);
                                    client.Map.View.MoveTo<Role.IMapObj>(client.Pet.monster, x, y);
                                    client.Pet.monster.X = x;
                                    client.Pet.monster.Y = y;
                                    client.Pet.monster.Facing = facing;

                                    client.Player.View.Role(false, stream.MovementCreate(&move));
                                    client.Pet.monster.UpdateMonsterView2(null, stream);
                                }
                            }
                        }
                    }
                    else
                    {
                        client.Pet.monster.Facing = facing;
                        var x = client.Pet.monster.X;
                        var y = client.Pet.monster.Y;
                        var move = new WalkQuery()
                        {
                            Direction = (uint)facing,
                            UID = client.Pet.monster.UID,
                            Running = 1
                        };

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (stream != null)
                            {
                                client.Player.View.SendView(stream.MovementCreate(&move), true);
                                client.Map.View.MoveTo<Role.IMapObj>(client.Pet.monster, x, y);
                                client.Pet.monster.X = x;
                                client.Pet.monster.Y = y;
                                client.Pet.monster.Facing = facing;

                                client.Player.View.Role(false, stream.MovementCreate(&move));
                                client.Pet.monster.UpdateMonsterView2(null, stream);
                            }
                        }
                    }
                }
                else
                {
                    var monster = client.Pet;
                    if (monster.Target != null && monster.Target.UID != client.Player.UID)
                    {

                        short dis = MonsterView.GetDistance(monster.monster.X, monster.monster.Y, monster.Target.X, monster.Target.Y);
                        if (dis <= 13)
                        {
                            if (monster.Target.Alive)//bahaa
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();

                                    var SpellPacket = new MsgSpellAnimation(monster.monster.UID, 0, monster.Target.X, monster.Target.Y, (ushort)monster.Family.SpellId, 0, 0);
                                    uint Damage = 0;
                                    if (monster.Target is MonsterRole)
                                    {
                                        var tar = monster.Target as MonsterRole;
                                        Damage = ActionHandler.PhysicalAttackPet(monster.Owner, tar);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Monster.ExecutePet(stream, (uint)Damage, monster.Owner, tar);
                                        Game.MsgServer.AttackHandler.Updates.IncreaseExperience.Up(stream, monster.Owner, Damage);

                                    }
                                    else if (monster.Target is Player)
                                    {
                                        /*
                                         var arrays = client.Map.View.GetAllMapRoles(MapObjectType.Player).ToArray();
                                        foreach (Role.Player P in arrays)
                                        {
                                            if (!P.ContainFlag(MsgUpdate.Flags.Stigma))
                                            {
                                                if (Core.GetDistance(P.X, P.Y, client.Player.X, client.Player.Y) <= 10)
                                                {
                                                    InteractQuery AttackPaket = new InteractQuery
                                                    {
                                                        OpponentUID = P.UID,
                                                        UID = client.Player.UID,
                                                        X = P.X,
                                                        Y = P.Y,
                                                        SpellID = (ushort)1095
                                                    };
                                                    AttackPaket.AtkType = MsgAttackPacket.AttackID.Magic;
                                                    MsgAttackPacket.ProcescMagic(client, stream, AttackPaket, true);

                                                    
                                                }
                                            }
                                        } 
                                        */
                                        var tar = monster.Target as Player;
                                        Damage = ActionHandler.PhysicalAttackPet(monster.Owner, tar.Owner);
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.ExecutePet((int)Damage, monster.Owner, tar);
                                    }
                                    else if (monster.Target is SobNpc)
                                    {
                                        return;
                                        //var tar = monster.Target as SobNpc;
                                        //Damage = (uint)monster.monster.Family.MaxAttack * 2;
                                        //Game.MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(stream, new MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgAttackPacket.AttackEffect.None), monster.Owner, tar);
                                    }
                                    SpellPacket.Targets.Enqueue(new MsgSpellAnimation.SpellObj(monster.Target.UID, Damage, MsgAttackPacket.AttackEffect.None));

                                        SpellPacket.SetStream(stream);
                                        SpellPacket.Send(monster.Owner);
                                        SpellPacket.Send(monster.monster);
                                }
                            }
                            else monster.monster.Target = null;
                        }
                    }
                }
            }
        }

        public static void CheckSeconds(GameClient client, Time32 timer)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || !client.Player.CompleteLogin)
                    return;
                //MsgProtect.ProGuardHandler.OnThread(client);               
                if (Program.MonsterCity.Count > 0)
                {
                    if (DateTime.Now > client.StampMonsterCityAlret.AddSeconds(5))
                    {
                        var MobCity = Program.MonsterCity.Values.FirstOrDefault(x => x != null && x.MapID == client.Player.Map);
                        MobCity?.SendAlret(client);
                        client.StampMonsterCityAlret = DateTime.Now;
                    }
                }
                #region save
                RoyalPassManager.Save(client);
                DbKillMobsExterminator.Save(client);
                OnlinePointsManager.Save(client);
                TournamentsManager.Save(client);
                LimitedDailyTimes.Save(client);
                DbDailyTraining.Save(client);
                #endregion
                #region GuildBeast
                if (DateTime.Now.Hour == 20 && DateTime.Now.Minute == 00 && client.Player.SpawnGuildBeast)
                {
                    var Map = Database.Server.ServerMaps[1038];

                    if (!Map.ContainMobID(3122))
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Warning! The GuildBeast has appeared in the GuildCastle! Everyone shall gather their weapons and fight it!", "ALLUSERS", "Server", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            Database.Server.AddMapMonster(stream, Map, 3122, 87, 77, 1, 1, 1);
                        }
                    }
                }
                else if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 00 && client.Player.SpawnGuildBeast)
                {
                    var Map = Database.Server.ServerMaps[1038];
                    if (Map.GetMobLoc(3122) != null)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            var Array = Map.View.GetAllMapRoles(Role.MapObjectType.Monster);
                            foreach (var map_mob in Array)
                            {
                                if (map_mob.Map == 1038 && map_mob.Alive)
                                {
                                    var Monster = (map_mob as Game.MsgMonster.MonsterRole);
                                    if (Monster.Name == "GuildBeast")
                                    {
                                        Monster.Dead(stream, client, 3122, Monster.GMap);
                                       
                                        client.Player.SpawnGuildBeast = false;
                                        client.Player.GuildBeastClaimd = true;
                                    }
                                }

                            }

                        }
                    }

                }
                
                #endregion
                #region royalpass check
                if (client.Player.Map == 1121)
                {
                    if (AtributesStatus.IsWater(client.Player.Class))
                    {
                        using (RecycledPacket recycledPacket6 = new RecycledPacket())
                        {
                            Packet stream5;
                            stream5 = recycledPacket6.GetStream();
                            client.Player.SendString(stream5, MsgStringPacket.StringID.Effect, true, "hunpo02");
                        }
                        if (client.Player.StampKick.AddSeconds(10) < timer)
                        {
                            client.Teleport(428, 378, 1002);
                            client.CreateBoxDialog("waters not allowed in this map, cya next time.");
                            client.Player.StampKick = timer;

                        }
                    }
                }
                #endregion
                //using (var rec = new ServerSockets.RecycledPacket())
                //{
                //    var stream = rec.GetStream();
                //    client.UpdateEquipsLvl(client, stream);
                //}
                if (client.Player.Map == 601 && !client.Map.ValidLocation(client.Player.X, client.Player.Y))
                    client.Teleport(64, 56, 601);
                if (DateTime.Now > client.Player.ExpireVip && client.Player.VipLevel > 4)
                {
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream6;
                        stream6 = recycledPacket.GetStream();
                        client.Player.VipLevel = 1;

                        client.Player.SendUpdate(stream6, client.Player.VipLevel, MsgUpdate.DataType.VIPLevel);
                        client.Player.UpdateVip(stream6);
                        client.Player.LootDragonBall = (client.Player.LootExpBall = (client.Player.LootLetters = (client.Player.LootMeteor = (client.Player.StonesLoot = (client.Player.ItemPlusLoot = (client.Player.GemsLoot = (client.Player.TabAllyInvisible = (client.Player.TabEnemyInvisible = false))))))));
                        if (client.Player.ContainFlag(MsgUpdate.Flags.VIP))
                            client.Player.RemoveFlag(MsgUpdate.Flags.VIP);
                        client.CreateBoxDialog("Your VIPLevel expired!");
                        if (client.Player.OfflineTraining == MsgOfflineTraining.Mode.Hunting)
                        {
                            client.Player.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                            client.Socket.Disconnect("Offline~Hunter~completed");
                            //client.Socket.Disconnect(client.Player.OfflineTraining.ToString());
                        }
                        if (client.Player.AutoHunting == AutoStructures.Mode.Enable)
                        {
                            client.Player.AutoHunting = AutoStructures.Mode.Disable;
                        }
                        if (client.Player.ContainFlag(MsgUpdate.Flags.OfflineMode))
                        {
                            client.Player.RemoveFlag(MsgUpdate.Flags.OfflineMode);
                        }
                        //client.Player.UpdateSurroundings(stream6, clear: false, true);

                    }
                }
                if (DateTime.Now > client.Player.VendorTime)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        DataVendor.TimerVendor(client, stream);
                    }
                }
                if (client.Player.SpecialTitles.Count > 0)
                {
                    foreach (KeyValuePair<MsgTitle.TitleType, DateTime> title in client.Player.SpecialTitles.ToList())
                    {
                        if (DateTime.Now > title.Value)
                        {
                            using (RecycledPacket recycledPacket2 = new RecycledPacket())
                            {
                                Packet stream9;
                                stream9 = recycledPacket2.GetStream();
                                client.Player.RemoveSpecialTitle(title.Key, stream9);
                                client.Player.SwitchTitle(0);
                                client.CreateBoxDialog("Your title (" + SpecialTitles.Titles[(uint)title.Key].Name + ") expired!");
                            }
                        }
                    }
                }
                if (client.EventBase != null)
                {
                    var events = Program.Events.Find(x => x.EventTitle == client.EventBase.EventTitle);
                    if (events != null)
                    {
                        if (events.Stage == Game.MsgEvents.EventStage.Fighting)
                        {
                            events.CharacterChecks(client);
                            if (events.InTournament(client))
                            {
                                if (events.ReviveAllowed)
                                {
                                    if (!client.Player.Alive)
                                    {
                                        events.RevivePlayer(client);
                                    }
                                }
                            }
                        }
                    }
                }
                //if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Process == Game.MsgTournaments.ProcesType.Alive)
                //{
                //    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.FindTheBox)
                //    {
                //        var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgFindTheBox;
                //        tournament.Revive(timer, client);
                //    }
                //}
                if (client.Player.OnDefensePotion && timer > client.Player.OnDefensePotionStamp)
                    client.Player.OnDefensePotion = false;
                if (client.Player.OnAttackPotion && timer > client.Player.OnAttackPotionStamp)
                    client.Player.OnAttackPotion = false;
                if (client.Player.OnAccuracy)
                {
                    if (timer > client.Player.OnAccuracyStamp)
                    {
                        client.Player.Owner.Status.Accuracy -= client.Player.Owner.Player.HitRateAcc;
                        client.Player.Owner.Player.RemoveFlag(MsgUpdate.Flags.StarOfAccuracy);
                        client.Player.OnAccuracy = false;
                    }
                }
                if (client.Player.OnDodge)
                {
                    if (timer > client.Player.OnDodgeStamp)
                    {
                        client.Player.Owner.Status.Dodge -= 10;
                        client.Player.Owner.Player.RemoveFlag(MsgUpdate.Flags.Dodge);
                        client.Player.OnDodge = false;
                    }
                }
                if (MsgSchedules.CurrentTournament.Process == ProcesType.Alive && MsgSchedules.CurrentTournament.Type == TournamentType.SkillTournament)
                {
                    MsgSkillTournament tournament;
                    tournament = MsgSchedules.CurrentTournament as MsgSkillTournament;
                    tournament.Revive(timer, client);
                }
                if (client.Player.DelayedTask && timer > client.Player.DelayedTaskStamp)
                {
                    client.Player.DelayedTask = false;
                    switch (client.Player.DelayedTaskOption)
                    {
                        case MsgAutoHunt.TaskButton.TabAllyInvisible:
                            {
                                //if (client.Player.VipLevel < 6)
                                //    return;
                                using (RecycledPacket recycledPacket6 = new RecycledPacket())
                                {
                                    Packet stream4;
                                    stream4 = recycledPacket6.GetStream();
                                    if (!client.Player.TabAllyInvisible)
                                    {
                                        client.Player.TabAllyInvisible = true;
                                        client.SendSysMesage("Switched to hide your guild allies on main screen", MsgMessage.ChatMode.SlideFromRightRedVib);
                                    }
                                    else
                                    {
                                        client.Player.TabAllyInvisible = false;
                                        client.SendSysMesage("Switched to display your guild allies on main screen", MsgMessage.ChatMode.SlideFromRightRedVib);
                                    }
                                    client.Player.SendString(stream4, MsgStringPacket.StringID.Effect, true, "end_task");
                                }
                                break;
                            }
                        case MsgAutoHunt.TaskButton.TabEnemyInvisible:
                            {
                                //if (client.Player.VipLevel < 6)
                                //    return;
                                using (RecycledPacket recycledPacket5 = new RecycledPacket())
                                {
                                    Packet stream5;
                                    stream5 = recycledPacket5.GetStream();
                                    if (!client.Player.TabEnemyInvisible)
                                    {
                                        client.Player.TabEnemyInvisible = true;
                                        client.SendSysMesage("Switched to hide your guild enemies on main screen", MsgMessage.ChatMode.SlideFromRightRedVib);
                                    }
                                    else
                                    {
                                        client.Player.TabEnemyInvisible = false;
                                        client.SendSysMesage("Switched to display your guild enemies on main screen", MsgMessage.ChatMode.SlideFromRightRedVib);
                                    }
                                    client.Player.SendString(stream5, MsgStringPacket.StringID.Effect, true, "end_task");
                                }
                                break;
                            }
                        case MsgAutoHunt.TaskButton.SummoneGuild:
                            {
                                if (client.Player.VipLevel <= 0 || client.Player.MyGuild == null)
                                    break;
                                using (RecycledPacket recycledPacket5 = new RecycledPacket())
                                {
                                    Packet stream8;
                                    stream8 = recycledPacket5.GetStream();
                                    if (Program.BlockTeleportMap.Contains(client.Player.Map) || client.Player.Map == 1038|| client.InFIveOut || client.InTDM || client.InLastManStanding || client.InPassTheBomb || client.InST)
                                    {
                                        client.SendSysMesage("You can`t use it in " + client.Map.Name + " ", MsgMessage.ChatMode.SlideFromRightRedVib);
                                        break;
                                    }
                                    if (DateTime.Now < client.Player.MyGuild.SummonGuild.AddMinutes(5.0))
                                    {
                                        client.SendSysMesage("You need to wait 5 minutes before summoning again.", MsgMessage.ChatMode.SlideFromRightRedVib);
                                        break;
                                    }
                                    client.Player.MyGuild.SummonGuild = DateTime.Now;
                                    ushort X;
                                    X = client.Player.X;
                                    ushort Y;
                                    Y = client.Player.Y;
                                    uint Map;
                                    Map = client.Player.Map;
                                    uint Dynamic;
                                    Dynamic = client.Player.DynamicID;
                                    foreach (GameClient member in Server.GamePoll.Values.Where((GameClient e) => e.Player.GuildID != 0 && e.Player.GuildID == client.Player.GuildID && e.Player.UID != client.Player.UID))
                                    {
                                        member.Player.MessageBox("Your guild leader has summoned you to " + client.Map.Name + "! Would you like to go?", delegate (GameClient user)
                                        {
                                            user.Teleport((ushort)(X + Core.Random.Next(0, 5)), (ushort)(Y + Core.Random.Next(0, 5)), Map, Dynamic);
                                        }, null, 60);
                                    }
                                    Program.SendGlobalPackets.Enqueue(new MsgMessage(client.Player.Name + " guild leader of " + client.Player.MyGuild.GuildName + " has summoned his members to " + client.Map.Name, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.SlideFromRightRedVib).GetArray(stream8));
                                    client.Player.SendString(stream8, MsgStringPacket.StringID.Effect, true, "end_task");
                                }
                                break;
                            }
                        case MsgAutoHunt.TaskButton.VIPStorage:
                            {
                                if (client.Player.VipLevel == 6)
                                    break;
                                using (RecycledPacket recycledPacket5 = new RecycledPacket())
                                {
                                    Packet stream8;
                                    stream8 = recycledPacket5.GetStream();
                                    client.Player.Storage = true;
                                    client.SendSysMesage("storage enable");
                                    client.Player.SendString(stream8, MsgStringPacket.StringID.Effect, true, "end_task");
                                }
                                break;
                            }
                        case MsgAutoHunt.TaskButton.EnableAutoHunt:
                            {

                               client.CreateBoxDialog("Autohunt is not available on this server!");
                                //using (RecycledPacket recycledPacket3 = new RecycledPacket())
                                //{
                                //    Packet stream7;
                                //    stream7 = recycledPacket3.GetStream();
                                //    if (client.Player.AutoHunting == AutoStructures.Mode.NotActive)
                                //        client.Player.AutoHunting = AutoStructures.Mode.Enable;
                                //}
                                break;
                            }
                    }
                }
                if (client.Player.X == 0 || client.Player.Y == 0)
                    client.Teleport(428, 378, 1002);
                if (client.Player.HeavenBlessing > 0 && client.Player.ContainFlag(MsgUpdate.Flags.HeavenBlessing))
                {
                    if (timer > client.Player.HeavenBlessTime)
                    {
                        client.Player.RemoveFlag(MsgUpdate.Flags.HeavenBlessing);
                        client.Player.HeavenBlessing = 0;
                        using (RecycledPacket recycledPacket7 = new RecycledPacket())
                        {
                            Packet stream4;
                            stream4 = recycledPacket7.GetStream();
                            client.Player.SendUpdate(stream4, 0, MsgUpdate.DataType.HeavensBlessing);
                            client.Player.SendUpdate(stream4, 5, MsgUpdate.DataType.OnlineTraining);
                            client.Player.Stamina = (ushort)Math.Min((int)client.Player.Stamina, 100);
                            client.Player.SendUpdate(stream4, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                        }
                    }
                }
                if (client.Player.Reborn >= 1)
                {
                    if (timer > client.Player.ReceivePointsOnlineTraining)
                    {
                        client.Player.ReceivePointsOnlineTraining = timer.AddMinutes(5);
                        client.Player.OnlinePoints++;
                        using (RecycledPacket recycledPacket8 = new RecycledPacket())
                        {
                            Packet stream3;
                            stream3 = recycledPacket8.GetStream();
                            client.Player.SendUpdate(stream3, 4, MsgUpdate.DataType.OnlineTraining);
                        }
                        if (DateTime.Now.Minute % 20 == 0)
                            client.SendSysMesage($"You have {client.Player.OnlinePoints} Online Training Points[OTP's], Talk to ExchangeOfficer in TwinCity to exchange your points with the available prizes!", MsgMessage.ChatMode.TopLeft);


                    }
                    //if (client.Player.Map == 1002)
                    //{
                    //    if (timer > client.Player.OnlineTrainingTime)
                    //    {
                    //        client.Player.Money += ServerKernel.STAY_ONLINE;
                    //        client.Player.OnlineTrainingTime = timer.AddMinutes(60);
                    //        using (RecycledPacket recycledPacket9 = new RecycledPacket())
                    //        {
                    //            Packet stream2;
                    //            stream2 = recycledPacket9.GetStream();
                    //            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(client.Player.Name + " got " + ServerKernel.STAY_ONLINE + " Extra points for staying online for an hour straight! ", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream2));

                    //            //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("" + killer.Name + " just killed a " + Name + " and it dropped a " + DBItem.Name + "!!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                    //        }
                    //    }
                    //}
                    if (client.Player.Map != 1038)
                    {

                        using (RecycledPacket recycledPacket9 = new RecycledPacket())
                        {
                            Packet stream2;
                            stream2 = recycledPacket9.GetStream();
                            if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces != Game.MsgTournaments.ProcesType.Dead)
                            {
                                if (DateTime.Now.Minute % 20 == 0)
                                    client.SendSysMesage($"GuildWar has began!", MsgMessage.ChatMode.TopLeft);

                            }
                        }

                    }
                }
                if (client.Player.OfflineTraining == MsgOfflineTraining.Mode.TrainingGroup)
                {
                    TimeSpan T1;
                    T1 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan T2;
                    T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                    ushort minutes;
                    minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                    minutes = Math.Min((ushort)900, minutes);
                    if (client.Player.Map == 1039 && minutes >= 900)
                    {
                        client.Player.OfflineTraining = MsgOfflineTraining.Mode.Completed;
                        client.Socket.Disconnect();
                    }
                }
                if (client.Player.Map == 1038 && MsgSchedules.GuildWar.Proces == ProcesType.Dead)
                {
                    //Database.GuildTable.LoadGuildLeader(MsgSchedules.GuildWar.Winner.GuildID);
                    if (client.Player.StampGuildWarScore.AddSeconds(3) < timer)
                    {
                        client.SendSysMesage("# GuildWar Stats #", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage($"WinnerGuild: {MsgSchedules.GuildWar.Winner.Name}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        //client.SendSysMesage($"GuildLeader: {MsgSchedules.GuildWar.Winner.GLName}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        client.Player.StampGuildWarScore = timer;
                    }
                }

                #region Arena Stats
                    if (client.Player.Map == 1005 && client.Player.DynamicID == 0)//pk arena
                {
                    if (!client.Player.Alive)
                    {
                        if (client.Player.DeadStamp.AddSeconds(4) < timer)
                        {
                            ushort x = 0; ushort y = 0;
                            client.Map.GetRandCoord(ref x, ref y);
                            client.Teleport(x, y, 1005, 0);
                        }
                    }
                    if (client.Player.StampArenaScore.AddSeconds(3) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100);

#if Arabic
                        client.SendSysMesage("[Arena Stats]", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Shots: " + client.Player.MisShoot + " Hits: " + client.Player.HitShoot + " Rate: " + Rate.ToString() + " percent", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Kills: " + client.Player.ArenaKills + " Deaths: " + client.Player.ArenaDeads + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);

#else
                        client.SendSysMesage("[Arena Stats]", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Shots: " + client.Player.MisShoot + " Hits: " + client.Player.HitShoot + " Rate: " + Rate.ToString() + " / 100", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                        client.SendSysMesage("Kills: " + client.Player.ArenaKills + " Deaths: " + client.Player.ArenaDeads + " ", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);

#endif

                        client.Player.StampArenaScore = timer;


                    }
                }
                #endregion
                #region UnlimitedArenaRooms Stats

                if (UnlimitedArenaRooms.Maps.ContainsValue(client.Player.DynamicID))//pk arena
                {
                    
                    if (client.Player.StampArenaScore.AddMilliseconds(100) < timer)
                    {
                        uint Rate = 0;
                        if (client.Player.MisShoot != 0)
                            Rate = (uint)(((float)client.Player.HitShoot / (float)client.Player.MisShoot) * 100);
                        client.SendSysMesage("Accuracy Rates.", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                        foreach (var player in client.Map.Values.Where(e => e.Player.DynamicID == client.Player.DynamicID))
                        {
                            
                            client.SendSysMesage(player.Player.Name + " " + Math.Round((double)(player.Player.Hits * 100.0 / Math.Max(1, player.Player.TotalHits)), 2) + "%, H: " + player.Player.Hits + ", M: " + (player.Player.TotalHits - player.Player.Hits) + ", M.C: " + player.Player.MaxChains, MsgMessage.ChatMode.ContinueRightCorner);
                        }

                        client.Player.StampArenaScore = timer;


                    }
                }
                #endregion
                
                TheChosenProject.Database.VoteSystem.CheckUp(client);

                if (client.Player.Map == 3080)
                {
                    //if (!client.Player.Alive)
                    //{
                    //    if (client.Player.DeadStamp.AddSeconds(4) < timer)
                    //    {
                    //        ushort x = 0; ushort y = 0;
                    //        client.Map.GetRandCoord(ref x, ref y);
                    //        client.Teleport(x, y, 1005, 0);
                    //    }
                    //}
                    var Map = Database.Server.ServerMaps[3080];
                    //if (Map.GetMobLoc(3120) != null)
                    //{
                    using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();

                            var Array = Map.View.GetAllMapRoles(Role.MapObjectType.Monster);
                            foreach (var map_mob in Array)
                            {
                                if (map_mob.Map == 3080 && map_mob.Alive)
                                {
                                    var Monster = (map_mob as Game.MsgMonster.MonsterRole);
                                    if (Monster.Name == "DungeonStage1")
                                    {
                                        client.SendSysMesage("# HP: "+ Monster.HitPoints + " / "+ Monster.Family.MaxHealth +" #", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                                        client.SendSysMesage($"--- Stage 1 ---", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                                    }

                                }
                                if (map_mob.Map == 3080 && map_mob.Alive)
                                {
                                    var Monster = (map_mob as Game.MsgMonster.MonsterRole);
                                    if (Monster.Name == "DungeonStage2")
                                    {
                                        client.SendSysMesage("# HP: " + Monster.HitPoints + " / " + Monster.Family.MaxHealth + " #", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                                        client.SendSysMesage($"--- Stage 2 ---", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                                    }

                                }
                                if (map_mob.Map == 3080 && map_mob.Alive)
                                {
                                    var Monster = (map_mob as Game.MsgMonster.MonsterRole);
                                    if (Monster.Name == "DungeonStage3")
                                    {
                                        client.SendSysMesage("# HP: " + Monster.HitPoints + " / " + Monster.Family.MaxHealth + " #", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                                        client.SendSysMesage($"--- Stage 3 ---", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                                    }

                                }
                                if (map_mob.Map == 3080 && map_mob.Alive)
                                {
                                    var Monster = (map_mob as Game.MsgMonster.MonsterRole);
                                    if (Monster.Name == "DungeonStage4")
                                    {
                                        client.SendSysMesage("# HP: " + Monster.HitPoints + " / " + Monster.Family.MaxHealth + " #", MsgMessage.ChatMode.FirstRightCorner, MsgMessage.MsgColor.yellow);
                                        client.SendSysMesage($"--- Stage 4 ---", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                                    }

                                }

                        }
                        }
                    //}
                }
                //bahaa
                //if (client.Player.EnlightenReceive > 0 && DateTime.Now > client.Player.EnlightenTime.AddMinutes(20.0))
                //{
                //    client.Player.EnlightenTime = DateTime.Now;
                //    client.Player.EnlightenReceive--;
                //}
                if (client.Player.DExpTime != 0)
                {
                    client.Player.DExpTime--;
                    if (client.Player.DExpTime == 0)
                        client.Player.RateExp = 1u;
                }
                if (!client.Player.leveldown || !(DateTime.Now > client.Player.Last_LevelStamp.AddSeconds(60.0)))
                    return;
                client.Player.Last_LevelStamp = DateTime.Now;
                client.Player.leveldown = false;
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    client.Player.SendUpdate(stream, client.Player.Level, MsgUpdate.DataType.Level);
                    client.Player.SendUpdate(stream, client.Player.HeavenBlessing, MsgUpdate.DataType.HeavensBlessing);
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Players theards problem", false, LogType.EXCEPTION);
            }
        }

        public static void AutoAttackCallback(GameClient client, Time32 timer)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || !client.Player.CompleteLogin)
                    return;
                if (!client.Player.Alive && client.Player.CompleteLogin && DateTime.Now > client.Player.GhostStamp && !client.Player.ContainFlag(MsgUpdate.Flags.Ghost))
                {
                    client.Player.AddFlag(MsgUpdate.Flags.Ghost, 2592000, true);
                    if ((int)client.Player.Body % 10 < 3)
                        client.Player.TransformationID = 99;
                    else
                        client.Player.TransformationID = 98;
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        client.Send(stream.MapStatusCreate(client.Player.Map, client.Map.ID, client.Map.TypeStatus));
                    }
                }
                if (!client.OnAutoAttack || !client.Player.Alive)
                    return;
                if (client.Player.ContainFlag(MsgUpdate.Flags.Dizzy))
                {
                    client.OnAutoAttack = false;
                    return;
                }
                InteractQuery action;
                action = default(InteractQuery);
                action = InteractQuery.ShallowCopy(client.AutoAttack);
                client.Player.RandomSpell = action.SpellID;
                if (action.SpellID >= 1000 && action.SpellID <= 1002 && client.Player.Mana < 100)
                    client.OnAutoAttack = false;
                else
                    MsgAttackPacket.Process(client, action);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Auto Attack theards problem", false, LogType.EXCEPTION);
            }
        }
        public static void StampXPCountCallback(GameClient client, Time32 Timer)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || !client.Player.CompleteLogin)
                    return;
                if (ItemType.IsTwoHand(client.Equipment.RightWeapon) && client.Equipment.LeftWeapon != 0 && !ItemType.IsShield(client.Equipment.LeftWeapon) && !ItemType.IsArrow(client.Equipment.LeftWeapon) && client.Inventory.HaveSpace(1))
                {
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        if (!client.Equipment.Remove(Flags.ConquerItem.LeftWeapon, stream2))
                            client.Equipment.Remove(Flags.ConquerItem.AleternanteLeftWeapon, stream2);
                        client.Equipment.LeftWeapon = 0u;
                    }
                }
                if (client.Player.PKPoints > 0 && Timer > client.Player.PkPointsStamp.AddMinutes(6))
                {
                    client.Player.PKPoints--;
                    client.Player.PkPointsStamp = Time32.Now;
                }
                if (Timer > client.Player.XPListStamp.AddSeconds(4) && client.Player.Alive)
                {
                    client.Player.XPListStamp = Timer.AddSeconds(4);
                    if (!client.Player.ContainFlag(MsgUpdate.Flags.XPList))
                    {
                        client.Player.XPCount++;
                        using (RecycledPacket rec = new RecycledPacket())
                        {
                            Packet stream;
                            stream = rec.GetStream();
                            client.Player.SendUpdate(stream, client.Player.XPCount, MsgUpdate.DataType.XPCircle);
                            if (client.Player.XPCount >= 100)
                            {
                                client.Player.XPCount = 0;
                                if (client.Player.AutoHunting == AutoStructures.Mode.Enable)
                                {
                                    if (client.MySpells.ClientSpells.ContainsKey(1110))
                                        client.Player.OpenXpSkill(MsgUpdate.Flags.Cyclone, 20);
                                }
                                else
                                    client.Player.AddFlag(MsgUpdate.Flags.XPList, 20, true);
                                //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "xp");
                            }
                        }
                    }
                }
                if (client.Player.InUseIntensify && Timer > client.Player.IntensifyStamp.AddSeconds(2) && !client.Player.Intensify)
                {
                    client.Player.Intensify = true;
                    client.Player.InUseIntensify = false;
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("XPList theards problem", false, LogType.EXCEPTION);
            }
        }

        public static void StaminaCallback(GameClient client, Time32 Now)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || !client.Player.CompleteLogin || !client.Player.Alive || client.Player.ContainFlag(MsgUpdate.Flags.Fly))
                    return;
                byte MaxStamina;
                MaxStamina = (byte)((client.Player.HeavenBlessing > 0) ? 150u : 100u);
                if (client.Player.Stamina < MaxStamina)
                {
                    ushort addstamin;
                    addstamin = 0;
                    addstamin = (ushort)(addstamin + client.Player.GetAddStamina());
                    client.Player.Stamina = (ushort)Math.Min(client.Player.Stamina + addstamin, MaxStamina);
                    using (RecycledPacket recycledPacket = new RecycledPacket())
                    {
                        Packet stream2;
                        stream2 = recycledPacket.GetStream();
                        client.Player.SendUpdate(stream2, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                    }
                }
                if (!client.Player.ContainFlag(MsgUpdate.Flags.Ride) || !client.Player.CheckInvokeFlag(MsgUpdate.Flags.Ride, Now) || client.Vigor >= client.Status.MaxVigor)
                    return;
                client.Vigor = (ushort)Math.Min(client.Vigor + 2, client.Status.MaxVigor);
                using (RecycledPacket rec = new RecycledPacket())
                {
                    Packet stream;
                    stream = rec.GetStream();
                    client.Send(stream.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Stamina theards problem", false, LogType.EXCEPTION);
            }
        }

        public static void CopyDirectory(string sourceDir, string destDir, bool copySubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
            {
                Console.WriteLine($"Source directory does not exist: {sourceDir}");
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetPath = Path.Combine(destDir, file.Name);
                file.CopyTo(targetPath, true);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string targetSubDir = Path.Combine(destDir, subdir.Name);
                    CopyDirectory(subdir.FullName, targetSubDir);
                }
            }
        }

        public unsafe static void BuffersCallback(GameClient client, Time32 Timer)
        {
            try
            {
                if (client == null || !client.FullLoading || client.Player == null || !client.Player.CompleteLogin)
                    return;
               
                Extensions.Time32 Timer2 = Extensions.Time32.Now;
                #region Anti bot
                if (DateTime.Now > client.Player.LastSuspect.AddMinutes(2))
                    client.Player.ReceiveTest = 0;

                if (Timer2 < client.Player.LastAttack.AddSeconds(5))
                {
                    if (client.MobsKilled > 5000 && (DateTime.Now > client.Player.LastSuccessCaptcha.AddMinutes(client.Player.NextCaptcha)))
                    {
                        if (Timer > client.Player.KillCountCaptchaStamp.AddMinutes(10))
                        {
                            if (client.Player.AutoHunting != AutoStructures.Mode.Enable || client.Player.OfflineTraining != MsgOfflineTraining.Mode.Hunting)
                            {

                                if (!client.Player.WaitingKillCaptcha)
                                {
                                    client.Player.KillCountCaptchaStamp = Extensions.Time32.Now;
                                    client.Player.WaitingKillCaptcha = true;
                                    client.ActiveNpc = 9999997;
                                    client.Player.KillCountCaptcha = Role.Core.Random.Next(10000, 50000).ToString();
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(client, stream);
                                        dialog.Text("Input the current text: " + client.Player.KillCountCaptcha + " to verify your humanity.");
                                        dialog.AddInput("Captcha message:", (byte)client.Player.KillCountCaptcha.Length);
                                        dialog.Option("No thank you.", 255);
                                        dialog.AddAvatar(39);
                                        dialog.FinalizeDialog();

                                    }
                                }
                            }
                            else
                                client.Socket.Disconnect();
                        }
                    }
                }
                #endregion
                //  if (DateTime.Now > client.Player.LastMove.AddSeconds(1))// Fix for FB /SS angles

                //if (client.Player.BlockMovementCo && DateTime.Now > client.Player.BlockMovement)
                //{
                //    client.Player.Protect = Extensions.Time32.Now.AddSeconds(1);
                //    client.Player.BlockMovementCo = false;
                //    client.SendSysMesage("You`re free to move now. You have 1 second to jump away.");
                //}
                DateTime DateNow = DateTime.Now;

                #region Back UP
                //if (DateNow.Minute % 10 == 0 && DateNow.Second == 30)// Every hour at XX:00:30
                //{
                //    try
                //    {
                //        string today = DateTime.Now.ToString("MM-dd-yyyy");
                //        string hour = DateTime.Now.ToString("HH");
                //        string minute = DateTime.Now.ToString("mm");
                //        string basePath = Path.Combine(ServerKernel.CO2FOLDER, $"BackupDivineData\\{today}\\{hour}\\{minute}");

                //        string createUsers = Path.Combine(basePath, "Users");
                //        string createspells = Path.Combine(basePath, "PlayersSpells");
                //        string createprofs = Path.Combine(basePath, "PlayersProfs");
                //        string createitems = Path.Combine(basePath, "PlayersItems");
                //        string createhouses = Path.Combine(basePath, "Houses");
                //        string createclans = Path.Combine(basePath, "Clans");
                //        string createguilds = Path.Combine(basePath, "Guilds");
                //        string quests = Path.Combine(basePath, "Quests");

                //        // Always ensure all directories exist
                //        Directory.CreateDirectory(basePath);
                //        Directory.CreateDirectory(createUsers);
                //        Directory.CreateDirectory(createspells);
                //        Directory.CreateDirectory(createprofs);
                //        Directory.CreateDirectory(createitems);
                //        Directory.CreateDirectory(createhouses);
                //        Directory.CreateDirectory(createclans);
                //        Directory.CreateDirectory(createguilds);
                //        Directory.CreateDirectory(quests);

                //        // Copy .ini and .txt files (always overwrite)
                //        File.Copy(ServerKernel.CO2FOLDER + "\\Arena.ini", Path.Combine(basePath, "Arena.ini"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\BanIp.txt", Path.Combine(basePath, "BanIp.txt"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\BanUID.txt", Path.Combine(basePath, "BanUID.txt"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\ClassPkWar.ini", Path.Combine(basePath, "ClassPkWar.ini"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\ElitePk.ini", Path.Combine(basePath, "ElitePk.ini"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\GuildWarInfo.ini", Path.Combine(basePath, "GuildWarInfo.ini"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\Npcs.txt", Path.Combine(basePath, "Npcs.txt"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\SkillTeamPK.ini", Path.Combine(basePath, "SkillTeamPK.ini"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\SobNpcs.txt", Path.Combine(basePath, "SobNpcs.txt"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\TeamArena.ini", Path.Combine(basePath, "TeamArena.ini"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\TeamElitePK.ini", Path.Combine(basePath, "TeamElitePK.ini"), true);
                //        File.Copy(ServerKernel.CO2FOLDER + "\\Votes.txt", Path.Combine(basePath, "Votes.txt"), true);

                //        // Recursively copy directories (always overwrite)
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\Users", createUsers);
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\PlayersSpells", createspells);
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\PlayersProfs", createprofs);
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\PlayersItems", createitems);
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\Houses", createhouses);
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\Clans", createclans);
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\Guilds", createguilds);
                //        CopyDirectory(ServerKernel.CO2FOLDER + "\\Quests", quests);

                //        //Console.WriteLine("Backup completed: " + today + " Hour: " + hour);
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine("Backup failed:");
                //        Console.WriteLine(e.ToString());
                //    }
                //}

                #endregion

                if (client.Player.BlackSpot && Timer > client.Player.Stamp_BlackSpot)
                {
                    client.Player.BlackSpot = false;
                    using (RecycledPacket recycledPacket2 = new RecycledPacket())
                    {
                        Packet stream8;
                        stream8 = recycledPacket2.GetStream();
                        client.Player.View.SendView(stream8.BlackspotCreate(false, client.Player.UID), true);
                    }
                }
                StatusFlagsBigVector32.Flag[] flags;
                flags = client.Player.BitVector.GetFlags();
                foreach (StatusFlagsBigVector32.Flag flag in flags)
                {
                    if (flag.Expire(Timer))
                    {
                        if (flag.Key >= 98 && flag.Key <= 110)
                        {
                            client.Player.AddAura(client.Player.UseAura, null, 0);
                            continue;
                        }
                        if (flag.Key == 32)
                        {
                            client.Player.CursedTimer = 0;
                            client.Player.RemoveFlag(MsgUpdate.Flags.Cursed);
                            continue;
                        }
                        if (flag.Key == 18 || flag.Key == 23)
                            KOBoard.KOBoardRanking.AddItem(new KOBoard.Entry
                            {
                                UID = client.Player.UID,
                                Name = client.Player.Name,
                                Points = client.Player.KillCounter
                            }, true);
                        client.Player.RemoveFlag((MsgUpdate.Flags)flag.Key);
                    }
                    else if (flag.Key == 111)
                    {
                        client.Player.RemovedShackle = DateTime.Now;
                    }
                    else if (flag.Key == 1)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            int damage;
                            damage = (int)Base.CalculatePoisonDamageFog((uint)client.Player.HitPoints, 80,4);
                            if (client.Player.HitPoints == 1) //Poison S
                                damage = 0;
                            else
                                client.Player.HitPoints = Math.Max(1, client.Player.HitPoints - damage);
                            using (RecycledPacket recycledPacket3 = new RecycledPacket())
                            {
                                Packet stream2;
                                stream2 = recycledPacket3.GetStream();
                                InteractQuery interactQuery;
                                interactQuery = default(InteractQuery);
                                interactQuery.Damage = damage;
                                interactQuery.AtkType = MsgAttackPacket.AttackID.Physical;
                                interactQuery.X = client.Player.X;
                                interactQuery.Y = client.Player.Y;
                                interactQuery.OpponentUID = client.Player.UID;
                                InteractQuery action;
                                action = interactQuery;
                                client.Player.View.SendView(stream2.InteractionCreate(&action), true);
                            }
                        }
                    }
                    else if (flag.Key == 46)
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            using (RecycledPacket rec = new RecycledPacket())
                            {
                                Packet stream;
                                stream = rec.GetStream();
                                InteractQuery interactQuery;
                                interactQuery = default(InteractQuery);
                                interactQuery.UID = client.Player.UID;
                                interactQuery.X = client.Player.X;
                                interactQuery.Y = client.Player.Y;
                                interactQuery.SpellID = 6009;
                                interactQuery.AtkType = MsgAttackPacket.AttackID.Magic;
                                InteractQuery action2;
                                action2 = interactQuery;
                                MsgAttackPacket.ProcescMagic(client, stream.InteractionCreate(&action2), action2);
                            }
                        }
                    }
                    else if (flag.Key == 14 || (flag.Key == 15 && client.Player.Map != 6000))
                    {
                        if (flag.CheckInvoke(Timer))
                        {
                            if (client.Player.PKPoints > 0)
                                client.Player.PKPoints--;
                            client.Player.PkPointsStamp = Time32.Now;
                        }
                    }
                    else if (flag.Key == 32 && flag.CheckInvoke(Timer))
                    {
                        if (client.Player.CursedTimer > 0)
                        {
                            client.Player.CursedTimer--;
                            continue;
                        }
                        client.Player.CursedTimer = 0;
                        client.Player.RemoveFlag(MsgUpdate.Flags.Cursed);
                    }
                }
                if (client.Player.OnTransform && client.Player.TransformInfo != null && client.Player.TransformInfo.CheckUp(Timer))
                    client.Player.TransformInfo = null;
                if (client.Player.ContainFlag(MsgUpdate.Flags.Praying))
                {
                    if (client.Player.BlessTime < 7170000)
                    {
                        if (Timer > client.Player.CastPrayStamp.AddSeconds(30))
                        {
                            bool have;
                            have = false;
                            foreach (IMapObj ownerpraying in client.Player.View.Roles(MapObjectType.Player))
                            {
                                if (Core.GetDistance(client.Player.X, client.Player.Y, ownerpraying.X, ownerpraying.Y) <= 2)
                                {
                                    Player target;
                                    target = ownerpraying as Player;
                                    if (target.ContainFlag(MsgUpdate.Flags.CastPray))
                                    {
                                        have = true;
                                        break;
                                    }
                                }
                            }
                            if (!have)
                                client.Player.RemoveFlag(MsgUpdate.Flags.Praying);
                            client.Player.CastPrayStamp = new Time32(Timer.AllMilliseconds);
                            client.Player.BlessTime += 30000;
                        }
                    }
                    else
                        client.Player.BlessTime = 3100000;
                }
                if (client.Player.ContainFlag(MsgUpdate.Flags.CastPray))
                {
                    if (client.Player.BlessTime < 7140000)
                    {
                        if (Timer > client.Player.CastPrayStamp.AddSeconds(30))
                        {
                            client.Player.CastPrayStamp = new Time32(Timer.AllMilliseconds);
                            client.Player.BlessTime += 60000;
                        }
                    }
                    else
                        client.Player.BlessTime = 7200000;
                    if (Timer > client.Player.CastPrayActionsStamp.AddSeconds(5))
                    {
                        client.Player.CastPrayActionsStamp = new Time32(Timer.AllMilliseconds);
                        foreach (IMapObj obj2 in client.Player.View.Roles(MapObjectType.Player))
                        {
                            if (Core.GetDistance(client.Player.X, client.Player.Y, obj2.X, obj2.Y) > 2)
                                continue;
                            Player Target;
                            Target = obj2 as Player;
                            if (Target.Reborn < 2 && !Target.ContainFlag(MsgUpdate.Flags.Praying))
                            {
                                Target.AddFlag(MsgUpdate.Flags.Praying, 2592000, true);
                                using (RecycledPacket recycledPacket4 = new RecycledPacket())
                                {
                                    Packet stream7;
                                    stream7 = recycledPacket4.GetStream();
                                    ActionQuery actionQuery;
                                    actionQuery = default(ActionQuery);
                                    actionQuery.ObjId = client.Player.UID;
                                    actionQuery.dwParam = (uint)client.Player.Action;
                                    actionQuery.Timestamp = (int)obj2.UID;
                                    ActionQuery action3;
                                    action3 = actionQuery;
                                    client.Player.View.SendView(stream7.ActionCreate(&action3), true);
                                }
                            }
                        }
                    }
                }
                else if (client.Player.BlessTime != 0 && !client.Player.ContainFlag(MsgUpdate.Flags.CastPray) && !client.Player.ContainFlag(MsgUpdate.Flags.Praying) && Timer > client.Player.CastPrayStamp.AddSeconds(2))
                {
                    if (client.Player.BlessTime > 2000)
                        client.Player.BlessTime -= 2000;
                    else
                        client.Player.BlessTime = 0u;
                    using (RecycledPacket recycledPacket5 = new RecycledPacket())
                    {
                        Packet stream6;
                        stream6 = recycledPacket5.GetStream();
                        client.Player.SendUpdate(stream6, client.Player.BlessTime, MsgUpdate.DataType.LuckyTimeTimer);
                    }
                    client.Player.CastPrayStamp = new Time32(Timer.AllMilliseconds);
                }
                if (Timer > client.Player.UpdateBossocationStamp.AddSeconds(5))
                {
                    client.Player.UpdateBossocationStamp = Timer;
                    if (client.Map != null)
                    {
                        foreach (Boss boss in BossDatabase.Bosses.Values)
                        {
                            if (boss != null && boss.Alive && client.Player.Map == boss.MapID)
                            {
                                using (RecycledPacket recycledPacket6 = new RecycledPacket())
                                {
                                    Packet stream5;
                                    stream5 = recycledPacket6.GetStream();
                                    ActionQuery actionQuery;
                                    actionQuery = default(ActionQuery);
                                    actionQuery.Type = ActionType.TeamSearchForMember;
                                    actionQuery.ObjId = boss.MonsterID;
                                    actionQuery.wParam1 = boss.MapX;
                                    actionQuery.wParam2 = boss.MapY;
                                    ActionQuery action4;
                                    action4 = actionQuery;
                                    client.Send(stream5.ActionCreate(&action4));
                                }
                            }
                        }
                    }
                }
                if (Timer > client.Player.UpdateNotficationLavaStamp.AddSeconds(5))
                {
                    client.Player.UpdateNotficationLavaStamp = Timer;
                    if (client.Map != null)
                    {
                        var Map = Database.Server.ServerMaps[2056];
                        if (Map != null)
                        {
                            var role = Map.GetMob(20055, true);
                            if (role != null)
                            {
                                if (role.Alive && client.Player.Map == role.Map)
                                {
                                    using (RecycledPacket recycledPacket6 = new RecycledPacket())
                                    {
                                        Packet stream5;
                                        stream5 = recycledPacket6.GetStream();
                                        ActionQuery actionQuery;
                                        actionQuery = default(ActionQuery);
                                        actionQuery.Type = ActionType.TeamSearchForMember;
                                        actionQuery.ObjId = 20055;
                                        actionQuery.wParam1 = role.X;
                                        actionQuery.wParam2 = role.Y;
                                        ActionQuery action4;
                                        action4 = actionQuery;
                                        client.Send(stream5.ActionCreate(&action4));
                                    }
                                }
                            }
                        }
                    }
                }
                if (client.Team == null)
                    return;
                if (client.Team.AutoInvite && client.Player.Map != 1036 && client.Team.CkeckToAdd() && Timer > client.Team.InviteTimer.AddSeconds(10))
                {
                    client.Team.InviteTimer = Timer;
                    foreach (IMapObj obj in client.Player.View.Roles(MapObjectType.Player))
                    {
                        if (client.Team.SendInvitation.Contains(obj.UID))
                            continue;
                        client.Team.SendInvitation.Add(obj.UID);
                        if ((obj as Player).Owner.Team == null)
                        {
                            using (RecycledPacket recycledPacket7 = new RecycledPacket())
                            {
                                Packet stream4;
                                stream4 = recycledPacket7.GetStream();
                                obj.Send(stream4.PopupInfoCreate(client.Player.UID, obj.UID, client.Player.Level, client.Player.BattlePower));
                                stream4.TeamCreate(MsgTeam.TeamTypes.InviteRequest, client.Player.UID);
                                obj.Send(stream4);
                            }
                        }
                    }
                }
                if (!client.Team.TeamLider(client) || !(Timer > client.Team.UpdateLeaderLocationStamp.AddSeconds(4)))
                    return;
                client.Team.UpdateLeaderLocationStamp = Timer;
                using (RecycledPacket recycledPacket8 = new RecycledPacket())
                {
                    Packet stream3;
                    stream3 = recycledPacket8.GetStream();
                    ActionQuery actionQuery;
                    actionQuery = default(ActionQuery);
                    actionQuery.ObjId = client.Player.UID;
                    actionQuery.dwParam = 1015;
                    actionQuery.Type = ActionType.LocationTeamLieder;
                    actionQuery.wParam1 = client.Team.Leader.Player.X;
                    actionQuery.wParam2 = client.Team.Leader.Player.Y;
                    ActionQuery action5;
                    action5 = actionQuery;
                    client.Team.SendTeam(stream3.ActionCreate(&action5), client.Player.UID, client.Player.Map);
                }
            }
            catch
            {
                ServerKernel.Log.SaveLog("Buffers theards problem", false, LogType.EXCEPTION);
            }
        }
        private static bool Check(GameClient client)
        {
            if (client == null || client.Map == null || !client.FullLoading || client.Player == null || client.Player.CompleteLogin == false)
                return false;
            return true;
        }
        public static unsafe void MiningProcess(Client.GameClient client)
        {
            if (!Check(client))
                return;
            Time32 Clock = Time32.Now;
            if (!client.Mining)
                return;
            else
            {
                //if (client.Player.MiningAttempts == 0)
                //{
                //    client.SendSysMesage("Sorry, you need to get some rest come back tomorrow.");
                //    client.StopMining();
                //    return;
                //}
                Game.MsgServer.MsgGameItem MiningWeapon = null;
                if (!client.Equipment.Alternante)
                {
                    if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out MiningWeapon))
                    {
                        client.SendSysMesage("You have to wear PickAxe to start mining.");
                        client.StopMining();
                        return;
                    }
                }
                else
                {
                    if (!client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteRightWeapon, out MiningWeapon))
                    {
                        client.SendSysMesage("You have to wear PickAxe to start mining.");
                        client.StopMining();
                        return;
                    }
                }
                if (MiningWeapon == null)
                {
                    client.SendSysMesage("You have to wear PickAxe to start mining.");
                    client.StopMining();
                    return;
                }
                if (!Database.ItemType.IsPickAxe(MiningWeapon.ITEM_ID))
                {
                    client.SendSysMesage("You have to wear PickAxe to start mining.");
                    client.StopMining();
                    return;
                }
                if (!client.Inventory.HaveSpace(1))
                {
                    client.SendSysMesage("Your inventory is full. You can not mine anymore items.");
                    client.StopMining();
                    return;
                }
                if (client.Mining)
                {
                    //using (var rec = new ServerSockets.RecycledPacket())
                    //{
                    //    var stream = rec.GetStream();
                    //    ActionQuery daction = new ActionQuery()
                    //    {
                    //        ObjId = client.Player.UID,
                    //        dwParam = client.Player.MiningAttempts,
                    //        Type = ActionType.Mining,
                    //        wParam1 = 24,
                    //        wParam2 = 68,
                    //    };
                    //    client.Player.View.SendView(stream.ActionCreate(&daction), true);
                    //    TheChosenProject.Database.MineRule Rolee = null;
                    //    switch (client.Player.Map)
                    //    {
                    //        case 1218://meteor zone mine
                    //        case 6001://jail war mine
                    //        case 6000://jails
                    //            {
                    //                if (TheChosenProject.Database.MiningTable.GetRandomRole(out Rolee))
                    //                {
                    //                    switch (Rolee.RuleType)
                    //                    {
                    //                        case Database.MineRuleType.MineOre:
                    //                            {
                    //                                if (client.Player.VipLevel == 6 && client.Player.skipore == true)
                    //                                {
                    //                                    switch (Rolee.RuleValue)
                    //                                    {
                    //                                        case 1072025:
                    //                                        case 1072045:
                    //                                            client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                            break;
                    //                                    }
                    //                                }
                    //                                else
                    //                                {
                    //                                    switch (Rolee.RuleValue)
                    //                                    {
                    //                                        case 1072015:
                    //                                        case 1072010:
                    //                                        case 1072011:
                    //                                        case 1072025:
                    //                                        case 1072045:
                    //                                            client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                            break;
                    //                                    }
                    //                                }
                    //                                break;
                    //                            }
                    //                        case Database.MineRuleType.MineGem:
                    //                            {
                    //                                uint ItemID = Rolee.RuleValue;
                    //                                switch (ItemID)
                    //                                {
                    //                                    case 700001:
                    //                                    case 700011:
                    //                                    case 700031:
                    //                                    case 700041:
                    //                                        //if (ItemID % 10 == 1)
                    //                                        //{
                    //                                        //    if (Database.MiningTable.PercentSuccess(0.5))
                    //                                        //        ItemID += 1;
                    //                                        //    if (ItemID % 10 == 1)
                    //                                        //    {
                    //                                        //        if (Database.MiningTable.PercentSuccess(0.5))
                    //                                        //            ItemID += 2;
                    //                                        //    }
                    //                                        //    if (ItemID % 10 == 2)
                    //                                        //    {
                    //                                        //        if (Database.MiningTable.PercentSuccess(0.2))
                    //                                        //            ItemID += 1;
                    //                                        //    }
                    //                                        //}
                    //                                        client.Inventory.AddMine(stream, ItemID, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "~from~mining!", 0, 0, 0, true);
                    //                                        break;
                    //                                }
                    //                                break;
                    //                            }
                    //                    }
                    //                }
                    //                break;
                    //            }
                    //        case 1025://phoenixcity minecave
                    //        case 1028://twincity minecave
                    //            {
                    //                if (TheChosenProject.Database.MiningTable.GetRandomRole(out Rolee))
                    //                {
                    //                    switch (Rolee.RuleType)
                    //                    {
                    //                        case Database.MineRuleType.MineOre:
                    //                            {
                    //                                if (client.Player.VipLevel == 6 && client.Player.skipore == true)
                    //                                {
                    //                                    switch (Rolee.RuleValue)
                    //                                    {
                    //                                        case 1072031:
                    //                                        case 810032:
                    //                                        case 810033:
                    //                                        case 810034:
                    //                                            if (Role.Core.ChanceSuccess(Rolee.RuleChance))
                    //                                            {
                    //                                                client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                            }
                    //                                            break;
                    //                                    }
                    //                                }
                    //                                else
                    //                                {
                    //                                    switch (Rolee.RuleValue)
                    //                                    {
                    //                                        case 1072047:
                    //                                        case 1072048:
                    //                                        case 1072049:
                    //                                        case 1072050:
                    //                                        case 1072010:
                    //                                        case 1072015:
                    //                                        case 1072011:
                    //                                        case 1072025:
                    //                                        case 1072045:
                    //                                        case 1072031:
                    //                                        case 810032:
                    //                                        case 810033:
                    //                                        case 810034:
                    //                                            if (Role.Core.ChanceSuccess(Rolee.RuleChance))
                    //                                            {
                    //                                                client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                            }
                    //                                        break;
                    //                                    }
                    //                                }
                    //                                break;
                    //                            }
                    //                        case Database.MineRuleType.MineGem:
                    //                            {
                    //                                uint ItemID = Rolee.RuleValue;
                    //                                switch (ItemID)
                    //                                {
                    //                                    case 700001:
                    //                                    case 700011:
                    //                                    case 700021:
                    //                                        if (ItemID % 10 == 1)
                    //                                        {
                    //                                            //if (Database.MiningTable.PercentSuccess(0.08))
                    //                                            //    ItemID += 1;
                    //                                            //if (ItemID % 10 == 1)
                    //                                            //{
                    //                                            //    if (Database.MiningTable.PercentSuccess(0.008))
                    //                                            //        ItemID += 2;
                    //                                            //}
                    //                                            //if (ItemID % 10 == 2)
                    //                                            //{
                    //                                            //    if (Database.MiningTable.PercentSuccess(0.008))
                    //                                            //        ItemID += 1;
                    //                                            //}
                    //                                            if (Role.Core.ChanceSuccess(Rolee.RuleChance))
                    //                                            {
                    //                                                client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                            }
                    //                                        }
                    //                                        break;
                    //                                }
                    //                                break;
                    //                            }
                    //                    }
                    //                }
                    //                break;
                    //            }
                    //        case 1027://DesertMine
                    //        case 1026://ApeMine
                    //            {
                    //                if (TheChosenProject.Database.MiningTable.GetRandomRole(out Rolee))
                    //                {
                    //                    switch (Rolee.RuleType)
                    //                    {
                    //                        case Database.MineRuleType.MineOre:
                    //                            {
                    //                                if (client.Player.VipLevel == 6 && client.Player.skipore == true)
                    //                                {
                    //                                    switch (Rolee.RuleValue)
                    //                                    {
                    //                                        case 1072047:
                    //                                        case 1072048:
                    //                                        case 1072049:
                    //                                        case 1072050:
                    //                                            if (Role.Core.ChanceSuccess(Rolee.RuleChance))
                    //                                            {
                    //                                                client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                            }
                    //                                            break;
                    //                                        default:
                    //                                            break;
                    //                                    }
                    //                                }
                    //                                else
                    //                                {
                    //                                    switch (Rolee.RuleValue)
                    //                                    {
                    //                                        case 1072047:
                    //                                        case 1072048:
                    //                                        case 1072049:
                    //                                        case 1072050:
                    //                                        case 1072010:
                    //                                        case 1072015:
                    //                                        case 1072011:
                    //                                            if (Role.Core.ChanceSuccess(Rolee.RuleChance))
                    //                                            {
                    //                                                client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                            }
                    //                                            break;
                    //                                        default:
                    //                                            break;
                    //                                    }
                    //                                }
                    //                                break;
                    //                            }
                    //                        case Database.MineRuleType.MineGem:
                    //                            {
                    //                                uint ItemID = Rolee.RuleValue;
                    //                                switch (ItemID)
                    //                                {
                    //                                    case 700051:
                    //                                    case 700061:
                    //                                        //if (ItemID % 10 == 1)
                    //                                        //{
                    //                                        //    if (Database.MiningTable.PercentSuccess(0.5))
                    //                                        //        ItemID += 1;
                    //                                        //    if (ItemID % 10 == 1)
                    //                                        //    {
                    //                                        //        if (Database.MiningTable.PercentSuccess(0.5))
                    //                                        //            ItemID += 2;
                    //                                        //    }
                    //                                        //    if (ItemID % 10 == 2)
                    //                                        //    {
                    //                                        //        if (Database.MiningTable.PercentSuccess(0.2))
                    //                                        //            ItemID += 1;
                    //                                        //    }
                    //                                        //}
                    //                                        if (Role.Core.ChanceSuccess(Rolee.RuleChance))
                    //                                        {
                    //                                            client.Inventory.AddMine(stream, Rolee.RuleValue, 1, 0, 0, 0, TheChosenProject.Role.Flags.Gem.NoSocket, TheChosenProject.Role.Flags.Gem.NoSocket, false, TheChosenProject.Role.Flags.ItemEffect.None, true, "!", 0, 0, 0, true);
                    //                                        }
                    //                                        break;
                    //                                }
                    //                                break;
                    //                            }
                    //                    }
                    //                }
                    //                break;
                    //            }
                    //        //case 1836:
                    //        //    {
                    //        //        //if (Role.Core.Rate(0.05))
                    //        //        //{
                    //        //        //    uint MineCps = (uint)Role.Core.Random.Next(3, 10);
                    //        //        //    client.Player.ConquerPoints += MineCps;
                    //        //        //    client.SendSysMesage("[ChamberFloor1] : You have mined " + MineCps + " conquer points!", MsgMessage.ChatMode.System);
                    //        //        //}
                    //        //        if (Role.Core.Rate(0.1))
                    //        //        {
                    //        //            if (client.Inventory.HaveSpace(1))
                    //        //            {
                    //        //                client.Inventory.AddMine(stream, 723694, 1, 0, 0 ,0 , Flags.Gem.NoSocket, Flags.Gem.NoSocket,false,Flags.ItemEffect.None,true,"!",0,0,0,false);
                    //        //                client.SendSysMesage("[ChamberFloor1] : You have mined  conquer points!", MsgMessage.ChatMode.System);
                    //        //            }
                    //        //            else
                    //        //            {
                    //        //                client.SendSysMesage("You have a full inventory.");
                    //        //                client.Player.Owner.StopMining();
                    //        //            }
                    //        //        }
                    //        //        break;
                    //        //    }
                    //        default:
                    //            {
                    //                client.SendSysMesage("You cannot mine here. You must go inside a mine.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.purple);
                    //                client.Mining = false;
                    //                break;
                    //            }
                    //    }
                    ////}
                    #region Old Mining
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        ActionQuery daction = new ActionQuery()
                        {
                            ObjId = client.Player.UID,
                            dwParam = client.Player.MiningAttempts,
                            Type = ActionType.Mining,
                            wParam1 = 24,
                            wParam2 = 68,
                        };
                        client.Player.View.SendView(stream.ActionCreate(&daction), true);


                        int RatePick = Program.Rand.Next(0, 10);

                        byte Pickaxe = 0;
                        MsgProficiency Prof;
                        if (!client.MyProfs.ClientProf.TryGetValue(MiningWeapon.ITEM_ID / 1000, out Prof))
                            client.MyProfs.Add(stream, MiningWeapon.ITEM_ID / 1000);
                        if (Prof == null)
                        {
                            Prof = new MsgProficiency();
                            Prof.Level = 0;
                        }
                        Pickaxe = 5;

                        client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, 10 * (uint)(Pickaxe != 0 ? Pickaxe : 1) - Prof.Level, stream);

                        double Nothing = (45 - ((.5 * Prof.Level) + (.5 * Pickaxe)) / 1);
                        double EuxChance = (10000 + ((.01 * Prof.Level) + (.01 * Pickaxe)) * 1); // 5%
                        double GemChance = ServerKernel.CHANCE_GEMS;
                        TheChosenProject.Database.MineRule Rolee = null;
                        //double Meteor = (3 + ((.01 * Prof.Level) + (.01 * Pickaxe)) * 1);
                        //double StoneChance = (2 + (.01 * Prof.Level) + (.01 * Pickaxe) * 1);
                        //double Refined = (1 + (.001 * Prof.Level) + (.001 * Pickaxe) * 1);
                        //double Super = (.01 + (.0001 * Prof.Level) + (.0001 * Pickaxe) * 1);
                        switch (client.Player.Map)
                        {
                            case 1218://meteor zone mine
                            case 6001://jail war mine
                            case 6000://jails
                                {
                                    if (Role.Core.ChanceSuccess(Nothing * 2))
                                        return;
                                    if (Role.Core.ChanceSuccess(GemChance))
                                    {
                                        uint add = 0;
                                        uint[] specialGems = new uint[]
                                        {
                                            700001, 700011, 700031, 700041
                                        };

                                        // Randomly select one of those base gem IDs
                                        uint itemid = specialGems[Role.Core.Random.Next(0, specialGems.Length)] + add;

                                        //uint itemid = Database.ItemType.Gems[Role.Core.Random.Next(0, Database.ItemType.Gems.Length)] + add;
                                        Database.ItemType.DBItem DbItem;
                                        if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                        {
                                            string qty = "Normal";
                                            client.Inventory.Add(stream, itemid, 1);
                                            client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                            client.SendSysMesage("You have found a " + DbItem.Name + $"{qty}. Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                        }
                                    }
                                    else
                                    {
                                        if (client.Player.VipLevel == 6 && client.Player.skipore == true)
                                        {
                                            return;
                                        }
                                        uint itemid = Database.ItemType.Ores[Role.Core.Random.Next(0, Database.ItemType.Ores.Length)];
                                        Database.ItemType.DBItem DbItem;
                                        if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                        {
                                            uint add = 0;
                                            client.Inventory.Add(stream, itemid, 1);
                                            client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                            client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                            //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                            //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                        }
                                    }
                                    break;
                                }
                            case 1025://phoenixcity minecave
                            case 1028://twincity minecave
                                {
                                    if (Role.Core.ChanceSuccess(Nothing * 2)) //60%
                                        return;
                                    if (Role.Core.ChanceSuccess(GemChance)) //20 %
                                    {
                                        uint add = 0;
                                        uint[] specialGems = new uint[]
                                        {
                                           700011,700021,700001
                                        };

                                        // Randomly select one of those base gem IDs
                                        uint itemid = specialGems[Role.Core.Random.Next(0, specialGems.Length)] + add;

                                        //uint itemid = Database.ItemType.Gems[Role.Core.Random.Next(0, Database.ItemType.Gems.Length)] + add;
                                        Database.ItemType.DBItem DbItem;
                                        if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                        {
                                            string qty = "Normal";
                                            client.Inventory.Add(stream, itemid, 1);
                                            client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                            client.SendSysMesage("You have found a " + DbItem.Name + $"{qty}. Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                        }
                                    }
                                    else if (Role.Core.ChanceSuccess(EuxChance)) // 20%
                                    {
                                        if (client.Player.VipLevel == 6 && client.Player.skipore == true)
                                        {
                                            Database.ItemType.DBItem DbItem;
                                            if (Role.Core.ChanceSuccess(1)) //3%
                                            {
                                                client.SendSysMesage("You have skipore. But still can get Euxente Ore", MsgMessage.ChatMode.TopLeft);
                                                if (Server.ItemsBase.TryGetValue(1072031, out DbItem))
                                                {
                                                    client.Inventory.Add(stream, DbItem.ID, 1);
                                                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (uint)(150 + RatePick * 20), stream);
                                                    client.SendSysMesage("You have found a " + DbItem.Name + ". Gained " + (150 + RatePick * 20) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                                }
                                                return;
                                            }
                                            else if (Role.Core.ChanceSuccess(3))
                                            {
                                                uint itemid = Database.ItemType.Ores[Role.Core.Random.Next(0, Database.ItemType.promoteOre.Length)];
                                                if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                                {
                                                    uint add = 0;
                                                    client.Inventory.Add(stream, itemid, 1);
                                                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                                    client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                                    //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                    //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                                }
                                            }
                                            //else
                                            //{
                                            //    uint itemid = Database.ItemType.SkipGoldMine[Role.Core.Random.Next(0, Database.ItemType.SkipGoldMine.Length)];
                                            //    if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                            //    {
                                            //        uint add = 0;
                                            //        client.Inventory.Add(stream, itemid, 1);
                                            //        client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                            //        client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                            //        //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                            //        //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                            //    }
                                            //}
                                                return;
                                        }
                                        else
                                        {
                                            if (Role.Core.ChanceSuccess(2))
                                            {
                                                Database.ItemType.DBItem DbItem;
                                                if (Server.ItemsBase.TryGetValue(1072031, out DbItem))
                                                {
                                                    client.Inventory.Add(stream, DbItem.ID, 1);
                                                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (uint)(150 + RatePick * 20), stream);
                                                    client.SendSysMesage("You have found a " + DbItem.Name + ". Gained " + (150 + RatePick * 20) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                                }
                                            }
                                            else if (Role.Core.ChanceSuccess(5))
                                            {
                                                uint itemid = Database.ItemType.promoteOre[Role.Core.Random.Next(0, Database.ItemType.promoteOre.Length)];
                                                Database.ItemType.DBItem DbItem;
                                                if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                                {
                                                    uint add = 0;
                                                    client.Inventory.Add(stream, itemid, 1);
                                                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                                    client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                                    //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                                    //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                                }
                                            }
                                            //else
                                            //{
                                            //    uint itemid = Database.ItemType.SkipGoldMine[Role.Core.Random.Next(0, Database.ItemType.SkipGoldMine.Length)];
                                            //    Database.ItemType.DBItem DbItem;
                                            //    if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                            //    {
                                            //        uint add = 0;
                                            //        client.Inventory.Add(stream, itemid, 1);
                                            //        client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                            //        client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                            //        //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                            //        //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                            //    }
                                            //}
                                        }
                                    }
                                    break;
                                }
                            case 1027://DesertMine
                            case 1026://ApeMine
                                {
                                    if (Role.Core.ChanceSuccess(Nothing * 2))
                                        return;
                                    if (Role.Core.ChanceSuccess(GemChance))
                                    {
                                        uint add = 0;
                                        uint[] specialGems = new uint[]
                                        {
                                            700051, 700061
                                        };

                                        // Randomly select one of those base gem IDs
                                        uint itemid = specialGems[Role.Core.Random.Next(0, specialGems.Length)] + add;

                                        //uint itemid = Database.ItemType.Gems[Role.Core.Random.Next(0, Database.ItemType.Gems.Length)] + add;
                                        Database.ItemType.DBItem DbItem;
                                        if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                        {
                                            string qty = "Normal";
                                            client.Inventory.Add(stream, itemid, 1);
                                            client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                            client.SendSysMesage("You have found a " + DbItem.Name + $"{qty}. Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                        }
                                    }
                                    else
                                    {
                                        if (client.Player.VipLevel == 6 && client.Player.skipore == true)
                                        {
                                            return;
                                        }
                                        uint itemid = Database.ItemType.Ores[Role.Core.Random.Next(0, Database.ItemType.Ores.Length)];
                                        Database.ItemType.DBItem DbItem;
                                        if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                                        {
                                            uint add = 0;
                                            client.Inventory.Add(stream, itemid, 1);
                                            client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                                            client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                                            //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                                            //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    client.SendSysMesage("You cannot mine here. You must go inside a mine.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.purple);
                                    client.Mining = false;
                                    break;
                                }
                        }
                        //if (client.Map.ID >= 1025 && client.Map.ID <= 1028)
                        //{
                        //    if (Role.Core.ChanceSuccess(Nothing * 2))
                        //        return;
                        //    if (Role.Core.ChanceSuccess(GemChance))
                        //    {
                        //        uint add = 0;
                        //        //if (Role.Core.ChanceSuccess(Refined))
                        //        //    add += 1;
                        //        //else if (ChanceSuccess(Super))
                        //        //    add += 2;
                        //        //else if (ChanceSuccess(Super))
                        //        //    add += 2;
                        //        uint itemid = Database.ItemType.Gems[Role.Core.Random.Next(0, Database.ItemType.Gems.Length)] + add;
                        //        Database.ItemType.DBItem DbItem;
                        //        if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                        //        {
                        //            string qty = "Normal";
                        //            //if (add == 1)
                        //            //    qty = "(Refined)";
                        //            //if (add == 2)
                        //            //    qty = "(Super)";
                        //            client.Inventory.Add(stream, itemid, 1);
                        //            client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                        //            client.SendSysMesage("You have found a " + DbItem.Name + $"{qty}. Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                        //            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        //            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                        //        }
                        //    }
                        //    else if (Role.Core.ChanceSuccess(EuxChance))
                        //    {
                        //        if (client.Player.VipLevel == 6 && client.Player.skipore == true)
                        //        {
                        //            client.SendSysMesage("You have skipore. But still can get Euxente Ore", MsgMessage.ChatMode.TopLeft);
                        //            if (Role.Core.ChanceSuccess(3))
                        //            {
                        //                Database.ItemType.DBItem DbItem;
                        //                if (Server.ItemsBase.TryGetValue(1072031, out DbItem))
                        //                {
                        //                    client.Inventory.Add(stream, DbItem.ID, 1);
                        //                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (uint)(150 + RatePick * 20), stream);
                        //                    client.SendSysMesage("You have found a " + DbItem.Name + ". Gained " + (150 + RatePick * 20) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                        //                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        //                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                        //                }
                        //                else if (Role.Core.ChanceSuccess(5))
                        //                {
                        //                    uint itemid = Database.ItemType.Ores[Role.Core.Random.Next(0, Database.ItemType.promoteOre.Length)];
                        //                    if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                        //                    {
                        //                        uint add = 0;
                        //                        client.Inventory.Add(stream, itemid, 1);
                        //                        client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                        //                        client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                        //                        //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        //                        //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                        //                    }
                        //                }
                        //            }
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            if (Role.Core.ChanceSuccess(5))
                        //            {
                        //                Database.ItemType.DBItem DbItem;
                        //                if (Server.ItemsBase.TryGetValue(1072031, out DbItem))
                        //                {
                        //                    client.Inventory.Add(stream, DbItem.ID, 1);
                        //                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (uint)(150 + RatePick * 20), stream);
                        //                    client.SendSysMesage("You have found a " + DbItem.Name + ". Gained " + (150 + RatePick * 20) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                        //                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        //                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                        //                }
                        //            }
                        //            else if (Role.Core.ChanceSuccess(5))
                        //            {
                        //                uint itemid = Database.ItemType.promoteOre[Role.Core.Random.Next(0, Database.ItemType.promoteOre.Length)];
                        //                Database.ItemType.DBItem DbItem;
                        //                if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                        //                {
                        //                    uint add = 0;
                        //                    client.Inventory.Add(stream, itemid, 1);
                        //                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                        //                    client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                        //                    //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        //                    //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                        //                }
                        //            }
                        //            else
                        //            {
                        //                uint itemid = Database.ItemType.Ores[Role.Core.Random.Next(0, Database.ItemType.Ores.Length)];
                        //                Database.ItemType.DBItem DbItem;
                        //                if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                        //                {
                        //                    uint add = 0;
                        //                    client.Inventory.Add(stream, itemid, 1);
                        //                    client.MyProfs.CheckUpdate(MiningWeapon.ITEM_ID / 1000, (add + 1) * 1500, stream);
                        //                    client.SendSysMesage("You have found a " + DbItem.Name + $". Gained " + ((add + 1) * 1500) + " Mining Exp.", MsgMessage.ChatMode.TopLeft);
                        //                    //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations to " + client.Player.Name + "! He/She has won " + DbItem.Name + " from~mining!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        //                    //client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "firework-like");
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    if (Role.Core.ChanceSuccess(Nothing * 4))
                        //        return;
                        //    if (Role.Core.ChanceSuccess(GemChance / 4))
                        //    {
                        //        uint itemid = Database.ItemType.Ores[Role.Core.Random.Next(0, Database.ItemType.Ores.Length)];
                        //        Database.ItemType.DBItem DbItem;
                        //        if (Server.ItemsBase.TryGetValue(itemid, out DbItem))
                        //        {
                        //            client.Inventory.Add(stream, itemid, 1);
                        //        }
                        //    }
                        //}
                    }
                    #endregion
                }
            }
        }

    }
}