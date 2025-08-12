using TheChosenProject.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using static TheChosenProject.Role.Flags;
using TheChosenProject.Game.MsgServer;

namespace TheChosenProject.Game.MsgEvents
{
    class WhackTheThief : Events
    {
        public WhackTheThief()
        {
            IDEvent = 11;
            IDMessage = MsgStaticMessage.Messages.WhackTheThief;
            EventTitle = "ShadowHeist";
            Duration = 10;
            BaseMap = 11266;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = true;
            ReviveAllowed = true;
            AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
            Duration = 180;
            isTerr = false;
            PotionsAllowed = false;
        }

        public override void TeleportPlayersToMap()
        {
            foreach (GameClient client in PlayerList.Values)
            {
                ChangePKMode(client, PKMode.PK);
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DinamicID, true, true);
                client.SendSysMesage(" Warning: Beware of trap portals — one wrong step and you’ll be out of the game!", ChatMode.BroadcastMessage);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.FireAura))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.FireAura);
                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                if (!isTerr)
                {
                    client.Player.AddFlag(MsgServer.MsgUpdate.Flags.FireAura, 6666666, true);
                    isTerr = true;
                }
            }
        }

        public override void CharacterChecks(GameClient C)
        {
            base.CharacterChecks(C);
            if (DateTime.Now >= LastScores.AddSeconds(1))
            {
                if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.FireAura))
                    PlayerScores[C.EntityID] += 1;
            }

        }

        public override void WaitForWinner()
        {
            if (Duration <= 0 || PlayerList.Count == 0)
            {
                Broadcast("There is no winner, as all participants choose to leave the event.", BroadCastLoc.World);
                Finish();
            }
            if(Duration <= 0 || PlayerList.Count == 1)
            {
                Finish();
            }
            if (DateTime.Now >= LastScores.AddMilliseconds(3000))
                DisplayScore();
        }

        public override void RemovePlayer(GameClient client, bool Diss = false, bool CanTeleport = true)
        {
            base.RemovePlayer(client);
            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.FireAura))
                isTerr = false;
            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.FireAura);
        }

        public override void End()
        {
            DisplayScore();
            int NO = 1;
            foreach (var player in PlayerScores.OrderByDescending(s => s.Value).ToList())
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

        public override uint GetDamage(GameClient User, GameClient C)
        {
            //if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.DragonSwing))
            //    return C.Status.MaxHitpoints;
            //else
                return 1;
        }

        public override void Kill(GameClient Attacker, GameClient Victim)
        {
            if (Victim.Player.ContainFlag(MsgServer.MsgUpdate.Flags.FireAura))
            {
                Victim.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.FireAura);
                PlayerScores[Attacker.EntityID] += 1;
                Attacker.Player.AddFlag(MsgServer.MsgUpdate.Flags.FireAura, 6666666, true);
            }
        }

        public override void Hit(GameClient Attacker, GameClient Victim)
        {
            if (Victim.Player.ContainFlag(MsgServer.MsgUpdate.Flags.FireAura))
            {
                Victim.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.FireAura);
                //PlayerScores[Attacker.EntityID] += 3;
                Attacker.Player.AddFlag(MsgServer.MsgUpdate.Flags.FireAura, 6666666, true);
                Victim.Player.Dead(Victim.Player, Victim.Player.X, Victim.Player.Y, Victim.EntityID);
            }
            if (Attacker.Player.ContainFlag(MsgServer.MsgUpdate.Flags.FireAura))
            {
                PlayerScores[Attacker.EntityID] += 1;
                //Victim.Player.Dead(Victim.Player, Victim.Player.X, Victim.Player.Y, Victim.EntityID);
                //Attacker.Player.ConquerPoints += 100;

            }
            if (!isTerr)
            {
                Attacker.Player.AddFlag(MsgServer.MsgUpdate.Flags.FireAura, 6666666, true);
                isTerr = true;
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values)
            {
                player.SendSysMesage($"---------{EventTitle}---------", ChatMode.FirstRightCorner);
                
            }
            byte Score = 2;
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)))
            {
                if (Score == 5)
                    break;
                if (Score == PlayerScores.Count + 2)
                    break;
                Broadcast($"Nº {Score - 1}: {PlayerList[kvp.Key].Player.Name} - {kvp.Value}", BroadCastLoc.Score, Score);
                Score++;
            }
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)))
            {
                if (PlayerList[kvp.Key].Player.ContainFlag(MsgServer.MsgUpdate.Flags.FireAura))
                    Broadcast($"TargetThief : {PlayerList[kvp.Key].Player.Name}", BroadCastLoc.Score, Score);
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }

        public override void Reward(GameClient client)
        {
            base.Reward(client);
        }
    }
}
