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
//* クラス名        ：WinCustomMaskedTextBox
//* クラス日本語名  ：マスク テキスト ボックス（Win）のカスタム・コントロール（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2016/01/28  Sai               Corrected IsIndispensabile property spelling
//*  2017/01/31  西野 大介         "Indispensable" ---> "Required"
//**********************************************************************************

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Touryo.Infrastructure.Framework.RichClient.Util;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>System.Windows.Forms.MaskedTextBoxのカスタム・コントロール</summary>
    [DefaultProperty("Text")]
    [Designer(typeof(WinCustomMaskedTextBoxDesigner))]
    public class WinCustomMaskedTextBox : MaskedTextBox, ICheck, IGetValue, INotifyPropertyChanged
    {
        ///<summary>デザイナ上の表示をカスタマイズするインナークラス</summary>
        ///<remarks>
        ///自作コントロールにおいて、不要なプロパティをデザイナANDインテリセンスで隠したい時
        ///http://jehupc.exblog.jp/8157762/
        ///継承と属性プログラミングで実現するRAD開発 － ＠IT
        ///http://www.atmarkit.co.jp/fdotnet/winexp/winexp02/winexp02_03.html
        ///</remarks>
        internal class WinCustomMaskedTextBoxDesigner : ControlDesigner
        {
            ///<summary>下記で指定してあるプロパティはデザイナでは非表示とする。</summary>
            protected override void PostFilterProperties(IDictionary Properties)
            {
                Properties.Remove("Text2");
                Properties.Remove("Text3");
            }
        }

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public WinCustomMaskedTextBox()
            : base()
        {
            this.InitializeComponent();

            // Textはマスク適用強制
            ((MaskedTextBox)this).TextMaskFormat = MaskFormat.IncludeLiterals;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="mask">マスク</param>
        public WinCustomMaskedTextBox(string mask)
            : base(mask)
        {
            this.InitializeComponent();
        }

        /// <summary>初期化</summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WinCustomMaskedTextBox
            // 
            this.Layout += new LayoutEventHandler(this.WinCustomMaskedTextBox_Layout);
            this.TextChanged += new EventHandler(this.WinCustomMaskedTextBox_TextChanged);

            this.Enter += new EventHandler(this.WinCustomMaskedTextBox_Enter);
            this.Validating += new CancelEventHandler(this.WinCustomMaskedTextBox_Validating);
            this.Validated += new EventHandler(this.WinCustomMaskedTextBox_Validated);
            this.Leave += new EventHandler(this.WinCustomMaskedTextBox_Leave);

            this.KeyDown += new KeyEventHandler(this.WinCustomMaskedTextBox_KeyDown);
            this.KeyUp += new KeyEventHandler(this.WinCustomMaskedTextBox_KeyUp);

            this.MouseDown += new MouseEventHandler(this.WinCustomMaskedTextBox_MouseDown);

            this.ResumeLayout(false);

        }

        #endregion

        #region プロパティ拡張

        #region 変更イベントや、変更通知

        // Text2、3プロパティの変更イベントは実装しないTextプロパティのものを使用する。

        /// <summary>変更通知イベント（汎用）</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>変更通知イベント発生（汎用）</summary>
        /// <param name="propertyName">プロパティ名</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        /// <summary>Textプロパティにテキスト変更時の変更通知機能を追加実装する。</summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                if (Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") != -1)
                {
                    // Binding（Target_Validate）から呼ばれた時は無視。
                    return;
                }

                base.Text = value;

                // ここでは、直接入力やbase.Textの呼び出しの際に変更されないため、
                // 実装箇所を、WinCustomMaskedTextBox_TextChangedに変更する。

                //// Text2、3プロパティの変更通知
                //this.NotifyPropertyChanged("Text2");
                ////this.NotifyPropertyChanged("Text3");
            }
        }

        /// <summary>
        /// TextMaskFormatは初期値、IncludeLiteralsのままReadonlyに変更。
        /// このため、Textプロパティでは必ずマスク適用時の値を取得する。
        /// </summary>
        [Category("動作"),
        Description("TextMaskFormatは初期値、IncludeLiteralsのままReadonlyに。")]
        public new MaskFormat TextMaskFormat
        {
            get
            {
                return ((MaskedTextBox)this).TextMaskFormat;
            }
        }

        /// <summary>Text2プロパティではマスクを除いた値を取得する。</summary>
        /// <remarks>Bindingsで使用可能なようにset_Text2を用意した。</remarks>
        [Category("表示"),
        Description("Text2プロパティではマスクを除いた値を設定・取得する。")]
        public string Text2
        {
            get
            {
                // ExcludePromptAndLiteralsの値を取る。
                MaskedTextBox mtb = new MaskedTextBox();
                mtb.Mask = base.Mask;
                mtb.Text = base.Text;
                mtb.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                return mtb.Text;
            }

            set
            {
                if (Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") != -1)
                {
                    // Binding（Target_Validate）から呼ばれた時は無視。
                    return;
                }

                // ユーザ入力
                base.Text = value;
            }
        }

        /// <summary>Text3プロパティでは表示時マスク適用時の値を取得する。</summary>
        [Category("表示"),
        Description("Text3プロパティでは表示時マスク適用時の値を取得する。")]
        public string Text3
        {
            get
            {
                if (!string.IsNullOrEmpty(this.OriginalMask))
                {
                    // 表示時マスク有り（入力中）
                    MaskedTextBox mtb = new MaskedTextBox(this.OriginalMask);
                    mtb.Text = this.Text2;

                    // 表示時マスク適用時の値
                    mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                    return mtb.Text;
                }
                else if (!string.IsNullOrEmpty(this.Mask))
                {
                    // 表示時マスク有り（入力後）
                    MaskedTextBox mtb = new MaskedTextBox(this.Mask);
                    mtb.Text = this.Text2;

                    // 表示時マスク適用時の値
                    mtb.TextMaskFormat = MaskFormat.IncludeLiterals;
                    return mtb.Text;
                }
                else
                {
                    // マスクなし。
                    return base.Text;
                }
            }
        }

        #endregion

        #region 値取得（IGetValue）

        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        public DateTime GetDateTime()
        {
            return DateTime.Parse(base.Text);
        }

        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <param name="provider">書式</param>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        public DateTime GetDateTime(IFormatProvider provider)
        {
            return DateTime.Parse(base.Text, provider);
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
            return DateTime.Parse(base.Text, provider, styles);
        }

        /// <summary>
        /// Text値をDecimal型にキャストして返す。
        /// </summary>
        /// <returns>Decimal値</returns>
        [DebuggerStepThrough]
        public decimal GetDecimal()
        {
            return decimal.Parse(this.DeleteFigure());
        }

        /// <summary>
        /// Text値をDouble型にキャストして返す。
        /// </summary>
        /// <returns>Double値</returns>
        [DebuggerStepThrough]
        public double GetDouble()
        {
            return double.Parse(this.DeleteFigure());
        }

        /// <summary>
        /// Text値をFloat型にキャストして返す。
        /// </summary>
        /// <returns>Float値</returns>
        [DebuggerStepThrough]
        public float GetFloat()
        {
            return float.Parse(this.DeleteFigure());
        }

        /// <summary>
        /// Text値をInt16型にキャストして返す。
        /// </summary>
        /// <returns>Int16値</returns>
        [DebuggerStepThrough]
        public short GetInt16()
        {
            return short.Parse(this.DeleteFigure());
        }

        /// <summary>
        /// Text値をInt32型にキャストして返す。
        /// </summary>
        /// <returns>Int32値</returns>
        [DebuggerStepThrough]
        public int GetInt32()
        {
            return int.Parse(this.DeleteFigure());
        }


        /// <summary>
        /// Text値をInt64型にキャストして返す。
        /// </summary>
        /// <returns>Int64値</returns>
        [DebuggerStepThrough]
        public long GetInt64()
        {
            return long.Parse(this.DeleteFigure());
        }

        /// <summary>桁区切り文字を削除</summary>
        /// <returns>桁区切り文字削除後の文字列</returns>
        private string DeleteFigure()
        {
            // 区切りを削除（数値用途限定）
            // マスクでこの処理でＯＫにしておく前提。
            // 例えば「,,,1,,,1,,,」でも、「11」になるので。
            return base.Text.Replace(",", "");
        }

        #endregion

        #region デザインタイム・プロパティ

        #region チェック プロパティ

        /// <summary>Validatingイベントでチェックする</summary>
        /// [DefaultValue(false),
        [DefaultValue(false),
        Category("Check"),
        Description("Validatingイベントでチェックするかどうか")]
        public bool CheckValidating { get; set; }

        /// <summary>入力文字種チェック</summary>
        private CheckType _checkType = new CheckType();

        /// <summary>入力文字種チェック</summary>
        [Category("Check"),
        Description("入力文字種チェック")]
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
        [DefaultValue(""),
        Category("Check"),
        Description("正規表現チェック")]
        public string CheckRegExp { get; set; }

        /// <summary>正規表現チェック</summary>
        [DefaultValue(false),
        Category("Check"),
        Description("禁則文字チェック")]
        public bool CheckProhibitedChar { get; set; }

        #endregion

        #region 編集プロパティ

        /// <summary>初期値編集</summary>
        private EditInitialValue _editInitialValue = EditInitialValue.Blank;

        /// <summary>初期値編集</summary>
        [Category("Edit"),
        Description("初期値編集")]
        public EditInitialValue EditInitialValue
        {
            get
            {
                return this._editInitialValue;
            }
            set
            {
                this._editInitialValue = value;
            }
        }

        /// <summary>初期値編集のチェックのデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeEditInitialValue()
        {
            return this._editInitialValue != EditInitialValue.Blank;
        }

        /// <summary>オリジナルのマスク</summary>
        /// <remarks>退避しておくためのワーク</remarks>
        internal string OriginalMask { get; private set; }

        /// <summary>入力中のマスク</summary>
        [Category("Edit"),
        Description("入力中のマスク")]
        public string Mask_Editing { get; set; }

        /// <summary>半角編集</summary>
        [DefaultValue(false),
        Category("Edit"),
        Description("半角指定（マスクで指定できないため）")]
        public bool EditToHankaku { get; set; }

        /// <summary>日付編集</summary>
        [DefaultValue(false),
        Category("Edit"),
        Description("YYYYMMDDのM、Dが１桁の時に補正処理を行う。")]
        public bool EditToYYYYMMDD { get; set; }
        
        #endregion

        #endregion

        #region イベント系

        // ＜イベントの説明＞
        // TextBox1からTextBox2にフォーカスを移動したときのイベントの発生順序。
        // http://homepage1.nifty.com/rucio/main/dotnet/shokyu/standard23.htm

        // マウス、またはFocusメソッドでフォーカスを移動する場合
        // -----------------------------------------------------
        // TextBox1.LostFocus → TextBox1.Leave
        // → TextBox1.Validating → TextBox1.Validated
        // → TextBox2.Enter → TextBox2.GotFocus

        // その他の方法でフォーカスを移動する場合
        // -----------------------------------------------------
        // TextBox1.Leave → TextBox1.Validating → TextBox1.Validated
        // → TextBox2.Enter → TextBox1.LostFocus → TextBox2.GotFocus

        // ★共通する法則★
        // -----------------------------------------------------
        // TextBox1.Leave → TextBox1.Validating → TextBox1.Validated → TextBox2.Enter
        // TextBox1.Enter → TextBox1.Leave → TextBox1.Validating → TextBox1.Validated

        #region 初期化処理（Layout）

        /// <summary>初期化処理</summary>
        private void WinCustomMaskedTextBox_Layout(object sender, EventArgs e)
        {
            if (!RcFxCmnFunction.IsDesignMode())
            {
                // デザイン・モードでは無い場合。
                if (base.Text == ""
                    && this.CheckType.IsNumeric
                    && this.EditInitialValue == EditInitialValue.Zero)
                {
                    // 「0」初期化
                    base.Text = "0";
                    this.ReEdit();
                }
            }
        }

        #endregion

        #region チェック処理（Validating、Validated）

        /// <summary>背景色（バックアップ）</summary>
        private Color? _backupBkColor = null;

        /// <summary>チェック処理</summary>
        private void WinCustomMaskedTextBox_Validating(object sender, CancelEventArgs e)
        {
            // チェック前の変換処理
            this.PreValidate();

            // Validatingイベントでチェックするしない。
            if (this.CheckValidating)
            {
                // Validatingイベントでチェックする。
                if (!this.Validate())
                {
                    //// 数値チェックエラー時はクリア
                    //if (this.HasNumericCheckError)
                    //{
                    //    if (this.EditInitialValue == EditInitialValue.Blank)
                    //    {
                    //        // 空文字列クリア
                    //        base.Text = "";
                    //    }
                    //    else
                    //    {
                    //        // 「0」クリア
                    //        base.Text = "0";
                    //    }
                    //}
                }
            }
            else
            {
                // Validatingイベントでチェックしない。
            }
        }

        /// <summary>チェック後処理</summary>
        private void WinCustomMaskedTextBox_Validated(object sender, EventArgs e)
        {
            // 編集処理
            this.ReEdit();
        }

        #endregion

        #region フォーマット処理（Enter、Leave、TextChanged）

        /// <summary>MouseDown状態の確認用フラグ</summary>
        private bool IsMouseDown = false;

        /// <summary>マウスが入った</summary>
        private void WinCustomMaskedTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.IsMouseDown = true;
        }

        /// <summary>フォーカス Enter</summary>
        private void WinCustomMaskedTextBox_Enter(object sender, EventArgs e)
        {
            // 編集処理
            this.Edit();

            if (!this.IsMouseDown)
            {
                // MouseDown状態で無ければ全選択
                this.BeginInvoke(new MethodInvoker(MethodInvokerDelegate_SelectAll));
            }
        }

        /// <summary>（MethodInvoker）delegate廃止（VB化時に問題）</summary>
        private void MethodInvokerDelegate_SelectAll()
        {
            this.SelectAll();
        }

        /// <summary>ロスト フォーカス</summary>
        private void WinCustomMaskedTextBox_Leave(object sender, EventArgs e)
        {
            this.IsMouseDown = false;
        }

        /// <summary>テキスト変更時</summary>
        private void WinCustomMaskedTextBox_TextChanged(object sender, EventArgs e)
        {
            // Text2、3プロパティの変更通知
            this.NotifyPropertyChanged("Text2");
            //this.NotifyPropertyChanged("Text3");
        }

        #endregion

        #region フィルタ処理（KeyPress）

        //// <summary>フィルタ処理</summary>
        //private void WinCustomMaskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    ・・・
        //}

        //↓Maskの場合はKeyPressに行かない・・・

        /// <summary>ProcessCmdKey</summary>
        /// <param name="msg">ウィンドウ メッセージ</param>
        /// <param name="keyData">Keys</param>
        /// <returns>
        /// 文字がコントロールによって
        /// ・処理された場合は true。
        /// ・それ以外の場合は false
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 数値指定がある場合
            if (this.CheckType.IsNumeric)
            {
                if (keyData == Keys.OemPeriod || keyData == Keys.Decimal)
                {
                    // 「.」キー

                    // 数値に'.'が入力済み
                    if (base.Text.IndexOf('.') != -1)
                    {
                        // '.'キーの無効化
                        return true;
                    }

                    // 入力を許可
                }
                else if (keyData == Keys.OemMinus || keyData == Keys.Subtract)
                {
                    // 「-」キー

                    if (this.SelectionStart == 0)
                    {
                        // 先頭
                        if (this.SelectionLength == 0)
                        {
                            // 選択でない
                            // 数値に'-'が入力済み
                            if (base.Text.IndexOf('-') != -1)
                            {
                                // '-'キーの無効化
                                return true;
                            }
                        }
                        else
                        {
                            // 選択
                            // 先頭の-を選択している筈なので、
                            // '-'を入力可能とする。
                        }
                    }
                    else
                    {
                        // 先頭以外
                        // '-'キーの無効化
                        return true;
                    }

                    // 入力を許可
                }
                else if (keyData == Keys.ProcessKey
                    || keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)
                    || keyData == Keys.Back || keyData == Keys.Delete
                    || keyData == Keys.Home || keyData == Keys.End
                    || keyData == Keys.Left || keyData == Keys.Right
                    || keyData == (Keys.Left | Keys.Shift) || keyData == (Keys.Right | Keys.Shift)
                    || (keyData >= Keys.D0 && keyData <= Keys.D9)
                    || (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9)
                    //|| keyData == (Keys.Oemplus | Keys.Shift) || keyData == Keys.Add
                    || keyData == Keys.Oemcomma)
                {
                    // ・「漢字モード」、
                    // ・「TAB」キー、「SHIFT＋TAB」キー、
                    // ・「BSP・DEL」、「HOME・END」
                    // ・「←・→」キー、「SHIFT＋←・→」（選択）キー
                    // ・「0-9」、「+」（除外）、「,」キー

                    // 入力を許可
                }
                else
                {
                    // 数値以外キーの無効化
                    return true;
                }
            }

            // 入力を許可
            return false;
        }

        #endregion

        #region クリア処理（KeyDown）

        /// <summary>クリア処理</summary>
        private void WinCustomMaskedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // ここではダメでした。
        }

        #endregion

        #region 復元処理（KeyUp）

        /// <summary>０の復元</summary>
        private void WinCustomMaskedTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // 無限ループ対応
            string text = base.Text;

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                // DELキーとBackSpaceキー

                // クリアされてしまった。
                if (text == "")
                {
                    // 初期化
                    if (this.InitializeValue(ref text))
                    {
                        // DELキーとBackSpaceキーの無効化
                        e.Handled = true;
                        // Textプロパティをクリア。
                        base.Text = text;
                        // 選択状態
                        this.SelectAll();
                    }
                }
            }
            else
            {
                // なにもしない。
            }
        }

        #endregion

        #endregion

        #region Validate

        /// <summary>チェック前の変換処理</summary>
        public void PreValidate()
        {
            // 生入力
            string txt = this.Text2;

            // 半角指定（マスクで指定できないため）
            if (this.EditToHankaku)
            {
                // ワーク
                string temp = "";
                StringBuilder sb = new StringBuilder();

                // 半角化する。
                temp = Public.Str.StringConverter.ToHankaku(txt);

                // 残っている全角文字を削る。
                foreach (char ch in temp)
                {
                    if (StringChecker.IsHankaku(ch.ToString()))
                    {
                        // 半角だけ追加する。
                        sb.Append(ch);
                    }
                }

                if (txt != sb.ToString())
                {
                    // 変更された場合は再設定
                    txt = sb.ToString();
                    base.Text = txt;
                }
            }

            // YYYYMMDDのM、Dが１桁の時に補正処理を行う。
            if (this.EditToYYYYMMDD)
            {
                if (Public.Str.StringConverter.EditYYYYMMDDString(ref txt))
                {
                    // 変更された場合は再設定
                    base.Text = txt;
                }
            }
        }

        ///// <summary>数値チェックエラーの有無</summary>
        ///// <remarks>
        ///// true：数値チェックエラー有
        ///// false：数値チェックエラー無
        ///// </remarks>
        //private bool HasNumericCheckError = false;

        /// <summary>チェック処理</summary>
        /// <returns>
        /// ・エラーなし：true
        /// ・エラーあり：false
        /// </returns>
        /// <remarks>
        /// マスク前の値をチェックする
        /// ・必須入力チェック
        /// ・数値チェック
        /// 
        /// マスク後の値をチェックする
        /// ・半角チェック
        /// ・全角チェック
        /// ・片仮名チェック
        /// ・半角片仮名チェック
        /// ・平仮名チェック
        /// ・日付チェック
        /// ・正規表現チェック
        /// ・禁則文字チェック
        /// </remarks>
        public bool Validate()
        {
            //// 初期化
            //this.HasNumericCheckError = false;

            string[] temp;
            bool ret = this.Validate(out temp);

            //foreach (string s in temp)
            //{
            //    if (s == CmnCheckFunction.IsNumericCheckErrorMessage)
            //    {
            //        // 数値チェックエラー
            //        this.HasNumericCheckError = true;
            //    }
            //}

            return ret;
        }

        /// <summary>編集中 or 編集後テキストを返す。</summary>
        /// <param name="editingText">編集中テキスト</param>
        /// <param name="editedText">編集後テキスト</param>
        private void GetTexts(out string editingText, out string editedText)
        {
            // マスク後の値を取得
            MaskedTextBox mtb = null;

            // どちらのマスクを適用するか。
            if (this.OriginalMask == null || this.OriginalMask == "")
            {
                // ReEdit後の場合
                if (this.Mask_Editing == null || this.Mask_Editing == "")
                {
                    // この場合は、何も設定されていないので、
                    editingText = base.Text; // ＝そのまま
                    editedText = base.Text; // ＝そのまま
                }
                else
                {
                    // 編集中マスクを適用
                    mtb = new MaskedTextBox(this.Mask_Editing);
                    mtb.Text = this.Text2;
                    editingText = mtb.Text;

                    // ＝そのまま
                    editedText = base.Text;
                }
            }
            else
            {
                // ReEdit前の場合

                // 編集後マスクを適用
                mtb = new MaskedTextBox(this.OriginalMask);
                mtb.Text = this.Text2;
                editedText = mtb.Text;

                // ＝そのまま
                editingText = base.Text;
            }
        }

        /// <summary>チェック処理</summary>
        /// <param name="result">結果文字列</param>
        /// <returns>
        /// ・エラーなし：true
        /// ・エラーあり：false
        /// </returns>
        /// <remarks>
        /// マスク前の値をチェックする
        /// ・必須入力チェック
        /// ・数値チェック
        /// 
        /// マスク後の値をチェックする
        /// ・半角チェック
        /// ・全角チェック
        /// ・片仮名チェック
        /// ・半角片仮名チェック
        /// ・平仮名チェック
        /// ・日付チェック
        /// ・正規表現チェック
        /// ・禁則文字チェック
        /// </remarks>
        public bool Validate(out string[] result)
        {
            // フラグ
            bool hasError = false;
            // ワーク
            List<string> lstRet = new List<string>();

            // 生入力
            string text = this.Text2;
            // 編集中
            string editingText = "";
            // 編集後
            string editedText = "";
            // 編集中・編集後
            this.GetTexts(out editingText, out editedText);

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
                    // 編集後を使用してチェック
                    // 編集中・編集後（再取得）
                    this.GetTexts(out editingText, out editedText);

                    DateTime dateTime;
                    if (!DateTime.TryParse(editedText, out dateTime)
                        || text.IndexOfAny(new char[] { ' ', '　' }) != -1)
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
                if (this._backupBkColor == null)
                {
                    this._backupBkColor = (Color?)this.BackColor;
                    this.BackColor = Color.Red;
                }
            }
            else
            {
                // 正常時の背景色
                if (this._backupBkColor != null)
                {
                    this.BackColor = (Color)this._backupBkColor;
                    this._backupBkColor = null;
                }
            }

            result = lstRet.ToArray();
            return !hasError;
        }

        ///// <summary>
        ///// Formを閉じる時だけValidatingイベント内での検証をやめる方法
        ///// http://d.hatena.ne.jp/NAL-6295/20081015/p1
        ///// </summary>
        //protected override void OnValidating(CancelEventArgs e)
        //{
        //    Control parentControl = this.Parent;

        //    while (parentControl != null)
        //    {
        //        if (parentControl is Form)
        //        {
        //            // Formの場合
        //            if (parentControl.CausesValidation)
        //            {
        //                // Formの検証が必要なので、
        //                // Validatingイベントを呼ぶ。
        //                break;
        //            }
        //            else
        //            {
        //                // Formの検証が不要
        //                if ((parentControl as Form).ActiveControl == this)
        //                {
        //                    // 自身がアクティブである場合、
        //                    // Validatingイベントを呼ばない。
        //                    return;
        //                }
        //                else if (!(parentControl as Form).ActiveControl.CausesValidation)
        //                {
        //                    // アクティブな自分以外の
        //                    // （親）コントロールが
        //                    // 検証を必要としていない場合、
        //                    // Validatingイベントを呼ばない。
        //                    return;
        //                }
        //                else
        //                {
        //                    // 次へ or 抜ける。
        //                }
        //            }
        //        }

        //        // 遡る。
        //        parentControl = parentControl.Parent;
        //    }

        //    // Validatingイベントを呼ぶ。
        //    base.OnValidating(e);
        //}

        #endregion

        #region 編集

        /// <summary>編集されたかどうか</summary>
        /// <remarks>
        /// true：編集された
        /// false：編集されていない
        /// </remarks>
        protected bool Edited = false;

        /// <summary>編集</summary>
        internal void Edit()
        {
            if (!this.Edited)
            {
                // falseの時、１回だけ
                this.Edited = true;
                this.OriginalMask = this.Mask;
                this.Mask = this.Mask_Editing;
            }
        }

        /// <summary>編集（逆）</summary>
        internal void ReEdit()
        {
            // フォーマット変更
            if (this.Edited)
            {
                // trueの時、１回だけ
                this.Edited = false;
                this.Mask = this.OriginalMask;
                this.OriginalMask = "";
            }
        }

        /// <summary>値を初期化する</summary>
        /// <param name="text">初期値</param>
        /// <returns>
        /// true：初期化された。
        /// false：初期化されていない。
        /// </returns>
        private bool InitializeValue(ref string text)
        {
            // 数値の場合
            if (this.CheckType.IsNumeric)
            {
                // 数値の場合

                if (this.EditInitialValue == EditInitialValue.Blank)
                {
                    // 空文字列クリア
                    text = "";
                    return true;
                }
                else if (this.EditInitialValue == EditInitialValue.Zero)
                {
                    // 「0」クリア
                    text = "0";
                    return true;
                }
                else
                {
                    //・・・必要に応じてカスタマイズ可能・・・
                }
            }
            else
            {
                // 数値以外の場合
            }

            return false;
        }

        #endregion
    }
}
