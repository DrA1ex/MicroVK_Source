
Imports MicroVK.Api

Class BrowserLogin
    Private Sub ButtonBase_OnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        OtherApi.ProcessStart(
            "https://oauth.vk.com/authorize?client_id=2664100&scope=2080255&display=mobile&revoke=1&redirect_uri=https://oauth.vk.com/blank.html&v=" &
            OtherApi.ApiVersion & "&response_type=token")
        'Dim a = Await MahApps.Metro.Controls.Dialogs.DialogManager.ShowInputAsync(MyWindow1, "dfdf", "fdfdf")
    End Sub

    Private Async Sub LoginButton_OnClick(sender As Object, e As RoutedEventArgs)
        LoginErrorTextBlock.Text = ""
        Dim t = TokenTextBox1.Text.GetParametr("access_token")
        If t.Length = 0 Then
            LoginErrorTextBlock.Text = My.Resources.BrowserLoginError1
            Exit Sub

        End If
        dim tempToken = my.Settings.AccessToken
        my.Settings.AccessToken = t
        Dim a = Await users.get()
        My.Settings.AccessToken = tempToken

        OtherApi.MyWindow1.Activate()
        If IsNothing(a) OrElse a.Count = 0 Then
            LoginErrorTextBlock.Text = My.Resources.BrowserLoginError2
        Else
            Await OtherApi.LoginInToken(TokenTextBox1.Text)
        End If
    End Sub
End Class
