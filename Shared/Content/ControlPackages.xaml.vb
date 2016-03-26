Imports System.Net
Imports FirstFloor.ModernUI.Windows

Public Class ControlPackages
    Implements IContent

    Public Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Async Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
        If IsNothing(BBCodeBlock1.BBCode) Then
#If Not XP Then
            BBCodeBlock1.BBCode = Await (New WebClient().DownloadStringTaskAsync("packages.txt"))
#Else
            BBCodeBlock1.BBCode = IO.File.ReadAllText("packages.txt")
#End If
        End If
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class
