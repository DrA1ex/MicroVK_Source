Imports FirstFloor.ModernUI.Presentation
Imports FirstFloor.ModernUI.Windows
Imports MahApps.Metro.Controls

Class PageAudio
    Inherits UserControl
    Implements IContent

    Private Sub ListBox1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If ListBox1.SelectedIndex >= 0 Then
            ListBox2.SelectedIndex = - 1
            ModernFrame1.Source = CType(ListBox1.SelectedItem, FirstFloor.ModernUI.Presentation.Link).Source
        End If
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        ListBox1.SelectedIndex = - 1
        ListBox2.SelectedIndex = - 1
        ModernFrame1.Source = New Uri("\Content\ControlAudio.xaml#page=search,q=" & TextBox1.Text,
                                      UriKind.RelativeOrAbsolute)
    End Sub

    Private Sub ListBox2_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If ListBox2.SelectedIndex >= 0 Then
            ListBox1.SelectedIndex = - 1
            ModernFrame1.Source =
                New Uri(
                    CType(ListBox2.SelectedItem, FirstFloor.ModernUI.Presentation.Link).Source.ToString & ",only_eng=" &
                    CInt(CheckBox1.IsChecked)*- 1,
                    UriKind.RelativeOrAbsolute)
        End If
    End Sub

    Public Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        Dim target_audio = e.Fragment.GetParametr("target_audio")
        If target_audio.Length > 0 Then
            ModernFrame1.Source = New Uri("\Content\ControlAudio.xaml#page=recommendations,target_audio=" & target_audio,
                                          UriKind.RelativeOrAbsolute)

        End If
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub TextBox1_OnKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
        If e.Key = Key.Enter Then
            ListBox1.SelectedIndex = - 1
            ListBox2.SelectedIndex = - 1
            ModernFrame1.Source = New Uri("\Content\ControlAudio.xaml#page=search,q=" & TextBox1.Text,
                                          UriKind.RelativeOrAbsolute)
        End If
    End Sub

    Private Sub TextBox2_OnInitialized(sender As Object, e As EventArgs)
        TextBoxHelper.SetButtonCommand(TextBox1,
                                       New RelayCommand(Sub(o)
                                           ListBox1.SelectedIndex = - 1
                                           ListBox2.SelectedIndex = - 1
                                           ModernFrame1.Source =
                                                           New Uri(
                                                               "\Content\ControlAudio.xaml#page=search,q=" &
                                                               TextBox1.Text,
                                                               UriKind.RelativeOrAbsolute)
                                                           End Sub))
    End Sub
End Class
