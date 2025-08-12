using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Extensions;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;
using static DevExpress.XtraEditors.Mask.Design.MaskSettingsForm.DesignInfo.MaskManagerInfo;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgCityWar
    {
        public bool SendInvitation = false;
        public const uint MapID = 2058;
        public class Basse
        {
            public SobNpc Npc;
            public SafeDictionary<uint, WarScrore> Scores = new SafeDictionary<uint, WarScrore>();
            public uint CapturerID = 0;
            public bool claimed;
            public class WarScrore
            {
                public uint GuildID;
                public string Name;
                public uint Score;
            }

            internal void UpdateScore(Player client, uint Damage)
            {
                if (client.MyGuild == null)
                    return;
                if (!Scores.ContainsKey(client.GuildID))
                {
                    Scores.Add(client.GuildID, new WarScrore()
                    {
                        GuildID = client.MyGuild.Info.GuildID,
                        Name = client.MyGuild.GuildName,
                        Score = Damage
                    });
                }
                else
                {
                    Scores[client.MyGuild.Info.GuildID].Score += Damage;
                }
            }
        }
        public class Scoreboard
        {
            public uint UID;
            public string Name;
            public uint MainPoints;
            public uint DeathPoints;
            public byte Class;
            public bool Claimed = false;
        }

        public ConcurrentDictionary<uint, Scoreboard> Scoreboards = new ConcurrentDictionary<uint, Scoreboard>();

        public void AddPoints(Role.Player client, uint points, uint death)
        {
            Scoreboards.AddOrUpdate(client.UID, new Scoreboard()
            {
                UID = client.UID,
                Name = client.Name,
                MainPoints = points,
                DeathPoints = death,
                Class = client.Class
            }, (key, oldValue) =>
            {
                oldValue.MainPoints += points;
                oldValue.DeathPoints += death;
                return oldValue;
            });
        }
        public GameMap Map;
        public ProcesType Proces;
        public SafeDictionary<uint, Basse> Bases;
        public MsgCityWar()
        {
            Bases = new SafeDictionary<uint, Basse>();
            Proces = ProcesType.Dead;
        }

        public void Start()
        {
            if (Proces == ProcesType.Dead)
            {
                Scoreboards.Clear();
                Bases.Clear();
                CreateBases();
                byte id = 1;
                foreach (var bas in Bases.Values.OrderBy(x=> x.Npc.UID))
                {
                    bas.Npc.Name = $"Base[{id}]";
                    bas.CapturerID = 0;
                    id++;
                }
                Proces = ProcesType.Alive;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    Program.SendGlobalPackets.Enqueue(new MsgMessage("City war has started!", MsgMessage.MsgColor.white, MsgMessage.ChatMode.Talk).GetArray(stream));
                }
            }
        }

        public void CreateBases()
        {
            Map = Database.Server.ServerMaps[MapID];

            foreach (var npc in Map.View.GetAllMapRoles(MapObjectType.SobNpc).OrderBy(x=> x.UID))
                Bases.Add(npc.UID, new Basse() { Npc = npc as SobNpc });
        }
        public bool TryGetBase(Client.GameClient user, out Basse bas)
        {
            if (user.Player.Map == MapID)
            {
                foreach (var flag_base in Bases.Values)
                {
                    if (Core.GetDistance(user.Player.X, user.Player.Y, flag_base.Npc.X, flag_base.Npc.Y) <= 18)
                    {
                        bas = flag_base;
                        return true;
                    }
                }
            }
            bas = null;
            return false;
        }

        public void Finish()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendMapPacket(new MsgMessage($"City War has finished.", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                }
                foreach (var user in Map.Values)
                {
                    user.Teleport(50, 50, 1005);
                }
            }
        }
        public void UpdateScore(Role.Player client, SobNpc Attacked, uint Damage)
        {
            if (Proces != ProcesType.Alive)
                return;
            if (client.MyGuild == null)
                return;

            if (Bases.TryGetValue(Attacked.UID, out var Bas))
            {
                Bas.UpdateScore(client, Damage);

                if (Bas.Npc.HitPoints == 0)
                {
                    var array = Bas.Scores.Values.OrderByDescending(p => p.Score).ToArray();
                    var GuildWinner = array.First();

                    Bas.CapturerID = GuildWinner.GuildID;

                    Bas.Scores.Clear();
                    Bas.Npc.HitPoints = Bas.Npc.MaxHitPoints;
                    Bas.Npc.Name = GuildWinner.Name;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        foreach (var user in Map.Values)
                        {
                            if (Core.GetDistance(user.Player.X, user.Player.Y, Bas.Npc.X, Bas.Npc.Y) <= 18)
                            {
                                var Updates = new MsgUpdate(stream, Bas.Npc.UID, 2);
                                stream = Updates.Append(stream, MsgUpdate.DataType.Mesh, (long)Bas.Npc.Mesh);
                                stream = Updates.Append(stream, MsgUpdate.DataType.Hitpoints, Bas.Npc.HitPoints);
                                stream = Updates.GetArray(stream);
                                client.Send(stream);
                                client.Send(Bas.Npc.GetArray(stream, false));
                            }
                        }
                    }
                }
            }
        }

        public void CheckUp()
        {
            if (Proces == ProcesType.Alive)
            {
                SortScores();
            }
        }
        public void SortScores()
        {
            if (Proces == ProcesType.Alive)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    bool first = false;
                    SendMapPacket(new MsgMessage($"", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
                    foreach (var client in Map.Values)
                    {
                        if (TryGetBase(client, out var Base))
                        {
                            var score = Base.Scores.Values.OrderByDescending(x => x.Score).ToArray();
                            for (var i = 0; i < score.Length; i++)
                            {
                                first = true;
                                client.SendSysMesage("No " + (i + 1).ToString() + ". " + score[i].Name + " (" + score[i].Score + ")", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                                if (i == 4) break;
                            }
                        }
                    }
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, first ? MsgMessage.ChatMode.ContinueRightCorner : MsgMessage.ChatMode.FirstRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage("Winners Board", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    var bs = Bases.Values.ToArray();
                    SendMapPacket(new MsgMessage($"Phoenix City Winner: {bs[0].Npc.Name}", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Ape City Winner: {bs[1].Npc.Name}", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Desert City Winner: {bs[2].Npc.Name}", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Bird Island Winner: {bs[3].Npc.Name}", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));

                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Players Kill Scores (Kill/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    var top = Scoreboards.Values.Where(x=> !AtributesStatus.IsWater(x.Class)).OrderByDescending(x => x.MainPoints).ToArray();
                    foreach (var killer in top.Take(5))
                    {
                        SendMapPacket(new MsgMessage(killer.Name + " " + killer.MainPoints + " / " + killer.DeathPoints + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    }
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Players Revive Scores (Revive/Death)", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    top = Scoreboards.Values.Where(x => AtributesStatus.IsWater(x.Class)).OrderByDescending(x => x.MainPoints).ToArray();
                    foreach (var killer in top.Take(5))
                    {
                        SendMapPacket(new MsgMessage(killer.Name + " " + killer.MainPoints + " / " + killer.DeathPoints + " ", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    }
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    foreach (var user in Map.Values)
                    {
                        if (Scoreboards.TryGetValue(user.Player.UID, out var score))
                        {
                            if (AtributesStatus.IsWater(score.Class))
                            {
                                user.SendSysMesage("Your Revive Score (Revive/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                                user.SendSysMesage($"{score.Name}- {score.MainPoints}/{score.DeathPoints}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                            }
                            else
                            {
                                user.SendSysMesage("Your Kills Score (Kills/Death)", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                                user.SendSysMesage($"{score.Name}- {score.MainPoints}/{score.DeathPoints}", MsgMessage.ChatMode.ContinueRightCorner, MsgMessage.MsgColor.yellow);
                            }
                        }
                    }
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"Guild - [Total Players] - [Remaining]", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    SendMapPacket(new MsgMessage($"-----------------------", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    var Guilds = new List<Role.Instance.Guild>();
                    foreach (var client in Map.Values)
                    {
                        if (client.Player.MyGuild != null)
                        {
                            if (!Guilds.Contains(client.Player.MyGuild))
                                Guilds.Add(client.Player.MyGuild);
                        }
                    }
                    foreach (var guild in Guilds.OrderByDescending(x=> x.Members.Count))
                    {
                        SendMapPacket(new MsgMessage($"{guild.GuildName} - {guild.Members.Count} - {Map.Values.Count(x=> x.Player.GuildID == guild.Info.GuildID && x.Player.Alive)}", MsgMessage.MsgColor.yellow, MsgMessage.ChatMode.ContinueRightCorner).GetArray(stream));
                    }
                }
            }
        }
        internal void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.ServerMaps[MapID].Values)
            {
                client.Send(packet);
            }
        }
        public void Save()
        {
            var ini = new WindowsAPI.IniFile("\\CityWar.ini");
            foreach (var xx in Bases.Values)
            {
                ini.Write<uint>(xx.Npc.UID.ToString(), "UID", xx.Npc.UID);
                ini.Write<string>(xx.Npc.UID.ToString(), "Name", xx.Npc.Name);
                ini.Write<bool>(xx.Npc.UID.ToString(), "Claimed", xx.claimed);
                ini.Write<uint>(xx.Npc.UID.ToString(), "CapturerID", xx.CapturerID);
            }
            ini.Write<int>("Scoreboard", "Count", Scoreboards.Count);
            int count = 0;
            foreach (var xx in Scoreboards.Values)
            {
                ini.Write<uint>("Scoreboard", "UID" + count, xx.UID);
                ini.Write<string>("Scoreboard", "Name" + count, xx.Name);
                ini.Write<byte>("Scoreboard", "Class" + count, xx.Class);
                ini.Write<uint>("Scoreboard", "MainPoints" + count, xx.MainPoints);
                ini.Write<uint>("Scoreboard", "DeathPoints" + count, xx.DeathPoints);
                ini.Write<bool>("Scoreboard", "Claimed" + count, xx.Claimed);
                count++;
            }
        }
        public void Load()
        {
            var ini = new WindowsAPI.IniFile("\\CityWar.ini");
            foreach (var xx in Bases.Values)
            {
                xx.Npc.Name = ini.ReadString(xx.Npc.UID.ToString(), "Name", "");
                xx.claimed = ini.ReadBool(xx.Npc.UID.ToString(), "Claimed", false);
                xx.CapturerID = ini.ReadUInt32(xx.Npc.UID.ToString(), "CapturerID", 0);
            }
            var count = ini.ReadInt32("Scoreboard", "Count", 0);
            for (var xx = 0; xx < count; xx++)
            {
                var UID = ini.ReadUInt32("Scoreboard", "UID" + xx, 0);
                var Name = ini.ReadString("Scoreboard", "Name" + xx, "");
                var Class = ini.ReadByte("Scoreboard", "Class" + xx, 0);
                var MainPoints = ini.ReadUInt32("Scoreboard", "MainPoints" + xx, 0);
                var DeathPoints = ini.ReadUInt32("Scoreboard", "DeathPoints" + xx, 0);
                var claimed = ini.ReadBool("Scoreboard", "Claimed" + xx, false);
                Scoreboards.TryAdd(UID, new Scoreboard()
                {
                    UID = UID,
                    Name = Name,
                    Class = Class,
                    MainPoints = MainPoints,
                    DeathPoints = DeathPoints,
                    Claimed = claimed,
                });
            }
        }
    }
}
