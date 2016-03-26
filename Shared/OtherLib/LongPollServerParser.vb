Imports System.Net
Imports System.Text
Imports System.Windows.Threading
Imports MicroVK.Api

Class LongPollServerParser
    Public Shared MaxMsgId As Integer
    Public Shared LongPollInfo As types.LongPollServer
    Private Shared ReadOnly _connectRawString As String = "http://{0}?act=a_check&key={1}&ts={2}&wait=25&mode=66"
    Private Shared _connectString As String
    Private Shared _mTimer As DispatcherTimer
    Private Shared _mWebClient As New WebClient With {.Encoding = Encoding.UTF8}

    Shared Sub LongPollServerStart()
        _lastResponseTime = Now
        If _mTimer Is Nothing Then
            _mTimer = New DispatcherTimer()
            _mTimer.Interval = New TimeSpan(0, 0, 22)
            m_timer_Tick(Nothing, Nothing)
            AddHandler _mTimer.Tick, AddressOf m_timer_Tick
            _mTimer.Start()
        End If
    End Sub

    Private Shared Sub LongPollWebClient1_DownloadStringCompleted(sender As Object,
                                                                  e As DownloadStringCompletedEventArgs)
        Dim d = True
#If DEBUG And False Then
        d = Now.Minute Mod 2 = 0
#End If

        If (IsNothing(e.Error)) AndAlso Not e.Cancelled AndAlso d Then
            _lastResponseTime = Now
            LongPollServerParse(e.Result.ToString().Trim())
        Else
            TrayIconManager.ChangeNetworkStatus(True, If(e.Error IsNot Nothing, e.Error.Message, ""))

#If DEBUG And False Then
            OtherApi.ShowMicroVkNot("LongPollWebClient1_DownloadStringCompleted", "error")
