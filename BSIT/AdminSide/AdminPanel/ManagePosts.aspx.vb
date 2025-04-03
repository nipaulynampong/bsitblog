Imports System.Data
Imports System.Data.SqlClient

Namespace BSIT
    Partial Public Class ManagePosts
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Redirect to login if not logged in
                If Session("AdminID") Is Nothing Then
                    Response.Redirect("~/AdminSide/AdminLogin.aspx")
                    Return
                End If

                LoadPosts()
            End If
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
            LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)
        End Sub

        Protected Sub ddlStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)
        End Sub

        Protected Sub btnAddPost_Click(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect("AddPost.aspx")
        End Sub

        Protected Sub gvPosts_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
            ' Validate command argument
            If e.CommandArgument Is Nothing OrElse String.IsNullOrEmpty(e.CommandArgument.ToString()) Then
                Return
            End If

            Dim postId As Integer
            If Not Integer.TryParse(e.CommandArgument.ToString(), postId) Then
                ' Invalid post ID
                Return
            End If

            Select Case e.CommandName
                Case "EditPost"
                    Response.Redirect("~/AdminSide/AdminPanel/ViewPostDetails.aspx?id=" & postId)

                Case "ViewPost"
                    Response.Redirect("~/AdminSide/AdminPanel/ViewPostDetails.aspx?id=" & postId)

                Case "PublishPost"
                    UpdatePostStatus(postId, "Published")
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)

                Case "ArchivePost"
                    UpdatePostStatus(postId, "Archived")
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)

                Case "DeletePost"
                    DeletePost(postId)
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)

                Case "SubmitForApproval"
                    UpdatePostStatus(postId, "Pending")
                    LogApprovalAction(postId, "Submitted for approval")
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)

                Case "ApprovePost"
                    UpdatePostStatus(postId, "Approved")
                    LogApprovalAction(postId, "Approved")
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)

                Case "RejectPost"
                    UpdatePostStatus(postId, "Rejected")
                    LogApprovalAction(postId, "Rejected")
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)
            End Select
        End Sub

        ' Public function used by ASPX for status CSS class
        Public Function GetStatusCssClass(ByVal status As String) As String
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

        ' Private methods for database operations
        Private Sub LoadPosts(Optional ByVal searchTerm As String = "", Optional ByVal status As String = "")
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT p.PostID, p.Title, " & _
                                      "CASE WHEN LEN(p.Content) > 100 THEN LEFT(p.Content, 100) + '...' ELSE p.Content END AS Excerpt, " & _
                                      "u.fullname AS Author, " & _
                                      "p.Category, " & _
                                      "p.Status, p.CreatedDate, p.Views " & _
                                      "FROM Posts p " & _
                                      "INNER JOIN users u ON p.AuthorID = u.UserID"

                    ' Build where clause based on parameters
                    Dim whereConditions As New List(Of String)()
                    Dim parameters As New List(Of SqlParameter)()

                    ' Add search filter if provided
                    If Not String.IsNullOrEmpty(searchTerm) Then
                        whereConditions.Add("(p.Title LIKE @SearchTerm OR p.Content LIKE @SearchTerm)")
                        parameters.Add(New SqlParameter("@SearchTerm", "%" & searchTerm & "%"))
                    End If

                    ' Add status filter if provided
                    If Not String.IsNullOrEmpty(status) Then
                        whereConditions.Add("p.Status = @Status")
                        parameters.Add(New SqlParameter("@Status", status))
                    End If

                    ' Add WHERE clause if we have conditions
                    If whereConditions.Count > 0 Then
                        query += " WHERE " & String.Join(" AND ", whereConditions)
                    End If

                    ' Add ORDER BY to sort results
                    query += " ORDER BY p.CreatedDate DESC"

                    Using cmd As New SqlCommand(query, conn)
                        ' Add parameters to command
                        For Each param As SqlParameter In parameters
                            cmd.Parameters.Add(param)
                        Next

                        ' Create data adapter and dataset
                        Using adapter As New SqlDataAdapter(cmd)
                            Dim ds As New DataSet()
                            conn.Open()
                            Dim rowCount As Integer = adapter.Fill(ds)

                            If rowCount > 0 Then
                                ' Bind data to GridView
                                gvPosts.DataSource = ds
                                gvPosts.DataBind()
                                
                                ' Show GridView and hide "no results" panel
                                gvPosts.Visible = True
                                pnlNoResults.Visible = False
                            Else
                                ' No results found, show the "no results" panel
                                gvPosts.Visible = False
                                pnlNoResults.Visible = True
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Show error message
                Response.Write("<div class='alert alert-danger'>Error loading posts: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Sub UpdatePostStatus(ByVal postId As Integer, ByVal status As String)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("UPDATE Posts SET Status = @Status WHERE PostID = @PostID", conn)
                        cmd.Parameters.AddWithValue("@Status", status)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error updating post status: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Sub DeletePost(ByVal postId As Integer)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("DELETE FROM Posts WHERE PostID = @PostID", conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error deleting post: " & ex.Message & "</div>")
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
                    Dim checkTableCmd As New SqlCommand( _
                        "IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PostApprovalLog') " & _
                        "SELECT 1 ELSE SELECT 0", conn)
                    
                    Dim tableExists As Boolean = Convert.ToBoolean(checkTableCmd.ExecuteScalar())
                    
                    If Not tableExists Then
                        ' Create the table if it doesn't exist
                        Dim createTableCmd As New SqlCommand( _
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
    End Class
End Namespace