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

namespace TheChosenProject.Game.MsgEvents
{
    public class DizzyWar : Base
    {
        public DizzyWar()
        {
            IDEvent = 3;
            EventTitle = "DizzyWar";
            IDMessage = MsgStaticMessage.Messages.DizzyFight;
            NoDamage = false;
            MagicAllowed = true;
            MeleeAllowed = true;
            FriendlyFire = true;
            FlyAllowed = false;
            BaseMap = 700;
            Duration = 180;
        }

        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
            foreach (var player in PlayerList.Values.ToArray())
            {
                player.Player.FairbattlePower = TheChosenProject.Role.Flags.FairbattlePower.UpdateToSerf;

                player.Player.AddFlag(MsgUpdate.Flags.Confused, (int)Duration + 10, true);
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
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Victim.Player.View.SendView(stream.GameUpdateCreate(Victim.Player.UID, Game.MsgServer.MsgGameUpdate.DataType.Dizzy, false, 0, 0, 0), true);
            }
        }

        public override void End()
        {
            byte NO = 1;
            foreach (var player in PlayerScores.OrderByDescending(s => s.Value).ToArray())
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    PlayerList[player.Key].Player.View.SendView(stream.GameUpdateCreate(PlayerList[player.Key].Player.UID, Game.MsgServer.MsgGameUpdate.DataType.Dizzy, false, 0, 0, 0), true);
                }
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