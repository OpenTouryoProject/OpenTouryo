using System;

using Touryo.Infrastructure.Public.Dbg;
using Touryo.Infrastructure.Public.FastReflection;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestEnumToStringExtensions
    {
        #region enum
        public enum BaseIsNotZeroEnum : byte
        {
            V2 = 2,
            V3,
            V4,
            V5,
            V6,
        }

        public enum ByteEnum : byte
        {
            V0 = 0,
            V2 = 2,
            V4 = 4,
        }

        public enum LongEnum : long
        {
            V0 = 0,
            V2 = 2,
            V1620100 = 1620100,
            V23372036854775807 = 23372036854775807,
        }

        [FlagsAttribute]
        enum FlagsEnum
        {
            None = 0x00, // 0000 0000
            One = 0x01, // 0000 0001
            Two = 0x02, // 0000 0010 
            Three = 0x04, // 0000 0100
            Four = 0x08, // 0000 1000
            All = 0x0F // 0000 1111
        }
        #endregion

        #region public
        public void Test()
        {
            this.BasicEnumTest();
            this.BaseIsNotZeroEnumTest();
            //this.EmptyValueTest();
            this.ByteEnumTest();
            this.FlagEnumTest();
        }
        #endregion

        #region private
        private void BasicEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("Friday", DayOfWeek.Friday.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("Wednesday", DayOfWeek.Wednesday.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("Saturday", DayOfWeek.Saturday.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("Friday", ((DayOfWeek)(5)).ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("Saturday", ((DayOfWeek)(6)).ToStringFromEnum());
        }
        private void BaseIsNotZeroEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("V2", BaseIsNotZeroEnum.V2.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("V3", BaseIsNotZeroEnum.V3.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("V4", BaseIsNotZeroEnum.V4.ToStringFromEnum());
        }
        //private void EmptyValueTest() -> InvalidOperationException
        //{
        //    String s = "";
        //    s = ((DayOfWeek)(-1)).ToStringFromEnum();
        //    s = ((DayOfWeek)(7)).ToStringFromEnum();
        //    //s = ((HttpStatusCode)(3)).ToStringFromEnum();
        //}
        private void ByteEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("V0", ByteEnum.V0.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("V2", ByteEnum.V2.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("V4", ByteEnum.V4.ToStringFromEnum());
        }
        private void LongEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("V0", LongEnum.V0.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("V2", LongEnum.V2.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("V1620100", LongEnum.V1620100.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("V23372036854775807", LongEnum.V23372036854775807.ToStringFromEnum());
        }
        private void FlagEnumTest()
        {
            WriteLine.OutPutDebugAndConsole("One", FlagsEnum.One.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("One,Two", (FlagsEnum.One | FlagsEnum.Two).ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("Three", FlagsEnum.Three.ToStringFromEnum());
            WriteLine.OutPutDebugAndConsole("Four", FlagsEnum.Four.ToStringFromEnum());
        }
        #endregion
    }
}
