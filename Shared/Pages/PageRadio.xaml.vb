Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.OtherLib

Class PageRadio
    Implements IContent

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
        SettingSystem.SaveSettings("last_radio", SettingSystem.LastRadio)
    End Sub

    Public Async Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        dim temp = Await SettingSystem.LoadSettingsAsync (of List(Of types.audio))("last_radio")
        if temp IsNot Nothing
            SettingSystem.LastRadio = temp
        End If
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class
