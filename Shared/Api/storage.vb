Imports Newtonsoft.Json.Linq

Namespace Api
    Class Storage
        Public Shared Async Function [Get](key As String) As Task(Of String)
            Try
                Return (Await MicroVkApi.GetResponse (Of JToken)("storage.get", {{"key", key}})).ToString()
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Shared Async Function [Set](key As String, value As String) As Task(Of Integer)

#If XP Then
            Return _
                Await _
                    MicroVKApi.GetResponse (Of Integer)("storage.set",
                                                        {{"key", key}, {"value", Uri.EscapeUriString(value)}})
#Else
            Return _
                Await _
                    MicroVkApi.GetResponse (Of Integer)("storage.set",
                                                        {{"key", key}, {"value", Net.WebUtility.UrlEncode(value)}})
#End If
        End Function
    End Class
End NameSpace