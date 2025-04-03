<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ResetPasswordEditor.aspx.vb" Inherits="BSIT.ResetPasswordEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Password - BSIT Blog</title>
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

        .reset-container {
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

        .reset-header {
            text-align: center;
            margin-bottom: 30px;
        }

        .reset-header h1 {
            color: var(--maroon-primary);
            font-size: 24px;
            margin-bottom: 10px;
        }

        .reset-header p {
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

        .reset-footer {
            text-align: center;
            margin-top: 20px;
        }

        .reset-footer a {
            color: var(--maroon-primary);
            text-decoration: none;
            font-size: 14px;
            transition: color 0.3s ease;
        }

        .reset-footer a:hover {
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

        .password-strength {
            height: 4px;
            background-color: #ddd;
            border-radius: 2px;
            margin-top: 5px;
            overflow: hidden;
        }

        .password-strength-bar {
            height: 100%;
            width: 0;
            transition: all 0.3s ease;
        }

        .strength-weak { width: 33%; background-color: #f44336; }
        .strength-medium { width: 66%; background-color: #ff9800; }
        .strength-strong { width: 100%; background-color: #4caf50; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="reset-container">
        <div class="reset-header">
            <h1>Reset Password</h1>
            <p>Please enter your new password.</p>
        </div>

        <div class="success-message" id="successMessage" runat="server">
            Password has been reset successfully.
        </div>

        <div class="error-message" id="errorMessage" runat="server">
            Password reset failed. Please try again.
        </div>

        <div class="form-group">
            <label class="form-label">New Password</label>
            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter new password"></asp:TextBox>
            <div class="password-strength">
                <div class="password-strength-bar" id="passwordStrengthBar"></div>
            </div>
        </div>

        <div class="form-group">
            <label class="form-label">Confirm New Password</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirm new password"></asp:TextBox>
        </div>

        <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" CssClass="btn-reset" />

        <div class="reset-footer">
            <a href="EditorLogin.aspx">Back to Login</a>
        </div>
    </div>
    </form>

    <script type="text/javascript">
        function checkPasswordStrength(password) {
            let strength = 0;
            if (password.length >= 8) strength++;
            if (password.match(/[a-z]/) && password.match(/[A-Z]/)) strength++;
            if (password.match(/\d/)) strength++;
            if (password.match(/[^a-zA-Z\d]/)) strength++;

            const bar = document.getElementById('passwordStrengthBar');
            bar.className = 'password-strength-bar';
            
            if (strength <= 2) {
                bar.classList.add('strength-weak');
            } else if (strength <= 3) {
                bar.classList.add('strength-medium');
            } else {
                bar.classList.add('strength-strong');
            }
        }

        document.getElementById('<%= txtNewPassword.ClientID %>').addEventListener('input', function(e) {
            checkPasswordStrength(e.target.value);
        });
    </script>
</body>
</html>
