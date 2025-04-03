Imports System.Data
Imports System.Data.SqlClient

Namespace BSIT
    Partial Public Class ViewPosts
    Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Redirect to login if not logged in
                If Session("AdminID") Is Nothing Then
                    Response.Redirect("~/AdminSide/AdminLogin.aspx")
                    Return
                End If

                ' Load categories for filter dropdown
                LoadCategories()
                
                ' Load posts
                LoadPosts()
            End If
        End Sub

        Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
            LoadPosts(txtSearch.Text.Trim(), ddlCategory.SelectedValue)
        End Sub

        Protected Sub ddlCategory_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            LoadPosts(txtSearch.Text.Trim(), ddlCategory.SelectedValue)
        End Sub

        Protected Sub btnReturnToManage_Click(ByVal sender As Object, ByVal e As EventArgs)
            Response.Redirect("ManagePosts.aspx")
        End Sub

        Protected Sub lvPosts_ItemCommand(ByVal sender As Object, ByVal e As ListViewCommandEventArgs)
            ' Validate command argument
            If e.CommandArgument Is Nothing OrElse String.IsNullOrEmpty(e.CommandArgument.ToString()) Then
                Return
            End If

            Dim postId As Integer
            If Not Integer.TryParse(e.CommandArgument.ToString(), postId) Then
                ' Invalid post ID
                Return
            End If

            If e.CommandName = "ViewPost" Then
                Response.Redirect("~/AdminSide/AdminPanel/ViewPostDetails.aspx?id=" & postId)
            End If
        End Sub

        ' Public function used by ASPX to get post image URL
        Public Function GetPostImageUrl(ByVal postId As Object) As String
            Dim defaultImageUrl As String = "../Content/images/post-placeholder.jpg"
            
            If postId Is Nothing Then
                Return defaultImageUrl
            End If
            
            Dim postIdInt As Integer
            If Not Integer.TryParse(postId.ToString(), postIdInt) Then
                Return defaultImageUrl
            End If
            
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT ImageUrl FROM Posts WHERE PostID = @PostID"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", postIdInt)
                        conn.Open()
                        
                        Dim result As Object = cmd.ExecuteScalar()
                        If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                            Dim imageUrl As String = result.ToString()
                            If Not String.IsNullOrEmpty(imageUrl) Then
                                ' Check if the URL starts with http:// or https:// (external image)
                                If imageUrl.StartsWith("http://") OrElse imageUrl.StartsWith("https://") Then
                                    Return imageUrl
                                Else
                                    ' Handle relative URLs
                                    If Not imageUrl.StartsWith("~/") AndAlso Not imageUrl.StartsWith("/") Then
                                        imageUrl = "~/" & imageUrl
                                    End If
                                    Return ResolveUrl(imageUrl)
                                End If
                            End If
                        End If
                    End Using
                End Using
            Catch ex As Exception
                ' Just log the error, don't show to the user
                System.Diagnostics.Debug.WriteLine("Error getting post image URL: " & ex.Message)
            End Try
            
            Return defaultImageUrl
        End Function

        ' Private methods for database operations
        Private Sub LoadCategories()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT DISTINCT Category FROM Posts WHERE Status = 'Published' AND Category IS NOT NULL AND Category <> '' ORDER BY Category"
                    
                    Using cmd As New SqlCommand(query, conn)
                        conn.Open()
                        
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            While reader.Read()
                                ddlCategory.Items.Add(New ListItem(reader("Category").ToString(), reader("Category").ToString()))
                            End While
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Just log the error, don't show to the user
                System.Diagnostics.Debug.WriteLine("Error loading categories: " & ex.Message)
            End Try
    End Sub

        Private Sub LoadPosts(Optional ByVal searchTerm As String = "", Optional ByVal category As String = "")
            Try
                Using conn As New SqlConnection(ConnectionString)
                    ' Modify the query to handle the excerpt more safely
                    Dim query As String = "SELECT p.PostID, p.Title, p.ImageUrl, " & _
                                      "CASE " & _
                                      "   WHEN p.Excerpt IS NOT NULL AND LEN(p.Excerpt) > 0 THEN p.Excerpt " & _
                                      "   WHEN p.Content IS NOT NULL THEN " & _
                                      "       CASE WHEN LEN(p.Content) > 300 THEN LEFT(p.Content, 300) + '...' ELSE p.Content END " & _
                                      "   ELSE 'No content available' " & _
                                      "END AS Excerpt, " & _
                                      "ISNULL(u.fullname, 'Unknown') AS Author, " & _
                                      "ISNULL(p.Category, 'Uncategorized') AS Category, " & _
                                      "p.Status, p.CreatedDate, ISNULL(p.Views, 0) AS Views " & _
                                      "FROM Posts p " & _
                                      "LEFT JOIN users u ON p.AuthorID = u.UserID " & _
                                      "WHERE p.Status = 'Published'"

                    ' Add search filter if provided
                    If Not String.IsNullOrEmpty(searchTerm) Then
                        query += " AND (p.Title LIKE @SearchTerm OR p.Content LIKE @SearchTerm OR p.Excerpt LIKE @SearchTerm)"
                    End If

                    ' Add category filter if provided
                    If Not String.IsNullOrEmpty(category) Then
                        query += " AND p.Category = @Category"
                    End If

                    ' Add ORDER BY to sort results
                    query += " ORDER BY p.CreatedDate DESC"

                    Using cmd As New SqlCommand(query, conn)
                        ' Add parameters if needed
                        If Not String.IsNullOrEmpty(searchTerm) Then
                            cmd.Parameters.AddWithValue("@SearchTerm", "%" & searchTerm & "%")
                        End If

                        If Not String.IsNullOrEmpty(category) Then
                            cmd.Parameters.AddWithValue("@Category", category)
                        End If

                        ' Create data adapter and dataset
                        Using adapter As New SqlDataAdapter(cmd)
                            Dim ds As New DataSet()
                            conn.Open()
                            adapter.Fill(ds)

                            ' Check if we have results
                            If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                                ' Process data if needed
                                For Each row As DataRow In ds.Tables(0).Rows
                                    Try
                                        ' Clean up excerpt (remove HTML tags for cleaner display)
                                        If row("Excerpt") IsNot DBNull.Value AndAlso Not String.IsNullOrEmpty(row("Excerpt").ToString()) Then
                                            row("Excerpt") = Server.HtmlDecode(System.Text.RegularExpressions.Regex.Replace(row("Excerpt").ToString(), "<.*?>", ""))
                                        Else
                                            row("Excerpt") = "No content available"
                                        End If
                                    Catch ex As Exception
                                        ' If there's any issue processing an excerpt, set a default value
                                        row("Excerpt") = "Content preview not available"
                                        System.Diagnostics.Debug.WriteLine("Error processing excerpt for post " & row("PostID") & ": " & ex.Message)
                                    End Try
                                Next
                            End If

                            lvPosts.DataSource = ds
                            lvPosts.DataBind()
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Show detailed error message
                Dim errorMsg As String = "<div class='alert alert-danger'>Error loading posts: " & ex.Message
                If ex.InnerException IsNot Nothing Then
                    errorMsg &= "<br>Details: " & ex.InnerException.Message
                End If
                errorMsg &= "<br>Stack Trace: " & ex.StackTrace
                errorMsg &= "</div>"
                
                Response.Write(errorMsg)
                System.Diagnostics.Debug.WriteLine("Error in LoadPosts: " & ex.ToString())
            End Try
        End Sub
End Class
End Namespace