Imports System.Globalization
Imports System.Threading
Imports System.Windows.Forms
Imports FirstFloor.ModernUI.Presentation

Partial Public Class ControlSettingsAppearance
    Private Sub ListBox1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If CType(ListBox1.SelectedItem, System.Windows.Media.Color) <> AppearanceManager.Current.AccentColor Then
            AppearanceManager.Current.AccentColor = CType(ListBox1.SelectedItem, System.Windows.Media.Color)
            My.Settings.AccentColor = CType(ListBox1.SelectedItem, System.Windows.Media.Color)
            Debug.Print(My.Settings.AccentColor.ToString())
        End If
    End Sub

    Private Sub ComboBox1_OnSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If My.Settings.Theme <> ComboBox1.SelectedIndex Then
            Select Case ComboBox1.SelectedIndex
                Case 0
                    AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImage.xaml",
                                                                    UriKind.RelativeOrAbsolute)
                Case 1
                    AppearanceManager.Current.ThemeSource = New Uri("Themes/ThemeMyImageDark.xaml",
                                                                    UriKind.RelativeOrAbsolute)
            End Select
            My.Settings.Theme = ComboBox1.SelectedIndex
            OtherApi.UpdateTheme()
        End If
    End Sub

    Private Sub ComboBox2_OnSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If My.Settings.Font <> ComboBox2.SelectedIndex Then
            Select Case ComboBox2.SelectedIndex
                Case 0
                    AppearanceManager.Current.FontSize = FirstFloor.ModernUI.Presentation.FontSize.Small
                Case 1
                    AppearanceManager.Current.FontSize = FirstFloor.ModernUI.Presentation.FontSize.Large
                Case 2
                    AppearanceManager.Current.FontSize = FirstFloor.ModernUI.Presentation.FontSize.Big
            End Select
            My.Settings.Font = ComboBox2.SelectedIndex
        End If
    End Sub

    Private Sub checkBox_Checked(sender As Object, e As RoutedEventArgs)
        If Not IsNothing(Slider1) Then Slider1.Value = 0.15
    End Sub

    Private Sub ControlSettingsAppearance_Onloaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If IsNothing(ListBox1.ItemsSource) Then
            ListBox1.ItemsSource = New System.Windows.Media.Color() _
                {System.Windows.Media.Color.FromRgb(89, 125, 163), System.Windows.Media.Color.FromRgb(&HA4, &HC4, &H0),
                 System.Windows.Media.Color.FromRgb(&H60, &HA9, &H17),
                 System.Windows.Media.Color.FromRgb(&H0, &H8A, &H0), System.Windows.Media.Color.FromRgb(&H0, &HAB, &HA9),
                 System.Windows.Media.Color.FromRgb(&H1B, &HA1, &HE2),
                 System.Windows.Media.Color.FromRgb(&H0, &H50, &HEF),
                 System.Windows.Media.Color.FromRgb(&H6A, &H0, &HFF),
                 System.Windows.Media.Color.FromRgb(&HAA, &H0, &HFF),
                 System.Windows.Media.Color.FromRgb(&HF4, &H72, &HD0),
                 System.Windows.Media.Color.FromRgb(&HD8, &H0, &H73),
                 System.Windows.Media.Color.FromRgb(&HA2, &H0, &H25),
                 System.Windows.Media.Color.FromRgb(&HE5, &H14, &H0),
                 System.Windows.Media.Color.FromRgb(&HFA, &H68, &H0),
                 System.Windows.Media.Color.FromRgb(&HF0, &HA3, &HA),
                 System.Windows.Media.Color.FromRgb(&HE3, &HC8, &H0),
                 System.Windows.Media.Color.FromRgb(&H82, &H5A, &H2C),
                 System.Windows.Media.Color.FromRgb(&H6D, &H87, &H64),
                 System.Windows.Media.Color.FromRgb(&H64, &H76, &H87),
                 System.Windows.Media.Color.FromRgb(&H76, &H60, &H8A),
                 System.Windows.Media.Color.FromRgb(&H87, &H79, &H4E)}
            ComboBox1.ItemsSource = {My.Resources.themeLight, My.Resources.themeDark}
            ComboBox2.ItemsSource = {My.Resources.FontSmall, My.Resources.FontNormal, My.Resources.big}

            For Each i As Windows.Media.Color In ListBox1.Items
                If My.Settings.AccentColor = i Then
                    ListBox1.SelectedItem = i
                End If
            Next

            ComboBox1.SelectedIndex = My.Settings.Theme
            ComboBox2.SelectedIndex = My.Settings.Font
            ComboBox3.ItemsSource = Application.LanguageArray
            For Each l As types.LanguageItem In ComboBox3.Items
                If l.Culture = My.Settings.Culture Then
                    ComboBox3.SelectedItem = l
                    Exit For
                End If
            Next

            If ComboBox3.SelectedIndex < 0 Then
                ComboBox3.SelectedIndex = 3
            End If
        End If
    End Sub

    Private Sub ComboBox3_OnSelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        My.Settings.Culture = CType(ComboBox3.SelectedItem, types.LanguageItem).Culture
        Dim c As New CultureInfo(My.Settings.Culture)
#If NET46 Then
        CultureInfo.DefaultThreadCurrentCulture = c
        CultureInfo.DefaultThreadCurrentUICulture = c
#ElseIf XP Then
        Thread.CurrentThread.CurrentCulture = c
        Thread.CurrentThread.CurrentUICulture = c
#End If
        My.Settings.Save()
    End Sub

    Private Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        System.Windows.Forms.Application.Restart()
        My.Application.Shutdown()
    End Sub

    Private Sub CheckBox1_OnChecked(sender As Object, e As RoutedEventArgs)
    End Sub


    Private Sub Button1_OnClick(sender As Object, e As RoutedEventArgs)
        If My.Settings.UseYandexImage Then
        End If
        Dim dlg = New OpenFileDialog
        Dim result = dlg.ShowDialog()
        If result = DialogResult.OK Then
            My.Settings.UseYandexImage = False
            My.Settings.Background = dlg.FileName
        End If
    End Sub

    Private Sub CheckBox1_OnClick(sender As Object, e As RoutedEventArgs)
        If checkBox1.IsChecked Then
            My.Settings.IsBackground = True
            My.Settings.Background = ""
            OtherApi.ModernProgressRing1 = ModernProgressRing1
            OtherApi.UpdateYandexImage()
        End If
    End Sub

    Private Sub ModernProgressRing1_OnLoaded(sender As Object, e As RoutedEventArgs)
        If OtherApi.IsUpdateYandexImage Then
            ModernProgressRing1.Visibility = Visibility.Visible
        Else
            ModernProgressRing1.Visibility = Visibility.Collapsed
        End If
    End Sub
End Class
