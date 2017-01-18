//**********************************************************************************
//* クラス名        ：testBlankScreen
//* クラス日本語名  ：ブランクのマスタ ページ
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
    /// <summary>ブランクのマスタ ページ</summary>
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
