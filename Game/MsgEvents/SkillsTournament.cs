using DevExpress.Data.Extensions;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using Extensions;
using MongoDB.Driver.Core.Configuration;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.AI;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgServer.AttackHandler;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;
using TheChosenProject.ServerCore;
using TheChosenProject.ServerSockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static TheChosenProject.Database.ClientSpells;
using static TheChosenProject.Game.MsgServer.MsgAttackPacket;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using static TheChosenProject.Role.Flags;
using static TheChosenProject.Role.KOBoard;

namespace TheChosenProject.Game.MsgEvents
{
    public class SkillsTournament
    {
        public static bool Started = false;
        public static bool AcceptingPlayers = false;
        public static List<Client.GameClient> AwaitingPlayers = new List<Client.GameClient>();
        private static List<ushort> AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
        private static GameMap EventMap;
        private static uint DynamicID;
        private static SafeDictionary<uint, int> Hits = new SafeDictionary<uint, int>();
        public static KillerSystem KillSystem = new KillerSystem();
        public static Dictionary<string, string> MacJoin = new Dictionary<string, string>();

        public static bool AcceptAddPlayers = false;

        public SkillsTournament()
        {
            if (!Started)
            {
                Started = true;
                AcceptingPlayers = true;
                Thread T = new Thread(new ThreadStart(Execute));
                T.Name = "[SupremeDuelist]Tournament";
                MacJoin.Clear();
                T.Start();
            }
        }

