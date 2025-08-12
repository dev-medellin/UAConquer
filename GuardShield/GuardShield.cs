using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TheChosenProject.Client;
using TheChosenProject.Cryptography;
using TheChosenProject.Database;
using TheChosenProject.Game.MsgServer;
using TheChosenProject.Role;

namespace CMsgGuardShield
{
    public class GuardShield
    {
        private GameClient client = null;
        private DateTime PingStamp = new DateTime();
        private DateTime PingPacketStamp = new DateTime();
        private DateTime TaskProcess = new DateTime();

        private uint PingValue = 0;
        private uint PingValue_2 = 0;
        private int PrivOwnerSeed = 0;
        public GuardShield(GameClient pclient)
        {
            client = pclient;
            PingStamp = DateTime.Now;
            PingPacketStamp = DateTime.Now;
            TaskProcess = DateTime.Now;
            PrivOwnerSeed = new Random().Next() ^ 0x514E;
            PingValue = (uint)new Random().Next(1, 0x1000);
            PingValue_2 = (uint)new Random((int)this.PingValue).Next(1, 0x1000);
            _CreateConnect();
        }
        public enum DataPacket : ushort
        {
            CreateLogin = 100,
            CreatePing = 101,

            OtherWindow = 200,
            Aimbot = 201,
        }

        public unsafe void HandleOwner(byte[] data)
        {
            var srand = new srand(PrivOwnerSeed);
            byte[] Key1 = new byte[128];
            byte[] Key2 = new byte[128];

            //key 1
            for (int i = 0; i < Key1.Length; i++)
            {
                Key1[i] = (byte)srand.rand();
                Key1[i] = (byte)(i ^ Key1[i & 0x5]);
            }
            //key 2
            for (int i = 0; i < Key2.Length; i++)
            {
                Key2[i] = (byte)srand.rand();
                Key2[i] = (byte)(i ^ Key2[i & 0x8]);
            }
            //handle
            for (int x = 4; x < data.Length; x++)
            {
                data[x] = (byte)((byte)(Key1[44 * x & 28] ^ data[x]));
                data[x] = (byte)((byte)(Key2[99 * x & 31] ^ data[x]));
            }
        }

        public static byte[] CreateDataPacket(DataPacket type, params int[] ints)
        {
            byte[] buffer = new byte[28 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                    *((ushort*)(Buffer + 2)) = (ushort)0x3F1;

                    for (int i = 0; i < Math.Min(ints.Length, 2); i++)
                        *((int*)(Buffer + (i + 1) * 4)) = ints[i];

                    *((ushort*)(Buffer + 12)) = (ushort)type;
                }
            }
            return buffer;
        }
        public void _CreateConnect()
        {
            client.Send(CreateDataPacket(DataPacket.CreateLogin, new int[] { MsgGuardShield.GCipher.PublicSeed, PrivOwnerSeed }));
        }

        public static void EncodePing(uint idUser, ref uint data1, ref uint data2/*, ref ushort usPosX, ref ushort usPosY*/)
        {
            data1 = (ExchangeShortBits((uint)(data1 - 0x12EE), 3) ^ (idUser) ^ 0x615D);
            data2 = (uint)ExchangeLongBits(((uint)(data2 - 0x6EE65841) ^ (idUser) ^ 0x5F2D2463), 32 - 13);
        }

