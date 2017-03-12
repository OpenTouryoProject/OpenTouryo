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
//* クラス名        ：WebCustomTextBox
//* クラス日本語名  ：テキストボックス（Web）のカスタム・コントロール（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2016/01/28  Sai               Corrected IsIndispensabile property spelling 
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

using System.Web.UI;
using System.Web.UI.WebControls;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.CustomControl
{
    /// <summary>テキストボックス（Web）のカスタム・コントロール</summary>
    [DefaultProperty("TextBox")]
    [ToolboxData("<{0}:WebCustomTextBox runat=server></{0}:WebCustomTextBox>")]
    public class WebCustomTextBox : TextBox, ICheck, IGetValue
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebCustomTextBox()
        {
            // コンストラクタでのスタイル系のプロパティ設定は、
            // CSSとの相性が悪と考え、廃止しました。

            //this.Font.Size = 12;
            //this.ForeColor = System.Drawing.Color.Black;
            //this.Font.Name = "ＭＳ ゴシック";
        }

        #region 値取得（IGetValue）

        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        public DateTime GetDateTime()
        {
            return DateTime.Parse(this.Text);
        }

        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <param name="provider">書式</param>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        public DateTime GetDateTime(IFormatProvider provider)
        {
            return DateTime.Parse(this.Text, provider);
        }

        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <param name="provider">書式</param>
        /// <param name="styles">スタイル</param>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        public DateTime GetDateTime(IFormatProvider provider, DateTimeStyles styles)
        {
            return DateTime.Parse(this.Text, provider, styles);
        }

        /// <summary>
        /// Text値をDecimal型にキャストして返す。
        /// </summary>
        /// <returns>Decimal値</returns>
        [DebuggerStepThrough]
        public decimal GetDecimal()
        {
            return decimal.Parse(this.Text);
        }

        /// <summary>
        /// Text値をDouble型にキャストして返す。
        /// </summary>
        /// <returns>Double値</returns>
        [DebuggerStepThrough]
        public double GetDouble()
        {
            return double.Parse(this.Text);
        }

        /// <summary>
        /// Text値をFloat型にキャストして返す。
        /// </summary>
        /// <returns>Float値</returns>
        [DebuggerStepThrough]
        public float GetFloat()
        {
            return float.Parse(this.Text);
        }

        /// <summary>
        /// Text値をInt16型にキャストして返す。
        /// </summary>
        /// <returns>Int16値</returns>
        [DebuggerStepThrough]
        public short GetInt16()
        {
            return short.Parse(this.Text);
        }

        /// <summary>
        /// Text値をInt32型にキャストして返す。
        /// </summary>
        /// <returns>Int32値</returns>
        [DebuggerStepThrough]
        public int GetInt32()
        {
            return int.Parse(this.Text);
        }

        /// <summary>
        /// Text値をInt64型にキャストして返す。
        /// </summary>
        /// <returns>Int64値</returns>
        [DebuggerStepThrough]
        public long GetInt64()
        {
            return long.Parse(this.Text);
        }

        #endregion

        #region デザインタイム・プロパティ

        #region チェック プロパティ

        // こちらのプロパティは基本的にPGで変更しないのでViewState化しないこととした。
        // 変更＋状態保存が必要であれば、必要に応じてViewState化すること。

        ///// <summary>TEST</summary>
        ///// <remarks>
        ///// The Official Microsoft ASP.NET Forums
        ///// WebCategoryAttribute and WebSysDescription compile errors.
        ///// http://forums.asp.net/t/1330898.aspx/1
        ///// </remarks>
        //[Category("Test"),
        //Description("テスト"), 
        //DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        ////WebCategory("Appearance"),
        ////WebSysDescription("WebControl_Font")]
        //public virtual FontInfo TestFont
        //{
        //    get
        //    {
        //        return this.ControlStyle.Font;
        //    }
        //}

        // HOW TO: Create a Web Control with an Expandable Property in the Designer by Using Visual C# .NET
        // http://support.microsoft.com/kb/324301/ja
        /// <summary>入力文字種チェック</summary>
        private CheckType _checkType = new CheckType();

        /// <summary>入力文字種チェック</summary>
        [Category("Check"),
        Description("入力文字種チェック"),
       PersistenceMode(PersistenceMode.InnerProperty),
       DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CheckType CheckType
        {
            get
            {
                return this._checkType;
            }
            set
            {
                this._checkType = value;
            }
        }

        /// <summary>入力文字種チェックのデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeCheckType()
        {
            return this._checkType != new CheckType();
        }

        /// <summary>正規表現チェック</summary>
        private string _checkRegExp = "";

        /// <summary>正規表現チェック</summary>
        [DefaultValue(""),
        Category("Check"),
        Description("正規表現チェック")]
        public string CheckRegExp
        {
            get
            {
                return this._checkRegExp;
            }
            set
            {
                this._checkRegExp = value;
            }
        }

        /// <summary>正規表現チェック</summary>
        private bool _checkProhibitedChar = false;

        /// <summary>正規表現チェック</summary>
        [DefaultValue(false),
        Category("Check"),
        Description("禁則文字チェック")]
        public bool CheckProhibitedChar
        {
            get
            {
                return this._checkProhibitedChar;
            }
            set
            {
                this._checkProhibitedChar = value;
            }
        }

        #endregion

        #endregion

        #region チェック処理（Validating、Validated）

        #region Validate

        /// <summary>チェック処理</summary>
        /// <returns>
        /// ・エラーなし：true
        /// ・エラーあり：false
        /// </returns>
        /// <remarks>
        /// ・必須入力チェック
        /// ・数値チェック
        /// ・半角チェック
        /// ・全角チェック
        /// ・片仮名チェック
        /// ・半角片仮名チェック
        /// ・平仮名チェック
        /// ・日付チェック
        /// 
        /// ・正規表現チェック
        /// ・禁則文字チェック
        /// </remarks>
        public bool Validate()
        {
            string[] temp;
            bool ret = this.Validate(out temp);

            return ret;
        }

        /// <summary>チェック処理</summary>
        /// <param name="result">結果文字列</param>
        /// <returns>
        /// ・エラーなし：true
        /// ・エラーあり：false
        /// </returns>
        /// <remarks>
        /// ・必須入力チェック
        /// ・数値チェック
        /// ・半角チェック
        /// ・全角チェック
        /// ・片仮名チェック
        /// ・半角片仮名チェック
        /// ・平仮名チェック
        /// ・日付チェック
        /// 
        /// ・正規表現チェック
        /// ・禁則文字チェック
        /// </remarks>
        public bool Validate(out string[] result)
        {
            // フラグ
            bool hasError = false;
            // ワーク
            List<string> lstRet = new List<string>();

            string text = this.Text;

            if (this.CheckType != null)
            {
                // 必須入力チェック
                if (this.CheckType.Required)
                {
                    if ((text == ""))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.RequiredCheckErrorMessage);
                    }
                }

                // 数値チェック（空文字列は対象外）
                if (this.CheckType.IsNumeric && text.Trim() != "")
                {
                    if (!StringChecker.IsNumeric(text))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsNumericCheckErrorMessage);
                    }
                }

                // 半角チェック
                if (this.CheckType.IsHankaku)
                {
                    if (!StringChecker.IsHankaku(text))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsHankakuCheckErrorMessage);
                    }
                }

                // 全角チェック
                if (this.CheckType.IsZenkaku)
                {
                    if (!StringChecker.IsZenkaku(text))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsZenkakuCheckErrorMessage);
                    }
                }

                // 片仮名チェック
                if (this.CheckType.IsKatakana)
                {
                    if (!StringChecker.IsKatakana(text))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsKatakanaCheckErrorMessage);
                    }
                }

                // 半角片仮名チェック
                if (this.CheckType.IsHanKatakana)
                {
                    if (!StringChecker.IsKatakana_Hankaku(text))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsHanKatakanaCheckErrorMessage);
                    }
                }

                // 平仮名チェック
                if (this.CheckType.IsHiragana)
                {
                    if (!StringChecker.IsHiragana(text))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsHiraganaCheckErrorMessage);
                    }
                }

                // 日付チェック（空文字列は対象外）
                if (this.CheckType.IsDate && text.Trim() != "")
                {
                    DateTime dateTime;
                    if (!DateTime.TryParse(text, out dateTime))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsDateCheckErrorMessage);
                    }
                }
            }

            // 正規表現チェック（空文字列は対象外）
            if (this.CheckRegExp != null && this.CheckRegExp != "" && text.Trim() != "")
            {
                if (!StringChecker.Match(text, this.CheckRegExp))
                {
                    hasError = true;
                    lstRet.Add(CmnCheckFunction.RegularExpressionCheckErrorMessage);
                }
            }

            // 禁則文字チェック
            if (this.CheckProhibitedChar)
            {
                foreach (char ch in CmnCheckFunction.ProhibitedChars)
                {
                    if (text.IndexOf(ch) != -1)
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.ProhibitedCharsCheckErrorMessage);
                        break;
                    }
                }
            }

            // 背景変更
            if (hasError)
            {
                // エラーの背景色
                if (this.ViewState["wcc_backupBkColor"] == null)
                {
                    this.ViewState["wcc_backupBkColor"] = (Color?)this.BackColor;
                    this.BackColor = Color.Red;
                }
            }
            else
            {
                // 正常時の背景色
                if (this.ViewState["wcc_backupBkColor"] != null)
                {
                    this.BackColor = (Color)this.ViewState["wcc_backupBkColor"];
                    this.ViewState["wcc_backupBkColor"] = null;
                }
            }

            result = lstRet.ToArray();
            return !hasError;
        }

        #endregion

        #endregion

        #region HTML描画処理のカスタマイズ用テンプレート

        #region Renderの制御

        /// <summary>Visibleプロパティ、ページのトレースなどの制御を行い、ページにコントロールを表示する。</summary>
        /// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
        /// <remarks>
        /// このメソッドは、表示中にページによって自動的に呼び出される。
        /// カスタム コントロールの開発者はこのメソッドをオーバーライドできる。
        /// 
        /// WebControl.RenderControlメソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.ui.webcontrols.webcontrol.rendercontrol.aspx
        /// </remarks>
        public override void RenderControl(HtmlTextWriter output)
        {
            //if (false)
            //{
            //    // RenderControlにRenderの制御処理を作り込む。
            //    // Render処理のHtmlTextWriterを他のWriterでdecorateする。

            //    // （例）
            //    // StringWriterの書き出し先のStringBuilderを生成
            //    StringBuilder sb = new StringBuilder();
            //    // StringWriterを使用するStringWriterを生成
            //    StringWriter sw = new StringWriter(sb);

            //    // 上記StringWriterを使用するHtmlTextWriterを生成（decorate）
            //    HtmlTextWriter decoratedHTW = new HtmlTextWriter(sw);

            //    // StringWriterを使用するHtmlTextWriterを指定し、Renderメソッドを実行する。
            //    // すると、HtmlTextWriter→StringWriterにWriteされる。
            //    this.Render(decoratedHTW);

            //    // RenderメソッドでStringWriterにWriteされた、
            //    // コントロールのHTMLをStringBuilderから取得する。
            //    string html = sb.ToString();

            //    // TODO：string htmlの編集処理を実装する。
            //    // 本来はCustomWriterのWriteメソッド内で、
            //    // 変換処理を実施後にHtmlTextWriterにWriteする。

            //    // ページにコントロールを表示
            //    output.Write(html);
            //}
            //else
            //{

            // 通常通りのRenderControl
            base.RenderControl(output);

            //}
        }

        #endregion

        #region Renderの実体

        /// <summary>
        /// Renderメソッドは、
        /// ・RenderBeginTag（開始のタグ）
        /// ・RenderContents（中間の部分）
        /// ・RenderEndTag（終了のタグ）
        /// の各メソッドをこの順に呼び出して、
        /// コントロールをクライアントに送信する。
        /// </summary>
        /// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
        /// <remarks>
        /// このメソッドは、表示中にページによって自動的に呼び出される。
        /// また、このメソッドは、主にコントロールの開発者によって使用される。
        /// 
        /// WebControl.Renderメソッド
        /// http://msdn.microsoft.com/ja-jp/library/system.web.ui.webcontrols.webcontrol.render.aspx
        /// </remarks>
        protected override void Render(HtmlTextWriter output)
        {
            // Render処理を作り込む。

            //if (false)
            //{
            //    // Renderｘの順次呼び出し
            //    // ・RenderBeginTag（開始のタグ）
            //    // ・RenderContents（中間の部分）
            //    // ・RenderEndTag（終了のタグ）

            //    // 独自にWriteでも良い。
            //    output.Write("xxxx");
            //}
            //else
            //{

            // 通常通りのRender
            base.Render(output);

            //}
        }

        #endregion

        #region 以下は、RenderBeginTag、RenderContents、RenderEndTagの実装例

        ///// <summary>RenderBeginTag</summary>
        ///// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
        //public override void RenderBeginTag(HtmlTextWriter output)
        //{
        //    output.Write("<test>");
        //}

        ///// <summary>RenderContents</summary>
        ///// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
        //protected override void RenderContents(HtmlTextWriter output)
        //{
        //    output.Write("test");
        //}

        ///// <summary>RenderEndTagのテスト</summary>
        ///// <param name="output">コントロールの内容を受け取る HtmlTextWriter のオブジェクト</param>
        //public override void RenderEndTag(HtmlTextWriter output)
        //{
        //    output.Write("</test>");
        //}

        #endregion

        #endregion
    }
}
