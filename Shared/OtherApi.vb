Imports System.Windows.Threading
Imports System.Collections.ObjectModel
Imports System.Configuration
Imports System.IO
Imports System.Net
Imports FirstFloor.ModernUI.Windows.Controls
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports MicroVK.WPFGrowlNotification
Imports System.Security.Cryptography
Imports System.Windows.Controls.Primitives
Imports MahApps.Metro.Controls
Imports MicroVK.Api
Imports MicroVK.OtherLib

Class OtherApi
    Public Shared AccessToken As String = ""
    Public Shared DialogListBox1 As ListBox
    Public Shared WebBrowser1 As Forms.WebBrowser
    Public Shared MyWindow1 As MainWindow

    Public Shared _
        Timer1 As New DispatcherTimer With {.Interval = New TimeSpan(0, 0, 0, If(My.Settings.OnlineWait, 30, 10))}

    Public Shared TopOffset As Double = 5
    Public Shared BellSleepTime As Date
    Public Shared IsBellSleep As Boolean
    Public Shared IsBellOff As Boolean
    Public Shared LeftOffset As Double = 305
    Public Shared Tolerance As Double = 0.0001

    Public Shared GrowlNotifiactions1 As New GrowlNotifiactions With {
        .Left =
        If _
        (CBool(My.Settings.NotificationH),
         SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - LeftOffset,
         SystemParameters.WorkArea.Left + TopOffset)}

    Public Shared MyUser As types.profile
    Public Shared SoundPlayer1 As New MediaPlayer
    Public Shared Publiclistbox1 As ListBox
    Public Shared IsCaptchaShow As Boolean = False
    Public Shared IsAuthShow As Boolean = False
    Public Shared BassPlayer1 As WindowPlayer
    Public Shared VideoPlayer1 As VideoPlayer
    Public Shared Game20481 As Game2048
    Public Shared Md As ModernDialog

    Public Shared _
        Regex1 As _
            New RegularExpressions.Regex(
                "(\uD83D[\uDEC1-\uDEC5]|\uD83C[\uDFE7-\uDFF0]|[\u2795-\u3299]|[\u203C-\u23EC]|\uE50A|[0-9][\u20E3]|[\u23F0-\u2764]|\uD83C[\uDC04-\uDFE6]|\uD83D[\uDC00-\uDEC0]|®|©)|((http)\S+)")

    Public Shared DialogsTabControl As TabControl
    Public Shared SmilePopup1 As Popup

    Public Shared _
        AttachmentDock As Dock()() =
            {({Dock.Left}), ({Dock.Left, Dock.Right}), ({Dock.Left, Dock.Top, Dock.Bottom}),
             ({Dock.Left, Dock.Top, Dock.Top, Dock.Top}), ({Dock.Left, Dock.Top, Dock.Top, Dock.Top, Dock.Top}),
             ({Dock.Top, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left}),
             ({Dock.Top, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left}),
             ({Dock.Top, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left}),
             ({Dock.Top, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left}),
             ({Dock.Top, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left, Dock.Left,
               Dock.Left})}

    Public Shared _
        AttachmentWidthAndHeight As Integer()() =
            {({400, 300}), ({200, 300, 200, 300}), ({200, 300, 200, 150, 200, 150}),
             ({200, 300, 200, 100, 200, 100, 200, 100}), ({300, 300, 100, 74, 100, 74, 100, 74, 100, 74}),
             ({400, 200, 79, 99, 79, 99, 79, 99, 79, 99, 79, 99}),
             ({400, 233, 65, 65, 65, 65, 65, 65, 65, 65, 65, 65, 65, 65}),
             ({400, 242, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56, 56}),
             ({400, 250, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48}),
             ({400, 255, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43, 43})}

    Public Shared IsNetworkDeployed As Boolean
    Public Shared AboutDialog1 As ModernDialog
    Public Shared NotificationList1 As New ObservableCollection(Of types.Notifycation)
    Public Shared FavoriteNotificationList1 As New ObservableCollection(Of types.Notifycation)
    Public Shared FavoriteUser As New List(Of Integer)
    Public Shared PageMessageRun1 As Run
    Public Shared PageMessageRun2 As Run
    Public Shared PhotoSizes() As Integer = {0, 75, 130, 604, 807, 1280, 2560}
    Public Shared PageMessageRun3 As Run
    Public Shared Lang As String
    Public Shared ApiVersion As String = "5.37"


    Public Shared Async Function VkPost(method As String,
                                        Optional parameters As String = "",
                                        Optional getRaw As Boolean = False,
                                        Optional customToken As String = "") As Task(Of JToken)

