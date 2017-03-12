//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：GMTMaster
//* クラス日本語名  ：ローカル時刻⇔世界協定時刻(UTC)変換クラス
//*                   ・自前で時差管理が必要な場合は
//*                     「#region カスタム」のメソッドを使用する。
//*                   ・.NET3.以降5で、自前で時差管理が面倒な場合は
//*                     「#region .NET3.5以降」のメソッドを使用する。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/20  西野 大介         新規作成
//*  2017/03/10  西野 大介         .NET3.0以前の独自実装を破棄
//**********************************************************************************

using System;
using System.ComponentModel;

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>ローカル時刻⇔世界協定時刻(UTC)変換クラス</summary>
    public class GMTMaster
    {
        #region カスタム（.NET3.0以前）

        ///// <summary>ロック・オブジェクト</summary>
        //private static object Lock = new object();

        ///// <summary>MyTimeZone</summary>
        //private static MyTimeZone TZ = new MyTimeZone();

        ///// <summary>MyTimeZone更新用の隠しメソッド</summary>
        ///// <remarks>隠しメソッドなのでインテリセンスから参照不可</remarks>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public static void RefreshMyTimeZone()
        //{
        //    lock (GMTMaster.Lock)
        //    {
        //        // 再読み込み
        //        GMTMaster.TZ = new MyTimeZone();
        //    }
        //}

        ///// <summary>ローカル時刻→世界協定時刻(UTC)変換</summary>
        ///// <param name="localTime">ローカル時刻</param>
        ///// <param name="myTimeZoneId">ローカル時刻のタイムゾーン</param>
        ///// <returns>世界協定時刻(UTC)</returns>
        //public static DateTime ConvertLocalTimeToUtcTimeManual(DateTime localTime, MyTimeZoneEnum myTimeZoneId)
        //{
        //    // チェック
        //    if (localTime.Kind != DateTimeKind.Utc)
        //    {
        //        // != DateTimeKind.Utc
        //        DateTime utcTime = new DateTime(localTime.Ticks, DateTimeKind.Utc);

        //        // 変換
        //        lock (GMTMaster.Lock)
        //        {
        //            // 時差（分数）を引算
        //            utcTime = utcTime.AddMinutes(GMTMaster.TZ.GetTimezoneOffset(myTimeZoneId) * -1);
        //        }

        //        return utcTime;
        //    }
        //    else
        //    {
        //        // == DateTimeKind.Utc
        //        throw new ArgumentException("localTime.Kind == DateTimeKind.Utc", "localTime");
        //    }
        //}

        ///// <summary>世界協定時刻(UTC)→ローカル時刻変換</summary>
        ///// <param name="utcTime">世界協定時刻(UTC)</param>
        ///// <param name="myTimeZoneId">ローカル時刻のタイムゾーン</param>
        ///// <returns>
        ///// true:成功
        ///// false：失敗（≠ DateTimeKind.Utc）
        ///// </returns>
        //public static DateTime ConvertUtcTimeToLocalTimeManual(DateTime utcTime, MyTimeZoneEnum myTimeZoneId)
        //{
        //    // チェック
        //    if (utcTime.Kind != DateTimeKind.Local)
        //    {
        //        // != DateTimeKind.Local
        //        DateTime localTime = new DateTime(utcTime.Ticks, DateTimeKind.Local);

        //        // 変換
        //        lock (GMTMaster.Lock)
        //        {
        //            // 時差（分数）を加算
        //            localTime = localTime.AddMinutes(GMTMaster.TZ.GetTimezoneOffset(myTimeZoneId));
        //        }

        //        return localTime;
        //    }
        //    else
        //    {
        //        // == DateTimeKind.Local
        //        throw new ArgumentException("utcTime.Kind == DateTimeKind.Local", "utcTime");
        //    }
        //}

        #endregion

        #region .NET3.5以降

        // .NET2.0ではタイムゾーン情報がないので、ローカルのタイムゾーンにしか変換できない。
        // CurrentThreadのcultureの情報を変更できるが、cultureはTimeZoneInfoには１：１で対応しない。
        // このため、.NET3.5を使用して実装することにしている（母体が2.0なので変換後コメントを外して使用する）。

        // .NET3.5以降は、タイムゾーン・時間差情報を持っており、これを自分で管理する必要はない。

        // ConvertTimeToUtc、ConvertTimeFromUtcメソッドの仕様自体はブラックボックスだが、
        // TimeZoneInfo.GetSystemTimeZones();でTimeZoneInfoを取得し、
        // 以下のプロパティを確認することで仕様確認は可能である。
        //
        // TimeZoneInfo.SupportsDaylightSavingTime プロパティ
        // http://msdn.microsoft.com/ja-jp/library/system.timezoneinfo.supportsdaylightsavingtime.aspx
        // TimeZoneInfo.BaseUtcOffset プロパティ
        // http://msdn.microsoft.com/ja-jp/library/system.timezoneinfo.baseutcoffset.aspx

        /// <summary>ローカル時刻→世界協定時刻(UTC)変換</summary>
        /// <param name="localTime">ローカル時刻</param>
        /// <returns>世界協定時刻(UTC)</returns>
        public static DateTime ConvertLocalTimeToUtcTime35(DateTime localTime)
        {
            return ConvertLocalTimeToUtcTime35(localTime, TimeZoneInfo.Local);
        }

        /// <summary>ローカル時刻→世界協定時刻(UTC)変換</summary>
        /// <param name="localTime">ローカル時刻</param>
        /// <param name="tzInfo">ローカル時刻のタイムゾーン</param>
        /// <returns>世界協定時刻(UTC)</returns>
        public static DateTime ConvertLocalTimeToUtcTime35(DateTime localTime, TimeZoneInfo tzInfo)
        {
            return TimeZoneInfo.ConvertTimeToUtc(localTime, tzInfo);
        }

        /// <summary>
        /// 世界協定時刻(UTC)→ローカル時刻変換
        /// </summary>
        /// <param name="utcTime">世界協定時刻(UTC)</param>
        /// <returns>ローカル時刻</returns>
        public static DateTime ConvertUtcTimeToLocalTime35(DateTime utcTime)
        {
            return ConvertUtcTimeToLocalTime35(utcTime, TimeZoneInfo.Local);
        }

        /// <summary>
        /// 世界協定時刻(UTC)→ローカル時刻変換
        /// </summary>
        /// <param name="utcTime">世界協定時刻(UTC)</param>
        /// <param name="tzInfo">ローカル時刻のタイムゾーン</param>
        /// <returns>ローカル時刻</returns>
        public static DateTime ConvertUtcTimeToLocalTime35(DateTime utcTime, TimeZoneInfo tzInfo)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzInfo);
        }

        #endregion
    }
}