#End If
        End If
    End Sub

    Private Shared _lastResponseTime As Date

    Private Shared Async Sub m_timer_Tick(sender As Object, e As EventArgs)

        If _mWebClient Is Nothing Then
            _mWebClient = New WebClient()
            AddHandler _mWebClient.DownloadStringCompleted, AddressOf LongPollWebClient1_DownloadStringCompleted
        End If
        If IsNothing(LongPollInfo) Then
            LongPollInfo = Await messages.GetLongPollServer()
            RemoveHandler _mWebClient.DownloadStringCompleted, AddressOf LongPollWebClient1_DownloadStringCompleted
            AddHandler _mWebClient.DownloadStringCompleted, AddressOf LongPollWebClient1_DownloadStringCompleted

        End If
        If LongPollInfo IsNot Nothing Then
            _connectString = String.Format(_connectRawString, LongPollInfo.server, LongPollInfo.key, LongPollInfo.ts)
            If DateDiff(DateInterval.Second, _lastResponseTime, Now) > 60 Then
                _mWebClient.CancelAsync()
            End If
            ForceDownloadStringAsync()
        End If
    End Sub

    Private Shared Async Sub LongPollServerParse(response As String)
        Try
            Dim d = Await MyJsonConvert.DeserializeObjectAsync (Of types.LongPoolServerUpdates)(response)
            Dim user_ids = ""
            Dim group_ids = ""
            Dim msgs As New List(Of types.message)

            LongPollInfo.ts = d.ts
            _connectString = String.Format(_connectRawString, LongPollInfo.server, LongPollInfo.key, LongPollInfo.ts)
            If d.failed > 0 Then
                TrayIconManager.ChangeNetworkStatus(True)
                If d.failed = 1 Then
                    _connectString = String.Format(_connectRawString,
                                                   LongPollInfo.server,
                                                   LongPollInfo.key,
                                                   LongPollInfo.ts)
                    ForceDownloadStringAsync()
                Else
                    LongPollInfo = Await messages.GetLongPollServer()
                    Exit Sub
                End If
            Else
                TrayIconManager.ChangeNetworkStatus(False, "")
            End If
            If Not IsNothing(d.updates) AndAlso d.updates.Count > 0 Then
                Dim updates As New List(Of types.Notifycation)
                For Each i In d.updates
                    Select Case CInt(i(0))
                        Case 0
                            'удаление сообщения с указанным local_id 
                        Case 1
                            'замена флагов сообщения (FLAGS:=$flags)
                        Case 2
                            'установка флагов сообщения (FLAGS|=$mask) 
                        Case 3
                            'сброс флагов сообщения (FLAGS&=~$mask) 
                            Dim flags1 = CType(CInt(i(2)), messages.MessageFlags)
                            If flags1.HasFlag(messages.MessageFlags.Unread) Then
                                Dim id = CInt(i(1))
                                Dim chat_id As Integer
                                Dim user_id As Integer

                                If messages.BadMessagesDictionary.ContainsKey(id) Then
                                    If _
                                        Not _
                                        lists.ProfilesDictionary.ContainsKey(messages.BadMessagesDictionary(id).user_id) _
                                        Then _
                                        user_ids = user_ids & messages.BadMessagesDictionary(id).user_id.ToString & ","
                                    messages.BadMessagesDictionary(id).read_state = True
                                    If _
                                        messages.BadMessagesDictionary(id).out AndAlso OtherApi.MyUser IsNot Nothing AndAlso
                                        (OtherApi.MyUser.id <> messages.BadMessagesDictionary(id).user_id) Then
                                        updates.Add(New types.Notifycation _
                                                       With {.user_id = messages.BadMessagesDictionary(id).user_id,
                                                       .chat_id = messages.BadMessagesDictionary(id).chat_id,
                                                       .text = messages.BadMessagesDictionary(id).body,
                                                       .type = 3})
                                    End If

                                    Dim key = messages.BadMessagesDictionary(id).Key
                                    If messages.BadMessagesDictionary(id).chat_id > 0 Then
                                        chat_id = messages.BadMessagesDictionary(id).chat_id
                                        If messages.MessagesDictionary.ContainsKey(key) Then
                                            messages.MessagesDictionary(key).unread -= 1
                                        End If
                                        If _
                                            messages.DialogsDictionary.ContainsKey(key) And
                                            messages.DialogsDictionary(key).message.read_state = True Then
                                            messages.DialogsDictionary(key).unread = 0
                                        End If
                                    Else
                                        user_id = messages.BadMessagesDictionary(id).user_id
                                        If messages.MessagesDictionary.ContainsKey(key) Then
                                            messages.MessagesDictionary(key).unread -= 1
                                        End If
                                        If messages.DialogsDictionary.ContainsKey(key) Then
                                            messages.DialogsDictionary(key).message.read_state = True

                                            messages.DialogsDictionary(key).unread = 0
                                        End If
                                    End If
                                    messages.BadMessagesDictionary.Remove(id)
                                End If

                                Dim key1 = If(chat_id > 0, "c" & chat_id, user_id.ToString())

                                If _
                                    messages.MessagesDictionary.ContainsKey(key1) AndAlso
                                    Not IsNothing(messages.MessagesDictionary(key1).items) Then
                                    For j = messages.MessagesDictionary(key1).items.Count - 1 To 0 Step - 1
                                        If j >= 0 Then
                                            If messages.MessagesDictionary(key1).items(j).id = id Then
                                                Dim temp = messages.MessagesDictionary(key1).items(j)
                                                temp.read_state = True
                                                Exit For
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        Case 4
                            'добавление нового сообщения

                            Dim msgNew As New types.message
                            Dim flags1 As messages.MessageFlags = CType(CInt(i(2)), messages.MessageFlags)

                            msgNew.chat_id =
                                If(flags1.HasFlag(messages.MessageFlags.n4), CInt(Mid(i(3).ToString, 2)), 0)
                            msgNew.user_id = If _
                                (msgNew.chat_id = 0,
                                 CInt(i(3)),
                                 If(i.Count > 7, CInt(i(7)("from")), OtherApi.MyUser.id))

                            If Speak.TypingDictionary.ContainsKey(msgNew.user_id.ToString) Then
                                Speak.TypingDictionary.Remove(msgNew.user_id.ToString)
                            End If

                            Dim key = msgNew.Key
                            If i.Count > 7 AndAlso IsNothing(i(7)) Then
                                msgNew = New types.message With {.id = CInt(i(1)),
                                    .body = WebUtility.HtmlDecode(i(6).ToString).Replace("<br>", vbNewLine),
                                    .user_id = msgNew.user_id,
                                    .date = OtherApi.GetUnixTime(),
                                    .emoji = True,
                                    .out = flags1.HasFlag(messages.MessageFlags.Outbox),
                                    .read_state = Not flags1.HasFlag(messages.MessageFlags.Unread)}
                            Else
                                If _
                                    i.Count > 7 AndAlso
                                    (Not IsNothing(i(7)("attach1_type")) Or Not IsNothing(i(7)("fwd"))) Then
                                    msgNew = (Await messages.GetById(i(1).ToString))(0)
                                    msgs.Add(msgNew)
                                    If msgNew IsNot Nothing AndAlso msgNew.attachments IsNot Nothing Then
                                        For Each a As types.attachment In msgNew.attachments
                                            If a.wall IsNot Nothing Then
                                                If a.wall.from_id < 0 Then
                                                    group_ids = group_ids & Math.Abs(a.wall.from_id) & ","
                                                Else
                                                    user_ids = user_ids & a.wall.from_id & ","
                                                End If
                                            End If
                                        Next
                                    End If
                                Else
                                    msgNew = New types.message With {.id = CInt(i(1)),
                                        .body = WebUtility.HtmlDecode(i(6).ToString).Replace("<br>", vbNewLine),
                                        .user_id = msgNew.user_id,
                                        .emoji = True,
                                        .chat_id = msgNew.chat_id,
                                        .date = OtherApi.GetUnixTime(),
                                        .out = flags1.HasFlag(messages.MessageFlags.Outbox),
                                        .read_state = Not flags1.HasFlag(messages.MessageFlags.Unread)}
                                End If
                            End If
                            Dim is_send = True
                            If _
                                messages.MessagesDictionary.ContainsKey(key) AndAlso
                                messages.MessagesDictionary(key).items IsNot Nothing Then
                                If _
                                    msgNew.out Or
                                    (Not IsNothing(OtherApi.MyUser) AndAlso msgNew.user_id = OtherApi.MyUser.id) Then
                                    For j = messages.MessagesDictionary(key).items.Count - 1 To _
                                        messages.MessagesDictionary(key).items.Count - 5 Step - 1
                                        If _
                                            j > 0 AndAlso
                                            (messages.MessagesDictionary(key).items(j).id = msgNew.id Or
                                             (messages.MessagesDictionary(key).items(j).id = 0 AndAlso
                                              Not IsNothing(messages.MessagesDictionary(key).items(j).body) AndAlso
                                              messages.MessagesDictionary(key).items(j).body.Trim = msgNew.body.Trim)) _
                                            Then
                                            If messages.MessagesDictionary(key).items.Count > 0 Then _
                                                messages.MessagesDictionary(key).items.RemoveAt(j)
                                            messages.MessagesDictionary(key).items.Add(msgNew)
                                            messages.MessagesDictionary(key).count += 1
                                            is_send = False
                                            Exit For
                                        End If
                                    Next
                                    If _
                                        messages.MessagesDictionary.ContainsKey(key) AndAlso
                                        messages.MessagesDictionary(key).items IsNot Nothing Then
                                        If is_send Then messages.MessagesDictionary(key).items.Add(msgNew)
                                    End If
                                Else
                                    messages.MessagesDictionary(key).items.Add(msgNew)
                                    messages.MessagesDictionary(key).count += 1
                                End If
                            End If
                            If Not IsNothing(OtherApi.PageMessageRun1) Then
                                OtherApi.PageMessageRun1.Text = msgNew.id.ToString()
                            End If
                            If _
                                messages.MessagesDictionary.ContainsKey(key) AndAlso
                                messages.MessagesDictionary(key).items IsNot Nothing Then
                                For j = messages.MessagesDictionary(key).items.Count - 1 To _
                                    messages.MessagesDictionary(key).items.Count - 5 Step - 1
                                    If j < messages.MessagesDictionary(key).items.Count AndAlso j > 0 Then
                                        If messages.MessagesDictionary(key).items(j).IsActive Then
                                            messages.MessagesDictionary(key).items.RemoveAt(j)
                                        End If
                                    End If
                                Next
                            End If
                            If Not messages.BadMessagesDictionary.ContainsKey(msgNew.id) And Not msgNew.read_state Then
                                messages.BadMessagesDictionary.Add(msgNew.id, msgNew)
                            End If
                            If _
                                Not msgNew.read_state AndAlso Not msgNew.out AndAlso
                                messages.MessagesDictionary.ContainsKey(key) Then
                                messages.MessagesDictionary(key).unread += 1
                            End If

                            If messages.DialogsDictionary.ContainsKey(key) Then
                                Dim diag = messages.DialogsDictionary(key)
                                Dim index = messages.DialogsList1.IndexOf(diag)
                                messages.DialogsDictionary(key).message.out = msgNew.out
                                If Not msgNew.out Then
                                    messages.DialogsDictionary(key).unread += 1
                                End If
                                Dim selectD As Object = nothing
                                If OtherApi.DialogListBox1 IsNot Nothing Then
                                    selectD = OtherApi.DialogListBox1.SelectedItem
                                End If

                                messages.DialogsList1.RemoveAt(index)
                                diag.message.attachments = msgNew?.attachments
                                diag.message.body = msgNew.body
                                diag.message.read_state = msgNew.read_state
                                diag.message.out = msgNew.out
                                messages.DialogsList1.Insert(0, diag)
                                'DialogsList1.Move(DialogsList1.IndexOf(diag), 0)
                                If OtherApi.DialogListBox1 IsNot Nothing AndAlso selectD IsNot Nothing Then
                                    OtherApi.DialogListBox1.SelectedItem = selectD
                                End If
                            Else
                                messages.DialogsList1.Clear()
                                messages.DialogsDictionary.Clear()
                                Await messages.GetDialogs
                            End If
                            If _
                                Not flags1.HasFlag(Messages.MessageFlags.Outbox) OrElse
                                (My.Settings.Notification41 AndAlso Not My.Settings.SendSuccessfullyNotify) Then
                                updates.Add(New types.Notifycation With {.user_id = msgNew.user_id,
                                               .text = WebUtility.HtmlDecode(i(6).ToString).Replace("<br>", vbNewLine),
                                               .type =
                                               If _
                                               (flags1.HasFlag(Messages.MessageFlags.Outbox),
                                                41,
                                                If(msgNew.chat_id > 0, 40, 4)),
                                               .Message = msgNew,
                                               .tag =
                                               If _
                                               (msgNew.chat_id > 0,
                                                "chat_id=" & msgNew.chat_id,
                                                "user_id=" & msgNew.user_id)})
                            End If
                            If Not lists.ProfilesDictionary.ContainsKey(msgNew.user_id) Then _
                                user_ids = user_ids & msgNew.user_id & ","
                        Case 6
                            'прочтение всех входящих сообщений с $peer_id вплоть до $local_id включительно 
                        Case 7
                            'прочтение всех исходящих сообщений с $peer_id вплоть до $local_id включительно 
                        Case 8
                            'друг $user_id стал онлайн, $extra не равен 0, если в mode был передан флаг 64, в младшем байте (остаток от деления на 256) числа $extra лежит идентификатор платформы (таблица ниже) 
                            Dim user_id = CInt(i(1))*- 1

                            updates.Add(New types.Notifycation With {.user_id = user_id,
                                           .type = 8,
                                           .text = "(" & messages.DeviceNames(CInt(i(2))) & ")"})

                            ChangeOnlineStatus(user_id.ToString(), True, CInt(i(2)) <> 7)
                            If Not lists.ProfilesDictionary.ContainsKey(user_id) Then _
                                user_ids = user_ids & user_id & ","
                        Case 9
                            'друг $user_id стал оффлайн ($flags равен 0, если пользователь покинул сайт (например, нажал выход) и 1, если оффлайн по таймауту (например, статус away)) 
                            Dim user_id = CInt(i(1))*- 1
                            updates.Add(New types.Notifycation With {.user_id = user_id,
                                           .type = 9,
                                           .text = "(" & messages.DeviceNames(CInt(i(2))) & ")"})
                            ChangeOnlineStatus(user_id.ToString(), False, False)
                            If Not lists.ProfilesDictionary.ContainsKey(CInt((i(1)))*- 1) Then _
                                user_ids = user_ids & user_id & ","
                        Case 51
                            'один из параметров (состав, тема) беседы $chat_id были изменены. $self - были ли изменения вызваны самим пользователем 
                        Case 61
                            'пользователь $user_id начал набирать текст в диалоге. событие должно приходить раз в ~5 секунд при постоянном наборе текста. $flags = 1 
                            Dim user_id = CInt(i(1))
                            updates.Add(New types.Notifycation With {.user_id = user_id,
                                           .type = 61})
                            If Not lists.ProfilesDictionary.ContainsKey(user_id) Then _
                                user_ids = user_ids & user_id.ToString & ","
                            If _
                                messages.MessagesDictionary.ContainsKey(user_id.ToString()) AndAlso
                                Not IsNothing(messages.MessagesDictionary(user_id.ToString).items) Then
                                For j = messages.MessagesDictionary(user_id.ToString).items.Count - 1 To _
                                    messages.MessagesDictionary(user_id.ToString).items.Count - 5 Step - 1
                                    If j < messages.MessagesDictionary(user_id.ToString).items.Count AndAlso j > 0 Then
                                        If messages.MessagesDictionary(user_id.ToString).items(j).IsActive Then
                                            messages.MessagesDictionary(user_id.ToString).items.RemoveAt(j)
                                        End If
                                    End If
                                Next
                                messages.MessagesDictionary(i(1).ToString()).items.Add(New types.message _
                                                                                          With {.out = False,
                                                                                          .user_id = CInt(i(1)),
                                                                                          .IsActive = True})
                                If Not IsNothing(messages.MessagesDictionary(i(1).ToString()).DialogTabItem) Then
                                    messages.MessagesDictionary(i(1).ToString()).DialogTabItem.Status = False
                                    messages.MessagesDictionary(i(1).ToString()).DialogTabItem.Status = True
                                End If
                            End If
                        Case 62
                            'пользователь $user_id начал набирать текст в беседе $chat_id.
                            updates.Add(New types.Notifycation With {.user_id = CInt(i(1)),
                                           .type = 62})

                            If Not lists.ProfilesDictionary.ContainsKey(CInt(i(1))) Then _
                                user_ids = user_ids & i(1).ToString & ","

                        Case 70
                            'пользователь $user_id совершил звонок имеющий идентификатор $call_id, дополнительную информацию о звонке можно получить используя метод voip.getCallInfo. 
                        Case 80
                            'новый счетчик непрочитанных в левом меню стал равен $count. 
                        Case Else
