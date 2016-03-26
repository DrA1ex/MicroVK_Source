
Imports System.Speech.Synthesis
Imports MicroVK.Api

Public Class ContentSettingNotification
    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim a = CType(sender, ComboBox).SelectedIndex
        If a <> My.Settings.NotificationH Then
            My.Settings.NotificationH = a
            OtherApi.GrowlNotifiactions1.Left = If _
                (CBool(My.Settings.NotificationH),
                 SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - OtherApi.LeftOffset,
                 SystemParameters.WorkArea.Left + OtherApi.TopOffset)
            OtherApi.ShowMicroVkNot("MicroVK", My.Resources.notifycation_aligment)
        End If
    End Sub

    Private Sub ComboBox_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)
        Dim a = CType(sender, ComboBox).SelectedIndex
        If a <> My.Settings.NotificationV Then
            My.Settings.NotificationV = a
            OtherApi.GrowlNotifiactions1.Top = If _
                (CBool(My.Settings.NotificationV),
                 SystemParameters.WorkArea.Top + SystemParameters.WorkArea.Height -
                 OtherApi.GrowlNotifiactions1.Notifications.Count*64 - OtherApi.TopOffset,
                 SystemParameters.WorkArea.Top + OtherApi.TopOffset)
            OtherApi.ShowMicroVkNot("MicroVK", My.Resources.notifycation_aligment)
        End If
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim a As types.Notifycation = Nothing
        If OtherApi.MyUser IsNot Nothing Then

            Select Case CInt(CType(sender, Button).Tag)
                Case 3
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .type = 3,
                        .text = My.Resources.notifycation_color_prewiew}
                Case 4
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .text = My.Resources.notifycation_color_prewiew,
                        .type = 4,
                        .Message = New types.message() With {.body = My.Resources.notifycation_color_prewiew},
                        .tag = "user_id=" & OtherApi.MyUser.id}
                Case 40
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .text = My.Resources.notifycation_color_prewiew,
                        .type = 40,
                        .tag = "user_id=" & OtherApi.MyUser.id}
                Case 41
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .type = 41,
                        .text = My.Resources.SendSuccessfully,
                        .tag = "user_id=" & OtherApi.MyUser.id}
                Case 8
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .type = 8,
                        .text = "(" & messages.DeviceNames(6) & ")"}
                Case 9
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .type = 9,
                        .text = "(" & messages.DeviceNames(6) & ")"}
                Case 61
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .type = 61}
                Case 62
                    a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                        .type = 62}
                Case Else
                    a = Nothing
            End Select
        End If
        If a IsNot Nothing Then
            OtherApi.ShowNotify({a}.ToList, True)
        End If
    End Sub

    Private Sub ComboBox1_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If ComboBox1.SelectedItem IsNot Nothing Then
            My.Settings.Voice = TryCast(ComboBox1.SelectedItem, VoiceInfo).Name
        End If
    End Sub

    Private Sub ContentSettingNotification_OnLoaded(sender As Object, e As RoutedEventArgs)
        ComboBox1.Items.Clear()
        ErrorTextBlock.Text = ""
        Try
            For Each i In Speak.SpeechSynthesizer1.GetInstalledVoices()
                If i.Enabled Then
                    ComboBox1.Items.Add(i.VoiceInfo)
                End If
            Next

            For Each i As VoiceInfo In ComboBox1.Items
                If i.Name = My.Settings.Voice Then
                    ComboBox1.SelectedItem = i
                    Exit For
                End If
            Next
        Catch ex As Exception
            ErrorTextBlock.Text = ex.Message
        End Try

        If ComboBox1.Items.Count > 0 AndAlso ComboBox1.SelectedItem Is Nothing Then
            ComboBox1.SelectedIndex = 0
        End If
    End Sub
End Class
