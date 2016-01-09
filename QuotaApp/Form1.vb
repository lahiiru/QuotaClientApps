Imports System.Net
Imports System.ServiceProcess
Imports SimpleWifi.Win32
Imports SimpleWifi.Win32.Interop
Imports Twitterizer
Imports Newtonsoft.Json.Linq


Public Class Form1

    Public curr_ssid As String
    Public curr_passKey As String() = {"", ""}      'primary and secondry passkeys
    Public default_passKey As String = "w!fIdisable23"      'default passkey

    'connection method states
    Enum ConnectState
        CONNECT_DEFAULT
        CONNECT_NORMAL
        CONNECT_CUSTOM
    End Enum

    'connection state variable
    Public connect_using As ConnectState = ConnectState.CONNECT_NORMAL

    Sub load()
        If My.Settings.ssidCollection Is Nothing Then
            My.Settings.ssidCollection = New Specialized.StringCollection
        End If
        If My.Settings.pryKeyCollection Is Nothing Then
            My.Settings.pryKeyCollection = New Specialized.StringCollection
        End If
        If My.Settings.secKeyCollection Is Nothing Then
            My.Settings.secKeyCollection = New Specialized.StringCollection
        End If

        If My.Computer.Network.IsAvailable Then
            retriveData()
        End If
        On Error Resume Next
        Label2.Text = (My.Settings.quota / 1024.0).ToString("N2") & " Mb"
        TextBox1.Text = My.Settings.comment.Replace("<br>", vbNewLine)
        Label4.Text = (My.Settings.fullquota / 1000.0).ToString("N2") & " Mb"
        Label9.Text = My.Settings.expiry.Split(" ")(0)
        ToolStripStatusLabel3.Text = My.Settings.bssid
        If My.Settings.blocked = 1 Then
            Label11.Text = "YES"
            Label11.ForeColor = Color.Brown
        Else
            Label11.Text = "No"
            Label11.ForeColor = Color.DarkGreen
        End If
        Button1.Enabled = True
        fillSSIDList()

    End Sub
    Function getDetails() As String()
        Dim url As String = requestHandler & "req=get"
        Dim tries As Integer = 0
re:
        Dim response As String = WebRequest.DownloadString(url)
        If response.Split(";").Count < 4 Then
            Threading.Thread.Sleep(1000)
            tries = tries + 1
            If tries > 5 Then
                MsgBox("Unable retrive data from server.", MsgBoxStyle.Exclamation, "Error")
                End
            End If
            GoTo re
        End If
        Return response.Split(";")
    End Function
    Function check() As Boolean
        If My.Settings.quota < 1000 Or My.Settings.blocked = 1 Then
            If My.Settings.quota < 1000 Then
                MsgBox("Your  quota is less than 1Mb", MsgBoxStyle.Exclamation, "Can't connect")
            Else
                MsgBox("You are blacklisted!", MsgBoxStyle.Exclamation, "Can't connect")
            End If
            Return False
        End If
        Return True
    End Function
    Public Sub retriveData()
        On Error GoTo e103
        Dim arr As String() = getDetails()
        On Error GoTo e104
        My.Settings.quota = Val(arr(0))
        My.Settings.fullquota = Val(arr(1))
        My.Settings.comment = arr(2)
        My.Settings.blocked = arr(3)
        My.Settings.expiry = arr(4)
        My.Settings.passkey = arr(5)
        My.Settings.name = arr(6)
        My.Settings.Save()
        'MsgBox("Your have " & CInt(My.Settings.quota / 1024).ToString("N2") & "Mb remaining.", MsgBoxStyle.Information, "Connected.")
        Exit Sub
e103:
        MsgBox("Error 103 occured!" & Err.Description & vbNewLine & Err.Source, MsgBoxStyle.Exclamation, "Error")
        Exit Sub
