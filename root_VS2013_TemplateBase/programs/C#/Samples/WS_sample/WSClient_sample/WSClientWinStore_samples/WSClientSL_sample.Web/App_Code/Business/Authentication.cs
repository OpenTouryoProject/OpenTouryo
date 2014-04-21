//**********************************************************************************
//* フレームワーク・テストクラス（Ｂ層）
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Authentication
//* クラス日本語名  ：SilverlightにForms認証の認証情報を返す。
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

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using WSClientSL_sample.Web.Dao;

using System.Web;

namespace WSClientSL_sample.Web.Business
{

    /// <summary>SilverlightにForms認証の認証情報を返す</summary>
    public class Authentication : MyFcBaseLogic
    {
        /// <summary>認証情報を返すメソッド</summary>
        /// <param name="parameter">引数クラス</param>
        public void UOC_GetFormsAuthInfo(MuParameterValue parameter)
        {
            // 戻り値クラスを生成
            MuReturnValue muReturn = new MuReturnValue();

            // 認証情報を返す
            this.ReturnValue = muReturn;
            muReturn.Value = HttpContext.Current.User.Identity.Name;
        }
    }
}
