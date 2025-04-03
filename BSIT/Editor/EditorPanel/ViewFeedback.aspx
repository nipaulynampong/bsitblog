<%@ Page Language="vb" AutoEventWireup="true" CodeFile="ViewFeedback.aspx.vb" Inherits="BSIT.ViewFeedback" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Post Feedback</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
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
            max-width: 900px;
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

        .post-details {
            margin-bottom: 30px;
            padding-bottom: 20px;
            border-bottom: 1px solid #eee;
        }

        .post-title {
            font-size: 20px;
            font-weight: 600;
            margin-bottom: 10px;
            color: var(--dark-color);
        }

        .feedback-container {
            background-color: #fff9ec;
            border-left: 4px solid #f77f00;
            padding: 20px;
            border-radius: 4px;
            margin-bottom: 20px;
        }

        .feedback-title {
            font-weight: 600;
            color: #e63946;
            margin-bottom: 15px;
            font-size: 16px;
        }

        .feedback-content {
            color: #333;
            line-height: 1.6;
        }

        .feedback-meta {
            font-size: 12px;
            color: #666;
            margin-top: 15px;
            display: flex;
            align-items: center;
        }

        .feedback-meta i {
            margin-right: 5px;
            color: #999;
        }

        .feedback-actions {
            margin-top: 20px;
            text-align: right;
        }

        .btn {
            padding: 10px 20px;
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

        .btn-info {
            background-color: var(--info-color);
            border-color: var(--info-color);
            color: white;
        }

        @media (max-width: 768px) {
            .card-header {
                flex-direction: column;
                gap: 10px;
                align-items: flex-start;
            }
            
            .card-header .btn {
                width: 100%;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1 class="page-title"><i class="fas fa-comment-alt"></i> Post Feedback</h1>
            
            <div class="card">
                <div class="card-header">
                    <h5><i class="fas fa-clipboard-list"></i> Rejection Feedback</h5>
                    <asp:Button ID="btnBackToList" runat="server" Text="Back to My Posts" CssClass="btn btn-primary" OnClick="btnBackToList_Click" />
                </div>
                <div class="card-body">
                    <div class="post-details">
                        <div class="post-title">
                            <asp:Label ID="lblPostTitle" runat="server"></asp:Label>
                        </div>
                        <div>
                            <strong>Submitted on:</strong> 
                            <asp:Label ID="lblSubmissionDate" runat="server"></asp:Label>
                        </div>
                        <div>
                            <strong>Status:</strong> 
                            <span style="color: #e63946; font-weight: 600;">Rejected</span>
                        </div>
                    </div>
                    
                    <div class="feedback-container">
                        <div class="feedback-title">
                            <i class="fas fa-exclamation-circle"></i> Reviewer Feedback
                        </div>
                        <div class="feedback-content">
                            <asp:Label ID="lblFeedbackContent" runat="server"></asp:Label>
                        </div>
                        <div class="feedback-meta">
                            <i class="fas fa-user"></i> 
                            <asp:Label ID="lblReviewerName" runat="server"></asp:Label>
                            &nbsp;&nbsp;|&nbsp;&nbsp;
                            <i class="fas fa-calendar-alt"></i> 
                            <asp:Label ID="lblFeedbackDate" runat="server"></asp:Label>
                        </div>
                    </div>
                    
                    <div class="feedback-actions">
                        <asp:Button ID="btnEditPost" runat="server" Text="Edit Post" CssClass="btn btn-info" OnClick="btnEditPost_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="../../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../../Scripts/bootstrap.bundle.min.js"></script>
</body>
</html> 