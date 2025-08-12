using TheChosenProject.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Role.Flags;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;

namespace TheChosenProject.Game.MsgEvents
{
    class LastManStand : Events
    {
        public LastManStand()
        {
            IDEvent = 8;
            IDMessage = MsgStaticMessage.Messages.LastMan;
            EventTitle = "Last Man Stand";
            Duration = 50;
            NoDamage = false;
            MagicAllowed = true;
            MeleeAllowed = true;
            FriendlyFire = true;
            BaseMap = 1767;
            _Duration = 180;
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
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                KillSystem = new KillerSystem();

            }
        }

        public override void CharacterChecks(GameClient C)
        {
            base.CharacterChecks(C);
            if (!C.Player.Alive)
            {
                if (C.Player.DeadStamp.AddSeconds(3) < Extensions.Time32.Now)
                {
                    C.EventBase?.RemovePlayer(C);
                }
            }
        }

        public override void Kill(GameClient Attacker, GameClient Victim)
        {
            RemovePlayer(Victim);
        }

        public override void End()
        {
            DisplayScore();
            byte NO = 1;
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

        public override void Reward(GameClient client)
        {


            base.Reward(client);
        }
        public override void Inviting()
        {
            base.Inviting();


        }
        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values)
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