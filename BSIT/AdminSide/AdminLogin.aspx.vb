Imports System.Data.SqlClient
Imports System.Data

Public Class AdminLogin
    Inherits System.Web.UI.Page

    Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

    ' Add protected declarations for the controls
    Protected WithEvents pnlError As Panel
    Protected WithEvents lblErrorMessage As Label
    Protected WithEvents txtUsername As TextBox
    Protected WithEvents txtPassword As TextBox

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Check if user is already logged in as admin
        If Not IsNothing(Session("AdminID")) Then
            ' Redirect to admin panel
            Response.Redirect("~/AdminSide/AdminPanel/AdminPanel.aspx")
        End If
    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Clear any previous error messages
        pnlError.Visible = False
        lblErrorMessage.Text = String.Empty

        ' Get the username and password
        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text.Trim()

        ' Validate input
        If String.IsNullOrEmpty(username) Or String.IsNullOrEmpty(password) Then
            DisplayError("Username and password are required.")
            Return
        End If

        ' Authenticate against database
        Try
            Using conn As New SqlConnection(ConnectionString)
                ' SQL query to check for admin user with matching credentials
                Dim query As String = "SELECT UserID, username, fullname FROM users WHERE username = @Username AND password = @Password AND usertype = 'admin' AND isActive = 1"
                
                Using cmd As New SqlCommand(query, conn)
                    ' Add parameters
                    cmd.Parameters.AddWithValue("@Username", username)
                    cmd.Parameters.AddWithValue("@Password", password)
                    
                    ' Open connection
                    conn.Open()
                    
                    ' Execute reader
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Valid admin login
                            Dim userId As Integer = Convert.ToInt32(reader("UserID"))
                            Dim fullName As String = reader("fullname").ToString()
                            
                            ' Store user info in session
                            Session("AdminID") = userId
                            Session("AdminName") = fullName
                            Session("AdminUsername") = username
                            
                            ' Redirect to admin panel
                            Response.Redirect("~/AdminSide/AdminPanel/AdminPanel.aspx")
                        Else
                            ' Invalid login
                            DisplayError("Invalid username or password, or you do not have admin privileges.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ' Handle any errors
            DisplayError("An error occurred: " & ex.Message)
        End Try
    End Sub
    
    Private Sub DisplayError(ByVal message As String)
        pnlError.Visible = True
        lblErrorMessage.Text = message
    End Sub
End Class