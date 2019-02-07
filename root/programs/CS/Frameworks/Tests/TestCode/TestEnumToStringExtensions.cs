using System;

using Touryo.Infrastructure.Public.Dbg;
using Touryo.Infrastructure.Public.FastReflection;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestEnumToStringExtensions
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            TestEnumToStringExtensions.BasicEnumTest();
            WriteLine.OutPutDebugAndConsole("--------------------------------------------------");
            TestEnumToStringExtensions.BaseIsNotZeroEnumTest();
            WriteLine.OutPutDebugAndConsole("--------------------------------------------------");
            //TestEnumToStringExtensions.EmptyValueTest();
            //WriteLine.OutPutDebugAndConsole("--------------------------------------------------");
            TestEnumToStringExtensions.ByteEnumTest();
            WriteLine.OutPutDebugAndConsole("--------------------------------------------------");
            TestEnumToStringExtensions.FlagEnumTest();
        }
        #endregion

        #region private

        #region Enum
        /// <summary>BaseIsNotZeroEnum</summary>
        public enum BaseIsNotZeroEnum : byte
        {
            V2 = 2,
            V3,
            V4,
            V5,
            V6,
        }
        /// <summary>ByteEnum</summary>
        public enum ByteEnum : byte
        {
            V0 = 0,
            V2 = 2,
            V4 = 4,
        }
        /// <summary>LongEnum</summary>
        public enum LongEnum : long
        {
            V0 = 0,
            V2 = 2,
            V1620100 = 1620100,
            V23372036854775807 = 23372036854775807,
        }
        /// <summary>FlagsEnum</summary>
        [FlagsAttribute]
        public enum FlagsEnum
        {
            None = 0x00, // 0000 0000
            One = 0x01, // 0000 0001
            Two = 0x02, // 0000 0010 
            Three = 0x04, // 0000 0100
            Four = 0x08, // 0000 1000
            All = 0x0F // 0000 1111
        }
        #endregion

        #region Method
        /// <summary>BasicEnumTest</summary>
        private static void BasicEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("Friday", DayOfWeek.Friday.ToString1());
            WriteLine.OutPutDebugAndConsole("Wednesday", DayOfWeek.Wednesday.ToString1());
            WriteLine.OutPutDebugAndConsole("Saturday", DayOfWeek.Saturday.ToString1());
            WriteLine.OutPutDebugAndConsole("Friday", ((DayOfWeek)(5)).ToString1());
            WriteLine.OutPutDebugAndConsole("Saturday", ((DayOfWeek)(6)).ToString1());
#if NETCORE
            WriteLine.OutPutDebugAndConsole("-------------------------");
            WriteLine.OutPutDebugAndConsole("Friday", DayOfWeek.Friday.ToString2());
            WriteLine.OutPutDebugAndConsole("Wednesday", DayOfWeek.Wednesday.ToString2());
            WriteLine.OutPutDebugAndConsole("Saturday", DayOfWeek.Saturday.ToString2());
            WriteLine.OutPutDebugAndConsole("Friday", ((DayOfWeek)(5)).ToString2());
            WriteLine.OutPutDebugAndConsole("Saturday", ((DayOfWeek)(6)).ToString2());
#endif
        }
        /// <summary>BaseIsNotZeroEnumTest</summary>
        private static void BaseIsNotZeroEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("V2", BaseIsNotZeroEnum.V2.ToString1());
            WriteLine.OutPutDebugAndConsole("V3", BaseIsNotZeroEnum.V3.ToString1());
            WriteLine.OutPutDebugAndConsole("V4", BaseIsNotZeroEnum.V4.ToString1());
#if NETCORE
            WriteLine.OutPutDebugAndConsole("-------------------------");
            WriteLine.OutPutDebugAndConsole("V2", BaseIsNotZeroEnum.V2.ToString2());
            WriteLine.OutPutDebugAndConsole("V3", BaseIsNotZeroEnum.V3.ToString2());
            WriteLine.OutPutDebugAndConsole("V4", BaseIsNotZeroEnum.V4.ToString2());
#endif
        }
        ///// <summary>EmptyValueTest</summary>
        //private static void EmptyValueTest() //-> InvalidOperationException
        //{
        //    string s = "";
        //    s = ((DayOfWeek)(-1)).ToString1();
        //    s = ((DayOfWeek)(7)).ToString1();
        //    //s = ((HttpStatusCode)(3)).ToString1();
#if NETCORE
        //    WriteLine.OutPutDebugAndConsole("-------------------------");
        //    s = ((DayOfWeek)(-1)).ToString2();
        //    s = ((DayOfWeek)(7)).ToString2();
        //    //s = ((HttpStatusCode)(3)).ToString2();
#endif
        //}
        /// <summary>ByteEnumTest</summary>
        private static void ByteEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("V0", ByteEnum.V0.ToString1());
            WriteLine.OutPutDebugAndConsole("V2", ByteEnum.V2.ToString1());
            WriteLine.OutPutDebugAndConsole("V4", ByteEnum.V4.ToString1());
#if NETCORE
            WriteLine.OutPutDebugAndConsole("-------------------------");
            WriteLine.OutPutDebugAndConsole("V0", ByteEnum.V0.ToString2());
            WriteLine.OutPutDebugAndConsole("V2", ByteEnum.V2.ToString2());
            WriteLine.OutPutDebugAndConsole("V4", ByteEnum.V4.ToString2());
#endif
        }
        /// <summary>LongEnumTest</summary>
        private static void LongEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("V0", LongEnum.V0.ToString1());
            WriteLine.OutPutDebugAndConsole("V2", LongEnum.V2.ToString1());
            WriteLine.OutPutDebugAndConsole("V1620100", LongEnum.V1620100.ToString1());
            WriteLine.OutPutDebugAndConsole("V23372036854775807", LongEnum.V23372036854775807.ToString1());
#if NETCORE
            WriteLine.OutPutDebugAndConsole("-------------------------");
            WriteLine.OutPutDebugAndConsole("V0", LongEnum.V0.ToString2());
            WriteLine.OutPutDebugAndConsole("V2", LongEnum.V2.ToString2());
            WriteLine.OutPutDebugAndConsole("V1620100", LongEnum.V1620100.ToString2());
            WriteLine.OutPutDebugAndConsole("V23372036854775807", LongEnum.V23372036854775807.ToString2());
#endif
        }
        /// <summary>FlagEnumTest</summary>
        private static void FlagEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("One", FlagsEnum.One.ToString1());
            WriteLine.OutPutDebugAndConsole("One,Two", (FlagsEnum.One | FlagsEnum.Two).ToString1()); // 既定では無理
            WriteLine.OutPutDebugAndConsole("Three", FlagsEnum.Three.ToString1());
            WriteLine.OutPutDebugAndConsole("Four", FlagsEnum.Four.ToString1());
#if NETCORE
            WriteLine.OutPutDebugAndConsole("-------------------------");
            WriteLine.OutPutDebugAndConsole("One", FlagsEnum.One.ToString2());
            WriteLine.OutPutDebugAndConsole("One,Two", (FlagsEnum.One | FlagsEnum.Two).ToString2()); // ここ凄い。
            WriteLine.OutPutDebugAndConsole("Three", FlagsEnum.Three.ToString2());
            WriteLine.OutPutDebugAndConsole("Four", FlagsEnum.Four.ToString2());
#endif
        }
#endregion

#endregion
    }
}