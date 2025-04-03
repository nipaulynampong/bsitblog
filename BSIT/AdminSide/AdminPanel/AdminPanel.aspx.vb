Public Class AdminPanel
    Inherits System.Web.UI.Page
    
    ' Add protected declaration for the control
    Protected WithEvents lblAdminName As Label
    Protected WithEvents lnkLogout As LinkButton

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Check if user is logged in and is an admin
        If Session("AdminID") Is Nothing Then
            ' Not logged in, redirect to login page
            Response.Redirect("~/AdminSide/AdminLogin.aspx")
        End If
        
        ' Set the admin name from session
        If Not IsPostBack Then
            If Not Session("AdminName") Is Nothing Then
                lblAdminName.Text = Session("AdminName").ToString()
            End If
        End If
    End Sub
    
    Protected Sub lnkLogout_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Clear all session variables
        Session.Clear()
        Session.Abandon()
        
        ' Redirect to login page
        Response.Redirect("~/AdminSide/AdminLogin.aspx")
    End Sub
End Class