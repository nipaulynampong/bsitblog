<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EditorPanel.aspx.vb" Inherits="BSIT.EditorPanel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Editor Panel - BSIT Blog</title>
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
            background-color: var(--gray-light);
            min-height: 100vh;
        }

        .editor-container {
            display: flex;
            min-height: 100vh;
        }

        .sidebar {
            width: 250px;
            background-color: var(--maroon-primary);
            padding: 20px;
            color: var(--white);
            position: fixed;
            height: 100vh;
            overflow-y: auto;
        }

        .logo {
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 30px;
            text-align: center;
            padding: 10px;
            background-color: var(--maroon-dark);
            border-radius: 5px;
        }

        .nav-menu {
            list-style: none;
        }

        .nav-item {
            margin-bottom: 15px;
            transition: all 0.3s ease;
        }

        .nav-item:hover {
            transform: translateX(5px);
        }

        .nav-link {
            color: var(--white);
            text-decoration: none;
            display: flex;
            align-items: center;
            padding: 12px 15px;
            border-radius: 5px;
            transition: all 0.3s ease;
        }

        .nav-link:hover {
            background-color: var(--maroon-light);
        }

        .nav-link.active {
            background-color: var(--maroon-light);
            border-left: 4px solid var(--white);
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
        }

        .header {
            background-color: var(--white);
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            margin-bottom: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .welcome-text {
            font-size: 24px;
            color: var(--maroon-primary);
        }

        .user-profile {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .profile-img {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background-color: var(--maroon-light);
            display: flex;
            align-items: center;
            justify-content: center;
            color: var(--white);
        }

        .content-frame {
            background-color: var(--white);
            border-radius: 10px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            height: calc(100vh - 180px);
            overflow: hidden;
        }

        iframe {
            width: 100%;
            height: 100%;
            border: none;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .content-frame {
            animation: fadeIn 0.5s ease-out;
        }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
</head>
<body>
    <form id="form1" runat="server">
    <div class="editor-container">
        <div class="sidebar">
            <div class="logo">BSIT Blog</div>
            <ul class="nav-menu">
                <li class="nav-item">
                    <a href="EditorDashboard.aspx" class="nav-link" target="contentFrame">
                        <i class="fas fa-home"></i> Dashboard
                    </a>
                </li>
                <li class="nav-item">
                    <a href="CreatePosts.aspx" class="nav-link" target="contentFrame">
                        <i class="fas fa-edit"></i> Create Post
                    </a>
                </li>
                <li class="nav-item">
                    <a href="EditorSettings.aspx" class="nav-link" target="contentFrame">
                        <i class="fas fa-cog"></i> Settings
                    </a>
                </li>
            </ul>
        </div>
        <div class="main-content">
            <div class="header">
                <h1 class="welcome-text">Welcome, Editor</h1>
                <div class="user-profile">
                    <div class="profile-img">
                        <i class="fas fa-user"></i>
                    </div>
                    <span>Editor Name</span>
                </div>
            </div>
            <div class="content-frame">
                <iframe name="contentFrame" src="EditorDashboard.aspx"></iframe>
            </div>
        </div>
    </div>
    </form>

    <script type="text/javascript">
        // Add active class to current navigation link
        function setActiveNavLink() {
            const links = document.querySelectorAll('.nav-link');
            links.forEach(link => {
                if (link.href === window.location.href) {
                    link.classList.add('active');
                }
            });
        }

        // Update active nav link when iframe content changes
        window.addEventListener('load', function() {
            const iframe = document.querySelector('iframe[name="contentFrame"]');
            iframe.addEventListener('load', function() {
                const links = document.querySelectorAll('.nav-link');
                links.forEach(link => {
                    if (link.href === iframe.contentWindow.location.href) {
                        link.classList.add('active');
                    } else {
                        link.classList.remove('active');
                    }
                });
            });
        });
    </script>
</body>
</html> 