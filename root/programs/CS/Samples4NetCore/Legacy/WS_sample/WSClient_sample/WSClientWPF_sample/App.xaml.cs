//**********************************************************************************
//* ３層型 サンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：App
//* クラス日本語名  ：App.xaml の相互作用ロジック
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Windows;

using Touryo.Infrastructure.Public.Util;

namespace WSClientWPF_sample
{
    /// <summary>App.xaml の相互作用ロジック</summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // configの初期化
            GetConfigParameter.InitConfiguration("appsettings.json");
        }
    }
}
