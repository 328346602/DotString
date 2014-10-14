<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DelFeature.aspx.cs" Inherits="InputTextDotString.View.Input.InputTextDotString.DelFeature" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="~/VFDSourceFile/Content/themes/default/Site.css" type="text/css" rel="stylesheet">
    <style type="text/css">
        .divOuter
        {
            width: 90%;
            margin: 0 auto;
        }
        .divInner
        {
            width: 100%;
        }
        table
        {
            border-collapse: collapse;
            width: 100%;
        }
        .style1
        {
            height: 29px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <br>
        <br>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="DeleteFeature" runat="server" Text="删除矿区" OnClick="DeleteFeature_Click" class="tbbutton"/>
&nbsp;<asp:Button ID="Cancel" runat="server" Text="取消" OnClick="Cancel_Click" class="tbbutton"/>
    
    </div>
    </form>
</body>
</html>
