<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AnalyResult.aspx.cs" Inherits="InputTextDotString.View.Input.InputTextDotString.AnalyResult" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        (function () {
            $("#main").width(screen.availWidth);  //只有外层的div设置了固定宽度，内部的百分比宽度才会有效果。      
        })

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="main" style="min-width:1000px; height: 100%">
        <div id="divTree" style="float: left; width: 15%;  height: 900px;">
            请选择年度:
            <asp:TreeView ID="treeView" runat="Server" Height="800" OnSelectedNodeChanged="treeView_SelectedNodeChanged">
            </asp:TreeView>
        </div>
        <div id="divContent" style="width: 85%;  height: 900px; float: left;
            overflow: hidden">
            <iframe id="iframeContent" frameborder="0" scrolling="no" style="width: 100%; height: 900px;"
                runat="Server"></iframe>
        </div>
    </div>
    </form>
</body>
</html>
