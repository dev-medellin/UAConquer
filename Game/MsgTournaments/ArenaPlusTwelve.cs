using System;
using System.Collections.Generic;
using System.Linq;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgEvents;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role;

namespace TheChosenProject.Game.MsgTournaments
{
    public class ArenaPlusTwelve
    {
        #region Properties

        public ushort BaseMap = 3020;
        public List<ushort> AllowedSkills = new List<ushort>();
        public Dictionary<uint, GameClient> PlayerList = new Dictionary<uint, GameClient>();
        public readonly Dictionary<uint, uint> PlayerScores = new Dictionary<uint, uint>();
        public DateTime LastScores;
        public DateTime DisplayScores;
        public KillerSystem KillSystem;

        public Role.GameMap Map;
        public uint DinamicID = 10000002;

        #endregion

        public ArenaPlusTwelve()
        {
        
            Map = Database.Server.ServerMaps[this.BaseMap];

            DinamicID = Map.GenerateDynamicID();
        }
    
        public virtual void TeleportPlayersToMap(GameClient client)
        {
            ChangePKMode(client, Flags.PKMode.PK);
            ushort x = 0;
            ushort y = 0;
            Map.GetRandCoord(ref x, ref y);
            client.Teleport(x, y, Map.ID,CanTeleport: true);
        
        
        
            if (!PlayerList.ContainsKey(client.Player.UID))
            {
                PlayerList.Add(client.Player.UID,client);
                PlayerScores.Add(client.Player.UID, 0);
            }
      
        
            client.Player.HitPoints = (int)client.Status.MaxHitpoints;
            client.Player.Mana = (ushort)client.Status.MaxMana;
        
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                String msg = $"{client.Player.Name} Has Enter Arena +12 Come To Fight Him";
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Whisper).GetArray(stream));
                //Program.DiscordAPI.Enqueue(msg);
            }

        }
    
        public bool InTournament(GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID;
        }
        public void ChangePKMode(GameClient client, Flags.PKMode Mode)
        {
            client.Player.SetPkMode(Mode);
        }

        public void Hit(GameClient client, GameClient attackedOwner, MsgSpellAnimation.SpellObj objDamage)
        {
            //Console.WriteLine($"{client.Player.Name} Give {attackedOwner.Player.Name} By {objDamage.Damage}");
            try
            {
                PlayerScores[client.Player.UID] += objDamage.Damage;
            }
            catch (Exception e)
            {
               Console.WriteLine("Player Not Register In Arena");
           
            }
        }

        public void Kill(GameClient client, GameClient attackedOwner, MsgSpellAnimation.SpellObj objDamage)
        {
            try
            {
                PlayerScores[client.Player.UID] += objDamage.Damage;
        
                //Reset Killed Score
                if (PlayerList.ContainsKey(attackedOwner.Player.UID))
                {
         
                    PlayerScores[attackedOwner.Player.UID] = 0;
                }
        
      
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    String msg = $"{client.Player.Name} Kill {attackedOwner.Player.Name} In Arena Plus";
                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.white, MsgServer.MsgMessage.ChatMode.Whisper).GetArray(stream));
                    //Program.DiscordAPI.Enqueue(msg);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Player Not Register In Arena");
            }
     
          //  Console.WriteLine($"{client.Player.Name} Kill {attackedOwner.Player.Name} By {spellObj.Damage}");
        }

        public void actionHandler()
        {
            if (DateTime.Now >= LastScores.AddMilliseconds(3000))
                DisplayScore();
        }

        private void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values)
            {   
            
                if(InTournament(player))
                player.SendSysMesage($"---------Top Arena Hit---------", MsgMessage.ChatMode.FirstRightCorner);
            }

            byte Score = 2;
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)))
            {
                if (Score == 7)
                    break;
                if (Score == PlayerScores.Count + 2)
                    break;
            
                Broadcast($"Nº {Score - 1}: {PlayerList[kvp.Key].Player.Name} - {kvp.Value}", BroadCastLoc.Score,
                    Score);
                Score++;
            }
        }
           public void Broadcast(string msg, BroadCastLoc loc, uint Map = 0)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    switch (loc)
                    {
                        case BroadCastLoc.World:
                            Program.SendGlobalPackets.Enqueue(
                                new MsgServer.MsgMessage(msg, MsgMessage.MsgColor.white, MsgMessage.ChatMode.Center).GetArray(stream));
                            break;
                        case BroadCastLoc.WorldY:
                            Program.SendGlobalPackets.Enqueue(
                                new MsgServer.MsgMessage(msg, MsgMessage.MsgColor.white, MsgMessage.ChatMode.TopLeft).GetArray(stream));
                            break;
                        case BroadCastLoc.Map:
                            foreach (GameClient client in PlayerList.Values.ToList())
                            {
                                client.SendSysMesage(msg);
                            }

                            break;
                        case BroadCastLoc.YellowMap:
                            foreach (GameClient client in PlayerList.Values.ToList())
                            {
                                if(InTournament(client))
                                client.SendSysMesage(msg, MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.yellow);
                            }

                            break;
                        case BroadCastLoc.Title:
                            foreach (GameClient client in PlayerList.Values.ToList())
                            {
                                if(InTournament(client))
                                client.SendSysMesage(msg, MsgMessage.ChatMode.FirstRightCorner);
                            }

                            break;
                        case BroadCastLoc.Score:
                            foreach (GameClient client in PlayerList.Values.ToList())
                            {
                                if(InTournament(client))
                                client.SendSysMesage(msg, MsgMessage.ChatMode.ContinueRightCorner);
                            }

                            break;
                        case BroadCastLoc.SpecificMap:
                            foreach (GameClient client in PlayerList.Values.ToList())
                            {
                                     if(InTournament(client))
                                    client.SendSysMesage(msg, MsgMessage.ChatMode.ContinueRightCorner);
                            }

                            break;
                    }
                }
            }
    }
}