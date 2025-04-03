Imports System.Data
Imports System.Data.SqlClient

Namespace BSIT
    Partial Public Class ArchivePosts
    Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Redirect to login if not logged in
                If Session("AdminID") Is Nothing Then
                    Response.Redirect("~/AdminSide/AdminLogin.aspx")
                    Return
                End If

                LoadArchivedPosts()
            End If
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
            LoadArchivedPosts(txtSearch.Text.Trim(), ddlArchiveDate.SelectedValue)
        End Sub

        Protected Sub ddlArchiveDate_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            LoadArchivedPosts(txtSearch.Text.Trim(), ddlArchiveDate.SelectedValue)
        End Sub

        Protected Sub btnReturnToManage_Click(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Protected Sub gvArchivedPosts_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
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
                Case "ViewPost"
                    Response.Redirect("~/AdminSide/AdminPanel/ViewPostDetails.aspx?id=" & postId)

                Case "RestorePost"
                    RestorePost(postId)
                    LoadArchivedPosts(txtSearch.Text.Trim(), ddlArchiveDate.SelectedValue)

                Case "DeletePost"
                    DeletePost(postId)
                    LoadArchivedPosts(txtSearch.Text.Trim(), ddlArchiveDate.SelectedValue)
            End Select
        End Sub

        ' Private methods for database operations
        Private Sub LoadArchivedPosts(Optional ByVal searchTerm As String = "", Optional ByVal daysFilter As String = "")
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT p.PostID, p.Title, " & _
                                      "CASE WHEN LEN(p.Content) > 100 THEN LEFT(p.Content, 100) + '...' ELSE p.Content END AS Excerpt, " & _
                                      "u.fullname AS Author, " & _
                                      "p.Category, " & _
                                      "p.ModifiedDate AS ArchivedDate, p.Views " & _
                                      "FROM Posts p " & _
                                      "INNER JOIN users u ON p.AuthorID = u.UserID " & _
                                      "WHERE p.Status = 'Archived'"

                    ' Add search filter if provided
                    If Not String.IsNullOrEmpty(searchTerm) Then
                        query += " AND (p.Title LIKE @SearchTerm OR p.Content LIKE @SearchTerm)"
                    End If

                    ' Add days filter if provided
                    If Not String.IsNullOrEmpty(daysFilter) Then
                        query += " AND p.ModifiedDate >= DATEADD(day, -@DaysFilter, GETDATE())"
                    End If

                    ' Add ORDER BY to sort results
                    query += " ORDER BY p.ModifiedDate DESC"

                    Using cmd As New SqlCommand(query, conn)
                        ' Add parameters if needed
                        If Not String.IsNullOrEmpty(searchTerm) Then
                            cmd.Parameters.AddWithValue("@SearchTerm", "%" & searchTerm & "%")
                        End If

                        If Not String.IsNullOrEmpty(daysFilter) Then
                            cmd.Parameters.AddWithValue("@DaysFilter", Integer.Parse(daysFilter))
                        End If

                        ' Create data adapter and dataset
                        Using adapter As New SqlDataAdapter(cmd)
                            Dim ds As New DataSet()
                            conn.Open()
                            Dim rowCount As Integer = adapter.Fill(ds)

                            If rowCount > 0 Then
                                ' Bind data to GridView
                                gvArchivedPosts.DataSource = ds
                                gvArchivedPosts.DataBind()
                                
                                ' Show GridView and hide "no results" panel
                                gvArchivedPosts.Visible = True
                                pnlNoResults.Visible = False
                            Else
                                ' No results found, show the "no results" panel
                                gvArchivedPosts.Visible = False
                                pnlNoResults.Visible = True
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Show error message
                Response.Write("<div class='alert alert-danger'>Error loading archived posts: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Sub RestorePost(ByVal postId As Integer)
            Try
                ' First, determine what the post status was before archiving
                Dim previousStatus As String = GetOriginalPostStatus(postId)
                
                ' If we couldn't determine previous status, default to Draft
                If String.IsNullOrEmpty(previousStatus) OrElse previousStatus = "Archived" Then
                    previousStatus = "Draft"
                End If
                
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("UPDATE Posts SET Status = @Status, ModifiedDate = @ModifiedDate, ModifiedBy = @ModifiedBy WHERE PostID = @PostID", conn)
                        cmd.Parameters.AddWithValue("@Status", previousStatus)
                        cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now)
                        
                        If Session("AdminID") IsNot Nothing Then
                            cmd.Parameters.AddWithValue("@ModifiedBy", Convert.ToInt32(Session("AdminID")))
                        Else
                            cmd.Parameters.AddWithValue("@ModifiedBy", DBNull.Value)
                        End If
                        
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                
                ' Log the restoration action
                LogArchiveAction(postId, "Restored from archive to " & previousStatus & " status")
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error restoring post: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Function GetOriginalPostStatus(ByVal postId As Integer) As String
            Dim originalStatus As String = "Draft"
            
            Try
                Using conn As New SqlConnection(ConnectionString)
                    ' Check the archive log for the original status before archiving
                    Dim query As String = "SELECT TOP 1 PreviousStatus FROM PostArchiveLog WHERE PostID = @PostID ORDER BY ActionDate DESC"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        conn.Open()
                        
                        Dim result As Object = cmd.ExecuteScalar()
                        If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                            originalStatus = result.ToString()
                        End If
                    End Using
                End Using
            Catch ex As Exception
                ' Just log the error, don't show to the user
                System.Diagnostics.Debug.WriteLine("Error getting original post status: " & ex.Message)
            End Try
            
            Return originalStatus
        End Function

        Private Sub DeletePost(ByVal postId As Integer)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("DELETE FROM Posts WHERE PostID = @PostID", conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                
                ' Log the deletion action
                LogArchiveAction(postId, "Permanently deleted from the system")
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error deleting post: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Sub LogArchiveAction(ByVal postId As Integer, ByVal action As String)
            Try
                ' Check if the PostArchiveLog table exists, create it if it doesn't
                EnsureArchiveLogTableExists()
                
                Using conn As New SqlConnection(ConnectionString)
                    Using cmd As New SqlCommand("INSERT INTO PostArchiveLog (PostID, AdminID, Action, ActionDate, AdminName) VALUES (@PostID, @AdminID, @Action, @ActionDate, @AdminName)", conn)
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
                System.Diagnostics.Debug.WriteLine("Error logging archive action: " & ex.Message)
            End Try
    End Sub

        Private Sub EnsureArchiveLogTableExists()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    conn.Open()
                    
                    ' Check if table exists
                    Dim checkTableCmd As New SqlCommand( _
                        "IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PostArchiveLog') " & _
                        "SELECT 1 ELSE SELECT 0", conn)
                    
                    Dim tableExists As Boolean = Convert.ToBoolean(checkTableCmd.ExecuteScalar())
                    
                    If Not tableExists Then
                        ' Create the table if it doesn't exist
                        Dim createTableCmd As New SqlCommand( _
                            "CREATE TABLE PostArchiveLog (" & _
                            "LogID INT PRIMARY KEY IDENTITY(1,1), " & _
                            "PostID INT NOT NULL, " & _
                            "AdminID INT, " & _
                            "Action NVARCHAR(100) NOT NULL, " & _
                            "ActionDate DATETIME NOT NULL, " & _
                            "AdminName NVARCHAR(100), " & _
                            "PreviousStatus NVARCHAR(50), " & _
                            "Notes NVARCHAR(500))", conn)
                        
                        createTableCmd.ExecuteNonQuery()
                    End If
                End Using
            Catch ex As Exception
                ' Just log the error, don't show to the user
                System.Diagnostics.Debug.WriteLine("Error ensuring archive log table: " & ex.Message)
            End Try
        End Sub
End Class
End Namespace 