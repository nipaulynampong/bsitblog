Imports System.Data.SqlClient
Imports System.IO
Imports System.Text.RegularExpressions
Imports BSIT.Module

Public Class CreatePosts
    Inherits System.Web.UI.Page
    
    ' Control declarations
    Protected WithEvents txtTitle As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtSlug As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtExcerpt As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtContent As System.Web.UI.WebControls.TextBox
    Protected WithEvents fileImage As System.Web.UI.WebControls.FileUpload
    Protected WithEvents ddlCategory As System.Web.UI.WebControls.DropDownList
    Protected WithEvents txtNewCategory As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddlParentCategory As System.Web.UI.WebControls.DropDownList
    Protected WithEvents ddlTags As System.Web.UI.WebControls.ListBox
    Protected WithEvents txtPublishDate As System.Web.UI.WebControls.TextBox
    Protected WithEvents ddlStatus As System.Web.UI.WebControls.DropDownList
    Protected WithEvents chkAllowComments As System.Web.UI.WebControls.CheckBox
    Protected WithEvents txtMetaTitle As System.Web.UI.WebControls.TextBox
    Protected WithEvents txtMetaDescription As System.Web.UI.WebControls.TextBox
    Protected WithEvents pnlAlert As System.Web.UI.WebControls.Panel
    Protected WithEvents litAlertMessage As System.Web.UI.WebControls.Literal
    Protected WithEvents btnSaveAsDraft As System.Web.UI.WebControls.Button
    Protected WithEvents btnSave As System.Web.UI.WebControls.Button
    Protected WithEvents btnPublish As System.Web.UI.WebControls.Button
    Protected WithEvents btnPreview As System.Web.UI.WebControls.Button
    Protected WithEvents btnCancel As System.Web.UI.WebControls.Button
    
    ' Use the Connection class from the BSIT.Module namespace
    Private conn As New BSIT.Module.Connection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadCategories()
            LoadTags()
            
            ' Set default publish date to current date/time
            txtPublishDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        End If
    End Sub

    Private Sub LoadCategories()
        Try
            ' Use the Connection class
            If conn.Query("SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName") Then
                If conn.DataCount > 0 Then
                    ddlCategory.Items.Clear()
                    ddlCategory.Items.Add(New ListItem("-- Select Category --", ""))
                    ddlParentCategory.Items.Clear()
                    ddlParentCategory.Items.Add(New ListItem("-- None --", ""))
                    
                    For Each row As DataRow In conn.Data.Tables(0).Rows
                        Dim categoryId As String = row("CategoryID").ToString()
                        Dim categoryName As String = row("CategoryName").ToString()
                        
                        ddlCategory.Items.Add(New ListItem(categoryName, categoryId))
                        ddlParentCategory.Items.Add(New ListItem(categoryName, categoryId))
                    Next
                End If
            End If
            
            ' Add "Create New Category" option
            ddlCategory.Items.Add(New ListItem("+ Create New Category", "custom"))
        Catch ex As Exception
            ShowAlert("Error loading categories: " & ex.Message, "danger")
        End Try
    End Sub
    
    Private Sub LoadTags()
        Try
            ' Use the Connection class
            If conn.Query("SELECT TagID, TagName FROM Tags ORDER BY TagName") Then
                If conn.DataCount > 0 Then
                    ddlTags.Items.Clear()
                    
                    For Each row As DataRow In conn.Data.Tables(0).Rows
                        Dim tagId As String = row("TagID").ToString()
                        Dim tagName As String = row("TagName").ToString()
                        
                        ddlTags.Items.Add(New ListItem(tagName, tagId))
                    Next
                End If
            End If
        Catch ex As Exception
            ShowAlert("Error loading tags: " & ex.Message, "danger")
        End Try
    End Sub
    
    Private Function GenerateSlug(title As String) As String
        ' Remove special characters
        Dim slug As String = Regex.Replace(title.ToLower(), "[^a-z0-9\s-]", "")
        ' Replace spaces with hyphens
        slug = Regex.Replace(slug, "\s+", "-")
        ' Remove duplicate hyphens
        slug = Regex.Replace(slug, "-+", "-")
        ' Trim hyphens from start and end
        slug = slug.Trim("-"c)
        Return slug
    End Function
    
    Private Function UploadFeaturedImage() As String
        Dim imageUrl As String = String.Empty
        
        If fileImage.HasFile Then
            Try
                ' Create uploads directory if it doesn't exist
                Dim uploadsDir As String = Server.MapPath("~/uploads/")
                If Not Directory.Exists(uploadsDir) Then
                    Directory.CreateDirectory(uploadsDir)
                End If
                
                ' Generate unique filename
                Dim fileExtension As String = Path.GetExtension(fileImage.FileName)
                Dim uniqueFileName As String = Guid.NewGuid().ToString() & fileExtension
                Dim filePath As String = Path.Combine(uploadsDir, uniqueFileName)
                
                ' Save file
                fileImage.SaveAs(filePath)
                
                ' Return relative URL for database
                imageUrl = "~/uploads/" & uniqueFileName
            Catch ex As Exception
                ShowAlert("Error uploading image: " & ex.Message, "danger")
            End Try
        End If
        
        Return imageUrl
    End Function
    
    Private Function GetOrCreateCategory() As Integer
        Dim categoryId As Integer = 0
        
        If ddlCategory.SelectedValue = "custom" AndAlso Not String.IsNullOrEmpty(txtNewCategory.Text) Then
            ' Create new category using the Connection class
            Try
                conn.AddParam("@CategoryName", txtNewCategory.Text.Trim())
                
                If String.IsNullOrEmpty(ddlParentCategory.SelectedValue) Then
                    conn.AddParam("@ParentCategoryID", DBNull.Value.ToString())
                Else
                    conn.AddParam("@ParentCategoryID", ddlParentCategory.SelectedValue)
                End If
                
                If conn.Query("INSERT INTO Categories (CategoryName, ParentCategoryID) VALUES (@CategoryName, @ParentCategoryID); SELECT SCOPE_IDENTITY();") Then
                    If conn.DataCount > 0 Then
                        Dim result As Object = conn.Data.Tables(0).Rows(0)(0)
                        If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                            categoryId = Convert.ToInt32(result)
                        End If
                    End If
                End If
            Catch ex As Exception
                ShowAlert("Error creating new category: " & ex.Message, "danger")
            End Try
        ElseIf Not String.IsNullOrEmpty(ddlCategory.SelectedValue) Then
            ' Use existing category
            categoryId = Convert.ToInt32(ddlCategory.SelectedValue)
        End If
        
        Return categoryId
    End Function
    
    Private Sub SaveTags(postId As Integer)
        If postId > 0 Then
            Try
                ' First delete existing tag associations for this post
                conn.AddParam("@PostID", postId.ToString())
                conn.Query("DELETE FROM PostTags WHERE PostID = @PostID")
                
                ' Then add new tag associations
                For Each item As ListItem In ddlTags.Items
                    If item.Selected Then
                        Dim tagId As Integer
                        
                        ' Check if it's an existing tag or new one
                        If item.Value.StartsWith("new_") Then
                            ' Create new tag
                            Dim tagName As String = item.Text
                            
                            conn.AddParam("@TagName", tagName)
                            If conn.Query("INSERT INTO Tags (TagName) VALUES (@TagName); SELECT SCOPE_IDENTITY();") Then
                                If conn.DataCount > 0 Then
                                    Dim result As Object = conn.Data.Tables(0).Rows(0)(0)
                                    If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                                        tagId = Convert.ToInt32(result)
                                    End If
                                End If
                            End If
                        Else
                            ' Use existing tag
                            tagId = Convert.ToInt32(item.Value)
                        End If
                        
                        ' Add association
                        conn.AddParam("@PostID", postId.ToString())
                        conn.AddParam("@TagID", tagId.ToString())
                        conn.Query("INSERT INTO PostTags (PostID, TagID) VALUES (@PostID, @TagID)")
                    End If
                Next
            Catch ex As Exception
                ShowAlert("Error saving tags: " & ex.Message, "danger")
            End Try
        End If
    End Sub
    
    Private Sub SavePostFixed(status As String)
        If Not IsValid Then
            LogDebugInfo("Form validation failed")
            Return
        End If
        
        Try
            LogDebugInfo("Starting SavePostFixed with status: " & status)
            
            ' Upload featured image if provided
            Dim imageUrl As String = UploadFeaturedImage()
            LogDebugInfo("Featured image URL: " & (If(String.IsNullOrEmpty(imageUrl), "None", imageUrl)))
            
            ' Generate slug if not provided
            Dim slug As String = txtSlug.Text.Trim()
            If String.IsNullOrEmpty(slug) Then
                slug = GenerateSlug(txtTitle.Text.Trim())
            End If
            LogDebugInfo("Post slug: " & slug)
            
            ' Get or create category
            Dim categoryId As Integer = GetOrCreateCategory()
            LogDebugInfo("Category ID: " & categoryId.ToString())
            
            ' Get category name from selected value
            Dim categoryName As String = "Uncategorized" ' Default value
            If categoryId > 0 Then
                If ddlCategory.SelectedValue = "custom" Then
                    ' Use the new category name
                    categoryName = txtNewCategory.Text.Trim()
                Else
                    ' Get the selected category name from the dropdown
                    categoryName = ddlCategory.SelectedItem.Text
                    ' If it's the default selection, set to Uncategorized
                    If categoryName = "-- Select Category --" Then
                        categoryName = "Uncategorized"
                    End If
                End If
            End If
            LogDebugInfo("Category Name: " & categoryName)
            
            ' Set publish date
            Dim publishDate As String = ""
            If status = "Published" Then
                If String.IsNullOrEmpty(txtPublishDate.Text.Trim()) Then
                    publishDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                Else
                    publishDate = DateTime.Parse(txtPublishDate.Text.Trim()).ToString("yyyy-MM-dd HH:mm:ss")
                End If
            End If
            LogDebugInfo("Publish date: " & (If(String.IsNullOrEmpty(publishDate), "NULL", publishDate)))
            
            ' Get current user ID (assuming you have user authentication)
            Dim authorId As Integer = 1 ' Default to admin if not available
            If Session("UserID") IsNot Nothing Then
                authorId = Convert.ToInt32(Session("UserID"))
            End If
            LogDebugInfo("Author ID: " & authorId.ToString())
            
            ' Generate meta title and description if not provided
            Dim metaTitle As String = txtMetaTitle.Text.Trim()
            If String.IsNullOrEmpty(metaTitle) Then
                metaTitle = txtTitle.Text.Trim()
            End If
            
            Dim metaDescription As String = txtMetaDescription.Text.Trim()
            If String.IsNullOrEmpty(metaDescription) AndAlso Not String.IsNullOrEmpty(txtExcerpt.Text.Trim()) Then
                metaDescription = txtExcerpt.Text.Trim()
            End If
            
            ' Clear any existing parameters
            conn.Parameters.Clear()
            
            ' Add all parameters with correct names
            conn.AddParam("@Title", txtTitle.Text.Trim())
            conn.AddParam("@Slug", slug)
            conn.AddParam("@Content", txtContent.Text)
            conn.AddParam("@Excerpt", If(String.IsNullOrEmpty(txtExcerpt.Text.Trim()), "", txtExcerpt.Text.Trim()))
            conn.AddParam("@Category", categoryName) ' Using Category, not CategoryID
            conn.AddParam("@AuthorID", authorId.ToString())
            conn.AddParam("@Status", status)
            conn.AddParam("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            conn.AddParam("@LastModifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            conn.AddParam("@Featuredlmage", If(String.IsNullOrEmpty(imageUrl), "", imageUrl)) ' Note the lowercase 'l'
            conn.AddParam("@AllowComments", If(chkAllowComments.Checked, "1", "0"))
            conn.AddParam("@MetaTitle", metaTitle)
            conn.AddParam("@MetaDescription", If(String.IsNullOrEmpty(metaDescription), "", metaDescription))
            conn.AddParam("@Views", "0") ' Initialize views to 0
            
            ' Build SQL query manually with correct column names
            Dim sqlQuery As String = "INSERT INTO Posts (Title, Slug, Content, Excerpt, Category, AuthorID, Status, " & _
                "CreatedDate, LastModifiedDate, Featuredlmage, AllowComments, MetaTitle, MetaDescription, Views"
            
            Dim valuesClause As String = " VALUES (@Title, @Slug, @Content, @Excerpt, @Category, @AuthorID, @Status, " & _
                "@CreatedDate, @LastModifiedDate, @Featuredlmage, @AllowComments, @MetaTitle, @MetaDescription, @Views"
            
            ' Add PublishDate parameter and column if available
            If Not String.IsNullOrEmpty(publishDate) Then
                sqlQuery &= ", PublishDate"
                valuesClause &= ", @PublishDate"
                conn.AddParam("@PublishDate", publishDate)
            End If
            
            ' Complete the query
            sqlQuery &= ")" & valuesClause & "); SELECT SCOPE_IDENTITY();"
            
            LogDebugInfo("Executing fixed SQL query: " & sqlQuery)
            
            If conn.Query(sqlQuery) Then
                LogDebugInfo("Query executed successfully")
                LogDebugInfo("DataCount after query: " & conn.DataCount)
                
                If conn.DataCount > 0 AndAlso conn.Data IsNot Nothing AndAlso conn.Data.Tables.Count > 0 Then
                    LogDebugInfo("Tables count: " & conn.Data.Tables.Count)
                    LogDebugInfo("First table row count: " & conn.Data.Tables(0).Rows.Count)
                    
                    If conn.Data.Tables(0).Rows.Count > 0 Then
                        Dim result As Object = conn.Data.Tables(0).Rows(0)(0)
                        LogDebugInfo("Result value: " & (If(result Is Nothing OrElse DBNull.Value.Equals(result), "NULL", result.ToString())))
                        
                        If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) AndAlso Not String.IsNullOrEmpty(result.ToString()) Then
                            Dim postId As Integer = Convert.ToInt32(result)
                            LogDebugInfo("Post ID: " & postId.ToString())
                            
                            ' If post ID is 0, it means the post was created but we couldn't get its ID
                            If postId = 0 Then
                                LogDebugInfo("Warning: Post ID is 0, attempting to find the post by slug")
                                ' Try to find the post by slug as a fallback
                                Dim findQuery As String = "SELECT PostID FROM Posts WHERE Slug = @Slug"
                                conn.Parameters.Clear()
                                conn.AddParam("@Slug", slug)
                                
                                If conn.Query(findQuery) AndAlso conn.DataCount > 0 AndAlso conn.Data.Tables(0).Rows.Count > 0 Then
                                    postId = Convert.ToInt32(conn.Data.Tables(0).Rows(0)(0))
                                    LogDebugInfo("Found post by slug, ID: " & postId.ToString())
                                Else
                                    LogDebugInfo("Could not find post by slug, using ID 0")
                                End If
                            End If
                            
                            ' Save tags
                            SaveTags(postId)
                            
                            ' Store newly created post ID in session for preview
                            Session("NewPostID") = postId
                            
                            ' Show success message and redirect
                            If status = "Published" Then
                                ShowAlert("Post published successfully!", "success")
                                LogDebugInfo("Post published successfully with ID: " & postId.ToString())
                            Else
                                ShowAlert("Post saved successfully!", "success")
                                LogDebugInfo("Post saved successfully with ID: " & postId.ToString())
                            End If
                            
                            ' Redirect after a short delay
                            Response.AddHeader("REFRESH", "2;URL=ManagePosts.aspx")
                        Else
                            LogDebugInfo("Error: No valid ID returned")
                            ShowAlert("Error saving post: No valid ID returned. The post may have been saved, but we couldn't get its ID.", "warning")
                            Response.AddHeader("REFRESH", "2;URL=ManagePosts.aspx")
                        End If
                    Else
                        LogDebugInfo("Error: Data table has no rows")
                        ShowAlert("Error saving post: Data table has no rows. The post may have been saved, but we couldn't get its ID.", "warning")
                        Response.AddHeader("REFRESH", "2;URL=ManagePosts.aspx")
                    End If
                Else
                    LogDebugInfo("Error: No data returned. DataCount=" & conn.DataCount)
                    ShowAlert("Error saving post: No data returned. The post may have been saved, but we couldn't get its ID.", "warning")
                    Response.AddHeader("REFRESH", "2;URL=ManagePosts.aspx")
                End If
            Else
                LogDebugInfo("Error executing SQL command")
                ShowAlert("Error executing SQL command", "danger")
            End If
        Catch ex As Exception
            LogDebugInfo("Exception in SavePostFixed: " & ex.Message & Environment.NewLine & ex.StackTrace)
            ShowAlert("Error saving post: " & ex.Message, "danger")
        End Try
    End Sub
    
    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        SavePostFixed(ddlStatus.SelectedValue)
    End Sub
    
    Protected Sub btnSaveAsDraft_Click(sender As Object, e As EventArgs)
        SavePostFixed("Draft")
    End Sub
    
    Protected Sub btnPublish_Click(sender As Object, e As EventArgs)
        SavePostFixed("Published")
    End Sub
    
    Protected Sub btnPreview_Click(sender As Object, e As EventArgs)
        Try
            LogDebugInfo("Preview button clicked")
            
            ' Store post data in session for preview
            Session("PreviewTitle") = txtTitle.Text.Trim()
            Session("PreviewContent") = txtContent.Text
            Session("PreviewExcerpt") = txtExcerpt.Text.Trim()
            
            ' If a featured image was uploaded, store it for preview
            If fileImage.HasFile Then
                ' Upload the image temporarily for preview
                Dim imageUrl As String = UploadFeaturedImage()
                Session("PreviewImageUrl") = imageUrl
                LogDebugInfo("Preview image URL: " & imageUrl)
            End If
            
            LogDebugInfo("Redirecting to preview page")
            
            ' Redirect to preview page
            Response.Redirect("~/Editor/EditorPanel/PreviewPost.aspx")
        Catch ex As Exception
            LogDebugInfo("Error in preview: " & ex.Message)
            ShowAlert("Error generating preview: " & ex.Message, "danger")
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        ' Redirect back to post list
        Response.Redirect("~/Editor/EditorPanel/ManagePosts.aspx")
    End Sub

    Private Sub ShowAlert(message As String, type As String)
        pnlAlert.Visible = True
        pnlAlert.CssClass = "alert alert-" & type
        litAlertMessage.Text = message
    End Sub

    ' Debug method to log information
    Private Sub LogDebugInfo(message As String)
        Try
            ' Create a log file in the App_Data directory
            Dim logPath As String = Server.MapPath("~/App_Data/debug_log.txt")
            Dim logDir As String = Path.GetDirectoryName(logPath)
            
            ' Create directory if it doesn't exist
            If Not Directory.Exists(logDir) Then
                Directory.CreateDirectory(logDir)
            End If
            
            ' Append to log file
            Using writer As New StreamWriter(logPath, True)
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " - " & message)
            End Using
        Catch ex As Exception
            ' Silently fail if logging fails
        End Try
    End Sub
End Class