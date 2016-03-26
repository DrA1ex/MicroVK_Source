Imports FirstFloor.ModernUI.Windows
Imports MicroVK.Api

Class PageNews
    Implements IContent
    Private _fragment As String
    Private _newList As types.NewsList()

    Private Sub ListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If ListBox1.SelectedIndex > 0 Then
            Dim p = "filters=" & _fragment & If _
                        (Not IsNothing(CType(ListBox1.SelectedItem, ListBoxItem).Tag),
                         "&source_ids=" & CType(ListBox1.SelectedItem, ListBoxItem).Tag.ToString,
                         "")
            ModernFrame1.Source = New Uri("Content/ControllWall.xaml#" & p, UriKind.RelativeOrAbsolute)
        End If
    End Sub

    Public Async Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        _fragment = e.Fragment
        ListBox1.SelectedIndex = 0
        Dim p = "filters=" & _fragment
        ModernFrame1.Source = New Uri("Content/ControllWall.xaml#" & p, UriKind.RelativeOrAbsolute)
        If IsNothing(_newList) Then
            _newList = Await newsfeed.GetLists
            If Not IsNothing(_newList) Then
                For Each i In _newList
                    ListBox1.Items.Add(New ListBoxItem With {.Content = i.title.ToUpper,
                                          .Tag = "list" & i.id})
                Next
            End If
        End If
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class
