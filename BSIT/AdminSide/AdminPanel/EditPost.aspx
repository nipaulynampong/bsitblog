<%@ Page Language="vb" AutoEventWireup="true" CodeFile="EditPost.aspx.vb" Inherits="BSIT.EditPost" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edit Post</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script src="https://cdn.tiny.cloud/1/no-api-key/tinymce/5/tinymce.min.js" referrerpolicy="origin"></script>
    <script>
        $(document).ready(function() {
            // Initialize Select2 for tags
            $('#<%= ddlTags.ClientID %>').select2({
                tags: true,
                tokenSeparators: [','],
                placeholder: "Select or add tags...",
                allowClear: true
            });

            // Initialize Flatpickr for date/time picker
            flatpickr('#<%= txtPublishDate.ClientID %>', {
                enableTime: true,
                dateFormat: "Y-m-d H:i",
                time_24hr: false,
                minDate: "today"
            });

            // Show/hide custom category input based on dropdown selection
            $('#<%= ddlCategory.ClientID %>').change(function() {
                if ($(this).val() === 'custom') {
                    $('#custom-category-container').show();
                } else {
                    $('#custom-category-container').hide();
                }
            });
        });

        // Initialize TinyMCE
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
            content_style: 'body { font-family:Poppins,Arial,sans-serif; font-size:16px; }',
            // Setup for image uploads
            images_upload_handler: function (blobInfo, success, failure) {
                var xhr, formData;
                xhr = new XMLHttpRequest();
                xhr.withCredentials = false;
                xhr.open('POST', 'UploadImage.ashx');
                xhr.onload = function() {
                    var json;
                    if (xhr.status != 200) {
                        failure('HTTP Error: ' + xhr.status);
                        return;
                    }
                    json = JSON.parse(xhr.responseText);
                    if (!json || typeof json.location != 'string') {
                        failure('Invalid JSON: ' + xhr.responseText);
                        return;
                    }
                    success(json.location);
                };
                formData = new FormData();
                formData.append('file', blobInfo.blob(), blobInfo.filename());
                xhr.send(formData);
            }
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
            background-color: #d1e7dd;
            border-color: #badbcc;
            color: #0f5132;
        }

        .alert-danger {
            background-color: #f8d7da;
            border-color: #f5c2c7;
            color: #842029;
        }

        .select2-container {
            width: 100% !important;
        }

        .select2-container--default .select2-selection--multiple {
            border-radius: var(--border-radius);
            border: 1px solid #ced4da;
            min-height: 40px;
        }

        .select2-container--default .select2-selection--multiple .select2-selection__choice {
            background-color: var(--primary-color);
            color: white;
            border: none;
            border-radius: 4px;
            padding: 5px 10px;
            margin-top: 5px;
        }

        .select2-container--default .select2-selection--multiple .select2-selection__choice__remove {
            color: white;
            margin-right: 5px;
        }

        #custom-category-container {
            display: none;
            margin-top: 15px;
            padding: 15px;
            background-color: #f8f9fa;
            border-radius: var(--border-radius);
            border: 1px solid #ced4da;
        }

        .toggle-container {
            display: flex;
            align-items: center;
            margin-bottom: 20px;
        }

        .toggle-switch {
            position: relative;
            display: inline-block;
            width: 60px;
            height: 34px;
            margin-right: 10px;
        }

        .toggle-switch input {
            opacity: 0;
            width: 0;
            height: 0;
        }

        .toggle-slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            transition: .4s;
            border-radius: 34px;
        }

        .toggle-slider:before {
            position: absolute;
            content: "";
            height: 26px;
            width: 26px;
            left: 4px;
            bottom: 4px;
            background-color: white;
            transition: .4s;
            border-radius: 50%;
        }

        input:checked + .toggle-slider {
            background-color: var(--success-color);
        }

        input:focus + .toggle-slider {
            box-shadow: 0 0 1px var(--success-color);
        }

        input:checked + .toggle-slider:before {
            transform: translateX(26px);
        }

        .toggle-label {
            font-weight: 500;
        }

        .tabs {
            display: flex;
            margin-bottom: 20px;
            border-bottom: 1px solid #dee2e6;
        }

        .tab {
            padding: 10px 20px;
            cursor: pointer;
            border-bottom: 2px solid transparent;
            margin-right: 5px;
            font-weight: 500;
        }

        .tab.active {
            border-bottom: 2px solid var(--primary-color);
            color: var(--primary-color);
        }

        .tab-content {
            display: none;
        }

        .tab-content.active {
            display: block;
        }

        @media (max-width: 768px) {
            .actions-row {
                flex-direction: column;
            }

            .actions-left, .actions-right {
                width: 100%;
                flex-direction: column;
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
            <h1 class="page-title">
                <i class="fas fa-edit"></i> Edit Post
            </h1>

            <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert">
                <asp:Literal ID="litAlertMessage" runat="server" />
            </asp:Panel>

            <div class="card">
                <div class="card-header">
                    <h5><i class="fas fa-file-alt"></i> Post Details</h5>
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <asp:Label ID="lblTitle" runat="server" CssClass="form-label" AssociatedControlID="txtTitle">Title</asp:Label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Enter post title" required="required" />
                        <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                            ErrorMessage="Title is required" Display="Dynamic" CssClass="text-danger" />
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblSlug" runat="server" CssClass="form-label" AssociatedControlID="txtSlug">Slug (URL)</asp:Label>
                        <asp:TextBox ID="txtSlug" runat="server" CssClass="form-control" placeholder="enter-post-slug" />
                        <small class="text-muted">Leave empty to auto-generate from title</small>
                    </div>

                    <div class="tabs">
                        <div class="tab active" data-target="editor-tab">Content</div>
                        <div class="tab" data-target="settings-tab">Settings</div>
                        <div class="tab" data-target="seo-tab">SEO</div>
                    </div>

                    <div id="editor-tab" class="tab-content active">
                        <div class="form-group">
                            <asp:Label ID="lblExcerpt" runat="server" CssClass="form-label" AssociatedControlID="txtExcerpt">Excerpt</asp:Label>
                            <asp:TextBox ID="txtExcerpt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter a short excerpt for this post" />
                            <small class="text-muted">A short summary of your post (optional)</small>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lblContent" runat="server" CssClass="form-label" AssociatedControlID="txtContent">Content</asp:Label>
                            <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvContent" runat="server" ControlToValidate="txtContent"
                                ErrorMessage="Content is required" Display="Dynamic" CssClass="text-danger" />
                        </div>
                        
                        <div class="form-group">
                            <asp:Label ID="lblFeaturedImage" runat="server" CssClass="form-label" AssociatedControlID="fileImage">Featured Image</asp:Label>
                            <asp:FileUpload ID="fileImage" runat="server" CssClass="form-control" accept="image/*" />
                            <small class="text-muted">Recommended size: 1200x630 pixels</small>
                            
                            <div id="currentImageContainer" runat="server" visible="false" class="mt-3">
                                <p>Current image:</p>
                                <asp:Image ID="imgCurrentImage" runat="server" CssClass="img-fluid img-thumbnail" style="max-height: 200px;" />
                                <div class="mt-2">
                                    <asp:Button ID="btnRemoveImage" runat="server" Text="Remove Image" CssClass="btn btn-sm btn-danger" 
                                        OnClick="btnRemoveImage_Click" CausesValidation="false" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="settings-tab" class="tab-content">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label ID="lblCategory" runat="server" CssClass="form-label" AssociatedControlID="ddlCategory">Category</asp:Label>
                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="-- Select Category --" Value="" />
                                    </asp:DropDownList>
                                </div>
                                
                                <div id="custom-category-container">
                                    <div class="form-group">
                                        <asp:Label ID="lblNewCategory" runat="server" CssClass="form-label" AssociatedControlID="txtNewCategory">New Category Name</asp:Label>
                                        <asp:TextBox ID="txtNewCategory" runat="server" CssClass="form-control" placeholder="Enter new category name" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblParentCategory" runat="server" CssClass="form-label" AssociatedControlID="ddlParentCategory">Parent Category (Optional)</asp:Label>
                                        <asp:DropDownList ID="ddlParentCategory" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="-- None --" Value="" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label ID="lblTags" runat="server" CssClass="form-label" AssociatedControlID="ddlTags">Tags</asp:Label>
                                    <asp:ListBox ID="ddlTags" runat="server" CssClass="form-control" SelectionMode="Multiple"></asp:ListBox>
                                    <small class="text-muted">Select existing tags or type to add new ones</small>
                                </div>
                            </div>
                        </div>
                        
                        <div class="row mt-4">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label ID="lblPublishDate" runat="server" CssClass="form-label" AssociatedControlID="txtPublishDate">Publish Date</asp:Label>
                                    <asp:TextBox ID="txtPublishDate" runat="server" CssClass="form-control" placeholder="Select publish date and time" />
                                    <small class="text-muted">Leave empty to publish immediately when setting status to Published</small>
                                </div>
                            </div>
                            
                            <div class="col-md-6">
                                <div class="form-group">
                                    <asp:Label ID="lblStatus" runat="server" CssClass="form-label" AssociatedControlID="ddlStatus">Status</asp:Label>
                                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="Draft" Value="Draft" />
                                        <asp:ListItem Text="Published" Value="Published" />
                                        <asp:ListItem Text="Pending Review" Value="Pending" />
                                        <asp:ListItem Text="Archived" Value="Archived" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        
                        <div class="toggle-container mt-3">
                            <label class="toggle-switch">
                                <asp:CheckBox ID="chkAllowComments" runat="server" Checked="true" />
                                <span class="toggle-slider"></span>
                            </label>
                            <span class="toggle-label">Allow Comments</span>
                        </div>
                    </div>

                    <div id="seo-tab" class="tab-content">
                        <div class="form-group">
                            <asp:Label ID="lblMetaTitle" runat="server" CssClass="form-label" AssociatedControlID="txtMetaTitle">Meta Title</asp:Label>
                            <asp:TextBox ID="txtMetaTitle" runat="server" CssClass="form-control" placeholder="Enter SEO title (max 60 characters)" MaxLength="60" />
                            <small class="text-muted">Leave empty to use the post title</small>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lblMetaDescription" runat="server" CssClass="form-label" AssociatedControlID="txtMetaDescription">Meta Description</asp:Label>
                            <asp:TextBox ID="txtMetaDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter meta description (max 160 characters)" MaxLength="160" />
                            <small class="text-muted">Brief description for search engines (recommended 50-160 characters)</small>
                        </div>
                    </div>

                    <div class="actions-row">
                        <div class="actions-left">
                            <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSave_Click">
                                <i class="fas fa-save"></i>
                            </asp:Button>
                            <asp:Button ID="btnSaveAsDraft" runat="server" Text="Save as Draft" CssClass="btn btn-secondary" OnClick="btnSaveAsDraft_Click" CausesValidation="false">
                                <i class="fas fa-save"></i>
                            </asp:Button>
                            <asp:Button ID="btnPublish" runat="server" Text="Publish" CssClass="btn btn-success" OnClick="btnPublish_Click">
                                <i class="fas fa-paper-plane"></i>
                            </asp:Button>
                        </div>
                        <div class="actions-right">
                            <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="btn btn-info" OnClick="btnPreview_Click" CausesValidation="false">
                                <i class="fas fa-eye"></i>
                            </asp:Button>
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false">
                                <i class="fas fa-times"></i>
                            </asp:Button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script>
            // Tab functionality
            document.querySelectorAll('.tab').forEach(tab => {
                tab.addEventListener('click', function() {
                    // Remove active class from all tabs
                    document.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
                    // Add active class to clicked tab
                    this.classList.add('active');
                    
                    // Hide all tab content
                    document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));
                    // Show content related to clicked tab
                    document.getElementById(this.dataset.target).classList.add('active');
                });
            });
        </script>
    </form>
</body>
</html> 