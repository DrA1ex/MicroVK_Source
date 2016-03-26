Imports MicroVK.Api

Public Class ControlNotificationMessageDeleteFlags
    Private Sub ControlNotificationMessageDeleteFlags_OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim a = CType(DataContext, types.Notifycation)
        If lists.ProfilesDictionary.ContainsKey(a.user_id) Then
            Select Case a.type
                Case 3
                    If lists.ProfilesDictionary(a.user_id).sex = 0 Then
                        Run1.Text = My.Resources.read_your_message
                    Else
                        Run1.Text = If _
                            (lists.ProfilesDictionary(a.user_id).sex = 1,
                             My.Resources.read_your_message1,
                             My.Resources.read_your_message2)
                    End If
            End Select
        Else
            Run1.Text = My.Resources.read_your_message
        End If
        If My.Settings.speak3 Then
            Speak.Speak(TextBlock1.Text)
        End If
    End Sub
End Class
