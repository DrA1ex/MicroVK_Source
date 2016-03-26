
Imports System.Collections.ObjectModel
Imports FirstFloor.ModernUI.Presentation
Imports Newtonsoft.Json.Linq

Namespace types
    ' ReSharper disable InconsistentNaming
    Public Class user
        Property id As Integer
        Property deactivated As String
        Property first_name As String
        Property last_name As String
        Property photo As String
        Property photo_100 As String
        Property sex As Integer

        Public ReadOnly Property full_name() As String
            Get
                Return first_name & " " & last_name
            End Get
        End Property
    End Class

    Class profile
        Property id As Integer
        Property first_name As String
        Property last_name As String
        Property photo As String
        Property photo_50 As String
        Property photo_100 As String
        Property deactivated As String
        Property sex As Integer
        Property online_mobile As Boolean
        Property online_app As String
        Property online As Boolean

        Public ReadOnly Property full_name() As String
            Get
                Return first_name & " " & last_name
            End Get
        End Property
    End Class

    Class group
        Property id As Integer
        Property name As String
        Property photo_50 As String
    End Class

    Class GetUserInfo
        Property user As user_info
        Property lastActivity As lastActivity
    End Class

    Class lastActivity
        Property online As Boolean
        Property time As String
    End Class

    Class user_info
        Inherits user
        Property photo_200_orig As String
        Property nickname As String
        Property bdate As String
        Property city As city
        Property home_town As String
        Property mobile_phone As String
        Property home_phone As String
        Property skype As String
        Property site As String
        Property university_name As String
        Property faculty_name As String
        Property education_form As String
        Property graduation As Integer
        Property universities As List(Of university)
        Property personal As personal
        Property activities As String
        Property interests As String
        Property music As String
        Property movies As String
        Property tv As String
        Property books As String
        Property games As String
        Property about As String
        Property quotes As String
        Property can_write_private_message As Boolean
        Property counters As counters
        Property online As Boolean
        Property online_mobile As Boolean
        Property online_app As String
    End Class

    Class VKError
        Property error_code As Integer
        Property error_msg As String
        Property request_params As request_param()
    End Class

    Public Class SetDockAttachments
        Inherits NotifyPropertyChanged
        Private _attachments As List(Of attachment)

        Property attachments As List(Of attachment)
            Get
                Return _attachments
            End Get
            Set(value As List(Of attachment))

                If Not IsNothing(value) AndAlso value.Count > 0 Then
                    Dim n = 0, audio_count = 0, attachment_count = 0
                    For Each i In value
                        If i.type <> "audio" And i.type <> "link" And i.type <> "poll" Then
                            attachment_count += 1
                        Else
                            audio_count += 1
                        End If
                    Next
                    If attachment_count = 0 Then GoTo label1
                    If attachment_count = 1 Then
                        If value(0).type = "photo" Then
                            Dim w = value(0).photo.width
                            Dim h = value(0).photo.height
                            Dim ratio As Double
                            If w < 400 And h < 300 Then
                                value(0).Width = w
                                value(0).Height = h
                            Else
                                If w > h Then
                                    ratio = 400/w
                                Else
                                    ratio = 300/h
                                End If
                                value(0).Width = ratio*w
                                value(0).Height = ratio*h
                            End If

                        ElseIf value(0).type = "sticker" Then
                            value(0).Width = 128
                            value(0).Height = 128
                        ElseIf value(0).type = "video" Then
                            value(0).Width = 400
                            value(0).Height = 300
                        ElseIf value(0).type = "doc" Then
                            value(0).Width = 400
                            value(0).Height = 50
                        ElseIf value(0).type = "wall" Then
                            value(0).Width = 400
                            value(0).Height = 50
                        End If
                    Else
                        Dim b = attachment_count
                        For Each i In value
                            i.Dock = OtherApi.AttachmentDock(b - 1)(n)
                            i.Width = OtherApi.AttachmentWidthAndHeight(b - 1)(n*2)
                            i.Height = OtherApi.AttachmentWidthAndHeight(b - 1)(n*2 + 1)
                            n += 1
                            If n >= b Then Exit For
                        Next
                    End If

                End If
                label1:
                _attachments = value
            End Set
        End Property

        Public ReadOnly Property TopAttachment As List(Of attachment)
            Get
                Return _attachments?.Where(Function(attachment)
                    Return _
                                              attachment.type <> "audio" AndAlso attachment.type <> "link" AndAlso
                                              attachment.type <> "link" AndAlso attachment.type <> "wall"
                                              End Function).ToList()
            End Get
        End Property

        Public ReadOnly Property BottomAttachment As List(Of attachment)
            Get
                Return _attachments?.Where(Function(attachment) _
                                              attachment.type = "audio" OrElse attachment.type = "poll" OrElse
                                              attachment.type = "link" OrElse attachment.type = "wall").ToList()
            End Get
        End Property
    End Class

    Public Class message
        Inherits SetDockAttachments
        Private _isSuccessfulSending1 As Boolean = True
        Private _readState As Boolean
        Private _isError1 As Boolean
        Private _isSend1 As Boolean

        Property id As Integer
            Get
                Return _id1
            End Get
            Set
                If LongPollServerParser.MaxMsgId < Value Then
                    LongPollServerParser.MaxMsgId = Value