        public unsafe void DatePacketHandle(ushort type, TheChosenProject.ServerSockets.Packet stream)
        {
            stream.Seek(4);
            var data_1 = stream.ReadInt32();//4
            var data_2 = stream.ReadInt32();//8

            var data_3 = stream.ReadInt32();//12
            var data_4 = stream.ReadInt32();//16
            switch ((DataPacket)type)
            {
                case DataPacket.CreatePing:
                    {
                        if ((data_1 != PingValue || data_2 != PingValue_2) && !Debugger.IsAttached)
                        {
                            Disconnect("PING ERROR");
                        }
                        else
                        {
                            PingValue = (uint)new Random().Next(1, 0x1000);
                            PingValue_2 = (uint)new Random((int)PingValue).Next(1, 0x1000);
                            PingPacketStamp = DateTime.Now;
                        }
                        break;
                    }
                case DataPacket.OtherWindow:
                    {
                        Console.WriteLine(string.Format("(OtherWindow) PLAYER > {0} USE {1} FOR ATTACK TO UID {2}", client.Player.Name, data_1, data_2));
                        Disconnect("OtherWindow");
                        break;
                    }
                case DataPacket.Aimbot:
                    {
                        Console.WriteLine(string.Format("(Aimbot) PLAYER > {0} USE {1} FOR ATTACK TO UID {2}", client.Player.Name, data_1, data_2));
                        Disconnect("Aimbot");
                        break;
                    }
                default:
                    break;
            }
        }
        public void _OnThread()
        {
            DateTime Now = DateTime.Now;
            if (Now > PingStamp.AddSeconds(45))
            {
                PingStamp = DateTime.Now;
                //TODO
                var _Ping = (uint)PingValue;
                var _Ping2 = (uint)PingValue_2;
                EncodePing((uint)PrivOwnerSeed, ref _Ping, ref _Ping2);
                client.Send(CreateDataPacket(DataPacket.CreatePing, new int[] { (int)_Ping, (int)_Ping2 }));

            }
            if (!Debugger.IsAttached && Now > PingPacketStamp.AddMinutes(10))
            {
				PingPacketStamp = DateTime.Now;
                Disconnect("PingPacketStamp");
            }
            if (Now > TaskProcess.AddMinutes(5))
            {
                TaskProcess = DateTime.Now;
                //TODO

            }
        }
        internal void Disconnect(string reason = "")
        {
            Console.WriteLine("[GuardShield] {0} KICKOUT {1}", client.Player.Name, reason);
            client.Socket.Disconnect();
        }

        internal void TerminateLoader(string MessageCaption = "", string MessageText = "")
        {
            var Packet = MsgGuardShield.TerminateLoader(MessageCaption, MessageText);
            client.Guard.HandleOwner(Packet);
            if (MsgGuardShield.GCipher.HandleBuffer(ref Packet, false) == 0)
            {
                client.Socket.Disconnect();
                return;
            }
            client.Send(Packet);
        }

        internal void RequestMachineInfo()
        {
            var Packet = MsgGuardShield.RequestMachineInfo();
            client.Guard.HandleOwner(Packet);
            if (MsgGuardShield.GCipher.HandleBuffer(ref Packet, false) == 0)
            {
                client.Socket.Disconnect();
                return;
            }
            client.Send(Packet);
        }
        internal void RequestOpenedProcesses()
        {
            var Packet = MsgGuardShield.RequestOpenedProcesses();
            client.Guard.HandleOwner(Packet);
            if (MsgGuardShield.GCipher.HandleBuffer(ref Packet, false) == 0)
            {
                client.Socket.Disconnect();
                return;
            }
            client.Send(Packet);
        }

        internal void PingStatuesLoader(string PingStatuesMsg = "")
        {
            var Packet = MsgGuardShield.PingStatuesLoader(PingStatuesMsg);
            client.Guard.HandleOwner(Packet);
            if (MsgGuardShield.GCipher.HandleBuffer(ref Packet, false) == 0)
            {
                client.Socket.Disconnect();
                return;
            }
            client.Send(Packet);
        }
        internal void CommandToClient(string command)
        {
            var Packet = MsgGuardShield.CommandToClient(command);
            client.Guard.HandleOwner(Packet);
            if (MsgGuardShield.GCipher.HandleBuffer(ref Packet, false) == 0)
            {
                client.Socket.Disconnect();
                return;
            }
            client.Send(Packet);
        }
        internal void TaskJump(MsgGuardShield.JumpType type, byte Value)
        {
            var Packet = MsgGuardShield.RequestJumpInfo(type, Value);
            client.Guard.HandleOwner(Packet);
            if (MsgGuardShield.GCipher.HandleBuffer(ref Packet, false) == 0)
            {
                client.Socket.Disconnect();
                return;
            }
            client.Send(Packet);
        }


        public static int BitFold32(int lower16, int higher16)
        {
            return (lower16) | (higher16 << 16);
        }
        public static void BitUnfold32(int bits32, out int lower16, out int upper16)
        {
            lower16 = (int)(bits32 & UInt16.MaxValue);
            upper16 = (int)(bits32 >> 16);
        }
        public static void BitUnfold64(ulong bits64, out int lower32, out int upper32)
        {
            lower32 = (int)(bits64 & UInt32.MaxValue);
            upper32 = (int)(bits64 >> 32);
        }
        private static uint ExchangeShortBits(uint data, int bits)
        {
            data &= 0xffff;
            return ((data >> bits) | (data << (16 - bits))) & 0xffff;
        }

        public static uint ExchangeLongBits(uint data, int bits)
        {
            return (data >> bits) | (data << (32 - bits));
        }

    }
}
