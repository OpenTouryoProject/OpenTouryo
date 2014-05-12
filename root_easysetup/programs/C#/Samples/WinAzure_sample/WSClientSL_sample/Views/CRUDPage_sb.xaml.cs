﻿//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：CRUDPage_sb
//* クラス日本語名  ：SOAP & Bean 個別S-I/F画面
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

// 汎用DTO
using Touryo.Infrastructure.Public.Dto;

using WSClientSL_sample.Converter;

namespace WSClientSL_sample.Views
{
    public partial class CRUDPage_sb : Page
    {
        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public CRUDPage_sb()
        {
            InitializeComponent();
        }

        // ユーザーがこのページに移動したときに実行されます。
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

        #endregion

        #region ＣＲＵＤ処理メソッド

        #region 参照系

        /// <summary>件数取得</summary>
        private void button1_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.SelectCountCompleted += new EventHandler<SbServiceReference.SelectCountCompletedEventArgs>(client_SelectCount_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.SelectCountAsync(context, actionType);
        }

        /// <summary>呼出しの完了後イベント（件数取得）</summary>
        void client_SelectCount_CallCompleted(object sender, SbServiceReference.SelectCountCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = e.returnValue.ToString() + "件のデータがあります";
            }
        }

        /// <summary>一覧取得（dt）</summary>
        private void button2_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.SelectAll_DTCompleted += new EventHandler<SbServiceReference.SelectAll_DTCompletedEventArgs>(client_SelectAll_DT_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.SelectAll_DTAsync(context, actionType);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（dt））</summary>
        void client_SelectAll_DT_CallCompleted(object sender, SbServiceReference.SelectAll_DTCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド
                this.dataGrid1.ItemsSource = e.returnValue;
            }
        }

         /// <summary>一覧取得（ds）</summary>
        private void button3_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.SelectAll_DSCompleted += new EventHandler<SbServiceReference.SelectAll_DSCompletedEventArgs>(client_SelectAll_DS_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.SelectAll_DSAsync(context, actionType);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（ds））</summary>
        void client_SelectAll_DS_CallCompleted(object sender, SbServiceReference.SelectAll_DSCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド
                this.dataGrid1.ItemsSource = e.returnValue;
            }
        }

        /// <summary>一覧取得（dr）</summary>
        private void button4_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.SelectAll_DRCompleted += new EventHandler<SbServiceReference.SelectAll_DRCompletedEventArgs>(client_SelectAll_DR_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.SelectAll_DRAsync(context, actionType);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（dr））</summary>
        void client_SelectAll_DR_CallCompleted(object sender, SbServiceReference.SelectAll_DRCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド
                this.dataGrid1.ItemsSource = e.returnValue;
            }
        }
        
        /// <summary>一覧取得（動的sql）</summary>
        private void button5_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.SelectAll_DSQLCompleted += new EventHandler<SbServiceReference.SelectAll_DSQLCompletedEventArgs>(client_SelectAll_DSQL_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.SelectAll_DSQLAsync(context, actionType,
                ((ComboBoxItem)this.ddlOrderColumn.SelectedItem).Value,
                ((ComboBoxItem)this.ddlOrderSequence.SelectedItem).Value);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（動的sql））</summary>
        void client_SelectAll_DSQL_CallCompleted(object sender, SbServiceReference.SelectAll_DSQLCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                // データバインド
                this.dataGrid1.ItemsSource = e.returnValue;
            }
        }

        /// <summary>参照処理</summary>
        /// <remarks>
        /// 非同期フレームワークを使用してB層の呼び出し処理を非同期化
        /// （結果表示にだけ匿名デリゲードを使用するパターン）
        /// </remarks>
        private void button6_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.SelectCompleted += new EventHandler<SbServiceReference.SelectCompletedEventArgs>(client_Select_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.SelectAsync(context, actionType, int.Parse(this.textBox1.Text));
        }

        /// <summary>呼出しの完了後イベント（参照処理）</summary>
        void client_Select_CallCompleted(object sender, SbServiceReference.SelectCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.textBox2.Text = e.returnValue.CompanyName;
                this.textBox3.Text = e.returnValue.Phone;
            }
        }

        #endregion

        #region 更新系

        /// <summary>追加処理</summary>
        private void button7_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.InsertCompleted += new EventHandler<SbServiceReference.InsertCompletedEventArgs>(client_Insert_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.InsertAsync(context, actionType, this.textBox2.Text, this.textBox3.Text);
        }

        /// <summary>呼出しの完了後イベント（追加処理）</summary>
        void client_Insert_CallCompleted(object sender, SbServiceReference.InsertCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = e.returnValue.ToString() + "件追加";
            }
        }

        /// <summary>更新処理</summary>
        private void button8_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            SbServiceReference.Shipper shipper = new SbServiceReference.Shipper();
            shipper.ShipperID = int.Parse(this.textBox1.Text);
                shipper.CompanyName=this.textBox2.Text;

                shipper.Phone=this.textBox3.Text;

            // 呼び出しが完了した場合のイベントハンドラを設定する。
                client.UpdateCompleted += new EventHandler<SbServiceReference.UpdateCompletedEventArgs>(client_Update_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.UpdateAsync(context, actionType, shipper);
        }

        /// <summary>呼出しの完了後イベント（更新処理）</summary>
        void client_Update_CallCompleted(object sender, SbServiceReference.UpdateCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = e.returnValue.ToString() + "件更新";
            }
        }

        /// <summary>削除処理</summary>
        private void button9_Click(object sender, RoutedEventArgs e)
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
            SbServiceReference.ServiceForSbSoapClient client
                = new SbServiceReference.ServiceForSbSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.DeleteCompleted += new EventHandler<SbServiceReference.DeleteCompletedEventArgs>(client_Delete_CallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.DeleteAsync(context, actionType, int.Parse(this.textBox1.Text));
        }

        /// <summary>呼出しの完了後イベント（削除処理）</summary>
        void client_Delete_CallCompleted(object sender, SbServiceReference.DeleteCompletedEventArgs e)
        {
            if (e.Result != "")
            {
                // 例外発生時
                WSErrorInfo wse = new WSErrorInfo(e.Result);
                this.labelMessage.Text = e.Result + " , " + e.returnValue + " , " + wse.MessageID + " , " + wse.Message + " , " + wse.Information;
            }
            else
            {
                // 結果（正常系）
                this.labelMessage.Text = e.returnValue + "件削除";
            }
        }

        #endregion

        #endregion

        /// <summary>クリア</summary>
        private void button10_Click(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.ItemsSource = null;
        }
    }
}
