﻿using TheChosenProject.Client;
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
    public class LadderTournament : Events
    {
        bool start = false;
        bool nextRound = false;
        public LadderTournament()
        {
            IDEvent = 3;
            IDMessage = MsgStaticMessage.Messages.PTB;
            EventTitle = "Skill Ladder";
            Duration = 20;
            BaseMap = 1507;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
            _Duration = 180;
            PotionsAllowed = false;
        }
        private readonly Dictionary<uint, List<uint>> mapsPairs = new Dictionary<uint, List<uint>>();
        private readonly List<string> WinnerList = new List<string>();

        public override void RemovePlayer(GameClient C, bool Diss = false, bool CanTeleport = true)
        {
            base.RemovePlayer(C);
            foreach (KeyValuePair<uint, List<uint>> Map in mapsPairs.ToList())
                if (Map.Value.Contains(C.EntityID))
                {
                    Map.Value.Remove(C.EntityID);
                    Broadcast(PlayerList[Map.Value[0]].Name + " has defeated " + C.Name + " in the Ladder Tournament and moved on to the next stage!", BroadCastLoc.World);

                    PlayerList[Map.Value[0]].Teleport(1616, 54, 64);
                    WinnerList.Add(PlayerList[Map.Value[0]].Player.Name);
                    mapsPairs.Remove(Map.Key);
                    //DMaps.DeleteDynamicMap(Map.Key, true);
                    break;
                }
        }

        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
            WinnerList.Clear();
            ushort _mapEvent = 700;
            byte _count = 0;
            int number = PlayerList.Count;
            Random r = new Random();
            foreach (GameClient C in PlayerList.OrderBy(x => r.Next()).ToDictionary(item => item.Key, item => item.Value).Values.ToList())
            {
                ChangePKMode(C, PKMode.PK);
                if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                    C.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                    C.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                    C.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                C.Player.HitPoints = (int)C.Status.MaxHitpoints;
                X = (ushort)(51 + Program.Rnd.Next(1, 21) - Program.Rnd.Next(1, 21));
                Y = (ushort)(50 + Program.Rnd.Next(1, 20) - Program.Rnd.Next(1, 20));
                C.EventBase.MapEvent = _mapEvent;
                if (_count == 0)
                {
                    if (number == 1)
                        WinnerList.Add(C.Player.Name);
                    else
                    {
                        Map = Database.Server.ServerMaps[700];
                        DinamicID = Map.GenerateDynamicID();
                        C.Teleport(X, Y, _mapEvent, DinamicID, true, true);
                        mapsPairs.Add(_mapEvent, new List<uint>());
                        mapsPairs[_mapEvent].Add(C.EntityID);
                        C.Teleport(X, Y, _mapEvent, DinamicID, true, true);
                        _count++;
                    }
                }
                else
                {
                    mapsPairs[_mapEvent].Add(C.EntityID);
                    C.Teleport(X, Y, _mapEvent, DinamicID, true, true);
                    _count = 0;
                    _mapEvent++;
                    number = number - 2;
                }
            }
        }

        public override void WaitForWinner()
        {
            base.WaitForWinner();

            if (DateTime.Now >= DisplayScores.AddMilliseconds(2000))
                DisplayScore();
            if (mapsPairs.Count == 0 || start)
            {
                if (!nextRound && !start)
                {
                    CountDown = 30;
                    nextRound = true;
                }
                else if (CountDown == 0 && mapsPairs.Count == 0)
                    nextRound = false;
                _nextMatch();
            }
        }

        public override void CharacterChecks(GameClient C)
        {
            base.CharacterChecks(C);
            if (!C.Player.Alive && DateTime.Now > C.Player.DeathHit.AddSeconds(2))
                RemovePlayer(C);
            foreach (KeyValuePair<uint, List<uint>> M in mapsPairs.ToList())
                if (M.Value.Contains(C.EntityID))
                    if (C.Player.Map != M.Key)
                        RemovePlayer(C);
        }

        public override void Hit(GameClient Attacker, GameClient Victim)
        {
            if (PlayerScores.ContainsKey(Victim.EntityID))
            {
                if (PlayerScores[Victim.EntityID] < 8)
                {
                    PlayerScores[Victim.EntityID]++;
                    Victim.SendSysMesage($"You can only be hitted [{(10 - PlayerScores[Victim.EntityID])}] more times!", MsgServer.MsgMessage.ChatMode.FirstRightCorner);

                }
                else if (PlayerScores[Victim.EntityID] == 8)
                {
                    PlayerScores[Victim.EntityID]++;
                    Victim.SendSysMesage("You'll be kicked if anyone hits you again! Watch out!");

                }
                else
                {
                    RemovePlayer(Victim);
                    //foreach (KeyValuePair<uint, List<uint>> Map in mapsPairs.ToList())
                    //    if (Map.Value.Contains(Victim.EntityID))
                    //        Map.Value.Remove(Victim.EntityID);
                    //Broadcast(Attacker.Name + " has defeated " + Victim.Name + " in the Ladder Tournament and moved on to the next stage!", BroadCastLoc.World);
                }
            }
        }

        private void _nextMatch()
        {
            if (CountDown > 0 && !start)
            {
                foreach (GameClient C in PlayerList.Values.ToList())
                    C.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);

                Broadcast("Time left: " + CountDown + " Seconds!", BroadCastLoc.Map);
                CountDown--;
            }
            else if (!start)
            {
                TeleportPlayersToMap();
                Broadcast("Next round of the " + EventTitle + " is about to start!", BroadCastLoc.Map);
                start = true;
                CountDown = 5;
            }
            else if (CountDown > 0)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (GameClient C in PlayerList.Values.ToList())
                        C.Player.SendString(stream, MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { $"downnumber{CountDown}" });
                    CountDown--;
                }
            }
            else
            {
                foreach (GameClient C in PlayerList.Values.ToList())
                    C.Player.RemoveFlag(MsgUpdate.Flags.Dead);

                start = false;
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            byte Score = 2;
            byte Rank = 1;
            foreach (GameClient C in PlayerList.Values.ToList())
                C.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
            foreach (KeyValuePair<uint, List<uint>> M in mapsPairs.ToList())
            {
                if (M.Value.Count == 2)
                {
                    Broadcast($"{PlayerList[M.Value[0]].Name} - {(10 - PlayerScores[M.Value[0]])} VS {PlayerList[M.Value[1]].Name} - {(10 - PlayerScores[M.Value[1]])}", BroadCastLoc.Score, Score);
                    Score++;
                }
            }
            foreach (string Name in WinnerList.ToList())
            {
                Broadcast($"Nº {Rank}: {Name}", BroadCastLoc.Score, Score);
                Rank++;
            }
            TimeSpan T = TimeSpan.FromSeconds(_Duration);
            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (_Duration > 0)
                --_Duration;
        }
        public override void Reward(GameClient client)
        {
            base.Reward(client);
        }
    }
}
