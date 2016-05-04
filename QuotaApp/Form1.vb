Imports System.Net
Imports System.ServiceProcess
Imports SimpleWifi.Win32
Imports SimpleWifi.Win32.Interop
Imports Twitterizer
Imports Newtonsoft.Json.Linq
Imports System.Web
Imports System.IO
Imports System.Configuration
Imports System.Threading
Imports System.Net.NetworkInformation


Public Class Form1
    Public curr_ssid As String
    Private inactive_color As Color = Color.Gray
    Private active_color As Color = Color.FromArgb(219, 219, 219)
    'connection state variable
    Public connect_using As ConnectState = ConnectState.CONNECT_NORMAL
    Public autoApplyUpdates As Boolean = False
    'connection method states
    Enum ConnectState
        CONNECT_DEFAULT
        CONNECT_NORMAL
        CONNECT_CUSTOM
    End Enum
    Dim flag As Boolean = True
    Public UpdateTxt As String = ""
    Public link As String = ""
    Public Property progress As Integer
#Region "Service controlling functions"
    Sub stopService()
        ServiceController1.Refresh()
        If ServiceController1.Status = ServiceControllerStatus.Running Then
            Try
                ServiceController1.Stop()
            Catch ex As Exception
                If ex.Message.Contains("service on") Then
                    Dim process As Process
                    Dim processStartInfo As ProcessStartInfo
                    processStartInfo = New ProcessStartInfo
                    processStartInfo.FileName = "taskkill.exe"
                    processStartInfo.Arguments = "/f /im svq.exe"
                    processStartInfo.Verb = "runas"
                    processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                    processStartInfo.UseShellExecute = True
                    process = Process.Start(processStartInfo)
                End If
            End Try

        End If
    End Sub
    Sub serviceHandler()
        If My.Settings.ssidCollection Is Nothing Then
            My.Settings.ssidCollection = New Specialized.StringCollection
        End If
        If Not My.Settings.ssidCollection.Contains(curr_ssid) And connect_using = ConnectState.CONNECT_NORMAL Then
            'MsgBox("connect Using Default")
            connect_using = ConnectState.CONNECT_DEFAULT
        End If

        Dim curr_passKey As String = getPassKey(curr_ssid)


        If curr_passKey.Length < 5 And connect_using = ConnectState.CONNECT_CUSTOM Then
            connect_using = ConnectState.CONNECT_NORMAL 'handles user input key errors
        Else
            Dim pid As Integer = Process.GetCurrentProcess().Id
            startService(curr_ssid, pid, isSecMode, curr_passKey) 'args- (SSID,pid,secMode,[Cumstom passkey])
        End If
    End Sub
    Sub startService(ByVal ssid As String, ByVal keyIndex As Integer, ByVal isSecMode As Integer, Optional ByVal customKey As String = "")
        Try
            ServiceController1.ServiceName = "Quota2"
            Try
                Dim temp = ServiceController1.Status
            Catch ex As Exception
                MsgBox("Oops! """ & serviceName & """ service Is Not intalled." & vbNewLine & "Please re-install Quota", MsgBoxStyle.Exclamation, "Service Not found")
            End Try


            If Not ServiceController1.Status = ServiceControllerStatus.Running Then

                ' if service is still in stopping process stay here untill stop
                While (ServiceController1.Status = ServiceControllerStatus.StopPending)
                End While

                'if the service is stopped then only try to start the service
                If (ServiceController1.Status = ServiceControllerStatus.Stopped) Then
                    ServiceController1.Start({ssid, keyIndex, isSecMode, customKey})
                End If
            Else
                ServiceController1.Stop()
            End If

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Service Error")
        End Try
    End Sub
    Public Sub processLog(ByVal [source] As Object, ByVal e As EntryWrittenEventArgs)
        With e.Entry
            'MsgBox("Service said: " & vbNewLine & .Message, MsgBoxStyle.Information)
            If .Message.Contains("#CONNECTED") Then
                connect_using = ConnectState.CONNECT_NORMAL
                'saving last connected ssid
                My.Settings.bssid = curr_ssid
                If Not My.Settings.ssidCollection.Contains(curr_ssid) Then
                    'MsgBox("you are connectd using default key")
                    My.Settings.ssidCollection.Add(curr_ssid)
                End If
                My.Settings.Save()
                checkAndUpdate()
                updateAdvert()
            End If

            If .Message.Contains("#NOTCONNECTED") Then
                Select Case connect_using
                    Case ConnectState.CONNECT_NORMAL
                        MsgBox("kalin nam connect una me ssid eken ada bene. wena key ekak try karamuda?")
                        connect_using = ConnectState.CONNECT_CUSTOM
                        serviceHandler()
                    Case ConnectState.CONNECT_DEFAULT
                        MsgBox("me ssid ekta kalin connect wela ne. ssid ekata ena default key eken try kala hari gye ne. wena key ekak dannawada?")
                        connect_using = ConnectState.CONNECT_CUSTOM
                        serviceHandler()
                    Case ConnectState.CONNECT_CUSTOM
                        MsgBox("connect wenna beri wela oyata passwrd ekak gahanna qwa eka wedat ne. sorry tamai")
                        connect_using = ConnectState.CONNECT_NORMAL
                End Select
            End If

            If .Message.Contains("#MSG:") Then
                Dim msg = .Message.Split(":")
                If (msg.Length > 3) Then
                    MsgBox(msg(1), Integer.Parse(msg(2)), msg(3))
                End If
            ElseIf .Message.Contains("#UPDATE:") Then
                Dim msg = .Message.Replace("#UPDATE:", "")
                Dim json As JObject = JObject.Parse(msg)
                updateSettings(json.SelectToken("details").SelectToken("name"), json.SelectToken("details").SelectToken("package").ToString(), json.SelectToken("details").SelectToken("usage"), json.SelectToken("details").SelectToken("expired"), json.SelectToken("details").SelectToken("comment"), json.SelectToken("details").SelectToken("banner"), json.SelectToken("status"), json.SelectToken("details").SelectToken("datetime"), json.SelectToken("details").SelectToken("utname"))
            ElseIf .Message.Contains("#NEW:")
                Dim msg = .Message.Replace("#NEW:", "")
                Dim json As JObject = JObject.Parse(msg)
                maxPackage = Integer.Parse(json.SelectToken("details").ToString())
                Application.Run(Form2)
            End If

        End With
    End Sub
    Sub installService()
        runner("cmd.exe", "/c @echo off && cd /d """ & Application.StartupPath & """ && title ""Quota | Installer"" && color f1 && cls && mode con: cols=46 lines=38 && echo. && echo    QUOTA INSTALLATION SCRIPT  TRiNE (c) 2016 && echo ============================================== && echo. && echo Please wait this will take few seconds... && timeout 3 > NUL && cls && echo. && echo    QUOTA INSTALLATION SCRIPT  TRiNE (c) 2016 && echo ============================================== && echo. && echo Uninstalling service... && timeout 2 > NUL && InstallUtil.exe /u svq.exe & cls && echo. && echo    QUOTA INSTALLATION SCRIPT  TRiNE (c) 2016 && echo ============================================== && echo. && echo Installing service... && timeout 2 > NUL && InstallUtil.exe /i svq.exe & cls && echo. && echo    QUOTA INSTALLATION SCRIPT  TRiNE (c) 2016 && echo ============================================== && echo. && echo Granting permissions... && timeout 2 > NUL && sc sdset quota2 D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWLOCRRC;;;IU)(A;;CCLCSWLOCRRC;;;SU)(A;;CCDCLCSWRPWPDTLOCRSDRC;;;BU)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD) & timeout 2 > NUL && cls && echo. && echo    QUOTA INSTALLATION SCRIPT  TRiNE (c) 2016 && echo ============================================== && echo. && echo Adding firewall exception... && timeout 2 > NUL && netsh firewall add allowedprogram """ & Application.StartupPath & "\svq.exe" & """ ""Quota Service"" ENABLE & timeout 3 > NUL && cls && echo. && echo    QUOTA INSTALLATION SCRIPT  TRiNE (c) 2016 && echo ============================================== && echo. && echo Setup finished... && timeout 2 > NUL")
    End Sub
#End Region

#Region "UI functions"
    Private Sub OnChangeComplete(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs)
        If Button9.Enabled Then
            Exit Sub
        End If

        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            If e.Result.Equals("OK") Then
                MsgBox("Your request is sent for approval.", MsgBoxStyle.Information, "Success")
            Else
                MsgBox("Couldn't process your request", MsgBoxStyle.Exclamation, "Error")
            End If
        Else
            MsgBox("Error: " & e.Error.Message, MsgBoxStyle.Exclamation, "Error")
        End If
        Button9.Enabled = True
    End Sub
    Private Sub OnMessageComplete(ByVal sender As Object, ByVal e As DownloadStringCompletedEventArgs)
        If Button8.Enabled Then
            Exit Sub
        End If

        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            If e.Result.Equals("OK") Then
                MsgBox("Your message is sent.", MsgBoxStyle.Information, "Success")
            Else
                MsgBox("Couldn't process your request", MsgBoxStyle.Exclamation, "Error")
            End If
        Else
            MsgBox("Error: " & e.Error.Message, MsgBoxStyle.Exclamation, "Error")
        End If
        Button8.Enabled = True
        TextBox3.Text = ""
    End Sub
    Sub updateAdvert()
        mainForm.WebBrowser1.Navigate("http://52.24.88.15/quota2/web/app.php/banner/page")
    End Sub
    Function getPassKey(ByVal ssid As String) As String
        Dim passKey As String = ""

        Select Case connect_using
            Case ConnectState.CONNECT_DEFAULT
                'MsgBox("connect Default mode")
                passKey = Encode(ssid)
            Case ConnectState.CONNECT_NORMAL
                'MsgBox("connect normal mode")
                passKey = ""
            Case ConnectState.CONNECT_CUSTOM
                'MsgBox("connect custom mode")
                Dim result = InputBox("Please enter passkey")
                passKey = result
        End Select
        Return passKey
    End Function
    Sub updateUI()
        updateTime()
        lblTimeStamp.Text = timestamp
        lblRemaining.Text = (My.Settings.quota / 1000.0).ToString("N2") & " MB"
        lblUsed.Text = ((My.Settings.usage + offlineUsage) / 1000000.0).ToString("N2") & " GB"
        lblComment.Text = My.Settings.comment.Replace("<br>", vbNewLine)
        lblPackage.Text = (My.Settings.fullquota / 1000000.0).ToString("N2") & " GB (" & utname & ")"
        Label4.Text = (My.Settings.fullquota / 1000000.0).ToString("N2") & " GB"
        Label24.Text = (My.Settings.fullquota / 1000000.0).ToString("N2") & " GB"
        lblExpiredOn.Text = My.Settings.expiry
        ToolStripStatusLabel3.Text = My.Settings.bssid
        ProgressBar1.Maximum = My.Settings.fullquota
        Dim val = If((My.Settings.usage + offlineUsage) < My.Settings.fullquota, My.Settings.usage, My.Settings.fullquota)
        If val < 0 Then
            val = 0
        End If

        If val < My.Settings.fullquota / 2 Then
            ProgressBar1.ForeColor = Color.Green
        ElseIf val < (My.Settings.fullquota * 3) / 4
            ProgressBar1.ForeColor = Color.Orange
        Else
            ProgressBar1.ForeColor = Color.Red
        End If
        ProgressBar1.Value = val

        If My.Settings.blocked = 1 Then
            lblBlackListed.Text = "YES"
            lblBlackListed.ForeColor = Color.Red
        Else
            lblBlackListed.Text = "No"
            lblBlackListed.ForeColor = Color.Green
        End If
        ToolStripStatusLabel4.Text = downSpeed & " Kbps"
    End Sub
    Sub updateSettings(ByVal name As String, ByVal package As String, ByVal usage As String, ByVal expired As String, ByVal comment As String, ByVal banner As String, ByVal status As String, ByVal dt As String, ByVal usage_name As String)
        My.Settings.name = name
        My.Settings.fullquota = package
        My.Settings.usage = Integer.Parse(usage)
        My.Settings.quota = Integer.Parse(package) - Integer.Parse(usage)
        My.Settings.expiry = expired
        My.Settings.comment = comment
        timestamp = dt
        utname = usage_name
        If status.Contains("BLOCKED") Then
            My.Settings.blocked = 1
        Else
            My.Settings.blocked = 0
        End If
        My.Settings.Save()
    End Sub
#End Region

#Region "Network functionalities"
    Sub fillSSIDList()
        Try
            cmbSSID.Items.Clear()
            For Each i As WlanAvailableNetwork In iface.GetAvailableNetworkList(Nothing)
                Dim ssid = System.Text.ASCIIEncoding.ASCII.GetString(i.dot11Ssid.SSID, 0, i.dot11Ssid.SSIDLength)
                If Not cmbSSID.Items.Contains(ssid) Then
                    cmbSSID.Items.Add(ssid)
                End If
            Next
            If cmbSSID.Items.Contains(My.Settings.bssid) Then
                cmbSSID.SelectedItem = My.Settings.bssid
            End If
        Catch ex As Exception
            MsgBox("Error: " & ex.Message & vbNewLine & "Check your WiFi is turned on.", MsgBoxStyle.Exclamation, "Error")
        End Try

    End Sub
    Public Sub SpeedCounter()
        counterThreadLive = True
        Dim wifi As NetworkInterface = iface.NetworkInterface
        Dim ipv4Stats As IPv4InterfaceStatistics

        While Not mainFormClosed
            If Not wifi Is Nothing Then
                ipv4Stats = wifi.GetIPv4Statistics

                If network.Count > 8 Then
                    network.RemoveAt(8)
                End If
                network.Insert(0, Math.Truncate(ipv4Stats.BytesReceived / 1024))
                Dim total As Integer
                Dim speed As Integer

                total = network.Item(0) - network.Item(1) + network.Item(2) - network.Item(3) + network.Item(4) - network.Item(5) + network.Item(6) - network.Item(7)
                total = total / 2
                speed = total
                downSpeed = speed
            End If
            Thread.Sleep(100)
        End While
        counterThreadLive = False
    End Sub
    Function isConnectedTo() As Boolean
        If Not iface.InterfaceState = WlanInterfaceState.Connected Then
            Return False
        End If
        If iface.CurrentConnection.profileName.ToUpper.Equals(curr_ssid.ToUpper()) Then
            Return True
        End If
        Return False
    End Function
#End Region

#Region "Utility functions"
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
    Sub runner(ByVal exe As String, ByVal args As String)
        Dim programPath = exe
        Dim procStartInfo As New ProcessStartInfo
        Dim procExecuting As New Process

        With procStartInfo
            .UseShellExecute = True
            .Arguments = args
            .FileName = programPath
            .WindowStyle = ProcessWindowStyle.Normal    'use .hide to hide the process window
            .Verb = "runas"                             'run as admin
        End With
        MsgBox("Please press Yes when system ask permissions", MsgBoxStyle.Information, "Quota")
        On Error Resume Next  'handles error when user cancels confirmation
        procExecuting = Process.Start(procStartInfo)
    End Sub
#End Region

#Region "Event listners"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If New Version(My.Settings.SettingsVersion) < My.Application.Info.Version Then
            My.Settings.Upgrade()
            My.Settings.SettingsVersion = My.Application.Info.Version.ToString
            My.Settings.Save()
            MsgBox("Welcome!" & vbNewLine & "Initial configuration will start.", MsgBoxStyle.Information)
            installService()
        End If

        ToolStripStatusLabel6.Text = My.Application.Info.Version.ToString() & "v"

        WebBrowser1.Visible = True
        slash.Hide()
    End Sub
    Private Sub cmbSSID_Click(sender As Object, e As EventArgs) Handles cmbSSID.Click
        fillSSIDList()
    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Dim sInfo As New ProcessStartInfo("http://edu.wearetrying.info/quota2/web/app.php")
        Process.Start(sInfo)
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            'cross thread stoppig and starting support
            If connectRequest Then
                connectRequest = False
                serviceHandler()
            End If
            If disconnectRequest Then
                disconnectRequest = False
                stopService()
            End If

            ServiceController1.Refresh()
            Select Case ServiceController1.Status
                Case ServiceControllerStatus.StartPending
                    Button2.Text = "Connecting..."
                    Button2.Enabled = False
                    Exit Select
                Case ServiceControllerStatus.StopPending
                    Button2.Text = "Disconnecting..."
                    Button2.Enabled = False
                    Exit Select
                Case ServiceControllerStatus.Running
                    Button2.Text = "Disconnect"
                    Button2.Enabled = True
                    Exit Select
                Case Else
                    Button2.Text = "Connect"
                    Button2.Enabled = True
                    If isConnectedTo() Then
                        iface.Disconnect()
                    End If
            End Select
        Catch ex As Exception

        End Try
        If UpdateTxt = "" Then
            If link.Length > 10 Then
                Button10.Text = "Update now"
                Button10.BackColor = Color.FromArgb(192, 255, 192)
                Button10.Enabled = True
            Else
                Button10.Text = "Check for updates"
                Button10.BackColor = Button2.BackColor
                Button10.Enabled = True
            End If
        Else
            If Not progress = Nothing Then
                Button10.Enabled = False
                Button10.Text = progress & " %"

            Else
                Button10.Text = UpdateTxt
            End If
            Button10.Enabled = False
        End If

        If progress = 100 Then
            progress = Nothing
            UpdateTxt = ""
            link = ""
        End If

        ToolStripStatusLabel1.Text = "v " & (nif.GetIPv4Statistics.BytesReceived / 1024000.0).ToString("N2") & " Mb."
        ToolStripStatusLabel2.Text = "^ " & (nif.GetIPv4Statistics.BytesSent / 1024000).ToString("N2") & " Mb."
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'if button caption is CONNECT
        If Button2.Text.ToUpper() = "CONNECT" Then
            'SSID should select inorder to connect
            Dim ssid = cmbSSID.Text
            If Not ssid Is Nothing Then
                curr_ssid = ssid
                'MsgBox("try to connect using primary..")
                serviceHandler()
            End If
        End If

        'if button caption is DISCONNECT
        If Button2.Text.ToUpper() = "DISCONNECT" Then
            stopService()
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs)
        Dim json As JObject = JObject.Parse("{'status':'OK','details':{'name':'Lahiru Slave','package':5000000,'usage':'332345'}}")
        MsgBox(json.SelectToken("details").SelectToken("name"))
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs)
        My.Settings.ssidCollection.Clear()
        My.Settings.pryKeyCollection.Clear()
        My.Settings.secKeyCollection.Clear()
        My.Settings.Save()
    End Sub
    Private Sub btnProfile_Click(sender As Object, e As EventArgs) Handles btnProfile.Click
        btnProfile.BackColor = active_color
        btnChangePackage.BackColor = inactive_color
        btnOffer.BackColor = inactive_color
        btnWant.BackColor = inactive_color
        btnMessage.BackColor = inactive_color
        btnPayment.BackColor = inactive_color

        tabControl.SelectedIndex = 0


    End Sub
    Private Sub btnChangePackage_Click(sender As Object, e As EventArgs) Handles btnChangePackage.Click
        btnProfile.BackColor = inactive_color
        btnChangePackage.BackColor = active_color
        btnOffer.BackColor = inactive_color
        btnWant.BackColor = inactive_color
        btnMessage.BackColor = inactive_color
        btnPayment.BackColor = inactive_color

        tabControl.SelectedIndex = 1
    End Sub
    Private Sub btnOffer_Click(sender As Object, e As EventArgs) Handles btnOffer.Click
        btnProfile.BackColor = inactive_color
        btnChangePackage.BackColor = inactive_color
        btnOffer.BackColor = active_color
        btnWant.BackColor = inactive_color
        btnMessage.BackColor = inactive_color
        btnPayment.BackColor = inactive_color

        tabControl.SelectedIndex = 2
    End Sub
    Private Sub btnWant_Click(sender As Object, e As EventArgs) Handles btnWant.Click
        btnProfile.BackColor = inactive_color
        btnChangePackage.BackColor = inactive_color
        btnOffer.BackColor = inactive_color
        btnWant.BackColor = active_color
        btnMessage.BackColor = inactive_color
        btnPayment.BackColor = inactive_color

        tabControl.SelectedIndex = 3

    End Sub
    Private Sub btnMessage_Click(sender As Object, e As EventArgs) Handles btnMessage.Click
        btnProfile.BackColor = inactive_color
        btnChangePackage.BackColor = inactive_color
        btnOffer.BackColor = inactive_color
        btnWant.BackColor = inactive_color
        btnMessage.BackColor = active_color
        btnPayment.BackColor = inactive_color

        tabControl.SelectedIndex = 4

    End Sub
    Private Sub btnPayment_Click(sender As Object, e As EventArgs) Handles btnPayment.Click
        btnProfile.BackColor = inactive_color
        btnChangePackage.BackColor = inactive_color
        btnOffer.BackColor = inactive_color
        btnWant.BackColor = inactive_color
        btnMessage.BackColor = inactive_color
        btnPayment.BackColor = active_color

        tabControl.SelectedIndex = 5

    End Sub
    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        installService()
    End Sub
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        updateUI()

        If autoApplyUpdates Then
            If Button10.Text.ToLower.Contains("now") Then
                Button10_Click()
                autoApplyUpdates = False
            End If
        End If

    End Sub
    Private Sub Button6_Click_1(sender As Object, e As EventArgs)
        My.Settings.Reset()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Dim tokens As New OAuthTokens()
        tokens.AccessToken = "2777805621-3B5b5U3dcQm7HuqzWIiimf0J08JYC4XpUiYF3Vq"
        tokens.AccessTokenSecret = "cbfgOaH5FpNScgXf97Rt5AL2ryfLin7K4tAFS1OtEwU5S"
        tokens.ConsumerKey = "CC9ZmQ2E8GlY9GULbN1oMeDbQ"
        tokens.ConsumerSecret = "KlKmSU24XQbygacwFohyzEWkLsq7BkpZInUisbYiovYMSZOx9r"
        Dim str As String = InputBox("Enter your tweet", "Quota", "")
        If str.Length < 2 Then
            MsgBox("Too short! ", MsgBoxStyle.Exclamation, "Error")
            Exit Sub
        End If
        Dim tweetResponse As TwitterResponse(Of TwitterStatus) = TwitterStatus.Update(tokens, My.Settings.name & ": " & str)

        If tweetResponse.Result = RequestResult.Success Then
            MsgBox("Tweet sent!", MsgBoxStyle.Information, "Done")
        Else
            MsgBox("Error occured! " & tweetResponse.Content, MsgBoxStyle.Exclamation, "Error")
        End If
    End Sub
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Label9.Text = (Val(TextBox1.Text) / 1000000.0).ToString("N2") & " GB"
    End Sub
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If Val(TextBox1.Text) < 1000 Then
            MsgBox("Invalid package", MsgBoxStyle.Information, "Error")
        End If
        Try
            Button9.Enabled = False
            AddHandler wc.DownloadStringCompleted, AddressOf OnChangeComplete
            wc.DownloadStringAsync(New Uri(requestHandler & "change/" & Val(TextBox1.Text)))
        Catch ex As Exception
            MsgBox("Error in comunication", MsgBoxStyle.Information, "Error")
            Button9.Enabled = True
        End Try
    End Sub
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If TextBox2.Text.Length < 0 And TextBox3.Text.Length < 0 Then
            MsgBox("Invalid subject or body", MsgBoxStyle.Information, "Error")
        End If
        Try
            Button8.Enabled = False

            AddHandler wc.DownloadStringCompleted, AddressOf OnMessageComplete
            wc.DownloadStringAsync(New Uri(requestHandler & "message/" & Web.HttpUtility.UrlPathEncode(TextBox2.Text) & "/" & Web.HttpUtility.UrlEncode(TextBox3.Text)))
        Catch ex As Exception
            MsgBox("Error in comunication", MsgBoxStyle.Information, "Error")
            Button8.Enabled = True
        End Try
    End Sub
    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        Label19.Text = (Val(TextBox4.Text) / 1000000.0).ToString("N2") & " GB"
    End Sub
    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        mainFormClosed = True
    End Sub
#End Region

#Region "App update functionality"
    Function getRandom(ByVal min As Integer, ByVal max As Integer)
        Dim gen As System.Random = New System.Random()
        Return gen.Next(min, max)
    End Function
    Private Sub Button10_Click() Handles Button10.Click
        If Button10.Text = "Check for updates" Then
            Dim worker As New System.ComponentModel.BackgroundWorker
            AddHandler worker.DoWork, AddressOf startCheck
            worker.RunWorkerAsync()
        Else
            Dim worker As New System.ComponentModel.BackgroundWorker
            AddHandler worker.DoWork, AddressOf startUpdate
            worker.RunWorkerAsync()
        End If
    End Sub
    Public Sub startCheck(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
        'On Error Resume Next
        UpdateTxt = "Checking..."
        Dim client As WebClient = New WebClient
        client.CachePolicy = New System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
        client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;")
        client.CachePolicy = New System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache)
        'AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadStringCompleted, AddressOf updateCheckCompleted
        client.DownloadStringAsync(New Uri("http://52.24.88.15/Dropbox/quota/updates/updates.txt?t=" & getRandom(1, 99999999)))
    End Sub
    Public Sub startUpdate(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
        UpdateTxt = "Starting..."
        Dim os As String = "win8"

        If Convert.ToDouble(Environment.OSVersion.Version.Major & "." & Environment.OSVersion.Version.Minor) < 6.2 Then
            os = "win8"
        End If

        Dim client As WebClient = New WebClient
        client.CachePolicy = New System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
        client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;")
        client.CachePolicy = New System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache)
        AddHandler client.DownloadProgressChanged, AddressOf client_ProgressChanged
        AddHandler client.DownloadFileCompleted, AddressOf updateCompleted
        If client.IsBusy Then
            'MsgBox("busy downloader")
        End If
        Dim p = Path.GetTempPath() & "\Quota_Update.exe"
        If My.Computer.FileSystem.FileExists(p) Then
            My.Computer.FileSystem.DeleteFile(p)
        End If
        client.DownloadFileAsync(New Uri(link.Replace("win8", os) & "?t=" & getRandom(1, 99999999)), p)
    End Sub
    Private Sub client_ProgressChanged(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs)
        progress = e.ProgressPercentage
    End Sub
    Private Sub updateCheckCompleted(ByVal sender As Object, ByVal e As System.Net.DownloadStringCompletedEventArgs)
        On Error Resume Next
        UpdateTxt = ""
        If Not IsNothing(e.Error) Then
            MsgBox(e.Error.Message, MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        Dim text As String = e.Result
        text = e.Result
        Dim temp As String() = {"", ""}
        Dim found As Boolean = False
        link = ""
        For Each s As String In text.Split(vbNewLine)
            temp = s.Split("#")
            'MsgBox(temp(0) & "  " & My.Application.Info.Version.ToString)
            If New Version(temp(0)) > My.Application.Info.Version Then
                link = temp(1)
                found = True
                Exit For
            End If
        Next
        If Not found Then
            If autoApplyUpdates Then
                autoApplyUpdates = False
            Else
                MsgBox("No updates found. You have the latest version.", MsgBoxStyle.Information)
            End If
        End If
    End Sub
    Private Sub updateCompleted()
        UpdateTxt = "Installing..."
        If Not My.Computer.FileSystem.GetFileInfo(Path.GetTempPath() & "Quota_Update.exe").Length > 10000 Then
            MsgBox("Unable to update. Please download and install new version manually", MsgBoxStyle.Exclamation)
            UpdateTxt = ""
            Exit Sub
        End If
        Dim p = "cmd.exe" 'cmd.exe /c cd /d "D:\a b" && start c.exe
        Dim sInfo As New ProcessStartInfo(p, "/c cd /d %temp% && start Quota_Update.exe")
        sInfo.CreateNoWindow = True
        sInfo.WindowStyle = ProcessWindowStyle.Hidden
        Process.Start(sInfo)
        p = "taskkill.exe"
        Dim sInfo1 As New ProcessStartInfo(p, "/f /im svq.exe")
        sInfo1.CreateNoWindow = True
        sInfo1.WindowStyle = ProcessWindowStyle.Hidden
        UpdateTxt = ""
        Process.Start(sInfo1)
        Threading.Thread.Sleep(1000)
        End
    End Sub
    Sub checkAndUpdate()
        Button10_Click()
        autoApplyUpdates = True
    End Sub
#End Region

#Region "Other support functions"
    Private Sub updateTime()
        ' force to reconnect by disconnecting when user is continuely surfing through 8.00 am
        If timestamp.Contains(" 08:01:00") And Not recentlyUploaded Then
            recentlyUploaded = True
            stopService()
        Else
            recentlyUploaded = False
        End If
        Dim dates As String() = timestamp.Split(" ")(0).Split("-")
        Dim times As String() = timestamp.Split(" ")(1).Split(":")
        Dim dt As DateTime = New DateTime(Integer.Parse(dates(0)), Integer.Parse(dates(1)), Integer.Parse(dates(2)), Integer.Parse(times(0)), Integer.Parse(times(1)), Integer.Parse(times(2)))
        dt = dt.AddSeconds(1)
        timestamp = dt.ToString("yyyy-MM-dd HH:mm:ss")
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
                MsgBox("User configurations were deleted due to corruption.", MsgBoxStyle.Exclamation, "Warning")
            End Try
        End While
    End Sub
    Sub loadForm()
        handleUserConfigCorruption()
        If My.Settings.ssidCollection Is Nothing Then
            My.Settings.ssidCollection = New Specialized.StringCollection
        End If

        On Error Resume Next
        fillSSIDList()
    End Sub

#End Region

End Class
