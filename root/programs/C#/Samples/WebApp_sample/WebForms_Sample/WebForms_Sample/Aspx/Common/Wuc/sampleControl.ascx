<%@ Control Language="C#" AutoEventWireup="true" Inherits="WebForms_Sample.Aspx.Common.Wuc.SampleControl" Codebehind="sampleControl.ascx.cs" %>
<%@ Register Src="~/Aspx/Common/Wuc/sampleChildControl.ascx" TagPrefix="uc1" TagName="sampleChildControl" %>

<asp:Button ID="btnUCButton" runat="server" Text="SampleControl上のButton" />
<asp:Label ID="lblUCResult" runat="server" Text=""></asp:Label><br />
<uc1:sampleChildControl runat="server" ID="sampleChildControl" />
