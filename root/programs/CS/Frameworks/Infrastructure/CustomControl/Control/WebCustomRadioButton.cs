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
//* クラス名        ：WebCustomRadioButton
//* クラス日本語名  ：ラジオボタン（Web）のカスタム・コントロール（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System.IO;
using System.Collections.Specialized;
using System.ComponentModel;

using System.Web.UI;
using System.Web.UI.WebControls;

namespace Touryo.Infrastructure.CustomControl
{
    /// <summary>ラジオボタン（Web）のカスタム・コントロール</summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:WebCustomRadioButton runat=server></{0}:WebCustomRadioButton>")]
    public class WebCustomRadioButton : RadioButton
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebCustomRadioButton()
        {
            // コンストラクタでのスタイル系のプロパティ設定は、
            // CSSとの相性が悪と考え、廃止しました。

            //this.Font.Size = 12;
            //this.ForeColor = System.Drawing.Color.Black;
            //this.Font.Name = "ＭＳ ゴシック";
        }

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

            // decorateしてRenderControl
            base.RenderControl(new WebCustomRadioButtonHtmlTextWriter(output, this));

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

        #region データバインドコントロール内で使用するカスタマイズ

        // データバインドコントロール内で使用できるカスタムラジオボタンの作成（1-2）：CodeZine
        // http://codezine.jp/article/detail/840
        // http://codezine.jp/article/detail/840?p=2

        /// <summary>ポストバック時にデータを復元</summary>
        /// <param name="postDataKey">ポストデータのキー</param>
        /// <param name="postCollection">ポストデータのコレクション</param>
        /// <returns>真偽（下記PG内コメントを参照）</returns>
        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            // GroupNameキーの値がUniqueIDと等しいかどうかによって選択されたかどうかを確認
            bool newChecked = (postCollection[this.GroupName] == this.UniqueID);

            // RadioButtonのときは、
            // 新たに選択されたときのみtrueを返す
            // (非選択になった時にはfalseを返す)
            if (this.Checked != newChecked)
            {
                // 新たに選択されたとき
                 
                // 値を変更する。
                this.Checked = newChecked;
                // 変更された値を返す。
                return newChecked;
            }
            else
            {
                // 新たに選択されいないのでfalseを返す。
                return false;
            }
        }

        /// <summary>カスタムのHtmlTextWriter</summary>
        private class WebCustomRadioButtonHtmlTextWriter : HtmlTextWriter
        {
            /// <summary>RadioButton Control</summary>
            private RadioButton _ctrl;

            /// <summary>コンストラクタ</summary>
            /// <param name="tw">TextWriter</param>
            /// <param name="ctrl">RadioButton Control</param>
            public WebCustomRadioButtonHtmlTextWriter(TextWriter tw, RadioButton ctrl)
                : base(tw)
            {
                // メンバに設定
                this._ctrl = ctrl;
            }

            /// <summary>
            /// ・POSTデータから値を読み込んでコントロールの状態を復元します。
            /// ・また、コントロールの状態が変わったらtrueを返します。
            /// </summary>
            /// <param name="key">Attributeのkey</param>
            /// <param name="value">Attributeのvalue</param>
            public override void AddAttribute(HtmlTextWriterAttribute key, string value)
            {
                // 描画時にname属性にGroupNameを、value属性にUniqueIDを設定
                switch (key)
                {
                    case HtmlTextWriterAttribute.Name:
                        base.AddAttribute(key, this._ctrl.GroupName);
                        break;

                    case HtmlTextWriterAttribute.Value:
                        base.AddAttribute(key, this._ctrl.UniqueID);
                        break;

                    default:
                        base.AddAttribute(key, value);
                        break;
                }
            }
        }

        #endregion

        #endregion
    }
}
