Imports System.Windows.Interop

Public Class HotKeyManager
    Private Shared _hlManager As CSharp.Helpers.HotKeyManager

    Friend Shared Sub InitializeHotkeys()
        _hlManager = New CSharp.Helpers.HotKeyManager(New WindowInteropHelper(Application.Current.MainWindow).Handle)
        _hlManager.RegisterHotkey(ModifierKeys.None,
                                  Key.MediaNextTrack,
                                  Async Sub(key)
                                      If Not My.Settings.IsMediaKey Then Exit Sub
                                      If OtherApi.BassPlayer1 Is Nothing Then
                                          Await WindowPlayer.PlayRandom()
                                      Else
                                          OtherApi.BassPlayer1?.NextTrack(False, True)
                                     End If
                                     End Sub)

        _hlManager.RegisterHotkey(ModifierKeys.None,
                                  Key.MediaPreviousTrack,
                                  Async Sub(key)
                                      If Not My.Settings.IsMediaKey Then Exit Sub
                                      If OtherApi.BassPlayer1 Is Nothing Then
                                          Await WindowPlayer.PlayRandom()
                                      Else
                                          OtherApi.BassPlayer1?.PreviousTrack(False, True)
                                     End If
                                     End Sub)

        _hlManager.RegisterHotkey(ModifierKeys.None,
                                  Key.MediaStop,
                                  Sub(key)
                                      If Not My.Settings.IsMediaKey Then Exit Sub
                                      If OtherApi.BassPlayer1 IsNot Nothing Then
                                          OtherApi.BassPlayer1?.Stop()
                                     End If
                                     End Sub)

        _hlManager.RegisterHotkey(ModifierKeys.None,
                                  Key.MediaPlayPause,
                                  Async Sub(key)
                                      If Not My.Settings.IsMediaKey Then Exit Sub
                                      If OtherApi.BassPlayer1 Is Nothing Then
                                          Await WindowPlayer.PlayRandom()
                                      Else
                                          OtherApi.BassPlayer1?.PlayPause()
                                     End If
                                     End Sub)
    End Sub
End Class
