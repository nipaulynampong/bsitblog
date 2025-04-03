<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CreatePosts.aspx.vb" Inherits="BSIT.CreatePosts" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Create Post - BSIT Blog</title>
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
            background-color: var(--white);
            padding: 20px;
        }

        .post-editor {
            background-color: var(--white);
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .form-group {
            margin-bottom: 20px;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: var(--maroon-primary);
            font-weight: 500;
        }

        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
            transition: border-color 0.3s ease;
        }

        .form-control:focus {
            border-color: var(--maroon-primary);
            outline: none;
        }

        .editor-toolbar {
            background-color: var(--gray-light);
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 10px;
        }

        .editor-toolbar button {
            background: none;
            border: none;
            padding: 5px 10px;
            margin-right: 5px;
            cursor: pointer;
            color: var(--maroon-primary);
            transition: color 0.3s ease;
        }

        .editor-toolbar button:hover {
            color: var(--maroon-dark);
        }

        .editor-content {
            min-height: 300px;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 15px;
            margin-bottom: 20px;
        }

        .image-upload {
            border: 2px dashed #ddd;
            padding: 20px;
            text-align: center;
            border-radius: 5px;
            cursor: pointer;
            transition: border-color 0.3s ease;
        }

        .image-upload:hover {
            border-color: var(--maroon-primary);
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
            background-color: var(--maroon-dark);
        }

        .btn-secondary {
            background-color: var(--gray-light);
            color: var(--gray-dark);
        }

        .btn-secondary:hover {
            background-color: #e0e0e0;
        }

        .button-group {
            display: flex;
            gap: 10px;
            justify-content: flex-end;
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
</head>
<body>
    <form id="form1" runat="server">
    <div class="post-editor">
        <div class="form-group">
            <label for="txtTitle">Post Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Enter post title"></asp:TextBox>
        </div>

        <div class="form-group">
            <label for="ddlCategory">Category</label>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                <asp:ListItem Text="Technology" Value="Technology" />
                <asp:ListItem Text="Education" Value="Education" />
                <asp:ListItem Text="Lifestyle" Value="Lifestyle" />
                <asp:ListItem Text="Other" Value="Other" />
            </asp:DropDownList>
        </div>

        <div class="form-group">
            <label>Featured Image</label>
            <div class="image-upload">
                <i class="fas fa-cloud-upload-alt" style="font-size: 2em; color: var(--maroon-primary); margin-bottom: 10px;"></i>
                <p>Drag and drop an image here or click to browse</p>
                <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="form-group">
            <label>Content</label>
            <div class="editor-toolbar">
                <button type="button"><i class="fas fa-bold"></i></button>
                <button type="button"><i class="fas fa-italic"></i></button>
                <button type="button"><i class="fas fa-underline"></i></button>
                <button type="button"><i class="fas fa-list"></i></button>
                <button type="button"><i class="fas fa-link"></i></button>
            </div>
            <asp:TextBox ID="txtContent" runat="server" CssClass="editor-content" TextMode="MultiLine"></asp:TextBox>
        </div>

        <div class="button-group">
            <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft" CssClass="btn btn-secondary" />
            <asp:Button ID="btnPublish" runat="server" Text="Publish" CssClass="btn btn-primary" />
        </div>
    </div>
    </form>
</body>
</html>
