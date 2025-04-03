Imports BSIT.Module
Imports System.Data

Namespace BSIT
Public Class EditorLogin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Hide error message initially
                errorMessage.Style.Add("display", "none")
                
                ' Check if user is already logged in
                If Session("EditorID") IsNot Nothing Then
                    Response.Redirect("~/Editor/EditorPanel/EditorPanel.aspx")
                End If
            End If
        End Sub
        
        Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
            Dim username As String = txtUsername.Text.Trim()
            Dim password As String = txtPassword.Text.Trim()
            
            ' Validate inputs
            If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
                ShowError("Please enter both username and password.")
                Return
            End If
            
            ' Authenticate user
            If AuthenticateEditor(username, password) Then
                Response.Redirect("~/Editor/EditorPanel/EditorPanel.aspx")
            Else
                ShowError("Invalid username or password. Please try again.")
            End If
        End Sub
        
        Private Function AuthenticateEditor(username As String, password As String) As Boolean
            Try
                ' Create connection using the Connection class
                Dim conn As New BSIT.Module.Connection()
                
                ' Add parameters
                conn.AddParam("@Username", username)
                conn.AddParam("@Password", password)
                conn.AddParam("@UserType", "Editor")
                
                ' Execute query
                Dim query As String = "SELECT UserID, FullName, Email FROM Users " & _
                                   "WHERE Username = @Username AND Password = @Password " & _
                                   "AND UserType = @UserType"
                
                If conn.Query(query) AndAlso conn.DataCount > 0 Then
                    ' Store user info in session
                    Dim userId As Integer = Convert.ToInt32(conn.Data.Tables(0).Rows(0)("UserID"))
                    Dim fullName As String = conn.Data.Tables(0).Rows(0)("FullName").ToString()
                    Dim email As String = conn.Data.Tables(0).Rows(0)("Email").ToString()
                    
                    Session("EditorID") = userId
                    Session("EditorName") = fullName
                    Session("EditorEmail") = email
                    Session("EditorUsername") = username
                    
                    ' Log login activity
                    LogLoginActivity(userId, username)
                    
                    Return True
                End If
                
                Return False
            Catch ex As Exception
                ' Log error
                System.Diagnostics.Debug.WriteLine("Login error: " & ex.Message)
                Return False
            End Try
        End Function
        
        Private Sub LogLoginActivity(userId As Integer, username As String)
            Try
                ' Create connection
                Dim conn As New BSIT.Module.Connection()
                
                ' Add parameters
                conn.AddParam("@UserID", userId.ToString())
                conn.AddParam("@Username", username)
                conn.AddParam("@LoginTime", DateTime.Now.ToString())
                conn.AddParam("@IPAddress", Request.UserHostAddress)
                conn.AddParam("@UserAgent", Request.UserAgent)
                
                ' Execute query - assuming there's a UserLoginLog table
                Dim query As String = "INSERT INTO UserLoginLog (UserID, Username, LoginTime, IPAddress, UserAgent) " & _
                                   "VALUES (@UserID, @Username, @LoginTime, @IPAddress, @UserAgent)"
                
                conn.Query(query)
            Catch ex As Exception
                ' Just log the error, don't interrupt the login process
                System.Diagnostics.Debug.WriteLine("Error logging login activity: " & ex.Message)
            End Try
    End Sub

        Private Sub ShowError(message As String)
            errorMessage.InnerText = message
            errorMessage.Style.Add("display", "block")
        End Sub
End Class
End Namespace