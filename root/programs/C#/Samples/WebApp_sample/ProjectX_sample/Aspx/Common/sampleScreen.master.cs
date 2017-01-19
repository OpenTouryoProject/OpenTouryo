//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：sampleScreen
//* クラス日本語名  ：サンプル画面用のマスタ ページ
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// OpenTouryo
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;

namespace ProjectX_sample.Aspx.Common
{
    /// <summary>サンプル画面用のマスタ ページ</summary>
    public partial class sampleScreen : BaseMasterController
    {
        /// <summary>マスタページにイベントハンドラを実装可能にしたのでそのテスト。</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnMPButton_Click(FxEventArgs fxEventArgs)
        {
            Response.Write("UOC_btnMPButton_Clickを実行できた。");

            return "";
        }

        /// <summary>UserName</summary>
        public string UserName
        {
            get
            {
                var user = (MyUserInfo)UserInfoHandle.GetUserInformation();

                if (user == null)
                {
                    return "anonymous";
                }
                else
                {
                    return user.UserName;
                }
            }
        }
    } 
}
