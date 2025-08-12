using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheChosenProject.Role
{
    internal class MyMath
    {
        public const int NORMAL_RANGE = 17;

        public const int BIG_RANGE = 34;

        public const int USERDROP_RANGE = 9;

        public static bool ChanceSuccess(int value)
        {
            if (value <= 0)
                return false;
            return value >= Generate(1, 120);
        }

        public static bool Success(double Chance)
        {
            return (double)Generate(1, 1000000) / 10000.0 >= 100.0 - Chance;
        }

        public static Int32 Generate(Int32 Min, Int32 Max)
        {
            if (Max != Int32.MaxValue)
                Max++;

            Int32 Value = 0;
            /*lock (Rand) { */
            Value = Role.Core.Random.Next(Min, Max); /*}*/
            return Value;
        }

        public static Int32 Generate(UInt32 Min, UInt32 Max)
        {
            if (Max != Int32.MaxValue)
                Max++;

            Int32 Value = 0;
            /*lock (Rand) { */
            Value = Role.Core.Random.Next((int)Min, (int)Max); /*}*/
            return Value;
        }
    }
}
