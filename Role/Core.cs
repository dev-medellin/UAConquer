using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheChosenProject.Game.MsgServer;
using Extensions;
using TheChosenProject.Game.MsgServer.AttackHandler.Calculate;
using System.Threading;
using TheChosenProject.ServerSockets;

namespace TheChosenProject.Role
{
    public static class Core
    {
        public static Random nRandom = new Random();

        private static int m_nTimes = 0;

        public static FastRandom Random = new FastRandom();
        public static string WriteNumber(int value)
        {
            // n0 because there will never be decimal places for an integer.
            return string.Format("{0:n0}", value);
        }
        public static void SendGlobalMessage(Packet stream, string Message, MsgMessage.ChatMode type = MsgMessage.ChatMode.System, MsgMessage.MsgColor color = MsgMessage.MsgColor.red)
        {
            Program.SendGlobalPackets.Enqueue(new MsgMessage(Message, MsgMessage.MsgColor.red, MsgMessage.ChatMode.Talk).GetArray(stream));
        }

        public static int RandFromGivingNums(params int[] nums)
        {
            return nums[ServerKernel.NextAsync(0, nums.Length)];
        }

        public static bool IsBoy(uint mesh)
        {
            if (mesh != 1003)
                return mesh == 1004;
            return true;
        }

        public static bool IsGirl(uint mesh)
        {
            if (mesh != 2001)
                return mesh == 2002;
            return true;
        }
        public static bool PercentSuccess(double _chance)//bahaa
        {
            return Program.DropRandom.NextDouble() * 100 < _chance;
        }
        public static bool ChanceSuccess(double Chance)
        {
            return Program.DropRandom.NextDouble() < (Chance / 100.0);
        }
        internal static int CreateTimer(int year, int month, int day)
        {
            return year * 10000 + month * 100 + day;
        }

        internal static int CreateTimer(DateTime timer)
        {
            return timer.Year * 10000 + timer.Month * 100 + timer.Day;
        }

        internal static DateTime GetTimer(int Timer)
        {
            int Year;
            Year = Timer / 10000;
            int Month;
            Month = Timer / 100 - Year * 100;
            int Day;
            Day = Timer - Year * 10000 - Month * 100;
            return new DateTime(Year, Month, Day);
        }

        public static bool ChanceCalc(float value)
        {
            try
            {
                if (value > 100f)
                    value = 100f;
                if (m_nTimes > 1000000)
                {
                    nRandom = new Random();
                    m_nTimes = 0;
                }
                Interlocked.Increment(ref m_nTimes);
                value *= 100f;
                int res;
                res = nRandom.Next(1, 10000);
                return (float)res <= value;
            }
            catch
            {
                return false;
            }
        }

        internal static ulong TqTimer(DateTime timer)
        {
            ulong year;
            year = (ulong)(10000000000000L * (timer.Year - 1900));
            ulong month;
            month = (ulong)(100000000000L * (timer.Month - 1));
            ulong dayofyear;
            dayofyear = 100000000uL * (ulong)(timer.DayOfYear - 1);
            ulong day;
            day = (ulong)(timer.Day * 1000000);
            ulong Hour;
            Hour = (ulong)(timer.Hour * 10000);
            ulong Minute;
            Minute = (ulong)(timer.Minute * 100);
            ulong Second;
            Second = (ulong)timer.Second;
            return year + month + dayofyear + day + Hour + Minute + Second;
        }

        public static bool Rate(double percent)
        {
            ushort testone;
            testone = 0;
            if (percent == 0.0)
                return false;
            while ((int)percent > 0)
            {
                testone = (ushort)(testone + 1);
                percent /= 10.0;
                if (testone > 300)
                {
                    ServerKernel.Log.SaveLog("Problem While in Kernel", true, LogType.WARNING);
                    return true;
                }
            }
            int discriminant;
            discriminant = 1;
            percent = Math.Round(percent, 4);
            testone = 0;
            while (percent != Math.Ceiling(percent))
            {
                percent *= 10.0;
                discriminant *= 10;
                percent = Math.Round(percent, 4);
                testone = (ushort)(testone + 1);
                if (testone > 300)
                {
                    ServerKernel.Log.SaveLog("Problem While in Kernel 2", true, LogType.WARNING);
                    return true;
                }
            }
            return Rate((int)percent, discriminant);
        }

        public static bool Rate(int value, int discriminant)
        {
            int rate;
            rate = Program.GetRandom.Next() % discriminant;
            return value > rate;
        }

        public static int GetJumpMiliSeconds(short Distance)
        {
            return Distance * 25;
        }

