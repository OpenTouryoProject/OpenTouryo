'------------------------------------------------------------------------------
' <自動生成>
'     このコードはツールによって生成されました。
'
'     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
'     コードが再生成されるときに損失したりします。 
' </自動生成>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Namespace Aspx.Sample._3Tier

    Partial Public Class ProductsConditionalSearch

        '''<summary>
        '''ddlDap Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents ddlDap As Global.Touryo.Infrastructure.CustomControl.WebCustomDropDownList

        '''<summary>
        '''txtProductID_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductID_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtProductName_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductName_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''ddlSupplierID_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents ddlSupplierID_And As Global.Touryo.Infrastructure.CustomControl.WebCustomDropDownList

        '''<summary>
        '''ddlCategoryID_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents ddlCategoryID_And As Global.Touryo.Infrastructure.CustomControl.WebCustomDropDownList

        '''<summary>
        '''txtQuantityPerUnit_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtQuantityPerUnit_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitPrice_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitPrice_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsInStock_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsInStock_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsOnOrder_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsOnOrder_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtReorderLevel_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtReorderLevel_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtDiscontinued_And Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtDiscontinued_And As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtProductID_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductID_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtProductName_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductName_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtSupplierID_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtSupplierID_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtCategoryID_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtCategoryID_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtQuantityPerUnit_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtQuantityPerUnit_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitPrice_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitPrice_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsInStock_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsInStock_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsOnOrder_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsOnOrder_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtReorderLevel_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtReorderLevel_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtDiscontinued_And_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtDiscontinued_And_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtProductID_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductID_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtProductName_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductName_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtSupplierID_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtSupplierID_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtCategoryID_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtCategoryID_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtQuantityPerUnit_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtQuantityPerUnit_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitPrice_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitPrice_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsInStock_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsInStock_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsOnOrder_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsOnOrder_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtReorderLevel_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtReorderLevel_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtDiscontinued_OR Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtDiscontinued_OR As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtProductID_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductID_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtProductName_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtProductName_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtSupplierID_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtSupplierID_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtCategoryID_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtCategoryID_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtQuantityPerUnit_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtQuantityPerUnit_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitPrice_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitPrice_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsInStock_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsInStock_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtUnitsOnOrder_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtUnitsOnOrder_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtReorderLevel_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtReorderLevel_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''txtDiscontinued_OR_Like Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents txtDiscontinued_OR_Like As Global.System.Web.UI.WebControls.TextBox

        '''<summary>
        '''btnSearch Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents btnSearch As Global.System.Web.UI.WebControls.Button

        '''<summary>
        '''btnInsert Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents btnInsert As Global.System.Web.UI.WebControls.Button

        '''<summary>
        '''gvwGridView1 Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents gvwGridView1 As Global.System.Web.UI.WebControls.GridView

        '''<summary>
        '''ObjectDataSource1 Control。
        '''</summary>
        '''<remarks>
        '''自動生成されたフィールド。
        '''変更するには、フィールドの宣言をデザイナー ファイルから分離コード ファイルに移動します。
        '''</remarks>
        Protected WithEvents ObjectDataSource1 As Global.System.Web.UI.WebControls.ObjectDataSource
    End Class
End Namespace
