' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports MahApps.Metro.Controls

Public NotInheritable Class HelpProgram
    Inherits UserControl

    Private Sub Button1_OnClick(sender As Object, e As RoutedEventArgs)
        My.Settings.isButton = Not My.Settings.isButton
    End Sub

    Private Sub DonateButton_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ContentSource = New Uri("Content/ControlDonate.xaml", UriKind.RelativeOrAbsolute)
        OtherApi.MyWindow1.Flyout1.IsOpen = False
    End Sub

    Private Sub ShareButton_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ShowFlyout(Position.Left, "Content/ControllShare.xaml", My.Resources.Share)
    End Sub

    Private Sub HelpProgram_OnLoaded(sender As Object, e As RoutedEventArgs)
        My.Settings.microVKHelp = True
    End Sub
End Class
