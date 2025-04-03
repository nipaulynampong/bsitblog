<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EditorDashboard.aspx.vb" Inherits="BSIT.EditorDashboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard - BSIT Blog</title>
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

        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .stat-card {
            background: linear-gradient(135deg, var(--maroon-primary), var(--maroon-light));
            color: var(--white);
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            transition: transform 0.3s ease;
        }

        .stat-card:hover {
            transform: translateY(-5px);
        }

        .stat-icon {
            font-size: 2em;
            margin-bottom: 10px;
        }

        .stat-value {
            font-size: 24px;
            font-weight: 600;
        }

        .stat-label {
            font-size: 14px;
            opacity: 0.9;
        }

        .recent-posts {
            background-color: var(--white);
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .recent-posts h2 {
            color: var(--maroon-primary);
            margin-bottom: 20px;
        }

        .post-list {
            list-style: none;
        }

        .post-item {
            padding: 15px;
            border-bottom: 1px solid #eee;
            transition: background-color 0.3s ease;
        }

        .post-item:hover {
            background-color: var(--gray-light);
        }

        .post-title {
            font-weight: 500;
            color: var(--gray-dark);
            margin-bottom: 5px;
        }

        .post-meta {
            font-size: 12px;
            color: #666;
        }

        .post-status {
            display: inline-block;
            padding: 3px 8px;
            border-radius: 12px;
            font-size: 12px;
            margin-left: 10px;
        }

        .status-published {
            background-color: #4CAF50;
            color: white;
        }

        .status-draft {
            background-color: #FFC107;
            color: black;
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
</head>
<body>
    <form id="form1" runat="server">
    <div class="dashboard-grid">
        <div class="stat-card">
            <div class="stat-icon">
                <i class="fas fa-file-alt"></i>
            </div>
            <div class="stat-value">
                <asp:Label ID="lblTotalPosts" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Total Posts</div>
        </div>
        <div class="stat-card">
            <div class="stat-icon">
                <i class="fas fa-eye"></i>
            </div>
            <div class="stat-value">
                <asp:Label ID="lblTotalViews" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Total Views</div>
        </div>
        <div class="stat-card">
            <div class="stat-icon">
                <i class="fas fa-clock"></i>
            </div>
            <div class="stat-value">
                <asp:Label ID="lblDraftPosts" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Draft Posts</div>
        </div>
    </div>

    <div class="recent-posts">
        <h2>Recent Posts</h2>
        <asp:GridView ID="gvRecentPosts" runat="server" AutoGenerateColumns="False" 
            CssClass="post-list" GridLines="None">
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:MMM dd, yyyy}" />
                <asp:BoundField DataField="Views" HeaderText="Views" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
