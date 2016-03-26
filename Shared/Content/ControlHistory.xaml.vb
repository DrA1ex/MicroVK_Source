Imports FirstFloor.ModernUI.Windows
Imports System.Net

Public Class ControlHistory
    Implements IContent

    Public Async Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        If IsNothing(BBCodeBlock1.BBCode) Then
#If NET46 Then
            BBCodeBlock1.BBCode = Await (New WebClient().DownloadStringTaskAsync("history.txt"))
#ElseIf XP Then
            BBCodeBlock1.BBCode = IO.File.ReadAllText("history.txt")
#End If
        End If
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class
