<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="PreviewPost.aspx.vb" Inherits="BSIT.PreviewPost" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Post Preview - BSIT Blog</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../../AdminSide/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        :root {
            --primary-color: #800000;
            --secondary-color: #600000;
            --accent-color: #A52A2A;
            --text-color: #333333;
            --border-radius: 8px;
            --card-shadow: 0 4px 6px rgba(0, 0, 0, 0.1), 0 1px 3px rgba(0, 0, 0, 0.08);
        }

        body {
            background-color: #f5f5f5;
            font-family: 'Poppins', sans-serif;
            margin: 0;
            padding: 0;
            color: var(--text-color);
            line-height: 1.6;
        }

        .container {
            max-width: 1000px;
            padding: 30px 15px;
            margin: 0 auto;
        }

        .preview-header {
            background-color: var(--primary-color);
            color: white;
            padding: 15px 20px;
            margin-bottom: 30px;
            border-radius: var(--border-radius);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .preview-header h2 {
            margin: 0;
            font-size: 18px;
            font-weight: 600;
        }

        .preview-notice {
            display: inline-block;
            background-color: rgba(255, 255, 255, 0.2);
            padding: 5px 10px;
            border-radius: 4px;
            font-size: 14px;
            margin-left: 10px;
        }

        .btn {
            padding: 10px 15px;
            border-radius: var(--border-radius);
            font-weight: 500;
            font-size: 14px;
            letter-spacing: 0.5px;
            transition: all 0.3s;
            border: none;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 5px;
            cursor: pointer;
            text-decoration: none;
        }

        .btn-light {
            background-color: white;
            color: var(--primary-color);
        }

        .btn-light:hover {
            background-color: #f8f9fa;
            transform: translateY(-2px);
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .post-card {
            background-color: white;
            border-radius: var(--border-radius);
            box-shadow: var(--card-shadow);
            overflow: hidden;
            margin-bottom: 30px;
        }

        .post-header {
            padding: 30px 30px 20px;
        }

        .post-title {
            font-size: 32px;
            font-weight: 700;
            margin: 0 0 15px;
            color: var(--primary-color);
        }

        .post-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            font-size: 14px;
            color: #666;
            margin-bottom: 20px;
        }

        .post-meta-item {
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .post-image {
            width: 100%;
            max-height: 500px;
            object-fit: cover;
        }

        .post-content {
            padding: 30px;
            font-size: 16px;
        }

        .post-excerpt {
            font-size: 18px;
            font-weight: 500;
            line-height: 1.6;
            margin-bottom: 20px;
            padding: 15px;
            background-color: #f9f9f9;
            border-left: 4px solid var(--primary-color);
            border-radius: 0 var(--border-radius) var(--border-radius) 0;
        }

        /* Content styling */
        .post-content h1, .post-content h2, .post-content h3,
        .post-content h4, .post-content h5, .post-content h6 {
            color: var(--primary-color);
            margin-top: 30px;
            margin-bottom: 15px;
        }

        .post-content p {
            margin-bottom: 20px;
        }

        .post-content img {
            max-width: 100%;
            height: auto;
            border-radius: var(--border-radius);
            margin: 20px 0;
        }

        .post-content a {
            color: var(--accent-color);
            text-decoration: none;
        }

        .post-content a:hover {
            text-decoration: underline;
        }

        .post-content blockquote {
            border-left: 4px solid var(--primary-color);
            padding: 15px;
            margin: 20px 0;
            background-color: #f9f9f9;
            font-style: italic;
            border-radius: 0 var(--border-radius) var(--border-radius) 0;
        }

        .post-content ul, .post-content ol {
            margin-bottom: 20px;
            padding-left: 20px;
        }

        .post-content li {
            margin-bottom: 10px;
        }

        .post-content table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }

        .post-content th, .post-content td {
            padding: 12px 15px;
            border: 1px solid #ddd;
        }

        .post-content th {
            background-color: #f2f2f2;
            font-weight: 600;
        }

        .post-content tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .preview-footer {
            margin-top: 30px;
            display: flex;
            justify-content: center;
        }

        @media (max-width: 768px) {
            .post-title {
                font-size: 24px;
            }
            
            .post-meta {
                flex-direction: column;
                gap: 8px;
            }
            
            .post-header, .post-content {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preview-header">
            <div>
                <h2>Post Preview <span class="preview-notice">This is a preview only</span></h2>
            </div>
            <a href="CreatePosts.aspx" class="btn btn-light">
                <i class="fas fa-arrow-left"></i> Back to Editor
            </a>
        </div>

        <div class="container">
            <div class="post-card">
                <div class="post-header">
                    <h1 class="post-title">
                        <asp:Literal ID="litTitle" runat="server"></asp:Literal>
                    </h1>
                    <div class="post-meta">
                        <div class="post-meta-item">
                            <i class="far fa-user"></i>
                            <span>Current User</span>
                        </div>
                        <div class="post-meta-item">
                            <i class="far fa-calendar"></i>
                            <span><%= DateTime.Now.ToString("MMMM dd, yyyy") %></span>
                        </div>
                        <div class="post-meta-item">
                            <i class="far fa-folder"></i>
                            <span>Uncategorized</span>
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlFeaturedImage" runat="server" Visible="false">
                    <img id="imgFeatured" runat="server" class="post-image" alt="Featured Image" />
                </asp:Panel>

                <div class="post-content">
                    <asp:Panel ID="pnlExcerpt" runat="server" Visible="false">
                        <div class="post-excerpt">
                            <asp:Literal ID="litExcerpt" runat="server"></asp:Literal>
                        </div>
                    </asp:Panel>

                    <asp:Literal ID="litContent" runat="server"></asp:Literal>
                </div>
            </div>

            <div class="preview-footer">
                <a href="CreatePosts.aspx" class="btn btn-light">
                    <i class="fas fa-arrow-left"></i> Back to Editor
                </a>
            </div>
        </div>
    </form>
</body>
</html> 