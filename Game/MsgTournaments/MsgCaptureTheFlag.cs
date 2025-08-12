using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role.Instance;
using TheChosenProject.Role;
using TheChosenProject.ServerSockets;
using TheChosenProject.Database;

namespace TheChosenProject.Game.MsgTournaments
{
    public class MsgCaptureTheFlag
    {
        public class Basse
        {
            public class WarScrore
            {
                public uint GuildID;

                public string Name;

                public uint Score;
            }

            public SobNpc Npc;

            public SafeDictionary<uint, WarScrore> Scores = new SafeDictionary<uint, WarScrore>();

            public uint CapturerID;

            public bool IsX2;

            internal void UpdateScore(Player client, uint Damage)
            {
                if (client.MyGuild != null)
                {
                    if (!Scores.ContainsKey(client.GuildID))
                        Scores.Add(client.GuildID, new WarScrore
                        {
                            GuildID = client.MyGuild.Info.GuildID,
                            Name = client.MyGuild.GuildName,
                            Score = Damage
                        });
                    else
                        Scores[client.MyGuild.Info.GuildID].Score += Damage;
                }
            }
        }

        public const ushort MapID = 2057;

        public const ushort AliveTournamentMinutes = 59;

        public const ushort X2CastleMinutes = 15;

        public const ushort UpScoreBoardSeconds = 6;

        public GameMap Map;

        public uint X2Castle;

        public ProcesType Proces;

        public SafeDictionary<uint, Basse> Bases;

        public DateTime UpdateStampScore;

        public DateTime SendX2LoctionStamp;

        public DateTime TournamentStamp;

        public ConcurrentDictionary<uint, Guild> RegistredGuilds = new ConcurrentDictionary<uint, Guild>();

        public MsgCaptureTheFlag()
        {
            Proces = ProcesType.Dead;
            Bases = new SafeDictionary<uint, Basse>();
            Program.FreePkMap.Add(2057);
        }

