Imports System.Collections.ObjectModel
Imports Newtonsoft.Json.Linq

Namespace Api
    Class Users
        Public Shared Async Function GetInfo(Optional id As String = "") As Task(Of types.GetUserInfo)
            Dim a = Await MicroVkApi.GetResponse (Of JObject)("execute.GetInfo", {{"user_id", id}})
            If IsNothing(a) Then Return Nothing
            Dim u = New types.GetUserInfo

            If a("user")(0)("personal") IsNot Nothing AndAlso a("user")(0)("personal").Type = JTokenType.Array Then
                a("user")(0)("personal") = Nothing
            End If

            u.user = Await MyJsonConvert.DeserializeObjectAsync (Of types.user_info)((a)("user")(0).ToString)
            u.lastActivity =
                Await MyJsonConvert.DeserializeObjectAsync (Of types.lastActivity)((a)("lastActivity").ToString())
            Return u
        End Function

        Public Shared Async Function [Get](Optional user_ids As String = "", Optional fields As String = "") _
            As Task(Of ObservableCollection(Of types.profile))
            Return _
                Await _
                    MicroVkApi.GetResponse (Of ObservableCollection(Of types.profile))("users.get",
                                                                                       {{"user_ids", user_ids},
                                                                                        {"fields", fields}})
        End Function

        'Public Async Function [get](Optional Fields As String = "") As Task(Of types.profile())
        '    Return Await JsonConvert.DeserializeObjectAsync(Of types.profile())((Await VkPost("users.get", "fields=" & Fields)).ToString)
        'End Function
    End Class
End NameSpace