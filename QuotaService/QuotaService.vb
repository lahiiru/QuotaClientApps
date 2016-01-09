Imports SimpleWifi
Imports SimpleWifi.Win32.Interop
Imports Twitterizer
Imports Newtonsoft
Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.ServiceProcess
Imports Newtonsoft.Json.Linq

Public Class QuotaService
    Public client As SimpleWifi.Win32.WlanClient = New SimpleWifi.Win32.WlanClient
    Public iface As SimpleWifi.Win32.WlanInterface
    Public wc As WebClient = New WebClient
    Public requestHandler As String = "http://localhost/quota_new/Quota/web/app_dev.php/request/user/"
    Public mac As String = ""
    Public myLog As New EventLog()
    ' Set up a timer to trigger every minute.
    Public timer As System.Timers.Timer = New System.Timers.Timer()

    Private ssid As String
    Private pKey As String
    Private skey As String
    Protected Overrides Sub OnStart(ByVal args() As String)

        ssid = args(0)
        pKey = args(1)
        skey = args(2)

        My.Settings.ssid = args(0)
        My.Settings.pKey = args(1)
        My.Settings.sKey = args(2)
        My.Settings.Save()

        'create custom log called Quatalog
        myLog.Log = "QuotaLog"
        myLog.Source = "QuotaSvr"

        Try
            EventLog.CreateEventSource("QuotaSvr", "QuotaLog")
        Catch ex As Exception
        End Try

        Log("starting")

        timer.Interval = 1000 ' 60 seconds
        AddHandler timer.Elapsed, AddressOf Me.OnTimer
        wc.Proxy = Nothing
        iface = client.Interfaces(0)
        mac = iface.NetworkInterface.GetPhysicalAddress().ToString()
        requestHandler = requestHandler & My.Settings.ssid & "/" & mac & "/"
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        Dim retries As Integer = 3
retry:
        connect()
        Thread.Sleep(3000 / retries)
        If isConnectedTo() Then
            Log("#CONNECTED")
            usercheck()
        ElseIf retries > 1 Then
            retries = retries - 1
            Log("Connecting uploading tries " & retries)
            GoTo retry
        Else
            Log("#NOTCONNECTED")

        End If

        'retriveAndSetSettings()
        'check()
        'timer.Start()

    End Sub
    Private Sub OnTimer(sender As Object, e As Timers.ElapsedEventArgs)
        ' TODO: Insert monitoring activities here.
        prepareUsage()
        If (My.Settings.pending > 100000) Then
            uploadData()
        End If
        If Not iface.NetworkInterface.OperationalStatus = NetworkInformation.OperationalStatus.Up Or Not isConnectedTo() Then
            My.Settings.Save()
            timer.Stop()
            Me.Stop()
        End If
    End Sub

    'check for valid user
    Function usercheck() As Boolean
        On Error GoTo err
        Dim url As String = requestHandler & "check"
        Dim response As String = wc.DownloadString(url)
        Dim json As JObject = JObject.Parse(response)
        If (json.SelectToken("status") = "OK") Then
            Log("#MSG:Connection successful")
            Log("#UPDATE:" & response)
            Return True
        ElseIf (json.SelectToken("status") = "BLOCKED") Then
            Log("#MSG:You are blacklisted")
            Log("#UPDATE:" & response)
            disconnect()
        ElseIf (json.SelectToken("status") = "OVER") Then
            Log("#MSG:Your remaining quota is less than 1Mb")
            Log("#UPDATE:" & response)
            disconnect()
        ElseIf (json.SelectToken("status") = "ERROR") Then
            Log("#MSG:Internal server error occured")
        End If
        Return False
        Exit Function
