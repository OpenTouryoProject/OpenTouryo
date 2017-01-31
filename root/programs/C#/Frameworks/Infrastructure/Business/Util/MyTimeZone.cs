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
//* クラス名        ：MyTimeZone
//* クラス日本語名  ：TimeZoneのIDとオフセット（分）の管理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/20  西野 大介         新規作成
//**********************************************************************************

using System.Collections.Generic;

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>
    /// TimeZoneのIDとオフセット（分）の管理クラス
    /// </summary>
    /// <remarks>
    /// TimeZoneInfo.GetSystemTimeZones()から取得した情報を基に作成
    /// </remarks>
    internal class MyTimeZone
    {
        /// <summary>TimeZoneのIDとオフセット（分）のディクショナリ</summary>
        private static readonly Dictionary<MyTimeZoneEnum, int>
            TimeZoneDictionary = new Dictionary<MyTimeZoneEnum, int>();

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// TimeZoneのIDに対応するオフセット（分）を指定（定義から読む等の変更可能）
        /// </summary>
        public MyTimeZone()
        {
            // UTCと指定のTimeZoneとの時差を分単位で設定する。
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MoroccoStandardTime, 0);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.GMTStandardTime, 0);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.GreenwichStandardTime, 0);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.UTC, 0);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.W_EuropeStandardTime, 60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.NamibiaStandardTime, 60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralEuropeanStandardTime, 60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.RomanceStandardTime, 60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralEuropeStandardTime, 60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.W_CentralAfricaStandardTime, 60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.GTBStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.JordanStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.TurkeyStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.IsraelStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.EgyptStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SyriaStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.E_EuropeStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SouthAfricaStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.FLEStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MiddleEastStandardTime, 120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.KaliningradStandardTime, 180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.ArabStandardTime, 180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.E_AfricaStandardTime, 180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.ArabicStandardTime, 180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.IranStandardTime, 210);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.ArabianStandardTime, 240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.ArmenianStandardTime, 240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CaucasusStandardTime, 240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.GeorgianStandardTime, 240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.AzerbaijanStandardTime, 240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MauritiusStandardTime, 240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.RussianStandardTime, 240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.AfghanistanStandardTime, 270);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.PakistanStandardTime, 300);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.WestAsiaStandardTime, 300);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SriLankaStandardTime, 330);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.IndiaStandardTime, 330);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.NepalStandardTime, 345);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralAsiaStandardTime, 360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.EkaterinburgStandardTime, 360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.BangladeshStandardTime, 360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MyanmarStandardTime, 390);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.N_CentralAsiaStandardTime, 420);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SEAsiaStandardTime, 420);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.UlaanbaatarStandardTime, 480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SingaporeStandardTime, 480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.NorthAsiaStandardTime, 480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.W_AustraliaStandardTime, 480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.ChinaStandardTime, 480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.TaipeiStandardTime, 480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.NorthAsiaEastStandardTime, 540);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.KoreaStandardTime, 540);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.TokyoStandardTime, 540);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.Cen_AustraliaStandardTime, 570);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.AUSCentralStandardTime, 570);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.AUSEasternStandardTime, 600);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.WestPacificStandardTime, 600);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.E_AustraliaStandardTime, 600);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.TasmaniaStandardTime, 600);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.YakutskStandardTime, 600);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.VladivostokStandardTime, 660);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralPacificStandardTime, 660);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.NewZealandStandardTime, 720);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.FijiStandardTime, 720);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.KamchatkaStandardTime, 720);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MagadanStandardTime, 720);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.UTC_12, 720);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SamoaStandardTime, 780);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.TongaStandardTime, 780);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.AzoresStandardTime, -60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CapeVerdeStandardTime, -60);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.Mid_AtlanticStandardTime, -120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.UTC_02, -120);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SAEasternStandardTime, -180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.GreenlandStandardTime, -180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.BahiaStandardTime, -180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.ArgentinaStandardTime, -180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.E_SouthAmericaStandardTime, -180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MontevideoStandardTime, -180);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.NewfoundlandStandardTime, -210);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.ParaguayStandardTime, -240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralBrazilianStandardTime, -240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.PacificSAStandardTime, -240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SAWesternStandardTime, -240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.AtlanticStandardTime, -240);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.VenezuelaStandardTime, -270);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.USEasternStandardTime, -300);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.SAPacificStandardTime, -300);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.EasternStandardTime, -300);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralStandardTime_Mexico, -360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MexicoStandardTime, -360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CanadaCentralStandardTime, -360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralAmericaStandardTime, -360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.CentralStandardTime, -360);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.USMountainStandardTime, -420);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MountainStandardTime_Mexico, -420);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MexicoStandardTime2, -420);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.MountainStandardTime, -420);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.PacificStandardTime_Mexico, -480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.PacificStandardTime, -480);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.AlaskanStandardTime, -540);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.HawaiianStandardTime, -600);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.UTC_11, -660);
            MyTimeZone.TimeZoneDictionary.Add(MyTimeZoneEnum.DatelineStandardTime, -720);
        }

        #endregion

        /// <summary>TimeZoneのオフセットを取得</summary>
        /// <param name="myTimeZoneEnum"></param>
        /// <returns>TimeZoneのオフセット（分）</returns>
        public int GetTimezoneOffset(MyTimeZoneEnum myTimeZoneEnum)
        {
            return MyTimeZone.TimeZoneDictionary[myTimeZoneEnum];
        }
    }
}
