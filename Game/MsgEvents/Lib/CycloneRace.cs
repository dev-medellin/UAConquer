using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using static TheChosenProject.Role.Flags;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgEvents
{
    public class CycloneRace : Events
    {
        public Role.SobNpc npc;
        public uint Rank = 0;

        public CycloneRace()
        {
            IDEvent = 1;
            EventTitle = "TempestRush";
            IDMessage = MsgStaticMessage.Messages.CycloneRace;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = true;
            ReviveAllowed = true;
            BaseMap = 2062;
            FlyAllowed = false;
            AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
            Duration = 180;
            isTerr = false;
            PotionsAllowed = false;
        }

        public override void TeleportPlayersToMap()
        {
            if (Map != null)
            {
                if (npc == null)
                {
                    npc = new Role.SobNpc();
                    npc.ObjType = Role.MapObjectType.SobNpc;
                    npc.UID = 465221;
                    npc.Name = "Exit";
                    npc.Type = NpcType.Talker;
                    npc.Mesh = (SobNpc.StaticMesh)35920;
                    npc.Map = 2062;
                    npc.AddFlag(Game.MsgServer.MsgUpdate.Flags.Praying, StatusFlagsBigVector32.PermanentFlag, false);
                    npc.X = 995;
                    npc.Y = 734;
                    npc.HitPoints = 0;
                    npc.MaxHitPoints = 0;
                    npc.Sort = 1;
                    Map.View.EnterMap<Role.IMapObj>(npc);
                }
            }
            foreach (GameClient client in PlayerList.Values.ToArray())
            {
                ChangePKMode(client, PKMode.PK);
                ushort x = 125;
                ushort y = 323;
                Map.GetRandCoord(ref x, ref y, 7);
                client.Teleport(x, y, 2062, DinamicID);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Ride);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 6, true);
                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
            }
        }

        public override void CharacterChecks(GameClient C)
        {
            DisplayScore();
            base.CharacterChecks(C);
            if (!C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                C.Player.AddFlag(MsgServer.MsgUpdate.Flags.Cyclone, StatusFlagsBigVector32.PermanentFlag, true);
            if (C.Player.TransformInfo != null && C.Player.TransformationID != 0)
                C.Player.TransformInfo.FinishTransform();
            if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                C.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Ride);
            if (!C.Player.Alive)
            {
                if (C.Player.DeadStamp.AddSeconds(3) < Extensions.Time32.Now)
                {
                    C.EventBase?.RemovePlayer(C);
                }
            }
        }

        public override void End()
        {
            DisplayScore();
            foreach (var player in PlayerScores.OrderByDescending(s => s.Value).ToArray())
            {
                Reward(PlayerList[player.Key]);
                RemovePlayer(PlayerList[player.Key]);
            }
            PlayerList.Clear();
            PlayerScores.Clear();
            Program.Events.Remove(this);
            return;
        }

        public override void Reward(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Rank++;
                switch (Rank)
                {
                    case 1:
                        {
                        int value = 150000;
                        client.Player.Money += value;
                        client.Inventory.Add(stream, 722178, 1);
                        client.CreateBoxDialog($"You've received {value} ConquerMoney for Rank {Rank} in {EventTitle} Tournament!.");
                        string msg = $"{client.Player.Name} has won {value} ConquerMoney and [SurpriseBox] from Rank {Rank} in the hourly {EventTitle} Tournament!";
                        Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, MsgColor.red, ChatMode.System).GetArray(stream));
                        string reward = "[EVENT]" + msg;
                        Database.ServerDatabase.LoginQueue.Enqueue(reward);
                         break;
                        }
                    default:
                        {
                            if (client.Player.Level < 137)
                            {                                
                                uint value = 150000 - (Rank * 30000);
                                if (value < 5)
                                    value = 5;
                                client.Player.Money += (int)value;
                                client.CreateBoxDialog($"You've received {value} ConquerMoney for Rank {Rank} in {EventTitle} Tournament!.");
                                string msg = $"{client.Player.Name} has won {value} ConquerMoney from Rank {Rank} in the hourly {EventTitle} Tournament!";
                                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, MsgColor.red, ChatMode.System).GetArray(stream));
                                string reward = "[EVENT]" + msg;
                                Database.ServerDatabase.LoginQueue.Enqueue(reward);
                            }
                            
                            break;
                        }
                }
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values.ToArray())
            {
                player.SendSysMesage($"---------{EventTitle}---------", ChatMode.FirstRightCorner);
                TimeSpan T = TimeSpan.FromSeconds(Duration);
                player.SendSysMesage($"Time left {T.ToString(@"mm\:ss")}", ChatMode.ContinueRightCorner);
                if (Duration > 0)
                    --Duration;
            }
            Broadcast($"Players left: {PlayerList.Count}", BroadCastLoc.Score);
        }
    }
}