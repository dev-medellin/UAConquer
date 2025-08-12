using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheChosenProject.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public enum Action : ushort
        {
            WeaponSkill = 0,
            Magic = 1,
            Skill = 2,
        };


        //writer.Write(Identity);
        //writer.Write(Level);
        //writer.Write(Experience);
        //writer.Write(LevelExperience);//UID);
        public static readonly uint[] RequiredExperience = new uint[21]
{
          0,
          1200,
          68000,
          250000,
          640000,
          1600000,
          4000000,
          10000000,
          22000000,
          40000000,
          90000000,
          95000000,
          142500000,
          213750000,
          320625000,
          480937500,
          721406250,
          1082109375,
          1623164063,
          2100000000,
          0
};
        public static unsafe ServerSockets.Packet UpdateProfExperienceCreate(this ServerSockets.Packet stream, uint Experience, uint UID, uint ID)
        {
            stream.InitWriter();

            stream.Write(Experience);
            stream.Write(UID);
            stream.Write(ID);
            stream.ZeroFill(8);//unknow

            stream.Finalize(GamePackets.UpgradeSpellExperience);
            return stream;
        }
    }
}
