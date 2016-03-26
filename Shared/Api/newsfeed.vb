

Namespace Api
    Class Newsfeed
        Shared Async Function [Get](Optional parameters As String = "") As Task
            Dim b = Await MicroVkApi.GetResponse (Of lists.newsfeedlist)("newsfeed.get", {{"", parameters}})
            If IsNothing(b) Then Return
            lists.NewsfeedList1.next_from = b.next_from
            Dim profiles = b.profiles.ToDictionary(Function(profile) profile.id)
            Dim groups = b.groups.ToDictionary(Function(group) group.id)

            For Each p In b.items
                If p.copy_history IsNot Nothing AndAlso p.copy_history.Length > 0 Then
                    p.copy_history(0).IsCopyHistory = True
                    For Each wallPost As types.wall_post In p.copy_history
                        If wallPost.from_id > 0 Then
                            wallPost.from_name = profiles.GetSafeValue(wallPost.from_id)?.full_name
                        Else
                            wallPost.from_name = groups.GetSafeValue(Math.Abs(wallPost.from_id))?.name
                        End If
                    Next
                End If
                If p.source_id > 0 Then
                    p.from_name = profiles.GetSafeValue(p.source_id)?.full_name
                Else
                    p.from_name = groups.GetSafeValue(Math.Abs(p.source_id))?.name
                End If
                If p.friends IsNot Nothing Then
                    For Each u As types.vk_list_user_uid In p.friends.items
                        u.photo = profiles.GetSafeValue(u.user_id)?.photo_50

                    Next
                End If
            Next
            For Each i In b.items
                lists.NewsfeedList1.items.Add(i)
            Next
        End Function

        Shared Async Function GetLists() As Task(Of types.NewsList())
            Return Await MicroVkApi.GetResponse (Of types.NewsList())("newsfeed.getLists@items")
        End Function
    End Class
End NameSpace