#If DEBUG Then
        If CInt(Rnd()*2) = 1 And method Like "*send*" AndAlso False Then
            Return Nothing
        End If
#End If
        Dim temp1 As String
        Label1:
        Using webClient1 As New WebClient
            webClient1.Encoding = Encoding.UTF8
            webClient1.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded")
            If AccessToken.Length = 0 Then
                AccessToken = My.Settings.AccessToken
            End If
            Debug.Print(method & "_" & parameters)
            Dim urls As String = "https://api.vk.com/method/" & method
            Try

                temp1 =
                    Await _
                        webClient1.UploadStringTaskAsync(urls,
                                                         String.Format("{0}&lang={1}&v={2}&access_token={3}",
                                                                       parameters,
                                                                       Lang,
                                                                       ApiVersion,
                                                                       If(customToken = "", AccessToken, customToken)))
            Catch ex As Exception
                Return Nothing
            End Try
        End Using
#If Not XP Then
        If method.Contains("messages.send") Then
#If DEBUG Then
            Yandex.Metrica.YandexMetrica.ReportEvent(method)
#Else
            If Not IsNothing(MyUser) Then
                If MyUser.id <> 64974451 Then
                    Yandex.Metrica.YandexMetrica.ReportEvent(method)
                End If
            Else
                Yandex.Metrica.YandexMetrica.ReportEvent(method)
            End If

#End If
        End If
