Imports System.Data
Imports BSIT.Module

Namespace BSIT
    Partial Public Class EditorManagePosts
        Inherits System.Web.UI.Page

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
                ' Use Connection class instead of direct SqlConnection
                Dim conn As New BSIT.Module.Connection()

                ' Add parameters
                conn.AddParam("@EditorID", Session("EditorID").ToString())

                ' Build query
                Dim query As String = "SELECT p.PostID, p.Title, " & _
                                  "CASE WHEN LEN(p.Content) > 100 THEN LEFT(p.Content, 100) + '...' ELSE p.Content END AS Excerpt, " & _
                                  "p.Category, p.Status, p.CreatedDate, p.Views " & _
                                  "FROM Posts p " & _
                                  "WHERE p.AuthorID = @EditorID"  ' Only show posts by the current editor

                ' Add search filter if provided
                If Not String.IsNullOrEmpty(searchTerm) Then
                    query += " AND (p.Title LIKE @SearchTerm OR p.Content LIKE @SearchTerm)"
                    conn.AddParam("@SearchTerm", "%" & searchTerm & "%")
                End If

                ' Add status filter if provided
                If Not String.IsNullOrEmpty(status) Then
                    query += " AND p.Status = @Status"
                    conn.AddParam("@Status", status)
                End If

                ' Add ORDER BY to sort results
                query += " ORDER BY p.CreatedDate DESC"

                ' Execute the query
                If conn.Query(query) Then
                    If conn.DataCount > 0 Then
                        ' Bind data to GridView
                        gvPosts.DataSource = conn.Data
                        gvPosts.DataBind()
                        
                        ' Show GridView and hide "no results" panel
                        gvPosts.Visible = True
                        pnlNoResults.Visible = False
                    Else
                        ' No results found, show the "no results" panel
                        gvPosts.Visible = False
                        pnlNoResults.Visible = True
                    End If
                Else
                    ' Query failed
                    Response.Write("<div class='alert alert-danger'>Error loading posts: Query failed</div>")
                End If
            Catch ex As Exception
                ' Show error message
                Response.Write("<div class='alert alert-danger'>Error loading posts: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Sub LoadPostStats()
            Try
                ' Use Connection class
                Dim conn As New BSIT.Module.Connection()
                
                ' Add parameters
                conn.AddParam("@EditorID", Session("EditorID").ToString())
                
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

                If conn.Query(query) AndAlso conn.DataCount > 0 Then
                    ' Get the results from the dataset
                    Dim row As DataRow = conn.Data.Tables(0).Rows(0)
                    
                    lblTotalPosts.Text = row("TotalPosts").ToString()
                    lblPublished.Text = row("Published").ToString()
                    lblPending.Text = row("Pending").ToString()
                    lblDrafts.Text = row("Drafts").ToString()
                    lblRejected.Text = row("Rejected").ToString()
                    lblTotalViews.Text = row("TotalViews").ToString()
                End If
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

                ' Use Connection class
                Dim conn As New BSIT.Module.Connection()
                
                ' Add parameters
                conn.AddParam("@Status", status)
                conn.AddParam("@PostID", postId.ToString())
                conn.AddParam("@EditorID", Session("EditorID").ToString())
                
                ' Update the post status
                Dim query As String = "UPDATE Posts SET Status = @Status WHERE PostID = @PostID AND AuthorID = @EditorID"
                
                If Not conn.Query(query) Then
                    Response.Write("<div class='alert alert-warning'>Unable to update post status. The post may have been deleted or you don't have permission.</div>")
                End If
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

                ' Use Connection class
                Dim conn As New BSIT.Module.Connection()
                
                ' Add parameters
                conn.AddParam("@PostID", postId.ToString())
                conn.AddParam("@EditorID", Session("EditorID").ToString())
                
                ' Delete the post
                Dim query As String = "DELETE FROM Posts WHERE PostID = @PostID AND AuthorID = @EditorID"
                
                If Not conn.Query(query) Then
                    Response.Write("<div class='alert alert-warning'>Unable to delete post. The post may have been already deleted or you don't have permission.</div>")
                End If
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error deleting post: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Function PostBelongsToCurrentEditor(ByVal postId As Integer) As Boolean
            Try
                ' Use Connection class
                Dim conn As New BSIT.Module.Connection()
                
                ' Add parameters
                conn.AddParam("@PostID", postId.ToString())
                conn.AddParam("@EditorID", Session("EditorID").ToString())
                
                ' Check if the post belongs to the current editor
                Dim query As String = "SELECT COUNT(*) FROM Posts WHERE PostID = @PostID AND AuthorID = @EditorID"
                
                If conn.Query(query) AndAlso conn.DataCount > 0 Then
                    Dim count As Integer = Convert.ToInt32(conn.Data.Tables(0).Rows(0)(0))
                    Return count > 0
                End If
                
                Return False
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error checking post ownership: " & ex.Message & "</div>")
                Return False
            End Try
        End Function
    End Class
End Namespace 