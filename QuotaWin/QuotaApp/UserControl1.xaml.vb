Imports System.Windows
Imports Quota

Public Class UserControl1
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Shared Widening Operator CType(v As UserControl1) As UserControl
        Throw New NotImplementedException()
    End Operator

    Private Sub button_Copy1_Click(sender As Object, e As RoutedEventArgs) Handles button_Copy1.Click
        Form1.Button2_Click()
    End Sub

    Private Sub comboBox_SelectionChanged(sender As Object, e As Controls.SelectionChangedEventArgs) Handles comboBox.SelectionChanged

    End Sub
End Class
