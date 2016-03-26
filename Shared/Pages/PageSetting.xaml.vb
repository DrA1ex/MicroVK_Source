Imports FirstFloor.ModernUI.Windows

Class PageSetting
    Implements IContent

    Public Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        ModernTab1.SelectedSource = ModernTab1.Links(CInt(e.Fragment)).Source
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
        Call OtherApi.SaveSettingsToVK()
    End Sub
End Class
