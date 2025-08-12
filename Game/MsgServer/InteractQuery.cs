using System.Runtime.InteropServices;



namespace TheChosenProject.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct InteractQuery
    {
        public static InteractQuery ShallowCopy(InteractQuery item)
        {
            return (InteractQuery)item.MemberwiseClone();
        }

        public int TimeStamp;
        public uint UID;
        public uint OpponentUID;
        public ushort X;
        public ushort Y;
        public MsgAttackPacket.AttackID AtkType;//24
        public ushort SpellID;//28
        public bool KilledMonster
        {
            get { return (SpellID == 1); }
            set { SpellID = (ushort)(value ? 1 : 0); }
        }
        public ushort SpellLevel;//30
        public int Data
        {
            get { fixed (void* ptr = &X) { return *((int*)ptr); } }
            set { fixed (void* ptr = &X) { *((int*)ptr) = value; } }
        }
        public int dwParam
        {
            get { fixed (void* ptr = &SpellLevel) { return *((int*)ptr); } }
            set { fixed (void* ptr = &SpellLevel) { *((int*)ptr) = value; } }
        }
        public uint KillCounter
        {
            get { return SpellLevel; }
            set { SpellLevel = (ushort)value; }
        }
        public int Damage
        {
            get { fixed (void* ptr = &SpellID) { return *((int*)ptr); } }
            set { fixed (void* ptr = &SpellID) { *((int*)ptr) = value; } }
        }
        public bool OnCounterKill
        {

            get { return Damage != 0; }
            set { Damage = value ? 1 : 0; }
        }
        public uint ResponseDamage;//32
        public MsgAttackPacket.AttackEffect Effect;//36
        public uint Unknow;
    }

}
