
Imports System.Globalization
Imports System.Windows.Interactivity
Imports FirstFloor.ModernUI.Windows
Imports FirstFloor.ModernUI.Windows.Controls
Imports FirstFloor.ModernUI.Windows.Media
Imports MahApps.Metro.Controls
Imports MicroVK.Api
Imports MicroVK.Command
Imports MicroVK.Converter

Class PageMessage
    Inherits UserControl
    Implements IContent

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        ListBox1.ItemsSource = Messages.DialogsList1
        'If IsNothing(DockManager1) Then
        '    DockManager1 = New Xceed.Wpf.AvalonDock.DockingManager With {.Theme = New GenericTheme()}
        '    AddHandler DockManager1.DocumentClosed, AddressOf DockingManager1_DocumentClosed
        'End If
        'ContentControl1.Content = DockManager1
        OtherApi.PageMessageRun1 = Run1
        OtherApi.PageMessageRun2 = Run2
        OtherApi.PageMessageRun3 = Run3
        OtherApi.PageMessageRun1.Text =
            If(Messages.DialogsList1.Count > 0, Messages.DialogsList1(0).message.id.ToString(), "0")
        Messages.DialogCountRun1 = Run4
        OtherApi.DialogsTabControl = MessagesTabControl1
        OtherApi.SendControl = SendControl1
    End Sub

    Private Async Sub ListBox1_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        Dim p = CType(e.OriginalSource, ScrollViewer)
        p.ApplyTemplate()
        Dim a = p.Template.FindName("PART_VerticalScrollBar", p)
        Dim b As Primitives.ScrollBar = CType(a, Primitives.ScrollBar)
        If _
            Math.Abs(b.Value - b.Maximum) < OtherApi.TOLERANCE And Messages.DialogsList1.Count <> 0 And
            Messages.DCount > Messages.DialogsList1.Count Then
            Await Messages.GetDialogs(Messages.DialogsList1.Count.ToString())
        End If
        If My.Settings.runMinimized AndAlso Not OtherApi.RunMinimizedActive Then
            OtherApi.MyWindow1.Visibility = If(My.Settings.InTaskbar, Visibility.Visible, Visibility.Hidden)

        End If
        OtherApi.RunMinimizedActive = True
    End Sub

    Async Sub CreateLayoutDocumentDialog(user_id As Integer,
                                         Optional chat_id As Integer = 0,
                                         Optional forward_messages As String = "",
                                         Optional message As types.message = Nothing)
        Dim key = If(chat_id = 0, user_id.ToString, "c" & chat_id)
        Dim isChat = chat_id > 0
        Dim uri = New Uri(
            String.Format("\Content\ControlDialogListBox.xaml#{0}&forward_messages={1}",
                          If(isChat, "chat_id=" & chat_id, "user_id=" & user_id),
                          forward_messages),
            UriKind.RelativeOrAbsolute)
        Dim title As String
        Dim fullTitle As String
        Dim description As String = If(isChat, "chat_id=" & chat_id, "user_id=" & user_id)
        If Not Messages.MessagesDictionary.ContainsKey(key) Then
            Dim a As types.profile = Nothing
            Dim chat As types.chat

            If Not isChat Then
                If Not Lists.ProfilesDictionary.ContainsKey(user_id) Then
                    Dim a1 = (Await Users.Get(user_id.ToString))
                    If a1 Is Nothing Then
                        Exit Sub
                    End If
                    a = a1.FirstOrDefault
                Else
                    a = Lists.ProfilesDictionary(CInt(user_id))
                End If
                title = a?.first_name()
                fullTitle = a?.first_name()
            Else

                If message Is Nothing Then
                    chat = Await Messages.GetChat(chat_id.ToString())
                    If chat Is Nothing Then Exit Sub
                Else
                    chat = New types.chat() With {.title = message.title}
                End If
                title = chat?.title
                fullTitle = title
            End If

            Dim ld = New MyTabItem With {.Title = title,
                    .ToolTip = fullTitle,
                    .Description = description,
                    .IsSelected = True,
                    .Photo = If(isChat, "", a.photo),
                    .ForwardMessages = forward_messages,
                    .Frame =
                    New ModernFrame() With {
                    .Source = uri},
                    .RawTitle = title,
                    .Content = uri}
            If Not Messages.MessagesDictionary.ContainsKey(key) Then
                Messages.MessagesDictionary.Add(key, New types.vk_list With {.DialogTabItem = ld})
                MessagesTabControl1.Items.Add(ld)
                MessagesTabControl1.SelectedIndex = MessagesTabControl1.Items.Count - 1
            End If
        Else
            For i = 0 To MessagesTabControl1.Items.Count - 1
                Dim item = TryCast(MessagesTabControl1.Items(i), MyTabItem)
                If item.Description = description Then
                    MessagesTabControl1.SelectedIndex = i
                    item.ForwardMessages = forward_messages
                    item.Frame.Source = uri
                    Exit For
                End If
            Next
        End If
        SendControl1.FwdMessages = forward_messages
        If MessagesTabControl1.Items.Count > 10 AndAlso My.Settings.hideTabs Then
            Dim c = New CloseDialogTab()
            c.Execute(MessagesTabControl1.Items(0))
        End If
    End Sub

    Public Sub OnFragmentNavigation(e As Navigation.FragmentNavigationEventArgs) _
        Implements IContent.OnFragmentNavigation

        Dim user_id = e.Fragment.GetParametr("user_id").ToInt()
        Dim chat_id = e.Fragment.GetParametr("chat_id").ToInt()
        Dim forward_messages = e.Fragment.GetParametr("forward_messages")
        CreateLayoutDocumentDialog(user_id, chat_id, forward_messages)
    End Sub

    Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedFrom
    End Sub

    Public Async Sub OnNavigatedTo(e As Navigation.NavigationEventArgs) Implements IContent.OnNavigatedTo
        If Messages.DialogsList1.Count = 0 Then
            If Await Messages.GetDialogs("", "", "true") Then
                ListBox1.ItemsSource = Messages.DialogsList1
                OtherApi.PageMessageRun1.Text =
                    If(Messages.DialogsList1.Count > 0, Messages.DialogsList1(0).message.id.ToString(), "0")
                ListBox1.SelectedIndex = 0
                Dim msg = CType(ListBox1.SelectedItem, types.dialog).message
                If ListBox1.SelectedIndex >= 0 Then
                    CreateLayoutDocumentDialog(msg.user_id, msg.chat_id, "", msg)
                End If
            End If

        End If
        Await NetHelper.Delay(100)
        Keyboard.Focus(SendControl1.TextBox1)
    End Sub

    Public Sub OnNavigatingFrom(e As Navigation.NavigatingCancelEventArgs) Implements IContent.OnNavigatingFrom
        If e.Source.OriginalString.Contains("ContentFavoritesMessage") Then
            OtherApi.MyWindow1.ModernFrame1.Source = Nothing
            OtherApi.MyWindow1.ShowFlyout(Position.Left, "Content/ContentFavoritesMessage.xaml", My.Resources.Important)
            e.Cancel = True
        End If
    End Sub

    Private Sub ListBox1_OnInitialized(sender As Object, e As EventArgs)
        OtherApi.DialogListBox1 = ListBox1
