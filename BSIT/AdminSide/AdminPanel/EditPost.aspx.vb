Imports System.Data
Imports System.Data.SqlClient

Namespace BSIT
    Partial Public Class EditPost
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"
        Private _postId As Integer = 0

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
                        LoadCategories()
                        LoadPostData(_postId)
                    End If
                Else
                    ShowError("Invalid post ID specified.")
                    btnSave.Enabled = False
                    btnPublish.Enabled = False
                End If
            Else
                ShowError("No post ID specified.")
                btnSave.Enabled = False
                btnPublish.Enabled = False
            End If
        End Sub

        Private Sub LoadCategories()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName"
                    Using cmd As New SqlCommand(query, conn)
                        conn.Open()
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            ddlCategory.Items.Clear()
                            ddlCategory.Items.Add(New ListItem("-- Select Category --", ""))
                            
                            While reader.Read()
                                ddlCategory.Items.Add(New ListItem(
                                    reader("CategoryName").ToString(),
                                    reader("CategoryID").ToString()))
                            End While
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error loading categories: " & ex.Message)
            End Try
        End Sub

        Private Sub LoadPostData(ByVal postId As Integer)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT p.*, c.CategoryID FROM Posts p " & _
                                         "LEFT JOIN Categories c ON p.Category = c.CategoryName " & _
                                         "WHERE p.PostID = @PostID"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        conn.Open()
                        
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                ' Populate form with post data
                                txtTitle.Text = reader("Title").ToString()
                                txtContent.Text = reader("Content").ToString()
                                
                                ' Set category if available
                                If Not IsDBNull(reader("CategoryID")) Then
                                    ddlCategory.SelectedValue = reader("CategoryID").ToString()
                                End If
                                
                                ' Set excerpt if available
                                If Not IsDBNull(reader("Excerpt")) Then
                                    txtExcerpt.Text = reader("Excerpt").ToString()
                                End If
                                
                                ' Set tags if available
                                If Not IsDBNull(reader("Tags")) Then
                                    txtTags.Text = reader("Tags").ToString()
                                End If
                                
                                ' Set status
                                If Not IsDBNull(reader("Status")) Then
                                    Dim status As String = reader("Status").ToString()
                                    If ddlStatus.Items.FindByValue(status) IsNot Nothing Then
                                        ddlStatus.SelectedValue = status
                                    End If
                                End If
                            Else
                                ShowError("Post not found with the specified ID.")
                                btnSave.Enabled = False
                                btnPublish.Enabled = False
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error loading post data: " & ex.Message)
            End Try
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs)
            SavePost(False)
        End Sub

        Protected Sub btnPublish_Click(ByVal sender As Object, ByVal e As EventArgs)
            SavePost(True)
        End Sub

        Protected Sub btnSaveAsDraft_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Save without validation
            Page.Validate("")
            If Not Page.IsValid Then
                ShowError("Please fill in all required fields before saving.")
                Return
            End If
            
            ddlStatus.SelectedValue = "Draft"
            SavePost(False)
        End Sub

        Protected Sub btnPreview_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Save post to session and redirect to preview page
            Session("PreviewTitle") = txtTitle.Text
            Session("PreviewContent") = txtContent.Text
            Session("PreviewExcerpt") = txtExcerpt.Text
            
            ' Open in new tab - client script
            ClientScript.RegisterStartupScript(Me.GetType(), "Preview", "window.open('PreviewPost.aspx', '_blank');", True)
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Cancel editing and return to manage posts
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Private Sub SavePost(ByVal publish As Boolean)
            If Not Page.IsValid Then
                ShowError("Please fill in all required fields before saving.")
                Return
            End If

            Try
                ' If publish is true, set status to Published
                If publish Then
                    ddlStatus.SelectedValue = "Published"
                End If

                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "UPDATE Posts SET " & _
                                         "Title = @Title, " & _
                                         "Content = @Content, " & _
                                         "Excerpt = @Excerpt, " & _
                                         "Category = @Category, " & _
                                         "Tags = @Tags, " & _
                                         "Status = @Status, " & _
                                         "ModifiedDate = @ModifiedDate, " & _
                                         "ModifiedBy = @ModifiedBy " & _
                                         "WHERE PostID = @PostID"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim())
                        cmd.Parameters.AddWithValue("@Content", txtContent.Text)
                        cmd.Parameters.AddWithValue("@Excerpt", txtExcerpt.Text.Trim())
                        
                        ' Get the selected category text
                        Dim categoryText As String = ddlCategory.SelectedItem.Text
                        If categoryText = "-- Select Category --" Then
                            categoryText = String.Empty
                        End If
                        cmd.Parameters.AddWithValue("@Category", categoryText)
                        
                        cmd.Parameters.AddWithValue("@Tags", txtTags.Text.Trim())
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now)
                        
                        If Session("AdminID") IsNot Nothing Then
                            cmd.Parameters.AddWithValue("@ModifiedBy", Convert.ToInt32(Session("AdminID")))
                        Else
                            cmd.Parameters.AddWithValue("@ModifiedBy", DBNull.Value)
                        End If
                        
                        cmd.Parameters.AddWithValue("@PostID", _postId)
                        
                        conn.Open()
                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                        
                        If rowsAffected > 0 Then
                            ' Log the action in the post approval log if status is changed
                            If ddlStatus.SelectedValue = "Published" Then
                                LogApprovalAction(_postId, "Published by admin")
                            ElseIf ddlStatus.SelectedValue = "Approved" Then
                                LogApprovalAction(_postId, "Approved by admin")
                            End If
                            
                            ' Show success message
                            ShowSuccess("Post saved successfully!")
                            
                            ' Delay redirect to allow the user to see the success message
                            ClientScript.RegisterStartupScript(Me.GetType(), "RedirectAfterDelay", 
                                "setTimeout(function() { window.location.href = 'ManagePosts.aspx'; }, 2000);", True)
                        Else
                            ShowError("Failed to save post. The post may have been deleted.")
                        End If
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error saving post: " & ex.Message)
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

        Private Sub ShowError(ByVal message As String)
            pnlAlert.CssClass = "alert alert-danger"
            litAlertMessage.Text = message
            pnlAlert.Visible = True
        End Sub

        Private Sub ShowSuccess(ByVal message As String)
            pnlAlert.CssClass = "alert alert-success"
            litAlertMessage.Text = message
            pnlAlert.Visible = True
        End Sub
    End Class
End Namespace 