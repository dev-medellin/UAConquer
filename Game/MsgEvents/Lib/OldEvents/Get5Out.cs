using TheChosenProject.Client;
using TheChosenProject.Role;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheChosenProject.Game.MsgEvents
{
    class Get5Out : Base
    {
        public Get5Out()
        {
            EventTitle = "Five(n)Out";
            Duration = 35;
            BaseMap = 700;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = true;
            AllowedSkills = new List<ushort> { (ushort)12350, (ushort)1045, (ushort)1046, (ushort)1047, (ushort)12930 };
            Duration = 180;
            PotionsAllowed = false;
        }


        public override void End()
        {
            //DisplayScore();
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
            return;
        }

        public override void Reward(GameClient client)
        {
            base.Reward(client);
        }
        public void DisplayScore(GameClient client)
        {
            foreach (var player in PlayerList.Values)
            {
                if (player == client)
                    //Broadcast($"You have [{PlayerScores[player.Player.UID]}] Hits Left.", BroadCastLoc.Title);
                    player.SendSysMesage($"You have [{PlayerScores[player.Player.UID]}] Hits Left.", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            //Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }
        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values)
            {
                player.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
            }
            Broadcast($"Players left: {PlayerList.Count}", BroadCastLoc.Score, 2);
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            //Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }
        public override void Hit(GameClient Attacker, GameClient Victim)
        {
            //if (PlayerScores.ContainsKey(Victim.Player.UID) && PlayerScores[Victim.Player.UID] > 1)
            //    PlayerScores[Victim.Player.UID]--;
            //else
            //    RemovePlayer(Victim);
            //DisplayScore(Attacker);
            //DisplayScore(Victim);
            if (PlayerScores.ContainsKey(Victim.EntityID))
            {
                if (PlayerScores[Victim.EntityID] < 3)
                {
                    PlayerScores[Victim.EntityID]++;
                    Victim.SendSysMesage($"You can only be hitted " + (5 - PlayerScores[Victim.EntityID]) + " more times!", MsgServer.MsgMessage.ChatMode.System);

                }
                else if (PlayerScores[Victim.EntityID] < 5)
                {
                    PlayerScores[Victim.EntityID]++;
                    Victim.SendSysMesage($"You'll be kicked if anyone hits you again! Watch out!", MsgServer.MsgMessage.ChatMode.System);

                }
                else if (PlayerScores[Victim.EntityID] >= 5)
                    RemovePlayer(Victim);
            }
        }
    }
}
