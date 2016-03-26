Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI.Windows

Public Class ControlNotificationCentr
    Implements IContent
    Private ReadOnly _notifiications As New ObservableCollection(Of types.Notifycation)

    Public Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        Exit Sub
        Dim mode = e.Fragment.GetParametr("mode")
        TextBlock1.Visibility = Visibility.Collapsed
        If mode = "all" Then
            _notifiications.Add(New types.Notifycation)
            For Each n In OtherApi.NotificationList1
                _notifiications.Add(n)
            Next


        ElseIf mode = "favorite"

        End If
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
        If DataGrid1.ItemsSource Is Nothing
            DataGrid1.ItemsSource = OtherApi.NotificationList1
        End If
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub
End Class
