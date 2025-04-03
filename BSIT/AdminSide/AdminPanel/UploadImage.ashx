<%@ WebHandler Language="VB" Class="UploadImage" %>

Imports System
Imports System.Web
Imports System.IO
Imports System.Web.Script.Serialization

Public Class UploadImage : Implements IHttpHandler
    
    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        ' Check if user is authenticated
        If context.Session("AdminID") Is Nothing Then
            context.Response.ContentType = "application/json"
            context.Response.Write("{""error"":""Not authorized""}")
            Return
        End If
        
        ' Check if this is a POST request with a file
        If Not context.Request.Files.Count > 0 Then
            context.Response.ContentType = "application/json"
            context.Response.Write("{""error"":""No file uploaded""}")
            Return
        End If
        
        Try
            ' Get the file from the request
            Dim file As HttpPostedFile = context.Request.Files(0)
            
            ' Validate file type
            Dim allowedTypes As String() = {"image/jpeg", "image/png", "image/gif", "image/webp"}
            If Not Array.Exists(allowedTypes, Function(t) t = file.ContentType.ToLower()) Then
                context.Response.ContentType = "application/json"
                context.Response.Write("{""error"":""Invalid file type. Only JPEG, PNG, GIF, and WebP images are allowed.""}")
                Return
            End If
            
            ' Validate file size (max 5MB)
            Dim maxSize As Integer = 5 * 1024 * 1024 ' 5MB
            If file.ContentLength > maxSize Then
                context.Response.ContentType = "application/json"
                context.Response.Write("{""error"":""File too large. Maximum allowed size is 5MB.""}")
                Return
            End If
            
            ' Create uploads directory if it doesn't exist
            Dim uploadsDir As String = context.Server.MapPath("~/uploads/")
            If Not Directory.Exists(uploadsDir) Then
                Directory.CreateDirectory(uploadsDir)
            End If
            
            ' Generate unique filename
            Dim fileExtension As String = Path.GetExtension(file.FileName)
            Dim uniqueFileName As String = Guid.NewGuid().ToString() & fileExtension
            Dim filePath As String = Path.Combine(uploadsDir, uniqueFileName)
            
            ' Save the file
            file.SaveAs(filePath)
            
            ' Generate URL for the uploaded file
            Dim fileUrl As String = context.Request.Url.GetLeftPart(UriPartial.Authority) & 
                context.Request.ApplicationPath.TrimEnd("/") & "/uploads/" & uniqueFileName
            
            ' Return the URL as JSON
            context.Response.ContentType = "application/json"
            Dim serializer As New JavaScriptSerializer()
            Dim result As New Dictionary(Of String, String)()
            result.Add("location", fileUrl)
            context.Response.Write(serializer.Serialize(result))
            
        Catch ex As Exception
            ' Handle errors
            context.Response.ContentType = "application/json"
            context.Response.Write("{""error"":""" & ex.Message.Replace("""", "\""") & """}")
        End Try
    End Sub
    
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
    
End Class 