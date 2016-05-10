Imports System.ServiceProcess

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class QuotaService
    Inherits System.ServiceProcess.ServiceBase

    'UserService overrides dispose to clean up the component list.
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

    ' The main entry point for the process
    <MTAThread()>
    <System.Diagnostics.DebuggerNonUserCode()>
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' More than one NT Service may run within the same process. To add
        ' another service to this process, change the following line to
        ' create a second service object. For example,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New QuotaService}

        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  
    ' Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.EventLog1 = New System.Diagnostics.EventLog()
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'EventLog1
        '
        Me.EventLog1.Log = "Application"
        Me.EventLog1.Source = "Quota"
        '
        'QuotaService
        '
        Me.ServiceName = "Service1"
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

    Friend WithEvents EventLog1 As EventLog
End Class
