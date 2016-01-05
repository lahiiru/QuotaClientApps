Imports SimpleWifi
Imports SimpleWifi.Win32.Interop
Imports Twitterizer
Imports Newtonsoft
Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.ServiceProcess

Public Class QuotaService
    Public client As SimpleWifi.Win32.WlanClient = New SimpleWifi.Win32.WlanClient
    Public iface As SimpleWifi.Win32.WlanInterface
    Public wc As WebClient = New WebClient
    Public requestHandler As String = "http://edu.wearetrying.info/quota/requestHandler.php?"
    Public mac As String = ""
    ' Set up a timer to trigger every minute.
    Public timer As System.Timers.Timer = New System.Timers.Timer()

    Protected Overrides Sub OnStart(ByVal args() As String)
        Log("starting")
        'disconectProcess()
        timer.Interval = 1000 ' 60 seconds
        AddHandler timer.Elapsed, AddressOf Me.OnTimer
        wc.Proxy = Nothing
        iface = client.Interfaces(0)
        mac = iface.NetworkInterface.GetPhysicalAddress().ToString()
        requestHandler = requestHandler & "mac=" & mac & "&"
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        Dim retries As Integer = 3
retry:
        connectProcess()
        Thread.Sleep(3000 / retries)
        If isConnectedTo() Then
            Log("Successfully connected.")
        ElseIf retries > 1
            retries = retries - 1
            Log("Connecting uploading tries " & retries)
            GoTo retry
        Else
            Log("Coult'nt connect")
            'Me.Stop()

        End If
        'uploadData()
        'retriveAndSetSettings()
        'check()
        'timer.Start()
    End Sub
    Private Sub OnTimer(sender As Object, e As Timers.ElapsedEventArgs)
        ' TODO: Insert monitoring activities here.
        prepareUsage()
        If Not iface.NetworkInterface.OperationalStatus = NetworkInformation.OperationalStatus.Up Or Not isConnectedTo() Then
            My.Settings.Save()
            timer.Stop()
            Me.Stop()
        End If
    End Sub
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
        EventLog1.WriteEntry(msg)
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
        ElseIf retries > 0
            Threading.Thread.Sleep(5000 / retries)
            retries = retries - 1
            Log("Data uploading tries " & retries)
            GoTo retry
        End If


    End Sub
    Function downloadData() As String()
        Dim url As String = requestHandler & "req=get"
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
        Dim url As String = requestHandler & "req=set&kbytes=" & kbytes.ToString()
        Dim response As String = wc.DownloadString(url)
        If response.ToUpper() = "OK" Then
            Return True
        End If
        Return False
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
    Sub connectProcess()
        Dim profileXml As String = "<?xml version=""1.0""?>
            <WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">
	            <name>{0}</name>
	            <SSIDConfig>
		            <SSID>
			            <name>{0}</name>
		            </SSID>
	            </SSIDConfig>
	            <connectionType>ESS</connectionType>
	            <connectionMode>manual</connectionMode>
	            <MSM>
		            <security>
			            <authEncryption>
				            <authentication>WPA2PSK</authentication>
				            <encryption>AES</encryption>
				            <useOneX>false</useOneX>
			            </authEncryption>
			            <sharedKey>
				            <keyType>passPhrase</keyType>
				            <protected>false</protected>
				            <keyMaterial>{1}</keyMaterial>
			            </sharedKey>
		            </security>
	            </MSM>
	            <MacRandomization xmlns=""http://www.microsoft.com/networking/WLAN/profile/v3"">
		            <enableRandomization>false</enableRandomization>
	            </MacRandomization>
            </WLANProfile>
            "
        profileXml = String.Format(profileXml, My.Settings.ssid, My.Settings.key)
        Log("connect process")
        iface.Connect(WlanConnectionMode.TemporaryProfile, Dot11BssType.Any, profileXml)

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
        disconnect()
        prepareUsage()
        preparePending()
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
