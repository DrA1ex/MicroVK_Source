Imports FirstFloor.ModernUI.Presentation
Imports MahApps.Metro.Controls

Class PageLogin2
    Implements FirstFloor.ModernUI.Windows.IContent

    Public Sub OnFragmentNavigation(e As FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs) _
        Implements FirstFloor.ModernUI.Windows.IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs) _
        Implements FirstFloor.ModernUI.Windows.IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs) _
        Implements FirstFloor.ModernUI.Windows.IContent.OnNavigatedTo
        AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml", UriKind.RelativeOrAbsolute)
        If My.Settings.AccessToken.Count > 0 Then
            OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageMessage.xaml", UriKind.RelativeOrAbsolute)
            If _
                Deployment.Application.ApplicationDeployment.IsNetworkDeployed AndAlso
                My.Settings.Version <>
                Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() Then
                OtherApi.MyWindow1.ShowFlyout(Position.Right, "Content/ContentHistoryAndGroupJoin.xaml#1", "")
            End if
        end if
    End Sub

    Public Sub OnNavigatingFrom(e As FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs) _
        Implements FirstFloor.ModernUI.Windows.IContent.OnNavigatingFrom
        Select Case My.Settings.Theme
            Case 0
                AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml", UriKind.RelativeOrAbsolute)
            Case 1
#If Not xp Then
                AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImageDark.xaml",
                                                                UriKind.RelativeOrAbsolute)
#Else
                    AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml",
                                                                    UriKind.RelativeOrAbsolute)
#End If
        End Select
    End Sub

    Private Async Sub WebBrowser_DocumentCompleted(sender As Object, e As Forms.WebBrowserDocumentCompletedEventArgs)
        If e.Url.AbsoluteUri.IndexOf("access_token=", StringComparison.OrdinalIgnoreCase) > 0 Then
            Await OtherApi.LoginInToken(e.Url.AbsoluteUri)
        End If
    End Sub

    Private Sub PageLogin2_OnLoaded(sender As Object, e As RoutedEventArgs)
        WebBrowser1.Navigate(
            String.Format(
                "https://oauth.vk.com/authorize?client_id=2664100&scope=2080255&display=mobile&revoke=1&redirect_uri=https://oauth.vk.com/blank.html&v={0}&response_type=token",
                OtherApi.ApiVersion))
    End Sub
End Class
