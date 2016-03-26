
Imports FirstFloor.ModernUI.Windows.Media
Imports FirstFloor.ModernUI.Windows
Imports System.Collections.ObjectModel
Imports System.Text
Imports System.Windows.Controls.Primitives
Imports System.Windows.Media.Animation
Imports FirstFloor.ModernUI.Windows.Navigation
Imports MahApps.Metro.Controls
Imports MicroVK.Api
Imports MicroVK.Converter

Public Class ControlDialogListBox
    Implements IContent

    Private user_id As String,
            chat_id As String = "",
            _isScroll As Boolean = True,
            _messageCount As Integer = 0,
            _isLoadNext As Boolean = True,
            _isOffsetLoaded As Boolean = False

    Public Async Sub OnFragmentNavigation(e As FragmentNavigationEventArgs) Implements IContent.OnFragmentNavigation
        OtherApi.Publiclistbox1 = ListBox1
        user_id = e.Fragment.GetParametr("user_id")
        chat_id = e.Fragment.GetParametr("chat_id")
        Dim key = If(chat_id = "", user_id, "c" & chat_id)
        _fwdMessages = e.Fragment.GetParametr("forward_messages")
        _isLoadNext = False
        If SendControl IsNot Nothing Then
            SendControl.FwdMessages = e.Fragment.GetParametr("forward_messages")
            _isLoadNext = False
            SendControl.UserId = user_id
            SendControl.ChatId = chat_id
        End If

        If IsNothing(Messages.MessagesDictionary(key).items) Then
            Dim a As types.vk_list
            If Messages.FirstDialogKey = key Then
                a = Messages.FirstDialog
            Else
                a = Await Messages.GetHistory(e.Fragment)
            End If

            Messages.FirstDialogKey = ""
            Messages.FirstDialog = Nothing
            If Not IsNothing(a) Then
                If Not Messages.MessagesDictionary.ContainsKey(key) Then Exit Sub
                Messages.MessagesDictionary(key).count = a.count
                Messages.MessagesDictionary(key).items = New ObservableCollection(Of types.message)
                Messages.MessagesDictionary(key).unread = a.unread
                For Each i In a.items
                    Messages.MessagesDictionary(key).items.Insert(0, i)
                    If Not Messages.BadMessagesDictionary.ContainsKey(i.id) And Not i.read_state Then _
                        Messages.BadMessagesDictionary.Add(i.id, i)
                Next
            End If
        End If
        If Not Messages.MessagesDictionary.ContainsKey(key) Then Exit Sub
        Messages.MessagesDictionary(key).ListBoxDialog = ListBox1
        ListBox1.ItemsSource = Messages.MessagesDictionary(key).items
        If ListBox1.Items.Count = 0
            ModernProgressRing1.Visibility = Visibility.Collapsed
            TextBlock1.Visibility = Visibility.Visible
        End If
        OtherApi.UpdatePageMessageStatus(user_id.ToInt,
                                         chat_id.ToInt(),
                                         If _
                                            (
                                                chat_id.Length > 0 AndAlso
                                                Messages.DialogsDictionary.ContainsKey("c" & chat_id),
                                                Messages.DialogsDictionary("c" & chat_id).message.title,
                                                My.Resources.unknown))
        CType(OtherApi.GetScrollViewer(ListBox1), ScrollViewer)?.ScrollToBottom()
    End Sub

    Public Sub OnNavigatedFrom(e As NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Sub OnNavigatedTo(e As NavigationEventArgs) Implements IContent.OnNavigatedTo
    End Sub

    Public Sub OnNavigatingFrom(e As NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
    End Sub

    Public Async Sub ListBox1_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        If TextBlock1.Visibility = Visibility.Visible Then
            TextBlock1.Visibility = If(ListBox1.Items.Count = 0, Visibility.Visible, Visibility.Collapsed)
        End If
        ScrollViewer1 = CType(e.OriginalSource, AniScrollViewer)
        If IsNotScroll Then
            RemoveHandler ScrollViewer1.ScrollChanged, AddressOf ListBox1_ScrollChanged
            Exit Sub
        End If
        ScrollViewer1.ApplyTemplate()
        Dim key = If(user_id.Length > 0, user_id, "c" + chat_id)
        Dim a = ScrollViewer1.Template.FindName("PART_VerticalScrollBar", ScrollViewer1)
        ScrollBar1 = CType(a, ScrollBar)
        If Math.Abs(ScrollBar1.Value - 0) < OtherApi.Tolerance AndAlso _isScroll Then
            _isScroll = False
            If _
                _isLoadNext AndAlso Messages.MessagesDictionary(key).items IsNot Nothing AndAlso
                Messages.MessagesDictionary(key).items.Count <> 0 AndAlso
                Messages.MessagesDictionary(key).items.Count < Messages.MessagesDictionary(key).count Then
                _isOffsetLoaded = True
                Dim temp =
                        (Await _
                        Messages.GetHistory(If(user_id.Length > 0, "user_id=" & user_id, "chat_id=" & chat_id),
                                            Messages.MessagesDictionary(key).items(0).id.ToString))

                If Not IsNothing(temp) Then

                    Dim c = temp.items.Skip(1)
                    If Not Messages.MessagesDictionary.ContainsKey(key) Then Exit Sub
                    Messages.MessagesDictionary(key).count = temp.count
                    Messages.MessagesDictionary(key).unread = temp.unread
                    For Each i In c
                        Messages.MessagesDictionary(key).items.Insert(0, i)
                    Next
                    If Messages.MessagesDictionary(key).items.Count > 19 Then _
                        ListBox1.ScrollIntoView(Messages.MessagesDictionary(key).items(19))
                End If
            Else
                _isLoadNext = True
            End If
            _isScroll = True
        End If
        If _
            _isScroll AndAlso Not ScrollBar1.IsMouseCaptureWithin AndAlso
            (e.ExtentHeightChange > 0 Or
             (Messages.MessagesDictionary(key).items IsNot Nothing AndAlso
              _messageCount <> Messages.MessagesDictionary(key).items.Count)) AndAlso
            Math.Abs(Now.Second - _mouseDate.Second) > 1 Then
            _messageCount = Messages.MessagesDictionary(key).items.Count
            If Not _isOffsetLoaded Then ScrollToEnd(ScrollBar1, ScrollViewer1)
        End If
        If Math.Abs(e.VerticalOffset - ScrollViewer1.ScrollableHeight) < OtherApi.Tolerance Then
            DownScrollButton.Visibility = Visibility.Collapsed
            _isScroll = True
        Else
            DownScrollButton.Visibility = Visibility.Visible
        End If
    End Sub

    Sub MarkMessage()
        Messages.MessagesDictionary(If(user_id <> "", user_id, "c" & chat_id)).unread = 0
        Dim unread_id = 0
        For i = ListBox1.Items.Count - 1 To 0 Step - 1
            Dim a = CType(ListBox1.Items(i), types.message)
            If Not a.read_state And Not a.out Then
                unread_id = a.id
            Else
                Exit For
            End If
        Next

        If unread_id > 0 Then
            Messages.MarkAsRead(If(user_id.Length > 0, "user_id=" & user_id, "chat_id=" & chat_id),
                                unread_id.ToString())
        End If
    End Sub

    Private _isLoadedControl As Boolean

    Private Sub userControl_Loaded(sender As Object, e As RoutedEventArgs)
#If Not XP Then
        VirtualizingPanel.SetScrollUnit(ListBox1, ScrollUnit.Pixel)
#End If
        OtherApi.Publiclistbox1 = ListBox1
        Dim p = Ancestors().ToArray()
        ' ListBox1.Visibility = If(ControlSendMessage1.IsSmall, Visibility.Collapsed, Visibility.Visible)
        If p.Count <= 15 Then
            ListBox1.ItemTemplateSelector = CType(FindResource("SmallMessageDt1"), DataTemplateSelector)
            FontSize = CType(FindResource("SmallFontSize"), Double)

        Else
            ListBox1.ItemTemplateSelector = CType(FindResource("MessageDT1"), DataTemplateSelector)
            FontSize = CType(FindResource("DefaultFontSize"), Double)
        End If
        OtherApi.UpdatePageMessageStatus(user_id.ToInt(), chat_id.ToInt())
        If Not _isLoadedControl Then
            _isLoadedControl = True
            ScrollToEnd(scrollBar1, scrollViewer1)
        End If
    End Sub

    Private Sub ForwardMessageMenuItem_OnClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ReplyMessages()
    End Sub

    Private ReadOnly _scrollEndAnim As New DoubleAnimation With {.Duration = TimeSpan.FromMilliseconds(500),
        .EasingFunction = New SineEase}

    Private Sub ScrollToEnd(scrollBar2 As ScrollBar, scrollViewer2 As AniScrollViewer)
        If Not IsNothing(scrollBar2) Then
            If Math.Abs(scrollViewer2.VerticalOffset - 0) < OtherApi.TOLERANCE Then
                scrollViewer2.ScrollToVerticalOffset(1)
            End If
            _scrollEndAnim.From = scrollViewer2.VerticalOffset
            _scrollEndAnim.To = scrollBar2.Maximum
            scrollViewer2.BeginAnimation(AniScrollViewer.CurrentVerticalOffsetProperty, _scrollEndAnim)
            _isScroll = True
            DownScrollButton.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private _mouseDate As Date
    Private _fwdMessages As String
    Public SendControl As SendControl
    Public IsNotScroll As Boolean
    Public Property ScrollBar1 As ScrollBar
    Public Property ScrollViewer1 As AniScrollViewer

    Private Sub UIElement_OnPreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        _mouseDate = Now
    End Sub

    Private Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        ScrollToEnd(scrollBar1, scrollViewer1)
        _isOffsetLoaded = False
    End Sub

    Private Sub ListBox1_OnInitialized(sender As Object, e As EventArgs)
#If Not XP Then
        VirtualizingPanel.SetScrollUnit(ListBox1, ScrollUnit.Pixel)
#End If
    End Sub

    Private Sub CopyMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        CopyMessages()
    End Sub

    Private Sub ListBox1_OnKeyDown(sender As Object, e As KeyEventArgs)
        If Keyboard.Modifiers.HasFlag(ModifierKeys.Control)
            If e.Key = Key.C Then
                CopyMessages()
            ElseIf e.Key = Key.R Then
                ReplyMessagesToActiveDialog()
            ElseIf e.Key = Key.F Then
                ReplyMessages()
            ElseIf e.Key = Key.D then
                MarkAsImportant()
            ElseIf ReSendMenuItem.Visibility = Visibility.Visible AndAlso e.Key = Key.S Then
                ReSend()
            End If
        End If
    End Sub

    Private Sub CopyMessages()
        Dim sb As New StringBuilder
        For Each m As types.message In ListBox1.SelectedItems
            sb.AppendFormat("[{0}] {1}: {2}",
                            New DateConvert().Convert(m.date, Nothing, Nothing, Nothing),
                            If _
                               (Lists.ProfilesDictionary.ContainsKey(m.user_id),
                                Lists.ProfilesDictionary(m.user_id).full_name,
                                My.Resources.unknown),
                            m.body)
            sb.AppendLine()
        Next
        Clipboard.Clear()
        Clipboard.SetData(DataFormats.UnicodeText, sb.ToString())
    End Sub

    Private Sub ReplyMessages()
        Dim forward_messages As New StringBuilder
        For Each m As types.message In ListBox1.SelectedItems
            forward_messages.Append(m.id & ",")
        Next
        OtherApi.MyWindow1?.ShowFlyout(Position.Left,
                                       "Content/ContentSearch.xaml#mode=forward_messages&selectIndex=1&forward_messages=" &
                                       forward_messages.ToString,
                                       "")
    End Sub

    Private Async Sub ReSend()
        Dim m = TryCast(ListBox1.SelectedItem, types.message)
        If m IsNot Nothing AndAlso m.IsError AndAlso m.Tag IsNot Nothing Then
            Dim fragment = m.Tag.ToString()
            Await _
                Messages.Send(fragment.GetParametr("params"),
                              m.body,
                              fragment.GetParametr("attachment"),
                              fragment.GetParametr("forward_messages"))
            Dim key = If(chat_id.IsNullOrEmpty, user_id.ToString(), "c" & chat_id)
            If Messages.MessagesDictionary.ContainsKey(key) Then
                Messages.MessagesDictionary(key).items.Remove(m)
            End If
        End If
    End Sub

    Private Sub ReplyMessagesToActiveDialog()
        Dim forward_messages As New StringBuilder
        For Each m As types.message In ListBox1.SelectedItems
            forward_messages.Append(m.id & ",")
        Next
        _fwdMessages = forward_messages.ToString()
        If SendControl IsNot Nothing Then
            SendControl.FwdMessages = _fwdMessages
        Else
            OtherApi.SendControl.FwdMessages = _fwdMessages
        End If
    End Sub

    Private Sub ReplyMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        ReplyMessagesToActiveDialog()
    End Sub

    Private Sub ReSendMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        ReSend()
    End Sub

    Private Sub ContextMenu_OnOpened(sender As Object, e As RoutedEventArgs)
        Dim m = TryCast(ListBox1.SelectedItem, types.message)
        if m is Nothing then exit sub
        MarkAsImportantMenuItem.IsChecked = m.important
        If m.IsError Then
            ReSendMenuItem.Visibility = Visibility.Visible
        Else
            ReSendMenuItem.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub DeleteMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        DeleteMessages()
    End Sub

    Sub DeleteMessages()
    End Sub

    Private Sub MarkAsImportantMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        MarkAsImportant
    End Sub

    Private Async Sub MarkAsImportant()
        dim importantMsgs as New StringBuilder
        dim msgs as New StringBuilder
        Dim msgs1 = ListBox1.SelectedItems.Cast (Of types.message).ToArray()
        For Each selectedItem As types.message In ListBox1.SelectedItems
            if selectedItem.important
                importantMsgs.Append(selectedItem.id & ",")
            else
                msgs.Append(selectedItem.id & ",")
            End If
        Next
        if importantMsgs.Length > 0
            dim ids = await Messages.MarkAsImportant(importantMsgs.ToString(), False)
            For Each message In msgs1
                if ids IsNot Nothing
                    if ids.Contains(message.id)
                        message.important = false
                    End If
                End If
            Next
        End If
        if Msgs.Length > 0
            dim ids = await Messages.MarkAsImportant(msgs.ToString(), true)
            For Each message In msgs1
                if ids IsNot Nothing
                    if ids.Contains(message.id)
                        message.important = True
                    End If
                End If
            Next
        End If
    End Sub
End Class