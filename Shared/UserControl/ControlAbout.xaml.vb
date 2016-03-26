Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public Class ControlAbout
    Implements IContent

    Private Sub ControlAbout_Onloaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
            TextBlock1.Visibility = Visibility.Collapsed

            'XP
            'Run1.Text = Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() & " для Windows XP"

        Else
            TextBlock1.Visibility = Visibility.Visible
            TextBlock1.Text = My.Resources.VersionError
        End If
    End Sub

    Private Sub Hyperlink_Click(sender As Object, e As RoutedEventArgs)
        OtherApi.ProcessStart("license.rtf")
    End Sub

    Private Sub Button1_OnClick(sender As Object, e As RoutedEventArgs)
        My.Settings.isButton = Not My.Settings.isButton
    End Sub

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Async Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        If Button1.Visibility = Visibility.Visible or true
            Exit Sub
        End If
        OtherApi.ModernProgressRing1.Visibility = Visibility.Visible
        Dim b = Await groups.IsMember("microvk")
        if b
            Button1.Visibility = Visibility.Visible
        End If
        OtherApi.ModernProgressRing1.Visibility = Visibility.Collapsed
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub Button1_OnLoaded(sender As Object, e As RoutedEventArgs)
        Button1.Visibility = If(OtherApi.MyUser IsNot Nothing, Visibility.Visible, Visibility.Collapsed)
    End Sub
End Class
