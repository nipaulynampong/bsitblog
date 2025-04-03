Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Namespace BSIT.Module
    Public Class Connection
        Public ConnectionString As String = "Data Source=NIPAULYN\SQLEXPRESS01;Initial Catalog=blog;Integrated Security=True"

        Public Connect As New SqlConnection(ConnectionString)

        Public Parameters As New List(Of SqlParameter)

        Public Data As DataSet

        Public DataCount As Integer

        Public Sub Open()
            If Connect.State = ConnectionState.Closed Then
                Connect.Open()
            End If
        End Sub

        Public Sub Close()
            If Connect.State = ConnectionState.Open Then
                Connect.Close()
            End If
        End Sub

        Public Sub AddParam(ByVal key As String, ByVal value As String)
            Parameters.Add(New SqlParameter(key, value))
        End Sub
        Public Function Query(ByVal command_query As String) As Boolean
            Open()

            Dim command As New SqlCommand(command_query, Connect)

            If Parameters.Count > 0 Then
                For Each param As SqlParameter In Parameters
                    command.Parameters.Add(param)
                Next
                Parameters.Clear()
            End If

            If command_query.Contains("SELECT SCOPE_IDENTITY()") Then
                ' This is an INSERT query that also wants to return the new ID
                Try
                    Dim result As Object = command.ExecuteScalar()
                    Data = New DataSet()
                    Dim dt As New DataTable()
                    dt.Columns.Add("ID")
                    Dim row As DataRow = dt.NewRow()
                    
                    ' Handle the case when result is NULL
                    If result Is Nothing OrElse DBNull.Value.Equals(result) Then
                        row("ID") = DBNull.Value
                    Else
                        row("ID") = result
                    End If
                    
                    dt.Rows.Add(row)
                    Data.Tables.Add(dt)
                    DataCount = 1
                    Close()
                    Return True
                Catch ex As Exception
                    ' Log the exception details for debugging
                    Console.WriteLine("Error in Query method: " & ex.Message)
                    Close()
                    Return False
                End Try
            ElseIf command_query.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) OrElse
               command_query.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase) OrElse
               command_query.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase) Then

                Dim rowsAffected As Integer = command.ExecuteNonQuery()

                Return rowsAffected > 0

            ElseIf command_query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) Then

                Dim adapter As New SqlDataAdapter(command)
                Data = New DataSet()
                DataCount = adapter.Fill(Data)
                Close()

                Return True
            Else
                Close()
                Return False
            End If
        End Function
    End Class
End Namespace
