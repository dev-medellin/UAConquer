using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using System.Net.NetworkInformation;
using System.Net.Http;
using TheChosenProject.Cryptography;

namespace TheChosenProject.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet LoginHandlerCreate(this ServerSockets.Packet stream, uint Type, uint Map)
        {
            stream.InitWriter();

            stream.Write(0);
            stream.Write(Type);
            stream.Write(Map);

            stream.Finalize(GamePackets.MapLoading);

            return stream;
        }

    }
    public class top_typ
    {
        public const byte
        VIP = 33,
        KingRank = 34;
    }
    public unsafe struct MsgLoginHandler
    {

        [PacketAttribute(GamePackets.MapLoading)]
        public unsafe static void LoadMap(GameClient client, Packet packet)
        {
            if ((client.ClientFlag & ServerFlag.AcceptLogin) != ServerFlag.AcceptLogin)
                return;
            try
            {
                client.Send(packet.HeroInfo(client.Player));
                client.Send(packet.FlowerCreate(Core.IsBoy(client.Player.Body) ? MsgFlower.FlowerAction.Flower : MsgFlower.FlowerAction.FlowerSender, 0, 0, client.Player.Flowers.RedRoses, client.Player.Flowers.RedRoses.Amount2day, client.Player.Flowers.Lilies, client.Player.Flowers.Lilies.Amount2day, client.Player.Flowers.Orchids, client.Player.Flowers.Orchids.Amount2day, client.Player.Flowers.Tulips, client.Player.Flowers.Tulips.Amount2day));
                if (client.Player.Flowers.FreeFlowers != 0)
                    client.Send(packet.FlowerCreate(Core.IsBoy(client.Player.Body) ? MsgFlower.FlowerAction.FlowerSender : MsgFlower.FlowerAction.Flower, 0, 0, client.Player.Flowers.FreeFlowers));
                client.Send(packet.NobilityIconCreate(client.Player.Nobility));
                if (client.Player.BlessTime != 0)
                    client.Player.SendUpdate(packet, client.Player.BlessTime, MsgUpdate.DataType.LuckyTimeTimer);
                client.Player.ProtectAttack(10000);
                client.Player.CreateHeavenBlessPacket(packet, true);
                if (client.Player.DExpTime != 0)
                    client.Player.CreateExtraExpPacket(packet);
                if (client.Player.MyClan != null)
                {
                    client.Player.MyClan.SendThat(packet, client);
                    foreach (Clan ally in client.Player.MyClan.Ally.Values)
                    {
                        client.Send(packet.ClanRelationCreate(client.Player.MyClan.ID, ally.Name, ally.LeaderName, MsgClan.Info.AddAlly));
                    }
                    foreach (Clan enemy in client.Player.MyClan.Enemy.Values)
                    {
                        client.Send(packet.ClanRelationCreate(client.Player.MyClan.ID, enemy.Name, enemy.LeaderName, MsgClan.Info.AddEnemy));
                    }
                }
                client.Equipment.Show(packet);
                client.Inventory.ShowALL(packet);
                foreach (MsgDetainedItem item in client.Confiscator.RedeemContainer.Values)
                {
                    MsgDetainedItem Dataitem;
                    Dataitem = item;
                    Dataitem.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Confiscator.GetTimer(item.Date).Ticks).Days);
                    if (Dataitem.DaysLeft > 7)
                        Dataitem.Action = MsgDetainedItem.ContainerType.RewardCps;
                    if (Dataitem.Action != MsgDetainedItem.ContainerType.RewardCps)
                    {
                        Dataitem.Action = MsgDetainedItem.ContainerType.DetainPage;
                        Dataitem.Send(client, packet);
                    }
                    if (Dataitem.Action == MsgDetainedItem.ContainerType.RewardCps)
                        client.Confiscator.RedeemContainer.TryRemove(item.UID, out Dataitem);
                }
                foreach (MsgDetainedItem item2 in client.Confiscator.ClaimContainer.Values)
                {
                    MsgDetainedItem Dataitem2;
                    Dataitem2 = item2;
                    Dataitem2.DaysLeft = (uint)(TimeSpan.FromTicks(DateTime.Now.Ticks).Days - TimeSpan.FromTicks(Confiscator.GetTimer(item2.Date).Ticks).Days);
                    if (Dataitem2.RewardConquerPoints != 0)
                        Dataitem2.Action = MsgDetainedItem.ContainerType.RewardCps;
                    Dataitem2.Send(client, packet);
                    client.Confiscator.ClaimContainer[item2.UID] = Dataitem2;
                }
                if (MsgSchedules.GuildWar.RewardDeputiLeader.Contains(client.Player.UID))
                    client.Player.AddFlag(MsgUpdate.Flags.TopDeputyLeader, 2592000, false);
                if (MsgSchedules.GuildWar.RewardLeader.Contains(client.Player.UID))
                    client.Player.AddFlag(MsgUpdate.Flags.TopGuildLeader, 2592000, false);
                client.Player.PKPoints = client.Player.PKPoints;
                if (client.Player.CursedTimer > 0)
                    client.Player.AddCursed(client.Player.CursedTimer);
                client.Send(packet.ServerTimerCreate());
                MsgSchedules.ClassPkWar.LoginClient(client);
                MsgSchedules.ElitePkTournament.GetTitle(client, packet);
                MsgSchedules.TeamPkTournament.GetTitle(client, packet);
                MsgSchedules.SkillTeamPkTournament.GetTitle(client, packet);
                MsgSchedules.PkWar.AddTop(client);
                MsgSchedules.MonthlyPKWar.AddTop(client);
                //MsgTournaments.MsgSchedules.MrConquer.LoginClient(client);
                //MsgTournaments.MsgSchedules.MsConquer.LoginClient(client);
                //if (MsgTournaments.MsgSchedules.CouplesPKWar.Winner1 == client.Player.Name ||
                //        MsgTournaments.MsgSchedules.CouplesPKWar.Winner2 == client.Player.Name)
                //client.Player.AddFlag(MsgUpdate.Flags.TopSpouse, Role.StatusFlagsBigVector32.PermanentFlag, false);
                if (TheChosenProject.Game.MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityID != 1)
                    client.Send(new MsgMessage(TheChosenProject.Game.MsgTournaments.MsgBroadcast.CurrentBroadcast.Message, "ALLUSERS", TheChosenProject.Game.MsgTournaments.MsgBroadcast.CurrentBroadcast.EntityName, MsgMessage.MsgColor.red, MsgMessage.ChatMode.BroadcastMessage).GetArray(packet));
                if (client.Player.ChampionPoints != 0)
                    client.Player.SendUpdate(packet, client.Player.ChampionPoints, MsgUpdate.DataType.RaceShopPoints);
                if (client.Player.OnlinePoints != 0)
                    client.Player.SendUpdate(packet, client.Player.OnlinePoints, MsgUpdate.DataType.BoundConquerPoints);
                client.Player.UpdateVip(packet);
                client.Player.SendUpdate(packet, 255, MsgUpdate.DataType.Merchant);
                ActionQuery actionQuery;
                actionQuery = default(ActionQuery);
                actionQuery.ObjId = client.Player.UID;
                actionQuery.Type = (ActionType)157;
                actionQuery.dwParam = 2;
                ActionQuery action;
                action = actionQuery;
                client.Send(packet.ActionCreate(&action));
                client.Send(packet.ServerConfig());
                if (client.Player.SecurityPassword == 0)
                    client.Player.VerifiedPassword = true;
                if (ServerKernel.StaticGUIType)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.WelcomeMessage;
                        Game.MsgNpc.NpcHandler.WelcomeMessage(client, stream, 0, "", 0);
                    }
                }

                //newbie invite
                //if (client.Player.Level == 1 && client.Player.NewbieProtection == Flags.NewbieExperience.Enable)
                //{
                //    client.Player.MessageBox("welcome to our server, hope you enjoy!\n Talk to [FreePLevel!] to level up.", new Action<Client.GameClient>(p => { p.Teleport(439, 386, 1002); }), null, 60);
                //}

                // Welcome Messages.
                client.SendSysMesage("Welcome to UAConquer Conquer! Visit the Guide NPC in TC for assistance.", MsgMessage.ChatMode.Talk);
                client.SendSysMesage("Warning: Red and Black gear will drop!", MsgMessage.ChatMode.Talk);
                client.SendSysMesage("NA host optimized for both NA and EU players. Ping is set for FastBlade and ScentSword.", MsgMessage.ChatMode.Talk);
                client.SendSysMesage("Enjoy free auto hunt if you keep your accounts online.", MsgMessage.ChatMode.Talk);
                client.SendSysMesage("For any assistance, contact UAConquerStaff. Yours truly, UAConquer.", MsgMessage.ChatMode.Talk);
                if (MsgSchedules.GuildWar.Proces == ProcesType.Alive)
                {
                    var timeLeft = DateTime.Now;
                    if (timeLeft.DayOfWeek != DayOfWeek.Sunday)
                        timeLeft = timeLeft.AddDays(7 - (byte)timeLeft.DayOfWeek);
                    timeLeft = timeLeft.AddHours(18 - timeLeft.Hour).AddMinutes(-timeLeft.Minute);
                    var toDisplay = timeLeft.Subtract(DateTime.Now);
                    client.SendSysMesage($"The Guild War will end in {toDisplay.Days} Days, {toDisplay.Hours} Hours and {toDisplay.Minutes} Minutes. Make sure you won't miss it!", MsgMessage.ChatMode.Talk);
                }
                client.Player.VipLevel = client.Player.VipLevel;
                if (client.Player.VipLevel == 6)
                {
                    TimeSpan timer2;
                    timer2 = new TimeSpan(client.Player.ExpireVip.Ticks);
                    TimeSpan Now3;
                    Now3 = new TimeSpan(DateTime.Now.Ticks);
                    int days_left;
                    days_left = (int)(timer2.TotalDays - Now3.TotalDays);
                    int hour_left;
                    hour_left = (int)(timer2.TotalHours - Now3.TotalHours);
                    int left_minutes;
                    left_minutes = (int)(timer2.TotalMinutes - Now3.TotalMinutes);
                    if (days_left > 0)
                        client.SendWhisper("You have " + hour_left.ToString("0,0") + " hour(s) and " + left_minutes.ToString("0,0") + " minute(s) of vip service remaining.", "VIPSystem", client.Player.Name, 2991003);
                }

                client.Warehouse.SendReturnedItems(packet);
                //client.Player.EmoneyPoints = client.Player.EmoneyPoints;
                //if (client.Player.Level <= 110)
                //{
                //    if (client.Player.NewbieProtection != Flags.NewbieExperience.Remove)
                //        client.Player.NewbieProtection = client.Player.NewbieProtection;
                //}
                if (client.Player.VipLevel != 6)
                    client.Player.VipLevel = 1;
                if (client.Player.LastLoginClient.Ticks > 0)
                {
                    TimeSpan timer1;
                    timer1 = new TimeSpan(client.Player.LastLoginClient.Ticks);
                    TimeSpan Now2;
                    Now2 = new TimeSpan(DateTime.Now.Ticks);
                    int Offlinetimes;
                    Offlinetimes = (int)(Now2.TotalDays - timer1.TotalDays);
                    if (Offlinetimes >= ServerKernel.MIN_Offline_ && Offlinetimes <= ServerKernel.MAX_Offline_)
                        client.Player.MessageBox($"Welcome back {client.Player.Name}!\n All your donations to {ServerKernel.ServerName} was fully recorded. Offline Times : {Offlinetimes} Day(s)", null, null);
                }
                if (client.Player.OfflineTraining == MsgOfflineTraining.Mode.Completed)
                {
                    TimeSpan T1;
                    T1 = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan T2;
                    T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                    ushort minutes;
                    minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                    minutes = Math.Min((ushort)900, minutes);
                    client.Send(packet.OfflineTGStatsCreate(minutes, (ushort)(900 - minutes), 0, 0L));
                    client.Player.OfflineTraining = MsgOfflineTraining.Mode.NotActive;
                }
                ServerKernel.ServerManager.AddUser(client.Player);
                //MsgProtect.ProGuardHandler.OnLogin(client);
                if (!client.ProjectManager)
                {
                    if (client.Player.MyGuild != null)
                    {
                        //string webhookUrl = "https://discord.com/api/webhooks/1330806957183074327/YrGosKN_Xn5D2TrfEzmbq5NheJTpCD9aJ6xkh2A0g5xTM4Hy7QaTTl_3keGiBozZbJq1"; // Replace with your actual webhook URL

                        // Enqueue to global packets
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Name:" + client.Player.Name + " Level: " + client.Player.Level + " Guild: " + client.Player.MyGuild.GuildName + " has just logged back on!", "ALLUSERS", "SYSTEM", Game.MsgServer.MsgMessage.MsgColor.white, (Game.MsgServer.MsgMessage.ChatMode)2012).GetArray(packet));

                        // Enqueue to DiscordEventsAPI
                        //if (DatabaseConfig.discord_stat == true)
                            //Program.DiscordPlayerOnAPI.Enqueue("Name:" + client.Player.Name + " Level: " + client.Player.Level + " Guild: " + client.Player.MyGuild.GuildName + " has just logged back on!");

                        // Send to webhook
                        //using (var httpClient = new HttpClient())
                        //{
                        //    var content = new StringContent($"{{\"content\":\"Name: {client.Player.Name} Level: {client.Player.Level} Guild: {client.Player.MyGuild.GuildName} has just logged back on!\"}}", Encoding.UTF8, "application/json");
                        //    httpClient.PostAsync(webhookUrl, content).Wait();
                        //}

                    }
                    else
                    {
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Name:" + client.Player.Name + " Level: " + client.Player.Level + " Guild: None has just logged back on!", "ALLUSERS", "SYSTEM", Game.MsgServer.MsgMessage.MsgColor.white, (Game.MsgServer.MsgMessage.ChatMode)2012).GetArray(packet));
                        //if (DatabaseConfig.discord_stat == true)
                            //Program.DiscordPlayerOnAPI.Enqueue("Name: " + client.Player.Name + " Level: " + client.Player.Level + " Guild: None has just logged back on!");
                    }
                }
                else
                {
                    //Program.DiscordAdminLogHandler.Enqueue("Name: " + client.Player.Name + " IP: " + client.IP + " Enviroment: " + DatabaseConfig.EnvironmentType + "!");
                }

                if (client.Player.Map == 700 || client.Player.Map == 1038 || client.Player.Map == 1076 || client.Player.Map == 1212)
                {
                    client.Teleport(428, 378, 1002);
                }
                if (client.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Friends))
                {
                    foreach (var fr in client.Player.Associate.Associat[Role.Instance.Associate.Friends].Values)
                    {
                        Client.GameClient gameClient;
                        if (Database.Server.GamePoll.TryGetValue(fr.UID, out gameClient))
                        {
                            gameClient.SendSysMesage("Your friend " + client.Player.Name + " has logged on.", (Game.MsgServer.MsgMessage.ChatMode)2005);
                        }
                    }
                }
                if (client.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Enemy))
                {
                    foreach (var fr in client.Player.Associate.Associat[Role.Instance.Associate.Enemy].Values)
                    {
                        Client.GameClient gameClient;
                        if (Database.Server.GamePoll.TryGetValue(fr.UID, out gameClient))
                        {
                            gameClient.SendSysMesage("Your Enemy " + client.Player.Name + " has logged on.", (Game.MsgServer.MsgMessage.ChatMode)2005);
                        }
                    }
                }
                #region Spells
                if (client.Player.Reborn >= 1)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!client.MySpells.ClientSpells.ContainsKey(4000))
                            client.MySpells.Add(stream, 4000);//Normal guard

                        //if (Database.AtributesStatus.IsFire(client.Player.Class))
                        //    if (!client.MySpells.ClientSpells.ContainsKey(4010))//Fire reborn
                        //        client.MySpells.Add(stream, 4010);

                        //if (Database.AtributesStatus.IsWater(client.Player.Class))
                        //    if (!client.MySpells.ClientSpells.ContainsKey(4020))//Water reborn
                        //        client.MySpells.Add(stream, 4020);

                        //if (Database.AtributesStatus.IsWarrior(client.Player.Class))
                        //    if (!client.MySpells.ClientSpells.ContainsKey(4050))//warrior reborn
                        //        client.MySpells.Add(stream, 4050);

                        //if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                        //    if (!client.MySpells.ClientSpells.ContainsKey(4060))//Trojan reborn
                        //        client.MySpells.Add(stream, 4060);

                        //if (Database.AtributesStatus.IsArcher(client.Player.Class))
                        //    if (!client.MySpells.ClientSpells.ContainsKey(4070))//Archer reborn
                        //        client.MySpells.Add(stream, 4070);
                    }
                }
                #endregion
                client.GeneratorItemDrop(DropRule.Status.All);
                client.ClientFlag &= ~ServerFlag.AcceptLogin;
                client.ClientFlag |= ServerFlag.LoginFull;
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

    }
}
