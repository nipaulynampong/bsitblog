Imports System.Data
Imports System.Data.SqlClient

Namespace BSIT
    Partial Public Class ViewFeedback
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"
        Private _postId As Integer = 0

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Redirect to login if not logged in
                If Session("EditorID") Is Nothing Then
                    Response.Redirect("~/Editor/EditorLogin.aspx")
                    Return
                End If

                ' Check if post ID is provided
                If Not Request.QueryString("id") Is Nothing AndAlso Integer.TryParse(Request.QueryString("id"), _postId) Then
                    ' Verify if post belongs to current editor
                    If Not PostBelongsToCurrentEditor(_postId) Then
                        Response.Write("<div class='alert alert-danger'>You don't have permission to view this post's feedback.</div>")
                        Return
                    End If

                    LoadPostDetails()
                    LoadFeedback()
                Else
                    Response.Redirect("ManagePosts.aspx")
                End If
            End If
        End Sub

        Protected Sub btnBackToList_Click(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Protected Sub btnEditPost_Click(ByVal sender As Object, ByVal e As EventArgs)
            If _postId > 0 Then
                Response.Redirect("CreatePosts.aspx?id=" & _postId)
            Else
                Response.Redirect("ManagePosts.aspx")
            End If
        End Sub

        Private Sub LoadPostDetails()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT Title, CreatedDate, SubmissionDate " & _
                                         "FROM Posts " & _
                                         "WHERE PostID = @PostID AND AuthorID = @EditorID"

                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", _postId)
                        cmd.Parameters.AddWithValue("@EditorID", Session("EditorID"))
                        
                        conn.Open()
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                lblPostTitle.Text = reader("Title").ToString()
                                
                                ' Use submission date if available, otherwise use created date
                                Dim submissionDate As DateTime
                                If Not reader.IsDBNull(reader.GetOrdinal("SubmissionDate")) Then
                                    submissionDate = Convert.ToDateTime(reader("SubmissionDate"))
                                Else
                                    submissionDate = Convert.ToDateTime(reader("CreatedDate"))
                                End If
                                
                                lblSubmissionDate.Text = submissionDate.ToString("MMMM dd, yyyy hh:mm tt")
                            Else
                                ' Post not found or doesn't belong to editor
                                Response.Redirect("ManagePosts.aspx")
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error loading post details: " & ex.Message & "</div>")
            End Try
        End Sub

        Private Sub LoadFeedback()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    ' Query the PostApprovalLog table to get rejection feedback
                    Dim query As String = "SELECT TOP 1 Action, Feedback, ActionDate, " & _
                                         "ISNULL(AdminName, 'System') AS AdminName " & _
                                         "FROM PostApprovalLog " & _
                                         "WHERE PostID = @PostID AND Action = 'Rejected' " & _
                                         "ORDER BY ActionDate DESC"

                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", _postId)
                        
                        conn.Open()
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                Dim feedback As String = reader("Feedback").ToString()
                                
                                ' Set a default message if no specific feedback was provided
                                If String.IsNullOrEmpty(feedback) Then
                                    feedback = "No specific feedback was provided by the reviewer. Please review your post for compliance with our content guidelines and make necessary improvements before resubmitting."
                                End If
                                
                                lblFeedbackContent.Text = feedback
                                lblReviewerName.Text = reader("AdminName").ToString()
                                lblFeedbackDate.Text = Convert.ToDateTime(reader("ActionDate")).ToString("MMMM dd, yyyy hh:mm tt")
                            Else
                                ' No feedback found, set default message
                                lblFeedbackContent.Text = "No specific feedback was recorded. Please review your post for compliance with our content guidelines."
                                lblReviewerName.Text = "System"
                                lblFeedbackDate.Text = DateTime.Now.ToString("MMMM dd, yyyy")
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Response.Write("<div class='alert alert-danger'>Error loading feedback: " & ex.Message & "</div>")
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