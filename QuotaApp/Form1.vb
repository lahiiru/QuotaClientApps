Imports System.Net
Imports System.ServiceProcess
Imports SimpleWifi.Win32
Imports SimpleWifi.Win32.Interop
Imports Twitterizer
Imports Newtonsoft.Json.Linq


Public Class Form1

    Sub load()
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
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim sInfo As New ProcessStartInfo("http://edu.wearetrying.info/quota")
        Process.Start(sInfo)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            ServiceController1.Refresh()
            EventLog1.Source = "Quota"

            Dim msg As String
            Dim update_msg As String
            Dim log = EventLog1.Entries(EventLog1.Entries.Count - 1).Message

            If log.Contains("#MSG") Then
                msg = log.Replace("#MSG", "")
                MsgBox("Message : " & msg)
            ElseIf log.Contains("#UPDATE") Then
                update_msg = log.Replace("#UPDATE", "")
                MsgBox("UPDATE msg : " & update_msg)
            End If

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

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
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

        Try
            ServiceController1.ServiceName = "Quota2"
            Try
                Dim temp = ServiceController1.Status
            Catch ex As Exception
                MsgBox("Oops! """ & serviceName & """ service is not intalled." & vbNewLine & "Please re-install Quota", MsgBoxStyle.Exclamation, "Service not found")
            End Try

            If Not ServiceController1.Status = ServiceControllerStatus.Running Then
                ServiceController1.Start()
            Else
                ServiceController1.Stop()
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Service error")
        End Try
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim json As JObject = JObject.Parse("{'status':'OK','details':{'name':'Lahiru Slave','package':5000000,'usage':'332345'}}")
        MsgBox(json.SelectToken("details").SelectToken("name"))
    End Sub
    Sub connectProcess()
        Dim profileXml As String = "<?xml version=""1.0""?>       <WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">       <name>{0}</name>       <SSIDConfig>       <SSID>       <name>{0}</name>       </SSID>       </SSIDConfig>       <connectionType>ESS</connectionType>       <connectionMode>manual</connectionMode>       <MSM>       <security>       <authEncryption>       <authentication>WPA2PSK</authentication>       <encryption>AES</encryption>       <useOneX>false</useOneX>       </authEncryption>       <sharedKey>       <keyType>passPhrase</keyType>       <protected>false</protected>       <keyMaterial>{1}</keyMaterial>       </sharedKey>       </security>       </MSM>       <MacRandomization xmlns=""http://www.microsoft.com/networking/WLAN/profile/v3"">       <enableRandomization>false</enableRandomization>       </MacRandomization>       </WLANProfile>"
        profileXml = String.Format(profileXml, "NO FREE", "w!fIdisable123")
        iface.Connect(WlanConnectionMode.TemporaryProfile, Dot11BssType.Any, profileXml)
    End Sub
  
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        connectProcess()
    End Sub
End Class
