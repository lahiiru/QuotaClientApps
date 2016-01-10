Imports System.Net
Imports System.ServiceProcess
Imports SimpleWifi.Win32
Imports SimpleWifi.Win32.Interop
Imports Twitterizer
Imports Newtonsoft.Json.Linq


Public Class Form1

    Public curr_ssid As String
    Public default_passKey As String = "w!fIdisable23"      'default passkey
    Private inactive_color As Color = Color.FromArgb(197, 197, 197)
    Private active_color As Color = Color.FromArgb(219, 219, 219)
    'connection state variable
    Public connect_using As ConnectState = ConnectState.CONNECT_NORMAL
    'connection method states
    Enum ConnectState
        CONNECT_DEFAULT
        CONNECT_NORMAL
        CONNECT_CUSTOM
    End Enum

    Sub loadForm()
        If My.Settings.ssidCollection Is Nothing Then
            My.Settings.ssidCollection = New Specialized.StringCollection
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
            Dim ssid = cmbSSID.Text
            If Not ssid Is Nothing Then
                curr_ssid = ssid
                'MsgBox("try to connect using primary..")
                serviceHandler()
            End If
        End If

        'if button caption is DISCONNECT
        If Button2.Text.ToUpper() = "DISCONNECT" Then
            ServiceController1.Refresh()
            If ServiceController1.Status = ServiceControllerStatus.Running Then
                ServiceController1.Stop()
            End If
        End If


    End Sub

    'handle the service while connecting -- send passwords to service and try to connect
    Sub serviceHandler()
        If Not My.Settings.ssidCollection.Contains(curr_ssid) And connect_using = ConnectState.CONNECT_NORMAL Then
            'MsgBox("connect using default")
            connect_using = ConnectState.CONNECT_DEFAULT
        End If

        Dim curr_passKey As String = getPassKey(curr_ssid)


        If curr_passKey.Length < 5 And connect_using = ConnectState.CONNECT_CUSTOM Then
            connect_using = ConnectState.CONNECT_NORMAL 'handles user input key errors
        Else
            Dim keyIndex As Integer = 0
            If My.Settings.secKeyCollection.Count() > My.Settings.ssidCollection.IndexOf(curr_ssid) Then
                keyIndex = My.Settings.ssidCollection.IndexOf(curr_ssid)
            End If
            startService(curr_ssid, keyIndex, curr_passKey) 'args- (SSID,keyIndex,[Cumstom passkey])
        End If
    End Sub

    'get passkeys according to states DIFAULT / NORMAL / CUSTOM
    Function getPassKey(ByVal ssid As String) As String
        Dim passKey As String = ""

        Select Case connect_using
            Case ConnectState.CONNECT_DEFAULT
                'MsgBox("connect default mode")
                passKey = default_passKey
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

    Sub startService(ByVal ssid As String, ByVal keyIndex As Integer, Optional ByVal customKey As String = "")
        Try
            ServiceController1.ServiceName = "Quota2"
            Try
                Dim temp = ServiceController1.Status
            Catch ex As Exception
                MsgBox("Oops! """ & serviceName & """ service is not intalled." & vbNewLine & "Please re-install Quota", MsgBoxStyle.Exclamation, "Service not found")
            End Try


            If Not ServiceController1.Status = ServiceControllerStatus.Running Then

                ' if service is still in stopping process stay here untill stop
                While (ServiceController1.Status = ServiceControllerStatus.StopPending)
                End While

                'if the service is stopped then only try to start the service
                If (ServiceController1.Status = ServiceControllerStatus.Stopped) Then
                    ServiceController1.Start({ssid, keyIndex, customKey})
                End If
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
    Public Sub processLog(ByVal [source] As Object, ByVal e As EntryWrittenEventArgs)
        With e.Entry
            'MsgBox("Service said: " & vbNewLine & .Message, MsgBoxStyle.Information)
            If .Message.Contains("#CONNECTED") Then
                connect_using = ConnectState.CONNECT_NORMAL
                'saving last connected ssid
                My.Settings.bssid = curr_ssid
                If Not My.Settings.ssidCollection.Contains(curr_ssid) Then
                    'MsgBox("you are connectd using default key")
                End If
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
            End If

        End With


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
        If cmbSSID.Items.Contains(My.Settings.bssid) Then
            cmbSSID.SelectedItem = My.Settings.bssid
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

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        MsgBox("aa" & Integer.Parse(MsgBoxStyle.Exclamation), 48)

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