e104:
        MsgBox("Error 104 occured!" & Err.Description & vbNewLine & Err.Source, MsgBoxStyle.Exclamation, "Error")
    End Sub
    Function isConnectedTo()
        If wlanIface.CurrentConnection.profileName.Contains(My.Settings.bssid.ToUpper()) Then
            Return True
        End If
        Return False
    End Function

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Dim sInfo As New ProcessStartInfo("http://edu.wearetrying.info/quota")
        Process.Start(sInfo)
    End Sub
    Dim flag As Boolean = True
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
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
            End Select
        Catch ex As Exception

        End Try

        ToolStripStatusLabel1.Text = "v " & (iface.NetworkInterface.GetIPv4Statistics.BytesReceived / 1024000.0).ToString("N2") & " Mb."
        ToolStripStatusLabel2.Text = "^ " & (iface.NetworkInterface.GetIPv4Statistics.BytesSent / 1024000).ToString("N2") & " Mb."
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        'if button caption is CONNECT
        If Button2.Text.ToUpper() = "CONNECT" Then
            'SSID should select inorder to connect
            If Not cmbSSID.SelectedItem Is Nothing Then
                curr_ssid = cmbSSID.SelectedItem
                MsgBox("try to connect using primary..")
                serviceHandler()
            End If
        End If

        'if button caption is DISCONNECT
        If Button2.Text.ToUpper() = "DISCONNECT" Then
            ServiceController1.Stop()
        End If


    End Sub

    'handle the service while connecting -- send passwords to service and try to connect
    Sub serviceHandler()
        Dim k = My.Settings.ssidCollection.Contains(curr_ssid)
        If Not My.Settings.ssidCollection.Contains(curr_ssid) And connect_using = ConnectState.CONNECT_NORMAL Then
            If connect_using = ConnectState.CONNECT_NORMAL Then
                MsgBox("connect using default")
                connect_using = ConnectState.CONNECT_DEFAULT
            End If
        End If

        curr_passKey = getPassKey(curr_ssid)
        startService(curr_ssid, curr_passKey(0), curr_passKey(1))
    End Sub

    'get passkeys according to states DIFAULT / NORMAL / CUSTOM
    Function getPassKey(ByVal ssid As String) As String()
        Dim passKey As String() = {"", ""}

        Select Case connect_using
            Case ConnectState.CONNECT_DEFAULT
                MsgBox("connect default mode")
                passKey(0) = default_passKey
            Case ConnectState.CONNECT_NORMAL
                MsgBox("connect normal mode")
                If My.Settings.pryKeyCollection.Count() > My.Settings.ssidCollection.IndexOf(ssid) Then
                    passKey(0) = My.Settings.pryKeyCollection(My.Settings.ssidCollection.IndexOf(ssid))
                End If
                If My.Settings.secKeyCollection.Count() > My.Settings.ssidCollection.IndexOf(ssid) Then
                    passKey(1) = My.Settings.secKeyCollection(My.Settings.ssidCollection.IndexOf(ssid))
                End If
            Case ConnectState.CONNECT_CUSTOM
                MsgBox("connect custom mode")
                passKey(0) = InputBox("Please enter passkey")
        End Select
        Return passKey
    End Function

    Sub startService(ByVal ssid As String, ByVal pKey As String, ByVal sKey As String)
        Try
            ServiceController1.ServiceName = "Quota2"
            Try
                Dim temp = ServiceController1.Status
            Catch ex As Exception
                MsgBox("Oops! """ & serviceName & """ service is not intalled." & vbNewLine & "Please re-install Quota", MsgBoxStyle.Exclamation, "Service not found")
            End Try

            If Not ServiceController1.Status = ServiceControllerStatus.Running Then
                ServiceController1.Start({ssid, pKey, sKey})
            Else
                ServiceController1.Stop()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Service error")
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        Dim json As JObject = JObject.Parse("{'status':'OK','details':{'name':'Lahiru Slave','package':5000000,'usage':'332345'}}")
        MsgBox(json.SelectToken("details").SelectToken("name"))
    End Sub
    Sub connectProcess()
        Dim profileXml As String = "<?xml version=""1.0""?><WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">       <name>{0}</name>       <SSIDConfig>       <SSID>       <name>{0}</name>       </SSID>       </SSIDConfig>       <connectionType>ESS</connectionType>       <connectionMode>manual</connectionMode>       <MSM>       <security>       <authEncryption>       <authentication>WPA2PSK</authentication>       <encryption>AES</encryption>       <useOneX>false</useOneX>       </authEncryption>       <sharedKey>       <keyType>passPhrase</keyType>       <protected>false</protected>       <keyMaterial>{1}</keyMaterial>       </sharedKey>       </security>       </MSM>       <MacRandomization xmlns=""http://www.microsoft.com/networking/WLAN/profile/v3"">       <enableRandomization>false</enableRandomization>       </MacRandomization>       </WLANProfile>"
        profileXml = String.Format(profileXml, "NO FREE", "w!fIdisable123")
        iface.Connect(WlanConnectionMode.TemporaryProfile, Dot11BssType.Any, profileXml)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs)
        connectProcess()
    End Sub

    Public Sub processLog(ByVal [source] As Object, ByVal e As EntryWrittenEventArgs)
        MsgBox(e.Entry.Message)
        If e.Entry.Message = "#CONNECTED" Then
            If Not My.Settings.ssidCollection.Contains(curr_ssid) Then
                MsgBox("you are connectd using default key")
            End If
        End If

        If e.Entry.Message = "#NOTCONNECTED" Then
            MsgBox("service going to stop")
            ServiceController1.Stop()
            Select Case connect_using
                Case ConnectState.CONNECT_NORMAL
                    MsgBox("connection try with custom1")
                    connect_using = ConnectState.CONNECT_CUSTOM
                    serviceHandler()
                Case ConnectState.CONNECT_DEFAULT
                    MsgBox("connection try with custom2")
                    connect_using = ConnectState.CONNECT_CUSTOM
                    serviceHandler()
                Case ConnectState.CONNECT_CUSTOM
                    connect_using = ConnectState.CONNECT_NORMAL
            End Select

        End If



    End Sub

    Private Sub cmbSSID_Click(sender As Object, e As EventArgs) Handles cmbSSID.Click
        fillSSIDList()
    End Sub

    Sub fillSSIDList()
        cmbSSID.Items.Clear()
        For Each i As WlanAvailableNetwork In iface.GetAvailableNetworkList(Nothing)
            Dim ssid = System.Text.ASCIIEncoding.ASCII.GetString(i.dot11Ssid.SSID, 0, i.dot11Ssid.SSIDLength)
            If Not cmbSSID.Items.Contains(ssid) Then
                cmbSSID.Items.Add(ssid)
            End If
        Next
    End Sub

    Private Shared Function Encode(ssid As String) As String
        Dim upper As String() = {"A", "B", "C", "D", "E", "F", _
            "G", "H", "I", "J", "K", "L", _
            "M", "N", "O", "P", "Q", "R", _
            "S", "T", "U", "V", "W", "X", _
            "Y", "Z"}
        Dim lower As String() = {"a", "b", "c", "d", "e", "f", _
            "g", "h", "i", "j", "k", "l", _
            "m", "n", "o", "p", "q", "r", _
            "s", "t", "u", "v", "w", "x", _
            "y", "z"}
        Dim symbols As String() = {"*", "#", "$", "&", "%"}
        Dim numbers As Integer() = {0, 1, 2, 3, 4, 5, _
            6, 7, 8, 9}

        Dim encode__1 As String = ssid
        While encode__1.Length < 10
            encode__1 += ssid
        End While

        encode__1 = encode__1.Substring(0, 10)


        Dim encoded_str As String = lower(CInt(AscW(encode__1(0))) Mod 26)

        encoded_str += numbers(CInt(AscW(encode__1(1))) Mod 10)
        encoded_str += numbers(CInt(AscW(encode__1(2))) Mod 10)
        encoded_str += symbols(CInt(AscW(encode__1(3))) Mod 5)
        encoded_str += lower(CInt(AscW(encode__1(4))) Mod 26)
        encoded_str += upper(CInt(AscW(encode__1(5))) Mod 26)
        encoded_str += lower(CInt(AscW(encode__1(6))) Mod 26)
        encoded_str += upper(CInt(AscW(encode__1(7))) Mod 26)
        encoded_str += numbers(CInt(AscW(encode__1(8))) Mod 10)
        encoded_str += symbols(CInt(AscW(encode__1(9))) Mod 5)

        Return encoded_str
    End Function

    Private Sub Button5_Click(sender As Object, e As EventArgs)
        My.Settings.ssidCollection.Clear()
        My.Settings.pryKeyCollection.Clear()
        My.Settings.secKeyCollection.Clear()
        My.Settings.Save()

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs)
        My.Settings.ssidCollection.Add("buhua")
        My.Settings.ssidCollection.Add("NO EE")
        My.Settings.pryKeyCollection.Add("elaaa")
        My.Settings.pryKeyCollection.Add("123456")
        My.Settings.secKeyCollection.Add("2323")
        My.Settings.secKeyCollection.Add("w!fIdisable13")
        My.Settings.Save()
        MsgBox("settings changed")


    End Sub

    Private Sub btnProfile_Click(sender As Object, e As EventArgs) Handles btnProfile.Click
        btnProfile.BackColor = Color.FromArgb(219, 219, 219)
        btnChangePackage.BackColor = Color.FromArgb(197, 197, 197)
        btnOffer.BackColor = Color.FromArgb(197, 197, 197)
        btnWant.BackColor = Color.FromArgb(197, 197, 197)
        btnMessage.BackColor = Color.FromArgb(197, 197, 197)
        btnPayment.BackColor = Color.FromArgb(197, 197, 197)

        tabControl.SelectedIndex = 0


    End Sub

    Private Sub btnChangePackage_Click(sender As Object, e As EventArgs) Handles btnChangePackage.Click
        btnProfile.BackColor = Color.FromArgb(197, 197, 197)
        btnChangePackage.BackColor = Color.FromArgb(219, 219, 219)
        btnOffer.BackColor = Color.FromArgb(197, 197, 197)
        btnWant.BackColor = Color.FromArgb(197, 197, 197)
        btnMessage.BackColor = Color.FromArgb(197, 197, 197)
        btnPayment.BackColor = Color.FromArgb(197, 197, 197)

        tabControl.SelectedIndex = 1

    End Sub

    Private Sub btnOffer_Click(sender As Object, e As EventArgs) Handles btnOffer.Click
        btnProfile.BackColor = Color.FromArgb(197, 197, 197)
        btnChangePackage.BackColor = Color.FromArgb(197, 197, 197)
        btnOffer.BackColor = Color.FromArgb(219, 219, 219)
        btnWant.BackColor = Color.FromArgb(197, 197, 197)
        btnMessage.BackColor = Color.FromArgb(197, 197, 197)
        btnPayment.BackColor = Color.FromArgb(197, 197, 197)

        tabControl.SelectedIndex = 2

    End Sub

    Private Sub btnWant_Click(sender As Object, e As EventArgs) Handles btnWant.Click
        btnProfile.BackColor = Color.FromArgb(197, 197, 197)
        btnChangePackage.BackColor = Color.FromArgb(197, 197, 197)
        btnOffer.BackColor = Color.FromArgb(197, 197, 197)
        btnWant.BackColor = Color.FromArgb(219, 219, 219)
        btnMessage.BackColor = Color.FromArgb(197, 197, 197)
        btnPayment.BackColor = Color.FromArgb(197, 197, 197)

        tabControl.SelectedIndex = 3

    End Sub

    Private Sub btnMessage_Click(sender As Object, e As EventArgs) Handles btnMessage.Click
        btnProfile.BackColor = Color.FromArgb(197, 197, 197)
        btnChangePackage.BackColor = Color.FromArgb(197, 197, 197)
        btnOffer.BackColor = Color.FromArgb(197, 197, 197)
        btnWant.BackColor = Color.FromArgb(197, 197, 197)
        btnMessage.BackColor = Color.FromArgb(219, 219, 219)
        btnPayment.BackColor = Color.FromArgb(197, 197, 197)

        tabControl.SelectedIndex = 4

    End Sub

    Private Sub btnPayment_Click(sender As Object, e As EventArgs) Handles btnPayment.Click
        btnProfile.BackColor = Color.FromArgb(197, 197, 197)
        btnChangePackage.BackColor = Color.FromArgb(197, 197, 197)
        btnOffer.BackColor = Color.FromArgb(197, 197, 197)
        btnWant.BackColor = Color.FromArgb(197, 197, 197)
        btnMessage.BackColor = Color.FromArgb(197, 197, 197)
        btnPayment.BackColor = Color.FromArgb(219, 219, 219)

        tabControl.SelectedIndex = 5

    End Sub
End Class
