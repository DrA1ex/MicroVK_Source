' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports MicroVK.Command
Imports MicroVK.OtherLib

Public NotInheritable Class AccountManager
    Inherits UserControl

    Private Sub AccountManager_OnLoaded(sender As Object, e As RoutedEventArgs)
        ListBox1.ItemsSource = SettingSystem.MicroVkAccounts
    End Sub

    Private Sub RestartButton_OnClick(sender As Object, e As RoutedEventArgs)
        My.Settings.AccessToken = ""
        System.Windows.Forms.Application.Restart()
        My.Application.Shutdown()
    End Sub

    Private Sub ListBox1_OnMouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        dim a = TryCast(ListBox1.SelectedItem, MicroVkAccount)
        if a IsNot nothing
            dim c = new LoginCommand
            c.Execute(a)
        End If
    End Sub
End Class
