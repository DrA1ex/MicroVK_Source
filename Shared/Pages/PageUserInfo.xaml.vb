Imports FirstFloor.ModernUI.Windows
Imports MicroVK.Api

Public Class PageUserInfo
    Inherits UserControl
    Implements IContent

    Public Async Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        Dim user_id = e.Fragment.GetParametr("user_id")
        If IsNothing(OtherApi.MyUser) Then Return
        If user_id = "my" Then
            'MyWindow1.Link1.Source = New Uri("Pages/PageUserInfo.xaml#user_id=" & MyUser.id.ToString,
            '                                 UriKind.RelativeOrAbsolute)
            OtherApi.MyWindow1.Link2.Source =
                New Uri("Pages/PageFriends.xaml#mode=all&user_id=" & OtherApi.MyUser.id.ToString,
                        UriKind.RelativeOrAbsolute)

            DataContext = (Await users.GetInfo(OtherApi.MyUser.id.ToString))
        Else
            'MyWindow1.Link1.Source = New Uri("Pages/PageUserInfo.xaml#user_id=" & e.Fragment.GetParametr("user_id"),
            '                                 UriKind.RelativeOrAbsolute)
            'MyWindow1.Link2.Source = New Uri("Pages/PageFriends.xaml#mode=all&user_id=" & user_id,
            '                                 UriKind.RelativeOrAbsolute)

            DataContext = (Await users.GetInfo(user_id))
        End If
        TextBlock1.GetBindingExpression(TextBlock.TextProperty).UpdateTarget()
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataContext) Then Return
        OtherApi.MyWindow1.ContentSource =
            New Uri("Pages/PageMessage.xaml#user_id=" & CType(Me.DataContext, types.GetUserInfo).user.id,
                    UriKind.RelativeOrAbsolute)
    End Sub

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataContext) Then Exit Sub
        Dim u = CType(DataContext, types.GetUserInfo).user
        If u Is Nothing Then Exit Sub
        OtherApi.MyWindow1.LinkGo2(New Uri("Pages/PageFriends.xaml#mode=all&user_id=" & u.id,
                                           UriKind.RelativeOrAbsolute))
    End Sub

    Private Sub Hyperlink_Click_1(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataContext) Then Return
        Dim u = CType(DataContext, types.GetUserInfo).user
        If u Is Nothing Then Exit Sub
        OtherApi.MyWindow1.LinkGo2(New Uri("Pages/PageFriends.xaml#mode=online&user_id=" & u.id,
                                           UriKind.RelativeOrAbsolute))
    End Sub

    Private Sub Hyperlink_Click_2(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataContext) Then Return
        Dim u = CType(DataContext, types.GetUserInfo).user
        If u Is Nothing Then Exit Sub
        OtherApi.MyWindow1.LinkGo2(New Uri("Pages/PageFriends.xaml#mode=mutual&user_id=" & u.id,
                                           UriKind.RelativeOrAbsolute))
    End Sub
End Class
