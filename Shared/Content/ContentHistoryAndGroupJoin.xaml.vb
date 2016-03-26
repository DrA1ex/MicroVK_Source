
Imports System.Net
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MicroVK.Api

Public Class ContentHistoryAndGroupJoin
    Implements IContent

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        My.Settings.isMicroVKGroup = true
        If IsNothing(BBCodeBlock1.BBCode) Then
#If Not XP Then
            BBCodeBlock1.BBCode = Await (New WebClient().DownloadStringTaskAsync("history.txt"))
#Else
            BBCodeBlock1.BBCode = IO.File.ReadAllText("history.txt")
#End If
            My.Settings.isMicroVKGroup = Await groups.IsMember("microvk")
        End If
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Async Sub MicroVKGroupJoin_OnClick(sender As Object, e As RoutedEventArgs)
        dim a = await groups.Join("35726826")
        if a = 1
            My.Settings.isMicroVKGroup = true
        End If
    End Sub
End Class
