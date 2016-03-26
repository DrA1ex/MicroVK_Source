Imports System.ComponentModel
Imports System.Deployment.Application
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Threading
Imports MicroVK.WPFGrowlNotification
Imports FirstFloor.ModernUI.Windows.Controls
Imports Blue.Windows
Imports FirstFloor.ModernUI.Presentation
Imports MahApps.Metro.Controls
Imports MicroVK.Api
Imports MicroVK.OtherLib
Imports Newtonsoft.Json
Imports Un4seen.Bass

Class MainWindow
    Inherits ModernWindow

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        If Not String.IsNullOrEmpty(My.Settings.AccessToken) Then
            CType(sender, Button).ContextMenu.IsOpen = False
            CType(sender, Button).ContextMenu.IsOpen = True
        End If
    End Sub

    Private Sub MenuItem_Click_1(sender As Object, e As RoutedEventArgs)
        My.Application.Shutdown()
    End Sub

    Private Sub MenuItem_Click_2(sender As Object, e As RoutedEventArgs)
        OtherApi.ProcessStart("http://vk.com/microvk")
    End Sub

    Private Sub MenuItem_Click_3(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ContentSource = New Uri("UserControl/ControlAbout.xaml", UriKind.RelativeOrAbsolute)
    End Sub

    Private Sub MenuItem_OnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        WindowState = WindowState.Normal
        Show()
        Activate()
    End Sub

    Private Async Sub MenuItem_Click_4(sender As Object, e As RoutedEventArgs)
        Await SettingSystem.UpdateMicroVkAccounts()
        If SettingSystem.MicroVkAccounts?.Count > 0 Then
            ModernFrame1.Source = Nothing
            ShowFlyout(Position.Left, "Content/Flyouts/AccountManager.xaml", My.Resources.AccountManager)
        Else
            My.Settings.AccessToken = ""
            Forms.Application.Restart()
            My.Application.Shutdown()
        End If
    End Sub

    Private Sub MenuItem_Checked(sender As Object, e As RoutedEventArgs)
        OtherApi.Timer1.Stop()
        TrayIconManager.SetOnlineIcon(False)
        If My.Settings.AccessToken <> "" Then
            Account.SetOffline()
        Else
            Exit Sub
        End If
        OtherApi.GrowlNotifiactions1.AddNotification(New Notification With {
                                                        .Content =
                                                        New ControlNotificationMicroVk With {
                                                        .DataContext = New types.Notifycation With {.title = "MicroVK",
                                                        .text = My.Resources.Invisible_enabled}}})
    End Sub

    Private Sub MenuItem_Unchecked(sender As Object, e As RoutedEventArgs)
        If OtherApi.AccessToken = "" Then
            Exit Sub
        End If
        OtherApi.Timer1.Start()
        Account.SetOnline()
        TrayIconManager.SetOnlineIcon(True)
        OtherApi.GrowlNotifiactions1.AddNotification(New Notification With {
                                                        .Content =
                                                        New ControlNotificationMicroVk With {
                                                        .DataContext = New types.Notifycation With {.title = "MicroVK",
                                                        .text = My.Resources.Invisible_disabled}}})
    End Sub

    Private Sub MenuItem_Click_5(sender As Object, e As RoutedEventArgs)
        Forms.Application.Restart()
        My.Application.Shutdown()
    End Sub

    Private Sub MenuItem_Click(sender As Object, e As RoutedEventArgs)
        'NavigationService.GetNavigationService(Me).Navigate(New Uri("/Pages/PageSetting.xaml", UriKind.RelativeOrAbsolute))
        OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageSetting.xaml", UriKind.RelativeOrAbsolute)
    End Sub

    Public Sub LinkGo1(a As Uri)
        Link1.Source = a
        OtherApi.MyWindow1.ContentSource = Link1.Source
    End Sub

    Public Sub LinkGo2(a As Uri)
        Link2.Source = a
        OtherApi.MyWindow1.ContentSource = Link2.Source
    End Sub

    Private Sub W1_Closed(sender As Object, e As EventArgs)
        Notify1.Visibility = Visibility.Hidden
        My.Settings.windowsSize = New Size(CInt(Width), CInt(Height))
        My.Application.Shutdown()
    End Sub

    Public Sub UpdateLogo()
        Image1.GetBindingExpression(Windows.Controls.Image.SourceProperty).UpdateTarget()
    End Sub

    Private Sub Button1_Click(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ContentSource = New Uri("Content/ControlNotificationCentr.xaml", UriKind.RelativeOrAbsolute)
    End Sub

    Private Sub W1_Activated(sender As Object, e As EventArgs)
        TaskbarItemInfo.ProgressValue = 0
        TrayIconManager.SetUnreadTaskBarEvent(0)
    End Sub

    Private Sub MenuItem_Click_6(sender As Object, e As RoutedEventArgs)
    End Sub

    Private Sub MenuItem4_OnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Notify1.Visibility = Visibility.Hidden
        My.Application.Shutdown()
    End Sub


    Private Sub MainWindow_OnClosing(ByVal sender As Object, ByVal e As CancelEventArgs)
        If Not My.Settings.IsClosed AndAlso Not String.IsNullOrEmpty(My.Settings.AccessToken) Then
            e.Cancel = True
            If Not My.Settings.InTaskbar Then
                Task.Factory.StartNew(Async Function()
                    Await NetHelper.Delay(100)
                    If CheckAccess() Then
                        Hide()
                    Else
                        Dispatcher.Invoke(Sub() Hide())
                                         End If
                                         End Function)
            Else
                WindowState = WindowState.Minimized
            end if

        Else
            My.Settings.Save()
        End If
    End Sub

    Private Async Sub ButtonBase_OnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Not IsNothing(OtherApi.BassPlayer1) Then
            If Bass.BASS_ChannelIsActive(OtherApi.BassPlayer1.stream) = BASSActive.BASS_ACTIVE_PLAYING Then
                Bass.BASS_ChannelPause(OtherApi.BassPlayer1.stream)
            ElseIf Bass.BASS_ChannelIsActive(OtherApi.BassPlayer1.stream) = BASSActive.BASS_ACTIVE_PAUSED Then
                Bass.BASS_ChannelPlay(OtherApi.BassPlayer1.stream, False)
                OtherApi.BassPlayer1.TimerPlayer1.Start()
            Else
                Await WindowPlayer.PlayRandom()
            End If
        Else
            Await WindowPlayer.PlayRandom()
        End If
    End Sub

    Public Sub SetButtonData(state As Boolean)
        If (state) Then
            PlayerButtonPath1.Data = TryCast(PlayerButtonPath1.FindResource("playStyle0"), Geometry)
            PlayThumbButtonInfo.ImageSource = New BitmapImage(New Uri("images/play.png", UriKind.RelativeOrAbsolute))
        Else
            PlayerButtonPath1.Data = TryCast(PlayerButtonPath1.FindResource("pauseStyle0"), Geometry)
            PlayThumbButtonInfo.ImageSource = New BitmapImage(New Uri("images/pause.png", UriKind.RelativeOrAbsolute))
        End If
    End Sub

    Public Sub SetPlayerProgressRingVisible(a As Boolean)
        If a Then
            PlayerModernProgressRing.Visibility = Visibility.Visible
            PlayerButton.Visibility = Visibility.Hidden
        Else
            PlayerModernProgressRing.Visibility = Visibility.Hidden
            PlayerButton.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub ThumbButtonInfo_OnClick(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsNothing(OtherApi.BassPlayer1) Then
            OtherApi.BassPlayer1.Button_Click_2(sender, Nothing, True)
        Else
            ButtonBase_OnClick(sender, Nothing)
        End If
    End Sub

    Private Sub ThumbButtonInfo2_OnClick(ByVal sender As Object, ByVal e As EventArgs)
        ButtonBase_OnClick(sender, Nothing)
    End Sub

    Private Sub ThumbButtonInfo3_OnClick(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsNothing(OtherApi.BassPlayer1) Then
            OtherApi.BassPlayer1.Button_Click_3(sender, Nothing, True)
        Else
            ButtonBase_OnClick(sender, Nothing)
        End If
    End Sub

    Private Sub ThemeButton_OnClick(sender As Object, e As RoutedEventArgs)
        If AppearanceManager.Current.ThemeSource.ToString Like "*Dark.xaml*" Then
            AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml", UriKind.RelativeOrAbsolute)
            My.Settings.Theme = 0
        Else
            AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImageDark.xaml", UriKind.RelativeOrAbsolute)
            My.Settings.Theme = 1
        End If
        OtherApi.UpdateTheme()
    End Sub

    Private Sub ButtonBase_OnClick1(sender As Object, e As RoutedEventArgs)
        OtherApi.ProcessStart("setup.exe")
        My.Application.Shutdown()
    End Sub


    Private Sub SearchButton_OnClick(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrEmpty(My.Settings.AccessToken) Then Exit Sub
        If Flyout1.IsOpen AndAlso ModernFrame1.Source.ToString Like "*ContentSearch.*" Then
            Flyout1.IsOpen = False
            Exit Sub
        End If
        ShowFlyout(Position.Left, "Content/ContentSearch.xaml", "")
    End Sub

    Private _activeFlyout As String
    Public FlyoutUrl As String
    Private _flyoutStretch As Boolean

    Public Sub ShowFlyout(pos As Position,
                          uri As String,
                          titleFlyout As String,
                          Optional stretch As Boolean = False,
                          Optional customWidth As Integer = 0)
        _flyoutStretch = stretch
        If _activeFlyout <> uri Then
            Flyout1.IsOpen = False
        End If
        FlyoutUrl = uri
        _activeFlyout = uri
        Flyout1.Position = pos
        If stretch Then
            Flyout1.Width = ActualWidth - 32
        Else
            Flyout1.Width = If(customWidth > 0, customWidth, 300)
        End If
        Flyout1.Header = titleFlyout
        ModernFrame1.Source = New Uri(uri, UriKind.RelativeOrAbsolute)
        If OtherApi.MyWindow1.WindowState = WindowState.Minimized Then _
            OtherApi.MyWindow1.WindowState = WindowState.Normal
        OtherApi.MyWindow1.Show()
        OtherApi.MyWindow1.Activate()
        Flyout1.IsOpen = True
    End Sub

    Private Sub Flyout1_OnIsOpenChanged(sender As Object, e As RoutedEventArgs)

        If Flyout1.IsOpen AndAlso Not _activeFlyout.Contains("DialogStyleSettings") Then
            RootBorder.Visibility = Visibility.Visible
        Else
            RootBorder.Visibility = Visibility.Collapsed
            FlyoutUrl = ""

        End If
    End Sub

    Private Sub RadioUpdate()
        'Dim f = File.ReadAllText("radio/temp.json")

        'Dim j = JObject.Parse(f)("response").ToString()
        'Dim audiolist = JsonConvert.DeserializeObject(Of types.radio_debug())(j)
        'Dim s = JsonConvert.SerializeObject(audiolist)
        Dim f = File.ReadAllText("radio/stations.json")
        Dim audiolist = JsonConvert.DeserializeObject (Of types.radio_debug())(f)
        Dim audiolist1 = audiolist.OrderBy(Function(debug) debug.rating).Reverse().ToList()
' ReSharper disable once UnusedVariable
        Dim s = JsonConvert.SerializeObject(audiolist1)
    End Sub

    Private Sub DebugButton_Click(sender As Object, e As RoutedEventArgs)


        Exit Sub
' ReSharper disable VbUnreachableCode
        TrayIconManager.ChangeNetworkStatus(CBool(CInt(Rnd()*2)))
        Exit Sub
        Dim a1 = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                .text = My.Resources.notifycation_color_prewiew,
                .type = 4,
                .tag = "user_id=" & OtherApi.MyUser.id}
        OtherApi.ShowNotify({a1}.ToList())
        Exit Sub
        ShowFlyout(Position.Left, "2048/Game2048.xaml", My.Resources.Game)
        Exit Sub
#If DEBUG Then
        RadioUpdate()
#End If
        Flyout1.IsOpen = True
        ModernFrame1.Source = New Uri("Content/ContentHistoryAndGroupJoin.xaml#1", UriKind.RelativeOrAbsolute)
        Exit Sub
        SettingSystem.BassFXSettings.Rotate.fRate = - 5000
        Dim fxRotateHandle = Bass.BASS_ChannelSetFX(OtherApi.BassPlayer1.stream, BASSFXType.BASS_FX_BFX_ROTATE, 0)

        Bass.BASS_FXSetParameters(fxRotateHandle, SettingSystem.BassFXSettings.Rotate)
        Exit Sub
        UpdateSmiles()
        Flyout1.IsOpen = True


        Dim g = 8
        Dim a As types.Notifycation
        Select Case g
            Case 3
                a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                    .type = 3,
                    .text = My.Resources.notifycation_color_prewiew}
            Case 4
                a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                    .text = My.Resources.notifycation_color_prewiew,
                    .type = 4,
                    .tag = "user_id=" & OtherApi.MyUser.id}
            Case 40
                a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                    .text = My.Resources.notifycation_color_prewiew,
                    .type = 40,
                    .tag = "user_id=" & OtherApi.MyUser.id}
            Case 8
                a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                    .type = 8,
                    .text = "(" & Messages.DeviceNames(6) & ")"}
            Case 9
                a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                    .type = 9,
                    .text = "(" & Messages.DeviceNames(6) & ")"}
            Case 61
                a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                    .type = 61}
            Case 62
                a = New types.Notifycation With {.user_id = OtherApi.MyUser.id,
                    .type = 62}
            Case Else
                a = Nothing
        End Select
        OtherApi.ShowNotify({a}.ToList)
