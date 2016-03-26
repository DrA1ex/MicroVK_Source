Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public Class ContentSearchGlobal
    Implements IContent
    Private q As String

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        q = e.Fragment.GetParametr("q")
        Dim hints = Await search.GetHints(q)
        ListBox1.ItemsSource = hints
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub ListBox1_OnMouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        Dim s = CType(ListBox1.SelectedItem, types.SearchHints)
        If s IsNot Nothing Then
            Dim mode = OtherApi.MyWindow1.FlyoutUrl.GetParametr("mode")
            Dim forward_messages = OtherApi.MyWindow1.FlyoutUrl.GetParametr("forward_messages")
            If s.profile IsNot Nothing AndAlso s.profile.deactivated = "MicroVK2048" And False Then
                OtherApi.MyWindow1.ContentSource = New Uri("2048/Game2048.xaml", UriKind.RelativeOrAbsolute)
            Else
                Select Case mode
                    Case "forward_messages"
                        OtherApi.MyWindow1.ContentSource =
                            New Uri(
                                "Pages/PageMessage.xaml#" & "user_id=" & s.profile.id & "&forward_messages=" &
                                forward_messages,
                                UriKind.RelativeOrAbsolute)
                    Case Else
                        OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageUserInfo.xaml#user_id=" & s.profile.id,
                                                                   UriKind.RelativeOrAbsolute)
                End Select
            End If
            OtherApi.MyWindow1.Flyout1.IsOpen = False
        End If
    End Sub
End Class
