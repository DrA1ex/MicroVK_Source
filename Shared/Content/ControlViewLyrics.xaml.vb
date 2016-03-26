' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public NotInheritable Class ControlViewLyrics
    Inherits UserControl
    Implements IContent

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        dim lyrics_id = e.Fragment.GetParametr("lyrics_id")
        LyricsTextBox.Text = await audio.GetLyrics(lyrics_id)
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class
