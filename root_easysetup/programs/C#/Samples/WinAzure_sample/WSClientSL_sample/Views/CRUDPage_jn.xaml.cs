//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：CRUDPage_jn
//* クラス日本語名  ：WCF Json REST汎用S-I/F画面
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

using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

// DataContract
using Touryo.Infrastructure.Business.ServiceInterface.WcfDataContract.Rest;

namespace WSClientSL_sample.Views
{
    public partial class CRUDPage_jn : Page
    {
        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public CRUDPage_jn()
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer 
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_SelectCount_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }

        /// <summary>呼出しの完了後イベント（件数取得）</summary>
        void client_SelectCount_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {   
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                }
                else
                {
                    // 結果（正常系）
                    this.labelMessage.Text = returnValue.Info.Str + "件のデータがあります";
                }
            }
        }

        /// <summary>一覧取得（dt）</summary>
        private void button2_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_SelectAll_DT_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（dt））</summary>
        void client_SelectAll_DT_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.dataGrid1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }

         /// <summary>一覧取得（ds）</summary>
        private void button3_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_SelectAll_DS_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（ds））</summary>
        void client_SelectAll_DS_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.dataGrid1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }

        /// <summary>一覧取得（dr）</summary>
        private void button4_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_SelectAll_DR_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（dr））</summary>
        void client_SelectAll_DR_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.dataGrid1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }
        
        /// <summary>一覧取得（動的sql）</summary>
        private void button5_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_SelectAll_DSQL_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }

        /// <summary>呼出しの完了後イベント（一覧取得（動的sql））</summary>
        void client_SelectAll_DSQL_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                }
                else
                {
                    // 結果（正常系）
                    // データバインド
                    this.dataGrid1.ItemsSource = returnValue.Info.DicList;
                }
            }
        }

        /// <summary>参照処理</summary>
        /// <remarks>
        /// 非同期フレームワークを使用してB層の呼び出し処理を非同期化
        /// （結果表示にだけ匿名デリゲードを使用するパターン）
        /// </remarks>
        private void button6_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_Select_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }
        
        /// <summary>呼出しの完了後イベント（参照処理）</summary>
        void client_Select_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
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
        private void button7_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_Insert_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }

        /// <summary>呼出しの完了後イベント（追加処理）</summary>
        void client_Insert_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                }
                else
                {
                    // 結果（正常系）
                    this.labelMessage.Text = returnValue.Info.Str + "件追加";
                }
            }
        }

        /// <summary>更新処理</summary>
        private void button8_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/json";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_Update_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }

        /// <summary>呼出しの完了後イベント（更新処理）</summary>
        void client_Update_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
                }
                else
                {
                    // 結果（正常系）
                    this.labelMessage.Text = returnValue.Info.Str + "件更新";
                }
            }
        }

        /// <summary>削除処理</summary>
        private void button9_Click(object sender, RoutedEventArgs e)
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

            // JSON 形式に変換
            string jsonStringData = "";
            DataContractJsonSerializer serializer
                = new DataContractJsonSerializer(typeof(ParamDataContract));

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, param);
                jsonStringData = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }

            // HTTPクライアントを使用
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers["Content-type"] = "application/XML";

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_Delete_UploadStringCompleted);
            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            // ・・・仮想にしないとクロス・ドメイン・エラーになるようになった。
            client.UploadStringAsync(new Uri("/WebService/ServiceForRt.svc/JSON", UriKind.RelativeOrAbsolute), "POST", jsonStringData);
        }
        
        /// <summary>呼出しの完了後イベント（削除処理）</summary>
        void client_Delete_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            // レスポンスを ServiceReturnValue 形式に変換
            byte[] byteResponse = Encoding.UTF8.GetBytes(e.Result);
            using (MemoryStream ms = new MemoryStream(byteResponse))
            {
                // JSON 形式のレスポンスを復元するための Serializer
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReturnDataContract));
                ReturnDataContract returnValue = (ReturnDataContract)serializer.ReadObject(ms);

                if (returnValue.Error != null)
                {
                    // 例外発生時
                    MessageBox.Show("以下のエラーが発生しました\r\nメッセージ : " + returnValue.Error.Message);
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
            this.dataGrid1.ItemsSource = null;
        }
    }
}
