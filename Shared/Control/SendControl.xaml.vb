' Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Windows.Controls.Primitives
Imports JetBrains.Annotations
Imports MahApps.Metro.Controls
Imports Microsoft.Win32
Imports MicroVK.Api
Imports MicroVK.CSharp
Imports MicroVK.OtherLib
Imports MicroVK.WPFGrowlNotification
Imports Yandex.Metrica

Public NotInheritable Class SendControl
    Inherits UserControl

    Enum SendControlType
        Messages
        Wall
    End Enum

    Class ImagePath
        Property Path As String
        Property Status As Boolean
        Property Response As types.ResponseUploadServer
    End Class

    Public IsSmall As Boolean
    Public Property Type As SendControlType = SendControlType.Messages
    Public Property UserId As String
    Public Property ChatId As String

    Public Property RawKey As String
        Get
            Return If(ChatId.IsNullOrEmpty, "user_id=" & UserId, "chat_id=" & ChatId)
        End Get
        Set
            UserId = Value.GetParametr("user_id")
            ChatId = Value.GetParametr("chat_id")
        End Set
    End Property

    Public WallId As String

    Public Event OnSending()
    Public Event OnSend()
    Private _attachments As List(Of String)
    Private _fwdMessages1 As String = ""

    Public Property FwdMessages As String
        Get

            Return _fwdMessages1
        End Get
        Set(value As String)
            _fwdMessages1 = value
            If IsNothing(FwdTextBlock) Then Exit Property
            FwdTextBlock.Visibility = If(value.Length > 0, Visibility.Visible, Visibility.Collapsed)
            Run1.Text = value.Split({","}, StringSplitOptions.RemoveEmptyEntries).Length.ToString()
        End Set
    End Property

    Private Sub FwdButton_Click(sender As Object, e As RoutedEventArgs)
        FwdMessages = ""
        If DialogTab IsNot Nothing Then
            DialogTab.ForwardMessages = ""
        End If
    End Sub

    Private Sub TextBox1_Pasting(sender As Object, e As DataObjectPastingEventArgs)
        Select Case e.FormatToApply
            Case "Bitmap"
                Dim file = Path.GetTempFileName() & ".jpg"
                Dim image = Clipboard.GetImage()
                Using fileStream As New FileStream(file, FileMode.Create)
                    Dim encoder = New JpegBitmapEncoder
                    encoder.Frames.Add(BitmapFrame.Create(image))
                    encoder.Save(fileStream)
                End Using
                If ListBox1.Items.Count >= 10 Then
                    OtherApi.ShowMicroVkNot(My.Resources.AttachmentError1, My.Resources.AttachmentError2)
                Else
                    Dim b = New ImagePath With {.Path = file}
                    ListBox1.Items.Add(b)
                End If
                e.Handled = True
                Clipboard.Clear()
                ProgressBar1.Visibility = Visibility.Visible
                ProgressBar2.Visibility = Visibility.Visible
                ListBox1.Visibility = Visibility.Visible
                Button1.Visibility = If(ListBox1.Items.Count > 0, Visibility.Visible, Visibility.Collapsed)
                UploadImage()
            Case "Rich Text Format", "Text", "UnicodeText"
                e.FormatToApply = "UnicodeText"
            Case Else
                Dim formats = Clipboard.GetDataObject.GetFormats
                If formats.Contains("UnicodeText") Then
                    e.FormatToApply = "UnicodeText"
                End If
        End Select
    End Sub

    Private Sub TextBox1_GotFocus(sender As Object, e As RoutedEventArgs)
        If DialogListBox IsNot Nothing Then
            DialogListBox.SelectedIndex = - 1
        End If
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim a = New OpenFileDialog _
                With {.Filter = My.Resources.Picture_file & "|*.jpeg;*.jpg;*.jpeg;*.jpe;*.png;*.gif;*.bmp",
                .Multiselect = True}
        If a.ShowDialog() Then
            AddAttachmentFromPath(a.FileNames)
        End If
        Button1.Visibility = If(ListBox1.Items.Count > 0, Visibility.Visible, Visibility.Collapsed)
    End Sub

    Public Sub AddAttachmentFromPath(paths As String())
        ProgressBar1.Visibility = Visibility.Visible
        ProgressBar2.Visibility = Visibility.Visible
        ListBox1.Visibility = Visibility.Visible
        For Each i In paths
            If ListBox1.Items.Count >= 10 Then
                OtherApi.ShowMicroVkNot(My.Resources.AttachmentError1, My.Resources.AttachmentError2)
                Exit For
            Else
                Dim b = New ImagePath With {.Path = i}
                ListBox1.Items.Add(b)
            End If
        Next
        UploadImage()
    End Sub

    Public Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        ListBox1.Items.Clear()
        _attachments?.Clear()
        Button1.Visibility = If(ListBox1.Items.Count > 0, Visibility.Visible, Visibility.Collapsed)
        ProgressBar1.Visibility = Visibility.Collapsed
        ProgressBar2.Visibility = Visibility.Collapsed
    End Sub

    Private Sub TextBox1_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If My.Settings.MessageSendType1 Then
            If e.Key = Key.Enter Then
                If Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) Then

                Else
                    SendMessages()
                    e.Handled = True
                End If
            End If
        ElseIf My.Settings.MessageSendType2 Then
            If e.Key = Key.Enter Then
                If Keyboard.Modifiers.HasFlag(ModifierKeys.Control) Then
                    SendMessages()
                Else
                    TextBox1.CaretPosition = TextBox1.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward)
                    TextBox1.CaretPosition.InsertTextInRun(vbCrLf)
                End If
                e.Handled = True
                TextBox1.Focus()
            End If
        End If
        If OtherApi.SmilePopup1 IsNot Nothing AndAlso OtherApi.SmilePopup1.IsOpen Then
            OtherApi.SmilePopup1.IsOpen = False
        End If
        If Type = SendControlType.Messages AndAlso My.Settings.IsSetActivity Then
            Messages.SetActivity(UserId, ChatId)
        End If
    End Sub

    Private Async Sub toggleButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(OtherApi.SmilePopup1) Then
            OtherApi.SmilePopup1 = CType(FindResource("SmilePopup1"), Popup)
            Dim tb = CType(CType(OtherApi.SmilePopup1.Child, Border).Child, TabControl)
            AddHandler OtherApi.SmilePopup1.Closed, AddressOf SettingSystem.SmilePopup1_Closed
            Await SettingSystem.UpdateRecentSmiles()

            If SettingSystem.RecentSmiles.Count = 0 Then
                tb.SelectedIndex = 1
            Else
            End If
            SettingSystem.RecentSmilesProducts = New types.Product With {.type = "RecentSmiles"}
            tb.Items.Add(SettingSystem.RecentSmilesProducts)
            tb.Items.Add(New types.Product With {.type = "MicroVKSmiles"})
            Dim p = Await Store.GetProducts
            If Not IsNothing(p) Then
                For Each i In p
                    tb.Items.Add(i)
                Next
            End If

        End If
        If ToggleButton1.Tag IsNot Nothing AndAlso ToggleButton1.Tag.ToString = "float" Then
            OtherApi.SmilePopup1.VerticalOffset = 16
            OtherApi.SmilePopup1.Placement = PlacementMode.Bottom
            OtherApi.SmilePopup1.VerticalOffset = 8
            OtherApi.SmilePopup1.PlacementTarget = TextBox1
        Else
            OtherApi.SmilePopup1.Placement = PlacementMode.Top
            OtherApi.SmilePopup1.PlacementTarget = ToggleButton1
        End If

        OtherApi.SmilePopup1.Tag = Me
        OtherApi.SmilePopup1.IsOpen = Not OtherApi.SmilePopup1.IsOpen
    End Sub

    <UsedImplicitly>
    Private Sub ButtonBase_OnClick(sender As Object, e As RoutedEventArgs)
        If False Then
            Dim g As String() = CType(Me.FindResource("Smile1"), String())
            Dim t = ""
            For i = 518 To g.Count - 1
                Dim tp = TextBox1.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward)
                Dim iuc = New InlineUIContainer(New Image With {
                                                   .Source =
                                                   New BitmapImage(
                                                       New Uri(
                                                           String.Format(
                                                               "pack://application:,,,/MicroVK.Smiles;component/Smile/{0}.png",
                                                               g(i)))),
                                                   .Stretch = Stretch.None,
                                                   .Margin = New Thickness(1, 0, 0, 1),
                                                   .Tag = 45},
                                                TextBox1.CaretPosition)
                TextBox1.CaretPosition = tp
            Next
        End If
        SendMessages()
    End Sub

    Public Function GetText() As String
        Dim temp As String = ""
        Try
            For Each i As Paragraph In TextBox1.Document.Blocks
                For n = 0 To i.Inlines.Count - 1
                    Dim j = i.Inlines(n)
                    If TypeOf j Is LineBreak Then
                        temp += vbNewLine
                    ElseIf TypeOf j Is Run Then
                        Dim u = CType(j, Run)
                        temp += u.Text
                    ElseIf TypeOf j Is InlineUIContainer Then
                        Dim u = CType(j, InlineUIContainer)
                        If TypeOf u.Child Is Image Then
                            Dim t =
                                    CType(u.Child, Image).Source.ToString.Split("/"c).LastOrDefault.Split("."c).
                                    FirstOrDefault

                            temp += OtherApi.GetVkSmile(t)
                        End If
                    End If
                Next
            Next
        Catch ex As Exception

        End Try
        Return temp
    End Function

    Public LastText As String
    Public DialogTab As MyTabItem
    Public DialogListBox As ListBox

    Public Async Sub SendMessages()
        Dim a As String = GetText()
        LastText = a
        If _attachments Is Nothing Then _attachments = New List(Of String)()
        If a.Length = 0 And _attachments.Count = 0 And FwdMessages.Length = 0 Then
            Exit Sub
        End If
        TextBox1.Document.Blocks.Clear()

        Dim attachmentsString = ""
        If _attachments.Any() Then attachmentsString = _attachments.Aggregate(Function(s, s1) s & "," & s1)
        If Type = SendControlType.Messages Then
            If _
                UserId.Length > 0 AndAlso Lists.ProfilesDictionary.ContainsKey(CInt(UserId)) AndAlso
                Lists.ProfilesDictionary(CInt(UserId)).deactivated = "deleted" Then
                OtherApi.ShowMicroVkNot(My.Resources.messageError1, My.Resources.user_deleted)
            Else
                If OnSendingEvent IsNot Nothing Then RaiseEvent OnSending()
                Dim message_id = Await Messages.Send(RawKey, a, attachmentsString, FwdMessages)
                If message_id > 0 Then
                    If OnSendEvent IsNot Nothing Then
                        RaiseEvent OnSend()
                    End If
                End If
            End If
        Else
            If OnSendingEvent IsNot Nothing Then RaiseEvent OnSending()
            Await Api.Wall.Post(a, attachmentsString)
            If OnSendEvent IsNot Nothing Then
                RaiseEvent OnSend()
            End If
        End If

        FwdMessages = ""
        ListBox1.Items.Clear()
        _attachments.Clear()
        Button1.Visibility = If(ListBox1.Items.Count > 0, Visibility.Visible, Visibility.Collapsed)
        ProgressBar1.Visibility = Visibility.Collapsed
        ProgressBar2.Visibility = Visibility.Collapsed
    End Sub

    Public Sub RaiseEventOnSend()
        If OnSendEvent IsNot Nothing Then
            RaiseEvent OnSend()
        End If
    End Sub

    Public Async Sub UploadImage()
        Dim a As New WebClient

        If IsNothing(_attachments) Then _attachments = New List(Of String)()
        For Each i As ImagePath In ListBox1.Items
            If Not i.Status Then
                Dim upload_url As String = ""
                If Type = SendControlType.Messages Then
                    Dim b = Await Photos.GetMessagesUploadServer
                    upload_url = b.upload_url
                ElseIf Type = SendControlType.Wall Then
                    Dim b = Await Photos.GetWallUploadServer()
                    upload_url = b.upload_url
                End If


                ListBox1.Tag = i
                ProgressBar1.Visibility = Visibility.Visible
                ProgressBar2.Visibility = Visibility.Visible
                AddHandler a.UploadFileCompleted, AddressOf a_UploadFileCompleted
                AddHandler a.UploadProgressChanged, AddressOf a_UploadProgressChanged
                a.UploadFileAsync(New Uri(upload_url), i.Path)
                Exit For
            End If
        Next
        Dim s = ListBox1.Items.Cast (Of ImagePath)().Count(Function(i) i.Status)
        If ListBox1.Items.Count > 0 Then ProgressBar1.Value = ProgressBar1.Maximum*(s/ListBox1.Items.Count)
    End Sub

    Private Async Sub a_UploadFileCompleted(sender As Object, e As UploadFileCompletedEventArgs)
        If IsNothing(e.Error) Then


            Dim response =
                    Await _
                    MyJsonConvert.DeserializeObjectAsync (Of types.ResponseUploadServer)(
                        Encoding.UTF8.GetString(e.Result))
            CType(ListBox1.Tag, ImagePath).Status = True
            CType(ListBox1.Tag, ImagePath).Response = response

            Dim attachmentString = ""
            Dim photo As types.photo = Nothing
            If Type = SendControlType.Messages Then
                photo = (Await Photos.SaveMessagesPhoto(response))?.FirstOrDefault()
            ElseIf Type = SendControlType.Wall Then
                photo = (Await Photos.SaveWallPhoto(response, ""))?.FirstOrDefault()
            End If
            If photo IsNot Nothing Then attachmentString = "photo" & photo.owner_id & "_" & photo.id
            If attachmentString.Length > 0 AndAlso ListBox1.Items.Count > 0 Then _attachments.Add(attachmentString)

            ProgressBar1.Visibility = Visibility.Collapsed
            ProgressBar2.Visibility = Visibility.Collapsed
            UploadImage()
        Else
            OtherApi.ShowMicroVkNot("MicroVK", My.Resources.ErrorUploadFile & e.Error.Message)
        End If
    End Sub

    Private Sub a_UploadProgressChanged(sender As Object, e As UploadProgressChangedEventArgs)
        If e.ProgressPercentage <= 100 Then ProgressBar2.Value = e.ProgressPercentage
    End Sub

    Private Async Sub SendControl_OnInitialized(sender As Object, e As EventArgs)
        DataObject.RemovePastingHandler(TextBox1, AddressOf TextBox1_Pasting)
        DataObject.AddPastingHandler(TextBox1, AddressOf TextBox1_Pasting)
        Await SettingSystem.UpdateRecentSmiles()
        SmileListBox.ItemsSource = SettingSystem.RecentSmiles
    End Sub

    Private Sub SendButton1_OnClick(sender As Object, e As RoutedEventArgs)
        SendMessages()
    End Sub

    Private Sub SendSettingButton_OnClick(sender As Object, e As RoutedEventArgs)
        SendSettingButton.ContextMenu.IsOpen = Not SendSettingButton.ContextMenu.IsOpen
    End Sub


    Private Sub EnterSendMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        My.Settings.MessageSendType1 = True
        My.Settings.MessageSendType2 = False
    End Sub

    Private Sub CtrlSendMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        My.Settings.MessageSendType1 = False
        My.Settings.MessageSendType2 = True
    End Sub

    Private Sub DialogStyleMenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        OtherApi.MyWindow1.ShowFlyout(Position.Left,
                                      "Content/Flyouts/DialogStyleSettings.xaml",
                                      My.Resources.DialogStyle)
    End Sub

    Private Sub SendControl_OnLoaded(sender As Object, e As RoutedEventArgs)
        SetSpellCheck()
    End Sub

    Private Sub SetSpellCheck()
        Try
            SpellCheck.SetIsEnabled(TextBox1, My.Settings.SpellCheck)
        Catch ex As Exception
            If My.Settings.SpellCheck Then
                OtherApi.GrowlNotifiactions1.AddNotification(New Notification With {
                                                                .Content =
                                                                New ControllNotificationBBCode With {
                                                                .DataContext =
                                                                New types.Notifycation With {.title = "MicroVK",
                                                                .text = My.Resources.SpellCheckError}}})
                My.Settings.SpellCheck = False
