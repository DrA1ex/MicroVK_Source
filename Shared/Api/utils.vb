

Namespace Api
    Class Utils
        Shared Async Function CheckLink(url As String) As Task(Of types.CheskUrl)
            Return _
                Await _
                    MicroVkApi.GetResponse (Of types.CheskUrl)("utils.checkLink",
                                                               {{"url", Net.WebUtility.HtmlEncode(url)}})
        End Function
    End Class
End NameSpace