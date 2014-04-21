using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Touryo.Infrastructure.Framework.Presentation;

public partial class Aspx_Common_WebUserControl : System.Web.UI.UserControl
{
    /// <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnUCButton_Click(FxEventArgs fxEventArgs)
    {
        Response.Write("UOC_btnUCButton_Clickを実行できた。");

        return "";
    }
}
