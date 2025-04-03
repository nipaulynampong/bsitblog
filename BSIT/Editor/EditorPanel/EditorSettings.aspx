<%@ Page Language="vb" AutoEventWireup="true" CodeFile="EditorSettings.aspx.vb" Inherits="BSIT.EditorSettings" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Editor Settings</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        :root {
            --maroon-primary: #800000;
            --maroon-light: #A52A2A;
            --maroon-dark: #4B0082;
            --white: #FFFFFF;
            --gray-light: #F5F5F5;
            --gray-dark: #333333;
        }

        body {
            background-color: var(--gray-light);
            font-family: 'Poppins', sans-serif;
            margin: 0;
            padding: 0;
            color: var(--gray-dark);
        }

        .container {
            max-width: 800px;
            padding: 30px 15px;
            margin: 0 auto;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 20px;
            color: var(--maroon-primary);
        }

        .settings-container {
            max-width: 600px;
            margin: 0 auto;
        }

        .settings-section {
            background-color: var(--white);
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .settings-section h3 {
            color: var(--maroon-primary);
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid var(--gray-light);
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
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 16px;
            transition: border-color 0.3s ease;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--maroon-primary);
        }

        .profile-image {
            width: 100px;
            height: 100px;
            border-radius: 50%;
            background-color: var(--maroon-light);
            margin-bottom: 15px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: var(--white);
            font-size: 40px;
        }

        .btn {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-weight: 500;
            transition: all 0.3s ease;
        }

        .btn-primary {
            background-color: var(--maroon-primary);
            color: var(--white);
        }

        .btn-primary:hover {
            background-color: var(--maroon-light);
        }

        .notification-toggle {
            display: flex;
            align-items: center;
            margin-bottom: 10px;
        }

        .toggle-switch {
            position: relative;
            display: inline-block;
            width: 50px;
            height: 24px;
            margin-right: 10px;
        }

        .toggle-switch input {
            opacity: 0;
            width: 0;
            height: 0;
        }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            transition: .4s;
            border-radius: 24px;
        }

        .slider:before {
            position: absolute;
            content: "";
            height: 16px;
            width: 16px;
            left: 4px;
            bottom: 4px;
            background-color: white;
            transition: .4s;
            border-radius: 50%;
        }

        input:checked + .slider {
            background-color: var(--maroon-primary);
        }

        input:checked + .slider:before {
            transform: translateX(26px);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1 class="page-title"><i class="fas fa-cog"></i> Editor Settings</h1>
            
            <div class="settings-container">
                <div class="settings-section">
                    <h3>Profile Settings</h3>
                    <div class="profile-image">
                        <i class="fas fa-user"></i>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Display Name</label>
                        <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Bio</label>
                        <asp:TextBox ID="txtBio" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                    </div>
                </div>

                <div class="settings-section">
                    <h3>Password Settings</h3>
                    <div class="form-group">
                        <label class="form-label">Current Password</label>
                        <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">New Password</label>
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Confirm New Password</label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                    </div>
                </div>

                <div class="settings-section">
                    <h3>Notification Settings</h3>
                    <div class="notification-toggle">
                        <label class="toggle-switch">
                            <asp:CheckBox ID="chkEmailNotifications" runat="server" />
                            <span class="slider"></span>
                        </label>
                        <span>Email Notifications</span>
                    </div>
                    <div class="notification-toggle">
                        <label class="toggle-switch">
                            <asp:CheckBox ID="chkCommentNotifications" runat="server" />
                            <span class="slider"></span>
                        </label>
                        <span>Comment Notifications</span>
                    </div>
                </div>

                <div class="form-group">
                    <asp:Button ID="btnSaveSettings" runat="server" Text="Save Changes" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </form>
    <script src="../../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../../Scripts/bootstrap.bundle.min.js"></script>
</body>
</html>
