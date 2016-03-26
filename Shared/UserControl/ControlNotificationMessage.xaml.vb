Imports MicroVK.Api

Public Class ControlNotificationMessage
    Private Sub Expander_Expanded(sender As Object, e As RoutedEventArgs)
        If IsInitialized Then
            SendControl1.Visibility = Visibility.Visible
            ScrollViewer1.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub Expander1_Collapsed(sender As Object, e As RoutedEventArgs)
        SendControl1.Visibility = Visibility.Collapsed
        ScrollViewer1.Visibility = Visibility.Visible
    End Sub

    Private Sub ScrollViewer1_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        If Not CBool(ScrollViewer1.Tag) Then
            If ScrollViewer1.ComputedVerticalScrollBarVisibility = Visibility.Visible Then
                PreviewTextBlock1.Visibility = Visibility.Collapsed
                Expander1.IsExpanded = False
            Else
                Expander1.IsExpanded = True
            End If
            ScrollViewer1.Tag = True
        End If
    End Sub

    Private Sub ControlNotificationMessage_OnInitialized(sender As Object, e As EventArgs)
        SendControl1.RootBorder.Background = Brushes.White
        SendControl1.TextBox1.Foreground = Brushes.Black
        SendControl1.AddAttachmentButton.Visibility = Visibility.Collapsed
        SendControl1.SeparatorRectangle1.Visibility = Visibility.Collapsed
        SendControl1.SendSettingButton.Visibility = Visibility.Collapsed
        SendControl1.SmileListBox.Visibility = Visibility.Collapsed
        AddHandler SendControl1.KeyDown, AddressOf SendControl1_KeyDown
        AddHandler SendControl1.OnSending, Sub()
                                           End Sub
        AddHandler SendControl1.OnSend, Sub()
            Dim n = TryCast(DataContext, types.Notifycation)
            Dim i = n.Index
            Dim n1 = OtherApi.GrowlNotifiactions1.Notifications.FirstOrDefault(Function(notification) _
                                                                                  TryCast(
                                                                                      TryCast(notification.Content,
                                                                                              FrameworkElement).
                                                                                      DataContext,
                                                                                      types.Notifycation).Index = i)
            If My.Settings.Notification41 AndAlso My.Settings.SendSuccessfullyNotify Then
                OtherApi.ShowNotify(
                    New List(Of types.Notifycation)({New types.Notifycation With {.type = 41, .user_id = n.user_id,
                                                       .Message =
                                                       New types.message() With {.body = SendControl1.LastText}}}))
            End If
            If n1 IsNot Nothing Then OtherApi.GrowlNotifiactions1.RemoveNotification(n1)
                                        End Sub
    End Sub

    Private Sub PinnedNotyfication()
        Dim i = TryCast(DataContext, types.Notifycation).Index
        Dim n = OtherApi.GrowlNotifiactions1.Notifications.FirstOrDefault(Function(notification) _
                                                                             TryCast(
                                                                                 TryCast(notification.Content,
                                                                                         FrameworkElement).DataContext,
                                                                                 types.Notifycation).Index = i)
        If n IsNot Nothing Then
            n.IsPinned = True
        End If
    End Sub

    Private Sub SendControl1_KeyDown(sender As Object, e As KeyEventArgs)
        PinnedNotyfication()
        RemoveHandler SendControl1.KeyDown, AddressOf SendControl1_KeyDown
    End Sub

    Private Sub ControlNotificationMessage_OnPreviewMouseDown(sender As Object, e As MouseButtonEventArgs)
        PinnedNotyfication()
        If PreviewTextBlock1.Visibility = Visibility.Visible Then
            PreviewTextBlock1.Visibility = Visibility.Collapsed
            SendControl1.TextBox1.FontSize = 12
            SendControl1.TextBox1.MaxHeight = 100
            'SendControl1.TextBox1.Focus()
            Keyboard.Focus(SendControl1.TextBox1)
            Dim m = TryCast(DataContext, types.Notifycation).Message
            If m IsNot Nothing AndAlso Not m.read_state Then
                Messages.MarkAsRead(TryCast(DataContext, types.Notifycation).tag, m.id.ToString())
            End If
        End If
    End Sub

    Private Sub PreviewTextBlock1_OnLoaded(sender As Object, e As RoutedEventArgs)
        SendControl1.TextBox1.Height = PreviewTextBlock1.ActualHeight + 5
    End Sub

    Private Sub ControlNotificationMessage_OnLoaded(sender As Object, e As RoutedEventArgs)
        Dim n = TryCast(DataContext, types.Notifycation)
        Dim text As String
        If n.type = 4 AndAlso My.Settings.speak4 Then
            text = UserNameRun1.Text & " " & My.Resources.NewMessage
            If My.Settings.SpeakMessage Then
                text = text & ". " & TextBlockHelper.GetRawText(PreviewTextBlock1)
            End If
            Speak.Speak(text)
        ElseIf n.type = 40 AndAlso My.Settings.speak40 Then
            text = UserNameRun1.Text & " " & My.Resources.NewMessage
            If My.Settings.SpeakChatMessage Then
                text = text & ". " & TextBlockHelper.GetRawText(PreviewTextBlock1)
            End If
            Speak.Speak(text)
        End If
        SendControl1.RawKey = TryCast(DataContext, types.Notifycation).tag
    End Sub
End Class
