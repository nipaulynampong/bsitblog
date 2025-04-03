Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Text.RegularExpressions

Namespace BSIT
    Partial Public Class EditPost
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"
        Private _postId As Integer = 0
        Private _currentImagePath As String = String.Empty

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
                        LoadTags()
                        LoadParentCategories()
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
                            
                            ' Add custom category option
                            ddlCategory.Items.Add(New ListItem("+ Add New Category", "custom"))
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error loading categories: " & ex.Message)
            End Try
        End Sub
        
        Private Sub LoadParentCategories()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT CategoryID, CategoryName FROM Categories WHERE ParentCategoryID IS NULL ORDER BY CategoryName"
                    Using cmd As New SqlCommand(query, conn)
                        conn.Open()
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            ddlParentCategory.Items.Clear()
                            ddlParentCategory.Items.Add(New ListItem("-- None --", ""))
                            
                            While reader.Read()
                                ddlParentCategory.Items.Add(New ListItem(
                                    reader("CategoryName").ToString(),
                                    reader("CategoryID").ToString()))
                            End While
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error loading parent categories: " & ex.Message)
            End Try
        End Sub
        
        Private Sub LoadTags()
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT TagID, TagName FROM Tags ORDER BY TagName"
                    Using cmd As New SqlCommand(query, conn)
                        conn.Open()
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            ddlTags.Items.Clear()
                            
                            While reader.Read()
                                ddlTags.Items.Add(New ListItem(
                                    reader("TagName").ToString(),
                                    reader("TagID").ToString()))
                            End While
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error loading tags: " & ex.Message)
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
                                
                                ' Set slug if available
                                If Not IsDBNull(reader("Slug")) Then
                                    txtSlug.Text = reader("Slug").ToString()
                                End If
                                
                                ' Set category if available
                                If Not IsDBNull(reader("CategoryID")) Then
                                    ddlCategory.SelectedValue = reader("CategoryID").ToString()
                                End If
                                
                                ' Set excerpt if available
                                If Not IsDBNull(reader("Excerpt")) Then
                                    txtExcerpt.Text = reader("Excerpt").ToString()
                                End If
                                
                                ' Set status
                                If Not IsDBNull(reader("Status")) Then
                                    Dim status As String = reader("Status").ToString()
                                    If ddlStatus.Items.FindByValue(status) IsNot Nothing Then
                                        ddlStatus.SelectedValue = status
                                    End If
                                End If
                                
                                ' Set publish date if available
                                If Not IsDBNull(reader("PublishDate")) Then
                                    txtPublishDate.Text = Convert.ToDateTime(reader("PublishDate")).ToString("yyyy-MM-dd HH:mm")
                                End If
                                
                                ' Set meta title if available
                                If Not IsDBNull(reader("MetaTitle")) Then
                                    txtMetaTitle.Text = reader("MetaTitle").ToString()
                                End If
                                
                                ' Set meta description if available
                                If Not IsDBNull(reader("MetaDescription")) Then
                                    txtMetaDescription.Text = reader("MetaDescription").ToString()
                                End If
                                
                                ' Set allow comments if available
                                If Not IsDBNull(reader("AllowComments")) Then
                                    chkAllowComments.Checked = Convert.ToBoolean(reader("AllowComments"))
                                End If
                                
                                ' Handle featured image
                                If Not IsDBNull(reader("Featuredlmage")) Then
                                    _currentImagePath = reader("Featuredlmage").ToString()
                                    If Not String.IsNullOrEmpty(_currentImagePath) Then
                                        currentImageContainer.Visible = True
                                        imgCurrentImage.ImageUrl = _currentImagePath
                                    End If
                                End If
                            Else
                                ShowError("Post not found with the specified ID.")
                                btnSave.Enabled = False
                                btnPublish.Enabled = False
                            End If
                        End Using
                    End Using
                    
                    ' Load tags for this post
                    LoadPostTags(postId)
                End Using
            Catch ex As Exception
                ShowError("Error loading post data: " & ex.Message)
            End Try
        End Sub
        
        Private Sub LoadPostTags(ByVal postId As Integer)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "SELECT t.TagID FROM Tags t " & _
                                         "INNER JOIN PostTags pt ON t.TagID = pt.TagID " & _
                                         "WHERE pt.PostID = @PostID"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        conn.Open()
                        
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            ' Create a list to hold selected tag IDs
                            Dim selectedTagIds As New List(Of String)
                            
                            While reader.Read()
                                selectedTagIds.Add(reader("TagID").ToString())
                            End While
                            
                            ' Set selected items in the listbox
                            For Each item As ListItem In ddlTags.Items
                                If selectedTagIds.Contains(item.Value) Then
                                    item.Selected = True
                                End If
                            Next
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Log error but don't show to user since it's not critical
                System.Diagnostics.Debug.WriteLine("Error loading post tags: " & ex.Message)
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
        
        Protected Sub btnRemoveImage_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Remove the image from post
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "UPDATE Posts SET Featuredlmage = NULL WHERE PostID = @PostID"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@PostID", _postId)
                        conn.Open()
                        
                        cmd.ExecuteNonQuery()
                        
                        ' Hide image container
                        currentImageContainer.Visible = False
                        _currentImagePath = String.Empty
                        
                        ShowSuccess("Featured image removed successfully.")
                    End Using
                End Using
            Catch ex As Exception
                ShowError("Error removing featured image: " & ex.Message)
            End Try
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
                
                ' Handle custom category if selected
                Dim categoryId As String = ddlCategory.SelectedValue
                Dim categoryName As String = String.Empty
                
                If categoryId = "custom" AndAlso Not String.IsNullOrWhiteSpace(txtNewCategory.Text) Then
                    ' Create new category
                    categoryId = CreateNewCategory(txtNewCategory.Text, ddlParentCategory.SelectedValue)
                    categoryName = txtNewCategory.Text
                Else
                    ' Get category name from dropdown
                    categoryName = ddlCategory.SelectedItem.Text
                    If categoryName = "-- Select Category --" Then
                        categoryName = "Uncategorized"
                    End If
                End If
                
                ' Handle image upload if present
                Dim imageUrl As String = _currentImagePath
                If fileImage.HasFile Then
                    imageUrl = UploadImage()
                End If
                
                ' Generate slug if empty
                Dim slug As String = txtSlug.Text.Trim()
                If String.IsNullOrWhiteSpace(slug) Then
                    slug = GenerateSlug(txtTitle.Text)
                End If
                
                ' Ensure unique slug
                slug = EnsureUniqueSlug(slug, _postId)
                
                ' Set publish date
                Dim publishDate As Object = DBNull.Value
                If Not String.IsNullOrWhiteSpace(txtPublishDate.Text) Then
                    publishDate = Convert.ToDateTime(txtPublishDate.Text)
                ElseIf ddlStatus.SelectedValue = "Published" Then
                    publishDate = DateTime.Now
                End If

                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "UPDATE Posts SET " & _
                                         "Title = @Title, " & _
                                         "Content = @Content, " & _
                                         "Excerpt = @Excerpt, " & _
                                         "Category = @Category, " & _
                                         "Status = @Status, " & _
                                         "Slug = @Slug, " & _
                                         "MetaTitle = @MetaTitle, " & _
                                         "MetaDescription = @MetaDescription, " & _
                                         "PublishDate = @PublishDate, " & _
                                         "AllowComments = @AllowComments, " & _
                                         "LastModifiedDate = @ModifiedDate "
                    
                    ' Only update featured image if we have one
                    If Not String.IsNullOrEmpty(imageUrl) Then
                        query &= ", Featuredlmage = @Featuredlmage "
                    End If
                    
                    query &= "WHERE PostID = @PostID"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim())
                        cmd.Parameters.AddWithValue("@Content", txtContent.Text)
                        cmd.Parameters.AddWithValue("@Excerpt", txtExcerpt.Text.Trim())
                        cmd.Parameters.AddWithValue("@Category", categoryName)
                        cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)
                        cmd.Parameters.AddWithValue("@Slug", slug)
                        cmd.Parameters.AddWithValue("@MetaTitle", If(String.IsNullOrWhiteSpace(txtMetaTitle.Text), txtTitle.Text.Trim(), txtMetaTitle.Text.Trim()))
                        cmd.Parameters.AddWithValue("@MetaDescription", txtMetaDescription.Text.Trim())
                        cmd.Parameters.AddWithValue("@PublishDate", publishDate)
                        cmd.Parameters.AddWithValue("@AllowComments", chkAllowComments.Checked)
                        cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now)
                        
                        If Not String.IsNullOrEmpty(imageUrl) Then
                            cmd.Parameters.AddWithValue("@Featuredlmage", imageUrl)
                        End If
                        
                        cmd.Parameters.AddWithValue("@PostID", _postId)
                        
                        conn.Open()
                        Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                        
                        If rowsAffected > 0 Then
                            ' Save tags
                            SavePostTags(_postId)
                            
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
        
        Private Function UploadImage() As String
            Dim imageUrl As String = String.Empty
            
            Try
                ' Ensure uploads directory exists
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
                
                ' Return virtual path
                imageUrl = "~/uploads/" & uniqueFileName
                
                Return imageUrl
            Catch ex As Exception
                ShowError("Error uploading image: " & ex.Message)
                Return String.Empty
            End Try
        End Function
        
        Private Function CreateNewCategory(ByVal categoryName As String, ByVal parentCategoryId As String) As String
            Dim newCategoryId As String = String.Empty
            
            Try
                Using conn As New SqlConnection(ConnectionString)
                    ' Generate slug from category name
                    Dim slug As String = GenerateSlug(categoryName)
                    
                    ' Check if parent ID is empty
                    Dim parentIdParam As Object = DBNull.Value
                    If Not String.IsNullOrEmpty(parentCategoryId) Then
                        parentIdParam = Convert.ToInt32(parentCategoryId)
                    End If
                    
                    Dim query As String = "INSERT INTO Categories (CategoryName, Slug, ParentCategoryID, CreatedDate) " & _
                                         "VALUES (@CategoryName, @Slug, @ParentCategoryID, @CreatedDate); " & _
                                         "SELECT SCOPE_IDENTITY();"
                    
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@CategoryName", categoryName.Trim())
                        cmd.Parameters.AddWithValue("@Slug", slug)
                        cmd.Parameters.AddWithValue("@ParentCategoryID", parentIdParam)
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now)
                        
                        conn.Open()
                        newCategoryId = cmd.ExecuteScalar().ToString()
                    End Using
                End Using
                
                Return newCategoryId
            Catch ex As Exception
                ShowError("Error creating new category: " & ex.Message)
                Return String.Empty
            End Try
        End Function
        
        Private Sub SavePostTags(ByVal postId As Integer)
            Try
                Using conn As New SqlConnection(ConnectionString)
                    ' First delete existing tag associations
                    Dim deleteQuery As String = "DELETE FROM PostTags WHERE PostID = @PostID"
                    
                    Using cmd As New SqlCommand(deleteQuery, conn)
                        cmd.Parameters.AddWithValue("@PostID", postId)
                        conn.Open()
                        cmd.ExecuteNonQuery()
                        
                        ' Get selected tags
                        Dim selectedTags As New List(Of String)
                        
                        For Each item As ListItem In ddlTags.Items
                            If item.Selected Then
                                selectedTags.Add(item.Value)
                            End If
                        Next
                        
                        ' Process each selected tag
                        For Each tagId As String In selectedTags
                            ' Check if it's a new tag (not numeric)
                            If Not IsNumeric(tagId) Then
                                ' Create new tag
                                tagId = CreateNewTag(tagId)
                            End If
                            
                            If Not String.IsNullOrEmpty(tagId) Then
                                ' Insert tag association
                                Dim insertQuery As String = "INSERT INTO PostTags (PostID, TagID) VALUES (@PostID, @TagID)"
                                
                                Using insertCmd As New SqlCommand(insertQuery, conn)
                                    insertCmd.Parameters.AddWithValue("@PostID", postId)
                                    insertCmd.Parameters.AddWithValue("@TagID", Convert.ToInt32(tagId))
                                    insertCmd.ExecuteNonQuery()
                                End Using
                            End If
                        Next
                    End Using
                End Using
            Catch ex As Exception
                ' Log error but don't show to user since it's not critical
                System.Diagnostics.Debug.WriteLine("Error saving post tags: " & ex.Message)
            End Try
        End Sub
        
        Private Function CreateNewTag(ByVal tagName As String) As String
            Dim newTagId As String = String.Empty
            
            Try
                Using conn As New SqlConnection(ConnectionString)
                    ' Generate slug from tag name
                    Dim slug As String = GenerateSlug(tagName)
                    
                    ' Check if tag already exists
                    Dim checkQuery As String = "SELECT TagID FROM Tags WHERE TagName = @TagName"
                    
                    Using checkCmd As New SqlCommand(checkQuery, conn)
                        checkCmd.Parameters.AddWithValue("@TagName", tagName.Trim())
                        
                        If conn.State <> ConnectionState.Open Then
                            conn.Open()
                        End If
                        
                        Dim existingTagId As Object = checkCmd.ExecuteScalar()
                        
                        If existingTagId IsNot Nothing Then
                            ' Tag already exists
                            newTagId = existingTagId.ToString()
                        Else
                            ' Insert new tag
                            Dim insertQuery As String = "INSERT INTO Tags (TagName, Slug) VALUES (@TagName, @Slug); " & _
                                                      "SELECT SCOPE_IDENTITY();"
                            
                            Using insertCmd As New SqlCommand(insertQuery, conn)
                                insertCmd.Parameters.AddWithValue("@TagName", tagName.Trim())
                                insertCmd.Parameters.AddWithValue("@Slug", slug)
                                
                                newTagId = insertCmd.ExecuteScalar().ToString()
                            End Using
                        End If
                    End Using
                End Using
                
                Return newTagId
            Catch ex As Exception
                ' Log error but don't show to user since it's not critical
                System.Diagnostics.Debug.WriteLine("Error creating new tag: " & ex.Message)
                Return String.Empty
            End Try
        End Function
        
        Private Function GenerateSlug(ByVal text As String) As String
            ' Remove accents
            Dim normalizedString As String = text.Normalize(System.Text.NormalizationForm.FormD)
            Dim stringBuilder As New System.Text.StringBuilder()
            
            For Each c As Char In normalizedString
                If System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) <> System.Globalization.UnicodeCategory.NonSpacingMark Then
                    stringBuilder.Append(c)
                End If
            Next
            
            Dim slug As String = stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC)
            
            ' Remove special characters
            slug = Regex.Replace(slug, "[^a-zA-Z0-9\s-]", "")
            
            ' Convert multiple spaces into one
            slug = Regex.Replace(slug, "\s+", " ").Trim()
            
            ' Cut and trim
            slug = If(slug.Length <= 100, slug, slug.Substring(0, 100)).Trim()
            
            ' Replace spaces with hyphens
            slug = Regex.Replace(slug, "\s", "-").ToLower()
            
            Return slug
        End Function
        
        Private Function EnsureUniqueSlug(ByVal slug As String, ByVal postId As Integer) As String
            Dim uniqueSlug As String = slug
            Dim counter As Integer = 1
            
            Try
                Using conn As New SqlConnection(ConnectionString)
                    conn.Open()
                    
                    Dim duplicateExists As Boolean = True
                    
                    While duplicateExists
                        Dim query As String = "SELECT COUNT(*) FROM Posts WHERE Slug = @Slug AND PostID != @PostID"
                        
                        Using cmd As New SqlCommand(query, conn)
                            cmd.Parameters.AddWithValue("@Slug", uniqueSlug)
                            cmd.Parameters.AddWithValue("@PostID", postId)
                            
                            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                            
                            If count > 0 Then
                                ' Duplicate found, append counter to slug
                                uniqueSlug = slug & "-" & counter.ToString()
                                counter += 1
                            Else
                                ' No duplicate found
                                duplicateExists = False
                            End If
                        End Using
                    End While
                End Using
                
                Return uniqueSlug
            Catch ex As Exception
                ' Log error but don't show to user since it's not critical
                System.Diagnostics.Debug.WriteLine("Error checking unique slug: " & ex.Message)
                Return slug & "-" & Guid.NewGuid().ToString().Substring(0, 8)
            End Try
        End Function

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
        
        Private Sub ShowSuccess(ByVal message As String)
            pnlAlert.CssClass = "alert alert-success"
            litAlertMessage.Text = message
            pnlAlert.Visible = True
        End Sub

        Private Sub ShowError(ByVal message As String)
            pnlAlert.CssClass = "alert alert-danger"
            litAlertMessage.Text = message
            pnlAlert.Visible = True
        End Sub
    End Class
End Namespace 