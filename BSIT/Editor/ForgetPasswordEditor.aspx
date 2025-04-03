<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ForgetPasswordEditor.aspx.vb" Inherits="BSIT.ForgetPasswordEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forgot Password - BSIT Blog</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
    <style>
        :root {
            --maroon-primary: #800000;
            --maroon-light: #A52A2A;
            --maroon-dark: #4B0082;
            --white: #FFFFFF;
            --gray-light: #F5F5F5;
            --gray-dark: #333333;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Poppins', sans-serif;
        }

        body {
            background: linear-gradient(135deg, var(--maroon-primary), var(--maroon-dark));
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .forgot-container {
            background-color: var(--white);
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 10px 20px rgba(0,0,0,0.2);
            width: 100%;
            max-width: 400px;
            animation: slideUp 0.5s ease-out;
        }

        @keyframes slideUp {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .forgot-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .forgot-header h1 {
            color: var(--maroon-primary);
            font-size: 24px;
            margin-bottom: 10px;
        }

        .forgot-header p {
            color: #666;
            font-size: 14px;
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            margin-bottom: 8px;
            color: var(--gray-dark);
            font-weight: 500;
        }

        .form-control {
            width: 100%;
            padding: 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 16px;
            transition: all 0.3s ease;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--maroon-primary);
            box-shadow: 0 0 0 2px rgba(128,0,0,0.1);
        }

        .btn-reset {
            width: 100%;
            padding: 12px;
            background-color: var(--maroon-primary);
            color: var(--white);
            border: none;
            border-radius: 5px;
            font-size: 16px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .btn-reset:hover {
            background-color: var(--maroon-light);
            transform: translateY(-2px);
        }

        .forgot-footer {
            text-align: center;
            margin-top: 20px;
        }

        .forgot-footer a {
            color: var(--maroon-primary);
            text-decoration: none;
            font-size: 14px;
            transition: color 0.3s ease;
        }

        .forgot-footer a:hover {
            color: var(--maroon-light);
        }

        .success-message {
            background-color: #e8f5e9;
            color: #2e7d32;
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 20px;
            display: none;
        }

        .error-message {
            background-color: #ffebee;
            color: #c62828;
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 20px;
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="forgot-container">
        <div class="forgot-header">
            <h1>Forgot Password</h1>
            <p>Enter your email address to reset your password.</p>
        </div>

        <div class="success-message" id="successMessage" runat="server">
            Password reset instructions have been sent to your email.
        </div>

        <div class="error-message" id="errorMessage" runat="server">
            Email address not found.
        </div>

        <div class="form-group">
            <label class="form-label">Email Address</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter your email address"></asp:TextBox>
        </div>

        <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" CssClass="btn-reset" />

        <div class="forgot-footer">
            <a href="EditorLogin.aspx">Back to Login</a>
        </div>
    </div>
    </form>
</body>
</html>
