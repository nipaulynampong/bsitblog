Imports System.Data.SqlClient
Imports System.Configuration

Namespace BSIT
    Public Class AddUsers
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                If Not IsPostBack Then
                    If Not txtUsername Is Nothing Then
                        txtUsername.Focus()
                    End If
                End If
            Catch ex As Exception
                ShowError("Error during page load: " & ex.Message)
            End Try
        End Sub

        Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
            Try
                If Not Page.IsValid Then
                    ShowError("Please fill in all required fields correctly.")
                    Return
                End If

                If txtUsername Is Nothing OrElse String.IsNullOrEmpty(txtUsername.Text) OrElse _
                   txtPassword Is Nothing OrElse String.IsNullOrEmpty(txtPassword.Text) OrElse _
                   txtEmail Is Nothing OrElse String.IsNullOrEmpty(txtEmail.Text) OrElse _
                   txtFullName Is Nothing OrElse String.IsNullOrEmpty(txtFullName.Text) OrElse _
                   ddlUserType Is Nothing OrElse String.IsNullOrEmpty(ddlUserType.SelectedValue) Then
                    ShowError("All required fields must be filled out.")
                    Return
                End If

                ' Create connection directly with hardcoded connection string
                Using connection As New SqlConnection(ConnectionString)
                    Try
                        ' Open connection
                        connection.Open()

                        ' Create command
                        Using cmd As New SqlCommand("INSERT INTO users (username, password, email, fullname, usertype, isActive, createdDate) VALUES (@username, @password, @email, @fullname, @usertype, @isActive, @createdDate)", connection)
                            ' Add parameters
                            cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim())
                            cmd.Parameters.AddWithValue("@password", txtPassword.Text)
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim())
                            cmd.Parameters.AddWithValue("@fullname", txtFullName.Text.Trim())
                            cmd.Parameters.AddWithValue("@usertype", ddlUserType.SelectedValue)
                            cmd.Parameters.AddWithValue("@isActive", chkActive.Checked)
                            cmd.Parameters.AddWithValue("@createdDate", DateTime.Now)

                            ' Execute command
                            Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                            If rowsAffected > 0 Then
                                ' Success - redirect to ManageUsers.aspx
                                Response.Redirect("ManageUsers.aspx")
                            Else
                                ShowError("Failed to add user to database. No rows were affected.")
                            End If
                        End Using
                    Catch sqlEx As SqlException
                        ShowError("Database error: " & sqlEx.Message)
                    End Try
                End Using
            Catch ex As Exception
                ShowError("Error: " & ex.Message)
            End Try
        End Sub

        Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
            Try
                If Not Response Is Nothing Then
                    Response.Redirect("ManageUsers.aspx")
                End If
            Catch ex As Exception
                ShowError("Error during cancellation: " & ex.Message)
            End Try
        End Sub

        Private Sub ShowError(message As String)
            If Not lblMessage Is Nothing Then
                lblMessage.Text = message
                lblMessage.Visible = True
            End If
        End Sub
    End Class
End Namespace