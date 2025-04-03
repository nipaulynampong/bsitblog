<%@ Page Language="vb" AutoEventWireup="true" CodeFile="EditPost.aspx.vb" Inherits="BSIT.EditPost" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Post</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <script src="https://cdn.tiny.cloud/1/no-api-key/tinymce/5/tinymce.min.js" referrerpolicy="origin"></script>
    <script>
        tinymce.init({
            selector: '#<%=txtContent.ClientID%>',
            height: 500,
            plugins: 'print preview paste importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists wordcount imagetools textpattern noneditable help charmap quickbars emoticons',
            menubar: 'file edit view insert format tools table help',
            toolbar: 'undo redo | bold italic underline strikethrough | fontselect fontsizeselect formatselect | alignleft aligncenter alignright alignjustify | outdent indent |  numlist bullist | forecolor backcolor removeformat | pagebreak | charmap emoticons | fullscreen  preview save print | insertfile image media template link anchor codesample | ltr rtl',
            toolbar_sticky: true,
            image_advtab: true,
            importcss_append: true,
            branding: false,
            quickbars_selection_toolbar: 'bold italic | quicklink h2 h3 blockquote quickimage quicktable',
            entity_encoding: 'raw',
            content_style: 'body { font-family:Poppins,Arial,sans-serif; font-size:16px; }'
        });
    </script>
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
            display: flex;
            align-items: center;
            gap: 10px;
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
            background-color: var(--success-color);
            border-color: var(--success-color);
            color: white;
        }

        .btn-success:hover {
            background-color: #3db8dc;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(76, 201, 240, 0.3);
        }

        .btn-danger {
            background-color: var(--danger-color);
            border-color: var(--danger-color);
            color: white;
        }

        .btn-danger:hover {
            background-color: #d92b38;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(230, 57, 70, 0.3);
        }

        .btn-secondary {
            background-color: #6c757d;
            border-color: #6c757d;
            color: white;
        }

        .btn-secondary:hover {
            background-color: #5a6268;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(108, 117, 125, 0.3);
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            font-weight: 500;
            margin-bottom: 8px;
            display: block;
            color: var(--text-color);
        }

        .form-control {
            display: block;
            width: 100%;
            padding: 12px 15px;
            font-size: 14px;
            font-weight: 400;
            line-height: 1.5;
            color: #495057;
            background-color: #fff;
            background-clip: padding-box;
            border: 1px solid #ced4da;
            border-radius: var(--border-radius);
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }

        .form-control:focus {
            border-color: var(--primary-color);
            outline: 0;
            box-shadow: 0 0 0 0.2rem rgba(67, 97, 238, 0.25);
        }

        .tox-tinymce {
            border-radius: var(--border-radius) !important;
            border: 1px solid #ced4da !important;
        }

        .form-select {
            display: block;
            width: 100%;
            padding: 12px 15px;
            font-size: 14px;
            font-weight: 400;
            line-height: 1.5;
            color: #495057;
            background-color: #fff;
            border: 1px solid #ced4da;
            border-radius: var(--border-radius);
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }

        .form-select:focus {
            border-color: var(--primary-color);
            outline: 0;
            box-shadow: 0 0 0 0.2rem rgba(67, 97, 238, 0.25);
        }

        .actions-row {
            display: flex;
            justify-content: space-between;
            margin-top: 30px;
            flex-wrap: wrap;
            gap: 10px;
        }

        .actions-left {
            display: flex;
            gap: 10px;
        }

        .actions-right {
            display: flex;
            gap: 10px;
        }

        .alert {
            padding: 15px;
            margin-bottom: 20px;
            border: 1px solid transparent;
            border-radius: var(--border-radius);
        }

        .alert-success {
            color: #155724;
            background-color: #d4edda;
            border-color: #c3e6cb;
        }

        .alert-danger {
            color: #721c24;
            background-color: #f8d7da;
            border-color: #f5c6cb;
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

        @media (max-width: 768px) {
            .actions-row {
                flex-direction: column;
            }
            
            .actions-left, .actions-right {
                flex-direction: column;
                width: 100%;
            }

            .btn {
                width: 100%;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <ul class="breadcrumb">
                <li class="breadcrumb-item"><a href="~/AdminSide/AdminPanel/Dashboard.aspx" runat="server"><i class="fas fa-home"></i> Dashboard</a></li>
                <li class="breadcrumb-separator">/</li>
                <li class="breadcrumb-item"><a href="~/AdminSide/AdminPanel/ManagePosts.aspx" runat="server">Manage Posts</a></li>
                <li class="breadcrumb-separator">/</li>
                <li class="breadcrumb-active">Edit Post</li>
            </ul>
            
            <h1 class="page-title"><i class="fas fa-edit"></i> Edit Post</h1>

            <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-success">
                <asp:Literal ID="litAlertMessage" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="card">
                <div class="card-header">
                    <h5><i class="fas fa-file-alt"></i> Post Details</h5>
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <asp:Label ID="lblTitle" runat="server" CssClass="form-label" AssociatedControlID="txtTitle">Title</asp:Label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Enter post title"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle" 
                            ErrorMessage="Title is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblCategory" runat="server" CssClass="form-label" AssociatedControlID="ddlCategory">Category</asp:Label>
                        <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                            <asp:ListItem Text="-- Select Category --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory" 
                            ErrorMessage="Category is required" CssClass="text-danger" Display="Dynamic" InitialValue=""></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblExcerpt" runat="server" CssClass="form-label" AssociatedControlID="txtExcerpt">Excerpt</asp:Label>
                        <asp:TextBox ID="txtExcerpt" runat="server" CssClass="form-control" placeholder="Enter a short excerpt (summary)" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblContent" runat="server" CssClass="form-label" AssociatedControlID="txtContent">Content</asp:Label>
                        <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Rows="15" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvContent" runat="server" ControlToValidate="txtContent" 
                            ErrorMessage="Content is required" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblTags" runat="server" CssClass="form-label" AssociatedControlID="txtTags">Tags</asp:Label>
                        <asp:TextBox ID="txtTags" runat="server" CssClass="form-control" placeholder="Enter tags separated by commas"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblStatus" runat="server" CssClass="form-label" AssociatedControlID="ddlStatus">Status</asp:Label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Draft" Value="Draft"></asp:ListItem>
                            <asp:ListItem Text="Published" Value="Published"></asp:ListItem>
                            <asp:ListItem Text="Pending Approval" Value="Pending"></asp:ListItem>
                            <asp:ListItem Text="Archived" Value="Archived"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="actions-row">
                        <div class="actions-left">
                            <asp:Button ID="btnSaveAsDraft" runat="server" Text="Save as Draft" CssClass="btn btn-secondary" OnClick="btnSaveAsDraft_Click" CausesValidation="false" />
                            <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="btn btn-info" OnClick="btnPreview_Click" CausesValidation="false" />
                        </div>
                        <div class="actions-right">
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" CausesValidation="false" />
                            <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:Button ID="btnPublish" runat="server" Text="Publish" CssClass="btn btn-success" OnClick="btnPublish_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html> 