        public static void IncXY(Flags.ConquerAngle Facing, ref ushort x, ref ushort y)
        {
            sbyte xi;
            sbyte yi;
            xi = (yi = 0);
            switch (Facing)
            {
                case Flags.ConquerAngle.North:
                    xi = -1;
                    yi = -1;
                    break;
                case Flags.ConquerAngle.South:
                    xi = 1;
                    yi = 1;
                    break;
                case Flags.ConquerAngle.East:
                    xi = 1;
                    yi = -1;
                    break;
                case Flags.ConquerAngle.West:
                    xi = -1;
                    yi = 1;
                    break;
                case Flags.ConquerAngle.NorthWest:
                    xi = -1;
                    break;
                case Flags.ConquerAngle.SouthWest:
                    yi = 1;
                    break;
                case Flags.ConquerAngle.NorthEast:
                    yi = -1;
                    break;
                case Flags.ConquerAngle.SouthEast:
                    xi = 1;
                    break;
            }
            x = (ushort)(x + xi);
            y = (ushort)(y + yi);
        }

        public static short GetDistance(ushort x1, ushort y1, ushort x2, ushort y2)
        {
            return (short)Math.Round(Math.Sqrt((double)((x1 - x2) * (x1 - x2)) + (double)((y1 - y2) * (y1 - y2)) * 1.0));
        }

        public static short GetAiDistance(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            return (short)Math.Sqrt((X - X2) * (X - X2) + (Y - Y2) * (Y - Y2));
        }

        public static double GetRadian(float posSourX, float posSourY, float posTargetX, float posTargetY)
        {
            float PI;
            PI = (float)Math.PI;
            float fDeltaX;
            fDeltaX = posTargetX - posSourX;
            float fDeltaY;
            fDeltaY = posTargetY - posSourY;
            float fDistance;
            fDistance = SquareRootFloat(fDeltaX * fDeltaX + fDeltaY * fDeltaY);
            double fRadian;
            fRadian = Math.Asin(fDeltaX / fDistance);
            if (!(fDeltaY > 0f))
                return (double)PI + fRadian + (double)(PI / 2f);
            return (double)(PI / 2f) - fRadian;
        }

        private unsafe static float SquareRootFloat(float number)
        {
            float x;
            x = number * 0.5f;
            float y;
            y = number;
            long i;
            i = *(long*)(&y);
            i = 1597463007 - (i >> 1);
            y = *(float*)(&i);
            y *= 1.5f - x * y * y;
            y *= 1.5f - x * y * y;
            return number * y;
        }

        public static Flags.ConquerAngle GetAngle(ushort X, ushort Y, ushort X2, ushort Y2)
        {
            double direction;
            direction = 0.0;
            double AddX;
            AddX = X2 - X;
            double AddY;
            AddY = Y2 - Y;
            double r;
            r = Math.Atan2(AddY, AddX);
            if (r < 0.0)
                r += Math.PI * 2.0;
            direction = 360.0 - r * 180.0 / Math.PI;
            byte Dir;
            Dir = (byte)(7.0 - Math.Floor(direction) / 45.0 % 8.0 - 1.0);
            return (Flags.ConquerAngle)((int)Dir % 8);
        }

        public static bool Rate(int value)
        {
            return value > Program.GetRandom.Next() % 100;
        }

        public unsafe static void memcpy(void* dest, void* src, int size)
        {
            int count;
            count = size / 8;
            for (int j = 0; j < count; j++)
            {
                *(long*)((byte*)dest + (long)j * 8L) = *(long*)((byte*)src + (long)j * 8L);
            }
            int pos;
            pos = size - size % 8;
            for (int i = 0; i < size % 8; i++)
            {
                ((byte*)dest + pos)[i] = ((byte*)src + pos)[i];
            }
        }

        public static int i32Direction(int x1, int y1, int x2, int y2)
        {
            int nx;
            nx = x1 - x2;
            int ny;
            ny = y1 - y2;
            if (ny == 0)
            {
                if (nx <= 0)
                    return 6;
                if (ny >= 0)
                    return 2;
            }
            else if (nx == 0)
            {
                if (ny <= 0)
                    return 0;
                if (ny >= 0)
                    return 4;
            }
            else
            {
                if (nx < 0)
                {
                    if (ny < 0)
                        return 7;
                    return 5;
                }
                if (nx > 0)
                {
                    if (ny > 0)
                        return 3;
                    return 1;
                }
            }
            return 0;
        }
    }
    public class FastRandom
    {
        private object object_0;
        private uint uint_0;
        private uint uint_1;
        private uint uint_2;
        private uint uint_3;

        public FastRandom() : this(Time32.Now.AllMilliseconds)
        {
            // Class1.Class0.smethod_0();
        }

