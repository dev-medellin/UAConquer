using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheChosenProject.Database;
using static TheChosenProject.Role.Flags;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.Game.MsgTournaments;
using System.IO;
using MongoDB.Driver.Core.Configuration;
using TheChosenProject.Game.MsgServer.AttackHandler;

namespace TheChosenProject.Game.MsgEvents
{
    public class Get5HitOut
    {
        public static byte MaxHits = 5;
        public static bool Started = false;
        public static bool AcceptingPlayers = false;
        public static List<Client.GameClient> AwaitingPlayers = new List<Client.GameClient>();
        private static List<ushort> AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
        public static GameMap EventMap;
        public static uint DynamicID;
        public static KillerSystem KillSystem = new KillerSystem();
        public static Dictionary<string, string> MacJoin = new Dictionary<string, string>();

        public Get5HitOut()
        {
            Started = true;
            Thread T = new Thread(new ThreadStart(Execute));
            T.Name = "FiveFuryTrial";
            MacJoin.Clear();
            T.Start();
        }

        public static void JoinClient(Client.GameClient client)
        {
            //if (!MacJoin.ContainsKey(client.OnLogin.MacAddress))
            //{
                //MacJoin.Add(client.OnLogin.MacAddress, client.Player.Name);
                if (AcceptingPlayers)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!AwaitingPlayers.Contains(client))
                        {
                            AwaitingPlayers.Add(client);
                            client.SendSysMesage("You are now in Queue for a FiveFuryTrial event.");
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{client.Player.Name} join to FiveFuryTrial.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Talk).GetArray(stream));
                        }
                        else client.SendSysMesage("You are now in Queue for a FiveFuryTrial event.");
                    }
                }
            }
           // else client.CreateBoxDialog($"You can't join a event while you have other player ({MacJoin[client.OnLogin.MacAddress]}) joined to this event.");

        //}

        private static Client.GameClient[] GetPlayers()
        {
            return EventMap.Values.Where(p=> p.Player.DynamicID == DynamicID && p.InFIveOut).ToArray();
        }
        public static List<coords> Randomtele = new List<coords>() {
            new coords(34,35) { },
            new coords(67,35) { },
            new coords(68,67) { },
            new coords(35,67) { },
            new coords(50,50) { }
            };
        private void Execute()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                #region SetupMap
                if (EventMap == null)
                {
                    EventMap = Server.ServerMaps[3040];
                    DynamicID = EventMap.GenerateDynamicID();

                }
                #endregion

                #region Sign Ups
                AcceptingPlayers = true;
                foreach (var client in Server.GamePoll.Values)
                {
                    client.Player.MessageBox("FiveFuryTrial Event will be starting, Do you want to join", new Action<Client.GameClient>(p => { p.Teleport(457, 352, 1002);  /*JoinClient(p);*/ }), null, 60);
                }
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("FiveFuryTrial Event will be starting in 60 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(30000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("FiveFuryTrial Event will be starting in 30 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(20000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("10 Seconds left before FiveFuryTrial starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("5 Seconds left before FiveFuryTrial starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                //Program.DiscordEventsAPI.Enqueue("FiveFuryTrial Has Started");
                AcceptingPlayers = false;
                #endregion

                #region Check if there is enough players to play
                if (AwaitingPlayers.Count <= 1)
                {
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Error. There were not enough players signed up to start.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    AwaitingPlayers.Clear();
                    Started = false;
                    return;
                }
                #endregion

                #region Setup Characters
                Client.GameClient[] _chrz = AwaitingPlayers.ToArray();
                foreach (Client.GameClient client in _chrz)
                {
                    var spawnLoc = Randomtele[Program.Rand.Next(0, Randomtele.Count)];
                    client.Teleport((ushort)spawnLoc.X, (ushort)spawnLoc.Y, EventMap.ID, DynamicID);
                    client.Player.Revive(stream);
                    client.FIveOutHits = 0;
                    client.InFIveOut = true;
                    //client.Player.AddTempEquipment(stream, 410239, ConquerItem.RightWeapon);
                    //client.Player.AddTempEquipment(stream, 420239, ConquerItem.LeftWeapon);
                    client.Player.SetPkMode(Role.Flags.PKMode.PK);
                    //foreach (var skillid in AllowedSkills)
                    //{
                    //    if (!client.MySpells.ClientSpells.ContainsKey(skillid))
                    //    {
                    //        client.MySpells.Add(stream, skillid, 4);
                    //    }
                    //    else
                    //    {
                    //        client.MySpells.ClientSpells[skillid].Level = 4;
                    //        client.Send(stream.SpellCreate(client.MySpells.ClientSpells[skillid]));
                    //    }
                    //}
                }
                AwaitingPlayers.Clear();
                #endregion

                #region Initial Countdown & Start
                Thread.Sleep(100);
                _chrz = GetPlayers();
                foreach (Client.GameClient C in _chrz)
                    if (C != null && C.InFIveOut)
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber3" });
                Thread.Sleep(1000); //Wait 1 Second
                _chrz = GetPlayers();
                foreach (Client.GameClient C in _chrz)
                    if (C != null && C.InFIveOut)
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber2" });
                Thread.Sleep(1000); //Wait 1 Second
                _chrz = GetPlayers();
                foreach (Client.GameClient C in _chrz)
                    if (C != null && C.InFIveOut)
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber1" });
                Thread.Sleep(1000); //Wait 1 Second
                _chrz = GetPlayers();
                foreach (Client.GameClient C in _chrz)
                {
                    if (C != null && C.InFIveOut)
                    {
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber0" });
                        C.SendSysMesage("Begin!!!", MsgServer.MsgMessage.ChatMode.Center);
                    }
                }
                #endregion

                #region Wait For Winner
                while (Started)
                {
                    _chrz = GetPlayers();
                    foreach (var client in _chrz)
                    {
                        client.SendSysMesage($"---------FiveFuryTrial---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                        client.SendSysMesage($"Players  : {_chrz.Length}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        client.SendSysMesage($"Hits Left: {Math.Min(MaxHits, MaxHits - client.FIveOutHits)}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                    }
                    Thread.Sleep(2000);
                }
                #endregion

                #region Awards distribution
                _chrz = GetPlayers();
                if (_chrz.Length > 0)
                {
                    var hero = _chrz[0];
                    #region reward
                    {
                        using (var rec2 = new ServerSockets.RecycledPacket())
                        {
                            var stream2 = rec2.GetStream();
                            hero.Player.Money += 150000;
                            hero.Inventory.Add(stream, 722178, 1);
                            hero.Player.SendUpdate(stream, hero.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                            hero.Player.TournamentsPoints++;

                        }

                        if (hero.Player.Level < 137)
                            hero.GainExpBall(3600, false, Role.Flags.ExperienceEffect.angelwing);

                        if (hero.Inventory.HaveSpace(1))
                        {
                            hero.Inventory.Add(stream, 730002, 1);//+3
                        }
                        else
                        {
                            hero.Inventory.AddReturnedItem(stream, 730002, 1);
                        }
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{hero.Player.Name} Won FiveFuryTrial Tournament and received a 50,000 Gold and [SurpriseBox] and (1) Tournament Points!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        
                    }
                    #endregion
                    using (var conn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString))
                    {
                        using (var cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into cq_events (EntityID,name,type) " +
                            "values (@EntityID,@name,@type)"
                            , conn))
                        {
                            conn.Open();
                            #region ClearCommand
                            try
                            {
                                using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from cq_events where type=4", conn))
                                    cmdd.ExecuteNonQuery();

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                            #endregion

                            cmd.Parameters.AddWithValue("@EntityID", hero.Player.UID);
                            cmd.Parameters.AddWithValue("@name", hero.Player.Name);
                            cmd.Parameters.AddWithValue("@type", "4");

                            cmd.ExecuteNonQuery();

                        }
                    }
                    //Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{hero.Player.Name} Won Get5HitOut!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    hero.SendSysMesage("You won in FiveFuryTrial check your Inventory.");
                }
                #endregion

                #region Unset Event Handlers
                _chrz = GetPlayers();
                foreach (Client.GameClient c in _chrz)
                {
                    c.InFIveOut = false;
                    c.Player.RemoveTempEquipment(stream);
                    c.Teleport(429, 378, 1002);
                    c.SendSysMesage("", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                }
                #endregion
                MacJoin.Clear();
                AwaitingPlayers.Clear();

            }
        }
        public string ConnectionString = DatabaseConfig.ConnectionString;

        public static void OnTakeAttack(Client.GameClient Attacked, ref uint Damage)
        {
            Damage = 1;
            Attacked.FIveOutHits++;
            if (Attacked.FIveOutHits >= MaxHits)
            {
                Attacked.Teleport(428, 378, 1002);
                KillSystem.Update(Attacked);
                KillSystem.CheckDead(Attacked.Player.UID);
                if (MacJoin.ContainsKey(Attacked.OnLogin.MacAddress))
                    MacJoin.Remove(Attacked.OnLogin.MacAddress);
            }
        }

        public static void OnTeleport(Client.GameClient arg1)
        {
            if (Started)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    arg1.InFIveOut = false;
                    arg1.FIveOutHits = 0;
                    var chars = GetPlayers();
                    if(chars.Length == 1)
                    {
                        Started = false;
                    }
                    arg1.Player.RemoveTempEquipment(stream);
                    if (MacJoin.ContainsKey(arg1.OnLogin.MacAddress))
                        MacJoin.Remove(arg1.OnLogin.MacAddress);
                    arg1.Player.SetPkMode(Role.Flags.PKMode.Capture);
                }
            }
        }

        public static void OnDisconnect(Client.GameClient obj)
        {
            if (Started)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    obj.InFIveOut = false;
                    obj.FIveOutHits = 0;
                    var chars = GetPlayers();
                    if (chars.Length == 1)
                    {
                        Started = false;
                    }
                    obj.Player.Map = 1002;
                    obj.Player.X = 429;
                    obj.Player.Y = 378;
                    obj.Player.RemoveTempEquipment(stream);
                    if (MacJoin.ContainsKey(obj.OnLogin.MacAddress))
                        MacJoin.Remove(obj.OnLogin.MacAddress);
                }

            }
        }
    }
}
