Imports System.ComponentModel

#if Not XP
Imports Yandex.Metrica

#End If

Public Class VideoPlayer
    Private _playerUrl As String

    Public Async Sub Play(video As types.video)
#If not XP
        if OtherApi.IsSendStatistic()
            YandexMetrica.ReportEvent("video_play")
        End If
#End If
        WebBrowser1.Visibility = Visibility.Hidden
        ModernProgressRing1.Visibility = Visibility.Visible
        Title = video.title
        If video.player Is Nothing Then
            Dim v =
                    (Await _
                    Api.video.Get(
                        video.owner_id & "_" & video.id &
                        If(Not String.IsNullOrEmpty(video.access_key), String.Format("_{0}", video.access_key), ""))).
                    FirstOrDefault()
            If (v IsNot Nothing) Then
                _playerUrl = v.player
            End If
        else
            _playerUrl = video.player
        End If
        WebBrowser1.Navigate(new Uri(_playerUrl))

        Show()
    End Sub


    Private Sub WebBrowser1_OnLoadCompleted(sender As Object, e As NavigationEventArgs)
        WebBrowser1.Visibility = Visibility.Visible
        ModernProgressRing1.Visibility = Visibility.Collapsed
    End Sub

    Private Sub VideoPlayer_OnClosing(sender As Object, e As CancelEventArgs)
        WebBrowser1.NavigateToString("5")
        e.Cancel = true
        me.Hide()
    End Sub

    Private Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        Button1.ContextMenu.IsOpen = True
    End Sub
End Class
