Imports System.Net
Imports System.Text.RegularExpressions

Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If maxPackage >= TrackBar1.Minimum * 100000 Then
            TrackBar1.Maximum = maxPackage / 100000.0
        End If
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Label3.Text = (TrackBar1.Value / 10.0).ToString("N2") & " GB"
    End Sub
    Private Sub TextBox3_KeyPress(sender As Object, e As KeyPressEventArgs) _
                              Handles TextBox3.KeyPress

        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyz 0123456789.,"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If

    End Sub
    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) _
                              Handles TextBox1.KeyPress

        If Not (Asc(e.KeyChar) = 8) Then
            Dim allowedChars As String = "abcdefghijklmnopqrstuvwxyz"
            If Not allowedChars.Contains(e.KeyChar.ToString.ToLower) Then
                e.KeyChar = ChrW(0)
                e.Handled = True
            End If
        End If

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim name As New Regex("^[a-zA-Z]+$")
        Dim msg As New Regex("^[a-zA-Z0-9 ,.]+$")

        If TextBox1.Text.Length < 3 Or TrackBar1.Value < 1 Or Not msg.IsMatch(TextBox3.Text) Or Not name.IsMatch(TextBox1.Text) Then
            MsgBox("Invalid name or package size", MsgBoxStyle.Exclamation, "Invalid")
            Exit Sub
        End If
        isSecMode = 1 'connect for 2 seconds
        connectRequest = True
        Threading.Thread.Sleep(1000)
        isSecMode = 0
        sendNewRequest(TextBox1.Text, TrackBar1.Value * 100000, TextBox3.Text)
    End Sub
    Function sendNewRequest(ByVal name, ByVal kbytes, ByVal msg) As Boolean

        Try
            Button1.Enabled = False
            AddHandler wc.DownloadStringCompleted, AddressOf OnDlComplete
            AddHandler wc.DownloadProgressChanged, AddressOf OnChangeComplete

            Dim s As String = String.Format("{0}new/{1}/{2}/{3}", GetRequestHandlerURL(), name, kbytes, Web.HttpUtility.UrlPathEncode(msg))
            Log("Sending new client request: " & s)
            wc.DownloadStringAsync(New Uri(s))
            Return True
        Catch ex As Exception
            MsgBox("Error in comunication. " & ex.Message, MsgBoxStyle.Information, "Error")
            Button1.Enabled = True
            Return False
        End Try
    End Function
    Sub Log(msg As String)
        myLog.WriteEntry(msg)
    End Sub
    Private Sub OnChangeComplete(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        Button1.Text = e.ProgressPercentage
    End Sub

    Private Sub OnDlComplete(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs)
        disconnectRequest = True  'disconnect

        If Button1.Enabled Then
            Exit Sub
        End If

        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            Log("New client request response: " & e.Result)
            If e.Result.Equals("OK") Then
                MsgBox("Your registration request is sent for approval.", MsgBoxStyle.Information, "Success")
                Application.ExitThread()
            Else
                MsgBox("Couldn't process your request", MsgBoxStyle.Exclamation, "Error")
            End If
        Else
            Log("New client request error: " & e.Error.StackTrace)
            MsgBox("Error: " & e.Error.Message, MsgBoxStyle.Exclamation, "Error")
        End If

        Button1.Text = "JOIN"
        Button1.Enabled = True
    End Sub
End Class