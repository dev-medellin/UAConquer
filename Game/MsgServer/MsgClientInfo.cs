using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using TheChosenProject.Client;
using TheChosenProject.Role;
using TheChosenProject.Role.Instance;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetHeroInfo(this Packet stream, GameClient Owner, out Player user)
        {
            user = new Player(Owner)
            {
                InitTransfer = stream.ReadUInt32(),
                RealUID = stream.ReadUInt32(),
                AparenceType = stream.ReadUInt16()
            };
            uint mesh;
            mesh = stream.ReadUInt32();
            user.Body = (ushort)(mesh % 10000);
            user.Face = (ushort)((mesh - user.Body) / 10000);
            user.Hair = stream.ReadUInt16();
            user.Money = (int)stream.ReadUInt32();
            user.ConquerPoints = (int)stream.ReadUInt32();
            user.Experience = stream.ReadUInt64();
            user.ServerID = stream.ReadUInt16();
            user.SetLocationType = stream.ReadUInt16();
            user.VirtutePoints = stream.ReadUInt32();
            user.HeavenBlessing = stream.ReadInt32();
            user.Strength = stream.ReadUInt16();
            user.Agility = stream.ReadUInt16();
            user.Vitality = stream.ReadUInt16();
            user.Spirit = stream.ReadUInt16();
            user.Atributes = stream.ReadUInt16();
            user.HitPoints = stream.ReadInt32();
            user.Mana = stream.ReadUInt16();
            user.PKPoints = stream.ReadUInt16();
            user.Level = stream.ReadUInt8();
            user.Class = stream.ReadUInt8();
            user.FirstClass = stream.ReadUInt8();
            user.SecondClass = stream.ReadUInt8();
            user.NobilityRank = (Nobility.NobilityRank)stream.ReadUInt8();
            user.Reborn = stream.ReadUInt8();
            stream.SeekForward(1);
            user.QuizPoints = stream.ReadUInt32();
            stream.SeekForward(4);
            user.Enilghten = stream.ReadUInt16();
            user.EnlightenReceive = (ushort)((int)stream.ReadUInt16() / 100);
            stream.SeekForward(4);
            user.VipLevel = (byte)stream.ReadUInt32();
            user.MyTitle = (byte)stream.ReadUInt16();
            user.OnlinePoints = stream.ReadInt32();
            stream.SeekForward(1);
            stream.SeekForward(4);
            stream.SeekForward(8);
            stream.SeekForward(2);
            stream.SeekForward(4);
            stream.SeekForward(4);
            stream.SeekForward(4);
            string[] strs;
            strs = stream.ReadStringList();
            user.Name = strs[0];
            user.Spouse = strs[1];
        }


        public static Packet HeroInfo(this Packet stream, Player client, int inittransfer = 0)
        {
            stream.InitWriter();
            stream.Write(client.UID);
            stream.Write((ushort)client.AparenceType);
            stream.Write(client.Mesh);
            stream.Write(client.Hair);
            stream.Write(client.Money);
            stream.Write(client.ConquerPoints);
            stream.Write(client.Experience);
            stream.Write(client.SetLocationType);
            stream.Write(0);
            stream.Write(0);
            stream.Write((ushort)0);
            stream.Write(client.VirtutePoints);
            stream.Write(client.HeavenBlessing);
            stream.Write(client.Strength);
            stream.Write(client.Agility);
            stream.Write(client.Vitality);
            stream.Write(client.Spirit);
            stream.Write(client.Atributes);
            stream.Write((ushort)client.HitPoints);
            stream.Write(client.Mana);
            stream.Write(client.PKPoints);
            stream.Write((byte)client.Level);
            stream.Write(client.Class);
            stream.Write(client.FirstClass);
            stream.Write(client.SecondClass);
            stream.Write((byte)client.NobilityRank);
            stream.Write(client.Reborn);
            stream.Write((byte)0);
            stream.Write(client.QuizPoints);
            stream.Write(client.Enilghten);
            stream.Write((ushort)(client.EnlightenReceive * 100));
            stream.Write(0);
            stream.Write((uint)client.VipLevel);
            stream.Write((ushort)client.MyTitle);
            stream.Write(client.OnlinePoints);
            if (client.SubClass != null)
            {
                stream.Write((byte)client.ActiveSublass);
                stream.Write(client.SubClass.GetHashPoint());
            }
            else
                stream.ZeroFill(5);
            stream.Write(0);
            stream.Write((ushort)client.ChampionPoints);
            stream.ZeroFill(2);
            stream.Write((byte)3);
            stream.WriteStringWithLength(client.Name);
            stream.ZeroFill(1);
            stream.WriteStringWithLength(client.Spouse);
            stream.Finalize(1006);
            return stream;
        }

    }

}
