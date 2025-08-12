using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Role.Flags;

namespace TheChosenProject.Game.MsgEvents
{
    public class DragonWar : Base
    {
        private DateTime LastScore;

        public DragonWar()
        {
            IDEvent = 4;
            EventTitle = "DragonWar";
            IDMessage = MsgStaticMessage.Messages.DragonWar;
            BaseMap = 700;
            Reflect = false;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            AllowedSkills = new System.Collections.Generic.List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
            Duration = 180;
            PotionsAllowed = false;
        }

        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (GameClient client in PlayerList.Values.ToArray())
                {
                    client.Player.FairbattlePower = TheChosenProject.Role.Flags.FairbattlePower.UpdateToSerf;

                    client.Player.DragonWar = 0;
                    if (!DW)
                    {
                        client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Top4Weekly, 6666666, true);
                        DW = true;
                    }
                }
                LastScore = DateTime.Now;
                DisplayScores = DateTime.Now;
            }
        }

        //public override void WaitForWinner()
        //{
        //    base.WaitForWinner();
        //    if (PlayerScores.ContainsValue(300))
        //        Finish();
        //    else
        //    {
        //        bool victim = false;
        //        foreach (var v in PlayerList.Values.ToArray())
        //        {
        //            if (v.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Top4Weekly))
        //            {
        //                victim = true;
        //            }
        //        }
        //        if (!victim)
        //        {
        //            var vic = PlayerScores.OrderByDescending((s => s.Value)).ToArray().FirstOrDefault();
        //            PlayerList[vic.Key].Player.AddFlag(MsgServer.MsgUpdate.Flags.Top4Weekly, StatusFlagsBigVector32.PermanentFlag, true);
        //            DW = true;
        //        }
        //        if (DateTime.Now >= DisplayScores.AddMilliseconds(1000))
        //            DisplayScore();
        //    }
        //}
        public override void WaitForWinner()
        {
            base.WaitForWinner();
            if (PlayerScores.ContainsValue(300))
                Finish();
            else if (DateTime.Now >= DisplayScores.AddMilliseconds(3000))
                DisplayScore();
        }
        public override void CharacterChecks(GameClient client)
        {
            base.CharacterChecks(client);
            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Top4Weekly))
            {
                if (DateTime.Now >= LastScore.AddMilliseconds(1000))
                {
                    LastScore = DateTime.Now;
                    if (PlayerScores[client.EntityID] + 3 > 300)
                        PlayerScores[client.EntityID] = 300;
                    else
                        PlayerScores[client.EntityID] += 3;
                }
            }
        }

        public override void RemovePlayer(GameClient client, bool diss = false, bool CanTeleport = false)
        {
            base.RemovePlayer(client);
            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Top4Weekly))
            {
                DW = false;
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Top4Weekly);
            }
        }

        public override void End()
        {
            foreach (GameClient client in PlayerList.Values.ToArray())
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Top4Weekly);

            DisplayScore();
            byte NO = 1;
            foreach (var player in PlayerScores.OrderByDescending(s => s.Value).ToArray())
            {
                if (NO == 1)
                {
                    Reward(PlayerList[player.Key]);
                    RemovePlayer(PlayerList[player.Key]);
                    NO++;
                }
                else
                {
                    if (PlayerList.ContainsKey(player.Key))
                    {
                        RemovePlayer(PlayerList[player.Key]);
                        NO++;
                    }
                }
            }

            PlayerList.Clear();
            PlayerScores.Clear();
            Program.Events.Remove(this);
            return;
        }

        public override void Reward(GameClient client)
        {
            //client.Player.DragonWar += 1;
            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Top4Weekly);
            base.Reward(client);
        }

        public override uint GetDamage(GameClient User, GameClient C)
        {
            if (User.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Top4Weekly))
                return Convert.ToUInt32(C.Status.MaxHitpoints);
            else if (!DW)
                return Convert.ToUInt32(C.Status.MaxHitpoints * 0.4);
            else if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Top4Weekly))
                return Convert.ToUInt32(C.Status.MaxHitpoints * 0.4);
            return 1;
        }

        public override void Hit(GameClient Attacker, GameClient Victim)
        {
            if (Victim.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Top4Weekly))
            {
                if (Victim.Player.HitPoints < Victim.Status.MaxHitpoints * 0.4)
                {
                    Victim.Player.RemoveFlag(MsgUpdate.Flags.Top4Weekly);
                    Attacker.Player.AddFlag(MsgUpdate.Flags.Top4Weekly, 666666, true);
                    Attacker.Player.Stamina = 100;
                    if (PlayerScores[Attacker.EntityID] + 5 > 300)
                        PlayerScores[Attacker.EntityID] = 300;
                    else
                        PlayerScores[Attacker.EntityID] += 5;
                }
                else if (PlayerScores[Attacker.EntityID] + 5 > 300)
                    PlayerScores[Attacker.EntityID] = 300;
                else
                    PlayerScores[Attacker.EntityID] += 5;
            }
        }
        public override void Kill(GameClient Attacker, GameClient Victim)
        {
            if (Victim.Player.ContainFlag(MsgUpdate.Flags.Top4Weekly))
            {
                Victim.Player.RemoveFlag(MsgUpdate.Flags.Top4Weekly);
                Attacker.Player.AddFlag(MsgUpdate.Flags.Top4Weekly, 666666, true);
                Attacker.Player.Stamina = 100;
                if (PlayerScores[Attacker.EntityID] + 5 > 300)
                    PlayerScores[Attacker.EntityID] = 300;
                else
                    PlayerScores[Attacker.EntityID] += 5;
            }
            else if (!DW)
            {
                Attacker.Player.AddFlag(MsgUpdate.Flags.Top4Weekly, 666666, true);
                PlayerScores[Attacker.EntityID]++;
                DW = true;
            }
            else if (Attacker.Player.ContainFlag(MsgUpdate.Flags.Top4Weekly))
            {
                if (PlayerScores[Attacker.EntityID] + 5 > 300)
                    PlayerScores[Attacker.EntityID] = 300;
                else
                    PlayerScores[Attacker.EntityID] += 5;
            }
            else
            {
                if (PlayerScores[Attacker.EntityID]++ > 300)
                    PlayerScores[Attacker.EntityID] = 300;
                else
                    PlayerScores[Attacker.EntityID]++;
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values)
            {
                player.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
            }
            byte Score = 2;
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)))
            {
                if (Score == 7)
                    break;
                if (Score == PlayerScores.Count + 2)
                    break;
                Broadcast($"Nº {Score - 1}: {PlayerList[kvp.Key].Name} - {kvp.Value}", BroadCastLoc.Score, Score);
                Score++;
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            Broadcast("----------------------------------", BroadCastLoc.Score);
            Broadcast("Who gain [300] Points first will win", BroadCastLoc.Score);
            Broadcast("----------------------------------", BroadCastLoc.Score);
            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }
    }
}