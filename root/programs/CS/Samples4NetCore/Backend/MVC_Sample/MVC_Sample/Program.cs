//**********************************************************************************
//* テンプレート
//**********************************************************************************

// サンプル中のテンプレートなので、必要に応じて使用して下さい。

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：Program
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System.Net.Http;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Touryo.Infrastructure.Framework.Authentication;

namespace MVC_Sample
{
    /// <summary>Program</summary>
    public class Program
    {
        /// <summary>
        /// Main（エントリポイント）</summary>
        /// <param name="args">コマンドライン引数</param>
        public static void Main(string[] args)
        {
            // OpenID用
            OAuth2AndOIDCClient.HttpClient = new HttpClient();

            // BuildWebHostが返すIWebHostをRunする。
            // 呼び出し元スレッドは終了までブロックされる。
            Program.BuildWebHost(args).Run();
        }

        /// <summary>BuildWebHost</summary>
        /// <param name="args">コマンドライン引数</param>
        /// <returns>IWebHost</returns>
        public static IWebHost BuildWebHost(string[] args)
        {
            // WebHost経由で、IWebHost, IWebHostBuilderにアクセスする。

            return WebHost.CreateDefaultBuilder(args) //  IWebHostBuilderを取得する。
                .UseStartup<Startup>() // IWebHostBuilder.UseStartup<TStartup> メソッドにStartupクラスを指定。
                .Build(); // IWebHostBuilder.Build メソッドでIWebHostクラスインスタンスを返す。
        }
    }
}
