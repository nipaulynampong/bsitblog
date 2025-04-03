<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="Reports.aspx.vb" Inherits="BSIT.BSIT.Reports" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Blog Reports</title>
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
        .stats-row {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 30px;
        }
        .stat-card {
            flex: 1;
            min-width: 200px;
            background-color: #f8f9fa;
            border-radius: 8px;
            padding: 20px;
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
        }
        .stat-icon {
            font-size: 32px;
            margin-bottom: 10px;
            border-radius: 50%;
            width: 70px;
            height: 70px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
        }
        .stat-posts {
            background-color: #3498db;
        }
        .stat-views {
            background-color: #2ecc71;
        }
        .stat-comments {
            background-color: #f39c12;
        }
        .stat-users {
            background-color: #9b59b6;
        }
        .stat-value {
            font-size: 24px;
            font-weight: 700;
            margin: 5px 0;
        }
        .stat-label {
            color: #7f8c8d;
            font-size: 16px;
        }
        .filter-row {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 20px;
        }
        .filter-item {
            flex: 1;
            min-width: 150px;
        }
        .chart-container {
            padding: 20px 0;
            margin-bottom: 20px;
            height: 400px;
            border: 1px solid #eee;
            border-radius: 8px;
            background-color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .chart-placeholder {
            text-align: center;
            color: #7f8c8d;
        }
        .top-table {
            margin-top: 30px;
        }
        .table th {
            background-color: #f8f9fa;
            font-weight: 600;
        }
        .table td, .table th {
            vertical-align: middle;
        }
        .post-title {
            max-width: 300px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }
        .post-author {
            color: #7f8c8d;
            font-size: 14px;
        }
        .no-data {
            padding: 60px 20px;
            text-align: center;
            color: #7f8c8d;
        }
        .btn-export {
            background-color: #2c3e50;
            color: white;
        }
        .date-selection {
            display: flex;
            gap: 10px;
            align-items: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1 class="mb-4">Blog Analytics Report</h1>
            
            <!-- Date range filter -->
            <div class="card mb-4">
                <div class="card-header">
                    Report Date Range
                </div>
                <div class="card-body">
                    <div class="filter-row">
                        <div class="filter-item date-selection">
                            <label>From:</label>
                            <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="filter-item date-selection">
                            <label>To:</label>
                            <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="filter-item">
                            <label>&nbsp;</label>
                            <asp:Button ID="btnApplyDates" runat="server" Text="Apply Dates" OnClick="btnApplyDates_Click" CssClass="btn btn-primary form-control" />
                        </div>
                        <div class="filter-item">
                            <label>&nbsp;</label>
                            <asp:DropDownList ID="ddlDateRange" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDateRange_SelectedIndexChanged">
                                <asp:ListItem Text="Last 7 Days" Value="7"></asp:ListItem>
                                <asp:ListItem Text="Last 30 Days" Value="30" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Last 90 Days" Value="90"></asp:ListItem>
                                <asp:ListItem Text="Last Year" Value="365"></asp:ListItem>
                                <asp:ListItem Text="All Time" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Summary statistics -->
            <div class="card mb-4">
                <div class="card-header">
                    Summary Statistics
                </div>
                <div class="card-body">
                    <div class="stats-row">
                        <div class="stat-card">
                            <div class="stat-icon stat-posts">
                                <i class="fas fa-newspaper"></i>
                            </div>
                            <div class="stat-value"><asp:Literal ID="litTotalPosts" runat="server">0</asp:Literal></div>
                            <div class="stat-label">Posts</div>
                        </div>
                        <div class="stat-card">
                            <div class="stat-icon stat-views">
                                <i class="fas fa-eye"></i>
                            </div>
                            <div class="stat-value"><asp:Literal ID="litTotalViews" runat="server">0</asp:Literal></div>
                            <div class="stat-label">Views</div>
                        </div>
                        <div class="stat-card">
                            <div class="stat-icon stat-comments">
                                <i class="fas fa-comment"></i>
                            </div>
                            <div class="stat-value"><asp:Literal ID="litTotalComments" runat="server">0</asp:Literal></div>
                            <div class="stat-label">Comments</div>
                        </div>
                        <div class="stat-card">
                            <div class="stat-icon stat-users">
                                <i class="fas fa-users"></i>
                            </div>
                            <div class="stat-value"><asp:Literal ID="litActiveUsers" runat="server">0</asp:Literal></div>
                            <div class="stat-label">Active Users</div>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Page Views Chart -->
            <div class="card mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <span>Page Views</span>
                    <asp:Button ID="btnExportViews" runat="server" Text="Export CSV" CssClass="btn btn-sm btn-export" OnClick="btnExportViews_Click" />
                </div>
                <div class="card-body">
                    <!-- In a real implementation, this would be a chart -->
                    <div class="chart-container">
                        <div class="chart-placeholder">
                            <i class="fas fa-chart-line fa-3x mb-3"></i>
                            <h5>Page Views Over Time</h5>
                            <p>The chart showing page views trend would appear here.<br />This is a placeholder for visualization.</p>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Top Posts -->
            <div class="card mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <span>Top Performing Posts</span>
                    <asp:Button ID="btnExportPosts" runat="server" Text="Export CSV" CssClass="btn btn-sm btn-export" OnClick="btnExportPosts_Click" />
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvTopPosts" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-striped top-table" 
                        EmptyDataText="No post data available for the selected time period."
                        ShowHeaderWhenEmpty="true">
                        <HeaderStyle CssClass="text-center" />
                        <Columns>
                            <asp:BoundField DataField="PostID" HeaderText="ID" Visible="false" />
                            <asp:TemplateField HeaderText="Post">
                                <ItemTemplate>
                                    <div class="post-title"><%# Eval("Title") %></div>
                                    <div class="post-author">By <%# Eval("Author") %> • <%# Eval("CreatedDate", "{0:MMM dd, yyyy}") %></div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Views" HeaderText="Views" ItemStyle-CssClass="text-center" />
                            <asp:BoundField DataField="Comments" HeaderText="Comments" ItemStyle-CssClass="text-center" />
                            <asp:BoundField DataField="Category" HeaderText="Category" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            
            <!-- Category Distribution -->
            <div class="card mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <span>Post Distribution by Category</span>
                    <asp:Button ID="btnExportCategories" runat="server" Text="Export CSV" CssClass="btn btn-sm btn-export" OnClick="btnExportCategories_Click" />
                </div>
                <div class="card-body">
                    <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="False" 
                        CssClass="table table-striped top-table" 
                        EmptyDataText="No category data available."
                        ShowHeaderWhenEmpty="true">
                        <HeaderStyle CssClass="text-center" />
                        <Columns>
                            <asp:BoundField DataField="CategoryID" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                            <asp:BoundField DataField="PostCount" HeaderText="Posts" ItemStyle-CssClass="text-center" />
                            <asp:BoundField DataField="ViewCount" HeaderText="Total Views" ItemStyle-CssClass="text-center" />
                            <asp:BoundField DataField="AverageViews" HeaderText="Avg. Views" ItemStyle-CssClass="text-center" DataFormatString="{0:F1}" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
    </div>
    </form>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/bootstrap.bundle.min.js"></script>
</body>
</html>
