//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：DTOPage
//* クラス日本語名  ：汎用DTOテスト画面
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
    public partial class DTOPage : Page
    {
        /// <summary>テーブル＠汎用DTO</summary>
        private DTTable Dtt =new DTTable("x");

        #region 初期化

        /// <summary>コンストラクタ</summary>
        public DTOPage()
        {
            InitializeComponent();
        }

        // ユーザーがこのページに移動したときに実行されます。
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>ページの初期化</summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitDataGrid();
        }

        /// <summary>DataGridの初期化</summary>
        private void InitDataGrid()
        {
            // テーブル＠汎用DTOにデータを詰めてバインディングさせる。

            // 引数１：コンテキスト
            string Context = "User1";

            // 引数２：共通部
            MuServiceReference.ArrayOfString param
                = new MuServiceReference.ArrayOfString();

            // 共通部を生成
            param.Add(this.Name);               // 画面名
            param.Add("Page_Loaded");           // ボタン名
            param.Add("InitDataGrid");          // メソッド名
            param.Add("ActionType");            // アクションタイプ

            // 引数３：汎用DTOデータ部

            // 空のDTTables
            DTTables dtts = new DTTables();
            dtts.Add(this.Dtt);

            // 汎用サービスI/FのWebサービスは通常のWeb参照を用いる。
            MuServiceReference.ServiceForMuSoapClient client
                = new MuServiceReference.ServiceForMuSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.CallCompleted += new EventHandler<MuServiceReference.CallCompletedEventArgs>(client_InitDataGridCallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.CallAsync(Context, "muWebService", param, DTTables.DTTablesToString(dtts));
        }

        /// <summary>呼出しの完了後イベント（データグリッドの初期化）</summary>
        void client_InitDataGridCallCompleted(object sender, MuServiceReference.CallCompletedEventArgs e)
        {
            // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
            this.Dtt = DTTables.StringToDTTables(e.returnValue)[0];
            this.dataGrid1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        #endregion

        #region 操作

        /// <summary>再データバインド</summary>
        private void btnReBind_Click(object sender, RoutedEventArgs e)
        {
            // 再データバインド（ToDataSourceでは、削除行を除いたリストを返す）
            this.dataGrid1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        #region 追加・削除

        /// <summary>選択行の削除</summary>
        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            // 選択行の削除
            int selectedIndex = this.dataGrid1.SelectedIndex;
            if (selectedIndex > -1)
            {
                // 汎用DTOテーブルを論理削除し、
                ((DTRows)this.dataGrid1.ItemsSource).Delete(selectedIndex);
                // 再データバインド（ToDataSourceでは、削除行を除いたリストを返す）
                this.dataGrid1.ItemsSource = this.Dtt.Rows.ToDataSource();
            }
        }

        /// <summary>
        /// １行、行を追加する。
        /// </summary>
        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            DTRow dtrow = this.Dtt.Rows.AddNew();

            // 1行追加
            dtrow["boolVal"] = true;
            dtrow["charVal"] = 'z';
            dtrow["dateVal"] = new DateTime(1946, 12, 11, 10, 20, 30, 444);
            dtrow["decimalVal"] = 99999;
            dtrow["doubleVal"] = 9.99D;
            dtrow["shortVal"] = 900;
            dtrow["intVal"] = 9000000;
            dtrow["longVal"] = 9000000000000;
            dtrow["singleVal"] = 9.9f;
            dtrow["stringVal"] = "test" + this.Dtt.Rows.Count.ToString();

            this.dataGrid1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        /// <summary>
        /// Webサービスに汎用DTOデータを渡し
        /// Webサービス内で１行、行を追加する。
        /// </summary>
        private void btnAddRow_CallWebService_Click(object sender, RoutedEventArgs e)
        {
            // 引数１：コンテキスト
            string Context = "User1";

            // 引数２：共通部
            MuServiceReference.ArrayOfString param
                = new MuServiceReference.ArrayOfString();

            // 共通部を生成
            param.Add(this.Name);               // 画面名
            param.Add(((Button)sender).Name);   // ボタン名
            param.Add("AddRow");                // メソッド名
            param.Add("ActionType");            // アクションタイプ

            // 引数３：汎用DTOデータ部

            // 空のDTTables
            DTTables dtts = new DTTables();
            dtts.Add(this.Dtt);

            // 汎用サービスI/FのWebサービスは通常のWeb参照を用いる。
            MuServiceReference.ServiceForMuSoapClient client
                = new MuServiceReference.ServiceForMuSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.CallCompleted += new EventHandler<MuServiceReference.CallCompletedEventArgs>(client_AddRowCallCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.CallAsync(Context, "muWebService", param, DTTables.DTTablesToString(dtts));
        }

        /// <summary>呼出しの完了後イベント（行追加）</summary>
        void client_AddRowCallCompleted(object sender, MuServiceReference.CallCompletedEventArgs e)
        {
            // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
            this.Dtt = DTTables.StringToDTTables(e.returnValue)[0];
            this.dataGrid1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        #endregion

        #region DTOメソッド

        /// <summary>変更情報を取得</summary>
        private void btnGetChanges_Click(object sender, RoutedEventArgs e)
        {
            // 変更された行数を表示する
            int ChangedRowCount = this.Dtt.GetChanges().Count;

            // 変更の内訳情報を取得する。
            // （AcceptChangesされるまで、変更情報を保持）
            if (ChangedRowCount > 0)
            {
                // 変更された行が有る場合。

                // 追加された行
                int AddedRowCount = this.Dtt.Rows.Find(DataRowState.Added).Count;
                // 変更された行
                int ModifiedRowCount = this.Dtt.Rows.Find(DataRowState.Modified).Count;
                // 削除された行
                int DeletedRowCount = this.Dtt.Rows.Find(DataRowState.Deleted).Count;
                MessageBox.Show(string.Format(
                    "{0}行変更されました。\r\n  追加{1}件　修正{2}件　削除{3}件",
                    new object[] { ChangedRowCount, AddedRowCount, ModifiedRowCount, DeletedRowCount }));
            }
            else
            {
                // 変更された行が無い場合。
                MessageBox.Show("変更された行はありません");
            }
        }

        /// <summary>変更を確定</summary>
        private void btnAcceptChanges_Click(object sender, RoutedEventArgs e)
        {
            // 変更を確定させる
            this.Dtt.AcceptChanges();
            MessageBox.Show("変更を確定しました。");
        }

        /// <summary>セーブ（ダイアログにセーブした文字列を表示する）</summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DTTables dtts = new DTTables();
            dtts.Add(this.Dtt);

            DialogWindow dialogWindow = new DialogWindow(DTTables.DTTablesToString(dtts));
            dialogWindow.Show();
        }

        #endregion

        #endregion
    }
}