#If DEBUG Then
                    Debug.Print(LongPollServerParser.MaxMsgId & " " & Value)
#End If
                End If
                _id1 = Value
            End Set
        End Property

        Property user_id As Integer
        Public Property body As String
        Property out As Boolean

        ReadOnly Property not_out As Boolean
            Get
                Return Not out
            End Get
        End Property

        Property read_state As Boolean
            Get
                Return _readState
            End Get
            Set
                _readState = Value
                OnPropertyChanged("read_state")
            End Set
        End Property

        Property chat_id As Integer
        Property title As String
        Property fwd_messages As List(Of message)
        Property emoji As Boolean

        Property IsSend As Boolean
            Get
                Return _isSend1
            End Get
            Set
                _isSend1 = Value
                OnPropertyChanged(NameOf(IsSend))
            End Set
        End Property

        Property IsActive As Boolean
        Property [date] As String
        Property action As String
        Property action_text As String
        Property action_email As String
        Property action_mid As String

        Property IsSuccessfulSending As Boolean
            Get
                Return _isSuccessfulSending1
            End Get
            Set
                _isSuccessfulSending1 = Value
                OnPropertyChanged(NameOf(IsSuccessfulSending))
            End Set
        End Property

        Property IsError As Boolean
            Get
                Return _isError1
            End Get
            Set
                _isError1 = Value
                OnPropertyChanged(NameOf(IsError))
            End Set
        End Property

        Property Tag As String
        Private _key As String
        Private _id1 As Integer
        Private _important1 As Boolean

        Property important As Boolean
            Get
                Return _important1
            End Get
            Set
                _important1 = Value
                OnPropertyChanged(NameOf(important))
            End Set
        End Property

        ReadOnly Property Key As String
            Get
                If _key.IsNullOrEmpty Then
                    _key = If(chat_id > 0, "c" & chat_id, user_id.ToString())
                End If
                Return _key
            End Get
        End Property
    End Class

    Public Class attachment
        Property type As String
        Property photo As photo
        Property video As video
        Property audio As audio
        Property link As linkvk
        Property sticker As sticker
        Property wall As wall_post
        Property doc As doc
        Property poll As poll
        Property Dock As Dock
        Property Width As Double
        Property Height As Double
    End Class

    Public Class wall_post
        Inherits SetDockAttachments
        Property photos As photo_list
        Property photo_tags As photo_list
        Property source_name As String
        Property IsMessage As Boolean
        Private _name1 As String
        Private _sourceId As Integer
        Private _postId As Integer
        Property type As String

        Property source_id As Integer
            Get
                Return _sourceId
            End Get
            Set
                _sourceId = Value
                from_id = Value
            End Set
        End Property

        Property friends As vk_list_user
        Property [date] As String
        Property copy_history As wall_post()
        Property copy_owner_id As String
        Property from_id As Integer

        Property post_id As Integer
            Get
                Return _postId
            End Get
            Set
                id = Value
                _postId = Value
            End Set
        End Property

        Property from_name As String

        Friend Sub SetFromName(fname As String)
            from_name = fname
            OnPropertyChanged(NameOf(from_name))
        End Sub

        Property id As Integer
        Property IsCopyHistory As Boolean
        Property likes As likes
        Property owner_id As Integer
        Property owner_name As String
        Property post_type As String
        Property text As String
        Property to_id As Integer

        Property name As String
            Get
                If IsCopyHistory AndAlso post_type = "photo" Then
                    Return owner_name
                Else
                    Return from_name
                End If
            End Get
            Set
                _name1 = Value
            End Set
        End Property
    End Class

    Public Class poll
        Property answer_id As Integer
        Property answers As answer()
        Property created As Integer
        Property id As Integer
        Property owner_id As Integer
        Property question As String
        Property votes As Integer
    End Class

    Public Class answer
        Property id As Integer
        Property text As String
        Property votes As Integer
        Property rate As Double
    End Class

    Public Class sticker
        Property id As Integer
        Property photo_64 As String
        Property photo_128 As String
        Property photo_256 As String
        Property width As Integer
        Property height As Integer
    End Class

    Public Class doc
        Property id As Integer
        Property owneer_id As Integer
        Property title As String
        Property size As Integer
        Property ext As String
        Property url As String
        Property photo_100 As String
        Property photo_130 As String
    End Class

    Public Class linkvk
        Property description As String
        Property photo As photo
        Property preview_page As String
        Property title As String
        Property url As String
    End Class

    Public Class photo
        Inherits AttachmentHelper
        Property id As Integer
        Property album_id As Integer
        Property owner_id As Integer
        Property photo_75 As String
        Property photo_130 As String
        Property photo_604 As String
        Property photo_807 As String
        Property photo_1280 As String
        Property photo_2560 As String
        Property width As Integer
        Property height As Integer
        Property text As String
        Property [date] As String
    End Class

    Public Class AttachmentHelper
        Public Function UpdatePhotoFromSize(avariableSize As Size) As String
            Dim url = ""
            If TypeOf Me Is photo Then
                Dim photo = TryCast(Me, photo)
                Dim size = OtherApi.PhotoSizes(My.Settings.photoSize)
                Dim maxSize = Math.Max(avariableSize.Width, avariableSize.Height)
                If maxSize <= 75 AndAlso Not photo.photo_75.IsNullOrEmpty() Then
                    url = photo.photo_75
                ElseIf maxSize <= 130 AndAlso Not photo.photo_130.IsNullOrEmpty() Then
                    url = photo.photo_130
                ElseIf maxSize <= 604 AndAlso Not photo.photo_604.IsNullOrEmpty() Then
                    url = photo.photo_604
                ElseIf maxSize <= 807 AndAlso Not photo.photo_807.IsNullOrEmpty() Then
                    url = photo.photo_807
                ElseIf maxSize <= 1280 AndAlso Not photo.photo_1280.IsNullOrEmpty() Then
                    url = photo.photo_1280
                ElseIf maxSize <= 2560 AndAlso Not photo.photo_2560.IsNullOrEmpty() Then
                    url = photo.photo_2560
                Else
                    url = GetPhotoMaxSize()
                End If
            End If
            Return url
        End Function

        Public Function GetPhotoMaxSize() As String
            Dim url = ""
            If TypeOf Me Is photo Then
                Dim photo = TryCast(Me, photo)
                If Not photo.photo_2560.IsNullOrEmpty() Then
                    url = photo.photo_2560
                ElseIf Not photo.photo_1280.IsNullOrEmpty() Then
                    url = photo.photo_1280
                ElseIf Not photo.photo_807.IsNullOrEmpty() Then
                    url = photo.photo_807
                ElseIf Not photo.photo_604.IsNullOrEmpty() Then
                    url = photo.photo_604
                ElseIf Not photo.photo_130.IsNullOrEmpty() Then
                    url = photo.photo_130
                ElseIf Not photo.photo_75.IsNullOrEmpty() Then
                    url = photo.photo_75
                End If
            End If
            Return url
        End Function
    End Class

    Public Class video
        Property id As Integer
        Property owner_id As Integer
        Property title As String
        Property description As String
        Property duration As Integer
        Property link As String
        Property photo_130 As String
        Property photo_320 As String
        Property photo_640 As String
        Property [date] As Integer
        Property views As Integer
        Property comments As Integer
        Property player As String
        Property access_key As String
    End Class

    Public Class audio
        Property id As Integer
        Property owner_id As Integer
        Property artist As String
        Property title As String

        ReadOnly Property full_title As String
            Get
                If Not is_radio Then
                    Return artist & " - " & title
                Else
                    Return title
                End If
            End Get
        End Property

        Property duration As Integer
        Property url As String
        Property lyrics_id As Integer
        Property album_id As Integer
        Property genre_id As Integer

        Property teg As String
        Property is_radio As Boolean
    End Class

    Public Class post_news
        Inherits SetDockAttachments
        Property type As String
        Property photos As photo_list
        Property source_name As String
        Property source_id As Integer
        Property [date] As Integer
        Property post_id As Integer

        Property photo_tags As photo_list
        Property text As String
        Property friends As vk_list_user
        Property likes As likes
        Property copy_history1 As copy_post
        Private _copy_history As copy_post()

        Property copy_history As copy_post()
            Get
                Return _copy_history
            End Get
            Set(value As types.copy_post())
                copy_history1 = value.FirstOrDefault
            End Set
        End Property

        Property comments As comment
    End Class

    Public Class comment
        Property count As Integer
        Property can_post As Boolean
    End Class

    Public Class vk_list_user
        Property count As Integer
        Property items As List(Of vk_list_user_uid)
    End Class

    Public Class vk_list_user_uid
        Property user_id As Integer
        Property photo As String
    End Class

    Public Class copy_post
        Inherits SetDockAttachments
        Property type As String
        Property source_id As Integer
        Property owner_id As Integer
        Property [date] As Integer
        Property post_id As Integer
        Property text As String
        Property owner_name As String
    End Class

    Public Class likes
        Inherits NotifyPropertyChanged
        Private _user_likes As Boolean
        Private _count1 As Integer

        Property count As Integer
            Get
                Return _count1
            End Get
            Set
                _count1 = Value
                OnPropertyChanged("count")
            End Set
        End Property

        Property user_likes As Boolean
            Get
                Return _user_likes
            End Get
            Set(value As Boolean)
                _user_likes = value
                OnPropertyChanged("user_likes")
            End Set
        End Property

        Property can_like As Boolean
        Property can_publish As Boolean
    End Class

    Public Class photo_list
        Property count As Integer
        Property items As List(Of photo)
    End Class

    Public Class app_list
        Property count As Integer
        Property items As List(Of app)
    End Class


    Class vk_list
        Property count As Integer
        Private _unread As Integer

        Public Property unread() As Integer
            Get
                Return _unread
            End Get
            Set(ByVal value As Integer)
                If Not IsNothing(DialogTabItem) Then
                    If value > 0 Then
                        DialogTabItem.Unread = "+" & value
                        DialogTabItem.Title = DialogTabItem.RawTitle & " (+" & value & ")"
                    Else
                        DialogTabItem.Unread = "0"
                        value = 0
                        DialogTabItem.Title = DialogTabItem.RawTitle
                    End If
                End If
                _unread = value
            End Set
        End Property

        Property items As ObservableCollection(Of types.message)
        Property DialogTabItem As MyTabItem
        Property ListBoxDialog As ListBox
    End Class

    Class LongPollServer
        Property key As String
        Property server As String
        Property ts As String
    End Class

    Class LongPoolServerUpdates
        Property ts As String
        Property failed As Integer
        Property pts As String
        Property updates As JArray
    End Class

    Class request_param
        Property key As String
        Property value As String
    End Class

    Class Notifycation
        Public Index As Integer
        Property Message As message
        Property type As Integer
        Property user_id As Integer
        Property chat_id As Integer
        Property title As String
        Property text As String
        Property tag As String
        Property [date] As String
        Property mytag As Object
    End Class

    Class ExecuteSearchGetHints
        Property hints As List(Of types.SearchHints)
        Property users As profile()
    End Class

    Class SearchHints
        Property type As String
        Property section As String
        Property description As String
        Property profile As profile
        Property group As group
    End Class

    Class city
        Property id As Integer
        Property title As String
    End Class

    Class university
        Property id As Integer
        Property name As String
        Property faculty_name As String
        Property chair_name As String
        Property graduation As Integer
    End Class

    Class personal
        Private ReadOnly _
            _masPolitical As String() =
                {"", My.Resources.personalPolitical1, My.Resources.personalPolitical2, My.Resources.personalPolitical3,
                 My.Resources.personalPolitical4, My.Resources.personalPolitical5, My.Resources.personalPolitical6,
                 My.Resources.personalPolitical7, My.Resources.personalPolitical8, My.Resources.personalPolitical9}

        Private ReadOnly _
            _masPeopleMain As String() =
                {"", My.Resources.personal1, My.Resources.personal2, My.Resources.personal3, My.Resources.personal4,
                 My.Resources.personal5, My.Resources.personal6}

        Private ReadOnly _
            _masLifeMain As String() =
                {"", My.Resources.personallife1, My.Resources.personallife2, My.Resources.personallife3,
                 My.Resources.personallife4, My.Resources.personallife5, My.Resources.personallife6,
                 My.Resources.personallife7, My.Resources.personallife8}

        Private ReadOnly _
            _masSmoking As String() =
                {"", My.Resources.smoking1, My.Resources.smoking2, My.Resources.smoking3, My.Resources.smoking4,
                 My.Resources.smoking5}

        Private _political As String

        Property political() As String
            Get
                Return _political
            End Get
            Set(ByVal value As String)
                _political = _masPolitical.GetSafeValue(CInt(value))
            End Set
        End Property

        Private _life_main As String

        Public Property life_main() As String
            Get
                Return _life_main
            End Get
            Set(ByVal value As String)
                _life_main = _masLifeMain.GetSafeValue(CInt(value))
            End Set
        End Property

        Private _people_main As String

        Public Property people_main() As String
            Get
                Return _people_main
            End Get
            Set(ByVal value As String)
                _people_main = _masPeopleMain.GetSafeValue(CInt(value))
            End Set
        End Property

        Private _smoking As String

        Public Property smoking() As String
            Get
                Return _smoking
            End Get
            Set(ByVal value As String)
                _smoking = _masSmoking.GetSafeValue(CInt(value))
            End Set
        End Property

        Private _alcohol As String

        Public Property alcohol() As String
            Get
                Return _alcohol
            End Get
            Set(ByVal value As String)
                _alcohol = _masSmoking.GetSafeValue(CInt(value))
            End Set
        End Property

        Property langs As String()
        Property religion As String
        Property inspired_by As String
    End Class

    Class counters
        Property albums As Integer
        Property videos As Integer
        Property audios As Integer
        Property photos As Integer
        Property notes As Integer
        Property friends As Integer
        Property groups As Integer
        Property online_friends As Integer
        Property mutual_friends As Integer
        Property user_videos As Integer
        Property followers As Integer
        Property user_photos As Integer
        Property subscriptions As Integer
    End Class

    Class UploadServer
        Property upload_url As String
        Property album_id As Integer
        Property user_id As Integer
    End Class

    Public Class ResponseUploadServer
        Property server As String
        Property photo As String
        Property hash As String
        Property user_id As String
        Property group_id As String
    End Class

    Class MicroVKDowload
        Property Url As String
        Property FileName As String
    End Class

    Class CheskUrl
        Property status As String
        Property link As String
    End Class

    Class chat
        Property id As Integer
        Property type As String
        Property title As String
        Property admin_id As String
        Property users As Integer()
    End Class

    Class chatAndProfile
        Property id As Integer
        Property type As String
        Property title As String
        Property admin_id As String
        Property users As profile()
    End Class

    Class MessagesSearchDialogsType
        Property id As Integer
        Property type As String
        Property title As String
        Property users As Integer()
        Property first_name As String
        Property last_name As String
        Property deactivated As String
    End Class

    Class NewsList
        Property id As Integer
        Property title As String
    End Class

    Class Product
        Property id As Integer
        Property type As String
        Property purchased As Boolean
        Property active As Boolean
        Property promoted As Boolean
        Property purchase_date As String
        Property title As String
        Property base_url As String
        Property stickers As Stickers
        Property ListBox1 As ListBox
    End Class

    Class Stickers
        Property base_url As String
        Property sticker_ids As Integer()
    End Class

    Public Class LanguageItem
        Property Culture As String
        Property Description As String
        Property AutoTranslate As Boolean
    End Class

    Class Post
        Inherits SetDockAttachments
        Property id As Integer
        Property owner_id As Integer
        Property from_id As Integer
        Property [date] As String
        Property text As String
        Property reply_owner_id As Integer
        Property reply_post_id As Integer
        Property friends_only As Boolean
        Property post_type As String
        Property likes As likes
    End Class

    Class SearchDialogsItem
        Property id As String
        Property first_name As String
        Property last_name As String

        Public ReadOnly Property full_name() As String
            Get
                Return first_name & " " & last_name
            End Get
        End Property

        Property title As String
        Property photo_50 As String
    End Class

    Class dialog
        Inherits NotifyPropertyChanged
        Private _unread As Integer
        Private _user1 As profile

        Property unread As Integer
            Get
                Return _unread
            End Get
            Set
                _unread = Value
                OnPropertyChanged(NameOf(unread))
                OnPropertyChanged(NameOf(unread_string))
            End Set
        End Property

        ReadOnly Property unread_string As String
            Get
                Return "+" & unread
            End Get
        End Property

        Property message As message

        Property user As profile
            Get
                Return _user1
            End Get
            Set
                _user1 = Value
                OnPropertyChanged(NameOf(user))
            End Set
        End Property
    End Class

    Class wall
        Property items As List(Of wall_post)
        Property profiles As List(Of profile)
        Property groups As group()
    End Class

    Class radio
        Inherits audio
        Private _name1 As String

        Property name As String
            Get
                Return _name1
            End Get
            Set
                _name1 = Value
                title = Value
            End Set
        End Property

        Property rating As Integer
    End Class

    Class radio_debug
        Property rating As Integer
        Property name As String
        Property url As String

        Property teg As String
    End Class

    Public Class app
        Property members_count As Integer
    End Class

    ' ReSharper restore InconsistentNaming
End Namespace