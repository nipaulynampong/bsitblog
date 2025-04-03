<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="Settings.aspx.vb" Inherits="BSIT.BSIT.Settings" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Blog Settings</title>
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
            max-width: 1200px;
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
            font-weight: 600;
            margin-bottom: 8px;
            display: block;
        }
        .form-control-lg {
            font-size: 16px;
            padding: 12px 15px;
            height: auto;
        }
        .btn-primary {
            background-color: #3498db;
            border-color: #3498db;
            padding: 10px 20px;
            font-size: 16px;
            font-weight: 600;
        }
        .btn-primary:hover {
            background-color: #2980b9;
            border-color: #2980b9;
        }
        .btn-danger {
            background-color: #e74c3c;
            border-color: #e74c3c;
            padding: 10px 20px;
            font-size: 16px;
            font-weight: 600;
        }
        .btn-danger:hover {
            background-color: #c0392b;
            border-color: #c0392b;
        }
        .help-text {
            font-size: 14px;
            color: #7f8c8d;
            margin-top: 5px;
        }
        .category-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 15px;
            border-bottom: 1px solid #eee;
        }
        .category-item:last-child {
            border-bottom: none;
        }
        .category-name {
            font-weight: 500;
        }
        .category-actions {
            display: flex;
            gap: 10px;
        }
        .backup-history {
            max-height: 300px;
            overflow-y: auto;
        }
        .success-message {
            background-color: #d4edda;
            color: #155724;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
            display: none;
        }
        .error-message {
            background-color: #f8d7da;
            color: #721c24;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
            display: none;
        }
        .nav-tabs {
            border-bottom: 2px solid #eee;
        }
        .nav-tabs .nav-link {
            border: none;
            color: #7f8c8d;
            font-weight: 600;
            padding: 15px 20px;
        }
        .nav-tabs .nav-link.active {
            color: #3498db;
            border-bottom: 3px solid #3498db;
        }
        .tab-content {
            padding-top: 30px;
        }
        .checkbox-list {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
            gap: 10px;
            margin-top: 15px;
        }
        .custom-checkbox {
            display: flex;
            align-items: center;
            margin-bottom: 10px;
        }
        .custom-checkbox label {
            margin-left: 10px;
            font-weight: 400;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1 class="mb-4">Blog Settings</h1>
            
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-message" Visible="false">
                <i class="fas fa-check-circle mr-2"></i>
                <asp:Literal ID="litSuccessMessage" runat="server"></asp:Literal>
            </asp:Panel>
            
            <asp:Panel ID="pnlError" runat="server" CssClass="error-message" Visible="false">
                <i class="fas fa-exclamation-circle mr-2"></i>
                <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
            </asp:Panel>
            
            <ul class="nav nav-tabs" id="settingsTabs" role="tablist">
                <li class="nav-item">
                    <a class="nav-link active" id="general-tab" data-toggle="tab" href="#general" role="tab">General</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="categories-tab" data-toggle="tab" href="#categories" role="tab">Categories</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="users-tab" data-toggle="tab" href="#users" role="tab">Users</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="comments-tab" data-toggle="tab" href="#comments" role="tab">Comments</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="backup-tab" data-toggle="tab" href="#backup" role="tab">Backup</a>
                </li>
            </ul>
            
            <div class="tab-content" id="settingsTabContent">
                <!-- General Settings -->
                <div class="tab-pane fade show active" id="general" role="tabpanel">
                    <div class="card">
                        <div class="card-header">Blog Information</div>
                        <div class="card-body">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtBlogTitle">Blog Title</asp:Label>
                                <asp:TextBox ID="txtBlogTitle" runat="server" CssClass="form-control form-control-lg" MaxLength="100"></asp:TextBox>
                                <p class="help-text">This appears in the header of your blog and in browser tabs.</p>
                            </div>
                            
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtBlogDescription">Blog Description</asp:Label>
                                <asp:TextBox ID="txtBlogDescription" runat="server" CssClass="form-control form-control-lg" TextMode="MultiLine" Rows="3" MaxLength="500"></asp:TextBox>
                                <p class="help-text">A short description of your blog for SEO and social sharing.</p>
                            </div>
                            
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtAdminEmail">Admin Email</asp:Label>
                                <asp:TextBox ID="txtAdminEmail" runat="server" CssClass="form-control form-control-lg" TextMode="Email" MaxLength="100"></asp:TextBox>
                                <p class="help-text">Used for system notifications and password resets.</p>
                            </div>
                            
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ddlPostsPerPage">Posts Per Page</asp:Label>
                                <asp:DropDownList ID="ddlPostsPerPage" runat="server" CssClass="form-control form-control-lg">
                                    <asp:ListItem Text="5 posts" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="10 posts" Value="10" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="15 posts" Value="15"></asp:ListItem>
                                    <asp:ListItem Text="20 posts" Value="20"></asp:ListItem>
                                    <asp:ListItem Text="25 posts" Value="25"></asp:ListItem>
                                </asp:DropDownList>
                                <p class="help-text">Number of posts to display per page on the blog homepage.</p>
                            </div>
                            
                            <div class="form-group">
                                <asp:Button ID="btnSaveGeneral" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSaveGeneral_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Categories Management -->
                <div class="tab-pane fade" id="categories" role="tabpanel">
                    <div class="card">
                        <div class="card-header">Manage Categories</div>
                        <div class="card-body">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtNewCategory">Add New Category</asp:Label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtNewCategory" runat="server" CssClass="form-control form-control-lg" MaxLength="50" placeholder="Category name"></asp:TextBox>
                                    <div class="input-group-append">
                                        <asp:Button ID="btnAddCategory" runat="server" Text="Add" CssClass="btn btn-primary" OnClick="btnAddCategory_Click" />
                                    </div>
                                </div>
                            </div>
                            
                            <hr />
                            
                            <h5 class="mb-3">Existing Categories</h5>
                            <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
                                <ItemTemplate>
                                    <div class="category-item">
                                        <span class="category-name"><%# Eval("CategoryName") %></span>
                                        <div class="category-actions">
                                            <asp:LinkButton ID="lnkEditCategory" runat="server" CssClass="btn btn-sm btn-outline-primary" 
                                                CommandName="Edit" CommandArgument='<%# Eval("CategoryID") %>'>
                                                <i class="fas fa-edit"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkDeleteCategory" runat="server" CssClass="btn btn-sm btn-outline-danger" 
                                                CommandName="Delete" CommandArgument='<%# Eval("CategoryID") %>'
                                                OnClientClick="return confirm('Are you sure you want to delete this category? Posts in this category will not be deleted but will no longer be categorized.');">
                                                <i class="fas fa-trash"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="alert alert-info">No categories found. Add your first category above.</div>
                                </EmptyDataTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
                
                <!-- User Management -->
                <div class="tab-pane fade" id="users" role="tabpanel">
                    <div class="card">
                        <div class="card-header">User Settings</div>
                        <div class="card-body">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label">User Registration</asp:Label>
                                <div class="custom-control custom-checkbox">
                                    <asp:CheckBox ID="chkAllowRegistration" runat="server" />
                                    <asp:Label runat="server" AssociatedControlID="chkAllowRegistration">Allow new user registrations</asp:Label>
                                </div>
                                <div class="custom-control custom-checkbox mt-2">
                                    <asp:CheckBox ID="chkEmailVerification" runat="server" />
                                    <asp:Label runat="server" AssociatedControlID="chkEmailVerification">Require email verification for new accounts</asp:Label>
                                </div>
                            </div>
                            
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ddlDefaultUserRole">Default Role for New Users</asp:Label>
                                <asp:DropDownList ID="ddlDefaultUserRole" runat="server" CssClass="form-control form-control-lg">
                                    <asp:ListItem Text="Reader" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Contributor" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="Author" Value="3"></asp:ListItem>
                                </asp:DropDownList>
                                <p class="help-text">Role assigned to newly registered users.</p>
                            </div>
                            
                            <div class="form-group">
                                <asp:Button ID="btnSaveUserSettings" runat="server" Text="Save User Settings" CssClass="btn btn-primary" OnClick="btnSaveUserSettings_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Comment Settings -->
                <div class="tab-pane fade" id="comments" role="tabpanel">
                    <div class="card">
                        <div class="card-header">Comment Settings</div>
                        <div class="card-body">
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label">Comment Options</asp:Label>
                                <div class="custom-control custom-checkbox">
                                    <asp:CheckBox ID="chkAllowComments" runat="server" />
                                    <asp:Label runat="server" AssociatedControlID="chkAllowComments">Allow comments on posts</asp:Label>
                                </div>
                                <div class="custom-control custom-checkbox mt-2">
                                    <asp:CheckBox ID="chkModerateComments" runat="server" />
                                    <asp:Label runat="server" AssociatedControlID="chkModerateComments">Moderate comments before publishing</asp:Label>
                                </div>
                                <div class="custom-control custom-checkbox mt-2">
                                    <asp:CheckBox ID="chkAllowGuestComments" runat="server" />
                                    <asp:Label runat="server" AssociatedControlID="chkAllowGuestComments">Allow guest comments (without registration)</asp:Label>
                                </div>
                            </div>
                            
                            <div class="form-group">
                                <asp:Label runat="server" CssClass="form-label" AssociatedControlID="txtCommentsPerPage">Comments Per Page</asp:Label>
                                <asp:TextBox ID="txtCommentsPerPage" runat="server" CssClass="form-control form-control-lg" TextMode="Number" min="5" max="100"></asp:TextBox>
                                <p class="help-text">Number of comments to show per page on post pages.</p>
                            </div>
                            
                            <div class="form-group">
                                <asp:Button ID="btnSaveCommentSettings" runat="server" Text="Save Comment Settings" CssClass="btn btn-primary" OnClick="btnSaveCommentSettings_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Backup and Restore -->
                <div class="tab-pane fade" id="backup" role="tabpanel">
                    <div class="card">
                        <div class="card-header">Backup and Restore</div>
                        <div class="card-body">
                            <div class="form-group">
                                <h5>Create Backup</h5>
                                <p>Create a backup of your blog database and content.</p>
                                <asp:Button ID="btnCreateBackup" runat="server" Text="Create Backup Now" CssClass="btn btn-primary" OnClick="btnCreateBackup_Click" />
                            </div>
                            
                            <hr />
                            
                            <div class="form-group">
                                <h5>Restore from Backup</h5>
                                <p class="text-danger">Warning: Restoring from a backup will overwrite your current data.</p>
                                <div class="input-group">
                                    <asp:FileUpload ID="fileBackupRestore" runat="server" CssClass="form-control" />
                                    <div class="input-group-append">
                                        <asp:Button ID="btnRestoreBackup" runat="server" Text="Restore" CssClass="btn btn-danger" OnClick="btnRestoreBackup_Click" 
                                            OnClientClick="return confirm('WARNING: Restoring from backup will overwrite ALL current data. This cannot be undone. Are you sure you want to proceed?');" />
                                    </div>
                                </div>
                            </div>
                            
                            <hr />
                            
                            <h5>Backup History</h5>
                            <div class="backup-history">
                                <asp:GridView ID="gvBackupHistory" runat="server" AutoGenerateColumns="False" 
                                    CssClass="table table-striped" OnRowCommand="gvBackupHistory_RowCommand"
                                    EmptyDataText="No backups found">
                                    <Columns>
                                        <asp:BoundField DataField="BackupDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                        <asp:BoundField DataField="BackupSize" HeaderText="Size" />
                                        <asp:BoundField DataField="BackupBy" HeaderText="Created By" />
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDownloadBackup" runat="server" CssClass="btn btn-sm btn-outline-primary"
                                                    CommandName="Download" CommandArgument='<%# Eval("BackupID") %>'>
                                                    <i class="fas fa-download"></i>
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="lnkDeleteBackup" runat="server" CssClass="btn btn-sm btn-outline-danger"
                                                    CommandName="Delete" CommandArgument='<%# Eval("BackupID") %>'
                                                    OnClientClick="return confirm('Are you sure you want to delete this backup?');">
                                                    <i class="fas fa-trash"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    </div>
    </form>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
    <script>
        $(document).ready(function () {
            // Show success message with fade effect if visible
            if ($('.success-message').length && $('.success-message').is(':visible')) {
                $('.success-message').fadeIn().delay(3000).fadeOut();
            }
            
            // Initialize Bootstrap tabs
            $('.nav-tabs a').click(function (e) {
                e.preventDefault();
                $(this).tab('show');
            });
        });
    </script>
</body>
</html>
