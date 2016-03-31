Imports System.Net
Imports SimpleWifi.Win32

Module Main
    Public iface As SimpleWifi.Win32.WlanInterface = Nothing
    Public requestHandler As String = "http://52.24.88.15/quota2/web/app.php/request"
    Public mac As String = ""
    Public wc As WebClient = New WebClient
    Public client As WlanClient = New WlanClient()
    Public wlanIface As WlanInterface
    Public serviceName As String = "Quota2"
    Public myLog As New EventLog()
    Public mainForm As New Form1
    Public slash As New SplashScreen1
    Public maxPackage As Integer = 25000000 'max package a new user can request
    Public isSecMode As Integer = 0
    Public connectRequest As Boolean = False
    Public disconnectRequest As Boolean = False
    Public utname As String = "N/A"
    Public recentlyUploaded As Boolean = False
    Public timestamp As String = Now.ToString("yyyy-MM-dd HH:mm:ss")

    Sub Main()

        slash.Show()
        Application.DoEvents()
        'On Error GoTo e107
        iface = client.Interfaces(0)
        mainForm.WebBrowser1.Navigate("about:blank")
        mainForm.WebBrowser1.Document.Write("<body bgcolor='#7f7f7f'></body>")
        Application.DoEvents()
        If IsNothing(iface) Then
            MsgBox("Couldn't find wifi adapter!", MsgBoxStyle.Information)
            Exit Sub
        End If
        'On Error GoTo e108
        mac = iface.NetworkInterface.GetPhysicalAddress.ToString()
        requestHandler = requestHandler & "/user/" & Web.HttpUtility.UrlPathEncode(My.Settings.bssid) & "/" & mac & "/"
        wc.Headers(HttpRequestHeader.UserAgent) = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727)"
        wc.Proxy = Nothing
        Application.DoEvents()
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
