<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AdminLogin.aspx.vb" Inherits="BSIT.AdminLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Login</title>
    <style type="text/css">
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
        }
        .login-container {
            width: 400px;
            margin: 100px auto;
            background-color: #fff;
            padding: 30px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        .login-header {
            text-align: center;
            margin-bottom: 30px;
        }
        .login-header h2 {
            color: #333;
            margin: 0;
        }
        .form-group {
            margin-bottom: 20px;
        }
        .form-control {
            width: 100%;
            padding: 10px;
            box-sizing: border-box;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
        }
        .btn-login {
            background-color: #4CAF50;
            color: white;
            border: none;
            padding: 10px 15px;
            border-radius: 4px;
            cursor: pointer;
            width: 100%;
            font-size: 16px;
        }
        .btn-login:hover {
            background-color: #45a049;
        }
        .login-footer {
            text-align: center;
            margin-top: 20px;
        }
        .login-footer a {
            color: #337ab7;
            text-decoration: none;
        }
        .message {
            padding: 10px;
            margin-bottom: 20px;
            border-radius: 4px;
        }
        .error {
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="login-container">
        <div class="login-header">
            <h2>Admin Login</h2>
            <p>Access to admin features</p>
        </div>
        
        <asp:Panel ID="pnlError" runat="server" CssClass="message error" Visible="false">
            <asp:Label ID="lblErrorMessage" runat="server"></asp:Label>
        </asp:Panel>
        
        <div class="form-group">
            <asp:Label ID="lblUsername" runat="server" Text="Username:" AssociatedControlID="txtUsername"></asp:Label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" 
                ErrorMessage="Username is required" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
        </div>
        
        <div class="form-group">
            <asp:Label ID="lblPassword" runat="server" Text="Password:" AssociatedControlID="txtPassword"></asp:Label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" 
                ErrorMessage="Password is required" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
        </div>
        
        <div class="form-group">
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" />
        </div>
        
        <div class="login-footer">
            <asp:LinkButton ID="lnkForgotPassword" runat="server" Text="Forgot Password?" PostBackUrl="~/AdminSide/ForgetPasswordAdmin.aspx"></asp:LinkButton>
        </div>
    </div>
    </form>
</body>
</html>
