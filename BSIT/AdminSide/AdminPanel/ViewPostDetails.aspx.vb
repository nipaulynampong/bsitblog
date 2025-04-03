Imports System.Data
Imports System.Data.SqlClient

Namespace BSIT
    Partial Public Class ViewPostDetails
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"
        Private _postId As Integer = 0
        Private _postStatus As String = String.Empty

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            ' Security check - redirect to login if not authenticated
            If Session("AdminID") Is Nothing Then
                Response.Redirect("~/AdminSide/AdminLogin.aspx")
                Return
            End If

            ' Get post ID from query string
            If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                If Integer.TryParse(Request.QueryString("id"), _postId) Then
                    If Not IsPostBack Then
                        LoadPostData(_postId)
                    End If
                Else
                    ShowError("Invalid post ID specified.")
                End If
            Else
                ShowError("No post ID specified.")
            End If
        End Sub

        Private Sub LoadPostData(ByVal postId As Integer)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT p.PostID, p.Title, p.Content, p.AuthorID, p.Category, p.Status, " & _
                                         "p.CreatedDate, p.LastModifiedDate, p.Views, p.Featuredlmage, " & _
                                         "u.fullname AS AuthorName " & _
                                         "FROM Posts p " & _
                                         "INNER JOIN users u ON p.AuthorID = u.UserID " & _
                                         "WHERE p.PostID = @PostID"
                    
                    System.Diagnostics.Debug.WriteLine("Executing query for PostID: " & postId)
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        conn.Open()
                        
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                System.Diagnostics.Debug.WriteLine("Row found in database")
                                
                                ' Debug: Print all column names
                                For i As Integer = 0 To reader.FieldCount - 1
                                    System.Diagnostics.Debug.WriteLine("Column " & i & ": " & reader.GetName(i))
                                Next
                                
                                ' Populate form with post data
                                litPostTitle.Text = reader("Title").ToString()
                                litContent.Text = reader("Content").ToString()
                                litAuthor.Text = reader("AuthorName").ToString()
                                litCategory.Text = reader("Category").ToString()
                                litStatus.Text = reader("Status").ToString()
                                
                                ' Format dates
                                Dim createdDate As DateTime = Convert.ToDateTime(reader("CreatedDate"))
                                Dim lastModifiedDate As DateTime = Convert.ToDateTime(reader("LastModifiedDate"))
                                
                                ' Display dates and views
                                litCreatedDate.Text = createdDate.ToString("MMMM dd, yyyy hh:mm tt")
                                litLastModifiedDate.Text = lastModifiedDate.ToString("MMMM dd, yyyy hh:mm tt")
                                litViews.Text = reader("Views").ToString()
                                
                                ' Handle FeaturedImage
                                Try
                                    System.Diagnostics.Debug.WriteLine("Checking Featuredlmage column...")
                                    If Not IsDBNull(reader("Featuredlmage")) Then
                                        Dim imageUrl As String = reader("Featuredlmage").ToString()
                                        System.Diagnostics.Debug.WriteLine("Raw image URL from DB: " & imageUrl)
                                        
                                        If Not String.IsNullOrEmpty(imageUrl) Then
                                            ' Convert virtual path to physical path
                                            If imageUrl.StartsWith("~/") Then
                                                imageUrl = imageUrl.Substring(2)  ' Remove the ~/
                                            End If
                                            
                                            ' Ensure the path starts with a slash
                                            If Not imageUrl.StartsWith("/") Then
                                                imageUrl = "/" & imageUrl
                                            End If
                                            
                                            System.Diagnostics.Debug.WriteLine("Processed image URL: " & imageUrl)
                                            litFeaturedImage.Text = "<img src='" & imageUrl & "' alt='Featured Image' class='img-fluid rounded' style='max-width: 100%; height: auto;' />"
                                        Else
                                            System.Diagnostics.Debug.WriteLine("Image URL is empty")
                                        End If
                                    Else
                                        System.Diagnostics.Debug.WriteLine("Featuredlmage is NULL in database")
                                    End If
                                Catch ex As Exception
                                    System.Diagnostics.Debug.WriteLine("Error handling Featuredlmage: " & ex.Message)
                                    System.Diagnostics.Debug.WriteLine("Stack trace: " & ex.StackTrace)
                                End Try
                            Else
                                ShowError("Post not found with the specified ID.")
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error loading post data: " & ex.Message)
            End Try
        End Sub

        Protected Sub btnViewPublic_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Redirect to the public view of the post
            Response.Redirect("~/ViewPost.aspx?id=" & _postId)
        End Sub

        Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Return to manage posts
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Protected Sub btnApprove_Click(ByVal sender As Object, ByVal e As EventArgs)
            UpdatePostStatus("Approved")
            LogApprovalAction(_postId, "Approved by admin")
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Protected Sub btnReject_Click(ByVal sender As Object, ByVal e As EventArgs)
            UpdatePostStatus("Rejected")
            LogApprovalAction(_postId, "Rejected by admin")
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Protected Sub btnPublish_Click(ByVal sender As Object, ByVal e As EventArgs)
            UpdatePostStatus("Published")
            LogApprovalAction(_postId, "Published by admin")
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Private Sub UpdatePostStatus(ByVal status As String)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("UPDATE Posts SET Status = @Status, ModifiedDate = @ModifiedDate, ModifiedBy = @ModifiedBy WHERE PostID = @PostID", conn)
                        cmd.Parameters.AddWithValue("@Status", status)
                        cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now)
                        
                        If Session("AdminID") IsNot Nothing Then
                            cmd.Parameters.AddWithValue("@ModifiedBy", Convert.ToInt32(Session("AdminID")))
                        Else
                            cmd.Parameters.AddWithValue("@ModifiedBy", DBNull.Value)
                        End If
                        
                        cmd.Parameters.AddWithValue("@PostID", _postId)
                        
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error updating post status: " & ex.Message)
            End Try
        End Sub

        Private Sub LogApprovalAction(ByVal postId As Integer, ByVal action As String)
            Try
                ' Check if the PostApprovalLog table exists, create it if it doesn't
                EnsureApprovalLogTableExists()
                
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("INSERT INTO PostApprovalLog (PostID, AdminID, Action, ActionDate, AdminName) VALUES (@PostID, @AdminID, @Action, @ActionDate, @AdminName)", conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        
                        ' Handle potential null session values
                        If Session("AdminID") IsNot Nothing Then
                            cmd.Parameters.AddWithValue("@AdminID", Session("AdminID"))
                        Else
                            cmd.Parameters.AddWithValue("@AdminID", DBNull.Value)
                        End If
                        
                        cmd.Parameters.AddWithValue("@Action", action)
                        cmd.Parameters.AddWithValue("@ActionDate", DateTime.Now)
                        
                        ' Handle potential null session values
                        If Session("AdminName") IsNot Nothing Then
                            cmd.Parameters.AddWithValue("@AdminName", Session("AdminName").ToString())
                        Else
                            cmd.Parameters.AddWithValue("@AdminName", "Unknown Admin")
                        End If
                        
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
                ' Just log the error, don't show to the user
                System.Diagnostics.Debug.WriteLine("Error logging approval action: " & ex.Message)
            End Try
        End Sub

        Private Sub EnsureApprovalLogTableExists()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    conn.Open()
                    
                    ' Check if table exists
                    Dim checkTableCmd As New SqlCommand(
                        "IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PostApprovalLog') " & _
                        "SELECT 1 ELSE SELECT 0", conn)
                    
                    Dim tableExists As Boolean = Convert.ToBoolean(checkTableCmd.ExecuteScalar())
                    
                    If Not tableExists Then
                        ' Create the table if it doesn't exist
                        Dim createTableCmd As New SqlCommand(
                            "CREATE TABLE PostApprovalLog (" & _
                            "LogID INT PRIMARY KEY IDENTITY(1,1), " & _
                            "PostID INT NOT NULL, " & _
                            "AdminID INT, " & _
                            "Action NVARCHAR(100) NOT NULL, " & _
                            "ActionDate DATETIME NOT NULL, " & _
                            "AdminName NVARCHAR(100), " & _
                            "Notes NVARCHAR(500))", conn)
                        
                        createTableCmd.ExecuteNonQuery()
                    End If
                End Using
            Catch ex As Exception
                ' Just log the error, don't show to the user
                System.Diagnostics.Debug.WriteLine("Error ensuring approval log table: " & ex.Message)
            End Try
        End Sub
        
        Private Function GetStatusCssClass(ByVal status As String) As String
            If String.IsNullOrEmpty(status) Then
                Return String.Empty
            End If

            Select Case status.Trim().ToLower()
                Case "published"
                    Return "status-published"
                Case "draft"
                    Return "status-draft"
                Case "archived"
                    Return "status-archived"
                Case "pending"
                    Return "status-pending"
                Case "approved"
                    Return "status-approved"
                Case "rejected"
                    Return "status-rejected"
                Case Else
                    Return String.Empty
            End Select
        End Function

        Private Sub ShowError(ByVal message As String)
            pnlAlert.CssClass = "alert alert-danger"
            litAlertMessage.Text = message
            pnlAlert.Visible = True
        End Sub
    End Class
End Namespace 