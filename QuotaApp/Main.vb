Imports System.Net
Imports SimpleWifi.Win32

Module Main
    Public iface As SimpleWifi.Win32.WlanInterface = Nothing
    Public requestHandler As String = "http://edu.wearetrying.info/quota/requestHandler.php?"
    Public mac As String = ""
    Public WebRequest As WebClient = New WebClient
    Public client As WlanClient = New WlanClient()
    Public wlanIface As WlanInterface
    Public serviceName As String = "Quota2"
    Public myLog As New EventLog()
    Public mainForm As New Form1
    Sub Main()
        Dim slash As New SplashScreen1
        slash.Show()
        Application.DoEvents()
        Application.DoEvents()
        'On Error GoTo e107
        iface = client.Interfaces(0)

        If IsNothing(iface) Then
            MsgBox("Couldn't find wifi adapter!", MsgBoxStyle.Information)
            Exit Sub
        End If
        'On Error GoTo e108
        mac = iface.NetworkInterface.GetPhysicalAddress.ToString()
        requestHandler = requestHandler & "mac=" & mac & "&"
        WebRequest.Headers(HttpRequestHeader.UserAgent) = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727)"
        WebRequest.Proxy = Nothing

        myLog.Log = "QuotaLog"
        AddHandler myLog.EntryWritten, AddressOf mainForm.processLog
        Try
            myLog.EnableRaisingEvents = True
        Catch ex As Exception

        End Try

        'On Error GoTo e109

        mainForm.ServiceController1.ServiceName = serviceName
        mainForm.loadForm()
        mainForm.Timer1.Enabled = True
        slash.Hide()
        'On Error Resume Next
        Application.Run(mainForm)
        Exit Sub
e107:
        MsgBox("Error 107 occured!" & Err.Description & vbNewLine & Err.Source, MsgBoxStyle.Exclamation, "Error")
        Exit Sub
e108:
        MsgBox("Error 108 occured!" & Err.Description & vbNewLine & Err.Source, MsgBoxStyle.Exclamation, "Error")
        Exit Sub
e109:
        MsgBox("Error 109 occured!" & Err.Description & vbNewLine & Err.Source, MsgBoxStyle.Exclamation, "Error")


    End Sub

End Module
