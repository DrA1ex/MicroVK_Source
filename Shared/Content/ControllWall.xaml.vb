Imports FirstFloor.ModernUI.Windows
Imports System.Windows.Media.Animation
Imports MicroVK.Api

Public Class ControllWall
    Implements IContent

    Public Async Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation
        ModernProgressRing1.Visibility = Visibility.Visible
        ListBox1.ItemsSource = Nothing
        lists.newsfeedlist1.items.Clear()
        ListBox1.Items.Refresh()
        Me.Tag = e.Fragment
        Await newsfeed.get(e.Fragment)
        ListBox1.ItemsSource = lists.newsfeedlist1.items
        ModernProgressRing1.Visibility = Visibility.Collapsed
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Private Async Sub ListBox1_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        Dim p = CType(e.OriginalSource, ScrollViewer)
        p.ApplyTemplate()
        Dim a = p.Template.FindName("PART_VerticalScrollBar", p)
        Dim b As Primitives.ScrollBar = CType(a, Primitives.ScrollBar)
        If b.Value/b.Maximum > 0.9 AndAlso ListBox1.Items.Count > 0 Then
            ModernProgressRing1.Visibility = Visibility.Visible
            If lists.newsfeedlist1.next_from <> "" Then _
                Await newsfeed.get(Me.Tag.ToString & "&start_from=" & lists.newsfeedlist1.next_from)
            ModernProgressRing1.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        ModernProgressRing1.Visibility = Visibility.Visible
        lists.newsfeedlist1.items.Clear()
        ListBox1.Items.Refresh()
        ListBox1.ItemsSource = Nothing
        Await newsfeed.get(Me.Tag.ToString)
        ListBox1.ItemsSource = lists.newsfeedlist1.items
        ModernProgressRing1.Visibility = Visibility.Collapsed
    End Sub

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        scrollViewer1 = CType(VisualTreeHelper.GetChild(ListBox1, 0), AniScrollViewer)
        If ListBox1.SelectedIndex < 0 Then
            ListBox1.SelectedIndex = 0
        End If
        If ListBox1.SelectedIndex <= ListBox1.Items.Count Then
            ListBox1.SelectedIndex += 1
            Dim sdf = ListBox1.GetVerticalOffset(ListBox1.SelectedItem)
            If Math.Abs(sdf - 0) < OtherApi.TOLERANCE Then
                ListBox1.ScrollIntoView(ListBox1.SelectedItem)
            Else
                ScrollToEnd(ListBox1.GetVerticalOffset(ListBox1.SelectedItem))
            End If


        End If
    End Sub

    Private ReadOnly scrollEndAnim As New DoubleAnimation With {.Duration = TimeSpan.FromMilliseconds(500),
        .EasingFunction = New SineEase}

    Private scrollViewer1 As AniScrollViewer

    Private Sub ScrollToEnd(offset As Double)
        scrollEndAnim.From = scrollViewer1.VerticalOffset
        scrollEndAnim.To = offset
        scrollViewer1.BeginAnimation(AniScrollViewer.CurrentVerticalOffsetProperty, scrollEndAnim)
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        scrollViewer1 = CType(VisualTreeHelper.GetChild(ListBox1, 0), AniScrollViewer)
        If ListBox1.SelectedIndex > 0 Then
            ListBox1.SelectedIndex -= 1
            Dim sdf = ListBox1.GetVerticalOffset(ListBox1.SelectedItem)
            If Math.Abs(sdf - 0) < OtherApi.TOLERANCE Then
                ListBox1.ScrollIntoView(ListBox1.SelectedItem)
            Else
                ScrollToEnd(ListBox1.GetVerticalOffset(ListBox1.SelectedItem))
            End If
        End If
    End Sub

    Private Sub UIElement_OnPreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Down Then
            Button_Click_1(sender, Nothing)
            e.Handled = True
        ElseIf e.Key = Key.Up
            Button_Click_2(sender, Nothing)
            e.Handled = True
        End If
    End Sub

    Private Sub ListBox1_OnInitialized(sender As Object, e As EventArgs)
#If not XP
        VirtualizingPanel.SetScrollUnit(ListBox1, ScrollUnit.Pixel)
#End If
    End Sub
End Class
