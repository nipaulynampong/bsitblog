<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EditorSettings.aspx.vb" Inherits="BSIT.EditorSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
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
</asp:Content>
