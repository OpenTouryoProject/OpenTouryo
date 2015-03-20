//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：JSONService
//* クラス日本語名  ：JSONService
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2015/03/05  Supragyan         Created JSONService for invoking Product table data.
//*  2015/03/20  Sai               Modified method 'GetProductData()' return type to Json string 
//*                                and added paging parameters      
//**********************************************************************************

//System
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//Touryo
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Public.Dto;
using Newtonsoft.Json;

public class JSONService : IJSONService
{
    /// <summary>
    /// GetProductData method for fetching product table data.
    /// </summary>
    public string GetProductData(int startIndex, int lastindex)
    {
        HttpContext.Current.Session["DAP"] = "SQL";
        HttpContext.Current.Session["DBMS"] = DbEnum.DBMSType.SQLServer;

        ProductsTableAdapter productTableAdapter = new ProductsTableAdapter();
        DataTable productTableData = productTableAdapter.SelectMethod(startIndex, lastindex);
        int count = productTableAdapter.SelectCountMethod();

        //Calling SavejqGridJson
        DTTable dtTable = new DTTable("Product");
        dtTable.SavejqGridJson(productTableData, count);

        //Converts Product table into JSon strig
        return JsonConvert.SerializeObject(dtTable);
    }
}
