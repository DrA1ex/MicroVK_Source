Imports FirstFloor.ModernUI.Windows

Class PageFriends
    Inherits UserControl
    Implements IContent

    Public Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        Dim user_id = e.Fragment.GetParametr("user_id")
        Link1.Source = New Uri("/Content/FriendsList.xaml#mode=all&user_id=" & user_id, UriKind.RelativeOrAbsolute)
        Link2.Source = New Uri("/Content/FriendsList.xaml#mode=online&user_id=" & user_id, UriKind.RelativeOrAbsolute)
        Link3.Source = New Uri("/Content/FriendsList.xaml#mode=mutual&user_id=" & user_id, UriKind.RelativeOrAbsolute)
        Select Case e.Fragment.GetParametr("mode")
            Case "all" : ModernTab1.SelectedSource = Link1.Source
            Case "online" : ModernTab1.SelectedSource = Link2.Source
            Case "mutual" : ModernTab1.SelectedSource = Link3.Source
        End Select
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class


