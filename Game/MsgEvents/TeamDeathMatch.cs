using DevExpress.XtraPrinting.Native;
using Extensions;
using Poker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler;
using TheChosenProject.Game.MsgTournaments;
using static TheChosenProject.Role.Flags;
using static TheChosenProject.Role.KOBoard;

namespace TheChosenProject.Game.MsgEvents
{
    public class TeamDeathMatch
    {
        public static byte MaxHits = 10;
        public static bool Started = false;
        public static bool AcceptingPlayers = false;
        private static bool WhiteTeamWon = false;
        public static List<Client.GameClient> AwaitingPlayers = new List<Client.GameClient>();
        private static List<Client.GameClient> BlueTeam = new List<Client.GameClient>();
        private static List<Client.GameClient> RedTeam = new List<Client.GameClient>();
        public static uint LastPlayerUID = 0;
        private static List<ushort> AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
        public static KillerSystem KillSystem = new KillerSystem();
        public static Dictionary<string, string> MacJoin = new Dictionary<string, string>();

        public TeamDeathMatch()
        {
            Started = true;
            Thread T = new Thread(new ThreadStart(Execute));
            T.Name = "SquadShowDown";
            MacJoin.Clear();
            T.Start();
            foreach(var client in Server.GamePoll.Values)
            {
                client.Player.MessageBox("SquadShowDown Event will be starting, Do you want to join", new Action<Client.GameClient>(p => { p.Teleport(457, 352, 1002); }), null, 60);
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
                            LastPlayerUID = client.Player.UID;
                            client.SendSysMesage("You are now in Queue for a SquadShowDown event.");
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{client.Player.Name} join to SquadShowDown.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));

                        }
                        else client.SendSysMesage("You are now in Queue for a SquadShowDown event.");
                    }
                }
            }
          //  else client.CreateBoxDialog($"You can't join a event while you have other player ({MacJoin[client.OnLogin.MacAddress]}) joined to this event.");
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
                #region Sign Ups
                AcceptingPlayers = true;
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("SquadShowDown Event will be starting in 60 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(30000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("SquadShowDown Event will be starting in 30 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(20000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Ten Seconds left before SquadShowDown starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Five Seconds left before SquadShowDown starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                //Program.DiscordEventsAPI.Enqueue("SquadShowDown Has Started");
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

                #region Make Teams Even
                if (AwaitingPlayers.Count % 2 != 0)
                {
                    bool Found = false;
                    Client.GameClient KickedPlayer = null;
                    Client.GameClient[] _players = AwaitingPlayers.ToArray();
                    foreach (Client.GameClient player in _players)
                    {
                        if (player != null)
                        {
                            if (player.Socket != null && player.Socket.Connection.Connected)
                            {
                                if (player.Player.UID == LastPlayerUID)
                                {
                                    Found = true;
                                    KickedPlayer = player;
                                    break;
                                }
                            }
                        }
                    }
                    if (!Found)
                        KickedPlayer = AwaitingPlayers[Program.GetRandom.Next(0, AwaitingPlayers.Count)];
                    KickedPlayer.SendSysMesage("Sorry, but you were randomly selected to be the odd man out.");
                    AwaitingPlayers.Remove(KickedPlayer);
                }
                #endregion

                #region Assign Teams
                int PlayersPerTeam = AwaitingPlayers.Count / 2;
                for (int i = 0; i < PlayersPerTeam; i++)
                    BlueTeam.Add(AwaitingPlayers[i]);
                for (int i = PlayersPerTeam; i < PlayersPerTeam * 2; i++)
                    RedTeam.Add(AwaitingPlayers[i]);
                #endregion

                #region Setup Characters
                var TDM_MAP = Server.ServerMaps[3040];
                Client.GameClient[] _whiteMembers = BlueTeam.ToArray();
                foreach (Client.GameClient WhiteMem in _whiteMembers)
                {
                    //ushort x = 0, y = 0;
                    //TDM_MAP.GetRandCoord(ref x, ref y);
                    //WhiteMem.Teleport(x, y, 3040);
                    var spawnLoc = Randomtele[Program.Rand.Next(0, Randomtele.Count)];
                    WhiteMem.Teleport((ushort)spawnLoc.X, (ushort)spawnLoc.Y, 3040);
                    WhiteMem.Player.Revive(stream);
                    WhiteMem.InTDM = true;
                    WhiteMem.TDMHits = 0;
                    WhiteMem.OnWhiteTeam = true;
                    WhiteMem.Player.AddTempEquipment(stream, 183425, ConquerItem.Garment);
                    //WhiteMem.Player.AddTempEquipment(stream, 410239, ConquerItem.RightWeapon);
                    //WhiteMem.Player.AddTempEquipment(stream, 420239, ConquerItem.LeftWeapon);
                    WhiteMem.Player.SetPkMode(Role.Flags.PKMode.PK);
                    //foreach (var skillid in AllowedSkills)
                    //{
                    //    if (!WhiteMem.MySpells.ClientSpells.ContainsKey(skillid))
                    //    {
                    //        WhiteMem.MySpells.Add(stream, skillid, 4);
                    //    }
                    //    else
                    //    {
                    //        WhiteMem.MySpells.ClientSpells[skillid].Level = 4;
                    //        WhiteMem.Send(stream.SpellCreate(WhiteMem.MySpells.ClientSpells[skillid]));
                    //    }
                    //}
                }
                Client.GameClient[] _blackMembers = RedTeam.ToArray();
                foreach (Client.GameClient BlackMem in _blackMembers)
                {
                    //ushort x = 0, y = 0;
                    var spawnLoc = Randomtele[Program.Rand.Next(0, Randomtele.Count)];
                    BlackMem.Teleport((ushort)spawnLoc.X, (ushort)spawnLoc.Y, 3040);
                    BlackMem.Player.Revive(stream);
                    BlackMem.InTDM = true;
                    BlackMem.TDMHits = 0;
                    BlackMem.OnWhiteTeam = false;
                    BlackMem.Player.AddTempEquipment(stream, 191305, ConquerItem.Garment);
                    //BlackMem.Player.AddTempEquipment(stream, 410239, ConquerItem.RightWeapon);
                    //BlackMem.Player.AddTempEquipment(stream, 420239, ConquerItem.LeftWeapon);
                    BlackMem.Player.SetPkMode(Role.Flags.PKMode.PK);
                    //foreach (var skillid in AllowedSkills)
                    //{
                    //    if (!BlackMem.MySpells.ClientSpells.ContainsKey(skillid))
                    //    {
                    //        BlackMem.MySpells.Add(stream, skillid, 4);
                    //    }
                    //    else
                    //    {
                    //        BlackMem.MySpells.ClientSpells[skillid].Level = 4;
                    //        BlackMem.Send(stream.SpellCreate(BlackMem.MySpells.ClientSpells[skillid]));
                    //    }
                    //}
                }
                #endregion

                AwaitingPlayers.Clear();

                #region Initial Countdown & Start
                Thread.Sleep(100);
                foreach (Client.GameClient C in BlueTeam)
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber3" });

                foreach (Client.GameClient C in RedTeam)
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber3" });

                Thread.Sleep(1000); //Wait 1 Second

                foreach (Client.GameClient C in BlueTeam)
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber2" });
                foreach (Client.GameClient C in RedTeam)
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber2" });

                Thread.Sleep(1000); //Wait 1 Second

                foreach (Client.GameClient C in BlueTeam)
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber1" });
                foreach (Client.GameClient C in RedTeam)
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber1" });

                Thread.Sleep(1000); //Wait 1 Second

                foreach (Client.GameClient C in BlueTeam)
                {
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber0" });
                    C.SendSysMesage("Begin!!!", MsgServer.MsgMessage.ChatMode.Center);
                }
                foreach (Client.GameClient C in RedTeam)
                {
                    C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber0" });
                    C.SendSysMesage("Begin!!!", MsgServer.MsgMessage.ChatMode.Center);
                }
                #endregion

                #region Wait For Winner
                while (Started)
                {
                    foreach(var client in TDM_MAP.Values)
                    {
                        client.SendSysMesage($"---------TeamDeathMatch---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                        client.SendSysMesage($"BlueTeam Count - {BlueTeam.Count}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        client.SendSysMesage($"RedTeam Count  - {RedTeam.Count}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        client.SendSysMesage($"", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        client.SendSysMesage($"----------------------", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        client.SendSysMesage($"Hits Left: {Math.Min(MaxHits, MaxHits - client.TDMHits)}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                    }
                    Thread.Sleep(3000);
                }
                #endregion

                

                #region Awards distribution
                Client.GameClient[] winners = null;
                if(BlueTeam.Count > 0)
                    winners = BlueTeam.ToArray();
                else winners = RedTeam.ToArray();
                foreach (Client.GameClient client in winners)
                {
                    
                    #region Announce Winner
                    if (WhiteTeamWon)
                    {

                        #region reward
                        {


                            using (var rec2 = new ServerSockets.RecycledPacket())
                            {
                                var stream2 = rec2.GetStream();
                                client.Player.Money += 50000;
                                client.Inventory.Add(stream, 722178, 1);
                                client.Player.SendUpdate(stream, client.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                                client.Player.TournamentsPoints++;
                            }

                            //if (client.Player.Level < 137)
                            //    client.GainExpBall(3600, false, Role.Flags.ExperienceEffect.angelwing);
                            //if (client.Inventory.HaveSpace(1))
                            //{
                            //    client.Inventory.Add(stream, 730003, 1);//+2
                            //}
                            //else
                            //{
                            //    client.Inventory.AddReturnedItem(stream, 730003, 1);
                            //}
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"The Blue Team Won SquadShowDown Tournament and received a 50,000 Conquer Money and [SurpriseBox] and (1) Tournament Points !", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }
                            #endregion
                            
                    }
                    else
                    {
                       
                        #region reward
                        {


                            using (var rec2 = new ServerSockets.RecycledPacket())
                            {
                                var stream2 = rec2.GetStream();
                                client.Player.Money += 150000;
                                client.Inventory.Add(stream, 722178, 1);
                                client.Player.SendUpdate(stream, client.Player.Money, Game.MsgServer.MsgUpdate.DataType.Money);
                                client.Player.TournamentsPoints++;
                            }

                            //if (client.Player.Level < 137)
                            //    client.GainExpBall(3600, false, Role.Flags.ExperienceEffect.angelwing);
                            //if (client.Inventory.HaveSpace(1))
                            //{
                            //    client.Inventory.Add(stream, 730003, 1);//+2
                            //}
                            //else
                            //{
                            //    client.Inventory.AddReturnedItem(stream, 730004, 1);
                            //}
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"The Red Team Won SquadShowDown Tournament and received a 150,000 Gold!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));



                        }
                        #endregion                       
                    }
                    #endregion
                    client.SendSysMesage("You won in SquadShowDown check your Inventory.");
                }
                #endregion

                #region Unset Event Handlers
                _whiteMembers = BlueTeam.ToArray();
                foreach (Client.GameClient WhiteMem in _whiteMembers)
                {
                    WhiteMem.InTDM = false;
                    WhiteMem.Player.RemoveTempEquipment(stream);
                    WhiteMem.Teleport(429, 378, 1002);
                    WhiteMem.SendSysMesage("", MsgServer.MsgMessage.ChatMode.FirstRightCorner);

                }

                _blackMembers = RedTeam.ToArray();
                foreach (Client.GameClient BlackMem in _blackMembers)
                {
                    BlackMem.InTDM = false;
                    BlackMem.Player.RemoveTempEquipment(stream);
                    BlackMem.Teleport(429, 378, 1002);
                    BlackMem.SendSysMesage("", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                }
                #endregion

                AwaitingPlayers.Clear();
                BlueTeam.Clear();
                RedTeam.Clear();
                MacJoin.Clear();

            }
        }

        public static void OnTakeAttack(Client.GameClient Attacked, ref uint Damage)
        {
            Damage = 1;
            Attacked.TDMHits++;
            if (Attacked.TDMHits >= MaxHits)
                Attacked.Teleport(428, 378, 1002);
            KillSystem.Update(Attacked);
            KillSystem.CheckDead(Attacked.Player.UID);
            if (MacJoin.ContainsKey(Attacked.OnLogin.MacAddress))
                MacJoin.Remove(Attacked.OnLogin.MacAddress);
        }

        public static void OnTeleport(Client.GameClient arg1)
        {
            if (Game.MsgEvents.TeamDeathMatch.Started)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    if (BlueTeam.Contains(arg1))
                        BlueTeam.Remove(arg1);
                    if (RedTeam.Contains(arg1))
                        RedTeam.Remove(arg1);
                        arg1.TDMHits = 0;

                    arg1.InTDM = false;
                    arg1.OnWhiteTeam = false;
                    if (BlueTeam.Count == 0)
                    {
                        WhiteTeamWon = false;
                        Started = false;
                    }
                    if (RedTeam.Count == 0)
                    {
                        WhiteTeamWon = true;
                        Started = false;
                    }
                    if (MacJoin.ContainsKey(arg1.OnLogin.MacAddress))
                        MacJoin.Remove(arg1.OnLogin.MacAddress);
                    arg1.Player.RemoveTempEquipment(stream);
                    arg1.Player.SetPkMode(Role.Flags.PKMode.Capture);
                }
            }
        }

        public static void OnDisconnect(Client.GameClient obj)
        {
            if (Game.MsgEvents.TeamDeathMatch.Started)
            {
                if (BlueTeam.Contains(obj))
                    BlueTeam.Remove(obj);
                if (RedTeam.Contains(obj))
                    RedTeam.Remove(obj);
                obj.TDMHits = 0;
                if (MacJoin.ContainsKey(obj.OnLogin.MacAddress))
                    MacJoin.Remove(obj.OnLogin.MacAddress);
                if (BlueTeam.Count == 0)
                {
                    WhiteTeamWon = false;
                    Started = false;
                }
                if (RedTeam.Count == 0)
                {
                    WhiteTeamWon = true;
                    Started = false;
                }
                obj.OnWhiteTeam = false;
                obj.InTDM = false;
                obj.Player.Map = 1002;
                obj.Player.X = 429;
                obj.Player.Y = 378;
            }
        }
    }
}
