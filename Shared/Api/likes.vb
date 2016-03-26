Namespace Api
    Class Likes
        Shared Async Function Add(type As String,
                                  item_id As String,
                                  Optional owner_id As String = "",
                                  Optional access_key As String = "") As Task(Of Integer)

            Return _
                Await _
                    MicroVkApi.GetResponse (Of Integer)("likes.add@likes",
                                                        {{"type", type}, {"item_id", item_id}, {"owner_id", owner_id},
                                                         {"access_key", access_key}})
        End Function

        Shared Async Function Delete(type As String,
                                     item_id As String,
                                     Optional owner_id As String = "",
                                     Optional access_key As String = "") As Task(Of Integer)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of Integer)("likes.delete@likes",
                                                        {{"type", type}, {"item_id", item_id}, {"owner_id", owner_id},
                                                         {"access_key", access_key}})
        End Function
    End Class
End NameSpace