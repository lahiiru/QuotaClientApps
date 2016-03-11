Imports SimpleWifi
Imports SimpleWifi.Win32.Interop
Imports Twitterizer
Imports Newtonsoft
Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.ServiceProcess
Imports Newtonsoft.Json.Linq
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Collections.Specialized
Imports System.Configuration

Public Class QuotaService
    Public client As SimpleWifi.Win32.WlanClient = New SimpleWifi.Win32.WlanClient
    Public iface As SimpleWifi.Win32.WlanInterface
    Public wc As WebClient = New WebClient
    Public requestHandler As String = "http://edu.wearetrying.info/quota2/web/app.php/request/user/"
    Public mac As String = ""
    Public myLog As New EventLog()
    ' Set up a timer to trigger every minute.
    Public timer As System.Timers.Timer = New System.Timers.Timer()
    Private irregularStop As Boolean = False 'whether the stop is not healthy or not
    Private ssid As String = ""
    Private customKey As String = ""
    Private keyIndex As Integer = 0
    Private lastUsage As Integer = 0
    Private usage As Integer = 0
    Private appPID As Integer = 0
    Private initialUsage As Integer = 0 'correction for win7
    Dim keys As NameValueCollection()
    Protected Overrides Sub OnStart(ByVal args() As String)
        handleUserConfigCorruption()
        'System.Diagnostics.Debugger.Launch()
        My.Settings.st = 0  'set successfully terminated to false
        My.Settings.Save()

        ssid = args(0)
        appPID = args(1)
        customKey = args(2)



        'create custom log called Quatalog
        myLog.Log = "QuotaLog"
        myLog.Source = "QuotaSvr"

        Try
            EventLog.CreateEventSource("QuotaSvr", "QuotaLog")
        Catch ex As Exception
        End Try

        Log("starting")

        timer.Interval = 2000 ' 60 seconds
        AddHandler timer.Elapsed, AddressOf Me.OnTimer
        wc.Proxy = Nothing
        Try
            iface = client.Interfaces(0)
            mac = iface.NetworkInterface.GetPhysicalAddress().ToString()
        Catch ex As Exception
            Log("error! " & ex.StackTrace & ex.Message)
            irregularStop = True
            Me.Stop()
        End Try

        requestHandler = requestHandler & ssid & "/" & mac & "/"
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        Dim retries As Integer = 2
        'loading keys
        keys = loadKeys()
        validateApp()
