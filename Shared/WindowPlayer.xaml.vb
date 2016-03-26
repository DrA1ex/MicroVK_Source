
Imports System.Windows.Threading
Imports Un4seen.Bass
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Threading
Imports Blue.Private.Win32Imports
Imports Blue.Windows
Imports MicroVK.Api
Imports MicroVK.OtherLib

#If Not XP Then
Imports Yandex.Metrica

#End If

Public Class WindowPlayer
    Implements IDisposable

    Public stream As Integer,
           TimerPlayer1 As New DispatcherTimer With {.Interval = New TimeSpan(0, 0, 0, 0, 50)},
           Playlist1 As ObservableCollection(Of types.audio),
           PlaylistTecIndex1 As Integer,
           TokenSource As New CancellationTokenSource(),
           pubstream As Integer

    Dim _stickyWindow As StickyWindow

    Public Async Sub Play(a As types.audio, Optional showing As Boolean = True)
        OtherApi.MyWindow1.SetButtonData(False)
        Me.Show()

        If a Is Nothing Then Exit Sub
#If Not XP Then
        If OtherApi.IsSendStatistic() Then

            If a.is_radio Then
                YandexMetrica.ReportEvent("radio_play")
            Else
                YandexMetrica.ReportEvent("audio_play")
            End If

        End If
#End If
        If a.is_radio Then

            Dim la = SettingSystem.LastRadio.FirstOrDefault(Function(audio) audio.url = a.url)
            If la IsNot Nothing Then
                SettingSystem.LastRadio.Remove(la)
            End If
            SettingSystem.LastRadio.Insert(0, a)
            If SettingSystem.LastRadio.Count > 100 Then
                SettingSystem.LastRadio.RemoveAt(100)
            End If
        End If

        Me.DataContext = a

        TimerPlayer1.Stop()
        Dim tempv = Bass.BASS_GetVolume
        Slider2.Value = 0
        Slider2.SelectionEnd = 0
        Slider2.Value = 0
        ProgressBar1.Visibility = Visibility.Visible
        OtherApi.MyWindow1.SetPlayerProgressRingVisible(True)
        ProgressBar1.IsIndeterminate = False
        ProgressBar1.IsIndeterminate = True
#If Not XP Then
        Dim stream1 = Await Task.Factory.StartNew(Function() _
                                                     Bass.BASS_StreamCreateURL(a.url,
                                                                               0,
                                                                               BASSFlag.BASS_DEFAULT Or
                                                                               BASSFlag.BASS_SAMPLE_FX,
                                                                               Nothing,
                                                                               System.IntPtr.Zero))
#Else
        Dim stream1 =
                Await _
                Task.Factory.StartNew(
                    Function() Bass.BASS_StreamCreateURL(a.url, 0, BASSFlag.BASS_DEFAULT, Nothing, Nothing),
                    TokenSource.Token)
