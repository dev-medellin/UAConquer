using TheChosenProject.Client;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using static TheChosenProject.Role.Flags;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgEvents
{
    public enum EventStage
    {
        None,
        Inviting,
        Countdown,
        Starting,
        Fighting,
        Over
    }

    public enum BroadCastLoc
    {
        Exit,
        World,
        WorldAndChannel,
        Map,
        System,
        Score,
        Title,
        SpecificMap,
        YellowMap,
        WorldY
    }

    public class Events
    {
        #region Properties

        //public uint RewardCps = (uint)ServerKernel.ONE_SOC_RATE;
        public DateTime LastScores;
        public string EventTitle = "Base Event";
        public byte IDEvent = 0;
        public MsgStaticMessage.Messages IDMessage = MsgStaticMessage.Messages.None;
        public bool isTerr = false;
        public bool isBlue = false;
        public bool isRed = false;
        public bool IsCapitan = false;
        public bool DW = false;
        public EventStage Stage = EventStage.None;
        public Role.GameMap Map;
        public bool NoDamage = false;
        public uint MapEvent = 11000;
        public ushort BaseMap = 700;
        public uint DinamicID = 0;
        public bool FFADamage = false;
        public uint Duration = 0;
        public bool Reflect = false;
        public uint _Duration = 0;
        public ushort X = 0;
        public ushort Y = 0;
        public bool MagicAllowed = true;
        public bool ReviveAllowed = true;
        public bool MeleeAllowed = true;
        public bool FlyAllowed = false;
        public bool PotionsAllowed = true;
        public bool ReviveTele = false;
        public bool FriendlyFire = false;
        public Dictionary<string, string> MacJoin = new Dictionary<string, string>();
        public Dictionary<uint, GameClient> PlayerList = new Dictionary<uint, GameClient>();
        public readonly Dictionary<uint, int> PlayerScores = new Dictionary<uint, int>();
        public Dictionary<uint, Dictionary<uint, GameClient>> Teams = new Dictionary<uint, Dictionary<uint, GameClient>>();
        public List<ushort> AllowedSkills = new List<ushort>();
        private byte minplayers = 2;
        public int CountDown;
        public int DialogID = 0;
        public DateTime EndTime;
        public DateTime DisplayScores;
        public ushort WaitingArea = 1616;
        public ISchedule.EventID EventID = ISchedule.EventID.None;
        public KillerSystem KillSystem;
        #endregion Properties

        /// <summary>
        /// Used to send messages related to the current PVP Event
        /// </summary>
        public void Broadcast(string msg, BroadCastLoc loc, uint Map = 0, GameClient user = null)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (user != null)
                {
                    switch (loc)
                    {
                        case BroadCastLoc.Exit:
                            {
                                if (!PlayerList.ContainsKey(user.Player.UID))
                                {
                                    user.SendSysMesage($"---------{EventTitle}---------", ChatMode.FirstRightCorner);
                                    user.SendSysMesage("You leaved of the event.", ChatMode.ContinueRightCorner);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (loc)
                    {
                        case BroadCastLoc.System:
                            {
                                Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgServer.MsgMessage(msg, MsgColor.white, ChatMode.Center).GetArray(stream));
                                break;
                            }
                        case BroadCastLoc.WorldAndChannel:
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, MsgColor.white, ChatMode.Center).GetArray(stream));
                            break;

                        case BroadCastLoc.World:
                            Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgServer.MsgMessage(msg, MsgColor.white, ChatMode.Center).GetArray(stream));
                            break;

                        case BroadCastLoc.WorldY:
                            Program.SendGlobalPackets.EnqueueWithOutChannel(new MsgServer.MsgMessage(msg, MsgColor.white, ChatMode.TopLeft).GetArray(stream));
                            break;

                        case BroadCastLoc.Map:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg, ChatMode.Center);
                            }
                            break;

                        case BroadCastLoc.YellowMap:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg, ChatMode.TopLeft, MsgColor.yellow);
                            }
                            break;

                        case BroadCastLoc.Title:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg, ChatMode.FirstRightCorner);
                            }
                            break;

                        case BroadCastLoc.Score:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                client.SendSysMesage(msg, ChatMode.ContinueRightCorner);
                            }
                            break;

                        case BroadCastLoc.SpecificMap:
                            foreach (GameClient client in PlayerList.Values.ToArray())
                            {
                                if (client.Player.Map == Map)
                                    client.SendSysMesage(msg, ChatMode.ContinueRightCorner);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Check skills to the current PVP Event
        /// </summary>
        public virtual void CheckSkills(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (AllowedSkills != null)
                {
                    if (AllowedSkills.Count > 0)
                    {
                        foreach (var skillid in AllowedSkills)
                        {
                            if (!client.MySpells.ClientSpells.ContainsKey(skillid))
                            {
                                client.MySpells.Add(stream, skillid, 4);
                            }
                            else
                            {
                                client.MySpells.ClientSpells[skillid].Level = 4;
                                client.Send(stream.SpellCreate(client.MySpells.ClientSpells[skillid]));
                            }
                        }
                        client.Player.AddTempEquipment(stream, 410239, ConquerItem.RightWeapon);
                        client.Player.AddTempEquipment(stream, 420239, ConquerItem.LeftWeapon);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a player to the current PVP Event
        /// </summary>
        public virtual bool AddPlayer(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (Stage == EventStage.Inviting)
                {
                    if (!GameMap.EventMaps.ContainsKey(client.Player.Map))
                    {
                        if (!client.InQualifier())
                        {
                            if (client.Player.Map != 1038 && client.Player.Map != 6001 && client.Player.Map != 6003)
                            {
                                if (!PlayerList.ContainsKey(client.Player.UID))
                                {
                                    
                                    
                                    if (!client.Inventory.HaveSpace(5))
                                    {
                                        client.CreateBoxDialog("Please make 5 more spaces in your inventory to join.");
                                    }
                                    else
                                    {
                                        if (!MacJoin.ContainsKey(client.OnLogin.MacAddress))
                                        {
                                            MacJoin.Add(client.OnLogin.MacAddress, client.Player.Name);
                                            client.Player.PMap = client.Player.Map;
                                            client.Player.PMapX = client.Player.X;
                                            client.Player.PMapY = client.Player.Y;
                                            client.Teleport(53, 65, WaitingArea, DinamicID);
                                            PlayerList.Add(client.Player.UID, client);
                                            PlayerScores.Add(client.Player.UID, 0);
                                            client.EventBase = this;
                                            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                                                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                                            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                                                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                                            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                                                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                                            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                                                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Ride);
                                            if (client.Player.OnTransform && client.Player.TransformInfo != null)
                                            {
                                                client.Player.TransformInfo.FinishTransform();
                                            }
                                            else if (client.Player.OnTransform)
                                                client.Player.TransformationID = 0;
                                            //CheckSkills(client);
                                            client.Player.SSFB = 0;
                                            client.SendSysMesage($"Just wait {CountDown} Seconds and we gonna send you to the event map automatically");
                                            if (!client.Player.Alive)
                                                client.Player.Revive(stream);
                                            return true;
                                        }
                                        else client.CreateBoxDialog($"You can't join a {EventTitle} event while you have other player ({MacJoin[client.OnLogin.MacAddress]}) joined to this event.");
                                    }
                                }
                                else
                                    client.CreateBoxDialog($"You can't join a {EventTitle} event while you're in same Event!");
                            }
                            else
                                client.CreateBoxDialog($"You can't join a {EventTitle} event while you're in guild war.");
                        }
                        else
                            client.CreateBoxDialog($"You can't join a {EventTitle} Event while you're fighting at the Arena Qualifier!");
                    }
                    else
                        client.CreateBoxDialog($"You can't join a {EventTitle} Event while you're fighting at other event!");
                }
                else
                    client.CreateBoxDialog($"There are no events running");
            }
            return false;
        }

        /// <summary>
        /// Removes a player from the current PVP Event
        /// </summary>
        ///
        public virtual void RemovePlayer(GameClient client, bool Diss = false, bool CanTeleport = true)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                client.EventBase = null;

                if (MacJoin.ContainsKey(client.OnLogin.MacAddress))
                    MacJoin.Remove(client.OnLogin.MacAddress);

                if (PlayerList.ContainsKey(client.Player.UID))
                    PlayerList.Remove(client.Player.UID);
                if (PlayerScores.ContainsKey(client.Player.UID))
                    PlayerScores.Remove(client.Player.UID);

                foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> T in Teams)
                    if (T.Value.ContainsKey(client.Player.UID))
                        T.Value.Remove(client.Player.UID);

                if (!Diss)
                {
                    if (AllowedSkills != null)
                    {
                        if (AllowedSkills.Count > 0)
                        {
                            client.Player.RemoveTempEquipment(stream);
                        }
                    }

                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Freeze))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Confused))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Confused);

                    ChangePKMode(client, PKMode.Capture);

                    client.SendSysMesage("", ChatMode.FirstRightCorner);

                    if (CanTeleport)
                    {
                        if (IDMessage == MsgStaticMessage.Messages.HeroOfGame)
                            client.Teleport(439, 367, 1002, 0);
                        else client.ExitToTwin();
                    }
                    else Broadcast("", Game.MsgEvents.BroadCastLoc.Exit);

                    client.Player.Revive(stream);
                }
            }
        }

        /// <summary>
        /// Used to teleport players to event map
        /// </summary>
        public virtual void TeleportPlayersToMap()
        {
            foreach (GameClient client in PlayerList.Values.ToArray())
            {
                ChangePKMode(client, PKMode.PK);
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DinamicID);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Ride);
                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                client.Player.FairbattlePower = TheChosenProject.Role.Flags.FairbattlePower.UpdateToSerf;

            }
        }

        public bool TeleportPlayersToMap(Client.GameClient client, out string message)
        {
            message = "";
            if (Stage != EventStage.Over)
            {
                //if (Duration > 60)
                {
                    ChangePKMode(client, PKMode.PK);
                    ushort x = 0;
                    ushort y = 0;
                    Map.GetRandCoord(ref x, ref y);
                    client.Teleport(x, y, Map.ID, DinamicID);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Ride);
                    client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                    client.Player.FairbattlePower = TheChosenProject.Role.Flags.FairbattlePower.UpdateToSerf;
                    return true;
                }
            }
            else
            {
                message = "Event is over right now.";
            }

            return false;
        }

        /// <summary>
        /// Handles the logic for event protection countdown
        /// </summary>
        ///
        public virtual void Countdown()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (CountDown == 0 && Stage == EventStage.Countdown)
                {
                    CountDown = 5;
                    Stage = EventStage.Starting;
                    Broadcast(EventTitle + " Event is about to start!", BroadCastLoc.Map);
                }
                else if (CountDown > 0)
                {
                    foreach (GameClient client in PlayerList.Values.ToArray())
                    {
                        client.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber{CountDown}" });
                        client.SendSysMesage("", ChatMode.FirstRightCorner);
                    }
                    CountDown--;
                }
                else
                {
                    foreach (GameClient client in PlayerList.Values.ToArray())
                        if (!client.Player.Alive)
                            client.Player.Revive(stream);
                    Stage = EventStage.Fighting;
                    EndTime = DateTime.Now.AddSeconds(Duration);

                    Broadcast(EventTitle + " Event has started! signups are now closed!", BroadCastLoc.World);
                    //Program.DiscordEventsAPI.Enqueue($"`{EventTitle}  Event has started!signups are now closed!`");

                }
            }
        }

        /// <summary>
        /// Handles all the logic during the events and determines conditions to find a winner
        /// </summary>
        public virtual void WaitForWinner()
        {
            foreach (var P in PlayerList.Values.Where(x => !x.Socket.Alive || x.Player.Map != Map.ID).ToArray())
            {
                if (P.Socket.Alive)
                    P.EventBase = null;

                PlayerScores.Remove(P.Player.UID);
                PlayerList.Remove(P.Player.UID);

                foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> T in Teams)
                    if (T.Value.ContainsKey(P.Player.UID))
                        T.Value.Remove(P.Player.UID);
            }

            if (DateTime.Now >= EndTime || (PlayerList.Count <= 1 && EventTitle != "CycloneRace") || Duration <= 0)
                Finish();
            if (DateTime.Now >= DisplayScores.AddMilliseconds(1000))
                DisplayScore();
        }

        /// <summary>
        /// Handles all the character related checks during the event
        /// </summary>
        /// <param name="C"></param>
        public virtual void CharacterChecks(GameClient client)
        {
            if (client.Player.Map != Map.ID)
                RemovePlayer(client);
        }

        /// <summary>
        /// Announces the event end. Gives each player a small protection and changes stage to over
        /// </summary>
        public void Finish()
        {
            Stage = EventStage.Over;
        }

        /// <summary>
        /// Here we choose who we want to reward and such, may depend on teams or w/e... Should add support for teams
        /// </summary>
        public virtual void End()
        {
            if (PlayerList.Count == 1)
                foreach (var client in PlayerList.Values.ToArray())
                    Reward(client);
            else
                Broadcast(Duration + " minutes have passed and no one won the " + EventTitle + " Event! Better luck next time!", BroadCastLoc.World);

            foreach (var client in PlayerList.Values.ToList())
                RemovePlayer(client);

            PlayerList.Clear();
            PlayerScores.Clear();
            MacJoin.Clear();

            Program.Events.Remove(this);
            Broadcast($"{EventTitle} event has ended.", BroadCastLoc.System);
            //Program.DiscordEventsAPI.Enqueue($"``{EventTitle} event has ended.!``");

            return;
        }

        /// <summary>
        /// Used to choose which rewards we want to give
        /// </summary>
        ///
        public virtual void Reward(GameClient client)
        {
            //using (var rec = new ServerSockets.RecycledPacket())
            //{
            //    var stream = rec.GetStream();
            //    client.IncreaseExperience(stream, 600, Role.Flags.ExperienceEffect.angelwing);

            //    //client.Player.QuizPoints += 50;
            //    client.Player.ConquerPoints += RewardCps;
            //    Broadcast($"{client.Player.Name} has won the {EventTitle} event and he received {RewardCps} Cps!", BroadCastLoc.WorldAndChannel);
                //Server.WorldGameChannel.Send($"{client.Player.Name} has won the {EventTitle} event and he received {RewardCps} Cps!", EventTitle);
            //}
            bool DragonBall = false;
            bool MeteorScroll = false;
            bool StoneTwo = false;
            bool PowerExpBall = false;
            bool DragonBallScroll = false;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                //client.Player.ConquerPoints += (uint)ServerKernel.ONE_SOC_RATE;
                client.Player.Money += 150000;//1,075,000
                client.Inventory.Add(stream, 722178, 1);
                client.Player.SendUpdate(stream, client.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);

            }

            //if (client.Player.Level < 137)
                //client.GainExpBall(1600, false, Role.Flags.ExperienceEffect.angelwing);
            //#region EventID reward from panel
            //if (EventID != ISchedule.EventID.None)
            //{
            //    var rwd = ISchedule.GetReward(EventID);
            //    if (rwd != null)
            //    {
            //        switch (Program.GetRandom.Next(1, 1))
            //        {
            //            case 0://item 1
            //                using (var rec = new ServerSockets.RecycledPacket())
            //                {
            //                    var stream = rec.GetStream();
            //                    if (client.Inventory.HaveSpace(1))
            //                    {
            //                        client.Inventory.Add(stream, rwd.ItemOne, 1);//GarmentTicket
            //                    }
            //                    else
            //                    {
            //                        client.Inventory.AddReturnedItem(stream, rwd.ItemOne, 1);
            //                    }
            //                }
            //                //PowerExpBall = true;
            //                break;
            //            case 1: //2
            //                using (var rec = new ServerSockets.RecycledPacket())
            //                {
            //                    var stream = rec.GetStream();
            //                    if (client.Inventory.HaveSpace(1))
            //                    {
            //                        client.Inventory.Add(stream, rwd.ItemTwo, 1);//MeteorScroll
            //                    }
            //                    else
            //                    {
            //                        client.Inventory.AddReturnedItem(stream, rwd.ItemTwo, 1);
            //                    }
            //                }
            //                MeteorScroll = true;
            //                break;
            //            default:
            //                using (var rec = new ServerSockets.RecycledPacket())
            //                {
            //                    var stream = rec.GetStream();
            //                    if (client.Inventory.HaveSpace(1))
            //                    {
            //                        client.Inventory.Add(stream, rwd.ItemThree, 1);//DragonBall
            //                    }
            //                    else
            //                    {
            //                        client.Inventory.AddReturnedItem(stream, rwd.ItemThree, 1);
            //                    }
            //                }
            //                DragonBall = true;
            //                break;
            //        }
            //    }
            //}
            //else
            //#endregion
            //{//hena el eventat ely mlhash control
            //    switch (Program.GetRandom.Next(0, 3))
            //    {
            //        case 0://item 1
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                if (client.Inventory.HaveSpace(1))
            //                {
            //                    client.Inventory.Add(stream, ItemType.MeteorScroll, 1);//DragonBall
            //                }
            //                else
            //                {
            //                    client.Inventory.AddReturnedItem(stream, ItemType.DragonBall, 1);
            //                }
            //            }
            //            MeteorScroll = true;
            //            break;
            //        case 1: //2
            //            using (var rec = new ServerSockets.RecycledPacket())
            //            {
            //                var stream = rec.GetStream();
            //                if (client.Inventory.HaveSpace(1))
            //                {
            //                    client.Inventory.Add(stream, ItemType.MeteorScroll, 1);//MeteorScroll
            //                }
            //                else
            //                {
            //                    client.Inventory.AddReturnedItem(stream, ItemType.MeteorScroll, 1);
            //                }
            //            }
            //            MeteorScroll = true;
            //            break;
            //    }
            //}


            //if (MeteorScroll == true)
            //    Broadcast($"{client.Player.Name} has won the hourly " + EventTitle + " Tournament and received a MeteorScroll and " + 150000 + " Gold", BroadCastLoc.World);
            //if (PowerExpBall == true)
            //    Broadcast($"{client.Player.Name} has won the hourly " + EventTitle + " Tournament and received a PowerExpBall and " + 150000 + " Gold!", BroadCastLoc.World);
            //if (DragonBall == true)
            //    Broadcast($"{client.Player.Name} has won the hourly " + EventTitle + " Tournament and received a DragonBall and " + 150000 + " Gold!", BroadCastLoc.World);
            //if (StoneTwo == true)
            //    Broadcast($"{client.Player.Name} has won the hourly " + EventTitle + " Tournament and received a +2-Stone and " + 150000 + " Gold!", BroadCastLoc.World);
            //if (DragonBallScroll == true)
            //    Broadcast($"{client.Player.Name} has won the hourly " + EventTitle + " Tournament and received a DragonBallScroll and " + 150000 + " Gold!", BroadCastLoc.World);
            //else
            //    Broadcast($"{client.Player.Name} has won the hourly " + EventTitle + " Tournament!", BroadCastLoc.World);
        }

        /// <summary>
        /// Display the score on the top right corner
        /// </summary>
        public virtual void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values.ToArray())
            {
                player.SendSysMesage($"---------{EventTitle}---------", ChatMode.FirstRightCorner);
            }

            byte Score = 2;
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)).ToArray())
            {
                if (Score == 7)
                    break;
                if (Score == PlayerScores.Count + 2)
                    break;
                Broadcast($"Nº {Score - 1}: {PlayerList[kvp.Key].Player.Name} - {kvp.Value}", BroadCastLoc.Score, Score);
                Score++;
            }
        }

        /// <summary>
        /// Determines if we're supposed to do something when a player gets killed
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Victim"></param>
        public virtual void Kill(GameClient Attacker, GameClient Victim)
        {
            if (PlayerScores.ContainsKey(Attacker.Player.UID))
                PlayerScores[Attacker.Player.UID]++;
        }

        /// <summary>
        /// Determines if we're supposed to do something when a NPC gets killed
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Victim"></param>
        public virtual void Kill(GameClient Attacker, SobNpc Victim)
        {
            if (PlayerScores.ContainsKey(Attacker.Player.UID))
                PlayerScores[Attacker.Player.UID]++;
        }

        /// <summary>
        /// Determines wether if an event has unlimited stamina or not and allow us to track number of sent FB/SS
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="SU"></param>
        public virtual void Shot(GameClient Attacker, Database.MagicType.Magic SU)
        {
            Attacker.Player.Stamina -= SU.UseStamina;
        }

        /// <summary>
        /// Determines what we're supposed to do when a player gets hit
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Victim"></param>
        public virtual void Hit(GameClient Attacker, GameClient Victim)
        {
            //PlayerScores[Attacker.EntityID]++;
        }

        public virtual void Hit(GameClient client, SobNpc npc, uint Damage)
        {
        }

        /// <summary>
        /// Overrides the melee damage dealt
        /// </summary>
        /// <param name="User"></param>
        /// <param name="C"></param>
        /// <param name="AttackType"></param>
        /// <returns></returns>
        public virtual uint GetDamage(GameClient User, GameClient C)//, AttackType AttackType
        {
            return 1;
        }

        /// <summary>
        /// Sets events' configuration and starts it, possible to determine how long we want signup to last
        /// </summary>
        /// <param name="_signuptime"></param>
        public void StartTournament(int _signuptime = 30)
        {
            if (Program.Events.Find(x => x.EventTitle == EventTitle) == null)
            {
                PlayerList.Clear();
                PlayerScores.Clear();
                MacJoin.Clear();
                CountDown = _signuptime;
                Stage = EventStage.Inviting;
                if (Map == null)
                {
                    GameMap.EnterMap(this.BaseMap);
                    Map = Server.ServerMaps[this.BaseMap];
                }
                if (IDMessage != MsgStaticMessage.Messages.CycloneRace)
                    DinamicID = Map.GenerateDynamicID();
                if (!Program.FreePkMap.Contains(Map.ID))
                    Program.FreePkMap.Add(Map.ID);
                if (!Program.BlockAttackMap.Contains(WaitingArea))
                    Program.BlockAttackMap.Add(WaitingArea);
                if (!Program.NoDropItems.Contains(WaitingArea))
                    Program.NoDropItems.Add(WaitingArea);
                if (!Program.FreePkMap.Contains(WaitingArea))
                    Program.FreePkMap.Add(WaitingArea);
                if (!Program.NoDropItems.Contains(Map.ID))
                    Program.NoDropItems.Add(Map.ID);
                Program.Events.Add(this);
                BeginTournament();
            }
        }

        /// <summary>
        /// Initializes tournament
        /// </summary>
        ///
        public virtual void BeginTournament()
        {
            Broadcast($"{EventTitle} Event has started! Type @joinpvp or @pvp to sign up within the next 60 seconds!", BroadCastLoc.World);
            Broadcast($"{EventTitle} Event has started! Type @joinpvp or @pvp to sign up within the next 60 seconds!", BroadCastLoc.System);
            //SendInvitation(60, IDMessage);
            //MyLogger.Events.WriteInfo($"{EventTitle} Event has started!");
            //Program.DiscordEventsAPI.Enqueue($"`{EventTitle} Event has started.!`");
        }

        /// <summary>
        /// Tells the server which part of the PVP Event we want to execute next
        /// </summary>
        public void ActionHandler()
        {
            if (Stage == EventStage.Inviting)
            {
                Inviting();
            }
            else if (Stage == EventStage.Countdown || Stage == EventStage.Starting)
                Countdown();
            else if (Stage == EventStage.Fighting)
                WaitForWinner();
            else if (Stage == EventStage.Over)
            {
                Stage = EventStage.None;
                End();
            }
        }

        /// <summary>
        /// Handles the logic while the event is on sign-up
        /// </summary>
        public virtual void Inviting()
        {
            if (CountDown > 0)
            {
                if (CountDown == 120)
                    Broadcast(EventTitle + " Event will start in 2 minutes!", BroadCastLoc.World);
                else if (CountDown == 60)
                    Broadcast(EventTitle + " Event will start in 1 minute!", BroadCastLoc.World);
                else if (CountDown == 10)
                {
                    if (!CanStart())
                    {
                        Broadcast("The " + EventTitle + " Event requires atleast " + minplayers + " players to start! Event was cancelled!", BroadCastLoc.World);
                        Broadcast($"---------{EventTitle}---------", BroadCastLoc.Title, 0);
                        Broadcast("Event cancelled", BroadCastLoc.Score, 2);

                        foreach (GameClient client in PlayerList.Values.ToArray())
                        {
                            RemovePlayer(client);
                        }

                        PlayerList.Clear();
                        PlayerScores.Clear();
                        Stage = EventStage.None;
                        Program.Events.Remove(this);
                        return;
                    }
                    Broadcast("10 seconds until start", BroadCastLoc.Map);
                }
                else if (CountDown < 6)
                    Broadcast(CountDown.ToString() + " seconds until start", BroadCastLoc.Map);
                foreach (GameClient client in PlayerList.Values.ToArray())
                {
                    if (client.Player.Map != WaitingArea || !client.Socket.Alive)
                        RemovePlayer(client, false, false);
                }
                Broadcast($"---------{EventTitle}---------", BroadCastLoc.Title, 0);
                TimeSpan T = TimeSpan.FromSeconds(CountDown);
                Broadcast($"Total Players: " + PlayerList.Count(), BroadCastLoc.Score, 2);
                Broadcast($"Start in: {T.ToString(@"mm\:ss")}", BroadCastLoc.Score, 2);
                --CountDown;
            }
            else
            {
                Stage = EventStage.Countdown;
                TeleportPlayersToMap();
            }
        }

        public virtual void AddPlayerTitle(GameClient client)
        {
            if (client.EventBase == null)
            {
                var events = Program.Events.Find(x => x.EventTitle == EventTitle);
                client.EventBase = events;
                AddPlayer(client);
            }
        }

        public virtual void SendInvitation(int Seconds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            string Message = $"{EventTitle} is about to begin! Will you join it?";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                foreach (var client in Server.GamePoll.Values)
                {
                    if (client.Player.Map == 1038 || client.Player.Map == 6001 || client.Player.Map == 6003 || client.InQualifier())
                    {
                        continue;
                    }
                    else if (client.Player.Map == 1616 || client.Player.Map == Map.ID)
                    {
                        continue;
                    }
                    else 
                    {
                        client.Player.MessageBox(Message, new Action<Client.GameClient>(user => user.Teleport(448, 353, 1002)), null, Seconds, messaj);
                    }
                    //client.Player.LastMan = 0;
                   // client.Player.DragonWar = 0;
                    //client.Player.Infection = 0;
                    //client.Player.FreezeWar = 11;
                    //client.Player.Kungfu = 0;
                   // client.Player.Get5Out = 0;
                    //client.Player.SSFB = 0;
                    //client.Player.TheCaptain = 0;
                    //client.Player.WhackTheThief = 0;
                    //client.Player.VampireWar = 0;

                    //client.Player.MessageBox(Message, new Action<Client.GameClient>(user => user.Teleport(448, 353, 1002)), null, Seconds, messaj);
                }
            }
        }

        public bool InTournament(GameClient user, bool checkmap = false, uint MapID = 0, uint dMapID = 0)
        {
            if (Map == null)
                return false;
            if (checkmap)
            {
                return MapID == Map.ID && dMapID == DinamicID || MapID == WaitingArea;
            }
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID || user.Player.Map == WaitingArea;
        }

        public void RevivePlayer(GameClient client, int amount = 4)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                if (client.Player.DeadStamp.AddSeconds(amount) < Extensions.Time32.Now)
                {
                    ushort x = 0; ushort y = 0;
                    Map.GetRandCoord(ref x, ref y);
                    client.Teleport(x, y, Map.ID, DinamicID);
                    client.Player.Revive(stream);
                }
            }
        }

        /// <summary>
        /// Do all the requirement checks to start the event in here
        /// </summary>
        /// <returns></returns>
        public virtual bool CanStart()
        {
            return PlayerList.Count >= minplayers;
        }

        /// <summary>
        /// Chane PK Mode
        /// </summary>
        ///

        public void ChangePKMode(GameClient client, PKMode Mode)
        {
            client.Player.SetPkMode(Mode);
        }
    }
}