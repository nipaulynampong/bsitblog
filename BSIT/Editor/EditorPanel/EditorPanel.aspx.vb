Imports System.Data
Imports BSIT.Module

Namespace BSIT
    Partial Public Class EditorPanel
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Redirect to login if not logged in
                If Session("EditorID") Is Nothing Then
                    Response.Redirect("~/Editor/EditorLogin.aspx")
                    Return
                End If

                ' Load editor info
                LoadEditorInfo()
            End If
        End Sub

        Private Sub LoadEditorInfo()
            Try
                ' Use Connection class from Module
                Dim conn As New BSIT.Module.Connection()
                
                ' Add parameters
                conn.AddParam("@EditorID", Session("EditorID").ToString())
                conn.AddParam("@UserType", "Editor")
                
                ' Execute query
                Dim query As String = "SELECT Username, FullName FROM Users " & _
                                   "WHERE UserID = @EditorID AND UserType = @UserType"
                
                If conn.Query(query) AndAlso conn.DataCount > 0 Then
                    ' Set editor name from retrieved data
                    lblEditorName.Text = conn.Data.Tables(0).Rows(0)("FullName").ToString()
                    lblUsername.Text = conn.Data.Tables(0).Rows(0)("Username").ToString()
                Else
                    ' If editor not found with correct UserType, redirect to login
                    Session.Clear()
                    Response.Redirect("~/Editor/EditorLogin.aspx")
                End If
            Catch ex As Exception
                ' Log error but don't show to user
                System.Diagnostics.Debug.WriteLine("Error loading editor info: " & ex.Message)
            End Try
        End Sub
    End Class
End Namespace