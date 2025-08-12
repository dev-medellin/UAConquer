using System;
using System.Linq;
using System.Reflection;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using TheChosenProject.Role;
using static TheChosenProject.Role.Flags;

namespace TheChosenProject.Game.MsgEvents
{
    class DragonWar : Events
    {
        DateTime LastScore;
        DateTime EventStartTime;

        public DragonWar()
        {
            IDEvent = 9;
            EventTitle = "Dragon War";
            Duration = 3;
            BaseMap = 700;
            Reflect = false;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            AllowedSkills = new System.Collections.Generic.List<ushort> { (ushort)12350, (ushort)1045, (ushort)1046, (ushort)1047, (ushort)12930 };
            _Duration = 180;
            PotionsAllowed = false;
            IDMessage = MsgStaticMessage.Messages.DragonWar;
        }

        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (GameClient client in PlayerList.Values)
                {
                    ChangePKMode(client, PKMode.PK);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                    client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                    if (!DW)
                    {
                        client.Player.AddFlag(MsgServer.MsgUpdate.Flags.dragonwar, 6666666, true);
                        DW = true;
                    }
                }
                LastScore = DateTime.Now;
                DisplayScores = DateTime.Now;
                EventStartTime = DateTime.Now;

                Console.WriteLine("[TeleportPlayersToMap] Event started.");
            }
        }

        public override void WaitForWinner()
        {
            base.WaitForWinner();

            if (PlayerScores.ContainsValue(300))
            {
                Console.WriteLine("[WaitForWinner] A player reached 300 points. Ending event.");
                Finish();
            }
            else if (DateTime.Now >= DisplayScores.AddMilliseconds(3000))
            {
                DisplayScore();
            }

            // Ensure the event doesn't prematurely end by checking the duration
            if (_Duration <= 0 && DateTime.Now >= EventStartTime.AddSeconds(180))
            {
                Console.WriteLine("[WaitForWinner] Time expired. Ending event.");
                Finish();
            }
        }

        public override void CharacterChecks(GameClient client)
        {
            base.CharacterChecks(client);
            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.dragonwar))
            {
                if (DateTime.Now >= LastScore.AddMilliseconds(1000))
                {
                    LastScore = DateTime.Now;
                    if (PlayerScores.ContainsKey(client.EntityID))
                    {
                        if (PlayerScores[client.EntityID] + 3 > 300)
                            PlayerScores[client.EntityID] = 300;
                        else
                            PlayerScores[client.EntityID] += 3;

                        Console.WriteLine($"[CharacterChecks] Player {client.EntityID} Score: {PlayerScores[client.EntityID]}");
                    }
                    else
                    {
                        PlayerScores[client.EntityID] = 3;
                        Console.WriteLine($"[CharacterChecks] New Player {client.EntityID} joined with 3 points.");
                    }
                }
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;

            Console.WriteLine($"[DisplayScore] Time left: {_Duration} seconds");
            foreach (var kvp in PlayerScores.OrderByDescending(s => s.Value))
            {
                Console.WriteLine($"[DisplayScore] Player {kvp.Key}: {kvp.Value} points");
            }

            if (_Duration > 0)
                --_Duration;
            else
                Console.WriteLine("[DisplayScore] Timer reached zero, but event hasn't ended yet.");
        }

        public override void End()
        {
            foreach (GameClient client in PlayerList.Values.ToList())
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.dragonwar);

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
            Console.WriteLine("[End] Event has ended.");
            return;
        }

        public override void Reward(GameClient client)
        {
            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.dragonwar);
            Console.WriteLine($"[Reward] Player {client.EntityID} rewarded.");
            base.Reward(client);
        }
    }
}
