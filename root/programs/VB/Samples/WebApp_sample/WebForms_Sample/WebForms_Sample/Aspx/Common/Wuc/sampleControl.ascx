<%@ Control Language="VB" AutoEventWireup="false" Codebehind="sampleControl.ascx.vb" Inherits="WebForms_Sample.Aspx.Common.Wuc.SampleControl" %>
<%@ Register src="sampleChildControl.ascx" tagname="sampleChildControl" tagprefix="uc1" %>

<asp:Button ID="btnUCButton" runat="server" Text="SampleControl上のButton" />
<asp:Label ID="lblUCResult" runat="server" Text=""></asp:Label><br />
<uc1:sampleChildControl ID="sampleChildControl" runat="server" />

