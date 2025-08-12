using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheChosenProject.Role.Flags;

namespace TheChosenProject.Game.MsgEvents
{
    class Spacelnvasion : Events
    {
        int ScoreMars = 0, ScoreSaturn = 0;
        Dictionary<uint, GameClient> Mars = new Dictionary<uint, GameClient>();
        Dictionary<uint, GameClient> Saturn = new Dictionary<uint, GameClient>();
        public Spacelnvasion()
        {
            IDEvent = 7;
            EventTitle = "FactionAnnihilation";
            IDMessage = MsgStaticMessage.Messages.KungfuSchool;
            Duration = 3;
            BaseMap = 1505;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = false;
            //ReviveAllowed = false;
            AllowedSkills = new List<ushort> { (ushort)12350, (ushort)1045, (ushort)1046, (ushort)1047 };
            Duration = 180;
            PotionsAllowed = false;

        }
        public override void BeginTournament()
        {
            Teams = new Dictionary<uint, Dictionary<uint, GameClient>>();
            base.BeginTournament();
        }
        public override void TeleportPlayersToMap()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var counter = 0;
                foreach (GameClient client in PlayerList.Values)
                {
                    ChangePKMode(client, PKMode.PK);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                    if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.DragonSwing))
                        client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.DragonSwing);
                    //client.Player.HitPoints = (int)client.Status.MaxHitpoints;
                    client.Equipment.Remove(Role.Flags.ConquerItem.Garment, stream);

                    if (counter % 2 == 0)
                    {
                        Mars.Add(client.EntityID, client);
                        client.Teleport(136, 211, Map.ID, DinamicID, true, true);
                        client.Player.AddTempEquipment(stream, 193605, ConquerItem.Garment);
                        client.SendSysMesage($"Welcome to {EventTitle} you're a member of team FieryRedUniform!");
                    }
                    else
                    {
                        Saturn.Add(client.EntityID, client);
                        client.Teleport(187, 207, Map.ID, DinamicID, true, true);
                        client.Player.AddTempEquipment(stream, 193075, ConquerItem.Garment);
                        client.SendSysMesage($"Welcome to {EventTitle} you're a member of team KungfuGown!");
                    }
                    counter++;
                }
                Teams.Add(193605, Mars);//FieryFieryRedUniform
                Teams.Add(193075, Saturn);//KungfuGown
            }
        }

        public void TeleafterRev(GameClient client)
        {
            if (Teams[193605].ContainsKey(client.EntityID))
                client.Teleport(136, 211, Map.ID, DinamicID);
            else
                client.Teleport(187, 207, Map.ID, DinamicID);
        }

        public override void Hit(GameClient Attacker, GameClient Victim)
        {
            if (Stage == EventStage.Fighting)
            {
                Victim.Player.Dead(Attacker.Player, Victim.Player.X, Victim.Player.Y, Attacker.EntityID);
                Attacker.SendSysMesage("" + Victim.Name.ToString() + " was killed by " + Attacker.Name + "!", MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red, true);
                Victim.SendSysMesage("" + Victim.Name.ToString() + " was killed by " + Attacker.Name + "!", MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red, true);

                if (Teams[193605].ContainsKey(Attacker.EntityID))
                    ScoreMars++;
                else
                    ScoreSaturn++;
                PlayerScores[Attacker.EntityID]++;
                //RevivePlayer(Victim);
                //TeleafterRev(Victim);
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values)
                player.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);

            foreach (var player in PlayerList.Values)
            {
                if (ScoreMars > ScoreSaturn)
                {
                    player.SendSysMesage($"Team FieryRedUniform - {ScoreMars}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                    player.SendSysMesage($"Team KungfuGown - {ScoreSaturn}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                }
                else
                {
                    player.SendSysMesage($"Team KungfuGown - {ScoreSaturn}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                    player.SendSysMesage($"Team FieryRedUniform - {ScoreMars}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                }
                player.SendSysMesage($"My Score - {PlayerScores[player.EntityID]}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }

        public override void End()
        {
            DisplayScore();
            if (ScoreMars == ScoreSaturn)
            {
                Broadcast("It's a tie! event Minutes have passed and the teams scored the same points! Better luck next time!", BroadCastLoc.World);
                foreach (GameClient C in PlayerList.Values.ToList())
                    C.EventBase?.RemovePlayer(C);
            }
            else
            {
                if (ScoreMars > ScoreSaturn)
                {
                    Broadcast("The FieryRedUniform Team has won the " + EventTitle + "! Congratulations to all their members!", BroadCastLoc.World);
                    foreach (GameClient C2 in Teams[193075].Values.ToList())
                        C2.EventBase?.RemovePlayer(C2);

                }
                else if (ScoreSaturn > ScoreMars)
                {
                    Broadcast("The KungfuGown Team has won the " + EventTitle + "! Congratulations to all their members!", BroadCastLoc.World);
                    foreach (GameClient C in Teams[193605].Values.ToList())
                        C.EventBase?.RemovePlayer(C);

                }
                foreach (var c in PlayerList.Values.ToList())
                {
                    Reward(c);
                    RemovePlayer(c);
                }
            }
            PlayerList.Clear();
            PlayerScores.Clear();
            Teams.Clear();
            Program.Events.Remove(this);

            return;
        }

        public override void Reward(GameClient client)
        {
            base.Reward(client);

        }
    }
}