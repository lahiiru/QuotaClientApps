Imports System.Net
Imports System.Net.NetworkInformation
Imports System.Threading
Imports System.Configuration
Imports System.IO
Imports SimpleWifi.Win32

Module Main
    Public iface As WlanInterface = Nothing
    Public requestHandlerBase As String = "http://quota.wearetrying.info/request"
    Public updateIndexURL As String = "http://wearetrying.info/builds/quota/index.txt"
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
    Public offlineUsage As Integer = 0
    Public network As ArrayList
    Public downSpeed As Integer = 0
    Public counterThreadLive As Boolean = False
    Public nif As NetworkInterface
    Public mainFormClosed As Boolean = False
    Public curr_ssid As String = "not-set"
    Public M As Mutex
    Sub Main()
        slash.Show()
        Application.DoEvents()
        Try
            Dim M = Mutex.OpenExisting("Quota")
            MsgBox("Quota already is running." & vbNewLine & "Please check task manager.", MsgBoxStyle.Exclamation, "Quota starter")
            Environment.Exit(0)
        Catch ex As Exception
            Dim M = New Mutex(True, "Quota")
        End Try
        'On Error GoTo e107
        Try
            iface = client.Interfaces(0)
            nif = iface.NetworkInterface
        Catch ex As Exception
            MsgBox("Couldn't find wifi adapter!", MsgBoxStyle.Exclamation)
            Exit Sub
        End Try

        mainForm.WebBrowser1.Navigate("about: blank")
        mainForm.WebBrowser1.Document.Write("<body bgcolor='#3C8DBC'></body>")
        Application.DoEvents()
        If IsNothing(iface) Then
            MsgBox("Couldn't find wifi adapter!", MsgBoxStyle.Information)
            Exit Sub
        End If

        'Handle user.config corruption
        Try
            My.Settings.Reload()
            Dim x = My.Settings.bssid
        Catch ex As Exception
            Dim e As ConfigurationErrorsException
            e = ex.InnerException
            Dim filename As String = e.Filename
            If MsgBox("Settings are corrupted. Press OK to reset." & vbNewLine & ex.Message, MsgBoxStyle.OkCancel, "Startup error") = MsgBoxResult.Ok Then
                File.Delete(filename)
                My.Settings.Reload()
            Else
                End
            End If
        End Try
        mac = iface.NetworkInterface.GetPhysicalAddress.ToString()
        wc.Headers(HttpRequestHeader.UserAgent) = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727)"
        wc.Proxy = Nothing

        WebRequest.DefaultWebProxy = Nothing

        ServicePointManager.DefaultConnectionLimit = 100

        Application.DoEvents()

        'create custom log called Quatalog
        myLog.Log = "QuotaLog"
        myLog.Source = "QuotaSvr"

        Try
            EventLog.CreateEventSource("QuotaSvr", "QuotaLog")
        Catch ex As Exception
        End Try

        AddHandler myLog.EntryWritten, AddressOf mainForm.processLog
        Try
            myLog.EnableRaisingEvents = True
        Catch ex As Exception

        End Try

        'On Error GoTo e109
        mainForm.ServiceController1.ServiceName = serviceName
        mainForm.loadForm()
        mainForm.Timer1.Enabled = True

        network = New ArrayList
        For j As Integer = 0 To 10
            network.Add(0)
        Next

        If Not counterThreadLive Then
            Dim c As Thread
            c = New Thread(AddressOf mainForm.SpeedCounter)
            c.Start()
        End If

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
    Function GetRequestHandlerURL() As String
        Return requestHandlerBase & "/user/" & Web.HttpUtility.UrlPathEncode(curr_ssid) & "/" & mac & "/"
    End Function

End Module