        public FastRandom(int seed)
        {
            this.object_0 = new object();
            this.Reinitialise(seed);
        }

        public int Next()
        {
            lock (this.object_0)
            {
                uint num = this.uint_0 ^ (this.uint_0 << 11);
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                this.uint_3 = (this.uint_3 ^ (this.uint_3 >> 0x13)) ^ (num ^ (num >> 8));
                uint num2 = this.uint_3 & 0x7fffffff;
                if (num2 == 0x7fffffff)
                {
                    return this.Next();
                }
                return (int)num2;
            }
        }

        public int Next(int upperBound)
        {
            lock (this.object_0)
            {
                if (upperBound < 0)
                {
                    upperBound = 0;
                }
                uint num = this.uint_0 ^ (this.uint_0 << 11);
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return (int)((4.6566128730773926E-10 * (0x7fffffff & (this.uint_3 = (this.uint_3 ^ (this.uint_3 >> 0x13)) ^ (num ^ (num >> 8))))) * upperBound);
            }
        }

        public int Next(int lowerBound, int upperBound)
        {
            lock (this.object_0)
            {
                if (lowerBound > upperBound)
                {
                    int num = lowerBound;
                    lowerBound = upperBound;
                    upperBound = num;
                }
                uint num2 = this.uint_0 ^ (this.uint_0 << 11);
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                int num3 = upperBound - lowerBound;
                if (num3 < 0)
                {
                    return (lowerBound + ((int)((2.3283064365386963E-10 * (this.uint_3 = (this.uint_3 ^ (this.uint_3 >> 0x13)) ^ (num2 ^ (num2 >> 8)))) * (upperBound - lowerBound))));
                }
                return (lowerBound + ((int)((4.6566128730773926E-10 * (0x7fffffff & (this.uint_3 = (this.uint_3 ^ (this.uint_3 >> 0x13)) ^ (num2 ^ (num2 >> 8))))) * num3)));
            }
        }

        public unsafe void NextBytes(byte[] buffer)
        {
            lock (this.object_0)
            {
                if ((buffer.Length % 8) != 0)
                {
                    new Random().NextBytes(buffer);
                }
                else
                {
                    uint num = this.uint_0;
                    uint num2 = this.uint_1;
                    uint num3 = this.uint_2;
                    uint num4 = this.uint_3;
                    fixed (byte* numRef = buffer)
                    {
                        uint* numPtr = (uint*)numRef;
                        int index = 0;
                        int num6 = buffer.Length >> 2;
                        while (index < num6)
                        {
                            uint num7 = num ^ (num << 11);
                            num = num2;
                            num2 = num3;
                            num3 = num4;
                            numPtr[index] = num4 = (num4 ^ (num4 >> 0x13)) ^ (num7 ^ (num7 >> 8));
                            num7 = num ^ (num << 11);
                            num = num2;
                            num2 = num3;
                            num3 = num4;
                            numPtr[index + 1] = num4 = (num4 ^ (num4 >> 0x13)) ^ (num7 ^ (num7 >> 8));
                            index += 2;
                        }
                    }
                }
            }
        }

        public double NextDouble()
        {
            lock (this.object_0)
            {
                uint num = this.uint_0 ^ (this.uint_0 << 11);
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return (4.6566128730773926E-10 * (0x7fffffff & (this.uint_3 = (this.uint_3 ^ (this.uint_3 >> 0x13)) ^ (num ^ (num >> 8)))));
            }
        }

        public int NextInt()
        {
            lock (this.object_0)
            {
                uint num = this.uint_0 ^ (this.uint_0 << 11);
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return (0x7fffffff & ((int)(this.uint_3 = (this.uint_3 ^ (this.uint_3 >> 0x13)) ^ (num ^ (num >> 8)))));
            }
        }

        public uint NextUInt()
        {
            lock (this.object_0)
            {
                uint num = this.uint_0 ^ (this.uint_0 << 11);
                this.uint_0 = this.uint_1;
                this.uint_1 = this.uint_2;
                this.uint_2 = this.uint_3;
                return (this.uint_3 = (this.uint_3 ^ (this.uint_3 >> 0x13)) ^ (num ^ (num >> 8)));
            }
        }

        public void Reinitialise(int seed)
        {
            lock (this.object_0)
            {
                this.uint_0 = (uint)seed;
                this.uint_1 = 0x32378fc7;
                this.uint_2 = 0xd55f8767;
                this.uint_3 = 0x104aa1ad;
            }
        }

        public int Sign()
        {
            if (this.Next(0, 2) == 0)
            {
                return -1;
            }
            return 1;
        }
    }
}