#End If

        Dim a = JObject.Parse(temp1)
        If (getRaw) Then
            Return a
        End If
        If Not IsNothing(a("error")) Then
            Dim b = Await MyJsonConvert.DeserializeObjectAsync (Of types.VKError)(a("error").ToString)
            Select Case b.error_code
                Case 1
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError1 & " (" & method & ")")
                Case 2
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError2 & " (" & method & ")")
                Case 3
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError3 & " (" & method & ")")
                Case 4
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError4 & " (" & method & ")")
                Case 5
                    Forms.Application.Restart()
                    My.Settings.AccessToken = ""
                    My.Settings.Save()
                    My.Application.Shutdown()
                Case 6
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError6 & " (" & method & ")")
                Case 7
                    If (method = "messages.send") Then
                        ModernDialog.ShowMessage(My.Resources.ApiError_messageSend,
                                                 "MicroVK - VKAPI",
                                                 MessageBoxButton.OK)
                    Else
                        If _
                            ModernDialog.ShowMessage(
                                My.Resources.ApiError7 & " (" & method & ")" & vbNewLine & My.Resources.ApiError7_1,
                                "MicroVK - VKAPI",
                                MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                            Forms.Application.Restart()
                            My.Settings.AccessToken = ""
                            My.Settings.Save()
                            My.Application.Shutdown()
                        End If
                    End If
                Case 8
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError8 & " (" & method & ")")
                Case 9
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError9 & " (" & method & ")")
                Case 10
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError10 & " (" & method & ")")
                Case 11
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError11 & " (" & method & ")")
                Case 14
                    If Not IsCaptchaShow Then
                        IsCaptchaShow = True
                        Dim stackPanel1 As New StackPanel
                        Dim textCaptcha1 As New TextBox
                        stackPanel1.Children.Add(New Image With {
                                                    .Source =
                                                    New BitmapImage(New Uri(a("error")("captcha_img").ToString)),
                                                    .Stretch = Stretch.Uniform,
                                                    .Width = 130,
                                                    .Height = 50,
                                                    .HorizontalAlignment = HorizontalAlignment.Left})
                        stackPanel1.Children.Add(textCaptcha1)
                        Call New ModernDialog() With {.Title = My.Resources.ApiError14,
                            .Content = stackPanel1}.ShowDialog()
                        IsCaptchaShow = False
                        parameters = parameters & "&captcha_sid=" & a("error")("captcha_sid").ToString & "&captcha_key=" &
                                     textCaptcha1.Text
                        GoTo Label1
                    End If
                Case 15
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError15 & " (" & method & ")")
                Case 16
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError16 & " (" & method & ")")
                Case 17
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError17 & " (" & method & ")")
                Case 20
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError20 & " (" & method & ")")
                Case 21
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError21 & " (" & method & ")")
                Case 23
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError23 & " (" & method & ")")
                Case 100
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError100 & " (" & method & ")")
                Case 101
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError101 & " (" & method & ")")
                Case 113
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError113 & " (" & method & ")")
                Case 150
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError150 & " (" & method & ")")
                Case 200
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError200 & " (" & method & ")")
                Case 201
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError201 & " (" & method & ")")
                Case 203
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError203 & " (" & method & ")")
                Case 300
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError300 & " (" & method & ")")
                Case 500
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError500 & " (" & method & ")")
                Case 600
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError600 & " (" & method & ")")
                Case 603
                    ShowMicroVkNot("MicroVK - VKAPI", My.Resources.ApiError603 & " (" & method & ")")
                Case Else
                    MsgMui(
                        "error_code: " & b.error_code & vbNewLine & "error_msg: " & b.error_msg & vbNewLine &
                        "request_params: " & Newtonsoft.Json.JsonConvert.SerializeObject(b.request_params))
            End Select
            Return Nothing
        Else
            Return a("response")
        End If
    End Function

    Public Shared Sub MsgMui(text As String)
        ModernDialog.ShowMessage(text, "MicroVK", MessageBoxButton.OK)
    End Sub

    Public Shared Function GetScrollViewer(o As DependencyObject) As DependencyObject
        If TypeOf o Is ScrollViewer Then
            Return o
        End If

        For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(o) - 1
            Dim child = VisualTreeHelper.GetChild(o, i)
            Dim result = GetScrollViewer(child)
            If result Is Nothing Then
                Continue For
            Else
                Return result
            End If
        Next
        Return Nothing
    End Function

    Public Shared Sub ShowNotify(a As List(Of types.Notifycation), Optional isIgnoreSetting As Boolean = False)
        Dim d = Now
        If IsBellOff Then
            Exit Sub
        ElseIf IsBellSleep Then
            If bellSleepTime > Now Then
                Exit Sub
            Else
                MyWindow1.BeelChangeStatus(0)
            End If
        End If

        For Each i In a
            i.Index = GrowlNotifiactions1.IndexCounter
            GrowlNotifiactions1.IndexCounter += 1
            If GrowlNotifiactions1.IndexCounter = Integer.MaxValue Then
                GrowlNotifiactions1.IndexCounter = 0
            End If
            i.date = d.ToString("HH:mm:ss  dd.MM.yy")
            AddNotification(i)
            Dim isShow As Boolean = CBool(My.Settings.PropertyValues("Notification" & i.type).PropertyValue) OrElse
                                    isIgnoreSetting
            Dim content As UserControl = Nothing
            If isShow Then
                Select Case i.type
                    Case 3
                        content = New ControlNotificationMessageDeleteFlags
                    Case 4, 40
                        SoundPlayer1.Stop()
                        SoundPlayer1.Play()
                        Dim tempBool = True
                        For Each w In My.Application.Windows.OfType (Of Window)
                            If w IsNot Nothing AndAlso w.IsActive Then
                                tempBool = False
                                Exit For
                            End If
                        Next

                        If tempBool Then
                            MyWindow1.TaskbarItemInfo.ProgressValue = 100
                            TrayIconManager.SetUnreadTaskBarEvent(TrayIconManager.UnreadLocalEvent + 1)
                        End If
                        content = New ControlNotificationMessage
                    Case 41
                        content = New ControlMessageSendSuccessfully
                    Case 8
                        content = New ControlNotificationFriendsOnline
                    Case 9
                        content = New ControlNotificationFriendsOnline
                    Case 61
                        For Each j In GrowlNotifiactions1.Notifications
                            If TypeOf j.Content Is ControlNotificationFriendsOnline Then
                                Dim cfo = CType(CType(j.Content, ControlNotificationFriendsOnline).DataContext,
                                                types.Notifycation)
                                If cfo.user_id = i.user_id And cfo.type = 61 Then
                                    GrowlNotifiactions1.Notifications.Remove(j)
                                    Exit For
                                End If
                            End If
                        Next
                        content = New ControlNotificationFriendsOnline
                    Case 62
                        For Each j In GrowlNotifiactions1.Notifications
                            If TypeOf j.Content Is ControlNotificationFriendsOnline Then
                                Dim cfo = CType(CType(j.Content, ControlNotificationFriendsOnline).DataContext,
                                                types.Notifycation)
                                If cfo.user_id = i.user_id Then
                                    GrowlNotifiactions1.Notifications.Remove(j)
                                    Exit For
                                End If
                            End If
                        Next
                        content = New ControlNotificationFriendsOnline
                End Select

                If content IsNot Nothing AndAlso isShow Then
                    content.DataContext = i
                    GrowlNotifiactions1.AddNotification(New Notification With {
                                                           .Content = content,
                                                           .Type = i.type})
                End If
            End If
        Next
    End Sub

    Public Shared Sub ShowMicroVkNot(title As String, text As String)
        GrowlNotifiactions1.RemoveByType(0)
        GrowlNotifiactions1.AddNotification(New Notification With {
                                               .Content =
                                               New ControlNotificationMicroVk _
                                               With {.DataContext = New types.Notifycation With {.title = title,
                                               .text = text}}})
    End Sub

    Public Shared Async Function GoToUrl(url As String) As Task
        If My.Settings.CheckLink Then
            Dim b = Await Utils.CheckLink(Trim(url))
            If Not IsNothing(b) Then
                If b.status = "banned" Then
                    ShowMicroVkNot(My.Resources.BadUrl, String.Format(My.Resources.BlockUrl, b.link))
                Else
                    If url <> b.link Then
                        ProcessStart(url)
                    Else
                        ProcessStart(b.link)
                    End If
                End If
            End If
        Else
            ProcessStart(Trim(url))
        End If
    End Function

    Shared Sub ListBox1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim a = CType(sender, ListBox)
        If IsNothing(a.Tag) Then a.Tag = ""
        Dim sc = CType(SmilePopup1.Tag, SendControl)
        If a.SelectedIndex >= 0 Then
            sc.AddSmile(a.SelectedItem?.ToString, a.Tag.ToString(), False)
            a.SelectedIndex = - 1
        End If
    End Sub

    Public Shared Sub UpdatePageMessageStatus(Optional user_id As Integer = 0,
                                              Optional chat_id As Integer = 0,
                                              Optional title As String = "")
        If user_id > 0 Then
            If Messages.MessagesDictionary.ContainsKey(user_id.ToString()) Then
                PageMessageRun2.Text = Messages.MessagesDictionary(user_id.ToString()).count.ToString()
            End If
            If Lists.ProfilesDictionary.ContainsKey(user_id) Then
                PageMessageRun3.Text = Lists.ProfilesDictionary(user_id).first_name
            End If
        ElseIf chat_id > 0 Then
            If Messages.MessagesDictionary.ContainsKey("c" & chat_id) Then
                PageMessageRun2.Text = Messages.MessagesDictionary("c" & chat_id).count.ToString()
            End If
            PageMessageRun3.Text = title
        End If
    End Sub

    Public Shared MySettingInVk As String = ""

    Public Shared Function DataToString(value As String) As String
