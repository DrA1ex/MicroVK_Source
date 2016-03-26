Imports Newtonsoft.Json.Linq

Namespace Api
    Class Status
        Async Sub [Set](text As String)
            Await MicroVkApi.GetResponse (Of JToken)("status.set", {{"text", Net.WebUtility.HtmlEncode(text)}})
        End Sub

        Shared Async Sub SetDebugTime(text As String, group_text As String)
            Dim t =
                    Await _
                    MicroVkApi.GetResponse (Of JToken)("execute.DebugTimeSet",
                                                       {{"text", Net.WebUtility.HtmlEncode(text)},
                                                        {"group_text", group_text}})
        End Sub
    End Class
End NameSpace