#End If
        ProgressBar1.Visibility = Visibility.Collapsed
        OtherApi.MyWindow1.SetPlayerProgressRingVisible(False)
        Bass.BASS_SetVolume(tempv)
        Bass.BASS_StreamFree(stream)
        stream = stream1
        Slider2.Maximum = Bass.BASS_ChannelGetLength(stream1)
        Bass.BASS_ChannelPlay(stream1, False)
        Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, CSng(Slider1.Value/Slider1.Maximum))
        If Not IsNothing(OtherApi.MyUser) AndAlso a.owner_id = OtherApi.MyUser.id Then
            CheckBox1.IsChecked = True
        Else
            CheckBox1.IsChecked = False
        End If
        TimerPlayer1.Start()
        If CheckBox2.IsChecked AndAlso Not a.is_radio Then
            Await audio.SetBroadcast(a.owner_id & "_" & a.id)
            OtherApi.ShowMicroVkNot(a.artist & "-" & a.title, My.Resources.setaudioStatus)
        End If
        'stats.trackEvents(
        '    New StatsEvent() _
        '                     With{.e=stats.StatsEventString.audio_play,.audio_id=a.owner_id & "_" & a.id,
        '                     .source=stats.AudioPlaySourceString.audios_user})
    End Sub

    Private Sub Window_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        DragMove()
        Me.OnMouseLeftButtonDown(e)
    End Sub

    Private Sub Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, CSng(Slider1.Value/Slider1.Maximum))
    End Sub

    Private isActiveStream As BASSActive

    Private Async Sub TimerPlayer1_Tick(sender As Object, e As EventArgs)

        Dim er = Bass.BASS_ErrorGetCode
        If er <> BASSError.BASS_OK Then
            OtherApi.ShowMicroVkNot("Bass.dll error", er.ToString())
            TimerPlayer1.Stop()
            Exit Sub
        End If

        Dim bassActive = Bass.BASS_ChannelIsActive(stream)
        If (bassActive <> isActiveStream) Then
            isActiveStream = bassActive
            If bassActive <> BASSActive.BASS_ACTIVE_PLAYING Then
                OtherApi.MyWindow1.SetButtonData(True)
            Else
                OtherApi.MyWindow1.SetButtonData(False)
            End If
        End If
        If bassActive <> BASSActive.BASS_ACTIVE_PLAYING Then
            TimerPlayer1.Stop()
        End If
        Slider2.SelectionEnd = Slider2.Maximum*
                               (Bass.BASS_StreamGetFilePosition(stream, BASSStreamFilePosition.BASS_FILEPOS_DOWNLOAD)/
                                Bass.BASS_StreamGetFilePosition(stream, BASSStreamFilePosition.BASS_FILEPOS_END))
        If Bass.BASS_ChannelIsActive(stream) <> BASSActive.BASS_ACTIVE_PLAYING Then
            OtherApi.MyWindow1.SetButtonData(True)
        End If
        If Mouse.LeftButton = MouseButtonState.Released Then
            Slider2.Maximum = Bass.BASS_ChannelGetLength(stream)
            Slider2.Value = Bass.BASS_ChannelGetPosition(stream)

            If Bass.BASS_ChannelIsActive(stream) = BASSActive.BASS_ACTIVE_STOPPED Then
                If Not IsNothing(Playlist1) AndAlso Playlist1.Count > 0 Then
                    If Not CBool(CheckBox4.IsChecked) Then
                        If RandomCheckBox.IsChecked Then
                            PlaylistTecIndex1 = New Random().Next(0, Playlist1.Count)
                        Else
                            If PlaylistTecIndex1 = Playlist1.Count - 1 Then
                                PlaylistTecIndex1 = 0
                            Else
                                PlaylistTecIndex1 += 1
                            End If
                        End If
                        Play(Playlist1(PlaylistTecIndex1), False)
                    Else
                        'Bass.BASS_ChannelSetPosition(stream, 0)
                        Bass.BASS_ChannelPlay(stream, True)
                        TimerPlayer1.Start()
                        If CheckBox2.IsChecked Then
                            Dim a1 = CType(DataContext, types.audio)
                            If Not a1.is_radio Then Await Audio.SetBroadcast(a1.owner_id & "_" & a1.id)
                        End If
                    End If
                    'pubstream = stream
                End If
            ElseIf Bass.BASS_ChannelIsActive(stream) = BASSActive.BASS_ACTIVE_STALLED Then
                Debug.Print("455")
            End If
        End If
        Slider1.SelectionEnd = Math.Abs(Un4seen.Bass.Utils.HighWord(Bass.BASS_ChannelGetLevel(stream))/4)
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        PlayPause()
    End Sub

    Private Async Sub CheckBox1_Click(sender As Object, e As RoutedEventArgs)
        If CType(sender, CheckBox).IsChecked Then
            Dim b = CType(DataContext, types.audio)
            Await audio.Add(b.id.ToString(), b.owner_id.ToString())
            OtherApi.ShowMicroVkNot(b.artist & "-" & b.title, My.Resources.addToYourAudio)
        End If
    End Sub

    Private Async Sub CheckBox_Click(sender As Object, e As RoutedEventArgs)
        Dim a = CType(DataContext, types.audio)
        If CType(sender, CheckBox).IsChecked Then
            Await audio.SetBroadcast(a.owner_id & "_" & a.id)
            OtherApi.ShowMicroVkNot(a.artist & "-" & a.title, My.Resources.setaudioStatus)
        Else
            Await audio.SetBroadcast("")
            OtherApi.ShowMicroVkNot("MicroVK", My.Resources.delAudioStatus)
        End If
    End Sub

    Public Sub Button_Click_2(sender As Object, e As RoutedEventArgs, Optional isStop As Boolean = False)
        PreviousTrack(isStop)
    End Sub

    Public Sub Button_Click_3(sender As Object, e As RoutedEventArgs, Optional isStop As Boolean = False)
        NextTrack(isStop)
    End Sub

    Private Sub Slider2_OnPreviewMouseUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        Bass.BASS_ChannelSetPosition(stream, CLng(Slider2.Value))
    End Sub


    Private Sub WindowPlayer_OnLocationChanged(ByVal sender As Object, ByVal e As EventArgs)
        If My.Settings.MusicPlayerSnap AndAlso _stickyWindow IsNot Nothing AndAlso WindowState <> WindowState.Minimized _
            Then
            Dim MousePoint As Point = Mouse.GetPosition(Me)
            Dim ScreenPoint As Point = Me.PointToScreen(MousePoint)
            if MousePoint.X > 0 AndAlso MousePoint.Y > 0
                Win32.SendMessage(_stickyWindow.Handle,
                                  Win32.WM.WM_NCLBUTTONDOWN,
                                  Win32.HT.HTCAPTION,
                                  Win32.MakeLParam(Convert.ToInt32(ScreenPoint.X), Convert.ToInt32(ScreenPoint.Y)))
                Win32.SendMessage(_stickyWindow.Handle,
                                  Win32.WM.WM_MOUSEMOVE,
                                  Win32.HT.HTCAPTION,
                                  Win32.MakeLParam(Convert.ToInt32(MousePoint.X), Convert.ToInt32(MousePoint.Y)))
            End If
        elseif _stickyWindow is Nothing andalso WindowState <> WindowState.Minimized
            CreateStickyWindow()
        End If
    End Sub

