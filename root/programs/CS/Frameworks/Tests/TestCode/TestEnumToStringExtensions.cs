using System;

using Touryo.Infrastructure.Public.FastReflection;
using Touryo.Infrastructure.Public.Diagnostics;

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
            MyDebug.OutputDebugAndConsole("--------------------------------------------------");
            TestEnumToStringExtensions.BaseIsNotZeroEnumTest();
            MyDebug.OutputDebugAndConsole("--------------------------------------------------");
            //TestEnumToStringExtensions.EmptyValueTest();
            //MyDebug.OutputDebugAndConsole("--------------------------------------------------");
            TestEnumToStringExtensions.ByteEnumTest();
            MyDebug.OutputDebugAndConsole("--------------------------------------------------");
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
            MyDebug.OutputDebugAndConsole("Friday", DayOfWeek.Friday.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("Wednesday", DayOfWeek.Wednesday.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("Saturday", DayOfWeek.Saturday.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("Friday", ((DayOfWeek)(5)).ToStringByEmit());
            MyDebug.OutputDebugAndConsole("Saturday", ((DayOfWeek)(6)).ToStringByEmit());
#if NETCOREAPP
            MyDebug.OutputDebugAndConsole("-------------------------");
            MyDebug.OutputDebugAndConsole("Friday", DayOfWeek.Friday.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("Wednesday", DayOfWeek.Wednesday.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("Saturday", DayOfWeek.Saturday.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("Friday", ((DayOfWeek)(5)).ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("Saturday", ((DayOfWeek)(6)).ToStringByExpressionTree());
#endif
        }
        /// <summary>BaseIsNotZeroEnumTest</summary>
        private static void BaseIsNotZeroEnumTest()
        {
            MyDebug.OutputDebugAndConsole("V2", BaseIsNotZeroEnum.V2.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("V3", BaseIsNotZeroEnum.V3.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("V4", BaseIsNotZeroEnum.V4.ToStringByEmit());
#if NETCOREAPP
            MyDebug.OutputDebugAndConsole("-------------------------");
            MyDebug.OutputDebugAndConsole("V2", BaseIsNotZeroEnum.V2.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("V3", BaseIsNotZeroEnum.V3.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("V4", BaseIsNotZeroEnum.V4.ToStringByExpressionTree());
#endif
        }
        ///// <summary>EmptyValueTest</summary>
        //private static void EmptyValueTest() //-> InvalidOperationException
        //{
        //    string s = "";
        //    s = ((DayOfWeek)(-1)).ToStringByEmit();
        //    s = ((DayOfWeek)(7)).ToStringByEmit();
        //    //s = ((HttpStatusCode)(3)).ToStringByEmit();
#if NETCOREAPP
        //    MyDebug.OutputDebugAndConsole("-------------------------");
        //    s = ((DayOfWeek)(-1)).ToStringByExpressionTree();
        //    s = ((DayOfWeek)(7)).ToStringByExpressionTree();
        //    //s = ((HttpStatusCode)(3)).ToStringByExpressionTree();
#endif
        //}
        /// <summary>ByteEnumTest</summary>
        private static void ByteEnumTest()
        {
            MyDebug.OutputDebugAndConsole("V0", ByteEnum.V0.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("V2", ByteEnum.V2.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("V4", ByteEnum.V4.ToStringByEmit());
#if NETCOREAPP
            MyDebug.OutputDebugAndConsole("-------------------------");
            MyDebug.OutputDebugAndConsole("V0", ByteEnum.V0.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("V2", ByteEnum.V2.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("V4", ByteEnum.V4.ToStringByExpressionTree());
#endif
        }
        /// <summary>LongEnumTest</summary>
        private static void LongEnumTest()
        {
            MyDebug.OutputDebugAndConsole("V0", LongEnum.V0.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("V2", LongEnum.V2.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("V1620100", LongEnum.V1620100.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("V23372036854775807", LongEnum.V23372036854775807.ToStringByEmit());
#if NETCOREAPP
            MyDebug.OutputDebugAndConsole("-------------------------");
            MyDebug.OutputDebugAndConsole("V0", LongEnum.V0.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("V2", LongEnum.V2.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("V1620100", LongEnum.V1620100.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("V23372036854775807", LongEnum.V23372036854775807.ToStringByExpressionTree());
#endif
        }
        /// <summary>FlagEnumTest</summary>
        private static void FlagEnumTest()
        {
            MyDebug.OutputDebugAndConsole("One", FlagsEnum.One.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("One,Two", (FlagsEnum.One | FlagsEnum.Two).ToStringByEmit()); // 既定では無理
            MyDebug.OutputDebugAndConsole("Three", FlagsEnum.Three.ToStringByEmit());
            MyDebug.OutputDebugAndConsole("Four", FlagsEnum.Four.ToStringByEmit());
#if NETCOREAPP
            MyDebug.OutputDebugAndConsole("-------------------------");
            MyDebug.OutputDebugAndConsole("One", FlagsEnum.One.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("One,Two", (FlagsEnum.One | FlagsEnum.Two).ToStringByExpressionTree()); // ここ凄い。
            MyDebug.OutputDebugAndConsole("Three", FlagsEnum.Three.ToStringByExpressionTree());
            MyDebug.OutputDebugAndConsole("Four", FlagsEnum.Four.ToStringByExpressionTree());
#endif
        }
        #endregion

        #endregion
    }
}