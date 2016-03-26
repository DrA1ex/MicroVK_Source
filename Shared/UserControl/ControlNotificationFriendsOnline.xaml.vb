
Imports MicroVK.Api

Public Class ControlNotificationFriendsOnline
    Private Sub ControlNotificationFriendsOnline_OnLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim a = CType(DataContext, types.Notifycation)
        If lists.ProfilesDictionary.ContainsKey(a.user_id) Then
            Select Case a.type
                Case 8
                    Run1.Text = OtherApi.SexToString(My.Resources.friend_online,
                                                     lists.ProfilesDictionary(a.user_id).sex)
                Case 9
                    Run1.Text = OtherApi.SexToString(My.Resources.friend_offline,
                                                     lists.ProfilesDictionary(a.user_id).sex)
                Case 61
                    Run1.Text = My.Resources.friend_typing
                Case 62
                    Run1.Text = My.Resources.friend_typing_chat
            End Select
        End If
        Select Case a.type
            Case 8
                If My.Settings.speak8 Then Speak.Speak(TextBlock1.Text)
            Case 9
                If My.Settings.speak9 Then Speak.Speak(TextBlock1.Text)
            Case 61, 62
                Dim key = a.user_id.ToString()
                If (My.Settings.speak61 AndAlso a.type = 61) Or (My.Settings.speak62 AndAlso a.type = 62) Then
                    If Speak.TypingDictionary.ContainsKey(key) Then
                        If DateDiff(DateInterval.Second, Speak.TypingDictionary(key), Now) > 60 Then
                            Speak.TypingDictionary(key) = Now
                            Speak.Speak(TextBlock1.Text)
                        End If
                    Else
                        Speak.TypingDictionary.Add(key, Now)
                        Speak.Speak(TextBlock1.Text)
                    End If
                End If
            Case Else
                Speak.Speak(TextBlock1.Text)
        End Select
    End Sub
End Class
