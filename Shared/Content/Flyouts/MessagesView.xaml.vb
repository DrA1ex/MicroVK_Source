' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation

Public NotInheritable Class MessagesView
    Inherits UserControl
    Implements IContent

    Public Shared Messages As List(Of types.message)

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        ControlDialogListBox1.ListBox1.ItemsSource = Messages
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub MessagesView_OnInitialized(sender As Object, e As EventArgs)
        ControlDialogListBox1.IsNotScroll = true
    End Sub

    Private Sub MessagesView_OnLoaded(sender As Object, e As RoutedEventArgs)
        ControlDialogListBox1.ReplyMenuItem.Visibility = Visibility.Collapsed
        ControlDialogListBox1.ReSendMenuItem.Visibility = Visibility.Collapsed
        ControlDialogListBox1.MarkAsImportantMenuItem.Visibility = Visibility.Collapsed
        ControlDialogListBox1.DeleteMenuItem.Visibility = Visibility.Collapsed
        ControlDialogListBox1.RootGrid1.VerticalAlignment = VerticalAlignment.Stretch
    End Sub
End Class
