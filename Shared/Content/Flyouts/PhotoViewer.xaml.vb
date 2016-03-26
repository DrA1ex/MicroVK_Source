' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
Imports System.Collections.ObjectModel
Imports System.Windows.Interactivity
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Controls
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MahApps.Metro.Controls
Imports MicroVK.CSharp.Helpers
Imports MicroVK.types

Public NotInheritable Class PhotoViewer
    Inherits UserControl
    Implements IContent
    Public Shared Attachments As New ObservableCollection(Of Object)

    Public Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
        If FlipView1.ItemsSource IsNot Nothing Then
            FlipView1.ItemsSource = Nothing
        End If
        FlipView1.ItemsSource = Attachments
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private _fullScreenWindow As ModernWindow

    Private Sub FullScreenButton_OnClick(sender As Object, e As RoutedEventArgs)
        ToogleFullScreen()
    End Sub

    Sub ToogleFullScreen()
        If (_fullScreenWindow Is Nothing) Then
            _fullScreenWindow = New ModernWindow
            _fullScreenWindow.Style = TryCast(_fullScreenWindow.FindResource("EmptyWindow"), Style)
            Content = Nothing
            _fullScreenWindow.Topmost = True
            _fullScreenWindow.Content = RootGrid1
            _fullScreenWindow.WindowStyle = WindowStyle.None
            _fullScreenWindow.WindowState = WindowState.Maximized
            _fullScreenWindow.ResizeMode = ResizeMode.NoResize
            AddHandler _fullScreenWindow.Closing, Sub()
                _fullScreenWindow.Content = Nothing
                _fullScreenWindow = Nothing
                Content = RootGrid1
                                                  End Sub
            ContentControl1.FindChild (Of Path)("Path1").Data = TryCast(FindResource("fullscreen_exit"), Geometry)

            UpdteFlipView()

            _fullScreenWindow.Show()
        Else
            _fullScreenWindow.Content = Nothing
            _fullScreenWindow.Close()
            _fullScreenWindow = Nothing
            Content = RootGrid1
            ContentControl1.FindChild (Of Path)("Path1").Data = TryCast(FindResource("fullscreen"), Geometry)
        End If
    End Sub

    Private Sub FlipView1_OnMouseDown(sender As Object, e As MouseButtonEventArgs)
        If FlipView1.SelectedIndex < FlipView1.Items.Count - 1 Then
            FlipView1.SelectedIndex += 1
        Else
            FlipView1.SelectedIndex = 0
        End If
    End Sub

    Sub UpdteFlipView()
        Dim temp = FlipView1.SelectedIndex
        If FlipView1.ItemsSource IsNot Nothing Then
            FlipView1.ItemsSource = Nothing
        End If
        FlipView1.ItemsSource = Attachments
        FlipView1.SelectedIndex = temp
    End Sub

    Private Sub FlipView1_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim a = TryCast(FlipView1.SelectedItem, attachment)
        If OtherApi.MyWindow1.FlyoutUrl Like "*PhotoViewer*" AndAlso a IsNot Nothing Then
            OtherApi.MyWindow1.Flyout1.Header = OtherApi.AttachmentToString(a.type) & " (" & FlipView1.SelectedIndex + 1 &
                                                "/" & FlipView1.Items.Count & ")"

            If TypeOf FlipView1.SelectedItem Is wall_post Then
                RootGrid1.Cursor = Cursors.Arrow
                ScrollViewer1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                ScrollViewer1.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            Else
                RootGrid1.Cursor = Cursors.Hand
                ScrollViewer1.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
                ScrollViewer1.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            End If
        End If
    End Sub

    Private Sub PhotoViewer_OnLoaded(sender As Object, e As RoutedEventArgs)
    End Sub
End Class