retry:
        connect()
        Thread.Sleep(500 / retries)
        If isConnectedTo() Then
            Log("#CONNECTED")
            setInitialUsage()
            irregularStop = False
            usercheck()
        ElseIf retries > 1 Then
            retries = retries - 1
            Log("Connecting retries " & retries)
            GoTo retry
        Else
            Log("#NOTCONNECTED")
            irregularStop = True
            Me.Stop()
        End If
        'retriveAndSetSettings()
        'check()
        timer.Start()

    End Sub
    Sub handleUserConfigCorruption()
        Dim isConfigurationValid As Boolean = False
        While Not isConfigurationValid
            Try
                Dim appSettings As AppSettingsSection = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).AppSettings
                isConfigurationValid = True
            Catch e As ConfigurationErrorsException
                If e.Filename.EndsWith("user.config") Then
                    File.Delete(e.Filename)
                End If
                Log("User configurations were deleted due to corruption.")
            End Try
        End While
    End Sub
    Sub validateApp()
        Try
            Dim p As Process = Process.GetProcessById(appPID)
            Log("Debug code 114")
        Catch ex As Exception
            disconnect()
            irregularStop = True
            Me.Stop()
        End Try
    End Sub
    Sub checkForSt()
        Dim kbytes As Integer = (iface.NetworkInterface.GetIPv4Statistics.BytesReceived + iface.NetworkInterface.GetIPv4Statistics.BytesSent) / 1024
        If usage < kbytes Then
            usage = kbytes
        End If
    End Sub

    Private Sub OnTimer(sender As Object, e As Timers.ElapsedEventArgs)
        prepareUsage()
        preparePending()
        If (Not iface.NetworkInterface.OperationalStatus = NetworkInformation.OperationalStatus.Up) Or (Not isConnectedTo()) Then
            My.Settings.Save()
            Log("Debug code 113")
            Me.Stop()
        End If
        If (My.Settings.pending > 20000) Then
            Log("Debug code 112")
            uploadData()
        End If
        validateApp()
    End Sub
    Sub saveKeys(ByVal UserSelection As NameValueCollection())
        Using fs As New FileStream("DataFile.dat", FileMode.Create)
            Dim formatter As New BinaryFormatter
            formatter.Serialize(fs, UserSelection)
        End Using
    End Sub
    Function loadKeys() As NameValueCollection()
        Dim UserSelection As NameValueCollection() = Nothing
        Try
            Using fs As New FileStream("DataFile.dat", FileMode.Open)
                Dim formatter As New BinaryFormatter
                UserSelection = DirectCast(formatter.Deserialize(fs), NameValueCollection())
            End Using
        Catch ex As Exception
        End Try
        Return UserSelection
    End Function
    'check for valid user
    Sub processResponse(ByVal response As String)
        Log("Response: " & response)
        Dim json As JObject = JObject.Parse(response)

        'saving recieved keys
        Dim pkeys As NameValueCollection
        Dim skeys As NameValueCollection

        If IsNothing(keys) Then
            keys = {New NameValueCollection(), New NameValueCollection()}
            pkeys = New NameValueCollection()
            skeys = New NameValueCollection()
        Else
            pkeys = keys(0)
            skeys = keys(1)
        End If

        pkeys.Set(ssid, json.SelectToken("details").SelectToken("pkey"))
        skeys.Set(ssid, json.SelectToken("details").SelectToken("skey"))

        Dim newKeys = {pkeys, skeys}

        saveKeys(newKeys)

        'json.Property("skey").Remove()
        'json.Property("pkey").Remove()

        If (json.SelectToken("status") = "NEW") Then
            LogMsg("#NEW:")
        ElseIf (json.SelectToken("status") = "OK") Then
            'LogMsg("User OK.") 'for debugging perpose
            Log("#UPDATE:" & json.ToString())
        ElseIf (json.SelectToken("status") = "BLOCKED") Then
            LogMsg("You are blacklisted!", MsgBoxStyle.Exclamation)
            Log("#UPDATE:" & json.ToString())
            irregularStop = True
            Me.Stop()
        ElseIf (json.SelectToken("status") = "OVER") Then
            LogMsg("Reserved quota for you is too low", MsgBoxStyle.Exclamation)
            Log("#UPDATE:" & json.ToString())
            irregularStop = True
            Me.Stop()
        ElseIf (json.SelectToken("status") = "ERROR") Then
            LogMsg("Internal server error occured.", MsgBoxStyle.Exclamation)
            irregularStop = True
            Me.Stop()
        End If
    End Sub
    Sub usercheck()

        Dim url As String = requestHandler & "check"
        Log("Posting to: " & url)
        Try
            Dim response As String = wc.DownloadString(url)
            processResponse(response)
            Log("Debug code 115")
        Catch ex As Exception
            disconnect()
            LogMsg("Internal server error occured!!" & vbNewLine & Err.Description & Err.Source & " line => " & Err.Erl, MsgBoxStyle.Exclamation)
            irregularStop = True
            Me.Stop()
        End Try
    End Sub
    Sub LogMsg(ByVal body As String, Optional ByVal type As MsgBoxStyle = MsgBoxStyle.Information, Optional ByVal title As String = "Quota service says")
        Log(String.Format("#MSG:{0}:{1}:{2}", body.Replace(":", "-"), Integer.Parse(type), title.Replace(":", "-")))
    End Sub
    Sub setInitialUsage()
        Dim kbytes As Integer = (iface.NetworkInterface.GetIPv4Statistics.BytesReceived + iface.NetworkInterface.GetIPv4Statistics.BytesSent) / 1024
        initialUsage = kbytes
    End Sub
    Sub prepareUsage()
        If Not iface.CurrentConnection.profileName.ToUpper.Equals(ssid.ToUpper()) Then
            usage = 0
            Log("Debug code 110")
            Exit Sub
        End If

        Dim kbytes As Integer = (iface.NetworkInterface.GetIPv4Statistics.BytesReceived + iface.NetworkInterface.GetIPv4Statistics.BytesSent) / 1024
        kbytes = kbytes - initialUsage

        If usage < kbytes Then
            usage = kbytes
        End If
    End Sub
    Protected Overrides Sub OnShutdown()
        Log("shutting")
    End Sub
    Sub Log(msg As String)
        myLog.WriteEntry(msg)
    End Sub
    Sub uploadData()
        Dim retries As Integer = 2
