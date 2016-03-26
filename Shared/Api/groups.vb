

Namespace Api
    Class Groups
        Shared Async Function IsMember(group_id As String) As Task(Of Boolean)
            Return Await MicroVkApi.GetResponse (Of Boolean)("groups.isMember", {{"group_id", group_id}})
        End Function

        Async Function GetMembers(group_id As String) As Task(Of List(Of types.profile))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of List(Of types.profile))("groups.getMembers@items",
                                                                       {{"group_id", group_id}, {"fields", "photo_100"},
                                                                        {"sort", "time_desc"}})
        End Function

        Shared Async Function Join(group_id As String) As Task(Of Integer)
            Return Await MicroVkApi.GetResponse (Of Integer)("groups.join", {{"group_id", group_id}})
        End Function
    End Class
End Namespace