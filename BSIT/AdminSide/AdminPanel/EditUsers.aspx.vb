Imports System.Data.SqlClient
Imports System.Configuration

Namespace BSIT
    Partial Public Class EditUsers
        Inherits System.Web.UI.Page

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Check if ID is provided in the URL
                If Not String.IsNullOrEmpty(Request.QueryString("id")) Then
                    Dim userId As Integer
                    If Integer.TryParse(Request.QueryString("id"), userId) Then
                        ' Store the user ID
                        hdnUserID.Value = userId.ToString()
                        
                        ' Load user data
                        LoadUserData(userId)
                    Else
                        ' Invalid ID format, redirect to ManageUsers.aspx
                        Response.Redirect("ManageUsers.aspx")
                    End If
                Else
                    ' No ID provided, redirect to ManageUsers.aspx
                    Response.Redirect("ManageUsers.aspx")
                End If
            End If
        End Sub

        Private Sub LoadUserData(ByVal userId As Integer)
            Try
                ' Create connection with direct connection string
                Using conn As New SqlConnection(ConnectionString)
                    ' Create command
                    Using cmd As New SqlCommand("SELECT * FROM users WHERE UserID = @UserID", conn)
                        ' Add parameter
                        cmd.Parameters.AddWithValue("@UserID", userId)
                        
                        ' Open connection
                        conn.Open()
                        
                        ' Execute reader
                        Using reader As SqlDataReader = cmd.ExecuteReader()
                            If reader.Read() Then
                                ' Populate form fields
                                txtUsername.Text = reader("username").ToString()
                                txtEmail.Text = reader("email").ToString()
                                txtFullName.Text = reader("fullname").ToString()
                                ddlUserType.SelectedValue = reader("usertype").ToString()
                                chkActive.Checked = Convert.ToBoolean(reader("isActive"))
                                
                                ' Display created date
                                If Not IsDBNull(reader("createdDate")) Then
                                    Dim createdDate As DateTime = DirectCast(reader("createdDate"), DateTime)
                                    lblCreatedDate.Text = "Created: " & createdDate.ToString("MMMM dd, yyyy")
                                End If
                            Else
                                ' User not found, redirect to ManageUsers.aspx
                                Response.Redirect("ManageUsers.aspx")
                            End If
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                ' Show error message
                lblMessage.Text = "Error loading user data: " & ex.Message
                lblMessage.Visible = True
            End Try
        End Sub

        Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
            If Page.IsValid Then
                Try
                    ' Create connection with direct connection string
                    Using conn As New SqlConnection(ConnectionString)
                        ' Create command with appropriate SQL based on whether password is being updated
                        Dim sqlCommand As String
                        
                        If String.IsNullOrEmpty(txtPassword.Text) Then
                            ' Password not being updated
                            sqlCommand = "UPDATE users SET username = @username, email = @email, fullname = @fullname, usertype = @usertype, isActive = @isActive WHERE UserID = @UserID"
                        Else
                            ' Password is being updated
                            sqlCommand = "UPDATE users SET username = @username, email = @email, password = @password, fullname = @fullname, usertype = @usertype, isActive = @isActive WHERE UserID = @UserID"
                        End If
                        
                        ' Create command
                        Using cmd As New SqlCommand(sqlCommand, conn)
                            ' Add parameters
                            cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim())
                            cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim())
                            cmd.Parameters.AddWithValue("@fullname", txtFullName.Text.Trim())
                            cmd.Parameters.AddWithValue("@usertype", ddlUserType.SelectedValue)
                            cmd.Parameters.AddWithValue("@isActive", If(chkActive.Checked, 1, 0))
                            cmd.Parameters.AddWithValue("@UserID", hdnUserID.Value)
                            
                            ' Add password parameter if needed
                            If Not String.IsNullOrEmpty(txtPassword.Text) Then
                                cmd.Parameters.AddWithValue("@password", txtPassword.Text) ' In a real application, you should hash this password
                            End If
                            
                            ' Open connection and execute command
                            conn.Open()
                            cmd.ExecuteNonQuery()
                        End Using
                    End Using

                    ' Redirect to ManageUsers.aspx
                    Response.Redirect("ManageUsers.aspx")
                Catch ex As Exception
                    ' Show error message
                    lblMessage.Text = "Error: " & ex.Message
                    lblMessage.Visible = True
                End Try
            End If
        End Sub

        Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
            ' Redirect to ManageUsers.aspx
            Response.Redirect("ManageUsers.aspx")
        End Sub
    End Class
End Namespace