Imports Newtonsoft.Json.Linq

Namespace Api
    Class Account
        Public Shared Async Sub SetOnline()
            Await MicroVkApi.GetResponse (Of JToken)("account.setOnline")
        End Sub

        Public Shared Async Sub SetOffline()
            Await MicroVkApi.GetResponse (Of JToken)("account.setOffline")
        End Sub
    End Class
End NameSpace