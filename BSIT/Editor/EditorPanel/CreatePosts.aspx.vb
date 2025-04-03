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
    
    Protected Sub btnSave_Click(sender As Object, e As EventArgs)
        SavePost(ddlStatus.SelectedValue)
    End Sub
    
    Protected Sub btnSaveAsDraft_Click(sender As Object, e As EventArgs)
        SavePost("Draft")
    End Sub
    
    Protected Sub btnPublish_Click(sender As Object, e As EventArgs)
        SavePost("Published")
    End Sub
    
    Private Sub SavePost(status As String)
        If Not IsValid Then
            Return
        End If
        
        Try
            ' Upload featured image if provided
            Dim imageUrl As String = UploadFeaturedImage()
            
            ' Generate slug if not provided
            Dim slug As String = txtSlug.Text.Trim()
            If String.IsNullOrEmpty(slug) Then
                slug = GenerateSlug(txtTitle.Text.Trim())
            End If
            
            ' Get or create category
            Dim categoryId As Integer = GetOrCreateCategory()
            
            ' Set publish date
            Dim publishDate As String = "NULL"
            If status = "Published" Then
                If String.IsNullOrEmpty(txtPublishDate.Text.Trim()) Then
                    publishDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                Else
                    publishDate = DateTime.Parse(txtPublishDate.Text.Trim()).ToString("yyyy-MM-dd HH:mm:ss")
                End If
            End If
            
            ' Get current user ID (assuming you have user authentication)
            Dim authorId As Integer = 1 ' Default to admin if not available
            If Session("UserID") IsNot Nothing Then
                authorId = Convert.ToInt32(Session("UserID"))
            End If
            
            ' Generate meta title and description if not provided
            Dim metaTitle As String = txtMetaTitle.Text.Trim()
            If String.IsNullOrEmpty(metaTitle) Then
                metaTitle = txtTitle.Text.Trim()
            End If
            
            Dim metaDescription As String = txtMetaDescription.Text.Trim()
            If String.IsNullOrEmpty(metaDescription) AndAlso Not String.IsNullOrEmpty(txtExcerpt.Text.Trim()) Then
                metaDescription = txtExcerpt.Text.Trim()
            End If
            
            ' Use Connection class for insert
            conn.AddParam("@Title", txtTitle.Text.Trim())
            conn.AddParam("@Slug", slug)
            conn.AddParam("@Content", txtContent.Text)
            
            If String.IsNullOrEmpty(txtExcerpt.Text.Trim()) Then
                conn.AddParam("@Excerpt", DBNull.Value.ToString())
            Else
                conn.AddParam("@Excerpt", txtExcerpt.Text.Trim())
            End If
            
            If categoryId > 0 Then
                conn.AddParam("@CategoryID", categoryId.ToString())
            Else
                conn.AddParam("@CategoryID", DBNull.Value.ToString())
            End If
            
            conn.AddParam("@AuthorID", authorId.ToString())
            conn.AddParam("@Status", status)
            conn.AddParam("@PublishedDate", publishDate)
            conn.AddParam("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            conn.AddParam("@LastModifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            
            If String.IsNullOrEmpty(imageUrl) Then
                conn.AddParam("@FeaturedImage", DBNull.Value.ToString())
            Else
                conn.AddParam("@FeaturedImage", imageUrl)
            End If
            
            conn.AddParam("@AllowComments", If(chkAllowComments.Checked, "1", "0"))
            conn.AddParam("@MetaTitle", metaTitle)
            
            If String.IsNullOrEmpty(metaDescription) Then
                conn.AddParam("@MetaDescription", DBNull.Value.ToString())
            Else
                conn.AddParam("@MetaDescription", metaDescription)
            End If
            
            ' Execute the query
            If conn.Query("INSERT INTO Posts (Title, Slug, Content, Excerpt, CategoryID, AuthorID, Status, PublishedDate, " & _
                         "CreatedDate, LastModifiedDate, FeaturedImage, AllowComments, MetaTitle, MetaDescription) " & _
                         "VALUES (@Title, @Slug, @Content, @Excerpt, @CategoryID, @AuthorID, @Status, @PublishedDate, " & _
                         "@CreatedDate, @LastModifiedDate, @FeaturedImage, @AllowComments, @MetaTitle, @MetaDescription); " & _
                         "SELECT SCOPE_IDENTITY();") Then
                
                If conn.DataCount > 0 Then
                    Dim result As Object = conn.Data.Tables(0).Rows(0)(0)
                    If result IsNot Nothing AndAlso Not DBNull.Value.Equals(result) Then
                        Dim postId As Integer = Convert.ToInt32(result)
                        
                        ' Save tags
                        SaveTags(postId)
                        
                        ' Show success message and redirect
                        If status = "Published" Then
                            ShowAlert("Post published successfully!", "success")
                        Else
                            ShowAlert("Post saved successfully!", "success")
                        End If
                        
                        ' Redirect after a short delay
                        Response.AddHeader("REFRESH", "2;URL=ManagePosts.aspx")
                    End If
                End If
            End If
        Catch ex As Exception
            ShowAlert("Error saving post: " & ex.Message, "danger")
        End Try
    End Sub
    
    Protected Sub btnPreview_Click(sender As Object, e As EventArgs)
        ' Store post data in session for preview
        Session("PreviewTitle") = txtTitle.Text.Trim()
        Session("PreviewContent") = txtContent.Text
        Session("PreviewExcerpt") = txtExcerpt.Text.Trim()
        
        ' Redirect to preview page
        Response.Redirect("~/Editor/EditorPanel/PreviewPost.aspx")
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
End Class