#If Not XP Then
        Dim a = DateTimeOffset.FromUnixTimeSeconds(CInt(value)).DateTime.ToLocalTime()
#Else
        Dim a = TimeZone.CurrentTimeZone.ToLocalTime(New DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(CInt(value)))
#End If
        Dim s As String
        Dim g = DateDiff(DateInterval.Day, a, Now)
        If g = 0 AndAlso a.Day = Now.Day Then
            s = String.Format("{0} {1} {2}", My.Resources.today, My.Resources.in1, a.ToLongTimeString())
        Else
            s = a.ToString()
        End If
        Return s
    End Function

    Public Shared Function GetUnixTime() As String
        Return (DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1))).TotalSeconds.ToString()
    End Function

    Public Shared Async Sub SaveSettingsToVk()
        Dim a = New Dictionary(Of String, Object)
        Dim ignoreKeys =
                {"YandexToday", "isMicroVKGroup", "MD5", "AccessToken", "Version", "Voice", "windowsSize",
                 "isWindowsSize", "saveProfile", "SmilePanel", "FloatTopMost", "IsMediaKey", "IsTabBottom", "photoSize",
                 "OnlineWait"}
        For Each s As SettingsProperty In My.Settings.Properties
            If _
                Not ignoreKeys.Contains(s.Name) AndAlso Not s.Name Like "speak*" AndAlso Not s.Name Like "microVK*" AndAlso
                Not s.Name Like "ds*" AndAlso s.DefaultValue.ToString() <> My.Settings.Item(s.Name).ToString() Then
                a.Add(s.Name, My.Settings.Item(s.Name))

            End If
        Next
        If a.Count = 0 Then Exit Sub
        Dim p = JsonConvert.SerializeObject(a)
        Dim m =
                BitConverter.ToString(
                    CType(CryptoConfig.CreateFromName("MD5"), HashAlgorithm).ComputeHash(New UTF8Encoding().GetBytes(p))) _
                .Replace("-"c, "").ToLower()
        If My.Settings.MD5 <> m Then
            Await Storage.Set("MicroVKSettings", p)
            My.Settings.MD5 = m
            My.Settings.Save()
        End If
    End Sub

    Public Function AdjustBrightness(originalColour As Color, brightnessFactor As Double) As Color
        Dim adjustedColour As Color = Color.FromArgb(originalColour.A,
                                                     CByte(originalColour.R*brightnessFactor),
                                                     CByte(originalColour.G*brightnessFactor),
                                                     CByte(originalColour.B*brightnessFactor))
        Return adjustedColour
    End Function

    Public Shared VkContentAttachment As types.attachment


    Public Shared Function IsSendStatistic() As Boolean
