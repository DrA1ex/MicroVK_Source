' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public NotInheritable Class ContentFavoritesMessage
    Implements IContent
    Private ReadOnly _messageColection As New ObservableCollection(Of types.message)

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Async Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        Dim messagesList As types.vk_list = Nothing
        if ControlDialogListBox1.ListBox1.ItemsSource IsNot Nothing
            _messageColection.Clear()
        End If
        If _messageColection.Count = 0 Then
            messagesList = Await Messages.Get("8")
            ControlDialogListBox1.ListBox1.ItemsSource = _messageColection
            For Each item As types.message In messagesList.items
                _messageColection.Add(item)
            Next
        End If
        If OtherApi.MyWindow1.FlyoutUrl.Contains("ContentFavoritesMessage") andalso messagesList IsNot Nothing Then
            OtherApi.MyWindow1.Flyout1.Header = String.Format("{0} ({1})", My.Resources.Important, messagesList.count)
        End If
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub ContentFavoritesMessage_OnInitialized(sender As Object, e As EventArgs)
        ControlDialogListBox1.IsNotScroll = True
    End Sub

    Private Sub ContentFavoritesMessage_OnLoaded(sender As Object, e As RoutedEventArgs)
        ControlDialogListBox1.ReplyMenuItem.Visibility = Visibility.Collapsed
        ControlDialogListBox1.RootGrid1.VerticalAlignment = VerticalAlignment.Stretch

        _scrollViewer = TryCast(OtherApi.GetScrollViewer(ControlDialogListBox1.ListBox1), ScrollViewer)
        RemoveHandler _scrollViewer.ScrollChanged, AddressOf ListBox_ScrollChanged
        AddHandler _scrollViewer.ScrollChanged, AddressOf ListBox_ScrollChanged
    End Sub

    Private _scrollViewer As ScrollViewer

    Private Async Sub ListBox_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        If _
            e.VerticalOffset = _scrollViewer.ScrollableHeight AndAlso e.VerticalChange <> 0 AndAlso
            _messageColection.Count > 0 Then
            Dim messagesList = Await Messages.Get("8", _messageColection.Count.ToString())
            For Each item As types.message In messagesList.items
                _messageColection.Add(item)
            Next
        End If
    End Sub
End Class
