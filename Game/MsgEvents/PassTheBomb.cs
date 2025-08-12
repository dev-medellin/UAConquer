using DevExpress.XtraPrinting.Native;
using Extensions;
using MongoDB.Driver.Core.Configuration;
using Poker;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;
using static TheChosenProject.Role.Flags;
using static TheChosenProject.Role.KOBoard;

namespace TheChosenProject.Game.MsgEvents
{
    public class PassTheBomb
    {
        public static int LastStarted = 0;
        public static bool Started = false;
        public static bool AcceptingPlayers = false;
        public static bool PlayerHasBomb = false;
        private static List<ushort> AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
        public static List<GameClient> AwaitingPlayers = new List<GameClient>();
        private static List<GameClient> CurrentPlayers = new List<GameClient>();
        private static GameMap EventMap;
        private static uint DynamicID;
        public static Dictionary<string, string> MacJoin = new Dictionary<string, string>();
        public static KillerSystem KillSystem = new KillerSystem();

        private GameClient Bomber
        {
            get
            {
                GameClient[] _charz = CurrentPlayers.ToArray();
                foreach (GameClient C in _charz)
                    if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.BlueBall))
                        return C;
                return null;
            }
        }

        public PassTheBomb()
        {
            if (!Started)
            {
                Started = true;
                AcceptingPlayers = true;
                Thread T = new Thread(new ThreadStart(Execute));
                T.Name = "InfernoDetonation";
                MacJoin.Clear();
                T.Start();
            }
        }

        public static void JoinClient(Client.GameClient client)
        {
            if (!MacJoin.ContainsKey(client.OnLogin.MacAddress))
            {
                MacJoin.Add(client.OnLogin.MacAddress, client.Player.Name);
                if (AcceptingPlayers)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!AwaitingPlayers.Contains(client))
                        {
                            AwaitingPlayers.Add(client);
                            client.SendSysMesage("You are now in Queue for a InfernoDetonation event.");
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{client.Player.Name} join to InfernoDetonation.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                        else client.SendSysMesage("You are now in Queue for a InfernoDetonation event.");
                    }
                }
            }
           // else client.CreateBoxDialog($"You can't join a event while you have other player ({MacJoin[client.OnLogin.MacAddress]}) joined to this event.");

        }


        private static Client.GameClient[] GetPlayers()
        {
            return EventMap.Values.Where(p => p.Player.DynamicID == DynamicID && p.InPassTheBomb).ToArray();
        }

        public static void PassBomb()
        {
            if (CurrentPlayers.Count > 1)
            {
                GameClient NewBomber = CurrentPlayers[Program.GetRandom.Next(0, CurrentPlayers.Count)];
                if (NewBomber != null)
                {
                    NewBomber.Player.AddFlag(MsgUpdate.Flags.BlueBall, 6666666, true);
                    NewBomber.SendSysMesage("You now have the bomb. You must hit someone with InfernoDetonation to get rid of it.", MsgMessage.ChatMode.TopLeft);
                }
                if (MacJoin.ContainsKey(NewBomber.OnLogin.MacAddress))
                    MacJoin.Remove(NewBomber.OnLogin.MacAddress);
            }
            
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
                    client.Player.MessageBox("InfernoDetonation Event will be starting, Do you want to join", new Action<Client.GameClient>(p => { p.Teleport(457, 352, 1002);  /*JoinClient(p);*/ }), null, 60);
                }

                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("InfernoDetonation Event will be starting in 60 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(30000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("InfernoDetonation Event will be starting in 30 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(20000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("10 Seconds left before InfernoDetonation starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("5 Seconds left before InfernoDetonation starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                //Program.DiscordEventsAPI.Enqueue("InfernoDetonation Has Started");
                AcceptingPlayers = false;
                #endregion

                #region Check if there is enough players to play
                if (AwaitingPlayers.Count <= 1)
                {
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Error. There were not enough players signed up to start.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                    CurrentPlayers.Clear();
                    AwaitingPlayers.Clear();
                    Started = false;
                    return;
                }
                #endregion

                #region Setup Characters
                Client.GameClient[] _chrz = AwaitingPlayers.ToArray();
                foreach (Client.GameClient client in _chrz)
                {
                    //ushort x = 0, y = 0;
                    //EventMap.GetRandCoord(ref x, ref y);
                    var spawnLoc = Randomtele[Program.Rand.Next(0, Randomtele.Count)];
                    client.Teleport((ushort)spawnLoc.X, (ushort)spawnLoc.Y, EventMap.ID, DynamicID);
                    client.Player.Revive(stream);
                    client.InPassTheBomb = true;
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
                    CurrentPlayers.Add(client);
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
                    if (C != null && C.InPassTheBomb)
                    {
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber0" });
                        C.SendSysMesage("Begin!!!", MsgServer.MsgMessage.ChatMode.Center);
                    }
                }
                #endregion

                PassBomb();

                while (CurrentPlayers.Count > 1)
                {
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber9" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber8" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber7" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber6" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber5" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber4" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber3" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber2" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                        Bomber.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber1" });
                    else
                        PassBomb();
                    Thread.Sleep(2000);
                    if (Bomber != null)
                    {
                        GameClient B = Bomber;
                        int c = CurrentPlayers.Count;
                        if (c == 3)
                        {
                            
                                Bomber.Player.Money += 50000;
                                Bomber.Player.SendUpdate(stream, Bomber.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);

                            if (Bomber.Player.Level < 137)
                                Bomber.GainExpBall(3600, false, Role.Flags.ExperienceEffect.angelwing);

                            if (Bomber.Inventory.HaveSpace(1))
                            {
                                Bomber.Inventory.Add(stream, 730002, 1);//+3
                            }
                            else
                            {
                                Bomber.Inventory.AddReturnedItem(stream, 730002, 1);
                            }

                            Bomber.SendSysMesage("You took 3rd place in PTB and were rewarded 50,000 Conquer Money", MsgMessage.ChatMode.Talk);
                        }
                        else if (c == 2)
                        {
                            Bomber.Player.Money += 100000;
                            Bomber.Player.SendUpdate(stream, Bomber.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                            if (Bomber.Player.Level < 137)
                                Bomber.GainExpBall(3600, false, Role.Flags.ExperienceEffect.angelwing);

                            if (Bomber.Inventory.HaveSpace(1))
                            {
                                Bomber.Inventory.Add(stream, 730001, 1);//+3
                            }
                            else
                            {
                                Bomber.Inventory.AddReturnedItem(stream, 730001, 1);
                            }
                            Bomber.SendSysMesage("You took 2nd place in PTB and were rewarded 100,000 Conquer Money.", MsgMessage.ChatMode.Talk);

                        }
                        CurrentPlayers.Remove(B);
                        B.Teleport(429, 378, 1002);
                        PassBomb();
                    }
                    if (Bomber == null)
                        PassBomb();
                }
                GameClient Winner;
                if (CurrentPlayers.Count > 0)
                {
                    Winner = CurrentPlayers[0];
                    if (Winner != null)
                    {
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{Winner.Player.Name}  has won Pass The Bomb!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        Winner.Player.Money += 150000;
                        Winner.Inventory.Add(stream, 722178, 1);
                        Winner.Player.SendUpdate(stream, Winner.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                        Winner.Player.TournamentsPoints++;
                        //if (Winner.Player.Level < 137)
                        //    Winner.GainExpBall(3600, false, Role.Flags.ExperienceEffect.angelwing);
                        //if (Winner.Inventory.HaveSpace(6))
                        //{
                        //    Winner.Inventory.Add(stream, 730003, 1);//+3
                        //    Winner.Inventory.Add(stream, ItemType.DragonBall, 5);//DragonBall

                        //}
                        //else
                        //{
                        //    Winner.Inventory.AddReturnedItem(stream, 730003, 1);
                        //    Winner.Inventory.AddReturnedItem(stream, ItemType.DragonBall, 5);

                        //}
                        Winner.SendSysMesage("You took 1st place in PTB and were rewarded 150,000 Conquer Money and [SurpriseBox] and 1 Tournament Points.", MsgMessage.ChatMode.Talk);
                        Winner.Teleport( 429, 378,1002);
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
                                    using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from cq_events where type=2", conn))
                                        cmdd.ExecuteNonQuery();

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                                #endregion

                                cmd.Parameters.AddWithValue("@EntityID", Winner.Player.UID);
                                cmd.Parameters.AddWithValue("@name", Winner.Player.Name);
                                cmd.Parameters.AddWithValue("@type", "2");

                                cmd.ExecuteNonQuery();

                            }
                        }
                    }
                }
                CurrentPlayers.Clear();
                AwaitingPlayers.Clear();
                MacJoin.Clear();
                Started = false;
            }
        }
        public string ConnectionString = DatabaseConfig.ConnectionString;


        public static void OnTakeAttack(Client.GameClient Attacker, Client.GameClient Attacked, ref uint Damage)
        {
            Damage = 1;
            if (Attacker.Player.ContainFlag(MsgUpdate.Flags.BlueBall))
            {
                Attacker.Player.RemoveFlag(MsgUpdate.Flags.BlueBall);
                Attacked.Player.AddFlag(MsgUpdate.Flags.BlueBall, 6666666, true);
                Attacked.SendSysMesage("You now have the bomb. You must hit someone with FB/SS to get rid of it.", MsgMessage.ChatMode.Talk);
            }
            KillSystem.Update(Attacked);
            KillSystem.CheckDead(Attacked.Player.UID);
        }

        public static void OnTeleport(Client.GameClient arg1)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                arg1.InPassTheBomb = false;
                if (arg1.Player.ContainFlag(MsgUpdate.Flags.BlueBall))
                {
                    arg1.Player.RemoveFlag(MsgUpdate.Flags.BlueBall);
                    PassBomb();
                }
                if (CurrentPlayers.Contains(arg1))
                    CurrentPlayers.Remove(arg1);
                arg1.Player.RemoveTempEquipment(stream);
                if (MacJoin.ContainsKey(arg1.OnLogin.MacAddress))
                    MacJoin.Remove(arg1.OnLogin.MacAddress);
                arg1.Player.SetPkMode(Role.Flags.PKMode.Capture);
            }
        }

        public static void OnDisconnect(Client.GameClient obj)
        {
            if (PassTheBomb.Started)
            {
                if (obj.Player.ContainFlag(MsgUpdate.Flags.BlueBall))
                {
                    obj.Player.RemoveFlag(MsgUpdate.Flags.BlueBall);
                    PassBomb();
                }
                if (CurrentPlayers.Contains(obj))
                    CurrentPlayers.Remove(obj);
                obj.InPassTheBomb = false;
                obj.Player.Map = 1002;
                obj.Player.X = 429;
                obj.Player.Y = 378;
                if (MacJoin.ContainsKey(obj.OnLogin.MacAddress))
                    MacJoin.Remove(obj.OnLogin.MacAddress);
            }
        }        
    }
}