#If Not XP Then
        VirtualizingPanel.SetScrollUnit(ListBox1, ScrollUnit.Pixel)
#End If
    End Sub

    Private Sub ListBox1_OnPreviewMouseUp(sender As Object, e As MouseButtonEventArgs)
        If ListBox1.SelectedItem Is Nothing Then Exit Sub
        Dim msg = TryCast(ListBox1.SelectedItem, types.dialog)?.message
        If msg IsNot Nothing Then
            CreateLayoutDocumentDialog(msg.user_id, msg.chat_id, "", msg)
        End If
    End Sub

    Private Sub DialogDeleteMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        DeleteDialog()
    End Sub

    Sub DeleteDialog()
        Dim d = TryCast(ListBox1.SelectedItem, types.dialog)
        If d Is Nothing OrElse d.message Is Nothing Then Exit Sub
        OtherApi.MyWindow1.ShowFlyout(Position.Right,
                                      String.Format(
                                          "Content/Flyouts/ContentDeleteDialog.xaml#chat_id={0}&user_id={1}&title={2}",
                                          d.message.chat_id,
                                          d.message.user_id,
                                          New DialogGetName().Convert(d.message,
                                                                      GetType(String),
                                                                      Nothing,
                                                                      CultureInfo.CurrentCulture)),
                                      My.Resources.dialogDelete)
    End Sub

    Private Sub ListBox1_OnKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Delete Then
            DeleteDialog()
        End If
    End Sub

    Private Sub OptionButton_OnClick(sender As Object, e As RoutedEventArgs)
        OptionButton.ContextMenu.IsOpen = Not OptionButton.ContextMenu.IsOpen
    End Sub

    Private Sub HideTabsMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        If MessagesTabControl1.Items.Count > 0 AndAlso Not My.Settings.hideTabs Then
            For i As Integer = MessagesTabControl1.Items.Count - 1 To 0 Step -1
                If i <> MessagesTabControl1.SelectedIndex Then
                    Dim c = New CloseDialogTab()
                    c.Execute(MessagesTabControl1.Items(i))
                End If
            Next
        End If
    End Sub

    Private Sub UnpinMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        UnpinCurrentTab()
    End Sub

    Private Sub DialogStyleMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ShowFlyout(Position.Left,
                                      "Content/Flyouts/DialogStyleSettings.xaml",
                                      My.Resources.DialogStyle)
    End Sub

    Private Sub OpenInBrowserMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        Dim tab = TryCast(MessagesTabControl1.SelectedItem, MyTabItem)
        If tab Is Nothing Then Exit Sub
        Dim user_id = tab.Description.GetParametr("user_id")
        Dim chat_id = tab.Description.GetParametr("chat_id")
        OtherApi.ProcessStart(String.Format("https://vk.com/im?sel={0}",
                                            If _
                                               (Not chat_id.IsNullOrEmpty(),
                                                "c" + chat_id,
                                                If _
                                                   (user_id.Length = 10,
                                                    "-" & user_id.TrimStart("1"c).TrimStart("0"c),
                                                    user_id))))
    End Sub

    Private Sub Refresh()
        Dim tab = TryCast(MessagesTabControl1.SelectedItem, MyTabItem)
        If tab Is Nothing Then Exit Sub
        Dim user_id = tab.Description.GetParametr("user_id")
        Dim chat_id = tab.Description.GetParametr("chat_id")
        Call New CloseDialogTab().Execute(tab)
        CreateLayoutDocumentDialog(user_id.ToInt(), chat_id.ToInt())
    End Sub

    Private Sub PageMessage_OnKeyDown(sender As Object, e As KeyEventArgs)
    End Sub

    Private Sub UnpinCurrentTab()
        Dim c = New OpenFloatWindows
        c.Execute(MessagesTabControl1.SelectedItem)
    End Sub

    Private Sub RefreshMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        Refresh()
    End Sub

    Private Async Sub MessagesTabControl1_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim myTab = TryCast(MessagesTabControl1.SelectedItem, MyTabItem)
        If myTab Is Nothing Then Exit Sub
        If TypeOf e.OriginalSource Is TabControl Then
            If e.RemovedItems.Count > 0 Then
                Dim oldMyTab = TryCast(e.RemovedItems(0), MyTabItem)
                If oldMyTab IsNot Nothing Then
                    oldMyTab.FlowDocument = SendControl1.TextBox1.Document
                    oldMyTab.CaretPosition = SendControl1.TextBox1.CaretPosition
                    oldMyTab.ForwardMessages = SendControl1.FwdMessages
                    SendControl1.TextBox1.Document = New FlowDocument()
                End If
            End If
            SendControl1.RawKey = myTab.Description
            SendControl1.DialogTab = myTab
            SendControl1.FwdMessages = myTab.ForwardMessages
            If myTab.FlowDocument IsNot Nothing Then
                SendControl1.TextBox1.Document = myTab.FlowDocument
                SendControl1.TextBox1.CaretPosition = myTab.CaretPosition
            End If
            Await NetHelper.Delay(100)
            Keyboard.Focus(SendControl1.TextBox1)
            SendControl1.DialogListBox = TryCast(myTab.Frame.Content, ControlDialogListBox)?.ListBox1
        End If
    End Sub

    Private Sub TabBottomMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        My.Settings.hideTabs = True
        My.Settings.hideTabs = False
    End Sub

    Private Sub PageMessage_OnPreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.F5 Then
            Refresh()
        ElseIf Keyboard.Modifiers.HasFlag(ModifierKeys.Control) And e.Key = Key.U Then
            UnpinCurrentTab()
        End If
    End Sub
End Class
