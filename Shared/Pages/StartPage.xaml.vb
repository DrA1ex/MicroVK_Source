' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports FirstFloor.ModernUI.Presentation
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MahApps.Metro.Controls
Imports MicroVK.OtherLib

Public NotInheritable Class StartPage
    Inherits UserControl
    Implements IContent

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
        OtherApi.MyWindow1.BackButton.Visibility = Visibility.Visible
        Select Case My.Settings.Theme
            Case 0
                AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml", UriKind.RelativeOrAbsolute)
            Case 1
#If Not xp Then
                AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImageDark.xaml",
                                                                UriKind.RelativeOrAbsolute)
#Else
                    AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml",
                                                                    UriKind.RelativeOrAbsolute)
#End If
        End Select
    End Sub

    Public Async Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml", UriKind.RelativeOrAbsolute)
        OtherApi.MyWindow1.BackButton.Visibility = Visibility.Collapsed
        Await SettingSystem.UpdateMicroVkAccounts()
        If SettingSystem.MicroVkAccounts?.Count > 0
            AccountsButton.Visibility = Visibility.Visible
        else
            AccountsButton.Visibility = Visibility.Collapsed
        End If
        MembersRun1.Text = (Await Api.Apps.Get("2664100"))?.items?.FirstOrDefault()?.members_count.ToString()
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub StartPage_OnInitialized(sender As Object, e As EventArgs)
        LanguageComboBox.ItemsSource = Application.LanguageArray
        For Each l As types.LanguageItem In LanguageComboBox.Items
            If l.Culture = My.Settings.Culture Then
                LanguageComboBox.SelectedItem = l
                Exit For
            End If
        Next

        If LanguageComboBox.SelectedIndex < 0 Then
            LanguageComboBox.SelectedIndex = 3
        End If
        _isSelectedLanguage = True
    End Sub

    Private Sub StartButton_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ContentSource = New Uri("Pages/PageLogin2.xaml", UriKind.RelativeOrAbsolute)
    End Sub

    Private _isSelectedLanguage As Boolean

    Private Sub LanguageComboBox_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If _isSelectedLanguage Then
            My.Settings.Culture = CType(LanguageComboBox.SelectedItem, types.LanguageItem).Culture
            Forms.Application.Restart()
            My.Application.Shutdown()
        End If
    End Sub

    Private Sub StatsButton_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.ProcessStart("https://vk.com/stats?aid=2664100")
    End Sub

    Private Sub AccountsButton_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ModernFrame1.Source = nothing
        OtherApi.MyWindow1.ShowFlyout(Position.Left, "Content/Flyouts/AccountManager.xaml", My.Resources.AccountManager)
    End Sub
End Class
