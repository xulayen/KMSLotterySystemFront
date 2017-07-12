<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="KMSLotterySystemFront.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        数码：
        <asp:TextBox ID="txtDigit" runat="server"></asp:TextBox>
        手机号：
        <asp:TextBox ID="txtTel" runat="server"></asp:TextBox>
        openid：
        <asp:TextBox ID="txtOpenid" runat="server"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="抽奖" OnClick="Button1_Click" />
        <br />
        <br />
        <asp:Button ID="Button2" runat="server" Text="手机号码是否存在" OnClick="Button2_Click" />
        <br />
        <br />
        手机号：
        <asp:TextBox ID="txtUmobile" runat="server"></asp:TextBox>
        姓名：
        <asp:TextBox ID="txtUname" runat="server"></asp:TextBox>
        地址：
        <asp:TextBox ID="txtUaddress" runat="server"></asp:TextBox>
        中奖id：
        <asp:TextBox ID="txtlid" runat="server"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="Button3" runat="server" Text="填写信息" OnClick="Button3_Click" />
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