#If NET46 Then
                YandexMetrica.ReportError(ex.Message, ex)
#End If
            End If
        End Try
    End Sub

    Private Sub MenuItem_OnClick(sender As Object, e As RoutedEventArgs)
        SetSpellCheck()
    End Sub

    Private Sub TextBox1_OnMouseEnter(sender As Object, e As MouseEventArgs)
        If My.Settings.MarkMessageInvisible Then MarkMessage()
    End Sub

    Sub MarkMessage()
        Dim key = If(ChatId.IsNullOrEmpty(), UserId, "c" & ChatId)
        If key Is Nothing Then Exit Sub
        If Messages.MessagesDictionary.ContainsKey(key) AndAlso Messages.MessagesDictionary(key).items IsNot Nothing _
            Then
            Messages.MessagesDictionary(key).unread = 0
            Dim unread_id = 0
            For i = Messages.MessagesDictionary(key).items.Count - 1 To 0 Step - 1
                Dim msg = Messages.MessagesDictionary(key).items(i)
                If Not msg.read_state And Not msg.out Then
                    unread_id = msg.id
                Else
                    Exit For
                End If
            Next

            If unread_id > 0 Then
                Messages.MarkAsRead(RawKey, unread_id.ToString())
            End If
        End If
    End Sub

    Public Sub AddSmile(ByVal smile As String, ByVal smile_option As String, Optional ByVal notSave As Boolean = False)
        If smile_option.IsNullOrEmpty() Then
            TextBox1.BeginChange()
            Dim tp = TextBox1.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward)
            Try
                Dim iuc = New InlineUIContainer(New Image With {
                                                   .Source =
                                                   New BitmapImage(
                                                       New Uri(
                                                           String.Format(
                                                               "pack://application:,,,/MicroVK.Smiles;component/Smile/{0}.png",
                                                               smile))),
                                                   .Stretch = Stretch.None,
                                                   .Margin = New Thickness(1, 0, 0, 1),
                                                   .Tag = 45},
                                                TextBox1.CaretPosition)
            Catch
            End Try
            If Not notSave Then
                If SettingSystem.RecentSmiles.Count < 2 Then
                    SmileListBox.ItemsSource = SettingSystem.RecentSmiles
                    SmileListBox.Items.Refresh()
                End If

                If SettingSystem.RecentSmiles.IndexOf(smile) >= 0 Then
                    SettingSystem.RecentSmiles.Remove(smile)
                End If
                SettingSystem.RecentSmiles.Insert(0, smile)
                Debug.Print(smile)
                If SettingSystem.RecentSmiles.Count > 75 Then
                    SettingSystem.RecentSmiles.RemoveAt(SettingSystem.RecentSmiles.Count - 1)
                End If
                If SettingSystem.RecentSmilesProducts.ListBox1 IsNot Nothing Then
                    SettingSystem.RecentSmilesProducts.ListBox1.ItemsSource = SettingSystem.RecentSmiles
                    SettingSystem.RecentSmilesProducts.ListBox1.Items.Refresh()
                End If
            End If

            TextBox1.CaretPosition = tp
            TextBox1.EndChange()
        Else
            Messages.SendSticker(RawKey, smile)
            RaiseEventOnSend()
            OtherApi.SmilePopup1.IsOpen = False
        End If
        Keyboard.Focus(TextBox1)
    End Sub

    Private Sub SmileListBox_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If SmileListBox.SelectedIndex >= 0 Then
            AddSmile(SmileListBox.SelectedItem?.ToString, "", True)
            SmileListBox.SelectedIndex = - 1
        End If
    End Sub

    Private Sub SmileListBox_OnLoaded(sender As Object, e As RoutedEventArgs)
    End Sub
End Class

Class ImagePath
    Property Path As String
    Property Status As Boolean
    Property Response As types.ResponseUploadServer
End Class


