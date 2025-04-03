<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="ViewPostDetails.aspx.vb" Inherits="BSIT.ViewPostDetails" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Post Details</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container mt-4">
            <asp:Panel ID="pnlAlert" runat="server" Visible="false" CssClass="alert alert-danger">
                <asp:Literal ID="litAlertMessage" runat="server" />
            </asp:Panel>

            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Literal ID="litFeaturedImage" runat="server" />
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col-md-12">
                            <h2>
                                <asp:Literal ID="litPostTitle" runat="server" />
                            </h2>
                        </div>
                    </div>

                    <div class="row mt-3">
                        <div class="col-md-12">
                            <div class="d-flex flex-wrap gap-3">
                                <div>
                                    <i class="fas fa-user"></i>
                                    <asp:Literal ID="litAuthor" runat="server" />
                                </div>
                                <div>
                                    <i class="fas fa-folder"></i>
                                    <asp:Literal ID="litCategory" runat="server" />
                                </div>
                                <div>
                                    <i class="fas fa-calendar"></i>
                                    <asp:Literal ID="litCreatedDate" runat="server" />
                                </div>
                                <div>
                                    <i class="fas fa-clock"></i>
                                    <asp:Literal ID="litLastModifiedDate" runat="server" />
                                </div>
                                <div>
                                    <i class="fas fa-eye"></i>
                                    <asp:Literal ID="litViews" runat="server" />
                                </div>
                                <div>
                                    <i class="fas fa-info-circle"></i>
                                    <asp:Literal ID="litStatus" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col-md-12">
                            <div class="post-content">
                                <asp:Literal ID="litContent" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div class="row mt-4">
                        <div class="col-md-12">
                            <div class="d-flex gap-2">
                                <asp:LinkButton ID="btnViewPublic" runat="server" CssClass="btn btn-primary" OnClick="btnViewPublic_Click">
                                    <i class="fas fa-eye"></i> View Publicly
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-secondary" OnClick="btnBack_Click">
                                    <i class="fas fa-arrow-left"></i> Back to List
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    </form>
</body>
</html> 