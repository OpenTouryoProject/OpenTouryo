using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Touryo.Infrastructure.Public.Dto;
using WSClientWinStore_sample.Converter;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace WSClientWinStore_sample.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CRUDPage_sb : Page
    {
        public CRUDPage_sb()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// このページがフレームに表示されるときに呼び出されます。
        /// </summary>
        /// <param name="e">このページにどのように到達したかを説明するイベント データ。Parameter 
        /// プロパティは、通常、ページを構成するために使用します。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>ロード イベント</summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // ddlDap
            this.ddlDap.Items.Add(new ComboBoxItem("SQL Server / SQL Client", "SQL"));
            this.ddlDap.Items.Add(new ComboBoxItem("Multi-DB / OLEDB.NET", "OLE"));
            this.ddlDap.Items.Add(new ComboBoxItem("Multi-DB / ODCB.NET", "ODB"));
            this.ddlDap.Items.Add(new ComboBoxItem("Oracle / ODP.NET", "ODP"));
            this.ddlDap.Items.Add(new ComboBoxItem("DB2 / DB2.NET", "DB2"));
            this.ddlDap.Items.Add(new ComboBoxItem("HiRDB / HiRDB-DP", "HIR"));
            this.ddlDap.Items.Add(new ComboBoxItem("MySQL Cnn/NET", "MCN"));
            this.ddlDap.Items.Add(new ComboBoxItem("PostgreSQL / Npgsql", "NPS"));

            this.ddlDap.SelectedIndex = 0;

            // ddlMode1
            this.ddlMode1.Items.Add(new ComboBoxItem("個別Ｄａｏ", "individual"));
            this.ddlMode1.Items.Add(new ComboBoxItem("共通Ｄａｏ", "common"));
            this.ddlMode1.Items.Add(new ComboBoxItem("自動生成Ｄａｏ（更新のみ）", "generate"));
            this.ddlMode1.SelectedIndex = 0;

            // ddlMode2
            this.ddlMode2.Items.Add(new ComboBoxItem("静的クエリ", "static"));
            this.ddlMode2.Items.Add(new ComboBoxItem("動的クエリ", "dynamic"));
            this.ddlMode2.SelectedIndex = 0;

            // ddlIso
            this.ddlIso.Items.Add(new ComboBoxItem("ノットコネクト", "NC"));
            this.ddlIso.Items.Add(new ComboBoxItem("ノートランザクション", "NT"));
            this.ddlIso.Items.Add(new ComboBoxItem("ダーティリード", "RU"));
            this.ddlIso.Items.Add(new ComboBoxItem("リードコミット", "RC"));
            this.ddlIso.Items.Add(new ComboBoxItem("リピータブルリード", "RR"));
            this.ddlIso.Items.Add(new ComboBoxItem("シリアライザブル", "SZ"));
            this.ddlIso.Items.Add(new ComboBoxItem("スナップショット", "SS"));
            this.ddlIso.Items.Add(new ComboBoxItem("デフォルト", "DF"));
            this.ddlIso.SelectedIndex = 1;

            // WSでは使用しない（設定できないので）。
            this.ddlIso.IsEnabled = false;

            // ddlExRollback
            this.ddlExRollback.Items.Add(new ComboBoxItem("正常時", "-"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("業務例外", "Business"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("システム例外", "System"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("その他、一般的な例外", "Other"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("業務例外への振替", "Other-Business"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("システム例外への振替", "Other-System"));
            this.ddlExRollback.SelectedIndex = 0;

            // ddlTransmission
            this.ddlTransmission.Items.Add(new ComboBoxItem("Webサービス呼出", "testWebService"));
            this.ddlTransmission.Items.Add(new ComboBoxItem("インプロセス呼出", "testInProcess"));
            this.ddlTransmission.SelectedIndex = 0;

            // SLでは使用しない（インプロセスは使用できないので）。
            this.ddlTransmission.IsEnabled = false;

            // ddlOrderColumn
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c1", "c1"));
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c2", "c2"));
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c3", "c3"));
            this.ddlOrderColumn.SelectedIndex = 0;

            // ddlOrderSequence
            this.ddlOrderSequence.Items.Add(new ComboBoxItem("ASC", "A"));
            this.ddlOrderSequence.Items.Add(new ComboBoxItem("DESC", "D"));
            this.ddlOrderSequence.SelectedIndex = 0;
        }

        #region コンボボックス用

        /// <summary>コンボボックス用インナークラス</summary>
        private class ComboBoxItem
        {
            /// <summary>表示名</summary>
            private string m_name = "";

            /// <summary>値</summary>
            private string m_value = "";

            /// <summary>コンストラクタ</summary>
            public ComboBoxItem(string name, string value)
            {
                m_name = name;
                m_value = value;
            }

            /// <summary>表示名</summary>
            public string Name
            {
                get
                {
                    return m_name;
                }
            }

            /// <summary>値</summary>
            public string Value
            {
                get
                {
                    return m_value;
                }
            }

            /// <summary>
            /// オーバーライドしたメソッド
            /// これがコンボボックスに表示される
            /// </summary>
            public override string ToString()
            {
                return m_name;
            }
        }

        #endregion
        
        #region ＣＲＵＤ処理メソッド

        #region 参照系

        /// <summary>件数取得</summary>
        async private void button1_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.SelectCountResponse response = await client.SelectCountAsync(context, actionType);

            if (response.Body.SelectCountResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.SelectCountResult);
                this.labelMessage.Text = response.Body.SelectCountResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = response.Body.returnValue + "件のデータがあります";
            }
        }

        /// <summary>一覧取得（dt）</summary>
        async private void button2_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.SelectAll_DTResponse response = await client.SelectAll_DTAsync(context, actionType);

            if (response.Body.SelectAll_DTResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.SelectAll_DTResult);
                this.labelMessage.Text = response.Body.SelectAll_DTResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
                this.ListView1.ItemsSource = response.Body.returnValue;
            }
        }

        /// <summary>一覧取得（ds）</summary>
        async private void button3_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            SbServiceReference.SelectAll_DSResponse response = await client.SelectAll_DSAsync(context, actionType);

            if (response.Body.SelectAll_DSResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.SelectAll_DSResult);
                this.labelMessage.Text = response.Body.SelectAll_DSResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
                this.ListView1.ItemsSource = response.Body.returnValue;
            }
        }

        /// <summary>一覧取得（dr）</summary>
        async private void button4_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.SelectAll_DRResponse response = await client.SelectAll_DRAsync(context, actionType);

            if (response.Body.SelectAll_DRResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.SelectAll_DRResult);
                this.labelMessage.Text = response.Body.SelectAll_DRResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
                this.ListView1.ItemsSource = response.Body.returnValue;
            }
        }

        /// <summary>一覧取得（動的sql）</summary>
        async private void button5_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.SelectAll_DSQLResponse response = await client.SelectAll_DSQLAsync(context, actionType,
                ((ComboBoxItem)this.ddlOrderColumn.SelectedItem).Value,
                ((ComboBoxItem)this.ddlOrderSequence.SelectedItem).Value);

            if (response.Body.SelectAll_DSQLResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.SelectAll_DSQLResult);
                this.labelMessage.Text = response.Body.SelectAll_DSQLResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
                this.ListView1.ItemsSource = response.Body.returnValue;
            }
        }

        /// <summary>参照処理</summary>
        /// <remarks>
        /// 非同期フレームワークを使用してB層の呼び出し処理を非同期化
        /// （結果表示にだけ匿名デリゲードを使用するパターン）
        /// </remarks>
        async private void button6_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.SelectResponse response = await client.SelectAsync(context, actionType, int.Parse(this.textBox1.Text));

            if (response.Body.SelectResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.SelectResult);
                this.labelMessage.Text = response.Body.SelectResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
                SbServiceReference.Shipper shipper = response.Body.returnValue;
                this.textBox2.Text = shipper.CompanyName;
                this.textBox3.Text = shipper.Phone;
            }
        }

        #endregion

        #region 更新系

        /// <summary>追加処理</summary>
        async private void button7_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.InsertResponse response = await client.InsertAsync(context, actionType, this.textBox2.Text, this.textBox3.Text);

            if (response.Body.InsertResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.InsertResult);
                this.labelMessage.Text = response.Body.InsertResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = response.Body.returnValue + "件追加";
            }
        }

        /// <summary>更新処理</summary>
        async private void button8_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.UpdateResponse response = await client.UpdateAsync(context, actionType,
                new SbServiceReference.Shipper() { ShipperID = int.Parse(this.textBox1.Text), CompanyName = this.textBox2.Text, Phone = this.textBox3.Text });

            if (response.Body.UpdateResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.UpdateResult);
                this.labelMessage.Text = response.Body.UpdateResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = response.Body.returnValue + "件更新";
            }
        }

        /// <summary>削除処理</summary>
        async private void button9_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string context = "User1";

            // 引数２：アクションタイプ
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // Soap & Bean の個別WebメソッドのWeb参照
            SbServiceReference.ServiceForSbSoapClient client = new SbServiceReference.ServiceForSbSoapClient();

            // 非同期呼出しを行う
            SbServiceReference.DeleteResponse response = await client.DeleteAsync(context, actionType, int.Parse(this.textBox1.Text));

            if (response.Body.DeleteResult != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(response.Body.DeleteResult);
                this.labelMessage.Text = response.Body.DeleteResult + " , " + response.Body.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = response.Body.returnValue + "件削除";
            }
        }

        #endregion

        #endregion

        /// <summary>クリア</summary>
        private void button10_Click(object sender, RoutedEventArgs e)
        {
            this.ListView1.ItemsSource = null;
        }
    }
}
