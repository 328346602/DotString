<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InputCK.aspx.cs" Inherits="InputTextDotString.View.Input.InputTextDotString.InputCK" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <!--link href="~/Css/plot.css" rel="stylesheet" type="text/css" /-->
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
<body class="tablerange" scroll="yes">
    <form id="form1" runat="server">
        <div class="divOuter">
        <div class="divInner">
            <!--table class="tablezw"-->
    <div>
        <asp:FileUpload ID="FileUpload" runat="server" />
        <asp:Button ID="btnAnaly" runat="server" OnClick="btnAnaly_Click" Text="解析坐标" align="right" class="tbbutton"/>
        <asp:Button ID="btnInput" runat="server" OnClick="btnInput_Click" Text="保存" align="right" class="tbbutton"/>
        <asp:Button ID="btnInputFeature" runat="server" OnClick="btnInputFeature_Click" Text="入库" align="right" class="tbbutton"/>
    
    </div>
    <p>
        <textarea id="TextArea1" cols="60" name="TextArea1" rows="10" runat="server" readonly="readonly" visible="false"></textarea></p>
        <asp:GridView ID="GridView1" runat="server" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnRowEditing="GridView1_RowEditing" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowUpdating="GridView1_RowUpdating" OnRowDeleting="GridView1_RowDeleting" CssClass="datagrid" AutoGenerateColumns="False" >
            <HeaderStyle CssClass="dg_header" />
            <Columns>
                <asp:BoundField DataField="序号" HeaderText="序号" />
                <asp:BoundField DataField="X坐标" HeaderText="X坐标" />
                <asp:BoundField DataField="Y坐标" HeaderText="Y坐标 " />
                <asp:CommandField ShowEditButton="false" ButtonType="link" CancelText="" />
            </Columns>
        </asp:GridView>
    </form>
    </body>
</html>
