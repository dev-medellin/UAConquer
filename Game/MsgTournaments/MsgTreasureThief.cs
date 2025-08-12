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
    public class MsgTreasureThief : ITournament
    {
        public static ushort MapID = 1126;

        public int CurrentBoxes;

        public DateTime StartTimer;

        public DateTime BoxesStamp;

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

        public MsgTreasureThief(TournamentType _type)
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
            foreach (GameClient user in values)
            {
                user.Teleport(430, 388, 1002);
            }
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
            StartTimer = DateTime.Now.AddMinutes(10.0);
            BoxesStamp = DateTime.Now.AddSeconds(30.0);
            CurrentBoxes = 0; // Reset the current boxes count
            GenerateBoxes(); // Ensure boxes are generated at the start of the event
            MsgSchedules.SendInvitation("TreasureThief", "ConquerPoints, Money, and other treasures", 352, 358, 1002, 0, 60);
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

        private void Create()
        {
            GenerateBoxes();
        }

        private void GenerateBoxes()
        {
            while (CurrentBoxes < 30) // Ensure we keep generating until we have 30 boxes
            {
                byte rand = (byte)ServerKernel.NextAsync(0, 5);
                ushort x = 0, y = 0;
                Map.GetRandCoord(ref x, ref y);
                Npc np = Npc.Create();

                do
                {
                    np.UID = (uint)ServerKernel.NextAsync(10000, 100000);
                }
                while (Map.View.Contain(np.UID, x, y));

                np.NpcType = Flags.NpcType.Talker;
                switch (rand)
                {
                    case 0:
                        np.Mesh = 26586;
                        break;
                    case 1:
                        np.Mesh = 26596;
                        break;
                    case 2:
                        np.Mesh = 26606;
                        break;
                    case 3:
                        np.Mesh = 26616;
                        break;
                    case 4:
                        np.Mesh = 26626;
                        break;
                    default:
                        np.Mesh = 26586;
                        break;
                }

                np.Map = MapID;
                np.X = x;
                np.Y = y;
                Map.AddNpc(np);
                CurrentBoxes++; // Increment the count of current boxes
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
                    ServerKernel.Log.SaveLog("Could not finish Treasure Thief", true, LogType.WARNING);
                }

                foreach (IMapObj target in Map.View.GetAllMapRoles(MapObjectType.Npc))
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
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

            return;
            while (true)
            {
                switch ((byte)ServerKernel.NextAsync(0, 5))
                {
                    case 0:
                        {
                            int value;
                            value = (int)ServerKernel.NextAsync(ServerKernel.TREASURE_THIEF_MIN * 100, ServerKernel.TREASURE_THIEF_MAX * 100);
                            user.Player.Money -= value;
                            user.Player.SendUpdate(stream, user.Player.Money, MsgUpdate.DataType.Money);
                            IEventRewards.Add(Type.ToString() + " [Money]", 0, (uint)value, "", "[" + user.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                            user.CreateBoxDialog("You've received " + value + " Money.");
                            user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession2");
                            MsgSchedules.SendSysMesage(user.Player.Name + " got " + value + " Money while opening the TreasureBox!", MsgMessage.ChatMode.Talk);
                            break;
                        }
                    case 1:
                        goto IL_0124;
                    case 2:
                        {
                            user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession3");
                            int value2;
                            value2 = ServerKernel.NextAsync(ServerKernel.TREASURE_THIEF_MIN, ServerKernel.TREASURE_THIEF_MAX);                            
                            {
                                user.Player.ConquerPoints -= value2;
                                IEventRewards.Add(Type.ToString() + " [CPS]", (uint)value2, 0, "", "[" + user.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                                MsgSchedules.SendSysMesage(user.Player.Name + " got " + value2 + " CPs while opening the TreasureBox!", MsgMessage.ChatMode.Talk);
                                user.CreateBoxDialog("You've received " + value2 + " ConquerPoints.");
                            }
                            break;
                        }
                    case 3:
                        user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "zf2-e215");
                        user.Player.Dead(null, user.Player.X, user.Player.Y, 0);
                        MsgSchedules.SendSysMesage(user.Player.Name + " found DEATH! while opening the TreasureBox!", MsgMessage.ChatMode.Talk);
                        break;
                    case 4:
                    case 5:
                        {
                            Rifinery.Item[] array;
                            array = ItemType.Refinary[1u].Values.ToArray();
                            int position;
                            position = ServerKernel.NextAsync(0, array.Length);
                            uint item1;
                            item1 = array[position].ItemID;
                            Rifinery.Item[] array2;
                            array2 = ItemType.Refinary[2u].Values.ToArray();
                            int position2;
                            position2 = ServerKernel.NextAsync(0, array.Length);
                            uint item2;
                            item2 = array[position].ItemID;
                            Rifinery.Item[] array3;
                            array3 = ItemType.Refinary[3u].Values.ToArray();
                            int position3;
                            position3 = ServerKernel.NextAsync(0, array.Length);
                            uint item3;
                            item3 = array[position].ItemID;
                            uint[] obj;
                            obj = new uint[10] { 1088000, 1088001, 722057, 722136, 723712, 730001, 730002, 0, 0, 0u };
                            obj[7] = item1;
                            obj[8] = item2;
                            obj[9] = item3;
                            uint[] Items;
                            Items = obj;
                            uint ItemID;
                            ItemID = Items[ServerKernel.NextAsync(0, Items.Length)];
                            if (Server.ItemsBase.TryGetValue(ItemID, out var DBItem))
                            {
                                if (user.Inventory.HaveSpace(1))
                                    user.Inventory.Add(stream, DBItem.ID, 1, 0, 0, 0);
                                else
                                    user.Inventory.AddReturnedItem(stream, DBItem.ID, 1, 0, 0, 0, Flags.Gem.NoSocket, Flags.Gem.NoSocket, false, Flags.ItemEffect.None, 0);
                                IEventRewards.Add(Type.ToString() + " [Item]", 0, 0, DBItem.Name, "[" + user.Player.Name + "]: " + DateTime.Now.ToString("d/M/yyyy (H:mm)"));
                                MsgSchedules.SendSysMesage(user.Player.Name + " got " + DBItem.Name + " while opening the TreasureBox!", MsgMessage.ChatMode.Talk);
                                user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession");
                            }
                            break;
                        }
                }
                break;
            IL_0124:
                if (user.Player.Level != ServerKernel.MAX_UPLEVEL)
                {
                    user.GainExpBall(1200.0, true, Flags.ExperienceEffect.angelwing);
                    MsgSchedules.SendSysMesage(user.Player.Name + " got 2xExpBalls while opening the TreasureBox!", MsgMessage.ChatMode.Talk);
                    user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "accession1");
                    break;
                }
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