' ReSharper restore VbUnreachableCode
        'TrayIconManager.SetUnreadEvent(4545)
        'TrayIconManager.SetUnreadTaskBarEvent(2000)
    End Sub

    Private Sub MainWindow_OnInitialized(sender As Object, e As EventArgs)
        AddHandler _resizeTimer.Tick, AddressOf _resizeTimer_Tick
        If My.Settings.isWindowsSize Then
            Width = My.Settings.windowsSize.Width
            Height = My.Settings.windowsSize.Height
        End If
        TaskbarItemInfo.ProgressValue = 100
        OtherApi.MyWindow1 = Me
        If File.Exists("setup.exe") Then
            updatebutton.Visibility = Visibility.Visible
        End If

        '#If DEBUG
        '        Flyout1.IsOpen = true
        '        ModernFrame1.Source = new Uri("Content/ControlHistory.xaml#1", UriKind.RelativeOrAbsolute)
        '#End If
#If DEBUG Then
        'DebugButton.Visibility = Visibility.Visible
#End If
        Application.Current.MainWindow = Me
        TrayIconManager.TaskbarIcon1 = Notify1
        If My.Settings.AccessToken = "" Then ContentSource = New Uri("Pages/StartPage.xaml", UriKind.RelativeOrAbsolute)
        StickyWindow.RegisterExternalReferenceForm(Me)
        If ApplicationDeployment.IsNetworkDeployed Then
            If My.Settings.Version <> ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() Then
                If My.Settings.AccessToken <> "" Then
                    ShowFlyout(Position.Right, "Content/ContentHistoryAndGroupJoin.xaml#1", "")
                End If
                OtherApi.GrowlNotifiactions1.AddNotification(New Notification With {
                                                                .Content =
                                                                New ControlNotificationNewVersion With {
                                                                .DataContext =
                                                                New types.Notifycation With {
                                                                .title =
                                                                "MicroVK " &
                                                                ApplicationDeployment.CurrentDeployment.CurrentVersion.
                                                                    ToString(),
                                                                .text = My.Resources.successfully_updated}}})
                My.Settings.Version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
            End If
            AboutGroup1.DisplayName = "MicroVK v" &
                                      ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString().ToString().
                                          ToString().ToString().ToString()
        Else
            AboutGroup1.DisplayName = "MicroVK"
        End If
        StickyWindow.RegisterExternalReferenceForm(Me)
        HotKeyManager.InitializeHotkeys()

        If My.Settings.AccessToken <> "" AndAlso Not My.Settings.Invisible Then
            If OtherApi.AccessToken = "" Then
                Exit Sub
            End If
            OtherApi.Timer1.Start()
            TrayIconManager.SetOnlineIcon(True)
        End If
        If My.Settings.runMinimized Then
            WindowState = WindowState.Minimized
        End If
    End Sub


    Private Sub _resizeTimer_Tick(sender As Object, e As EventArgs)
        _resizeTimer.IsEnabled = False
        If BorderContent.Visibility = Visibility.Collapsed Then
            BorderContent.Visibility = Visibility.Visible
            ImageContent.Visibility = Visibility.Collapsed
            ImageContent.Source = Nothing
        End If
    End Sub

    Sub UpdateSmiles()
