using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Touryo.Infrastructure.Public.Dto;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace WinStore_sample.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class DTOPage : Page
    {
        /// <summary>テーブル＠汎用DTO</summary>
        private DTTable Dtt = new DTTable("x");

        public DTOPage()
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

        /// <summary>ページの初期化</summary>
        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.InitListView();
        }

        /// <summary>ListViewの初期化</summary>
        async private void InitListView()
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
            MuServiceReference.ServiceForMuSoapClient client = new MuServiceReference.ServiceForMuSoapClient();

            // 非同期呼出しを行う
            MuServiceReference.CallResponse response = await client.CallAsync(Context, "muWebService", param, DTTables.DTTablesToString(dtts));

            // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
            this.Dtt = DTTables.StringToDTTables(response.Body.returnValue)[0];
            this.ListView1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        /// <summary>再データバインド</summary>
        private void btnReBind_Click(object sender, RoutedEventArgs e)
        {
            // 再データバインド（ToDataSourceでは、削除行を除いたリストを返す）
            this.ListView1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        /// <summary>選択行の削除</summary>
        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            // 選択行の削除
            int selectedIndex = this.ListView1.SelectedIndex;
            if (selectedIndex > -1)
            {
                // 汎用DTOテーブルを論理削除し、
                ((DTRows)this.ListView1.ItemsSource).Delete(selectedIndex);
                // 再データバインド（ToDataSourceでは、削除行を除いたリストを返す）
                this.ListView1.ItemsSource = this.Dtt.Rows.ToDataSource();
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

            this.ListView1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        /// <summary>
        /// Webサービスに汎用DTOデータを渡し
        /// Webサービス内で１行、行を追加する。
        /// </summary>
        async private void btnAddRow_CallWebService_Click(object sender, RoutedEventArgs e)
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

            // 非同期呼出しを行う
            MuServiceReference.CallResponse response = await client.CallAsync(Context, "muWebService", param, DTTables.DTTablesToString(dtts));

            // データバインド（ToDataSourceでは、削除行を除いたリストを返す）
            this.Dtt = DTTables.StringToDTTables(response.Body.returnValue)[0];
            this.ListView1.ItemsSource = this.Dtt.Rows.ToDataSource();
        }

        /// <summary>変更情報を取得</summary>
        async private void btnGetChanges_Click(object sender, RoutedEventArgs e)
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

                MessageDialog dialog = new MessageDialog(string.Format(
                    "{0}行変更されました。\r\n  追加{1}件　修正{2}件　削除{3}件",
                    new object[] { ChangedRowCount, AddedRowCount, ModifiedRowCount, DeletedRowCount }));
                await dialog.ShowAsync();
            }
            else
            {
                // 変更された行が無い場合。
                MessageDialog dialog = new MessageDialog("変更された行はありません");
                await dialog.ShowAsync();
            }
        }

        /// <summary>変更を確定</summary>
        async private void btnAcceptChanges_Click(object sender, RoutedEventArgs e)
        {
            // 変更を確定させる
            this.Dtt.AcceptChanges();
            MessageDialog dialog = new MessageDialog("変更を確定しました。");
            await dialog.ShowAsync();
        }

        /// <summary>セーブ（ダイアログにセーブした文字列を表示する）</summary>
        async private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DTTables dtts = new DTTables();
            dtts.Add(this.Dtt);

            MessageDialog dialog = new MessageDialog(DTTables.DTTablesToString(dtts));
            await dialog.ShowAsync();
        }
    }
}
