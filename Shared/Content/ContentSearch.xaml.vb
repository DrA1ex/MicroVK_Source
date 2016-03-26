
Imports System.Windows.Threading
Imports FirstFloor.ModernUI.Presentation
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MahApps.Metro.Controls

Class SearchPath
    Public Property Name As String
    Property Source As String
End Class

Public Class ContentSearch
    Implements IContent
    Private ReadOnly _timer1 As New DispatcherTimer With {.Interval = New TimeSpan(0, 0, 0, 0, 500)}


    Private SearchPathList As List(Of SearchPath)

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        Dim mode = e.Fragment.GetParametr("mode")
        Dim selectIndex = e.Fragment.GetParametr("selectIndex")
        If selectIndex.Length > 0 Then
            ComboBox1.SelectedIndex = CInt(selectIndex)
        End If
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        If SearchPathList Is Nothing Then
            SearchPathList = New List(Of SearchPath)() From {New SearchPath() With {.Name = My.Resources.GlobalSearch,
                .Source = "Content/ContentSearchGlobal.xaml"},
                New SearchPath() With {.Name = My.Resources.Dialogs,
                    .Source = "Content/ContentSearchDialog.xaml"},
                New SearchPath() With {.Name = My.Resources.Messages,
                    .Source = "Content/ContentSearchMessage.xaml"}
                }
        End If
        ComboBox1.ItemsSource = SearchPathList
        ComboBox1.SelectedIndex = My.Settings.SearchSelectIndex
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Sub ComboBox1_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        My.Settings.SearchSelectIndex = ComboBox1.SelectedIndex
        ModernFrame1.Source = New Uri(CType(ComboBox1.SelectedItem, SearchPath).Source & "#q=" & TextBox1.Text,
                                      UriKind.RelativeOrAbsolute)
    End Sub

    Private Sub TextBox1_OnInitialized(sender As Object, e As EventArgs)
        TextBoxHelper.SetButtonCommand(TextBox1,
                                       New RelayCommand(Sub(o)
                                           ModernFrame1.Source =
                                                           New Uri(
                                                               CType(ComboBox1.SelectedItem, SearchPath).Source & "#q=" &
                                                               TextBox1.Text,
                                                               UriKind.RelativeOrAbsolute)
                                                           End Sub))
    End Sub

    Private Sub TextBox1_OnTextChanged(sender As Object, e As TextChangedEventArgs)
        _timer1.Stop()
        _timer1.Start()
        ModernProgressRing1.Visibility = Visibility.Visible
    End Sub

    Private Sub ContentSearch_OnInitialized(sender As Object, e As EventArgs)
        AddHandler _timer1.Tick, AddressOf timer1_Tick
        _timer1.Start()
    End Sub

    Private Sub timer1_Tick(sender As Object, e As EventArgs)
        ModernProgressRing1.Visibility = Visibility.Collapsed
        ModernFrame1.Source = New Uri(CType(ComboBox1.SelectedItem, SearchPath).Source & "#q=" & TextBox1.Text,
                                      UriKind.RelativeOrAbsolute)
    End Sub

    Private Sub TextBox1_OnKeyDown(sender As Object, e As KeyEventArgs)
    End Sub
End Class
