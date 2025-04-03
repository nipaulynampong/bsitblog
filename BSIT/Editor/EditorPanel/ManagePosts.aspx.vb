Imports System.Data
Imports System.Data.SqlClient

Namespace BSIT
    Partial Public Class EditorManagePosts
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Redirect to login if not logged in
                If Session("EditorID") Is Nothing Then
                    Response.Redirect("~/Editor/EditorLogin.aspx")
                    Return
                End If

                LoadPosts()
                LoadPostStats()
            End If
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
            LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)
        End Sub

        Protected Sub ddlStatus_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)
        End Sub

        Protected Sub btnCreatePost_Click(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect("CreatePosts.aspx")
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
                    Response.Redirect("CreatePosts.aspx?id=" & postId)

                Case "ViewPost"
                    Response.Redirect("PreviewPost.aspx?id=" & postId)

                Case "DeletePost"
                    DeletePost(postId)
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)
                    LoadPostStats()

                Case "SubmitForApproval"
                    UpdatePostStatus(postId, "Pending")
                    LoadPosts(txtSearch.Text.Trim(), ddlStatus.SelectedValue)
                    LoadPostStats()

                Case "ViewFeedback"
                    ' Open a feedback modal or redirect to a feedback page
                    ' For now, we'll redirect to a placeholder page
                    Response.Redirect("ViewFeedback.aspx?id=" & postId)
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
                                      "p.Category, p.Status, p.CreatedDate, p.Views " & _
                                      "FROM Posts p " & _
                                      "WHERE p.AuthorID = @EditorID"  ' Only show posts by the current editor

                    ' Build where clause based on parameters
                    Dim whereConditions As New List(Of String)()
                    Dim parameters As New List(Of SqlParameter)()

                    ' Add EditorID parameter (always required)
                    parameters.Add(New SqlParameter("@EditorID", Session("EditorID")))

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

                    ' Add additional WHERE conditions if we have them
                    If whereConditions.Count > 0 Then
                        query += " AND " & String.Join(" AND ", whereConditions)
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

        Private Sub LoadPostStats()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    ' Get counts for different post statuses
                    Dim query As String = "SELECT " & _
                                        "COUNT(*) AS TotalPosts, " & _
                                        "SUM(CASE WHEN Status = 'Published' THEN 1 ELSE 0 END) AS Published, " & _
                                        "SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS Pending, " & _
                                        "SUM(CASE WHEN Status = 'Draft' THEN 1 ELSE 0 END) AS Drafts, " & _
                                        "SUM(CASE WHEN Status = 'Rejected' THEN 1 ELSE 0 END) AS Rejected, " & _
                                        "SUM(Views) AS TotalViews " & _
                                        "FROM Posts " & _
                                        "WHERE AuthorID = @EditorID"

                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@EditorID", Session("EditorID"))
                        
                        conn.Open()
                        Dim reader As SqlDataReader = cmd.ExecuteReader()
                        
                        If reader.Read() Then
                            lblTotalPosts.Text = reader("TotalPosts").ToString()
                            lblPublished.Text = reader("Published").ToString()
                            lblPending.Text = reader("Pending").ToString()
                            lblDrafts.Text = reader("Drafts").ToString()
                            lblRejected.Text = reader("Rejected").ToString()
                            lblTotalViews.Text = reader("TotalViews").ToString()
                        End If
                    End Using
                End Using
            Catch ex As Exception
                ' Just log the error, don't show to the user
                System.Diagnostics.Debug.WriteLine("Error loading post stats: " & ex.Message)
            End Try
        End Sub

        Private Sub UpdatePostStatus(ByVal postId As Integer, ByVal status As String)
            Try
                ' First check if the post belongs to the current editor
                If Not PostBelongsToCurrentEditor(postId) Then
                    Response.Write("<div class='alert alert-danger'>You don't have permission to modify this post.</div>")
                    Return
                End If

                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("UPDATE Posts SET Status = @Status WHERE PostID = @PostID AND AuthorID = @EditorID", conn)
                        cmd.Parameters.AddWithValue("@Status", status)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        cmd.Parameters.AddWithValue("@EditorID", Session("EditorID"))
                        
                        conn.Open()
                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                        
                        If rowsAffected <= 0 Then
                            Response.Write("<div class='alert alert-warning'>Unable to update post status. The post may have been deleted or you don't have permission.</div>")
                        End If
                    End Using
                End Using
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error updating post status: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Sub DeletePost(ByVal postId As Integer)
            Try
                ' First check if the post belongs to the current editor
                If Not PostBelongsToCurrentEditor(postId) Then
                    Response.Write("<div class='alert alert-danger'>You don't have permission to delete this post.</div>")
                    Return
                End If

                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("DELETE FROM Posts WHERE PostID = @PostID AND AuthorID = @EditorID", conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        cmd.Parameters.AddWithValue("@EditorID", Session("EditorID"))
                        
                        conn.Open()
                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                        
                        If rowsAffected <= 0 Then
                            Response.Write("<div class='alert alert-warning'>Unable to delete post. The post may have been already deleted or you don't have permission.</div>")
                        End If
                    End Using
                End Using
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error deleting post: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Function PostBelongsToCurrentEditor(ByVal postId As Integer) As Boolean
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("SELECT COUNT(*) FROM Posts WHERE PostID = @PostID AND AuthorID = @EditorID", conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        cmd.Parameters.AddWithValue("@EditorID", Session("EditorID"))
                        
                        conn.Open()
                        Dim postCount As Integer = CInt(cmd.ExecuteScalar())
                        Return postCount > 0
                    End Using
                End Using
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error checking post ownership: " & ex.Message & "</div>")
                Return False
            End Try
        End Function
    End Class
End Namespace 