' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public NotInheritable Class ContentDeleteDialog
    Inherits UserControl
    Implements IContent

    Private user_id As String
    Private chat_id As String

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        user_id = e.Fragment.GetParametr("user_id")
        chat_id = e.Fragment.GetParametr("chat_id")
        TitleTextBlock.Text = e.Fragment.GetParametr("title")
        If chat_id.ToInt > 0 Then
            user_id = ""
        End If
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Async Sub Confirm_OnClick(sender As Object, e As RoutedEventArgs)
        ModernProgressRing1.Visibility = Visibility.Visible
        Dim result =
                Await _
                messages.DeleteDialog(user_id, chat_id, CInt(CountNumericUpDown.Value), CInt(OffsetNumericUpDown.Value))
        If result = 1 Then
            OtherApi.MyWindow1.Flyout1.IsOpen = False
        End If
        ModernProgressRing1.Visibility = Visibility.Collapsed
        messages.DialogsList1.Clear()
        messages.DialogsDictionary.Clear()
        Await messages.GetDialogs
    End Sub
End Class
