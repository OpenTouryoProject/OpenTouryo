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
//* クラス名        ：CmnCheckFunction
//* クラス日本語名  ：リッチクライアント用カスタムコントロールの共通関数クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2016/01/28  Sai               Corrected IsIndispensabile property spelling
//**********************************************************************************

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Web.UI;

namespace Touryo.Infrastructure.CustomControl
{
    /// <summary>リッチクライアント用カスタムコントロールの共通関数クラス</summary>
    public class CmnCheckFunction
    {
        #region メッセージ
        
        /// <summary>
        /// 必須チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string RequiredCheckErrorMessage = "必須チェック エラー";

        /// <summary>
        /// 半角チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string IsHankakuCheckErrorMessage = "半角チェック エラー";
        /// <summary>
        /// 全角チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string IsZenkakuCheckErrorMessage = "全角チェック エラー";

        /// <summary>
        /// 数値チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string IsNumericCheckErrorMessage = "数値チェック エラー";

        /// <summary>
        /// 片仮名チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string IsKatakanaCheckErrorMessage = "片仮名チェック エラー";
        /// <summary>
        /// 半角片仮名チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string IsHanKatakanaCheckErrorMessage = "半角片仮名チェック エラー";

        /// <summary>
        /// 平仮名チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string IsHiraganaCheckErrorMessage = "平仮名チェック エラー";

        /// <summary>
        /// 日付チェック エラー
        /// のエラー メッセージ
        /// </summary>
        public const string IsDateCheckErrorMessage = "日付チェック エラー";

        /// <summary>
        /// 正規表現チェック エラーの
        /// エラー メッセージ
        /// </summary>
        public const string RegularExpressionCheckErrorMessage = "正規表現チェック エラー";
        /// <summary>
        /// 禁則文字チェック エラーの
        /// エラー メッセージ
        /// </summary>
        public const string ProhibitedCharsCheckErrorMessage = "禁則文字チェック エラー";

        #endregion

        #region チェック リテラル

        /// <summary>禁則文字</summary>
        /// <remarks>
        /// ・#（シャープ）
        /// ・'（シングルクォーテーション）
        /// ・\（円マーク）
        /// ・|（パイプ）
        /// ・%（パーセント）
        /// ・_（アンダースコア）
        /// </remarks>
　　　public static readonly ReadOnlyCollection<char> ProhibitedChars = 
　　　　　new ReadOnlyCollection<char>(new List<char>() { '#', '\'', '\\', '|', '%', '_' });

        #endregion

        #region 一括チェック

        /// <summary>コントロールのバリデーション</summary>
        /// <param name="parentCtrl">チェックルートのコントロール</param>
        /// <param name="lstCheckResult">チェック結果を保持するリスト</param>
        /// <returns>
        /// ・エラーあり：true
        /// ・エラーなし：false
        /// </returns>
        public static bool HasErrors(Control parentCtrl, List<CheckResult> lstCheckResult)
        {
            // チェック結果を保持するリスト
            if (lstCheckResult == null) { lstCheckResult = new List<CheckResult>(); }

            // チェック対象のコントロールなら、
            if (parentCtrl is WebCustomTextBox)
            {
                // チェックし、
                string[] temp = null;
                ICheck ic = (ICheck)parentCtrl;
                if (!ic.Validate(out temp))
                {
                    // エラーならエラー情報を保持する。
                    CheckResult cr = new CheckResult(parentCtrl.ID);
                    cr.CheckErrorInfo = temp;
                    lstCheckResult.Add(cr);
                }
            }

            // コントロールを再起検索する。
            foreach (Control childctrl in parentCtrl.Controls)
            {
                CmnCheckFunction.HasErrors(childctrl, lstCheckResult);
            }

            // エラーが有れば、trueを返す。
            return (0 < lstCheckResult.Count);
        }

        #endregion
    }
}