#If DEBUG Then
        Return True
#End If
        If Not IsNothing(MyUser) AndAlso MyUser.id = 64974451 Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Shared IsUpdateYandexImage As Boolean

    Public Shared Async Function LoginInToken(url As String) As Task(Of Boolean)
        Dim t = url.GetParametr("access_token")
        If t.Length > 0 Then
            AccessToken = t
            My.Settings.AccessToken = AccessToken
            MyWindow1.ContentSource = New Uri("Pages/PageMessage.xaml", UriKind.RelativeOrAbsolute)
            If _
                Deployment.Application.ApplicationDeployment.IsNetworkDeployed AndAlso
                My.Settings.Version <>
                Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() Then
                MyWindow1.ShowFlyout(Position.Right, "Content/ContentHistoryAndGroupJoin.xaml#1", "")
            End If
            My.Settings.Save()
            MySettingInVk = Await Storage.Get("MicroVKSettings")
            If Not IsNothing(MySettingInVk) AndAlso IsNothing(MyUser) Then
                Dim u = Await Users.Get(, "photo")
                If Not IsNothing(u) Then
                    MyUser = u(0)
                    UpdateMyUser()
                End If
            End If
            If Not IsNothing(MySettingInVk) AndAlso Not IsNothing(MyUser) Then
                If _
                    ModernDialog.ShowMessage(String.Format(My.Resources.WelcomeBack, MyUser.first_name),
                                             My.Resources.WelcomeBack1,
                                             MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                    Try
                        Dim d =
                                Await _
                                MyJsonConvert.DeserializeObjectAsync (Of Dictionary(Of String, Object))(MySettingInVk)
                        If d?.Count > 0 Then
                            For Each k In d
                                Dim v = My.Settings.Properties(k.Key)
                                If Not IsNothing(v) Then
                                    Dim type1 = My.Settings.Properties(k.Key).PropertyType
                                    If type1 = GetType(Windows.Media.Color) Then
                                        My.Settings.Item(k.Key) = ColorConverter.ConvertFromString(k.Value.ToString())
                                    Else
                                        My.Settings.Item(k.Key) = CTypeDynamic(k.Value, type1)
                                    End If


                                End If
                            Next
                            My.Settings.Save()
                            Forms.Application.Restart()
                            My.Application.Shutdown()
                        End If
                    Catch ex As Exception

                    End Try

                End If
            End If
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared ModernProgressRing1 As ModernProgressRing
    Public Shared RunMinimizedActive As Boolean
    Public Shared SendControl As SendControl

    Public Shared Async Sub UpdateYandexImage()
        If _
            My.Settings.UseYandexImage AndAlso File.Exists(My.Settings.Background) AndAlso
            (DateTime.Today = My.Settings.YandexToday) Then

            Exit Sub
        End If

        If My.Settings.UseYandexImage AndAlso Not IsUpdateYandexImage Then
            Using wc As New WebClient
                IsUpdateYandexImage = True
                Dim img = Path.GetTempFileName()
                img = img.Replace(".tmp", ".jpg")

                If Not img Like "*.jpg" Then
                    img = img & ".jpg"
                End If

                If ModernProgressRing1 IsNot Nothing Then ModernProgressRing1.Visibility = Visibility.Visible
                Try
                    Await wc.DownloadFileTaskAsync("https://yandex.com/images/today?client=microVK", img)
                Catch ex As Exception

                End Try

                If ModernProgressRing1 IsNot Nothing Then ModernProgressRing1.Visibility = Visibility.Collapsed
                If My.Settings.UseYandexImage Then My.Settings.Background = img
                IsUpdateYandexImage = False
                My.Settings.YandexToday = DateTime.Today
                Exit Sub
            End Using
        End If
        Exit Sub
    End Sub

    Private Shared Sub AddNotification(n As types.Notifycation)
        NotificationList1.Add(n)
        If FavoriteUser.IndexOf(n.user_id) >= 0 Then
            FavoriteNotificationList1.Add(n)
        End If
    End Sub

    Public Shared Function GetVkSmile(hex As String) As String
        If (hex.IndexOf("-", StringComparison.Ordinal) > 0) Then
            Dim t = hex.Split("-"c)
            hex = t(0).PadLeft(8, "0"c) & t(1).PadLeft(8, "0"c)
        Else
            hex = hex.PadLeft(8, "0"c)
        End If
        Dim a = Encoding.GetEncoding(12001).GetString(FromHex(hex))
        Return a
    End Function

    Public Shared Function FromHex(hex As String) As Byte()

        hex = hex.Replace("-", "")
        Dim bytes(hex.Length\2 - 1) As Byte
        For i1 = 0 To bytes.Length - 1
            bytes(i1) = Byte.Parse(hex.Substring(i1*2, 2), Globalization.NumberStyles.HexNumber)
        Next

        Return bytes
    End Function


    '1 — женский;
    '2 — мужской;
    '0 — пол не указан.
    Shared Function SexToString(text As String, sex As Integer) As String
        Dim str = text.Split(";"c)
        If sex > str.Length - 1 Then
            Return str(0)
        Else
            Return str(sex)
        End If
    End Function

    Public Function GetJsonForMetric(key As String, value As String) As String
        Return String.Format("{{""{0}"":""{1}""}}", key, value)
    End Function

    Public Shared Sub ProcessStart(path As String)
        Try
            Process.Start(path)
        Catch ex As Exception
            MsgMui(ex.Message)
        End Try
    End Sub

    Public Shared Function AttachmentToString(type As String) As String
        Select Case type
            Case "sticker"
                Return My.Resources.sticker
            Case "photo"
                Return My.Resources.Photo
            Case "video"
                Return My.Resources.Video
            Case "audio"
                Return My.Resources.Audio
            Case "doc"
                Return My.Resources.Doc
            Case "wall"
                Return My.Resources.postWall
            Case "wall_reply"
                Return My.Resources.wall_reply
            Case "link"
                Return My.Resources.Link
            Case Else
                Return My.Resources.unknown
        End Select
    End Function

    Public Shared Sub UpdateTheme()
        My.Settings.dsColor = My.Settings.dsColor
        My.Settings.dsFontColor = My.Settings.dsFontColor
    End Sub

    Public Shared Async Sub UpdateMyUser()
        If MyUser IsNot Nothing Then
            If MyUser.id <> 64974451 Then
                Stats.TrackVisitor()
            End If
            If My.Settings.saveProfile Then
                Await SettingSystem.UpdateMicroVkAccounts()
                Dim firstAccount = SettingSystem.MicroVkAccounts.FirstOrDefault(Function(account) _
                                                                                   account.id = MyUser.id.ToString() AndAlso
                                                                                   account.AccessToken =
                                                                                   SettingSystem.AES_Encrypt(AccessToken,
                                                                                                             MyUser.id.
                                                                                                                ToString()))
                If firstAccount Is Nothing Then
                    Dim p =
                            SettingSystem.MicroVkAccounts.FirstOrDefault(
                                Function(account) account.id = MyUser.id.ToString())
                    If p IsNot Nothing Then
                        SettingSystem.MicroVkAccounts.Remove(p)
                    End If
                    If SettingSystem.MicroVkAccounts.Count > 20 Then
                        SettingSystem.MicroVkAccounts.RemoveAt(20)
                    End If
                    SettingSystem.MicroVkAccounts.Insert(0,
                                                         New MicroVkAccount() With {.full_name = MyUser.full_name,
                                                            .AccessToken =
                                                            SettingSystem.AES_Encrypt(AccessToken, MyUser.id.ToString()),
                                                            .photo = MyUser.photo,
                                                            .id = MyUser.id.ToString()})
                    SettingSystem.SaveSettings("MicroVkAccounts", SettingSystem.MicroVkAccounts)
                End If
            End If
            MyWindow1.UpdateLogo()
            Lists.ProfilesDictionary(MyUser.id) = MyUser
        End If
    End Sub
End Class