err:
        disconnect()

    End Function

    Sub prepareUsage()
        If Not isConnectedTo() Then
            My.Settings.usage = 0
            Exit Sub
        End If

        Dim kbytes As Integer = (iface.NetworkInterface.GetIPv4Statistics.BytesReceived + iface.NetworkInterface.GetIPv4Statistics.BytesSent) / 1024
        If My.Settings.usage < kbytes Then
            My.Settings.usage = kbytes
        End If
    End Sub
    Protected Overrides Sub OnShutdown()
        Log("Shutting down")
        prepareUsage()
        preparePending()
    End Sub
    Sub Log(msg As String)
        myLog.WriteEntry(msg)
    End Sub
    Sub check()
        If My.Settings.quota < 1000 Or My.Settings.blocked = 1 Then
            disconnect()
            If My.Settings.quota < 1000 Then
                Log("Your  quota is less than 1Mb")
            Else
                Log("You are blacklisted!")
            End If
        End If
    End Sub
    Sub retriveAndSetSettings()
        Dim arr As String() = downloadData()
        My.Settings.blocked = arr(3)
        My.Settings.quota = Val(arr(0))
        My.Settings.Save()
    End Sub
    Sub uploadData()
        Dim retries As Integer = 5
retry:
        If uploadUsage(My.Settings.pending) Then
            My.Settings.pending = 0
        ElseIf retries > 0 Then
            Threading.Thread.Sleep(5000 / retries)
            retries = retries - 1
            Log("Data uploading tries " & retries)
            GoTo retry
        End If


    End Sub
    Function downloadData() As String()
        Dim url As String = requestHandler & "check"
        Dim tries As Integer = 0
re:
        Dim response As String = wc.DownloadString(url)

        If response.Split(";").Count < 4 Then
            Threading.Thread.Sleep(1000)
            tries = tries + 1
            If tries > 5 Then
                disconnect()
                Log("Unable retrive data from server.")
                Stop
            End If
            GoTo re
        End If
        Return response.Split(";")
    End Function

    Function uploadUsage(ByVal kbytes As Integer) As Boolean
        Dim url As String = requestHandler & "usage/" & kbytes.ToString()
        Dim response As String = wc.DownloadString(url)
        Dim json As JObject = JObject.Parse(response)
        If json.SelectToken("status") = "OK" Then
            Return True
        Else
            Return False
        End If
    End Function


    Function isConnectedTo()
        If Not iface.InterfaceState = WlanInterfaceState.Connected Then
            Return False
        End If
        If iface.CurrentConnection.profileName.ToUpper.Contains(My.Settings.ssid.ToUpper()) Then
            Return True
        End If
        Return False
    End Function

    'try to connect using primary_pass_key and secondry_pass_key
    Sub connect()
        connectProcess(My.Settings.pKey)
        If Not isConnectedTo() Then
            If Not skey = "" Then
                connectProcess(My.Settings.sKey)
            End If
        End If
    End Sub
    Sub connectProcess(ByVal pass_key As String)
        Dim profileXml As String = "<?xml version=""1.0""?><WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1""><name>{0}</name><SSIDConfig><SSID><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>manual</connectionMode><MSM><security><authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{1}</keyMaterial></sharedKey></security></MSM><MacRandomization xmlns=""http://www.microsoft.com/networking/WLAN/profile/v3""><enableRandomization>false</enableRandomization></MacRandomization></WLANProfile>"
        profileXml = String.Format(profileXml, My.Settings.ssid, pass_key)
        Log("connect process")
        Try
            iface.Connect(WlanConnectionMode.TemporaryProfile, Dot11BssType.Any, profileXml)
        Catch ex As Exception
            Log("ERROR : Check your wifi connection")
            Log(ex.Message)
        End Try


    End Sub
    Function getRandom(ByVal min As Integer, ByVal max As Integer)
        Dim gen As System.Random = New System.Random()
        Return gen.Next(min, max)
    End Function
    Sub disconnect()
        Dim x = 0
        While My.Computer.Network.IsAvailable And x < 50 And isConnectedTo()
            disconnectProcess()
            Threading.Thread.Sleep(500)
            x = x + 1
        End While

    End Sub
    Sub disconnectProcess()
        iface.Disconnect()
    End Sub
    Protected Overrides Sub OnStop()
        Log("Stopping")
        prepareUsage()
        preparePending()
        uploadData()
        disconnect()
        My.Settings.Save()
        ' Add code here to perform any tear-down necessary to stop your service.
    End Sub
    Protected Overrides Sub OnCustomCommand(command As Integer)
        If (command = 1) Then
            Run(Me)
        End If

    End Sub

    Sub preparePending()
        My.Settings.pending = My.Settings.pending + My.Settings.usage
        My.Settings.usage = 0
    End Sub
End Class
