using DevExpress.Utils.DirectXPaint;
using Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Database;
using TheChosenProject.Game.ConquerStructures.PathFinding;
using TheChosenProject.Game.MsgServer;

using TheChosenProject.Game.MsgTournaments;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Role
{
    public class SobNpc : IMapObj
    {
        public int BotCount = 0;

        public enum StaticMesh : ushort
        {
            Vendor = 406,
            LeftGate = 241,
            OpenLeftGate = 251,
            RightGate = 277,
            OpenRightGate = 287,
            Pole = 1137,
            SuperGuildWarPole = 31220
        }

        public Statue statue;

        public StatusFlagsBigVector32 BitVector;

        public const byte SeedDistrance = 100;

        private int Hit;

        internal DateTime LeftGateStamp;

        internal DateTime RightGateStamp = DateTime.Now;

        public Time32 LastVIPTeleport;

        public Time32 LastRefresh;

        public StaticMesh Mesh;

        public Flags.NpcType Type;

        public ushort Sort;

        public string Name;

        public GameClient OwnerVendor;

        public bool AllowDynamic { get; set; }

        public uint IndexInScreen { get; set; }

        public bool InLine { get; set; }

        public bool IsStatue => statue != null;

        public Position Position => new Position((int)Map, X, Y);

        public uint UID { get; set; }

        public int MaxHitPoints { get; set; }

        public int HitPoints
        {
            get
            {
                return Hit;
            }
            set
            {
                Hit = value;
            }
        }

        public ushort X { get; set; }

        public ushort Y { get; set; }

        public uint Map { get; set; }

        public uint DynamicID { get; set; }

        public bool Alive => HitPoints > 0;

        public MapObjectType ObjType { get; set; }

        public SobNpc(Statue _statue)
        {
            statue = _statue;
            BitVector = new StatusFlagsBigVector32(160);
        }

        public SobNpc()
        {
            AllowDynamic = false;
            BitVector = new StatusFlagsBigVector32(160);
        }

        public bool IsTrap()
        {
            return false;
        }

        public void RemoveRole(IMapObj obj)
        {
        }

        public void Send(byte[] packet)
        {
        }

        public void Send(Packet msg)
        {
        }

        public bool AddFlag(MsgUpdate.Flags Flag, int Seconds, bool RemoveOnDead, int StampSeconds = 0, uint showamount = 0u, uint amount = 0u)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Seconds, RemoveOnDead, StampSeconds);
                UpdateFlagScreen();
                return true;
            }
            return false;
        }

        public bool RemoveFlag(MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagScreen();
                return true;
            }
            return false;
        }

        public bool ContainFlag(MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }

        public void UpdateFlagScreen()
        {
            using (RecycledPacket rec = new RecycledPacket())
            {
                Packet stream;
                stream = rec.GetStream();
                MsgUpdate upd;
                upd = new MsgUpdate(stream, UID);
                stream = upd.Append(stream, MsgUpdate.DataType.StatusFlag, BitVector.bits);
                stream = upd.GetArray(stream);
                foreach (GameClient user in Server.GamePoll.Values)
                {
                    if (user.Player.Map == Map)
                        user.Send(stream);
                }
            }
        }

        public unsafe void Die(Packet stream, GameClient killer)
        {
           
            if (killer.OnAutoAttack && Map != 1002)
                killer.OnAutoAttack = false;
            if (IsStatue)
            {
                HitPoints = 0;
                Statue.RemoveStatue(stream, killer, UID, this);
                return;
            }
            if (UID == MsgSchedules.GuildWar.Furnitures[StaticMesh.RightGate].UID)
            {
                if (MsgSchedules.GuildWar.Winner != null && Guild.GuildPoll.TryGetValue(MsgSchedules.GuildWar.Winner.GuildID, out var guild2))
                    guild2.SendMessajGuild("[GuildWar] The right gate has been breached!");
                Mesh = StaticMesh.OpenRightGate;
                MsgUpdate upd2;
                upd2 = new MsgUpdate(stream, UID);
                stream = upd2.Append(stream, MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd2.GetArray(stream);
                foreach (GameClient client2 in Server.GamePoll.Values)
                {
                    if (client2.Player.Map == Map && Core.GetDistance(client2.Player.X, client2.Player.Y, X, Y) <= 19)
                        client2.Send(stream);
                }
            }
            else if (UID == MsgSchedules.GuildWar.Furnitures[StaticMesh.LeftGate].UID)
            {
                if (MsgSchedules.GuildWar.Winner != null && Guild.GuildPoll.TryGetValue(MsgSchedules.GuildWar.Winner.GuildID, out var guild))
                    guild.SendMessajGuild("[GuildWar] The left gate has been breached!");
                Mesh = StaticMesh.OpenLeftGate;
                MsgUpdate upd;
                upd = new MsgUpdate(stream, UID);
                stream = upd.Append(stream, MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd.GetArray(stream);
                foreach (GameClient client in Server.GamePoll.Values)
                {
                    if (client.Player.Map == Map && Core.GetDistance(client.Player.X, client.Player.Y, X, Y) <= 19)
                        client.Send(stream);
                }
            }
            else if (UID == MsgSchedules.ClanWar.Furnitures[StaticMesh.Pole].UID)
            {
                uint Damage4;
                Damage4 = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.ClanWar.UpdateScore(killer.Player, Damage4);
            }
            else if (UID == MsgSchedules.GuildWar.Furnitures[StaticMesh.Pole].UID)
            {
                uint Damage3;
                Damage3 = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.GuildWar.UpdateScore(killer.Player, Damage3);
            }
            //else if (UID == MsgSchedules.EliteGuildWar.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage2;
            //    Damage2 = (uint)HitPoints;
            //    if (HitPoints > 0)
            //        HitPoints = 0;
            //    MsgSchedules.EliteGuildWar.UpdateScore(killer.Player, Damage2);
            //}
            else if (UID == TheChosenProject.Game.MsgTournaments.MsgEliteGuildWar.PoleUID)
            {
                uint Damage2;
                Damage2 = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.EliteGuildWar.UpdateScore(killer.Player, Damage2);
            }
            else if (UID == TheChosenProject.Game.MsgTournaments.MsgScoresWar.PoleUID)
            {
                uint Damage2;
                Damage2 = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.ScoresWar.UpdateScore(killer.Player, Damage2);
            }
            //else if (UID == MsgSchedules.ScoresWar.Furnitures[StaticMesh.RightGate].UID)
            //{
            //    Mesh = StaticMesh.OpenRightGate;
            //    MsgUpdate upd2;
            //    upd2 = new MsgUpdate(stream, UID);
            //    stream = upd2.Append(stream, MsgUpdate.DataType.Mesh, (long)Mesh);
            //    stream = upd2.GetArray(stream);
            //    foreach (GameClient client2 in Server.GamePoll.Values)
            //    {
            //        if (client2.Player.Map == Map && Core.GetDistance(client2.Player.X, client2.Player.Y, X, Y) <= 19)
            //            client2.Send(stream);
            //    }
            //}
            else if (UID == 516977)
            {
                Mesh = StaticMesh.OpenRightGate;
                MsgUpdate upd2;
                upd2 = new MsgUpdate(stream, UID);
                stream = upd2.Append(stream, MsgUpdate.DataType.Mesh, (long)Mesh);
                stream = upd2.GetArray(stream);
                foreach (GameClient client2 in Server.GamePoll.Values)
                {
                    if (client2.Player.Map == Map && Core.GetDistance(client2.Player.X, client2.Player.Y, X, Y) <= 19)
                        client2.Send(stream);
                }
            }
            else if (UID == TheChosenProject.Game.MsgPoleDomination.PoleUID)
            {
                uint Damage;
                Damage = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.PoleDomination.KillPole(killer);
            }
            else if (MsgSchedules.CityWar.Bases.ContainsKey(UID))
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                MsgSchedules.CityWar.UpdateScore(killer.Player, this, 0);

            }
            else if (UID == TheChosenProject.Game.MsgSmallCityGuilWar.PoleUID)
            {
                uint Damage;
                Damage = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.SmallCityGuilWar.KillPole(killer);
            }
            else if (MsgSchedules.MsgCityPole != null &&
         MsgSchedules.MsgCityPole.Pole != null &&
         UID == MsgSchedules.MsgCityPole.Pole.UID)
            {
                uint Damage3;
                Damage3 = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;

                // Null checks to prevent NullReferenceException
                if (killer != null && killer.Player != null)
                {
                    MsgSchedules.MsgCityPole.UpdateScore(killer.Player, Damage3);

                    // Ensure the pole displays the guild's name
                    if (killer.Player.MyGuild != null)
                    {
                        MsgSchedules.MsgCityPole.UpdateScore(killer.Player, Damage3);
                    }
                }
                else
                {
                    // Log or handle the null reference case here
                    Console.WriteLine("Error: killer or killer.Player is null.");
                }
            }
            else if (UID == Game.MsgTournaments.MsgSchedules.CitywarTC.Furnitures[StaticMesh.Pole].UID)
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.CitywarTC.UpdateScore(killer.Player, Damage);
            }
            //else if (UID == Game.MsgTournaments.MsgSchedules.CitywarBI.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage3;
            //    Damage3 = (uint)HitPoints;
            //    if (HitPoints > 0)
            //        HitPoints = 0;
            //    MsgSchedules.CitywarBI.UpdateScore(killer.Player, Damage3);
            //}
            //else if (UID == Game.MsgTournaments.MsgSchedules.CitywarDC.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;
            //    if (HitPoints > 0)
            //    {
            //        HitPoints = 0;
            //    }
            //    Game.MsgTournaments.MsgSchedules.CitywarDC.UpdateScore(killer.Player, Damage);
            //}
            else if (UID == Game.MsgTournaments.MsgSchedules.CitywarPC.Furnitures[StaticMesh.Pole].UID)
            {
                uint Damage = (uint)HitPoints;
                if (HitPoints > 0)
                {
                    HitPoints = 0;
                }
                Game.MsgTournaments.MsgSchedules.CitywarPC.UpdateScore(killer.Player, Damage);
            }
            //else if (UID == Game.MsgTournaments.MsgSchedules.CitywarTC.Furnitures[StaticMesh.Pole].UID)
            //{
            //    uint Damage = (uint)HitPoints;
            //    if (HitPoints > 0)
            //    {
            //        HitPoints = 0;
            //    }
            //    Game.MsgTournaments.MsgSchedules.CitywarTC.UpdateScore(killer.Player, Damage);
            //}
            else if (UID == TheChosenProject.Game.MsgGuildDeathMatch.PoleUID)
            {
                uint Damage;
                Damage = (uint)HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.GuildDeathMatch.AddScore(Damage, killer.Player.MyGuild);
                MsgSchedules.GuildDeathMatch.KillPole();
            }
            else if (MsgSchedules.CaptureTheFlag.Bases.ContainsKey(UID))
            {
                _ = HitPoints;
                if (HitPoints > 0)
                    HitPoints = 0;
                MsgSchedules.CaptureTheFlag.UpdateFlagScore(killer.Player, this, 0u, stream);
            }
            else if (UID == 22340)
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == 22341)
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole1.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == 22342)
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole2.UpdateScore(stream, Damage, killer.Player);
            }
            else if (UID == 22343)
            {
                uint Damage = (uint)HitPoints;

                if (HitPoints > 0)
                    HitPoints = 0;

                Game.MsgTournaments.MsgNobilityPole3.UpdateScore(stream, Damage, killer.Player);
            }
        }

        public void SendString(Packet stream, MsgStringPacket.StringID id, params string[] args)
        {
            MsgStringPacket packet;
            packet = new MsgStringPacket
            {
                ID = id,
                UID = UID,
                Strings = args
            };
            SendScrennPacket(stream.StringPacketCreate(packet));
        }

        public void SendScrennPacket(Packet packet)
        {
            foreach (GameClient client in Server.GamePoll.Values)
            {
                if (client.Player.Map == Map && client.Player.GetMyDistance(X, Y) < 28)
                    client.Send(packet);
            }
        }

        public unsafe Packet GetArray(Packet stream, bool view)
        {
            if (statue != null)
            {
                if (statue.StatuePacket != null && statue.Static)
                {
                    stream.Seek(0);
                    fixed (byte* ptr2 = statue.StatuePacket)
                    {
                        stream.memcpy(stream.Memory, ptr2, statue.StatuePacket.Length);
                    }
                    stream.Size = statue.StatuePacket.Length;
                    return stream;
                }
                stream.InitWriter();
                Player Player;
                Player = statue.user.Player;
                stream.Write(Player.Mesh);
                stream.Write(statue.UID);
                if (statue.Static)
                {
                    stream.Write(0);
                    stream.Write((ushort)1000);
                }
                else
                {
                    stream.Write(Player.GuildID);
                    stream.Write((ushort)Player.GuildRank);
                }
                stream.ZeroFill(4);
                for (int x = 0; x < Player.BitVector.bits.Length; x++)
                {
                    stream.Write(0);
                }
                stream.Write((ushort)Player.AparenceType);
                stream.Write(Player.HeadId);
                stream.Write(Player.GarmentId);
                stream.Write(Player.ArmorId);
                stream.Write(Player.LeftWeaponId);
                stream.Write(Player.RightWeaponId);
                stream.Write(Player.LeftWeaponAccessoryId);
                stream.Write(Player.RightWeaponAccessoryId);
                stream.Write(Player.SteedId);
                stream.Write(Player.MountArmorId);
                stream.Write(0u);
                stream.Write(100);
                stream.Write(Player.Hair);
                stream.Write(X);
                stream.Write(Y);
                if (statue.Static)
                    stream.Write((byte)0);
                else
                    stream.Write((byte)statue.user.Player.Angle);
                if (statue.Static)
                    stream.Write((ushort)250);
                else
                    stream.Write((ushort)statue.Action);
                stream.Write((byte)0);
                stream.Write(0);
                stream.Write(Player.Reborn);
                stream.Write(Player.Level);
                stream.Write((byte)0);
                stream.Write(Player.Away);
                stream.Write(Player.ExtraBattlePower);
                stream.Write((ushort)0);
                stream.Write(0);
                stream.Write((ushort)0);
                stream.Write(Player.FlowerRank + 10000);
                stream.Write((uint)Player.NobilityRank);
                stream.Write(Player.ColorArmor);
                stream.Write(Player.ColorShield);
                stream.Write(Player.ColorHelment);
                stream.Write(Player.QuizPoints);
                stream.Write(Player.SteedPlus);
                stream.Write((ushort)0);
                stream.Write(Player.SteedColor);
                stream.Write((byte)Player.Enilghten);
                stream.Write((ushort)0);
                stream.Write(0);
                stream.Write(0);
                stream.Write((byte)0);
                stream.Write(Player.ClanUID);
                stream.Write((uint)Player.ClanRank);
                stream.Write(0u);
                stream.Write((ushort)Player.MyTitle);
                stream.Write(0u);
                stream.Write(0u);
                stream.Write(0u);
                stream.Write((byte)0);
                stream.Write(Player.HeadSoul);
                stream.Write(Player.ArmorSoul);
                stream.Write(Player.LeftWeapsonSoul);
                stream.Write(Player.RightWeapsonSoul);
                stream.Write(0);
                stream.Write(0);
                stream.ZeroFill(1);
                stream.Write((ushort)Player.FirstClass);
                stream.Write((ushort)Player.SecondClass);
                stream.Write((ushort)Player.Class);
                stream.Write(0);
                stream.ZeroFill(1);
                stream.Write(Player.Name, string.Empty, Player.ClanName);
                stream.Finalize(10014);
                if (statue.StatuePacket == null && statue.Static)
                {
                    statue.StatuePacket = new byte[stream.Size];
                    int size;
                    size = stream.Size;
                    fixed (byte* ptr = statue.StatuePacket)
                    {
                        stream.memcpy(ptr, stream.Memory, size);
                    }
                }
                return stream;
            }
            stream.InitWriter();
            stream.Write(UID);
            stream.Write(0);
            stream.Write(MaxHitPoints);
            stream.Write(HitPoints);
            stream.Write(X);
            stream.Write(Y);
            stream.Write((ushort)Mesh);
            stream.Write((ushort)Type);
            stream.Write(Sort);
            if (Name != "" && Name != null)
            {
                if (Name.Length > 16)
                    Name = Name.Substring(0, 16);
                stream.Write(Name);
            }
            stream.Finalize(1109);
            return stream;
        }
    }
}
