Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Public Class ManageUsers
    Inherits System.Web.UI.Page
    
    Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

    ' Control declarations
    Protected WithEvents gvUsers As GridView
    Protected WithEvents pnlNoResults As Panel
    Protected WithEvents txtSearch As TextBox
    Protected WithEvents btnSearch As Button
    Protected WithEvents btnAddUser As Button
    Protected WithEvents btnLoadData As Button
    Protected WithEvents ScriptManager1 As ScriptManager
    Protected WithEvents hdnDataLoaded As HiddenField
    Protected WithEvents ddlUserType As DropDownList

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                ' Set initial state
                hdnDataLoaded.Value = "false"
                ' Load users on initial page load
                LoadUsers()
            End If
        Catch ex As Exception
            ' Catch any unexpected errors in Page_Load
            Response.Write("<div class='alert alert-danger'>Error loading page: " & ex.Message & "</div>")
        End Try
    End Sub

    Private Sub LoadUsers(Optional ByVal searchTerm As String = "", Optional ByVal userType As String = "")
        Try
            ' Debug info
            Response.Write("<div style='display:none;'>LoadUsers entered with searchTerm: " & searchTerm & " and userType: " & userType & "</div>")

            ' Always set a default DataSource for the GridView regardless of success or failure
            ' This ensures the GridView is always rendered
            gvUsers.DataSource = CreateEmptyTable()

            ' Build the base query
            Dim query As String = "SELECT UserID, username, email, usertype AS Role, " & _
                               "CASE WHEN isActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status, " & _
                               "createdDate FROM users"

            ' Build where clause based on parameters
            Dim whereConditions As New List(Of String)
            Dim parameters As New List(Of SqlParameter)

            ' Add search filter if provided
            If Not String.IsNullOrEmpty(searchTerm) Then
                whereConditions.Add("(username LIKE @SearchTerm OR email LIKE @SearchTerm)")
                parameters.Add(New SqlParameter("@SearchTerm", "%" & searchTerm & "%"))
            End If

            ' Add usertype filter if provided
            If Not String.IsNullOrEmpty(userType) Then
                whereConditions.Add("usertype = @UserType")
                parameters.Add(New SqlParameter("@UserType", userType))
            End If

            ' Add WHERE clause if we have conditions
            If whereConditions.Count > 0 Then
                query += " WHERE " & String.Join(" AND ", whereConditions)
            End If

            ' Add ORDER BY to sort results
            query += " ORDER BY createdDate DESC"

            ' Debug info about the query being executed
            Response.Write("<div style='display:none;'>Executing query: " & query & "</div>")

            Using conn As New SqlConnection(ConnectionString)
                Using cmd As New SqlCommand(query, conn)
                    ' Add parameters to command
                    For Each param As SqlParameter In parameters
                        cmd.Parameters.Add(param)
                    Next

                    ' Create data adapter
                    Using adapter As New SqlDataAdapter(cmd)
                        ' Create dataset to hold results
                        Dim ds As New DataSet()
                        
                        ' Open connection and fill dataset
                        conn.Open()
                        Dim rowCount As Integer = adapter.Fill(ds)
                        
                        ' Debug info about row count
                        Response.Write("<div style='display:none;'>Query returned " & rowCount & " rows</div>")
                        
                        If rowCount > 0 Then
                            ' Bind data to GridView
                            gvUsers.DataSource = ds
                            gvUsers.DataBind()
                            
                            ' Show GridView and hide "no results" panel
                            gvUsers.Visible = True
                            pnlNoResults.Visible = False
                            
                            ' Debug info
                            Response.Write("<div style='display:none;'>Bound " & rowCount & " rows to GridView</div>")
                        Else
                            ' No results found, show the "no results" panel
                            gvUsers.Visible = False
                            pnlNoResults.Visible = True
                            
                            ' Debug info
                            Response.Write("<div style='display:none;'>No results found</div>")
                        End If
                    End Using
                End Using
            End Using
            
            ' Set data loaded flag
            hdnDataLoaded.Value = "true"
        Catch ex As Exception
            ' Show error
            Response.Write("<div class='alert alert-danger'>Error loading users: " & ex.Message & "</div>")
            
            ' Still set data loaded flag to prevent repeated attempts
            hdnDataLoaded.Value = "true"
        End Try
    End Sub

    ' Helper function to create empty table with correct structure
    Private Function CreateEmptyTable() As DataTable
        Dim emptyTable As New DataTable()
        emptyTable.Columns.Add("UserID", GetType(Integer))
        emptyTable.Columns.Add("username", GetType(String))
        emptyTable.Columns.Add("email", GetType(String))
        emptyTable.Columns.Add("Role", GetType(String))
        emptyTable.Columns.Add("Status", GetType(String))
        emptyTable.Columns.Add("createdDate", GetType(DateTime))
        Return emptyTable
    End Function

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        ' Load users with both search term and usertype filter
        LoadUsers(txtSearch.Text.Trim(), ddlUserType.SelectedValue)
    End Sub

    Protected Sub btnAddUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddNew.Click
        Response.Redirect("AddUsers.aspx")
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect("AddUsers.aspx")
    End Sub

    Protected Sub gvUsers_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "EditUser" Then
            ' Redirect to edit page with user ID
            Response.Redirect("EditUsers.aspx?id=" & e.CommandArgument.ToString())
        ElseIf e.CommandName = "DeactivateUser" Then
            ' Handle user deactivation
            Dim userId As Integer = Convert.ToInt32(e.CommandArgument)
            If UpdateUserStatus(userId, "Inactive") Then
                ' Reload the grid
                LoadUsers()
            End If
        ElseIf e.CommandName = "ActivateUser" Then
            ' Handle user activation
            Dim userId As Integer = Convert.ToInt32(e.CommandArgument)
            If UpdateUserStatus(userId, "Active") Then
                ' Reload the grid
                LoadUsers()
            End If
        End If
    End Sub

    Protected Sub gvUsers_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Add status color
            Dim statusCell As TableCell = e.Row.Cells(4) ' Status column (index 4)
            If statusCell.Text = "Active" Then
                statusCell.CssClass = "status-active"
            Else
                statusCell.CssClass = "status-inactive"
            End If

            ' Add confirmation dialog for deactivate button
            Dim btnDeactivate As Button = DirectCast(e.Row.FindControl("btnDeactivate"), Button)
            If btnDeactivate IsNot Nothing Then
                btnDeactivate.OnClientClick = "return confirm('Are you sure you want to deactivate this user?');"
                
                ' Hide deactivate button for admin users
                Dim roleCell As TableCell = e.Row.Cells(3) ' Role column (index 3)
                If roleCell.Text.ToLower() = "admin" Then
                    btnDeactivate.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub gvUsers_PreRender(sender As Object, e As EventArgs)
        ' This event occurs right before the control is rendered
        ' Make sure the GridView has data to display
        If gvUsers.Rows.Count = 0 AndAlso gvUsers.DataSource Is Nothing Then
            ' Set a default empty table as data source if none exists
            gvUsers.DataSource = CreateEmptyTable()
            gvUsers.DataBind()
        End If
    End Sub

    ' Explicit load handler that will be triggered by script
    Protected Sub btnLoadData_Click(sender As Object, e As EventArgs)
        ' Only load if data hasn't been loaded yet
        If hdnDataLoaded.Value = "false" Then
            LoadUsers()
            hdnDataLoaded.Value = "true"
        End If
    End Sub

    Private Function UpdateUserStatus(userId As Integer, status As String) As Boolean
        Try
            ' Convert status string to binary value
            Dim isActive As Integer = If(status = "Active", 1, 0)
            
            ' Build the query to update the user status
            Dim query As String = "UPDATE users SET isActive = @IsActive WHERE UserID = @UserID"
            Using conn As New SqlConnection(ConnectionString)
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.Add(New SqlParameter("@IsActive", isActive))
                    cmd.Parameters.Add(New SqlParameter("@UserID", userId))

                    ' Open connection and execute query
                    conn.Open()
                    Dim rowCount As Integer = cmd.ExecuteNonQuery()
                    
                    ' Debug info about query success
                    Response.Write("<div style='display:none;'>Query success: " & (rowCount > 0) & "</div>")
                    
                    Return rowCount > 0
                End Using
            End Using
        Catch ex As Exception
            ' Log and display error message with more details
            Response.Write("<div style='display:none;'>Exception: " & ex.Message & "</div>")
            Response.Write("<div class='alert alert-danger'>An error occurred while updating user status: " & ex.Message & "</div>")
            
            Return False
        End Try
    End Function
End Class