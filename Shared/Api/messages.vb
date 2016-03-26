Imports System.Collections.ObjectModel
Imports JetBrains.Annotations
Imports MicroVK.CSharp
Imports MicroVK.CSharp.Controls
Imports Newtonsoft.Json.Linq

Namespace Api
    ' ReSharper disable CheckNamespace
    Class Messages
        ' ReSharper restore CheckNamespace
        Public MCount As Integer
        Public Shared DCount As Integer
        Public Shared DialogCountRun1 As Run

        Public Shared DialogsList1 As New ObservableCollection(Of types.dialog)

        Public Shared DialogsDictionary As New Dictionary(Of String, types.dialog)

        Public Shared MessagesDictionary As New Dictionary(Of String, types.vk_list)

        Public Shared _
            DeviceNames As String() = {"", "Mobile", "iPhone", "iPad", "Android", "Windows Phone", "Windows 8", "Web"}

        Public Shared BadMessagesDictionary As New Dictionary(Of Integer, types.message)
        Public Shared LastMessagesDictionary As New Dictionary(Of Integer, types.message)

        Public Shared ControlSendMessagesDyctionary As New Dictionary(Of String, SendControl)

        <Flags()>
        Enum MessageFlags
            None = 0
            Unread = 1 'сообщение не прочитано 
            Outbox = 2 'исходящее сообщение 
            Replied = 4 'на сообщение был создан ответ 
            Important = 8 'помеченное сообщение 
            Chat = 16 'сообщение отправлено через чат 
            Friends = 32 'сообщение отправлено другом 
            Spam = 64 'сообщение помечено как "Спам" 
            Deleted = 128 'сообщение удалено (в корзине) 
            Fixed = 256 'сообщение проверено пользователем на спам 
            Media = 512 'сообщение содержит медиаконтент 
            N1 = 1024
            N2 = 2048
            N3 = 4096
            N4 = 8192
        End Enum

        Public Shared FirstDialogKey As String
        Public Shared FirstDialog As types.vk_list

        <CanBeNull>
        Shared Async Function [Get](Optional filters As String = "", Optional offset As String = "") _
            As Task(Of types.vk_list)
            Dim result =
                    Await _
                    MicroVkApi.GetResponse (Of types.vk_list)("messages.get",
                                                              {{"filters", filters}, {"offset", offset}})
            Await ProcessingMessages(result)
            Return result
        End Function

        Shared Async Function GetDialogs(Optional offset As String = "",
                                         Optional preview_length As String = "",
                                         Optional is_first_request As String = "") As Task(Of Boolean)

            Dim a =
                    Await _
                    MicroVkApi.GetResponse (Of JToken)("execute.getDialogsAndNameNew",
                                                       {{"offset", offset}, {"preview_length", preview_length},
                                                        {"IsFirstRequest", is_first_request}})
            If IsNothing(LongPollServerParser.LongPollInfo) Then LongPollServerParser.LongPollServerStart()
            If IsNothing(a) Then Return False

            If IsNothing(OtherApi.MyUser) Then
                OtherApi.MyUser =
                    (Await MyJsonConvert.DeserializeObjectAsync (Of types.profile())(a("myprofile").ToString))?.
                        FirstOrDefault()
                OtherApi.UpdateMyUser()
            End If
            Dim profiles = Await MyJsonConvert.DeserializeObjectAsync (Of types.profile())(a("profiles").ToString)
            If profiles.Length = 0 Then Return False
            Dim dialogs = Await MyJsonConvert.DeserializeObjectAsync (Of types.dialog())(a("dialogs")("items").ToString)
            DCount = CInt(a("dialogs")("count"))
            DialogCountRun1.Text = DCount.ToString()
            For Each p As types.profile In profiles
                Lists.ProfilesDictionary(p.id) = p
            Next

            Dim profiles1 = profiles.ToDictionary(Function(profile) profile.id)


            For Each d In dialogs
                Dim m = d.message
                If Not DialogsDictionary.ContainsKey(m.Key) Then
                    If m.chat_id = 0 Then d.user = profiles1.GetSafeValue(m.user_id)
                    DialogsList1.Add(d)
                    DialogsDictionary.Add(m.Key, d)
                End If
            Next

            FirstDialogKey = DialogsList1(0).message.Key
            FirstDialog = Await MyJsonConvert.DeserializeObjectAsync (Of types.vk_list)(a("first_dialog").ToString)
            Await ProcessingMessages(FirstDialog)
            Return True
        End Function

        Shared Async Function GetHistory(Optional params As String = "", Optional startMessageId As String = "") _
            As Task(Of types.vk_list)
            Dim result =
                    Await _
                    MicroVkApi.GetResponse (Of types.vk_list)("messages.getHistory",
                                                              {{"", params}, {"start_message_id", startMessageId}})
            Await ProcessingMessages(result)
            Return result
        End Function

        Private Shared Async Function ProcessingMessages(result As types.vk_list) As Task
            Dim attachments = New List(Of types.attachment)
            If result IsNot Nothing Then
                For Each item As types.message In result.items
                    If item.attachments IsNot Nothing Then
                        For Each a As types.attachment In item.attachments
                            If a.wall IsNot Nothing Then
                                attachments.Add(a)
                            End If
                        Next
                    End If
                Next
            End If

            If attachments.Any() Then
                Dim postsString =
                        attachments.Select(Function(wallPost) wallPost.wall.from_id & "_" & wallPost.wall.id).Aggregate(
                            Function(s, s1) s & "," & s1)
                Dim p = Await Wall.GetById(postsString)
                If p IsNot Nothing Then
                    Dim wallDictionary = p.ToDictionary(Function(wallPost) wallPost.id)
                    For Each wallPost As types.attachment In attachments
                        If wallDictionary.ContainsKey(wallPost.wall.id) Then
                            wallPost.wall = wallDictionary(wallPost.wall.id)
                        End If
                    Next
                End If
            End If
        End Function

        Shared Async Function GetById(message_ids As String) As Task(Of types.message())
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.message())("messages.getById@items",
                                                                {{"message_ids", message_ids}})
        End Function

        Shared Async Function Send(params As String,
                                   Optional message As String = "",
                                   Optional attachment As String = "",
                                   Optional forward_messages As String = "") As Task(Of Integer)
            Debug.Print(String.Format("params={0}&message={1}&attachment={2}&forward_messages={3}",
                                      params,
                                      message,
                                      attachment,
                                      forward_messages))
            Dim user_id, chat_id, message_id As Integer, mnew As types.message = Nothing
            user_id = params.GetParametr("user_id").ToInt()
            chat_id = params.GetParametr("chat_id").ToInt()
            Dim key = If(chat_id > 0, "c" & chat_id, user_id.ToString)
            If MessagesDictionary.ContainsKey(key) AndAlso MessagesDictionary(key).items IsNot Nothing Then
                For j = MessagesDictionary(key).items.Count - 1 To MessagesDictionary(key).items.Count - 5 Step - 1
                    If j < MessagesDictionary(key).items.Count AndAlso j > 0 Then
                        If MessagesDictionary(key).items(j).IsActive Then
                            MessagesDictionary(key).items.RemoveAt(j)
                        End If
                    End If
                Next
            End If
            If MessagesDictionary.ContainsKey(key) AndAlso MessagesDictionary(key).items IsNot Nothing Then
                mnew = New types.message With {.body = message,
                    .date = "no",
                    .Tag =
                        String.Format("params={0}&attachment={1}&forward_messages={2}",
                                      params,
                                      attachment,
                                      forward_messages),
                    .out = True,
                    .user_id = user_id,
                    .chat_id = chat_id,
                    .IsSend = True,
                    .IsSuccessfulSending = False}
                MessagesDictionary(key).items.Add(mnew)
            End If

