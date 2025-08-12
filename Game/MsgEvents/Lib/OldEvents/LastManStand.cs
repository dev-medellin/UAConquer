using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Game.MsgServer.MsgMessage;
using static TheChosenProject.Role.Flags;

namespace TheChosenProject.Game.MsgEvents
{
    public class LastManStand : Base
    {
        public LastManStand()
        {
            IDEvent = 1;
            EventTitle = "LastManStand";
            IDMessage = MsgStaticMessage.Messages.LastMan;
            NoDamage = false;
            MagicAllowed = true;
            MeleeAllowed = true;
            FriendlyFire = true;
            FlyAllowed = false;
            BaseMap = 10143;
            Duration = 180;
            RewardCps = 400 * 2;
        }

        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
            foreach (GameClient client in PlayerList.Values.ToArray())
            {
                client.Player.LastMan = 0;
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
            //RemovePlayer(Victim);
        }

        public override void End()
        {
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
            client.Player.LastMan += 1;
            base.Reward(client);
        }

        public override void Inviting()
        {
            base.Inviting();
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