' ReSharper disable UnusedVariable
        Dim a = "1F60A"
        Dim b = "D83DDE0A"
        ' ReSharper restore UnusedVariable
        Dim smile1() As String = CType(FindResource("Smile1"), String())

        Dim path1 = "D:\Project\MicroVK\MicroVK.Smiles\Smile"
        Dim files1() = Directory.GetFiles(path1)
        Dim sb = New StringBuilder
        For Each s In files1
            Dim n = s.Split("\"c).LastOrDefault.Split("."c).FirstOrDefault.ToUpper()
            If Array.IndexOf(smile1, n) < 0 Then
                sb.AppendFormat("<sys:String>{0}</sys:String>", n)
                sb.AppendLine()
            End If
        Next
' ReSharper disable once UnusedVariable
        Dim res = sb.ToString()
    End Sub

    Function GetTwitterSmileName(vkname As String) As String
        If vkname.Length Mod 2 <> 0 Then
            vkname = "0" + vkname
        End If

        Dim bytes = OtherApi.FromHex(vkname)
        Dim g = Encoding.GetEncoding(12001).GetString(bytes)

        Dim a = BitConverter.ToString(Encoding.GetEncoding(1201).GetBytes(g)).Replace("-", "").TrimStart({"0"c})
        Return a
    End Function

    Private Sub ShareButton_OnClick(sender As Object, e As RoutedEventArgs)


        Dim t = New RenderTargetBitmap(CInt(OtherApi.MyWindow1.ActualWidth),
                                       CInt(OtherApi.MyWindow1.ActualHeight),
                                       96,
                                       96,
                                       PixelFormats.Default)
        t.Render(OtherApi.MyWindow1)
        Dim file = Path.GetTempFileName() & ".jpg"

        Dim jpeg = New JpegBitmapEncoder()
        jpeg.Frames.Add(BitmapFrame.Create(t))
        Using filestream = New FileStream(file, FileMode.Create)
            jpeg.Save(filestream)
        End Using
        ControllShare.ScreenshotPath = file
        ControllShare.CustomText = ""
        ModernFrame1.Source = Nothing
        ShowFlyout(Position.Left, "Content/ControllShare.xaml", My.Resources.Share)
    End Sub

    Private ReadOnly _
        _resizeTimer As DispatcherTimer = New DispatcherTimer _
        With {.Interval = New TimeSpan(0, 0, 0, 0, 300), .IsEnabled = False}

    Private Sub MainWindow_OnSizeChanged(sender As Object, e As SizeChangedEventArgs)

        _resizeTimer.Stop()
        _resizeTimer.Start()
        If BorderContent.Visibility = Visibility.Visible AndAlso OtherApi.MyUser IsNot Nothing Then
            Dim rtb = New RenderTargetBitmap(CInt(ActualWidth), CInt(ActualHeight), 96, 96, PixelFormats.Pbgra32)
            rtb.Render(OtherApi.MyWindow1)
            ImageContent.Source = rtb.Clone()
            ImageContent.Visibility = Visibility.Visible
            BorderContent.Visibility = Visibility.Collapsed
        End If

        If Flyout1.IsOpen AndAlso _flyoutStretch Then
            Flyout1.Width = ActualWidth - 32
        End If
    End Sub

    Private Sub GameButton_OnClick(sender As Object, e As RoutedEventArgs)
        ShowFlyout(Position.Right, "2048/Game2048.xaml", My.Resources.Game, True)
    End Sub

    Private Sub W1_StateChanged(sender As Object, e As EventArgs)
        If WindowState = WindowState.Minimized Then
            If Not My.Settings.InTaskbar Then
                Hide()
            End If
        ElseIf WindowState = WindowState.Maximized Then
            WindowStyle = WindowStyle.ThreeDBorderWindow
        Else
            WindowStyle = WindowStyle.SingleBorderWindow
            Show()
        End If
    End Sub

    Private Sub BellButton_OnClick(sender As Object, e As RoutedEventArgs)
        BellButton.ContextMenu.IsOpen = Not BellButton.ContextMenu.IsOpen
    End Sub

    Private Sub BellButtonContextMenu_OnInitialized(sender As Object, e As EventArgs)
        BellSleep5MMenuItem.Header = String.Format(My.Resources.bell_sleep_minute, 5)
        BellSleep15MMenuItem.Header = String.Format(My.Resources.bell_sleep_minute, 15)
        BellSleep30MMenuItem.Header = String.Format(My.Resources.bell_sleep_minute, 30)
        BellSleep1HMenuItem.Header = String.Format(My.Resources.bell_sleep_hour, 1)
        BellSleep2HMenuItem.Header = String.Format(My.Resources.bell_sleep_hour, 2)
        BellSleep5HMenuItem.Header = String.Format(My.Resources.bell_sleep_hour, 5)
        BellSleep8HMenuItem.Header = String.Format(My.Resources.bell_sleep_hour, 8)
    End Sub

    Private Sub BellMenuItem_OnClick(sender As Object, e As RoutedEventArgs)


        Dim mi = TryCast(sender, MenuItem)
        Dim minute = mi.Tag?.ToString().GetParametr("minute")
        Dim hour = mi.Tag?.ToString().GetParametr("hour")
        Dim isChecked = mi.IsChecked
        If mi.IsChecked Then

            If Not String.IsNullOrEmpty(minute) Then
                OtherApi.bellSleepTime = Now.AddMinutes(CType(minute, Double))
                BeelChangeStatus(2)
            ElseIf Not String.IsNullOrEmpty(hour) Then
                OtherApi.bellSleepTime = Now.AddHours(CType(hour, Double))
                BeelChangeStatus(2)
            Else
                BeelChangeStatus(1)
            End If
        Else
            BeelChangeStatus(0)
        End If
        BellOffMenuItem.IsChecked = False
        BellSleep5MMenuItem.IsChecked = False
        BellSleep15MMenuItem.IsChecked = False
        BellSleep30MMenuItem.IsChecked = False
        BellSleep1HMenuItem.IsChecked = False
        BellSleep2HMenuItem.IsChecked = False
        BellSleep5HMenuItem.IsChecked = False
        BellSleep8HMenuItem.IsChecked = False

        mi.IsChecked = isChecked
    End Sub

    Public Sub BeelChangeStatus(statusId As Integer)
        Select Case statusId
            Case 0
                BellPath.Data = TryCast(BellPath.FindResource("bell"), Geometry)
                BellOffMenuItem.IsChecked = False
                BellSleep5MMenuItem.IsChecked = False
                BellSleep15MMenuItem.IsChecked = False
                BellSleep30MMenuItem.IsChecked = False
                BellSleep1HMenuItem.IsChecked = False
                BellSleep2HMenuItem.IsChecked = False
                BellSleep5HMenuItem.IsChecked = False
                BellSleep8HMenuItem.IsChecked = False
                OtherApi.IsBellOff = False
                OtherApi.IsBellSleep = False
            Case 1
                BellPath.Data = TryCast(BellPath.FindResource("bell_off"), Geometry)
                OtherApi.IsBellOff = True
                OtherApi.GrowlNotifiactions1.Notifications.Clear()
            Case 2
                OtherApi.GrowlNotifiactions1.Notifications.Clear()
                BellPath.Data = TryCast(BellPath.FindResource("bell_sleep"), Geometry)
                OtherApi.IsBellSleep = True
        End Select
    End Sub

    Private Sub ShowNotifySettings_OnClick(sender As Object, e As RoutedEventArgs)
        ShowFlyout(Position.Left, "Content/ContentSettingNotification.xaml", My.Resources.Notification, False, 500)
    End Sub

    Private Sub HelpMicroVKMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        Dim t = New RenderTargetBitmap(CInt(OtherApi.MyWindow1.ActualWidth),
                                       CInt(OtherApi.MyWindow1.ActualHeight),
                                       96,
                                       96,
                                       PixelFormats.Default)
        t.Render(OtherApi.MyWindow1)
        Dim file = Path.GetTempFileName() & ".jpg"

        Dim jpeg = New JpegBitmapEncoder()
        jpeg.Frames.Add(BitmapFrame.Create(t))
        Using filestream = New FileStream(file, FileMode.Create)
            jpeg.Save(filestream)
        End Using
        ControllShare.ScreenshotPath = file
        ControllShare.CustomText = ""
        ModernFrame1.Source = Nothing
        OtherApi.MyWindow1.ShowFlyout(Position.Left, "Content/Flyouts/HelpProgram.xaml", My.Resources.HelpMicroVK)
    End Sub
End Class