        public static void JoinClient(Client.GameClient client)
        {
            //if (!MacJoin.ContainsKey(client.OnLogin.MacAddress))
            {
                //MacJoin.Add(client.OnLogin.MacAddress, client.Player.Name);

                if (AcceptingPlayers)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (!AwaitingPlayers.Contains(client))
                        {
                            AwaitingPlayers.Add(client);
                            client.SendSysMesage("You are now in Queue for a [SupremeDuelist]Tournament event.");
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{client.Player.Name} join to [SupremeDuelist]Tournament.", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                        }
                        else client.SendSysMesage("You are now in Queue for a [SupremeDuelist]Tournament event.");
                    }
                }

                else if (AcceptAddPlayers)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        //ushort x = 0, y = 0;
                        //EventMap.GetRandCoord(ref x, ref y);
                        client.Teleport(50, 50, EventMap.ID, DynamicID);
                        client.Player.Revive(stream);
                        client.InST = true;
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
                }

            }
            //else client.CreateBoxDialog($"You can't join a event while you have other player ({MacJoin[client.OnLogin.MacAddress]}) joined to this event.");
        }

        private static Client.GameClient[] GetPlayers()
        {
            return EventMap.Values.Where(p => p.Player.DynamicID == DynamicID && p.InST).ToArray();
        }

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
                //KillSystem = new KillerSystem();

                #region Sign Ups
                AcceptingPlayers = true;
                foreach (var client in Server.GamePoll.Values)
                {
                    client.Player.MessageBox("[SupremeDuelist]Tournament Event will be starting, Do you want to join", new Action<Client.GameClient>(p => { p.Teleport(457, 352, 1002);  /*JoinClient(p);*/ }), null, 60);
                }
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[SupremeDuelist]Tournament Event will be starting in 60 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(30000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("[SupremeDuelist]Tournament Event will be starting in 30 seconds!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(20000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("10 Seconds left before [SupremeDuelist]Tournament starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("5 Seconds left before [SupremeDuelist]Tournament starts!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                Thread.Sleep(5000);
                //Program.DiscordEventsAPI.Enqueue("SupremeDuelist Has Started");
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

                AcceptAddPlayers = true;

                #region Setup Characters
                Client.GameClient[] _chrz = AwaitingPlayers.ToArray();
                foreach (Client.GameClient client in _chrz)
                {
                    
                   
                        //ushort x = 0, y = 0;
                        //EventMap.GetRandCoord(ref x, ref y);
                        if (!Program.FreePkMap.Contains(3040))
                            Program.FreePkMap.Add(3040);
                        if (!Program.NoDropItems.Contains(3040))
                            Program.NoDropItems.Add(3040);
                        var spawnLoc = SoulShackle[Program.Rand.Next(0, SoulShackle.Count)];
                        client.Teleport((ushort)spawnLoc.X, (ushort)spawnLoc.Y, 3040, DynamicID);
                        client.Player.Revive(stream);
                        client.InST = true;
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
                    if (C != null && C.InST)
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber3" });
                Thread.Sleep(1000); //Wait 1 Second
                _chrz = GetPlayers();
                foreach (Client.GameClient C in _chrz)
                    if (C != null && C.InST)
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber2" });
                Thread.Sleep(1000); //Wait 1 Second
                _chrz = GetPlayers();
                foreach (Client.GameClient C in _chrz)
                    if (C != null && C.InST)
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber1" });
                Thread.Sleep(1000); //Wait 1 Second
                _chrz = GetPlayers();
                foreach (Client.GameClient C in _chrz)
                {
                    if (C != null && C.InST)
                    {
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber0" });
                        C.SendSysMesage("Begin!!!", MsgServer.MsgMessage.ChatMode.Center);
                    }
                }
                #endregion

                uint Duration = 300;

                #region Wait For Winner
                while (Duration > 0)
                {
                    _chrz = GetPlayers();
                    List<string> list = new List<string>();
                    uint place = 1;
                    var array = Hits.OrderByDescending(p => p.Value).ToList();
                    foreach (var item in array)
                    {
                        var player = _chrz.Where(p => p.Player.UID == item.Key).FirstOrDefault();
                        if (player != null)
                        {
                            list.Add($"*{place}- {player.Player.Name} : {item.Value}");
                            place++;
                        }
                    }
                    foreach (var client in _chrz)
                    {
                        client.SendSysMesage($"---------[SupremeDuelist]Tournament---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                        for (var i = 0; i < list.Count; i++)
                        {
                            client.SendSysMesage(list[i], MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        }
                        client.SendSysMesage($"-----------", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        client.SendSysMesage($"Players  : {_chrz.Length}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        TimeSpan T = TimeSpan.FromSeconds(Duration);
                        client.SendSysMesage($"Time left {T.ToString(@"mm\:ss")}", ChatMode.ContinueRightCorner);
                    }
                    if (Duration > 0)
                        --Duration;
                    Thread.Sleep(1000);
                }
                #endregion

                AcceptAddPlayers = false;

               
                #region Awards distribution
                _chrz = GetPlayers();
                if (_chrz.Length > 0)
                {
                    List<string> list = new List<string>();
                    uint place = 0;
                    var array = Hits.OrderByDescending(p => p.Value).ToList();
                    foreach (var item in array)
                    {
                        var hero = _chrz.Where(p => p.Player.UID == item.Key).FirstOrDefault();
                        if (hero != null)
                        {
                            place++;
                            switch(place)
                            {
                                case 1:
                                    {
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

                                            //if (hero.Player.Level < 137)
                                            //    hero.GainExpBall(3600, false, Role.Flags.ExperienceEffect.angelwing);
                                            //if (hero.Inventory.HaveSpace(1))
                                            //{
                                            //    hero.Inventory.Add(stream, 730003, 1);//+2
                                            //}
                                            //else
                                            //{
                                            //    hero.Inventory.AddReturnedItem(stream, 730003, 1);
                                            //}
                                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage($"{hero.Player.Name} Won [SupremeDuelist]Tournament Tournament and received a 150,000 Conquer Money and [SurpriseBox] and (1) Tournament Points!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                                            
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
                                                    using (var cmdd = new MySql.Data.MySqlClient.MySqlCommand("Delete from cq_events where type=3", conn))
                                                        cmdd.ExecuteNonQuery();

                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e.ToString());
                                                }
                                                #endregion

                                                cmd.Parameters.AddWithValue("@EntityID", hero.Player.UID);
                                                cmd.Parameters.AddWithValue("@name", hero.Player.Name);
                                                cmd.Parameters.AddWithValue("@type", "3");

                                                cmd.ExecuteNonQuery();

                                            }
                                        }
                                        hero.SendSysMesage("You won in [SupremeDuelist]Tournament check your Inventory.");
                                        break;
                                    }
                                //case 2:
                                //    {
                                //        break;
                                //    }
                            }
                        }
                    }
                }
                #endregion

                #region Unset Event Handlers
                _chrz = GetPlayers();
                foreach (Client.GameClient c in _chrz)
                {
                    c.InST = false;
                    c.Teleport(429, 378, 1002);
                    c.Player.RemoveTempEquipment(stream);
                    c.SendSysMesage("", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                c.Player.SetPkMode(Role.Flags.PKMode.Capture);

                }
                #endregion
                MacJoin.Clear();
                AwaitingPlayers.Clear();
                Hits.Clear();
                Started = false;

            }
        }
        public string ConnectionString = DatabaseConfig.ConnectionString;

        public static List<coords> SoulShackle = new List<coords>() {
            new coords(34,35) { },
            new coords(67,35) { },
            new coords(68,67) { },
            new coords(35,67) { }           
            };
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
        "BETTER LUCK NEXT TIME BUDDIE! GO PLAY REALCO! #06",
        "bring me someone who can help u your dog shit#54 #58"
      };
            int index = new Random().Next(stringList.Count);
            return stringList[index];
        }
        public static void OnTakeAttack(Client.GameClient Attacker, Client.GameClient Attacked, ref uint Damage)
        {
            Damage = 1;
            if (!Hits.ContainsKey(Attacker.Player.UID))
                Hits.Add(Attacker.Player.UID, 1);
            else Hits[Attacker.Player.UID]++;
            var spawnLoc = SoulShackle[Program.Rand.Next(0, SoulShackle.Count)];
            Attacked.Teleport((ushort)spawnLoc.X, (ushort)spawnLoc.Y, 3040, DynamicID, false);            
            Attacked.Player.Protect = Extensions.Time32.Now.AddSeconds(7);
            //foreach (Client.GameClient C in Attacked)
           
            Attacked.Player.AddFlag(MsgServer.MsgUpdate.Flags.Dizzy, 5, false);

            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                string Msg;
                Msg = Translator.GetTranslatedString(GetKillerMessage(), Translator.Language.EN, Attacker.Language);
                Attacker.Player.View.SendView(new MsgMessage(Msg, Attacked.Player.Name, Attacker.Player.Name, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream), true);
                Attacked.Player.View.SendView(new MsgMessage(Msg, Attacked.Player.Name, Attacker.Player.Name, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream), true);
                KillSystem.Update(Attacked);
                KillSystem.CheckDead(Attacked.Player.UID);

            }
            //KillSystem.Update(Attacked);
            //KillSystem.CheckDead(Attacked.Player.UID);
        }

        public static void OnTeleport(Client.GameClient arg1)
        {
            if (Started)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    arg1.InST = false;
                    var chars = GetPlayers();
                    if (chars.Length == 1 && !AcceptAddPlayers)
                    {
                        Started = false;
                    }
                    //arg1.Player.RemoveTempEquipment(stream);
                    if (MacJoin.ContainsKey(arg1.OnLogin.MacAddress))
                        MacJoin.Remove(arg1.OnLogin.MacAddress);
                    //arg1.Player.SetPkMode(Role.Flags.PKMode.Capture);
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
                    obj.InST = false;
                    var chars = GetPlayers();
                    if (chars.Length == 1 && !AcceptAddPlayers)
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
