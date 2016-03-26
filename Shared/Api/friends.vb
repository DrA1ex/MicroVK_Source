
Imports System.Collections.ObjectModel

Namespace Api
    Class Friends
        Shared Async Function [Get](Optional userId As String = "",
                                    Optional offset As String = "",
                                    Optional count As String = "",
                                    Optional fields As String = "",
                                    Optional name_case As String = "") As Task(Of ObservableCollection(Of types.user))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.user))("friends.get@items",
                                                                                    {{"fields", fields},
                                                                                     {"order", "hints"},
                                                                                     {"user_id", userId},
                                                                                     {"offset", offset},
                                                                                     {"count", count},
                                                                                     {"name_case", name_case}})
        End Function

        Shared Async Function GetOnline(user_id As String, Optional offset As String = "") _
            As Task(Of ObservableCollection(Of types.user))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.user))("execute.FriendsOnline",
                                                                                    {{"user_id", user_id}, {"order", If _
                                                                                       (
                                                                                           user_id =
                                                                                           OtherApi.MyUser.id.ToString,
                                                                                           "hints",
                                                                                           "random")},
                                                                                     {"offset", offset}})
        End Function

        Shared Async Function GetMutual(user_id As String, Optional offset As String = "") _
            As Task(Of ObservableCollection(Of types.user))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.user))("execute.FriendGetMutual",
                                                                                    {{"user_id", user_id},
                                                                                     {"offset", offset}})
        End Function
    End Class
End Namespace