<%@ Page Language="vb" AutoEventWireup="true" CodeFile="ArchivePosts.aspx.vb" Inherits="BSIT.ArchivePosts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Archive Posts</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        :root {
            --primary-color: #4361ee;
            --secondary-color: #3f37c9;
            --accent-color: #4895ef;
            --success-color: #4cc9f0;
            --warning-color: #f72585;
            --info-color: #4361ee;
            --light-color: #f8f9fa;
            --dark-color: #212529;
            --danger-color: #e63946;
            --text-color: #2b2d42;
            --border-radius: 8px;
            --card-shadow: 0 4px 6px rgba(0, 0, 0, 0.1), 0 1px 3px rgba(0, 0, 0, 0.08);
        }

        body {
            background-color: #f0f2f5;
            font-family: 'Poppins', sans-serif;
            margin: 0;
            padding: 0;
            color: var(--text-color);
        }

        .container {
            max-width: 1200px;
            padding: 30px 15px;
            margin: 0 auto;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 20px;
            color: var(--primary-color);
        }

        .card {
            border-radius: var(--border-radius);
            box-shadow: var(--card-shadow);
            margin-bottom: 25px;
            border: none;
            background-color: #fff;
            overflow: hidden;
        }

        .card-header {
            background: linear-gradient(90deg, var(--primary-color) 0%, var(--secondary-color) 100%);
            color: white;
            padding: 20px;
            border-bottom: none;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .card-header h5 {
            font-weight: 600;
            font-size: 18px;
            margin: 0;
            display: flex;
            align-items: center;
        }

        .card-header i {
            margin-right: 10px;
        }

        .card-body {
            padding: 25px;
        }

        .action-buttons {
            margin-bottom: 25px;
            display: flex;
            align-items: stretch;
            gap: 15px;
            flex-wrap: wrap;
        }

        .search-box {
            display: flex;
            align-items: stretch;
            flex-grow: 1;
            max-width: 450px;
            border-radius: var(--border-radius);
            overflow: hidden;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }

        .search-input {
            border: 1px solid #e0e0e0;
            border-right: none;
            border-top-left-radius: var(--border-radius);
            border-bottom-left-radius: var(--border-radius);
            padding: 12px 15px;
            width: 100%;
            font-size: 14px;
            outline: none;
            transition: border-color 0.2s;
        }

        .search-input:focus {
            border-color: var(--primary-color);
            box-shadow: 0 0 0 0.2rem rgba(67, 97, 238, 0.25);
        }

        .filter-box {
            min-width: 200px;
            flex-grow: 1;
            max-width: 250px;
        }

        .filter-box select {
            width: 100%;
            padding: 12px 15px;
            border: 1px solid #e0e0e0;
            border-radius: var(--border-radius);
            font-size: 14px;
            background-color: white;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
            outline: none;
            transition: border-color 0.2s;
        }

        .filter-box select:focus {
            border-color: var(--primary-color);
            box-shadow: 0 0 0 0.2rem rgba(67, 97, 238, 0.25);
        }

        .btn {
            padding: 12px 20px;
            border-radius: var(--border-radius);
            font-weight: 500;
            font-size: 14px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            transition: all 0.3s;
            border: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 5px;
            cursor: pointer;
        }

        .btn-sm {
            padding: 6px 10px;
            font-size: 12px;
        }

        .btn-primary {
            background: linear-gradient(90deg, var(--primary-color) 0%, var(--accent-color) 100%);
            color: white;
            border: none;
        }

        .btn-primary:hover {
            background: linear-gradient(90deg, var(--secondary-color) 0%, var(--primary-color) 100%);
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(67, 97, 238, 0.3);
        }

        .btn-success {
            background-color: #4cc9f0;
            border-color: #4cc9f0;
        }

        .btn-danger {
            background-color: var(--danger-color);
            border-color: var(--danger-color);
        }

        .btn-warning {
            background-color: #f72585;
            border-color: #f72585;
            color: white;
        }

        .btn-info {
            background-color: var(--info-color);
            border-color: var(--info-color);
            color: white;
        }

        .table {
            width: 100%;
            margin-bottom: 0;
            background-color: transparent;
            border-collapse: separate;
            border-spacing: 0 5px;
        }

        .table th {
            background-color: #f8f9fa;
            font-weight: 600;
            padding: 15px;
            text-transform: uppercase;
            font-size: 12px;
            letter-spacing: 1px;
            border: none;
            color: #6c757d;
        }

        .table td {
            vertical-align: middle;
            padding: 15px;
            border: none;
            background-color: white;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.05);
        }

        .table tr td:first-child {
            border-top-left-radius: var(--border-radius);
            border-bottom-left-radius: var(--border-radius);
        }

        .table tr td:last-child {
            border-top-right-radius: var(--border-radius);
            border-bottom-right-radius: var(--border-radius);
        }

        .status {
            font-weight: 600;
        }

        .status-archived {
            color: #6c757d;
        }

        .post-title {
            font-weight: 600;
            color: var(--text-color);
            max-width: 300px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .post-excerpt {
            max-width: 400px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            color: #6c757d;
            font-size: 13px;
            margin-top: 5px;
        }

        .action-buttons-cell {
            white-space: nowrap;
            display: flex;
            gap: 5px;
            justify-content: center;
        }

        .action-buttons-cell .btn {
            margin: 0 2px;
        }

        .archive-date {
            font-size: 13px;
            color: #6c757d;
        }

        .breadcrumb {
            display: flex;
            flex-wrap: wrap;
            padding: 0;
            margin-bottom: 20px;
            list-style: none;
            gap: 5px;
            font-size: 14px;
        }

        .breadcrumb-item {
            display: flex;
            align-items: center;
        }

        .breadcrumb-item a {
            color: var(--primary-color);
            text-decoration: none;
            transition: color 0.2s;
        }

        .breadcrumb-item a:hover {
            color: var(--secondary-color);
            text-decoration: underline;
        }

        .breadcrumb-separator {
            margin: 0 8px;
            color: #6c757d;
        }

        .breadcrumb-active {
            color: #6c757d;
        }

        .no-results {
            text-align: center;
            padding: 60px 30px;
            background-color: white;
            border-radius: var(--border-radius);
            box-shadow: var(--card-shadow);
        }

        .no-results i {
            font-size: 60px;
            color: #e0e0e0;
            margin-bottom: 20px;
        }

        .no-results h4 {
            font-weight: 600;
            margin-bottom: 15px;
            color: var(--text-color);
        }

        .no-results p {
            color: #6c757d;
            margin-bottom: 20px;
        }

        .gridview-container {
            overflow-x: auto;
            border-radius: var(--border-radius);
        }

        @media (max-width: 768px) {
            .action-buttons {
                flex-direction: column;
                align-items: stretch;
            }
            
            .search-box {
                max-width: 100%;
            }
            
            .filter-box {
                max-width: 100%;
            }
            
            .card-header {
                flex-direction: column;
                gap: 10px;
                align-items: flex-start;
            }
            
            .card-header .btn {
                width: 100%;
            }
            
            .table th, .table td {
                padding: 10px;
            }
            
            .action-buttons-cell {
                flex-wrap: wrap;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="container">
            <ul class="breadcrumb">
                <li class="breadcrumb-item"><a href="~/AdminSide/AdminPanel/Dashboard.aspx" runat="server"><i class="fas fa-home"></i> Dashboard</a></li>
                <li class="breadcrumb-separator">/</li>
                <li class="breadcrumb-item"><a href="~/AdminSide/AdminPanel/ManagePosts.aspx" runat="server">Manage Posts</a></li>
                <li class="breadcrumb-separator">/</li>
                <li class="breadcrumb-active">Archive Posts</li>
            </ul>
            
            <h1 class="page-title"><i class="fas fa-archive"></i> Archive Posts</h1>
            
            <div class="card">
                <div class="card-header">
                    <h5><i class="fas fa-box-archive"></i> Archived Content</h5>
                    <asp:Button ID="btnReturnToManage" runat="server" Text="Return to Manage Posts" CssClass="btn btn-primary" OnClick="btnReturnToManage_Click" />
                </div>
                <div class="card-body">
                    <div class="action-buttons">
                        <div class="search-box">
                            <label for="txtSearch" style="position: absolute; font-size: 12px; margin-top: -18px; color: #6c757d;">Search archived posts</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" ToolTip="Search by title or content..."></asp:TextBox>
                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                        </div>
                        <div class="filter-box">
                            <asp:DropDownList ID="ddlArchiveDate" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlArchiveDate_SelectedIndexChanged">
                                <asp:ListItem Text="All Time" Value=""></asp:ListItem>
                                <asp:ListItem Text="Last 30 Days" Value="30"></asp:ListItem>
                                <asp:ListItem Text="Last 90 Days" Value="90"></asp:ListItem>
                                <asp:ListItem Text="Last 365 Days" Value="365"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="gridview-container">
                        <asp:GridView ID="gvArchivedPosts" runat="server" AutoGenerateColumns="False" 
                            CssClass="table" 
                            OnRowCommand="gvArchivedPosts_RowCommand"
                            DataKeyNames="PostID" EmptyDataText="No archived posts found">
                            <HeaderStyle CssClass="text-center" />
                            <RowStyle CssClass="text-center" />
                            <Columns>
                                <asp:BoundField DataField="PostID" HeaderText="ID" ReadOnly="True" Visible="false" />
                                <asp:TemplateField HeaderText="Title">
                                    <ItemTemplate>
                                        <div class="post-title"><%# Eval("Title") %></div>
                                        <div class="post-excerpt"><%# Eval("Excerpt") %></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Author" HeaderText="Author" />
                                <asp:BoundField DataField="Category" HeaderText="Category" />
                                <asp:TemplateField HeaderText="Archive Date">
                                    <ItemTemplate>
                                        <span class="archive-date">
                                            <%# Convert.ToDateTime(Eval("ArchivedDate")).ToString("MMM dd, yyyy") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Views" HeaderText="Views" />
                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <div class="action-buttons-cell">
                                            <asp:LinkButton ID="btnView" runat="server" CssClass="btn btn-sm btn-primary" 
                                                CommandName="ViewPost" CommandArgument='<%# Eval("PostID") %>' 
                                                ToolTip="View Post">
                                                <i class="fas fa-eye"></i>
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton ID="btnRestore" runat="server" CssClass="btn btn-sm btn-success" 
                                                CommandName="RestorePost" CommandArgument='<%# Eval("PostID") %>' 
                                                ToolTip="Restore Post">
                                                <i class="fas fa-undo"></i>
                                            </asp:LinkButton>
                                            
                                            <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-sm btn-danger" 
                                                CommandName="DeletePost" CommandArgument='<%# Eval("PostID") %>' 
                                                ToolTip="Delete Permanently"
                                                OnClientClick="return confirm('Are you sure you want to permanently delete this post? This action cannot be undone.');">
                                                <i class="fas fa-trash"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        
                        <asp:Panel ID="pnlNoResults" runat="server" CssClass="no-results" Visible="false">
                            <i class="fas fa-archive"></i>
                            <h4>No Archived Posts Found</h4>
                            <p>There are no archived posts matching your search criteria.</p>
                            <asp:Button ID="btnBackToManage" runat="server" Text="Return to Manage Posts" CssClass="btn btn-primary" OnClick="btnReturnToManage_Click" />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
</body>
</html> 