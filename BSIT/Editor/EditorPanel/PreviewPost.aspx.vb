Imports BSIT.Module

Public Class PreviewPost
    Inherits System.Web.UI.Page

    ' Use the Connection class from the BSIT.Module namespace
    Private conn As New BSIT.Module.Connection()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadPreviewData()
        End If
    End Sub

    Private Sub LoadPreviewData()
        ' Check if preview data exists in session
        If Session("PreviewTitle") IsNot Nothing Then
            litTitle.Text = Session("PreviewTitle").ToString()
        Else
            litTitle.Text = "Sample Post Title"
        End If

        ' Load content
        If Session("PreviewContent") IsNot Nothing Then
            litContent.Text = Session("PreviewContent").ToString()
        Else
            litContent.Text = "<p>This is sample content. No actual content was provided for preview.</p>"
        End If

        ' Load excerpt if available
        If Session("PreviewExcerpt") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("PreviewExcerpt").ToString()) Then
            litExcerpt.Text = Session("PreviewExcerpt").ToString()
            pnlExcerpt.Visible = True
        End If

        ' Load featured image if available
        If Session("PreviewImageUrl") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("PreviewImageUrl").ToString()) Then
            imgFeatured.Src = ResolveUrl(Session("PreviewImageUrl").ToString())
            pnlFeaturedImage.Visible = True
        End If
    End Sub
End Class 