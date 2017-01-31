//**********************************************************************************
//* フレームワーク・テスト API
//**********************************************************************************

//  API画面なので、必要に応じて流用 or 削除して下さい。

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
//*  2015/06/08  Supragyan         Modified startIndex and maximumRows in method 'GetProductData()'    
//**********************************************************************************

using Newtonsoft.Json;

using System.Text;
using System.Data;
using System.Web;
using System.Collections.Specialized;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Dto;

/// <summary>ProjectX_sample</summary>
namespace ProjectX_sample
{
    /// <summary>
    /// JSONService class for selecting product table data and displaying in JQGrid.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class JSONService : IJSONService
    {
        /// <summary>
        /// GetProductData method for fetching product table data.
        /// </summary>
        /// <returns>Message</returns>
        public Message GetProductData()
        {
            HttpContext.Current.Session["DAP"] = "SQL";
            HttpContext.Current.Session["DBMS"] = DbEnum.DBMSType.SQLServer;

            NameValueCollection queryStrings = HttpContext.Current.Request.QueryString;

            HttpContext.Current.Session["SortExpression"] = queryStrings["sidx"];
            HttpContext.Current.Session["SortDirection"] = queryStrings["sord"];
            string currentPage = queryStrings["page"];
            string rows = queryStrings["rows"];
            int startIndex = (int.Parse(currentPage) - 1) * int.Parse(rows);
            int maximumRows = int.Parse(rows);

            ProductsTableAdapter productTableAdapter = new ProductsTableAdapter();
            DataTable productTableData = productTableAdapter.SelectMethod(startIndex, maximumRows);
            int totalCount = productTableAdapter.SelectCountMethod();

            // Calling SavejqGridJson
            DTTable dtTable = new DTTable("Product");
            object jqGridObject = dtTable.SavejqGridJson(productTableData, totalCount, currentPage, rows);

            // Converts Product table into JSon strig
            string jsonData = JsonConvert.SerializeObject(jqGridObject);

            // Converts JSON data to Message format.
            WebOperationContext.Current.OutgoingResponse.Headers.Add("X-Content-Type-Options", "nosniff");

            // returns JSON string in Message format
            return WebOperationContext.Current.CreateTextResponse(jsonData, "application/json; charset=utf-8", Encoding.UTF8);
        }
    }
}


