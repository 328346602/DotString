<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InputDotString.aspx.cs" Inherits="InputTextDotString.View.Input.InputTextDotString.InputDotString" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="btnAnaly" runat="server" Text="解析" OnClick="btnAnaly_Click" />
        <asp:Button ID="btnInput" runat="server" Text="入库" />
    
    </div>
        <asp:GridView ID="GridView1" runat="server" Height="400" Width="800" AllowPaging="True">
        </asp:GridView>
    </form>
</body>
</html>
