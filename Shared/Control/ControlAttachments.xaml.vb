Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI.Windows.Controls
Imports MahApps.Metro.Controls

Public Class ControlAttachments
    Inherits UserControl

    Private Sub listBox_MouseDown(sender As Object, e As MouseButtonEventArgs)
        PhotoViewer.Attachments = New ObservableCollection(Of Object)(TryCast(Me.listBox.ItemsSource,
                                                                              List(Of types.attachment)))
        OtherApi.MyWindow1.ModernFrame1.Source = Nothing
        OtherApi.MyWindow1.ShowFlyout(Position.Right,
                                      "Content/Flyouts/PhotoViewer.xaml",
                                      OtherApi.AttachmentToString(
                                          TryCast(PhotoViewer.Attachments.FirstOrDefault, types.attachment)?.type) &
                                      " (1/" & Me.listBox.Items.Count & ")",
                                      True)
    End Sub

    Private Sub fv_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim a = CType(sender, MahApps.Metro.Controls.FlipView)
        CType(a.Tag, ModernDialog).Title = My.Resources.View_attachment & " (" & a.SelectedIndex + 1 & "/" &
                                           Me.listBox.Items.Count & ")"
    End Sub
End Class
