using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgNpc;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgFindTheBox : ITournament
    {
        public static ushort MapID = 1126;
    
        public int CurrentBoxes;

        public DateTime StartTimer;

        public DateTime BoxesStamp;
        public uint DinamicID = 0;

        private GameMap _map;

        public ProcesType Process { get; set; }

        public GameMap Map
        {
            get
            {
                if (_map == null)
                    _map = Server.ServerMaps[MapID];
                return _map;
            }
        }

        public TournamentType Type { get; set; }

        public MsgFindTheBox(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public bool InTournament(GameClient user)
        {
            return user.Player.Map == MapID;
        }

        public void Close()
        {
            GameClient[] values;
            values = Map.Values;
            
            Process = ProcesType.Dead;
            try
            {
                ITournamentsAlive.Tournments.Remove(7);
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not finish Treasure Thief", true, LogType.WARNING);
            }
        }

        public void Open()
        {
            if (Process == ProcesType.Alive)
                return;
            Create();
            foreach (GameClient user in Server.GamePoll.Values)
            {
                user.Player.CurrentTreasureBoxes = 0;
            }
            Process = ProcesType.Alive;
            StartTimer = DateTime.Now.AddMinutes(5.0); // Shorten the duration to 5 minutes
            BoxesStamp = DateTime.Now.AddSeconds(30.0);
            CurrentBoxes = 0; // Reset the current boxes count
            GenerateBoxes(); // Ensure boxes are generated at the start of the event
            MsgSchedules.SendInvitation("TreasureThief", "ConquerPoints, Money, and other treasures", 353, 358, 1002, 0, 60);
            //Program.DiscordEventsAPI.Enqueue("TreasureThief Has Started");
            Program.FreePkMap.Add(MapID);
            try
            {
                ITournamentsAlive.Tournments.Add(7, ": started at(" + DateTime.Now.ToString("H:mm)"));
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not start Treasure Thief", true, LogType.WARNING);
            }
        }



        public bool Join(GameClient user, Packet stream)
        {
            if (Process == ProcesType.Alive)
            {
                ushort x;
                x = 0;
                ushort y;
                y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, MapID);
                return true;
            }
            return false;
        }
        public void Revive(Extensions.Time32 Timer, Client.GameClient user)
        {
            if (user.Player.Alive == false && Process != ProcesType.Dead)
            {
                if (InTournament(user))
                {
                    if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                    {
                        ushort x = 0;
                        ushort y = 0;
                        Map.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, Map.ID, DinamicID);
                    }
                }
            }
        }
        private void Create()
        {
            GenerateBoxes();
        }

        private void GenerateBoxes()
        {
            while (CurrentBoxes < 30) // Ensure we keep generating until we have 30 boxes
            {
                for (int i = 0; i < 5; i++) // Generate 5 boxes at a time
                {
                    byte rand = (byte)Program.GetRandom.Next(0, 5);
                    ushort x = 0, y = 0;
                    Map.GetRandCoord(ref x, ref y);

                    Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();
                    while (true)
                    {
                        np.UID = (uint)Program.GetRandom.Next(10000, 100000);
                        if (!Map.View.Contain(np.UID, x, y))
                            break;
                    }
                    np.NpcType = Role.Flags.NpcType.Talker;
                    np.Mesh = 8206; // Set the same mesh for simplicity

                    np.Map = MapID;
                    np.X = x;
                    np.Y = y;
                    Map.AddNpc(np);
                    CurrentBoxes++; // Increment the count of current boxes
                }
            }
        }


        public void CheckUp()
        {
            if (Process != ProcesType.Alive)
                return;

            if (DateTime.Now > StartTimer)
            {
                try
                {
                    ITournamentsAlive.Tournments.Remove(7);
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not finish Find the box", true, LogType.WARNING);
                }

                foreach (IMapObj target in Map.View.GetAllMapRoles(MapObjectType.Npc))
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream = rec.GetStream();
                        Npc npc = target as Npc;
                        Map.RemoveNpc(npc, stream);
                    }
                }

                Process = ProcesType.Dead;
            }
            else if (DateTime.Now > BoxesStamp)
            {
                GenerateBoxes(); // Ensure continuous generation of boxes
                BoxesStamp = DateTime.Now.AddSeconds(30.0);
            }
        }


        public GameClient[] MapPlayers()
        {
            return Map.Values.Where((GameClient p) => p.Player.Map == Map.ID).ToArray();
        }

        public void Reward(GameClient user, Npc npc, Packet stream)
        {
            CurrentBoxes--;
            while (true)
            {
                
                    switch ((byte)ServerKernel.NextAsync(0, 6))
                    {

                    case 0:
                    case 1:
                        {
                            int value = (int)Program.GetRandom.Next(100, 500);
                            user.Player.Money += value * 1000;
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + user.Player.Name + " won " + value + " ConquerPoints while opening the Find The Box [Event]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Talk).GetArray(stream));
                            break;
                        }
                    case 2:
                            {
                                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Bad Luck! " + user.Player.Name + " freezes from Find The Box [Event]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Talk).GetArray(stream));
                            break;
                            }
                    case 3:
                        {
                            uint[] Items = new uint[]
                            {
                                Database.ItemType.DragonBall
                            };
                            uint ItemID = Items[Program.GetRandom.Next(0, Items.Length)];
                            Database.ItemType.DBItem DBItem;
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                            {

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    DataItem.ITEM_ID = DBItem.ID;
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, npc.X, npc.Y, MsgFloorItem.MsgItem.ItemType.Item, 0, user.Player.DynamicID, user.Player.Map, user.Player.UID, false, user.Map);
                                    if (user.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + user.Player.Name + " drop " + DBItem.Name + " while opening the Find The Box [Event]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Talk).GetArray(stream));
                                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Hell YEAH " + user.Player.Name + " he/she drop " + DBItem.Name + " while opening the Find The Box [Event]!", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                                }
                            }
                            break;
                        }
                    case 4:
                        //case 5:
                            {
                                uint[] Items = new uint[]
                                {
                                Database.ItemType.Meteor,
                                Database.ItemType.Bomb};
                                uint ItemID = Items[Program.GetRandom.Next(0, Items.Length)];
                                Database.ItemType.DBItem DBItem;
                                MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        DataItem.ITEM_ID = DBItem.ID;
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, npc.X, npc.Y, MsgFloorItem.MsgItem.ItemType.Item, 0, user.Player.DynamicID, user.Player.Map, user.Player.UID, false, user.Map);
                                        if (user.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + user.Player.Name + " won " + DBItem.Name + " in BoxReward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Talk).GetArray(stream));

                                    }
                                }
                                break;
                            }

                    case 5:
                        {
                            user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 5, true);
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Bad Luck! " + user.Player.Name + " freezes from Find The Box [Event]!", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Talk).GetArray(stream));
                            break;
                        }
                    case 6:
                        //case 5:
                        {
                            uint[] Items = new uint[]
                            {
                                Database.ItemType.Meteor,
                                Database.ItemType.Bomb,
                            };
                            uint ItemID = Items[Program.GetRandom.Next(0, Items.Length)];
                            Database.ItemType.DBItem DBItem;
                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                            if (Database.Server.ItemsBase.TryGetValue(ItemID, out DBItem))
                            {

                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    DataItem.ITEM_ID = DBItem.ID;
                                    MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, npc.X, npc.Y, MsgFloorItem.MsgItem.ItemType.Item, 0, user.Player.DynamicID, user.Player.Map, user.Player.UID, false, user.Map);
                                    if (user.Map.EnqueueItem(DropItem))
                                    {
                                        DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                    }
                                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + user.Player.Name + " won " + DBItem.Name + " in BoxReward.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.Talk).GetArray(stream));

                                }
                            }
                            break;
                        }
                }
                    break;
                //}
                //else
                //{
                //    user.CreateBoxDialog("The distance is too big between you and me.");
                //}

            }
            user.Player.CurrentTreasureBoxes++;
            Map.RemoveNpc(npc, stream);
            ShuffleGuildScores(stream);
        }

        public void ShuffleGuildScores(Packet stream)
        {
            GameClient[] values;
            values = Map.Values;
            foreach (GameClient user in values)
            {
                MsgMessage msg;
                msg = new MsgMessage("---Your Score: " + user.Player.CurrentTreasureBoxes + "---", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner);
                user.Send(msg.GetArray(stream));
            }
            GameClient[] array;
            array = Map.Values.OrderByDescending((GameClient p) => p.Player.CurrentTreasureBoxes).ToArray();
            for (int x = 0; x < Math.Min(10, Map.Values.Length); x++)
            {
                GameClient element;
                element = array[x];
                MsgMessage msg2;
                msg2 = new MsgMessage("No " + (x + 1) + "- " + element.Player.Name + " Opened " + element.Player.CurrentTreasureBoxes + " Boxes!", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner);
                Send(msg2.GetArray(stream));
            }
        }

        public void Send(Packet stream)
        {
            GameClient[] values;
            values = Map.Values;
            foreach (GameClient user in values)
            {
                user.Send(stream);
            }
        }
    }
}
