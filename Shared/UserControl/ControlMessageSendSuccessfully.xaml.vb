' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

Public NotInheritable Class ControlMessageSendSuccessfully
    Inherits UserControl

    Private Sub ControlMessageSendSuccessfully_OnLoaded(sender As Object, e As RoutedEventArgs)
        Dim n = TryCast(DataContext, types.Notifycation)
        Dim text As String
        If n.type = 41 AndAlso My.Settings.speak41 Then
            text = UserNameRun1.Text & " " & My.Resources.SendSuccessfully
            'If My.Settings.SpeakMessage Then
            '    Text = Text & ". " & TextBlockHelper.GetRawText(PreviewTextBlock1)
            'End If
            Speak.Speak(text)
        End If
    End Sub
End Class
