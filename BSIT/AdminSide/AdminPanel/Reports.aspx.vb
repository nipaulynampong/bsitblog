Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Namespace BSIT
    Public Class Reports
        Inherits System.Web.UI.Page

        ' Control declarations
        Protected WithEvents txtStartDate As System.Web.UI.WebControls.TextBox
        Protected WithEvents txtEndDate As System.Web.UI.WebControls.TextBox
        Protected WithEvents ddlDateRange As System.Web.UI.WebControls.DropDownList
        Protected WithEvents btnApplyDates As System.Web.UI.WebControls.Button
        Protected WithEvents litTotalPosts As System.Web.UI.WebControls.Literal
        Protected WithEvents litTotalViews As System.Web.UI.WebControls.Literal
        Protected WithEvents litTotalComments As System.Web.UI.WebControls.Literal
        Protected WithEvents litActiveUsers As System.Web.UI.WebControls.Literal
        Protected WithEvents btnExportViews As System.Web.UI.WebControls.Button
        Protected WithEvents btnExportPosts As System.Web.UI.WebControls.Button
        Protected WithEvents btnExportCategories As System.Web.UI.WebControls.Button
        Protected WithEvents gvTopPosts As System.Web.UI.WebControls.GridView
        Protected WithEvents gvCategories As System.Web.UI.WebControls.GridView

        Private Const ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Check if user is logged in and is admin
                If Session("UserID") Is Nothing Then
                    Response.Redirect("~/Login.aspx")
                End If

                ' Set default date range (last 30 days)
                txtStartDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd")
                txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd")

                ' Load the reports data
                LoadReportData()
            End If
        End Sub

        Protected Sub ddlDateRange_SelectedIndexChanged(sender As Object, e As EventArgs)
            Dim days As Integer = Convert.ToInt32(ddlDateRange.SelectedValue)
            
            If days = 0 Then
                ' All time - set to a year ago or more
                txtStartDate.Text = DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd")
            Else
                txtStartDate.Text = DateTime.Now.AddDays(-days).ToString("yyyy-MM-dd")
            End If
            
            txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
            
            ' Reload the data
            LoadReportData()
        End Sub

        Protected Sub btnApplyDates_Click(sender As Object, e As EventArgs)
            LoadReportData()
        End Sub

        Private Sub LoadReportData()
            Try
                Dim startDate As DateTime
                Dim endDate As DateTime

                If DateTime.TryParse(txtStartDate.Text, startDate) AndAlso DateTime.TryParse(txtEndDate.Text, endDate) Then
                    ' Add one day to end date to include the entire end date
                    endDate = endDate.AddDays(1).AddSeconds(-1)

                    ' Load summary statistics
                    LoadSummaryStatistics(startDate, endDate)

                    ' Load top posts data
                    LoadTopPosts(startDate, endDate)

                    ' Load category distribution data
                    LoadCategoryDistribution(startDate, endDate)
                Else
                    ' Invalid date format
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "alert", "alert('Please enter valid dates.');", True)
                End If
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "alert", "alert('Error loading report data: " & ex.Message.Replace("'", "\'") & "');", True)
            End Try
        End Sub

        Private Sub LoadSummaryStatistics(startDate As DateTime, endDate As DateTime)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()

                ' Get total posts in date range
                Dim cmdPosts As New SqlCommand("SELECT COUNT(*) FROM Posts WHERE CreatedDate BETWEEN @StartDate AND @EndDate", connection)
                cmdPosts.Parameters.AddWithValue("@StartDate", startDate)
                cmdPosts.Parameters.AddWithValue("@EndDate", endDate)
                litTotalPosts.Text = cmdPosts.ExecuteScalar().ToString()

                ' Get total views in date range
                Dim cmdViews As New SqlCommand("SELECT SUM(Views) FROM Posts WHERE CreatedDate BETWEEN @StartDate AND @EndDate", connection)
                cmdViews.Parameters.AddWithValue("@StartDate", startDate)
                cmdViews.Parameters.AddWithValue("@EndDate", endDate)
                Dim totalViews As Object = cmdViews.ExecuteScalar()
                litTotalViews.Text = If(totalViews IsNot DBNull.Value AndAlso totalViews IsNot Nothing, totalViews.ToString(), "0")

                ' Get total comments in date range
                Dim cmdComments As New SqlCommand("SELECT COUNT(*) FROM Comments WHERE CreatedDate BETWEEN @StartDate AND @EndDate", connection)
                cmdComments.Parameters.AddWithValue("@StartDate", startDate)
                cmdComments.Parameters.AddWithValue("@EndDate", endDate)
                litTotalComments.Text = cmdComments.ExecuteScalar().ToString()

                ' Get active users in date range
                Dim cmdUsers As New SqlCommand("SELECT COUNT(DISTINCT UserID) FROM (SELECT UserID FROM Posts WHERE CreatedDate BETWEEN @StartDate AND @EndDate UNION SELECT UserID FROM Comments WHERE CreatedDate BETWEEN @StartDate AND @EndDate) AS ActiveUsers", connection)
                cmdUsers.Parameters.AddWithValue("@StartDate", startDate)
                cmdUsers.Parameters.AddWithValue("@EndDate", endDate)
                litActiveUsers.Text = cmdUsers.ExecuteScalar().ToString()
            Catch ex As Exception
                Throw New Exception("Error loading summary statistics: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Private Sub LoadTopPosts(startDate As DateTime, endDate As DateTime)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                
                Dim cmdTopPosts As New SqlCommand("SELECT TOP 10 p.PostID, p.Title, u.Username AS Author, p.CreatedDate, p.Views, " & _
                                                "(SELECT COUNT(*) FROM Comments c WHERE c.PostID = p.PostID) AS Comments, " & _
                                                "cat.CategoryName AS Category " & _
                                                "FROM Posts p " & _
                                                "INNER JOIN Users u ON p.UserID = u.UserID " & _
                                                "LEFT JOIN Categories cat ON p.CategoryID = cat.CategoryID " & _
                                                "WHERE p.CreatedDate BETWEEN @StartDate AND @EndDate " & _
                                                "ORDER BY p.Views DESC", connection)
                cmdTopPosts.Parameters.AddWithValue("@StartDate", startDate)
                cmdTopPosts.Parameters.AddWithValue("@EndDate", endDate)
                
                Dim adapter As New SqlDataAdapter(cmdTopPosts)
                Dim dtTopPosts As New DataTable()
                adapter.Fill(dtTopPosts)
                
                gvTopPosts.DataSource = dtTopPosts
                gvTopPosts.DataBind()
            Catch ex As Exception
                Throw New Exception("Error loading top posts: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Private Sub LoadCategoryDistribution(startDate As DateTime, endDate As DateTime)
            Dim connection As New SqlConnection(ConnectionString)
            Try
                connection.Open()
                
                Dim cmdCategories As New SqlCommand("SELECT c.CategoryID, c.CategoryName, " & _
                                                   "COUNT(p.PostID) AS PostCount, " & _
                                                   "SUM(p.Views) AS ViewCount, " & _
                                                   "CASE WHEN COUNT(p.PostID) > 0 THEN CAST(SUM(p.Views) AS FLOAT) / COUNT(p.PostID) ELSE 0 END AS AverageViews " & _
                                                   "FROM Categories c " & _
                                                   "LEFT JOIN Posts p ON c.CategoryID = p.CategoryID AND p.CreatedDate BETWEEN @StartDate AND @EndDate " & _
                                                   "GROUP BY c.CategoryID, c.CategoryName " & _
                                                   "ORDER BY ViewCount DESC", connection)
                cmdCategories.Parameters.AddWithValue("@StartDate", startDate)
                cmdCategories.Parameters.AddWithValue("@EndDate", endDate)
                
                Dim adapter As New SqlDataAdapter(cmdCategories)
                Dim dtCategories As New DataTable()
                adapter.Fill(dtCategories)
                
                gvCategories.DataSource = dtCategories
                gvCategories.DataBind()
            Catch ex As Exception
                Throw New Exception("Error loading category distribution: " & ex.Message)
            Finally
                connection.Close()
            End Try
        End Sub

        Protected Sub btnExportViews_Click(sender As Object, e As EventArgs)
            ExportToCSV("page_views", GetPageViewsData())
        End Sub

        Protected Sub btnExportPosts_Click(sender As Object, e As EventArgs)
            ExportToCSV("top_posts", GetTopPostsData())
        End Sub

        Protected Sub btnExportCategories_Click(sender As Object, e As EventArgs)
            ExportToCSV("categories", GetCategoriesData())
        End Sub

        Private Function GetPageViewsData() As DataTable
            Dim connection As New SqlConnection(ConnectionString)
            Dim dtPageViews As New DataTable()

            Try
                connection.Open()
                
                Dim startDate As DateTime = DateTime.Parse(txtStartDate.Text)
                Dim endDate As DateTime = DateTime.Parse(txtEndDate.Text).AddDays(1).AddSeconds(-1)
                
                Dim cmdPageViews As New SqlCommand("SELECT CONVERT(varchar, CreatedDate, 101) AS Date, SUM(Views) AS Views " & _
                                                  "FROM Posts " & _
                                                  "WHERE CreatedDate BETWEEN @StartDate AND @EndDate " & _
                                                  "GROUP BY CONVERT(varchar, CreatedDate, 101) " & _
                                                  "ORDER BY Date", connection)
                cmdPageViews.Parameters.AddWithValue("@StartDate", startDate)
                cmdPageViews.Parameters.AddWithValue("@EndDate", endDate)
                
                Dim adapter As New SqlDataAdapter(cmdPageViews)
                adapter.Fill(dtPageViews)
            Catch ex As Exception
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "alert", "alert('Error exporting views data: " & ex.Message.Replace("'", "\'") & "');", True)
            Finally
                connection.Close()
            End Try

            Return dtPageViews
        End Function

        Private Function GetTopPostsData() As DataTable
            Dim dtTopPosts As New DataTable()
            
            ' Create columns based on visible columns in GridView
            For Each field As DataControlField In gvTopPosts.Columns
                If field.Visible Then
                    dtTopPosts.Columns.Add(field.HeaderText)
                End If
            Next
            
            ' Get the DataSource directly if it's still available
            If gvTopPosts.DataSource IsNot Nothing Then
                ' Use the existing data source
                Dim sourceTable As DataTable = DirectCast(gvTopPosts.DataSource, DataSet).Tables(0)
                
                For Each sourceRow As DataRow In sourceTable.Rows
                    Dim row As DataRow = dtTopPosts.NewRow()
                    Dim colIndex As Integer = 0
                    
                    For i As Integer = 0 To gvTopPosts.Columns.Count - 1
                        If gvTopPosts.Columns(i).Visible Then
                            If i = 0 Then ' Skip PostID column
                                colIndex += 1
                                Continue For
                            End If
                            
                            If TypeOf gvTopPosts.Columns(i) Is TemplateField AndAlso i = 1 Then
                                ' This is the title and author template field
                                Dim title As String = sourceRow("Title").ToString()
                                Dim author As String = sourceRow("Author").ToString()
                                row(colIndex) = title & " by " & author
                            Else
                                ' For other fields, use the column name to get data
                                Dim dataField As String = gvTopPosts.Columns(i).HeaderText
                                Dim sourceField As String = String.Empty
                                
                                ' Map header text to data field
                                Select Case dataField
                                    Case "Views"
                                        sourceField = "Views"
                                    Case "Comments"
                                        sourceField = "Comments"
                                    Case "Category"
                                        sourceField = "Category"
                                    Case Else
                                        sourceField = dataField
                                End Select
                                
                                If sourceTable.Columns.Contains(sourceField) Then
                                    row(colIndex) = sourceRow(sourceField).ToString()
                                Else
                                    row(colIndex) = ""
                                End If
                            End If
                            
                            colIndex += 1
                        End If
                    Next
                    
                    dtTopPosts.Rows.Add(row)
                Next
            Else
                ' Fallback to extracting data from grid cells directly
                For Each gridRow As GridViewRow In gvTopPosts.Rows
                    Dim row As DataRow = dtTopPosts.NewRow()
                    Dim colIndex As Integer = 0
                    
                    For i As Integer = 0 To gvTopPosts.Columns.Count - 1
                        If gvTopPosts.Columns(i).Visible Then
                            If TypeOf gvTopPosts.Columns(i) Is TemplateField Then
                                ' For template fields, we'll just use a placeholder
                                row(colIndex) = "Post #" & (gridRow.RowIndex + 1)
                            Else
                                row(colIndex) = gridRow.Cells(i).Text.Replace("&nbsp;", "")
                            End If
                            
                            colIndex += 1
                        End If
                    Next
                    
                    dtTopPosts.Rows.Add(row)
                Next
            End If
            
            Return dtTopPosts
        End Function

        Private Function GetCategoriesData() As DataTable
            Dim dtCategories As New DataTable()
            
            For Each col As DataControlField In gvCategories.Columns
                If col.Visible Then
                    dtCategories.Columns.Add(col.HeaderText)
                End If
            Next
            
            For Each row As GridViewRow In gvCategories.Rows
                Dim dataRow As DataRow = dtCategories.NewRow()
                Dim colIndex As Integer = 0
                
                For i As Integer = 0 To gvCategories.Columns.Count - 1
                    If gvCategories.Columns(i).Visible Then
                        dataRow(colIndex) = row.Cells(i).Text.Replace("&nbsp;", "")
                        colIndex += 1
                    End If
                Next
                
                dtCategories.Rows.Add(dataRow)
            Next
            
            Return dtCategories
        End Function

        Private Sub ExportToCSV(fileName As String, dt As DataTable)
            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition", "attachment;filename=" & fileName & "_" & DateTime.Now.ToString("yyyyMMdd") & ".csv")
            Response.Charset = ""
            Response.ContentType = "application/text"
            
            Dim sb As New StringBuilder()
            
            ' Add headers
            For i As Integer = 0 To dt.Columns.Count - 1
                sb.Append(dt.Columns(i).ColumnName)
                If i < dt.Columns.Count - 1 Then
                    sb.Append(",")
                End If
            Next
            sb.Append(vbCrLf)
            
            ' Add rows
            For Each row As DataRow In dt.Rows
                For i As Integer = 0 To dt.Columns.Count - 1
                    sb.Append("""" & row(i).ToString().Replace("""", """""") & """")
                    If i < dt.Columns.Count - 1 Then
                        sb.Append(",")
                    End If
                Next
                sb.Append(vbCrLf)
            Next
            
            Response.Output.Write(sb.ToString())
            Response.Flush()
            Response.End()
        End Sub
    End Class
End Namespace