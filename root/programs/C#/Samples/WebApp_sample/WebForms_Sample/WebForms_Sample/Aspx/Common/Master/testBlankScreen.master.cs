//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testBlankScreen
//* クラス日本語名  ：ブランクのMaster Page
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;

namespace WebForms_Sample.Aspx.Common.Master
{
    /// <summary>ブランクのMaster Page</summary>
    public partial class testBlankScreen : BaseMasterController
    {
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
