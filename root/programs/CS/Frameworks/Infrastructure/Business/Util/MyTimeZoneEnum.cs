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
//* クラス名        ：MyTimeZoneEnum
//* クラス日本語名  ：TimeZoneのID
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/20  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>TimeZoneのID</summary>
    public enum MyTimeZoneEnum
    {
        /// <summary>(GMT) カサブランカ</summary>
        MoroccoStandardTime,
        /// <summary>(GMT) グリニッジ標準時: ダブリン、エジンバラ、リスボン、ロンドン</summary>
        GMTStandardTime,
        /// <summary>(GMT) モンロビア、レイキャビク</summary>
        GreenwichStandardTime,
        /// <summary>(GMT) 協定世界時</summary>
        UTC,
        /// <summary>(GMT+01:00) アムステルダム、ベルリン､ベルン、ローマ､ストックホルム､ウィーン</summary>
        W_EuropeStandardTime,
        /// <summary>(GMT+01:00) ウィントフック</summary>
        NamibiaStandardTime,
        /// <summary>(GMT+01:00) サラエボ、スコピエ、ワルシャワ、ザグレブ</summary>
        CentralEuropeanStandardTime,
        /// <summary>(GMT+01:00) ブリュッセル、コペンハーゲン、マドリード、パリ</summary>
        RomanceStandardTime,
        /// <summary>(GMT+01:00) ベオグラード、ブラチスラバ、ブダペスト、リュブリャナ、プラハ</summary>
        CentralEuropeStandardTime,
        /// <summary>(GMT+01:00) 西中央アフリカ</summary>
        W_CentralAfricaStandardTime,
        /// <summary>(GMT+02:00) アテネ、ブカレスト</summary>
        GTBStandardTime,
        /// <summary>(GMT+02:00) アンマン</summary>
        JordanStandardTime,
        /// <summary>(GMT+02:00) イスタンブール</summary>
        TurkeyStandardTime,
        /// <summary>(GMT+02:00) エルサレム</summary>
        IsraelStandardTime,
        /// <summary>(GMT+02:00) カイロ</summary>
        EgyptStandardTime,
        /// <summary>(GMT+02:00) ダマスカス</summary>
        SyriaStandardTime,
        /// <summary>(GMT+02:00) ニコシア</summary>
        E_EuropeStandardTime,
        /// <summary>(GMT+02:00) ハラーレ、プレトリア</summary>
        SouthAfricaStandardTime,
        /// <summary>(GMT+02:00) ヘルシンキ、キエフ、リガ、スコピエ、ソフィア、タリン、ビリニュス</summary>
        FLEStandardTime,
        /// <summary>(GMT+02:00) ベイルート</summary>
        MiddleEastStandardTime,
        /// <summary>(GMT+03:00) カリーニングラード、ミンスク</summary>
        KaliningradStandardTime,
        /// <summary>(GMT+03:00) クウェート、リヤド</summary>
        ArabStandardTime,
        /// <summary>(GMT+03:00) ナイロビ</summary>
        E_AfricaStandardTime,
        /// <summary>(GMT+03:00) バグダッド</summary>
        ArabicStandardTime,
        /// <summary>(GMT+03:30) テヘラン</summary>
        IranStandardTime,
        /// <summary>(GMT+04:00) アブダビ、マスカット</summary>
        ArabianStandardTime,
        /// <summary>(GMT+04:00) エレバン</summary>
        ArmenianStandardTime,
        /// <summary>(GMT+04:00) コーカサス標準時</summary>
        CaucasusStandardTime,
        /// <summary>(GMT+04:00) トビリシ</summary>
        GeorgianStandardTime,
        /// <summary>(GMT+04:00) バク</summary>
        AzerbaijanStandardTime,
        /// <summary>(GMT+04:00) ポートルイス</summary>
        MauritiusStandardTime,
        /// <summary>(GMT+04:00) モスクワ、サンクトペテルブルク、ボルゴグラード</summary>
        RussianStandardTime,
        /// <summary>(GMT+04:30) カブール</summary>
        AfghanistanStandardTime,
        /// <summary>(GMT+05:00) イスラマバード、カラチ</summary>
        PakistanStandardTime,
        /// <summary>(GMT+05:00) タシケント</summary>
        WestAsiaStandardTime,
        /// <summary>(GMT+05:30) スリジャヤワルダナプラコッテ</summary>
        SriLankaStandardTime,
        /// <summary>(GMT+05:30) チェンナイ、コルカタ、ムンバイ、ニューデリー</summary>
        IndiaStandardTime,
        /// <summary>(GMT+05:45) カトマンズ</summary>
        NepalStandardTime,
        /// <summary>(GMT+06:00) アスタナ</summary>
        CentralAsiaStandardTime,
        /// <summary>(GMT+06:00) エカテリンバーグ</summary>
        EkaterinburgStandardTime,
        /// <summary>(GMT+06:00) ダッカ</summary>
        BangladeshStandardTime,
        /// <summary>(GMT+06:30) ヤンゴン (ラングーン)</summary>
        MyanmarStandardTime,
        /// <summary>(GMT+07:00) ノボシビルスク</summary>
        N_CentralAsiaStandardTime,
        /// <summary>(GMT+07:00) バンコク、ハノイ、ジャカルタ</summary>
        SEAsiaStandardTime,
        /// <summary>(GMT+08:00) ウランバートル</summary>
        UlaanbaatarStandardTime,
        /// <summary>(GMT+08:00) クアラルンプール、シンガポール</summary>
        SingaporeStandardTime,
        /// <summary>(GMT+08:00) クラスノヤルスク</summary>
        NorthAsiaStandardTime,
        /// <summary>(GMT+08:00) パース</summary>
        W_AustraliaStandardTime,
        /// <summary>(GMT+08:00) 北京、重慶、香港、ウルムチ</summary>
        ChinaStandardTime,
        /// <summary>(GMT+08:00) 台北</summary>
        TaipeiStandardTime,
        /// <summary>(GMT+09:00) イルクーツク</summary>
        NorthAsiaEastStandardTime,
        /// <summary>(GMT+09:00) ソウル</summary>
        KoreaStandardTime,
        /// <summary>(GMT+09:00) 大阪、札幌、東京</summary>
        TokyoStandardTime,
        /// <summary>(GMT+09:30) アデレード</summary>
        Cen_AustraliaStandardTime,
        /// <summary>(GMT+09:30) ダーウィン</summary>
        AUSCentralStandardTime,
        /// <summary>(GMT+10:00) キャンベラ、メルボルン、シドニー</summary>
        AUSEasternStandardTime,
        /// <summary>(GMT+10:00) グアム、ポートモレスビー</summary>
        WestPacificStandardTime,
        /// <summary>(GMT+10:00) ブリスベン</summary>
        E_AustraliaStandardTime,
        /// <summary>(GMT+10:00) ホバート</summary>
        TasmaniaStandardTime,
        /// <summary>(GMT+10:00) ヤクーツク</summary>
        YakutskStandardTime,
        /// <summary>(GMT+11:00) ウラジオストク</summary>
        VladivostokStandardTime,
        /// <summary>(GMT+11:00) ソロモン諸島、ニューカレドニア</summary>
        CentralPacificStandardTime,
        /// <summary>(GMT+12:00) オークランド、ウェリントン</summary>
        NewZealandStandardTime,
        /// <summary>(GMT+12:00) フィジー</summary>
        FijiStandardTime,
        /// <summary>(GMT+12:00) ペトロパブロフスク-カムチャツキー - 廃止</summary>
        KamchatkaStandardTime,
        /// <summary>(GMT+12:00) マガダン</summary>
        MagadanStandardTime,
        /// <summary>(GMT+12:00) 協定世界時+12</summary>
        UTC_12,
        /// <summary>(GMT+13:00) サモア</summary>
        SamoaStandardTime,
        /// <summary>(GMT+13:00) ヌクアロファ</summary>
        TongaStandardTime,
        /// <summary>(GMT-01:00) アゾレス諸島</summary>
        AzoresStandardTime,
        /// <summary>(GMT-01:00) カーボベルデ諸島</summary>
        CapeVerdeStandardTime,
        /// <summary>(GMT-02:00) 中央大西洋</summary>
        Mid_AtlanticStandardTime,
        /// <summary>(GMT-02:00) 協定世界時-02</summary>
        UTC_02,
        /// <summary>(GMT-03:00) カイエンヌ、フォルタレザ</summary>
        SAEasternStandardTime,
        /// <summary>(GMT-03:00) グリーンランド</summary>
        GreenlandStandardTime,
        /// <summary>(GMT-03:00) サルバドル</summary>
        BahiaStandardTime,
        /// <summary>(GMT-03:00) ブエノスアイレス</summary>
        ArgentinaStandardTime,
        /// <summary>(GMT-03:00) ブラジリア</summary>
        E_SouthAmericaStandardTime,
        /// <summary>(GMT-03:00) モンテビデオ</summary>
        MontevideoStandardTime,
        /// <summary>(GMT-03:30) ニューファンドランド</summary>
        NewfoundlandStandardTime,
        /// <summary>(GMT-04:00) アスンシオン</summary>
        ParaguayStandardTime,
        /// <summary>(GMT-04:00) クイアバ</summary>
        CentralBrazilianStandardTime,
        /// <summary>(GMT-04:00) サンティアゴ</summary>
        PacificSAStandardTime,
        /// <summary>(GMT-04:00) ジョージタウン、ラパス、マナウス、サンフアン</summary>
        SAWesternStandardTime,
        /// <summary>(GMT-04:00) 大西洋標準時 (カナダ)</summary>
        AtlanticStandardTime,
        /// <summary>(GMT-04:30) カラカス</summary>
        VenezuelaStandardTime,
        /// <summary>(GMT-05:00) インディアナ東部</summary>
        USEasternStandardTime,
        /// <summary>(GMT-05:00) ボゴタ、リマ、キト</summary>
        SAPacificStandardTime,
        /// <summary>(GMT-05:00) 東部標準時 (米国およびカナダ)</summary>
        EasternStandardTime,
        /// <summary>(GMT-06:00) グアダラハラ、メキシコシティ、モンテレー - 新</summary>
        CentralStandardTime_Mexico,
        /// <summary>(GMT-06:00) グアダラハラ、メキシコシティ、モンテレー - 旧</summary>
        MexicoStandardTime,
        /// <summary>(GMT-06:00) サスカチュワン</summary>
        CanadaCentralStandardTime,
        /// <summary>(GMT-06:00) 中央アメリカ</summary>
        CentralAmericaStandardTime,
        /// <summary>(GMT-06:00) 中部標準時 (米国およびカナダ)</summary>
        CentralStandardTime,
        /// <summary>(GMT-07:00) アリゾナ</summary>
        USMountainStandardTime,
        /// <summary>(GMT-07:00) チワワ、ラパス、マサトラン - 新</summary>
        MountainStandardTime_Mexico,
        /// <summary>(GMT-07:00) チワワ、ラパス、マサトラン - 旧</summary>
        MexicoStandardTime2,
        /// <summary>(GMT-07:00) 山地標準時 (米国およびカナダ)</summary>
        MountainStandardTime,
        /// <summary>(GMT-08:00) バハカリフォルニア</summary>
        PacificStandardTime_Mexico,
        /// <summary>(GMT-08:00) 太平洋標準時 (米国およびカナダ)</summary>
        PacificStandardTime,
        /// <summary>(GMT-09:00) アラスカ</summary>
        AlaskanStandardTime,
        /// <summary>(GMT-10:00) ハワイ</summary>
        HawaiianStandardTime,
        /// <summary>(GMT-11:00) 協定世界時-11</summary>
        UTC_11,
        /// <summary>(GMT-12:00) 国際日付変更線 西側</summary>
        DatelineStandardTime
    }
}