        public void Start()
        {
            if (Proces != ProcesType.Dead)
                return;
            TournamentStamp = DateTime.Now;
            Bases.Clear();
            CreateBases();
            foreach (Guild guild in Guild.GuildPoll.Values)
            {
                guild.ClaimCtfReward = 0;
                guild.CTF_Exploits = 0;
                guild.CTF_Rank = 0;
                foreach (Guild.Member user2 in guild.Members.Values)
                {
                    user2.CTF_Exploits = 0;
                    user2.RewardConquerPoints = 0;
                    user2.RewardMoney = 0;
                    user2.CTF_Claimed = 0;
                }
            }
            RegistredGuilds = new ConcurrentDictionary<uint, Guild>();
            GenerateX2Castle();
            Proces = ProcesType.Alive;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    user.Player.MessageBox("", delegate (GameClient p)
                    {
                        p.Teleport(354, 338, 1002);
                    }, null, 60, MsgStaticMessage.Messages.CapturetheFlag);
                }
            }
            try
            {
                ITournamentsAlive.Tournments.Add(14, ": started at(" + DateTime.Now.ToString("H:mm)"));
            }
            catch
            {
                ServerKernel.Log.SaveLog("Could not start CaptureTheFlag", true, LogType.WARNING);
            }
        }

        public void CheckFinish(bool finish = false)
        {
            if (!((Proces == ProcesType.Alive && DateTime.Now > TournamentStamp.AddMinutes(59.0)) || finish))
                return;
            Proces = ProcesType.Dead;
            Guild[] array;
            array = Guild.GuildPoll.Values.Where((Guild p) => p.CTF_Exploits != 0).ToArray();
            Guild[] ranks;
            ranks = array.OrderByDescending((Guild p) => p.CTF_Exploits).ToArray();
            for (int x2 = 0; x2 < Math.Min(9, ranks.Length); x2++)
            {
                ranks[x2].CTF_Rank = (byte)(x2 + 1);
            }
            foreach (Guild guild in Guild.GuildPoll.Values)
            {
                if (RegistredGuilds.ContainsKey(guild.Info.GuildID))
                {
                    Guild.Member[] array_members;
                    array_members = guild.Members.Values.Where((Guild.Member p) => p.CTF_Exploits != 0).ToArray();
                    Guild.Member[] Ranks_members;
                    Ranks_members = array_members.OrderByDescending((Guild.Member p) => p.CTF_Exploits).ToArray();
                    for (int x = 0; x < Ranks_members.Length; x++)
                    {
                        uint[] rank;
                        rank = CalculateMemberRewardCTF((uint)(x + 1), guild);
                        Ranks_members[x].RewardConquerPoints = rank[0];
                        Ranks_members[x].RewardMoney = rank[1];
                    }
                }
                guild.CTF_Next_ConquerPoints = 0;
                guild.CTF_Next_Money = 0;
            }
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                try
                {
                    ITournamentsAlive.Tournments.Remove(14);
                }
                catch
                {
                    ServerKernel.Log.SaveLog("Could not finish CaptureTheFlag", true, LogType.WARNING);
                }
                Program.SendGlobalPackets.Enqueue(new MsgMessage("Capture The Flag has finished.", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center).GetArray(stream));
            }
            foreach (GameClient user in Server.GamePoll.Values)
            {
                if (user.Player.Map == 2057)
                    user.Teleport(428, 378, 1002);
            }
        }

        private uint[] CalculateMemberRewardCTF(uint Rank, Guild guild)
        {
            return new uint[2]
            {
                guild.CTF_Next_ConquerPoints / (Rank + 1),
                guild.CTF_Next_Money / (Rank + 1)
            };
        }

        public void GenerateX2Castle()
        {
            int random;
            random = ServerKernel.NextAsync(0, Bases.Count);
            Basse basse;
            basse = Bases.Values.ToArray()[X2Castle];
            basse.IsX2 = false;
            X2Castle = (uint)random;
            UpdateMapX2Location();
        }

        public void UpdateMapX2Location()
        {
            Basse basse;
            basse = Bases.Values.ToArray()[X2Castle];
            basse.IsX2 = true;
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.X2Location, X2Castle + 1, 1);
                stream.AddX2LocationCaptureTheFlagUpdate(basse.Npc.X, basse.Npc.Y);
                stream.CaptureTheFlagUpdateFinalize();
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if (user.Player.Map == 2057 && user.Player.DynamicID == 0)
                        user.Send(stream);
                }
            }
        }

        public bool Join(GameClient user, Packet stream)
        {
            if (Proces == ProcesType.Alive)
            {
                if (!RegistredGuilds.ContainsKey(user.Player.GuildID))
                    RegistredGuilds.TryAdd(user.Player.GuildID, user.Player.MyGuild);
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.InitializeCTF, 0);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);
                Basse basse;
                basse = Bases.Values.ToArray()[X2Castle];
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.X2Location, X2Castle + 1, 1);
                stream.AddX2LocationCaptureTheFlagUpdate(basse.Npc.X, basse.Npc.Y);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);
                user.Teleport(478, 373, 2057);
                return true;
            }
            return false;
        }

        public void CreateBases()
        {
            Map = Server.ServerMaps[2057u];
            foreach (IMapObj npc in Map.View.GetAllMapRoles(MapObjectType.SobNpc))
            {
                Bases.Add(npc.UID, new Basse
                {
                    Npc = (npc as SobNpc)
                });
            }
            SpawnFlags();
        }

        public void CheckUpX2()
        {
            if (Proces == ProcesType.Alive && DateTime.Now > SendX2LoctionStamp.AddMinutes(15.0))
            {
                GenerateX2Castle();
                SendX2LoctionStamp = DateTime.Now;
            }
        }

        public void SpawnFlags()
        {
            for (int i = 10 - Map.View.GetAllMapRolesCount(MapObjectType.StaticRole); i > 0; i--)
            {
                ushort x;
                x = 0;
                ushort y;
                y = 0;
                Map.GetRandCoord(ref x, ref y);
                if (!InMainCastle(x, y))
                {
                    StaticRole role;
                    role = new StaticRole(x, y)
                    {
                        Map = 2057u
                    };
                    Map.AddStaticRole(role);
                }
            }
        }

        public void UpdateMapScore()
        {
            if (Proces == ProcesType.Alive && DateTime.Now > UpdateStampScore.AddSeconds(6.0))
            {
                SendUpdateBoardScore();
                UpdateStampScore = DateTime.Now;
            }
        }

        public void SendUpdateBoardScore()
        {
            Guild[] array;
            array = RegistredGuilds.Values.Where((Guild p) => p.CTF_Exploits != 0).ToArray();
            Guild[] rank;
            rank = array.OrderByDescending((Guild p) => p.CTF_Exploits).ToArray();
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.InitializeCTF, 0);
                stream.CaptureTheFlagUpdateFinalize();
                foreach (GameClient user3 in Server.GamePoll.Values)
                {
                    if (user3.Player.Map == 2057 && user3.Player.DynamicID == 0)
                        user3.Send(stream);
                }
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.ScoreUpdate, 2, (uint)Math.Min(rank.Length, 5));
                for (uint x2 = 0; x2 < rank.Length && x2 != 4; x2++)
                {
                    Guild element2;
                    element2 = rank[x2];
                    stream.AddItemCaptureTheFlagUpdate(x2, element2.CTF_Exploits, element2.GuildName);
                }
                stream.CaptureTheFlagUpdateFinalize();
                foreach (GameClient user2 in Server.GamePoll.Values)
                {
                    if (user2.Player.Map == 2057 && user2.Player.DynamicID == 0)
                        user2.Send(stream);
                }
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if (user.Player.Map == 2057 && user.Player.DynamicID == 0 && TryGetBase(user, out var flag_base))
                    {
                        Basse.WarScrore[] array_scorebasse;
                        array_scorebasse = flag_base.Scores.Values.OrderByDescending((Basse.WarScrore p) => p.Score).ToArray();
                        stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.ScoreBase, 0, (uint)Math.Min(array_scorebasse.Length, 5));
                        for (uint x = 0; x < array_scorebasse.Length && x != 4; x++)
                        {
                            Basse.WarScrore element;
                            element = array_scorebasse[x];
                            stream.AddItemCaptureTheFlagUpdate(x, element.Score, element.Name);
                        }
                        stream.CaptureTheFlagUpdateFinalize();
                        user.Send(stream);
                    }
                }
            }
        }

        public bool TryGetBase(GameClient user, out Basse bas)
        {
            if (user.Player.Map == 2057 && user.Player.DynamicID == 0)
            {
                foreach (Basse flag_base in Bases.Values)
                {
                    if (Core.GetDistance(user.Player.X, user.Player.Y, flag_base.Npc.X, flag_base.Npc.Y) <= 11)
                    {
                        bas = flag_base;
                        return true;
                    }
                }
            }
            bas = null;
            return false;
        }

        public void UpdateFlagScore(Player client, SobNpc Attacked, uint Damage, Packet stream)
        {
            if (Proces != ProcesType.Alive || client.MyGuild == null || !Bases.TryGetValue(Attacked.UID, out var Bas))
                return;
            Bas.UpdateScore(client, Damage);
            if (Bas.Npc.HitPoints != 0)
                return;
            Basse.WarScrore[] array;
            array = Bas.Scores.Values.OrderByDescending((Basse.WarScrore p) => p.Score).ToArray();
            Basse.WarScrore GuildWinner;
            GuildWinner = array.First();
            Bas.CapturerID = GuildWinner.GuildID;
            Bas.Scores.Clear();
            Bas.Npc.HitPoints = Bas.Npc.MaxHitPoints;
            Bas.Npc.Name = GuildWinner.Name;
            foreach (GameClient user in Server.GamePoll.Values)
            {
                if (user.Player.Map == 2057 && user.Player.DynamicID == 0 && Core.GetDistance(user.Player.X, user.Player.Y, Bas.Npc.X, Bas.Npc.Y) <= 9)
                {
                    MsgUpdate upd;
                    upd = new MsgUpdate(stream, Bas.Npc.UID, 2);
                    stream = upd.Append(stream, MsgUpdate.DataType.Mesh, (long)Bas.Npc.Mesh);
                    stream = upd.Append(stream, MsgUpdate.DataType.Hitpoints, Bas.Npc.HitPoints);
                    stream = upd.GetArray(stream);
                    client.Send(stream);
                    client.Send(Bas.Npc.GetArray(stream, true));
                }
            }
            stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.OccupiedBase, (byte)(Bas.Npc.UID % 10));
            stream.CaptureTheFlagUpdateFinalize();
            SendMapPacket(stream);
        }

        public void SendMapPacket(Packet stream)
        {
            foreach (GameClient user in Server.GamePoll.Values)
            {
                if (user.Player.Map == 2057 && user.Player.DynamicID == 0)
                    user.Send(stream);
            }
        }

        public bool Attackable(Player user)
        {
            return !InMainCastle(user.X, user.Y);
        }

        public bool InMainCastle(ushort X, ushort Y)
        {
            return Core.GetDistance(X, Y, 482, 367) < 32;
        }

        public void PlantTheFlag(GameClient user, Packet stream)
        {
            if (Proces == ProcesType.Alive && user.Player.Map == 2057 && user.Player.MyGuild != null && user.Player.ContainFlag(MsgUpdate.Flags.CTF_Flag) && TryGetBase(user, out var flag_base) && flag_base.CapturerID == user.Player.GuildID)
            {
                user.Player.RemoveFlag(MsgUpdate.Flags.CTF_Flag);
                uint exploits;
                exploits = (uint)((int)user.Player.Level / 2);
                if (flag_base.IsX2)
                    exploits *= 2;
                user.Player.MyGuild.CTF_Exploits += exploits;
                user.Player.MyGuildMember.CTF_Exploits += exploits;
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateTimer, 0, user.Player.UID);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateEffect, user.Player.UID);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);
                stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.RemoveFlagEffect, user.Player.UID);
                stream.CaptureTheFlagUpdateFinalize();
                user.Send(stream);
            }
        }

        public unsafe void ChechMoveFlag(GameClient user)
        {
            if (Proces != ProcesType.Alive || user.Player.Map != 2057 || user.Player.MyGuild == null || user.Player.ContainFlag(MsgUpdate.Flags.CTF_Flag))
                return;
            foreach (IMapObj flag in user.Map.View.Roles(MapObjectType.StaticRole, user.Player.X, user.Player.Y))
            {
                if (Core.GetDistance(user.Player.X, user.Player.Y, flag.X, flag.Y) < 2)
                {
                    using (RecycledPacket rec = new RecycledPacket())
                    {
                        Packet stream;
                        stream = rec.GetStream();
                        user.Player.AddFlag(MsgUpdate.Flags.CTF_Flag, 60, true);
                        stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateTimer, 60, user.Player.UID);
                        stream.CaptureTheFlagUpdateFinalize();
                        user.Send(stream);
                        stream.CaptureTheFlagUpdateCreate(MsgCaptureTheFlagUpdate.Mode.GenerateEffect, user.Player.UID);
                        stream.CaptureTheFlagUpdateFinalize();
                        user.Send(stream);
                        user.Map.View.LeaveMap(flag);
                        ActionQuery actionQuery;
                        actionQuery = default(ActionQuery);
                        actionQuery.ObjId = flag.UID;
                        actionQuery.Type = ActionType.RemoveEntity;
                        ActionQuery action;
                        action = actionQuery;
                        user.Player.View.SendView(stream.ActionCreate(&action), true);
                        break;
                    }
                }
            }
        }
    }
}
