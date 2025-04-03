<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Dashboard.aspx.vb" Inherits="BSIT.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>
    <style type="text/css">
        body {
            font-family: Arial, sans-serif;
            background-color: transparent;
            margin: 0;
            padding: 0;
        }
        .stats-container {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 30px;
        }
        .stat-card {
            flex: 1;
            min-width: 220px;
            background-color: #fff;
            border-radius: 5px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            padding: 20px;
            text-align: center;
            transition: all 0.3s;
        }
        .stat-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 5px 15px rgba(0,0,0,0.1);
        }
        .stat-card.users {
            border-top: 4px solid #3498db;
        }
        .stat-card.posts {
            border-top: 4px solid #2ecc71;
        }
        .stat-card.views {
            border-top: 4px solid #f39c12;
        }
        .stat-card.active {
            border-top: 4px solid #e74c3c;
        }
        .stat-icon {
            font-size: 48px;
            margin-bottom: 10px;
        }
        .stat-value {
            font-size: 30px;
            font-weight: bold;
            margin-bottom: 5px;
        }
        .stat-label {
            color: #777;
            font-size: 14px;
        }
        .chart-card {
            flex: 1;
            min-width: 48%;
            background-color: #fff;
            border-radius: 5px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            padding: 20px;
            margin-bottom: 20px;
        }
        .chart-title {
            font-size: 18px;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 1px solid #e0e0e0;
        }
        .gridview-container {
            margin-top: 20px;
            overflow-x: auto;
        }
        .gridview {
            width: 100%;
            border-collapse: collapse;
        }
        .gridview th {
            background-color: #f5f5f5;
            padding: 10px 15px;
            text-align: left;
            font-weight: 600;
            border-bottom: 2px solid #ddd;
        }
        .gridview td {
            padding: 10px 15px;
            border-bottom: 1px solid #ddd;
        }
        .gridview tr:hover {
            background-color: #f9f9f9;
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Stats Cards -->
        <div class="stats-container">
            <div class="stat-card users">
                <div class="stat-icon">
                    <i class="fas fa-users" style="color: #3498db;"></i>
                </div>
                <div class="stat-value">
                    <asp:Label ID="lblTotalUsers" runat="server">0</asp:Label>
                </div>
                <div class="stat-label">Total Users</div>
            </div>
            
            <div class="stat-card posts">
                <div class="stat-icon">
                    <i class="fas fa-newspaper" style="color: #2ecc71;"></i>
                </div>
                <div class="stat-value">
                    <asp:Label ID="lblTotalPosts" runat="server">0</asp:Label>
                </div>
                <div class="stat-label">Total Posts</div>
            </div>
            
            <div class="stat-card views">
                <div class="stat-icon">
                    <i class="fas fa-eye" style="color: #f39c12;"></i>
                </div>
                <div class="stat-value">
                    <asp:Label ID="lblTotalViews" runat="server">0</asp:Label>
                </div>
                <div class="stat-label">Total Views</div>
            </div>
            
            <div class="stat-card active">
                <div class="stat-icon">
                    <i class="fas fa-check-circle" style="color: #e74c3c;"></i>
                </div>
                <div class="stat-value">
                    <asp:Label ID="lblActivePosts" runat="server">0</asp:Label>
                </div>
                <div class="stat-label">Active Posts</div>
            </div>
        </div>
        
        <!-- Top Posts Section -->
        <div class="chart-card">
            <h3 class="chart-title">Most Viewed Posts</h3>
            <div class="gridview-container">
                <asp:GridView ID="GridViewTopPosts" runat="server" AutoGenerateColumns="False" 
                    CssClass="gridview" EmptyDataText="No posts found.">
                    <Columns>
                        <asp:BoundField DataField="PostID" HeaderText="ID" />
                        <asp:BoundField DataField="Title" HeaderText="Title" />
                        <asp:BoundField DataField="fullname" HeaderText="Author" />
                        <asp:BoundField DataField="Views" HeaderText="Views" />
                        <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        
        <!-- Recent Users Section -->
        <div class="chart-card">
            <h3 class="chart-title">Recently Added Users</h3>
            <div class="gridview-container">
                <asp:GridView ID="GridViewRecentUsers" runat="server" AutoGenerateColumns="False" 
                    CssClass="gridview" EmptyDataText="No recent users found.">
                    <Columns>
                        <asp:BoundField DataField="UserID" HeaderText="ID" />
                        <asp:BoundField DataField="username" HeaderText="Username" />
                        <asp:BoundField DataField="fullname" HeaderText="Full Name" />
                        <asp:BoundField DataField="usertype" HeaderText="User Type" />
                        <asp:BoundField DataField="isActive" HeaderText="Active" />
                        <asp:BoundField DataField="createdDate" HeaderText="Created" DataFormatString="{0:MM/dd/yyyy}" />
                    </Columns>
                </asp:GridView>
            </div>
    </div>
    </form>
</body>
</html>
