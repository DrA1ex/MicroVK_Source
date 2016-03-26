
Imports MahApps.Metro.Controls

Namespace DataTemplateSelectors
    Class NewsDataS
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim p = TryCast(item, types.wall_post)
            Dim element = TryCast(container, FrameworkElement)
            Select Case p.type
                Case "post"
                    Return CType(element.FindResource("WallPostTemplate"), DataTemplate)
                Case "photo"
                    Return CType(element.FindResource("PhotoTemplate"), DataTemplate)
                Case "photo_tag"
                    Return CType(element.FindResource("PhotoTagTemplate"), DataTemplate)
                Case "wall_photo"
                    Return CType(element.FindResource("WallPhotoTemplate"), DataTemplate)
                Case "friend"
                    Return CType(element.FindResource("FriendTemplate"), DataTemplate)
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class

    Class AttachmentDataS
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate

            Dim element = TryCast(container, FrameworkElement)
            Dim isView As Boolean = TypeOf TryCast(container, FrameworkElement).TemplatedParent Is FlipView
            If TypeOf item Is types.attachment Then
                Dim a = TryCast(item, types.attachment)
                If a Is Nothing Then
                    Return Nothing
                End If
                Select Case a.type
                    Case "photo"
                        Return _
                            CType(element.FindResource(if(isView, "PhotoViewAttachment", "PhotoAttachment")),
                                  DataTemplate)
                    Case "audio"
                        Return CType(element.FindResource("AudioAttachment"), DataTemplate)
                    Case "poll"
                        Return CType(element.FindResource("PollAttachment"), DataTemplate)
                    Case "doc"
                        Return CType(element.FindResource("DocAttachment"), DataTemplate)
                    Case "sticker"
                        Return CType(element.FindResource("StickerAttachment"), DataTemplate)
                    Case "link"
                        Return CType(element.FindResource("LinkAttachment"), DataTemplate)
                    Case "video"
                        Return CType(element.FindResource("VideoAttachment"), DataTemplate)
                    Case "wall"
                        If TypeOf container Is ContentPresenter Then
                            TryCast(container, ContentPresenter).Content = a.wall
                        Else
                            TryCast(container, TransitioningContentControl).Content = a.wall
                        End If
                        Return Nothing
                    Case Else
                        Return Nothing
                End Select
            ElseIf TypeOf item Is types.wall_post Then
                Dim w = TryCast(item, types.wall_post)
                If w.post_type = "post" Then
                    Return CType(element.FindResource("WallPostTemplate"), DataTemplate)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function
    End Class

    Class ContentVKAttachmentDataS
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim a = CType(item, types.Post)
            Dim element = TryCast(container, FrameworkElement)
            If a Is Nothing Then
                Return CType(element.FindResource("WaitTemplate"), DataTemplate)
            End If
            Select Case a.post_type
                Case "post"
                    Return CType(element.FindResource("WaitTemplate"), DataTemplate)
                Case Else
                    Return CType(element.FindResource("WaitTemplate"), DataTemplate)
            End Select
        End Function
    End Class

    Class SmileDT
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim p = CType(item, types.Product)
            Dim element = TryCast(container, FrameworkElement)
            Select Case p.type
                Case "MicroVKSmiles"
                    Return CType(element.FindResource("SmileTabItem"), DataTemplate)
                Case "RecentSmiles"
                    Return CType(element.FindResource("RecentSmileTabItem"), DataTemplate)
                Case Else
                    Return CType(element.FindResource("StickerListTemplate"), DataTemplate)
            End Select
        End Function
    End Class

    Class SmileItemDT
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim p = CType(item, types.Product)
            Dim element = TryCast(container, FrameworkElement)
            If p.type = "MicroVKSmiles" Then
                Return CType(element.FindResource("SmileTabItem1"), DataTemplate)
            ElseIf p.type = "RecentSmiles"
                Return CType(element.FindResource("RecentSmileTabItem1"), DataTemplate)
            Else
                Return CType(element.FindResource("StickerTabItem1"), DataTemplate)
            End If
        End Function
    End Class

    Class MessageDT
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim m = CType(item, types.message)
            Dim element = TryCast(container, FrameworkElement)
            Return DataTemplateHelper.GetTemplate(m, element, False)
        End Function
    End Class

    Class SmallMessageDT
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim m = CType(item, types.message)
            Dim element = TryCast(container, FrameworkElement)
            Return DataTemplateHelper.GetTemplate(m, element, True)
        End Function
    End Class

    Class DataTemplateHelper
        Public Shared Function GetTemplate(m As types.message,
                                           element As FrameworkElement,
                                           Optional isSmall As Boolean = False) As DataTemplate
            Select Case m.action
                Case "chat_create"
                    Return CType(element.FindResource("ChatCreateAction"), DataTemplate)
                Case Else
                    If Not m.IsActive Then
                        If isSmall Then
                            Return CType(element.FindResource("SmallMessageListTemplate1"), DataTemplate)
                        Else
                            Return CType(element.FindResource("SmallMessageListTemplate1"), DataTemplate)
                        End If
                    Else
                        Return CType(element.FindResource("MessageActivity1"), DataTemplate)
                    End If
            End Select
        End Function
    End Class

    Class SearhGlobalDT
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim m = CType(item, types.SearchHints)
            Dim element = TryCast(container, FrameworkElement)
            If m.type = "profile" Then
                Return CType(element.FindResource("ProfileSearchDataTemplate"), DataTemplate)
            Else
                Return CType(element.FindResource("GroupSearchDataTemplate"), DataTemplate)
            End If
        End Function
    End Class

    Class DialogDT
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim element = TryCast(container, FrameworkElement)
            If TypeOf item Is types.SearchDialogsItem Then
                Return CType(element.FindResource("DialogDataTemplate"), DataTemplate)
            ElseIf TypeOf item Is types.dialog
                Dim m = TryCast(item, types.dialog).message
                If m.chat_id = 0 Then
                    Return CType(element.FindResource("DialogListDataTemplate"), DataTemplate)
                Else
                    Return CType(element.FindResource("ChatListDataTemplate"), DataTemplate)
                End If
            Else
                Return Nothing
            End If
        End Function
    End Class

    Class WallTemplateSelector
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate

            Dim p = TryCast(item, types.wall_post)
            Dim element = TryCast(container, FrameworkElement)
            If p.post_type = "post" Then
                Return CType(element.FindResource("WallPostTemplate"), DataTemplate)
            Else
                Return Nothing
            End If
        End Function
    End Class

    Class WallCopyHistoryTemplateSelector
        Inherits DataTemplateSelector

        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) _
            As DataTemplate
            Dim p = TryCast(item, types.wall_post)
            Dim element = TryCast(container, FrameworkElement)
            If p Is Nothing Then
                Return Nothing
            End If
            Select Case p.post_type
                Case "post", "photo"
                    Return CType(element.FindResource("WallPostTemplate"), DataTemplate)
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class

    Class PhotoVieverContainerTemplateSelector
        Inherits StyleSelector

        Public Overloads Overrides Function SelectStyle(ByVal item As Object, ByVal container As DependencyObject) _
            As Style
            Return Nothing
        End Function
    End Class
End Namespace