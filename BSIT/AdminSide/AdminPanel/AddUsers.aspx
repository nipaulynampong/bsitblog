<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AddUsers.aspx.vb" Inherits="BSIT.BSIT.AddUsers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add New User</title>
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: white;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
        }
        .container {
            max-width: 800px;
            padding: 30px;
            margin: 0 auto;
        }
        .card {
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-bottom: 20px;
            border: none;
        }
        .card-header {
            background-color: #2c3e50;
            color: white;
            padding: 15px 20px;
            border-radius: 8px 8px 0 0;
            font-weight: 600;
            font-size: 18px;
        }
        .card-body {
            padding: 30px;
        }
        .form-group {
            margin-bottom: 20px;
        }
        .form-label {
            font-weight: 500;
            margin-bottom: 8px;
            display: block;
        }
        .form-control {
            padding: 10px 12px;
            border-radius: 5px;
            border: 1px solid #ddd;
            width: 95%;
        }
        .form-control:focus {
            border-color: #3498db;
            box-shadow: 0 0 0 0.2rem rgba(52, 152, 219, 0.25);
        }
        .btn {
            padding: 10px 20px;
            font-weight: 500;
            border-radius: 5px;
        }
        .btn-primary {
            background-color: #3498db;
            border-color: #3498db;
        }
        .btn-primary:hover {
            background-color: #2980b9;
            border-color: #2980b9;
        }
        .btn-secondary {
            background-color: #7f8c8d;
            border-color: #7f8c8d;
        }
        .btn-secondary:hover {
            background-color: #6c7a7a;
            border-color: #6c7a7a;
        }
        .button-group {
            display: flex;
            gap: 10px;
            margin-top: 10px;
        }
        .validation-error {
            color: #e74c3c;
            font-size: 14px;
            margin-top: 5px;
        }
        .required-field::after {
            content: " *";
            color: #e74c3c;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="card">
                <div class="card-header">
                    <i class="fas fa-user-plus"></i> Add New User
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <label for="txtUsername" class="form-label required-field">Username</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                            ControlToValidate="txtUsername" 
                            ErrorMessage="Username is required" 
                            CssClass="validation-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtFullName" class="form-label required-field">Full Name</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvFullName" runat="server" 
                            ControlToValidate="txtFullName" 
                            ErrorMessage="Full name is required" 
                            CssClass="validation-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtEmail" class="form-label required-field">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                            ControlToValidate="txtEmail" 
                            ErrorMessage="Email is required" 
                            CssClass="validation-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                            ControlToValidate="txtEmail" 
                            ErrorMessage="Please enter a valid email address" 
                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                            CssClass="validation-error"
                            Display="Dynamic">
                        </asp:RegularExpressionValidator>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtPassword" class="form-label required-field">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
                            ControlToValidate="txtPassword" 
                            ErrorMessage="Password is required" 
                            CssClass="validation-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                    </div>
                    
                    <div class="form-group">
                        <label for="txtConfirmPassword" class="form-label required-field">Confirm Password</label>
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="50"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" 
                            ControlToValidate="txtConfirmPassword" 
                            ErrorMessage="Please confirm your password" 
                            CssClass="validation-error"
                            Display="Dynamic">
                        </asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="cvPassword" runat="server" 
                            ControlToValidate="txtConfirmPassword" 
                            ControlToCompare="txtPassword" 
                            ErrorMessage="Passwords do not match" 
                            CssClass="validation-error"
                            Display="Dynamic">
                        </asp:CompareValidator>
                    </div>
                    
                    <div class="form-group">
                        <label for="ddlUserType" class="form-label required-field">User Type</label>
                        <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-control">
                            <asp:ListItem Text="-- Select User Type --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                            <asp:ListItem Text="Editor" Value="Editor"></asp:ListItem>
                            <asp:ListItem Text="Student" Value="Student"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvUserType" runat="server" 
                            ControlToValidate="ddlUserType" 
                            ErrorMessage="Please select a user type" 
                            CssClass="validation-error"
                            Display="Dynamic"
                            InitialValue="">
                        </asp:RequiredFieldValidator>
                    </div>
                    
                    <div class="form-group">
                        <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                        <label for="chkActive" class="form-label" style="display: inline-block; margin-left: 5px;">Active Account</label>
                    </div>

                    <asp:Label ID="lblMessage" runat="server" CssClass="validation-error" Visible="false"></asp:Label>
                    
                    <div class="button-group">
                        <asp:Button ID="btnSave" runat="server" Text="Save User" CssClass="btn btn-primary" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" CausesValidation="false" />
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
</body>
</html>
