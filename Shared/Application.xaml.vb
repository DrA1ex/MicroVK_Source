Imports System.Globalization
Imports System.Threading
Imports System.Windows.Threading
Imports FirstFloor.ModernUI.Presentation
Imports MicroVK.Api
Imports Un4seen.Bass

Public Class Application
    Public Shared _
        LanguageArray As types.LanguageItem() =
            {New types.LanguageItem With {.Culture = "en", .Description = "English"},
             New types.LanguageItem With {.Culture = "be", .Description = "Беларускі"},
             New types.LanguageItem With {.Culture = "uk", .Description = "Українська"},
             New types.LanguageItem With {.Culture = "ru", .Description = "Русский"},
             New types.LanguageItem With {.Culture = "kk", .Description = "Қазақ", .AutoTranslate = True}}

    Private ReadOnly _lang As String

    ' События приложения, например, Startup, Exit и DispatcherUnhandledException,
    ' можно обрабатывать в данном файле.
    Sub New()
        _lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName
        If My.Settings.Culture.Length = 0 Or My.Settings.Culture = "system" Then
            My.Settings.Culture = _lang
            For Each l In LanguageArray
                If Left(l.Culture, 2) = _lang AndAlso l.AutoTranslate Then
                    My.Settings.Culture = "ru"
                End If
            Next
        End If

        Dim c As New CultureInfo(My.Settings.Culture)
        My.Settings.Culture = My.Settings.Culture.ToLower()
#If NET46 Then
        CultureInfo.DefaultThreadCurrentCulture = c
        CultureInfo.DefaultThreadCurrentUICulture = c
#Else
        Thread.CurrentThread.CurrentCulture = c
        Thread.CurrentThread.CurrentUICulture = c
#End If
    End Sub

#If Not XP Then

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs)
#Else

    Private Async Sub Application_Startup(sender As Object, e As StartupEventArgs)
#End If

        OtherApi.Lang = Mid(My.Settings.Culture, 1, 2)

        If OtherApi.Lang = "Uk" Then
            OtherApi.Lang = "ua"
        End If

        If My.Settings.UseYandexImage Then
            Call OtherApi.UpdateYandexImage()
        End If

        If My.Settings.Theme = 2 Then My.Settings.Theme = 0
        AppearanceManager.Current.AccentColor = My.Settings.AccentColor
        OtherApi.SoundPlayer1.Open(New Uri("vk.mp3", UriKind.Relative))

        Select Case My.Settings.Font
            Case 0
                AppearanceManager.Current.FontSize = FirstFloor.ModernUI.Presentation.FontSize.Small
            Case 1
                AppearanceManager.Current.FontSize = FirstFloor.ModernUI.Presentation.FontSize.Large
            Case 2
                AppearanceManager.Current.FontSize = FirstFloor.ModernUI.Presentation.FontSize.Big
        End Select

        Select Case My.Settings.Theme
            Case 0
                AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml", UriKind.RelativeOrAbsolute)
            Case 1
#If Not XP Then
                AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImageDark.xaml",
                                                                UriKind.RelativeOrAbsolute)
#Else
                AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml", UriKind.RelativeOrAbsolute)
#End If
        End Select

#If DEBUG AndAlso Not XP Then
        Yandex.Metrica.YandexMetrica.Activate("d536b907-45d6-4831-b0a0-a89e6aac86c8")
#ElseIf Not XP Then
        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
            Yandex.Metrica.YandexMetrica.SetCustomAppVersion(
                Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion)
        End If
        Yandex.Metrica.YandexMetrica.Activate("23d01c34-a80b-4437-ae4c-fe82fab72235")
#ElseIf Not DEBUG Then
        Using w As New Net.WebClient
            If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
                Dim a = w.DownloadDataTaskAsync("https://mc.yandex.ru/watch/25144754")
            End If
        End Using
#End If
        AddHandler OtherApi.Timer1.Tick, AddressOf timer1_Tick
#If DEBUG Then
        If OtherApi.AccessToken.Length = 0 Then
            OtherApi.AccessToken = My.Settings.AccessToken
        End If
        If Not Deployment.Application.ApplicationDeployment.IsNetworkDeployed And OtherApi.AccessToken <> "" Then
            Status.SetDebugTime(
                String.Format("Пульс MicroVK. Последняя правка: {0} (UTC {1}:00)",
                              Now.ToString("dd.MM.yyyy HH:mm:ss"),
                              TimeZoneInfo.Local.BaseUtcOffset.Hours).ToString(),
                String.Format("Пульс разработки. Последняя правка: {0} (UTC {1}:00)",
                              Now.ToString("dd.MM.yyyy HH:mm:ss"),
                              TimeZoneInfo.Local.BaseUtcOffset.Hours).ToString())
        End If
#If Not XP Then
        Yandex.Metrica.YandexMetrica.ReportEvent("DEBUG")
#End If
#End If
        BassNet.Registration("kryeker@yandex.ru", "2X203737173438")
    End Sub

    Private Sub Application_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs)

        'MessageBox.Show(e.Exception.Message & vbNewLine & e.Exception.StackTrace)
        If Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
#If Not XP Then
            Yandex.Metrica.YandexMetrica.ReportError(e.Exception.Message, e.Exception)

            e.Handled = True
#Else

#End If
            e.Handled = True
        End If
    End Sub

    Private Sub Application_Exit(sender As Object, e As ExitEventArgs)
        TrayIconManager.TaskbarIcon1.Visibility = Visibility.Hidden
#If Not XP Then
        Yandex.Metrica.YandexMetrica.ReportExit()
#End If
        If Not IsNothing(OtherApi.BassPlayer1) Then
            Bass.BASS_Free()
            AddOn.Fx.BassFx.FreeMe()
        End If
        My.Settings.Save()
    End Sub

    Private Sub timer1_Tick(sender As Object, e As EventArgs)
        OtherApi.Timer1.Interval = New TimeSpan(0, 0, 14, 0)
        OtherApi.Timer1.Stop()
        OtherApi.Timer1.Start()
        Account.SetOnline()
    End Sub
End Class