#If DEBUG Then
                            Throw New NotImplementedException
#End If
                    End Select
                Next
                If updates.Count > 0 Then
                    user_ids = user_ids.Trim(","c).Replace("-"c, "")
                    group_ids = group_ids.Trim(","c)
                    If user_ids.Length > 0 OrElse group_ids.Length > 0 Then
                        Dim r =
                                Await _
                                MicroVkApi.GetResponse (Of types.wall)("execute.GetUserAndGroup",
                                                                       {{"user_ids", user_ids}, {"group_ids", group_ids}})
                        If r IsNot Nothing Then

                            Dim profiles = r.profiles?.ToDictionary(Function(profile) profile.id)
                            Dim groups = r.groups?.ToDictionary(Function(group1) group1.id)

                            If r.profiles IsNot Nothing Then

                                For Each i In _
                                    From i1 In r.profiles Where Not lists.ProfilesDictionary.ContainsKey(i1.id)
                                    lists.ProfilesDictionary.Add(i.id, i)
                                Next
                            End If

                            If groups IsNot Nothing Then
                                For Each msg As types.message In msgs
                                    If msg IsNot Nothing AndAlso msg.attachments IsNot Nothing Then
                                        For Each a As types.attachment In msg.attachments
                                            If a.wall IsNot Nothing Then
                                                If a.wall.from_id < 0 Then
                                                    a.wall.SetFromName(
                                                        groups.GetSafeValue(Math.Abs(a.wall.from_id))?.name)
                                                Else
                                                    a.wall.from_name =
                                                        profiles.GetSafeValue(Math.Abs(a.wall.from_id))?.full_name
                                                End If
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                        End If
                    End If
                    OtherApi.ShowNotify(updates)
                End If
            End If
            ForceDownloadStringAsync()
        Catch ex As Exception
            TrayIconManager.ChangeNetworkStatus(True)
#If DEBUG Then
            If Not Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
                OtherApi.MsgMui(ex.Message)
            End If
#End If
        End Try
    End Sub

    Shared Sub ForceDownloadStringAsync()
        If Not _mWebClient.IsBusy Then
            _mWebClient.DownloadStringAsync(New Uri(_connectString))
        End If
    End Sub

    Shared Sub ChangeOnlineStatus(user_id As String, online As Boolean, online_mobile As Boolean)
        If _
            messages.DialogsDictionary.ContainsKey(user_id.ToString()) AndAlso
            messages.DialogsDictionary(user_id.ToString()).user IsNot Nothing Then
            messages.DialogsDictionary(user_id.ToString()).user.online = online
            messages.DialogsDictionary(user_id.ToString()).user.online_mobile = online_mobile
            messages.DialogsDictionary(user_id.ToString()).user = messages.DialogsDictionary(user_id.ToString()).user
        End If
    End Sub
End Class
