//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：MainPage
//* クラス日本語名  ：（汎用DTOテスト）メイン画面
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System;
using System.Collections;
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

using WSClientSL_sample.Views;
using WSClientSL_sample.Converter;

namespace WSClientSL_sample
{
    /// <summary>（汎用DTOテスト）メイン画面</summary>
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitUserInfo();
        }

        /// <summary>UserInfoの初期化</summary>
        private void InitUserInfo()
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
            param.Add("GetFormsAuthInfo");      // メソッド名
            param.Add("ActionType");            // アクションタイプ

            // 引数３：汎用DTOデータ部

            // 空のDTTables
            DTTables dtts = new DTTables();
            dtts.Add(new DTTable("x"));

            // 汎用サービスI/FのWebサービスは通常のWeb参照を用いる。
            MuServiceReference.ServiceForMuSoapClient client
                = new MuServiceReference.ServiceForMuSoapClient();

            // 呼び出しが完了した場合のイベントハンドラを設定する。
            // (匿名メソッドを用いた場合)
            client.CallCompleted += (object s, MuServiceReference.CallCompletedEventArgs e) =>
            {
                if (e.Error == null && e.Result == "")
                {
                    this.textBlock1.Text = e.returnValue;
                }
                else
                {
                    this.textBlock1.Text = "－";
                }
            };

            // 非同期呼出しを行う（Silverlightは非同期呼び出しのみサポートする）。
            client.CallAsync(Context, "Authentication", param, DTTables.DTTablesToString(dtts));
        }

        /// <summary>
        /// Frame のナビゲートの後で、現在のページを表す HyperlinkButton が選択されていることを確認します
        /// </summary>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksWrapPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        /// <summary>
        /// ナビゲーション中にエラーが発生した場合は、エラー ウィンドウを表示します
        /// </summary>
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }
    }
}