#If XP Then
            message_id =
                Await _
                    MicroVKApi.GetResponse (Of Integer)("messages.send",
                                                        {{"", params}, {"message", Uri.EscapeUriString(message)},
                                                         {"attachment", attachment},
                                                         {"forward_messages", forward_messages}})
#Else
            message_id =
                Await _
                    MicroVkApi.GetResponse (Of Integer)("messages.send",
                                                        {{"", params}, {"message", Net.WebUtility.UrlEncode(message)},
                                                         {"attachment", attachment},
                                                         {"forward_messages", forward_messages}})
#End If
            If Not IsNothing(mnew) Then
                If message_id > 0 Then
                    mnew.id = message_id
                    mnew.IsSuccessfulSending = True
                Else
                    mnew.IsError = True
                    mnew.IsSend = False
                End If
            End If
            Return message_id
        End Function

        Shared Async Sub SendSticker(params As String, sticker_id As String)
            Await MicroVkApi.GetResponse (Of JToken)("messages.sendSticker", {{"", params}, {"sticker_id", sticker_id}})
        End Sub

        Async Sub Delete(messageIds As String)
            Await MicroVkApi.GetResponse (Of JToken)("messages.delete", {{"message_ids", messageIds}})
        End Sub

        Shared Async Function DeleteDialog(user_id As String, chat_id As String, count As Integer, offset As Integer) _
            As Task(Of Integer)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of Integer)("messages.deleteDialog",
                                                        {{"user_id", user_id}, {"chat_id", chat_id},
                                                         {"offset", offset.ToString()}, {"count", count.ToString()}})
        End Function

        Async Sub MarkAsNew(messageIds As String)
            Await MicroVkApi.GetResponse (Of JToken)("messages.markAsNew", {{"message_ids", messageIds}})
        End Sub

        Shared Async Sub MarkAsRead(params As String, startMessageId As String)
            Await _
                MicroVkApi.GetResponse (Of JToken)("messages.markAsRead",
                                                   {{"", params}, {"start_message_id", startMessageId}})
        End Sub

        <CanBeNull>
        Shared Async Function MarkAsImportant(message_ids As String, Optional important As Boolean = True) _
            As Task(Of Integer())
            return _
                Await _
                    MicroVkApi.GetResponse (Of Integer())("messages.markAsImportant",
                                                          {{"message_ids", message_ids},
                                                           {"important", Convert.ToInt32(important).ToString()}})
        End Function

        Shared Async Function GetLongPollServer() As Task(Of types.LongPollServer)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.LongPollServer)("messages.getLongPollServer", {{"need_pts", "1"}})
        End Function

        Shared Async Function GetChat(chat_id As String, Optional fields As String = "") As Task(Of types.chat)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.chat)("messages.getChat",
                                                           {{"chat_id", chat_id}, {"fields", fields}})
        End Function

        Shared Async Function GetChatAndProfile(chat_id As String, Optional fields As String = "") _
            As Task(Of types.chatAndProfile)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.chatAndProfile)("messages.getChat",
                                                                     {{"chat_id", chat_id}, {"fields", fields}})
        End Function

        Private Shared _timeActivity As Date

        Shared Async Sub SetActivity(user_id As String, chat_id As String)
            If DateDiff(DateInterval.Second, _timeActivity, Now) > 5 Then
                _timeActivity = Now
                Debug.Print(Now.ToString)
                Await _
                    MicroVkApi.GetResponse (Of JToken)("messages.setActivity",
                                                       {{"type", "typing"}, {"user_id", user_id}, {"chat_id", chat_id}})
            End If
        End Sub


        Private Shared _isSearchDialogs As Boolean
        Private Shared _tempSearchDialogs As types.SearchDialogsItem()

        Shared Async Function SearchDialogs(Optional q As String = "", Optional fields As String = "") _
            As Task(Of types.SearchDialogsItem())
            If Not _isSearchDialogs Then
                _isSearchDialogs = True
                _tempSearchDialogs =
                    Await _
                        MicroVkApi.GetResponse (Of types.SearchDialogsItem())("messages.searchDialogs",
                                                                              {{"q", q}, {"limit", "50"},
                                                                               {"fields", fields}})
                _isSearchDialogs = False
            End If
            Return _tempSearchDialogs
        End Function
    End Class
End Namespace