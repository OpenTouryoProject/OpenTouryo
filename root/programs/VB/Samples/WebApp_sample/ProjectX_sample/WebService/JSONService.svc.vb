
'**********************************************************************************
'* フレームワーク・テスト画面

'**********************************************************************************

'**********************************************************************************
'* クラス名        ：JSONService
'* クラス日本語名  ：JSONService
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/09/11  Supragyan         Created JSONService for invoking Product table data.
'*  2015/03/20  Sai               Modified method 'GetProductData()' return type to Json string
'*                                and added paging parameters
'*  2015/06/08  Supragyan         Modified startIndex and maximumRows in method 'GetProductData()'
'**********************************************************************************

' System
Imports System.Data
Imports System.Web
Imports System.Collections.Specialized
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Channels

'Touryo
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Dto

'Newtonsoft
Imports Newtonsoft.Json

''' <summary>
''' JSONService class for selecting product table data and displaying in JQGrid.
''' </summary>
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)>
Public Class JSONService
    Implements IJSONService

    Function GetProductData() As Message Implements IJSONService.GetProductData

        HttpContext.Current.Session("DAP") = "SQL"
        HttpContext.Current.Session("DBMS") = DbEnum.DBMSType.SQLServer

        Dim queryStrings As NameValueCollection = HttpContext.Current.Request.QueryString

        HttpContext.Current.Session("SortExpression") = queryStrings("sidx")
        HttpContext.Current.Session("SortDirection") = queryStrings("sord")
        Dim currentPage As String = queryStrings("page")
        Dim rows As String = queryStrings("rows")

        Dim startIndex As Integer = (Integer.Parse(currentPage) - 1) * Integer.Parse(rows)
        Dim maximumRows As Integer = Integer.Parse(rows)

        Dim productTableAdapter As New ProductsTableAdapter()

        ' Calling SelectMethod to fetch Product table data
        Dim productTableData As DataTable = productTableAdapter.SelectMethod(startIndex, maximumRows)

        ' Calling SelectCountMethod to get count of product table data
        Dim totalCount As Integer = productTableAdapter.SelectCountMethod()

        ' Calling SavejqGridJson
        Dim dtTable As New DTTable("Product")
        Dim jqGridObject As Object = dtTable.SavejqGridJson(productTableData, totalCount, currentPage, rows)

        ' Converts Product table into JSon strig
        Dim jsonData As String = JsonConvert.SerializeObject(jqGridObject)

        ' Converts JSON data to Message format.
        WebOperationContext.Current.OutgoingResponse.Headers.Add("X-Content-Type-Options", "nosniff")

        ' returns JSON string in Message format
        Return WebOperationContext.Current.CreateTextResponse(jsonData, "application/json; charset=utf-8", Encoding.UTF8)

    End Function

End Class