#Region "IDisposable Support"

    Private disposedValue As Boolean ' Для определения избыточных вызовов

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                TokenSource.Dispose()
            End If
            TimerPlayer1 = Nothing
            TokenSource = Nothing
            _stickyWindow = Nothing
        End If
        disposedValue = True
    End Sub

    ' TODO: переопределить Finalize(), только если Dispose(disposing As Boolean) выше имеет код для освобождения неуправляемых ресурсов.
    Protected Overrides Sub Finalize()
        ' Не изменяйте этот код. Разместите код очистки выше в методе Dispose(disposing As Boolean).
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' Этот код добавлен редактором Visual Basic для правильной реализации шаблона высвобождаемого класса.
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)

        GC.SuppressFinalize(Me)
    End Sub

#End Region

    Private Sub WindowPlayer_OnInitialized(sender As Object, e As EventArgs)
        Bass.BASS_Init(- 1, 44100, BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero)
        AddOn.Fx.BassFx.LoadMe()
        AddHandler TimerPlayer1.Tick, AddressOf TimerPlayer1_Tick
    End Sub

    Private _fxEchoHandle As Integer
    Private _fxDistortionHandle As Integer
    Private _fxRotateHandle As Integer
    Private _tempStream As Integer

    Public Sub UpdateFX()
        If SettingSystem.BassFXSettings.IsEcho Then
            If _fxEchoHandle = 0 OrElse _tempStream <> stream Then
                _fxEchoHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_BFX_ECHO4, 0)
                _tempStream = stream
            End If
            Bass.BASS_FXSetParameters(_fxEchoHandle, SettingSystem.BassFXSettings.Echo)
        ElseIf _fxEchoHandle > 0 Then
            Bass.BASS_ChannelRemoveFX(stream, _fxEchoHandle)
            _fxEchoHandle = 0
        End If
        If SettingSystem.BassFXSettings.IsDistortion Then
            If _fxDistortionHandle = 0 OrElse _tempStream <> stream Then
                _fxDistortionHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_BFX_DISTORTION, 0)
                _tempStream = stream
            End If
            Bass.BASS_FXSetParameters(_fxDistortionHandle, SettingSystem.BassFXSettings.Distortion)
        ElseIf _fxDistortionHandle > 0 Then
            Bass.BASS_ChannelRemoveFX(stream, _fxDistortionHandle)
            _fxDistortionHandle = 0
        End If

        If SettingSystem.BassFXSettings.IsDistortion Then
            If _fxRotateHandle = 0 OrElse _tempStream <> stream Then
                _fxRotateHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_BFX_ROTATE, 0)
                _tempStream = stream
            End If
            Bass.BASS_FXSetParameters(_fxRotateHandle, SettingSystem.BassFXSettings.Rotate)
        ElseIf _fxRotateHandle > 0 Then
            Bass.BASS_ChannelRemoveFX(stream, _fxRotateHandle)
            _fxRotateHandle = 0
        End If
    End Sub

    Private Sub OpenCurrentPlaylist_OnClick(sender As Object, e As RoutedEventArgs)
        AudioListPopup.IsOpen = Not AudioListPopup.IsOpen
        AudioListtModernFrame.Source = Nothing
        AudioListtModernFrame.Source = New Uri("/Content/ControlAudio.xaml#page=playlist", UriKind.RelativeOrAbsolute)
    End Sub

    Private Sub WindowPlayer_OnClosing(sender As Object, e As CancelEventArgs)
        e.Cancel = True
    End Sub

    Private Sub WindowPlayer_OnLoaded(sender As Object, e As RoutedEventArgs)
        CreateStickyWindow
    End Sub

    private sub CreateStickyWindow
        If My.Settings.MusicPlayerSnap AndAlso _stickyWindow Is Nothing Then
            _stickyWindow = New StickyWindow(Me) With {.StickToScreen = True,
                .StickToOther = True,
                .StickOnResize = True,
                .StickOnMove = true,
                .StickGap = 5}
        End If
    End sub

    Public Sub NextTrack(Optional ByVal isStop As Boolean = False, Optional showPopup As Boolean = False)
        If isStop Then
            Bass.BASS_ChannelPause(stream)
        End If
        If Not IsNothing(Playlist1) AndAlso Playlist1.Count > 0 Then
            If RandomCheckBox.IsChecked Then
                PlaylistTecIndex1 = New Random().Next(0, Playlist1.Count)
            Else
                If PlaylistTecIndex1 = Playlist1.Count - 1 Then
                    PlaylistTecIndex1 = 0
                Else
                    PlaylistTecIndex1 += 1
                End If
            End If
            Play(Playlist1(PlaylistTecIndex1))
            If showPopup Then
                Dim a = Playlist1(PlaylistTecIndex1)
                OtherApi.ShowMicroVkNot(a.title, a.artist)
            End If
        End If
    End Sub

    Public Sub PreviousTrack(Optional ByVal isStop As Boolean = False, Optional showPopup As Boolean = False)
        If isStop Then
            Bass.BASS_ChannelPause(stream)
        End If
        If Not IsNothing(Playlist1) AndAlso Playlist1.Count > 0 Then
            If RandomCheckBox.IsChecked Then
                PlaylistTecIndex1 = New Random().Next(0, Playlist1.Count)
            Else
                If PlaylistTecIndex1 = 0 Then
                    PlaylistTecIndex1 = Playlist1.Count - 1
                Else
                    PlaylistTecIndex1 -= 1
                End If
            End If
            Play(Playlist1(PlaylistTecIndex1))
            If showPopup Then
                Dim a = Playlist1(PlaylistTecIndex1)
                OtherApi.ShowMicroVkNot(a.title, a.artist)
            End If
        End If
    End Sub

    Friend Shared Async Function PlayRandom() As Task
        If OtherApi.AccessToken = "" Then
            Exit Function
        End If
        If IsNothing(OtherApi.BassPlayer1) Then OtherApi.BassPlayer1 = New WindowPlayer
        Dim a = Await Api.Audio.GetRecommendations("", "", "1")
        If a IsNot Nothing AndAlso a.Count > 0 Then
            OtherApi.BassPlayer1.Play(a.FirstOrDefault)
            OtherApi.BassPlayer1.Playlist1 = a
            OtherApi.BassPlayer1.PlaylistTecIndex1 = 0
            Dim b = a.FirstOrDefault()
            OtherApi.ShowMicroVkNot(b.title, b.artist)
        End If
    End Function

    Public Sub [Stop]()
        Bass.BASS_StreamFree(stream)
        TimerPlayer1.Stop()
        OtherApi.MyWindow1.SetButtonData(True)
    End Sub

    Public Sub PlayPause()
        If Bass.BASS_ChannelIsActive(stream) = BASSActive.BASS_ACTIVE_PAUSED Then
            Bass.BASS_ChannelPlay(stream, False)
            TimerPlayer1.Start()
        Else
            Bass.BASS_ChannelPause(stream)
        End If
    End Sub

    Private Sub MinimizeButton_OnClick(sender As Object, e As RoutedEventArgs)
        if my.Settings.PlayerNotInTaskBar
            me.Hide()
        else
            Me.WindowState = WindowState.Minimized
        End If
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        [Stop]()
        Me.Hide()
    End Sub

    Private Sub WindowPlayer_OnStateChanged(sender As Object, e As EventArgs)
    End Sub
End Class
