<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InputTextDotString.aspx.cs" Inherits="InputTextDotString.InputTextDotString.InputTextDotString" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:FileUpload ID="FileUpload" runat="server" />
        <asp:Button ID="InputDotString" runat="server" OnClick="InputDotString_Click" Text="导入坐标" />
    
    </div>
        <asp:TextBox ID="txtUrl" runat="server" Height="300px" TextMode="MultiLine" Width="800px"></asp:TextBox>
    </form>
</body>
</html>
