<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.EventLog1 = New System.Diagnostics.EventLog()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Button2 = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel3 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ServiceController1 = New System.ServiceProcess.ServiceController()
        Me.cmbSSID = New System.Windows.Forms.ComboBox()
        Me.tabControl = New System.Windows.Forms.TabControl()
        Me.tabProfile = New System.Windows.Forms.TabPage()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.tabChnageMyPackage = New System.Windows.Forms.TabPage()
        Me.tabOffer = New System.Windows.Forms.TabPage()
        Me.tabWant = New System.Windows.Forms.TabPage()
        Me.tabMessage = New System.Windows.Forms.TabPage()
        Me.tabPayments = New System.Windows.Forms.TabPage()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.btnProfile = New System.Windows.Forms.Button()
        Me.btnChangePackage = New System.Windows.Forms.Button()
        Me.btnOffer = New System.Windows.Forms.Button()
        Me.btnWant = New System.Windows.Forms.Button()
        Me.btnMessage = New System.Windows.Forms.Button()
        Me.btnPayment = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.tabControl.SuspendLayout()
        Me.tabProfile.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'EventLog1
        '
        Me.EventLog1.Log = "Application"
        Me.EventLog1.Source = "Quota"
        Me.EventLog1.SynchronizingObject = Me
        '
        'Timer1
        '
        '
        'Button2
        '
        Me.Button2.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button2.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.ForeColor = System.Drawing.Color.Black
        Me.Button2.Location = New System.Drawing.Point(455, 82)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(145, 36)
        Me.Button2.TabIndex = 14
        Me.Button2.Text = "Connect"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel3, Me.ToolStripStatusLabel1, Me.ToolStripStatusLabel2})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 468)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(616, 22)
        Me.StatusStrip1.TabIndex = 15
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel3
        '
        Me.ToolStripStatusLabel3.Name = "ToolStripStatusLabel3"
        Me.ToolStripStatusLabel3.Size = New System.Drawing.Size(30, 17)
        Me.ToolStripStatusLabel3.Text = "SSID"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(43, 17)
        Me.ToolStripStatusLabel1.Text = "v 0 Mb"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(45, 17)
        Me.ToolStripStatusLabel2.Text = "^ 0 Mb"
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.Navy
        Me.Label1.Location = New System.Drawing.Point(-1, 151)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(618, 10)
        Me.Label1.TabIndex = 20
        '
        'ServiceController1
        '
        Me.ServiceController1.ServiceName = "Quota"
        '
        'cmbSSID
        '
        Me.cmbSSID.FormattingEnabled = True
        Me.cmbSSID.Location = New System.Drawing.Point(455, 124)
        Me.cmbSSID.Name = "cmbSSID"
        Me.cmbSSID.Size = New System.Drawing.Size(143, 21)
        Me.cmbSSID.TabIndex = 22
        '
        'tabControl
        '
        Me.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
        Me.tabControl.Controls.Add(Me.tabProfile)
        Me.tabControl.Controls.Add(Me.tabChnageMyPackage)
        Me.tabControl.Controls.Add(Me.tabOffer)
        Me.tabControl.Controls.Add(Me.tabWant)
        Me.tabControl.Controls.Add(Me.tabMessage)
        Me.tabControl.Controls.Add(Me.tabPayments)
        Me.tabControl.Location = New System.Drawing.Point(103, 157)
        Me.tabControl.Name = "tabControl"
        Me.tabControl.SelectedIndex = 0
        Me.tabControl.Size = New System.Drawing.Size(514, 298)
        Me.tabControl.TabIndex = 23
        '
        'tabProfile
        '
        Me.tabProfile.BackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.tabProfile.Controls.Add(Me.Label6)
        Me.tabProfile.Controls.Add(Me.TextBox1)
        Me.tabProfile.Controls.Add(Me.Label11)
        Me.tabProfile.Controls.Add(Me.Button6)
        Me.tabProfile.Controls.Add(Me.Label9)
        Me.tabProfile.Controls.Add(Me.Button5)
        Me.tabProfile.Controls.Add(Me.Label10)
        Me.tabProfile.Controls.Add(Me.Button4)
        Me.tabProfile.Controls.Add(Me.Label8)
        Me.tabProfile.Controls.Add(Me.Button3)
        Me.tabProfile.Controls.Add(Me.Label5)
        Me.tabProfile.Controls.Add(Me.Label4)
        Me.tabProfile.Controls.Add(Me.Label2)
        Me.tabProfile.Controls.Add(Me.Button1)
        Me.tabProfile.Controls.Add(Me.Label16)
        Me.tabProfile.Location = New System.Drawing.Point(4, 25)
        Me.tabProfile.Name = "tabProfile"
        Me.tabProfile.Padding = New System.Windows.Forms.Padding(3)
        Me.tabProfile.Size = New System.Drawing.Size(506, 269)
        Me.tabProfile.TabIndex = 0
        Me.tabProfile.Text = "TabPage1"
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(44, 34)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(66, 23)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Remaining"
        '
        'TextBox1
        '
        Me.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TextBox1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.Location = New System.Drawing.Point(282, 39)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox1.Size = New System.Drawing.Size(194, 82)
        Me.TextBox1.TabIndex = 0
        Me.TextBox1.Text = "No announcements"
        '
        'Label11
        '
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(116, 103)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(100, 23)
        Me.Label11.TabIndex = 11
        Me.Label11.Text = "No"
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(387, 238)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(75, 23)
        Me.Button6.TabIndex = 34
        Me.Button6.Text = "CHANGE"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(116, 80)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(87, 23)
        Me.Label9.TabIndex = 12
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(310, 236)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(75, 23)
        Me.Button5.TabIndex = 33
        Me.Button5.Text = "CLEAR"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(44, 103)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(66, 23)
        Me.Label10.TabIndex = 13
        Me.Label10.Text = "Blacklisted"
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(229, 236)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(75, 23)
        Me.Button4.TabIndex = 32
        Me.Button4.Text = "Button4"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(44, 80)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(66, 23)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "Expired on"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(138, 236)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(85, 23)
        Me.Button3.TabIndex = 30
        Me.Button3.Text = "Update"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(44, 57)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(66, 23)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Package"
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(116, 57)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(100, 23)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "0"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(116, 34)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(13, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "0"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(47, 236)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(85, 23)
        Me.Button1.TabIndex = 25
        Me.Button1.Text = "Send tweet"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.Font = New System.Drawing.Font("Verdana", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(334, 56)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(142, 23)
        Me.Label16.TabIndex = 28
        '
        'tabChnageMyPackage
        '
        Me.tabChnageMyPackage.BackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.tabChnageMyPackage.Location = New System.Drawing.Point(4, 25)
        Me.tabChnageMyPackage.Name = "tabChnageMyPackage"
        Me.tabChnageMyPackage.Padding = New System.Windows.Forms.Padding(3)
        Me.tabChnageMyPackage.Size = New System.Drawing.Size(506, 269)
        Me.tabChnageMyPackage.TabIndex = 1
        Me.tabChnageMyPackage.Text = "TabPage2"
        '
        'tabOffer
        '
        Me.tabOffer.BackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.tabOffer.Location = New System.Drawing.Point(4, 25)
        Me.tabOffer.Name = "tabOffer"
        Me.tabOffer.Padding = New System.Windows.Forms.Padding(3)
        Me.tabOffer.Size = New System.Drawing.Size(506, 269)
        Me.tabOffer.TabIndex = 2
        Me.tabOffer.Text = "TabPage3"
        '
        'tabWant
        '
        Me.tabWant.BackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.tabWant.Location = New System.Drawing.Point(4, 25)
        Me.tabWant.Name = "tabWant"
        Me.tabWant.Padding = New System.Windows.Forms.Padding(3)
        Me.tabWant.Size = New System.Drawing.Size(506, 269)
        Me.tabWant.TabIndex = 3
        Me.tabWant.Text = "TabPage4"
        '
        'tabMessage
        '
        Me.tabMessage.BackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.tabMessage.Location = New System.Drawing.Point(4, 25)
        Me.tabMessage.Name = "tabMessage"
        Me.tabMessage.Padding = New System.Windows.Forms.Padding(3)
        Me.tabMessage.Size = New System.Drawing.Size(506, 269)
        Me.tabMessage.TabIndex = 4
        Me.tabMessage.Text = "TabPage5"
        '
        'tabPayments
        '
        Me.tabPayments.BackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.tabPayments.Location = New System.Drawing.Point(4, 25)
        Me.tabPayments.Name = "tabPayments"
        Me.tabPayments.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPayments.Size = New System.Drawing.Size(506, 269)
        Me.tabPayments.TabIndex = 5
        Me.tabPayments.Text = "TabPage6"
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(433, 455)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(165, 13)
        Me.LinkLabel1.TabIndex = 27
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "http://edu.wearetrying.info/quota"
        '
        'btnProfile
        '
        Me.btnProfile.BackColor = System.Drawing.Color.FromArgb(CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer))
        Me.btnProfile.FlatAppearance.BorderSize = 0
        Me.btnProfile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnProfile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnProfile.Location = New System.Drawing.Point(4, 175)
        Me.btnProfile.Name = "btnProfile"
        Me.btnProfile.Size = New System.Drawing.Size(102, 45)
        Me.btnProfile.TabIndex = 24
        Me.btnProfile.Text = "Profile"
        Me.btnProfile.UseVisualStyleBackColor = False
        '
        'btnChangePackage
        '
        Me.btnChangePackage.BackColor = System.Drawing.Color.FromArgb(CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer))
        Me.btnChangePackage.FlatAppearance.BorderSize = 0
        Me.btnChangePackage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnChangePackage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnChangePackage.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnChangePackage.Location = New System.Drawing.Point(4, 220)
        Me.btnChangePackage.Name = "btnChangePackage"
        Me.btnChangePackage.Size = New System.Drawing.Size(102, 45)
        Me.btnChangePackage.TabIndex = 25
        Me.btnChangePackage.Text = "Change my package"
        Me.btnChangePackage.UseVisualStyleBackColor = False
        '
        'btnOffer
        '
        Me.btnOffer.BackColor = System.Drawing.Color.FromArgb(CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer))
        Me.btnOffer.FlatAppearance.BorderSize = 0
        Me.btnOffer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnOffer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnOffer.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnOffer.Location = New System.Drawing.Point(4, 265)
        Me.btnOffer.Name = "btnOffer"
        Me.btnOffer.Size = New System.Drawing.Size(102, 45)
        Me.btnOffer.TabIndex = 26
        Me.btnOffer.Text = "Offer data"
        Me.btnOffer.UseVisualStyleBackColor = False
        '
        'btnWant
        '
        Me.btnWant.BackColor = System.Drawing.Color.FromArgb(CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer))
        Me.btnWant.FlatAppearance.BorderSize = 0
        Me.btnWant.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnWant.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnWant.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnWant.Location = New System.Drawing.Point(4, 310)
        Me.btnWant.Name = "btnWant"
        Me.btnWant.Size = New System.Drawing.Size(102, 45)
        Me.btnWant.TabIndex = 27
        Me.btnWant.Text = "Request data"
        Me.btnWant.UseVisualStyleBackColor = False
        '
        'btnMessage
        '
        Me.btnMessage.BackColor = System.Drawing.Color.FromArgb(CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer))
        Me.btnMessage.FlatAppearance.BorderSize = 0
        Me.btnMessage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnMessage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnMessage.Location = New System.Drawing.Point(4, 355)
        Me.btnMessage.Name = "btnMessage"
        Me.btnMessage.Size = New System.Drawing.Size(102, 45)
        Me.btnMessage.TabIndex = 28
        Me.btnMessage.Text = "Send message"
        Me.btnMessage.UseVisualStyleBackColor = False
        '
        'btnPayment
        '
        Me.btnPayment.BackColor = System.Drawing.Color.FromArgb(CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer), CType(CType(197, Byte), Integer))
        Me.btnPayment.FlatAppearance.BorderSize = 0
        Me.btnPayment.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnPayment.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnPayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPayment.Location = New System.Drawing.Point(4, 400)
        Me.btnPayment.Name = "btnPayment"
        Me.btnPayment.Size = New System.Drawing.Size(102, 45)
        Me.btnPayment.TabIndex = 29
        Me.btnPayment.Text = "My payments"
        Me.btnPayment.UseVisualStyleBackColor = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.Quota.My.Resources.Resources._default
        Me.PictureBox1.Location = New System.Drawing.Point(-1, -2)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(618, 161)
        Me.PictureBox1.TabIndex = 17
        Me.PictureBox1.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.ClientSize = New System.Drawing.Size(616, 490)
        Me.Controls.Add(Me.btnPayment)
        Me.Controls.Add(Me.btnProfile)
        Me.Controls.Add(Me.btnMessage)
        Me.Controls.Add(Me.btnWant)
        Me.Controls.Add(Me.btnOffer)
        Me.Controls.Add(Me.btnChangePackage)
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.cmbSSID)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.tabControl)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Quota 1.0"
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.tabControl.ResumeLayout(False)
        Me.tabProfile.ResumeLayout(False)
        Me.tabProfile.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents EventLog1 As EventLog
    Friend WithEvents Timer1 As Timer
    Friend WithEvents Button2 As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label1 As Label
    Public WithEvents ServiceController1 As ServiceProcess.ServiceController
    Friend WithEvents ToolStripStatusLabel3 As ToolStripStatusLabel
    Friend WithEvents cmbSSID As System.Windows.Forms.ComboBox
    Friend WithEvents btnMessage As System.Windows.Forms.Button
    Friend WithEvents btnWant As System.Windows.Forms.Button
    Friend WithEvents btnOffer As System.Windows.Forms.Button
    Friend WithEvents btnChangePackage As System.Windows.Forms.Button
    Friend WithEvents btnProfile As System.Windows.Forms.Button
    Friend WithEvents tabControl As System.Windows.Forms.TabControl
    Friend WithEvents tabProfile As System.Windows.Forms.TabPage
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents tabChnageMyPackage As System.Windows.Forms.TabPage
    Friend WithEvents btnPayment As System.Windows.Forms.Button
    Friend WithEvents tabOffer As System.Windows.Forms.TabPage
    Friend WithEvents tabWant As System.Windows.Forms.TabPage
    Friend WithEvents tabMessage As System.Windows.Forms.TabPage
    Friend WithEvents tabPayments As System.Windows.Forms.TabPage
End Class
