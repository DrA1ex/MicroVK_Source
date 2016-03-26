Imports FirstFloor.ModernUI.Windows
Imports System.Collections.ObjectModel
Imports MicroVK.Api

Public Class FriendsList
    Implements IContent

    Private FriendsCollection As ObservableCollection(Of types.user), _mode As String, _userId as string

    Public Async Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        _userId = e.Fragment.GetParametr("user_id")
        _mode = e.Fragment.GetParametr("mode")
        Select Case _mode
            Case "all"
                FriendsCollection = Await friends.get(_userId, "", "50", "first_name,")
            Case "online"
                FriendsCollection = Await friends.GetOnline(_userId)
            Case "mutual"
                FriendsCollection = Await friends.GetMutual(_userId)
        End Select
        ListBox1.ItemsSource = FriendsCollection
        If ListBox1.Items.Count > 0 Then
            ListBox1.ScrollIntoView(ListBox1.Items(0))
            ListBox1.SelectedIndex = 0
        End If
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub ListBox1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If ListBox1.SelectedIndex >= 0 Then
            Dim a = CType(ListBox1.SelectedItem, types.user).id
            ModernFrame1.Source = New Uri("/Pages/PageUserInfo.xaml#user_id=" & a, UriKind.RelativeOrAbsolute)
        End If
    End Sub

    Private Async Sub ListBox1_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        Dim p = CType(e.OriginalSource, ScrollViewer)
        p.ApplyTemplate()
        Dim a = p.Template.FindName("PART_VerticalScrollBar", p)
        Dim b As Primitives.ScrollBar = CType(a, Primitives.ScrollBar)
        If Not IsNothing(FriendsCollection) Then
            If _
                Math.Abs(b.Value - b.Maximum) < OtherApi.TOLERANCE And FriendsCollection.Count > 0 And
                FriendsCollection.Count Mod 50 = 0 Then

                Select Case _mode
                    Case "all"
                        For Each i In Await friends.Get(_userId, FriendsCollection.Count.ToString(), "50", "first_name")
                            FriendsCollection.Add(i)
                        Next
                    Case "online"
                        For Each i In Await friends.GetOnline(_userId, FriendsCollection.Count.ToString())
                            FriendsCollection.Add(i)
                        Next
                    Case "mutual"
                        For Each i In Await friends.GetMutual(_userId, FriendsCollection.Count.ToString())
                            FriendsCollection.Add(i)
                        Next
                End Select
            End If
        End If
    End Sub
End Class
