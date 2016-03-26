


Namespace Api
    Class Wall
        Shared Async Function [Get](owner_id As String) As Task(Of types.wall)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.wall)("wall.get",
                                                           {{"count", "100"}, {"extended", "1"}, {"owner_id", owner_id}})
        End Function

        Shared Async Function GetById(posts As String) As Task(Of List(Of types.wall_post))
            Dim a = Await MicroVkApi.GetResponse (Of types.wall)("wall.getById", {{"posts", posts}, {"extended", "1"}})

            Dim profiles = a?.profiles?.ToDictionary(Function(profile) profile.id)
            Dim groups = a?.groups?.ToDictionary(Function(group) group.id)
            For Each wallPost As types.wall_post In a?.items
                If wallPost.from_id > 0 Then
                    wallPost.from_name = profiles?.GetSafeValue(wallPost.from_id)?.full_name
                Else
                    wallPost.from_name = groups?.GetSafeValue(- wallPost.from_id)?.name
                End If
            Next
            Return a?.items
        End Function

        Shared Async Function Post(message As String, attachments As String) As Task(Of WallPostResponse)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of WallPostResponse)("wall.post",
                                                                 {{"message", message}, {"attachments", attachments}})
        End Function

        Class WallPostResponse
            Property post_id As Integer
        End Class
    End Class
End NameSpace