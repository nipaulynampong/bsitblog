<%@ Page Language="vb" AutoEventWireup="true" CodeFile="ViewPosts.aspx.vb" Inherits="BSIT.ViewPosts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Posts</title>
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

        .post-card {
            display: flex;
            flex-direction: column;
            border-radius: var(--border-radius);
            box-shadow: var(--card-shadow);
            overflow: hidden;
            margin-bottom: 25px;
            background-color: white;
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            height: 550px;
        }

        .post-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 15px 30px rgba(0, 0, 0, 0.1);
        }

        .post-image {
            width: 100%;
            height: 220px;
            overflow: hidden;
            position: relative;
            background-color: #f0f2f5;
        }

        .post-image img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            transition: transform 0.3s ease;
        }

        .post-card:hover .post-image img {
            transform: scale(1.05);
        }

        .post-category {
            position: absolute;
            top: 15px;
            left: 15px;
            background-color: var(--primary-color);
            color: white;
            font-size: 12px;
            font-weight: 500;
            padding: 5px 10px;
            border-radius: 20px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }

        .post-date {
            position: absolute;
            top: 15px;
            right: 15px;
            background-color: rgba(255, 255, 255, 0.9);
            color: var(--text-color);
            font-size: 12px;
            font-weight: 500;
            padding: 5px 10px;
            border-radius: 20px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
        }

        .post-content {
            padding: 20px;
            display: flex;
            flex-direction: column;
            flex-grow: 1;
            overflow: hidden;
        }

        .post-title {
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 10px;
            line-height: 1.4;
            color: var(--dark-color);
            height: 50px;
            overflow: hidden;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
        }

        .post-excerpt {
            margin-bottom: 15px;
            color: #6c757d;
            font-size: 14px;
            line-height: 1.6;
            flex-grow: 1;
            height: 150px;
            overflow: hidden;
            display: -webkit-box;
            -webkit-line-clamp: 6;
            -webkit-box-orient: vertical;
        }

        .post-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-top: 15px;
            border-top: 1px solid #e0e0e0;
            font-size: 13px;
            color: #6c757d;
        }

        .post-author {
            display: flex;
            align-items: center;
        }

        .post-author-avatar {
            width: 30px;
            height: 30px;
            border-radius: 50%;
            overflow: hidden;
            margin-right: 10px;
        }

        .post-author-avatar img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .post-stats {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .post-stat-item {
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .post-stat-item i {
            color: var(--primary-color);
        }

        .post-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 25px;
        }

        .pagination {
            display: flex;
            justify-content: center;
            margin-top: 40px;
            list-style: none;
            padding: 0;
        }

        .pagination-item {
            margin: 0 5px;
        }

        .pagination-link {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background-color: white;
            color: var(--text-color);
            text-decoration: none;
            box-shadow: var(--card-shadow);
            transition: all 0.3s ease;
        }

        .pagination-link:hover, .pagination-link.active {
            background-color: var(--primary-color);
            color: white;
        }

        .pagination-next, .pagination-prev {
            width: auto;
            padding: 0 15px;
            border-radius: 20px;
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
            
            .post-grid {
                grid-template-columns: 1fr;
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
                <li class="breadcrumb-active">View Posts</li>
            </ul>
            
            <h1 class="page-title"><i class="fas fa-newspaper"></i> Published Posts</h1>
            
            <div class="action-buttons">
                <div class="search-box">
                    <label for="txtSearch" style="position: absolute; font-size: 12px; margin-top: -18px; color: #6c757d;">Search posts</label>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" ToolTip="Search by title or content..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                </div>
                <div class="filter-box">
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                        <asp:ListItem Text="All Categories" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnReturnToManage" runat="server" Text="Back to Manage Posts" CssClass="btn btn-primary" OnClick="btnReturnToManage_Click" />
            </div>
            
            <asp:ListView ID="lvPosts" runat="server" OnItemCommand="lvPosts_ItemCommand">
                <LayoutTemplate>
                    <div class="post-grid">
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                    </div>
                    <div class="pagination">
                        <asp:DataPager ID="dpPosts" runat="server" PageSize="9">
                            <Fields>
                                <asp:NextPreviousPagerField ButtonCssClass="pagination-link pagination-prev" 
                                    ButtonType="Link" 
                                    ShowFirstPageButton="false" 
                                    ShowNextPageButton="false" 
                                    ShowPreviousPageButton="true" 
                                    PreviousPageText="<i class='fas fa-chevron-left'></i> Previous" />
                                    
                                <asp:NumericPagerField ButtonCount="5" 
                                    CurrentPageLabelCssClass="pagination-link active" 
                                    NumericButtonCssClass="pagination-link" />
                                    
                                <asp:NextPreviousPagerField ButtonCssClass="pagination-link pagination-next" 
                                    ButtonType="Link" 
                                    ShowLastPageButton="false" 
                                    ShowNextPageButton="true" 
                                    ShowPreviousPageButton="false" 
                                    NextPageText="Next <i class='fas fa-chevron-right'></i>" />
                            </Fields>
                        </asp:DataPager>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="post-card">
                        <div class="post-image">
                            <asp:Image ID="imgPost" runat="server" ImageUrl='<%# GetPostImageUrl(Eval("PostID")) %>' 
                                AlternateText='<%# Eval("Title") %>' 
                                onerror="this.onerror=null;this.src='../Content/images/post-placeholder.jpg';" />
                            <div class="post-category"><%# Eval("Category") %></div>
                            <div class="post-date"><%# Convert.ToDateTime(Eval("CreatedDate")).ToString("MMM dd, yyyy") %></div>
                        </div>
                        <div class="post-content">
                            <h3 class="post-title" title='<%# Eval("Title") %>'><%# Eval("Title") %></h3>
                            <div class="post-excerpt" title='<%# Eval("Excerpt") %>'><%# Eval("Excerpt") %></div>
                            <div class="post-footer">
                                <div class="post-author">
                                    <div class="post-author-avatar">
                                        <img src="../Content/images/avatar-placeholder.jpg" alt="Author" />
                                    </div>
                                    <span><%# Eval("Author") %></span>
                                </div>
                                <div class="post-stats">
                                    <div class="post-stat-item">
                                        <i class="fas fa-eye"></i>
                                        <span><%# Eval("Views") %></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <asp:LinkButton ID="btnView" runat="server" CssClass="btn btn-primary" 
                            CommandName="ViewPost" CommandArgument='<%# Eval("PostID") %>'
                            Style="margin: 0 20px 20px 20px;" Text="View Details"></asp:LinkButton>
                    </div>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="no-results">
                        <i class="fas fa-search"></i>
                        <h4>No Posts Found</h4>
                        <p>There are no published posts matching your search criteria.</p>
                        <asp:Button ID="btnBackToManage" runat="server" Text="Return to Manage Posts" CssClass="btn btn-primary" OnClick="btnReturnToManage_Click" />
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
    </form>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
</body>
</html> 