Imports MicroVK.WPFGrowlNotification

Public Class ControlNotificationNewVersion
    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ContentSource = New Uri("Content/ControlHistory.xaml#1", UriKind.RelativeOrAbsolute)
        For Each a As Notification In OtherApi.GrowlNotifiactions1.Notifications
            If TypeOf a.Content Is ControlNotificationNewVersion Then
                OtherApi.GrowlNotifiactions1.Notifications.Remove(a)
                Exit For
            End If
        Next
    End Sub
End Class
