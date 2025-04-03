Imports System.Data.SqlClient
Imports System.Text
Imports BSIT.Module

Namespace BSIT
    Public Class TestDB
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Display connection info
                DisplayConnectionInfo()
            End If
        End Sub

        Private Sub DisplayConnectionInfo()
            Try
                Dim conn As New Connection()
                lblConnectionInfo.Text = "<strong>Connection String:</strong> " & conn.ConnectionString & "<br/>"
                
                ' Test connection
                Try
                    conn.Open()
                    lblConnectionInfo.Text += "<span class='text-success'>Connection successful!</span><br/>"
                    
                    ' Get database information
                    Dim cmd As New SqlCommand("SELECT @@version", conn.Connect)
                    Dim version As String = cmd.ExecuteScalar().ToString()
                    lblConnectionInfo.Text += "<strong>SQL Server Version:</strong> " & version
                    
                Catch ex As Exception
                    lblConnectionInfo.Text += "<span class='text-danger'>Connection failed: " & ex.Message & "</span>"
                Finally
                    conn.Close()
                End Try
            Catch ex As Exception
                lblConnectionInfo.Text = "<span class='text-danger'>Error: " & ex.Message & "</span>"
            End Try
        End Sub

        Protected Sub btnTestInsert_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Clear previous results
            lblResults.Text = ""
            preDetails.InnerHtml = ""
            preDetails.Visible = False
            
            Dim sb As New StringBuilder()
            
            Try
                ' Create connection using the Connection module
                Dim conn As New Connection()
                
                Try
                    ' Build the query
                    Dim query As String = "INSERT INTO users (username, password, email, fullname, usertype, isActive, createdDate) " & _
                                         "VALUES (@username, @password, @email, @fullname, @usertype, @isActive, @createdDate)"
                    
                    sb.AppendLine("Query: " & query)
                    sb.AppendLine()
                    
                    ' Add parameters
                    conn.AddParam("@username", txtUsername.Text.Trim())
                    conn.AddParam("@password", txtPassword.Text)
                    conn.AddParam("@email", txtEmail.Text.Trim())
                    conn.AddParam("@fullname", txtFullName.Text.Trim())
                    conn.AddParam("@usertype", ddlUserType.SelectedValue)
                    conn.AddParam("@isActive", If(chkActive.Checked, "1", "0"))
                    conn.AddParam("@createdDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    
                    sb.AppendLine("Parameters:")
                    sb.AppendLine("@username = " & txtUsername.Text.Trim())
                    sb.AppendLine("@password = " & txtPassword.Text)
                    sb.AppendLine("@email = " & txtEmail.Text.Trim())
                    sb.AppendLine("@fullname = " & txtFullName.Text.Trim())
                    sb.AppendLine("@usertype = " & ddlUserType.SelectedValue)
                    sb.AppendLine("@isActive = " & If(chkActive.Checked, "1", "0"))
                    sb.AppendLine("@createdDate = " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    sb.AppendLine()
                    
                    ' Execute query
                    Dim success As Boolean = conn.Query(query)
                    
                    sb.AppendLine("Query execution result: " & success.ToString())
                    
                    If success Then
                        lblResults.Text = "<div class='alert alert-success'>User successfully added to database!</div>"
                    Else
                        lblResults.Text = "<div class='alert alert-danger'>Failed to add user to database.</div>"
                    End If
                Catch sqlEx As SqlException
                    lblResults.Text = "<div class='alert alert-danger'>SQL Error: " & sqlEx.Message & "</div>"
                    sb.AppendLine("SQL Exception:")
                    sb.AppendLine("Message: " & sqlEx.Message)
                    sb.AppendLine("Number: " & sqlEx.Number.ToString())
                    sb.AppendLine("LineNumber: " & sqlEx.LineNumber.ToString())
                    sb.AppendLine("Stack Trace: " & sqlEx.StackTrace)
                Catch ex As Exception
                    lblResults.Text = "<div class='alert alert-danger'>Error: " & ex.Message & "</div>"
                    sb.AppendLine("General Exception:")
                    sb.AppendLine("Message: " & ex.Message)
                    sb.AppendLine("Stack Trace: " & ex.StackTrace)
                Finally
                    ' Close the connection
                    conn.Close()
                    sb.AppendLine("Connection closed.")
                End Try
            Catch ex As Exception
                lblResults.Text = "<div class='alert alert-danger'>Error: " & ex.Message & "</div>"
                sb.AppendLine("Outer Exception:")
                sb.AppendLine("Message: " & ex.Message)
                sb.AppendLine("Stack Trace: " & ex.StackTrace)
            Finally
                ' Display debug details
                preDetails.InnerHtml = Server.HtmlEncode(sb.ToString())
                preDetails.Visible = True
            End Try
        End Sub

        Protected Sub btnTestDirectInsert_Click(ByVal sender As Object, ByVal e As EventArgs)
            ' Clear previous results
            lblResults.Text = ""
            preDetails.InnerHtml = ""
            preDetails.Visible = False
            
            Dim sb As New StringBuilder()
            
            Try
                ' Create connection and command directly
                Dim conn As New Connection()
                
                Try
                    ' Open connection
                    conn.Open()
                    sb.AppendLine("Connection opened.")
                    
                    ' Create command
                    Dim cmd As New SqlCommand()
                    cmd.Connection = conn.Connect
                    cmd.CommandText = "INSERT INTO users (username, password, email, fullname, usertype, isActive, createdDate) " & _
                                     "VALUES (@username, @password, @email, @fullname, @usertype, @isActive, @createdDate)"
                    
                    sb.AppendLine("Command text: " & cmd.CommandText)
                    
                    ' Add parameters
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim())
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text)
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim())
                    cmd.Parameters.AddWithValue("@fullname", txtFullName.Text.Trim())
                    cmd.Parameters.AddWithValue("@usertype", ddlUserType.SelectedValue)
                    cmd.Parameters.AddWithValue("@isActive", chkActive.Checked)
                    cmd.Parameters.AddWithValue("@createdDate", DateTime.Now)
                    
                    sb.AppendLine("Parameters:")
                    sb.AppendLine("@username = " & txtUsername.Text.Trim())
                    sb.AppendLine("@password = " & txtPassword.Text)
                    sb.AppendLine("@email = " & txtEmail.Text.Trim())
                    sb.AppendLine("@fullname = " & txtFullName.Text.Trim())
                    sb.AppendLine("@usertype = " & ddlUserType.SelectedValue)
                    sb.AppendLine("@isActive = " & chkActive.Checked.ToString())
                    sb.AppendLine("@createdDate = " & DateTime.Now.ToString())
                    
                    ' Execute command
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                    
                    sb.AppendLine("ExecuteNonQuery result: " & rowsAffected.ToString() & " rows affected")
                    
                    If rowsAffected > 0 Then
                        lblResults.Text = "<div class='alert alert-success'>User successfully added to database! (" & rowsAffected.ToString() & " rows affected)</div>"
                    Else
                        lblResults.Text = "<div class='alert alert-danger'>Failed to add user to database. No rows affected.</div>"
                    End If
                Catch sqlEx As SqlException
                    lblResults.Text = "<div class='alert alert-danger'>SQL Error: " & sqlEx.Message & "</div>"
                    sb.AppendLine("SQL Exception:")
                    sb.AppendLine("Message: " & sqlEx.Message)
                    sb.AppendLine("Number: " & sqlEx.Number.ToString())
                    sb.AppendLine("LineNumber: " & sqlEx.LineNumber.ToString())
                    sb.AppendLine("Stack Trace: " & sqlEx.StackTrace)
                Catch ex As Exception
                    lblResults.Text = "<div class='alert alert-danger'>Error: " & ex.Message & "</div>"
                    sb.AppendLine("General Exception:")
                    sb.AppendLine("Message: " & ex.Message)
                    sb.AppendLine("Stack Trace: " & ex.StackTrace)
                Finally
                    ' Close the connection
                    conn.Close()
                    sb.AppendLine("Connection closed.")
                End Try
            Catch ex As Exception
                lblResults.Text = "<div class='alert alert-danger'>Error: " & ex.Message & "</div>"
                sb.AppendLine("Outer Exception:")
                sb.AppendLine("Message: " & ex.Message)
                sb.AppendLine("Stack Trace: " & ex.StackTrace)
            Finally
                ' Display debug details
                preDetails.InnerHtml = Server.HtmlEncode(sb.ToString())
                preDetails.Visible = True
            End Try
        End Sub
    End Class
End Namespace 