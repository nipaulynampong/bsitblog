<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TestDB.aspx.vb" Inherits="BSIT.TestDB" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Database Test Page</title>
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { padding: 20px; }
        .container { max-width: 800px; }
        .card { margin-bottom: 20px; }
        .card-header { background-color: #2c3e50; color: white; }
        pre { background-color: #f8f9fa; padding: 15px; border-radius: 5px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Database Connection Test</h1>
            
            <div class="card">
                <div class="card-header">
                    <h5>Connection Information</h5>
                </div>
                <div class="card-body">
                    <asp:Label ID="lblConnectionInfo" runat="server" />
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">
                    <h5>Test Insert</h5>
                </div>
                <div class="card-body">
                    <div class="form-group mb-3">
                        <label for="txtUsername" class="form-label">Username:</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Text="testuser1" />
                    </div>
                    <div class="form-group mb-3">
                        <label for="txtPassword" class="form-label">Password:</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" Text="password123" />
                    </div>
                    <div class="form-group mb-3">
                        <label for="txtEmail" class="form-label">Email:</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Text="test@example.com" />
                    </div>
                    <div class="form-group mb-3">
                        <label for="txtFullName" class="form-label">Full Name:</label>
                        <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" Text="Test User" />
                    </div>
                    <div class="form-group mb-3">
                        <label for="ddlUserType" class="form-label">User Type:</label>
                        <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Admin" Value="Admin" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Editor" Value="Editor"></asp:ListItem>
                            <asp:ListItem Text="Student" Value="Student"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-check mb-3">
                        <asp:CheckBox ID="chkActive" runat="server" Checked="true" CssClass="form-check-input" />
                        <label for="chkActive" class="form-check-label">Active Account</label>
                    </div>
                    
                    <asp:Button ID="btnTestInsert" runat="server" Text="Test Insert" CssClass="btn btn-primary" OnClick="btnTestInsert_Click" />
                    <asp:Button ID="btnTestDirectInsert" runat="server" Text="Test Direct Insert" CssClass="btn btn-warning" OnClick="btnTestDirectInsert_Click" />
                </div>
            </div>
            
            <div class="card">
                <div class="card-header">
                    <h5>Test Results</h5>
                </div>
                <div class="card-body">
                    <asp:Label ID="lblResults" runat="server" />
                    <pre id="preDetails" runat="server" visible="false"></pre>
                </div>
            </div>
        </div>
    </form>
</body>
</html> 