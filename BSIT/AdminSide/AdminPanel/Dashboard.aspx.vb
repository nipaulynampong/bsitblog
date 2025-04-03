Imports System.Data.SqlClient
Imports System.Data

Public Class Dashboard
    Inherits System.Web.UI.Page
    
    Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"
    
    ' Control declarations
    Protected WithEvents lblTotalUsers As Label
    Protected WithEvents lblNewUsers As Label
    Protected WithEvents lblActiveUsers As Label
    Protected WithEvents lblTotalPosts As Label
    Protected WithEvents lblTotalViews As Label
    Protected WithEvents lblActivePosts As Label
    Protected WithEvents GridViewTopPosts As GridView
    Protected WithEvents GridViewRecentUsers As GridView

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Check if user is logged in
            If Session("AdminID") Is Nothing Then
                Response.Redirect("~/AdminSide/AdminLogin.aspx")
                Return
            End If
            
            ' Load dashboard data
            LoadUserCount()
            LoadNewUsersToday()
            LoadActiveUsers()
            
            ' Load dashboard statistics
            LoadStatistics()
            
            ' Load top posts by views
            LoadTopPosts()
            
            ' Load recent users
            LoadRecentUsers()
        End If
    End Sub
    
    Private Sub LoadUserCount()
        ' Get total user count
        Try
            Using conn As New SqlConnection(ConnectionString)
                Using cmd As New SqlCommand("SELECT COUNT(*) AS UserCount FROM users", conn)
                    conn.Open()
                    Dim userCount As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    If lblTotalUsers IsNot Nothing Then
                        lblTotalUsers.Text = userCount.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            If lblTotalUsers IsNot Nothing Then
                lblTotalUsers.Text = "0"
            End If
            ' Log error
            Response.Write("<!-- Error loading user count: " & ex.Message & " -->")
        End Try
    End Sub
    
    Private Sub LoadNewUsersToday()
        ' Get new users registered today
        Try
            Using conn As New SqlConnection(ConnectionString)
                ' SQL query to count users registered today
                Dim query As String = "SELECT COUNT(*) AS NewUsers FROM users WHERE CONVERT(date, createdDate) = CONVERT(date, GETDATE())"
                
                Using cmd As New SqlCommand(query, conn)
                    conn.Open()
                    Dim newUsers As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    If lblNewUsers IsNot Nothing Then
                        lblNewUsers.Text = newUsers.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            If lblNewUsers IsNot Nothing Then
                lblNewUsers.Text = "0"
            End If
            ' Log error
            Response.Write("<!-- Error loading new users: " & ex.Message & " -->")
        End Try
    End Sub
    
    Private Sub LoadActiveUsers()
        ' Get active user count
        Try
            Using conn As New SqlConnection(ConnectionString)
                Using cmd As New SqlCommand("SELECT COUNT(*) AS ActiveUsers FROM users WHERE isActive = 1", conn)
                    conn.Open()
                    Dim activeUsers As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    If lblActiveUsers IsNot Nothing Then
                        lblActiveUsers.Text = activeUsers.ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            If lblActiveUsers IsNot Nothing Then
                lblActiveUsers.Text = "0"
            End If
            ' Log error
            Response.Write("<!-- Error loading active users: " & ex.Message & " -->")
        End Try
    End Sub
    
    Private Sub LoadStatistics()
        Try
            Using conn As New SqlConnection(ConnectionString)
                ' Get total users count
                Dim queryUsers As String = "SELECT COUNT(*) AS UserCount FROM users"
                Using cmd As New SqlCommand(queryUsers, conn)
                    conn.Open()
                    If cmd.ExecuteScalar() IsNot Nothing Then
                        lblTotalUsers.Text = cmd.ExecuteScalar().ToString()
                    End If
                End Using
                
                ' Get total posts count
                Dim queryPosts As String = "SELECT COUNT(*) AS PostCount FROM Posts"
                Using cmd As New SqlCommand(queryPosts, conn)
                    conn.Open()
                    If cmd.ExecuteScalar() IsNot Nothing Then
                        lblTotalPosts.Text = cmd.ExecuteScalar().ToString()
                    End If
                End Using
                
                ' Get total views count
                Dim queryViews As String = "SELECT SUM(Views) AS TotalViews FROM Posts"
                Using cmd As New SqlCommand(queryViews, conn)
                    conn.Open()
                    Dim totalViews As Object = cmd.ExecuteScalar()
                    If totalViews IsNot Nothing And Not IsDBNull(totalViews) Then
                        lblTotalViews.Text = totalViews.ToString()
                    Else
                        lblTotalViews.Text = "0"
                    End If
                End Using
            End Using
            
            ' Get active posts count
            Using conn As New SqlConnection(ConnectionString)
                Dim queryActive As String = "SELECT COUNT(*) AS ActiveCount FROM Posts WHERE Status = 'Published'"
                Using cmd As New SqlCommand(queryActive, conn)
                    conn.Open()
                    Dim activeCount As Object = cmd.ExecuteScalar()
                    If activeCount IsNot Nothing Then
                        lblActivePosts.Text = activeCount.ToString()
                    End If
                End Using
            End Using
            
        Catch ex As Exception
            ' Handle errors - in production you'd want to log this
            ' For now, we'll just leave the default values (0)
        End Try
    End Sub
    
    Private Sub LoadTopPosts()
        Try
            Using conn As New SqlConnection(ConnectionString)
                ' Query to get top 5 posts by views with author name
                Dim query As String = "SELECT TOP 5 p.PostID, p.Title, p.Views, p.CreatedDate, p.Status, u.fullname " & _
                                    "FROM Posts p " & _
                                    "LEFT JOIN users u ON p.AuthorID = u.UserID " & _
                                    "ORDER BY p.Views DESC"
                
                Using cmd As New SqlCommand(query, conn)
                    conn.Open()
                    Dim dataAdapter As New SqlDataAdapter(cmd)
                    Dim dataSet As New DataSet()
                    dataAdapter.Fill(dataSet)
                    If dataSet.Tables.Count > 0 And dataSet.Tables(0).Rows.Count > 0 Then
                        GridViewTopPosts.DataSource = dataSet.Tables(0)
                        GridViewTopPosts.DataBind()
                    End If
                End Using
            End Using
            
        Catch ex As Exception
            ' Handle errors
        End Try
    End Sub
    
    Private Sub LoadRecentUsers()
        Try
            Using conn As New SqlConnection(ConnectionString)
                ' Query to get 5 most recently created users
                Dim query As String = "SELECT TOP 5 UserID, username, fullname, usertype, " & _
                                    "CASE WHEN isActive = 1 THEN 'Yes' ELSE 'No' END AS isActive, " & _
                                    "createdDate " & _
                                    "FROM users " & _
                                    "ORDER BY createdDate DESC"
                
                Using cmd As New SqlCommand(query, conn)
                    conn.Open()
                    Dim dataAdapter As New SqlDataAdapter(cmd)
                    Dim dataSet As New DataSet()
                    dataAdapter.Fill(dataSet)
                    If dataSet.Tables.Count > 0 And dataSet.Tables(0).Rows.Count > 0 Then
                        GridViewRecentUsers.DataSource = dataSet.Tables(0)
                        GridViewRecentUsers.DataBind()
                    End If
                End Using
            End Using
            
        Catch ex As Exception
            ' Handle errors
        End Try
    End Sub
End Class