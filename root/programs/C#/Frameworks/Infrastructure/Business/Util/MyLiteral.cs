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
//* クラス名        ：MyLiteral
//* クラス日本語名  ：Business層のリテラル クラス（必要なリテラルを追加）（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#6 ： Fxプレフィックスの欠如
//*  2009/07/21  西野 大介         コントロール取得処理の仕様変更
//*  2010/10/21  西野 大介         幾つかのイベント処理の正式対応（ベースクラス２→１へ）
//*  2011/11/20  西野 大介         リッチクライアント用P層フレームワークを追加
//*  2017/02/28  西野 大介         キャッシュ制御処理にスイッチを追加した。
//**********************************************************************************

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>Business層のリテラル クラス</summary>
    public class MyLiteral
    {
        #region app.configのキー（とデフォルト値）
        
        /// <summary>チェック ボックスのプレフィックスを設定するキー。</summary>
        /// <remarks>ベースクラス２から利用するので、internal</remarks>
        public const string PREFIX_OF_CHECK_BOX = "FxPrefixOfCheckBox";

        /// <summary>キャッシュ制御機能のON / OFFを設定するキー</summary>
        public const string CACHE_CONTROL = "FxCacheControl";

        #endregion
    }
}
