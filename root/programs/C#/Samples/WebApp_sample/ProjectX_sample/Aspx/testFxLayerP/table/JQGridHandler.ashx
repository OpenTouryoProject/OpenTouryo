<%@ WebHandler Language="C#" Class="JQGridHandler" %>

//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_table_testGridView
//* クラス日本語名  ：GridViewテスト画面（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/28/01  Supragyan         Crated JQGrid Handler for getting data into JQGrid
//**********************************************************************************

//system
using System;
using System.Web;

//Public
using Touryo.Infrastructure.Public;
using Touryo.Infrastructure.Public.Dto;

#region JQGridHandler

/// <summary>
/// JQGridHandler class
/// </summary>
public class JQGridHandler : IHttpHandler
{
    /// <summary>
    /// Process Request method loading Handler
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest(HttpContext context)
    {
        System.Collections.Specialized.NameValueCollection forms = context.Request.Form;
        string strOperation = forms.Get("oper");
        string strResponse = string.Empty;

        DTTable dtTable = new DTTable("JQGridData");

        if (strOperation == null)
        {
            //oper = null which means its first load.
            //Serialize object to json data.
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.Write(jsonSerializer.Serialize(dtTable.GetJson()));
        }
    }

    #region IsReusable

    /// <summary>
    /// IsReusable method to check thread-safe.
    /// </summary>
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    #endregion

#endregion
}