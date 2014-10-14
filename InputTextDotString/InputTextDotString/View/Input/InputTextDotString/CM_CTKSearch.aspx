<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CM_CTKSearch.aspx.cs" Inherits="InputTextDotString.View.Input.InputTextDotString.CM_CTKSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <base target="_self">
    <title>案件立案</title>
     <link href="../../../Content/themes/default/Site.css" type="text/css" rel="stylesheet">
    <%--<link href="/Content/themes/default/Site.css" type="text/css" rel="stylesheet">--%>
    <style type="text/css">
        #Button
        {
            text-align: justify;
            width: 46px;
        }
        .style1
        {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <table style="WIDTH: 600pt; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed" 
            class="tablerange" border="0" cellspacing="0" cellpadding="0" align="center">
            <tbody>
                <tr style="HEIGHT: 32.25pt; mso-height-source: userset" height="43" class="tabletop">
                    <td style="border-style: #3366FF; border-width: 1px;" class="style1" height="43" 
                        width="523" >
                        <font id="font1" size="4">选择矿权</font></td>
                </tr>
                <tr style="HEIGHT: 13.5pt" height="18">
                    <td style="border-style: #3366FF; border-width: 1px;" class="style1" 
                        height="18">
                        采矿证号 
                        <asp:TextBox ID="txtAJMC" runat="server"></asp:TextBox>
&nbsp;&nbsp;<asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click"  class="tbbutton"/>
                    </td>
                </tr>
                <tr >
                    <td  class="xl82" style="border-style: #3366FF; border-width: 1px;" >
                        <asp:GridView ID="GridView" runat="server" 
                            Width="100%" AllowPaging="True" class="MyDataGrid"
                            CellPadding="4" ForeColor="Blue" GridLines="None" 
                            AutoGenerateColumns="False" EmptyDataText="无数据！" BorderWidth="1px" DataKeyNames ="CK_GUID" 
                            style="text-align: left" ShowHeaderWhenEmpty="True" OnSelectedIndexChanged="GridView_SelectedIndexChanged">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <Columns>
                                <asp:BoundField DataField="许可证号" HeaderText="许可证号" ItemStyle-VerticalAlign="Middle">
                                <FooterStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <HeaderStyle Width="40%" BorderColor="#3366FF" 
                                    BorderWidth="1px" VerticalAlign="Middle" HorizontalAlign="Center" />

<ItemStyle VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="矿山名称" HeaderText="矿山名称" ItemStyle-VerticalAlign="Middle">
                                <FooterStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <HeaderStyle Width="50%" BorderColor="#3366FF" 
                                    BorderWidth="1px" VerticalAlign="Middle" HorizontalAlign="Center" />

<ItemStyle VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="CK_GUID" HeaderText="CK_GUID" ItemStyle-VerticalAlign="Middle" visible="false">
                                <FooterStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <HeaderStyle Width="30%" BorderColor="#3366FF" 
                                    BorderWidth="1px" VerticalAlign="Middle" HorizontalAlign="Center" />

<ItemStyle VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundField>
                                <asp:CommandField ButtonType="Button" InsertVisible="False" ShowCancelButton="False" ShowSelectButton="True" HeaderText="按钮" />
                            </Columns>
                            <EditRowStyle BackColor="#999999" />
                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle Height="20px"  Font-Bold="True" BackColor="lightblue" VerticalAlign="Middle"
                                ForeColor="White" BorderWidth="1px" BorderColor="#0066FF" />
<%--                            <PagerSettings FirstPageText="首页" LastPageText="末页" NextPageText="下页" 
                                PreviousPageText="上页" />--%>
                            <PagerTemplate> 
                                <asp:LinkButton ID="lb_firstpage" runat="server" onclick="lb_firstpage_Click">首页</asp:LinkButton> 
                                <asp:LinkButton ID="lb_previouspage" runat="server" 
                                onclick="lb_previouspage_Click">上一页</asp:LinkButton> 
                                <asp:LinkButton ID="lb_nextpage" runat="server" onclick="lb_nextpage_Click">下一页</asp:LinkButton> 
                                <asp:LinkButton ID="lb_lastpage" runat="server" onclick="lb_lastpage_Click">尾页</asp:LinkButton> 
                                第<asp:Label ID="lbl_nowpage" runat="server" Text="<%#GridView.PageIndex+1 %>" ForeColor="#db530f"></asp:Label>页/共<asp:Label 
                                ID="lbl_totalpage" runat="server" Text="<%#GridView.PageCount %>" ForeColor="#db530f"></asp:Label>页 
                                </PagerTemplate> 
                            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle Height="20px"  ForeColor="#333333" 
                                BorderColor="#3366FF" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#E9E7E2" />
                            <SortedAscendingHeaderStyle BackColor="#506C8C" />
                            <SortedDescendingCellStyle BackColor="#FFFDF8" />
                            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                        </asp:GridView>                                                  
                                                          
                    </td>
                </tr>
            </tbody>
    </table>
    </div>
    </form>
</body>
</html>
