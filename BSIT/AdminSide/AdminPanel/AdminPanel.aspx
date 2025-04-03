<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AdminPanel.aspx.vb" Inherits="BSIT.AdminPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Admin Panel - BSIT Blog</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style type="text/css">
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            color: #333;
        }
        .container {
            display: flex;
            min-height: 100vh;
        }
        .sidebar {
            width: 250px;
            background-color: #2c3e50;
            color: #ecf0f1;
            padding: 20px 0;
            position: fixed;
            height: 100%;
            overflow-y: auto;
        }
        .brand {
            padding: 0 20px 20px;
            border-bottom: 1px solid #34495e;
            margin-bottom: 20px;
        }
        .brand h2 {
            font-size: 22px;
            font-weight: 600;
        }
        .brand p {
            font-size: 12px;
            opacity: 0.7;
            margin-top: 5px;
        }
        .nav-menu {
            list-style: none;
        }
        .nav-item {
            margin-bottom: 5px;
        }
        .nav-link {
            display: block;
            padding: 12px 20px;
            color: #ecf0f1;
            text-decoration: none;
            font-size: 16px;
            transition: all 0.3s;
        }
        .nav-link:hover, .nav-link.active {
            background-color: #34495e;
            border-left: 4px solid #3498db;
        }
        .nav-link i {
            margin-right: 10px;
            width: 20px;
            text-align: center;
        }
        .main-content {
            flex: 1;
            margin-left: 250px;
            padding: 20px;
            transition: all 0.3s;
        }
        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px 0;
            margin-bottom: 20px;
            border-bottom: 1px solid #e0e0e0;
        }
        .page-title {
            font-size: 24px;
            font-weight: 600;
        }
        .user-profile {
            display: flex;
            align-items: center;
        }
        .user-info {
            margin-right: 15px;
            text-align: right;
        }
        .user-name {
            font-weight: 600;
        }
        .user-role {
            font-size: 12px;
            color: #777;
        }
        .dropdown {
            position: relative;
            display: inline-block;
        }
        .dropdown-content {
            display: none;
            position: absolute;
            right: 0;
            background-color: #f9f9f9;
            min-width: 160px;
            box-shadow: 0px 8px 16px 0px rgba(0,0,0,0.2);
            z-index: 1;
            border-radius: 4px;
        }
        .dropdown-content a {
            color: black;
            padding: 12px 16px;
            text-decoration: none;
            display: block;
            font-size: 14px;
        }
        .dropdown-content a:hover {
            background-color: #f1f1f1;
        }
        .dropdown:hover .dropdown-content {
            display: block;
        }
        .content-wrapper {
            background-color: #fff;
            border-radius: 5px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            padding: 20px;
            min-height: calc(100vh - 120px);
        }
        .admin-iframe {
            width: 100%;
            height: calc(100vh - 120px);
            border: none;
            overflow: hidden;
        }
        /* Responsive design */
        @media (max-width: 768px) {
            .sidebar {
                width: 70px;
                overflow: hidden;
            }
            .sidebar .brand h2, .sidebar .brand p, .nav-link span {
                display: none;
            }
            .nav-link i {
                margin-right: 0;
                font-size: 18px;
            }
            .main-content {
                margin-left: 70px;
            }
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Sidebar Navigation -->
            <div class="sidebar">
                <div class="brand">
                    <h2>BSIT Admin</h2>
                    <p>Management System</p>
                </div>
                <ul class="nav-menu">
                    <li class="nav-item">
                        <a href="javascript:void(0);" onclick="loadPage('Dashboard.aspx')" class="nav-link" id="navDashboard" runat="server">
                            <i class="fas fa-tachometer-alt"></i><span>Dashboard</span>
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="javascript:void(0);" onclick="loadPage('ManageUsers.aspx')" class="nav-link" id="navManageUsers" runat="server">
                            <i class="fas fa-users"></i><span>Manage Users</span>
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="javascript:void(0);" onclick="loadPage('ManagePosts.aspx')" class="nav-link" id="navManagePosts" runat="server">
                            <i class="fas fa-newspaper"></i><span>Manage Posts</span>
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="javascript:void(0);" onclick="loadPage('Reports.aspx')" class="nav-link" id="navReports" runat="server">
                            <i class="fas fa-chart-bar"></i><span>Reports</span>
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="javascript:void(0);" onclick="loadPage('Settings.aspx')" class="nav-link" id="navSettings" runat="server">
                            <i class="fas fa-cog"></i><span>Settings</span>
                        </a>
                    </li>
                </ul>
            </div>

            <!-- Main Content -->
            <div class="main-content">
                <div class="header">
                    <h1 class="page-title" id="currentPageTitle">
                        Admin Panel
                    </h1>
                    <div class="user-profile">
                        <div class="user-info">
                            <div class="user-name"><asp:Label ID="lblAdminName" runat="server"></asp:Label></div>
                            <div class="user-role">Administrator</div>
                        </div>
                        <div class="dropdown">
                            <i class="fas fa-user-circle" style="font-size: 32px; color: #3498db;"></i>
                            <div class="dropdown-content">
                                <a href="javascript:void(0);" onclick="loadPage('Settings.aspx')"><i class="fas fa-cog"></i> Settings</a>
                                <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click">
                                    <i class="fas fa-sign-out-alt"></i> Logout
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="content-wrapper">
                    <!-- Using iframe to load content pages -->
                    <iframe id="adminContentFrame" class="admin-iframe" src="Dashboard.aspx" frameborder="0"></iframe>
                </div>
            </div>
    </div>
    </form>

    <!-- Additional scripts -->
    <script>
        // Function to load pages in the iframe
        function loadPage(page) {
            document.getElementById('adminContentFrame').src = page;
            
            // Update page title based on which page is loaded
            var pageTitle = '';
            
            if (page === 'Dashboard.aspx') {
                pageTitle = 'Dashboard';
            } else if (page === 'ManageUsers.aspx') {
                pageTitle = 'Manage Users';
            } else if (page === 'ManagePosts.aspx') {
                pageTitle = 'Manage Posts';
            } else if (page === 'Reports.aspx') {
                pageTitle = 'Reports';
            } else if (page === 'Settings.aspx') {
                pageTitle = 'Settings';
            }
            
            document.getElementById('currentPageTitle').innerText = pageTitle;
            
            // Highlight active navigation
            var navLinks = document.querySelectorAll('.nav-link');
            navLinks.forEach(function (link) {
                link.classList.remove('active');
                
                if (link.onclick.toString().includes(page)) {
                    link.classList.add('active');
                }
            });
        }
        
        // On page load, activate Dashboard by default
        document.addEventListener('DOMContentLoaded', function () {
            var navLinks = document.querySelectorAll('.nav-link');
            navLinks[0].classList.add('active');
        });
    </script>
</body>
</html>
