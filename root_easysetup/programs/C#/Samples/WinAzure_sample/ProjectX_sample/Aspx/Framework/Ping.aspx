<%@ Page Language="C#" AutoEventWireup="true" Inherits="Aspx_Framework_Ping" Codebehind="Ping.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>無題のページ</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%=Session.SessionID.ToString()%>
    </div>
    </form>
</body>
</html>
