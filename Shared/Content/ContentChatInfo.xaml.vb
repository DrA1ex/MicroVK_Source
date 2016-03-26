Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public Class ContentChatInfo
    Implements IContent
    Private chat_id As String

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        chat_id = e.Fragment.GetParametr(chat_id)
        Dim chat = Await messages.GetChatAndProfile(chat_id, "photo_50")
        Me.DataContext = chat
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub Control_OnMouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Dim s = CType(ListBox1.SelectedItem, types.profile)
        If s IsNot Nothing Then
            OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageUserInfo.xaml#user_id=" & s.id,
                                                       UriKind.RelativeOrAbsolute)
            OtherApi.MyWindow1.Flyout1.IsOpen = False
        End If
    End Sub
End Class