retry:
        If uploadUsage(My.Settings.pending) Then
            My.Settings.pending = 0
        ElseIf retries > 0 Then
            Threading.Thread.Sleep(5000 / retries)
            retries = retries - 1
            Log("Data uploading tries " & retries)
            Log("Debug code 116")
            GoTo retry
        End If


    End Sub
    Private Shared Function Encode(ssid As String) As String
        Dim upper As String() = {"A", "B", "C", "D", "E", "F",
            "G", "H", "I", "J", "K", "L",
            "M", "N", "O", "P", "Q", "R",
            "S", "T", "U", "V", "W", "X",
            "Y", "Z"}
        Dim lower As String() = {"a", "b", "c", "d", "e", "f",
            "g", "h", "i", "j", "k", "l",
            "m", "n", "o", "p", "q", "r",
            "s", "t", "u", "v", "w", "x",
            "y", "z"}
        Dim symbols As String() = {"*", "#", "$", "&", "%"}
        Dim numbers As Integer() = {0, 1, 2, 3, 4, 5,
            6, 7, 8, 9}

        Dim encode__1 As String = ssid
        While encode__1.Length < 10
            encode__1 &= ssid
        End While

        encode__1 = encode__1.Substring(0, 10)


        Dim encoded_str As String = lower(AscW(encode__1(0)) Mod 26)

        encoded_str &= numbers(AscW(encode__1(1)) Mod 10)
        encoded_str &= numbers(AscW(encode__1(2)) Mod 10)
        encoded_str &= symbols(AscW(encode__1(3)) Mod 5)
        encoded_str &= lower(AscW(encode__1(4)) Mod 26)
        encoded_str &= upper(AscW(encode__1(5)) Mod 26)
        encoded_str &= lower(AscW(encode__1(6)) Mod 26)
        encoded_str &= upper(AscW(encode__1(7)) Mod 26)
        encoded_str &= numbers(AscW(encode__1(8)) Mod 10)
        encoded_str &= symbols(AscW(encode__1(9)) Mod 5)

        Return encoded_str
    End Function
    Function uploadUsage(ByVal kbytes As Integer) As Boolean
        Try
            Dim url As String = requestHandler & "usage/" & kbytes.ToString()
            Log("Posting to: " & url)
            Dim response As String = wc.DownloadString(url)
            processResponse(response)
            Log("Debug code 117")
            Return True
        Catch ex As Exception
            Log(ex.Message & ex.StackTrace)
            Return False
        End Try
    End Function
    Function isConnectedTo() As Boolean
        If Not iface.InterfaceState = WlanInterfaceState.Connected Then
            Return False
        End If
        If iface.CurrentConnection.profileName.ToUpper.Equals(ssid.ToUpper()) Then
            Return True
        End If
        Return False
    End Function
    Sub connect()
        'if custom key is defined
        If Not customKey.Equals("") Then
            Log("Debug code 100")
            connectProcess(customKey)
            Exit Sub
        End If

        'if keys is nothing it will be the app installed time or settings corrupted
        If IsNothing(keys) Then
            Log("Debug code 101")
            Exit Sub
        End If

        'if ssid is not in the pkey list,
        'first time with ssid. try default
        If Not keys(0).AllKeys().Contains(ssid) Then
            Log("Debug code 102")
            connectProcess(Encode(ssid))
            Exit Sub
        End If

        'if ssid is found and mapped pkey is corrupted
        If IsNothing(keys(0).Item(ssid)) Then
            Log("Debug code 103")
            connectProcess(Encode(ssid))
            Exit Sub
        End If

        'if custom key is not definded and
        'first try, connect using pkey
        If keys(0).AllKeys().Contains(ssid) Then
            Dim pKey = keys(0).Item(ssid)
            If Not pKey.Equals("") Then
                Log("Debug code 104")
                connectProcess(pKey)
            End If
        End If

        If isConnectedTo() Then
            Log("Debug code 105")
            Exit Sub
        Else
            'program flows below and try to connect with skey
        End If


        'if ssid is not in the skey list,
        'first time with ssid. try default
        If Not keys(1).AllKeys().Contains(ssid) Then
            Log("Debug code 106")
            Exit Sub
        End If

        'if ssid is found and mapped skey is corrupted
        If IsNothing(keys(1).Item(ssid)) Then
            Log("Debug code 107")
            Exit Sub
        End If

        'if custom key is not definded and
        'skey fails to connect, connect using skey

        If keys(1).AllKeys().Contains(ssid) Then
            Dim skey = keys(1).Item(ssid)
            Log("pKey was incorrect. Trying sKey")
            If Not skey.Equals("") Then
                Log("Debug code 108")
                connectProcess(skey)
            End If
        End If

    End Sub
    Sub connectProcess(ByVal pass_key As String)
        Dim profileXml As String = "<?xml version=""1.0""?><WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1""><name>{0}</name><SSIDConfig><SSID><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>manual</connectionMode><MSM><security><authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{1}</keyMaterial></sharedKey></security></MSM><MacRandomization xmlns=""http://www.microsoft.com/networking/WLAN/profile/v3""><enableRandomization>false</enableRandomization></MacRandomization></WLANProfile>"
        profileXml = String.Format(profileXml, ssid, pass_key)
        Log("connect process with " & ssid & " <=> M" & pass_key)
        Try
            iface.ConnectSynchronously(WlanConnectionMode.TemporaryProfile, Dot11BssType.Any, profileXml, 4000)
        Catch ex As Exception
            Log("ERROR : Check your wifi connection")
            Log(ex.Message & ex.StackTrace)
        End Try
    End Sub
    Function getRandom(ByVal min As Integer, ByVal max As Integer)
        Dim gen As System.Random = New System.Random()
        Return gen.Next(min, max)
    End Function
    Sub disconnect()
        Try
            prepareUsage()
            preparePending()
            My.Settings.Save()
        Catch ex As Exception

        End Try

        Dim x = 0
        While x < 50 And isConnectedTo()
            Log("Debug code 109")
            disconnectProcess()
            Threading.Thread.Sleep(500)
            x = x + 1
        End While

    End Sub
    Sub disconnectProcess()
        Try
            iface.Disconnect()
        Catch ex As Exception
            Log("error when disconnecting. " & ex.StackTrace & ex.Message)
            irregularStop = True
            Exit Sub
        End Try
    End Sub
    Protected Overrides Sub OnStop()
        Try
            Log("Stopping")
            prepareUsage()
            preparePending()
            My.Settings.Save()
            If Not irregularStop Then
                Log("auto stop")
                uploadData()
            End If


        Catch ex As Exception
        End Try

        My.Settings.st = 1
        My.Settings.Save()
        disconnect()
        ' Add code here to perform any tear-down necessary to stop your service.
    End Sub
    Sub preparePending()
        My.Settings.pending = My.Settings.pending + (usage - lastUsage)
        lastUsage = usage
        usage = 0
        My.Settings.Save()
    End Sub
End Class
