Namespace WPFGrowlNotification
    Partial Public Class GrowlNotifiactions
        Private Const MaxNotifications As Byte = 8
        Private _count As Integer
        Public IndexCounter As Integer
        Public Notifications As New Notifications()
        Private ReadOnly buffer As New Notifications()

        Public Sub AddNotification(notification As Notification)
            notification.Id = System.Math.Max(System.Threading.Interlocked.Increment(_count), _count - 1)
            If Notifications.Count + 1 > MaxNotifications Then
                buffer.Add(notification)
            Else
                Notifications.Add(notification)
            End If
            'Show window if there're notifications
            If Notifications.Count > 0 AndAlso Not IsActive Then
                If IsNothing(NotificationsControl.DataContext) Then NotificationsControl.DataContext = Notifications
                Show()
            End If
        End Sub


        Public Sub RemoveNotification(notification As Notification)
            If Notifications.Contains(notification) Then
                Notifications.Remove(notification)
            End If

            If buffer.Count > 0 Then
                Notifications.Add(buffer(0))
                buffer.RemoveAt(0)
            End If
            'Close window if there's nothing to show

            If Notifications.Count < 1 Then
                Hide()
            End If
        End Sub

        Public Sub RemoveByType(type As Integer)
            For i = Notifications.Count - 1 To 0 Step - 1
                Dim n As Notification = Notifications(i)
                If n.Type = 0 Then
                    Notifications.RemoveAt(i)
                End If
            Next
        End Sub

        Private Sub NotificationWindowSizeChanged(sender As Object, e As SizeChangedEventArgs)
            If Math.Abs(e.NewSize.Height - 0) > OtherApi.TOLERANCE Then
                Return
            End If
            Dim element = TryCast(sender, Grid)
            RemoveNotification(Notifications.First(Function(n) n.Id = Int32.Parse(element.Tag.ToString())))
        End Sub

        Private Sub NotificationsControl_SizeChanged(sender As Object, e As SizeChangedEventArgs)
            If My.Settings.NotificationV = 0 And My.Settings.NotificationH = 1 Then
                OtherApi.TopOffset = 20
            Else
                OtherApi.TopOffset = 5
                OtherApi.GrowlNotifiactions1.Left = If _
                    (CBool(My.Settings.NotificationH),
                     SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - OtherApi.LeftOffset,
                     SystemParameters.WorkArea.Left + OtherApi.TopOffset)
            End If
            OtherApi.GrowlNotifiactions1.Top = If _
                (CBool(My.Settings.NotificationV),
                 SystemParameters.WorkArea.Top + SystemParameters.WorkArea.Height - Me.ActualHeight - OtherApi.TopOffset,
                 SystemParameters.WorkArea.Top + OtherApi.TopOffset)
        End Sub

        Private Sub ContentControl1_MouseRightButtonDown(sender As Object, e As MouseButtonEventArgs)
            For Each a As Notification In OtherApi.GrowlNotifiactions1.Notifications
                If a.Content.Equals(e.Source) Then
                    OtherApi.GrowlNotifiactions1.Notifications.Remove(a)
                    Exit For
                End If
            Next
        End Sub
    End Class
End Namespace
