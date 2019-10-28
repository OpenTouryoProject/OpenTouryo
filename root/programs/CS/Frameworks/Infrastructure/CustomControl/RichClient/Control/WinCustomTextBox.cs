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
//* クラス名        ：WinCustomTextBox
//* クラス日本語名  ：テキスト ボックス（Win）のカスタム・コントロール（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2016/01/28  Sai               Corrected IsIndispensabile property spelling
//*  2017/01/31  西野 大介         "Indispensable" ---> "Required"
//*  2017/01/31  西野 大介         Obsolete of String.Copy.
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
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.CustomControl.RichClient
{
    /// <summary>System.Windows.Forms.TextBoxのカスタム・コントロール</summary>
    [DefaultProperty("Text")]
    [Designer(typeof(WinCustomTextBoxDesigner))]
    public class WinCustomTextBox : TextBox, ICheck, IEdit, IGetValue, INotifyPropertyChanged
    {
        ///<summary>デザイナ上の表示をカスタマイズするインナークラス</summary>
        ///<remarks>
        ///自作コントロールにおいて、不要なプロパティをデザイナANDインテリセンスで隠したい時
        ///http://jehupc.exblog.jp/8157762/
        ///継承と属性プログラミングで実現するRAD開発 － ＠IT
        ///http://www.atmarkit.co.jp/fdotnet/winexp/winexp02/winexp02_03.html
        ///</remarks>
        internal class WinCustomTextBoxDesigner : ControlDesigner
        {
            ///<summary>下記で指定してあるプロパティはデザイナでは非表示とする。</summary>
            protected override void PostFilterProperties(IDictionary Properties)
            {
                Properties.Remove("Text2");
                Properties.Remove("Text3");
                Properties.Remove("Value");
            }
        }

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public WinCustomTextBox()
        {
            this.InitializeComponent();
        }

        /// <summary>初期化</summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WinCustomTextBox
            // 
            this.Layout += new LayoutEventHandler(this.WinCustomTextBox_Layout);
            //this.TextChanged += new EventHandler(this.WinCustomTextBox_TextChanged);

            this.Enter += new EventHandler(this.WinCustomTextBox_Enter);
            this.Validating += new CancelEventHandler(this.WinCustomTextBox_Validating);
            this.Validated += new EventHandler(this.WinCustomTextBox_Validated);
            this.Leave += new EventHandler(this.WinCustomTextBox_Leave);

            this.KeyDown += new KeyEventHandler(this.WinCustomTextBox_KeyDown);
            this.KeyUp += new KeyEventHandler(this.WinCustomTextBox_KeyUp);

            this.MouseDown += new MouseEventHandler(this.WinCustomTextBox_MouseDown);

            this.ResumeLayout(false);

        }

        #endregion

        #region プロパティ拡張

        #region 変更イベントや、変更通知

        /// <summary>Valueプロパティの変更イベント</summary>
        private EventHandler _OnValueChanged;

        /// <summary>Valueプロパティの変更イベント・ハンドラ</summary>
        [Description("Valueプロパティの変更イベント・ハンドラ")]
        public event EventHandler ValueChanged
        {
            add
            {
                this._OnValueChanged += value;
            }
            remove
            {
                this._OnValueChanged -= value;
            }
        }

        /// <summary>ValueChangedイベントを発生させる</summary>
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (this._OnValueChanged != null)
            {
                this._OnValueChanged(this, e);
            }
        }

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

        /// <summary>Textプロパティにテキスト変更時の編集機能を追加実装する。</summary>
        public override string Text
        {
            set
            {
                //// これは、DataGridViewTextBoxCell.InitializeEditingControl経由のset_Textを無視するためのコード
                //// 解り難いので、Debugログより前に処理する。
                //if (Environment.StackTrace.IndexOf("_83B12F0F_CEA3_4f93_9233_B86EFA149BB2") != -1)
                //{
                //    return;
                //}

                //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + value.ToString());
                //Debug.WriteLine(Environment.StackTrace);

                if (Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") != -1)
                {
                    // Binding（Target_Validate）から呼ばれた時は無視。
                    return;
                }

                // まず設定。
                base.Text = value;

                if (this.Focused)
                {
                    // フォーカスがある場合は編集（ReEdite）しない。
                }
                else if (Environment.StackTrace.IndexOf("System.Windows.Forms.DataGridView.InitializeEditingControlValue") != -1)
                {
                    // DataGridView（InitializeEditingControlValue）から呼ばれた時は編集（ReEdite）しない。
                }
                else
                {
                    // 上記以外は編集（ReEdite）する。
                    this.ReEdit();
                }
            }
        }

        /// <summary>Text2プロパティ</summary>
        [Category("表示"),
        Description("ユーザ入力のTextプロパティ")]
        public string Text2
        {
            get
            {
                // コピーを作成
                WinCustomTextBox wctxt = new WinCustomTextBox();

                // 編集属性を指定
                wctxt.MaxLength = this.MaxLength;

                wctxt.IsNumeric = this.IsNumeric;
                wctxt.EditInitialValue = this.EditInitialValue;
                wctxt.EditAddFigure = this.EditAddFigure;
                wctxt.EditPadding = this.EditPadding;
                wctxt.EditDigitsAfterDP = this.EditDigitsAfterDP;
                wctxt.EditDigitsAfterDP_Editing = this.EditDigitsAfterDP_Editing;

                //wctxt.DisplayUnits = this.DisplayUnits;

                // 編集して返す。
                wctxt.Text = base.Text;
                wctxt.Edit();
                return wctxt.Text;
            }

            set
            {
                if (Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") != -1)
                {
                    // Binding（Target_Validate）から呼ばれた時は無視。
                    return;
                }

                // コピーを作成
                WinCustomTextBox wctxt = new WinCustomTextBox();

                // 編集属性を指定
                wctxt.MaxLength = this.MaxLength;

                wctxt.IsNumeric = this.IsNumeric;
                wctxt.EditInitialValue = this.EditInitialValue;
                wctxt.EditAddFigure = this.EditAddFigure;
                wctxt.EditPadding = this.EditPadding;
                wctxt.EditDigitsAfterDP = this.EditDigitsAfterDP;
                wctxt.EditDigitsAfterDP_Editing = this.EditDigitsAfterDP_Editing;

                //wctxt.DisplayUnits = this.DisplayUnits;

                // 編集してTextに設定する。
                wctxt.Text = value;
                wctxt.Edit();
                base.Text = wctxt.Text;
            }
        }

        /// <summary>Text3プロパティ</summary>
        [Category("表示"),
        Description("編集処理込のTextプロパティ")]
        public string Text3
        {
            get
            {
                // コピーを作成
                WinCustomTextBox wctxt = new WinCustomTextBox();

                // 編集属性を指定
                wctxt.MaxLength = this.MaxLength;

                wctxt.IsNumeric = this.IsNumeric;
                wctxt.EditInitialValue = this.EditInitialValue;
                wctxt.EditAddFigure = this.EditAddFigure;
                wctxt.EditPadding = this.EditPadding;
                wctxt.EditDigitsAfterDP = this.EditDigitsAfterDP;
                wctxt.EditDigitsAfterDP_Editing = this.EditDigitsAfterDP_Editing;

                //wctxt.DisplayUnits = this.DisplayUnits;

                // 編集して返す。
                wctxt.Text = base.Text;
                //wctxt.ReEdit(); // .TextでReEditされるので。
                return wctxt.Text;
            }

            set
            {
                if (Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") != -1)
                {
                    // Binding（Target_Validate）から呼ばれた時は無視。
                    return;
                }

                // コピーを作成
                WinCustomTextBox wctxt = new WinCustomTextBox();

                // 編集属性を指定
                wctxt.MaxLength = this.MaxLength;

                wctxt.IsNumeric = this.IsNumeric;
                wctxt.EditInitialValue = this.EditInitialValue;
                wctxt.EditAddFigure = this.EditAddFigure;
                wctxt.EditPadding = this.EditPadding;
                wctxt.EditDigitsAfterDP = this.EditDigitsAfterDP;
                wctxt.EditDigitsAfterDP_Editing = this.EditDigitsAfterDP_Editing;

                //wctxt.DisplayUnits = this.DisplayUnits;

                // 編集してTextに設定する。
                wctxt.Text = value;
                //wctxt.ReEdit(); // .TextでReEditされるので。
                base.Text = wctxt.Text;
            }
        }

        /// <summary>単位変換に対応したValueプロパティの実体値（退避用）</summary>
        /// <remarks>
        /// ポイント
        /// ・内部から触る値なので、
        /// ・string型であること。
        /// ・単位変換後の値とする。
        /// </remarks>
        private string _Value = "";

        /// <summary>単位変換に対応したValueプロパティ</summary>
        /// <remarks>内部からは触らない値</remarks>
        [Category("表示"),
        Description("単位変換に対応したValueプロパティ")]
        public object Value
        {
            get
            {
                //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + this._Value.ToString());
                //Debug.WriteLine(Environment.StackTrace);

                if (string.IsNullOrEmpty((string)this._Value))
                {
                    // 空文字列の場合は、DBNullを返す。
                    return DBNull.Value;
                }
                else
                {
                    // 単位変更して戻す（これではフォーットが崩れる問題あり）。
                    return this.FromKMGT(this._Value);
                }
            }

            set
            {
                //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + value.ToString());
                //Debug.WriteLine(Environment.StackTrace);

                if (Environment.StackTrace.IndexOf("System.Windows.Forms.Binding.Target_Validate") != -1)
                {
                    // Binding（Target_Validate）から呼ばれた時は無視。
                    return;
                }

                //　型変換（直ちにstring型に）
                string temp;
                if (value == null)
                {
                    // nullは、空文字列
                    temp = "";
                }
                else if (value.GetType() == typeof(DBNull))
                {
                    // DBNullは、空文字列
                    temp = "";
                }
                else
                {
                    // その他は、ToString()
                    temp = value.ToString(); // 指数問題はToKMGT内で解決
                }

                // 単位変換し、
                temp = this.ToKMGT(temp);

                // Textへ設定して、
                base.Text = temp;

                // Valueにも退避
                if (this._Value == temp)
                {
                    // なにもしない。
                }
                else
                {
                    // 変更された値の反映
                    this._Value = temp;
                    // Valueプロパティの変更通知
                    this.OnValueChanged(EventArgs.Empty);
                    this.NotifyPropertyChanged("Value");
                }

                if (this.Focused)
                {
                    // フォーカスがある場合は編集（ReEdite）しない。
                }
                else
                {
                    // 上記以外は編集（ReEdite）する。
                    this.ReEdit();
                }
            }
        }

        /// <summary>単位変換</summary>
        /// <param name="value">値</param>
        /// <returns>単位を変更し文字列化した値</returns>
        private string ToKMGT(string value)
        {
            double dbl = 0;
            string ret = "";

            if (this._displayUnits != null)
            {
                if (double.TryParse(value, out dbl))
                {
                    // 単位を変更
                    dbl = (dbl / Math.Pow(10, (int)this._displayUnits));
                    // 小数点以下30桁の精度まで保証
                    ret = dbl.ToString("F30");
                    // 余分な0を削除（第二引数はダミー
                    ret = this.DeleteZeroAfterDP(ret, new EditDigitsAfterDP(CutMethod._4sya5nyu, 1));
                }
                else
                {
                    // エラー時、初期化
                    this.InitializeValue(ref ret);
                }
            }
            else
            {
                // そのまま
                ret = value;
            }

            return ret;
        }

        /// <summary>単位変換（逆）</summary>
        /// <param name="value">値</param>
        /// <returns>単位を戻し文字列化した値</returns>
        private string FromKMGT(string value)
        {
            double dbl = 0;
            string ret = "";

            if (this._displayUnits != null)
            {
                if (double.TryParse(value, out dbl))
                {
                    // 単位を変更、初期化
                    dbl = (dbl * System.Math.Pow(10, (int)this._displayUnits));
                    // 小数点以下30桁の精度まで保証
                    ret = dbl.ToString("F30");
                    // 余分な0を削除（第二引数はダミー
                    ret = this.DeleteZeroAfterDP(ret, new EditDigitsAfterDP(CutMethod._4sya5nyu, 1));
                }
                else
                {
                    // エラー時、初期化
                    this.InitializeValue(ref ret);
                }
            }
            else
            {
                // そのまま
                ret = value;
            }

            return ret;
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
            if (this.EditAddFigure != EditAddFigure.None)
            {
                // EditAddFigure.Noneでない場合
                return base.Text.Replace(",", "");
            }
            else
            {
                // EditAddFigure.Noneの場合
                return base.Text;
            }
        }

        #endregion

        #region デザインタイム・プロパティ

        /// <summary>入力制限専用</summary>
        [DefaultValue(false),
        Category("Edit"),
        Description("入力制限専用")]
        public bool IsNumeric { get; set; }

        #region チェック プロパティ

        /// <summary>Validatingイベントでチェックする</summary>
        /// [DefaultValue(false),
        [DefaultValue(false),
        Category("Check"),
        Description("Validatingイベントでチェックするかどうか")]
        public bool CheckValidating { get; set; }

        //---

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

        //---

        /// <summary>正規表現チェック</summary>
        [DefaultValue(""),
        Category("Check"),
        Description("正規表現チェック")]
        public string CheckRegExp { get; set; }

        //---

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

        /// <summary>初期値編集のデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeEditInitialValue()
        {
            return this._editInitialValue != EditInitialValue.Blank;
        }

        //---

        /// <summary>桁区切り編集</summary>
        private EditAddFigure _editAddFigure = EditAddFigure.None;

        /// <summary>桁区切り編集</summary>
        [Category("Edit"),
        Description("桁区切り編集")]
        public EditAddFigure EditAddFigure
        {
            get
            {
                return this._editAddFigure;
            }
            set
            {
                this._editAddFigure = value;
            }
        }

        /// <summary>桁区切り編集のデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeEditAddFigure()
        {
            return this._editAddFigure != EditAddFigure.None;
        }

        //---

        /// <summary>文字埋め編集</summary>
        private EditPadding _editPadding = new EditPadding(PadDirection.None, null);

        /// <summary>文字埋め編集</summary>
        [Category("Edit"),
        Description("文字埋め編集")]
        public EditPadding EditPadding
        {
            get
            {
                return this._editPadding;
            }
            set
            {
                this._editPadding = value;
            }
        }

        /// <summary>文字埋め編集のデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeEditPadding()
        {
            return this._editPadding != new EditPadding(PadDirection.None, null);
        }

        //---

        /// <summary>小数点以下編集（入力後）</summary>
        private EditDigitsAfterDP _editDigitsAfterDP = new EditDigitsAfterDP(CutMethod.None, 0);

        /// <summary>小数点以下編集（入力後）</summary>
        [Category("Edit"),
        Description("小数点以下ｘ桁編集（入力後）")]
        public EditDigitsAfterDP EditDigitsAfterDP
        {
            get
            {
                return this._editDigitsAfterDP;
            }
            set
            {
                this._editDigitsAfterDP = value;
            }
        }

        /// <summary>小数点以下編集（入力後）のデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeEditDigitsAfterDP()
        {
            return this._editDigitsAfterDP != new EditDigitsAfterDP(CutMethod.None, 0);
        }

        //---

        /// <summary>小数点以下編集（入力中）</summary>
        private EditDigitsAfterDP _editDigitsAfterDP_Editing = new EditDigitsAfterDP(CutMethod.None, 0);

        /// <summary>小数点以下編集（入力中）</summary>
        [Category("Edit"),
        Description("小数点以下ｘ桁編集（入力中）")]
        public EditDigitsAfterDP EditDigitsAfterDP_Editing
        {
            get
            {
                return this._editDigitsAfterDP_Editing;
            }
            set
            {
                this._editDigitsAfterDP_Editing = value;
            }
        }

        /// <summary>小数点以下編集（入力中）のデフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeEditDigitsAfterDP_Editing()
        {
            return this._editDigitsAfterDP_Editing != new EditDigitsAfterDP(CutMethod.None, 0);
        }

        // ---

        /// <summary>単位編集</summary>
        private uint? _displayUnits = null;

        /// <summary>単位編集</summary>
        [Category("Edit"),
        Description("単位編集")]
        public uint? DisplayUnits
        {
            get
            {
                return this._displayUnits;
            }
            set
            {
                this._displayUnits = value;
            }
        }

        /// <summary>単位編集デフォルト</summary>
        /// <returns>
        /// デフォルト以外：true
        /// デフォルト：false
        /// </returns>
        public bool ShouldSerializeDisplayUnits()
        {
            return this._displayUnits != null;
        }

        #endregion

        #endregion

        #region イベント系

        #region イベントの説明

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

        #endregion

        #region 初期化処理（Layout）

        /// <summary>初期化処理</summary>
        private void WinCustomTextBox_Layout(object sender, EventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            if (!RcFxCmnFunction.IsDesignMode())
            {
                // デザイン・モードでは無い場合。
                if (base.Text == ""
                    && this.NumericalPossibility
                    && this.EditInitialValue == EditInitialValue.Zero)
                {
                    // 「0」初期化
                    base.Text = "0";

                    // ReEditeする。
                    this.ReEdit();
                }
            }
        }

        #endregion

        #region チェック処理（Validating、Validated）

        /// <summary>背景色（バックアップ）</summary>
        private Color? _backupBkColor = null;

        /// <summary>チェック処理</summary>
        private void WinCustomTextBox_Validating(object sender, CancelEventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

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
        private void WinCustomTextBox_Validated(object sender, EventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            // Valueに退避
            if (this._Value == base.Text)
            {
                // なにもしない。
            }
            else
            {
                // 変更された値の反映
                this._Value = base.Text;
                // Valueプロパティの変更通知
                this.OnValueChanged(EventArgs.Empty);
                this.NotifyPropertyChanged("Value");
            }

            // 編集処理
            this.ReEdit();
        }

        #endregion

        #region フォーマット処理（Enter、Leave、TextChanged）

        /// <summary>MouseDown状態の確認用フラグ</summary>
        private bool MouseDowned = false;

        /// <summary>マウスが入った</summary>
        private void WinCustomTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            this.MouseDowned = true;
        }

        /// <summary>フォーカス Enter</summary>
        private void WinCustomTextBox_Enter(object sender, EventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ((WinCustomTextBox)sender).Text);

            // 編集処理
            this.Edit();

            if (!this.MouseDowned)
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
        private void WinCustomTextBox_Leave(object sender, EventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + ((WinCustomTextBox)sender).Text);

            this.MouseDowned = false;
        }

        // ↓　リアーキし、Textプロパティに移動した。

        ///// <summary>テキスト変更時</summary>
        //private void WinCustomTextBox_TextChanged(object sender, EventArgs e)

        #endregion

        #region フィルタ処理（KeyPress）

        //// <summary>フィルタ処理</summary>
        //private void WinCustomTextBox_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    ・・・
        //}

        //↓Maskの実装に合わせる。

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
            // 数値、桁区切り指定、小数点以下切り捨て指定がある場合

            if (this.NumericalPossibility)
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

                    // 小数点以下設定が無い場合
                    EditDigitsAfterDP edad = this.EditDigitsAfterDP;
                    EditDigitsAfterDP edad_e = this.EditDigitsAfterDP_Editing;

                    // EditDigitsAfterDPがnullか、
                    // HowToCutがnullかCutMethod.Noneに指定されていた場合。
                    if ((edad == null || (edad != null && (edad.HowToCut.HasValue && edad.HowToCut.Value == CutMethod.None)))
                        && (edad_e == null || (edad_e != null && (edad_e.HowToCut.HasValue && edad_e.HowToCut.Value == CutMethod.None))))
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
                else if ((keyData >= Keys.D0 && keyData <= Keys.D9)
                    || (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9)
                    || keyData == Keys.Back || keyData == Keys.Delete
                    || keyData == (Keys.Control | Keys.Z)
                    || keyData == Keys.Left || keyData == Keys.Right
                    || keyData == Keys.Home || keyData == Keys.End
                    || keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)
                    || keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Enter
                    || keyData == (Keys.Left | Keys.Shift) || keyData == (Keys.Right | Keys.Shift)
                    || keyData == (Keys.Home | Keys.Shift) || keyData == (Keys.End | Keys.Shift)
                    || keyData == (Keys.Control | Keys.C) || keyData == (Keys.Control | Keys.X)
                    || keyData == (Keys.Control | Keys.V) || keyData == (Keys.Control | Keys.A))
                    // || keyData == Keys.ProcessKey
                    // || keyData == (Keys.Oemplus | Keys.Shift) || keyData == Keys.Add
                    // || keyData == Keys.Oemcomma)
                {
                    // 入力を許可
                    // ・「0-9」、「BSP・DEL」。「Ctrl・Z」（編集）
                    // ・「←・→」、「HOME・END」（カーソル移動）
                    // ・「TAB」、「SHIFT＋TAB」（タブ遷移）
                    // ・「↑・↓」、「Enter」（セル移動）
                    // ・「SHIFT＋←・→」、「SHIFT＋HOME・END」（選択系）
                    // ・「Ctrl・C」、「Ctrl・X」、「Ctrl・V」、「Ctrl・A」（コピペ系）

                    // 検討の結果、入力を許可しないようにしたキー。
                    // ・「漢字モード」、「+」、「,」

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
        private void WinCustomTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            // ここではダメでした。
        }

        #endregion

        #region 復元処理（KeyUp）

        /// <summary>０の復元</summary>
        private void WinCustomTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            // 無限ループ対応
#if NETCOREAPP
            string txt = this.StringCopy(base.Text);
#else
            string txt = String.Copy(base.Text);
#endif

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                // DELキーとBackSpaceキー

                // クリアされてしまった。
                if (txt == "")
                {
                    // 初期化
                    if (this.InitializeValue(ref txt))
                    {
                        // DELキーとBackSpaceキーの無効化
                        e.Handled = true;
                        // Textプロパティをクリア。
                        base.Text = txt;
                        // this.ReEdit(); // ステータス的に行われないため不要。
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
#if NETCOREAPP
            string txt = this.StringCopy(base.Text);
#else
            string txt = String.Copy(base.Text);
#endif

            // 半角化（数値指定されている場合）
            if (this.NumericalPossibility)
            {
                // ワーク
                string temp = "";
                StringBuilder sb = new StringBuilder();

                // 半角化する。
                temp = Public.Str.StringConverter.ToHankaku(txt);

                // 残っている全角＋非数値文字を削る。
                int minus = 0;
                bool period = false;

                foreach (char ch in temp)
                {
                    if (StringChecker.IsNumbers_Hankaku(ch.ToString()))
                    {
                        // 半角数値だけ追加する。
                        sb.Append(ch);
                    }
                    else if (ch == '-' && minus == 0)
                    {
                        // マイナスは先頭だけ
                        sb.Append(ch);
                    }
                    else if (ch == '.' && !period)
                    {
                        // ピリオドも先頭の一つだけ。

                        // ＆小数点以下指定がある場合だけ。
                        EditDigitsAfterDP edad = this.EditDigitsAfterDP;
                        EditDigitsAfterDP edad_e = this.EditDigitsAfterDP_Editing;
                        if ((edad != null && (edad.HowToCut.HasValue && edad.HowToCut.Value != CutMethod.None))
                        || (edad_e != null && (edad_e.HowToCut.HasValue && edad_e.HowToCut.Value != CutMethod.None)))
                        {
                            period = true;
                            sb.Append(ch);
                        }
                        else
                        {
                            // スルー
                        }
                    }
                    else
                    {
                        // スルー
                    }

                    minus++;
                }

                txt = sb.ToString();

                // 数値の筈が、数値で無い場合に初期化。
                if (!StringChecker.IsNumeric(txt))
                {   
                    this.InitializeValue(ref txt);
                }
            }

            // EditDigitsAfterDP
            // 空文字列の場合は編集せず。
            if (txt != "" && this.EditDigitsAfterDP_Editing != null)
            {
                // 小数点数以下ｘ桁四捨五入（丸め）編集
                switch (this.EditDigitsAfterDP_Editing.HowToCut)
                {
                    case CutMethod.Banker:
                        txt = FormatConverter.Round_Banker(txt, (int)this.EditDigitsAfterDP_Editing.DigitsAfterDP);
                        break;
                    case CutMethod._4sya5nyu:
                        txt = FormatConverter.Round_4sya5nyu(txt, (int)this.EditDigitsAfterDP_Editing.DigitsAfterDP);
                        break;
                    case CutMethod.Floor:
                        txt = FormatConverter.Floor(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RZ);
                        break;
                    case CutMethod.Ceiling:
                        txt = FormatConverter.Ceiling(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RI);
                        break;
                    case CutMethod.FloorRM:
                        txt = FormatConverter.Floor(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RM);
                        break;
                    case CutMethod.CeilingRP:
                        txt = FormatConverter.Ceiling(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RP);
                        break;
                    default: //case CutMethod.None:
                        break;
                }

                // 少数点以下の０削除
                txt = this.DeleteZeroAfterDP(txt, this.EditDigitsAfterDP_Editing);
                //// 少数点以下の０付け足し
                //txt = this.AddZeroAfterDP(txt, this.EditDigitsAfterDP_Editing);
                //ここでの０足しは無くす（丸めだけできれば良いので）。
            }

            if (base.Text != txt)
            {
                // 変更された場合は再設定
                base.Text = txt;
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
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":" + base.Text);

            // フラグ
            bool hasError = false;
            // ワーク
            List<string> lstRet = new List<string>();

#if NETCOREAPP
            string txt = this.StringCopy(base.Text);
#else
            string txt = String.Copy(base.Text);
#endif

            if (this.CheckType != null)
            {
                // 必須入力チェック
                if (this.CheckType.Required)
                {
                    if ((txt == ""))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.RequiredCheckErrorMessage);
                    }
                }

                // 数値チェック（空文字列は対象外）
                if (this.CheckType.IsNumeric && txt.Trim() != "")
                {
                    if (!StringChecker.IsNumeric(txt))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsNumericCheckErrorMessage);
                    }
                }

                // 半角チェック
                if (this.CheckType.IsHankaku)
                {
                    if (!StringChecker.IsHankaku(txt))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsHankakuCheckErrorMessage);
                    }
                }

                // 全角チェック
                if (this.CheckType.IsZenkaku)
                {
                    if (!StringChecker.IsZenkaku(txt))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsZenkakuCheckErrorMessage);
                    }
                }

                // 片仮名チェック
                if (this.CheckType.IsKatakana)
                {
                    if (!StringChecker.IsKatakana(txt))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsKatakanaCheckErrorMessage);
                    }
                }

                // 半角片仮名チェック
                if (this.CheckType.IsHanKatakana)
                {
                    if (!StringChecker.IsKatakana_Hankaku(txt))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsHanKatakanaCheckErrorMessage);
                    }
                }

                // 平仮名チェック
                if (this.CheckType.IsHiragana)
                {
                    if (!StringChecker.IsHiragana(txt))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsHiraganaCheckErrorMessage);
                    }
                }

                // 日付チェック（空文字列は対象外）
                if (this.CheckType.IsDate && txt.Trim() != "")
                {
                    DateTime dateTime;
                    if (!DateTime.TryParse(txt, out dateTime))
                    {
                        hasError = true;
                        lstRet.Add(CmnCheckFunction.IsDateCheckErrorMessage);
                    }
                }
            }

            // 正規表現チェック（空文字列は対象外）
            if (this.CheckRegExp != null && this.CheckRegExp != "" && txt.Trim() != "")
            {
                if (!StringChecker.Match(txt, this.CheckRegExp))
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
                    if (txt.IndexOf(ch) != -1)
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
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-start:" + base.Text);
            //Debug.WriteLine(Environment.StackTrace);

            // Editされた
            this.Edited = true;

            //// ０対応
            //string temp = "";

            string txt = "";

            // Editの取得元は可変。
            if (this.DisplayUnits == null)
            {
                // DisplayUnitsがNULLである。
#if NETCOREAPP
                txt = this.StringCopy(base.Text);
#else
                txt = String.Copy(base.Text);
#endif
            }
            else
            {
                // DisplayUnitsがNULLでない。
#if NETCOREAPP
                txt = this.StringCopy(this._Value);
#else
                txt = String.Copy(this._Value);
#endif
            }

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point1");

            // EditDigitsAfterDP_Editing
            // 空文字列の場合は編集せず。
            if (txt != "" && this.EditDigitsAfterDP_Editing != null)
            {
                // 小数点数以下ｘ桁四捨五入（丸め）編集
                switch (this.EditDigitsAfterDP_Editing.HowToCut)
                {
                    case CutMethod.Banker:
                        txt = FormatConverter.Round_Banker(txt, (int)this.EditDigitsAfterDP_Editing.DigitsAfterDP);
                        break;
                    case CutMethod._4sya5nyu:
                        txt = FormatConverter.Round_4sya5nyu(txt, (int)this.EditDigitsAfterDP_Editing.DigitsAfterDP);
                        break;
                    case CutMethod.Floor:
                        txt = FormatConverter.Floor(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RZ);
                        break;
                    case CutMethod.Ceiling:
                        txt = FormatConverter.Ceiling(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RI);
                        break;
                    case CutMethod.FloorRM:
                        txt = FormatConverter.Floor(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, FloorToward.RM);
                        break;
                    case CutMethod.CeilingRP:
                        txt = FormatConverter.Ceiling(txt, this.EditDigitsAfterDP_Editing.DigitsAfterDP, CeilingToward.RP);
                        break;
                    default: //case CutMethod.None:
                        break;
                }

                // 少数点以下の０削除
                txt = this.DeleteZeroAfterDP(txt, this.EditDigitsAfterDP_Editing);
                // 少数点以下の０付け足し
                txt = this.AddZeroAfterDP(txt, this.EditDigitsAfterDP_Editing);
            }

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point2");

            // EditAddFigure
            // 空文字列の場合は編集せず。
            if (txt != "" && this.EditAddFigure != EditAddFigure.None)
            {
                txt = txt.Replace(",", "");
            }

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point3");

            // 表示を変更
            base.Text = txt;

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-end:" + base.Text);
        }
        
        /// <summary>編集（逆）</summary>
        internal void ReEdit()
        {
            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-start:" + base.Text);
            //Debug.WriteLine(Environment.StackTrace);

            // ReEditされた
            this.Edited = true;

            // ０対応
            string temp = "";

#if NETCOREAPP
            string txt = this.StringCopy(base.Text);
#else
            string txt = String.Copy(base.Text);
#endif

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point1");

            // 数値だが、数値で無い場合、
            if (this.NumericalPossibility && (!StringChecker.IsNumeric(txt)))
            {
                // 初期化
                this.InitializeValue(ref txt);

                // 処理順を変えたくないので↓に追加した。

                // 変更された値の反映
                this._Value = txt;
                // Valueプロパティの変更通知
                this.OnValueChanged(EventArgs.Empty);
                this.NotifyPropertyChanged("Value");
            }

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point2");

            // EditDigitsAfterDP
            // 空文字列の場合は編集せず。
            if (txt != "" && this.EditDigitsAfterDP != null)
            {
                // 小数点数以下ｘ桁四捨五入（丸め）編集
                switch (this.EditDigitsAfterDP.HowToCut)
                {
                    case CutMethod.Banker:
                        txt = FormatConverter.Round_Banker(txt, (int)this.EditDigitsAfterDP.DigitsAfterDP);
                        break;
                    case CutMethod._4sya5nyu:
                        txt = FormatConverter.Round_4sya5nyu(txt, (int)this.EditDigitsAfterDP.DigitsAfterDP);
                        break;
                    case CutMethod.Floor:
                        txt = FormatConverter.Floor(txt, this.EditDigitsAfterDP.DigitsAfterDP, FloorToward.RZ);
                        break;
                    case CutMethod.Ceiling:
                        txt = FormatConverter.Ceiling(txt, this.EditDigitsAfterDP.DigitsAfterDP, CeilingToward.RI);
                        break;
                    case CutMethod.FloorRM:
                        txt = FormatConverter.Floor(txt, this.EditDigitsAfterDP.DigitsAfterDP, FloorToward.RM);
                        break;
                    case CutMethod.CeilingRP:
                        txt = FormatConverter.Ceiling(txt, this.EditDigitsAfterDP.DigitsAfterDP, CeilingToward.RP);
                        break;
                    default: //case CutMethod.None:
                        break;
                }

                // 少数点以下の０削除
                txt = this.DeleteZeroAfterDP(txt, this.EditDigitsAfterDP);
                // 少数点以下の０付け足し
                txt = this.AddZeroAfterDP(txt, this.EditDigitsAfterDP);
            }

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point3");

            // EditAddFigure
            EditAddFigure editAddFigure = this.EditAddFigure;

            // DataBindings対応

            // Target_Validateからの呼び出しを無視するようにしたため不要となった。

            //// 現象：小数点以下指定と、Formatの桁区切りが干渉して桁区切りがされない。
            //// 理由：Binding.Target_Validate→SetPropValueからのTextプロパティ呼出で、
            //// 編集を行ってしまうと、FormatStringの書式に戻らないため、
            //// 自力での編集処理が必要になる。
            //foreach (Binding b in this.DataBindings)
            //{
            //    if (b.FormatString.IndexOf("#,##0") != -1)
            //    {
            //        // DataBindingsで3桁区切りが指定されてる。
            //        editAddFigure = EditAddFigure.Af3;
            //    }
            //    else if (b.FormatString.IndexOf("#,###0") != -1)
            //    {
            //        // DataBindingsで4桁区切りが指定されてる。
            //        editAddFigure = EditAddFigure.Af4;
            //    }
            //}

            // 空文字列の場合は編集せず。
            if (txt != ""
                && editAddFigure != EditAddFigure.None)
            {
                // 整数部の桁区切り編集
                if (editAddFigure == EditAddFigure.Af3)
                {
                    // 3桁区切り
                    temp = FormatConverter.AddFigure3(txt);
                    if (temp != "0")
                    {
                        txt = temp;
                    }
                }
                else if (editAddFigure == EditAddFigure.Af4)
                {
                    // 4桁区切り
                    temp = FormatConverter.AddFigure4(txt);
                    if (temp != "0")
                    {
                        txt = temp;
                    }
                }
            }

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point4");

            // EditPadding
            // 空文字列の場合は編集せず。
            if (txt != ""
                && this.EditPadding != null && this.EditPadding.PadDir != PadDirection.None)
            {
                // 文字埋め編集
                char ch = ' ';

                // デフォルトは半角スペース埋め
                if (this.EditPadding.PadChar != null)
                {
                    ch = (char)this.EditPadding.PadChar;
                }

                // パディング
                if (this.EditPadding.PadDir == PadDirection.Left)
                {
                    txt = txt.PadLeft(this.MaxLength, ch);
                }
                else if (this.EditPadding.PadDir == PadDirection.Right)
                {
                    txt = txt.PadRight(this.MaxLength, ch);
                }
            }

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ":point5");

            // 表示を変更（Valueは変更しない）。
            base.Text = txt;

            //Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + "-end:" + base.Text);
        }

        /// <summary>CheckCharCodeクラス</summary>
        private static CheckCharCode CCC = new CheckCharCode("0", "9", Encoding.ASCII);

        /// <summary>設定から、数値である可能性を探る</summary>
        /// <remarks>数値、桁区切り指定、小数点以下切り捨て、数値パッド指定がある場合</remarks>
        private bool NumericalPossibility
        {
            get
            {
                EditDigitsAfterDP edad = this.EditDigitsAfterDP;
                EditDigitsAfterDP edad_e = this.EditDigitsAfterDP_Editing;
                //EditPadding ep = this.EditPadding;

                return (this.IsNumeric || this.CheckType.IsNumeric
                    || this.EditInitialValue == EditInitialValue.Zero
                    || this.EditAddFigure != EditAddFigure.None
                    || (edad != null && (edad.HowToCut.HasValue && edad.HowToCut.Value != CutMethod.None))
                    || (edad_e != null && (edad_e.HowToCut.HasValue && edad_e.HowToCut.Value != CutMethod.None))
                    || this.DisplayUnits != null); 
                //|| (ep != null && ep.PadChar.HasValue && WinCustomTextBox.CCC.IsInRange(ep.PadChar.ToString())));

                // 数値パッドは数値としない（数値埋めで、ASCIIコード入力のケースもある）。
            }
        }

        /// <summary>値を初期化する</summary>
        /// <param name="txt">初期値</param>
        /// <returns>
        /// ・true：初期化された。
        /// ・false：初期化されていない。
        /// </returns>
        private bool InitializeValue(ref string txt)
        {
            // 数値である可能性がある。
            if (this.NumericalPossibility)
            {
                // 数値である可能性がある場合
                if (this.EditInitialValue == EditInitialValue.Blank)
                {
                    // 空文字列クリア
                    txt = "";
                    return true;
                }
                else if (this.EditInitialValue == EditInitialValue.Zero)
                {
                    // 「0」クリア
                    txt = "0";

                    // 少数点以下の０付け足し

                    // 入力前、入力後で小数点以下設定を変える必要がある。
                    if (this.Focused)
                    {
                        txt = this.AddZeroAfterDP(txt, this.EditDigitsAfterDP_Editing);
                    }
                    else
                    {
                        txt = this.AddZeroAfterDP(txt, this.EditDigitsAfterDP);
                    }
                    return true;
                }
                else
                {
                    //・・・必要に応じてカスタマイズ可能・・・
                }
            }
            else
            {
                // 数値である可能性が無い場合
            }

            return false;
        }


        /// <summary>後ろに小数点以下ｘ桁を付与</summary>
        /// <param name="txt">入力文字列</param>
        /// <param name="edad">小数点以下切り捨て指定</param>
        /// <returns>編集後の文字列</returns>
        private string AddZeroAfterDP(string txt, EditDigitsAfterDP edad)
        {
            if ((edad != null && edad.DigitsAfterDP != 0 && 
                (edad.HowToCut.HasValue && edad.HowToCut.Value != CutMethod.None))
                && txt != "" && StringChecker.IsHankaku(txt) && StringChecker.IsNumeric(txt))
            {
                // 小数点以下切り捨て指定有り（の数値入力）の場合で、
                // 整数部が入力されている場合。

                // 少数部が入力されている・いない。
                int IndexOfDP = txt.IndexOf('.');

                if (IndexOfDP == -1)
                {
                    // DP無しの場合、

                    // MaxLength（最低2文字（.0）の空きが必須
                    if (txt.Length + 2 <= this.MaxLength)
                    {
                        // DP付与は可能
                        txt += ".";

                        for (int i = 0; i < edad.DigitsAfterDP; i++)
                        {
                            // MaxLengthに達していたら、forをbreak
                            if (this.MaxLength <= txt.Length) { break; }

                            // 以降、桁数分０追加
                            txt += "0";
                        }
                    }
                    else
                    {
                        // DP付与も不可
                    }
                }
                else
                {
                    // DP有りの場合、
                    for (int i = txt.Length - (IndexOfDP + 1); i < (int)edad.DigitsAfterDP; i++) // 条件間違っていた（修正済み）。
                    {
                        // MaxLengthに達していたら、forをbreak
                        if (this.MaxLength <= txt.Length) { break; }

                        // 以降、桁数分０追加
                        txt += "0";
                    }
                }

            }

            return txt;
        }

        /// <summary>小数点以下ｘ桁の０を削除</summary>
        /// <param name="txt">入力文字列</param>
        /// <param name="edad">小数点以下切り捨て指定</param>
        /// <returns>編集後の文字列</returns>
        private string DeleteZeroAfterDP(string txt, EditDigitsAfterDP edad)
        {
            if ((edad != null && (edad.HowToCut.HasValue && edad.HowToCut.Value != CutMethod.None))
                && txt != "" && StringChecker.IsHankaku(txt) && StringChecker.IsNumeric(txt))
            {
                // 小数点以下切り捨て指定有り（の数値入力）の場合で、
                // 整数部が入力されている場合。

                // 余分な「0」を削る。
                if (txt.IndexOf('.') == -1)
                {
                    // 小数点無し。
                }
                else
                {
                    // 小数点有り。

                    // 後ろから０を削っていく（.も対象となる）。
                    int i = txt.Length - 1;

                    if (i == -1)
                    {
                        // 空文字列
                    }
                    else
                    {
                        for (; txt[i] == '0' || txt[i] == '.'; i--)
                        {
                            if (i <= 0)
                            {
                                // ・・・
                                break;
                            }
                            else
                            {
                                // 削る
                                if (txt[i] == '0')
                                {
                                    txt = txt.Substring(0, i);
                                    continue; // '0'は継続
                                }
                                else if (txt[i] == '.')
                                {
                                    txt = txt.Substring(0, i);
                                    break; // '.'で停止
                                }
                                else
                                {
                                    // ここは通らない。
                                }
                            }
                        }
                    }
                }
            }

            return txt;
        }

        #endregion

#if NETCOREAPP
        /// <summary>StringCopy</summary>
        private string StringCopy(string input)
        {
            return (string)BinarySerialize.DeepClone(input);
        }
#else
#endif
    }
}
