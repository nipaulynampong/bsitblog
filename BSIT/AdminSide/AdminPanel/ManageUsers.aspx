<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageUsers.aspx.vb" Inherits="BSIT.ManageUsers" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Users</title>
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
            max-width: 100%;
            padding: 20px;
        }
        .action-buttons {
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 15px;
            flex-wrap: wrap;
        }
        .search-box {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-grow: 1;
            max-width: 450px;
        }
        .search-input {
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 8px 12px;
            width: 100%;
        }
        .filter-box {
            min-width: 200px;
        }
        .filter-box select {
            width: 100%;
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
        }
        .btn {
            padding: 8px 16px;
            border-radius: 5px;
            font-weight: 500;
        }
        .btn-primary {
            background-color: #3498db;
            border-color: #3498db;
        }
        .btn-add {
            background-color: #2c3e50;
            border-color: #2c3e50;
            color: white;
            display: flex;
            align-items: center;
            gap: 5px;
        }
        .btn-add:hover {
            background-color: #1a252f;
            border-color: #1a252f;
        }
        .btn-primary:hover {
            background-color: #2980b9;
            border-color: #2980b9;
        }
        .table-container {
            border-radius: 5px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            margin-bottom: 20px;
        }
        .table {
            width: 100%;
            margin-bottom: 0;
            border-collapse: collapse;
            border: none;
        }
        .table thead {
            background: #2c3e50;
            color: white;
        }
        .table th {
            font-weight: 600;
            border: none;
            text-align: center;
            padding: 12px 15px;
             background: #2c3e50;
            color: white;
        }
        .table td {
            text-align: center;
            vertical-align: middle;
            border: none;
            border-bottom: 1px solid #eee;
            padding: 12px 15px;
        }
        .table tr:last-child td {
            border-bottom: none;
        }
        .table tr:hover {
            background-color: #f8f9fa;
        }
        .status-active {
            color: #28a745;
            font-weight: 600;
            display: inline-block;
            padding: 4px 8px;
            background-color: rgba(40, 167, 69, 0.1);
            border-radius: 12px;
        }
        .status-inactive {
            color: #dc3545;
            font-weight: 600;
            display: inline-block;
            padding: 4px 8px;
            background-color: rgba(220, 53, 69, 0.1);
            border-radius: 12px;
        }
        .btn-action {
            margin: 0 3px;
            border-radius: 4px;
            padding: 6px 12px;
        }
        .btn-warning {
            background-color: #3498db;
            border-color: #3498db;
            color: white;
        }
        .btn-warning:hover {
            background-color: #2980b9;
            border-color: #2980b9;
        }
        .btn-danger {
            background-color: #f39c12;
            border-color: #f39c12;
        }
        .btn-danger:hover {
            background-color: #e67e22;
            border-color: #e67e22;
        }
        .btn-success:hover {
            background-color: #27ae60;
            border-color: #27ae60;
        }
        .gridview-container {
            margin: 0;
            overflow-x: auto;
        }
        .no-results {
            text-align: center;
            padding: 30px;
            background-color: #f8f9fa;
            border-radius: 5px;
            margin-top: 20px;
            color: #6c757d;
            font-size: 16px;
            border: 1px solid #eee;
        }
        .empty-data-template {
            text-align: center;
            padding: 30px;
            color: #6c757d;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:HiddenField ID="hdnDataLoaded" runat="server" Value="false" />
        
        <div class="container">
            <div class="action-buttons">
                <div class="search-box">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control search-input" style="placeholder: 'Search by username or email...'"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                </div>
                
                <div class="filter-box">
                    <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="btnSearch_Click">
                        <asp:ListItem Text="All User Types" Value="" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                        <asp:ListItem Text="Editor" Value="Editor"></asp:ListItem>
                        <asp:ListItem Text="Student" Value="Student"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <asp:Button ID="btnAddNew" runat="server" Text="Add New User" CssClass="btn btn-add" OnClick="btnAddUser_Click" />
            </div>

            <asp:Panel ID="pnlNoResults" runat="server" CssClass="no-results" Visible="false">
                <p>No users found. Try a different search or add a new user.</p>
            </asp:Panel>

            <div class="gridview-container">
                <div class="table-container">
                    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-striped" 
                        OnRowCommand="gvUsers_RowCommand"
                        OnRowDataBound="gvUsers_RowDataBound"
                        OnPreRender="gvUsers_PreRender"
                        DataKeyNames="UserID">
                        <HeaderStyle CssClass="text-center" />
                        <RowStyle CssClass="text-center" />
                        <Columns>
                            <asp:BoundField DataField="UserID" HeaderText="ID" ReadOnly="True" Visible="false" />
                            <asp:BoundField DataField="username" HeaderText="Username" />
                            <asp:BoundField DataField="email" HeaderText="Email" />
                            <asp:BoundField DataField="Role" HeaderText="User Type" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:BoundField DataField="createdDate" HeaderText="Created Date" DataFormatString="{0:MM/dd/yyyy}" Visible="false" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <a href='EditUsers.aspx?id=<%# Eval("UserID") %>' class="btn btn-warning btn-sm btn-action">
                                        <i class="fas fa-edit"></i> Edit
                                    </a>
                                    <asp:Button ID="btnDeactivate" runat="server" 
                                        Text='<%# IIf(Eval("Status").ToString() = "Active", "Deactivate", "Activate") %>' 
                                        CommandName='<%# IIf(Eval("Status").ToString() = "Active", "DeactivateUser", "ActivateUser") %>' 
                                        CommandArgument='<%# Eval("UserID") %>'
                                        CssClass='<%# IIf(Eval("Status").ToString() = "Active", "btn btn-danger btn-sm btn-action", "btn btn-success btn-sm btn-action") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="empty-data-template">
                                <p>No users found. Try a different search or add a new user.</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
    <script type="text/javascript">
        function pageLoad() {
            // Only trigger data load on first page load, not on postbacks
            if (document.getElementById('<%= hdnDataLoaded.ClientID %>').value === "false") {
                // Set the hidden field to true to prevent future loads
                document.getElementById('<%= hdnDataLoaded.ClientID %>').value = "true";
                // Only click the button if this is not a postback
                if (!document.getElementById('__EVENTTARGET') || document.getElementById('__EVENTTARGET').value === '') {
                    setTimeout(function() {
                        document.getElementById('<%= btnSearch.ClientID %>').click();
                    }, 100);
                }
            }
        }
    </script>
</body>
</html>
