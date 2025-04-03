Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Namespace BSIT
Public Class Settings
    Inherits System.Web.UI.Page

        ' Control declarations
        Protected WithEvents txtBlogTitle As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtBlogDescription As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtAdminEmail As System.Web.UI.WebControls.TextBox
        Protected WithEvents ddlPostsPerPage As System.Web.UI.WebControls.DropDownList
        Protected WithEvents btnSaveGeneral As System.Web.UI.WebControls.Button
        Protected WithEvents txtNewCategory As System.Web.UI.WebControls.TextBox
        Protected WithEvents btnAddCategory As System.Web.UI.WebControls.Button
        Protected WithEvents rptCategories As System.Web.UI.WebControls.Repeater
        Protected WithEvents chkAllowRegistration As System.Web.UI.WebControls.CheckBox
        Protected WithEvents chkEmailVerification As System.Web.UI.WebControls.CheckBox
        Protected WithEvents ddlDefaultUserRole As System.Web.UI.WebControls.DropDownList
        Protected WithEvents btnSaveUserSettings As System.Web.UI.WebControls.Button
        Protected WithEvents chkAllowComments As System.Web.UI.WebControls.CheckBox
        Protected WithEvents chkModerateComments As System.Web.UI.WebControls.CheckBox
        Protected WithEvents chkAllowGuestComments As System.Web.UI.WebControls.CheckBox
        Protected WithEvents txtCommentsPerPage As System.Web.UI.WebControls.TextBox
        Protected WithEvents btnSaveCommentSettings As System.Web.UI.WebControls.Button
        Protected WithEvents btnCreateBackup As System.Web.UI.WebControls.Button
        Protected WithEvents fileBackupRestore As System.Web.UI.WebControls.FileUpload
        Protected WithEvents btnRestoreBackup As System.Web.UI.WebControls.Button
        Protected WithEvents gvBackupHistory As System.Web.UI.WebControls.GridView
        Protected WithEvents pnlSuccess As System.Web.UI.WebControls.Panel
        Protected WithEvents pnlError As System.Web.UI.WebControls.Panel
        Protected WithEvents litSuccessMessage As System.Web.UI.WebControls.Literal
        Protected WithEvents litErrorMessage As System.Web.UI.WebControls.Literal

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Check if user is logged in and is admin
                If Session("UserID") Is Nothing Then
                    Response.Redirect("~/Login.aspx")
                End If

                ' Load settings from database
                LoadGeneralSettings()
                LoadCategories()
                LoadUserSettings()
                LoadCommentSettings()
                LoadBackupHistory()
            End If
        End Sub

        #Region "General Settings"
        Private Sub LoadGeneralSettings()
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim cmd As New SqlCommand("SELECT SettingName, SettingValue FROM BlogSettings WHERE SettingCategory = 'General'", connection)
                Dim reader As SqlDataReader = cmd.ExecuteReader()

                While reader.Read()
                    Dim settingName As String = reader("SettingName").ToString()
                    Dim settingValue As String = reader("SettingValue").ToString()

                    Select Case settingName
                        Case "BlogTitle"
                            txtBlogTitle.Text = settingValue
                        Case "BlogDescription"
                            txtBlogDescription.Text = settingValue
                        Case "AdminEmail"
                            txtAdminEmail.Text = settingValue
                        Case "PostsPerPage"
                            ddlPostsPerPage.SelectedValue = settingValue
                    End Select
                End While

                reader.Close()
            Catch ex As Exception
                ShowErrorMessage("Error loading general settings: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub btnSaveGeneral_Click(sender As Object, e As EventArgs)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim transaction As SqlTransaction = connection.BeginTransaction()

                Try
                    ' Update Blog Title
                    UpdateSetting("General", "BlogTitle", txtBlogTitle.Text, connection, transaction)

                    ' Update Blog Description
                    UpdateSetting("General", "BlogDescription", txtBlogDescription.Text, connection, transaction)

                    ' Update Admin Email
                    UpdateSetting("General", "AdminEmail", txtAdminEmail.Text, connection, transaction)

                    ' Update Posts Per Page
                    UpdateSetting("General", "PostsPerPage", ddlPostsPerPage.SelectedValue, connection, transaction)

                    transaction.Commit()
                    ShowSuccessMessage("General settings updated successfully.")
                Catch ex As Exception
                    transaction.Rollback()
                    Throw New Exception("Failed to save general settings: " & ex.Message)
                End Try
            Catch ex As Exception
                ShowErrorMessage(ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Private Sub UpdateSetting(category As String, name As String, value As String, connection As SqlConnection, transaction As SqlTransaction)
            Dim cmd As New SqlCommand("IF EXISTS (SELECT 1 FROM BlogSettings WHERE SettingCategory = @Category AND SettingName = @Name) " & _
                                     "UPDATE BlogSettings SET SettingValue = @Value WHERE SettingCategory = @Category AND SettingName = @Name " & _
                                     "ELSE " & _
                                     "INSERT INTO BlogSettings (SettingCategory, SettingName, SettingValue) VALUES (@Category, @Name, @Value)", connection, transaction)
            cmd.Parameters.AddWithValue("@Category", category)
            cmd.Parameters.AddWithValue("@Name", name)
            cmd.Parameters.AddWithValue("@Value", value)
            cmd.ExecuteNonQuery()
        End Sub
        #End Region

        #Region "Categories Management"
        Private Sub LoadCategories()
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim cmd As New SqlCommand("SELECT CategoryID, CategoryName FROM Categories ORDER BY CategoryName", connection)
                Dim adapter As New SqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)

                rptCategories.DataSource = dt
                rptCategories.DataBind()
            Catch ex As Exception
                ShowErrorMessage("Error loading categories: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub btnAddCategory_Click(sender As Object, e As EventArgs)
            If String.IsNullOrWhiteSpace(txtNewCategory.Text) Then
                ShowErrorMessage("Please enter a category name.")
                Return
            End If

            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                
                ' Check if category already exists
                Dim cmdCheck As New SqlCommand("SELECT COUNT(*) FROM Categories WHERE CategoryName = @CategoryName", connection)
                cmdCheck.Parameters.AddWithValue("@CategoryName", txtNewCategory.Text.Trim())
                Dim categoryExists As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())
                
                If categoryExists > 0 Then
                    ShowErrorMessage("A category with this name already exists.")
                    Return
                End If
                
                ' Insert new category
                Dim cmdInsert As New SqlCommand("INSERT INTO Categories (CategoryName) VALUES (@CategoryName)", connection)
                cmdInsert.Parameters.AddWithValue("@CategoryName", txtNewCategory.Text.Trim())
                cmdInsert.ExecuteNonQuery()
                
                txtNewCategory.Text = String.Empty
                ShowSuccessMessage("Category added successfully.")
                
                ' Reload categories
                LoadCategories()
            Catch ex As Exception
                ShowErrorMessage("Error adding category: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub rptCategories_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
            Dim categoryId As Integer = Convert.ToInt32(e.CommandArgument)
            
            If e.CommandName = "Edit" Then
                ' Implement edit functionality (could use a modal or redirect to edit page)
                ' For simplicity, we'll just show a JavaScript prompt
                Dim script As String = "var newName = prompt('Enter new name for category:', '');" & _
                                     "if (newName && newName.trim() !== '') {" & _
                                     "  __doPostBack('" & source.UniqueID & "', 'UpdateCategory|" & categoryId & "|' + newName);" & _
                                     "}"
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "EditCategoryScript", script, True)
            ElseIf e.CommandName = "Delete" Then
                DeleteCategory(categoryId)
            ElseIf e.CommandName.StartsWith("UpdateCategory") Then
                ' This is triggered by the JavaScript postback
                Dim parameters As String() = e.CommandName.Split("|"c)
                If parameters.Length = 3 Then
                    UpdateCategory(categoryId, parameters(2))
                End If
            End If
        End Sub

        Private Sub UpdateCategory(categoryId As Integer, newName As String)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                
                ' Check if category name already exists
                Dim cmdCheck As New SqlCommand("SELECT COUNT(*) FROM Categories WHERE CategoryName = @CategoryName AND CategoryID <> @CategoryID", connection)
                cmdCheck.Parameters.AddWithValue("@CategoryName", newName.Trim())
                cmdCheck.Parameters.AddWithValue("@CategoryID", categoryId)
                Dim categoryExists As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())
                
                If categoryExists > 0 Then
                    ShowErrorMessage("A category with this name already exists.")
                    Return
                End If
                
                ' Update category
                Dim cmdUpdate As New SqlCommand("UPDATE Categories SET CategoryName = @CategoryName WHERE CategoryID = @CategoryID", connection)
                cmdUpdate.Parameters.AddWithValue("@CategoryName", newName.Trim())
                cmdUpdate.Parameters.AddWithValue("@CategoryID", categoryId)
                cmdUpdate.ExecuteNonQuery()
                
                ShowSuccessMessage("Category updated successfully.")
                
                ' Reload categories
                LoadCategories()
            Catch ex As Exception
                ShowErrorMessage("Error updating category: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Private Sub DeleteCategory(categoryId As Integer)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim transaction As SqlTransaction = connection.BeginTransaction()
                
                Try
                    ' Update posts to remove category
                    Dim cmdUpdatePosts As New SqlCommand("UPDATE Posts SET CategoryID = NULL WHERE CategoryID = @CategoryID", connection, transaction)
                    cmdUpdatePosts.Parameters.AddWithValue("@CategoryID", categoryId)
                    cmdUpdatePosts.ExecuteNonQuery()
                    
                    ' Delete category
                    Dim cmdDeleteCategory As New SqlCommand("DELETE FROM Categories WHERE CategoryID = @CategoryID", connection, transaction)
                    cmdDeleteCategory.Parameters.AddWithValue("@CategoryID", categoryId)
                    cmdDeleteCategory.ExecuteNonQuery()
                    
                    transaction.Commit()
                    ShowSuccessMessage("Category deleted successfully.")
                    
                    ' Reload categories
                    LoadCategories()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            Catch ex As Exception
                ShowErrorMessage("Error deleting category: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub
        #End Region

        #Region "User Settings"
        Private Sub LoadUserSettings()
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim cmd As New SqlCommand("SELECT SettingName, SettingValue FROM BlogSettings WHERE SettingCategory = 'Users'", connection)
                Dim reader As SqlDataReader = cmd.ExecuteReader()

                While reader.Read()
                    Dim settingName As String = reader("SettingName").ToString()
                    Dim settingValue As String = reader("SettingValue").ToString()

                    Select Case settingName
                        Case "AllowRegistration"
                            chkAllowRegistration.Checked = Convert.ToBoolean(settingValue)
                        Case "EmailVerification"
                            chkEmailVerification.Checked = Convert.ToBoolean(settingValue)
                        Case "DefaultUserRole"
                            ddlDefaultUserRole.SelectedValue = settingValue
                    End Select
                End While

                reader.Close()
            Catch ex As Exception
                ShowErrorMessage("Error loading user settings: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub btnSaveUserSettings_Click(sender As Object, e As EventArgs)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim transaction As SqlTransaction = connection.BeginTransaction()

                Try
                    ' Update Allow Registration
                    UpdateSetting("Users", "AllowRegistration", chkAllowRegistration.Checked.ToString(), connection, transaction)

                    ' Update Email Verification
                    UpdateSetting("Users", "EmailVerification", chkEmailVerification.Checked.ToString(), connection, transaction)

                    ' Update Default User Role
                    UpdateSetting("Users", "DefaultUserRole", ddlDefaultUserRole.SelectedValue, connection, transaction)

                    transaction.Commit()
                    ShowSuccessMessage("User settings updated successfully.")
                Catch ex As Exception
                    transaction.Rollback()
                    Throw New Exception("Failed to save user settings: " & ex.Message)
                End Try
            Catch ex As Exception
                ShowErrorMessage(ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub
        #End Region

        #Region "Comment Settings"
        Private Sub LoadCommentSettings()
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim cmd As New SqlCommand("SELECT SettingName, SettingValue FROM BlogSettings WHERE SettingCategory = 'Comments'", connection)
                Dim reader As SqlDataReader = cmd.ExecuteReader()

                While reader.Read()
                    Dim settingName As String = reader("SettingName").ToString()
                    Dim settingValue As String = reader("SettingValue").ToString()

                    Select Case settingName
                        Case "AllowComments"
                            chkAllowComments.Checked = Convert.ToBoolean(settingValue)
                        Case "ModerateComments"
                            chkModerateComments.Checked = Convert.ToBoolean(settingValue)
                        Case "AllowGuestComments"
                            chkAllowGuestComments.Checked = Convert.ToBoolean(settingValue)
                        Case "CommentsPerPage"
                            txtCommentsPerPage.Text = settingValue
                    End Select
                End While

                reader.Close()
            Catch ex As Exception
                ShowErrorMessage("Error loading comment settings: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub btnSaveCommentSettings_Click(sender As Object, e As EventArgs)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim transaction As SqlTransaction = connection.BeginTransaction()

                Try
                    ' Update Allow Comments
                    UpdateSetting("Comments", "AllowComments", chkAllowComments.Checked.ToString(), connection, transaction)

                    ' Update Moderate Comments
                    UpdateSetting("Comments", "ModerateComments", chkModerateComments.Checked.ToString(), connection, transaction)

                    ' Update Allow Guest Comments
                    UpdateSetting("Comments", "AllowGuestComments", chkAllowGuestComments.Checked.ToString(), connection, transaction)

                    ' Update Comments Per Page
                    UpdateSetting("Comments", "CommentsPerPage", txtCommentsPerPage.Text, connection, transaction)

                    transaction.Commit()
                    ShowSuccessMessage("Comment settings updated successfully.")
                Catch ex As Exception
                    transaction.Rollback()
                    Throw New Exception("Failed to save comment settings: " & ex.Message)
                End Try
            Catch ex As Exception
                ShowErrorMessage(ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub
        #End Region

        #Region "Backup and Restore"
        Private Sub LoadBackupHistory()
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                Dim cmd As New SqlCommand("SELECT BackupID, BackupDate, BackupSize, BackupBy FROM BackupHistory ORDER BY BackupDate DESC", connection)
                Dim adapter As New SqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)

                gvBackupHistory.DataSource = dt
                gvBackupHistory.DataBind()
            Catch ex As Exception
                ShowErrorMessage("Error loading backup history: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub btnCreateBackup_Click(sender As Object, e As EventArgs)
            ' In a real implementation, this would create an actual database backup
            ' For this example, we'll just simulate it
            
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                
                ' Insert backup record
                Dim cmd As New SqlCommand("INSERT INTO BackupHistory (BackupDate, BackupSize, BackupPath, BackupBy) " & _
                                         "VALUES (@BackupDate, @BackupSize, @BackupPath, @BackupBy); " & _
                                         "SELECT SCOPE_IDENTITY()", connection)
                cmd.Parameters.AddWithValue("@BackupDate", DateTime.Now)
                cmd.Parameters.AddWithValue("@BackupSize", "4.2 MB") ' Simulated size
                cmd.Parameters.AddWithValue("@BackupPath", "backups/blog_backup_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".bak")
                cmd.Parameters.AddWithValue("@BackupBy", Session("Username"))
                
                cmd.ExecuteNonQuery()
                
                ShowSuccessMessage("Backup created successfully.")
                
                ' Reload backup history
                LoadBackupHistory()
            Catch ex As Exception
                ShowErrorMessage("Error creating backup: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub btnRestoreBackup_Click(sender As Object, e As EventArgs)
            ' Check if file was uploaded
            If Not fileBackupRestore.HasFile Then
                ShowErrorMessage("Please select a backup file to restore.")
                Return
            End If
            
            ' In a real implementation, this would restore from the uploaded backup file
            ' For this example, we'll just simulate it
            
            Try
                ' Simulate backup restore process
                System.Threading.Thread.Sleep(1000) ' Simulate processing time
                
                ShowSuccessMessage("Database restored successfully from backup.")
            Catch ex As Exception
                ShowErrorMessage("Error restoring backup: " & ex.Message)
            End Try
        End Sub

        Protected Sub gvBackupHistory_RowCommand(sender As Object, e As GridViewCommandEventArgs)
            Dim backupId As Integer = Convert.ToInt32(e.CommandArgument)
            
            If e.CommandName = "Download" Then
                DownloadBackup(backupId)
            ElseIf e.CommandName = "Delete" Then
                DeleteBackup(backupId)
            End If
        End Sub

        Private Sub DownloadBackup(backupId As Integer)
            ' In a real implementation, this would retrieve the backup file and provide it for download
            ' For this example, we'll just simulate it
            
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                
                ' Get backup info
                Dim cmd As New SqlCommand("SELECT BackupPath FROM BackupHistory WHERE BackupID = @BackupID", connection)
                cmd.Parameters.AddWithValue("@BackupID", backupId)
                Dim backupPath As String = cmd.ExecuteScalar().ToString()
                
                ' In a real implementation, you would:
                ' 1. Get the physical path of the backup file
                ' 2. Check if file exists
                ' 3. Set response headers for download
                ' 4. Write the file to response output stream
                
                ' For this example, we'll just show a message
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "alert", "alert('In a real implementation, the backup file would be downloaded now.');", True)
            Catch ex As Exception
                ShowErrorMessage("Error downloading backup: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Private Sub DeleteBackup(backupId As Integer)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                
                ' In a real implementation, you would also delete the actual backup file
                
                ' Delete backup record
                Dim cmd As New SqlCommand("DELETE FROM BackupHistory WHERE BackupID = @BackupID", connection)
                cmd.Parameters.AddWithValue("@BackupID", backupId)
                cmd.ExecuteNonQuery()
                
                ShowSuccessMessage("Backup deleted successfully.")
                
                ' Reload backup history
                LoadBackupHistory()
            Catch ex As Exception
                ShowErrorMessage("Error deleting backup: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub
        #End Region

        #Region "Helper Methods"
        Private Sub ShowSuccessMessage(message As String)
            litSuccessMessage.Text = message
            pnlSuccess.Visible = True
            pnlError.Visible = False
            
            ' Register script to hide the message after a few seconds
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "SuccessMessageTimeout", 
                "$('.success-message').show().delay(3000).fadeOut();", True)
    End Sub

        Private Sub ShowErrorMessage(message As String)
            litErrorMessage.Text = message
            pnlError.Visible = True
            pnlSuccess.Visible = False
        End Sub
        #End Region
End Class
End Namespace