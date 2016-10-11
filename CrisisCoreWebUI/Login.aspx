<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CrisisCoreWebUI.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><%: Page.Title %> - CrisisCore</title>

    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="~/Content/bootstrap.css" rel="stylesheet" />
    <link href="~/Content/Login.css" rel="stylesheet" />
    <%--<webopt:BundleReference runat="server" Path="~/Content/css" />--%>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
</head>
<body>
    <div class="container">
        <form class="form-signin" id="form1" runat="server">




            <h2 class="form-signin-heading">Please log in</h2>
            <label for="txtAccountId" class="sr-only">Account ID</label>
            <asp:TextBox ID="txtAccountId" runat="server" class="form-control" placeholder="Account ID" required autofocus></asp:TextBox>
            <label for="inputPassword" class="sr-only">Password</label>
            <asp:TextBox ID="txtPassword" type="password" runat="server" class="form-control" placeholder="Password" required></asp:TextBox>
            <%--<div class="checkbox">
                <label>
                    <input type="checkbox" value="remember-me">
                    Remember me
                </label>
            </div>--%>
            <asp:Button ID="btnSubmit"  class="btn btn-lg btn-primary btn-block" runat="server" Text="Log in" OnClick="btnSubmit_Click" />
        </form>

    </div>
    <!-- /container -->

</body>
</html>
