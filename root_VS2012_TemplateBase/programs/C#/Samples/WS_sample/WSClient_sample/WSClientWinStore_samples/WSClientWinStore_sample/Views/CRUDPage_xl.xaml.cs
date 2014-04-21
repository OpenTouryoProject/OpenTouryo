using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
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

using Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest;
using WSClientWinStore_sample.Converter;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace WSClientWinStore_sample.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CRUDPage_xl : Page
    {
        public CRUDPage_xl()
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
        private void Page_Loaded_1(object sender, RoutedEventArgs e)
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
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "SelectCount";
            param.ActionType = actionType;
            param.UserName = context;
            param.Info = new Informations("");

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    this.labelMessage.Text = returnValue.Info.Str + "件のデータがあります";
                }
            }
        }

        /// <summary>一覧取得（dt）</summary>
        async private void button2_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "SelectAll_DT";
            param.ActionType = actionType;
            param.UserName = context;
            param.Info = new Informations("");

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.ListView1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }

        /// <summary>一覧取得（ds）</summary>
        async private void button3_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "SelectAll_DS";
            param.ActionType = actionType;
            param.UserName = context;
            param.Info = new Informations("");

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.ListView1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }

        /// <summary>一覧取得（dr）</summary>
        async private void button4_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "SelectAll_DR";
            param.ActionType = actionType;
            param.UserName = context;
            param.Info = new Informations("");

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.ListView1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }

        /// <summary>一覧取得（動的sql）</summary>
        async private void button5_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "SelectAll_DSQL";
            param.ActionType = actionType;
            param.UserName = context;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["OrderColumn"] = ((ComboBoxItem)this.ddlOrderColumn.SelectedItem).Value;
            dic["OrderSequence"] = ((ComboBoxItem)this.ddlOrderSequence.SelectedItem).Value;
            param.Info = new Informations(dic);

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.ListView1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }

        /// <summary>参照処理</summary>
        /// <remarks>
        /// 非同期フレームワークを使用してB層の呼び出し処理を非同期化
        /// （結果表示にだけ匿名デリゲードを使用するパターン）
        /// </remarks>
        async private void button6_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "Select";
            param.ActionType = actionType;
            param.UserName = context;

            string shipperID = this.textBox1.Text;
            param.Info = new Informations(shipperID);

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    Dictionary<string, string> dic = returnValue.Info.Dictionary;
                    this.textBox2.Text = dic["CompanyName"].ToString();
                    this.textBox3.Text = dic["Phone"].ToString();
                }
            }
        }

        #endregion

        #region 更新系

        /// <summary>追加処理</summary>
        async private void button7_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "Insert";
            param.ActionType = actionType;
            param.UserName = context;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["CompanyName"] = this.textBox2.Text;
            dic["Phone"] = this.textBox3.Text;
            param.Info = new Informations(dic);

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    this.labelMessage.Text = returnValue.Info.Str + "件追加";
                }
            }
        }

        /// <summary>更新処理</summary>
        async private void button8_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "Update";
            param.ActionType = actionType;
            param.UserName = context;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["ShipperID"] = this.textBox1.Text;
            dic["CompanyName"] = this.textBox2.Text;
            dic["Phone"] = this.textBox3.Text;
            param.Info = new Informations(dic);

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    this.labelMessage.Text = returnValue.Info.Str + "件更新";
                }
            }
        }

        /// <summary>削除処理</summary>
        async private void button9_Click(object sender, RoutedEventArgs e)
        {
            // コンテキスト
            string context = "User1";

            // 共通部

            // 共通部を生成
            string actionType =
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value;

            // パラメタを纏める
            ParamDataContract param = new ParamDataContract();
            param.ServiceName = "rtWebService";
            param.ScreenId = this.Name;
            param.ControlId = ((Button)sender).Name;
            param.MethodName = "Delete";
            param.ActionType = actionType;
            param.UserName = context;

            string shipperID = this.textBox1.Text;
            param.Info = new Informations(shipperID);

            // XML 形式に変換
            DataContractSerializer serializer = new DataContractSerializer(typeof(ParamDataContract));

            // HTTPクライアントを使用
            WebRequest request = WebRequest.Create(new Uri("http://localhost:9996/WSClientSL_sample/WebService/ServiceForRt.svc/XML", UriKind.Absolute));
            request.ContentType = "application/xml; charset=utf-8";
            request.Method = "POST";

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);

                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    // パラメタの内容をリクエストストリームに書き込み
                    byte[] xmlBytes = ms.ToArray();
                    stream.Write(xmlBytes, 0, xmlBytes.Count());
                }
            }

            // 非同期呼出しを行う
            WebResponse response = await request.GetResponseAsync();

            // レスポンスを ServiceReturnValue 形式に変換
            using (Stream stream = response.GetResponseStream())
            {
                // XML 形式のレスポンスを復元するための Serializer
                DataContractSerializer serializer2 = new DataContractSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer2.ReadObject(stream);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageDialog dialog = new MessageDialog("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    // 結果（正常系）
                    this.labelMessage.Text = returnValue.Info.Str + "件削除";
                }
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
